using BepInEx;
using GorillaExtensions;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using TMPro;

namespace MonkeCosmetics
{
    [BepInPlugin("com.gtshady.gorillatag.monkecosmetics", "MonkeCosmetics", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public AssetBundle bundle;
        public static TextMeshPro MaterialName;
        public static GameObject MatSwitcherStand { get; private set; }

        void Start() => GorillaTagger.OnPlayerSpawned(OnGameInitialized);
#if DEBUG

        private Rect windowRect = new Rect(20, 20, 220, 200);

        void OnGUI()
        {
            windowRect = GUI.Window(0, windowRect, MakeWindow, "Monke Cosmetics Debug GUI");
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

            GUI.DragWindow(new Rect(0, 0, windowRect.width, 20));
        }
#endif


        void OnGameInitialized()
        {
            Instance = this;

            var net = new GameObject("MatNet");
            net.AddComponent<CosmeticsNetworking>();

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MonkeCosmetics.Assets.defaultassets_mc");
            bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            MatSwitcherStand = Instantiate(bundle.LoadAsset<GameObject>("MatSwitcherStand"));
            MatSwitcherStand.transform.position = new Vector3(-68.44f, 11.4509f, -81.399f);
            MatSwitcherStand.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            MatSwitcherStand.transform.Rotate(0, -80, 0);
            Destroy(MatSwitcherStand.transform.Find("Cylinder").gameObject);

            MaterialName = MatSwitcherStand.transform.Find("MatName").GetComponent<TextMeshPro>();

            MatSwitcherStand.AddComponent<CustomCosmeticManager>();
        }
    }
}