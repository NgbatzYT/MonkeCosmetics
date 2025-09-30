namespace MonkeCosmetics.Scripts
{
    internal class ButtonHandler : GorillaPressableButton
    {
        public override void ButtonActivationWithHand(bool e)
        {
            base.ButtonActivation();
            switch (gameObject.name)
            {
                case "Left":
                    CustomCosmeticManager.instance.LeftArrow();
                    break;
                case "Right":
                    CustomCosmeticManager.instance.RightArrow();
                    break;
                case "Select":
                    CustomCosmeticManager.instance.SelectPress();
                    break;
                case "Remove":
                    CustomCosmeticManager.instance.RemovePress();
                    break;
            }
        }
    }
}