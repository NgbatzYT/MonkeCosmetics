using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace MonkeCosmetics.Cosmetic
{
    public class MonkeCosmetic : ScriptableObject
    {
        public GameObject parentObject;
        public string cosmeticName;
        public string id;
        public bool moddedOnly;
        public Texture2D Thumbnail;

        public bool customColours;
        public Material[] materials;

        public GameObject anchor;

        public UnityEvent RightTriggerAction;
        public UnityEvent LeftTriggerAction;
        public UnityEvent RightGripAction;
        public UnityEvent LeftGripAction;
        public UnityEvent SecondaryAction;
        public UnityEvent PrimaryAction;
    }
}
