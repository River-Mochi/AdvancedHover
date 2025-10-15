// Systems/RenderSystemGuidelines.cs
// Advanced Hover — Translucent guidelines by editing RenderingSettings/GuideLineSettingsData.
// Applies on city load AND immediately when the Options UI toggle is changed.

namespace AdvancedHoverSystem
{
    using Colossal.Serialization.Entities; // Purpose
    using Game;                            // GameSystemBase, GameMode
    using Game.Prefabs;                    // PrefabSystem, PrefabBase, PrefabID, RenderingSettingsPrefab, GuideLineSettingsData
    using Unity.Entities;
    using UnityEngine;

    public partial class RenderSystemGuidelines : GameSystemBase
    {
        private PrefabSystem m_Prefabs = null!;

        // Instance handle so static callers (from Settings) can apply immediately.
        private static RenderSystemGuidelines? s_Instance;

        // Bridges the Options UI to this system.
        private static bool s_LastEnabled = true;
        private static bool s_PendingApply;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_Prefabs = World.GetOrCreateSystemManaged<PrefabSystem>();
            s_Instance = this; // allow immediate Apply from the settings toggle
        }

        protected override void OnDestroy()
        {
            if (ReferenceEquals(s_Instance, this))
                s_Instance = null;

            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
            if (!s_PendingApply)
                return;

            s_PendingApply = false;
            Apply(s_LastEnabled, "OnUpdate(Request)");
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            // Apply current toggle when the city entities are ready.
            Apply(Mod.Settings?.TransparentGuidelines ?? true, "OnGameLoadingComplete");
        }

        /// <summary>
        /// Called from Setting.TransparentGuidelines setter.
        /// If we have a live instance (we're in a city), apply immediately.
        /// Otherwise, queue for the next update/load.
        /// </summary>
        public static void RequestApplyFromSettings(bool enabled)
        {
            s_LastEnabled = enabled;

            // Try immediate apply first (works when the user toggles inside a running city).
            if (s_Instance != null)
            {
                s_Instance.Apply(enabled, "Request(Immediate)");
                return;
            }

            // Fallback: queue (e.g., toggled from frontend / no game world yet).
            s_PendingApply = true;
        }

        // ---------- Internal helpers ----------

        private void Apply(bool enabled, string reason)
        {
            // Locate RenderingSettings prefab & entity.
            if (!m_Prefabs.TryGetPrefab(new PrefabID(nameof(RenderingSettingsPrefab), "RenderingSettings"), out PrefabBase prefab))
            {
                if (Mod.Settings?.VerboseLogging == true)
                    Mod.s_Log.Info($"[Guidelines] {reason}: RenderingSettings prefab not found. Will retry on Update.");
                s_PendingApply = true; // retry next frame in case the world is still settling
                return;
            }

            if (!m_Prefabs.TryGetEntity(prefab, out Entity entity))
            {
                if (Mod.Settings?.VerboseLogging == true)
                    Mod.s_Log.Info($"[Guidelines] {reason}: Could not get entity for RenderingSettings. Will retry on Update.");
                s_PendingApply = true;
                return;
            }

            if (!EntityManager.HasComponent<GuideLineSettingsData>(entity))
            {
                if (Mod.Settings?.VerboseLogging == true)
                    Mod.s_Log.Info($"[Guidelines] {reason}: Entity missing GuideLineSettingsData. Will retry on Update.");
                s_PendingApply = true;
                return;
            }

            var data = EntityManager.GetComponentData<GuideLineSettingsData>(entity);

            if (enabled)
            {
                // Translucent palette (alpha respected by the engine).
                data.m_HighPriorityColor = new Color(1.000f, 1.000f, 1.000f, 0.05f);
                data.m_MediumPriorityColor = new Color(0.753f, 0.753f, 0.753f, 0.55f);
                data.m_LowPriorityColor = new Color(0.502f, 0.869f, 1.000f, 0.25f);
                data.m_VeryLowPriorityColor = new Color(0.695f, 0.877f, 1.000f, 0.584f);
            }
            else
            {
                // Restore full opacity (keep RGB to respect the game palette).
                data.m_HighPriorityColor.a = 1f;
                data.m_MediumPriorityColor.a = 1f;
                data.m_LowPriorityColor.a = 1f;
                data.m_VeryLowPriorityColor.a = 1f;
            }

            EntityManager.SetComponentData(entity, data);

            if (Mod.Settings?.VerboseLogging == true)
            {
                Mod.s_Log.Info(
                    $"[Guidelines] {reason}: enabled={enabled} " +
                    $"high={Fmt(data.m_HighPriorityColor)} " +
                    $"med={Fmt(data.m_MediumPriorityColor)} " +
                    $"low={Fmt(data.m_LowPriorityColor)} " +
                    $"vlow={Fmt(data.m_VeryLowPriorityColor)}");
            }
        }

        private static string Fmt(Color c) => $"({c.r:0.##},{c.g:0.##},{c.b:0.##},{c.a:0.##})";
    }
}
