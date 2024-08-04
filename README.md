# VMMWSGC (Virtual Memory Manager Working Set Garbage Collection)

VMMWSGC runs [EmptyWorkingSet()](https://learn.microsoft.com/windows/win32/api/psapi/nf-psapi-emptyworkingset) upon completion of a contract (mission) in HBS' Battletech (success, failure, or withdrawl). This reduces the private working set of the Battletech process to a few hundred MiB from multi-GiB. Battletech will then reallocate the necessary memory for the private working set.

### What does this do?

This mod allows you to run multiple Battletech missions, [re]load save games, etc. for a long duration without consuming a significant amount of memory, allowing you to play longer without restarting Battletech. It _maintains_ Battletech's private working set usage between missions.

### What does this _not_ do?

This doesn't fix the inherit memory issues present in HBS' Battletech.

### Measurement

Using [Process Explorer](https://learn.microsoft.com/sysinternals/downloads/process-explorer), drill down on the Battletech.exe Private WorkingSet. While playing a game, observe the value of the private working set; it will grow over time as missions are completed, save games are loaded, and so on. With this mod, when a mission is completed, VMMWSGC will empty the private working set to the minimum number of pages required. In Process Explorer, this will be observable when the private working set drops from multi-GiB values to hundreds of MiB (usually 200 - 300MiB). Soon after, the private working set will grow again, usually between 4 - 5GiB. This is expected behavior; without this mod, the private working set may grow >15 - 20GiB when playing multiple missions in a row.

### Performance

This mod runs in approximately 0.5 seconds on an AMD 5800X3D with DDR4-3200 RAM. It runs during a non-performance critical portion of the game.
