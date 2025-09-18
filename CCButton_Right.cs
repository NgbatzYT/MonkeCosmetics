using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MonkeCosmetics
{
    internal class CCButton_Right : GorillaPressableButton
    {
        public override void ButtonActivationWithHand(bool e)
        {
            base.ButtonActivation();
            gameObject.GetComponentInParent<CustomCosmeticManager>().RightArrow();
        }
    }
}