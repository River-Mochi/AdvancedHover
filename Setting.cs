// Setting.cs
namespace AdvancedHoverSystem
{
    using Colossal.IO.AssetDatabase;   // [FileLocation]
    using Game.Modding;                // IMod
    using Game.Settings;               // ModSetting + [SettingsUI*]
    using Game.UI.Widgets;             // DropdownItem<T>
    using System;

    public enum HoverColorPreset
    {
        Vanilla = 0,
        White = 1,
        Gray = 2,
        Green = 3,
        Purple = 4,
        Beige = 5,
    }

    [FileLocation("ModsSettings/AdvancedHover/AdvancedHover")]
    [SettingsUITabOrder(MainTab, AboutTab)]
    [SettingsUIGroupOrder(MainGroup, GuideGroup)]
    [SettingsUIShowGroupName(MainGroup, GuideGroup)]
    public sealed class Setting : ModSetting
    {
        public const string MainTab = "Main";
        public const string AboutTab = "About";

        public const string MainGroup = "HoverOutline";
        public const string GuideGroup = "Guidelines";

        // ---- Brightness mapping (bias toward darker by default) ----
        internal const float BrightnessMinFactor = 0.25f; // very dim
        internal const float BrightnessMaxFactor = 1.25f; // brighter
        internal const float BrightnessStep = 0.05f; // visual steps ~5%

        internal const float OpacityStep = 0.05f; // for guideline transparency

        public Setting(IMod mod) : base(mod) { }

        // ---------------- Hover outline options ----------------
        [SettingsUISection(MainTab, MainGroup)]
        public bool DisableHoverOutline { get; set; } = false;

        // Dropdown: color preset
        [SettingsUISection(MainTab, MainGroup)]
        [SettingsUIDropdown(typeof(Setting), nameof(GetHoverColorChoices))]
        public HoverColorPreset HoverColor { get; set; } = HoverColorPreset.Vanilla;

        // Slider (SDK-safe): normalized 0..1; we map to 0.25..1.25 internally.
        [SettingsUISection(MainTab, MainGroup)]
        [SettingsUISlider] // parameterless; range/step handled by setter & mapping
        public float HoverBrightness
        {
            get => m_HoverBrightness;
            set => m_HoverBrightness = Quantize01(value, BrightnessStep);
        }
        private float m_HoverBrightness = 0.70f; // default dimmer than preset

        public float GetHoverBrightnessFactor()
        {
            return BrightnessMinFactor + (BrightnessMaxFactor - BrightnessMinFactor) * Clamp01(m_HoverBrightness);
        }

        // ---------------- Guidelines options ----------------
        [SettingsUISection(MainTab, GuideGroup)]
        public bool TransparentGuidelines { get; set; } = false;

        // Slider: guideline opacity (0..1). We quantize to 5% steps for a nicer feel.
        [SettingsUISection(MainTab, GuideGroup)]
        [SettingsUISlider] // parameterless; we quantize in the setter
        public float GuidelineOpacity
        {
            get => m_GuideOpacity;
            set => m_GuideOpacity = Quantize01(value, OpacityStep);
        }
        private float m_GuideOpacity = 0.50f;

        // ---------------- Dropdown items provider ----------------
        public static DropdownItem<HoverColorPreset>[] GetHoverColorChoices() =>
            new[]
            {
                new DropdownItem<HoverColorPreset> { value = HoverColorPreset.Vanilla, displayName = "Vanilla" },
                new DropdownItem<HoverColorPreset> { value = HoverColorPreset.White,   displayName = "White"   },
                new DropdownItem<HoverColorPreset> { value = HoverColorPreset.Gray,    displayName = "Gray"    },
                new DropdownItem<HoverColorPreset> { value = HoverColorPreset.Green,   displayName = "Green"   },
                new DropdownItem<HoverColorPreset> { value = HoverColorPreset.Purple,  displayName = "Purple"  },
                new DropdownItem<HoverColorPreset> { value = HoverColorPreset.Beige,   displayName = "Beige/Tan"  },
            };

        public override void SetDefaults()
        {
            DisableHoverOutline = false;
            HoverColor = HoverColorPreset.Vanilla;
            m_HoverBrightness = 0.70f;
            TransparentGuidelines = false;
            m_GuideOpacity = 0.50f;
        }

        // ---------------- helpers ----------------
        private static float Clamp01(float v) => (v < 0f) ? 0f : ((v > 1f) ? 1f : v);

        private static float Quantize01(float v, float step)
        {
            v = Clamp01(v);
            if (step <= 0f) return v;
            var q = Math.Round(v / step) * step;
            return (q < 0f) ? 0f : ((q > 1f) ? 1f : (float)q);
        }
    }
}
