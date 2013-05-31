using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using UnityEngine;
using KSP;

namespace KerbalAlarmClock
{
    public partial class KACWorker 
    {
        //public static double timeOfClosestApproach(Orbit a, Orbit b, double time,out double closestdistance)
        //{
            
        //    return timeOfClosestApproach(a, b, time, a.period, 20, out closestdistance);
        //}

        //public static double timeOfClosestApproach(Orbit a, Orbit b, double time, int orbit, out double closestdistance)
        //{
        //    //return timeOfClosestApproach(a, b, time + ((orbit - 1) * a.period), (orbit * a.period), 20, out closestdistance);
        //    return timeOfClosestApproach(a, b, time + ((orbit - 1) * a.period), a.period, 20, out closestdistance);
        //}

        //public static double timeOfClosestApproach(Orbit a, Orbit b, double time, double periodtoscan, double numDivisions,out double closestdistance)
        //{
        //    double closestApproachTime = time;
        //    double closestApproachDistance = Double.MaxValue;
        //    double minTime = time;
        //    double maxTime = time + periodtoscan;
        //    //int numDivisions = 20;

        //    for (int iter = 0; iter < 8; iter++)
        //    {
        //        double dt = (maxTime - minTime) / numDivisions;
        //        for (int i = 0; i < numDivisions; i++)
        //        {
        //            double t = minTime + i * dt;
        //            double distance = (a.getRelativePositionAtUT(t) + a.referenceBody.position - (b.getRelativePositionAtUT(t)+b.referenceBody.position)).magnitude;
        //            if (distance < closestApproachDistance)
        //            {
        //                closestApproachDistance = distance;
        //                closestApproachTime = t;
        //            }
        //        }
        //        minTime = KACUtils.Clamp(closestApproachTime - dt, time, time + a.period);
        //        maxTime = KACUtils.Clamp(closestApproachTime + dt, time, time + a.period);
        //    }

        //    closestdistance = closestApproachDistance;
        //    return closestApproachTime;
        //}

        public void DebugActionTimed(GameScenes loadedscene)
        {
            DebugLogFormatted("Timed Debug Action Initiated");
            //KACWorker.DebugLogFormatted("Stuff Here");
            //KACWorker.DebugLogFormatted(FlightGlobals.ActiveVessel.orbit.closestEncounterBody.bodyName);
            //KACWorker.DebugLogFormatted(FlightGlobals.ActiveVessel.orbit.ClAppr.ToString());
            //KACWorker.DebugLogFormatted(FlightGlobals.ActiveVessel.orbit.closestEncounterBody.sphereOfInfluence.ToString());

            // how to detect Escape - eg to Solar orbit

            //Detect SOI Change and time
            //if ((FlightGlobals.ActiveVessel.orbit.closestEncounterBody != null) && (FlightGlobals.ActiveVessel.orbit.ClAppr > 0))
            //{
            //    //Is the closest approach less than the size of the SOI
            //    KACWorker.DebugLogFormatted(FlightGlobals.ActiveVessel.orbit.referenceBody + "," + FlightGlobals.ActiveVessel.orbit.closestEncounterBody + "," +
            //         FlightGlobals.ActiveVessel.orbit.nextPatch.referenceBody + "," + FlightGlobals.ActiveVessel.orbit.nextPatch.closestEncounterBody);
            //    if (FlightGlobals.ActiveVessel.orbit.ClAppr < FlightGlobals.ActiveVessel.orbit.closestEncounterBody.sphereOfInfluence)
            //    {
            //        KACWorker.DebugLogFormatted("SOI Change in :" + (FlightGlobals.ActiveVessel.orbit.nextPatch.StartUT - Planetarium.GetUniversalTime()));
            //    }
            //    else
            //    {
            //        KACWorker.DebugLogFormatted("Nextpatch in :" + (FlightGlobals.ActiveVessel.orbit.nextPatch.StartUT - Planetarium.GetUniversalTime()));
            //    }
            //}



            //looking for next action on the path use orbit.patchEndTransition - enum of Orbit.PatchTransitionType
            //  FINAL - fixed orbit no change
            //  ESCAPE - leaving SOI
            //  Intercept - entering new SOI inside current SOI
            //  INITIAL - ???
            //  MANEUVER - MAneuver Node
            //
            // orbit.UTsoi - time of next SOI change (base on above transition types - ie if type is final this time can be ignored)
            //orbit.nextpatch gives you the next orbit and you can read the new SOI!!!


            //Maneuver Node
            //To recreate should only need DeltaV, NodeRotation and UT of Node


            //write orbit, next orbit, patchedconicsolver nodes?
            //see what we need to store for a manuever node

            //FlightGlobals.Vessels[0].patchedConicSolver.maneuverNodes[0]

            //if (tmpVessel.orbit.nextPatch == null)
            //{
            //    DebugLogFormatted(tmpVessel.name + "-No next Patch");
            //}
            //else
            //{
            //    DebugLogFormatted(tmpVessel.name + "-Next patch @ " + (tmpVessel.orbit.nextPatch.StartUT-CurrentTime.UT));
            //    DebugLogFormatted("Same orbit: " + (tmpVessel.orbit == tmpVessel.orbit.nextPatch));
            //}
        }


