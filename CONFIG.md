# Configuration

Every setting lives in `BepInEx\config\com.ivmakk.thelastplagueblight.skipintrologos.cfg`, written the first time you run the game with the mod installed. Edit it with any text editor while the game is closed. Changes apply at the next game start. A value outside its range is set to the nearest end of the range.

## General

| Setting | Default | Values | What it does |
|---|---|---|---|
| `SkipSplash` | `true` | `true` / `false` | Skips the studio logos and the "Made with Unity" logo at the game start. With `false`, the logos show as in the game. |
| `FadeSeconds` | `0.1` | `0` to `1` | The longest black fade at the title screen, before and after each save load, at a new game, a host or a join, and at a respawn, in seconds. The game fades for 1 to 2 seconds. A game fade that is already shorter stays as it is. With `0`, the screen changes at once. The fades in the world, such as the wake-up after sleep, stay as in the game. |
| `FastWorldLoad` | `true` | `true` / `false` | Ends the load screen when the land near you is ready. The far land loads in the first second after the load. Your View Distance setting does not change, and with a low View Distance the world loads as in the game. With `false`, the world loads as in the game. |
| `Verbose` | `false` | `true` / `false` | Writes extra diagnostic lines to the BepInEx log for troubleshooting. Keep off in normal play. |
