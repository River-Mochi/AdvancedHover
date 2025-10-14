// Patches/EditorGizmoAlphaUnclamp.cs
#if ADVHOVER_HARMONY
namespace AdvancedHoverSystem
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;

    [HarmonyPatch]
    internal static class EditorGizmoAlphaUnclamp
    {
        static MethodBase TargetMethod()
        {
            var t = AccessTools.TypeByName("Game.Rendering.EditorGizmoSystem");
            return AccessTools.Method(t, "OnUpdate");
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var list = new List<CodeInstruction>(instructions);
            var colorAField = AccessTools.Field(typeof(UnityEngine.Color), "a");

            int removed = 0;

            for (int i = 0; i < list.Count - 2; i++)
            {
                if (list[i + 1].opcode == OpCodes.Ldc_R4
                    && list[i + 1].operand is float f && System.Math.Abs(f - 1f) < 0.0001f
                    && list[i + 2].opcode == OpCodes.Stfld
                    && Equals(list[i + 2].operand, colorAField))
                {
                    list.RemoveAt(i + 2);
                    list.RemoveAt(i + 1);
                    removed++;
                }
            }

            Mod.s_Log.Info($"[Patch] EditorGizmoSystem.OnUpdate alpha-unclamp removed {removed} alpha clamps.");
            return list;
        }

        public static void Apply(Harmony harmony)
        {
            harmony.Patch(TargetMethod(), transpiler: new HarmonyMethod(typeof(EditorGizmoAlphaUnclamp), nameof(Transpiler)));
            Mod.s_Log.Info("[Patch] Applied EditorGizmoSystem alpha-unclamp.");
        }
    }
}
#endif
