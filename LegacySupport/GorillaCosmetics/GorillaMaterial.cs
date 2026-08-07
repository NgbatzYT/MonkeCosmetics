using MonkeCosmetics;
using MonkeCosmetics.LegacySupport;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GorillaCosmetics.Data
{
    public class GorillaMaterial : IAsset
    {
        public string FileName { get; }
        public AssetBundle AssetBundle { get; }
        public CosmeticDescriptor Descriptor { get; }

        Material material;

        public GorillaMaterial(string path)
        {
            if (path != "Default")
            {
                try
                {
                    PackageJSON json = null;
                    using (ZipArchive archive = ZipFile.OpenRead(path))
                    {
                        var jsonEntry = archive.Entries.First(i => i.Name == "package.json");
                        if (jsonEntry != null)
                        {
                            var stream = new StreamReader(jsonEntry.Open(), Encoding.Default);
                            string jsonString = stream.ReadToEnd();
                            json = JsonConvert.DeserializeObject<PackageJSON>(jsonString);
                        }
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (json != null && entry.Name == json.pcFileName)
                            {
                                var SeekableStream = new MemoryStream();
                                entry.Open().CopyTo(SeekableStream);
                                SeekableStream.Position = 0;
                                AssetBundle = AssetBundle.LoadFromStream(SeekableStream);
                            }
                        }
                    }

                    


                    GameObject materialObject = AssetBundle.LoadAsset<GameObject>("_Material");
                    material = materialObject.GetComponent<Renderer>().material;
                    material.name = $"{json.descriptor.objectName}{(json.config.customColors ? "_FollowPlayerColor" : "")}";

                    LegacySupport.ConvertMaterial(material);
                }
                catch
                {
                    // no error loggin!!
                }
            }

            //if (material != null) CustomCosmeticManager.materials.Add(material);
        }

        public Material GetMaterial()
        {
            return material;
        }

        public GameObject GetPreviewOrb(Transform parent)
        {
            return null;
        }
    }
    [System.Serializable]
    public class PackageJSON
    {
        public string androidFileName;
        public string pcFileName;
        public Descriptor descriptor;
        public Config config;
    }
    [System.Serializable]
    public class Descriptor
    {
        public string objectName;
        public string author;
        public string description;
    }

    [System.Serializable]
    public class Config
    {
        public bool customColors;
        public bool disableInPublicLobbies;
    }
    public interface IAsset
    {
        string FileName { get; }
        CosmeticDescriptor Descriptor { get; }
    }

    public class CosmeticDescriptor
    {
        public string Name = "Cosmetic";
        public string AuthorName = "Author";
        public string Description = string.Empty;
        public bool CustomColors = false;
        public bool DisablePublicLobbies = false;
    }
}