using BepInEx;
using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MonkeCosmetics
{
    internal class CustomCosmeticManager : MonoBehaviour
    {
        public List<GameObject> Buttons = new List<GameObject>();
        public List<Material> materials = new List<Material>();
        string localplayermeshdir = "Local VRRig/Local Gorilla Player/gorilla_new";
        public Material DFPrisim = null;
        public Material DFmoon = null;
        public Material DFTrans = null;
        Hashtable LocalCosmetics = new Hashtable();

        public static AssetBundle[] LoadAllBundles()
        {

            // Changed to 'Paths.PlguinPath' so it doesn't matter where the '.mcmat' file is as log as its in the plugins folder it will load.
            string dllDir = Paths.PluginPath;

            string[] bundlePaths = Directory.GetFiles(dllDir, "*.MCmat", SearchOption.AllDirectories);

            AssetBundle[] bundles = new AssetBundle[bundlePaths.Length];

            for (int i = 0; i < bundlePaths.Length; i++)
            {
                try
                {
                    bundles[i] = AssetBundle.LoadFromFile(bundlePaths[i]);
                    Debug.Log($"[MonkeCosmetics] Loaded AssetBundle: {Path.GetFileName(bundlePaths[i])}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[MonkeCosmetics] Failed to load bundle {bundlePaths[i]}: {ex}");
                }
            }

            return bundles;
        }

        public int index = 0;

        public void StartAF()
        {
            materials.Add(DFmoon);
            materials.Add(DFPrisim);
            materials.Add(DFTrans);

            var Bunds = LoadAllBundles();

            for (int i = 0; i < Bunds.Length; i++)
            {
                Material[] Matss = Bunds[i].LoadAllAssets<Material>();
                for (int j = 0; j < Matss.Length; j++)
                {
                    materials.Add(Matss[j]);
                }
            }

            Buttons.Add(gameObject.transform.Find("LeftButton").gameObject);
            Buttons.Add(gameObject.transform.Find("RightButton").gameObject);
            Buttons.Add(gameObject.transform.Find("SelectButton").gameObject);

            Buttons[0].AddComponent<CCButton_Left>();
            Buttons[1].AddComponent<CCButton_Right>();
            Buttons[2].AddComponent<CCButton_Select>();
            Buttons[0].layer = 18;
            Buttons[1].layer = 18;
            Buttons[2].layer = 18;

            LeftArrow();
        }

        public void LoadBundle(string BundleDir)
        {
            string FullBndldr = Path.Combine(Path.GetDirectoryName(typeof(Plugin).Assembly.Location), BundleDir);
            AssetBundle LoadedMatsObj = AssetBundle.LoadFromFile(FullBndldr);
        }

        void setMat(Material mat)
        {
            if (PhotonNetwork.InRoom)
            {
                LocalCosmetics.Add("MonkeCosmetics::Material", mat.name);
                PhotonNetwork.LocalPlayer.SetCustomProperties(LocalCosmetics);
            }
            GameObject.Find("Player Objects").transform.Find(localplayermeshdir).GetComponent<SkinnedMeshRenderer>().material = mat;
        }

        public void LeftArrow()
        {
            if (index > 0)
                index -= 1;

            gameObject.transform.Find("MatPreveiw").GetComponent<MeshRenderer>().material = materials[index];

            var l = gameObject.transform.Find("MatL").GetComponent<MeshRenderer>();
            var r = gameObject.transform.Find("MatR").GetComponent<MeshRenderer>();

            if (index - 1 != -1)
            {
                l.enabled = true;
                l.material = materials[index - 1];
            }
            else
                l.enabled = false;
            

            if (index + 1 != materials.Count)
            {
                r.enabled = true;
                r.material = materials[index + 1];
            }
            else
                r.enabled = false;
            
        }

        public void RightArrow()
        {
            if (index != materials.Count - 1)
                index += 1;

            gameObject.transform.Find("MatPreveiw").GetComponent<MeshRenderer>().material = materials[index];
            var l = gameObject.transform.Find("MatL").GetComponent<MeshRenderer>();
            var r = gameObject.transform.Find("MatR").GetComponent<MeshRenderer>();

            if (index - 1 != -1)
            {
                l.enabled = true;
                l.material = materials[index - 1];
            }
            else
            {
                l.enabled = false;
            }

            if (index + 1 != materials.Count)
            {
                r.enabled = true;
                r.material = materials[index + 1];
            }
            else
            {
                r.enabled = false;
            }
        }
        public void SelectPress()
        {
            setMat(materials[index]);

            gameObject.transform.Find("MatPreveiw").GetComponent<MeshRenderer>().material = materials[index];
            var l = gameObject.transform.Find("MatL").GetComponent<MeshRenderer>();
            var r = gameObject.transform.Find("MatR").GetComponent<MeshRenderer>();

            if (index - 1 != -1)
            {
                l.enabled = true;
                l.material = materials[index - 1];
            }
            else
            {
                l.enabled = false;
            }

            if (index + 1 != materials.Count)
            {
                r.enabled = true;
                r.material = materials[index + 1];
            }
            else
            {
                r.enabled = false;
            }
        }
    }
}
