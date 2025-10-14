// Systems/RenderSystemHover.cs
namespace AdvancedHoverSystem
{
    using Colossal.Serialization.Entities; // Purpose
    using Game;                            // GameMode
    using Game.Prefabs;                    // RenderingSettingsData, GuideLineSettingsData, PrefabSystem, PrefabID
    using Unity.Entities;
    using UnityEngine;

    public partial class RenderSystemHover : GameSystemBase
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

        /// <summary>Apply hover outline re-tint on load (alpha ignored by gizmo).</summary>
        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            var settings = Mod.Settings;
            if (settings == null)
                return;

            if (settings.HoverColor == HoverColorPreset.Vanilla)
                return; // leave game's default color untouched

            if (!m_Prefabs.TryGetPrefab(new PrefabID(nameof(RenderingSettingsPrefab), "RenderingSettings"), out PrefabBase prefab))
                return;

            if (!m_Prefabs.TryGetEntity(prefab, out Entity e))
                return;

            if (!EntityManager.HasComponent<RenderingSettingsData>(e))
                return;

            var data = EntityManager.GetComponentData<RenderingSettingsData>(e);
            data.m_HoveredColor = ToHoverColor(settings.HoverColor);
            EntityManager.SetComponentData(e, data);
        }

        private static Color ToHoverColor(HoverColorPreset preset)
        {

            switch (preset)
            {
                case HoverColorPreset.MediumGray:
                    return new Color(0.420f, 0.420f, 0.420f, 0.5f);
                case HoverColorPreset.Purple:
                    return new Color(0.25f, 0.15f, 0.25f, 0.5f);
                case HoverColorPreset.Green:
                    return new Color(0.400f, 0.550f, 0.440f, 0.5f);
                default:
                    return new Color(0.420f, 0.420f, 0.420f, 1f);
            }
        }
    }
}
