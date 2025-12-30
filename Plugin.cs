using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace MonkeCosmetics
{
    [BepInPlugin("ngbatz.monkecosmetics", "MonkeCosmetics", "1.0.2")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public AssetBundle bundle;
        //public static TextMeshPro MaterialName;

        public ManualLogSource manualLogSource;

        public static GameObject MonkeCosmetics { get; private set; }

        public static GameObject Left;
        public static GameObject Right;
        public static GameObject E1;
        public static GameObject E2;
        public static GameObject E3;
        public static GameObject Remove;
        public static GameObject H1;
        public static GameObject H2;
        public static GameObject H3;

        public ConfigEntry<bool> materialSet;

        void Start() 
        {
            Harmony.CreateAndPatchAll(GetType().Assembly, "ngbatz.monkecosmetics");
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
        } 

        void OnGameInitialized()
        {
            // idk
            materialSet = Config.Bind("General", "SetMaterialForOthers", false, "If set to true it will set your material to people without the mod otherwise it won't.");
            Instance = this;

            // Asset Loading 
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MonkeCosmetics.Assets.monkecosmetics");
            bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            MonkeCosmetics = Instantiate(bundle.LoadAsset<GameObject>("MonkeCosmetics"));

            // Positioning
            MonkeCosmetics.transform.position = new Vector3(-63.2257f, 12.4455f, -82.4489f); 
            MonkeCosmetics.transform.Rotate(0, 129.6149f, 0);

            // Grabbing Objects
            E1 = MonkeCosmetics.transform.Find("e1").gameObject;
            E2 = MonkeCosmetics.transform.Find("e2").gameObject;
            E3 = MonkeCosmetics.transform.Find("e3").gameObject;
            Left = MonkeCosmetics.transform.Find("left").gameObject;
            Right = MonkeCosmetics.transform.Find("right").gameObject;
            Remove = MonkeCosmetics.transform.Find("Remove").gameObject;
            H1 = MonkeCosmetics.transform.Find("head1").gameObject;
            H2 = MonkeCosmetics.transform.Find("head2").gameObject;
            H3 = MonkeCosmetics.transform.Find("head3").gameObject;
            MonkeCosmetics.transform.Find("Material").gameObject.SetActive(false);
            MonkeCosmetics.transform.Find("Hats").gameObject.SetActive(false);

            // Adding Components
            MonkeCosmetics.AddComponent<CustomCosmeticManager>();
            MonkeCosmetics.AddComponent<CosmeticsNetworking>();
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