// Setting.cs
namespace AdvancedHoverSystem
{
    using Colossal.IO.AssetDatabase;  // [FileLocation]
    using Game.Modding;               // IMod
    using Game.Settings;              // ModSetting + [SettingsUI*]

    // Dropdown enum for hover outline color
    public enum HoverColorPreset
    {
        MediumGray = 0,   // default
        Purple = 1,
        Green = 2,
        MutedWhite = 3,
        Tan = 4,
        Vanilla = 5       // game's built-in cyan-like; we don't override
    }

    [FileLocation("ModsSettings/AdvancedHover/AdvancedHover")]
    [SettingsUITabOrder(MainTab)]
    [SettingsUIGroupOrder(MainGroup, GuideGroup)]
    [SettingsUIShowGroupName(MainGroup, GuideGroup)]
    public sealed class Setting : ModSetting
    {
        public const string MainTab = "Main";
        public const string MainGroup = "HoverOutline";
        public const string GuideGroup = "Guidelines";

        public Setting(IMod mod) : base(mod) { }

        // Checkbox: Disable vanilla gizmo overlay (hover outline)
        [SettingsUISection(MainTab, MainGroup)]
        public bool DisableHoverOutline { get; set; } = false;

        // Dropdown: enum => auto dropdown
        [SettingsUISection(MainTab, MainGroup)]
        public HoverColorPreset HoverColor { get; set; } = HoverColorPreset.MediumGray;

        // Checkbox: Transparent guidelines (alpha scaling, yenyang-style)
        [SettingsUISection(MainTab, GuideGroup)]
        public bool TransparentGuidelines { get; set; } = false;

        public override void SetDefaults()
        {
            DisableHoverOutline = false;
            HoverColor = HoverColorPreset.MediumGray; // requested default
            TransparentGuidelines = false;            // vanilla guideline palette by default
        }
    }
}
