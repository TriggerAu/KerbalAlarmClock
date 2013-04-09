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

        //int intTestheight = 336;
        //int intTestheight2 = 336;
        //int intTestheight3 = 336;

        public void FillDebugWindow(int WindowID)
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            //GUILayout.Label("A");
            //GUILayout.Label("B");
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

            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            //intTestheight = Convert.ToInt32(GUILayout.TextField(intTestheight.ToString()));
            //intTestheight2 = Convert.ToInt32(GUILayout.TextField(intTestheight2.ToString()));
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


            GUILayout.EndHorizontal();


            GUILayout.EndHorizontal();
        }

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
        //        tw.WriteLine("attachedGizmo:" + m[0].attachedGizmo);
        //        tw.WriteLine("DeltaV:" + m[0].DeltaV);
        //        tw.WriteLine("nextPatch:" + m[0].nextPatch);
        //        tw.WriteLine("nodeRotation:" + m[0].nodeRotation);
        //        tw.WriteLine("patch:" + m[0].patch);
        //        tw.WriteLine("scaledSpaceTarget:" + m[0].scaledSpaceTarget);
        //        tw.WriteLine("solver:" + m[0].solver);
        //        tw.WriteLine("UT:" + m[0].UT);
        //        tw.Close();
        //    }
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
