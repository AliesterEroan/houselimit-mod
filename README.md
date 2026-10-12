# House Limit Mod 🏠

**Unlock Unlimited Building Potential in Sun Haven**

A BepInEx mod that removes the restrictive house limit in Sun Haven, allowing you to build as many houses as you want. Replace the hardcoded limit of 8 with a configurable value (default: 999) and expand your town without boundaries.

## 🌟 Features

- **🔧 Configurable Limit** - Set your desired house limit via BepInEx config file
- **🎯 Harmony Patches** - Clean runtime patching using Harmony library
- **🔄 Comprehensive Coverage** - Patches all hardcoded house limit checks in the game
- **🛡️ Safe & Reversible** - Simply delete the DLL to disable - no permanent changes
- **📝 Runtime Configuration** - Change the limit without recompiling
- **⚡ Lightweight** - Minimal performance impact

## 🚀 Getting Started

### Prerequisites

- **Sun Haven** (Steam or GOG version)
- **BepInEx** - Already installed in your game directory
- **.NET SDK 8.0** - For building the mod (optional if using pre-built DLL)
- **Visual Studio / VS Code / Rider** - For development (optional)

### Installation

#### Option 1: Pre-built DLL (Recommended)

1. Download the latest `HouseLimitMod.dll` from [Releases](https://github.com/AliesterEroan/SunHavenHouseLimitMod/releases)
2. Place it in your game's BepInEx plugins folder:
   ```
   Sun Haven\BepInEx\plugins\HouseLimitMod.dll
   ```
3. Launch the game - the mod will load automatically

#### Option 2: Build from Source

1. **Clone the repository**
   ```bash
   git clone https://github.com/AliesterEroan/SunHavenHouseLimitMod.git
   cd SunHavenHouseLimitMod
   ```

2. **Install .NET SDK 8.0** (if not already installed)
   ```bash
   winget install Microsoft.DotNet.SDK.8
   ```

3. **Update DLL paths in `HouseLimitMod.csproj`**
   - Open `HouseLimitMod.csproj`
   - Update the `<HintPath>` values to point to your Sun Haven installation
   - The current paths are set to: `D:\personal\games\SteamUnlocked\Sun.Haven.v3.1.3b.ALL.DLC\Sun.Haven.v3.1.3b.ALL.DLC\`
   - Update these to match your game installation path

4. **Build the project**
   ```bash
   dotnet build
   ```

5. **Copy the DLL to your game** (if auto-copy is disabled)
   ```bash
   copy bin\Debug\net472\HouseLimitMod.dll "YOUR_SUN_HAVEN_PATH\BepInEx\plugins\"
   ```

## ⚙️ Configuration

After running the game once, a config file will be created at:
```
BepInEx\config\houselimit.cfg
```

Edit this file to change the house limit:
```ini
[General]
## Maximum number of houses allowed (default: 999)
Max Houses = 999
```

**Recommended values:**
- **Small towns:** 50-100
- **Medium towns:** 100-500
- **Large towns:** 500-999
- **Extreme:** 1000+ (may cause performance issues)

## 📁 Project Structure

```
SunHavenHouseLimitMod/
├── HouseLimitMod.cs          # Main mod code with Harmony patches
├── HouseLimitMod.csproj      # .NET project configuration
├── README.md                 # This file
└── bin/
    └── Debug/
        └── net472/
            └── HouseLimitMod.dll  # Compiled mod DLL
```

## 🛠️ Technologies Used

### Core
- **C#** - Programming language
- **.NET Framework 4.7.2** - Target framework
- **.NET SDK 8.0** - Build tooling

### Modding Framework
- **BepInEx 5.4** - Unity/mono modding framework
- **Harmony** - Runtime method patching library
- **Mono.Cecil** - IL code manipulation

### Game References
- **Assembly-CSharp.dll** - Sun Haven's main game assembly
- **UnityEngine.dll** - Unity engine core
- **UnityEngine.CoreModule.dll** - Unity core module

## 🎯 How It Works

The mod uses Harmony transpilers to modify hardcoded values at runtime:

1. **MaxNumber Property** - Replaces the hardcoded return value of `8` with your configured limit
2. **SetMeta Method** - Replaces `Mathf.Min(playerNumber, 7)` with `Mathf.Min(playerNumber, maxHouses-1)`
3. **Build Method** - Replaces `Mathf.Min(playerNumber, 7)` with `Mathf.Min(playerNumber, maxHouses-1)`
4. **SceneToLoad Method** - Replaces `Mathf.Min(houseNumber, 7)` with `Mathf.Min(houseNumber, maxHouses-1)`
5. **GetHouseSceneIndex Method** - Replaces `Mathf.Min(playerNumber, 7)` with `Mathf.Min(playerNumber, maxHouses-1)`
6. **HousePlaceable.PlaceDecoration** - Replaces the hardcoded `8` check that prevents placing houses
7. **HousePlaceable.Use1** - Replaces the hardcoded `8` check that shows the placement warning
8. **HousePlaceable.ModifyMeta** - Replaces the hardcoded `8` loop limit for house slot assignment

All patches are applied at game startup and are completely reversible.

## 🧪 Development

### Building

```bash
# Build the project
dotnet build

# Build in release mode
dotnet build --configuration Release
```

### Code Quality

```bash
# Analyze code
dotnet analyze

# Format code
dotnet format
```

## 🔧 Troubleshooting

### Mod doesn't load

1. Check `BepInEx\LogOutput.log` for errors
2. Verify BepInEx is properly installed (run the game once without mods)
3. Ensure `HouseLimitMod.dll` is in the correct folder: `BepInEx\plugins\`
4. Check that all game DLL references in `.csproj` point to correct paths

### House limit not changed

1. Verify the config file exists and has your desired value
2. Restart the game after changing the config
3. Check the log for "House Limit Mod loaded!" message

### Game crashes on startup

1. Remove the mod DLL and try again
2. Check the log for specific error messages
3. Try a lower house limit value (e.g., 50 instead of 9999)

## 📝 Technical Details

### Harmony Patch Locations

| Method | Original Value | Patched Value | Purpose |
|--------|---------------|---------------|---------|
| `PlayerHouse.MaxNumber.get` | `8` | `configMaxHouses.Value` | Main house limit |
| `PlayerHouse.SetMeta` | `7` | `configMaxHouses.Value - 1` | Player number clamping |
| `PlayerHouse.Build` | `7` | `configMaxHouses.Value - 1` | Portal house number |
| `PlayerHouse.SceneToLoad` | `7` | `configMaxHouses.Value - 1` | Scene name generation |
| `PlayerHouse.GetHouseSceneIndex` | `7` | `configMaxHouses.Value - 1` | Scene index lookup |
| `HousePlaceable.PlaceDecoration` | `8` | `configMaxHouses.Value` | Placement limit check |
| `HousePlaceable.Use1` | `8` | `configMaxHouses.Value` | Placement warning check |
| `HousePlaceable.ModifyMeta` | `8` | `configMaxHouses.Value` | House slot loop limit |

### Why `maxHouses - 1`?

House numbers are 0-indexed (0 to max-1), so if you want 999 houses, the valid range is 0-998, hence the `-1` adjustment.

## ⚠️ Known Limitations

- **UI:** The game UI may not display house numbers beyond the original limit properly
- **Performance:** Very high limits (1000+) may cause performance issues in large towns
- **Multiplayer:** All players must have the mod installed for consistency
- **Save Files:** Existing saves with houses at the old limit should work fine

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

##  Acknowledgments

- **BepInEx Team** - For the excellent Unity modding framework
- **Harmony Team** - For the powerful runtime patching library
- **Pixelated Milk** - For creating Sun Haven
- **Sun Haven Modding Community** - For inspiration and techniques

## 📧 Contact

- **Project:** Sun Haven House Limit Mod
- **GitHub:** [AliesterEroan](https://github.com/AliesterEroan)
- **Nexus Mods:** [GerardDelon](https://www.nexusmods.com/profile/GerardDelon)
- **Issues:** [Issue Tracker]

---

**Built with ❤️ by Aliester Eroan**
