using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;

using UnityEngine;
using KSP;
using KSPPluginFramework;

namespace KerbalAlarmClock
{
    public partial class KerbalAlarmClock
    {
        private Int32 intSettingsTab = 0;
        private Int32 intSettingsHeight = 334;

        private Int32 intAlarmDefaultsBoxheight = 105;
        private Int32 intUpdateBoxheight = 116;
        private Int32 intSOIBoxheight = 178; //166;

        private KACTimeStringArray timeDefaultMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeAutoSOIMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeAutoManNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeAutoManNodeThreshold = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);

        private KACTimeStringArray timeQuickManNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeQuickSOIMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeQuickNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        //private KACTimeStringArray timeQuickApNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        //private KACTimeStringArray timeQuickPeNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        //private KACTimeStringArray timeQuickANNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        //private KACTimeStringArray timeQuickDNNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        
        private void NewSettingsWindow()
        {
            if (settings.VersionAttentionFlag)
            {
                intSettingsTab = 2;
            }
            else
            {
                intSettingsTab = 0;
            }

            //reset the flag
            settings.VersionAttentionFlag = false;

            //work out the correct kerbaltime values
            timeDefaultMargin.BuildFromUT(settings.AlarmDefaultMargin);
            timeAutoSOIMargin.BuildFromUT(settings.AlarmAutoSOIMargin);
            timeAutoManNodeMargin.BuildFromUT(settings.AlarmAddManAutoMargin);
            timeAutoManNodeThreshold.BuildFromUT(settings.AlarmAddManAutoThreshold);

            timeQuickManNodeMargin.BuildFromUT(settings.AlarmAddManQuickMargin);
            timeQuickSOIMargin.BuildFromUT(settings.AlarmAddSOIQuickMargin);
            timeQuickNodeMargin.BuildFromUT(settings.AlarmAddNodeQuickMargin);
            //timeQuickApNodeMargin.BuildFromUT(settings.AlarmAddApQuickMargin);
            //timeQuickPeNodeMargin.BuildFromUT(settings.AlarmAddPeQuickMargin);
            //timeQuickANNodeMargin.BuildFromUT(settings.AlarmAddANQuickMargin);
            //timeQuickDNNodeMargin.BuildFromUT(settings.AlarmAddDNQuickMargin);

        }

