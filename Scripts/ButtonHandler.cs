using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MonkeCosmetics.Scripts
{
    internal class ButtonHandler : GorillaPressableButton
    {
        public override void ButtonActivationWithHand(bool e)
        {
            base.ButtonActivation();
            switch (gameObject.name)
            {
                case "LeftButton":
                    CustomCosmeticManager.instance.LeftArrow();
                    break;
                case "RightButton":
                    CustomCosmeticManager.instance.RightArrow();
                    break;
                case "SelectButton":
                    CustomCosmeticManager.instance.SelectPress();
                    break;
            }
            
        }
    }
}