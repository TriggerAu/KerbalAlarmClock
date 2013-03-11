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

        int intAddType = 0;
        private string strAlarmName = "";
        private string strAlarmMessage = "";
        private string strAlarmNameNode = "";
        private string strAlarmMessageNode = "";
        private string strAlarmNameSOI = "";
        private string strAlarmMessageSOI = "";
        //private string strAlarmNameXFER = "";
        //private string strAlarmMessageXFER = "";
        //private Boolean blnHaltWarp = true;
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
            strAlarmMessage = "Time to pay attention to " + FlightGlobals.ActiveVessel.vesselName + "\r\nManual Alarm";
            strAlarmMessageNode = "Time to pay attention to " + FlightGlobals.ActiveVessel.vesselName + "\r\nNearing Maneuvering Node";
            strAlarmMessageSOI = "Time to pay attention to " + FlightGlobals.ActiveVessel.vesselName + "\r\nNearing SOI Change";
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
            //string[] strAddTypes = new string[] { "Raw", "Maneuver Node", "SOI", "Transfer Window" };
            string[] strAddTypes = new string[] { "Raw", "Maneuver","SOI" };
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
        string strYears = "0", strDays = "0", strHours = "0", strMinutes = "0",strRawUT="0";
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
        string strNodeMargin = "1";
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
                    string strMarginConversion = "";
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
                    string strMarginConversion = "";
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


        private void WindowLayout_AddPane_Transfer()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("XFER", KACResources.styleHeading);

            GUIStyle testtext = new GUIStyle(GUI.skin.label);
            testtext.fixedHeight = 20;

            for (int intTemp = 6; intTemp < 20; intTemp++)
            {
                testtext.fontSize = intTemp;
                GUILayout.Label(intTemp.ToString(), testtext);
            }



            GUILayout.EndVertical();
        }
    }
}