        public void DebugActionTriggered(GameScenes loadedscene)
        {
            DebugLogFormatted("Manual Debug Action Initiated");

            //String strLine;
            //foreach (CelestialBody cbTemp in FlightGlobals.Bodies)
            //{
            //    strLine=String.Format("{0}({1}),", cbTemp.bodyName, Enum.GetName(typeof(CelestialBodyType), cbTemp.bodyType));
            //    try
            //    {
            //        strLine += String.Format("parent-{0},", cbTemp.referenceBody.bodyName);
            //    }
            //    catch (Exception) { }
            //    try
            //    {
            //        strLine += String.Format("radius-{0},", cbTemp.orbit.radius.ToString());
            //    }
            //    catch (Exception) { }
            //    try
            //    {
            //        strLine += String.Format("sma-{0},", cbTemp.orbit.semiMajorAxis.ToString());
            //    }
            //    catch (Exception) { }
            //    try
            //    {
            //        strLine += String.Format("period-{0},", cbTemp.orbit.period.ToString());
            //    }
            //    catch (Exception) { }

            //    try
            //    {
            //        strLine += String.Format("tA-{0},", cbTemp.orbit.trueAnomaly);
            //    }
            //    catch (Exception) { }
            //    try
            //    {
            //        strLine += String.Format("aOP-{0},", cbTemp.orbit.argumentOfPeriapsis);
            //    }
            //    catch (Exception) { }
            //    try
            //    {
            //        strLine += String.Format("LAN-{0},", cbTemp.orbit.LAN);
            //    }
            //    catch (Exception) { }

            //    DebugLogFormatted(strLine);
            //}


            //DebugLogFormatted("window textcolor r:{0}", KACResources.styleWindow.normal.textColor.r.ToString());
            //byte[] b = KSP.IO.IOUtils.SerializeToBinary(FlightGlobals.ActiveVessel.orbit);
            //bw.Write(b);
            //bw.Close();
            //KSP.IO.BinaryWriter bw = KSP.IO.BinaryWriter.CreateForType<KerbalAlarmClock>("testfile.bin");
            

            //DebugLogFormatted(scrollPosition.ToString());
            //Print each of the vessels UTSOI
            //foreach (Vessel tmpVessel in FlightGlobals.Vessels)
            //{
            //    DebugLogFormatted("{0}-{1}", tmpVessel.vesselName, tmpVessel.orbit.UTsoi.ToString());            
            //}

            
            //DebugLogFormatted(FlightGlobals.ActiveVessel.id.ToString());

            //Orbit o=FlightGlobals.ActiveVessel.orbit;

            //WriteOrbitFile(o,"Debug\\Orbit.txt");
            //if (o.nextPatch != null)
            //    WriteOrbitFile(o.nextPatch, "Debug\\OrbitNext.txt");

            //WriteManeuverFile(FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes,"Debug\\Nodes.txt");
        }



            //Orbit activeOrbit = FlightGlobals.fetch.activeVessel.orbit;
            //Orbit targetOrbit = (FlightGlobals.fetch.VesselTarget as Vessel).orbit;
            //Vector3d activePosition = activeOrbit.getRelativePositionAtUT(Planetarium.GetUniversalTime());
            //double ascendingNode = CalcAngleToAscendingNode(activePosition, activeOrbit, targetOrbit);
            //double timeToAN = CalcTimeToNode(activeOrbit, ascendingNode);

            //DebugLogFormatted("AN:{0}", timeToAN.ToString());



