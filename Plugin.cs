using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MonkeCosmetics.Scripts;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MonkeCosmetics
{
    [BepInPlugin("ngbatz.monkecosmetics", "Monke Cosmetics", "2.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public AssetBundle bundle;

        public ManualLogSource manualLogSource;

        public static GameObject MonkeCosmetics { get; private set; }

        public static GameObject Left;
        public static RawImage PageLeft;
        public static GameObject Right;
        public static RawImage PageRight;
        public static GameObject Equip;
        public static RawImage PageMain;
        public static TMP_Text EquipText;
        public static TMP_Text NameText;
        public static TMP_Text DescriptionText;
        public static GameObject Preview;

        public static RawImage Thumbnail;

        void Start()
        {
            Harmony.CreateAndPatchAll(GetType().Assembly, "ngbatz.monkecosmetics");
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
        }

        void OnGameInitialized()
        {
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
            Equip = MonkeCosmetics.transform.Find("Screen/SelectButton").gameObject;
            EquipText = Equip.transform.Find("Text").gameObject.GetComponent<TMP_Text>();
            NameText = MonkeCosmetics.transform.Find("Screen/Name").gameObject.GetComponent<TMP_Text>();
            DescriptionText = MonkeCosmetics.transform.Find("Screen/Description").gameObject.GetComponent<TMP_Text>();
            Left = MonkeCosmetics.transform.Find("Screen/ButtonLeft").gameObject;
            Right = MonkeCosmetics.transform.Find("Screen/ButtonRight").gameObject;
            Preview = MonkeCosmetics.transform.Find("Stand/Preview/Material").gameObject;
            Thumbnail = MonkeCosmetics.transform.Find("Screen/Image").gameObject.GetComponent<RawImage>();
            PageMain = MonkeCosmetics.transform.Find("Screen/PageButtonMain").gameObject.GetComponent<RawImage>();
            PageLeft = MonkeCosmetics.transform.Find("Screen/PageButtonLeft").gameObject.GetComponent<RawImage>();
            PageRight = MonkeCosmetics.transform.Find("Screen/PageButtonRight").gameObject.GetComponent<RawImage>();

            PageMain.gameObject.AddComponent<PageButtonHandler>();
            PageLeft.gameObject.AddComponent<PageButtonHandler>();
            PageRight.gameObject.AddComponent<PageButtonHandler>();

            PageMain.gameObject.GetComponent<Collider>().isTrigger = true;
            PageLeft.gameObject.GetComponent<Collider>().isTrigger = true;
            PageRight.gameObject.GetComponent<Collider>().isTrigger = true;

            PageMain.gameObject.layer = 18;
            PageLeft.gameObject.layer = 18;
            PageRight.gameObject.layer = 18;

            // Adding Components
            MonkeCosmetics.AddComponent<CustomCosmeticManager>();

            bundle.Unload(false);
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