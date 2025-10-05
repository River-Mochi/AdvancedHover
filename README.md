# CS2-Templates
CS2 templates: basic files to start a new mod

## Your Mod Name

Starter template for a Cities: Skylines II mod using the built-in Modding Toolchain.

- Clean `Mod.cs` entry point
- Optional `GatedSystemBase` for **gameplay gating** + **one-shot after-load hooks**
- `Settings/Setting.cs` wired for Options UI
- `Locale/LocaleEN.cs` ready for localization
- Example system under `Systems/ExampleSystem.cs`
- Project style aligned with Colossal conventions (`.editorconfig`)

## Requirements

- Visual Studio 2022 (or Rider)
- Cities: Skylines II installed
- **Modding Toolchain** installed (Colossal)
- Environment variable **`CSII_TOOLPATH`** pointing to the Toolchain folder  
  (the `.csproj` file imports `Mod.props` / `Mod.targets` from there).

> If you don’t have the toolchain, install it via the in-game Options > Mods before doing anything else.

## Folder Layout

```text
YourMod/
├─ YourMod.csproj
├─ Mod.cs
├─ GatedSystem.cs             # optional helper (gameplay gating + one-shot hook)
├─ Systems/
│  └─ ExampleSystem.cs            # sample system that inherits the helper
├─ Settings/
│  └─ Setting.cs
├─ Locale/
│  └─ LocaleEN.cs
├─ .editorconfig
├─ .gitattributes
├─ .gitignore
├─ LICENSE
└─ README.md
```

> For small mods, keeping it flat like this is easiest. If your project grows, feel free to add `/docs`, `/tools`, etc.


## Getting Started

1. **Clone** this repo (or "Use this template" on GitHub).
2. **Rename** the namespace `YourMod` to your mod's namespace (global Search & Replace).
3. Update `Mod.cs`:
   - `Name` and `VersionShort`
   - (Optional) enable any systems with `updateSystem.UpdateAt<YourSystem>(SystemUpdatePhase.X)`
4. Update `Settings/Setting.cs`:
   - Add your checkboxes, dropdowns, buttons (use the attributes shown in the file).
5. Update `Locale/LocaleEN.cs`:
   - Provide labels/descriptions for your options using the built-in localization keys.
6. **Build** the solution:
   - If `CSII_TOOLPATH` is set correctly, the toolchain will handle references & publishing.
7. **Launch the game**, enable your mod in **Content Manager**, and open **Options** to see your UI.

Register it in `Mod.OnLoad`:
`updateSystem.UpdateAt<YourMod.Systems.ExampleSystem>(SystemUpdatePhase.MainLoop);`

Localization

This template registers LocaleEN.cs (English). Add more locales the same way:
Example, in Mod.cs `OnLoad`
`TryAddLocale("fr-FR", new LocaleFR(s_Settings));`

In LocaleEN.cs: write locale strings using the Setting helper methods, e.g.:
```csharp
{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFeature)), "Enable Feature" },
{ m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFeature)),  "Turns the feature on or off." },
```

## Example: A System That only runs in active Gameplay

The template includes `GatedSystemBase.cs` helper. Inherit from it to restrict a system to gameplay and (optionally) do something once after a city loads:

```csharp
namespace YourMod.Systems
{
    using Game;
    using Unity.Entities;

    public sealed class ExampleSystem : GatedSystemBase
    {
        // Default kAllowedModes is Game only. Override if needed:
        // protected override GameMode kAllowedModes => GameMode.Game | GameMode.MapEditor;

        protected override void OnUpdate()
        {
            if (!Enabled) return;
            // per-frame work…
        }

        // Optional: run once when the city is ready after loading.
        // protected override void OnAfterLoadOnce()
        // {
        //     // one-shot setup…
        // }
    }
}
```

## “One-shot after load” Helper

Think of two kinds of code:
- Every-frame code: runs constantly while you’re in a city (like Unity’s Update()).
- One-time code: runs once right after a city finishes loading, then stops.
    - Get this by overriding OnGameLoadingComplete().

