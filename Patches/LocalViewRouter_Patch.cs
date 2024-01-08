using HarmonyLib;
using Kitchen;
using UnityEngine;

namespace CustomSettingsAndLayouts.Patches
{
    [HarmonyPatch]
    static class LocalViewRouter_Patch
    {
        [HarmonyPatch(typeof(LocalViewRouter), "GetPrefab")]
        [HarmonyPrefix]
        static bool GetPrefab_Prefix(ViewType view_type, ref GameObject __result)
        {
            if (Registry.TryGetCustomCustomerViewPrefab(view_type, out GameObject customerViewPrefab))
            {
                __result = customerViewPrefab;
                return false;
            }

            return true;
        }
    }
}
