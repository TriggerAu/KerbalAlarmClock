using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using UnityEngine;
using KSP;

namespace KerbalAlarmClock
{
    public partial class KACWorker
    {
        public void FillSettingsWindow(int WindowID)
        {
            GUILayout.BeginVertical();

            //Update Check Area
            GUILayout.Label("Version Check", KACResources.styleAddSectionHeading);

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            GUILayout.BeginHorizontal();
            if (DrawCheckbox2(ref Settings.DailyVersionCheck,"Check Version Daily"))
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
            GUILayout.Label("Last Check Attempt", KACResources.styleAddHeading);
            GUILayout.Label("Last Version from Web", KACResources.styleAddHeading);
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label(Settings.VersionCheckDate_AttemptString, KACResources.styleContent);
            
            if (Settings.VersionAvailable)
                GUILayout.Label(String.Format("{0} @ {1}",Settings.VersionWeb,Settings.VersionCheckDate_SuccessString), KACResources.styleVersionHighlight);
            else
                GUILayout.Label(String.Format("{0} @ {1}", Settings.VersionWeb, Settings.VersionCheckDate_SuccessString), KACResources.styleContent);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (Settings.VersionAvailable)
                GUILayout.Label("Updated Version Available",KACResources.styleVersionHighlight);
            GUILayout.EndVertical();

            //Preferences
            GUILayout.Label("Plugin Preferences", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            //two columns
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("Max Alarms before scrolling window (Approx)", KACResources.styleAddHeading);
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (DrawTextBox(ref Settings.AlarmListMaxAlarms, KACResources.styleAddField))
                Settings.Save();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            //Alarm position
            GUILayout.BeginHorizontal();
            GUILayout.Label("Alarm Position:", KACResources.styleAddHeading, GUILayout.Width(90));
            if (DrawRadioList(ref Settings.AlarmPosition, "Left", "Center", "Right"))
            {
                Settings.Save();
            }
            GUILayout.EndHorizontal();

            if (DrawCheckbox2(ref Settings.HideOnPause, "Hide AlarmClock when game is paused"))
                Settings.Save();

            if (DrawCheckbox2(ref Settings.TimeAsUT, "Display Times as UT (instead of Date/Time)"))
                Settings.Save();

            GUILayout.EndVertical();

            //Preferences
            GUILayout.Label("Global Alarms", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            if (DrawCheckbox2(ref Settings.AlarmAddSOIAuto, "Detect and Add Alarms for SOI Changes"))
                Settings.Save();
            if (!Settings.AlarmAddSOIAuto)
                Settings.AlarmCatchSOIChange = false;
            if (Settings.AlarmAddSOIAuto)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                if (DrawCheckbox2(ref Settings.AlarmCatchSOIChange, "Throw alarm on background SOI Change"))
                    Settings.Save();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                GUILayout.Label("On Alarm:", KACResources.styleAddHeading, GUILayout.Width(70));
                if (DrawRadioList(ref Settings.AlarmOnSOIChange_Action, "Message Only", "Kill Time Warp", "Pause Game"))
                {
                    Settings.Save();
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Space(68);
            }

            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }
    }
}
