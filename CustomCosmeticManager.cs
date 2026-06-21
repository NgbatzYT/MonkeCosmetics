using BepInEx;
using MonkeCosmetics.Editor.Cosmetic;
using MonkeCosmetics.Scripts;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace MonkeCosmetics
{
    public class CustomCosmeticManager : MonoBehaviour
    {
        public static CustomCosmeticManager instance;

        public static List<MonkeMaterial> materials = [];
        public static List<MonkeCosmetic> cosmetics = [];

        private SkinnedMeshRenderer localMesh;

        public MonkeMaterial currentMaterial;
        public static List<MonkeCosmetic> equippedCosmetics = [];

        public List<GameObject> Buttons = [];

        private int Index = 0;

        public Hashtable LocalCosmetics { get; private set; }

        void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(this);

            StartAF();
        }

        // returns false if duplicate
        private bool CheckDuplicate(MonkeMaterial material)
        {
            if (material.id == null) return false;

            if (materials.Contains(material)) return false;

            foreach (var mat in materials)
            {
                if (material.id == mat.id)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckDuplicate(MonkeCosmetic Cosmetic)
        {
            if (Cosmetic.id == null) return false;

            if (cosmetics.Contains(Cosmetic)) return false;

            foreach (var cos in cosmetics)
            {
                if (Cosmetic.id == cos.id)
                {
                    return false;
                }
            }

            return true;
        }

        private void StartAF()
        {
            foreach (var bundle in LoadAllBundles())
            {
                foreach (var material in bundle.LoadAllAssets<MonkeMaterial>())
                {
                    if (material != null && CheckDuplicate(material))
                        materials.Add(material);
                }

                bundle.Unload(false);
            }


            if(materials.Count == 0) gameObject.SetActive(false);

            localMesh = GameObject.Find("Player Objects")?.transform.Find("Local VRRig/Local Gorilla Player/gorilla_new").GetComponent<SkinnedMeshRenderer>();

            Buttons.AddRange([Plugin.Left, Plugin.Right, Plugin.Equip, Plugin.Cosmetic, Plugin.Material]);

            foreach (GameObject button in Buttons)
            {
                button.AddComponent<ButtonHandler>();
                button.layer = 18;
            }
            
            UpdateDisplay();
        }

        private static List<AssetBundle> LoadAllBundles()
        {
            List<AssetBundle> bundles = [];

            foreach (string path in Directory.GetFiles(Paths.PluginPath, "*.MCcosmetic", SearchOption.AllDirectories))
            {
                try
                {
                    var bundle = AssetBundle.LoadFromFile(path);
                    if(bundle != null)
                        bundles.Add(bundle);
                    else throw new NullReferenceException(path);


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

        void UpdateDisplay()
        {
            if (materials.Count == 0)
            {
                Debug.Log("No materials loaded!");
                return;
            }

            if (materials[Index] == currentMaterial)
            {
                Plugin.EquipText.text = "Remove";
            }
            else if (materials[Index] != currentMaterial)
            {
                Plugin.EquipText.text = "Equip";
            }

            Plugin.NameText.text = materials[Index].materialName;

            Plugin.Preview.GetComponent<MeshRenderer>().material = materials[Index].material;

            Plugin.Thumbnail.texture = materials[Index].Thumbnail;
        }

        public void LeftArrow()
        {
            if (Index > 0) Index--;
            else Index = materials.Count - 1; 
            UpdateDisplay();
        }

        public void RightArrow()
        {
            if (Index < materials.Count - 1) Index++;
            else Index = 0;
            UpdateDisplay();
        }

        public void SelectPress()
        {
            if (materials[Index] != currentMaterial)
            {
                SetMaterial(materials[Index]);
            }
            else
            {
                CosmeticsNetworking.Instance.ResetMaterial(VRRig.LocalRig);
            }

            UpdateDisplay();
        }


        public void SetMaterial(MonkeMaterial mat)
        {
            if (mat == null) return;

            currentMaterial = mat;

            if (mat.customColours)
            {
                var c = VRRig.LocalRig.playerColor;
                mat.material.color = new Color(c.r, c.g, c.b, mat.material.color.a);
            }

            if (!NetworkSystem.Instance.InRoom || !VRRig.LocalRig.IsTagged())
                localMesh.material = mat.material;
        }
    }
    public static class Extensions
    {
        public static bool IsTagged(this VRRig rig) => rig.setMatIndex == 2 || rig.setMatIndex == 11 || rig.setMatIndex == 1;
    }
}
