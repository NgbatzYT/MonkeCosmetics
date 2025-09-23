using BepInEx;
using BepInEx.Configuration;
using GorillaExtensions;
using GorillaNetworking;
using HarmonyLib;
using MonkeCosmetics.Scripts;
using Photon.Pun;
using System;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace MonkeCosmetics
{
    [BepInPlugin("ngbatz.monkecosmetics", "MonkeCosmetics", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public AssetBundle bundle;
        public static TextMeshPro MaterialName;
        public static GameObject MonkeCosmetics { get; private set; }

        public static GameObject Left;
        public static GameObject Right;
        public static GameObject Select;
        public static GameObject Remove;

        public ConfigEntry<bool> materialSet;

        void Start() => GorillaTagger.OnPlayerSpawned(OnGameInitialized);
#if DEBUG

        private Rect windowRect = new Rect(20, 20, 220, 200);

        void OnGUI()
        {
            windowRect = GUI.Window(1, windowRect, MakeWindow, "Monke Cosmetics Debug GUI");
        }

        private void MakeWindow(int id)
        {
            GUILayout.Space(10);

            if (GUILayout.Button("Next"))
            {
                CustomCosmeticManager.instance.RightArrow();
            }

            if (GUILayout.Button("Previous"))
            {
                CustomCosmeticManager.instance.LeftArrow();
            }

            if (GUILayout.Button("Select"))
            {
                CustomCosmeticManager.instance.SelectPress();
            }

            if (GUILayout.Button("Remove Material"))
            {
                CustomCosmeticManager.instance.RemovePress();
            }

            GUI.DragWindow(new Rect(0, 0, windowRect.width, 20));
        }
#endif


        void OnGameInitialized()
        {
            materialSet = Config.Bind("General", "SetMaterialForOthers", true, "If set to true it will set your material to people without the mod otherwise it won't." );

            Instance = this;
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MonkeCosmetics.Assets.monkecosmetics");
            bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();

            MonkeCosmetics = Instantiate(bundle.LoadAsset<GameObject>("MonkeCosmetics"));
            MonkeCosmetics.transform.position = new Vector3(-68.4556f, 11.4509f, -81.399f);
            MonkeCosmetics.transform.Rotate(0, 10.75f, 0);

            Select = MonkeCosmetics.transform.Find("Select").gameObject;
            Left = MonkeCosmetics.transform.Find("Left").gameObject;
            Right = MonkeCosmetics.transform.Find("Right").gameObject;
            Remove = MonkeCosmetics.transform.Find("Remove").gameObject;

            MaterialName = MonkeCosmetics.transform.Find("MaterialName").GetComponent<TextMeshPro>();

            MonkeCosmetics.AddComponent<CustomCosmeticManager>();
            MonkeCosmetics.AddComponent<CosmeticsNetworking>();
        }
    }

}
