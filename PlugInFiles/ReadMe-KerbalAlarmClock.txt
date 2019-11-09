KerbalAlarmClock - %VERSIONSTRING%
--------------------------
How to stop Jeb from flying past his destination at Warp speed.

By Trigger Au

Forum Thread for latest: https://forum.kerbalspaceprogram.com/topic/22809-kerbal-alarm-clock/
Documentation Site: https://triggerau.github.io/KerbalAlarmClock/
Install Instructions: https://triggerau.github.io/KerbalAlarmClock/install.html

INSTALLATION
******************* NOTE  ******************* NOTE ******************* NOTE *******************
IF YOU WANT TO MAINTAIN YOUR SETTINGS DO NOT COPY THE CONFIG.XML FILE OVER
******************* NOTE  ******************* NOTE ******************* NOTE *******************

Installing the plugin involves copying the plugin files into the correct location in the KSP aplication folder
1. Extract the Zip file you have downloaded to a temporary Location
2. Open the Extracted folder structure and open the KerbalAlarmClock_v%VERSIONSTRING% Folder
3. Inside this you will find a GameData folder which contains all the content you will need
4. Open another window to your KSP application folder - We'll call this <KSP_OS>
5. Copy the Contents of the extracted GameData folder to the <KSP_OS>\GameData Folder
6. Start the Game and enjoy :)

TROUBLESHOOTING
The plugin records troubleshooting data in the "<KSP_OS>\KSP_Data\output_log.txt".
If there are errors in loading the config you can delete the "<KSP_OS>\GameData\TriggerTech\KerbalAlarmClock\settings.cfg" and restart the game

LICENSE
This work is licensed under an MIT license as outlined at the OSI site. Visit the documentation site for more details and Attribution

