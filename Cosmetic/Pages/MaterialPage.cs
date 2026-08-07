using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MonkeCosmetics.Cosmetic.Pages
{
    public class MaterialPage : MonkeCosmeticPage
    {
        public static MaterialPage instance;
        public static bool initialised;

        public MonkeMaterial currentMaterial;
        public static List<MonkeMaterial> materials = [];
        private int Index = 0;
        private SkinnedMeshRenderer localMesh;

        public override void OnMonkeCosmeticsIntialised()
        {
            base.OnMonkeCosmeticsIntialised();

            instance = this;
            initialised = true;

            Icon = new Texture2D(1,1);
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MonkeCosmetics.Assets.material.png");
            using var ms = new MemoryStream();
            stream.CopyTo(ms);


            Icon.LoadImage(ms.ToArray());

            foreach (var bundle in LoadAllBundles())
            {
                foreach (var material in bundle.LoadAllAssets<MonkeMaterial>())
                {
                    if (material != null && CheckDuplicate(material))
                        materials.Add(material);
                }

                bundle.Unload(false);
            }

            CustomCosmeticManager.instance.gameObject.AddComponent<LegacySupport.LegacySupport>();

            localMesh = GameObject.Find("Player Objects")?.transform.Find("Local VRRig/Local Gorilla Player/gorilla_new").GetComponent<SkinnedMeshRenderer>();
        }

        public override void OnPageEntered()
        {
            base.OnPageEntered();

            DisplayUpdate();
        }

        public override void OnRightPress()
        {
            base.OnRightPress();

            if (Index < materials.Count - 1) Index++;
            else Index = 0;

            DisplayUpdate();
        }

        public override void OnLeftPress()
        {
            base.OnLeftPress();

            if (Index > 0) Index--;
            else Index = materials.Count - 1;

            DisplayUpdate();
        }

        public override void OnEquipPress()
        {
            base.OnEquipPress();
            if (materials[Index] != currentMaterial)
            {
                SetMaterial(materials[Index]);
            }
            else
            {
                ResetMaterial(VRRig.LocalRig);
            }

            DisplayUpdate();
        }

        public override void OnPageUpdate()
        {
            base.OnPageUpdate();

            DisplayUpdate();
        }
        private static List<AssetBundle> LoadAllBundles()
        {
            List<AssetBundle> bundles = [];

            foreach (string path in Directory.GetFiles(Paths.PluginPath, "*.MCcosmetic", SearchOption.AllDirectories))
            {
                try
                {
                    var bundle = AssetBundle.LoadFromFile(path);
                    if (bundle != null)
                        bundles.Add(bundle);
                    else throw new NullReferenceException(path);


                    Debug.Log($"[MonkeCosmetics] Loaded bundle: {Path.GetFileName(path)}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[MonkeCosmetics] Failed loading {path}: {e}");
                }
            }

            
            return bundles;
        }

        public void DisplayUpdate()
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
            Plugin.DescriptionText.text = "";

            Plugin.Preview.GetComponent<MeshRenderer>().material = materials[Index].material;

            Plugin.Thumbnail.texture = materials[Index].Thumbnail;
        }

        public void ResetMaterial(VRRig Rig)
        {
            if (Rig == null) return;
            Debug.Log("[Monke Cosmetics] Started to reset material");
            if (Rig.isLocal)
            {
                currentMaterial = null;

                GameObject.Find("Player Objects").transform.Find("Local VRRig/Local Gorilla Player/gorilla_new").GetComponent<SkinnedMeshRenderer>().material = Rig.materialsToChangeTo[Rig.setMatIndex];
                Debug.Log($"[Monke Cosmetics] Succesfully reset material");
            }
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
    }
}