Timeline (mental model)
Mod that runs one-time after load.

1. **Mod.OnLoad** — your mod is loaded in the menu (no city yet).
2. **OnGamePreload** — loading screen (stuff is still spawning).
3. **OnGameLoadingComplete** — city ready → run your one-shot helper setup.
4. **OnUpdate** — ticks every frame while playing.

That’s all “one-shot after load” means: a safe place to run setup exactly once per city load, instead of doing it repeatedly in the per-frame loop.


### Style & Conventions
- Private instance fields: `m_FieldName`
- Private static fields: `s_FieldName`
- Constants: `kName`
- Put `using` directives inside the namespace block (optional, but many Colossal game files do this).
- .editorconfig enforces these naming rules (remove or change it if you like different styles)

### Troubleshooting

- Toolchain not found: Check csproj file, Ensure `CSII_TOOLPATH` points to the Modding Toolchain directory.
- Options not showing: Verify Setting.cs `RegisterInOptionsUI()` is called and the mod is enabled in-game.
- Locales not applied: in Mod.cs, make sure you called `TryAddLocale(localeId, source)` **before** `RegisterInOptionsUI()`.


### License
MIT — see [LICENSE](https://github.com/River-Mochi/CS2-Templates/blob/main/LICENSE) file. Add your name to the year you started contributing. MIT License means this is free to use, just keep copy of the license file in every fork of this mod.
Either keep the license file as it is written or add your name to it like below (in your own fork of this repo). 

Example at the top of the License file:

``` MIT License
Copyright (c) 2025 RiverMochi
Copyright (c) 2025 YourName (modifications)     

Permission is hereby granted, free of charge, to any person obtaining a copy...
```
- Adding your username (or name) is optional but you have to keep the rest of the License file as is with each fork of this repo per MIT.


----
1. Clone & open in VS2022

  VS2022: File → Clone Repository… → paste your repo URL → Clone.


2. Open the project
In VS: open YourMod/YourMod.csproj (there’s no .sln on purpose; a single-project csproj is fine).

One-time setup (toolchain)
Install the CS2 Modding Toolchain (if you haven’t yet) and set the user environment variable:
Variable name: CSII_TOOLPATH
Value: the folder where the Toolchain put Mod.props and Mod.targets (e.g. C:\Colossal\CS2\ModdingToolchain)

3. Restart VS after setting the variable.

    First Build
    Restore & build

    VS will auto-restore. Build with Ctrl+Shift+B.

You should see the game/Unity references resolve via Mod.props/Mod.targets.

Rename it to your mod

Project name & namespace

In Solution Explorer, rename folder YourMod to your real mod name (optional).

Right-click the project → Edit Project File:

Change <AssemblyName> and <RootNamespace> from YourMod to your namespace (if you want the defaults updated).

In any source file, place cursor on namespace YourMod → Ctrl+R, R (Rename) → enter your namespace → Apply to entire solution.

Update mod metadata

In Mod.cs: set Name, VersionShort, and your logger name string.

In Setting.cs: keep or remove sample UI you don’t need.

In Locale/LocaleEN.cs: update strings (labels/descriptions); the attributes in Setting.cs refer to these.

Enable your systems

Wire systems into the update loop
In Mod.cs → OnLoad, enable systems you want:

updateSystem.UpdateAt<ExampleSystem>(SystemUpdatePhase.UIUpdate);


(Keep using the gated system base so it only runs during gameplay.)

Build artifacts

Build / Publish

A normal Build produces the DLL in bin\<Config>\.

If you later add a Publish Profile (like in your other repo), you can publish a ready-to-drop package. The Modding toolchain also supports pack targets; you can add those later.

if you hit anything odd after cloning (missing refs, build fails), the usual culprits are:

CSII_TOOLPATH not set or pointing at the wrong folder.

VS not restarted after adding the environment variable.
