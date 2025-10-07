// LocaleEN.cs
namespace AdvancedHoverSystem
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// English strings for Options UI (matches Setting.MainTab/MainGroup/GuideGroup).
    /// </summary>
    public sealed class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            // Tabs / Groups
            yield return new KeyValuePair<string, string>(Setting.MainTab, "Advanced Hover");
            yield return new KeyValuePair<string, string>(Setting.MainGroup, "Hover Outline");
            yield return new KeyValuePair<string, string>(Setting.GuideGroup, "Guidelines");

            // Property display names
            yield return new KeyValuePair<string, string>(nameof(Setting.DisableHoverOutline), "Disable hover outline");
            yield return new KeyValuePair<string, string>(nameof(Setting.HoverColor), "Hover outline color");
            yield return new KeyValuePair<string, string>(nameof(Setting.TransparentGuidelines), "Translucent guidelines (yen style)");

            // Descriptions
            yield return new KeyValuePair<string, string>($"{nameof(Setting.DisableHoverOutline)}.Description",
                "Attempt to suppress the vanilla hover outline (roads may still outline on name hover).");
            yield return new KeyValuePair<string, string>($"{nameof(Setting.HoverColor)}.Description",
                "Change the hover outline hue (alpha is fixed by the game).");
            yield return new KeyValuePair<string, string>($"{nameof(Setting.TransparentGuidelines)}.Description",
                "Use a softer, semi-transparent guideline palette for placement aids.");
        }

        public void Unload()
        {
        }
    }
}