        //private double CalcAngleToAscendingNode(Vector3d position, Orbit origin, Orbit target)
        //{
        //    double angleToNode = 0d;

        //    if (origin.inclination < 90)
        //    {
        //        angleToNode = CalcPhaseAngle(position, GetAscendingNode(origin, target));
        //    }
        //    else
        //    {
        //        angleToNode = 360 - CalcPhaseAngle(position, GetAscendingNode(origin, target));
        //    }

        //    return angleToNode;
        //}

        //private Vector3d GetAscendingNode(Orbit origin, Orbit target)
        //{
        //    return Vector3d.Cross(target.GetOrbitNormal(), origin.GetOrbitNormal());
        //}

        //private double CalcPhaseAngle(Vector3d origin, Vector3d target)
        //{
        //    double phaseAngle = Vector3d.Angle(target, origin);
        //    if (Vector3d.Angle(Quaternion.AngleAxis(90, Vector3d.forward) * origin, target) > 90)
        //    {
        //        phaseAngle = 360 - phaseAngle;
        //    }
        //    return (phaseAngle + 360) % 360;
        //}



        //private double CalcTimeToNode(Orbit origin, double angleToNode)
        //{
        //    return (origin.period / 360d) * angleToNode;
        //}

        //int intTestheight = 156;
        //int intTestheight2 = 0;
        //int intTestheight3 = 0;
        //int intTestheight4 = 0;
        //int intTestheight3 = 336;

