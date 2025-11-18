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

        public List<GameObject> Buttons = [];
        public static List<Material> materials = [];

        Hashtable LocalCosmetics;

        public Material currentMaterial;
        public string[] specialVariables = { "_followplayercolor", "_followplayercolour" };

        private SkinnedMeshRenderer localMesh;

        void Awake()
        {
            if (instance == null) instance = this; else Destroy(this);

            StartAF();
        }

        void StartAF()
        {
            materials.AddRange(Plugin.Instance.bundle.LoadAllAssets<Material>());

            var Bunds = LoadAllBundles();

            foreach (var mat in Bunds)
            {
                materials.AddRange(mat.LoadAllAssets<Material>());
            }

            Buttons.AddRange([Plugin.Left, Plugin.Right, Plugin.Select, Plugin.Remove]);

            foreach (GameObject button in Buttons)
            {
                button.AddComponent<ButtonHandler>();
                button.layer = 18;
            }

            localMesh = GameObject.Find("Player Objects")?.transform.Find("Local VRRig/Local Gorilla Player/gorilla_new").GetComponent<SkinnedMeshRenderer>();

            LeftArrow();
        }

        public static List<AssetBundle> LoadAllBundles()
        {
            List<AssetBundle> bundles = [];

            foreach (string bundlePath in Directory.GetFiles(Paths.PluginPath, "*.MCmat", SearchOption.AllDirectories))
            {
                try
                {
                    bundles.Add(AssetBundle.LoadFromFile(bundlePath));
                    Debug.Log($"[MonkeCosmetics] Loaded AssetBundle: {Path.GetFileName(bundlePath)}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[MonkeCosmetics] Failed to load bundle {bundlePath}: {ex}");
                }
            }

            return bundles;
        }

        public int index = 0;

        public void RemovePress()
        {
            if (NetworkSystem.Instance.InRoom)
            {
                foreach (VRRig p in GorillaParent.instance.vrrigs)
                {
                    CosmeticsNetworking.Instance.ResetMaterial(p);
                }
            }
            else
            {
                currentMaterial = null;
                localMesh.material = VRRig.LocalRig.materialsToChangeTo[0];
            }
        }

        private bool IsSpecial(string name)
        {
            return specialVariables.Any(s => string.Equals(s, CheckText(name), StringComparison.OrdinalIgnoreCase));
        }

        public void SetMaterial(Material mat)
        {
            if (IsSpecial(mat.name))
            {
                var c = VRRig.LocalRig.playerColor;
                mat.color = new Color(c.r, c.g, c.b, mat.color.a);
            }

            if(!NetworkSystem.Instance.InRoom || !VRRig.LocalRig.IsTagged()) localMesh.material = mat;

            if (NetworkSystem.Instance.InRoom)
                NetworkMaterial(mat);
            
        }

        void NetworkMaterial(Material mat)
        {
            LocalCosmetics = new Hashtable { { "MonkeCosmetics::Material", mat.name } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(LocalCosmetics);

            currentMaterial = mat;


            SetText(mat.name);

            if (NetworkSystem.Instance.InRoom)
            {
                foreach (VRRig e in GorillaParent.instance.vrrigs)
                {
                    if (e.isLocal || e.IsTagged()) { continue; }

                    if (e.Creator?.GetPlayerRef().CustomProperties["MonkeCosmetics::Material"] is not string matName)
                    {
                        if (!Plugin.Instance.materialSet.Value) continue;
                        Debug.Log($"[Monke Cosmetics] Setting material for non-monke cosmetics user {e.Creator?.NickName}");
                        CosmeticsNetworking.Instance.SetVRRigMaterial(currentMaterial, e);
                        continue;
                    }

                    foreach (var mate in materials)
                    {
                        if (mate.name == matName)
                        {
                            Debug.Log($"[Monke Cosmetics] Setting material for {e.Creator?.NickName}");
                            CosmeticsNetworking.Instance.SetVRRigMaterial(mate, e);
                            continue;
                        }
                    }
                }
            }
        }

        void SetText(string text)
        {
            var upperText = text.ToUpper();

            string match = specialVariables.FirstOrDefault(k => upperText.Contains(k, StringComparison.OrdinalIgnoreCase));
            if (!String.IsNullOrEmpty(match))
            {
                var e = upperText.Replace(match, "", StringComparison.OrdinalIgnoreCase);
                Plugin.MaterialName.text = e;
            }
            else
            {
                Plugin.MaterialName.text = upperText;
            }
        }

        public string CheckText(string text)
        {
            string[] specialVariables = { "_followplayercolour", "_followplayercolor" };

            string match = specialVariables.FirstOrDefault(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));
            if (!String.IsNullOrEmpty(match))
            {
                return match;
            }
            else
            {
                return null;
            }
        }

        public void LeftArrow()
        {
            if (index > 0)
                index -= 1;

            Plugin.Select.GetComponent<MeshRenderer>().material = materials[index];

            CheckButtonStatus();
            SetText(materials[index].name);
        }

        public void RightArrow()
        {
            if (index != materials.Count - 1)
                index += 1;

            Plugin.Select.GetComponent<MeshRenderer>().material = materials[index];

            CheckButtonStatus();

            SetText(materials[index].name);
        }
        public void SelectPress()
        {
            SetMaterial(materials[index]);

            Plugin.Select.GetComponent<MeshRenderer>().material = materials[index];

            SetText(materials[index].name);
        }

        void CheckButtonStatus()
        {
            Plugin.Left.SetActive(index > 0);

            Plugin.Right.SetActive(index < materials.Count - 1);

            Plugin.Left.GetComponent<MeshRenderer>().material = index > 0 ? materials[index - 1] : null;

            Plugin.Right.GetComponent<MeshRenderer>().material = index < materials.Count - 1 ? materials[index + 1] : null;
        }
    }
}