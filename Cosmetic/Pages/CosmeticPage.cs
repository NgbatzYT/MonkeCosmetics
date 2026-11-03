using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MonkeCosmetics.Cosmetic.Pages
{
    public class CosmeticPage : MonkeCosmeticPage
    {
        public static CosmeticPage instance;
        public static bool initialised;

        public List<MonkeCosmetic> currentCosmetics = [];
        public static List<MonkeCosmetic> cosmetics = [];
        private int Index = 0;

        public override void OnMonkeCosmeticsIntialised()
        {
            base.OnMonkeCosmeticsIntialised();

            instance = this;
            initialised = true;

            Icon = new Texture2D(1, 1);
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MonkeCosmetics.Assets.cosmetic.png");
            using var ms = new MemoryStream();
            stream.CopyTo(ms);


            Icon.LoadImage(ms.ToArray());

            foreach (var bundle in LoadAllBundles())
            {
                foreach (var cosmetic in bundle.LoadAllAssets<MonkeCosmetic>())
                {
                    if (cosmetic != null && CheckDuplicate(cosmetic))
                    {
                        cosmetics.Add(cosmetic);
                        cosmetic.obj = Instantiate(cosmetic.obj);
                    }
                }

                bundle.Unload(false);
            }


            foreach (var cosmetic in cosmetics)
            {
                cosmetic.obj.SetActive(false);

                switch (cosmetic.pos)
                {
                    case CosmeticPosition.body:
                        cosmetic.obj.transform.SetParent(GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body_pivot/").transform, false);
                        cosmetic.obj.transform.localPosition = Vector3.zero;
                        cosmetic.obj.transform.localRotation = Quaternion.identity;
                        continue;

                    case CosmeticPosition.head:
                        cosmetic.obj.transform.SetParent(GorillaTagger.Instance.offlineVRRig.bodyTransform, false);
                        cosmetic.obj.transform.localPosition = Vector3.zero;
                        cosmetic.obj.transform.localRotation = Quaternion.identity;
                        continue;

                    case CosmeticPosition.hand:
                        cosmetic.obj.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false);
                        cosmetic.obj.transform.localPosition = Vector3.zero;
                        cosmetic.obj.transform.localRotation = Quaternion.identity;
                        continue;
                }
            }
        }

        public override void OnPageEntered()
        {
            base.OnPageEntered();

            DisplayUpdate();
        }

        public override void OnRightPress()
        {
            base.OnRightPress();

            if (Index < cosmetics.Count - 1) Index++;
            else Index = 0;

            DisplayUpdate();
        }

        public override void OnLeftPress()
        {
            base.OnLeftPress();

            if (Index > 0) Index--;
            else Index = cosmetics.Count - 1;

            DisplayUpdate();
        }

        public override void OnEquipPress()
        {
            base.OnEquipPress();
            if (!currentCosmetics.Contains(cosmetics[Index]))
            {
                SetCosmetic(cosmetics[Index]);
            }
            else
            {
                RemoveCosmetic(cosmetics[Index]);
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
            if (cosmetics.Count == 0)
            {
                Debug.Log("No cosmetics loaded!");
                Plugin.NameText.text = "No Cosmetics";
                Plugin.DescriptionText.text = "No cosmetics are loaded.";
                return;
            }

            if (currentCosmetics.Contains(cosmetics[Index]))
            {
                Plugin.EquipText.text = "Remove";
            }
            else if (!currentCosmetics.Contains(cosmetics[Index]))
            {
                Plugin.EquipText.text = "Equip";
            }

            Plugin.NameText.text = cosmetics[Index].cosmeticName;
            Plugin.DescriptionText.text = "";

            Plugin.Thumbnail.texture = cosmetics[Index].Thumbnail;
        }

        public void RemoveCosmetic(MonkeCosmetic cos)
        {
            if (cos == null || !currentCosmetics.Contains(cos)) return;

            cos.obj.SetActive(false);
            currentCosmetics.Remove(cos);
        }

        public void SetCosmetic(MonkeCosmetic cos)
        {
            if (cos == null || currentCosmetics.Contains(cos)) return;

            foreach (var e in currentCosmetics)
            {
                if(e.pos == cos.pos)
                {
                    RemoveCosmetic(e);
                }
            }

            cos.obj.SetActive(true);
            currentCosmetics.Add(cos);
        }

        private bool CheckDuplicate(MonkeCosmetic cosmetic)
        {
            if (cosmetic.id == null) return false;

            if (cosmetics.Contains(cosmetic)) return false;

            foreach (var cos in cosmetics)
            {
                if (cosmetic.id == cos.id)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
