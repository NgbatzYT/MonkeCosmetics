namespace MonkeCosmetics.Scripts
{
    internal class ButtonHandler : GorillaPressableButton
    {
        public override void ButtonActivationWithHand(bool e)
        {
            base.ButtonActivation();
            switch (gameObject.name)
            {
                case "ButtonLeft":
                    Debug.Log("left");
                    CustomCosmeticManager.instance.LeftArrow();
                    break;
                case "ButtonRight":
                    Debug.Log("right");
                    CustomCosmeticManager.instance.RightArrow();
                    break;
                case "SelectButton":
                    Debug.Log("select");
                    CustomCosmeticManager.instance.SelectPress();
                    break;
            }
        }
    }
}