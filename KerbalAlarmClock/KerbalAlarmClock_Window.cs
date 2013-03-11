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


        #region "OnGUI Stuff"
        System.Random rnd = new System.Random();
        public void SetupDrawStuff()
        {
            GUI.skin = HighLogic.Skin;
            if (KACResources.styleWindow == null)
            {
                KACResources.SetStyles();
            }
        }

        /// <summary>
        /// Draw the icon on the screen
        /// </summary>
        public void DrawIcons()
        {
            Texture2D iconToShow;
            //Replace this with workerstate object that can test for pause and catch errors - is it doing this in flight mode??
            if (!KACWorkerGameState.PauseMenuOpen)
            {
                if (FlightDriver.Pause)
                {
                    iconToShow = KACResources.GetPauseIcon();
                }
                else if (KACWorkerGameState.CurrentlyUnderWarpInfluence)
                {
                    iconToShow = KACResources.GetWarpIcon();
                }
                else
                {
                    if (Settings.Alarms.ActiveEnabledFutureAlarms(HighLogic.CurrentGame.Title))
                    {
                        if (Settings.WindowVisible)
                            iconToShow = KACResources.iconAlarmShow;
                        else
                            iconToShow = KACResources.iconAlarm;
                    }
                    else
                    {
                        if (Settings.WindowVisible)
                            iconToShow = KACResources.iconNormShow;
                        else
                            iconToShow = KACResources.iconNorm;
                    }
                }

                if (GUI.Button(new Rect(152, 0, 32, 32), iconToShow, KACResources.styleIconStyle))
                {
                    Settings.WindowVisible = !Settings.WindowVisible;
                    Settings.Save();
                }
            }
        }


        //Basic setup of draw stuff
        private static int _WindowMainID = 0;
        int intLeftWindowWidth = 330;
        int intLeftWindowMinHeight = 120;
        int intLeftWindowBaseHeight = 128;
        int intRightWindowWidth = 388;


        //is the add pane visible
        private Boolean _ShowAddPane = false;
        private static int _WindowAddID = 0;
        static Rect _WindowAddRect;

        //Settings Window
        private Boolean _ShowSettings = false;
        private static int _WindowSettingsID = 0;
        private static Rect _WindowSettingsRect;

        //Edit Window
        private Boolean _ShowEditPane = false;
        private static int _WindowEditID = 0;
        private static Rect _WindowEditRect;

        public void DrawWindows()
        {
            Rect MainWindowPos = new Rect(Settings.WindowPos);
            if (Settings.WindowMinimized)
            {
                MainWindowPos.height = intLeftWindowMinHeight;
                if (Settings.AlarmAddSOIAuto)
                    MainWindowPos.height += 33;
            }
            else
            {
                MainWindowPos.height = intLeftWindowBaseHeight;
                if (Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title).Count > 1)
                {
                    if (Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title).Count<2)
                        MainWindowPos.height = intLeftWindowBaseHeight + 26;
                    else if (Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title).Count < Settings.AlarmListMaxAlarmsInt)
                        MainWindowPos.height = intLeftWindowBaseHeight + ((Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title).Count - 1) * 26);
                    else
                        MainWindowPos.height = intLeftWindowBaseHeight + ((Settings.AlarmListMaxAlarmsInt - 1) * 26);
                    if (Settings.AlarmAddSOIAuto)
                        MainWindowPos.height += 33;
                }
            }
            Settings.WindowPos = GUILayout.Window(_WindowMainID, MainWindowPos, FillWindow, "Kerbal Alarm Clock - " + Settings.Version,  GUILayout.MinWidth(intLeftWindowWidth), GUILayout.MaxWidth(intLeftWindowWidth));

            if (_ShowSettings)
            {
                _WindowSettingsRect = GUILayout.Window(_WindowSettingsID, new Rect(Settings.WindowPos.x + Settings.WindowPos.width, Settings.WindowPos.y, 388, 463), FillSettingsWindow, "Settings and Globals",  GUILayout.MinWidth(intRightWindowWidth), GUILayout.MaxWidth(intRightWindowWidth), GUILayout.ExpandWidth(false));
            }
            else if (_ShowAddPane)
            {
                long AddWindowHeight;
                switch (intAddType)
                {
                    case 0: AddWindowHeight = 413; break;
                    case 1: AddWindowHeight = 350; break;
                    case 2: AddWindowHeight = 350; break;
                    default: AddWindowHeight = 413; break;
                }
                _WindowAddRect = GUILayout.Window(_WindowAddID, new Rect(Settings.WindowPos.x + Settings.WindowPos.width, Settings.WindowPos.y, intRightWindowWidth, AddWindowHeight), FillAddWindow, "Add New Alarm",  GUILayout.MinWidth(intRightWindowWidth), GUILayout.MaxWidth(intRightWindowWidth), GUILayout.ExpandWidth(false));
            }
            else if (_ShowEditPane)
            {
                long EditWindowHeight = 260;
                if (alarmEdit.AlarmTime.UT < KACWorkerGameState.CurrentTime.UT) EditWindowHeight = 200;
                _WindowEditRect = GUILayout.Window(_WindowEditID, new Rect(Settings.WindowPos.x + Settings.WindowPos.width, Settings.WindowPos.y, 388, EditWindowHeight), FillEditWindow, "Editing Alarm", GUILayout.MinWidth(intRightWindowWidth), GUILayout.MaxWidth(intRightWindowWidth), GUILayout.ExpandWidth(false));
            }
        }

        //Now the layout

        public void FillWindow(int intWindowID)
        {
            GUILayout.BeginVertical();

            //Heading Part
            GUILayout.BeginHorizontal();
            GUILayout.Label("Alarm List", KACResources.styleHeading, GUILayout.ExpandWidth(true));

            //If the settings pane is visible then dont show the rest
            Texture2D imgMinMax = KACResources.btnMin;
            if (Settings.WindowMinimized) imgMinMax = KACResources.btnMax;
            //string strMinimizedText = "_";
            //if (Settings.WindowMinimized) strMinimizedText = "+";

            if (GUILayout.Button(imgMinMax, KACResources.styleSmallButton))
            {
                Settings.WindowMinimized = !Settings.WindowMinimized;
                Settings.Save();
            }

            //String strSettingsTooltip = "Configure Settings...";
            //if (Settings.VersionAvailable) strSettingsTooltip = "Updated Version Available - Configure Settings...";
            if (DrawToggle(ref _ShowSettings, KACResources.GetSettingsButtonIcon(Settings.VersionAttentionFlag), KACResources.styleSmallButton) && _ShowSettings)
            {
                Settings.VersionAttentionFlag = false;
                _ShowAddPane = false;
                _ShowEditPane = false;
            }
            if (DrawToggle(ref _ShowAddPane, KACResources.btnAdd, KACResources.styleSmallButton) && _ShowAddPane)
            {
                //reset the add stuff
                NewAddAlarm();
                _ShowSettings = false;
                _ShowEditPane = false;
            }

            GUILayout.EndHorizontal();

            //Text Area for content portion
            GUILayout.BeginVertical(KACResources.styleAlarmListArea);
            if (Settings.WindowMinimized)
            {
                WindowLayout_Minimized();
            }
            else
            {
                WindowLayout_AlarmList();
            }

            if (Settings.AlarmAddSOIAuto)
            {
                GUILayout.BeginHorizontal(KACResources.styleSOIIndicator, GUILayout.Height(10));
                GUILayout.Label("",GUILayout.ExpandWidth(true));
                GUILayout.Label(KACResources.iconSOISmall, KACResources.styleSOIIcon);
                float flblWidth = 95f;
                String strSOILabel = "SOI Auto Add";
                if (Settings.AlarmCatchSOIChange)
                {
                    strSOILabel += "-plus catchall";
                    flblWidth += 80f;
                }
                if (Settings.AlarmOnSOIChange_Action > 1) strSOILabel += " (P)";
                else if (Settings.AlarmOnSOIChange_Action > 0) strSOILabel += "(W)";
                GUILayout.Label(strSOILabel, KACResources.styleSOIIndicator, GUILayout.Width(flblWidth));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            //Current Game time at the botttom of the control 
            GUILayout.BeginHorizontal();
            GUILayout.Label("Current Time:", KACResources.styleHeading);
            if (Settings.TimeAsUT)
                GUILayout.Label(KACWorkerGameState.CurrentTime.UTString(), KACResources.styleContent);
            else
                GUILayout.Label(KACWorkerGameState.CurrentTime.DateString(), KACResources.styleContent);
            if ((Event.current.type == EventType.repaint) && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Input.GetMouseButtonDown(0))
            {
                Settings.TimeAsUT = !Settings.TimeAsUT;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        //Display minimal info about the next alarm
        private void WindowLayout_Minimized()
        {
            KACAlarm nextAlarm = null;

            //Find the next Alarm
            if (Settings.Alarms != null)
            {
                foreach (KACAlarm tmpAlarm in Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title))
                {
                    bool blnSwitch = false;
                    if (tmpAlarm.AlarmTime.UT > KACWorkerGameState.CurrentTime.UT && tmpAlarm.Enabled && !tmpAlarm.Actioned)
                    {
                        if (nextAlarm == null)
                        {
                            blnSwitch = true;
                        }
                        else
                        {
                            if (tmpAlarm.AlarmTime.UT < nextAlarm.AlarmTime.UT)
                                blnSwitch = true;
                        }

                    }
                    if (blnSwitch)
                        nextAlarm = tmpAlarm;
                }
            }

            if (nextAlarm == null)
            {
                GUILayout.Label("No Enabled Future Alarms in list");
            }
            else
            {
                if (Event.current.type == EventType.repaint)
                    rectScrollview = new Rect(0, 0, 0, 0);
                if (DrawAlarmLine(nextAlarm))
                    Settings.Alarms.Remove(nextAlarm);
            }
        }

        //Display Full alarm list - Sort by Date/Time????
        public static Rect rectScrollview;
        Vector2 scrollPosition = Vector2.zero;
        private void WindowLayout_AlarmList()
        {
            GUIStyle styleTemp = new GUIStyle();


            scrollPosition = GUILayout.BeginScrollView(scrollPosition, styleTemp);
            if (Settings.Alarms.CountInSave(HighLogic.CurrentGame.Title) == 0)
            {
                GUILayout.Label("No Alarms in the List");
            }
            else
            {
                List<KACAlarm> AlarmsToRemove = new List<KACAlarm>();
                List<KACAlarm> AlarmsToSort = Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title);
                AlarmsToSort.Sort(KACAlarm.SortByUT);
                foreach (KACAlarm tmpAlarm in AlarmsToSort)
                {
                    //Draw a line for each alarm, returns true is person clicked delete
                    if (DrawAlarmLine(tmpAlarm))
                        AlarmsToRemove.Add(tmpAlarm);
                }

                foreach (KACAlarm tmpAlarm in AlarmsToRemove)
                {
                    Settings.Alarms.Remove(tmpAlarm);
                    Settings.Save();
                }

            }
            GUILayout.EndScrollView();
            //Get the visible portion of the Scrollview and record it for hittesting later - needs to just be a box from the 0,0 point for the hit test
            // not sure why as the cursor test point is from the screen 0,0
            if (Event.current.type == EventType.repaint)
                rectScrollview = new Rect(0, scrollPosition.y, GUILayoutUtility.GetLastRect().width, GUILayoutUtility.GetLastRect().height);

        }

        private Boolean DrawAlarmLine(KACAlarm tmpAlarm)
        {
            Boolean blnReturn = false;

            GUILayout.BeginHorizontal();

            switch (tmpAlarm.TypeOfAlarm)
            {
                case KACAlarm.AlarmType.Raw:
                    GUILayout.Label(KACResources.iconNone, KACResources.styleAlarmIcon);
                    break;
                case KACAlarm.AlarmType.Maneuver:
                    GUILayout.Label(KACResources.iconMNode, KACResources.styleAlarmIcon);
                    break;
                case KACAlarm.AlarmType.SOIChange:
                    GUILayout.Label(KACResources.iconSOI, KACResources.styleAlarmIcon);
                    break;
                case KACAlarm.AlarmType.Transfer:
                    break;
                default:
                    break;
            }

            string strLabelText = "";
            if (Settings.TimeAsUT)
                strLabelText = string.Format("{0} ({1})", tmpAlarm.Name, tmpAlarm.Remaining.UTString());
            else
                strLabelText = string.Format("{0} ({1})", tmpAlarm.Name, tmpAlarm.Remaining.IntervalString(3));

            GUIStyle styleLabel = KACResources.styleAlarmText;
            if ((!tmpAlarm.Enabled || tmpAlarm.Actioned))
                styleLabel = KACResources.styleAlarmTextGrayed;
            GUIStyle styleLabelWarpWorking = KACResources.styleLabelWarp;
            if ((!tmpAlarm.Enabled || tmpAlarm.Actioned))
                styleLabelWarpWorking = KACResources.styleLabelWarpGrayed;

            //Draw the label and look for clicks on the label (ensure its in the visible scrollview window as well)
            GUILayout.Label(strLabelText, styleLabel);
            Rect rectLabel = GUILayoutUtility.GetLastRect();
            //is the mouseposition in the visible scrollview?
            //rectScrollview.Contains(

            //Now did we click the label
            if ((Event.current.type == EventType.repaint) && Input.GetMouseButtonDown(0) &&
                ((rectScrollview.Contains(Event.current.mousePosition) && rectLabel.Contains(Event.current.mousePosition))
                || (rectScrollview.height == 0 && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))))
            {
                if (alarmEdit == tmpAlarm)
                {
                    _ShowEditPane = !_ShowEditPane;
                }
                else
                {
                    alarmEdit = tmpAlarm;
                    _ShowEditPane = true;
                    _ShowSettings = false;
                    _ShowAddPane = false;
                }
            }

            if (tmpAlarm.PauseGame)
            {
                GUILayout.Label(KACResources.GetPauseListIcon(tmpAlarm.WarpInfluence), KACResources.styleLabelWarp);
            }
            else if (tmpAlarm.HaltWarp)
            {
                GUILayout.Label(KACResources.GetWarpListIcon(tmpAlarm.WarpInfluence), KACResources.styleLabelWarp);
            }

            if (GUILayout.Button(KACResources.btnRedCross, GUI.skin.button, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                blnReturn = true;
            GUILayout.EndHorizontal();

            return blnReturn;
        }


        public void ResetPanes()
        {
            _ShowAddPane = false;
            _ShowEditPane = false;
            _ShowSettings = false;
        }
        
        #endregion


        #region "Control Drawing"
        /// <summary>
        /// Draws a Toggle Button and sets the boolean variable to the state of the button
        /// </summary>
        /// <param name="blnVar">Boolean variable to set and store result</param>
        /// <param name="ButtonText"></param>
        /// <param name="style"></param>
        /// <param name="options"></param>
        /// <returns>True when the button state has changed</returns>
        public Boolean DrawToggle(ref Boolean blnVar, string ButtonText, GUIStyle style, params GUILayoutOption[] options)
        {
            Boolean blnReturn = GUILayout.Toggle(blnVar, ButtonText, style, options);
            return ToggleResult(ref blnVar, ref  blnReturn);
        }

        public Boolean DrawToggle(ref Boolean blnVar, Texture image, GUIStyle style, params GUILayoutOption[] options)
        {
            Boolean blnReturn = GUILayout.Toggle(blnVar, image, style, options);
            return ToggleResult(ref blnVar, ref blnReturn);
        }

        public Boolean DrawToggle(ref Boolean blnVar, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            Boolean blnReturn = GUILayout.Toggle(blnVar, content, style, options);
            return ToggleResult(ref blnVar, ref blnReturn);
        }

        private Boolean ToggleResult(ref Boolean Old, ref Boolean New)
        {
            if (Old != New)
            {
                Old = New;
                DebugLogFormatted("Toggle Changed:" + New.ToString());
                return true;
            }
            return false;
        }



        public Boolean DrawTextBox(ref String strVar, GUIStyle style, params GUILayoutOption[] options)
        {
            String strReturn = GUILayout.TextField(strVar, style, options);
            if (strReturn != strVar)
            {
                strVar = strReturn;
                DebugLogFormatted("String Changed:" + strVar.ToString());
                return true;
            }
            return false;
        }


        /// <summary>
        /// Draws a toggle button like a checkbox
        /// </summary>
        /// <param name="blnVar"></param>
        /// <returns>True when check state has changed</returns>
        public Boolean DrawCheckbox(ref Boolean blnVar, string strText, params GUILayoutOption[] options)
        {
            return DrawToggle(ref blnVar, strText, KACResources.styleCheckbox, options);
        }

        //CHANGED
        /// <summary>
        /// Draws a toggle button like a checkbox
        /// </summary>
        /// <param name="blnVar"></param>
        /// <returns>True when check state has changed</returns>
        public Boolean DrawCheckbox2(ref Boolean blnVar, string strText, params GUILayoutOption[] options)
        {
            // return DrawToggle(ref blnVar, strText, KACResources.styleCheckbox, options);
            Boolean blnReturn = false;
            Boolean blnToggleInitial = blnVar;

            GUILayout.BeginHorizontal();
            //Draw the radio
            DrawToggle(ref blnVar, "", KACResources.styleCheckbox, options);
            //Spacing
            GUILayout.Space(15);
            //and a label...
            GUILayout.Label(strText, KACResources.styleCheckboxLabel, options);
            //That is clickable
            if ((Event.current.type == EventType.repaint) && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Input.GetMouseButtonDown(0))
            {
                //if its clicked then toggle the boolean
                blnVar = !blnVar;
                KACWorker.DebugLogFormatted("Toggle Changed:" + blnVar);
            }
            GUILayout.EndHorizontal();

            //If output value doesnt = input value
            if (blnToggleInitial != blnVar)
            {
                KACWorker.DebugLogFormatted("Toggle recorded:" + blnVar);
                blnReturn = true;
            }
            return blnReturn;
        }

        public Boolean DrawRadioList(ref int Selected, params String[] Choices)
        {
            int InitialChoice = Selected;

            GUILayout.BeginHorizontal();
            for (int intChoice = 0; intChoice < Choices.Length; intChoice++)
            {
                if (GUILayout.Toggle((intChoice == Selected), "", KACResources.styleCheckbox))
                    Selected = intChoice;
                GUILayout.Label(Choices[intChoice], KACResources.styleCheckboxLabel);
                //That is clickable
                if ((Event.current.type == EventType.repaint) && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Input.GetMouseButtonDown(0))
                {
                    //if its clicked then toggle the boolean
                    Selected = intChoice;
                }
            }
            GUILayout.EndHorizontal();

            if (InitialChoice != Selected)
                DebugLogFormatted(String.Format("Radio List Changed:{0} to {1}", InitialChoice, Selected));


            return !(InitialChoice == Selected);
        }


        #endregion


    }
}
