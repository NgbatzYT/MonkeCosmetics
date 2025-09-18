using BepInEx;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MonkeCosmetics
{
    [BepInPlugin("com.gtshady.gorillatag.monkecosmetics", "MonkeCosmetics", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private static AssetBundle _cosmeticBundle;
        public static void Embedded_LoadBundle()
        {
            if (_cosmeticBundle != null) return;

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = "MonkeCosmetics.Assets.defaultassets_mc";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    Debug.LogError($"Failed to find embedded resource: {resourceName}");
                    return;
                }

                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                _cosmeticBundle = AssetBundle.LoadFromMemory(buffer);
                if (_cosmeticBundle == null)
                {
                    Debug.LogError("Failed to load AsstBundle from memory.");
                }
            }
        }

        public static GameObject LoadGameObj(string objName)
        {
            if (_cosmeticBundle == null) Embedded_LoadBundle();
            return _cosmeticBundle.LoadAsset<GameObject>(objName);
        }
        public static Material LoadMat(string matName)
        {
            if (_cosmeticBundle == null) Embedded_LoadBundle();
            return _cosmeticBundle.LoadAsset<Material>(matName);
        }
        void Start()
        {
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
        }

        void OnGameInitialized()
        {
            var net = new GameObject("MatNet");
            net.AddComponent<CosmeticsNetworking>();

            var e = Instantiate(LoadGameObj("MatSwitcherStand"));
            e.transform.position = new Vector3(-68.44f, 11.4509f, -81.399f);
            e.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            e.transform.Rotate(0, -80, 0);
            Destroy(e.transform.Find("Cylinder").gameObject);

            var A = e.AddComponent<CustomCosmeticManager>();

            A.DFmoon = LoadMat("MoonSkin");
            A.DFPrisim = LoadMat("PrisimSkin");
            A.DFTrans = LoadMat("TransPrisim");
            A.StartAF();
        }
    }
}