        public void FillDebugWindow(int WindowID)
        {
            GUILayout.BeginVertical();
            //GUILayout.BeginHorizontal();
            ////GUILayout.Label("Alarm Add Interface:", KACResources.styleAddHeading, GUILayout.Width(90));
            ////AddInterfaceType = Convert.ToInt32(GUILayout.TextField(AddInterfaceType.ToString()));
            //GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("test:");
            //GUILayout.Label("test2:");
            //GUILayout.Label("test3:");
            //GUILayout.Label("test4:");

            GUILayout.EndVertical();
            GUILayout.BeginVertical();

            
            //intTestheight = Convert.ToInt32(GUILayout.TextField(intTestheight.ToString()));
            //intTestheight2 = Convert.ToInt32(GUILayout.TextField(intTestheight2.ToString()));
            //intTestheight3 = Convert.ToInt32(GUILayout.TextField(intTestheight3.ToString()));
            //intTestheight4 = Convert.ToInt32(GUILayout.TextField(intTestheight4.ToString()));

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();





            //GUILayout.BeginVertical();
            //GUILayout.Label("Closest:");
            //GUILayout.Label("ClosestIn5:");
            //GUILayout.Label("test2:");
            //GUILayout.Label("test3:");
            //GUILayout.Label("test4:");

            //GUILayout.Label("AN1:");
            //GUILayout.Label("AN2:");
            //GUILayout.Label("Window Padding:", GUILayout.ExpandWidth(true));
            //GUILayout.Label("MainWindowWidth:", GUILayout.ExpandWidth(true));
            //GUILayout.Label("MainWindowMinHeight:", GUILayout.ExpandWidth(true));
            //GUILayout.Label("MainWindowBaseHeight:", GUILayout.ExpandWidth(true));
            //GUILayout.Label("MainWindowAlarmListItem:", GUILayout.ExpandWidth(true));
            //GUILayout.Label("MainWindowAlarmListScrollPad:", GUILayout.ExpandWidth(true));
            //GUILayout.Label("PaneWidth:", GUILayout.ExpandWidth(true));



            //GUILayout.Label("Moho", GUILayout.ExpandWidth(true));
            //GUILayout.Label("Eve", GUILayout.ExpandWidth(true));
            //GUILayout.Label("Duna", GUILayout.ExpandWidth(true));
            //GUILayout.Label("Dres", GUILayout.ExpandWidth(true));
            //GUILayout.Label("Jool", GUILayout.ExpandWidth(true));
            //GUILayout.Label("Eeloo", GUILayout.ExpandWidth(true));

            //GUILayout.EndVertical();


            //intTestheight = Convert.ToInt32(GUILayout.TextField(intTestheight.ToString()));
            //intTestheight2 = Convert.ToInt32(GUILayout.TextField(intTestheight2.ToString()));
            //intTestheight3 = Convert.ToInt32(GUILayout.TextField(intTestheight3.ToString()));
            //intTestheight4 = Convert.ToInt32(GUILayout.TextField(intTestheight4.ToString()));


            ////Double timeToAN;
            ////Boolean blnANExists = KACUtils.CalcTimeToANorDN(KACWorkerGameState.CurrentVessel, KACUtils.ANDNNodeType.Ascending, out timeToAN);
            //GUILayout.Label(Settings.XferUseModelData.ToString());
            //GUILayout.Label(Settings.XferModelDataLoaded.ToString());
            //if (blnANExists)
            //    GUILayout.Label(timeToAN.ToString());

            //intTestheight3 = Convert.ToInt32(GUILayout.TextField(intTestheight3.ToString()));
            ////intSettingsPaneHeight = Convert.ToInt32(GUILayout.TextField(intSettingsPaneHeight.ToString()));
            //String strPadding = KACResources.styleWindow.padding.left.ToString();
            //int intPadding = Convert.ToInt32(GUILayout.TextField(strPadding));
            //KACResources.styleWindow.padding = KACUtils.SetWindowRectOffset(KACResources.styleWindow.padding, intPadding);
            //    //.left = intPadding;// = new RectOffset(intPadding, intPadding, intPadding, intPadding);
            //intMainWindowWidth = Convert.ToInt32(GUILayout.TextField(intMainWindowWidth.ToString()));
            //intMainWindowMinHeight = Convert.ToInt32(GUILayout.TextField(intMainWindowMinHeight.ToString()));
            //intMainWindowBaseHeight = Convert.ToInt32(GUILayout.TextField(intMainWindowBaseHeight.ToString()));
            //intMainWindowSOIAutoHeight = Convert.ToInt32(GUILayout.TextField(intMainWindowSOIAutoHeight.ToString()));
            //intMainWindowAlarmListItemHeight = Convert.ToInt32(GUILayout.TextField(intMainWindowAlarmListItemHeight.ToString()));
            //intMainWindowAlarmListScrollPad = Convert.ToInt32(GUILayout.TextField(intMainWindowAlarmListScrollPad.ToString()));

            //intPaneWindowWidth = Convert.ToInt32(GUILayout.TextField(intPaneWindowWidth.ToString()));

            //CurrentPhase (Desired Phase: diff)
            //GUILabelPhaseApproach(FlightGlobals.Bodies[1].orbit, FlightGlobals.Bodies[4].orbit);
            //GUILabelPhaseApproach(FlightGlobals.Bodies[1].orbit, FlightGlobals.Bodies[5].orbit);
            //GUILabelPhaseApproach(FlightGlobals.Bodies[1].orbit, FlightGlobals.Bodies[6].orbit);
            //GUILabelPhaseApproach(FlightGlobals.Bodies[1].orbit, FlightGlobals.Bodies[15].orbit);
            //GUILabelPhaseApproach(FlightGlobals.Bodies[1].orbit, FlightGlobals.Bodies[8].orbit);
            //GUILabelPhaseApproach(FlightGlobals.Bodies[1].orbit, FlightGlobals.Bodies[16].orbit);


            //if (FlightGlobals.fetch.VesselTarget != null)
            //{
            //    KerbalTime ktmp;
            //    double closestdistance;
            //    ktmp = new KerbalTime(timeOfClosestApproach(KACWorkerGameState.CurrentVessel.orbit,
            //        FlightGlobals.fetch.VesselTarget.GetOrbit(),
            //        KACWorkerGameState.CurrentTime.UT,
            //            out closestdistance) - KACWorkerGameState.CurrentTime.UT);
            //    GUILayout.Label(KerbalTime.PrintInterval(ktmp, Settings.TimeFormat));

            //    //ktmp = new KerbalTime(timeOfClosestApproach(KACWorkerGameState.CurrentVessel.orbit,
            //    //    FlightGlobals.fetch.VesselTarget.GetOrbit(),
            //    //    KACWorkerGameState.CurrentTime.UT,
            //    //    KACWorkerGameState.CurrentVessel.orbit.period * 5,150) - KACWorkerGameState.CurrentTime.UT);
            //    //GUILayout.Label(KerbalTime.PrintInterval(ktmp, Settings.TimeFormat));

            //    for (int i = 1; i < 6; i++)
            //    {

            //        ktmp = new KerbalTime(timeOfClosestApproach(KACWorkerGameState.CurrentVessel.orbit,
            //            FlightGlobals.fetch.VesselTarget.GetOrbit(),
            //            KACWorkerGameState.CurrentTime.UT,
            //            i,
            //            out closestdistance)
            //            - KACWorkerGameState.CurrentTime.UT);
            //        string strDisplay = string.Format("s:{0:0},e:{1:0},p:{2:0},dist:{3:0},Time:{4:0}",
            //            KACWorkerGameState.CurrentTime.UT + ((i - 1) * KACWorkerGameState.CurrentVessel.orbit.period),
            //           KACWorkerGameState.CurrentTime.UT + ((i) * KACWorkerGameState.CurrentVessel.orbit.period),
            //            KACWorkerGameState.CurrentVessel.orbit.period,
            //            closestdistance,
            //            ktmp.UT);
            //        GUILayout.Label(strDisplay);
            //        GUILayout.Label(string.Format("\to:{0}-{1}", KerbalTime.PrintInterval(ktmp, Settings.TimeFormat), closestdistance));
            //    }
            //    /////////////////////////////////////////////////////////////////////////
            //    //need to draw up start and end of each prbit times to see whats going on!!
            //    /////////////////////////////////////////////////////////////////////////


            //    //try doing same values as first orbit, but do start from end 1st orbit and go one orbit
            //}
            //else
            //{
            //    GUILayout.Label("NoTarget");
            //    GUILayout.Label("NoTarget");
            //}


            //GUILayout.Label(blnRecalc.ToString());

            GUILayout.EndVertical();

        }
        //bool blnRecalc = false;

