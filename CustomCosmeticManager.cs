using BepInEx;
using ExitGames.Client.Photon;
using HarmonyLib;
using MonkeCosmetics.Scripts;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MonkeCosmetics
{
    public class CustomCosmeticManager : MonoBehaviour
    {
        public static CustomCosmeticManager instance;

        public List<GameObject> Buttons = new List<GameObject>();
        public List<Material> materials = new List<Material>();
        string localplayermeshdir = "Local VRRig/Local Gorilla Player/gorilla_new";
        public Material DFPrisim = null;
        public Material DFmoon = null;
        public Material DFTrans = null;
        Hashtable LocalCosmetics;

        public Material currentMaterial;


        void Awake()
        {
            if (instance == null) instance = this; else Destroy(this);


            StartAF();
        }

        public static List<AssetBundle> LoadAllBundles()
        {
            string[] bundlePaths = Directory.GetFiles(Paths.PluginPath, "*.MCmat", SearchOption.AllDirectories);

            List<AssetBundle> bundles = new List<AssetBundle>();

            foreach (string bundlePath in bundlePaths) {
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

        public void StartAF()
        {
            foreach(var material in Plugin.Instance.bundle.LoadAllAssets<Material>()) 
            {
                materials.Add(material);
            }


            var Bunds = LoadAllBundles();

            foreach (var mat in Bunds)
            {
                Material[] Matss = mat.LoadAllAssets<Material>();
                foreach (var f in Matss)
                {
                    materials.Add(f);
                }
            }

            Buttons.Add(Plugin.Left);
            Buttons.Add(Plugin.Right);
            Buttons.Add(Plugin.Select);

            foreach (GameObject button in Buttons)
            {
                button.GetComponent<SphereCollider>().isTrigger = true;
                button.layer = 18;
            }

            LeftArrow();
        }

        public void setMat(Material mat)
        {
            if (PhotonNetwork.InRoom)
            {
                LocalCosmetics = new Hashtable
                {
                    { "MonkeCosmetics::Material", mat.name }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(LocalCosmetics);



                if (!NetworkSystem.Instance.InRoom) { GameObject.Find("Player Objects").transform.Find(localplayermeshdir).GetComponent<SkinnedMeshRenderer>().material = mat; }
                else if (!VRRig.LocalRig.IsTagged()) { GameObject.Find("Player Objects").transform.Find(localplayermeshdir).GetComponent<SkinnedMeshRenderer>().material = mat; }
                currentMaterial = mat;

                SetText(mat.name);

                foreach (NetPlayer p in NetworkSystem.Instance.AllNetPlayers) 
                {
                    var e = GorillaGameManager.instance.FindPlayerVRRig(p);

                    if(e.isLocal) { continue; }
                    if(e.IsTagged()) { continue; }

                    string matName = (string)p.GetPlayerRef().CustomProperties["MonkeCosmetics::Material"];

                    if (matName == null) 
                    {
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

        void SetText(string text)
        {
            string[] specialVariables = { "_playermatdefault", "_followplayercolor", "_playermat" };

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
            var l = Plugin.Left.GetComponent<MeshRenderer>();
            var r = Plugin.Right.GetComponent<MeshRenderer>();

            if (index - 1 != -1)
            {
                Plugin.Left.SetActive(true);
                l.material = materials[index - 1];
            }
            else
                Plugin.Left.SetActive(false);


            if (index + 1 != materials.Count)
            {
                Plugin.Right.SetActive(true);
                r.material = materials[index + 1];
            }
            else
                Plugin.Right.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(VRRig), nameof(VRRig.ChangeMaterialLocal))]
    public class TagCheck
    {
        private static void PostFix(VRRig __instance)
        {
            if (!__instance.IsTagged())
                CustomCosmeticManager.instance.setMat(CustomCosmeticManager.instance.currentMaterial);
                
        }
    }
}
