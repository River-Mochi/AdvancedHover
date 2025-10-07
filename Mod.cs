// Mod.cs
namespace AdvancedHoverSystem
{
    using Colossal;
    using Colossal.IO.AssetDatabase;
    using Colossal.Logging;
    using Game;
    using Game.Modding;
    using Game.SceneFlow;

    public sealed class Mod : IMod
    {
        public const string Name = "Advanced Hover";
        public const string Version = "0.1.0";

        public static readonly ILog Log =
            LogManager.GetLogger("AdvancedHover").SetShowsErrorsInUI(false);

        internal static Setting? Settings { get; private set; }

        public void OnLoad(UpdateSystem updateSystem)
        {
            Log.Info($"[AHS] {Name} {Version} OnLoad");

            // Settings instance and persistence (path must match Setting.cs [FileLocation])
            var settings = new Setting(this);
            Settings = settings;
            AssetDatabase.global.LoadSettings(
                "ModsSettings/AdvancedHover/AdvancedHover",
                settings,
                new Setting(this)
            );

            // Locale BEFORE Options UI so labels resolve
            AddLocale("en-US", new LocaleEN(settings));

            // Register options
            settings.RegisterInOptionsUI();

            // --- Systems ---
            var world = updateSystem.World;

            // 1) Guidelines (yen translucent option) — one-shot
            world.GetOrCreateSystemManaged<RenderSystemGuidelines>().Enabled = true;

            // 2) Hover color (Gizmo alpha-locked; hue only) — one-shot
            world.GetOrCreateSystemManaged<RenderSystemHover>().Enabled = true;

            // 3) Runtime blocker for hover outlines (placeholder; safe no-op)
            world.GetOrCreateSystemManaged<BlockerSystemHighlights>().Enabled = true;
        }

        public void OnDispose()
        {
            Log.Info($"[AHS] {Name} {Version} OnDispose");
            Settings?.UnregisterInOptionsUI();
            Settings = null;
        }

        private static void AddLocale(string localeId, IDictionarySource source)
        {
            var lm = GameManager.instance?.localizationManager;
            if (lm == null)
            {
                Log.Warn("[AHS] LocalizationManager null; cannot add locale.");
                return;
            }
            lm.AddSource(localeId, source);
        }
    }
}
