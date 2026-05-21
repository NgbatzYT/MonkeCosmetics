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
                    Debug.Log("left");
                    CustomCosmeticManager.instance.LeftArrow();
                    break;
                case "right":
                    Debug.Log("right");
                    CustomCosmeticManager.instance.RightArrow();
                    break;
                case "Button":
                    Debug.Log("select");
                    CustomCosmeticManager.instance.SelectPress();
                    break;
                case "hats":
                    Debug.Log("Hats");
                    CustomCosmeticManager.instance.SelectPress();
                    break;
                case "materials":
                    Debug.Log("Mats");
                    CustomCosmeticManager.instance.SelectPress();
                    break;
            }
        }
    }
}