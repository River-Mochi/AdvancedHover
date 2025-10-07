// Systems/RenderSystemHover.cs
namespace AdvancedHoverSystem
{
    using System;
    using Colossal.Logging;
    using Game;
    using Game.Prefabs;
    using Unity.Entities;
    using UnityEngine;

    /// <summary>
    /// One-shot application of the hover outline color (Gizmo ignores alpha; we set hue only).
    /// Must be partial for Entities source generators.
    /// </summary>
    public sealed partial class RenderSystemHover : GameSystemBase
    {
        private ILog _log = default!;
        private PrefabSystem _prefabs = default!;

        protected override void OnCreate()
        {
            base.OnCreate();
            _log = Mod.Log;
            _prefabs = World.GetOrCreateSystemManaged<PrefabSystem>();
            Enabled = false; // run once in OnStartRunning
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            try
            {
                ApplyHoverColors();
            }
            catch (Exception ex)
            {
                _log.Error($"[AHS] Hover color apply failed: {ex}");
            }

            Enabled = false; // one-shot
        }

        // Required by SystemBase; we do nothing per-frame here.
        protected override void OnUpdate() { }

        private static Color GetPresetColor(HoverColorPreset preset)
        {
            // Order requested: MediumGray (default), Purple, Green, MutedWhite, Tan, Vanilla
            switch (preset)
            {
                case HoverColorPreset.MediumGray: return new Color(0.42f, 0.42f, 0.42f, 1.00f); // #6B6B6BFF approx
                case HoverColorPreset.Purple: return new Color(0.45f, 0.20f, 0.60f, 1.00f); // darker purple
                case HoverColorPreset.Green: return new Color(0.40f, 0.55f, 0.44f, 1.00f); // muted green
                case HoverColorPreset.MutedWhite: return new Color(0.72f, 0.72f, 0.72f, 1.00f); // soft white
                case HoverColorPreset.Tan: return new Color(0.78f, 0.69f, 0.49f, 1.00f); // muted tan
                case HoverColorPreset.Vanilla: return new Color(0.20f, 0.80f, 1.00f, 1.00f); // cyan-ish vanilla
                default: return new Color(0.42f, 0.42f, 0.42f, 1.00f);
            }
        }

        private void ApplyHoverColors()
        {
            if (Mod.Settings == null)
            {
                _log.Warn("[AHS] Settings not available; skipping hover color apply.");
                return;
            }

            Color hovered = GetPresetColor(Mod.Settings.HoverColor);

            // Locate RenderingSettings prefab entity
            if (!_prefabs.TryGetPrefab(new PrefabID(nameof(RenderingSettingsPrefab), "RenderingSettings"), out PrefabBase prefab))
            {
                _log.Warn("[AHS] RenderingSettings prefab not found.");
                return;
            }

            if (!_prefabs.TryGetEntity(prefab, out Entity prefabEntity))
            {
                _log.Warn("[AHS] Could not resolve RenderingSettings entity.");
                return;
            }

            if (!EntityManager.HasComponent<RenderingSettingsData>(prefabEntity))
            {
                _log.Warn("[AHS] RenderingSettingsData not present on prefab entity.");
                return;
            }

            var data = EntityManager.GetComponentData<RenderingSettingsData>(prefabEntity);
            data.m_HoveredColor = hovered; // alpha clamped by Gizmo; hue is what matters
            EntityManager.SetComponentData(prefabEntity, data);

#if DEBUG
            _log.Info($"[AHS] Hover color set to preset '{Mod.Settings.HoverColor}'.");
#endif
        }
    }
}
