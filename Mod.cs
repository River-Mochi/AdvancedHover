// Mod.cs
// Advanced Hover — Mod entry point: settings, locales, register keybindings, initialise systems

namespace AdvancedHoverSystem
{
    using Colossal;                            // IDictionarySource
    using Colossal.IO.AssetDatabase;           // AssetDatabase.global.LoadSettings
    using Colossal.Logging;                    // ILog, LogManager
    using Game;                                // UpdateSystem
    using Game.Modding;                        // IMod
    using Game.SceneFlow;                      // GameManager
    using UnityEngine;                         // Color

    public sealed class Mod : IMod
    {
        public const string kName = "Advanced Hover";
        public const string kVersionShort = "0.1.0";

        // Input action name referenced by Setting.cs
        public const string kToggleOverlayActionName = "ToggleOverlay";

        public static readonly ILog s_Log =
            LogManager.GetLogger("AdvancedHover").SetShowsErrorsInUI(false);

        public static Setting? Settings
        {
            get; private set;
        }

        public void OnLoad(UpdateSystem updateSystem)
        {
            s_Log.Info($"{kName} v{kVersionShort} OnLoad");

            if (GameManager.instance?.modManager != null &&
                GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
            {
                s_Log.Info($"Mod asset path: {asset.path}");
            }

            // Settings first (so locale can bind to it)
            var settings = new Setting(this);
            Settings = settings;

            // Locales
            TryAddLocale("en-US", new LocaleEN(settings));

            // Load + show in Options UI
            AssetDatabase.global.LoadSettings("AdvancedHover", settings, new Setting(this));
            settings.RegisterInOptionsUI();

            // Ensure actions exist for the attributes in Setting.cs
            settings.RegisterKeyBindings();

            // Apply initial UI state
            RenderSystemGuidelines.Configure(settings.EnableGuidelineTranslucency);

            // Apply initial hover color from dropdown (respect "DisableHoverOutline")
            {
                bool show = !settings.DisableHoverOutline;
                int preset = settings.HoverPresetIndex;
                string name = settings.GetPresetDisplayName(preset);
                Color c = show ? RenderSystemHover.ResolvePresetColor(preset) : new Color(0f, 0f, 0f, 0f);
                RenderSystemHover.ApplyHoverColor(c, show, name);
            }

            // Hook up F8 handler
            HotKeySystem.Initialize(settings);
        }

        public void OnDispose()
        {
            HotKeySystem.Dispose();

            if (Settings != null)
            {
                Settings.UnregisterInOptionsUI();
                Settings = null;
            }

            s_Log.Info("OnDispose");
        }

        private static void TryAddLocale(string localeId, IDictionarySource source)
        {
            var lm = GameManager.instance?.localizationManager;
            if (lm == null)
            {
                s_Log.Warn($"No LocalizationManager; cannot add locale '{localeId}'.");
                return;
            }
            lm.AddSource(localeId, source);
        }
    }
}
