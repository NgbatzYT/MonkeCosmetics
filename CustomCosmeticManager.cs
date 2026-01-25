using BepInEx;
using ExitGames.Client.Photon;
using MonkeCosmetics.Scripts;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MonkeCosmetics
{
    public class CustomCosmeticManager : MonoBehaviour
    {
        public static CustomCosmeticManager instance;

        public static List<Material> materials = [];

        private SkinnedMeshRenderer localMesh;

        public Material currentMaterial;

        public string[] specialVariables = { "_followplayercolor", "_followplayercolour" };
        public List<GameObject> Buttons = [];


        private int pageIndex = 0;
        private const int pageSize = 3;

        public Hashtable LocalCosmetics { get; private set; }

        void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(this);

            StartAF();
        }

        private void StartAF()
        {
            materials.AddRange(Plugin.Instance.bundle.LoadAllAssets<Material>());
            Plugin.Instance.bundle.Unload(false);

            foreach (var bundle in LoadAllBundles())
            {
                var mats = bundle.LoadAllAssets<Material>();
                materials.AddRange(mats);

                bundle.Unload(false);
            }


            localMesh = GameObject.Find("Player Objects")?.transform.Find("Local VRRig/Local Gorilla Player/gorilla_new").GetComponent<SkinnedMeshRenderer>();

            Buttons.AddRange([Plugin.Left, Plugin.Right, Plugin.E1, Plugin.Remove, Plugin.E2, Plugin.E3]);

            foreach (GameObject button in Buttons)
            {
                button.AddComponent<ButtonHandler>();
                button.layer = 18;
            }

            UpdateDisplays();
        }

        public string CheckText(string text)
        {
            string match = specialVariables.FirstOrDefault(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));

            return !string.IsNullOrEmpty(match) ? match : null;
        }

        private static List<AssetBundle> LoadAllBundles()
        {
            List<AssetBundle> bundles = [];

            foreach (string path in Directory.GetFiles(Paths.PluginPath, "*.MCmat", SearchOption.AllDirectories))
            {
                try
                {
                    bundles.Add(AssetBundle.LoadFromFile(path));
                    Debug.Log($"[MonkeCosmetics] Loaded bundle: {Path.GetFileName(path)}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[MonkeCosmetics] Failed loading {path}: {e}");
                }
            }

            instance.gameObject.AddComponent<LegacySupport.LegacySupport>();
            return bundles;
        }

        Material GetMaterial(int index)
        {
            if (index < 0 || index >= materials.Count)
                return null;

            return materials[index];
        }

        void UpdateDisplays()
        {
            int baseIndex = pageIndex * pageSize;

            SetDisplay(Plugin.H1, Plugin.E1, GetMaterial(baseIndex));
            SetDisplay(Plugin.H2, Plugin.E2, GetMaterial(baseIndex + 1));
            SetDisplay(Plugin.H3, Plugin.E3, GetMaterial(baseIndex + 2));



            UpdateState();
        }

        void SetDisplay(GameObject display, GameObject e, Material mat)
        {
            if (display == null || e == null) return;
            e.SetActive(mat != null);
            display.SetActive(mat != null);

            if (mat == null) return;
            var renderer = display.GetComponent<MeshRenderer>();
            renderer.material = mat ?? renderer.material;
        }

        void UpdateState()
        {
            int maxPage = Mathf.CeilToInt(materials.Count / (float)pageSize) - 1;

            Plugin.Left.SetActive(pageIndex > 0);
            Plugin.Right.SetActive(pageIndex < maxPage);
        }


        public void LeftArrow()
        {
            if (pageIndex > 0)
                pageIndex--;

            UpdateDisplays();
        }

        public void RightArrow()
        {
            int maxPage = Mathf.CeilToInt(materials.Count / (float)pageSize) - 1;

            if (pageIndex < maxPage)
                pageIndex++;

            UpdateDisplays();
        }

        public void SelectPress(int slot)
        {
            int materialIndex = pageIndex * pageSize + slot;
            Material mat = GetMaterial(materialIndex);

            if (mat == null)
                return;

            SetMaterial(mat);
            SetText(mat.name);
        }

        public void RemovePress()
        {
            currentMaterial = null;
            localMesh.material = VRRig.LocalRig.materialsToChangeTo[0];
        }


        public void SetMaterial(Material mat)
        {
            if (mat == null) return;

            Plugin.H4.GetComponent<Renderer>().material = mat;
            currentMaterial = mat;

            if (IsSpecial(mat.name))
            {
                var c = VRRig.LocalRig.playerColor;
                mat.color = new Color(c.r, c.g, c.b, mat.color.a);
            }

            if (!NetworkSystem.Instance.InRoom || !VRRig.LocalRig.IsTagged())
                localMesh.material = mat;

            if (NetworkSystem.Instance.InRoom)
            {
                LocalCosmetics = new Hashtable
                {
                    { "MonkeCosmetics::Material", mat.name }
                };

                PhotonNetwork.LocalPlayer.SetCustomProperties(LocalCosmetics);

                if (Plugin.Instance.network.Value) return;
                NetworkMaterial(mat);
            }
        }

        bool IsSpecial(string name)
        {
            return specialVariables.Any(s =>
                name.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        void NetworkMaterial(Material mat)
        {
            if (Plugin.Instance.network.Value) return;
            currentMaterial = mat;

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig.isLocal || rig.IsTagged()) continue;

                if (rig.Creator?.GetPlayerRef().CustomProperties["MonkeCosmetics::Material"] is not string matName)
                {
                    if (Plugin.Instance.materialSet.Value) CosmeticsNetworking.Instance.SetVRRigMaterial(mat, rig);
                    continue;
                }


                foreach (Material m in materials)
                {
                    if (m.name == matName)
                        CosmeticsNetworking.Instance.SetVRRigMaterial(m, rig);
                }
            }
        }

        void SetText(string text)
        {
            string upper = text.ToUpper();

            string match = specialVariables.FirstOrDefault(s =>
                upper.Contains(s, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(match))
                upper = upper.Replace(match, "", StringComparison.OrdinalIgnoreCase);

            // Plugin.MaterialName.text = upper;
        }

    }
    public static class Extensions
    {
        public static bool IsTagged(this VRRig rig) => rig.setMatIndex == 2 || rig.setMatIndex == 11 || rig.setMatIndex == 1;
    }
}
