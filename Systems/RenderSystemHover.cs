// Systems/RenderSystemHover.cs
// Advanced Hover — apply hover outline color (probe many likely properties + periodic re-assert)

namespace AdvancedHoverSystem
{
    using Game;                // GameSystemBase
    using Unity.Entities;      // ECS
    using UnityEngine;         // Material, Shader, Color, Resources

    public partial class RenderSystemHover : GameSystemBase
    {
        // Common IDs we’ll try first.
        private static readonly int s_ColorId = Shader.PropertyToID("_Color");
        private static readonly int s_BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int s_TintId = Shader.PropertyToID("_Tint");
        private static readonly int s_OutlineId = Shader.PropertyToID("_OutlineColor");
        private static readonly int s_LineColorId = Shader.PropertyToID("_LineColor");

        private static Material[]? s_CachedCandidates;
        private static Color s_LastColor;
        private static bool s_LastShow;
        private static string s_LastPresetName = "Unknown";
        private static bool s_HaveLast;

        private int m_Ticks;

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            if (!s_HaveLast)
                return;

            // Every ~0.5s, re-apply color to counter internal resets.
            if (++m_Ticks % 30 != 0)
                return;

            ApplyInternal(s_LastColor, s_LastShow, s_LastPresetName, "Periodic");
        }

        public static Color ResolvePresetColor(int preset)
        {
            return preset switch
            {
                0 => new Color(0.420f, 0.420f, 0.420f, 0.25f), // Medium Gray
                1 => new Color(0.400f, 0.200f, 0.550f, 0.25f), // Muted Purple
                2 => GetGameDefault(),
                _ => new Color(0.420f, 0.420f, 0.420f, 0.25f),
            };
        }

        public static Color GetGameDefault() => new Color(0f, 1f, 1f, 0.35f);

        public static void ApplyHoverColor(Color color, bool show, string presetName)
        {
            ApplyInternal(color, show, presetName, "Apply");
        }

        private static void ApplyInternal(Color color, bool show, string presetName, string reason)
        {
            var mats = GetHoverCandidates();
            int changedProps = 0;

            if (mats != null)
            {
                foreach (var mat in mats)
                {
                    if (mat == null)
                        continue;

                    // Primary guesses
                    if (mat.HasProperty(s_BaseColorId))
                    {
                        mat.SetColor(s_BaseColorId, color);
                        changedProps++;
                    }
                    if (mat.HasProperty(s_ColorId))
                    {
                        mat.SetColor(s_ColorId, color);
                        changedProps++;
                    }
                    if (mat.HasProperty(s_TintId))
                    {
                        mat.SetColor(s_TintId, color);
                        changedProps++;
                    }
                    if (mat.HasProperty(s_OutlineId))
                    {
                        mat.SetColor(s_OutlineId, color);
                        changedProps++;
                    }
                    if (mat.HasProperty(s_LineColorId))
                    {
                        mat.SetColor(s_LineColorId, color);
                        changedProps++;
                    }

                    // Generic property pass: enumerate shader properties and set plausible ones.
                    var shader = mat.shader;
                    if (shader != null)
                    {
                        int count = shader.GetPropertyCount();
                        for (int i = 0; i < count; i++)
                        {
                            var type = shader.GetPropertyType(i);
                            if (type != UnityEngine.Rendering.ShaderPropertyType.Color &&
                                type != UnityEngine.Rendering.ShaderPropertyType.Vector)
                                continue;

                            string pname = shader.GetPropertyName(i);
                            if (string.IsNullOrEmpty(pname))
                                continue;

                            // Heuristic: names that clearly relate to outline/selection/tint/color/edge/line/hover
                            string lower = pname.ToLowerInvariant();
                            if (lower.Contains("outline") || lower.Contains("select") || lower.Contains("hover") ||
                                lower.Contains("tint") || lower.Contains("color") || lower.Contains("edge") ||
                                lower.Contains("line"))
                            {
                                int id = Shader.PropertyToID(pname);
                                mat.SetColor(id, color);
                                changedProps++;
                            }
                        }
                    }
                }
            }

            // Also blast some common globals (harmless if unused).
            Shader.SetGlobalColor("_OutlineColor", color);
            Shader.SetGlobalColor("_SelectionColor", color);
            Shader.SetGlobalColor("_HoverColor", color);
            Shader.SetGlobalColor("_Tint", color);
            Shader.SetGlobalColor("_Color", color);
            Shader.SetGlobalColor("_BaseColor", color);
            Shader.SetGlobalColor("_LineColor", color);

            s_LastColor = color;
            s_LastShow = show;
            s_LastPresetName = presetName;
            s_HaveLast = true;

            Mod.s_Log.Info($"[Hover] {reason} '{presetName}' RGBA=({color.r:0.##},{color.g:0.##},{color.b:0.##},{color.a:0.##}) set {changedProps} property value(s)");

            if (Mod.Settings?.VerboseLogging == true)
            {
                MaterialSniffer.LogSnapshot("After.ApplyHoverColor");
            }
        }

        private static Material[]? GetHoverCandidates()
        {
            if (s_CachedCandidates != null)
                return s_CachedCandidates;

            var all = Resources.FindObjectsOfTypeAll<Material>();
            var list = new System.Collections.Generic.List<Material>();
            foreach (var mat in all)
            {
                if (mat == null)
                    continue;

                var n = mat.name ?? string.Empty;
                if (n.Length == 0)
                    continue;

                // Seen most often: "OutlinesCompose". Keep wide, but specific, net.
                if (n.IndexOf("OutlinesCompose", System.StringComparison.OrdinalIgnoreCase) >= 0
                    || n.IndexOf("Outline", System.StringComparison.OrdinalIgnoreCase) >= 0
                    || n.IndexOf("Hover", System.StringComparison.OrdinalIgnoreCase) >= 0
                    || n.IndexOf("Selection", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    list.Add(mat);
                }
            }

            s_CachedCandidates = list.Count > 0 ? list.ToArray() : System.Array.Empty<Material>();
            return s_CachedCandidates;
        }
    }
}
