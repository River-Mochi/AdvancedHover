// LocaleEN.cs
namespace AdvancedHoverSystem
{
    using Colossal;
    using System.Collections.Generic;

    public sealed class LocaleEN : IDictionarySource
    {
        private readonly Setting _s;
        public LocaleEN(Setting s) { _s = s; }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                // Mod title
                { _s.GetSettingsLocaleID(), "Advanced Hover" },

                // Tabs
                { _s.GetOptionTabLocaleID(Setting.MainTab),  "Main"  },
                { _s.GetOptionTabLocaleID(Setting.AboutTab), "About" },

                // Group
                { _s.GetOptionGroupLocaleID(Setting.MainGroup), "Hover Outline" },

                // Checkbox
                { _s.GetOptionLabelLocaleID(nameof(Setting.DisableHoverOutline)), "Disable hover outline" },
                { _s.GetOptionDescLocaleID(nameof(Setting.DisableHoverOutline)),  "Turns off the vanilla gizmo outline." },

                // Dropdown
                { _s.GetOptionLabelLocaleID(nameof(Setting.HoverColor)), "Hovered color preset" },
                { _s.GetOptionDescLocaleID(nameof(Setting.HoverColor)),  "Pick a color for the hover outline." },

                // Brightness
                { _s.GetOptionLabelLocaleID(nameof(Setting.HoverBrightness)), "Outline brightness" },
                { _s.GetOptionDescLocaleID(nameof(Setting.HoverBrightness)),  "Scales color intensity (0 = black, 1 = normal, 2 = bright)." },
            };
        }

        public void Unload() { }
    }
}
