# VMMWSGC (Virtual Memory Manager Working Set Garbage Collection)

VMMWSGC runs [EmptyWorkingSet()](https://learn.microsoft.com/windows/win32/api/psapi/nf-psapi-emptyworkingset) when various scenarios occur in HBS' Battletech, such as autosaves, saving games, loading games, and at the beginning of the first round in a contract. This reduces the private working set of the Battletech process to a few hundred MiB from multi-GiB. Battletech will then reallocate the necessary memory for the private working set.

This mod is targeted at large modpaks such as [RogueTech](https://roguetech.fandom.com/wiki/Roguetech_Wiki), [BTAU](https://www.bta3062.com/), and [BEX](https://discourse.modsinexile.com/t/battletech-extended-3025-3061-1-9-3-7/426) which may add significant memory overhead with their additional features, models, etc.

Version 1.0.0 is included with Battletech Extended - Tactics 2.0.

Version 1.1.1 is included in BATU/BTA Lite 18.0.1.

### What does this do?

This mod allows you to run multiple Battletech missions, [re]load save games, etc. for a long duration without consuming a significant amount of memory, allowing you to play longer without restarting Battletech, with a goal of not having to restart at all. It _maintains_ Battletech's private working set over time.

### What does this _not_ do?

This doesn't fix the inherit memory issues present in HBS' Battletech.

### Measurement

You can use various tools such as Process Explorer, Task Manager, and Process Monitor to watch the Battletech Working Set value. The Working Set is the measurement of physical memory a particular process is using.

Another value, "Commit size", can largely be ignored. It is a representation of the Working Set + additional Virtual Address Space allocated to a process -- not necessarily _used_ by said process.

### Performance

This mod runs in approximately 0.1 milliseconds on an AMD 5800X3D with DDR4-3200 RAM. It runs during a non-performance critical portions of the game.

### Settings

The mod includes a `mod.json` file with a `Settings` entry allowing the player to change when this mod runs. The original point of execution of this mod post-contract completion has been disabled by default as it is no longer necessary, but can be re-enabled by the player. The setting `RunOnNewRound` is disabled by default. Enable this setting to empty the working set on every new round in-mission.

### Managing Autosaves

Saving the game is a significant source of memory leaks and causes a brief freeze in the UI. This mod allows you to control when autosaves occur, if at all.

Autosaves can be enabled, selectively enabled, or disabled. By default, all autosave types are enabled with the setting `EnableAll` in `mod.json`. If `DisableAll` is set to true, no autosaves of any type will be taken. Selected autosave types may be set to true or false. Note the game has other autosave types that are not represented here (such as the end of a campaign). If you want other autosave types to be enabled, set `EnableAll` to true.

If you choose to disable autosaves, it is on you to save your game!

### Building

Requires Visual Studio 2022 and uses the .NET Framework 4.7.2.

Adjust the location of the Battletech game folder in Directory.Packages.props. Restore necessary NuGet packages. Build!
