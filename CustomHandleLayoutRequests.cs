using Kitchen;
using KitchenData;
using KitchenMods;
using System;
using System.Linq;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;
using static Kitchen.HandleLayoutRequests;

namespace CustomSettingsAndLayouts
{
    [UpdateBefore(typeof(HandleLayoutRequests))]
    public class CustomHandleLayoutRequests : FranchiseSystem, IModSystem
    {
        private EntityQuery Requests;

        private EntityQuery Slots;

        private EntityQuery MapItems;

        private EntityQuery SettingSelectors;

        Type t_CClearOnLayoutRequest = typeof(HandleLayoutRequests).GetNestedType("CClearOnLayoutRequest", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);

        protected override void Initialise()
        {
            base.Initialise();
            Requests = GetEntityQuery(typeof(SLayoutRequest));
            Slots = GetEntityQuery(typeof(CreateLayoutSlots.CLayoutSlot), typeof(CItemHolder));
            MapItems = GetEntityQuery(typeof(CItemLayoutMap), t_CClearOnLayoutRequest);
            SettingSelectors = GetEntityQuery(typeof(CSettingSelector));
            RequireForUpdate(Requests);
            RequireForUpdate(Slots);
        }

        protected override void OnUpdate()
        {
            if (!Require<SLayoutRequest>(out var comp) || comp.HasBeenCreated)
            {
                return;
            }
            if (t_CClearOnLayoutRequest == null)
            {
                Main.LogError("t_CClearOnLayoutRequest is null");
                return;
            }
            int setting_id = CSettingSelector.IDFromQuery(SettingSelectors);
            if (!Registry.TryGetValidLayoutIDs(setting_id, out int[] valid_layouts))
                return;

            base.EntityManager.DestroyEntity(MapItems);

            Registry.ReplaceAssetReferences(valid_layouts);
            using NativeArray<Entity> pedestalEntities = Slots.ToEntityArray(Allocator.Temp);
            foreach (Entity pedestalEntity in pedestalEntities)
            {
                int source = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                LayoutSeed ls = new LayoutSeed(source, valid_layouts);
                if (valid_layouts.Length > 1)
                {
                    valid_layouts = valid_layouts.Where((int x) => x != ls.LayoutID).ToArray();
                }
                Entity mapEntity = ls.GenerateMap(base.EntityManager, setting_id);
                if (!Require(mapEntity, out CItemLayoutMap layoutMap) || !HasBuffer<CLayoutFeature>(layoutMap.Layout) || !HasBuffer<CLayoutRoomTile>(layoutMap.Layout))
                    continue;
                base.EntityManager.AddComponent(mapEntity, t_CClearOnLayoutRequest);
                base.EntityManager.SetComponentData(pedestalEntity, (CItemHolder)mapEntity);
                base.EntityManager.SetComponentData(mapEntity, (CHeldBy)pedestalEntity);
                if (GameData.Main.TryGet(setting_id, out RestaurantSetting setting) && setting.FixedDish != null)
                {
                    base.EntityManager.AddComponentData(mapEntity, new CSettingDish
                    {
                        DishID = setting.FixedDish.ID
                    });
                }
            }
            Registry.RestoreAssetReferences();
            comp.HasBeenCreated = true;
            Set(comp);
        }
    }
}
