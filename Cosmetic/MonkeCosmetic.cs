using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace MonkeCosmetics.Cosmetic
{
    public class MonkeCosmetic : ScriptableObject
    {
        public GameObject obj;
        public string cosmeticName;
        public string id;
        public Texture2D Thumbnail;
        public CosmeticPosition pos;
    }

    public enum CosmeticPosition
    {
        hand,
        head,
        body
    }
}
