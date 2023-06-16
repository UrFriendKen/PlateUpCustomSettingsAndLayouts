using Kitchen;
using KitchenMods;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace CustomSettingsAndLayouts
{
    [UpdateBefore(typeof(GrantUpgrades))]
    public class GrantSettingUpgrades : FranchiseFirstFrameSystem, IModSystem
    {
        private EntityQuery SettingUpgrades;

        protected override void Initialise()
        {
            base.Initialise();
            SettingUpgrades = GetEntityQuery(typeof(CSettingUpgrade));
        }

        protected override void OnUpdate()
        {
            foreach (int settingOptionToAdd in Registry.GetSettingsToGrant())
            {
                if (settingOptionToAdd == 0)
                    continue;

                bool shouldAdd = true;
                using NativeArray<Entity> settingUpgrades = SettingUpgrades.ToEntityArray(Allocator.Temp);
                foreach (Entity settingUpgrade in settingUpgrades)
                {
                    if (base.EntityManager.GetComponentData<CSettingUpgrade>(settingUpgrade).SettingID == settingOptionToAdd)
                    {
                        shouldAdd = false;
                        break;
                    }
                }

                if (shouldAdd)
                {
                    Entity entity = base.EntityManager.CreateEntity(typeof(CSettingUpgrade), typeof(CPersistThroughSceneChanges));
                    base.EntityManager.SetComponentData(entity, new CSettingUpgrade
                    {
                        SettingID = settingOptionToAdd
                    });
                }
            }
        }
    }
}
