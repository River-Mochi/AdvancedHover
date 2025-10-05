// GatedSystemBase.cs
//
// Optional base class for CS2 mods.
// Purpose:
// 1) "Gameplay gating" — only run a system in specific GameMode(s) (default: gameplay).
// 2) Offers an optional one-shot hook after a city finishes loading.
//    If the one-shot isn’t needed, simply don’t override OnAfterLoadOnce().
//
// Usage:
//   public sealed class ExampleSystem : GatedSystemBase
//   {
//       // If you also want this in Map Editor, uncomment:
//       // protected override GameMode kAllowedModes => GameMode.Game | GameMode.MapEditor;
//
//       protected override void OnUpdate()
//       {
//           if (!Enabled) return;
//           // per-frame work…
//       }
//
//       // Optional:
//       // protected override void OnAfterLoadOnce() { /* run once after city is loaded */ }
//   }
//
namespace YourMod
{
    using Colossal.Logging;
    using Colossal.Serialization.Entities; // Purpose, GameMode
    using Game;
    using Unity.Entities;

    public abstract class GatedSystemBase : GameSystemBase
    {
        protected ILog m_Log;

        /// <summary>Override to change where the system is allowed to run.</summary>
        protected virtual GameMode kAllowedModes => GameMode.Game; // default: gameplay only

        protected override void OnCreate()
        {
            base.OnCreate();
            m_Log = Mod.s_Log;
            Enabled = false; // decide at preload
        }

        protected override void OnGamePreload(Purpose purpose, GameMode mode)
        {
            base.OnGamePreload(purpose, mode);
            Enabled = (kAllowedModes & mode) != GameMode.None; // friend2-style gating
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            if (Enabled)
            {
                OnAfterLoadOnce(); // one-shot, override only if needed
            }
        }

        /// <summary>Optional: one-shot when a city becomes ready (after load).</summary>
        protected virtual void OnAfterLoadOnce() { }
    }
}
