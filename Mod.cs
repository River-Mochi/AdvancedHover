// Mod.cs
namespace AdvancedHoverSystem
{
    using Colossal;                      // IDictionarySource
    using Colossal.IO.AssetDatabase;     // AssetDatabase
    using Colossal.Logging;              // ILog, LogManager
    using Game;                          // UpdateSystem
    using Game.Modding;                  // IMod
    using Game.SceneFlow;                // GameManager

    public sealed class Mod : IMod
    {
        public const string Name = "Advanced Hover";
        public const string Version = "0.1.0";

        public static readonly ILog Log = LogManager.GetLogger("Advanced Hover").SetShowsErrorsInUI(false);

        internal static Setting? Settings { get; private set; }

        public void OnLoad(UpdateSystem updateSystem)
        {
            Log.Info($"{Name} {Version} OnLoad");

            // Create/load persistent settings
            var settings = new Setting(this);
            Settings = settings;
            AssetDatabase.global.LoadSettings(
                "ModsSettings/AdvancedHover/AdvancedHover",
                settings,
                new Setting(this)
            );

            // Register locale BEFORE Options UI so labels resolve
            AddLocale("en-US", new LocaleEN(settings));

            // Options UI
            settings.RegisterInOptionsUI();

            // Ensure our (load + edge-triggered per-frame) system exists
            updateSystem.World
                        .GetOrCreateSystemManaged<HoverSettingsOnLoadSystem>()
                        .Enabled = true;
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
