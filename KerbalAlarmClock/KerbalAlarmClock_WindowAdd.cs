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

        int intAddType = 0;
        private String strAlarmName = "";
        private String strAlarmMessage = "";
        private String strAlarmNameNode = "";
        private String strAlarmMessageNode = "";
        private String strAlarmNameSOI = "";
        private String strAlarmMessageSOI = "";
        private String strAlarmNameXFER = "";
        private String strAlarmMessageXFER = "";
        //private Boolean blnHaltWarp = true;

        private String strAlarmDescSOI = "This will monitor the current active flight path for the next detected SOI change.\r\n\r\nIf the SOI Point changes the alarm will adjust until it is within {0} seconds of the Alarm time, at which point it just maintains the last captured time of the change.";
        private String strAlarmDescXfer = "This will check and recalculate the active transfer alarms for the correct phase angle - the math for these is based around circular orbits so the for any elliptical orbit these need to be recalculated over time.\r\n\r\nThe alarm will adjust until it is within {0} seconds of the target phase angle, at which point it just maintains the last captured time of the angle.\r\nI DO NOT RECOMMEND TURNING THIS OFF UNLESS THERE IS A MASSIVE PERFORMANCE GAIN";

        int intActionSelected = 0;

        KerbalTimeStringArray timeMargin = new KerbalTimeStringArray();

        /// <summary>
        /// Code to reset the settings etc when athe new button is hit
        /// </summary>
        private void NewAddAlarm()
        {
            strYears = "0";
            strDays = "0";
            strHours = "0";
            strMinutes = "10";
            strRawUT = "";

            strAlarmName = FlightGlobals.ActiveVessel.vesselName + "";
            strAlarmNameNode = FlightGlobals.ActiveVessel.vesselName + "";
            strAlarmNameSOI = FlightGlobals.ActiveVessel.vesselName + "";
            strAlarmNameXFER = "Transfer Window";//FlightGlobals.ActiveVessel.vesselName + "";
            strAlarmMessage = "Time to pay attention to " + FlightGlobals.ActiveVessel.vesselName + "\r\nManual Alarm";
            strAlarmMessageNode = "Time to pay attention to " + FlightGlobals.ActiveVessel.vesselName + "\r\nNearing Maneuvering Node";
            strAlarmMessageSOI = "Time to pay attention to " + FlightGlobals.ActiveVessel.vesselName + "\r\nNearing SOI Change";
            strAlarmMessageXFER  = "Time to pay attention to " + FlightGlobals.ActiveVessel.vesselName + "\r\nNearing Transfer Window";
            intActionSelected = Settings.AlarmDefaultAction;
            //blnHaltWarp = true;

            timeMargin.BuildFromUT(Settings.AlarmDefaultMargin);

            if (KACWorkerGameState.ManeuverNodeExists)
            {
                intAddType = 1;//AddAlarmType.Node;
            }
            else if (KACWorkerGameState.SOIPointExists)
            {
                intAddType = 2;//AddAlarmType.Node;
            }
            else
            {
                intAddType = 0;//AddAlarmType.Node;
            }

            SetUpXferParents();
            if (XferParentBodies.Contains(KACWorkerGameState.CurrentVessel.mainBody.referenceBody))
            {
                intXferCurrentParent = XferParentBodies.IndexOf(KACWorkerGameState.CurrentVessel.mainBody.referenceBody);
                SetupXferOrigins();
                intXferCurrentOrigin = XferOriginBodies.IndexOf(KACWorkerGameState.CurrentVessel.mainBody);
            }
            else
            {
                intXferCurrentParent = 0;
                SetupXferOrigins();
                intXferCurrentOrigin = 0;
            }
            strAlarmNameXFER = String.Format("{0} Transfer", XferOriginBodies[intXferCurrentOrigin].bodyName);
            SetupXFerTargets();
        }
        
        /// <summary>
        /// Draw the Add Window contents
        /// </summary>
        /// <param name="WindowID"></param>
        public void FillAddWindow(int WindowID)
        {
            GUILayout.BeginVertical();
            String[] strAddTypes = new String[] { "Raw", "Maneuver","SOI","Transfer" };
            intAddType = GUILayout.Toolbar((int)intAddType, strAddTypes,KACResources.styleButton);

            WindowLayout_AddPane_Common();

            switch (intAddType)
            {
                case 0:
                    WindowLayout_AddPane_Raw();
                    break;
                case 1:
                    WindowLayout_AddPane_Maneuver();
                    break;
                case 2:
                    WindowLayout_AddPane_SOI();
                    break;
                case 3:
                    WindowLayout_AddPane_Transfer();
                    break;
                default:
                    WindowLayout_AddPane_Raw();
                    break;
            }
            GUILayout.EndVertical();
            SetTooltipText();
        }

        int intHeight_AddWindowCommon;
        /// <summary>
        /// Layout of Common Parts of every alarm
        /// </summary>
        private void WindowLayout_AddPane_Common()
        {
            switch (intAddType)
            {
                case 3:
                    intHeight_AddWindowCommon = 116 + strAlarmMessageXFER.Split("\r".ToCharArray()).Length * 16;
                    WindowLayout_CommonFields(ref strAlarmNameXFER, ref strAlarmMessageXFER, ref intActionSelected, ref timeMargin, KACAlarm.AlarmType.Transfer, intHeight_AddWindowCommon);
                    break;
                case 2:
                    intHeight_AddWindowCommon = 116 + strAlarmMessageSOI.Split("\r".ToCharArray()).Length * 16;
                    WindowLayout_CommonFields(ref strAlarmNameSOI, ref strAlarmMessageSOI, ref intActionSelected, ref timeMargin, KACAlarm.AlarmType.SOIChange, intHeight_AddWindowCommon);
                    break;
                case 1:
                    intHeight_AddWindowCommon = 116 + strAlarmMessageNode.Split("\r".ToCharArray()).Length * 16;
                    WindowLayout_CommonFields(ref strAlarmNameNode, ref strAlarmMessageNode, ref intActionSelected, ref timeMargin, KACAlarm.AlarmType.Maneuver, intHeight_AddWindowCommon);
                    break;
                case 0:
                default:
                    intHeight_AddWindowCommon = 88 + strAlarmMessage.Split("\r".ToCharArray()).Length * 16;
                    WindowLayout_CommonFields(ref strAlarmName, ref strAlarmMessage, ref intActionSelected, ref timeMargin, KACAlarm.AlarmType.Raw, intHeight_AddWindowCommon);
                    break;
            }            
        }

        //Variabled for Raw Alarm screen
        String strYears = "0", strDays = "0", strHours = "0", strMinutes = "0",strRawUT="0";
        KerbalTime rawTime = new KerbalTime();
        KerbalTime rawTimeToAlarm = new KerbalTime();
        Boolean blnRawDate = false;
        Boolean blnRawInterval = true;
        /// <summary>
        /// Layout the raw alarm screen inputs
        /// </summary>
        private void WindowLayout_AddPane_Raw()
        {
            GUILayout.Label("Enter Raw Time Values...", KACResources.styleAddSectionHeading);

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Time type:", KACResources.styleAddHeading, GUILayout.Width(80));
            Boolean blnRawDateStart = blnRawDate;
            Boolean blnRawIntervalStart = blnRawInterval;
            DrawCheckbox(ref blnRawDate, "Date");
            DrawCheckbox(ref blnRawInterval, "Time Interval");
            if (blnRawDateStart != blnRawDate)
            {
                blnRawInterval = !blnRawDate;
            }
            else
            {
                if (blnRawIntervalStart != blnRawInterval)
                {
                    blnRawDate = !blnRawInterval;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(70), GUILayout.MaxWidth(70));
            GUILayout.Label("Years:", KACResources.styleAddHeading);
            GUILayout.Label("Days:", KACResources.styleAddHeading);
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(70), GUILayout.MaxWidth(70));
            strYears = GUILayout.TextField(strYears, KACResources.styleAddField);
            strDays = GUILayout.TextField(strDays, KACResources.styleAddField);
            GUILayout.EndVertical();
            GUILayout.Space(30);
            GUILayout.BeginVertical(GUILayout.Width(70), GUILayout.MaxWidth(70));
            GUILayout.Label("Hours:", KACResources.styleAddHeading);
            GUILayout.Label("Minutes:", KACResources.styleAddHeading);
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(70), GUILayout.MaxWidth(70));
            strHours = GUILayout.TextField(strHours, KACResources.styleAddField);
            strMinutes = GUILayout.TextField(strMinutes, KACResources.styleAddField);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("UT (raw seconds):", KACResources.styleAddHeading,GUILayout.Width(100));
            strRawUT = GUILayout.TextField(strRawUT, KACResources.styleAddField);
            GUILayout.EndHorizontal();


            GUILayout.EndVertical();
            try
            {
                if (strRawUT != "")
                {
                    rawTime.UT = Convert.ToDouble(strRawUT);
                }
                else
                {
                    rawTime.BuildUT(Convert.ToDouble(strYears),
                                    Convert.ToDouble(strDays),
                                    Convert.ToDouble(strHours),
                                    Convert.ToDouble(strMinutes),
                                    0);
                }
                //If its an interval add the interval to the current time
                if (blnRawInterval)
                {
                    rawTime = new KerbalTime(KACWorkerGameState.CurrentTime.UT + rawTime.UT);
                }
                rawTimeToAlarm = new KerbalTime(rawTime.UT - KACWorkerGameState.CurrentTime.UT);

                //turn off padding here
                GUILayout.BeginHorizontal(KACResources.styleAddAlarmArea);
                GUILayout.BeginVertical(GUILayout.Width(100), GUILayout.MaxWidth(100));
                GUILayout.Label("Alarm Date:", KACResources.styleAddHeading);
                GUILayout.Label("Time to Alarm:", KACResources.styleAddHeading);
                GUILayout.EndVertical();
                GUILayout.BeginVertical(GUILayout.Width(150), GUILayout.MaxWidth(150));

                GUILayout.Label(KerbalTime.PrintInterval(rawTime, Settings.TimeAsUT), KACResources.styleContent);
                GUILayout.Label(KerbalTime.PrintInterval(rawTimeToAlarm, Settings.TimeAsUT), KACResources.styleContent);

                GUILayout.EndVertical();
                GUILayout.Space(15);
                if (GUILayout.Button("Add Alarm",KACResources.styleButton, GUILayout.Width(90), GUILayout.Height(40)))
                {
                    //"VesselID, Name, Message, AlarmTime.UT, Type, Enabled,  HaltWarp, PauseGame, Manuever"
                    Settings.Alarms.Add(new KACAlarm(FlightGlobals.ActiveVessel.id.ToString(), strAlarmName, strAlarmMessage, rawTime.UT,0,KACAlarm.AlarmType.Raw, 
                        (intActionSelected > 0), (intActionSelected > 1)));
                    Settings.Save();
                    _ShowAddPane = false;
                }
                GUILayout.EndHorizontal();


            }
            catch (Exception)
            {
                GUILayout.Label("Unable to combine all text fields to date", GUILayout.ExpandWidth(true));
            }
        }

        //Variables for Node Alarms screen
        //String strNodeMargin = "1";
        /// <summary>
        /// Screen Layout for adding Alarm from Maneuver Node
        /// </summary>
        private void WindowLayout_AddPane_Maneuver()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Node Details...", KACResources.styleAddSectionHeading);

            Vessel myVessel = FlightGlobals.ActiveVessel;
            if (myVessel == null)
            {
                GUILayout.Label("No Active Vessel");
            }
            else
            {
                if (!KACWorkerGameState.ManeuverNodeExists)
                {
                    GUILayout.Label("No Maneuver Nodes Found", GUILayout.ExpandWidth(true));
                }
                else
                {
                    Boolean blnFoundNode = false;
                    String strMarginConversion = "";
                    for (int intNode = 0; (intNode < myVessel.patchedConicSolver.maneuverNodes.Count) && !blnFoundNode; intNode++)
                    {
                        KerbalTime nodeTime = new KerbalTime(myVessel.patchedConicSolver.maneuverNodes[intNode].UT);
                        KerbalTime nodeInterval = new KerbalTime(nodeTime.UT - KACWorkerGameState.CurrentTime.UT);

                        KerbalTime nodeAlarm;
                        KerbalTime nodeAlarmInterval;
                        try
                        {
                            nodeAlarm = new KerbalTime(nodeTime.UT - timeMargin.UT);
                            nodeAlarmInterval = new KerbalTime(nodeTime.UT - KACWorkerGameState.CurrentTime.UT - timeMargin.UT);
                        }
                        catch (Exception)
                        {
                            nodeAlarm = null;
                            nodeAlarmInterval = null;
                            strMarginConversion = "Unable to Add the Margin Minutes";
                        }

                        if ((nodeTime.UT > KACWorkerGameState.CurrentTime.UT) && strMarginConversion == "")
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.BeginVertical(GUILayout.Width(100), GUILayout.MaxWidth(100));
                            GUILayout.Label("Node Date:", KACResources.styleAddHeading);
                            GUILayout.Label("Time to Node:", KACResources.styleAddHeading);
                            GUILayout.Label("Time to Alarm:", KACResources.styleAddHeading);
                            GUILayout.EndVertical();
                            GUILayout.BeginVertical(GUILayout.Width(150), GUILayout.MaxWidth(150));

                            GUILayout.Label(KerbalTime.PrintDate(nodeTime, Settings.TimeAsUT), KACResources.styleContent);
                            GUILayout.Label(KerbalTime.PrintInterval(nodeInterval, Settings.TimeAsUT), KACResources.styleContent);
                            GUILayout.Label(KerbalTime.PrintInterval(nodeAlarmInterval, Settings.TimeAsUT), KACResources.styleContent);

                            GUILayout.EndVertical();
                            GUILayout.Space(15);

                            if (GUILayout.Button("Add Alarm",KACResources.styleButton, GUILayout.Width(90), GUILayout.Height(70)))
                            {
                                Settings.Alarms.Add(new KACAlarm(FlightGlobals.ActiveVessel.id.ToString(), strAlarmNameNode, strAlarmMessageNode, nodeAlarm.UT, timeMargin.UT, KACAlarm.AlarmType.Maneuver,
                                    (intActionSelected > 0), (intActionSelected > 1), myVessel.patchedConicSolver.maneuverNodes[intNode]));
                                Settings.Save();
                                _ShowAddPane = false;
                            }
                            GUILayout.EndHorizontal();

                            blnFoundNode = true;
                        }
                    }
                    if (strMarginConversion != "")
                    {
                        GUILayout.Label(strMarginConversion, GUILayout.ExpandWidth(true));
                    }
                    else if (!blnFoundNode)
                    {
                        GUILayout.Label("No Future Maneuver Nodes Found", GUILayout.ExpandWidth(true));
                    }
                }
            }

            GUILayout.EndVertical();
        }


        private void WindowLayout_AddPane_SOI()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("SOI Details...", KACResources.styleAddSectionHeading);

            Vessel myVessel = FlightGlobals.ActiveVessel;
            if (myVessel == null)
            {
                GUILayout.Label("No Active Vessel");
            }
            else
            {
                if (!KACWorkerGameState.SOIPointExists)
                {
                    GUILayout.Label("No SOI Point Found", GUILayout.ExpandWidth(true));
                }
                else
                {
                    String strMarginConversion = "";
                    KerbalTime soiTime = new KerbalTime(KACWorkerGameState.CurrentVessel.orbit.UTsoi);
                    KerbalTime soiInterval = new KerbalTime(soiTime.UT - KACWorkerGameState.CurrentTime.UT);

                    KerbalTime soiAlarm;
                    KerbalTime soiAlarmInterval;
                    try
                    {
                        soiAlarm = new KerbalTime(soiTime.UT - timeMargin.UT);
                        soiAlarmInterval = new KerbalTime(soiTime.UT - KACWorkerGameState.CurrentTime.UT - timeMargin.UT);
                    }
                    catch (Exception)
                    {
                        soiAlarm = null;
                        soiAlarmInterval = null;
                        strMarginConversion = "Unable to Add the Margin Minutes";
                    }

                    if ((soiTime.UT > KACWorkerGameState.CurrentTime.UT) && strMarginConversion == "")
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical(GUILayout.Width(100), GUILayout.MaxWidth(100));
                        GUILayout.Label("Node Date:", KACResources.styleAddHeading);
                        GUILayout.Label("Time to Node:", KACResources.styleAddHeading);
                        GUILayout.Label("Time to Alarm:", KACResources.styleAddHeading);
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical(GUILayout.Width(150), GUILayout.MaxWidth(150));

                        GUILayout.Label(KerbalTime.PrintDate(soiTime, Settings.TimeAsUT), KACResources.styleContent);
                        GUILayout.Label(KerbalTime.PrintInterval(soiInterval, Settings.TimeAsUT), KACResources.styleContent);
                        GUILayout.Label(KerbalTime.PrintInterval(soiAlarmInterval, Settings.TimeAsUT), KACResources.styleContent);

                        GUILayout.EndVertical();
                        GUILayout.Space(15);

                        if (GUILayout.Button("Add Alarm", KACResources.styleButton, GUILayout.Width(90), GUILayout.Height(70)))
                        {
                            Settings.Alarms.Add(new KACAlarm(FlightGlobals.ActiveVessel.id.ToString(), strAlarmNameSOI, strAlarmMessageSOI, soiAlarm.UT, timeMargin.UT, KACAlarm.AlarmType.SOIChange,
                                (intActionSelected > 0), (intActionSelected > 1)));
                            Settings.Save();
                            _ShowAddPane = false;
                        }
                        GUILayout.EndHorizontal();

                    }

                    if (strMarginConversion != "")
                    {
                        GUILayout.Label(strMarginConversion, GUILayout.ExpandWidth(true));
                    }
                }
            }

            GUILayout.EndVertical();
        }

        //private Dictionary<int, CelestialBody> ParentBodies;
        private List<CelestialBody> XferParentBodies = new List<CelestialBody>();
        private List<CelestialBody> XferOriginBodies = new List<CelestialBody>();
        private List<KACXFerTarget> XferTargetBodies = new List<KACXFerTarget>();

        private static int SortByDistance(CelestialBody c1, CelestialBody c2)
        {
            Double f1 = c1.orbit.semiMajorAxis;
            double f2 = c2.orbit.semiMajorAxis;
            DebugLogFormatted("{0}-{1}", f1.ToString(), f2.ToString());
            return f1.CompareTo(f2);
        }
        
        
        private int intXferCurrentParent = 0;
        private int intXferCurrentOrigin = 0;

        private void SetUpXferParents()
        {
            XferParentBodies = new List<CelestialBody>();
            //Build a list of parents - Cant sort this normally as the Sun has no radius - duh!
            foreach (CelestialBody tmpBody in FlightGlobals.Bodies)
            {
                //add any body that has more than 1 child to the parents list
                if (tmpBody.orbitingBodies.Count > 1)
                    XferParentBodies.Add(tmpBody);
            }
        }

        private void SetupXferOrigins()
        {
            //set the possible origins to be all the orbiting bodies around the parent
            XferOriginBodies = new List<CelestialBody>();
            XferOriginBodies = XferParentBodies[intXferCurrentParent].orbitingBodies.OrderBy(b => b.orbit.semiMajorAxis).ToList<CelestialBody>();
        }

        private void SetupXFerTargets()
        {
            XferTargetBodies = new List<KACXFerTarget>();
            
            //Loop through the Siblings of the origin planet
            foreach (CelestialBody bdyTarget in XferOriginBodies.OrderBy(b=>b.orbit.semiMajorAxis))
            {
                //add all the other siblings as target possibilities
                if (bdyTarget != XferOriginBodies[intXferCurrentOrigin])
                {
                    KACXFerTarget tmpTarget = new KACXFerTarget();
                    tmpTarget.Origin = XferOriginBodies[intXferCurrentOrigin];
                    tmpTarget.Target = bdyTarget;
                    //tmpTarget.SetPhaseAngleTarget();
                    //add it to the list
                    XferTargetBodies.Add(tmpTarget);
                }
            }
        }

        int intAddXferHeight=0;
        int intXferType = 0;
        private void WindowLayout_AddPane_Transfer()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Planetary Transfers", KACResources.styleHeading);

            if (!timeMargin.Valid)
            {
                GUILayout.Label("There is an issue in one of the margin fields - see red text",KACResources.styleLabelWarning, GUILayout.ExpandWidth(true));
            }

            try
            {
                //add something here to select the modelled or formula values
                if (Settings.XferModelDataLoaded)
                    DrawRadioList(ref intXferType, "Formula Based", "Modelled Data");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Transfer Parent:", KACResources.styleAddHeading,GUILayout.Width(80));
                GUILayout.Label(XferParentBodies[intXferCurrentParent].bodyName, KACResources.styleAddXferName, GUILayout.ExpandWidth(true));
                if (GUILayout.Button(new GUIContent("Change", "Click to cycle through Parent Bodies"), KACResources.styleAddXferOriginButton))
                {
                    intXferCurrentParent += 1;
                    if (intXferCurrentParent >= XferParentBodies.Count) intXferCurrentParent = 0;
                    SetupXferOrigins();
                    intXferCurrentOrigin = 0;
                    SetupXFerTargets();
                    strAlarmNameXFER = String.Format("{0} Transfer", XferOriginBodies[intXferCurrentOrigin].bodyName);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Transfer Origin:", KACResources.styleAddHeading, GUILayout.Width(80));
                GUILayout.Label(XferOriginBodies[intXferCurrentOrigin].bodyName, KACResources.styleAddXferName, GUILayout.ExpandWidth(true));
                if (GUILayout.Button(new GUIContent("Change", "Click to cycle through Origin Bodies"), KACResources.styleAddXferOriginButton))
                {
                    intXferCurrentOrigin += 1;
                    if (intXferCurrentOrigin >= XferOriginBodies.Count) intXferCurrentOrigin = 0;
                    SetupXFerTargets();
                    strAlarmNameXFER = String.Format("{0} Transfer", XferOriginBodies[intXferCurrentOrigin].bodyName);
                }
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("Target", KACResources.styleAddSectionHeading, GUILayout.Width(80));
                GUILayout.Label(new GUIContent("Phase Angle","Displayed as \"Current Angle (Target Angle)\""), KACResources.styleAddSectionHeading, GUILayout.Width(120));
                GUILayout.Label("Time to Transfer", KACResources.styleAddSectionHeading, GUILayout.ExpandWidth(true));
                //GUILayout.Label("Time to Alarm", KACResources.styleAddSectionHeading, GUILayout.ExpandWidth(true));
                GUILayout.Label("Add", KACResources.styleAddSectionHeading, GUILayout.Width(30));
                GUILayout.EndHorizontal();

                if (intXferType == 1)
                {
                    foreach (KACXFerTarget bdyTarget in XferTargetBodies)
                    {
                        GUILayout.BeginHorizontal();
                        try
                        {

                        GUILayout.Label(bdyTarget.Target.bodyName, KACResources.styleAddXferName, GUILayout.Width(80));

                        KACXFerModelPoint tmpModelPoint = KACResources.lstXferModelPoints.FirstOrDefault(
                            m => FlightGlobals.Bodies[m.Origin] == bdyTarget.Origin &&
                                FlightGlobals.Bodies[m.Target] == bdyTarget.Target && 
                                m.UT>=KACWorkerGameState.CurrentTime.UT);

                        if (tmpModelPoint == null)
                        {
                            GUILayout.Label("No Model Data Found");
                        }
                        else
                        {
                            String strPhase = String.Format("{0:0.00}({1:0.00})", bdyTarget.PhaseAngleCurrent, tmpModelPoint.PhaseAngle);
                            GUILayout.Label(strPhase, KACResources.styleAddHeading, GUILayout.Width(120));
                        }
                        KerbalTime tmpTime = new KerbalTime(tmpModelPoint.UT-KACWorkerGameState.CurrentTime.UT);
                        GUILayout.Label(KerbalTime.PrintInterval(tmpTime, Settings.TimeAsUT), KACResources.styleAddHeading, GUILayout.ExpandWidth(true));
                        
                        if (GUILayout.Button("Add", KACResources.styleAddXferButton))
                        {
                            Settings.Alarms.Add(new KACAlarm("", strAlarmNameXFER, strAlarmMessageXFER + "\r\n\tOrigin: " + bdyTarget.Origin.bodyName + "\r\n\tTarget: " + bdyTarget.Target.bodyName + "\r\n\tMargin: " + new KerbalTime(timeMargin.UT).IntervalString(),
                                (KACWorkerGameState.CurrentTime.UT + tmpTime.UT - timeMargin.UT), timeMargin.UT, KACAlarm.AlarmType.TransferModelled,
                                (intActionSelected > 0), (intActionSelected > 1), bdyTarget));
                            Settings.Save();
                            _ShowAddPane = false;
                        }
                        }
                        catch (Exception)
                        {

                            //throw;
                        }

                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    //  This is the formula version, otherwise we go get the UT/phaseangle from the model data
                    foreach (KACXFerTarget bdyTarget in XferTargetBodies)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(bdyTarget.Target.bodyName, KACResources.styleAddXferName, GUILayout.Width(80));
                        String strPhase = String.Format("{0:0.00}({1:0.00})", bdyTarget.PhaseAngleCurrent, bdyTarget.PhaseAngleTarget);
                        GUILayout.Label(strPhase, KACResources.styleAddHeading, GUILayout.Width(120));

                        GUILayout.Label(KerbalTime.PrintInterval(bdyTarget.AlignmentTime, Settings.TimeAsUT), KACResources.styleAddHeading, GUILayout.ExpandWidth(true));
                        //GUILayout.Label(KerbalTime.PrintInterval(new KerbalTime(bdyTarget.AlignmentTime.UT - timeMargin.UT), Settings.TimeAsUT), KACResources.styleAddHeading, GUILayout.ExpandWidth(true));
                        
                        if (GUILayout.Button("Add", KACResources.styleAddXferButton))
                        {
                            Settings.Alarms.Add(new KACAlarm("", strAlarmNameXFER, strAlarmMessageXFER + "\r\n\tOrigin: " + bdyTarget.Origin.bodyName + "\r\n\tTarget: " + bdyTarget.Target.bodyName + "\r\n\tMargin: " + new KerbalTime(timeMargin.UT).IntervalString(),
                                (KACWorkerGameState.CurrentTime.UT + bdyTarget.AlignmentTime.UT - timeMargin.UT), timeMargin.UT, KACAlarm.AlarmType.Transfer,
                                (intActionSelected > 0), (intActionSelected > 1), bdyTarget));
                            Settings.Save();
                            _ShowAddPane = false;
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                intAddXferHeight = XferTargetBodies.Count * 26;
            }
            catch (Exception ex)
            {
                GUILayout.Label("Something weird has happened");
                DebugLogFormatted(ex.Message);
                DebugLogFormatted(ex.StackTrace);
            }

            GUILayout.EndVertical();
        }
    }
}
