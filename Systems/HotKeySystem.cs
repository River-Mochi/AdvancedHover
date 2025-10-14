// Systems/HotKeySystem.cs
// Advanced Hover — central F8 (rebindable) toggle logic

namespace AdvancedHoverSystem
{
    using System;
    using Game.Input;                    // ProxyAction
    using UnityEngine;                   // Color
    using UnityEngine.InputSystem;       // InputActionPhase

    public static class HotKeySystem
    {
        private static ProxyAction? s_ToggleAction;
        private static Action<ProxyAction, InputActionPhase>? s_Handler;

        public static void Initialize(Setting settings)
        {
            s_ToggleAction = settings.GetAction(Mod.kToggleOverlayActionName);
            if (s_ToggleAction == null)
                return;

            s_Handler = (action, phase) =>
            {
                if (phase != InputActionPhase.Performed)
                    return;

                // Flip checkbox and persist so the Options UI reflects the state.
                settings.DisableHoverOutline = !settings.DisableHoverOutline;
                settings.ApplyAndSave();

                bool show = !settings.DisableHoverOutline;
                int preset = settings.HoverPresetIndex;
                string name = settings.GetPresetDisplayName(preset);

                Color c = show
                    ? RenderSystemHover.ResolvePresetColor(preset)
                    : new Color(0f, 0f, 0f, 0f);

                RenderSystemHover.ApplyHoverColor(c, show, name);

                Mod.s_Log.Info($"[Hotkey] ToggleOverlay → {(show ? $"Preset '{name}'" : "Hidden")} RGBA=({c.r:0.##},{c.g:0.##},{c.b:0.##},{c.a:0.##})");
            };

            s_ToggleAction.onInteraction += s_Handler;

            // One initial application (in case OnLoad didn’t just apply)
            ApplyCurrent(settings);
        }

        public static void Dispose()
        {
            if (s_ToggleAction != null && s_Handler != null)
                s_ToggleAction.onInteraction -= s_Handler;

            s_Handler = null;
            s_ToggleAction = null;
        }

        public static void ApplyCurrent(Setting? settings)
        {
            if (settings == null)
                return;

            bool show = !settings.DisableHoverOutline;
            int preset = settings.HoverPresetIndex;
            string name = settings.GetPresetDisplayName(preset);

            var c = show ? RenderSystemHover.ResolvePresetColor(preset) : new Color(0f, 0f, 0f, 0f);
            RenderSystemHover.ApplyHoverColor(c, show, name);
        }
    }
}
