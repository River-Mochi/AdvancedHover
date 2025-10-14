// LocaleEN.cs
// Advanced Hover — English locale entries (tabs, groups, labels, dropdown items)

namespace AdvancedHoverSystem
{
    using System.Collections.Generic;
    using Colossal; // IDictionarySource

    public sealed class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                // Settings root title
                { m_Setting.GetSettingsLocaleID(), "Advanced Hover" },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.kTabActions), "Actions" },
                { m_Setting.GetOptionTabLocaleID(Setting.kTabAbout),   "About"   },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.kGroupActionsMain),    "Main"         },
                { m_Setting.GetOptionGroupLocaleID(Setting.kGroupActionsKeybind), "Key bindings" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kGroupAboutInfo),      "Information"  },
                { m_Setting.GetOptionGroupLocaleID(Setting.kGroupAboutDebug),     "Debug"        },

                // Options — Actions/Main
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DisableHoverOutline)), "Disable hover outline color" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DisableHoverOutline)),  "When enabled, the hover outline color is hidden regardless of the dropdown selection." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.HoverPresetIndex)), "Hover outline color" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.HoverPresetIndex)),  "Choose a preset color for hovered prefabs." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableGuidelineTranslucency)), "Guidelines translucency" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableGuidelineTranslucency)),  "When enabled, guideline dashes render translucent." },

                // Options — Actions/Keybinds
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ToggleOverlayBinding)), "Toggle overlay (F8)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ToggleOverlayBinding)),  "Keyboard shortcut to toggle the hover outline on/off." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "Reset key bindings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)),  "Restore the default key bindings for this mod." },

                // Optional binding map/key strings (used by the keybinding UI)
                { m_Setting.GetBindingMapLocaleID(), "Advanced Hover — key bindings" },
                { m_Setting.GetBindingKeyLocaleID(Mod.kToggleOverlayActionName), "Toggle overlay" },

                // Options — About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NameDisplay)),    "Mod name" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionDisplay)), "Version"  },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VerboseLogging)), "Verbose logging" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VerboseLogging)),  "When enabled, writes extra diagnostics to the log." },

                // Dropdown item display names
                { "AH.Color.MediumGray",  "Medium Gray (river style)" },
                { "AH.Color.MutedPurple", "Muted Purple (yen style)"  },
                { "AH.Color.GameDefault", "Game Default (cyan blue)"  },
            };
        }

        public void Unload()
        {
        }
    }
}
