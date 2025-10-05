namespace YourMod
{
    using Colossal.IO.AssetDatabase;  // [FileLocation]
    using Game.Modding;               // IMod
    using Game.Settings;              // ModSetting, [SettingsUI*]
    using Game.UI.Widgets;            // DropdownItem<T>
    using UnityEngine;                // Application.OpenURL
    using System;                     // Exception

    [FileLocation("ModsSettings/YourMod/YourMod")]
    [SettingsUIGroupOrder(MainGroup, LinksGroup, NotesGroup)]
    [SettingsUIShowGroupName(MainGroup, LinksGroup)] // omit Notes header
    public sealed class Setting : ModSetting
    {
        // ---- Tabs & Groups ----
        public const string MainTab   = "Main";
        public const string MainGroup = "Main";
        public const string LinksGroup = "Links";
        public const string NotesGroup = "Notes";

        public Setting(IMod mod) : base(mod) { }

        // ---- Main meta displays (labels come from Locale) ----
        [SettingsUISection(MainTab, MainGroup)]
        public string NameDisplay => Mod.Name;

        [SettingsUISection(MainTab, MainGroup)]
        public string VersionDisplay => Mod.VersionShort;

        // ---- Example checkbox (filter toggle) ----
        [SettingsUISection(MainTab, MainGroup)]
        public bool ExampleToggle { get; set; } = true;

        // ---- Example dropdown ----
        [SettingsUISection(MainTab, MainGroup)]
        [SettingsUIDropdown(typeof(Setting), nameof(GetExampleChoices))]
        public string ExampleChoice { get; set; } = "Default";

        public static DropdownItem<string>[] GetExampleChoices() => new[]
        {
            new DropdownItem<string> { value = "Default", displayName = "Default" },
            new DropdownItem<string> { value = "OptionA", displayName = "Option A" },
            new DropdownItem<string> { value = "OptionB", displayName = "Option B" },
        };

        // ---- Example URL buttons (share a row) ----
        [SettingsUIButtonGroup(LinksGroup)]
        [SettingsUIButton]
        [SettingsUISection(MainTab, LinksGroup)]
        public bool OpenGitHub
        {
            set
            {
                if (!value) return;
                try { Application.OpenURL("https://github.com/your/repo"); }
                catch (Exception ex) { Mod.s_Log.Warn($"OpenGitHub failed: {ex.Message}"); }
            }
        }

        [SettingsUIButtonGroup(LinksGroup)]
        [SettingsUIButton]
        [SettingsUISection(MainTab, LinksGroup)]
        public bool OpenDiscord
        {
            set
            {
                if (!value) return;
                try { Application.OpenURL("https://discord.gg/your-server"); }
                catch (Exception ex) { Mod.s_Log.Warn($"OpenDiscord failed: {ex.Message}"); }
            }
        }

        // ---- Notes / multiline text (content via Locale) ----
        [SettingsUIMultilineText]
        [SettingsUISection(MainTab, NotesGroup)]
        public string MainNotes => string.Empty;

        public override void SetDefaults()
        {
            ExampleToggle = true;
            ExampleChoice = "Default";
        }
    }
}
