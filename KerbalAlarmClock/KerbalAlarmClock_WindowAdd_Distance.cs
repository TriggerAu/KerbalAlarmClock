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

        internal void WindowLayout_AddTypeDistanceChoice()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Distance Type:", KACResources.styleAddHeading);
            int intOption = 0;
            if (AddType != KACAlarm.AlarmTypeEnum.Closest) intOption = 1;
            if (DrawRadioList(ref intOption, "Closest", "Target Distance"))
            {
                if (intOption == 0)
                    AddType = KACAlarm.AlarmTypeEnum.Closest;
                else
                    AddType = KACAlarm.AlarmTypeEnum.Distance;
                AddTypeChanged();
            }
            GUILayout.EndHorizontal();
        }

        private Int32 intOrbits;
        private Single fltOrbits = 6;
        private void WindowLayout_AddPane_ClosestApproach()
        {
            GUILayout.BeginVertical();
            GUILayout.Label(strAlarmEventName + " Details...", KACResources.styleAddSectionHeading);

            if (KACWorkerGameState.CurrentVessel == null)
                GUILayout.Label("No Active Vessel");
            else
            {
                if (!(KACWorkerGameState.CurrentVesselTarget is Vessel) && !(KACWorkerGameState.CurrentVesselTarget is ModuleDockingNode))
                {
                    GUILayout.Label("No valid Vessel Target Selected", GUILayout.ExpandWidth(true));
                }
                else
                {
                    //GUILayout.Label("Adjust Lookahead amounts...", KACResources.styleAddSectionHeading);

                    GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Orbits to Search:", KACResources.styleAddHeading, GUILayout.Width(110));
                    GUILayout.Label(((int)Math.Round((Decimal)fltOrbits, 0)).ToString(), KACResources.styleAddXferName, GUILayout.Width(25));
                    fltOrbits = GUILayout.HorizontalSlider(fltOrbits, 1, 20);
                    fltOrbits = (float)Math.Floor((Decimal)fltOrbits);
                    GUILayout.EndHorizontal();

                    intOrbits = (int)fltOrbits;
                    int intClosestOrbitPass = 0;
                    double dblClosestDistance = Double.MaxValue;
                    double dblClosestUT = 0;

                    double dblOrbitTestClosest = Double.MaxValue;
                    double dblOrbitTestClosestUT = 0;
                    if (KACWorkerGameState.CurrentVessel.orbit.eccentricity > 1)
                        intOrbits = 1;
                    for (int intOrbitToTest = 1; intOrbitToTest <= intOrbits; intOrbitToTest++)
                    {
                        dblOrbitTestClosestUT = KACUtils.timeOfClosestApproach(KACWorkerGameState.CurrentVessel.orbit,
                                                                            KACWorkerGameState.CurrentVesselTarget.GetOrbit(),
                                                                            KACWorkerGameState.CurrentTime.UT,
                                                                            intOrbitToTest,
                                                                            out dblOrbitTestClosest
                                                                            );
                        if (dblOrbitTestClosest < dblClosestDistance)
                        {
                            dblClosestDistance = dblOrbitTestClosest;
                            dblClosestUT = dblOrbitTestClosestUT;
                            intClosestOrbitPass = intOrbitToTest;
                        }
                    }


                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Distance:", KACResources.styleAddHeading, GUILayout.Width(70));
                    String strDistance = string.Format("{0:#}m", dblClosestDistance);
                    if (dblClosestDistance > 999) strDistance = string.Format("{0:#.0}km", dblClosestDistance / 1000);
                    GUILayout.Label(strDistance, KACResources.styleAddXferName, GUILayout.Width(90));
                    GUILayout.Label("On Orbit:", KACResources.styleAddHeading);
                    GUILayout.Label(intClosestOrbitPass.ToString(), KACResources.styleAddXferName);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();


                    String strMarginConversion = "";
                    KSPDateTime eventTime = new KSPDateTime(dblClosestUT);
                    KSPTimeSpan eventInterval = new KSPTimeSpan(dblClosestUT - KACWorkerGameState.CurrentTime.UT);

                    KSPDateTime eventAlarm;
                    KSPTimeSpan eventAlarmInterval;
                    try
                    {
                        eventAlarm = new KSPDateTime(eventTime.UT - timeMargin.UT);
                        eventAlarmInterval = new KSPTimeSpan(eventTime.UT - KACWorkerGameState.CurrentTime.UT - timeMargin.UT);
                    }
                    catch (Exception)
                    {
                        eventAlarm = null;
                        eventAlarmInterval = null;
                        strMarginConversion = "Unable to Add the Margin Minutes";
                    }

                    if ((eventTime.UT > KACWorkerGameState.CurrentTime.UT) && strMarginConversion == "")
                    {
                        if (DrawAddAlarm(eventTime, eventInterval, eventAlarmInterval))
                        {
                            KACAlarm newAlarm = new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(), strAlarmName, strAlarmNotes,
                                eventAlarm.UT, timeMargin.UT, AddType,
                                AddActions);
                            newAlarm.TargetObject = KACWorkerGameState.CurrentVesselTarget;
                            newAlarm.ManNodes = KACWorkerGameState.CurrentVessel.patchedConicSolver.maneuverNodes;

                            alarms.Add(newAlarm);
                            //settings.Save();
                            _ShowAddPane = false;
                        }
                    }
                    else
                    {
                        strMarginConversion = "No Future Closest Approach found";
                    }

                    if (strMarginConversion != "")
                        GUILayout.Label(strMarginConversion, GUILayout.ExpandWidth(true));
                }
            }

            GUILayout.EndVertical();
        }



        //TODO:Need to rethink this - something is wrong in the orbit selection!



        private Int32 intOrbits_Distance;
        private Single fltOrbits_Distance = 6;
        private Int32 intSelectediTarget = 0;
        private ITargetable tgtSelectedDistance = null;
        private Double dblTargetDistance = 100000;

        private Int32 intAddDistanceHeight = 272;
        private void WindowLayout_AddPane_TargetDistance()
        {
            intAddDistanceHeight = 262;// 272;
            GUILayout.BeginVertical();
            GUILayout.Label(strAlarmEventName + " Details...", KACResources.styleAddSectionHeading);

            //What are the possible targets??
            List<ITargetable> iTargets = new List<ITargetable>();
            if (!(KACWorkerGameState.CurrentVesselTarget == null))
            {
                iTargets.Add(KACWorkerGameState.CurrentVesselTarget);   //VesselTarget
            }
            iTargets.Add(KACWorkerGameState.CurrentVessel.mainBody);    //Body we are orbiting
            if (KACWorkerGameState.SOIPointExists)
            {
                iTargets.Add(KACWorkerGameState.CurrentVessel.orbit.nextPatch.referenceBody);   //Body we will orbit next
            }

            if (intSelectediTarget > iTargets.Count - 1) 
                intSelectediTarget = 0;
            
            intAddDistanceHeight += (iTargets.Count*30);

            //Now give the user the choice
            GUILayout.BeginHorizontal();
            GUILayout.Label("Select Target:",KACResources.styleAddXferName);
            if (DrawRadioListVertical(ref intSelectediTarget, iTargets.Select(x => x.GetName()).ToArray()))
            {
                LogFormatted("Distance Target is:{0}", iTargets[intSelectediTarget].GetName());
            }
            GUILayout.EndHorizontal();

            //Set the tgt Object
            tgtSelectedDistance=iTargets[intSelectediTarget];
            string strDistanceName = "Distance";
            if (tgtSelectedDistance is CelestialBody) strDistanceName = "Altitude";

            //Ask for the target distance/altitude
            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Format("Target {0} (m):",strDistanceName), KACResources.styleAddXferName);
            dblTargetDistance = Convert.ToDouble(GUILayout.TextField(dblTargetDistance.ToString(),KACResources.styleAddField));
            GUILayout.EndHorizontal();

            //If the body has an atmosphere then add an option to set the Altitude straight to that
            if (tgtSelectedDistance is CelestialBody)
            {
                if ((tgtSelectedDistance as CelestialBody).atmosphere)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(string.Format("Atmosphere: {0}", (tgtSelectedDistance as CelestialBody).atmosphereDepth));
                    if (GUILayout.Button("Set to Edge"))
                    {
                        dblTargetDistance = (tgtSelectedDistance as CelestialBody).atmosphereDepth;
                    }
                    GUILayout.EndHorizontal();
                    intAddDistanceHeight += 26;
                }
            }

            //For a vessel give some options for orbits to look forwards
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            if (!(tgtSelectedDistance is CelestialBody))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Orbits to Search:", KACResources.styleAddHeading, GUILayout.Width(110));
                GUILayout.Label(((int)Math.Round((Decimal)fltOrbits_Distance, 0)).ToString(), KACResources.styleAddXferName, GUILayout.Width(25));
                fltOrbits_Distance = GUILayout.HorizontalSlider(fltOrbits_Distance, 1, 20);
                fltOrbits_Distance = (float)Math.Floor((Decimal)fltOrbits_Distance);
                GUILayout.EndHorizontal();
                intAddDistanceHeight += 18;
            }
            
            //What VesselOrbit do we care about
            Orbit VesselOrbitToCompare=KACWorkerGameState.CurrentVessel.GetOrbit();
            if ((KACWorkerGameState.SOIPointExists) && ((tgtSelectedDistance as CelestialBody) == KACWorkerGameState.CurrentVessel.orbit.nextPatch.referenceBody))
            {
                VesselOrbitToCompare = KACWorkerGameState.CurrentVessel.orbit.nextPatch;
            }
            //Get the startUT of the orbit
            Double VesselOrbitStartUT = KACWorkerGameState.CurrentVessel.GetOrbit().StartUT;

            //Set up some variables
            intOrbits_Distance = (int)fltOrbits_Distance;
            int intDistanceOrbitPass = 0;
            double dblClosestDistance = Double.MaxValue;
            double dblDistanceUT = 0;

            double dblOrbitTestDistance = Double.MaxValue;
            double dblOrbitTestDistanceUT = 0;

            //If its an Altitude alarm then do this
            if (tgtSelectedDistance is CelestialBody)
            {
                dblOrbitTestDistanceUT = KACUtils.timeOfTargetAltitude(VesselOrbitToCompare,
                                            VesselOrbitStartUT,
                                            out dblOrbitTestDistance,
                                            dblTargetDistance
                                            );

                dblClosestDistance = dblOrbitTestDistance;
                dblDistanceUT = dblOrbitTestDistanceUT;
            }
            else
            {
                //Else Iterate through the orbits to find the target separation
                for (int intOrbitToTest = 1; intOrbitToTest <= intOrbits_Distance; intOrbitToTest++)
                {
                    dblOrbitTestDistanceUT = KACUtils.timeOfTargetDistance(VesselOrbitToCompare,
                                                                tgtSelectedDistance.GetOrbit(),
                                                                KACWorkerGameState.CurrentTime.UT,
                                                                intOrbitToTest,
                                                                out dblOrbitTestDistance,
                                                                dblTargetDistance
                                                                );

                    if (dblOrbitTestDistance < dblClosestDistance)
                    {
                        dblClosestDistance = dblOrbitTestDistance;
                        dblDistanceUT = dblOrbitTestDistanceUT;
                        intDistanceOrbitPass = intOrbitToTest;
                    }
                }
            }

            //Now display what we got            
            GUILayout.BeginHorizontal();
            GUILayout.Label(String.Format("{0}:",strDistanceName), KACResources.styleAddHeading, GUILayout.Width(70));
            String strDistance = string.Format("{0:#}m", dblClosestDistance);
            if (dblClosestDistance > 999) strDistance = string.Format("{0:#.0}km", dblClosestDistance / 1000);
            GUILayout.Label(strDistance, KACResources.styleAddXferName, GUILayout.Width(90));
            if (!(tgtSelectedDistance is CelestialBody))
            {
                GUILayout.Label("On Orbit:", KACResources.styleAddHeading);
                GUILayout.Label(intDistanceOrbitPass.ToString(), KACResources.styleAddXferName);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            //Now do the stuff to draw the alarm button
            String strMarginConversion = "";
            KSPDateTime eventTime = new KSPDateTime(dblDistanceUT);
            KSPTimeSpan eventInterval = new KSPTimeSpan(dblDistanceUT - KACWorkerGameState.CurrentTime.UT);

            KSPDateTime eventAlarm;
            KSPTimeSpan eventAlarmInterval;
            try
            {
                eventAlarm = new KSPDateTime(eventTime.UT - timeMargin.UT);
                eventAlarmInterval = new KSPTimeSpan(eventTime.UT - KACWorkerGameState.CurrentTime.UT - timeMargin.UT);
            }
            catch (Exception)
            {
                eventAlarm = null;
                eventAlarmInterval = null;
                strMarginConversion = "Unable to Add the Margin Minutes";
            }

            if ((eventTime.UT > KACWorkerGameState.CurrentTime.UT) && strMarginConversion == "")
            {
                if (DrawAddAlarm(eventTime, eventInterval, eventAlarmInterval))
                {
                    KACAlarm newAlarm = new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(), strAlarmName, strAlarmNotes,
                        eventAlarm.UT, timeMargin.UT, AddType,
                        AddActions);
                    newAlarm.TargetObject = KACWorkerGameState.CurrentVesselTarget;
                    newAlarm.ManNodes = KACWorkerGameState.CurrentVessel.patchedConicSolver.maneuverNodes;

                    alarms.Add(newAlarm);
                    //settings.Save();
                    _ShowAddPane = false;
                }
            }
            else
            {
                strMarginConversion = "No Target Distance Approach found";
            }

            if (strMarginConversion != "")
                GUILayout.Label(strMarginConversion, GUILayout.ExpandWidth(true));

            GUILayout.EndVertical();
        }



    }
}
