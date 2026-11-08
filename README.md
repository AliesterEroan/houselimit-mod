# House Limit Mod

**A BepInEx plugin for Sun Haven that raises the hardcoded 8-house limit to any configured value (default: 999).**

> Sun Haven is a fantasy farming sim by Pixel Sprout Studios — solo or 8-player co-op, with farming, quests, and magic. This mod removes the restrictive cap on how many houses you can build.

---

## 📜 Table of Contents

- [✨ Features](#-features)
- [🚀 Installation](#-installation)
- [⚙️ Settings](#-settings)
- [🏗️ Architecture](#-architecture)
- [🛠️ Technologies Used](#-technologies-used)
- [⚠️ Limitations](#-limitations)
- [📜 License](#-license)
- [🙏 Acknowledgments](#-acknowledgments)
- [📧 Contact](#-contact)

---

## ✨ Features

- **Configurable house limit** - `Max Houses` setting (default 999), editable at runtime via the BepInEx config file
- **Runtime patching** - Harmony transpilers rewrite the hardcoded `8`/`7` constants at game startup; no game files are modified
- **Comprehensive coverage** - patches both `Wish.PlayerHouse` (limit property, meta, scene loading) and `Wish.HousePlaceable` (placement checks and slot assignment)
- **Safe & reversible** - delete the DLL to disable; existing saves keep working
- **Lightweight** - single-file plugin, negligible performance impact

---

## 🚀 Installation

### Prerequisites
- Sun Haven (Steam or GOG)
- [BepInEx 5.4](https://bepinex.com) already installed in the game directory (run the game once so it generates `BepInEx/`)
- A prebuilt `HouseLimitMod.dll` (in `release/` or the `HouseLimitMod-v1.0.0.zip` package)

### Steps
1. Copy `HouseLimitMod.dll` into:
   ```
   Sun Haven\BepInEx\plugins\HouseLimitMod.dll
   ```
2. Launch the game — the mod loads automatically. Check `BepInEx\LogOutput.log` for `House Limit Mod loaded successfully!`.
3. After the first run, edit `BepInEx\config\houselimit.cfg` to change the limit, then restart the game.

### Build from source
1. Clone the repository.
2. Install the .NET SDK (`winget install Microsoft.DotNet.SDK.8`).
3. Edit `HouseLimitMod.csproj` — the `<HintPath>` references point at a specific Sun Haven install (`D:\personal\games\...`); update them to your game directory's `BepInEx\core\` and `Sun Haven_Data\Managed\` folders.
4. Build:
   ```bash
   dotnet build
   ```
   The post-build step auto-copies the DLL into `BepInEx\plugins\`.

---

## ⚙️ Settings

`BepInEx\config\houselimit.cfg`:

- **`Max Houses`** - int (default: `999`) — maximum number of houses allowed. Houses are 0-indexed, so internal clamps use `Max Houses - 1`.

Suggested values: 50–100 for small towns, 100–500 for medium, 500–999 for large; 1000+ may cause performance issues.

---

## 🏗️ Architecture

```
├── HouseLimitMod.cs       # Plugin entry point + Harmony transpilers
├── HouseLimitMod.csproj   # net472 project; game DLL references + post-build copy
├── release/               # Prebuilt DLL + INSTALL.txt
└── HouseLimitMod-v1.0.0.zip
```

### Patched methods
| Method | Original constant | Patched to |
|---|---|---|
| `PlayerHouse.MaxNumber` getter | `8` | `Max Houses` |
| `PlayerHouse.SetMeta` | `7` | `Max Houses - 1` |
| `PlayerHouse.Build` | `7` | `Max Houses - 1` |
| `PlayerHouse.SceneToLoad` | `7` | `Max Houses - 1` |
| `PlayerHouse.GetHouseSceneIndex` | `7` | `Max Houses - 1` |
| `HousePlaceable.PlaceDecoration` | `8` | `Max Houses` |
| `HousePlaceable.Use1` | `8` | `Max Houses` |
| `HousePlaceable.ModifyMeta` | `8` | `Max Houses` |

### Data flow
1. BepInEx loads `HouseLimitMod.dll` → `Awake()` binds the config entry.
2. Harmony transpilers replace `ldc.i4.8` / `ldc.i4.7` opcodes with calls to `GetMaxHouses()` / `GetMaxHousesMinusOne()`.
3. All patched checks read the live config value at runtime.

---

## 🛠️ Technologies Used

### Core
- **C# / .NET Framework 4.7.2** - mod implementation (built with .NET SDK 8)
- **BepInEx 5.4** - Unity mod loader
- **Harmony (0Harmony)** - runtime IL patching via transpilers
- **Mono.Cecil / AccessTools** - type and method resolution

### Game references (not redistributed)
- `Assembly-CSharp.dll`, `UnityEngine*.dll` from the local Sun Haven install

---

## ⚠️ Limitations

- **Game version** - built and tested against Sun Haven v3.1.3b; a game update that changes the patched methods may break it (the plugin logs an error per missing method rather than crashing).
- **UI** - house numbers beyond the original limit may not display correctly.
- **Performance** - very high limits (1000+) can degrade large towns.
- **Multiplayer** - all players should run the same mod for consistent limits.
- **Testing** - verified by in-game use on the author's install; there is no automated test suite.

---

## 📜 License

No LICENSE file is present in this repository — **all rights reserved** by the author.

---

## 🙏 Acknowledgments

- **BepInEx team** — the modding framework this plugin runs on
- **Harmony team** — the runtime patching library
- **Pixelated Milk / Sun Haven team** — the game being modded
- **Sun Haven modding community** — techniques and inspiration

---

## 📧 Contact

- **GitHub**: [AliesterEroan](https://github.com/AliesterEroan)
- **Nexus Mods**: [GerardDelon](https://www.nexusmods.com/profile/GerardDelon)

---

*Built with ❤️ by AliesterEroan*
