// Mod.cs
namespace AdvancedHoverSystem
{
    using Colossal;                   // IDictionarySource
    using Colossal.IO.AssetDatabase;  // AssetDatabase
    using Colossal.Logging;           // ILog, LogManager
    using Game;                       // UpdateSystem, GameMode
    using Game.Modding;               // IMod
    using Game.SceneFlow;             // GameManager
    using Game.Settings;

    public sealed class Mod : IMod
    {
        public const string Name = "Advanced Hover";
        public const string Version = "0.1.0";

        public static readonly ILog Log =
            LogManager.GetLogger("AdvancedHover").SetShowsErrorsInUI(false);

        internal static Setting? Settings
        {
            get; private set;
        }

        public void OnLoad(UpdateSystem updateSystem)
        {
            Log.Info($"{Name} {Version} OnLoad");

            // Settings instance and persistence (path must match Setting.cs [FileLocation])
            var settings = new Setting(this);
            Settings = settings;

            AssetDatabase.global.LoadSettings(
                "ModsSettings/AdvancedHover/AdvancedHover",
                settings,
                new Setting(this));

            // Locale BEFORE Options UI so labels resolve
            AddLocale("en-US", new LocaleEN(settings));

            // Register options
            settings.RegisterInOptionsUI();

            // Systems
            var world = updateSystem.World;

            // Load-time systems (only use OnGameLoadingComplete)
            var hover = world.GetOrCreateSystemManaged<RenderSystemHover>();
            hover.Enabled = false; // no per-frame work; only lifecycle hooks

            var guides = world.GetOrCreateSystemManaged<RenderSystemGuidelines>();
            guides.Enabled = false; // no per-frame work; only lifecycle hooks

            // Runtime blocker (per-frame write of hideOverlay)
            var blocker = world.GetOrCreateSystemManaged<BlockerSystemHighlights>();
            blocker.Enabled = true;
        }

        public void OnDispose()
        {
            Log.Info($"{Name} {Version} OnDispose");
            Settings?.UnregisterInOptionsUI();
            Settings = null;
        }

        private static void AddLocale(string localeId, IDictionarySource source)
        {
            var lm = GameManager.instance?.localizationManager;
            if (lm == null)
            {
                Log.Warn("LocalizationManager null; cannot add locale.");
                return;
            }

            lm.AddSource(localeId, source);
        }
    }
}
