// LocaleEN.cs
namespace AdvancedHoverSystem
{
    using System.Collections.Generic;
    using Colossal; // IDictionarySource

    /// <summary>English locale (en-US)</summary>
    public sealed class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;
        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                // Options menu entry
                { m_Setting.GetSettingsLocaleID(), "Advanced Hover" },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.MainTab), "Main" },

                // Groups (Main tab)
                { m_Setting.GetOptionGroupLocaleID(Setting.MainGroup), "Hover Outline" },
                { m_Setting.GetOptionGroupLocaleID(Setting.GuideGroup), "Guidelines" },

                // Main >> Hover Outline
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DisableHoverOutline)), "Disable hover outlines" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DisableHoverOutline)), "Suppress vanilla hover highlights around prefabs." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.HoverColor)), "Hover outline color" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.HoverColor)), "Re-tint the hover outline (alpha ignored by gizmo). 'Vanilla' = game default." },

                // Main >> Guidelines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TransparentGuidelines)), "Translucent guidelines" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TransparentGuidelines)), "Makes curb/placement guides semi-transparent like in yenyang’s screenshot." },

                // Enum values
                { m_Setting.GetEnumValueLocaleID(HoverColorPreset.MediumGray), "Medium Gray (default)" },
                { m_Setting.GetEnumValueLocaleID(HoverColorPreset.Purple),     "Purple" },
                { m_Setting.GetEnumValueLocaleID(HoverColorPreset.Green),      "Green" },
                { m_Setting.GetEnumValueLocaleID(HoverColorPreset.MutedWhite), "Muted White" },
                { m_Setting.GetEnumValueLocaleID(HoverColorPreset.Tan),        "Tan" },
                { m_Setting.GetEnumValueLocaleID(HoverColorPreset.Vanilla),    "Vanilla (game default)" },
            };
        }

        public void Unload()
        {
        }
    }
}