        internal void FillSettingsWindow(int WindowID)
        {
            strAlarmDescSOI = String.Format(strAlarmDescSOI, settings.AlarmAddSOIAutoThreshold.ToString());
            strAlarmDescXfer = String.Format(strAlarmDescXfer, settings.AlarmXferRecalcThreshold.ToString());
            strAlarmDescMan = String.Format(strAlarmDescMan, settings.AlarmAddManAutoThreshold.ToString());

            GUILayout.BeginVertical();

            //String[] strSettingsTabs = new String[] { "All Alarms", "Specific Types", "Sounds", "About" };
            String[] strSettingsTabs = new String[] { "All Alarms","Specific Types","About" };
            GUIContent[] contSettingsTabs = new GUIContent[] 
            { 
                new GUIContent("General","Global Settings"), 
                //new GUIContent("Specifics-1","SOI, Ap, Pe, AN, DN Specific Settings" ), 
                //new GUIContent("Specifics-2","Man Node Specific Settings"), 
                new GUIContent("Alarm Settings","Specific Settings for Alarm Types"), 
                new GUIContent("Visibility", "Scene and Icon Settings"), 
                new GUIContent("About") 
            };
            GUIContent[] contSettingsTabsNewVersion = new GUIContent[] 
            { 
                new GUIContent("All Alarms","Global Settings"), 
                //new GUIContent("Specifics-1","SOI, Ap, Pe, AN, DN Specific Settings" ), 
                //new GUIContent("Specifics-2","Man Node Specific Settings"), 
                new GUIContent("Alarm Specifics","Specific Settings for Alarm Types"), 
                new GUIContent("Visibility", "Scene and Icon Settings"), 
                new GUIContent(" About", KACResources.btnSettingsAttention) 
            };

            GUIContent[] conTabstoShow = contSettingsTabs;
            if (settings.VersionAvailable) conTabstoShow = contSettingsTabsNewVersion;
            intSettingsTab = GUILayout.Toolbar(intSettingsTab, conTabstoShow, KACResources.styleButton);
            
            switch (intSettingsTab)
            {
                case 0:
                    WindowLayout_SettingsGlobal();
                    intSettingsHeight = 462; //463; //434;// 572;//542;
                    break;
                //case 1:
                //    WindowLayout_SettingsSpecifics1();
                //    intSettingsHeight = 422;//600; //513;// 374;
                //    break;
                //case 2:
                //    WindowLayout_SettingsSpecifics2();
                //    intSettingsHeight = 354 ;//600; //513;// 374;
                //    break;
                case 1:
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Select Alarm Type:",KACResources.styleAddHeading,GUILayout.Width(120));
                    ddlSettingsAlarmSpecs.DrawButton();
                    GUILayout.EndHorizontal();
                    switch (SettingsAlarmSpecSelected)
	                {
                        case SettingsAlarmSpecsEnum.Default:
                            WindowLayout_SettingsSpecifics_Default();
                            intSettingsHeight = 221; // 234;
                            break;
                        case SettingsAlarmSpecsEnum.ManNode:
                            WindowLayout_SettingsSpecifics_ManNode();
                            intSettingsHeight = 387; //318;
                            break;
                        case SettingsAlarmSpecsEnum.SOI:
                            WindowLayout_SettingsSpecifics_SOI();
                            intSettingsHeight = 362;// 367; // 358; //288;
                            break;
                        case SettingsAlarmSpecsEnum.Other:
                            WindowLayout_SettingsSpecifics_Other();
                            intSettingsHeight = 342; //270;
                            break;
                        default:
                            WindowLayout_SettingsSpecifics_Default();
                            intSettingsHeight = 221; //234;
                            break;
	                }
                    break;
                case 2:
                    WindowLayout_SettingsIcons();
                    intSettingsHeight =  509; //518;//466 //406;
                    break;
                case 3:
                    WindowLayout_SettingsAbout();
                    intSettingsHeight = intTestheight; // 294; //306;
                    break;
                default:
                    break;
            }
            //if (settings.SelectedSkin!= Settings.DisplaySkin.Default)
            //    intSettingsHeight -= intTestheight;
            GUILayout.EndVertical();

            SetTooltipText();
        }