        //private void GUILabelPhaseApproach(Orbit orbitOrig,Orbit orbitTarget)
        //{
        //    double angleTarget = KACUtils.clampDegrees360(180 * (1 - Math.Pow((orbitOrig.semiMajorAxis + orbitTarget.semiMajorAxis) / (2 * orbitTarget.semiMajorAxis), 1.5)));
        //    double angleCurrent = KACUtils.clampDegrees360(orbitTarget.trueAnomaly + orbitTarget.argumentOfPeriapsis + orbitTarget.LAN - (orbitOrig.trueAnomaly + orbitOrig.argumentOfPeriapsis + orbitOrig.LAN));
            
        //    double angleChangepersec = (360 / orbitTarget.period) - (360 / orbitOrig.period);

        //    double angleToMakeUp =angleCurrent-angleTarget;
        //    if (angleToMakeUp > 0 && angleChangepersec > 0)
        //        angleToMakeUp -= 360;
        //    if (angleToMakeUp < 0 && angleChangepersec < 0)
        //        angleToMakeUp += 360;

        //    double UTToTarget = Math.Floor(Math.Abs(angleToMakeUp / angleChangepersec));

        //    double UTTimeForAlarm = Math.Floor(KACWorkerGameState.CurrentTime.UT + Math.Abs(angleToMakeUp / angleChangepersec));
        //    KACAlarm alarmTarget = new KACAlarm(UTTimeForAlarm);

        //    GUILayout.Label(String.Format("{0:0.00} ({1:0.00})-{2:0}-{3:0}-{4}",
        //        angleTarget,
        //        angleToMakeUp,
        //        UTToTarget,
        //        UTTimeForAlarm,
        //        alarmTarget.Remaining.IntervalString(2)
        //        ));
        //}

        //public void WriteManeuverFile(List<ManeuverNode> m, String FileName)
        //{
        //    if (m.Count > 0)
        //    {
        //        KSP.IO.TextWriter tw = KSP.IO.TextWriter.CreateForType<KerbalAlarmClock>(FileName);
        //        String strInfo="";
        //        foreach (ManeuverNode mNode in m)
        //        {
        //            strInfo += WriteNodeDetails(mNode);
        //        }
        //        strInfo+= KACAlarm.ManNodeSerializeList(FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes) + "\r\n";
        //        //strInfo+= KACAlarm.ManNodeSerializeList(tmpAlarm.ManNodes)+ "\r\n";
        //        tw.WriteLine(strInfo);
        //        tw.Close();
        //    }
        //}

