using HarmonyLib;


namespace MonkeCosmetics
{
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.ChangeMaterialLocal))]
    public class MatCheckPatch
    {
        [HarmonyPostfix]
        public void Postfix(VRRig __instance)
        {
            CustomCosmeticManager.instance.SetMaterial(CustomCosmeticManager.instance.currentMaterial);
        }
    }

    [HarmonyPatch(typeof(VRRig), "InitializeNoobMaterialLocal")]
    internal class InitializeNoobMaterial
    {
        [HarmonyPostfix]
        private static void Postfix(VRRig __instance)
        {
            CustomCosmeticManager.instance.SetMaterial(CustomCosmeticManager.instance.currentMaterial);
        }
    }
}
