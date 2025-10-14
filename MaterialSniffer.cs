// MaterialSniffer.cs
// Advanced Hover — verbose inspector for likely hover/guideline materials

namespace AdvancedHoverSystem
{
    using System.Collections.Generic;
    using Game;                // GameSystemBase
    using UnityEngine;

    public partial class MaterialSniffer : GameSystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }
        protected override void OnUpdate()
        { /* no-op */
        }

        public static void LogSnapshot(string tag, bool deepScanIfMissing = false)
        {
            if (Mod.Settings?.VerboseLogging != true)
                return;

            // Hover candidates
            var hover = FindMaterials(
                containsAny: new[] { "OutlinesCompose", "Outline", "Hover" });

            Mod.s_Log.Info($"[Sniffer:{tag}] HoverCandidates: found {hover.Count} material(s)");
            foreach (var m in hover)
            {
                LogMat(tag, "HoverCandidates", m);
            }

            // Guideline candidates
            var guide = FindMaterials(
                containsAny: new[] { "Guideline", "PlacementGuide", "EditorGizmoLine", "GridGuide" });

            Mod.s_Log.Info($"[Sniffer:{tag}] GuidelineCandidates: found {guide.Count} material(s)");
            foreach (var m in guide)
            {
                LogMat(tag, "GuidelineCandidates", m);
            }
        }

        private static void LogMat(string tag, string group, Material m)
        {
            if (m == null)
                return;

            string name = m.name ?? "-";
            string shader = m.shader != null ? m.shader.name : "-";

            // Probe common property names seen in CS2 shaders.
            string baseCol = GetColorString(m, "_BaseColor");
            string color = GetColorString(m, "_Color");
            string tint = GetColorString(m, "_Tint");
            string outline = GetColorString(m, "_OutlineColor");
            string line = GetColorString(m, "_LineColor");

            Mod.s_Log.Info($"[Sniffer:{tag}] {group}='{name}' shader='{shader}' base={baseCol} color={color} tint={tint} outline={outline} line={line}");
        }

        private static string GetColorString(Material m, string prop)
        {
            int id = Shader.PropertyToID(prop);
            if (m.HasProperty(id))
            {
                var c = m.GetColor(id);
                return $"({c.r:0.##},{c.g:0.##},{c.b:0.##},{c.a:0.##})";
            }
            return "-";
        }

        private static List<Material> FindMaterials(string[] containsAny)
        {
            var list = new List<Material>();
            var all = Resources.FindObjectsOfTypeAll<Material>();
            foreach (var mat in all)
            {
                if (mat == null)
                    continue;
                string n = mat.name ?? string.Empty;
                if (n.Length == 0)
                    continue;

                bool match = false;
                foreach (var needle in containsAny)
                {
                    if (!string.IsNullOrEmpty(needle) &&
                        n.IndexOf(needle, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        match = true;
                        break;
                    }
                }
                if (match)
                    list.Add(mat);
            }
            return list;
        }
    }
}
