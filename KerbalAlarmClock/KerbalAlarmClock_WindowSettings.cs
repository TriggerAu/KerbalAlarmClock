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
        int intSOIBoxheight = 166;

        KerbalTimeStringArray timeDefaultMargin = new KerbalTimeStringArray();
        KerbalTimeStringArray timeAutoSOIMargin = new KerbalTimeStringArray();
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
        }

        public void FillSettingsWindow(int WindowID)
        {
            strAlarmDescSOI = String.Format(strAlarmDescSOI, Settings.AlarmAddSOIAutoThreshold.ToString());
            strAlarmDescXfer = String.Format(strAlarmDescXfer, Settings.AlarmXferRecalcThreshold.ToString());

            GUILayout.BeginVertical();

            //String[] strSettingsTabs = new String[] { "All Alarms", "Specific Types", "Sounds", "About" };
            String[] strSettingsTabs = new String[] { "All Alarms","Specific Types","About" };
            GUIContent[] contSettingsTabs = new GUIContent[] { new GUIContent("All Alarms"), new GUIContent("Specific Types"), new GUIContent("About") };
            GUIContent[] contSettingsTabsNewVersion = new GUIContent[] { new GUIContent("All Alarms"), new GUIContent("Specific Types"), new GUIContent(" About",KACResources.btnSettingsAttention) };

            GUIContent[] conTabstoShow = contSettingsTabs;
            if (Settings.VersionAvailable) conTabstoShow = contSettingsTabsNewVersion;
            intSettingsTab = GUILayout.Toolbar(intSettingsTab, conTabstoShow, KACResources.styleButton);
            
            switch (intSettingsTab)
            {
                case 0:
                    WindowLayout_SettingsGlobal();
                    intSettingsHeight = 368;
                    break;
                case 1:
                    WindowLayout_SettingsSpecifics();
                    intSettingsHeight = 374;
                    break;
                case 2:
                    WindowLayout_SettingsAbout();
                    intSettingsHeight = 306;
                    break;
                default:
                    break;
            }

            GUILayout.EndVertical();

            SetTooltipText();
        }

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
                Settings.TimeFormat = (KerbalTime.PrintTimeFormat)intTimeFormat;
                Settings.Save();
            }
            GUILayout.EndHorizontal();

            //if (DrawCheckbox(ref Settings.TimeAsUT, "Display Times as UT (instead of Date/Time)"))
            //    Settings.Save();

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

            if (DrawTimeEntry(ref timeDefaultMargin, TimeEntryPrecision.Hours, "Default Margin:",100))
            {
                //convert it and save it in the settings
                Settings.AlarmDefaultMargin = timeDefaultMargin.UT;
                Settings.Save();
            }
            if (DrawCheckbox(ref Settings.AlarmDeleteOnClose, "Delete Alarm On Close"))
                Settings.Save();

            GUILayout.EndVertical();
        }

        private void WindowLayout_SettingsSpecifics()
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
                if (DrawCheckbox(ref Settings.AlarmCatchSOIChange, new GUIContent("Throw alarm on background SOI Change", "This will throw an alarm whenever the name of the body a ship is orbiting changes.\r\n\r\nIt wont slow time as this approaches, just a big hammer in case we never looked at the flight path before it happened")))
                    Settings.Save();
                GUILayout.EndHorizontal();
                if (DrawAlarmActionChoice(ref Settings.AlarmOnSOIChange_Action, "On Alarm:", 90))
                {
                    Settings.Save();
                }
                if (DrawTimeEntry(ref timeAutoSOIMargin, TimeEntryPrecision.Hours, "Alarm Margin:", 100))
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

            if (Settings.VersionAvailable)
                GUILayout.Label(String.Format("{0} @ {1}", Settings.VersionWeb, Settings.VersionCheckDate_SuccessString), KACResources.styleVersionHighlight);
            else
                GUILayout.Label(String.Format("{0} @ {1}", Settings.VersionWeb, Settings.VersionCheckDate_SuccessString), KACResources.styleContent);
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
