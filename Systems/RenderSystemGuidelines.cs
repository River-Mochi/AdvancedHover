// Systems/RenderSystemGuidelines.cs
namespace AdvancedHoverSystem
{
    using Colossal.Serialization.Entities; // Purpose
    using Game;                            // GameMode
    using Game.Prefabs;                    // GuideLineSettingsData, PrefabSystem, PrefabID
    using Unity.Entities;
    using UnityEngine;                     // Color

    public partial class RenderSystemGuidelines : GameSystemBase
    {
        private PrefabSystem m_Prefabs = null!;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_Prefabs = World.GetOrCreateSystemManaged<PrefabSystem>();
        }

        protected override void OnUpdate()
        {
        }

        /// <summary>Apply translucent guideline palette on load when enabled.</summary>
        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            var settings = Mod.Settings;
            if (settings == null || !settings.TransparentGuidelines)
                return;

            if (!m_Prefabs.TryGetPrefab(new PrefabID(nameof(RenderingSettingsPrefab), "RenderingSettings"), out PrefabBase prefab))
                return;

            if (!m_Prefabs.TryGetEntity(prefab, out Entity e))
                return;

            if (!EntityManager.HasComponent<GuideLineSettingsData>(e))
                return;

            var gd = EntityManager.GetComponentData<GuideLineSettingsData>(e);

            // translucent values (alpha respected here)
            gd.m_HighPriorityColor = new Color(1.000f, 1.000f, 1.000f, 0.05f);
            gd.m_MediumPriorityColor = new Color(0.753f, 0.753f, 0.753f, 0.55f);
            gd.m_LowPriorityColor = new Color(0.502f, 0.869f, 1.000f, 0.25f);
            gd.m_VeryLowPriorityColor = new Color(0.695f, 0.877f, 1.000f, 0.584f);

            EntityManager.SetComponentData(e, gd);
        }
    }
}
