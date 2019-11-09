using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

using System.Reflection;

using UnityEngine;
using KSP;
using KSP.UI.Screens;
using KSPPluginFramework;

namespace KerbalAlarmClock
{
	public partial class KerbalAlarmClock
	{
		//On OnGUI - draw alarms if needed
		internal void TriggeredAlarms()
		{
			foreach (KACAlarm tmpAlarm in alarms)
			{
				if (tmpAlarm.Enabled)
				{
					//also test triggered and actioned
					//if (KACWorkerGameState.CurrentTime.UT >= tmpAlarm.AlarmTime.UT)
					if ((tmpAlarm.Remaining.UT<=0))
					{
						if (tmpAlarm.Actioned && !tmpAlarm.AlarmWindowClosed)
						{
							if (tmpAlarm.AlarmWindowID == 0)
							{
								tmpAlarm.AlarmWindowID = rnd.Next(1, 2000000);
								tmpAlarm.AlarmWindow = new Rect((Screen.width / 2) - 160, (Screen.height / 2) - 100, 320, tmpAlarm.AlarmWindowHeight);
								if (settings.AlarmPosition == 0)
									tmpAlarm.AlarmWindow.x = 5;
								else if (settings.AlarmPosition == 2)
									tmpAlarm.AlarmWindow.x = Screen.width - tmpAlarm.AlarmWindow.width - 5;

								//tmpAlarm.DeleteOnClose = settings.AlarmDeleteOnClose;
							}
							else
							{
								tmpAlarm.AlarmWindow.height = tmpAlarm.AlarmWindowHeight;
							}
							String strAlarmText = tmpAlarm.Name;
							
							switch (tmpAlarm.TypeOfAlarm)
							{
								case KACAlarm.AlarmTypeEnum.Raw:
									strAlarmText+= " - Manual";break;
								case KACAlarm.AlarmTypeEnum.Maneuver:
								case KACAlarm.AlarmTypeEnum.ManeuverAuto:
									strAlarmText += " - Maneuver Node"; break;
								case KACAlarm.AlarmTypeEnum.SOIChange:
								case KACAlarm.AlarmTypeEnum.SOIChangeAuto:
									strAlarmText += " - SOI Change"; break;
								case KACAlarm.AlarmTypeEnum.Transfer:
								case KACAlarm.AlarmTypeEnum.TransferModelled:
									strAlarmText += " - Transfer Point"; break;
								case KACAlarm.AlarmTypeEnum.Apoapsis:
									strAlarmText += " - Apoapsis"; break;
								case KACAlarm.AlarmTypeEnum.Periapsis:
									strAlarmText += " - Periapsis"; break;
								case KACAlarm.AlarmTypeEnum.AscendingNode:
									strAlarmText += " - Ascending Node"; break;
								case KACAlarm.AlarmTypeEnum.DescendingNode:
									strAlarmText += " - Descending Node"; break;
								case KACAlarm.AlarmTypeEnum.LaunchRendevous:
									strAlarmText += " - Launch Rendevous"; break;
								case KACAlarm.AlarmTypeEnum.Closest:
									strAlarmText += " - Closest Approach"; break;
								case KACAlarm.AlarmTypeEnum.EarthTime:
									strAlarmText += " - Earth Alarm"; break;
								case KACAlarm.AlarmTypeEnum.Crew:
									strAlarmText += " - Kerbal Alarm"; break;
								case KACAlarm.AlarmTypeEnum.Contract:
								case KACAlarm.AlarmTypeEnum.ContractAuto:
									strAlarmText += " - Contract"; break;
                                case KACAlarm.AlarmTypeEnum.ScienceLab:
                                    strAlarmText += " - Science Lab"; break;
								default:
									strAlarmText+= " - Manual";break;
							}
							tmpAlarm.AlarmWindow = GUILayout.Window(tmpAlarm.AlarmWindowID, tmpAlarm.AlarmWindow, FillAlarmWindow, strAlarmText, KACResources.styleWindow, GUILayout.MinWidth(320));
						}
					}
				}
			}

		}

