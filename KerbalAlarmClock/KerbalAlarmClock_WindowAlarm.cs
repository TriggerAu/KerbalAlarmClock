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
        //On OnGUI - draw alarms if needed
        public void TriggeredAlarms()
        {
            foreach (KACAlarm tmpAlarm in Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title))
            {
                if (tmpAlarm.Enabled)
                {
                    //also test triggered and actioned
                    //if (KACWorkerGameState.CurrentTime.UT >= tmpAlarm.AlarmTime.UT)
                    if ((tmpAlarm.Remaining.UT<=0))
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
                                tmpAlarm.AlarmWindow = new Rect((Screen.width / 2) - 160, (Screen.height / 2) - 100, 320, tmpAlarm.AlarmWindowHeight);
                                if (Settings.AlarmPosition == 0)
                                    tmpAlarm.AlarmWindow.x = 5;
                                else if (Settings.AlarmPosition == 2)
                                    tmpAlarm.AlarmWindow.x = Screen.width - tmpAlarm.AlarmWindow.width - 5;

                                tmpAlarm.DeleteOnClose = Settings.AlarmDeleteOnClose;
                            }
                            else
                            {
                                tmpAlarm.AlarmWindow.height = tmpAlarm.AlarmWindowHeight;
                            }
                            String strAlarmText = tmpAlarm.Name;
                            
                            switch (tmpAlarm.TypeOfAlarm)
                            {
                                case KACAlarm.AlarmType.Raw:
                                    strAlarmText+= " - Manual";break;
                                case KACAlarm.AlarmType.Maneuver:
                                    strAlarmText+= " - Maneuver Node";break;
                                case KACAlarm.AlarmType.SOIChange:
                                case KACAlarm.AlarmType.SOIChangeAuto:
                                    strAlarmText += " - SOI Change"; break;
                                case KACAlarm.AlarmType.Transfer:
                                case KACAlarm.AlarmType.TransferModelled:
                                    strAlarmText += " - Transfer Point"; break;
                                case KACAlarm.AlarmType.Apoapsis:
                                    strAlarmText += " - Apoapsis"; break;
                                case KACAlarm.AlarmType.Periapsis:
                                    strAlarmText += " - Periapsis"; break;
                                case KACAlarm.AlarmType.AscendingNode:
                                    strAlarmText += " - Ascending Node"; break;
                                case KACAlarm.AlarmType.DescendingNode:
                                    strAlarmText += " - Descending Node"; break;
                                case KACAlarm.AlarmType.LaunchRendevous:
                                    strAlarmText += " - Launch Rendevous"; break;
                                case KACAlarm.AlarmType.Closest:
                                    strAlarmText += " - Closest Approach"; break;
                                case KACAlarm.AlarmType.EarthTime:
                                    strAlarmText += " - Earth Alarm"; break;
                                case KACAlarm.AlarmType.Crew:
                                    strAlarmText += " - Kerbal Alarm"; break;
                                default:
                                    strAlarmText+= " - Manual";break;
                            }
                            tmpAlarm.AlarmWindow = GUILayout.Window(tmpAlarm.AlarmWindowID, tmpAlarm.AlarmWindow, FillAlarmWindow, strAlarmText, KACResources.styleWindow,GUILayout.MinWidth(320));
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
            if (tmpAlarm.TypeOfAlarm!= KACAlarm.AlarmType.EarthTime)
                GUILayout.Label(KACTime.PrintDate(tmpAlarm.AlarmTime, Settings.TimeFormat), KACResources.styleAlarmMessageTime);
            else
                GUILayout.Label(EarthTimeDecode(tmpAlarm.AlarmTime.UT).ToLongTimeString(), KACResources.styleAlarmMessageTime);
            if (tmpAlarm.TypeOfAlarm != KACAlarm.AlarmType.Raw && tmpAlarm.TypeOfAlarm != KACAlarm.AlarmType.EarthTime && tmpAlarm.TypeOfAlarm != KACAlarm.AlarmType.Crew)
                GUILayout.Label("(m: " + KACTime.PrintInterval(new KACTime(tmpAlarm.AlarmMarginSecs),3, Settings.TimeFormat)+ ")", KACResources.styleAlarmMessageTime);
            GUILayout.EndHorizontal();

            GUILayout.Label(tmpAlarm.Notes, KACResources.styleAlarmMessage);

            GUILayout.BeginHorizontal();
            DrawCheckbox(ref tmpAlarm.DeleteOnClose, "Delete On Close",0 );
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
            GUILayout.EndHorizontal();
            if (tmpAlarm.TypeOfAlarm == KACAlarm.AlarmType.Crew)
                DrawStoredCrewMissing(tmpAlarm.VesselID);
            else
                DrawStoredVesselIDMissing(tmpAlarm.VesselID);
            GUILayout.EndVertical();

            int intNoOfActionButtons = 0;
            //if the alarm has a vessel ID/Kerbal associated
            if (StoredVesselOrCrewExists(tmpAlarm.VesselID,tmpAlarm.TypeOfAlarm))
                //option to allow jumping from view only ships
                if (!parentBehaviour.ViewAlarmsOnly || Settings.AllowJumpFromViewOnly)
                    intNoOfActionButtons = DrawAlarmActionButtons(tmpAlarm);

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
                tmpAlarm.ActionedAt = KACWorkerGameState.CurrentTime.UT;
                if (tmpAlarm.PauseGame)
                    FlightDriver.SetPause(false);
                if (tmpAlarm.DeleteOnClose)
                    Settings.Alarms.Remove(tmpAlarm);
                Settings.SaveAlarms();
            }
          
            GUILayout.EndVertical();

            int intLines = tmpAlarm.Notes.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length;
            if (intLines == 0) intLines = 1;
            tmpAlarm.AlarmWindowHeight = 148 +
                 intLines * 16 +
                intNoOfActionButtons * 32; 
            SetTooltipText();
            GUI.DragWindow();

        }


        //VesselOrCrewStuff
        private static Boolean StoredVesselOrCrewExists(String ID, KACAlarm.AlarmType aType)
        {
            if (aType == KACAlarm.AlarmType.Crew && StoredCrewExists(ID))
            {
                return true;
            }
            else if (StoredVesselExists(ID))
            {
                return true;
            }
            else return false;

        }


        //Stuff to do with stored VesselIDs
        private static void DrawStoredVesselIDMissing(String VesselID)
        {
            if (VesselID!=null && VesselID != "" && !StoredVesselExists(VesselID))
            {
                GUILayout.Label("Stored VesselID no longer exists",KACResources.styleLabelWarning);
            }
        }
        public static Boolean StoredVesselExists(String VesselID)
        {
            return (VesselID != null) && (VesselID != "") && (FlightGlobals.Vessels.FirstOrDefault(v => v.id.ToString() == VesselID) != null);
        }
        public static Vessel StoredVessel(String VesselID)
        {
            return FlightGlobals.Vessels.FirstOrDefault(v => v.id.ToString() == VesselID);
        }

        //Stuff to do with Stored Kerbal Crew
        public static List<ProtoCrewMember> AllAssignedCrew()
        {
            List<ProtoCrewMember> lstReturn = new List<ProtoCrewMember>();
            foreach (Vessel v in FlightGlobals.Vessels)
            {
                List<ProtoCrewMember> pCM = v.GetVesselCrew();
                foreach (ProtoCrewMember CM in pCM)
                {
                    lstReturn.Add(CM);
                }
            }
            return lstReturn;
        }
        private static void DrawStoredCrewMissing(String KerbalName)
        {
            if (KerbalName != null && KerbalName != "" && !StoredCrewExists(KerbalName))
            {
                GUILayout.Label("Cannot find the stored Kerbal - perhaps he's lost :(", KACResources.styleLabelWarning);
            }
        }
        public static Boolean StoredCrewExists(String KerbalName)
        {
            return (KerbalName != null) && (KerbalName != "") && (AllAssignedCrew().FirstOrDefault(cm=>cm.name==KerbalName) != null);
        }

        public static ProtoCrewMember StoredCrew(String KerbalName)
        {
            return AllAssignedCrew().FirstOrDefault(cm => cm.name == KerbalName);
        }
        public static Vessel StoredCrewVessel(String KerbalName)
        {
            foreach (Vessel v in FlightGlobals.Vessels)
            {
                List<ProtoCrewMember> pCM = v.GetVesselCrew();
                foreach (ProtoCrewMember CM in pCM)
                {
                    if (CM.name == KerbalName)
                    {
                        return v;
                    }
                }
            }
            return null;
        }

        //Stuff to do with Celestial Bodies
        public static Boolean CelestialBodyExists(String BodyName)
        {
            return (BodyName != "") && (FlightGlobals.Bodies.FirstOrDefault(b => b.bodyName == BodyName) != null);
        }
        public static CelestialBody CelestialBody(String BodyName)
        {
            return FlightGlobals.Bodies.FirstOrDefault(a => a.bodyName == BodyName);
        }

        private KACAlarm alarmEdit;
        //track the height as we add/remove stuff
        private int intAlarmEditHeight;
        public void FillEditWindow(int WindowID)
        {
            if (alarmEdit.Remaining.UT > 0)
            {
                //Edit the Alarm if its not yet passed
                int intActionSelected = 0;
                if (alarmEdit.HaltWarp) intActionSelected = 1;
                if (alarmEdit.PauseGame) intActionSelected = 2;

                Double MarginStarting = alarmEdit.AlarmMarginSecs;
                int intHeight_EditWindowCommon = 88 +
                    alarmEdit.Notes.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length * 16;
                if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmType.Raw && alarmEdit.TypeOfAlarm != KACAlarm.AlarmType.EarthTime && alarmEdit.TypeOfAlarm != KACAlarm.AlarmType.Crew)
                    intHeight_EditWindowCommon += 28;
                WindowLayout_CommonFields(ref alarmEdit.Name, ref alarmEdit.Notes, ref intActionSelected, ref alarmEdit.AlarmMarginSecs, alarmEdit.TypeOfAlarm, intHeight_EditWindowCommon);
                //Adjust the UT of the alarm if the margin changed
                if (alarmEdit.AlarmMarginSecs != MarginStarting)
                {
                    alarmEdit.AlarmTime.UT += MarginStarting - alarmEdit.AlarmMarginSecs;
                }
                //Draw warning if the vessel no longer exists
                if (alarmEdit.TypeOfAlarm == KACAlarm.AlarmType.Crew)
                    DrawStoredCrewMissing(alarmEdit.VesselID);
                else
                    DrawStoredVesselIDMissing(alarmEdit.VesselID);

                //Draw the old and new times
                GUILayout.BeginHorizontal();
                if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmType.Raw && alarmEdit.TypeOfAlarm != KACAlarm.AlarmType.EarthTime && alarmEdit.TypeOfAlarm != KACAlarm.AlarmType.Crew)
                {
                    GUILayout.Label("Time To Alarm:", KACResources.styleContent);
                    GUILayout.Label(KACTime.PrintInterval(new KACTime(alarmEdit.AlarmTime.UT - KACWorkerGameState.CurrentTime.UT), Settings.TimeFormat), KACResources.styleAddHeading);
                }
                GUILayout.Label("Time To Event:", KACResources.styleContent);
                if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmType.EarthTime)
                    GUILayout.Label(KACTime.PrintInterval(new KACTime(alarmEdit.AlarmTime.UT + alarmEdit.AlarmMarginSecs-KACWorkerGameState.CurrentTime.UT),Settings.TimeFormat),KACResources.styleAddHeading);
                else
                    GUILayout.Label(KACTime.PrintInterval(new KACTime(alarmEdit.Remaining.UT), KACTime.PrintTimeFormat.DateTimeString  ), KACResources.styleAddHeading);
                GUILayout.EndHorizontal();

                
                alarmEdit.HaltWarp = (intActionSelected > 0);
                alarmEdit.PauseGame = (intActionSelected > 1);

                int intNoOfActionButtons = 0;
                //if the alarm has a vessel ID/Kerbal associated
                if (StoredVesselOrCrewExists(alarmEdit.VesselID, alarmEdit.TypeOfAlarm))
                    //option to allow jumping from view only ships
                    if (!parentBehaviour.ViewAlarmsOnly || Settings.AllowJumpFromViewOnly)
                        intNoOfActionButtons = DrawAlarmActionButtons(alarmEdit);

                if (GUILayout.Button("Close Alarm Details", KACResources.styleButton))
                {
                    Settings.Save();
                    _ShowEditPane = false;
                }

                //TODO: Edit the height of this for when we have big text in restore button
                intAlarmEditHeight = 197 + alarmEdit.Notes.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length * 16 + intNoOfActionButtons * 32;
                if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmType.Raw && alarmEdit.TypeOfAlarm != KACAlarm.AlarmType.Crew)
                    intAlarmEditHeight += 28;
            }
            else
            {

                //otherwise just show the details
                GUILayout.BeginVertical(GUI.skin.textArea);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Alarm:", KACResources.styleAlarmMessageTime);
                GUILayout.Label(alarmEdit.Name, KACResources.styleAlarmMessageTime);
                GUILayout.EndHorizontal();
                GUILayout.Label(alarmEdit.Notes, KACResources.styleAlarmMessage);

                //Draw warning if the vessel no longer exists
                if (alarmEdit.TypeOfAlarm == KACAlarm.AlarmType.Crew)
                    DrawStoredCrewMissing(alarmEdit.VesselID);
                else
                    DrawStoredVesselIDMissing(alarmEdit.VesselID);
                GUILayout.EndVertical();

                int intNoOfActionButtons=0;
                //if the alarm has a vessel ID/Kerbal associated
                if (StoredVesselOrCrewExists(alarmEdit.VesselID, alarmEdit.TypeOfAlarm))
                    //option to allow jumping from view only ships
                    if (!parentBehaviour.ViewAlarmsOnly || Settings.AllowJumpFromViewOnly)
                        intNoOfActionButtons = DrawAlarmActionButtons(alarmEdit);

                if (GUILayout.Button("Close Alarm Details", KACResources.styleButton))
                    _ShowEditPane = false;

                intAlarmEditHeight = 112 +
                    alarmEdit.Notes.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length * 16 +
                    intNoOfActionButtons * 32;
            }
            SetTooltipText();
        }
        
        private int DrawAlarmActionButtons(KACAlarm tmpAlarm)
        {
            int intReturnNoOfButtons = 0;
            
            ////is it the current vessel?
            if ((!parentBehaviour.ViewAlarmsOnly) && (FindVesselForAlarm(tmpAlarm).id.ToString() == KACWorkerGameState.CurrentVessel.id.ToString()))
            {
                //There is a node and the alarm + Margin is not expired
                if ((tmpAlarm.ManNodes != null))
                //if ((tmpAlarm.ManNodes != null) && ((tmpAlarm.Remaining.UT + tmpAlarm.AlarmMarginSecs) > 0))
                {
                    //Check if theres a manuever node and if so put a label saying that it already exists
                    //only display this node button if its the active ship
                    //Add this sae functionality to the alarm triggered window
                    //Add a jump to ship button if not the active ship
                    //As well as to the 
                    String strRestoretext = "Restore Maneuver Node(s)";
                    if (FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Count > 0)
                    {
                        strRestoretext = "Replace Maneuver Node(s)";
                        //if the count and UT's are the same then go from there
                        if (!KACAlarm.CompareManNodeListSimple(FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes,tmpAlarm.ManNodes))
                            strRestoretext += "\r\nNOTE: There is already a Node on the flight path";
                        else
                            strRestoretext += "\r\nNOTE: These Node's appear to be already set on the flight path";
                    }
                    if ((tmpAlarm.Remaining.UT + tmpAlarm.AlarmMarginSecs) < 0)
                    {
                        strRestoretext += "\r\nWARNING: The stored Nodes are in the past";
                    }
                    intReturnNoOfButtons++;
                    if (GUILayout.Button(strRestoretext, KACResources.styleButton))
                    {
                        DebugLogFormatted("Attempting to add Node");
                        FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Clear();
                        RestoreManeuverNodeList(tmpAlarm.ManNodes);
                    }
                }
                //There is a stored Target, that hasnt passed
                //if ((tmpAlarm.TargetObject != null) && ((tmpAlarm.Remaining.UT + tmpAlarm.AlarmMarginSecs) > 0))
                if ((tmpAlarm.TargetObject != null))
                    {
                    String strRestoretext = "Restore Target";
                    if (KACWorkerGameState.CurrentVesselTarget != null)
                    {
                        strRestoretext = "Replace Target";
                        if (KACWorkerGameState.CurrentVesselTarget != tmpAlarm.TargetObject)
                            strRestoretext += "\r\nNOTE: There is already a target and this will change";
                        else
                            strRestoretext += "\r\nNOTE: This already appears to be the target";
                    }
                    intReturnNoOfButtons++;
                    if (GUILayout.Button(strRestoretext, KACResources.styleButton))
                    {
                        if (tmpAlarm.TargetObject is Vessel)
                            FlightGlobals.fetch.SetVesselTarget(tmpAlarm.TargetObject as Vessel);
						else if (tmpAlarm.TargetObject is CelestialBody)
                            FlightGlobals.fetch.SetVesselTarget(tmpAlarm.TargetObject as CelestialBody);
                    }
                }
            }
            else
            {
                //not current vessel
                //There is a node and the alarm + Margin is not expired
                //if (tmpAlarm.ManNodes != null && tmpAlarm.Remaining.UT + tmpAlarm.AlarmMarginSecs > 0)
                if (tmpAlarm.ManNodes != null)
                    {
                    String strRestoretext = "Jump To Ship and Restore Maneuver Node";
                    if ((tmpAlarm.Remaining.UT + tmpAlarm.AlarmMarginSecs) < 0)
                    {
                        strRestoretext += "\r\nWARNING: The stored Nodes are in the past";
                    }
                    intReturnNoOfButtons++;

                    if (GUILayout.Button(strRestoretext, KACResources.styleButton))
                    {
                        Vessel tmpVessel = FindVesselForAlarm(tmpAlarm);

                        JumpToVessel(tmpVessel);
                       
                        //Set the Node in memory to restore once the ship change has completed
                        Settings.LoadManNode = KACAlarm.ManNodeSerializeList(tmpAlarm.ManNodes);
                        Settings.SaveLoadObjects();
                    }
                }

                //There is a target and the alarm has not expired
                //if (tmpAlarm.TargetObject != null && tmpAlarm.Remaining.UT + tmpAlarm.AlarmMarginSecs > 0)
                if (tmpAlarm.TargetObject != null )
                {
                    intReturnNoOfButtons++;
                    if (GUILayout.Button("Jump To Ship and Restore Target", KACResources.styleButton))
                    {
                        Vessel tmpVessel = FindVesselForAlarm(tmpAlarm);

                        JumpToVessel(tmpVessel);
                         
                        //Set the Target in persistant file to restore once the ship change has completed...
                        Settings.LoadVesselTarget = KACAlarm.TargetSerialize(tmpAlarm.TargetObject);
                        Settings.SaveLoadObjects();
                    }
                }
                
                intReturnNoOfButtons++;
                //Or just jump to ship - regardless of alarm time
                String strButton = "Jump To Ship";
                if (tmpAlarm.TypeOfAlarm == KACAlarm.AlarmType.Crew) strButton = strButton.Replace("Ship", "Kerbal");
                if (GUILayout.Button(strButton, KACResources.styleButton))
                {

                    Vessel tmpVessel = FindVesselForAlarm(tmpAlarm);
                    // tmpVessel.MakeActive();

                    JumpToVessel(tmpVessel);
                }
            }
            return intReturnNoOfButtons;
        }

        private static void JumpToVessel(Vessel vTarget)
        {
            if (KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT)
            {
                if (KACUtils.BackupSaves())
                    FlightGlobals.SetActiveVessel(vTarget);
                else 
                    DebugLogFormatted("Not Switching - unable to backup saves");
            }
            else
            {
                int intVesselidx = getVesselIdx(vTarget);
                if (intVesselidx < 0)
                {
                    DebugLogFormatted("Couldn't find the index for the vessel {0}({1})", vTarget.vesselName, vTarget.id.ToString());
                }
                else
                {
                    try
                    {
                        if (KACUtils.BackupSaves())
                        {
                            String strret = GamePersistence.SaveGame("KACJumpToShip", HighLogic.SaveFolder, SaveMode.OVERWRITE);
                            Game tmpGame = GamePersistence.LoadGame(strret, HighLogic.SaveFolder, false, false);
                            FlightDriver.StartAndFocusVessel(tmpGame, intVesselidx);
                        }
                        else
                        {
                            DebugLogFormatted("Not Switching - unable to backup saves");
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugLogFormatted("Unable to save/load for jump to ship: {0}", ex.Message);
                    }
                }
            }
        }

        private static Vessel FindVesselForAlarm(KACAlarm tmpAlarm)
        {
            Vessel tmpVessel;
            String strVesselID = "";
            if (tmpAlarm.TypeOfAlarm == KACAlarm.AlarmType.Crew)
            {
                strVesselID = StoredCrewVessel(tmpAlarm.VesselID).id.ToString();
            }
            else
            {
                strVesselID = tmpAlarm.VesselID;
            }

            tmpVessel = FlightGlobals.Vessels.Find(delegate(Vessel v)
                {
                    return (strVesselID == v.id.ToString());
                }
            );
            return tmpVessel;
        }

        private static int getVesselIdx(Vessel vtarget)
        {
            for (int i = 0; i < FlightGlobals.Vessels.Count; i++)
            {
                if (FlightGlobals.Vessels[i].id == vtarget.id)
                {
                    DebugLogFormatted("Found Target idx={0} ({1})", i, vtarget.id.ToString());
                    return i;
                }
            }
            return -1;
        }
    }
}
