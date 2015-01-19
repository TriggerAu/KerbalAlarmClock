using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;

using KSPPluginFramework;

namespace KerbalAlarmClock.Windows
{
    internal class AlarmImport
    {
        internal Int32 windowID;
        internal Rect windowRect;
        internal Boolean Visible;
        internal Boolean ShowOnLeft = false;

        internal KerbalAlarmClock KAC;

        KACAlarmList oldAlarms;
        Boolean AlarmLoadFailed,AlarmsLoaded;
        String LoadMessage;

        internal void InitWindow()
        {
            windowID = UnityEngine.Random.Range(1000, 2000000) + KerbalAlarmClock._AssemblyName.GetHashCode();
            windowRect = new Rect(0, 0, 400, 200);
            Visible = false;
            AlarmLoadFailed = false; AlarmsLoaded = false;
            LoadMessage = "";
        }
        internal void FillWindow(Int32 windowID)
        {
            Rect Toggle = KerbalAlarmClock.GetChildWindowRect(KAC.WindowPosByActiveScene, KAC.WindowPosByActiveScene.y, windowRect.width, windowRect.height,ref ShowOnLeft,KerbalAlarmClock.settings.WindowChildPosBelow);
            windowRect.x = Toggle.x;
            windowRect.y = KAC.WindowPosByActiveScene.y;

            GUILayout.BeginVertical();
            GUILayout.Label("Import Details", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Alarm Save File:");
            GUILayout.TextField(String.Format(String.Format("Alarms-{0}.txt", HighLogic.CurrentGame.Title)), KACResources.styleAddField);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Read File")) {
                AlarmsLoaded = false; AlarmLoadFailed = false;
                if (UtilitiesLegacy.Loadv2Alarms(out LoadMessage,out oldAlarms))
                {
                    AlarmsLoaded = true;
                }
                else
                {
                    AlarmLoadFailed = true;
                }
                windowRect.height = 0;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();


            GUILayout.Label("Found Alarms", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            if (AlarmLoadFailed)
            {
                GUILayout.Label(LoadMessage, KACResources.styleLabelError);
            } else if (AlarmsLoaded) {
                foreach (KACAlarm tmpAlarm in oldAlarms)
                {
                    GUILayout.BeginHorizontal();

                    switch (tmpAlarm.TypeOfAlarm)
                    {
                        case KACAlarm.AlarmTypeEnum.Raw:
                            GUILayout.Label(KACResources.iconRaw, KACResources.styleAlarmIcon);
                            break;
                        case KACAlarm.AlarmTypeEnum.Maneuver:
                        case KACAlarm.AlarmTypeEnum.ManeuverAuto:
                            GUILayout.Label(KACResources.iconMNode, KACResources.styleAlarmIcon);
                            break;
                        case KACAlarm.AlarmTypeEnum.SOIChange:
                        case KACAlarm.AlarmTypeEnum.SOIChangeAuto:
                            GUILayout.Label(KACResources.iconSOI, KACResources.styleAlarmIcon);
                            break;
                        case KACAlarm.AlarmTypeEnum.Transfer:
                        case KACAlarm.AlarmTypeEnum.TransferModelled:
                            GUILayout.Label(KACResources.iconXFer, KACResources.styleAlarmIcon);
                            break;
                        case KACAlarm.AlarmTypeEnum.Apoapsis:
                            GUILayout.Label(KACResources.iconAp, KACResources.styleAlarmIcon);
                            break;
                        case KACAlarm.AlarmTypeEnum.Periapsis:
                            GUILayout.Label(KACResources.iconPe, KACResources.styleAlarmIcon);
                            break;
                        case KACAlarm.AlarmTypeEnum.AscendingNode:
                            GUILayout.Label(KACResources.iconAN, KACResources.styleAlarmIcon);
                            break;
                        case KACAlarm.AlarmTypeEnum.DescendingNode:
                            GUILayout.Label(KACResources.iconDN, KACResources.styleAlarmIcon);
                            break;
                        case KACAlarm.AlarmTypeEnum.LaunchRendevous:
                            GUILayout.Label(KACResources.iconLaunchRendezvous, KACResources.styleAlarmIcon);
                            break;
                        case KACAlarm.AlarmTypeEnum.Closest:
                            GUILayout.Label(KACResources.iconClosest, KACResources.styleAlarmIcon);
                            break;
                        case KACAlarm.AlarmTypeEnum.Distance:
                            //TODO - SOMETHING HERE MAYBE
                            GUILayout.Label(KACResources.iconClosest, KACResources.styleAlarmIcon);
                            break;
                        case KACAlarm.AlarmTypeEnum.Crew:
                            GUILayout.Label(KACResources.iconCrew, KACResources.styleAlarmIcon);
                            break;
                        case KACAlarm.AlarmTypeEnum.EarthTime:
                            GUILayout.Label(KACResources.iconEarth, KACResources.styleAlarmIcon);
                            break;
                        default:
                            GUILayout.Label(KACResources.iconNone, KACResources.styleAlarmIcon);
                            break;
                    }

                    String strLabelText = "";
                    strLabelText = String.Format("{0} ({1})", tmpAlarm.Name, tmpAlarm.Remaining.ToStringStandard(KerbalAlarmClock.settings.TimeSpanFormat,3));
                    GUIStyle styleLabel = new GUIStyle(KACResources.styleAlarmText);
                    GUIContent contAlarmLabel = new GUIContent(strLabelText, tmpAlarm.Notes);
                    GUILayout.Label(contAlarmLabel, styleLabel);
                    GUILayout.EndHorizontal();


                }
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add to Alarms"))
                {
                    foreach (KACAlarm item in oldAlarms)
                    {
                        KerbalAlarmClock.alarms.Add(item);
                    }
                }
                if (GUILayout.Button("Replace Alarms"))
                {
                    KerbalAlarmClock.alarms = oldAlarms;
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("Click the Button above to start the process");
            }
            

            GUILayout.EndVertical();


            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close Import Tool"))
            {
                Visible=false;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            KAC.SetTooltipText();
        }
    }
}
