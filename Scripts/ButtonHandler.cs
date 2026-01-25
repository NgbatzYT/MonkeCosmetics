namespace MonkeCosmetics.Scripts
{
    internal class ButtonHandler : GorillaPressableButton
    {
        public override void ButtonActivationWithHand(bool e)
        {
            base.ButtonActivation();
            switch (gameObject.name)
            {
                case "left":
                    CustomCosmeticManager.instance.LeftArrow();
                    break;
                case "right":
                    CustomCosmeticManager.instance.RightArrow();
                    break;
                case "e1":
                    CustomCosmeticManager.instance.SelectPress(0);
                    break;
                case "e2":
                    CustomCosmeticManager.instance.SelectPress(1);
                    break;
                case "e3":
                    CustomCosmeticManager.instance.SelectPress(2);
                    break;
                case "Remove":
                    CustomCosmeticManager.instance.RemovePress();
                    break;
                case "Hats":
                    break;
                case "Material":
                    break;
            }
        }
    }
}