# Changelog

## DEPRECATED - 2023-03-31

The tool has been deprecated in favor of [ddinfo-tools](https://github.com/NoahStolk/ddinfo-tools).

## [1.8.3.0] - 2022-03-25

- API updates and fixes.

## [1.8.2.0] - 2022-03-25

- API updates.

## [1.8.1.0] - 2022-03-25

- API updates.
- Leaderboards without dagger times use the Silver color.

## [1.8.0.0] - 2022-03-19

- Replays can now be downloaded from within the app. Use the arrow keys to navigate and press enter to select the replay you want to view. It will automatically be injected into the game.
- Added paging to leaderboards.
- UI improvements for leaderboards.
- Reduced CPU and memory usage.
- Local replays are no longer uploaded.

## [1.6.0.1] - 2022-03-08

- Added support for Time Attack and Race game modes.

## [1.5.3.0] - 2022-03-05

- Applied API changes that allow for smaller requests to the server and therefore faster uploads.
- The application now checks if the custom leaderboard exists before uploading the run.

## [1.5.0.0] - 2022-01-09

- The application now runs on .NET 6 which is a lot faster and will be officially supported until late 2024.
- You do not need to install the .NET runtime anymore. You do not need to install anything for the program to work as of this update (unless you're running Windows 7).

## [1.4.2.0] - 2021-10-29

- API updates for V3.2.

## [1.4.1.0] - 2021-10-27

- API updates for V3.2.

## [1.4.0.0] - 2021-10-27

- Replays are now uploaded to the server.

## [1.3.0.0] - 2021-09-12

- Reduced CPU and memory usage.
- Fixed not always scanning homing and level up times.
- Removed 32-bit support completely because Devil Daggers does not run on 32-bit machines anymore.

## [1.2.0.0] - 2021-06-28

- The application now detects V3.1 graph data and sends it to the server. Graphs will be supported on the website later.
- The application now detects if prohibited mods are used. Submitting runs with prohibited mods is still allowed, but this opens the possibility to disallow it for certain leaderboards in the future.

## [1.0.0.0] - 2021-03-16

- The application now retrieves the memory offset from the DevilDaggers.info API, so no updates for the program are required each time Devil Daggers itself gets updated.

## [0.14.7.0] - 2021-03-12

- Fixed not detecting Devil Daggers process after restart.

## [0.14.6.0] - 2021-03-11

- Applied new API updates to fix replays overwriting real scores by 1/60 second due to bug in game.

## [0.14.5.0] - 2021-03-10

- You can now submit replays.
- New stat 'Homing daggers eaten' is now sent to the server.
- Improved logo.

## [0.14.2.0] - 2021-02-25

- Works with the latest Devil Daggers build.

## [0.14.1.0] - 2021-02-21

- The application now works with Devil Daggers V3.1 and has the new features built-in, such as the Leviathan dagger and the new death types.
- Added new stats
  - Gems despawned
  - Gems eaten
  - Gems total
  - Enemies alive per enemy type
  - Enemies killed per enemy type
- The Homing dagger has been replaced by the new Leviathan dagger.
- The application now runs on .NET 5.0.
- Graphical improvements (for as far as console graphics go) and other small improvements.
- Fixed detection for usernames longer than 16 characters.

## [0.10.4.0] - 2020-10-01

- The application does not display misleading stats in the menu or lobby anymore.
- Improved update messages.
- Fixed tiny bug where getting the very same score as a dagger time would display the score in the wrong color.
- Implemented API updates as preparation for a new leaderboard category.

## [0.10.0.0] - 2020-08-24

- Implemented custom improved colors.
- Fixed not resetting homing after death.
- Fixed user highlight being unreadable for 'Default' dagger scores.
- Improved anti-cheat.
- Implemented collecting graph data for custom leaderboards. This is only stored in the database for now and not yet displayed on the website.

## [0.9.6.1] - 2020-08-21

- Rebuilt application for API updates.

## [0.9.6.0] - 2020-08-20

- Rewrote much of the application.
- Removed dependencies.
- Ported to .NET Core. The application is no longer dependent on .NET Framework and does not require .NET Framework 4.6.1.
- Fixed crash that occurred when setting a very large custom font size.
- Added icon.
- Renamed 'Shots' to 'Daggers'.
- Fixed resetting homing to 0 just before detecting player death.
- Fixed writing level up difference when previous highscore did not achieve that level.
- Fixed not being able to read current upgrade and homing count in some Devil Daggers instances due to malformed pointer chains.

## [0.6.1.0] - 2020-05-08

- Fixed ascending leaderboards displaying incorrect dagger colors and statistic differences.

## [0.6.0.0] - 2020-05-07

- Leaderboards and run info are now displayed in the console when the player died.
- Added colors for daggers, deaths, and statistic differences.
- Fixed floating point imprecision issues with the leaderboard database.
- The program now shows a warning when homing and level up times are not being detected. This warning will be triggered after collecting the first gem. The problem can be resolved by restarting Devil Daggers. It happens about 1 out of 10 times for me and seems to appear randomly. I'm still investigating what causes it.

## [0.4.4.0] - 2019-11-03

- Compatibility and maintenance updates related to the website.
- The application is renamed to Devil Daggers Custom Leaderboards again for consistency with the other tool names.

## [0.4.3.0] - 2019-08-08

- Improved way of detecting survival file cheats; there is no need to record the entire run anymore.
- Fixed log file not being written to.
- Implemented "Speedrun" category leaderboards.

## [0.4.0.1] - 2019-06-05

- Compatibility update due to some internal bug fixes which aren't related to the application directly.

## [0.4.0.0] - 2019-05-27

- Leaderboards are now secured with the Advanced Encryption Standard (AES).

## [0.3.3.0] - 2019-05-24

- Fixed inconsistent spawnset hashing. The hashing system is no longer dependent on files. This fixes the problem where some spawnsets wouldn't work if they were downloaded directly from the website rather than imported via Devil Daggers Survival Editor.

## [0.3.0.0] - 2019-05-20

- The program now tells you when there is an update available and warns you when the current version is no longer accepted by the server.

## [0.2.5.0] - 2019-05-19

- Added logging.
- Some improvements in the layout and better feedback for when runs don't upload.
- Crash fixes and internal clean up. The "out of bounds" error shouldn't occur anymore when starting the application before starting Devil Daggers.
- Console is no longer resizable so it doesn't mess with the layout.
- Added a retry count for when the upload fails. Usually it retries 3 times and stops after that, waiting for you to restart a run.

## [0.2.1.0] - 2019-05-18

- Lots of internal clean up, improvements, and fixes.
- Program now outputs what values it submits to the server.
- Program only retrieves the spawnset hash during the first second of the run so people cannot cheat by changing the survival file during the run. If you start the program later than 1 second after the run starts, the hash will not be calculated and your submission will be marked as invalid and not upload.
- The server now has a minimal version it will accept submissions from.

## [0.1.10.0] - 2019-05-18

- Small fixes.

## [0.1.9.0] - 2019-05-18

- The application now uses .NET Framework 4.6.1 rather than 4.7.2.
- Fixed bug where level up values don't reset when you restart a run.
- Prevented replays from uploading so people won't submit runs to the wrong leaderboard by intentionally replacing the survival file during the replay.

## [0.1.5.0] - 2019-05-17

- Made application Windows-only because scanning memory for other operating systems will work differently anyway.
- Enforced en-US culture to fix broken submissions on PCs that use commas as decimal separators.
- Fixed usernames being limited to 4 characters.
- Prevented submissions with 0.0000 time from uploading by setting a time constraint of a minimum of 2.5 seconds.

## [0.1.0.0] - 2019-05-15

- Initial release.
