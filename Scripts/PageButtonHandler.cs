using System.Diagnostics;

namespace MonkeCosmetics.Scripts
{
    internal class PageButtonHandler : GorillaPressableButton
    {
        public override void ButtonActivationWithHand(bool e)
        {
            base.ButtonActivation();

            switch (gameObject.name.Replace("PageButton", ""))
            {
                case "Right":
                    CustomCosmeticManager.instance.PageRightArrow();
                    break;
                case "Left":
                    CustomCosmeticManager.instance.PageLeftArrow();
                    break;
                case "UpdateButton":
                    Process.Start("https://github.com/ngbatzyt/MonkeCosmetics/releases/latest");
                    Plugin.HideUpdate();
                    break;
            }
        }
    }
}