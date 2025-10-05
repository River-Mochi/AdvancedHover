# CS2 Mod Template — Super Simple Starter

This is a tiny, opinionated starter for **Cities: Skylines II** mods.
It compiles cleanly, shows a **sample Options UI** (checkbox, dropdown, buttons, friendly text).
And has an optional helper file that will wire in an **ECS DOTS system** with a gameplay gate and a one‑shot “after load” hook.

**Support / feedback:** https://discord.gg/HTav7ARPs2

---

## What you get
- **Mod.cs** entry that sets up logging, settings, and a test system.
- **Settings UI** with real Game.Settings attributes (checkbox, dropdown, buttons, multiline text).
- **LocaleEN.cs** so you keep strings out of code.
- **ExampleSystem.cs** with a minimal **Unity.Entities GameSystemBase** pattern:
  - Gate to **GameMode.Game** only
  - Optional **ApplyOnceAfterLoad()** example
- **.editorconfig** and **.gitignore** tuned for CS2 & VS2022.
- **MIT license** ready.

---

## Folder Tree
```text
YourMod/
├─ Locale/
│  └─ LocaleEN.cs
├─ Settings/
│  └─ Setting.cs
├─ Systems/
│  └─ ExampleSystem.cs
├─ Mod.cs
├─ YourMod.csproj
├─ README.md
├─ .editorconfig
├─ .gitignore
└─ .gitattributes
```

---

## Rename it (fast)
1. Rename the folder `YourMod` → `MyCoolMod`
2. Edit `YourMod.csproj` → set `<RootNamespace>MyCoolMod</RootNamespace>`
3. Replace namespace **YourMod** → **MyCoolMod** across the solution
4. Update `[FileLocation("ModsSettings/MyCoolMod/MyCoolMod")]` in `Setting.cs`
5. Update `Name` and `VersionShort` in `Mod.cs`

Build → copy happens via the CS2 toolchain (`CSII_TOOLPATH`).

---

## Build & test
- Build in **Release**
- Start CS2 → Options → Mods → *Your mod name*
- You should see a small UI with a checkbox, a dropdown, buttons, and a friendly message.
- Check the mod log here:
  `%LOCALAPPDATA%\Colossal Order\Cities Skylines II\logs\MyCoolMod.log`

---

## Make it yours
- Use **Settings.cs** to add more toggles/sliders/dropdowns.
- Put your logic in new **Systems/** classes.
- Add more **LocaleXX.cs** files for translations, then register them in `Mod.cs`.

> Tip: if you don't like usings **inside** namespaces, edit the .editorconfig

---

## License
MIT — do what you want, just keep the copyright header in the LICENSE file with the fork repo.

Have fun & share cool stuff with the community! 🎈
