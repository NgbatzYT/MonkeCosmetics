using BepInEx;
using ExitGames.Client.Photon;
using HarmonyLib;
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
        string localplayermeshdir = "Local VRRig/Local Gorilla Player/gorilla_new";
        Hashtable LocalCosmetics;

        public Material currentMaterial;
        public string[] specialVariables = { "_followplayercolor", "_followplayercolour" };

        void Awake()
        {
            if (instance == null) instance = this; else Destroy(this);

            StartAF();
        }

        void StartAF()
        {
            foreach (var material in Plugin.Instance.bundle.LoadAllAssets<Material>())
            {
                materials.Add(material);
            }

            var Bunds = LoadAllBundles();

            foreach (var mat in Bunds)
            {
                Material[] Matss = mat.LoadAllAssets<Material>();
                foreach (var f in Matss)
                {
                    f.enableInstancing = true;
                    materials.Add(f);
                }
            }

            Buttons.Add(Plugin.Left);
            Buttons.Add(Plugin.Right);
            Buttons.Add(Plugin.Select);
            Buttons.Add(Plugin.Remove);

            foreach (GameObject button in Buttons)
            {
                button.AddComponent<ButtonHandler>();
                button.layer = 18;
            }

            LeftArrow();
        }

        public static List<AssetBundle> LoadAllBundles()
        {
            string[] bundlePaths = Directory.GetFiles(Paths.PluginPath, "*.MCmat", SearchOption.AllDirectories);

            List<AssetBundle> bundles = [];

            foreach (string bundlePath in bundlePaths)
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
                foreach (NetPlayer p in NetworkSystem.Instance.AllNetPlayers)
                {
                    VRRig Rig = GorillaGameManager.instance.FindPlayerVRRig(p);
                    CosmeticsNetworking.Instance.ResetMaterial(Rig);
                }
            }
            else
            {
                currentMaterial = null;
                GameObject.Find("Player Objects").transform.Find("Local VRRig/Local Gorilla Player/gorilla_new").GetComponent<SkinnedMeshRenderer>().material = VRRig.LocalRig.materialsToChangeTo[0];
            }
        }
        public void setMat(Material mat)
        {
            if (!NetworkSystem.Instance.InRoom) 
            {
                if (specialVariables.Any(s => string.Equals(s, CheckText(mat.name), StringComparison.OrdinalIgnoreCase))) { mat.color = new Color(VRRig.LocalRig.playerColor.r, VRRig.LocalRig.playerColor.g, VRRig.LocalRig.playerColor.b, mat.color.a); }
                GameObject.Find("Player Objects").transform.Find(localplayermeshdir).GetComponent<SkinnedMeshRenderer>().material = mat;
            }
            else if (!VRRig.LocalRig.IsTagged()) 
            {
                if (specialVariables.Any(s => string.Equals(s, CheckText(mat.name), StringComparison.OrdinalIgnoreCase))) { mat.color = new Color(VRRig.LocalRig.playerColor.r, VRRig.LocalRig.playerColor.g, VRRig.LocalRig.playerColor.b, mat.color.a); }
                GameObject.Find("Player Objects").transform.Find(localplayermeshdir).GetComponent<SkinnedMeshRenderer>().material = mat;
            }

            if (NetworkSystem.Instance.InRoom)
            {
                LocalCosmetics = new Hashtable
                {
                    { "MonkeCosmetics::Material", mat.name }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(LocalCosmetics);

                currentMaterial = mat;

                
                SetText(mat.name);
                
                if (NetworkSystem.Instance.InRoom) 
                {
                    foreach (NetPlayer p in NetworkSystem.Instance.AllNetPlayers)
                    {
                        var e = GorillaGameManager.instance.FindPlayerVRRig(p);

                        if (e.isLocal) { continue; }
                        if (e.IsTagged()) { continue; }

                        string matName = (string)p.GetPlayerRef().CustomProperties["MonkeCosmetics::Material"];

                        if (matName == null)
                        {
                            if (!Plugin.Instance.materialSet.Value) continue;
                            Debug.Log($"[Monke Cosmetics] Setting material for non-monke cosmetics user {p.NickName}");
                            CosmeticsNetworking.Instance.SetVRRigMaterial(currentMaterial, e);
                        }
                        else
                        {

                            foreach (var mate in materials)
                            {
                                if (mate.name == matName)
                                {
                                    Debug.Log($"[Monke Cosmetics] Setting material for {p.NickName}");
                                    CosmeticsNetworking.Instance.SetVRRigMaterial(mate, e);
                                    continue;
                                }
                            }
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
            string[] specialVariables = { "_playermatdefault", "_followplayercolor", "_playermat" };

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
            setMat(materials[index]);

            Plugin.Select.GetComponent<MeshRenderer>().material = materials[index];

            SetText(materials[index].name);
        }

        void CheckButtonStatus() 
        {
            Plugin.Left.SetActive(index > 0);

            Plugin.Right.SetActive(index < materials.Count - 1);

            Plugin.Left.GetComponent<MeshRenderer>().material = index > 0 ? materials[index - 1] : null;

            Plugin.Right.GetComponent<MeshRenderer>().material =  index < materials.Count - 1 ? materials[index + 1] : null;
        }
    }

    [HarmonyPatch(typeof(VRRig), nameof(VRRig.ChangeMaterialLocal))]
    public static class TagCheck
    {
        [HarmonyPostfix]
        private static void Postfix(VRRig __instance)
        {
            if (!__instance.IsTagged())
                CustomCosmeticManager.instance.setMat(CustomCosmeticManager.instance.currentMaterial);
        }
    }
}
