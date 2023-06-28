using KitchenData;
using System.Collections.Generic;
using System.Linq;

namespace CustomSettingsAndLayouts
{
    public static class Registry
    {
        private static Dictionary<int, List<int>> _validLayoutsBySetting = new Dictionary<int, List<int>>();

        private static int[] AssetReferenceFixedRunLayoutCache;

        private static HashSet<int> RegisteredSettingsToGrant = new HashSet<int>();

        private static HashSet<int> RegisteredGenericLayoutsToAdd = new HashSet<int>();

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

        public static void AddGenericLayout(LayoutProfile layoutProfile, int totalCount = 2)
        {
            if ((layoutProfile?.ID ?? 0) != 0)
            {
                int currentCount = AssetReference.FixedRunLayout.Where(x => x == layoutProfile.ID).Count();
                for (int i = 0; i < totalCount - currentCount; i++)
                {
                    AssetReference.FixedRunLayout = AssetReference.FixedRunLayout.Append(layoutProfile.ID).ToArray();
                }
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
            return RegisteredSettingsToGrant;
        }
    }
}
