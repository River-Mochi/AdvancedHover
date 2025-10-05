namespace YourMod
{
    using Colossal;                   // IDictionarySource
    using System.Collections.Generic; // Dictionary

    /// <summary>English locale entries for Options UI.</summary>
    public sealed class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;
        public LocaleEN(Setting setting) { m_Setting = setting; }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                // Mod name in Options list
                { m_Setting.GetSettingsLocaleID(), Mod.Name },

                // Tabs/Groups
                { m_Setting.GetOptionTabLocaleID(Setting.MainTab), "Main" },
                { m_Setting.GetOptionGroupLocaleID(Setting.MainGroup), "Info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.LinksGroup), "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.NotesGroup), "Notes" },

                // Displays
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NameDisplay)), "Mod Name" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NameDisplay)),  "Display name of this mod." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionDisplay)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionDisplay)),  "Current mod version." },

                // Toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ExampleToggle)), "Enable Example Feature" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ExampleToggle)),  "When enabled, the example feature is active." },

                // Dropdown
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ExampleChoice)), "Example Choice" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ExampleChoice)),  "Select an option used by the example feature." },

                // Buttons (labels only; buttons don’t show a description panel)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenGitHub)),  "GitHub" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenDiscordButton)), "Come say hi. We don’t bite (usually)." },

                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenDocsButton)), "Docs! Because trial & error gets old fast." },

                // Notes block text
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MainNotes)), "You made it to the Options UI !!@@" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.MainNotes)), "This is where your settings live. Toggle things, click buttons, break nothing (hopefully)." },

            };
        }

        public void Unload() { }
    }

    /// <summary>Helper to add all supported locales.</summary>
    internal static class Locale
    {
        public static void AddAll(Setting setting)
        {
            var lm = Game.SceneFlow.GameManager.instance?.localizationManager;
            if (lm == null) { Mod.s_Log.Warn("No LocalizationManager; skipping locale add."); return; }

            lm.AddSource("en-US", new LocaleEN(setting));
            // Add more locales here later (fr-FR, de-DE, …)
        }
    }
}
