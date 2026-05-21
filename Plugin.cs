using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MonkeCosmetics.Editor.Cosmetic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MonkeCosmetics
{
    [BepInPlugin("ngbatz.monkecosmetics", "Monke Cosmetics Beta", "2.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public AssetBundle bundle;
        //public static TextMeshPro MaterialName;

        public ManualLogSource manualLogSource;

        public static GameObject MonkeCosmetics { get; private set; }

        public static GameObject Left;
        public static GameObject Right;
        public static GameObject Equip;
        public static TMP_Text EquipText;
        public static TMP_Text NameText;
        public static GameObject Preview;
        public static GameObject Material;
        public static GameObject Cosmetic;
        public static RawImage Thumbnail;


        public ConfigEntry<bool> materialSet;
        public ConfigEntry<bool> network;

        ControllerInputPoller c;

        void Start() 
        {
            Harmony.CreateAndPatchAll(GetType().Assembly, "ngbatz.monkecosmetics");
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
        } 

        void OnGameInitialized()
        {
            // idk
            materialSet = Config.Bind("General", "SetMaterialForOthers", false, "If set to true it will set your material to people without the mod otherwise it won't.");
            network = Config.Bind("General", "DisableNetworking", true, "If set to true it will disable all networking.");
            Instance = this;

            // Asset Loading 
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MonkeCosmetics.Assets.monkecosmetics");
            bundle = AssetBundle.LoadFromStream(stream);
            stream?.Close();
            MonkeCosmetics = Instantiate(bundle.LoadAsset<GameObject>("MonkeCosmetics"));

            // Positioning
            MonkeCosmetics.transform.position = new Vector3(-63.0757f, 12.4455f, -82.4489f);
            MonkeCosmetics.transform.rotation = Quaternion.Euler(new Vector3(0, 100, 0));

            // Grabbing Objects
            Equip = MonkeCosmetics.transform.Find("Screen/Button").gameObject;
            EquipText = MonkeCosmetics.transform.Find("Screen/Button/Text").gameObject.GetComponent<TMP_Text>();
            NameText = MonkeCosmetics.transform.Find("Screen/Name").gameObject.GetComponent<TMP_Text>();
            Left = MonkeCosmetics.transform.Find("Screen/left").gameObject;
            Right = MonkeCosmetics.transform.Find("Screen/right").gameObject;
            Preview = MonkeCosmetics.transform.Find("Stand/PreviewAnchor").gameObject;
            Material = MonkeCosmetics.transform.Find("Screen/materials").gameObject;
            Cosmetic = MonkeCosmetics.transform.Find("Screen/hats").gameObject;
            Thumbnail = MonkeCosmetics.transform.Find("Screen/RawImage").gameObject.GetComponent<RawImage>();

            Cosmetic.SetActive(false);
            Material.SetActive(false);
            // Adding Components
            MonkeCosmetics.AddComponent<CustomCosmeticManager>();
            MonkeCosmetics.AddComponent<CosmeticsNetworking>();

            bundle.Unload(false);
        }

        private bool funBool;

        private void Update()
        {
            if (Instance == null) return;

            if (c == null) { c = ControllerInputPoller.instance; return; } 

            bool pressed = c.leftControllerPrimaryButton && c.rightControllerPrimaryButton;

            switch (pressed)
            {
                case true when !funBool:
                {
                    MonkeMaterial mat = CustomCosmeticManager.instance.currentMaterial;
                    if (mat != null)
                        CustomCosmeticManager.instance.SetMaterial(mat);

                    funBool = true;
                    break;
                }
                case false when funBool:
                    funBool = false;
                    break;
            }
        }
    }

    public class Debug
    {
        public static void Log(string msg)
        {
#if DEBUG
            Plugin.Instance.manualLogSource.Log(msg);
#endif
        }
        public static void LogWarning(string msg)
        {
#if DEBUG
            Plugin.Instance.manualLogSource.LogWarning(msg);
#endif
        }
        public static void LogError(string msg)
        {
#if DEBUG
            Plugin.Instance.manualLogSource.LogError(msg);
#endif
        }
    }
}