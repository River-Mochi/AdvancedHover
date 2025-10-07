// AdvancedHoverSystem.cs
namespace AdvancedHoverSystem
{
    using Colossal.Entities;
    using Colossal.Logging;
    using Colossal.Serialization.Entities;
    using Game;
    using Game.Prefabs;
    using Game.Rendering;
    using Unity.Entities;
    using UnityEngine;

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateBefore(typeof(Game.Rendering.EditorGizmoSystem))]
    public sealed partial class HoverSettingsOnLoadSystem : GameSystemBase
    {
        public readonly PrefabID RenderingSettingsPrefab =
            new PrefabID(nameof(RenderingSettingsPrefab), "RenderingSettings");

        private ILog _log;
        private PrefabSystem _prefabs;
        private RenderingSystem _rendering;

        private bool? _lastHideOverlay = null;
        private Color? _lastHoveredColor = null;

        public void SetRenderingSettingsData(RenderingSettingsData data)
        {
            if (_prefabs.TryGetPrefab(RenderingSettingsPrefab, out PrefabBase prefab)
                && _prefabs.TryGetEntity(prefab, out Entity e)
                && EntityManager.HasComponent<RenderingSettingsData>(e))
            {
                EntityManager.SetComponentData(e, data);
            }
            _log.Info($"{nameof(HoverSettingsOnLoadSystem)}.{nameof(SetRenderingSettingsData)} complete.");
        }

        public void SetGuideLineSettingsData(GuideLineSettingsData data)
        {
            if (_prefabs.TryGetPrefab(RenderingSettingsPrefab, out PrefabBase prefab)
                && _prefabs.TryGetEntity(prefab, out Entity e)
                && EntityManager.HasComponent<GuideLineSettingsData>(e))
            {
                EntityManager.SetComponentData(e, data);
            }
            _log.Info($"{nameof(HoverSettingsOnLoadSystem)}.{nameof(SetGuideLineSettingsData)} complete.");
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            _log = Mod.Log;
            _prefabs = World.GetOrCreateSystemManaged<PrefabSystem>();
            _rendering = World.GetOrCreateSystemManaged<RenderingSystem>();
            Enabled = true;
        }

        protected override void OnUpdate()
        {
            var s = Mod.Settings;
            if (s == null) return;

            // Toggle vanilla gizmo overlay only if changed
            bool wantHide = s?.DisableHoverOutline ?? false;
            if (_lastHideOverlay != wantHide)
            {
                _rendering.hideOverlay = wantHide;
                _lastHideOverlay = wantHide;
            }

            // If visible, sync hovered color only if changed
            if (!wantHide && SystemAPI.TryGetSingletonRW<RenderingSettingsData>(out var rsd))
            {
                var hoverColor = s?.HoverColor ?? HoverColorPreset.Vanilla;
                var hoverBrightness = s?.HoverBrightness ?? 1.0f;
                var target = ApplyBrightness(GetPresetColor(hoverColor), hoverBrightness);
                if (_lastHoveredColor == null || _lastHoveredColor.Value != target)
                {
                    rsd.ValueRW.m_HoveredColor = target;
                    _lastHoveredColor = target;
                }
            }
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            var s = Mod.Settings;
            var hovered = ApplyBrightness(
                GetPresetColor(s?.HoverColor ?? HoverColorPreset.Vanilla),
                s?.HoverBrightness ?? 1.0f
            );

            var renderData = new RenderingSettingsData
            {
                m_ErrorColor = new Color(0.25f, 0.35f, 0.85f, 0.25f),
                m_HoveredColor = hovered,
                m_OverrideColor = new Color(1f, 1f, 0.5f, 0.4f),
                m_OwnerColor = new Color(1f, 0.5f, 0.5f, 0.4f),
                m_WarningColor = new Color(0.247f, 0.981f, 0.247f, 0.1f),
            };

            var guideData = new GuideLineSettingsData
            {
                m_HighPriorityColor = new Color(1f, 1f, 1f, 0.05f),
                m_LowPriorityColor = new Color(0.502f, 0.869f, 1.00f, 0.25f),
                m_MediumPriorityColor = new Color(0.753f, 0.753f, 0.753f, 0.55f),
                m_VeryLowPriorityColor = new Color(0.695f, 0.877f, 1.00f, 0.584f),
            };

            SetGuideLineSettingsData(guideData);
            SetRenderingSettingsData(renderData);

            // Reset edge caches
            _lastHideOverlay = null;
            _lastHoveredColor = null;
        }

        private static Color GetPresetColor(HoverColorPreset p) => p switch
        {
            HoverColorPreset.White => new Color(1f, 1f, 1f, 0.10f),
            HoverColorPreset.Gray => new Color(0.80f, 0.80f, 0.80f, 0.10f),
            HoverColorPreset.Green => new Color(0.50f, 1.00f, 0.60f, 0.10f),
            HoverColorPreset.Purple => new Color(0.75f, 0.55f, 1.00f, 0.10f),
            _ => new Color(0.50f, 0.50f, 1.00f, 0.10f),
        };

        private static Color ApplyBrightness(Color c, float k)
        {
            k = Mathf.Clamp(k, 0f, 2f);
            return new Color(c.r * k, c.g * k, c.b * k, c.a);
        }
    }
}