        private void WindowLayout_SettingsGlobal()
        {
            //Styles
            GUILayout.Label("Plugin Styles", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            //two columns
            GUILayout.BeginHorizontal();
            GUILayout.Label("Styling:", KACResources.styleAddHeading, GUILayout.Width(90));
            ddlSettingsSkin.DrawButton();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("App Button:", KACResources.styleAddHeading, GUILayout.Width(90));
            ddlSettingsButtonStyle.DrawButton();
            GUILayout.EndHorizontal();

            //intBlizzyToolbarMissingHeight = 0;
            if (!settings.BlizzyToolbarIsAvailable)
            {
                if (settings.ButtonStyleChosen == Settings.ButtonStyleEnum.Toolbar)
                {
                    if (GUILayout.Button(new GUIContent("Not Installed. Click for Toolbar Info", "Click to open your browser and find out more about the Common Toolbar"), KACResources.styleContent))
                        Application.OpenURL("http://forum.kerbalspaceprogram.com/threads/60863");
                    //intBlizzyToolbarMissingHeight = 18;
                }
            }
            GUILayout.EndVertical();
            //if (settings.SelectedSkin == Settings.DisplaySkin.Default) GUILayout.Space(intTestheight3);
            //Preferences
            GUILayout.Label("Plugin Preferences", KACResources.styleAddSectionHeading);

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            GUILayout.BeginHorizontal();
            if (DrawTextBox(ref settings.AlarmListMaxAlarms, KACResources.styleAddField,GUILayout.Width(45)))
                settings.Save();
            GUILayout.Label("Max alarms before scrolling the list", KACResources.styleAddHeading);
            GUILayout.EndHorizontal();

            if (DrawCheckbox(ref settings.HideOnPause, "Hide Alarm Clock when game is paused"))
                settings.Save();

            if (DrawCheckbox(ref settings.ShowTooltips, "Show Tooltips on Mouse Hover"))
                settings.Save();

            int intTimeFormat = (int)settings.TimeFormat;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Time Format:", KACResources.styleAddHeading, GUILayout.Width(90));
            if (DrawRadioList(ref intTimeFormat, new String[] { "UT", "KSP Time", "Normal Time" }))
            {   
                settings.TimeFormat = (KACTime.PrintTimeFormat)intTimeFormat;
                settings.Save();
            }
            GUILayout.EndHorizontal();

            if (DrawCheckbox(ref settings.AllowJumpFromViewOnly, "Allow Ship Jump in Space Center and Tracking Station"))
                settings.Save();

            if (DrawCheckbox(ref settings.AllowJumpToAsteroid, "Allow Ship Jump to Asteroids"))
                settings.Save();

            //if (DrawCheckbox(ref Settings.TimeAsUT, "Display Times as UT (instead of Date/Time)"))
            //    Settings.Save();

            GUILayout.EndVertical();

            GUIContent Saveheader = new GUIContent("Save File Backups", "This option will save your persistent and quicksave files prior to switching ships using the KAC Jump buttons");
            GUILayout.Label(Saveheader, KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas,GUILayout.Height(64));
            if (DrawCheckbox(ref settings.BackupSaves, "Backup Saves on Ship Jump"))
                settings.Save();

            if (DrawCheckbox(ref settings.CancelFlightModeJumpOnBackupFailure, "Cancel Jump if Backup Fails (Flight Mode Only)"))
                settings.Save();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Backups to Keep:", KACResources.styleAddHeading, GUILayout.Width(110));
            GUILayout.Label(settings.BackupSavesToKeep.ToString(), KACResources.styleAddXferName, GUILayout.Width(25));
            settings.BackupSavesToKeep = (int)Math.Floor(GUILayout.HorizontalSlider((float)settings.BackupSavesToKeep, 3, 50));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.Label("Time Warp/Math Checks", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            GUILayout.BeginHorizontal();

            GUILayout.Label("Checks per Sec:", KACResources.styleAddHeading, GUILayout.Width(100));
            ddlChecksPerSec.DrawButton();
            GUILayout.EndHorizontal();


            GUILayout.EndVertical();

        }


        private void WindowLayout_SettingsSpecifics_Default()
        {
            GUILayout.Label("Alarm Defaults", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas, GUILayout.Height(intAlarmDefaultsBoxheight));

            //Alarm position
            GUILayout.BeginHorizontal();
            GUILayout.Label("Alarm Position:", KACResources.styleAddHeading, GUILayout.Width(90));
            if (DrawRadioList(ref settings.AlarmPosition, "Left", "Center", "Right"))
            {
                settings.Save();
            }
            GUILayout.EndHorizontal();

            //Default Alarm Action
            if (DrawAlarmActionChoice3(ref settings.AlarmDefaultAction, "Default Action:", 108, 61))
                settings.Save();

            if (DrawTimeEntry(ref timeDefaultMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, "Default Margin:", 100))
            {
                //convert it and save it in the settings
                settings.AlarmDefaultMargin = timeDefaultMargin.UT;
                settings.Save();
            }
            if (DrawCheckbox(ref settings.AlarmDeleteOnClose, "Delete Alarm On Close"))
                settings.Save();

            GUILayout.EndVertical();
        }
        private void WindowLayout_SettingsSpecifics_ManNode()
        {
            GUILayout.Label("Maneuver Alarms", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas, GUILayout.Height(207)); //155
            if (DrawCheckbox(ref settings.AlarmAddManAuto, new GUIContent("Detect and Add Alarms for Maneuver Nodes", strAlarmDescMan)))
            {
                settings.Save();
                //if it was turned on then force a recalc regardless of the gap
                //if (Settings.AlarmAddManAuto)
                //{
                //    //RecalcManNodeAlarms(true);
                //}
            }
            if (settings.AlarmAddManAuto)
            {
                if (DrawCheckbox(ref settings.AlarmAddManAuto_andRemove, new GUIContent("Remove Auto Alarms if Maneuver Node Removed")))
                {
                    settings.Save();
                }
                GUILayout.Label("Dont Add New alarms if Man node is closer than this threshold", KACResources.styleAddHeading);
                if (DrawTimeEntry(ref timeAutoManNodeThreshold, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, "Threshold:", 100))
                {
                    //convert it and save it in the settings
                    settings.AlarmAddManAutoThreshold = timeAutoManNodeThreshold.UT;
                    settings.Save();
                }

                GUILayout.Label("Man Node Alarm Settings", KACResources.styleAddSectionHeading);
                if (DrawAlarmActionChoice3(ref settings.AlarmAddManAuto_Action, "On Alarm:", 108, 61))
                {
                    settings.Save();
                }
                if (DrawTimeEntry(ref timeAutoManNodeMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, "Alarm Margin:", 100))
                {
                    //convert it and save it in the settings
                    settings.AlarmAddManAutoMargin = timeAutoManNodeMargin.UT;
                    settings.Save();
                }

            }
            GUILayout.EndVertical();

            GUILayout.Label("Maneuver Quick Alarms", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            if (DrawAlarmActionChoice3(ref settings.AlarmAddManQuickAction, "Quick Action:", 108, 61))
                settings.Save();

            if (DrawTimeEntry(ref timeAutoManNodeMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, "Quick Margin:", 100))
            {
                //convert it and save it in the settings
                settings.AlarmAddManQuickMargin = timeAutoManNodeMargin.UT;
                settings.Save();
            }
            GUILayout.EndVertical();
        }
        private void WindowLayout_SettingsSpecifics_SOI()
        {
            //Sphere of Influence Stuff
            GUILayout.Label("Sphere Of Influence Alarms", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas, GUILayout.Height(intSOIBoxheight));

            if (DrawCheckbox(ref settings.AlarmSOIRecalc, new GUIContent("Auto Recalc of Manual SOI Alarms", strAlarmDescXfer)))
            {
                settings.Save();
                //if it was turned on then force a recalc regardless of the gap
                if (settings.AlarmSOIRecalc)
                {
                    RecalcSOIAlarmTimes(true);
                }
            }

            if (DrawCheckbox(ref settings.AlarmAddSOIAuto, new GUIContent("Detect and Add Alarms for SOI Changes", strAlarmDescSOI)))
                settings.Save();
            //if (!settings.AlarmAddSOIAuto)
            //    settings.AlarmCatchSOIChange = false;
            if (settings.AlarmAddSOIAuto)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                if (DrawCheckbox(ref settings.AlarmAddSOIAuto_ExcludeEVA, new GUIContent("Exclude EVA Kerbals from Auto Alarm", "If an EVA'd Kerbal is on an SOI Path dont create an alarm for this scenario")))
                    settings.Save();
                GUILayout.EndHorizontal();
                GUILayout.Space(-5);
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                if (DrawCheckbox(ref settings.AlarmAddSOIAuto_ExcludeDebris, new GUIContent("Exclude Debris from Auto Alarm", "If Debris is on an SOI Path dont create an alarm for this scenario")))
                    settings.Save();
                GUILayout.EndHorizontal();
                //GUILayout.BeginHorizontal();
                //GUILayout.Space(20);
                //if (DrawCheckbox(ref settings.AlarmCatchSOIChange, new GUIContent("Throw alarm on background SOI Change", "This will throw an alarm whenever the name of the body a ship is orbiting changes.\r\n\r\nIt wont slow time as this approaches, just a big hammer in case we never looked at the flight path before it happened")))
                //    settings.Save();
                //GUILayout.EndHorizontal();
                GUILayout.Label("SOI Alarm Settings", KACResources.styleAddSectionHeading);
                if (DrawAlarmActionChoice3(ref settings.AlarmOnSOIChange_Action, "On Alarm:", 108, 61))
                {
                    settings.Save();
                }
                if (DrawTimeEntry(ref timeAutoSOIMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, "Alarm Margin:", 100))
                {
                    //convert it and save it in the settings
                    settings.AlarmAutoSOIMargin = timeAutoSOIMargin.UT;
                    settings.Save();
                }

            }
            GUILayout.EndVertical();

            GUILayout.Label("SOI Quick Alarms", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            if (DrawAlarmActionChoice3(ref settings.AlarmAddSOIQuickAction, "Quick Action:", 108, 61))
                settings.Save();

            if (DrawTimeEntry(ref timeAutoSOIMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, "Quick Margin:", 100))
            {
                //convert it and save it in the settings
                settings.AlarmAddSOIQuickMargin = timeAutoSOIMargin.UT;
                settings.Save();
            }
            GUILayout.EndVertical();

        }
        private void WindowLayout_SettingsSpecifics_Other()
        {
            //Crew Alarm Stuff
            GUILayout.Label("Crew Alarms", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            if (DrawCheckbox(ref settings.AlarmCrewDefaultStoreNode, new GUIContent("Store Man Node/Target with Crew Alarm")))
            {
                settings.Save();
            }

            GUILayout.EndVertical();

            //Node Alarm Stuff
            GUILayout.Label("Orbital Node Alarms (Ap, Pe, AN, DN)", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            if (DrawCheckbox(ref settings.AlarmNodeRecalc, new GUIContent("Auto Recalc of Node points", strAlarmDescNode)))
            {
                settings.Save();
                //if it was turned on then force a recalc regardless of the gap
                if (settings.AlarmNodeRecalc)
                {
                    RecalcNodeAlarmTimes(true);
                }
            }

            GUILayout.Label("Quick Alarm Settings", KACResources.styleAddSectionHeading);

            if (DrawAlarmActionChoice3(ref settings.AlarmAddNodeQuickAction, "Quick Action:", 108, 61))
                settings.Save();

            if (DrawTimeEntry(ref timeQuickNodeMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, "Quick Margin:", 100))
            {
                //convert it and save it in the settings
                settings.AlarmAddNodeQuickMargin = timeQuickNodeMargin.UT;
                settings.Save();
            }

            GUILayout.EndVertical();

            //Transfer Alarm Stuff
            GUILayout.Label("Transfer Alarms", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            if (DrawCheckbox(ref settings.AlarmXferRecalc, new GUIContent("Auto Recalc of Transfer points", strAlarmDescXfer)))
            {
                settings.Save();
                //if it was turned on then force a recalc regardless of the gap
                if (settings.AlarmXferRecalc)
                {
                    RecalcTransferAlarmTimes(true);
                }
            }
            GUILayout.EndVertical();
        }

        private void WindowLayout_SettingsIcons()
        {
            Boolean blnTemp = false;

            //GUILayout.Label("Common Toolbar Integration (By Blizzy78)", KACResources.styleAddSectionHeading);
            //GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            //if (BlizzyToolbarIsAvailable)
            //{
            //    if (DrawCheckbox(ref settings.UseBlizzyToolbarIfAvailable, "Use Toolbar Button instead of KAC Button"))
            //    {
            //        DestroyToolbarButton(btnToolbarKAC);
            //        if (settings.UseBlizzyToolbarIfAvailable) InitToolbarButton();
            //        settings.Save();
            //    }
            //}
            //else
            //{
            //    GUILayout.BeginHorizontal();
            //    GUILayout.Label("Get the Common Toolbar:", KACResources.styleAddHeading);
            //    GUILayout.FlexibleSpace();
            //    if (GUILayout.Button("Click here", KACResources.styleContent))
            //        Application.OpenURL("http://forum.kerbalspaceprogram.com/threads/60863");
            //    GUILayout.EndHorizontal();
            //}
            //GUILayout.EndVertical();
            int MinimalDisplayChoice=(int)settings.WindowMinimizedType;

            GUILayout.Label("Minimal Mode", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Display What:", KACResources.styleAddHeading, GUILayout.Width(120));
            if (DrawRadioList(ref MinimalDisplayChoice, "Next Alarm", "Oldest Alarm"))
            {
                settings.WindowMinimizedType = (MiminalDisplayType)MinimalDisplayChoice;
                settings.Save();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            DrawIconPos("Flight Mode", false, ref blnTemp, ref settings.IconPos, ref settings.WindowVisible,ref settings.ClickThroughProtect_Flight);

            DrawIconPos("Space Center", true, ref settings.IconShow_SpaceCenter, ref settings.IconPos_SpaceCenter, ref settings.WindowVisible_SpaceCenter, ref settings.ClickThroughProtect_KSC);

            DrawIconPos("Tracking Station", true, ref settings.IconShow_TrackingStation, ref settings.IconPos_TrackingStation,ref settings.WindowVisible_TrackingStation,ref settings.ClickThroughProtect_Tracking);

        }

        private void DrawIconPos(String Title,Boolean Toggleable, ref Boolean IconShow,ref Rect IconPos,ref Boolean WindowVisible,ref Boolean ClickThroughProtect)
        {
            GUILayout.Label(Title, KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            //Checkbox to show/hide
            if (Toggleable)
            {
                if (DrawCheckbox(ref IconShow, new GUIContent("Alarm Clock Visible", "Show the Kerbal Alarm Clock Icon in this Game Mode")))
                {
                    WindowVisible = IconShow;
                    DestroyToolbarButton(btnToolbarKAC);
                    //if (settings.UseBlizzyToolbarIfAvailable) InitToolbarButton();
                    settings.Save();
                }
            }

            if (DrawCheckbox(ref ClickThroughProtect, "Prevent Click Through over Windows")) {
                    settings.Save();
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
            if (DrawCheckbox(ref settings.DailyVersionCheck, "Check Version Daily"))
                settings.Save();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Check Version Now", KACResources.styleButton))
            {
                settings.VersionCheck(true);
                //Hide the flag as we already have the window open;
                settings.VersionAttentionFlag = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Last Check Attempt:", KACResources.styleAddHeading);
            GUILayout.Label("Last Version from Web:", KACResources.styleAddHeading);
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label(settings.VersionCheckDate_AttemptString, KACResources.styleContent);

            if (settings.VersionCheckRunning)
            {
                Int32 intDots = Convert.ToInt32(Math.Truncate(DateTime.Now.Millisecond / 250d)) + 1;
                GUILayout.Label(String.Format("{0} Checking", new String('.', intDots)), KACResources.styleVersionHighlight);
            }
            else
            {
                if (settings.VersionAvailable)
                    GUILayout.Label(String.Format("{0} @ {1}", settings.VersionWeb, settings.VersionCheckDate_SuccessString), KACResources.styleVersionHighlight);
                else
                    GUILayout.Label(String.Format("{0} @ {1}", settings.VersionWeb, settings.VersionCheckDate_SuccessString), KACResources.styleContent);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (settings.VersionAvailable)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(80);
                if (GUILayout.Button("Updated Version Available - Click Here", KACResources.styleVersionHighlight))
                    Application.OpenURL("https://github.com/TriggerAu/KerbalAlarmClock/releases");
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            //About Area
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
                Application.OpenURL("http://triggerau.github.io/KerbalAlarmClock/");
            if (GUILayout.Button("Click Here", KACResources.styleContent))
                Application.OpenURL("http://github.com/TriggerAu/KerbalAlarmClock/");
            if (GUILayout.Button("Click Here", KACResources.styleContent))
                Application.OpenURL("http://forum.kerbalspaceprogram.com/showthread.php/24786-Kerbal-Alarm-Clock");

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            //Alarm Update Area
            GUILayout.Label("v2 Alarm Import", KACResources.styleAddSectionHeading);

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            //GUILayout.Label("Written by:", KACResources.styleAddHeading);
            if (GUILayout.Button("Open Import Tool")){
                winAlarmImport.Visible = true;
                _ShowSettings = false;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            //GUILayout.Label("Trigger Au",KACResources.styleContent);
            if (GUILayout.Button("Import Instructions", KACResources.styleContent))
                Application.OpenURL("http://triggerau.github.io/KerbalAlarmClock/Install.html#AlarmImport");
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }

}
