using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MonkeCosmetics
{
    internal class CCButton_Left : GorillaPressableButton
    {
        public override void ButtonActivationWithHand(bool e)
        {
            base.ButtonActivation();
            gameObject.GetComponentInParent<CustomCosmeticManager>().LeftArrow();
        }
    }
}