		internal void FillAlarmWindow(int windowID)
		{
			KACAlarm tmpAlarm = alarms.GetByWindowID(windowID);

			GUILayout.BeginVertical();

			GUILayout.BeginVertical(GUI.skin.textArea);

			GUILayout.BeginHorizontal();
			GUILayout.Label("Alarm Time:", KACResources.styleAlarmMessageTime);
			if (tmpAlarm.TypeOfAlarm!= KACAlarm.AlarmTypeEnum.EarthTime)
				GUILayout.Label(tmpAlarm.AlarmTime.ToStringStandard(settings.DateTimeFormat), KACResources.styleAlarmMessageTime);
			else
				GUILayout.Label(EarthTimeDecode(tmpAlarm.AlarmTime.UT).ToLongTimeString(), KACResources.styleAlarmMessageTime);
			if (tmpAlarm.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Raw && tmpAlarm.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime && tmpAlarm.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Crew && tmpAlarm.TypeOfAlarm != KACAlarm.AlarmTypeEnum.ScienceLab)
				GUILayout.Label("(m: " + new KSPTimeSpan(tmpAlarm.AlarmMarginSecs).ToStringStandard(settings.TimeSpanFormat, 3) + ")", KACResources.styleAlarmMessageTime);
			GUILayout.EndHorizontal();

			GUILayout.Label(tmpAlarm.Notes, KACResources.styleAlarmMessage);

			GUILayout.BeginHorizontal();
			DrawCheckbox(ref tmpAlarm.Actions.DeleteWhenDone, "Delete On Close",0 );
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
			if (tmpAlarm.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Crew)
				DrawStoredCrewMissing(tmpAlarm.VesselID);
			else
				DrawStoredVesselIDMissing(tmpAlarm.VesselID);
			GUILayout.EndVertical();

			int intNoOfActionButtons = 0;
			int intNoOfActionButtonsDoubleLine = 0;
			//if the alarm has a vessel ID/Kerbal associated
			if (CheckVesselOrCrewForJump(tmpAlarm.VesselID,tmpAlarm.TypeOfAlarm))
				//option to allow jumping from SC and TS
				if (settings.AllowJumpFromViewOnly)
					intNoOfActionButtons = DrawAlarmActionButtons(tmpAlarm, out intNoOfActionButtonsDoubleLine);

            intNoOfActionButtons += DrawTransferAngleButtons(tmpAlarm);

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

                //Stop playing the sound if it is playing
                if (audioController.isPlaying)
                    audioController.Stop();

				//tmpAlarm.ActionedAt = KACWorkerGameState.CurrentTime.UT;
				if (tmpAlarm.PauseGame)
					FlightDriver.SetPause(false);

				try { 
					APIInstance_AlarmStateChanged(tmpAlarm, AlarmStateEventsEnum.Closed);
				} catch (Exception ex) {
					MonoBehaviourExtended.LogFormatted("Error Raising API Event-Closed Alarm: {0}\r\n{1}", ex.Message, ex.StackTrace);
				} 

				if (tmpAlarm.Actions.DeleteWhenDone)
					alarms.Remove(tmpAlarm);
				//settings.SaveAlarms();
			}
		  
			GUILayout.EndVertical();

			int intLines = tmpAlarm.Notes.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length;
			if (intLines == 0) intLines = 1;
			tmpAlarm.AlarmWindowHeight = 148 +
				 intLines * 16 +
				intNoOfActionButtons * 32 +
				intNoOfActionButtonsDoubleLine * 14;

			SetTooltipText();
			GUI.DragWindow();

		}


