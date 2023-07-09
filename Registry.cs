using Kitchen.ShopBuilder;
using KitchenData;
using System.Collections.Generic;

namespace CustomSettingsAndLayouts
{
    public static class Registry
    {
        private static Dictionary<int, List<int>> _validLayoutsBySetting = new Dictionary<int, List<int>>();

        private static int[] AssetReferenceFixedRunLayoutCache;

        private static HashSet<int> RegisteredSettingsToGrant = new HashSet<int>();

        private static Dictionary<int, HashSet<int>> _specialDecorationsBySetting = new Dictionary<int, HashSet<int>>();

        private static Dictionary<int, HashSet<int>> _disableAppliancesBySetting = new Dictionary<int, HashSet<int>>();

        public static void AddSettingLayout(RestaurantSetting setting, LayoutProfile layoutProfile, bool noDuplicates = false)
        {
            if (!_validLayoutsBySetting.ContainsKey(setting.ID))
                _validLayoutsBySetting.Add(setting.ID, new List<int>());
            if (noDuplicates && _validLayoutsBySetting[setting.ID].Contains(layoutProfile.ID))
                return;
            _validLayoutsBySetting[setting.ID].Add(layoutProfile.ID);
        }

        public static void AddSettingLayout(RestaurantSetting setting, IEnumerable<LayoutProfile> layoutProfiles, bool noDuplicates = false)
        {
            foreach (LayoutProfile layoutProfile in layoutProfiles)
            {
                AddSettingLayout(setting, layoutProfile, noDuplicates);
            }
        }

        public static void AddSettingDecoration(RestaurantSetting setting, Appliance appliance)
        {
            if (!_specialDecorationsBySetting.ContainsKey(setting.ID))
                _specialDecorationsBySetting.Add(setting.ID, new HashSet<int>());
            if (_specialDecorationsBySetting[setting.ID].Contains(appliance.ID))
                return;
            _specialDecorationsBySetting[setting.ID].Add(appliance.ID);
        }

        public static void AddSettingDecoration(RestaurantSetting setting, IEnumerable<Appliance> appliances)
        {
            foreach (Appliance appliance in appliances)
            {
                AddSettingDecoration(setting, appliance);
            }
        }

        public static void AddSettingDisableAppliance(RestaurantSetting setting, Appliance appliance)
        {
            if (!_disableAppliancesBySetting.ContainsKey(setting.ID))
                _disableAppliancesBySetting.Add(setting.ID, new HashSet<int>());
            if (_disableAppliancesBySetting[setting.ID].Contains(appliance.ID))
                return;
            _disableAppliancesBySetting[setting.ID].Add(appliance.ID);
        }

        public static void AddSettingDisableAppliance(RestaurantSetting setting, IEnumerable<Appliance> appliances)
        {
            foreach (Appliance appliance in appliances)
            {
                AddSettingDisableAppliance(setting, appliance);
            }
        }

        public static void ClearSettingLayout(RestaurantSetting setting)
        {
            if (!_validLayoutsBySetting.ContainsKey(setting.ID))
                return;
            _validLayoutsBySetting.Remove(setting.ID);
        }

        public static void GrantCustomSetting(RestaurantSetting setting)
        {
            if ((setting?.ID ?? 0) != 0 && !RegisteredSettingsToGrant.Contains(setting.ID))
                RegisteredSettingsToGrant.Add(setting.ID);
        }

        internal static bool TryGetValidLayoutIDs(int settingID, out int[] validLayoutIDs)
        {
            validLayoutIDs = null;
            bool success = _validLayoutsBySetting.TryGetValue(settingID, out List<int> layoutIDsList);
            if (success)
            {
                validLayoutIDs = layoutIDsList.ToArray();
            }
            return success;
        }

        internal static void CacheAssetReferences()
        {
            AssetReferenceFixedRunLayoutCache = AssetReference.FixedRunLayout;
        }

        internal static void ReplaceAssetReferences(int[] valid_layout_ids, bool doCache = true)
        {
            if (doCache)
                CacheAssetReferences();
            AssetReference.FixedRunLayout = valid_layout_ids;
        }

        internal static void RestoreAssetReferences()
        {
            if (AssetReferenceFixedRunLayoutCache != null)
                AssetReference.FixedRunLayout = AssetReferenceFixedRunLayoutCache;
        }

        internal static HashSet<int> GetSettingsToGrant()
        {
            return new HashSet<int>(RegisteredSettingsToGrant);
        }

        internal static Dictionary<int, HashSet<int>> GetSpecialDecorationsBySetting()
        {
            return new Dictionary<int, HashSet<int>>(_specialDecorationsBySetting);
        }

        internal static bool ShouldBlockAppliance(int restaurantSettingID, CShopBuilderOption option)
        {
            if (option.IsRemoved)
                return false;

            HashSet<int> applianceIDs;
            if (option.Tags.HasFlag(ShoppingTags.SpecialEvent) &&
                _specialDecorationsBySetting.TryGetValue(restaurantSettingID, out applianceIDs) &&
                !applianceIDs.Contains(option.Appliance))
                return true;
            if (_disableAppliancesBySetting.TryGetValue(restaurantSettingID, out applianceIDs) &&
                applianceIDs.Contains(option.Appliance))
                return true;
            return false;
        }
    }
}
