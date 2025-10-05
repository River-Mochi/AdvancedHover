# ⚡ Quick Start — CS2 Modding by RiverMochi

Welcome! This is a tiny starter template for Cities: Skylines II mods. It opens in **Visual Studio 2022**, builds a `.dll`, and shows a **sample Options UI** (checkbox, dropdown, buttons, friendly text).

Need help? **Discord:** https://discord.gg/HTav7ARPs2

---

## 1) Prereqs (once)
a. Windows + free Visual Studio 2022
b. Assumes you downloaded all modding toolsets from in-game Options → Mods (this takes a bit time)
c. CS2 Modding Toolchain gets installed as part of "b." (sets the `CSII_TOOLPATH` user env var).  
  If you’ve already built other CS2 mods, you’re probably set.

> The template uses `Mod.props` / `Mod.targets` from the toolchain via `$(CSII_TOOLPATH)`.
Critical that your *.csproj file points to them.

---

## 2) Get the template
- **Clone** this repo (or download ZIP) into a folder without spaces if possible.
- Open the **solution**: `YourMod/YourMod.sln`

> You can move the folder anywhere; the `.csproj` references come from the toolchain and your game install, not your repo layout.

---

## 3) Rename it (2-minute makeover)
Pick a mod name, for example: **MyCoolMod**

- **Folder:** Rename `YourMod` → `MyCoolMod`
- **Project:** Open `MyCoolMod.csproj` and change:
  - `<AssemblyName>MyCoolMod</AssemblyName>` *(optional)*
  - `<RootNamespace>MyCoolMod</RootNamespace>`
- **Namespaces:** Do a solution-wide rename of namespace **YourMod** → **MyCoolMod**
- **Settings file:** In `Setting.cs`, update `[FileLocation("ModsSettings/MyCoolMod/MyCoolMod")]`
- **Mod display name:** In `Mod.cs`, change `Name` and `VersionShort`

That’s it. You now have a uniquely named mod.

---

## 4) Build it
- Set build config to **Release**
- Build the solution
- Output is copied to:  
  `%LOCALAPPDATA%\Colossal Order\Cities Skylines II\Mods\MyCoolMod\MyCoolMod.dll`

(If not, check your `CSII_TOOLPATH` and the `Mod.props/targets` links inside the `.csproj`.)

---

## 5) Try it in-game
- Launch CS2
- Go to **Options → Mods → MyCoolMod**
- You should see:
  - A **checkbox** that toggles a value
  - A **dropdown**
  - A couple **buttons** (one opens a link)
  - A **friendly “you made it!”** message on the right panel

> If you see the UI, congrats—your pipeline works! 🎉

---

## 6) Where to code things
- **Mod.cs** — entry point. Make settings, add locales, register your systems.
- **Setting.cs** — the Options UI and persistent values.
- **Locale/LocaleEN.cs** — strings for labels & tooltips.
- **Systems/ExampleSystem.cs** — a sample DOTS system with **gameplay gating** and a one‑shot **after load** hook.

> Want your logic to run only in gameplay (not in editor or menu)? See the `OnGamePreload` gate in `ExampleSystem.cs`.

---

## 7) Shipping / Publishing
- Keep **LICENSE** (MIT is already set, update copyright year/name)
- Write a chill **README** (see repo)
- Publish to Paradox Mods using Visual Studio 2022, just right click on Publishconfiguration.xml

---

## 8) Troubleshooting
- **No Options UI?** Build errors, missing toolchain, or namespace mismatches are the usual suspects.
- **Log file:** `%LOCALAPPDATA%\Colossal Order\Cities Skylines II\logs\MyCoolMod.log`
- **Still stuck?** Discord: https://discord.gg/HTav7ARPs2

Happy modding! 😊
