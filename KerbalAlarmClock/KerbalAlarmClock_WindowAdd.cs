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

        private String strAlarmDescSOI = "This will monitor the current active flight plan for the next detected SOI change.\r\n\r\nIf the SOI Point changes the alarm will adjust until it is within {0} seconds of the SOI point, at which point it just maintains the last captured time of the change.";
        private String strAlarmDescXfer = "This will check and recalculate the active transfer alarms for the correct phase angle - the math for these is based around circular orbits so the for any elliptical orbit these need to be recalculated over time.\r\n\r\nThe alarm will adjust until it is within {0} seconds of the target phase angle, at which point it just maintains the last captured time of the angle.\r\nI DO NOT RECOMMEND TURNING THIS OFF UNLESS TTHERE IS A MASSIVE PERFORMANCE GAIN";

        int intActionSelected = 0;

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
            intActionSelected = 1;
            //blnHaltWarp = true;

            strNodeMargin = "1";

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
            //GUILayout.BeginHorizontal();

            //GUILayout.BeginVertical(GUILayout.Width(5));
            //GUILayout.EndVertical();

            GUILayout.BeginVertical();
            //String[] strAddTypes = new String[] { "Raw", "Maneuver Node", "SOI", "Transfer Window" };
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


            //GUILayout.EndHorizontal();
            SetTooltipText();
        }

        /// <summary>
        /// Layout of Common Parts of every alarm
        /// </summary>
        private void WindowLayout_AddPane_Common()
        {
            //Two Columns
            GUILayout.Label("Common Alarm Properties", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(90));
            GUILayout.Label("Alarm Name:", KACResources.styleAddHeading);
            GUILayout.Label("Message:", KACResources.styleAddHeading);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(260), GUILayout.MaxWidth(260));
            switch (intAddType)
            {
                case 3:
                    strAlarmNameXFER = GUILayout.TextField(strAlarmNameXFER, KACResources.styleAddField).Replace("|", "");
                    strAlarmMessageXFER = GUILayout.TextArea(strAlarmMessageXFER, KACResources.styleAddField).Replace("|", "");
                    break;
                case 2:
                    strAlarmNameSOI = GUILayout.TextField(strAlarmNameSOI, KACResources.styleAddField).Replace("|", "");
                    strAlarmMessageSOI = GUILayout.TextArea(strAlarmMessageSOI, KACResources.styleAddField).Replace("|", "");
                    break;
                case 1:
                    strAlarmNameNode = GUILayout.TextField(strAlarmNameNode, KACResources.styleAddField).Replace("|", "");
                    strAlarmMessageNode = GUILayout.TextArea(strAlarmMessageNode, KACResources.styleAddField).Replace("|", "");
                    break;
                case 0:
                default:
                    strAlarmName = GUILayout.TextField(strAlarmName, KACResources.styleAddField).Replace("|", "");
                    strAlarmMessage = GUILayout.TextArea(strAlarmMessage, KACResources.styleAddField).Replace("|", "");
                    break;
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            //Full width one under the two columns for the kill time warp
            GUILayout.BeginHorizontal();
            GUILayout.Label("On Alarm:", KACResources.styleAddHeading, GUILayout.Width(90));
            DrawRadioList(ref intActionSelected, "Message Only", "Kill Time Warp", "Pause Game");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

        }

        /// <summary>
        /// Layout of Common Parts of every alarm
        /// </summary>
        private void WindowLayout_AddPane_Common2(ref String strName,ref String strMessage,ref int Action)
        {
            //Two Columns
            GUILayout.Label("Common Alarm Properties", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(90));
            GUILayout.Label("Alarm Name:", KACResources.styleAddHeading);
            GUILayout.Label("Message:", KACResources.styleAddHeading);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(260), GUILayout.MaxWidth(260));
            strName = GUILayout.TextField(strName, KACResources.styleAddField).Replace("|", "");
            strMessage = GUILayout.TextArea(strMessage, KACResources.styleAddField).Replace("|", "");
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            //Full width one under the two columns for the kill time warp
            GUILayout.BeginHorizontal();
            GUILayout.Label("On Alarm:", KACResources.styleAddHeading, GUILayout.Width(90));
            DrawRadioList(ref Action, "Message Only", "Kill Time Warp", "Pause Game");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

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
            DrawCheckbox2(ref blnRawDate, "Date");
            DrawCheckbox2(ref blnRawInterval, "Time Interval");
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

                if (Settings.TimeAsUT)
                {
                    GUILayout.Label(rawTime.UTString(), KACResources.styleContent);
                    GUILayout.Label(rawTimeToAlarm.UTString(), KACResources.styleContent);
                }
                else
                {
                    GUILayout.Label(rawTime.DateString(), KACResources.styleContent);
                    GUILayout.Label(rawTimeToAlarm.IntervalString(), KACResources.styleContent);
                }
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
        String strNodeMargin = "1";
        /// <summary>
        /// Screen Layout for adding Alarm from Maneuver Node
        /// </summary>
        private void WindowLayout_AddPane_Maneuver()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Node Details...", KACResources.styleAddSectionHeading);

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(140));
            GUILayout.Label("Margin Minutes:", KACResources.styleAddHeading);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(210), GUILayout.MaxWidth(210));
            strNodeMargin = GUILayout.TextField(strNodeMargin, KACResources.styleAddField);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();


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

                        long lngMarginMinutes=0;
                        KerbalTime nodeAlarm;
                        KerbalTime nodeAlarmInterval;

                        try
                        {
                            lngMarginMinutes = Convert.ToInt64(strNodeMargin);
                            nodeAlarm = new KerbalTime(nodeTime.UT - (lngMarginMinutes * 60));
                            nodeAlarmInterval = new KerbalTime(nodeTime.UT - KACWorkerGameState.CurrentTime.UT - (lngMarginMinutes * 60));
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

                            if (Settings.TimeAsUT)
                            {
                                GUILayout.Label(nodeTime.UTString(), KACResources.styleContent);
                                GUILayout.Label(nodeInterval.UTString(), KACResources.styleContent);
                                GUILayout.Label(nodeAlarmInterval.UTString(), KACResources.styleContent);
                            }
                            else
                            {
                                GUILayout.Label(nodeTime.DateString(), KACResources.styleContent);
                                GUILayout.Label(nodeInterval.IntervalString(), KACResources.styleContent);
                                GUILayout.Label(nodeAlarmInterval.IntervalString(), KACResources.styleContent);
                            }
                            GUILayout.EndVertical();
                            GUILayout.Space(15);

                            if (GUILayout.Button("Add Alarm",KACResources.styleButton, GUILayout.Width(90), GUILayout.Height(70)))
                            {
                                Settings.Alarms.Add(new KACAlarm(FlightGlobals.ActiveVessel.id.ToString(), strAlarmNameNode, strAlarmMessageNode, nodeAlarm.UT, (lngMarginMinutes * 60), KACAlarm.AlarmType.Maneuver,
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

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(140));
            GUILayout.Label("Margin Minutes:", KACResources.styleAddHeading);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(210), GUILayout.MaxWidth(210));
            strNodeMargin = GUILayout.TextField(strNodeMargin, KACResources.styleAddField);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();


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

                    long lngMarginMinutes=0;
                    KerbalTime soiAlarm;
                    KerbalTime soiAlarmInterval;

                    try
                    {
                        lngMarginMinutes = Convert.ToInt64(strNodeMargin);
                        soiAlarm = new KerbalTime(soiTime.UT - (lngMarginMinutes * 60));
                        soiAlarmInterval = new KerbalTime(soiTime.UT - KACWorkerGameState.CurrentTime.UT - (lngMarginMinutes * 60));
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

                        if (Settings.TimeAsUT)
                        {
                            GUILayout.Label(soiTime.UTString(), KACResources.styleContent);
                            GUILayout.Label(soiInterval.UTString(), KACResources.styleContent);
                            GUILayout.Label(soiAlarmInterval.UTString(), KACResources.styleContent);
                        }
                        else
                        {
                            GUILayout.Label(soiTime.DateString(), KACResources.styleContent);
                            GUILayout.Label(soiInterval.IntervalString(), KACResources.styleContent);
                            GUILayout.Label(soiAlarmInterval.IntervalString(), KACResources.styleContent);
                        }
                        GUILayout.EndVertical();
                        GUILayout.Space(15);

                        if (GUILayout.Button("Add Alarm", KACResources.styleButton, GUILayout.Width(90), GUILayout.Height(70)))
                        {
                            Settings.Alarms.Add(new KACAlarm(FlightGlobals.ActiveVessel.id.ToString(), strAlarmNameSOI, strAlarmMessageSOI, soiAlarm.UT, lngMarginMinutes*60, KACAlarm.AlarmType.SOIChange,
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
        private void WindowLayout_AddPane_Transfer()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Planetary Transfers", KACResources.styleHeading);
            
            GUILayout.BeginHorizontal(KACResources.styleAddFieldAreas);
            GUILayout.Label("Margin Minutes:", KACResources.styleAddHeading,GUILayout.Width(140));
            strNodeMargin = GUILayout.TextField(strNodeMargin, KACResources.styleAddField,GUILayout.Width(210), GUILayout.MaxWidth(210));
            GUILayout.EndHorizontal();
            long lngMarginMinutes = 1;
            String strMarginConversion = "";
            try
            {
                lngMarginMinutes = Convert.ToInt64(strNodeMargin);
            }
            catch (Exception)
            {
                lngMarginMinutes = 0;
                strMarginConversion = "Unable to Add the Margin Minutes - 0 will be used";
            }
            if (strMarginConversion != "")
            {
                GUILayout.Label(strMarginConversion, GUILayout.ExpandWidth(true));
            }


            GUILayout.BeginHorizontal();
            GUILayout.Label("Xfer Parent:", KACResources.styleAddHeading,GUILayout.Width(80));
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
            GUILayout.Label("Xfer Origin:", KACResources.styleAddHeading, GUILayout.Width(80));
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
            GUILayout.Label("Time to Xfer", KACResources.styleAddSectionHeading,GUILayout.ExpandWidth(true));
            GUILayout.Label("Add", KACResources.styleAddSectionHeading,GUILayout.Width(30));
            GUILayout.EndHorizontal();

            foreach (KACXFerTarget bdyTarget in XferTargetBodies)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(bdyTarget.Target.bodyName, KACResources.styleAddXferName,GUILayout.Width(80));
                String strPhase = String.Format("{0:0.00}({1:0.00})", bdyTarget.PhaseAngleCurrent, bdyTarget.PhaseAngleTarget);
                GUILayout.Label(strPhase, KACResources.styleAddHeading, GUILayout.Width(120));
                if (Settings.TimeAsUT)
                    GUILayout.Label(bdyTarget.AlignmentTime.UTString(), KACResources.styleAddHeading, GUILayout.ExpandWidth(true));
                else
                    GUILayout.Label(bdyTarget.AlignmentTime.IntervalString(3), KACResources.styleAddHeading, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Add", KACResources.styleAddXferButton))
                {
                    Settings.Alarms.Add(new KACAlarm("", strAlarmNameXFER, strAlarmMessageXFER + "\r\n\tOrigin: " + bdyTarget.Origin.bodyName +  "\r\n\tTarget: " + bdyTarget.Target.bodyName + "\r\n\tMargin Minutes: " + lngMarginMinutes , 
                        (KACWorkerGameState.CurrentTime.UT + bdyTarget.AlignmentTime.UT - (lngMarginMinutes*60)), lngMarginMinutes*60, KACAlarm.AlarmType.Transfer,
                        (intActionSelected > 0), (intActionSelected > 1),bdyTarget));
                    Settings.Save();
                    _ShowAddPane = false;
                }
                GUILayout.EndHorizontal();
            }

            if (XferTargetBodies.Count > 3)
                intAddXferHeight = (XferTargetBodies.Count - 3) * 27;
            else
                intAddXferHeight = 0;

            
            GUILayout.EndVertical();
        }
    }
}
