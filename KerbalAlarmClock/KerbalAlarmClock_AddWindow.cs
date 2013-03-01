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

            strAlarmName = FlightGlobals.ActiveVessel.vesselName + "-Alarm";
            strAlarmNameNode = FlightGlobals.ActiveVessel.vesselName + "-Maneuver";
            strAlarmMessage = "Time to pay attention to " + FlightGlobals.ActiveVessel.vesselName + "\r\nManual Alarm";
            strAlarmMessageNode = "Time to pay attention to " + FlightGlobals.ActiveVessel.vesselName + "\r\nNearing Maneuvering Node";
            intActionSelected = 1;
            //blnHaltWarp = true;

            strNodeMargin = "1";

            if (KACBehaviour.ManeuverNodeExists)
            {
                intAddType = 1;//AddAlarmType.Node;
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
            //string[] strAddTypes = new string[] { "Raw", "Maneuver Node", "Transfer Window" };
            string[] strAddTypes = new string[] { "Raw", "Maneuver Node" };
            intAddType = GUILayout.Toolbar((int)intAddType, strAddTypes,KACResources.styleButton);

            WindowLayout_AddPane_Common();

            switch (intAddType)
            {
                case 0:
                    WindowLayout_AddPane_Raw();
                    break;
                case 1:
                    WindowLayout_AddPane_Node();
                    break;
                case 2:
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
                case 1:
                    strAlarmNameNode = GUILayout.TextField(strAlarmNameNode, KACResources.styleAddField);
                    strAlarmMessageNode = GUILayout.TextArea(strAlarmMessageNode, KACResources.styleAddField);
                    break;
                case 0:
                default:
                    strAlarmName = GUILayout.TextField(strAlarmName, KACResources.styleAddField);
                    strAlarmMessage = GUILayout.TextArea(strAlarmMessage, KACResources.styleAddField);
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
            strName = GUILayout.TextField(strName, KACResources.styleAddField);
            strMessage = GUILayout.TextArea(strMessage, KACResources.styleAddField);
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
                    rawTime = new KerbalTime(CurrentTime.UT + rawTime.UT);
                }
                rawTimeToAlarm = new KerbalTime(rawTime.UT - CurrentTime.UT);

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
                    Settings.Alarms.Add(new KACAlarm(rawTime.UT, strAlarmName, strAlarmMessage, (intActionSelected>0),(intActionSelected>1)));
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
        private void WindowLayout_AddPane_Node()
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
                if (!KACBehaviour.ManeuverNodeExists)
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
                        KerbalTime nodeInterval = new KerbalTime(nodeTime.UT - CurrentTime.UT);

                        long lngMarginMinutes;
                        KerbalTime nodeAlarm;
                        KerbalTime nodeAlarmInterval;

                        try
                        {
                            lngMarginMinutes = Convert.ToInt64(strNodeMargin);
                            nodeAlarm = new KerbalTime(nodeTime.UT - (lngMarginMinutes * 60));
                            nodeAlarmInterval = new KerbalTime(nodeTime.UT - CurrentTime.UT - (lngMarginMinutes * 60));
                        }
                        catch (Exception)
                        {
                            nodeAlarm = null;
                            nodeAlarmInterval = null;
                            strMarginConversion = "Unable to Add the Margin Minutes";
                        }

                        if ((nodeTime.UT > CurrentTime.UT) && strMarginConversion == "")
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
                                Settings.Alarms.Add(new KACAlarm(nodeAlarm.UT, strAlarmNameNode, strAlarmMessageNode, (intActionSelected>0),(intActionSelected>1)));
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
