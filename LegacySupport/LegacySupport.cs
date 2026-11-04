using BepInEx;
using GorillaCosmetics.Data;
using MonkeCosmetics.Cosmetic;
using MonkeCosmetics.Cosmetic.Pages;
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

                if (new string[] { ".material", ".gmat", ".gmatplus", ".mcmat" }.Contains(Path.GetExtension(path).ToLower()))
                {
                    try
                    {
                        Debug.Log($"Starting To Load Legacy Material: {Path.GetFileName(path)}");

                        if (File.Exists(path) && File.ReadAllBytes(path).Length >= 2 && File.ReadAllBytes(path)[0] == 0x50 && File.ReadAllBytes(path)[1] == 0x4B)
                        {
                            try
                            {
                                new GorillaMaterial(path);
                            }
                            catch (Exception ex)
                            {
                                Debug.LogWarning($"Failed to load Legacy Material {Path.GetFileName(path)}: {ex.Message}");
                            }
                        }
                        else
                        {
                            AssetBundle b = AssetBundle.LoadFromFile(path);
                            try
                            {
                                var es = b.LoadAllAssets<MonkeMaterial>();

                                if (es.Length > 0)
                                {
                                    foreach (MonkeMaterial meat in es)
                                        MaterialPage.materials.Add(meat);
                                }

                                var e = b.LoadAllAssets<Material>();

                                foreach (Material mate in e)
                                {
                                    CreateMaterial(mate, path);
                                }

                            }
                            catch { /*ignore*/ }

                            try
                            {
                                var ess = b.LoadAsset<GameObject>("material");
                                var mat = ess.GetComponent<Renderer>().material;
                                CreateMaterial(mat, path);
                            }
                            catch { /*ignore*/ }

                            try
                            {
                                var ee = b.LoadAsset<GameObject>("_Material");
                                var mats = ee.GetComponent<Renderer>().material;
                                CreateMaterial(mats, path);
                            }
                            catch { /*ignore*/ }

                            b.Unload(false);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Failed to load Legacy Material {Path.GetFileName(path)}: {e.Message}");
                    }
                }
            }
        }

        public static void CreateMaterial(Material mat, string path)
        {
            try
            {
                ConvertMaterial(mat);

                MonkeMaterial matConverted = MonkeMaterial.CreateInstance<MonkeMaterial>();

                if (new string[] { "_followplayercolour", "_followplayercolor" }.Contains(mat.name.ToLower()))
                    matConverted.customColours = true;

                matConverted.material = mat;
                matConverted.id = $"legacy{Guid.NewGuid().ToString("N")[..5].ToUpper()}.{mat.name}";

                matConverted.materialName = matConverted.material.name;

                MaterialPage.materials.Add(matConverted);
                Debug.Log($"Loaded Legacy Material ({matConverted.id}, {matConverted.materialName}): {Path.GetFileName(path)}");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to load a material from: {Path.GetFileName(path)} | {e.Message}");
            }

        }

        public static void ConvertMaterial(Material mat)
        {
            if (mat.shader.name == "Standard")
            {
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
}
