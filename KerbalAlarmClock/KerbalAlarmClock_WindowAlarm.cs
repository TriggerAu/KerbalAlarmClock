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
                    if (KACWorkerGameState.CurrentTime.UT >= tmpAlarm.AlarmTime.UT)
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
                                tmpAlarm.AlarmWindow = new Rect((Screen.width / 2) - 160, (Screen.height / 2) - 100, 320, 140);
                                //if (tmpAlarm.PauseGame && Settings.AlarmPosition==1)
                                //    tmpAlarm.AlarmWindow.y -=200;
                                if (Settings.AlarmPosition == 0)
                                    tmpAlarm.AlarmWindow.x = 5;
                                else if (Settings.AlarmPosition == 2)
                                    tmpAlarm.AlarmWindow.x = Screen.width - tmpAlarm.AlarmWindow.width-5;
                            }
                            String strAlarmText = tmpAlarm.Name;
                            switch (tmpAlarm.TypeOfAlarm)
                            {
                                case KACAlarm.AlarmType.Raw:
                                    strAlarmText+= " - Manual";break;
                                case KACAlarm.AlarmType.Maneuver:
                                    strAlarmText+= " - Maneuver Node";break;
                                case KACAlarm.AlarmType.SOIChange:
                                    strAlarmText+= " - SOI Change";break;
                                case KACAlarm.AlarmType.Transfer:
                                    strAlarmText+= " - Transfer Point";break;
                                default:
                                    strAlarmText+= " - Manual";break;
                            }
                            tmpAlarm.AlarmWindow = GUILayout.Window(tmpAlarm.AlarmWindowID, tmpAlarm.AlarmWindow, FillAlarmWindow, strAlarmText, GUILayout.MinWidth(320));
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

            //if the alarm has a vessel ID associated
            if (tmpAlarm.VesselID != "")
            {
                DrawAlarmActionButtons(tmpAlarm);
            }

            //Work out the text
            String strText = "Close Alarm";
            if (tmpAlarm.PauseGame)
            {
                if (FlightDriver.Pause) strText = "Close Alarm and Unpause";
            }
            //Now draw the button
            if (GUILayout.Button(strText, KACResources.styleButton))
            {
                tmpAlarm.AlarmWindowClosed = true;
                if (tmpAlarm.PauseGame)
                    FlightDriver.SetPause(false);
            }
          

            GUILayout.EndVertical();
            GUI.DragWindow();

        }

        private KACAlarm alarmEdit;
        public void FillEditWindow(int WindowID)
        {
            if (alarmEdit.AlarmTime.UT > KACWorkerGameState.CurrentTime.UT)
            {
                //Edit the Alarm if its not yet passed
                int intActionSelected = 0;
                if (alarmEdit.HaltWarp) intActionSelected = 1;
                if (alarmEdit.PauseGame) intActionSelected = 2;
                WindowLayout_AddPane_Common2(ref alarmEdit.Name, ref alarmEdit.Message, ref intActionSelected);
                alarmEdit.HaltWarp = (intActionSelected > 0);
                alarmEdit.PauseGame = (intActionSelected > 1);

                //if the alarm has a vessel ID associated
                if (alarmEdit.VesselID != "")
                {
                    DrawAlarmActionButtons(alarmEdit);
                    
                }

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

                GUILayout.EndVertical();

                if (alarmEdit.VesselID != "")
                {
                    DrawAlarmActionButtons(alarmEdit);
                }

                if (GUILayout.Button("Close Alarm Details", KACResources.styleButton))
                    _ShowEditPane = false;
            }
        }


        private void DrawAlarmActionButtons(KACAlarm tmpAlarm)
        {
            //is it the current vessel?
            if (tmpAlarm.VesselID == KACWorkerGameState.CurrentVessel.id.ToString())
            {
                //There is a node and the alarm + Margin is not expired
                if ((tmpAlarm.ManNode != null) && ((tmpAlarm.Remaining.UT + tmpAlarm.AlarmMarginSecs) > 0))
                {
                    //Check if theres a manuever node and if so put a label saying that it already exists
                    //only display this node button if its the active ship
                    //Add this sae functionality to the alarm triggered window
                    //Add a jump to ship button if not the active ship
                    //As well as to the 
                    String strRestoretext = "Restore Maneuver Node";
                    if (FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Count > 0)
                        strRestoretext += "\r\nNOTE: There is already a Node on the flight plan";
                    if (GUILayout.Button(strRestoretext, KACResources.styleButton))
                    {
                        DebugLogFormatted("Attempting to add Node");
                        RestoreManeuverNode(tmpAlarm.ManNode);
                    }
                }

            }
            else
            {
                //not current vessel
                //There is a node and the alarm + Margin is not expired
                if (tmpAlarm.ManNode != null && tmpAlarm.Remaining.UT + tmpAlarm.AlarmMarginSecs > 0)
                {
                    if (GUILayout.Button("Jump To Ship and Restore Maneuver Node", KACResources.styleButton))
                    {
                        Vessel tmpVessel = FindVesselForAlarm(tmpAlarm);

                        FlightGlobals.SetActiveVessel(tmpVessel);
                        //Set the Node in memory to restore once the ship change has completed
                        manToRestore = tmpAlarm.ManNode;
                    }
                }

                //Or just jump to ship - regardless of alarm tie
                if (GUILayout.Button("Jump To Ship", KACResources.styleButton))
                {
                    Vessel tmpVessel = FindVesselForAlarm(tmpAlarm);
                    // tmpVessel.MakeActive();

                    FlightGlobals.SetActiveVessel(tmpVessel);
                }
            }
        }


        private static Vessel FindVesselForAlarm(KACAlarm tmpAlarm)
        {
            Vessel tmpVessel;
            tmpVessel = FlightGlobals.Vessels.Find(delegate(Vessel v)
            {
                return (tmpAlarm.VesselID == v.id.ToString());
            }
                        );
            return tmpVessel;
        }
        

    }
}
