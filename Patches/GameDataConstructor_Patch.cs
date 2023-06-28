using HarmonyLib;
using KitchenData;
using System;

namespace CustomSettingsAndLayouts.Patches
{
    [HarmonyPatch]
    static class GameDataConstructor_Patch
    {
        [HarmonyPatch(typeof(GameDataConstructor), "BuildGameData", new Type[] { })]
        [HarmonyPostfix]
        static void BuildGameData_Postfix()
        {
            Main.BuildDone = true;
        }
    }
}
