using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MonkeCosmetics.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace MonkeCosmetics
{
    [BepInPlugin("ngbatz.monkecosmetics", "Monke Cosmetics", "2.201")] // gd reference edition v2
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public AssetBundle bundle;

        public ManualLogSource manualLogSource;

        public static GameObject MonkeCosmetics { get; private set; }
        public static List<GameObject> Buttons = [];
        public static List<GameObject> PageButtons = [];
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

        public static GameObject Update;
        public static GameObject UpdateButton;
        public static TMP_Text UpdateText;

        public static RawImage Thumbnail;

        public const int Version = 2;

        void Start()
        {
            Harmony.CreateAndPatchAll(GetType().Assembly, "ngbatz.monkecosmetics");
            manualLogSource = base.Logger;
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
            MonkeCosmetics.transform.position = new Vector3(-66.8756f, 11.9f, -84.4489f);
            MonkeCosmetics.transform.rotation = Quaternion.Euler(new Vector3(0, 260, 0));

            // Grabbing Objects
            Equip = MonkeCosmetics.transform.Find("Screen/SelectButton").gameObject;
            EquipText = Equip.transform.Find("Text").gameObject.GetComponent<TMP_Text>();
            NameText = MonkeCosmetics.transform.Find("Screen/Name").GetComponent<TMP_Text>();
            DescriptionText = MonkeCosmetics.transform.Find("Screen/Description").GetComponent<TMP_Text>();
            Left = MonkeCosmetics.transform.Find("Screen/ButtonLeft").gameObject;
            Right = MonkeCosmetics.transform.Find("Screen/ButtonRight").gameObject;
            Preview = MonkeCosmetics.transform.Find("Stand/Preview/Material").gameObject;
            Update = MonkeCosmetics.transform.Find("Screen/Update").gameObject;
            UpdateButton = Update.transform.Find("UpdateButton").gameObject;
            UpdateText = MonkeCosmetics.transform.Find("Screen/Update/Text (TMP)").GetComponent<TMP_Text>();
            Thumbnail = MonkeCosmetics.transform.Find("Screen/Image").GetComponent<RawImage>();
            PageMain = MonkeCosmetics.transform.Find("Screen/PageButtonMain").GetComponent<RawImage>();
            PageLeft = MonkeCosmetics.transform.Find("Screen/PageButtonLeft").GetComponent<RawImage>();
            PageRight = MonkeCosmetics.transform.Find("Screen/PageButtonRight").GetComponent<RawImage>();

            PageMain.gameObject.AddComponent<PageButtonHandler>();
            PageLeft.gameObject.AddComponent<PageButtonHandler>();
            PageRight.gameObject.AddComponent<PageButtonHandler>();
            UpdateButton.gameObject.AddComponent<PageButtonHandler>();

            PageMain.gameObject.GetComponent<Collider>().isTrigger = true;
            PageLeft.gameObject.GetComponent<Collider>().isTrigger = true;
            PageRight.gameObject.GetComponent<Collider>().isTrigger = true;
            UpdateButton.gameObject.GetComponent<Collider>().isTrigger = true;

            PageMain.gameObject.layer = 18;
            PageLeft.gameObject.layer = 18;
            PageRight.gameObject.layer = 18;
            UpdateButton.gameObject.layer = 18;

            Buttons.AddRange([Left, Right, Equip]);
            PageButtons.AddRange([PageLeft.gameObject, PageRight.gameObject, PageMain.gameObject]);

            foreach (GameObject button in Buttons)
            {
                button.AddComponent<ButtonHandler>();
                button.layer = 18;
            }

            // Adding Components
            try
            {
                StartCoroutine(GetVersion());
                MonkeCosmetics.AddComponent<CustomCosmeticManager>();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }

            bundle.Unload(false);
        }

        public static void HideUpdate()
        {
            Update.SetActive(false);

            foreach (GameObject button in Buttons)
            {
                button.GetComponent<Collider>().enabled = true;
            }

            foreach (GameObject button in PageButtons)
            {
                button.GetComponent<Collider>().enabled = true;
            }
        }

        public static void ShowUpdate()
        {
            Update.SetActive(true);

            foreach (GameObject button in Buttons)
            {
                button.GetComponent<Collider>().enabled = false;
            }

            foreach (GameObject button in PageButtons)
            {
                button.GetComponent<Collider>().enabled = false;
            }
        }

        IEnumerator GetVersion()
        {
            using UnityWebRequest req = UnityWebRequest.Get("https://raw.githubusercontent.com/NgbatzYT/MonkeCosmetics/refs/heads/main/version");
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to check for updates!");
                yield break;
            }

            if (int.Parse(req.downloadHandler.text.Trim()) > Version)
            {
                ShowUpdate();
            }
        }
    }

    public class Debug
    {
        public static void Log(string msg)
        {
            Plugin.Instance.manualLogSource.LogInfo(msg);
        }
        public static void LogWarning(string msg)
        {
            Plugin.Instance.manualLogSource.LogWarning(msg);
        }
        public static void LogError(string msg)
        {
            Plugin.Instance.manualLogSource.LogError(msg);
        }
    }
}