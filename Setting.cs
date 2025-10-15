// Setting.cs
// Advanced Hover — Options UI (Actions + About), hover color dropdown, F8 keybind,
// guideline translucency toggle, verbose logging

namespace AdvancedHoverSystem
{
    using System.Collections.Generic;
    using Colossal;                            // IDictionarySource
    using Colossal.IO.AssetDatabase;
    using Game.Input;                          // BindingKeyboard, ActionType, ProxyBinding
    using Game.Modding;                        // ModSetting, IMod
    using Game.Settings;                       // SettingsUI* attributes
    using Game.UI;                             // DropdownItem<T>
    using Game.UI.Localization;                // LocalizedString
    using Game.UI.Widgets;

    [FileLocation(nameof(AdvancedHoverSystem))]
    // Declare our button action so a binding can target it.
    [SettingsUIKeyboardAction(Mod.kToggleOverlayActionName, ActionType.Button, usages: new[] { Usages.kMenuUsage })]

    [SettingsUIGroupOrder(kGroupActionsMain, kGroupActionsKeybind, kGroupAboutInfo, kGroupAboutDebug)]
    [SettingsUIShowGroupName(kGroupActionsMain, kGroupActionsKeybind, kGroupAboutInfo, kGroupAboutDebug)]
    public class Setting : ModSetting
    {
        // Tabs
        public const string kTabActions = "Actions";
        public const string kTabAbout = "About";

        // Groups
        public const string kGroupActionsMain = "Main";
        public const string kGroupActionsKeybind = "Key bindings";
        public const string kGroupAboutInfo = "Information";
        public const string kGroupAboutDebug = "Debug";

        // Stored state
        private bool m_DisableHoverOutline;
        private int m_HoverPresetIndex = 0;     // 0 = Medium Gray (default)
        private bool m_TransparentGuidelines = true;

        public Setting(IMod mod) : base(mod) { }

        // ===== Actions / Main =====

        // Disable/Enable hover outline color entirely.
        [SettingsUISection(kTabActions, kGroupActionsMain)]
        public bool DisableHoverOutline
        {
            get => m_DisableHoverOutline;
            set
            {
                m_DisableHoverOutline = value;

                // Apply immediately so user sees the effect.
                bool show = !m_DisableHoverOutline;
                int preset = HoverPresetIndex;
                string name = GetPresetDisplayName(preset);

                var color = show
                    ? RenderSystemHover.ResolvePresetColor(preset)
                    : new UnityEngine.Color(0f, 0f, 0f, 0f);

                RenderSystemHover.ApplyHoverColor(color, show, name);
            }
        }

        // Hover color preset dropdown (0 = Medium Gray, 1 = Muted Purple, 2 = Game Default)
        [SettingsUIDropdown(typeof(Setting), nameof(GetHoverColorItems))]
        [SettingsUISection(kTabActions, kGroupActionsMain)]
        public int HoverPresetIndex
        {
            get => m_HoverPresetIndex;
            set
            {
                m_HoverPresetIndex = value;

                if (!DisableHoverOutline)
                {
                    var name = GetPresetDisplayName(value);
                    var c = RenderSystemHover.ResolvePresetColor(value);
                    RenderSystemHover.ApplyHoverColor(c, true, name);
                }
            }
        }

        public DropdownItem<int>[] GetHoverColorItems()
        {
            // Localized labels (LocaleEN provides AH.Color.* entries)
            return new[]
            {
                new DropdownItem<int> { value = 0, displayName = LocalizedString.Id("AH.Color.MediumGray")  },
                new DropdownItem<int> { value = 1, displayName = LocalizedString.Id("AH.Color.MutedPurple") },
                new DropdownItem<int> { value = 2, displayName = LocalizedString.Id("AH.Color.GameDefault") },
            };
        }

        // Enable translucent guidelines (toggle). Default ON.
        [SettingsUISection(kTabActions, kGroupActionsMain)] // ← fixed: use kTabActions, not kSectionActions
        public bool TransparentGuidelines
        {
            get => m_TransparentGuidelines;
            set
            {
                m_TransparentGuidelines = value;
                // Apply now in the running game
                RenderSystemGuidelines.RequestApplyFromSettings(value);
            }
        }

        // ===== Actions / Key bindings =====

        [SettingsUIKeyboardBinding(BindingKeyboard.F8, Mod.kToggleOverlayActionName)]
        [SettingsUISection(kTabActions, kGroupActionsKeybind)]
        public ProxyBinding ToggleOverlayBinding
        {
            get; set;
        }

        [SettingsUISection(kTabActions, kGroupActionsKeybind)]
        public bool ResetBindings
        {
            set
            {
                ResetKeyBindings();
            }
        }

        // ===== About =====

        [SettingsUISection(kTabAbout, kGroupAboutInfo)]
        public string NameDisplay => Mod.kName;

        [SettingsUISection(kTabAbout, kGroupAboutInfo)]
        public string VersionDisplay => Mod.kVersionShort;

        [SettingsUISection(kTabAbout, kGroupAboutDebug)]
        public bool VerboseLogging { get; set; } = true;

        // ===== Helpers =====

        public string GetPresetDisplayName(int preset)
        {
            switch (preset)
            {
                case 0:
                    return "Medium Gray (river style)";
                case 1:
                    return "Muted Purple (yen style)";
                case 2:
                    return "Game Default (cyan blue)";
                default:
                    return "Unknown";
            }
        }

        public override void SetDefaults()
        {
            m_DisableHoverOutline = false;
            m_HoverPresetIndex = 0; // Medium Gray default
            m_TransparentGuidelines = true;
            VerboseLogging = true;
        }
    }
}
