using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace MonkeCosmetics
{
    internal class CCButton_Select : GorillaPressableButton
    {
        public override void ButtonActivationWithHand(bool e)
        {
            base.ButtonActivationWithHand(e);
            gameObject.GetComponentInParent<CustomCosmeticManager>().SelectPress();
        }  
    }
}