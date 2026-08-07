using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace MonkeCosmetics.Cosmetic
{
    public class MonkeCosmeticPage : ScriptableObject
    {
        public Texture2D Icon;

        public virtual void OnMonkeCosmeticsIntialised()
        {

        }

        public virtual void OnPageEntered()
        {
            Plugin.NameText.text = "Page failed to load.";
            Plugin.DescriptionText.text = "Page failed to load.";

        }

        public virtual void OnPageUpdate()
        {
            
        }

        public virtual void OnEquipPress()
        {

        }

        public virtual void OnLeftPress()
        {

        }

        public virtual void OnRightPress()
        {

        }
    }
}
