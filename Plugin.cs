using BepInEx;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MonkeCosmetics
{
    [BepInPlugin("com.gtshady.gorillatag.monkecosmetics", "MonkeCosmetics", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public AssetBundle bundle;
        public static GameObject MatSwitcherStand { get; private set; }

        void Start() => GorillaTagger.OnPlayerSpawned(OnGameInitialized);

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

            MatSwitcherStand.AddComponent<CustomCosmeticManager>();
        }
    }
}