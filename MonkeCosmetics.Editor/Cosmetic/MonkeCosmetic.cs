using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MonkeCosmetics.Editor.Cosmetic
{
    public class MonkeCosmetic : MonoBehaviour
    {
        public GameObject parentObject;
        public string cosmeticName;
        public string id;
        public bool moddedOnly;
        public Texture2D Thumbnail;

        public bool customColours;
        public Material[] materials;

        public GameObject anchor;
    }
}
