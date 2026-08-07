using GorillaCosmetics.Data;
using HarmonyLib;
using MonkeCosmetics.Cosmetic.Pages;

namespace MonkeCosmetics.Scripts
{
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.ChangeMaterialLocal))]
    public class MatCheckPatch
    {
        [HarmonyPostfix]
        public static void Postfix(VRRig __instance)
        {
            if(MaterialPage.instance.currentMaterial && MaterialPage.initialised) MaterialPage.instance.SetMaterial(MaterialPage.instance.currentMaterial);
        }
    }
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.InitializeNoobMaterialLocal))]
    public class InitializeNoobMaterial
    {
        [HarmonyPostfix]
        private static void Postfix(VRRig __instance)
        {
            if (MaterialPage.instance.currentMaterial && MaterialPage.initialised) MaterialPage.instance.SetMaterial(MaterialPage.instance.currentMaterial);
        }
    }
}