ATTRIBUTION-SOUNDS
Included Sounds are from freesfx.co.uk (https://www.freesfx.co.uk). EULA can be found here: https://www.freesfx.co.uk/info/eula/

VERSION HISTORY
Version 3.12.0.0	-	KSP Version: 1.8.1
- Recompiled for 1.8.1 (Issue #234)

Version 3.11.0.0	-	KSP Version: 1.7.3
- Recompiled for 1.7.3 (Issue #225)
- Dont report missing vessel if there isnt one (Issue #222)  - Thanks ReeseGlidden
- Fix Drop-dows and scaling (Issue #228) - Thanks Kerbas-Ad-Astra
- update some urls/uris (Issue #229) - thanks zatricky and neilser

Version 3.10.0.0	-	KSP Version: 1.6.0
- Recompiled for 1.6.0 (Issue #199, #199)
- Fix for hyperbolic orbits (Issue #210) - Thanks Taniwha
- Fix for version check issues (Issue #208) - Thanks MWerle
- Removed some compiler warnings and deprecated calls
- Change the way UI Hiding works (Issue #206)
- Fix perf issue in TS (Issue #207)
- Add UI scaling to the app (Issue #204)
- Add MapUI lock in TS for click through - as much aas can do with current locks (Issue #192)
- Add prevention of jumping to unowned vessels in career (Issue #154)
- Change the datetime formatting stuff so its Kronometer compatible (Issue #202, Issue #194, Issue #157) - Thanks Aelfhe1m 
- Fix for vessel name locazlization
- Updated some of the common UI calls to reduce GC Churn
- Updated WarpRate checking to rewmove GC Churn
- Removed some Triger specific stuff from the build scripts (Issue #196) - Thanks MWerle

Version 3.9.1.0		-	KSP Version: 1.4.1
- Removed custom cursors for users running Linux as this kills the game cursor for them (Issue #200)

Version 3.9.0.0		-	KSP Version: 1.4.1
- Recompiled for 1.4.1 (Issue #199, #199)
- Fix alarm delete when no message displayed (Issue #195)

Version 3.8.5.0		-	KSP Version: 1.3.0
- Recompiled for 1.3.0 (Issue #191)
- Merged in configurable tooltip period (Issue #190)
- Added Purge stuff for meta files (Issue #189)

Version 3.8.4.0		-	KSP Version: 1.2.2
- Fixed bug in HideWhenPaused logic - (Issue #184)
- Hide the KAC when in the SpaceCenter overlay scenes Issue #186)

- No longer create duplicate mannodes from alarm windows

Version 3.8.3.0		-	KSP Version: 1.2.2
- Fixed stupd typo with scrollview and add transfer (Issue #183)

Version 3.8.2.0		-	KSP Version: 1.2.2
- Another Initialization issue with object line renderer - Thanks JPL
- Fixed NRE re timewarp in editor and other guard statement - Thanks JPL
- Adjusted layout for long transfer window lists (Issue #171)
- Fixed Delete confirmation dialog positioning (Issue #170)

Version 3.8.1.0		-	KSP Version: 1.2.0
- Fixed Initialization issue with object line renderer
- Fixed SetVessel working in TS
- Fixed crash occuring when switching in flight to unloaded vessel
- fixed warp reduction not cancelling autowarp


Version 3.8.0.0		-	KSP Version: 1.2.0
- Code changes and recompile for 1.2 - Thanks Nightingale and JPLRepo
- Added ScienceLab Alarms - Thanks natsnudasoft 
- Adjusted Time code for in Editor - Thanks aw1621107

Version 3.7.1.0		-	KSP Version: 1.1.3
- Fixed issue with location of ToolbarIcon folder - my bad
- Fixed NRE when PauseMenu is not yet available to query (Issue #164)

Version 3.7.0.0		-	KSP Version: 1.1.3
- Recompiled to resolve isses with TimeWarp methods (Issue #169)
- Moved Add All Transfers button to top of list (Issue #163)
- Moved non GameDB files to PluginData folder (Issue #166)
- NOTE: the above fix means the folder structure changes

Version 3.6.3.0		-	KSP Version: 1.1.2
- Adjusted code to handle UIToggle and flight Pause menu

Version 3.6.2.1		-	KSP Version: 1.1.2
- Fixed issue in TimeObjects for PhaseAngle display

Version 3.6.1.0		-	KSP Version: 1.1.0
- Merged fork by jslater89 for conversion
- Added code to handle HideUI

Version 3.5.0.0		-	KSP Version: 1.0.5
- Fixed issue with dropdown pages and alignment
- Fixed issue with settings bleed (Issue #152)
- Solved limit on Target Distance Alamrs - for solar orbits (Issue #151)
- Updated KERWrapper to better handle the KER window not being open (Issue #150)
- Added AngleRenderer for ejection angle displays of TWP Alarms

Version 3.4.0.0		-	KSP Version: 1.0.4
- Added Audio to Alarms - ie play a sound when an alarm fires (Issue #4)
- Added repeat option to Ap/PE alarms (Issue #128)
- Added an action for only showing a message if the associated vessel is not the active one (Issue #125)
- Added "Create All Transfers" option for transfer alarms so you can create all windows from a planet (Issue #123)
- Added "Select Vessel" option for TS so you can highlight the vessel when an alarm fires (Issue #100)
- Added VOID wrapper to do burntime from it as well (Issue #138)
- Redesigned the Alarm Actions to allow many more combinations of actions (Issue #146)
- Change WarpTo Display so buttons hidden if any Manuever Gizmo is active (Issue #148)
- Fixed Issue with incorrect time formatting on Model Transfers (Issue #149)
- Fixed issue where autoManNode Alarms weren't calculating/updating burntime (Issue #139)
- Extra checking to SetCurrentFlightStates to prevent NRE (Issue #147)
- Extra checking to isEditorVAB to prevent NRE (Issue #142)
- Reworked selection code in TS

Version 3.3.2.1		-	KSP Version: 1.0.2
- Added KER Wrapper/Integration to provide BurnTime based margins (Issue #14)
- Fixed issue with editing Automatic ManNode Margins (Issue #135)
- Merged Fix re GUISkin and other plugins from mjn33 (Issue #133)
- Merged typo fix from Dennovin (Issue #119)
- (.1 because ) Missed an if statement

Version 3.3.1.1		-	KSP Version: 1.0.2
- Change to version file for CKAN and patch levels

Version 3.3.1.0		-	KSP Version: 1.0
- Changed .version values to cater to CKAN and patch releases (Issue #129)
- Updated some code around visibility of windows to fix scene changes (Issue #130)
- Added separate values for VAB,SPH for visibility

Version 3.3.0.1		-	KSP Version: 1.0
- KerbalStuff Release Issue - no code changes

Version 3.3.0.0		-	KSP Version: 1.0
- Recompiled for 1.0
- Code changes for launcher and Armosphere changes
- Updated KAC Wrapper
- Fixed Date of Event displaying Date of Alarm (Fixes #126)

Version 3.2.4.0		-	KSP Version: 0.90
- Fixed duplicate alarms with Auto and WarpTo (Issue #124)
- Fixed log message with missing image (Issue #121)
- Enabled KAC in Editor Scenes (Issue #120)
- Fixed issue woth Alarm clock opening on every scene (Issue #118)
- Added option to limit Warp rate for WarpTo Alarms (Issue #117)
- Fixed placement issue with Minimized mode (Issue #116)
- Fixed missing confirmation on delete on minimized window (Issue #115)


Version 3.2.3.0		-	KSP Version: 0.90
- Changed WarpTo buttons to only respond to left-click (Issue #112)
- Added Confirmation option to WarpTo Buttons (Issue #111)
- Fixed bug in Launch Rendezvous code for non Equatorial landing positions (Issue #110)
- Added Filter to this Vessel option for alarms list (Issue #108)
- Added Date of Alarm to editing window (Issue #107)
- Added option to display child windows below main window, not just to the side (Issue #106)

Version 3.2.2.0		-	KSP Version: 0.90
- Changed fix for Contracts going missing to use a gameevent (Issue #92 again)
- Changed orbit render stuff in TS so it is more specific (Issue #104)
- Added Margin options for WarpTo Alarms (Issue #105)

Version 3.2.1.0		-	KSP Version: 0.90
- Added check to not display WarpTo when vessel is landed

Version 3.2.0.0		-	KSP Version: 0.90
- Added Window Width Resizing via Mouse (Issue #102)
- Added WarpToNode Feature for MapView (Issue #98)
- Made Warp Kill via X the default behaviour (Issue #101)
- Added some code to fix Control Layout Errors (Issue #99)

Version 3.1.2.0		-	KSP Version: 0.90
- Updated default button state for app button (Issue #97)
- Fixed issue with "date" alarms not using epoch (Issue #96)
- Added feature to detect Ap/Pe when choosing Apsis alarms (Issue #95)
- Fixed Toolbar code to use relative paths (Issue #94)

Version 3.1.1.0		-	KSP Version: 0.90
- Fixed issue with Contract alarms disappearing on scene change (Issue #92)
- Added feature to swap the child windows to the left side if the KAC is near the right screen edge (Issue #94)

Version 3.1.0.0		-	KSP Version: 0.90
- Added option so when you hit the throttle cutoff ("X" by default) it will also kill warp (Issue #85)
- Added "No Action" Action options (Issue #80)
- Added functionality to change calendar to RSS cal (Issue #53)
- Added extra message for career mode when AP/PE points are not targettable (Issue #83)
- Added warning to Transfer values for RSS loaded games (Issue #18)
- Closed bug with Asteroid Closest Distance crash (Issue #54)
- Confirmed code requirement for closest Distance and landed craft (Issue #73)
- Closed request re "Focus"ing vessels in TS - no API exists (Issue #32)
- Replaced all the Time objects with new KSPDateTime and KSPTimeSpan classes (Issue #74)
- Added AVC .version file (Issue #88)
- Restructured Zip to better suit CKAN (Issue #87)
- Updated Toolbar wrapper
- Updated manual pages

Version 3.0.6.0		-	KSP Version: 0.90
- Recompiled for 0.90
- Couple of code changes for 0.90 changes
- Added Delete Confirmation optional Dialog (Issue #82)
- Adjusted Top Clamp value to be off screen slightly (Issue #79)
- Changed KACBackup savenames to start with z so they go to the bottom of the list

Version 3.0.5.0		-	KSP Version: 0.25.0
- Added extra warp rate calcs for non standard values (Issue #71)
- Added slider to configure weighting of transition periods
- Added some instant transition fallbacks if alarm is too close to the game UT (Issue #72)
- Fixed issue with Contract alarms in sandbox mode (Issue #78)
- Fixed issue with warp fighting when GUI not displayed (Issue #76)
- Double checked all input lock code

Version 3.0.4.0		-	KSP Version: 0.25.0
- Fixed tooltip typos (Issue #68)
- Fixed error on load of empty alarm block
- Fixed issues with jumping from paused alarms at SC
- Fixed versioncheck file

Version 3.0.2.0		-	KSP Version: 0.25.0
- Fixed issue where Alarm list was persisting between saves (Issue #67)
- Added extra logging and null checks re AppLauncher
- Updated KACWrapper to handle Alarmtime properly and add repeat properties

Version 3.0.1.0		-	KSP Version: 0.25.0
- Rework of majority of code base
- Ability to choose from 3 visual Skins
- Added Contract Alarms for Expiry and Deadlines
- Added Alarm Repeats for Raw, Crew and Model Transfer alarms
- QuickAdd option for contextual Alarms
- QuickAdd and Warp option
- Added Kill Warp with No Message action and restructured buttons/code behind
- Alarm Saving - now in the games save file - only saves and loads on game/save and load
- Adjusted settings tabs to better layout options
- API for Alarm Creation/Modification and events
- App Launcher integration - this is now the default
- Has input locking options for when mouse is over the panels - this prevents action groups from occuring when mouse is over windows
- Changed default Warp affecting to cater to non-instant transitions
- New Common Toolbar Wrapper - official one
- Removed periodic save of alarms file now that alarms are in the save files
- Screen clamping on main window
- Sorted the multiline alarm display stuff
- Window Resizing for better display
- Font changes to make more readable
- F11 keystroke disabled - can be reenabled via settings value
- Restructure settings
- Restructure plugin folders
- Added Alarm Import Tool for v2 Alarms
- Added Flags
- New documentation site - https://triggerau.github.io/KerbalAlarmClock/
- New versioncheck location on Github

Version 2.7.9.0		-	KSP Version: 0.25.0
- Recompiled for 0.25
- Image size optimisation thanks to dak180
- removal of last spaceport remains

Version 2.7.8.2		-	KSP Version: 0.24.2
- Fixed issue with inputlocks not coming off sometimes (Fixes #48)

Version 2.7.8.1		-	KSP Version: 0.24.2
- Fixed bug with Add alarm types ("!=" != "==") (Issue #47)

Version 2.7.8.0		-	KSP Version: 0.24.2
- Compiled against 0.24.2 binaries
- Fixed click through in KSC - used InputLockManager and the ControlType of KSCFacilities (Issue #45)
- Added Transfer Windows type to SC Alarms (Issue #46)

Version 2.7.7.0		-	KSP Version: 0.24.0
- Compiled against 0.24 binaries
- 4th attempt at texture blurring (Issue #33)

Version 2.7.6.1		-	KSP Version: 0.23.5
- Reversed change of Images to TGA, Back to PNGs (Issue #33)

Version 2.7.6.0		-	KSP Version: 0.23.5
- Change ability to Jump to Asteroid to be an Option - default is disabled (Issue #37)
- Fixed issue with Time Margin Entry and putting in large numbers (Issue #36)
- Added new code to detect when window is repositioned and save it after its stopped moving (Issue#35)
- Changed all Images to TGA format to fix compression artifacts (Issue #33)
- Added function to set default Date for Raw type alarms to start at today + 10mins (Issue #31)

Version 2.7.5.0		-	KSP Version: 0.23.5
- Tweaked Update check to be on a background worker process (Issue #28)
- Added capability to add certain alarm types to SC and TS Scenes (Issue #24)
- Added list icons for Raw and Earth alarms (Issue #29)
- License Changed to MIT license (less restrictive than previous)

Version 2.7.4.0		-	KSP Version: 0.23.5
- Removed log message on periodic saves (Issue #25)
- Window height now adjusts for Multiline Alarms (Issue #15)
- Closest Approach Alarms now allow target of Docking Port (Issue #1)
- Fixed duplication of Manuever Nodes on Ship Jump (Issue #23)
- Changed latest version file source to be Google site (smaller file)

Version 2.7.3.0		-	KSP Version: 0.23.5
- Fixed the pesky bug with the auto alarm periods not saving

Version 2.7.2.0		-	KSP Version: 0.23.5
- Adding Warp halting code to TS and SC
- Added extra code for guiload when texture loads fail

Version 2.7.1.0		-	KSP Version: 0.23.5
- 0.23.5 Recompile for new version of Unity/KSP
- Added functionality to detect time display type and match KAC times accordingly

Version 2.7.0.0		-	KSP Version: 0.23
- 0.23 Recompile for new version of Unity
- Added Option to choose to use Blizzy's excellent toolbar - uses latebinding so no need to include DLL and no hard reference
- Added link in game so if common toolbar not installed people can jump to forum page
- Added option so minimal display will show the next or oldest alarm
- Changed texture loading to use GameDB and this should fix Linux x64 issues with the Unity Texture2D Loader crashing
- Fixed bug where twitching SOIs on large orbits no longer creates lots of alarms for Auto-SOIs
- Fixed Bug using wrong path separators for MacOS/Linux re jumping from non-flight scenes
- Fixed Bug displaying alarms in non-flight screens when KAC not visible

Version 2.6.4.0		-	KSP Version: 0.22
- Added a threshold for Auto adding Maneuver Node alarms, so if you are closer than the threshold it wont make an alarm
- Added capacity to Kerbal alarms to store and restore targets and maneuvers regardless of vessel
- Fixed Alarm Icon in non Flight Scenes showing warp indicator
- Fixed some non-persistant settings in last few updates

Version 2.6.3.0		-	KSP Version: 0.22
- Tweaked functionality to for ship jumping and backups (now includes self closing alert dialog)
- Added Functionality to SOI Auto Alamrs to allow ignoring EVA Kerbals for Auto Add Alarms
- Added Functionality to Allow Auto Adding Maneuver Node Alarms
- Fixed some case issues with a couple of image files

Version 2.6.2.0		-	KSP Version: 0.22
- Added functionality to allow you to jump to a ship from space center and tracking station view lists
- Added functionality to backup save files before switching ships using KAC
- Both these features are configurable

Version 2.6.1.0		-	KSP Version: 0.22
- Recompiled it for 0.22
- Added Crew Alarms (track Kerbal rather than Vessel)
- Added Distance Target Alarms - distance from target vessel or altitude above planet
- Added Launch Rendezvous Alarm (under Ascending/Descending Node for Landed craft) - MechJeb2 code - thanks r4m0n
- Allow restoration of Nodes that you have passed (useful for interplanetary burns)
- Added missing Dres Transfer Model data - thanks Voneiden
- Added view only version of Alarm clock to both Space Center and Tracking Station
- Added options to hide/move the icon for all three visible scenes

Version 2.5.0.0		-	KSP Version: 0.21.1
- Recompiled it for 0.21
- Fixed some issues with Hyperbolic orbits and AN/DN Nodes

Version 2.4.0.2		-	KSP Version: 0.20.2
- Fixed stupid mistake AN/DN GUI Code

Version 2.4.0.1		-	KSP Version: 0.20.2
- Changed AN/DN code to use Mechjeb style ones to correct for my lack of math skills
- Changed AN/DN Add so if there is no target it displays the equatorial AN/DN times
- Adjusted Future orbits code to fix rounding issues
- Added capacity to edit the frequency of checks - people can now tweak this for their system and helps prevent severe warp changes that can affect vesel orbits 
- Adjusted delete alarm on close code so alarm did not reappear
- Fixed time display setting not persisting
- Sorted out Earth alarms not updating when paused
- Fixed Line wrapping on Add Alarm window (still need to work out the right answer for the main window) 
- Some other Graphical tweaks

Version 2.3.0.0		-	KSP Version: 0.20.2
- Added alarms for "Earth" Universe
- Added alarms for closest distance between vessels - using some of r4m0n's timeToClosestApproach function from MechJeb 1.9.8 under GPLv3.0
- Combined Ap/PE and AN/DN alarm buttons to get some more room
- Restructured Save File for future improvements
- Fixed issue with alarms refiring on vessel changes
- Reverted the texture loading to be via KSP.IO to remove compression artifacts from GUI

Version 2.1.2.0		-	KSP Version: 0.20.2
- Verified year calcs
- Adjusted Date Entry for Raw alarms and date displays to show Year 1, Day 1 based stuff so the clocks in tracking station, etc and the alarms all match display wise. Also means you can now create an alarm using the dates from external apps like the transfer pork chops without doing maths
- Fixed bug where Pe Node was not detected if there was no Ap Node on the flight plan
- Fixed bug with autogenerated and auto recalc alarms, by introducing save of the alarms file periodically and on alarm creation (can't rely on memeory in 0.20)
- Removed initial config.xml file from package (and added additional code) so upgrades should now maintain alarms AND settings

Version 2.1.1.0		-	KSP Version: 0.20.2
- Fixed issue with Pause alarms not slowing down warp first

Version 2.1.0.0		-	KSP Version: 0.20.2
- Tweaked for 0.20.2
- Added functionality to Store/Restore Vessel Targets for AN/DN Alarms
- Expanded Store/Restore of Maneuver Nodes to include a list of all nodes from time of alarm
- Resolved an issue with the SOI recalc code that was resetting all SOI Alarms to one time
- Restored the "Jump and Restore" functionality that stopped working in 0.20
- Improved the Save/Load routines to compartmentalise them

Version 2.0.0.0		-	KSP Version: 0.20
- Rebuild and rewrite for 0.20 Game Database Loading Structure
- Also fixes for - only recalc alarms when under Physwarp or nowarp - stops creep at high warp
- Reset Defaults for autocalc to prevent creep
- Reset Defaults for Transfer Mode
- Added position of icon to config.xml so people can move it around as needed

Version 1.4.1.0		-	KSP Version: 0.20
- Recompile and minor changes to package structure to facilitate 0.20 Legacy Mode

Version 1.4.0.3		-	KSP Version: 0.19.1
- Minor changes to facilitate Linux paths and case sensitivity

Version 1.4.0.2		-	KSP Version: 0.19.1
- Changed resource loading method to use direct file access - prevents some peoples issues with timeouts
- Added new Add Alarm window format - big change...
- Added new alarm types - Apoapsis, Periapsis, Ascending Node, Descending Node - thanks to Cybutek for use of his AN/DN calc functions from the Kerbal Engineer
- These alarms can be set to adjust if the flightplan changes
- Added a 2nd form of transfer calculation for transfers between bodies orbiting Kerbol - this uses voneiden'd excellent modelled data
- Ability to disconnect alarm from ships (and see this)
- Added another time format - hh:MM:ss - can toggle between them
- Added extra links to about tab
- And all the things I could remmeber from forum posts/PM's before the great crash :P

Version 1.3.5.1		-	KSP Version: 0.19.1
- New Documentation Site link
- Tidied up a minor GUI layout or two

Version 1.3.5.0		-	KSP Version: 0.19.1
- Added functionality to alarm windows to Delete on Close
- Added Default Settings for Alarm Action, Margin Period and Delete on Close
- Updated margin entry/timeentry control to be more robust and granular (now includes seconds)
- Added Margin to Auto SOI Alarms
- Ability to Edit Margins on existing alarms
- Indicator for alarm being edited
- A few other bugs
- Fixed up lots of annoying GUI design stuff - finalised main,settings, edit and alarm windows - new Add window is next

Version 1.3.3.0		-	KSP Version: 0.19.1
- Tweaked Orbital Transfer lists

Version 1.3.2.0		-	KSP Version: 0.18.4
- Added Transfer Alarms - Alarms that can be set based on Hohmann transfers and formulas by Kosmo-Not used in Olex's calculator - https://forum.kerbalspaceprogram.com/topic/16413-tutorial-interplanetary-how-to-guide/
- Added tooltips for more contextual info
- Tweaked GUI to move interface flags and sizes
- A number of general bugfixes

Version 1.3.0.0		-	KSP Version: 0.18.4
- Added storing and retreiving the maneuver node tied to an alarm
- Added ability to jump to the ship that the alarm was created on
- Add flight path SOI detection and alarm creation - when enabled will automatically create alarms for SOI changes and then adjust and maintain them for you
- Separated alarm storage from main config file - now a file per save game

Version 1.2.0.1		-	KSP Version: 0.18.4
- Resolved a few more GUI Issues

Version 1.2.0.0		-	KSP Version: 0.18.4
- Resolved a few more GUI Issues
- Added Web update check
- Added Settings Pane with a few global options
- Added Sphere of Influence Change detection
- Added Ability to work in UT as well as days,hours
- Added Pause on alarm option
- Added configurable scrolling view of alarms
- Added View/Edit capability to existing Alarms

Version 1.0.1.0		-	KSP Version: 0.18.4
- Resolved some GUI Issues

Version 1.0.0.0		-	KSP Version: 0.18.4
- Initial Release
- Allows for creating Raw Alarms
- Allows for creating Alarms based on Manuever Nodes