        //private static string WriteNodeDetails( ManeuverNode mNode)
        //{
        //    String strInfo = "";
        //    strInfo += "attachedGizmo:" + mNode.attachedGizmo + "\r\n";
        //    strInfo += "DeltaV:" + mNode.DeltaV + "\r\n";
        //    strInfo += "nextPatch:" + mNode.nextPatch + "\r\n";
        //    strInfo += "nodeRotation:" + mNode.nodeRotation + "\r\n";
        //    strInfo += "patch:" + mNode.patch + "\r\n";
        //    strInfo += "scaledSpaceTarget:" + mNode.scaledSpaceTarget + "\r\n";
        //    strInfo += "solver:" + mNode.solver + "\r\n";
        //    strInfo += "UT:" + mNode.UT + "\r\n";
        //    return strInfo;
        //}

        //public void WriteOrbitFile(Orbit o, String FileName)
        //{

        //    KSP.IO.TextWriter tw = KSP.IO.TextWriter.CreateForType<KerbalAlarmClock>(FileName);
        //    tw.WriteLine("ClAppr:" + o.ClAppr);
        //    tw.WriteLine("ClEctr1:" + o.ClEctr1);
        //    tw.WriteLine("ClEctr2:" + o.ClEctr2);
        //    tw.WriteLine("closestEncounterBody:" + o.closestEncounterBody);
        //    tw.WriteLine("closestEncounterLevel:" + o.closestEncounterLevel);
        //    tw.WriteLine("closestEncounterPatch:" + o.closestEncounterPatch);
        //    tw.WriteLine("closestTgtApprUT:" + o.closestTgtApprUT);
        //    tw.WriteLine("CrAppr:" + o.CrAppr);

        //    tw.WriteLine("EndUT:" + o.EndUT);
        //    tw.WriteLine("epoch:" + o.epoch);
        //    tw.WriteLine("FEVp:" + o.FEVp);
        //    tw.WriteLine("FEVs:" + o.FEVs);
        //    tw.WriteLine("fromE:" + o.fromE);
        //    tw.WriteLine("fromV:" + o.fromV);
        //    tw.WriteLine("h:" + o.h);
        //    tw.WriteLine("inclination:" + o.inclination);
        //    tw.WriteLine("LAN:" + o.LAN);
        //    tw.WriteLine("mag:" + o.mag);
        //    tw.WriteLine("meanAnomaly:" + o.meanAnomaly);
        //    tw.WriteLine("meanAnomalyAtEpoch:" + o.meanAnomalyAtEpoch);
        //    tw.WriteLine("nearestTT:" + o.nearestTT);
        //    tw.WriteLine("nextPatch:" + o.nextPatch);
        //    tw.WriteLine("nextTT:" + o.nextTT);
        //    tw.WriteLine("nextPatch:" + o.nextPatch);
        //    tw.WriteLine("ObT:" + o.ObT);
        //    tw.WriteLine("ObTAtEpoch:" + o.ObTAtEpoch);
        //    tw.WriteLine("orbitalEnergy:" + o.orbitalEnergy);
        //    tw.WriteLine("orbitalSpeed:" + o.orbitalSpeed);
        //    tw.WriteLine("orbitPercent:" + o.orbitPercent);
        //    tw.WriteLine("patchEndTransition:" + o.patchEndTransition);
        //    tw.WriteLine("patchStartTransition:" + o.patchStartTransition);
        //    tw.WriteLine("period:" + o.period);
        //    tw.WriteLine("pos:" + o.pos);
        //    tw.WriteLine("previousPatch:" + o.previousPatch);
        //    tw.WriteLine("radius:" + o.radius);
        //    tw.WriteLine("referenceBody:" + o.referenceBody);
        //    tw.WriteLine("SEVp:" + o.SEVp);
        //    tw.WriteLine("SEVs:" + o.SEVs);
        //    tw.WriteLine("StartUT:" + o.StartUT);
        //    tw.WriteLine("timeToTransition1:" + o.timeToTransition1);
        //    tw.WriteLine("timeToTransition2:" + o.timeToTransition2);
        //    tw.WriteLine("toE:" + o.toE);
        //    tw.WriteLine("toV:" + o.toV);
        //    tw.WriteLine("trueAnomaly:" + o.trueAnomaly);
        //    tw.WriteLine("UTappr:" + o.UTappr);
        //    tw.WriteLine("UTsoi:" + o.UTsoi);
        //    tw.WriteLine("V:" + o.V);
        //    tw.WriteLine("vel:" + o.vel);
        //    tw.Close();
        //}
		
		
    }
}
