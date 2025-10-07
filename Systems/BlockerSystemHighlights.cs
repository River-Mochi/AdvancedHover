// Systems/BlockerSystemHighlights.cs
namespace AdvancedHoverSystem
{
    using Colossal.Logging;
    using Game;
    using Unity.Entities;

    /// <summary>
    /// Runtime highlight/outline blocker (hover suppression).
    /// Must be partial for Entities source generators.
    /// </summary>
    public sealed partial class BlockerSystemHighlights : GameSystemBase
    {
        private ILog _log = default!;

        protected override void OnCreate()
        {
            base.OnCreate();
            _log = Mod.Log;
            Enabled = true; // keep enabled; body is safe no-op
        }

        protected override void OnUpdate()
        {
            // TODO: wire concrete suppression once we confirm the right handle/flag.
            // Keep this method extremely light; no per-frame allocations.
        }
    }
}
