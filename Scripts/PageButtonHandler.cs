namespace MonkeCosmetics.Scripts
{
    internal class PageButtonHandler : GorillaPressableButton
    {
        public override void ButtonActivationWithHand(bool e)
        {
            base.ButtonActivation();

            switch (gameObject.name.Replace("PageButton", ""))
            {
                case "Main":
                    CustomCosmeticManager.instance.PageSelectPress();
                    break;
                case "Right":
                    CustomCosmeticManager.instance.PageRightArrow();
                    break;
                case "Left":
                    CustomCosmeticManager.instance.PageLeftArrow();
                    break;
            }
        }
    }
}