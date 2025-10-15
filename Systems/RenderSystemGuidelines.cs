// Systems/RenderSystemGuidelines.cs
// Advanced Hover — translucent guidelines via GuideLineSettingsData (works across loads, respects toggle)

namespace AdvancedHoverSystem
{
    using System;
    using Colossal.Serialization.Entities;  // Purpose
    using Game;                            // GameMode, GameSystemBase
    using Game.Prefabs;                    // GuideLineSettingsData, PrefabSystem, PrefabID
    using Unity.Entities;                  // Entity, EntityManager
    using UnityEngine;                     // Color

    /// <summary>
    /// Applies translucent guideline palette by rewriting RenderingSettings → GuideLineSettingsData.
    /// - Works on load (OnGameLoadingComplete).
    /// - Can be requested on-demand (RequestApplyFromSettings).
    /// - Restores original values when disabled (we capture once per session).
    /// </summary>
    public partial class RenderSystemGuidelines : GameSystemBase
    {
        private PrefabSystem m_Prefabs = null!;

        // Capture/restore support
        private static bool s_OriginalCaptured;
        private static GuideLineSettingsData s_Original;

        // Requests from Settings UI
        private static bool s_PendingApply;
        private static bool s_LastDesiredEnabled;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_Prefabs = World.GetOrCreateSystemManaged<PrefabSystem>();
        }

        protected override void OnUpdate()
        {
            // If the Settings UI toggled the option during play, do the work here.
            if (!s_PendingApply)
                return;

            s_PendingApply = false;
            ApplyInternal(enabled: s_LastDesiredEnabled, reason: "OnUpdate-Request");
        }

        /// <summary>Called by Mod.cs once settings are loaded OR by Setting.cs when the toggle changes.</summary>
        public static void RequestApplyFromSettings(bool enabled)
        {
            s_LastDesiredEnabled = enabled;
            s_PendingApply = true;
        }

        /// <summary>Apply translucent palette after the game finished loading the save/map.</summary>
        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            var settings = Mod.Settings;
            if (settings == null)
                return;

            // Honor saved toggle immediately after load.
            ApplyInternal(enabled: settings.TransparentGuidelines, reason: "OnGameLoadingComplete");
        }

        // ---------- Core implementation ----------

        private void ApplyInternal(bool enabled, string reason)
        {
            try
            {
                if (!TryGetRenderingSettingsEntity(out var e))
                {
                    if (Mod.Settings?.VerboseLogging == true)
                        Mod.s_Log.Info("[Guidelines] No RenderingSettings prefab/entity found.");
                    return;
                }

                if (!EntityManager.HasComponent<GuideLineSettingsData>(e))
                {
                    if (Mod.Settings?.VerboseLogging == true)
                        Mod.s_Log.Info("[Guidelines] Entity has no GuideLineSettingsData.");
                    return;
                }

                var data = EntityManager.GetComponentData<GuideLineSettingsData>(e);

                // Capture original the first time we touch it
                if (!s_OriginalCaptured)
                {
                    s_Original = data;
                    s_OriginalCaptured = true;

                    if (Mod.Settings?.VerboseLogging == true)
                    {
                        Mod.s_Log.Info($"[Guidelines] Captured original: " +
                            $"High={ToRGBA(data.m_HighPriorityColor)} " +
                            $"Med={ToRGBA(data.m_MediumPriorityColor)} " +
                            $"Low={ToRGBA(data.m_LowPriorityColor)} " +
                            $"VeryLow={ToRGBA(data.m_VeryLowPriorityColor)}");
                    }
                }

                if (enabled)
                {
                    // Translucent palette (alpha respected by the line renderer)
                    data.m_HighPriorityColor = new Color(1.000f, 1.000f, 1.000f, 0.05f);
                    data.m_MediumPriorityColor = new Color(0.753f, 0.753f, 0.753f, 0.55f);
                    data.m_LowPriorityColor = new Color(0.502f, 0.869f, 1.000f, 0.25f);
                    data.m_VeryLowPriorityColor = new Color(0.695f, 0.877f, 1.000f, 0.584f);
                }
                else
                {
                    // Restore the original palette (what the game had)
                    if (s_OriginalCaptured)
                        data = s_Original;
                }

                EntityManager.SetComponentData(e, data);

                // Verbose telemetry
                if (Mod.Settings?.VerboseLogging == true)
                {
                    Mod.s_Log.Info($"[Guidelines] {reason} → {(enabled ? "ENABLED (translucent)" : "DISABLED (restored)")}: " +
                        $"High={ToRGBA(data.m_HighPriorityColor)} " +
                        $"Med={ToRGBA(data.m_MediumPriorityColor)} " +
                        $"Low={ToRGBA(data.m_LowPriorityColor)} " +
                        $"VeryLow={ToRGBA(data.m_VeryLowPriorityColor)}");
                }
            }
            catch (Exception ex)
            {
                Mod.s_Log.Warn($"[Guidelines] Apply failed: {ex.GetType().Name}: {ex.Message}");
            }
        }

        private bool TryGetRenderingSettingsEntity(out Entity e)
        {
            e = Entity.Null;

            if (!m_Prefabs.TryGetPrefab(new PrefabID(nameof(RenderingSettingsPrefab), "RenderingSettings"), out PrefabBase prefab))
                return false;

            if (!m_Prefabs.TryGetEntity(prefab, out e))
                return false;

            return true;
        }

        private static string ToRGBA(Color c)
            => $"({c.r:0.###},{c.g:0.###},{c.b:0.###},{c.a:0.###})";
    }
}
