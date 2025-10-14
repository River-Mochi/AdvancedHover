// Systems/RenderSystemGuidelines.cs
// Advanced Hover — keep guideline dashes translucent (toggle-based)

namespace AdvancedHoverSystem
{
    using Game;
    using UnityEngine;

    public partial class RenderSystemGuidelines : GameSystemBase
    {
        private static readonly int s_ColorId = Shader.PropertyToID("_Color");
        private static readonly int s_BaseColorId = Shader.PropertyToID("_BaseColor");

        private static Material[]? s_GuidelineCandidates;
        private static bool s_Enabled;
        private static float s_Alpha = 0.35f;

        private int m_Ticks;

        protected override void OnUpdate()
        {
            if (!s_Enabled)
                return;

            // Every ~0.5s, re-assert alpha (tools/materials may be rebuilt)
            if (++m_Ticks % 30 != 0)
                return;

            ApplyAlphaToAll(s_Alpha, "Periodic");
        }

        public static void Configure(bool enabled)
        {
            s_Enabled = enabled;
            s_Alpha = enabled ? 0.35f : 1f;
            ApplyAlphaToAll(s_Alpha, "Configure");
        }

        private static void ApplyAlphaToAll(float alpha, string reason)
        {
            var mats = GetGuidelineCandidates();
            int changed = 0;

            foreach (var mat in mats)
            {
                if (mat == null)
                    continue;

                if (mat.HasProperty(s_ColorId))
                {
                    var c = mat.GetColor(s_ColorId);
                    if (Mathf.Abs(c.a - alpha) > 0.01f)
                    {
                        c.a = alpha;
                        mat.SetColor(s_ColorId, c);
                        changed++;
                    }
                }
                if (mat.HasProperty(s_BaseColorId))
                {
                    var c = mat.GetColor(s_BaseColorId);
                    if (Mathf.Abs(c.a - alpha) > 0.01f)
                    {
                        c.a = alpha;
                        mat.SetColor(s_BaseColorId, c);
                        changed++;
                    }
                }
            }

            if (Mod.Settings?.VerboseLogging == true)
            {
                Mod.s_Log.Info($"[Guidelines] {reason} alpha={alpha:0.##} on {changed} material value(s)");
            }
        }

        private static Material[] GetGuidelineCandidates()
        {
            if (s_GuidelineCandidates != null)
                return s_GuidelineCandidates;

            var all = Resources.FindObjectsOfTypeAll<Material>();
            var list = new System.Collections.Generic.List<Material>();
            foreach (var mat in all)
            {
                if (mat == null)
                    continue;
                string n = mat.name ?? string.Empty;
                if (n.Length == 0)
                    continue;

                if (n.IndexOf("Guideline", System.StringComparison.OrdinalIgnoreCase) >= 0
                    || n.IndexOf("PlacementGuide", System.StringComparison.OrdinalIgnoreCase) >= 0
                    || n.IndexOf("EditorGizmoLine", System.StringComparison.OrdinalIgnoreCase) >= 0
                    || n.IndexOf("GridGuide", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    list.Add(mat);
                }
            }
            s_GuidelineCandidates = list.Count > 0 ? list.ToArray() : System.Array.Empty<Material>();
            return s_GuidelineCandidates;
        }
    }
}
