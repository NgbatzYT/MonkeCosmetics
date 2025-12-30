using BepInEx;
using GorillaCosmetics.Data;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MonkeCosmetics.LegacySupport
{
    public class LegacySupport : MonoBehaviour
    {

        public void Start()
        {

            foreach (string path in Directory.GetFiles(Paths.PluginPath, "*", SearchOption.AllDirectories))
            {
                
                if (new string[] { ".material", ".gmat", ".gmatplus" }.Contains(Path.GetExtension(path)))
                {
                    if (File.Exists(path) && File.ReadAllBytes(path).Length >= 2 && File.ReadAllBytes(path)[0] == 0x50 && File.ReadAllBytes(path)[1] == 0x4B)
                    {
                        try
                        {
                            new GorillaMaterial(path);
                            Debug.Log($"[MonkeCosmetics] Loaded Legacy Material: {Path.GetFileName(path)}");
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"[MonkeCosmetics] Failed to load Legacy Material {path}: {ex}");
                        }
                    }
                    else
                    {

                        AssetBundle b = AssetBundle.LoadFromFile(path);
                        var e = b.LoadAsset<GameObject>("material");

                        var mat = e.GetComponent<Renderer>().material;

                        ConvertMaterial(mat);
                        b.Unload(false);
                        CustomCosmeticManager.materials.Add(mat);
                    }
                }
            }
        }

        public static void ConvertMaterial(Material mat)
        {
            if (mat.shader.name != "Standard")
            {
                return;
            }

            Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");

            var mainTex = mat.mainTexture;
            var color = mat.color;
            var metallic = mat.GetFloat("_Metallic");
            var smoothness = mat.GetFloat("_Glossiness");
            var normalMap = mat.GetTexture("_BumpMap");
            var emissionColor = mat.GetColor("_EmissionColor");
            var emissionMap = mat.GetTexture("_EmissionMap");

            mat.shader = urpLit;

            mat.SetTexture("_BaseMap", mainTex);
            mat.color = color;
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Smoothness", smoothness);

            if (normalMap)
                mat.SetTexture("_BumpMap", normalMap);

            if (emissionMap || emissionColor.maxColorComponent > 0f)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetTexture("_EmissionMap", emissionMap);
                mat.SetColor("_EmissionColor", emissionColor);
            }
        }
    }
}
