using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MonkeCosmetics.Scripts;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace MonkeCosmetics
{
    [BepInPlugin("ngbatz.monkecosmetics", "MonkeCosmetics", "1.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public AssetBundle bundle;
        public static TextMeshPro MaterialName;


        public ManualLogSource manualLogSource;

        public static GameObject MonkeCosmetics { get; private set; }

        public static GameObject Left;
        public static GameObject Right;
        public static GameObject Select;
        public static GameObject Remove;

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
            MonkeCosmetics.transform.position = new Vector3(-68.4556f, 13.4509f, -81.399f);
            MonkeCosmetics.transform.Rotate(0, 10.75f, 0);

            // Grabbing Objects
            Select = MonkeCosmetics.transform.Find("Select").gameObject;
            Left = MonkeCosmetics.transform.Find("Left").gameObject;
            Right = MonkeCosmetics.transform.Find("Right").gameObject;
            Remove = MonkeCosmetics.transform.Find("Remove").gameObject;
            MaterialName = MonkeCosmetics.transform.Find("MaterialName").GetComponent<TextMeshPro>();

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