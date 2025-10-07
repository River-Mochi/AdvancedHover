// Systems/RenderSystemGuidelines.cs
namespace AdvancedHoverSystem
{
    using System;
    using Colossal.Logging;
    using Game;
    using Game.Prefabs;
    using Unity.Entities;
    using UnityEngine;

    /// <summary>
    /// One-shot customization of guideline colors/alpha (yen-style translucency).
    /// Must be partial for Entities source generators.
    /// </summary>
    public sealed partial class RenderSystemGuidelines : GameSystemBase
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
                ApplyGuidelinePalette();
            }
            catch (Exception ex)
            {
                _log.Error($"[AHS] Guideline palette apply failed: {ex}");
            }

            Enabled = false; // one-shot
        }

        // Required by SystemBase; no per-frame work here.
        protected override void OnUpdate() { }

        private void ApplyGuidelinePalette()
        {
            if (Mod.Settings == null)
            {
                _log.Warn("[AHS] Settings not available; skipping guideline apply.");
                return;
            }

            // Resolve prefab
            if (!_prefabs.TryGetPrefab(new PrefabID(nameof(RenderingSettingsPrefab), "RenderingSettings"), out PrefabBase prefab))
            {
                _log.Warn("[AHS] RenderingSettings prefab not found (guidelines).");
                return;
            }

            if (!_prefabs.TryGetEntity(prefab, out Entity prefabEntity))
            {
                _log.Warn("[AHS] Could not resolve RenderingSettings entity (guidelines).");
                return;
            }

            if (!EntityManager.HasComponent<GuideLineSettingsData>(prefabEntity))
            {
                _log.Warn("[AHS] GuideLineSettingsData not present on prefab entity.");
                return;
            }

            var g = EntityManager.GetComponentData<GuideLineSettingsData>(prefabEntity);

            if (Mod.Settings.TransparentGuidelines)
            {
                // Yen’s translucent palette
                g.m_HighPriorityColor = new Color(1f, 1f, 1f, 0.05f);
                g.m_MediumPriorityColor = new Color(0.753f, 0.753f, 0.753f, 0.55f);
                g.m_LowPriorityColor = new Color(0.502f, 0.869f, 1.00f, 0.25f);
                g.m_VeryLowPriorityColor = new Color(0.695f, 0.877f, 1.00f, 0.584f);
            }
            else
            {
                // Leave vanilla values untouched.
            }

            EntityManager.SetComponentData(prefabEntity, g);

#if DEBUG
            _log.Info($"[AHS] Guidelines {(Mod.Settings.TransparentGuidelines ? "translucent (yen)" : "vanilla")}.");
#endif
        }
    }
}
