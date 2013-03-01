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
        //On OnGUI - draw alarms if needed
        public void TriggeredAlarms()
        {
            foreach (KACAlarm tmpAlarm in Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title))
            {
                if (tmpAlarm.Enabled)
                {
                    //also test triggered and actioned
                    if (CurrentTime.UT >= tmpAlarm.AlarmTime.UT)
                    {
                        if (tmpAlarm.Triggered && !tmpAlarm.Actioned)
                        {
                            tmpAlarm.Actioned = true;
                            DebugLogFormatted("Actioning Alarm");
                        }
                        if (tmpAlarm.Actioned && !tmpAlarm.AlarmWindowClosed)
                        {
                            if (tmpAlarm.AlarmWindowID == 0)
                            {
                                tmpAlarm.AlarmWindowID = rnd.Next(1, 2000000);
                                tmpAlarm.AlarmWindow = new Rect((Screen.width / 2) - 160, (Screen.height / 2) - 100, 320, 100);
                                //if (tmpAlarm.PauseGame && Settings.AlarmPosition==1)
                                //    tmpAlarm.AlarmWindow.y -=200;
                                if (Settings.AlarmPosition == 0)
                                    tmpAlarm.AlarmWindow.x = 5;
                                else if (Settings.AlarmPosition == 2)
                                    tmpAlarm.AlarmWindow.x = Screen.width - tmpAlarm.AlarmWindow.width-5;
                            }
                            tmpAlarm.AlarmWindow = GUILayout.Window(tmpAlarm.AlarmWindowID, tmpAlarm.AlarmWindow, FillAlarmWindow, tmpAlarm.Name, GUILayout.MinWidth(320));
                        }
                    }
                }
            }

        }

        public void FillAlarmWindow(int windowID)
        {
            KACAlarm tmpAlarm = Settings.Alarms.GetByWindowID(windowID);

            GUILayout.BeginVertical();

            GUILayout.BeginVertical(GUI.skin.textArea);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Alarm Time:", KACResources.styleAlarmMessageTime);
            if (Settings.TimeAsUT)
                GUILayout.Label(tmpAlarm.AlarmTime.UTString(), KACResources.styleAlarmMessageTime);
            else
                GUILayout.Label(tmpAlarm.AlarmTime.DateString(), KACResources.styleAlarmMessageTime);
            GUILayout.EndHorizontal();
            GUILayout.Label(tmpAlarm.Message, KACResources.styleAlarmMessage);

            if (tmpAlarm.PauseGame)
            {
                if (FlightDriver.Pause)
                    GUILayout.Label("Game paused", KACResources.styleAlarmMessageActionPause);
                else
                    GUILayout.Label("Alarm paused game, but has been unpaused", KACResources.styleAlarmMessageActionPause);
            }
            else if (tmpAlarm.HaltWarp)
            {
                GUILayout.Label("Time Warp Halted", KACResources.styleAlarmMessageAction);
            }
            GUILayout.EndVertical();

            if (tmpAlarm.PauseGame)
            {
                String strText = "Close Alarm";
                if (FlightDriver.Pause) strText = "Close Alarm and Unpause";
                if (GUILayout.Button(strText, KACResources.styleButton))
                {
                    tmpAlarm.AlarmWindowClosed = true;
                    FlightDriver.SetPause(false);
                }
            }
            else
            {
                if (GUILayout.Button("Close Alarm", KACResources.styleButton))
                {
                    tmpAlarm.AlarmWindowClosed = true;
                }
            }

            GUILayout.EndVertical();
            GUI.DragWindow();

        }

        private KACAlarm alarmEdit;
        public void FillEditWindow(int WindowID)
        {
            if (alarmEdit.AlarmTime.UT > CurrentTime.UT)
            {
                //Edit the Alarm if its not yet passed
                int intActionSelected = 0;
                if (alarmEdit.HaltWarp) intActionSelected = 1;
                if (alarmEdit.PauseGame) intActionSelected = 2;
                WindowLayout_AddPane_Common2(ref alarmEdit.Name, ref alarmEdit.Message, ref intActionSelected);
                alarmEdit.HaltWarp = (intActionSelected > 0);
                alarmEdit.PauseGame = (intActionSelected > 1);

                if (GUILayout.Button("Close Alarm Details", KACResources.styleButton))
                {
                    Settings.Save();
                    _ShowEditPane = false;
                }
            }
            else
            {
                //otherwise just show the details
                GUILayout.BeginVertical(GUI.skin.textArea);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Alarm Name:", KACResources.styleAlarmMessageTime);
                GUILayout.Label(alarmEdit.Name, KACResources.styleAlarmMessageTime);
                GUILayout.EndHorizontal();
                GUILayout.Label(alarmEdit.Message, KACResources.styleAlarmMessage);

                if (GUILayout.Button("Close Alarm Details", KACResources.styleButton))
                    _ShowEditPane = false;
                GUILayout.EndVertical();
            }
        }

    }
}
