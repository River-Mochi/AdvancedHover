// Mod.cs
//
// Minimal, reusable Mod entry point for CS2 mods.
// - Sets up logging (s_Log)
// - Creates settings + loads saved values
// - Registers locales (optional)
// - Registers systems with UpdateSystem
//
// This template intentionally avoids any “one-shot after load” logic;
// systems should handle that themselves (if needed) via GatedSystemBase.
//
namespace YourMod
{
    using Colossal;                        // IDictionarySource
    using Colossal.IO.AssetDatabase;       // AssetDatabase.global.LoadSettings
    using Colossal.Logging;                // ILog, LogManager
    using Game;                            // UpdateSystem, SystemUpdatePhase
    using Game.Modding;                    // IMod
    using Game.SceneFlow;                  // GameManager

    public sealed class Mod : IMod
    {
        // --- Meta (adjust these for your mod) ---
        public const string Name = "Your Mod Name";
        public const string VersionShort = "0.1.0";

        // --- Logging ---
        // Log file: C:\Users\YourName\AppData\LocalLow\Colossal Order\Cities Skylines II\Logs\
        public static readonly ILog s_Log =
            LogManager.GetLogger(Name).SetShowsErrorsInUI(false);

        // --- Settings instance (optional) ---
        public static Setting? s_Settings { get; private set; }

        public void OnLoad(UpdateSystem updateSystem)
        {
            s_Log.Info($"{Name} {VersionShort} - OnLoad. If you can read this, your log pipeline works.");

            // Settings: create + load saved values, then register in Options UI
            var settings = new Setting(this);
            s_Settings = settings;
            AssetDatabase.global.LoadSettings(Name, settings, new Setting(this));
            settings.RegisterInOptionsUI();

            // Locales (optional): add before Options UI if you localize labels/descriptions
            TryAddLocale("en-US", new LocaleEN(settings));

            // Register systems (choose a phase that fits your work)
            // Example: a system that drives UI changes:
            // updateSystem.UpdateAt<ExampleSystem>(SystemUpdatePhase.UIUpdate);

            // Example: a system that runs in the main simulation loop:
            // updateSystem.UpdateAt<ExampleSystem>(SystemUpdatePhase.MainLoop);

            s_Log.Info($"{Name} initialized.");
        }

        public void OnDispose()
        {
            s_Log.Info($"{Name} - OnDispose");

            // If you subscribed to events, unsubscribe here.
            // Do NOT remove localization sources; CS2 manages them.

            if (s_Settings != null)
            {
                s_Settings.UnregisterInOptionsUI();
                s_Settings = null;
            }
        }

        private static void TryAddLocale(string localeId, IDictionarySource source)
        {
            var lm = GameManager.instance?.localizationManager;
            if (lm == null)
            {
                s_Log.Warn($"LocalizationManager null; cannot add locale '{localeId}'.");
                return;
            }
            lm.AddSource(localeId, source);
        }
    }
}
