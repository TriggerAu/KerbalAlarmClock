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

        internal KACTimeStringArray timeDefaultMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeAutoSOIMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeAutoManNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeAutoManNodeThreshold = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);

        private KACTimeStringArray timeQuickManNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeQuickSOIMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeQuickNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);

        private KACTimeStringArray timeContractExpireMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeContractDeadlineMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);

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

            timeContractExpireMargin.BuildFromUT(settings.AlarmOnContractExpireMargin);
            timeContractDeadlineMargin.BuildFromUT(settings.AlarmOnContractDeadlineMargin);

            //timeQuickApNodeMargin.BuildFromUT(settings.AlarmAddApQuickMargin);
            //timeQuickPeNodeMargin.BuildFromUT(settings.AlarmAddPeQuickMargin);
            //timeQuickANNodeMargin.BuildFromUT(settings.AlarmAddANQuickMargin);
            //timeQuickDNNodeMargin.BuildFromUT(settings.AlarmAddDNQuickMargin);

        }

        internal void FillSettingsWindow(int WindowID)
        {
            strAlarmDescSOI = String.Format(strAlarmDescSOI, settings.AlarmAddSOIAutoThreshold.ToString());
            strAlarmDescXfer = String.Format(strAlarmDescXfer, settings.AlarmXferRecalcThreshold.ToString());
            strAlarmDescNode = String.Format(strAlarmDescNode, settings.AlarmNodeRecalcThreshold.ToString());
            strAlarmDescMan = String.Format(strAlarmDescMan, settings.AlarmAddManAutoThreshold.ToString());

            GUILayout.BeginVertical();

            //String[] strSettingsTabs = new String[] { "All Alarms", "Specific Types", "Sounds", "About" };
            //String[] strSettingsTabs = new String[] { "All Alarms", "Specific Types", "About" };
            GUIContent[] contSettingsTabs = new GUIContent[] 
            { 
                new GUIContent("General","Global Settings"), 
                //new GUIContent("Specifics-1","SOI, Ap, Pe, AN, DN Specific Settings" ), 
                //new GUIContent("Specifics-2","Man Node Specific Settings"), 
                //new GUIContent("Alarm Settings","Specific Settings for Alarm Types"), 
                new GUIContent("Specifics","Specific Settings for Alarm Types"), 
                new GUIContent("Audio","Audio Settings"), 
                new GUIContent("Visibility", "Scene and Icon Settings"), 
                new GUIContent("Calendar", "Chosen Calendar and Details"), 
                new GUIContent("About") 
            };
            GUIContent[] contSettingsTabsNewVersion = new GUIContent[] 
            { 
                new GUIContent("All Alarms","Global Settings"), 
                //new GUIContent("Specifics-1","SOI, Ap, Pe, AN, DN Specific Settings" ), 
                //new GUIContent("Specifics-2","Man Node Specific Settings"), 
                //new GUIContent("Alarm Specifics","Specific Settings for Alarm Types"), 
                new GUIContent("Specifics","Specific Settings for Alarm Types"), 
                new GUIContent("Audio","Audio Settings"), 
                new GUIContent("Visibility", "Scene and Icon Settings"), 
                new GUIContent("Calendar", "Chosen Calendar and Details"), 
                new GUIContent(" About", KACResources.btnSettingsAttention) 
            };

            GUIContent[] conTabstoShow = contSettingsTabs;
            if (settings.VersionAvailable) conTabstoShow = contSettingsTabsNewVersion;
            intSettingsTab = GUILayout.Toolbar(intSettingsTab, conTabstoShow, KACResources.styleButton);

            switch (intSettingsTab)
            {
                case 0:
                    WindowLayout_SettingsGlobal();
                    intSettingsHeight = 620;// 591; // 567;// 514; //462; //463; //434;// 572;//542;
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
                    GUILayout.Label("Select Alarm Type:", KACResources.styleAddHeading, GUILayout.Width(120));
                    ddlSettingsAlarmSpecs.DrawButton();
                    GUILayout.EndHorizontal();
                    switch (SettingsAlarmSpecSelected)
                    {
                        case SettingsAlarmSpecsEnum.Default:
                            WindowLayout_SettingsSpecifics_Default();
                            intSettingsHeight = 221; // 234;
                            break;
                        case SettingsAlarmSpecsEnum.WarpTo:
                            WindowLayout_SettingsSpecifics_WarpTo();
                            intSettingsHeight = 477; // 453; // 419;//  395;//221; //318;
                            break;
                        case SettingsAlarmSpecsEnum.ManNode:
                            WindowLayout_SettingsSpecifics_ManNode();
                            intSettingsHeight = 437;// 387; //318;
                            break;
                        case SettingsAlarmSpecsEnum.SOI:
                            WindowLayout_SettingsSpecifics_SOI();
                            intSettingsHeight = 362;// 367; // 358; //288;
                            break;
                        case SettingsAlarmSpecsEnum.Contract:
                            WindowLayout_SettingsSpecifics_Contract();
                            intSettingsHeight = 400;
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
                    WindowLayout_SettingsAudio();
                    intSettingsHeight = 543;
                    break;
                case 3:
                    WindowLayout_SettingsIcons();
                    intSettingsHeight = 509; //518;//466 //406;
                    break;
                case 4:
                    WindowLayout_SettingsCalendar();
                    intSettingsHeight = 226;
                    break;
                case 5:
                    WindowLayout_SettingsAbout();
                    intSettingsHeight = 350; // 294; //306;
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
                        Application.OpenURL("https://forum.kerbalspaceprogram.com/topic/161857-toolbar-continued-common-api-for-draggableresizable-buttons-toolbar/");
                    //intBlizzyToolbarMissingHeight = 18;
                }
            }

            if (DrawCheckbox(ref settings.WindowChildPosBelow, "Show Child Windows Below (not to the side)"))
                settings.Save();
            GUILayout.EndVertical();
            //if (settings.SelectedSkin == Settings.DisplaySkin.Default) GUILayout.Space(38);
            //Preferences
            GUILayout.Label("Plugin Preferences", KACResources.styleAddSectionHeading);

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            GUILayout.BeginHorizontal();
            if (DrawTextBox(ref settings.AlarmListMaxAlarms, KACResources.styleAddField, GUILayout.Width(45)))
                settings.Save();
            GUILayout.Label("Max alarms before scrolling the list", KACResources.styleAddHeading);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if(DrawTextBox(ref settings.MaxToolTipTime, KACResources.styleAddField, GUILayout.Width(45)))
                settings.Save();
            GUILayout.Label("Max time before tooltip is auto-hidden", KACResources.styleAddHeading);
            GUILayout.EndHorizontal();

            if (DrawCheckbox(ref settings.HideOnPause, "Hide Alarm Clock when game is paused"))
                settings.Save();

            if (DrawCheckbox(ref settings.ShowTooltips, "Show Tooltips on Mouse Hover"))
                settings.Save();

            if (DrawCheckbox(ref settings.KillWarpOnThrottleCutOffKeystroke, "Halt TimeWarp when Throttle Cutoff by Keystroke"))
                settings.Save();

            int intTimeFormat = (int)settings.DateTimeFormat;
            if (intTimeFormat > 1) intTimeFormat--;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Time Format:", KACResources.styleAddHeading, GUILayout.Width(90));
            if (DrawRadioList(ref intTimeFormat, new String[] { "UT", "KSP Time", "Normal Time" }))
            {
                if (intTimeFormat > 0) intTimeFormat++;
                settings.DateTimeFormat = (DateStringFormatsEnum)intTimeFormat;
                settings.Save();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Label("Safety Options", KACResources.styleAddSectionHeading);

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            if (DrawCheckbox(ref settings.ConfirmAlarmDeletes, "Confirm before deleting alarms"))
                settings.Save();

            if (DrawCheckbox(ref settings.AllowJumpFromViewOnly, "Allow Ship Jump in Space Center and Tracking Station"))
                settings.Save();

            if (DrawCheckbox(ref settings.AllowJumpToAsteroid, "Allow Ship Jump to Asteroids"))
                settings.Save();

            //if (DrawCheckbox(ref Settings.TimeAsUT, "Display Times as UT (instead of Date/Time)"))
            //    Settings.Save();

            GUILayout.EndVertical();

            GUIContent Saveheader = new GUIContent("Save File Backups", "This option will save your persistent and quicksave files prior to switching ships using the KAC Jump buttons");
            GUILayout.Label(Saveheader, KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas, GUILayout.Height(64));
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

            if (DrawCheckbox(ref settings.WarpTransitions_Instant, new GUIContent("Use Instant Warp Transitions", "Slams the transitions between levels - can cause issues for large timewarp factors")))
                settings.Save();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Transition Weighting:", "How much leeway to give the transitions period. The higher this value the earlier the KAC will slow the warp rate"),
                KACResources.styleAddHeading, GUILayout.Width(115)); //110
            GUILayout.Label(settings.WarpTransitions_UTToRateTimesOneTenths.ToString(), KACResources.styleAddXferName, GUILayout.Width(25));
            Int32 intReturn = (Int32)Math.Floor(GUILayout.HorizontalSlider((float)settings.WarpTransitions_UTToRateTimesOneTenths, 10, 50));
            if (intReturn != settings.WarpTransitions_UTToRateTimesOneTenths)
            {
                settings.WarpTransitions_UTToRateTimesOneTenths = intReturn;
                settings.Save();
            }
            if (GUILayout.Button("Reset", GUILayout.Height(16), GUILayout.Width(40)))
            {
                settings.WarpTransitions_UTToRateTimesOneTenths = 15;
                settings.Save();
            }
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
            if (DrawAlarmActionChoice4(ref settings.AlarmDefaultAction, "Default Action:", 108))
                settings.Save();

            if (DrawTimeEntry(ref timeDefaultMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, "Default Margin:", 100))
            {
                //convert it and save it in the settings
                settings.AlarmDefaultMargin = timeDefaultMargin.UT;
                settings.Save();
            }
            //if (DrawCheckbox(ref settings.AlarmDeleteOnClose, "Delete Alarm On Close"))
            //    settings.Save();

            GUILayout.EndVertical();
        }

        private void WindowLayout_SettingsSpecifics_WarpTo()
        {
            GUILayout.Label("Warp To General Settings", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            if (DrawCheckbox(ref settings.WarpToEnabled, new GUIContent("Enable WarpTo Buttons", "Adds WarpTo Buttons near flight nodes to")))
            {
                settings.Save();
            }
            if (DrawCheckbox(ref settings.WarpToRequiresConfirm, new GUIContent("WarpTo Requires Confirmation (Two-Clicks)", "You need to click twice to use these so a single click doesnt inadvertantly cause a warp")))
            {
                settings.Save();
            }
            if (DrawCheckbox(ref settings.WarpToTipsHidden, new GUIContent("Hide WarpTo Tooltips", "Hides these labels regardless of the tooltips setting")))
            {
                settings.Save();
            }
            if (DrawCheckbox(ref settings.WarpToHideWhenManGizmoShown, new GUIContent("Hide all WarpTo's when Node Gizmo shown", "Hides all warpto stuff when the man node gizmo is visible")))
            {
                settings.Save();
            }

            GUILayout.BeginHorizontal();
            String strTemp = settings.WarpToDupeProximitySecs.ToString("0");
            if (DrawTextBox(ref strTemp, KACResources.styleAddField, GUILayout.Width(45)))
            {
                try
                {
                    settings.WarpToDupeProximitySecs = Convert.ToInt32(strTemp);
                    settings.Save();
                }
                catch (Exception)
                {

                }
            }
            GUILayout.Label("Reuse existing Alarm within if node within X(s)", KACResources.styleAddHeading);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            if (DrawToggle(ref settings.WarpToLimitMaxWarp, "Max Warp Limit", KACResources.styleCheckbox))
                settings.Save();

            if (settings.WarpToLimitMaxWarp)
            {
                GUILayout.Space(200);
                strTemp = settings.WarpToMaxWarp.ToString("0");
                if (DrawTextField(ref strTemp, "\\d+", false, "Limit:", 80, 0))
                {
                    settings.WarpToMaxWarp = Convert.ToInt32(strTemp);
                    settings.Save();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.Label("WarpTo Margins", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            DrawWarpToMarginCheck(ref settings.WarpToAddMarginAp, "Ap", "Ap", KACResources.iconAp);
            DrawWarpToMarginCheck(ref settings.WarpToAddMarginPe, "Pe", "Pe", KACResources.iconPe);
            DrawWarpToMarginCheck(ref settings.WarpToAddMarginAN, "AN", "AN", KACResources.iconAN);
            DrawWarpToMarginCheck(ref settings.WarpToAddMarginDN, "DN", "DN", KACResources.iconDN);
            DrawWarpToMarginCheck(ref settings.WarpToAddMarginSOI, "SOI", "SOI", KACResources.iconSOI);
            DrawWarpToMarginCheck(ref settings.WarpToAddMarginManNode, "Man Node", "Maneuver Node", KACResources.iconMNode);

            GUILayout.EndVertical();
        }

        private void DrawWarpToMarginCheck(ref Boolean settingsBool, String ShortName, String LongName, Texture2D icon)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(icon, GUILayout.Width(20));
            if (DrawCheckbox(ref settingsBool, new GUIContent("Add Margin to " + ShortName + " WarpTo", "Add the configured default Margin to " + LongName + " Alarms when Warping to")))
                settings.Save();
            GUILayout.EndHorizontal();
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
                if (DrawAlarmActionChoice4(ref settings.AlarmAddManAuto_Action, "On Alarm:", 108))
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

            if (DrawAlarmActionChoice4(ref settings.AlarmAddManQuickAction, "Quick Action:", 108))
                settings.Save();

            if (DrawTimeEntry(ref timeQuickManNodeMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, "Quick Margin:", 100))
            {
                //convert it and save it in the settings
                settings.AlarmAddManQuickMargin = timeQuickManNodeMargin.UT;
                settings.Save();
            }
            GUILayout.EndVertical();
            GUILayout.Label("Default Burn Node Margin (KER/VOID)", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            GUILayout.BeginHorizontal();

            GUILayout.Label("Add Burn Time: ", KACResources.styleAddHeading);
            ddlSettingsKERNodeMargin.DrawButton();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        private void WindowLayout_SettingsSpecifics_SOI()
        {
            //Sphere of Influence Stuff
            GUILayout.Label("Sphere Of Influence Alarms", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas, GUILayout.Height(intSOIBoxheight));

            if (DrawCheckbox(ref settings.AlarmSOIRecalc, new GUIContent("Auto Recalc of Manual SOI Alarms")))
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
                if (DrawAlarmActionChoice4(ref settings.AlarmOnSOIChange_Action, "On Alarm:", 108))
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

            if (DrawAlarmActionChoice4(ref settings.AlarmAddSOIQuickAction, "Quick Action:", 108))
                settings.Save();

            if (DrawTimeEntry(ref timeQuickSOIMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, "Quick Margin:", 100))
            {
                //convert it and save it in the settings
                settings.AlarmAddSOIQuickMargin = timeQuickSOIMargin.UT;
                settings.Save();
            }
            GUILayout.EndVertical();

        }

        private void WindowLayout_SettingsSpecifics_Contract()
        {
            GUILayout.Label("Active Contract Alarm Settings", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            if (DrawAlarmActionChoice4(ref settings.AlarmOnContractDeadline_Action, "On Alarm:", 108))
            {
                settings.Save();
            }
            if (DrawTimeEntry(ref timeContractDeadlineMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Days, "Alarm Margin:", 100))
            {
                //convert it and save it in the settings
                settings.AlarmOnContractDeadlineMargin = timeContractDeadlineMargin.UT;
                settings.Save();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Auto Create Active Alarms:");
            ddlSettingsContractAutoActive.DrawButton();
            GUILayout.EndHorizontal();

            if (DrawCheckbox(ref settings.ContractDeadlineDontCreateInsideMargin, "Dont Auto Create Alarm if inside Margin"))
                settings.Save();
            if (DrawCheckbox(ref settings.ContractDeadlineDelete, "Delete Contract Alarm on Deadline passing"))
                settings.Save();

            GUILayout.EndVertical();


            GUILayout.Label("Offered Contract Alarm Settings", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            if (DrawAlarmActionChoice4(ref settings.AlarmOnContractExpire_Action, "On Alarm:", 108))
            {
                settings.Save();
            }
            if (DrawTimeEntry(ref timeContractExpireMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Days, "Alarm Margin:", 100))
            {
                //convert it and save it in the settings
                settings.AlarmOnContractExpireMargin = timeContractExpireMargin.UT;
                settings.Save();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Auto Create Offered Alarms:");
            ddlSettingsContractAutoOffered.DrawButton();
            GUILayout.EndHorizontal();

            if (DrawCheckbox(ref settings.ContractExpireDontCreateInsideMargin, "Dont Auto Create Alarm if inside Margin"))
                settings.Save();
            if (DrawCheckbox(ref settings.ContractExpireDelete, "Delete Contract Alarm on Expiry passing"))
                settings.Save();

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

            if (DrawAlarmActionChoice4(ref settings.AlarmAddNodeQuickAction, "Quick Action:", 108))
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

        private void WindowLayout_SettingsAudio()
        {
            GUILayout.Label("Global Settings", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            //Columns
            GUILayout.BeginHorizontal();
            
            //Column1
            GUILayout.BeginVertical(GUILayout.Width(70));
            GUILayout.Space(0);
            GUILayout.Label("Volume:", KACResources.styleAddSectionHeading);
            GUILayout.Space(4);
            GUILayout.Label("     Level:", KACResources.styleAddHeading);
            GUILayout.EndVertical();

            //Column2
            GUILayout.BeginVertical();
            GUILayout.Space(-5);
            if (DrawToggle(ref settings.AlarmsVolumeFromUI, "Use KSP UI Volume", KACResources.styleCheckbox))
            {
                settings.Save();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (settings.AlarmsVolumeFromUI)
                GUILayout.HorizontalSlider((Int32)(GameSettings.UI_VOLUME * 100), 0, 100, GUILayout.Width(160));
            else
                settings.AlarmsVolume = GUILayout.HorizontalSlider(settings.AlarmsVolume * 100, 0, 100, GUILayout.Width(160)) / 100;
            GUIStyle stylePct = new GUIStyle(KACResources.styleAddHeading);
            stylePct.padding.top = -2;
            GUILayout.Label(KerbalAlarmClock.audioController.VolumePct.ToString() + "%", stylePct);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            
            //End Columns
            GUILayout.EndHorizontal();

            //Draw Raw Sound
            AlarmSound raw = settings.AlarmSounds.First(s => s.Name == "Raw");
            DrawSoundLine(ref raw,true);
            GUILayout.EndVertical();

            GUILayout.Label("Alarm Specifics", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            GUILayout.Label("Enable to select unique sounds or the Raw sound will be used", KACResources.styleAddHeading);

            for (int i = 0; i < settings.AlarmSounds.Count-1; i++)
			{
                AlarmSound sound = settings.AlarmSounds.Where(s => s.Name != "Raw").ElementAt(i);
                DrawSoundLine(ref sound);
            }

            GUILayout.EndVertical();

        }

        private void DrawSoundLine(ref AlarmSound sound,Boolean HideCheck=false)
        {
            GUILayout.BeginHorizontal();

            if (HideCheck) {
                GUILayout.Label("     " + sound.Name, KACResources.styleCheckboxLabel,GUILayout.Width(100));
            } else {
                if (DrawToggle(ref sound.Enabled, sound.Name, KACResources.styleCheckbox,GUILayout.Width(100)))
                {
                    settings.Save();
                }
            }
            sound.ddl.DrawButton();

            if (KACResources.clipAlarms.ContainsKey(sound.SoundName))
                DrawTestSoundButton(KACResources.clipAlarms[sound.SoundName], sound.RepeatCount);
            else
                DrawTestSoundButton(null, sound.RepeatCount);

            GUILayout.Label(new GUIContent("R:","Repeat"),KACResources.styleAddHeading,GUILayout.Width(14));
            //sound.RepeatCount = (Int32)GUILayout.HorizontalSlider(sound.RepeatCount, 1, 6, GUILayout.Width(intTestheight3));
            GUILayout.BeginVertical(GUILayout.Width(60));
            GUILayout.Space(8);
            if (DrawHorizontalSlider(ref sound.RepeatCount, 1, 6, GUILayout.Width(60)))
            {
                settings.Save();
            }
            GUILayout.EndVertical();
            GUILayout.Space(3);
            GUILayout.Label(sound.RepeatCount < 6 ? sound.RepeatCount.ToString() : "6+", KACResources.styleAddHeading, GUILayout.Width(14));

            GUILayout.EndHorizontal();
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
            //        Application.OpenURL("https://forum.kerbalspaceprogram.com/topic/161857-15-toolbar-continued-common-api-for-draggableresizable-buttons-toolbar/");
            //    GUILayout.EndHorizontal();
            //}
            //GUILayout.EndVertical();
            int MinimalDisplayChoice = (int)settings.WindowMinimizedType;

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

            DrawIconPos("Flight Mode", false, ref blnTemp, ref settings.IconPos, ref settings.WindowVisible, ref settings.ClickThroughProtect_Flight);

            DrawIconPos("Space Center", true, ref settings.IconShow_SpaceCenter, ref settings.IconPos_SpaceCenter, ref settings.WindowVisible_SpaceCenter, ref settings.ClickThroughProtect_KSC);

            DrawIconPos("Tracking Station", true, ref settings.IconShow_TrackingStation, ref settings.IconPos_TrackingStation, ref settings.WindowVisible_TrackingStation, ref settings.ClickThroughProtect_Tracking);

            DrawIconPos("Editor", true, ref settings.IconShow_EditorVAB, ref settings.IconPos_EditorVAB, ref settings.WindowVisible_EditorVAB, ref settings.ClickThroughProtect_Editor);

        }

        private void DrawIconPos(String Title, Boolean Toggleable, ref Boolean IconShow, ref Rect IconPos, ref Boolean WindowVisible, ref Boolean ClickThroughProtect)
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

            if (DrawCheckbox(ref ClickThroughProtect, "Prevent Click Through over Windows"))
            {
                settings.Save();
            }

            GUILayout.Label("Icon Position", KACResources.styleAddSectionHeading);
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

        private void WindowLayout_SettingsCalendar()
        {
            //Update Check Area
            GUILayout.Label("General Settings", KACResources.styleAddSectionHeading);

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(60));
            GUILayout.Space(2); //to even up the text
            GUILayout.Label("Calendar:", KACResources.styleAddHeading);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            ddlSettingsCalendar.DrawButton();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            if (DrawToggle(ref settings.ShowCalendarToggle, "Show Calendar Toggle in Main Window", KACResources.styleCheckbox))
                settings.Save();
            GUILayout.EndVertical();


            if (settings.SelectedCalendar == CalendarTypeEnum.Earth)
            {
                GUILayout.Label("Earth Settings", KACResources.styleAddSectionHeading);
                GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Earth Epoch:");

                String strYear, strMonth, strDay;
                strYear = KSPDateStructure.CustomEpochEarth.Year.ToString();
                strMonth = KSPDateStructure.CustomEpochEarth.Month.ToString();
                strDay = KSPDateStructure.CustomEpochEarth.Day.ToString();
                if (DrawYearMonthDay(ref strYear, ref strMonth, ref strDay))
                {
                    try
                    {
                        KSPDateStructure.SetEarthCalendar(strYear.ToInt32(), strMonth.ToInt32(), strDay.ToInt32());
                        settings.EarthEpoch = KSPDateStructure.CustomEpochEarth.ToString("yyyy-MM-dd");
                        settings.Save();
                    }
                    catch (Exception)
                    {
                        LogFormatted("Unable to set the Epoch date using the values provided-{0}-{1}-{2}", strYear, strMonth, strDay);
                    }
                }

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Reset Earth Epoch"))
                {
                    KSPDateStructure.SetEarthCalendar();
                    settings.EarthEpoch = KSPDateStructure.CustomEpochEarth.ToString("1951-01-01");
                    settings.Save();
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            //if RSS not installed and RSS chosen...

            ///section for custom stuff
        }

        private void WindowLayout_SettingsAbout()
        {
            //Update Check Area
            GUILayout.Label("Version Check", KACResources.styleAddSectionHeading);

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas, GUILayout.Height(intUpdateBoxheight));
            GUILayout.BeginHorizontal();
            if (DrawCheckbox(ref settings.DailyVersionCheck, "Check Version Daily"))
                settings.Save();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Check Version Now", KACResources.styleButton))
            {
                settings.VersionCheck(this, true);
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
            GUILayout.Label("Source Code / Downloads:", KACResources.styleAddHeading);
            GUILayout.Label("Forum Page:", KACResources.styleAddHeading);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            //GUILayout.Label("Trigger Au",KACResources.styleContent);
            if (GUILayout.Button("Click Here", KACResources.styleContent))
                Application.OpenURL("https://triggerau.github.io/KerbalAlarmClock/");
            if (GUILayout.Button("Click Here", KACResources.styleContent))
                Application.OpenURL("https://github.com/TriggerAu/KerbalAlarmClock/");
            if (GUILayout.Button("Click Here", KACResources.styleContent))
                Application.OpenURL("https://forum.kerbalspaceprogram.com/topic/22809-kerbal-alarm-clock/");

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            //Alarm Update Area
            GUILayout.Label("v2 Alarm Import", KACResources.styleAddSectionHeading);

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            //GUILayout.Label("Written by:", KACResources.styleAddHeading);
            if (GUILayout.Button("Open Import Tool"))
            {
                winAlarmImport.Visible = true;
                _ShowSettings = false;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            //GUILayout.Label("Trigger Au",KACResources.styleContent);
            if (GUILayout.Button("Import Instructions", KACResources.styleContent))
                Application.OpenURL("https://triggerau.github.io/KerbalAlarmClock/install.html#AlarmImport");
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }

}
