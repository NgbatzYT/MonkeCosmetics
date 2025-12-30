using HarmonyLib;


namespace MonkeCosmetics.Scripts
{
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.ChangeMaterialLocal))]
    public class MatCheckPatch
    {
        [HarmonyPostfix]
        public static void Postfix(VRRig __instance)
        {
            CustomCosmeticManager.instance.SetMaterial(CustomCosmeticManager.instance.currentMaterial);
        }
    }
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.InitializeNoobMaterialLocal))]
    public class InitializeNoobMaterial
    {
        [HarmonyPostfix]
        private static void Postfix(VRRig __instance)
        {
            CustomCosmeticManager.instance.SetMaterial(CustomCosmeticManager.instance.currentMaterial);
        }
    }
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.InitializeNoobMaterial))]
    public class InitializeNoobMaterialr
    {
        [HarmonyPostfix]
        private static void Postfix(VRRig __instance)
        {
            CustomCosmeticManager.instance.SetMaterial(CustomCosmeticManager.instance.currentMaterial);
        }
    }
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.ChangeMaterial))]
    public class MatCheckPatchr
    {
        [HarmonyPostfix]
        private static void Postfix(VRRig __instance)
        {
            CustomCosmeticManager.instance.SetMaterial(CustomCosmeticManager.instance.currentMaterial);
        }
    }
}