		//VesselOrCrewStuff
		private static Boolean CheckVesselOrCrewForJump(String ID, KACAlarm.AlarmTypeEnum aType)
		{
            if (aType == KACAlarm.AlarmTypeEnum.Crew && StoredCrewExists(ID))
            {
                return true;
            }
            else
            {
                Vessel v = StoredVessel(ID);

                if (v != null)
                {
                    if (v.vesselType != VesselType.SpaceObject && v.DiscoveryInfo.Level != DiscoveryLevels.Owned)
                        return false;
                    else if (settings.AllowJumpToAsteroid)
                        return true;
                    else if (StoredVessel(ID).vesselType != VesselType.SpaceObject)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
		}


		//Stuff to do with stored VesselIDs
		private static void DrawStoredVesselIDMissing(String VesselID)
		{
			if (!(VesselID == null || VesselID == "") && !StoredVesselExists(VesselID))
			{
				GUILayout.Label("Stored VesselID no longer exists",KACResources.styleLabelWarning);
			}
		}
		internal static Boolean StoredVesselExists(String VesselID)
		{
            return StoredVessel(VesselID) != null;
			//return (VesselID != null) && (VesselID != "") && (FlightGlobals.Vessels.FirstOrDefault(v => v.id.ToString() == VesselID) != null);
		}

        internal static Vessel StoredVessel(String VesselID)
        {
            if (VesselID == null || VesselID == "")
            {
                return null;
            }

            try
            {
                Guid g = new Guid(VesselID);
                return FlightGlobals.FindVessel(g);
            }
            catch { }

            return null;
			//return FlightGlobals.Vessels.FirstOrDefault(v => v.id.ToString() == VesselID);
		}

		//Stuff to do with Stored Kerbal Crew
		internal static List<ProtoCrewMember> AllAssignedCrew()
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
		internal static Boolean StoredCrewExists(String KerbalName)
		{
			return (KerbalName != null) && (KerbalName != "") && (AllAssignedCrew().FirstOrDefault(cm=>cm.name==KerbalName) != null);
		}

		internal static ProtoCrewMember StoredCrew(String KerbalName)
		{
			return AllAssignedCrew().FirstOrDefault(cm => cm.name == KerbalName);
		}
		internal static Vessel StoredCrewVessel(String KerbalName)
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
		internal static Boolean CelestialBodyExists(String BodyName)
		{
			return (BodyName != "") && (FlightGlobals.Bodies.FirstOrDefault(b => b.bodyName == BodyName) != null);
		}
		internal static CelestialBody CelestialBody(String BodyName)
		{
			return FlightGlobals.Bodies.FirstOrDefault(a => a.bodyName == BodyName);
		}

		private KACAlarm alarmEdit;
		//track the height as we add/remove stuff
		private Int32 intAlarmEditHeight;
		public void FillEditWindow(int WindowID)
		{
			if (alarmEdit.Remaining.UT > 0)
			{
				//Edit the Alarm if its not yet passed
				Double MarginStarting = alarmEdit.AlarmMarginSecs;
				int intHeight_EditWindowCommon = 103 +
					alarmEdit.Notes.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length * 16;
				if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Raw && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Crew && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.ScienceLab)
					intHeight_EditWindowCommon += 28;

                AlarmActions atemp = alarmEdit.Actions;
				WindowLayout_CommonFields(ref alarmEdit.Name, ref alarmEdit.Notes, ref atemp, ref alarmEdit.AlarmMarginSecs, alarmEdit.TypeOfAlarm, intHeight_EditWindowCommon);
                alarmEdit.Actions = atemp;
				//Adjust the UT of the alarm if the margin changed
				if (alarmEdit.AlarmMarginSecs != MarginStarting)
				{
					alarmEdit.AlarmTime.UT += MarginStarting - alarmEdit.AlarmMarginSecs;
				}
				//Draw warning if the vessel no longer exists
				if (alarmEdit.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Crew)
					DrawStoredCrewMissing(alarmEdit.VesselID);
				else
					DrawStoredVesselIDMissing(alarmEdit.VesselID);


                //Draw the old and new times
                GUILayout.BeginHorizontal();
                if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Raw && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Crew && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.ScienceLab)
                {
                    GUILayout.Label("Time To Alarm:", KACResources.styleContent);
                    GUILayout.Label((alarmEdit.AlarmTime - KACWorkerGameState.CurrentTime).ToStringStandard(settings.TimeSpanFormat), KACResources.styleAddHeading);
                }
                GUILayout.Label("Time To Event:", KACResources.styleContent);
                if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime)
                    GUILayout.Label((alarmEdit.AlarmTime - KACWorkerGameState.CurrentTime).Add(new KSPTimeSpan(alarmEdit.AlarmMarginSecs)).ToStringStandard(settings.TimeSpanFormat), KACResources.styleAddHeading);
                else
                    GUILayout.Label(alarmEdit.Remaining.ToStringStandard(TimeSpanStringFormatsEnum.DateTimeFormat), KACResources.styleAddHeading);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Date of Event:", KACResources.styleContent);
                if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime)
                    GUILayout.Label(alarmEdit.AlarmTime.AddSeconds(alarmEdit.AlarmMarginSecs).ToStringStandard(DateStringFormatsEnum.DateTimeFormat), KACResources.styleAddHeading);
                else
                    GUILayout.Label(DateTime.Now.AddSeconds(alarmEdit.Remaining.UT).ToLongTimeString(), KACResources.styleAddHeading);
                GUILayout.EndHorizontal();

