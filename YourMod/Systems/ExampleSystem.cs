namespace YourMod.Systems
{
    using Colossal.Logging;                  // ILog
    using Colossal.Serialization.Entities;   // Purpose, GameMode
    using Game;                              // GameSystemBase
    using Unity.Entities;                    // ECS base

    /// <summary>
    /// Example ECS system. Uses “gameplay-only” gating and an optional one-shot apply after load.
    /// </summary>
    public sealed class ExampleSystem : GameSystemBase
    {
        private ILog m_Log;
        private const GameMode kAllowedModes = GameMode.Game; // gameplay only

        protected override void OnCreate()
        {
            base.OnCreate();
            m_Log = Mod.s_Log;
            Enabled = false; // this system decides when to run
        }

        protected override void OnGamePreload(Purpose purpose, GameMode mode)
        {
            base.OnGamePreload(purpose, mode);
            UpdateEnabled(mode);
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            UpdateEnabled(mode);

            if (Enabled)
            {
                // One-shot work: runs once when a city is ready
                ApplyOnceAfterLoad();
            }
        }

        protected override void OnUpdate()
        {
            if (!Enabled) return;

            // Per-frame work goes here…
        }

        private void UpdateEnabled(GameMode mode)
        {
            bool shouldRun = (kAllowedModes & mode) != GameMode.None;
            if (Enabled != shouldRun)
            {
                Enabled = shouldRun;
                m_Log?.Info($"[{nameof(ExampleSystem)}] {(Enabled ? "ENABLED" : "DISABLED")} for mode={mode}");
            }
        }

        private void ApplyOnceAfterLoad()
        {
            m_Log.Info($"[{nameof(ExampleSystem)}] ApplyOnceAfterLoad");
            // Do one-shot initialization or application of settings here.
        }
    }
}
