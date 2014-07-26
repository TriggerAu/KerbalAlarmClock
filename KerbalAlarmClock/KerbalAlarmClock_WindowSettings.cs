using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;

using UnityEngine;
using KSP;

namespace KerbalAlarmClock
{
    public partial class KACWorker
    {
        int intSettingsTab = 0;
        int intSettingsHeight = 334;

        int intAlarmDefaultsBoxheight = 105;
        int intUpdateBoxheight = 116;
        int intSOIBoxheight = 214; //166;

        KACTimeStringArray timeDefaultMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        KACTimeStringArray timeAutoSOIMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        KACTimeStringArray timeAutoManNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        KACTimeStringArray timeAutoManNodeThreshold = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private void NewSettingsWindow()
        {
            if (Settings.VersionAttentionFlag)
            {
                intSettingsTab = 2;
            }
            else
            {
                intSettingsTab = 0;
            }

            //reset the flag
            Settings.VersionAttentionFlag = false;

            //work out the correct kerbaltime values
            timeDefaultMargin.BuildFromUT(Settings.AlarmDefaultMargin);
            timeAutoSOIMargin.BuildFromUT(Settings.AlarmAutoSOIMargin);
            timeAutoManNodeMargin.BuildFromUT(Settings.AlarmAddManAutoMargin);
            timeAutoManNodeThreshold.BuildFromUT(Settings.AlarmAddManAutoThreshold);

            contChecksPerSecChoices =  new GUIContent[] { 
                                new GUIContent("10"), 
                                new GUIContent("20"), 
                                new GUIContent("50"), 
                                new GUIContent("100"), 
                                new GUIContent("Custom (" + Settings.BehaviourChecksPerSec_Custom.ToString() + ")")
                            };
            switch (Settings.BehaviourChecksPerSec)
            {
                case 10: intChecksPerSecChoice = 0; break;
                case 20: intChecksPerSecChoice = 1; break;
                case 50: intChecksPerSecChoice = 2; break;
                case 100: intChecksPerSecChoice = 3; break;
                default: intChecksPerSecChoice = 4; break;
            }
        }

        public void FillSettingsWindow(int WindowID)
        {
            strAlarmDescSOI = String.Format(strAlarmDescSOI, Settings.AlarmAddSOIAutoThreshold.ToString());
            strAlarmDescXfer = String.Format(strAlarmDescXfer, Settings.AlarmXferRecalcThreshold.ToString());
            strAlarmDescMan = String.Format(strAlarmDescMan, Settings.AlarmAddManAutoThreshold.ToString());

            GUILayout.BeginVertical();

            //String[] strSettingsTabs = new String[] { "All Alarms", "Specific Types", "Sounds", "About" };
            String[] strSettingsTabs = new String[] { "All Alarms","Specific Types","About" };
            GUIContent[] contSettingsTabs = new GUIContent[] 
            { 
                new GUIContent("All Alarms","Global Settings"), 
                new GUIContent("Specifics-1","SOI, Ap, Pe, AN, DN Specific Settings" ), 
                new GUIContent("Specifics-2","Man Node Specific Settings"), 
                new GUIContent("Visibility", "Scene and Icon Settings"), 
                new GUIContent("About") 
            };
            GUIContent[] contSettingsTabsNewVersion = new GUIContent[] 
            { 
                new GUIContent("All Alarms","Global Settings"), 
                new GUIContent("Specifics-1","SOI, Ap, Pe, AN, DN Specific Settings" ), 
                new GUIContent("Specifics-2","Man Node Specific Settings"), 
                new GUIContent("Visibility", "Scene and Icon Settings"), 
                new GUIContent(" About", KACResources.btnSettingsAttention) 
            };

            GUIContent[] conTabstoShow = contSettingsTabs;
            if (Settings.VersionAvailable) conTabstoShow = contSettingsTabsNewVersion;
            intSettingsTab = GUILayout.Toolbar(intSettingsTab, conTabstoShow, KACResources.styleButton);
            
            switch (intSettingsTab)
            {
                case 0:
                    WindowLayout_SettingsGlobal();
                    intSettingsHeight = 602;// 572;//542;
                    break;
                case 1:
                    WindowLayout_SettingsSpecifics1();
                    intSettingsHeight = 422;//600; //513;// 374;
                    break;
                case 2:
                    WindowLayout_SettingsSpecifics2();
                    intSettingsHeight = 354;//600; //513;// 374;
                    break;
                case 3:
                    WindowLayout_SettingsIcons();
                    intSettingsHeight =  548;//518 //466 //406;
                    break;
                case 4:
                    WindowLayout_SettingsAbout();
                    intSettingsHeight = 306;
                    break;
                default:
                    break;
            }

            GUILayout.EndVertical();

            SetTooltipText();
        }