				int intNoOfActionButtons = 0;
				int intNoOfActionButtonsDoubleLine = 0;
				//if the alarm has a vessel ID/Kerbal associated
				if (CheckVesselOrCrewForJump(alarmEdit.VesselID, alarmEdit.TypeOfAlarm))
					//option to allow jumping from SC and TS
					if (settings.AllowJumpFromViewOnly)
						intNoOfActionButtons = DrawAlarmActionButtons(alarmEdit, out intNoOfActionButtonsDoubleLine);

                intNoOfActionButtons += DrawTransferAngleButtons(alarmEdit);

				if (GUILayout.Button("Close Alarm Details", KACResources.styleButton))
				{
					settings.Save();
					_ShowEditPane = false;
				}

				//TODO: Edit the height of this for when we have big text in restore button
				 intAlarmEditHeight = 197 + 16 + 20 + alarmEdit.Notes.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length * 16 + intNoOfActionButtons * 32 + intNoOfActionButtonsDoubleLine*14;
				if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Raw && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Crew && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.ScienceLab)
					intAlarmEditHeight += 28;
                if (alarmEdit.TypeOfAlarm==KACAlarm.AlarmTypeEnum.EarthTime)
                    intAlarmEditHeight -= 28;
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
				if (alarmEdit.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Crew)
					DrawStoredCrewMissing(alarmEdit.VesselID);
				else
					DrawStoredVesselIDMissing(alarmEdit.VesselID);
				GUILayout.EndVertical();

				//Draw the old and new times
				GUILayout.BeginHorizontal();
				if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Raw && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Crew && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.ScienceLab) {
					GUILayout.Label("Time To Alarm:", KACResources.styleContent);
					GUILayout.Label((alarmEdit.AlarmTime - KACWorkerGameState.CurrentTime).ToStringStandard(settings.TimeSpanFormat), KACResources.styleAddHeading);
				}
				GUILayout.Label("Time To Event:", KACResources.styleContent);
				if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime)
					GUILayout.Label((alarmEdit.AlarmTime - KACWorkerGameState.CurrentTime).Add(new KSPTimeSpan(alarmEdit.AlarmMarginSecs)).ToStringStandard(settings.TimeSpanFormat), KACResources.styleAddHeading);
				else
                    GUILayout.Label(alarmEdit.Remaining.ToStringStandard(TimeSpanStringFormatsEnum.DateTimeFormat), KACResources.styleAddHeading);
				GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Date of Event:", KACResources.styleContent);
                if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime)
                    GUILayout.Label(alarmEdit.AlarmTime.AddSeconds(alarmEdit.AlarmMarginSecs).ToStringStandard(DateStringFormatsEnum.DateTimeFormat), KACResources.styleAddHeading);
                else
                    GUILayout.Label(DateTime.Now.AddSeconds(alarmEdit.Remaining.UT).ToLongTimeString(), KACResources.styleAddHeading);
                GUILayout.EndHorizontal();

				int intNoOfActionButtons = 0;
				int intNoOfActionButtonsDoubleLine = 0;
				//if the alarm has a vessel ID/Kerbal associated
				if (CheckVesselOrCrewForJump(alarmEdit.VesselID, alarmEdit.TypeOfAlarm))
					//option to allow jumping from SC and TS
					if (settings.AllowJumpFromViewOnly)
						intNoOfActionButtons = DrawAlarmActionButtons(alarmEdit, out intNoOfActionButtonsDoubleLine);

                intNoOfActionButtons += DrawTransferAngleButtons(alarmEdit);

                if (GUILayout.Button("Close Alarm Details", KACResources.styleButton))
					_ShowEditPane = false;

				intAlarmEditHeight = 152 + 20 +
					alarmEdit.Notes.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length * 16 +
					intNoOfActionButtons * 32 + intNoOfActionButtonsDoubleLine * 14;
			}
			SetTooltipText();
		}
		
        private int DrawTransferAngleButtons(KACAlarm tmpAlarm)
        {
            if((tmpAlarm.TypeOfAlarm== KACAlarm.AlarmTypeEnum.Transfer|| tmpAlarm.TypeOfAlarm == KACAlarm.AlarmTypeEnum.TransferModelled) &&
                (HighLogic.LoadedScene == GameScenes.TRACKSTATION || HighLogic.LoadedScene == GameScenes.FLIGHT))
            {
                //right type of alarm, now is the text there
                Match matchPhase = Regex.Match(tmpAlarm.Notes, "(?<=Phase\\sAngle\\:\\s+)\\S+(?=°)");
                Match matchEjectPro = Regex.Match(tmpAlarm.Notes, "(?<=Ejection\\sAngle\\:\\s+)\\S+(?=°\\sto\\sprograde)");
                Match matchEjectRetro = Regex.Match(tmpAlarm.Notes, "(?<=Ejection\\sAngle\\:\\s+)\\S+(?=°\\sto\\sretrograde)");
                if (matchPhase.Success && (matchEjectPro.Success || matchEjectRetro.Success))
                {

                    try
                    {
                        //LogFormatted_DebugOnly("{0}", matchPhase.Value);
                        Double dblPhase = Convert.ToDouble(matchPhase.Value);
                        Double dblEject;
                        if (matchEjectPro.Success)
                            dblEject = Convert.ToDouble(matchEjectPro.Value);
                        else
                            dblEject = Convert.ToDouble(matchEjectRetro.Value);

                        GUILayout.BeginHorizontal();

                        CelestialBody cbOrigin = FlightGlobals.Bodies.Single(b => b.bodyName == tmpAlarm.XferOriginBodyName);
                        CelestialBody cbTarget = FlightGlobals.Bodies.Single(b => b.bodyName == tmpAlarm.XferTargetBodyName);

                        GUIStyle styleAngleButton = new GUIStyle(KACResources.styleSmallButton) { fixedWidth = 180 };

                        if (DrawToggle(ref blnShowPhaseAngle,"Show Phase Angle", styleAngleButton)){
                            if (blnShowPhaseAngle)
                            {
                                EjectAngle.HideAngle();
                                blnShowEjectAngle = false;
                                PhaseAngle.DrawAngle(cbOrigin, cbTarget, dblPhase);
                            }
                            else
                                PhaseAngle.HideAngle();
                        }
                        if (DrawToggle(ref blnShowEjectAngle, "Show Eject Angle", styleAngleButton))
                        {
                            if (blnShowEjectAngle)
                            {
                                PhaseAngle.HideAngle();
                                blnShowPhaseAngle = false;
                                EjectAngle.DrawAngle(cbOrigin, dblEject, matchEjectRetro.Success);
                            }
                            else
                                EjectAngle.HideAngle();
                        }
                        GUILayout.EndHorizontal();

                        //if (GUILayout.Toggle()) {

                        //}
                        //GUILayout.Label(String.Format("P:{0} - E:{1}",dblPhase,dblEject));

                        return 1;

                    }
                    catch (Exception)
                    {
                        GUILayout.Label("Unable to decipher TWP Phase and Eject Angle found in notes");
                        return 1;
                    }
                } else {
                    GUILayout.Label("No TWP Phase and Eject Angle found in notes");
                    return 1;
                }
            } else { return 0; }
        }

        private int DrawAlarmActionButtons(KACAlarm tmpAlarm, out int NoOfDoubleLineButtons)
		{
			int intReturnNoOfButtons = 0;
			NoOfDoubleLineButtons = 0;
			
			////is it the current vessel?
			//if ((!ViewAlarmsOnly) && (KACWorkerGameState.CurrentVessel != null) && (FindVesselForAlarm(tmpAlarm).id.ToString() == KACWorkerGameState.CurrentVessel.id.ToString()))
			if ((KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT) && (KACWorkerGameState.CurrentVessel != null) && (FindVesselForAlarm(tmpAlarm).id.ToString() == KACWorkerGameState.CurrentVessel.id.ToString()))
			{
				//There is a node and the alarm + Margin is not expired
				if ((tmpAlarm.ManNodes != null) && tmpAlarm.ManNodes.Count > 0)
				//if ((tmpAlarm.ManNodes != null) && ((tmpAlarm.Remaining.UT + tmpAlarm.AlarmMarginSecs) > 0))
				{
                    bool blnDontShowManNode = false;

					//Check if theres a Maneuver node and if so put a label saying that it already exists
					//only display this node button if its the active ship
					//Add this sae functionality to the alarm triggered window
					//Add a jump to ship button if not the active ship
					//As well as to the 
					String strRestoretext = "Restore Maneuver Node(s)";
					if (KACWorkerGameState.CurrentVessel.patchedConicSolver.maneuverNodes.Count > 0)
					{
						strRestoretext = "Replace Maneuver Node(s)";
                        //if the count and UT's are the same then go from there
                        if(!KACAlarm.CompareManNodeListSimple(KACWorkerGameState.CurrentVessel.patchedConicSolver.maneuverNodes, tmpAlarm.ManNodes)) {
                            strRestoretext += "\r\nNOTE: There is already a Node on the flight path";
                            NoOfDoubleLineButtons++;
                        } else {
                            //Dont show the button
                            blnDontShowManNode = true;
                        }
					}

                    if(!blnDontShowManNode) {
                        if((tmpAlarm.Remaining.UT + tmpAlarm.AlarmMarginSecs) < 0) {
                            strRestoretext += "\r\nWARNING: The stored Nodes are in the past";
                            NoOfDoubleLineButtons++;
                        }
                        intReturnNoOfButtons++;
                        if(GUILayout.Button(strRestoretext, KACResources.styleButton)) {
                            LogFormatted("Attempting to add Node");
                            KACWorkerGameState.CurrentVessel.patchedConicSolver.maneuverNodes.Clear();
                            RestoreManeuverNodeList(tmpAlarm.ManNodes);
                        }
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
						NoOfDoubleLineButtons++;
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
				if (tmpAlarm.ManNodes != null && tmpAlarm.ManNodes.Count > 0)
				{
					String strRestoretext = "Jump To Ship and Restore Maneuver Node";
					if (tmpAlarm.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Crew) strRestoretext = strRestoretext.Replace("Ship", "Kerbal");
					if ((tmpAlarm.Remaining.UT + tmpAlarm.AlarmMarginSecs) < 0)
					{
						strRestoretext += "\r\nWARNING: The stored Nodes are in the past";
						NoOfDoubleLineButtons++;
					}
					intReturnNoOfButtons++;

					if (GUILayout.Button(strRestoretext, KACResources.styleButton))
					{
						Vessel tmpVessel = FindVesselForAlarm(tmpAlarm);

						if (JumpToVessel(tmpVessel))
						{
							//Set the Node in memory to restore once the ship change has completed
							settings.LoadManNode = KACAlarm.ManNodeSerializeList(tmpAlarm.ManNodes);
							settings.Save();
						}
					}
				}

				//There is a target and the alarm has not expired
				//if (tmpAlarm.TargetObject != null && tmpAlarm.Remaining.UT + tmpAlarm.AlarmMarginSecs > 0)
				if (tmpAlarm.TargetObject != null )
				{
					intReturnNoOfButtons++;
					String strButtonT = "Jump To Ship and Restore Target";
					if (tmpAlarm.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Crew) strButtonT = strButtonT.Replace("Ship", "Kerbal");
					if (GUILayout.Button(strButtonT, KACResources.styleButton))
					{
						Vessel tmpVessel = FindVesselForAlarm(tmpAlarm);

						if (JumpToVessel(tmpVessel))
						{
							//Set the Target in persistant file to restore once the ship change has completed...
							settings.LoadVesselTarget = KACAlarm.TargetSerialize(tmpAlarm.TargetObject);
							settings.Save();
						}
					}
				}
				
				intReturnNoOfButtons++;
				//Or just jump to ship - regardless of alarm time
				String strButton = "Jump To Ship";
				if (tmpAlarm.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Crew) strButton = strButton.Replace("Ship", "Kerbal");
				if (GUILayout.Button(strButton, KACResources.styleButton))
				{

					Vessel tmpVessel = FindVesselForAlarm(tmpAlarm);
					// tmpVessel.MakeActive();

					JumpToVessel(tmpVessel);
				}

                //////////////////////////////////////////////////////////////////////////////////
                // Focus Vessel Code - reflecting to get SetVessel Focus in TS
                //////////////////////////////////////////////////////////////////////////////////
                if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION)
                {
                    Vessel vTarget = FlightGlobals.Vessels.FirstOrDefault(v => v.id.ToString().ToLower() == tmpAlarm.VesselID);
                    if (vTarget != null)
                    {
                        intReturnNoOfButtons++;
                        if (GUILayout.Button("Set Vessel Active", KACResources.styleButton))
                        {

                            SetVesselActiveInTS(vTarget);

                            //FlightGlobals.Vessels.ForEach(v =>
                            //    {
                            //        v.DetachPatchedConicsSolver();
                            //        v.orbitRenderer.isFocused = false;
                            //    });

                            //vTarget.orbitRenderer.isFocused = true;
                            //vTarget.AttachPatchedConicsSolver();
                            //FlightGlobals.SetActiveVessel(vTarget);

                            //SpaceTracking.GoToAndFocusVessel(vTarget);
                            //st.mainCamera.SetTarget(getVesselIdx(vTarget));
                        }
                    }
                    //}
                }
			}
			return intReturnNoOfButtons;
		}

        private static void SetVesselActiveInTS(Vessel vTarget)
        {
            if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION)
            {
                try
                {
                    SpaceTracking st = (SpaceTracking)KACSpaceCenter.FindObjectOfType(typeof(SpaceTracking));
                    st.SetVessel(vTarget, true);
                }
                catch (Exception ex)
                {
                    LogFormatted("Unable to set vessel as active in Tracking station:\r\n{0}", ex.Message);
                }
            }
        }

        private static Vessel vesselToJumpTo = null;
		private Boolean JumpToVessel(Vessel vTarget)
		{
			Boolean blnJumped = true;
			if (KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT)
			{
                LogFormatted_DebugOnly("Switching in Scene");
                if(KACUtils.BackupSaves() || !KerbalAlarmClock.settings.CancelFlightModeJumpOnBackupFailure)
                    vesselToJumpTo = vTarget;

                    //if(FlightGlobals.SetActiveVessel(vTarget))
                    //{
                    //    FlightInputHandler.SetNeutralControls();
                    //}
				else 
				{
					LogFormatted("Not Switching - unable to backup saves");
					ShowBackupFailedWindow("Not Switching - unable to backup saves");
					blnJumped = false;
				}
			}
			else
			{
                LogFormatted_DebugOnly("Switching in by Save");

                int intVesselidx = getVesselIdx(vTarget);
				if (intVesselidx < 0)
				{
					LogFormatted("Couldn't find the index for the vessel {0}({1})", vTarget.vesselName, vTarget.id.ToString());
					ShowBackupFailedWindow("Not Switching - unable to find vessel index");
					blnJumped = false;
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
                            //if (tmpAlarm.PauseGame)
                            //FlightDriver.SetPause(false);
                            //tmpGame.Start();
						}
						else
						{
							LogFormatted("Not Switching - unable to backup saves");
							ShowBackupFailedWindow("Not Switching - unable to backup saves");
							blnJumped = false;
						}
					}
					catch (Exception ex)
					{
						LogFormatted("Unable to save/load for jump to ship: {0}", ex.Message);
						ShowBackupFailedWindow("Not Switching - failed in loading new position");
						blnJumped = false;
					}
				}
			}
			return blnJumped;
		}

		private static Vessel FindVesselForAlarm(KACAlarm tmpAlarm)
		{
            Vessel tmpVessel;
			String strVesselID = "";
			if (tmpAlarm.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Crew)
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
					LogFormatted("Found Target idx={0} ({1})", i, vtarget.id.ToString());
					return i;
				}
			}
			return -1;
		}

		#region "BackupFailed Message"
		internal void ShowBackupFailedWindow(String Message)
		{
			BackupFailedMessage = Message;
			GUIContent contFailMessage = new GUIContent(BackupFailedMessage);
			float minwidth = 0; float maxwidth = 0;
			KACResources.styleAddHeading.CalcMinMaxWidth(contFailMessage, out minwidth, out maxwidth);

			switch (KACWorkerGameState.CurrentGUIScene)
			{
				case GameScenes.SPACECENTER: 
					_WindowBackupFailedRect = new Rect((Screen.width - maxwidth - 20) , Screen.height - 90 - 37, maxwidth + 20, 90);
					break;
				case GameScenes.TRACKSTATION: 
					_WindowBackupFailedRect = new Rect((Screen.width - maxwidth - 20) , Screen.height - 90, maxwidth + 20, 90);
					break;
				default: 
					_WindowBackupFailedRect = new Rect((Screen.width - maxwidth - 20) , Screen.height - 90 - 122, maxwidth + 20, 90);
					break;
			}

			_ShowBackupFailedMessageAt=DateTime.Now;
			_ShowBackupFailedMessage = true;
		}


		#region "Stuff for backupFailed dialog per scene"
		internal Rect ShowBackupFailedWindowPosByActiveScene
		{
			get
			{
				switch (KACWorkerGameState.CurrentGUIScene)
				{
					case GameScenes.SPACECENTER: return settings.WindowPos_SpaceCenter;
                    case GameScenes.TRACKSTATION: return settings.WindowPos_TrackingStation;
                    case GameScenes.EDITOR:
                        if (isEditorVAB) 
                            return settings.WindowPos_EditorVAB;
                        else
                            return settings.WindowPos_EditorSPH;
                    default: return settings.WindowPos;
				}
			}
		}

		#endregion

		internal void ResetBackupFailedWindow()
		{
			_ShowBackupFailedMessage = false;
			BackupFailedMessage = "";
		}

		private static String BackupFailedMessage = "";
		internal void FillBackupFailedWindow(int windowID)
		{
			GUILayout.BeginVertical();

			GUILayout.Label(new GUIContent(BackupFailedMessage), KACResources.styleAddHeading);

			int SecsToClose = _ShowBackupFailedMessageForSecs - DateTime.Now.Subtract(_ShowBackupFailedMessageAt).Seconds;
			if (GUILayout.Button(string.Format("Close ({0} secs)", SecsToClose)))
				ResetBackupFailedWindow();

			GUILayout.EndVertical();

		}
		#endregion
	}
}
