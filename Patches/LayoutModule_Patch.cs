using HarmonyLib;
using Kitchen.Layouts.Modules;

namespace CustomSettingsAndLayouts.Patches
{
    [HarmonyPatch]
    static class LayoutModule_Patch
    {
        [HarmonyPatch(typeof(Module), nameof(Module.OnCreateConnection))]
        [HarmonyPrefix]
        static bool OnCreateConnection_Prefix()
        {
            return Main.BuildDone;
        }
    }
}
