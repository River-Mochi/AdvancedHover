// Systems/RenderSystemGuidelines.cs
// Advanced Hover — Translucent guidelines by editing RenderingSettings/GuideLineSettingsData.
// Applies on city load and whenever the Options UI toggle is changed.

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

        // These bridge the Options UI (Setting.TransparentGuidelines) to the system.
        private static bool s_LastEnabled = true;
        private static bool s_PendingApply;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_Prefabs = World.GetOrCreateSystemManaged<PrefabSystem>();
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
        /// </summary>
        public static void RequestApplyFromSettings(bool enabled)
        {
            s_LastEnabled = enabled;
            s_PendingApply = true;
        }

        // ---------- Internal helpers ----------

        private void Apply(bool enabled, string reason)
        {
            // Locate RenderingSettings prefab & entity.
            if (!m_Prefabs.TryGetPrefab(new PrefabID(nameof(RenderingSettingsPrefab), "RenderingSettings"), out PrefabBase prefab))
            {
                if (Mod.Settings?.VerboseLogging == true)
                    Mod.s_Log.Info($"[Guidelines] {reason}: RenderingSettings prefab not found.");
                return;
            }

            if (!m_Prefabs.TryGetEntity(prefab, out Entity entity))
            {
                if (Mod.Settings?.VerboseLogging == true)
                    Mod.s_Log.Info($"[Guidelines] {reason}: Could not get entity for RenderingSettings.");
                return;
            }

            if (!EntityManager.HasComponent<GuideLineSettingsData>(entity))
            {
                if (Mod.Settings?.VerboseLogging == true)
                    Mod.s_Log.Info($"[Guidelines] {reason}: Entity missing GuideLineSettingsData.");
                return;
            }

            var data = EntityManager.GetComponentData<GuideLineSettingsData>(entity);

            if (enabled)
            {
                // translucent palette (alpha respected by the engine).
                data.m_HighPriorityColor = new Color(1.000f, 1.000f, 1.000f, 0.05f);
                data.m_MediumPriorityColor = new Color(0.753f, 0.753f, 0.753f, 0.55f);
                data.m_LowPriorityColor = new Color(0.502f, 0.869f, 1.000f, 0.25f);
                data.m_VeryLowPriorityColor = new Color(0.695f, 0.877f, 1.000f, 0.584f);
            }
            else
            {
                // Restore full opacity (leave RGB as-is to respect game palette).
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