        int intChecksPerSecChoice = 0;
        GUIContent[] contChecksPerSecChoices;
        private void WindowLayout_SettingsGlobal()
        {
            //Preferences
            GUILayout.Label("Plugin Preferences", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            //two columns
            GUILayout.BeginHorizontal();
            if (DrawTextBox(ref Settings.AlarmListMaxAlarms, KACResources.styleAddField,GUILayout.Width(45)))
                Settings.Save();
            GUILayout.Label("Max alarms before scrolling the list", KACResources.styleAddHeading);
            GUILayout.EndHorizontal();

            if (DrawCheckbox(ref Settings.HideOnPause, "Hide Alarm Clock when game is paused"))
                Settings.Save();

            if (DrawCheckbox(ref Settings.ShowTooltips, "Show Tooltips on Mouse Hover"))
                Settings.Save();

            int intTimeFormat = (int)Settings.TimeFormat;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Time Format:", KACResources.styleAddHeading, GUILayout.Width(90));
            if (DrawRadioList(ref intTimeFormat, new String[] { "UT", "KSP Time", "Normal Time" }))
            {   
                Settings.TimeFormat = (KACTime.PrintTimeFormat)intTimeFormat;
                Settings.SaveConfig();
            }
            GUILayout.EndHorizontal();

            if (DrawCheckbox(ref Settings.AllowJumpFromViewOnly, "Allow Ship Jump in Space Center and Tracking Station"))
                Settings.Save();

            if (DrawCheckbox(ref Settings.AllowJumpToAsteroid, "Allow Ship Jump to Asteroids"))
                Settings.Save();

            //if (DrawCheckbox(ref Settings.TimeAsUT, "Display Times as UT (instead of Date/Time)"))
            //    Settings.Save();

            GUILayout.EndVertical();

            GUIContent Saveheader = new GUIContent("Save File Backups", "This option will save your persistent and quicksave files prior to switching ships using the KAC Jump buttons");
            GUILayout.Label(Saveheader, KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas,GUILayout.Height(64));
            if (DrawCheckbox(ref Settings.BackupSaves, "Backup Saves on Ship Jump"))
                Settings.Save();

            if (DrawCheckbox(ref Settings.CancelFlightModeJumpOnBackupFailure, "Cancel Jump if Backup Fails (Flight Mode Only)"))
                Settings.Save();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Backups to Keep:", KACResources.styleAddHeading, GUILayout.Width(110));
            GUILayout.Label(Settings.BackupSavesToKeep.ToString(), KACResources.styleAddXferName, GUILayout.Width(25));
            Settings.BackupSavesToKeep = (int)Math.Floor(GUILayout.HorizontalSlider((float)Settings.BackupSavesToKeep, 3, 50));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.Label("Time Warp/Math Checks", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            GUILayout.BeginHorizontal();

            GUILayout.Label("Checks per Sec:", KACResources.styleAddHeading, GUILayout.Width(100));
            if (DrawButtonList(ref intChecksPerSecChoice,contChecksPerSecChoices))
            {
                switch (intChecksPerSecChoice)
                {
                    case 0: Settings.BehaviourChecksPerSec = 10; break;
                    case 1: Settings.BehaviourChecksPerSec = 20; break;
                    case 2: Settings.BehaviourChecksPerSec = 50; break;
                    case 3: Settings.BehaviourChecksPerSec = 100; break;
                    default: Settings.BehaviourChecksPerSec = Settings.BehaviourChecksPerSec_Custom; break;
                }
                parentBehaviour.SetupRepeatingFunction_BehaviourUpdate( Settings.BehaviourChecksPerSec);
                Settings.SaveConfig();
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.Label("Alarm Defaults", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas, GUILayout.Height(intAlarmDefaultsBoxheight));

            //Alarm position
            GUILayout.BeginHorizontal();
            GUILayout.Label("Alarm Position:", KACResources.styleAddHeading, GUILayout.Width(90));
            if (DrawRadioList(ref Settings.AlarmPosition, "Left", "Center", "Right"))
            {
                Settings.Save();
            }
            GUILayout.EndHorizontal();

            //Default Alarm Action
            if (DrawAlarmActionChoice(ref Settings.AlarmDefaultAction, "Default Action:", 90))
                Settings.Save();

            if (DrawTimeEntry(ref timeDefaultMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, "Default Margin:", 100))
            {
                //convert it and save it in the settings
                Settings.AlarmDefaultMargin = timeDefaultMargin.UT;
                Settings.Save();
            }
            if (DrawCheckbox(ref Settings.AlarmDeleteOnClose, "Delete Alarm On Close"))
                Settings.Save();

            GUILayout.EndVertical();
        }

        private void WindowLayout_SettingsSpecifics1()
        {
            //Sperer of Influence Stuff
            GUILayout.Label("Sphere Of Influence Alarms", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas, GUILayout.Height(intSOIBoxheight));

            if (DrawCheckbox(ref Settings.AlarmSOIRecalc, new GUIContent("Auto Recalc of Manual SOI Alarms", strAlarmDescXfer)))
            {
                Settings.Save();
                //if it was turned on then force a recalc regardless of the gap
                if (Settings.AlarmSOIRecalc)
                {
                    RecalcSOIAlarmTimes(true);
                }
            }

            if (DrawCheckbox(ref Settings.AlarmAddSOIAuto, new GUIContent("Detect and Add Alarms for SOI Changes", strAlarmDescSOI)))
                Settings.Save();
            if (!Settings.AlarmAddSOIAuto)
                Settings.AlarmCatchSOIChange = false;
            if (Settings.AlarmAddSOIAuto)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                if (DrawCheckbox(ref Settings.AlarmAddSOIAuto_ExcludeEVA, new GUIContent("Exclude EVA Kerbals from Auto Alarm", "If an EVA'd Kerbal is on an SOI Path dont create an alarm for this scenario")))
                    Settings.Save();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                if (DrawCheckbox(ref Settings.AlarmCatchSOIChange, new GUIContent("Throw alarm on background SOI Change", "This will throw an alarm whenever the name of the body a ship is orbiting changes.\r\n\r\nIt wont slow time as this approaches, just a big hammer in case we never looked at the flight path before it happened")))
                    Settings.Save();
                GUILayout.EndHorizontal();
                GUILayout.Label("SOI Alarm Settings", KACResources.styleAddSectionHeading);
                if (DrawAlarmActionChoice(ref Settings.AlarmOnSOIChange_Action, "On Alarm:", 90))
                {
                    Settings.Save();
                }
                if (DrawTimeEntry(ref timeAutoSOIMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, "Alarm Margin:", 100))
                {
                    //convert it and save it in the settings
                    Settings.AlarmAutoSOIMargin = timeAutoSOIMargin.UT;
                    Settings.Save();
                }

            }
            GUILayout.EndVertical();

            //Transfer Alarm Stuff
            GUILayout.Label("Transfer Alarms", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            if (DrawCheckbox(ref Settings.AlarmXferRecalc, new GUIContent("Auto Recalc of Transfer points", strAlarmDescXfer)))
            {
                Settings.Save();
                //if it was turned on then force a recalc regardless of the gap
                if (Settings.AlarmXferRecalc)
                {
                    RecalcTransferAlarmTimes(true);
                }
            }
            GUILayout.EndVertical();

            //Node Alarm Stuff
            GUILayout.Label("Orbital Node Alarms (Ap, Pe, AN, DN)", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            if (DrawCheckbox(ref Settings.AlarmNodeRecalc, new GUIContent("Auto Recalc of Node points", strAlarmDescNode)))
            {
                Settings.Save();
                //if it was turned on then force a recalc regardless of the gap
                if (Settings.AlarmNodeRecalc)
                {
                    RecalcNodeAlarmTimes(true);
                }
            }
            GUILayout.EndVertical();
        }

        private void WindowLayout_SettingsSpecifics2()
        {
            //Transfer Alarm Stuff
            GUILayout.Label("Maneuver Alarms", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas,GUILayout.Height(207)); //155
            if (DrawCheckbox(ref Settings.AlarmAddManAuto, new GUIContent("Detect and Add Alarms for Maneuver Nodes", strAlarmDescMan)))
            {
                Settings.Save();
                //if it was turned on then force a recalc regardless of the gap
                //if (Settings.AlarmAddManAuto)
                //{
                //    //RecalcManNodeAlarms(true);
                //}
            }
            if (Settings.AlarmAddManAuto)
            {
                if (DrawCheckbox(ref Settings.AlarmAddManAuto_andRemove, new GUIContent("Remove Auto Alarms if Maneuver Node Removed")))
                {
                    Settings.Save();
                }
                GUILayout.Label("Dont Add New alarms if Man node is closer than this threshold", KACResources.styleAddHeading);
                if (DrawTimeEntry(ref timeAutoManNodeThreshold , KACTimeStringArray.TimeEntryPrecisionEnum.Hours, "Threshold:", 100))
                {
                    //convert it and save it in the settings
                    Settings.AlarmAddManAutoThreshold= timeAutoManNodeThreshold.UT;
                    Settings.Save();
                }

                GUILayout.Label("Man Node Alarm Settings", KACResources.styleAddSectionHeading);
                if (DrawAlarmActionChoice(ref Settings.AlarmAddManAuto_Action, "On Alarm:", 90))
                {
                    Settings.Save();
                }
                if (DrawTimeEntry(ref timeAutoManNodeMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, "Alarm Margin:", 100))
                {
                    //convert it and save it in the settings
                    Settings.AlarmAddManAutoMargin = timeAutoManNodeMargin.UT;
                    Settings.Save();
                }

            }
            GUILayout.EndVertical();

            //Crew Alarm Stuff
            GUILayout.Label("Crew Alarms", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            if (DrawCheckbox(ref Settings.AlarmCrewDefaultStoreNode, new GUIContent("Store Man Node/Target with Crew Alarm")))
            {
                Settings.Save();
            }

            GUILayout.EndVertical();

        }


        private void WindowLayout_SettingsIcons()
        {
            Boolean blnTemp = false;

            GUILayout.Label("Common Toolbar Integration (By Blizzy78)", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            if (parentBehaviour.BlizzyToolbarIsAvailable)
            {
                if (DrawCheckbox(ref Settings.UseBlizzyToolbarIfAvailable, "Use Toolbar Button instead of KAC Button"))
                {
                    parentBehaviour.DestroyToolbarButton(btnToolbarKAC);
                    if (Settings.UseBlizzyToolbarIfAvailable) parentBehaviour.InitToolbarButton();
                    Settings.Save();
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Get the Common Toolbar:", KACResources.styleAddHeading);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Click here", KACResources.styleContent))
                    Application.OpenURL("http://forum.kerbalspaceprogram.com/threads/60863");
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            int MinimalDisplayChoice=(int)Settings.WindowMinimizedType;

            GUILayout.Label("Minimal Mode", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Display What:", KACResources.styleAddHeading, GUILayout.Width(120));
            if (DrawRadioList(ref MinimalDisplayChoice, "Next Alarm", "Oldest Alarm"))
            {
                Settings.WindowMinimizedType = (MiminalDisplayType)MinimalDisplayChoice;
                Settings.SaveConfig();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            DrawIconPos("Flight Mode", false, ref blnTemp, ref Settings.IconPos, ref Settings.WindowVisible);

            DrawIconPos("Space Center", true, ref Settings.IconShow_SpaceCenter, ref Settings.IconPos_SpaceCenter, ref Settings.WindowVisible_SpaceCenter);

            DrawIconPos("Tracking Station", true, ref Settings.IconShow_TrackingStation, ref Settings.IconPos_TrackingStation,ref Settings.WindowVisible_TrackingStation);

        }

        private void DrawIconPos(String Title,Boolean Toggleable, ref Boolean IconShow,ref Rect IconPos,ref Boolean WindowVisible)
        {
            GUILayout.Label(Title, KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            //Checkbox to show/hide
            if (Toggleable)
            {
                if (DrawCheckbox(ref IconShow, new GUIContent("Alarm Clock Visible", "Show the Kerbal Alarm Clock Icon in this Game Mode")))
                {
                    WindowVisible = IconShow;
                    parentBehaviour.DestroyToolbarButton(btnToolbarKAC);
                    if (Settings.UseBlizzyToolbarIfAvailable) parentBehaviour.InitToolbarButton();
                    Settings.Save();
                }
            }
            if (Title == "Space Center")
            {
                if (DrawCheckbox(ref Settings.KSCUIBlockersEnabled, new GUIContent("Enable UI Click Through Blockers", "This will insert invisible buttons between the Mod and the game interface and prevent clicking on the GUI from clicking the buildings underneath")))
                    Settings.Save();
            }
            GUILayout.Label("Icon Position",KACResources.styleAddSectionHeading);
            //Now two columns
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Horizontal: ", KACResources.styleAddHeading);
            GUILayout.Label(string.Format("{0}", Math.Floor((IconPos.xMin)).ToString()), KACResources.styleAddXferName, GUILayout.Width(50));
            GUILayout.EndHorizontal(); 
            IconPos.xMin = Convert.ToInt32(Math.Floor(GUILayout.HorizontalSlider(IconPos.xMin, 0, Screen.width - 32)));
            IconPos.xMax = IconPos.xMin + 32;
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(); 
            GUILayout.Label("Vertical: ", KACResources.styleAddHeading);
            GUILayout.Label(string.Format("{0}", Math.Floor((IconPos.yMin)).ToString()), KACResources.styleAddXferName, GUILayout.Width(50));
            GUILayout.EndHorizontal(); 
            IconPos.yMin = Convert.ToInt32(Math.Floor(GUILayout.HorizontalSlider(IconPos.yMin, 0, Screen.height - 32)));
            IconPos.yMax = IconPos.yMin + 32;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

        }

        private void WindowLayout_SettingsAbout()
        {
            //Update Check Area
            GUILayout.Label("Version Check", KACResources.styleAddSectionHeading);

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas,GUILayout.Height(intUpdateBoxheight));
            GUILayout.BeginHorizontal();
            if (DrawCheckbox(ref Settings.DailyVersionCheck, "Check Version Daily"))
                Settings.Save();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Check Version Now", KACResources.styleButton))
            {
                Settings.VersionCheck(true);
                //Hide the flag as we already have the window open;
                Settings.VersionAttentionFlag = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Last Check Attempt:", KACResources.styleAddHeading);
            GUILayout.Label("Last Version from Web:", KACResources.styleAddHeading);
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label(Settings.VersionCheckDate_AttemptString, KACResources.styleContent);

            if (Settings.VersionCheckRunning)
            {
                Int32 intDots = Convert.ToInt32(Math.Truncate(DateTime.Now.Millisecond/250d)) + 1;
                GUILayout.Label(String.Format("{0} Checking", new String('.', intDots)), KACResources.styleVersionHighlight);
            }
            else
            {
                if (Settings.VersionAvailable)
                    GUILayout.Label(String.Format("{0} @ {1}", Settings.VersionWeb, Settings.VersionCheckDate_SuccessString), KACResources.styleVersionHighlight);
                else
                    GUILayout.Label(String.Format("{0} @ {1}", Settings.VersionWeb, Settings.VersionCheckDate_SuccessString), KACResources.styleContent);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (Settings.VersionAvailable)
                GUILayout.Label("Updated Version Available", KACResources.styleVersionHighlight);
            GUILayout.EndVertical();

            //Update Check Area
            GUILayout.Label("About", KACResources.styleAddSectionHeading);

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            //GUILayout.Label("Written by:", KACResources.styleAddHeading);
            GUILayout.Label("Documentation and Links:", KACResources.styleAddHeading);
            GUILayout.Label("Spaceport Page:", KACResources.styleAddHeading);
            GUILayout.Label("Forum Page:", KACResources.styleAddHeading);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            //GUILayout.Label("Trigger Au",KACResources.styleContent);
            if (GUILayout.Button("Click Here", KACResources.styleContent))
                Application.OpenURL("https://sites.google.com/site/kerbalalarmclock/");
            if (GUILayout.Button("Click Here", KACResources.styleContent))
                Application.OpenURL("http://kerbalspaceport.com/kerbal-alarm-clock-2/");
            if (GUILayout.Button("Click Here", KACResources.styleContent))
                Application.OpenURL("http://forum.kerbalspaceprogram.com/showthread.php/24786-Kerbal-Alarm-Clock");

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }

}
