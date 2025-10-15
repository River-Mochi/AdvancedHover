// Systems/RenderSystemGuidelines.cs
// Advanced Hover — keep guideline dashes translucent by editing RenderingSettings/GuideLineSettingsData

namespace AdvancedHoverSystem
{
    using Colossal.Serialization.Entities; // Purpose
    using Game;                            // GameMode, GameSystemBase
    using Game.Prefabs;                    // GuideLineSettingsData, PrefabSystem, PrefabID, RenderingSettingsPrefab
    using Unity.Entities;
    using UnityEngine;

    public partial class RenderSystemGuidelines : GameSystemBase
    {
        private PrefabSystem m_Prefabs = null!;

        // Flip this from Setting.TransparentGuidelines setter — we’ll apply on next update.
        private static bool s_PendingApply;
        private static bool s_LastEnabled = true;

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

        /// <summary>Run after city has loaded so RenderingSettings exists.</summary>
        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            // Apply current choice once the entities are ready.
            Apply(Mod.Settings?.TransparentGuidelines ?? true, "OnGameLoadingComplete");
        }

        /// <summary>Called from Setting.TransparentGuidelines setter.</summary>
        public static void RequestApplyFromSettings(bool enabled)
        {
            s_LastEnabled = enabled;
            s_PendingApply = true;
        }

        // ------- Internal -------

        private void Apply(bool enabled, string reason)
        {
            // Find the RenderingSettings prefab/entity.
            if (!m_Prefabs.TryGetPrefab(new PrefabID(nameof(RenderingSettingsPrefab), "RenderingSettings"), out PrefabBase prefab))
            {
                if (Mod.Settings?.VerboseLogging == true)
                    Mod.s_Log.Info($"[Guidelines] {reason}: RenderingSettings prefab not found.");
                return;
            }

            if (!m_Prefabs.TryGetEntity(prefab, out Entity e))
            {
                if (Mod.Settings?.VerboseLogging == true)
                    Mod.s_Log.Info($"[Guidelines] {reason}: Could not get entity for RenderingSettings.");
                return;
            }

            if (!EntityManager.HasComponent<GuideLineSettingsData>(e))
            {
                if (Mod.Settings?.VerboseLogging == true)
                    Mod.s_Log.Info($"[Guidelines] {reason}: Entity missing GuideLineSettingsData.");
                return;
            }

            var gd = EntityManager.GetComponentData<GuideLineSettingsData>(e);

            // Your translucent palette (alphas respected by the engine).
            if (enabled)
            {
                gd.m_HighPriorityColor = new Color(1.000f, 1.000f, 1.000f, 0.05f);
                gd.m_MediumPriorityColor = new Color(0.753f, 0.753f, 0.753f, 0.55f);
                gd.m_LowPriorityColor = new Color(0.502f, 0.869f, 1.000f, 0.25f);
                gd.m_VeryLowPriorityColor = new Color(0.695f, 0.877f, 1.000f, 0.584f);
            }
            else
            {
                // Restore full opacity (keep RGB as-is).
                gd.m_HighPriorityColor.a = 1f;
                gd.m_MediumPriorityColor.a = 1f;
                gd.m_LowPriorityColor.a = 1f;
                gd.m_VeryLowPriorityColor.a = 1f;
            }

            EntityManager.SetComponentData(e, gd);

            if (Mod.Settings?.VerboseLogging == true)
            {
                Mod.s_Log.Info(
                    $"[Guidelines] {reason}: enabled={enabled} " +
                    $"high={Fmt(gd.m_HighPriorityColor)} " +
                    $"med={Fmt(gd.m_MediumPriorityColor)} " +
                    $"low={Fmt(gd.m_LowPriorityColor)} " +
                    $"vlow={Fmt(gd.m_VeryLowPriorityColor)}");
            }
        }

        private static string Fmt(Color c) => $"({c.r:0.##},{c.g:0.##},{c.b:0.##},{c.a:0.##})";
    }
}
