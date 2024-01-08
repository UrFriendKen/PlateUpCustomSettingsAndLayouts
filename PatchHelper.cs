using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace CustomSettingsAndLayouts
{
    public class PatchHelper : GameSystemBase, IModSystem
    {
        static PatchHelper _instance;

        protected override void Initialise()
        {
            base.Initialise();
            _instance = this;
        }

        protected override void OnUpdate()
        {
        }

        internal static bool StaticHas<T>() where T : struct, IComponentData
        {
            return _instance?.Has<T>() ?? false;
        }

        internal static bool StaticRequire<T>(out T comp) where T : struct, IComponentData
        {
            comp = default;
            return _instance?.Require(out comp) ?? false;
        }

        internal static bool StaticHas<T>(Entity e) where T : struct, IComponentData
        {
            return _instance?.Has<T>(e) ?? false;
        }

        internal static bool StaticRequire<T>(Entity e, out T comp) where T : struct, IComponentData
        {
            comp = default;
            return _instance?.Require(e, out comp) ?? false;
        }

        internal static bool StaticSet<T>(Entity e, T comp) where T : struct, IComponentData
        {
            if (_instance == null)
                return false;
            _instance.Set(e, comp);
            return true;
        }
    }
}
