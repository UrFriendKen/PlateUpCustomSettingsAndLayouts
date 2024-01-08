using HarmonyLib;
using Kitchen;
using KitchenData;
using System;
using Unity.Entities;

namespace CustomSettingsAndLayouts.Patches
{
    [HarmonyPatch]
    [HarmonyPriority(int.MinValue)]
    static class CreateCustomerGroup_Patch
    {
        [HarmonyPatch(typeof(CreateCustomerGroup), "NewCustomer", new Type[] { typeof(Entity), typeof(bool), typeof(CustomerType) })]
        [HarmonyPostfix]
        static void NewCustomer_Postfix(CustomerType type, ref Entity __result)
        {
            if (!Registry.TryGetCustomCustomerViewType(type, out ViewType viewType) ||
                !PatchHelper.StaticRequire<CRequiresView>(__result, out CRequiresView cRequiresView))
                return;

            cRequiresView.Type = viewType;
            PatchHelper.StaticSet(__result, cRequiresView);
        }
    }
}
