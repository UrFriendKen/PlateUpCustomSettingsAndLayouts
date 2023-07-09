using Kitchen;
using Kitchen.ShopBuilder;
using Kitchen.ShopBuilder.Filters;
using KitchenMods;
using Unity.Entities;

namespace CustomSettingsAndLayouts
{
    [UpdateAfter(typeof(TagStaples))]
    public class FilterBySetting : ShopBuilderFilter, IModSystem
    {
        protected override void Filter(ref CShopBuilderOption option)
        {
            if (option.IsRemoved || !RequireEntity<SLayout>(out var sLayoutEntity) || !Require(sLayoutEntity, out CSetting cSetting))
                return;
            if (Registry.ShouldBlockAppliance(cSetting.RestaurantSetting, option))
            {
                option.IsRemoved = true;
                option.FilteredBy = this;
            }
        }
    }
}
