// Systems/BlockerSystemHighlights.cs
namespace AdvancedHoverSystem
{
    using Colossal.Serialization.Entities; // Purpose
    using Game;                            // GameMode
    using Game.Rendering;                  // RenderingSystem
    using Unity.Entities;

    // NOTE: partial is required for the Unity Entities source generator.
    public partial class BlockerSystemHighlights : GameSystemBase
    {
        private RenderingSystem m_Rendering = null!;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_Rendering = World.GetOrCreateSystemManaged<RenderingSystem>();
        }

        /// <summary>Keep hideOverlay in sync with our checkbox.</summary>
        protected override void OnUpdate()
        {
            var settings = Mod.Settings;
            m_Rendering.hideOverlay = settings != null && settings.DisableHoverOutline;
        }

        /// <summary>Lifecycle hook present for parity; nothing to do here.</summary>
        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
        }
    }
}
