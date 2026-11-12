![version](https://img.shields.io/badge/version-1.0.0-blue) ![coverage](https://img.shields.io/badge/coverage-100%25-brightgreen)

# Skip Intro Logos

Skip Intro Logos is a mod for the Steam game *The Last Plague: Blight*. Each game start shows logo screens, and each save load waits behind slow black fades and a full world load. This mod skips the logos, shortens the black fades, and gets you into the world sooner.

On the test PC, the title screen showed after 6 s in place of 11 s, and the first save load took 6 s in place of 11 s.

## Features

- **No intro logos.** The game starts straight at the title screen (`SkipSplash`).
- **Short black fades.** 0.1 s in place of 1 to 2 s at the title screen and around loads (`FadeSeconds`). The sleep fade stays.
- **Faster world load.** Save loads take about half the time. Distant terrain loads with a short delay (`FastWorldLoad`).

## Save safety

The mod writes nothing to the save and changes no game file. You can remove the mod at any time, and your save loads as before.

## Multiplayer

Made and tested for single player. Co-op is not tested.

## Credits

The logos are hidden only to save time at each start. The Last Plague: Blight is made by **Original Studios**, and it runs on the [Unity](https://unity.com) engine. Thanks to both for the game.

## Requirements

- The Last Plague: Blight 0.12.7 or later.
- The [BepInEx Pack for The Last Plague: Blight](https://www.nexusmods.com/thelastplagueblight/mods/9), the BepInEx 6 (IL2CPP) build for the game.

## Install

Download the zip from [GitHub Releases](https://github.com/ivmakk-tlpb/tlpb-skip-intro-logos/releases).

1. Install the [BepInEx Pack for The Last Plague: Blight](https://www.nexusmods.com/thelastplagueblight/mods/9). If no other mods were installed before, start the game once so BepInEx finishes setup, then quit.
2. Extract this mod's zip into the game folder (the folder with the game .exe). The DLL lands in `BepInEx\plugins`. Full path example:
   - Steam: `C:\Program Files (x86)\Steam\steamapps\common\The Last Plague\BepInEx\plugins\SkipIntroLogos.dll`

## Uninstall

Delete `SkipIntroLogos.dll` from the `BepInEx\plugins` folder. On the next game start, the logos, the fades, and the world load are as in the game.

## Configuration

To change mod settings, edit `BepInEx\config\com.ivmakk.thelastplagueblight.skipintrologos.cfg` while the game is closed (see [CONFIG.md](CONFIG.md) for every setting). Changes apply at the next game start.

## Troubleshooting

If a game update renames a part of the game that the mod needs, only the feature that uses it turns off, with one warning in the log. The other features keep working.

If the game does not start with the mod, set `SkipSplash = false`: the logos then show as in the game, and the other features still work.

Check `BepInEx\LogOutput.log` for the `Skip Intro Logos loaded.` line, and for warnings or errors from Skip Intro Logos.

## Build

Skip Intro Logos is a BepInEx 6 IL2CPP plugin. Building requires the .NET 8 SDK and a game installation with BepInEx. Start the game once after installing BepInEx to generate the IL2CPP interop assemblies.

```
dotnet build src/SkipIntroLogos.csproj -c Release
```

`Directory.Build.props` sets `GameDir` to the default Steam installation path. For another location, set the `GameDir` environment variable or pass `-p:GameDir=...` to the build command. The output DLL is `src\bin\Release\SkipIntroLogos.dll`.

Unit tests cover all files of `src/` and run with no game and no BepInEx, because the test project compiles the patch code against fake game types.

```
dotnet test tests/SkipIntroLogos.Tests
```

## Package

```
dotnet build src/SkipIntroLogos.csproj -c Release -p:Package=true
```

The zip is `dist/SkipIntroLogos-<version>.zip`, laid out as `BepInEx/plugins/SkipIntroLogos.dll`.

## License

Licensed under the GNU General Public License v3.0. Copyright (C) 2026 ivmakk. See [LICENSE](LICENSE).

You may reuse and modify this mod, but you must keep it open under the same license and give credit. Do not reupload it without credit.
