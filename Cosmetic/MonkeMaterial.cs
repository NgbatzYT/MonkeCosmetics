using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MonkeCosmetics.Cosmetic
{
    public class MonkeMaterial : ScriptableObject
    {
        public Material material;
        public string materialName;
        public string id;
        public Texture2D Thumbnail;

        public bool customColours;
        public bool moddedOnly;
    }
}