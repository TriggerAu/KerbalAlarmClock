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
        //Updates the variables that are used in the drawing - this is not on the OnGUI thread
        private Dictionary<String, KACVesselSOI> lstVessels = new Dictionary<String, KACVesselSOI>();
        int intPeriodicSaveCounter = 0;

        private void MonitorSOIOnPath()
        {
            //Is there an SOI Point on the path - looking for next action on the path use orbit.patchEndTransition - enum of Orbit.PatchTransitionType
            //  FINAL - fixed orbit no change
            //  ESCAPE - leaving SOI
            //  ENCOUNTER - entering new SOI inside current SOI
            //  INITIAL - ???
            //  MANEUVER - Maneuver Node
            // orbit.UTsoi - time of next SOI change (base on above transition types - ie if type is final this time can be ignored)
            //orbit.nextpatch gives you the next orbit and you can read the new SOI!!!

            double timeSOIChange = 0;
            double timeSOIAlarm = 0;

            String strSOIAlarmName = "";
            String strSOIAlarmNotes = "";
            //double timeSOIAlarm = 0;

            if (Settings.AlarmAddSOIAuto_ExcludeEVA && KACGameState.CurrentVessel.vesselType == VesselType.EVA)
                return;

            if (Settings.SOITransitions.Contains(KACGameState.CurrentVessel.orbit.patchEndTransition))
            {
                timeSOIChange = KACGameState.CurrentVessel.orbit.UTsoi;
                //timeSOIAlarm = timeSOIChange - Settings.AlarmAddSOIMargin;
                //strOldAlarmNameSOI = KACWorkerGameState.CurrentVessel.vesselName + "";
                //strOldAlarmMessageSOI = KACWorkerGameState.CurrentVessel.vesselName + " - Nearing SOI Change\r\n" +
                //                "     Old SOI: " + KACWorkerGameState.CurrentVessel.orbit.referenceBody.bodyName + "\r\n" +
                //                "     New SOI: " + KACWorkerGameState.CurrentVessel.orbit.nextPatch.referenceBody.bodyName;
                strSOIAlarmName = KACGameState.CurrentVessel.vesselName;// + "-Leaving " + KACWorkerGameState.CurrentVessel.orbit.referenceBody.bodyName;
                strSOIAlarmNotes = KACGameState.CurrentVessel.vesselName + " - Nearing SOI Change\r\n" +
                                "     Old SOI: " + KACGameState.CurrentVessel.orbit.referenceBody.bodyName + "\r\n" +
                                "     New SOI: " + KACGameState.CurrentVessel.orbit.nextPatch.referenceBody.bodyName;
            }

            //is there an SOI alarm for this ship already that has not been triggered
            KACAlarm tmpSOIAlarm =
            Settings.Alarms.Find(delegate(KACAlarm a)
            {
                return
                    (a.VesselID == KACGameState.CurrentVessel.id.ToString())
                    && ((a.TypeOfAlarm == KACAlarm.AlarmType.SOIChangeAuto) || (a.TypeOfAlarm == KACAlarm.AlarmType.SOIChange))
                    && (a.Triggered == false);
            });

            //if theres a manual SOI alarm already then ignore it
            if ((tmpSOIAlarm != null) && tmpSOIAlarm.TypeOfAlarm == KACAlarm.AlarmType.SOIChange)
            {
                //Dont touch manually created SOI Alarms
            }
            else
            {
                //Is there an SOI point
                if (timeSOIChange != 0)
                {
                    timeSOIAlarm = timeSOIChange - Settings.AlarmAutoSOIMargin;
                    //and an existing alarm
                    if (tmpSOIAlarm != null)
                    {
                        //update the time (if more than threshold secs)
                        if ((timeSOIAlarm - KACGameState.CurrentTime.UT) > Settings.AlarmAddSOIAutoThreshold)
                        {
                            tmpSOIAlarm.AlarmTime.UT = timeSOIAlarm;
                        }
                    }
                    //Otherwise if its in the future add a new alarm
                    else if (timeSOIAlarm > KACGameState.CurrentTime.UT)
                    {
                        //Settings.Alarms.Add(new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(), strOldAlarmNameSOI, strOldAlarmMessageSOI, timeSOIAlarm, Settings.AlarmAutoSOIMargin,
                        //    KACAlarm.AlarmType.SOIChange, (Settings.AlarmOnSOIChange_Action > 0), (Settings.AlarmOnSOIChange_Action > 1)));
                        Settings.Alarms.Add(new KACAlarm(KACGameState.CurrentVessel.id.ToString(), strSOIAlarmName, strSOIAlarmNotes, timeSOIAlarm, Settings.AlarmAutoSOIMargin,
                            KACAlarm.AlarmType.SOIChangeAuto, (Settings.AlarmOnSOIChange_Action > 0), (Settings.AlarmOnSOIChange_Action > 1)));
                        Settings.SaveAlarms();
                    }
                }
                else
                {
                    //remove any existing alarm - if less than threshold - this means old alarms not touched
                    if (tmpSOIAlarm != null && (tmpSOIAlarm.Remaining.UT > Settings.AlarmAddSOIAutoThreshold))
                    {
                        Settings.Alarms.Remove(tmpSOIAlarm);
                    }
                }

            }
        }

        private void RecalcSOIAlarmTimes(Boolean OverrideDriftThreshold)
        {
            foreach (KACAlarm tmpAlarm in Settings.Alarms.Where(a => a.TypeOfAlarm == KACAlarm.AlarmType.SOIChange && a.VesselID == KACGameState.CurrentVessel.id.ToString()))
            {
                if (tmpAlarm.Remaining.UT > Settings.AlarmSOIRecalcThreshold)
                {
                    //do the check/update on these
                    if (Settings.SOITransitions.Contains(KACGameState.CurrentVessel.orbit.patchEndTransition))
                    {
                        double timeSOIChange = 0;
                        timeSOIChange = KACGameState.CurrentVessel.orbit.UTsoi;
                        tmpAlarm.AlarmTime.UT = KACGameState.CurrentVessel.orbit.UTsoi - tmpAlarm.AlarmMarginSecs;
                    }
                }
            }
        }

        private void RecalcTransferAlarmTimes(Boolean OverrideDriftThreshold)
        {
            foreach (KACAlarm tmpAlarm in Settings.Alarms.Where(a => a.TypeOfAlarm == KACAlarm.AlarmType.Transfer))
            {
                if (tmpAlarm.Remaining.UT > Settings.AlarmXferRecalcThreshold)
                {
                    KACXFerTarget tmpTarget = new KACXFerTarget();
                    tmpTarget.Origin = FlightGlobals.Bodies.Single(b => b.bodyName == tmpAlarm.XferOriginBodyName);
                    tmpTarget.Target = FlightGlobals.Bodies.Single(b => b.bodyName == tmpAlarm.XferTargetBodyName);

                    //LogFormatted("{0}+{1}-{2}", KACWorkerGameState.CurrentTime.UT.ToString(), tmpTarget.AlignmentTime.UT.ToString(), tmpAlarm.AlarmMarginSecs.ToString());
                    //recalc the transfer spot, but dont move it if the difference is more than the threshold value
                    if (Math.Abs(KACGameState.CurrentTime.UT - tmpTarget.AlignmentTime.UT) < Settings.AlarmXferRecalcThreshold || OverrideDriftThreshold)
                        tmpAlarm.AlarmTime.UT = KACGameState.CurrentTime.UT - tmpAlarm.AlarmMarginSecs + tmpTarget.AlignmentTime.UT;
                }
            }
        }

        List<KACAlarm.AlarmType> TypesToRecalc = new List<KACAlarm.AlarmType>() {KACAlarm.AlarmType.Apoapsis,KACAlarm.AlarmType.Periapsis,
                                                                                KACAlarm.AlarmType.AscendingNode,KACAlarm.AlarmType.DescendingNode};
        private void RecalcNodeAlarmTimes(Boolean OverrideDriftThreshold)
        {
            //only do these recalcs for the current flight plan
            foreach (KACAlarm tmpAlarm in Settings.Alarms.Where(a => TypesToRecalc.Contains(a.TypeOfAlarm) && a.VesselID == KACGameState.CurrentVessel.id.ToString()))
            {
                if (tmpAlarm.Remaining.UT > Settings.AlarmNodeRecalcThreshold)
                {
                    switch (tmpAlarm.TypeOfAlarm)
                    {
                        case KACAlarm.AlarmType.Apoapsis:
                            if (KACGameState.ApPointExists &&
                                ((Math.Abs(KACGameState.CurrentVessel.orbit.timeToAp) > Settings.AlarmNodeRecalcThreshold) || OverrideDriftThreshold))
                                tmpAlarm.AlarmTime.UT = KACGameState.CurrentTime.UT - tmpAlarm.AlarmMarginSecs + KACGameState.CurrentVessel.orbit.timeToAp;
                            break;
                        case KACAlarm.AlarmType.Periapsis:
                            if (KACGameState.PePointExists &&
                                ((Math.Abs(KACGameState.CurrentVessel.orbit.timeToPe) > Settings.AlarmNodeRecalcThreshold) || OverrideDriftThreshold))
                                tmpAlarm.AlarmTime.UT = KACGameState.CurrentTime.UT - tmpAlarm.AlarmMarginSecs + KACGameState.CurrentVessel.orbit.timeToPe;
                            break;
                        case KACAlarm.AlarmType.AscendingNode:
                            Double timeToAN;
                            //Boolean blnANExists = KACUtils.CalcTimeToANorDN(KACWorkerGameState.CurrentVessel, KACUtils.ANDNNodeType.Ascending, out timeToAN);
                            Boolean blnANExists;
                            if (KACGameState.CurrentVesselTarget == null)
                            {
                                blnANExists = KACGameState.CurrentVessel.orbit.AscendingNodeEquatorialExists();
                                timeToAN = KACGameState.CurrentVessel.orbit.TimeOfAscendingNodeEquatorial(KACGameState.CurrentTime.UT) - KACGameState.CurrentTime.UT;
                            }
                            else
                            {
                                blnANExists = KACGameState.CurrentVessel.orbit.AscendingNodeExists(KACGameState.CurrentVesselTarget.GetOrbit());
                                timeToAN = KACGameState.CurrentVessel.orbit.TimeOfAscendingNode(KACGameState.CurrentVesselTarget.GetOrbit(), KACGameState.CurrentTime.UT) - KACGameState.CurrentTime.UT;
                            }

                            if (blnANExists &&
                                ((Math.Abs(timeToAN) > Settings.AlarmNodeRecalcThreshold) || OverrideDriftThreshold))
                                tmpAlarm.AlarmTime.UT = KACGameState.CurrentTime.UT - tmpAlarm.AlarmMarginSecs + timeToAN;
                            break;

                        case KACAlarm.AlarmType.DescendingNode:
                            Double timeToDN;
                            //Boolean blnDNExists = KACUtils.CalcTimeToANorDN(KACWorkerGameState.CurrentVessel, KACUtils.ANDNNodeType.Descending, out timeToDN);
                            Boolean blnDNExists;
                            if (KACGameState.CurrentVesselTarget == null)
                            {
                                blnDNExists = KACGameState.CurrentVessel.orbit.DescendingNodeEquatorialExists();
                                timeToDN = KACGameState.CurrentVessel.orbit.TimeOfDescendingNodeEquatorial(KACGameState.CurrentTime.UT - KACGameState.CurrentTime.UT);
                            }
                            else
                            {
                                blnDNExists = KACGameState.CurrentVessel.orbit.DescendingNodeExists(KACGameState.CurrentVesselTarget.GetOrbit());
                                timeToDN = KACGameState.CurrentVessel.orbit.TimeOfDescendingNode(KACGameState.CurrentVesselTarget.GetOrbit(), KACGameState.CurrentTime.UT) - KACGameState.CurrentTime.UT;
                            }

                            if (blnDNExists &&
                                ((Math.Abs(timeToDN) > Settings.AlarmNodeRecalcThreshold) || OverrideDriftThreshold))
                                tmpAlarm.AlarmTime.UT = KACGameState.CurrentTime.UT - tmpAlarm.AlarmMarginSecs + timeToDN;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void GlobalSOICatchAll(double SecondsTillNextUpdate)
        {
            foreach (Vessel tmpVessel in FlightGlobals.Vessels)
            {
                //only track vessels, not debris, EVA, etc
                //and not the current vessel
                //and no SOI alarm for it within the threshold - THIS BIT NEEDS TUNING
                if (Settings.VesselTypesForSOI.Contains(tmpVessel.vesselType) && (tmpVessel != KACGameState.CurrentVessel) &&
                    (Settings.Alarms.FirstOrDefault(a =>
                        (a.VesselID == tmpVessel.id.ToString() &&
                        (a.TypeOfAlarm == KACAlarm.AlarmType.SOIChange) &&
                        (Math.Abs(a.Remaining.UT) < SecondsTillNextUpdate + Settings.AlarmAddSOIAutoThreshold)
                        )) == null)
                    )
                {
                    if (lstVessels.ContainsKey(tmpVessel.id.ToString()) == false)
                    {
                        //Add new Vessels
                        LogFormatted(String.Format("Adding {0}-{1}-{2}-{3}", tmpVessel.id, tmpVessel.vesselName, tmpVessel.vesselType, tmpVessel.mainBody.bodyName));
                        lstVessels.Add(tmpVessel.id.ToString(), new KACVesselSOI(tmpVessel.vesselName, tmpVessel.mainBody.bodyName));
                    }
                    else
                    {
                        //get this vessel from the memory array we are keeping and compare to its SOI
                        if (lstVessels[tmpVessel.id.ToString()].SOIName != tmpVessel.mainBody.bodyName)
                        {
                            //Set a new alarm to display now
                            KACAlarm newAlarm = new KACAlarm(FlightGlobals.ActiveVessel.id.ToString(), tmpVessel.vesselName + "- SOI Catch",
                                tmpVessel.vesselName + " Has entered a new Sphere of Influence\r\n" +
                                "     Old SOI: " + lstVessels[tmpVessel.id.ToString()].SOIName + "\r\n" +
                                "     New SOI: " + tmpVessel.mainBody.bodyName,
                                 KACGameState.CurrentTime.UT, 0, KACAlarm.AlarmType.SOIChange,
                                (Settings.AlarmOnSOIChange_Action > 0), (Settings.AlarmOnSOIChange_Action > 1));
                            Settings.Alarms.Add(newAlarm);

                            LogFormatted("Triggering SOI Alarm - " + newAlarm.Name);
                            newAlarm.Triggered = true;
                            newAlarm.Actioned = true;
                            if (Settings.AlarmOnSOIChange_Action > 1)
                            {
                                LogFormatted(String.Format("{0}-Pausing Game", newAlarm.Name));
                                TimeWarp.SetRate(0, true);
                                FlightDriver.SetPause(true);
                            }
                            else if (Settings.AlarmOnSOIChange_Action > 0)
                            {
                                LogFormatted(String.Format("{0}-Halt Warp", newAlarm.Name));
                                TimeWarp.SetRate(0, true);
                            }

                            //reset the name String for next check
                            lstVessels[tmpVessel.id.ToString()].SOIName = tmpVessel.mainBody.bodyName;
                        }
                    }
                }
            }
        }

        private void MonitorManNodeOnPath()
        {
            //is there an alarm
            KACAlarm tmpAlarm = Settings.Alarms.FirstOrDefault(a => a.TypeOfAlarm == KACAlarm.AlarmType.ManeuverAuto && a.VesselID == KACGameState.CurrentVessel.id.ToString());

            //is there an alarm and no man node?
            if (KACGameState.ManeuverNodeExists && (KACGameState.ManeuverNodeFuture != null))
            {
                KACTime nodeAutoAlarm;
                nodeAutoAlarm = new KACTime(KACGameState.ManeuverNodeFuture.UT - Settings.AlarmAddManAutoMargin);

                List<ManeuverNode> manNodesToStore = KACGameState.ManeuverNodesFuture;

                String strManNodeAlarmName = KACGameState.CurrentVessel.vesselName;
                String strManNodeAlarmNotes = "Time to pay attention to\r\n    " + KACGameState.CurrentVessel.vesselName + "\r\nNearing Maneuver Node";

                //Are we updating an alarm
                if (tmpAlarm != null)
                {
                    tmpAlarm.AlarmTime.UT = nodeAutoAlarm.UT;
                    tmpAlarm.ManNodes = manNodesToStore;
                }
                else
                {
                    //dont add an alarm if we are within the threshold period
                    if (nodeAutoAlarm.UT + Settings.AlarmAddManAutoMargin - Settings.AlarmAddManAutoThreshold > KACGameState.CurrentTime.UT)
                    {
                        //or are we setting a new one
                        Settings.Alarms.Add(new KACAlarm(FlightGlobals.ActiveVessel.id.ToString(), strManNodeAlarmName, strManNodeAlarmNotes, nodeAutoAlarm.UT, Settings.AlarmAddManAutoMargin, KACAlarm.AlarmType.ManeuverAuto,
                            (Settings.AlarmAddManAuto_Action == (int)KACAlarm.AlarmAction.KillWarp), (Settings.AlarmAddManAuto_Action == (int)KACAlarm.AlarmAction.PauseGame), manNodesToStore));
                        Settings.Save();
                    }
                }
            }
            else if (Settings.AlarmAddManAuto_andRemove && !KACGameState.ManeuverNodeExists)
            {
                Settings.Alarms.Remove(tmpAlarm);
            }
        }

        /// <summary>
        /// Only called when game is in paused state
        /// </summary>
        public void UpdateEarthAlarms()
        {
            foreach (KACAlarm tmpAlarm in Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title).Where(a => a.TypeOfAlarm == KACAlarm.AlarmType.EarthTime))
            {
                tmpAlarm.Remaining.UT = (EarthTime.EarthTimeDecode(tmpAlarm.AlarmTime.UT) - DateTime.Now).TotalSeconds;
            }
        }

        private void ParseAlarmsAndAffectWarpAndPause(double SecondsTillNextUpdate)
        {
            foreach (KACAlarm tmpAlarm in Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title))
            {
                //reset each alarms WarpInfluence flag
                if (KACGameState.CurrentWarpInfluenceStartTime == null)
                    tmpAlarm.WarpInfluence = false;
                else
                    //if the lights been on long enough
                    if (KACGameState.CurrentWarpInfluenceStartTime.AddSeconds(SecondsWarpLightIsShown) < DateTime.Now)
                        tmpAlarm.WarpInfluence = false;

                //Update Remaining interval for each alarm
                if (tmpAlarm.TypeOfAlarm != KACAlarm.AlarmType.EarthTime)
                    tmpAlarm.Remaining.UT = tmpAlarm.AlarmTime.UT - KACGameState.CurrentTime.UT;
                else
                    tmpAlarm.Remaining.UT = (EarthTime.EarthTimeDecode(tmpAlarm.AlarmTime.UT) - DateTime.Now).TotalSeconds;

                //set triggered for passed alarms so the OnGUI part can draw the window later
                //if ((KACWorkerGameState.CurrentTime.UT >= tmpAlarm.AlarmTime.UT) && (tmpAlarm.Enabled) && (!tmpAlarm.Triggered))
                if ((tmpAlarm.Remaining.UT <= 0) && (tmpAlarm.Enabled) && (!tmpAlarm.Triggered))
                {
                    if (tmpAlarm.ActionedAt > 0)
                    {
                        LogFormatted("Suppressing Alarm due to Actioned At being set:{0}", tmpAlarm.Name);
                        tmpAlarm.Triggered = true;
                        tmpAlarm.Actioned = true;
                        tmpAlarm.AlarmWindowClosed = true;
                    }
                    else
                    {

                        LogFormatted("Triggering Alarm - " + tmpAlarm.Name);
                        tmpAlarm.Triggered = true;

                        //If we are simply past the time make sure we halt the warp
                        //only do this in flight mode
                        if (!ViewAlarmsOnly)
                        {
                            if (tmpAlarm.PauseGame)
                            {
                                LogFormatted(String.Format("{0}-Pausing Game", tmpAlarm.Name));
                                TimeWarp.SetRate(0, true);
                                FlightDriver.SetPause(true);
                            }
                            else if (tmpAlarm.HaltWarp)
                            {
                                if (!FlightDriver.Pause)
                                {
                                    LogFormatted(String.Format("{0}-Halt Warp", tmpAlarm.Name));
                                    TimeWarp.SetRate(0, true);
                                }
                                else
                                {
                                    LogFormatted(String.Format("{0}-Game paused, skipping Halt Warp", tmpAlarm.Name));
                                }
                            }
                        }
                    }
                }


                //skip this if we aren't in flight mode
                if (!ViewAlarmsOnly)
                {
                    //if in the next two updates we would pass the alarm time then slow down the warp
                    if (!tmpAlarm.Actioned && tmpAlarm.Enabled && (tmpAlarm.HaltWarp || tmpAlarm.PauseGame))
                    {
                        Double TimeNext = KACGameState.CurrentTime.UT + SecondsTillNextUpdate * 2;
                        //LogFormatted(CurrentTime.UT.ToString() + "," + TimeNext.ToString());
                        if (TimeNext > tmpAlarm.AlarmTime.UT)
                        {
                            tmpAlarm.WarpInfluence = true;
                            KACGameState.CurrentlyUnderWarpInfluence = true;
                            KACGameState.CurrentWarpInfluenceStartTime = DateTime.Now;

                            TimeWarp w = TimeWarp.fetch;
                            if (w.current_rate_index > 0)
                            {
                                LogFormatted("Reducing Warp");
                                TimeWarp.SetRate(w.current_rate_index - 1, true);
                            }
                        }
                    }
                }
            }
        }


        #region RestoreNodesSection
                private Int32 targetToRestoreAttempts = 0;
        private Int32 manToRestoreAttempts = 0;

        public void RestoreManeuverNodeList(List<ManeuverNode> newManNodes)
        {
            
            foreach (ManeuverNode tmpMNode in newManNodes)
            {
                RestoreManeuverNode(tmpMNode);
            }
        }

        public void RestoreManeuverNode(ManeuverNode newManNode)
        {
            ManeuverNode tmpNode = FlightGlobals.ActiveVessel.patchedConicSolver.AddManeuverNode(newManNode.UT);
            tmpNode.DeltaV = newManNode.DeltaV;
            tmpNode.nodeRotation = newManNode.nodeRotation;
            FlightGlobals.ActiveVessel.patchedConicSolver.UpdateFlightPlan();
        }

        #endregion
    }
}
