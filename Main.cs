using HarmonyLib;
using Kitchen;
using KitchenData;
using KitchenMods;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomSettingsAndLayouts
{
    public class Main : IModInitializer
    {
        public const string MOD_GUID = "IcedMilo.PlateUp.CustomSettingsAndLayouts";
        public const string MOD_NAME = "Custom Settings and Layouts";
        public const string MOD_VERSION = "0.1.7";

        public Main()
        {
            Harmony harmony = new Harmony(MOD_GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void PostActivate(KitchenMods.Mod mod)
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        public void PreInject()
        {
            Main.LogInfo("PreInject");
            CustomerType genericCustomerType = null;
            foreach (CustomerType customerType in GameData.Main.Get<CustomerType>())
            {
                if (customerType.IsGenericGroup)
                {
                    genericCustomerType = customerType;
                    break;
                }
            }

            if (genericCustomerType == null)
                return;
            Main.LogInfo($"Generic Customer Type: {genericCustomerType}");
            GameObject catPrefab = Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(x => x.GetComponent<CustomerView>() != null && x.name.ToLowerInvariant().Contains("cat"))
                .FirstOrDefault();

            if (catPrefab == null)
                return;
            Main.LogInfo($"Cat Prefab: {catPrefab}");
            Registry.SetCustomerModelForType(genericCustomerType, catPrefab);
        }

        public void PostInject()
        {
        }

        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }

        #endregion
    }
}
