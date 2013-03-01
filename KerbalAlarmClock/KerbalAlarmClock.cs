using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using UnityEngine;
using KSP;

namespace KerbalAlarmClock
{

    /// <summary>
    /// Basic Part piece that creates the Classes that run in the background at all times
    /// </summary>
    partial class KerbalAlarmClock : PartModule
    {
        public static KACSettings Settings = new KACSettings();

        public override void OnAwake()
        {
            if (KACBehaviour.GameObjectInstance == null)
                KACBehaviour.GameObjectInstance = GameObject.Find("KACBehaviour") ?? new GameObject("KACBehaviour", typeof(KACBehaviour));

        }

        public override void OnLoad(ConfigNode node)
        {
            Settings.Load();
        }
        public override void OnSave(ConfigNode node)
        {
            Settings.Save();

        }

    }

    /// <summary>
    /// This is the behaviour object that we hook events on to 
    /// </summary>
    public class KACBehaviour : MonoBehaviour
    {
        //Game object that keeps us running
        public static GameObject GameObjectInstance;
        
        //Worker and Settings objects
        private KACWorker WorkerObjectInstance;
        private KACSettings Settings;
        public static float UpdateInterval = 0.25F;

        //Constructor to set KACWorker parent object required
        public KACBehaviour()
        {
            WorkerObjectInstance = new KACWorker(this);
            Settings = KerbalAlarmClock.Settings;
        }

        //Awake Event - when the DLL is loaded
        public void Awake()
        {
            KACWorker.DebugLogFormatted("Awakening the KerbalAlarmClock");
            //Keep the Behaviour active even on scene Loads
            DontDestroyOnLoad(this);

            //Load Image resources
            KACResources.loadGUIAssets();
            
            //Set up the updating function - do this 4 times a sec not on every frame.
            KACWorker.DebugLogFormatted("Invoking Worker Function KerbalAlarmClock");
            CancelInvoke();
            InvokeRepeating("BehaviourUpdate", UpdateInterval, UpdateInterval);
        }

        public void BehaviourUpdate()
        {
            //Only Do any of this work if we are in FlightMode
            if (IsFlightMode)
            {

                WorkerObjectInstance.UpdateDetails();
            }
        }

        //OnGUI is where any drawing must happen - you cant do this in the update functions
        private static Boolean blnInPostQueue = false;
        private static Double GameUT = 0;
        private static Vessel vesselActive=null;
        public void OnGUI()
        {
            //Do the GUI Stuff - basically get the workers draw stuff into the postrendering queue
            if (IsFlightMode)
            {
                //If time goes backwards assume a reload/restart/ if vessel changes and readd to rendering queue
                if ((GameUT != 0) && (GameUT > Planetarium.GetUniversalTime()))
                {
                    KACWorker.DebugLogFormatted("Time Went Backwards - Load or restart - resetting inqueue flag");
                    blnInPostQueue = false;
                }
                else if (vesselActive == null)
                {
                    KACWorker.DebugLogFormatted("Active Vessel unreadable - resetting inqueue flag");
                    blnInPostQueue = false;
                }
                else if (vesselActive != FlightGlobals.ActiveVessel)
                {
                    KACWorker.DebugLogFormatted("Active Vessel changed - resetting inqueue flag");
                    blnInPostQueue = false;
                }

                //tag these for next time round
                GameUT = Planetarium.GetUniversalTime();
                vesselActive = FlightGlobals.ActiveVessel;

                //if the variable is false assume starting flight mode and add to rendering queue
                if (!blnInPostQueue)
                {
                    //fire off the logic to whether we need to check settings
                    if(Settings.DailyVersionCheck)
                        Settings.VersionCheck(false);

                    KACWorker.DebugLogFormatted("Adding DrawGUI to PostRender Queue");
                    RenderingManager.AddToPostDrawQueue(5, DrawGUI);
                    blnInPostQueue=true;
                }
            }
            else
            {
                //if the variable is true assume leaving flight mode and remove from rendering queue
                if (blnInPostQueue)
                {
                    KACWorker.DebugLogFormatted("Removinging DrawGUI to PostRender Queue");
                    RenderingManager.RemoveFromPostDrawQueue(5, DrawGUI);
                    blnInPostQueue = false;
                }
            }
        }

        //This is what we do every frame when the object is being drawn
        //We dont get here unless we are in flight mode
        public void DrawGUI()
        {
            //one off stuff for styles, etc
            WorkerObjectInstance.SetupDrawStuff();


            //Draw the icon that should be there all the time
            WorkerObjectInstance.DrawIcons();

            //If game has pause menu up see whether to display the interface
            if (!(Settings.HideOnPause && PauseMenu.isOpen))
            {
                //If in flight mode then look for passed alarms to display stuff
                WorkerObjectInstance.TriggeredAlarms();

                //If the mainwindow is visible And no pause menu then draw it
                if (Settings.WindowVisible)
                {
                    WorkerObjectInstance.DrawWindows();
                }
            }
        }
        
        //Update Function - Happens on every frame - this is where behavioural stuff is typically done
        public void Update()
        {
            //Look for key inputs to change settings
            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F11))
            {
                Settings.WindowVisible = !Settings.WindowVisible;
                Settings.Save();
            }

            //if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F10))
            //    Settings.Save();

            //if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F9)) 
            //    Settings.Load();

            //If we have paused the game and the menu is not visible, then unpause so the menu will display
            if (Input.GetKey(KeyCode.Escape) && !PauseMenu.isOpen && FlightDriver.Pause)
            {
                FlightDriver.SetPause(false);
            }
        }

        //Are we flying any ship?
        public static bool IsFlightMode
        {
            get { return FlightGlobals.fetch != null && FlightGlobals.ActiveVessel != null; }
        }

        //Does the active vessel have any manuever nodes
        public static Boolean ManeuverNodeExists
        {
            get
            {
                Boolean blnReturn = false;
                if (IsFlightMode)
                {
                    if (FlightGlobals.ActiveVessel.patchedConicSolver != null)
                    {
                        if (FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes != null)
                        {
                            if (FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Count > 0)
                            {
                                blnReturn = true;
                            }
                        }
                    }
                }
                return blnReturn;
            }
        }
    }

    /// <summary>
    /// Alarm Clock Worker Object
    /// Contains Update and drawing routines
    /// </summary>
    public partial class KACWorker
    {

        //All persistant stuff is stored in the settings object

        //The current UT time - for alarm comparison
        private KerbalTime CurrentTime =new KerbalTime();
        private Boolean blnWarpInfluence = false;
        private DateTime dteWarpInfluence;
        private long lngSecondsWarpLight = 3;

        //Constructor - link to parent and set up time
        #region "Constructor"
        private MonoBehaviour parentBehaviour;
        private KACSettings Settings;

        public KACWorker(MonoBehaviour parent)
        {
            parentBehaviour = parent;
            Settings = KerbalAlarmClock.Settings;

            InitWorkerVariables();
        }

        private void InitWorkerVariables()
        {
            _WindowAddID = rnd.Next(1000, 2000000);
            _WindowMainID = rnd.Next(1000, 2000000);
            _WindowSettingsID = rnd.Next(1000, 2000000);
            _WindowEditID = rnd.Next(1000, 2000000);

            lstVesselTypesForSOI.Add(VesselType.Base);
            lstVesselTypesForSOI.Add(VesselType.Lander);
            lstVesselTypesForSOI.Add(VesselType.Probe);
            lstVesselTypesForSOI.Add(VesselType.Ship);
            lstVesselTypesForSOI.Add(VesselType.Station);
        }
        #endregion

        //Updates the variables that are used in the drawing - this is not on the OnGUI thread
        private Dictionary<String, KACVesselSOI> lstVessels = new Dictionary<String,KACVesselSOI>();
        List<VesselType> lstVesselTypesForSOI = new List<VesselType>();
        public void UpdateDetails()
        {
            CurrentTime.UT = Planetarium.GetUniversalTime();

			//Do we need to turn off the global warp light
            if (dteWarpInfluence == null)
                blnWarpInfluence = false;
            else
                //has it been on long enough?
				if (dteWarpInfluence.AddSeconds(lngSecondsWarpLight) < DateTime.Now)
                    blnWarpInfluence = false;

			//Work out how many game seconds will pass till this runs again
			double SecondsTillNextUpdate;
            double dWarpRate = TimeWarp.CurrentRate;
            SecondsTillNextUpdate = KACBehaviour.UpdateInterval * dWarpRate;

            //Loop through the alarms
            foreach (KACAlarm tmpAlarm in Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title))
            {
                //reset each alarms WarpInfluence flag
                if (dteWarpInfluence == null)
                    tmpAlarm.WarpInfluence= false;
                else
					//if the lights been on long enough
                    if (dteWarpInfluence.AddSeconds(lngSecondsWarpLight) < DateTime.Now)
                        tmpAlarm.WarpInfluence = false;

                //Update Remaining interval for each alarm
                tmpAlarm.Remaining.UT = tmpAlarm.AlarmTime.UT - CurrentTime.UT;

                //set triggered for passed alarms so the OnGUI part can draw the window later
                if ((CurrentTime.UT >= tmpAlarm.AlarmTime.UT) && (tmpAlarm.Enabled) && (!tmpAlarm.Triggered))
                {
                    DebugLogFormatted("Triggering Alarm - " + tmpAlarm.Name);
                    tmpAlarm.Triggered = true;

					//If we are simply past the time make sure we halt the warp
                    if (tmpAlarm.PauseGame)
                    {
                        DebugLogFormatted(string.Format("{0}-Pausing Game", tmpAlarm.Name));
                        TimeWarp.SetRate(0, true);
                        FlightDriver.SetPause(true);
                    }
                    else if (tmpAlarm.HaltWarp)
                    {
                        DebugLogFormatted(string.Format("{0}-Halt Warp",tmpAlarm.Name));
                        TimeWarp.SetRate(0, true);
                    }
                }

				//if in the next two updates we would pass the alarm time then slow down the warp
                if (!tmpAlarm.Actioned && tmpAlarm.Enabled && tmpAlarm.HaltWarp)
                {
                    Double TimeNext = CurrentTime.UT + SecondsTillNextUpdate * 2;
                    //DebugLogFormatted(CurrentTime.UT.ToString() + "," + TimeNext.ToString());
                    if (TimeNext > tmpAlarm.AlarmTime.UT)
                    {
                        tmpAlarm.WarpInfluence = true;
                        blnWarpInfluence = true;
                        dteWarpInfluence = DateTime.Now;

                        TimeWarp w = TimeWarp.fetch;
                        if (w.current_rate_index > 0)
                        {
                            DebugLogFormatted("Reducing Warp");
                            TimeWarp.SetRate(w.current_rate_index - 1, true);
                        }
                    }
                }
            }
			
			//If we are to throw alarms on SOI Changes
            if (Settings.AlarmOnSOIChange)
			{
				foreach(Vessel tmpVessel in FlightGlobals.Vessels)
				{
                    //only track vessels, not debris, EVA, etc
                    if (lstVesselTypesForSOI.Contains(tmpVessel.vesselType))
                    {
                        if (lstVessels.ContainsKey(tmpVessel.id.ToString()) == false)
                        {
                            //Add new Vessels
                            DebugLogFormatted(String.Format("Adding {0}-{1}-{2}-{3}", tmpVessel.id,tmpVessel.vesselName, tmpVessel.vesselType, tmpVessel.mainBody.bodyName));
                            lstVessels.Add(tmpVessel.id.ToString(), new KACVesselSOI(tmpVessel.vesselName, tmpVessel.mainBody.bodyName));
                        }
                        else
                        {
                            //get this vessel from the memory array we are keeping and compare to its SOI
                            if (lstVessels[tmpVessel.id.ToString()].SOIName != tmpVessel.mainBody.bodyName)
                            {
                                //Set a new alarm to display now
                                KACAlarm newAlarm= new KACAlarm(CurrentTime.UT,tmpVessel.vesselName + "- SOI Change",
                                    tmpVessel.vesselName + " Has entered a new Sphere of Influence\r\n" +
                                    "     Old SOI: " + lstVessels[tmpVessel.id.ToString()].SOIName + "\r\n" +
                                    "     New SOI: " + tmpVessel.mainBody.bodyName,
                                    (Settings.AlarmOnSOIChange_Action>0), (Settings.AlarmOnSOIChange_Action>1));
                                Settings.Alarms.Add(newAlarm);

                                DebugLogFormatted("Triggering SOI Alarm - " + newAlarm.Name);
                                newAlarm.Triggered = true;
                                newAlarm.Actioned = true;
                                if (Settings.AlarmOnSOIChange_Action > 1)
                                {
                                    DebugLogFormatted(string.Format("{0}-Pausing Game", newAlarm.Name));
                                    TimeWarp.SetRate(0, true);
                                    FlightDriver.SetPause(true);
                                }
                                else if (Settings.AlarmOnSOIChange_Action > 0)
                                {
                                    DebugLogFormatted(string.Format("{0}-Halt Warp", newAlarm.Name));
                                    TimeWarp.SetRate(0, true);
                                }

                                //reset the name string for next check
                                lstVessels[tmpVessel.id.ToString()].SOIName = tmpVessel.mainBody.bodyName;
                            }
                        }
                    }
                }
			}
        }
		
		
		
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
            if (!PauseMenu.isOpen)
            {
                if (FlightDriver.Pause)
                {
                    iconToShow = KACResources.GetPauseIcon();
                }
                else if (blnWarpInfluence)
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
        int intLeftWindowMinHeight = 129;
        int intLeftWindowBaseHeight = 119;
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
                }
                else
                {
                    MainWindowPos.height = intLeftWindowBaseHeight;
                    if (Settings.Alarms.Count > 1)
                    {
                        if (Settings.Alarms.Count <Settings.AlarmListMaxAlarmsInt)
                            MainWindowPos.height = intLeftWindowBaseHeight + ((Settings.Alarms.Count - 1) * 26);
                        else
                            MainWindowPos.height = intLeftWindowBaseHeight + ((Settings.AlarmListMaxAlarmsInt - 1) * 26);
                        if (Settings.AlarmOnSOIChange)
                            MainWindowPos.height += 31;
                    }
                }
                Settings.WindowPos = GUILayout.Window(_WindowMainID, MainWindowPos, FillWindow, "Kerbal Alarm Clock - " + Settings.Version, GUILayout.MinWidth(intLeftWindowWidth), GUILayout.MaxWidth(intLeftWindowWidth));

                if (_ShowSettings)
                {
                    _WindowSettingsRect = GUILayout.Window(_WindowSettingsID, new Rect(Settings.WindowPos.x + Settings.WindowPos.width, Settings.WindowPos.y, 388, 400), FillSettingsWindow, "Settings and Globals", GUILayout.MinWidth(intRightWindowWidth), GUILayout.MaxWidth(intRightWindowWidth), GUILayout.ExpandWidth(false));
                }
                else if (_ShowAddPane)
                {
                    long AddWindowHeight;
                    switch (intAddType)
                    {
                        case 0: AddWindowHeight = 413; break;
                        case 1: AddWindowHeight = 350; break;
                        default: AddWindowHeight = 413; break;
                    }
                    _WindowAddRect = GUILayout.Window(_WindowAddID, new Rect(Settings.WindowPos.x + Settings.WindowPos.width, Settings.WindowPos.y, intRightWindowWidth, AddWindowHeight), FillAddWindow, "Add New Alarm", GUILayout.MinWidth(intRightWindowWidth), GUILayout.MaxWidth(intRightWindowWidth), GUILayout.ExpandWidth(false));
                }
                else if (_ShowEditPane)
                {
                    long EditWindowHeight=210;
                    if (alarmEdit.AlarmTime.UT < CurrentTime.UT) EditWindowHeight = 170;
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
            if (DrawToggle(ref _ShowSettings,KACResources.GetSettingsButtonIcon(Settings.VersionAttentionFlag), KACResources.styleSmallButton) && _ShowSettings)
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

            if (Settings.AlarmOnSOIChange)
            {
                GUILayout.BeginHorizontal(KACResources.styleSOIIndicator,GUILayout.Height(18));
                GUILayout.FlexibleSpace();
                GUILayout.Label(KACResources.iconSOI, KACResources.styleSOIIndicator);
                String strSOILabel = "SOI Change Alarms Enabled";
                if (Settings.AlarmOnSOIChange_Action > 1) strSOILabel += " (P)";
                else if (Settings.AlarmOnSOIChange_Action > 0) strSOILabel += "(W)";
                GUILayout.Label(strSOILabel, KACResources.styleSOIIndicator);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            //Current Game time at the botttom of the control 
            GUILayout.BeginHorizontal();
            GUILayout.Label("Current Time:", KACResources.styleHeading);
            if(Settings.TimeAsUT)
                GUILayout.Label(CurrentTime.UTString(), KACResources.styleContent);
            else
                GUILayout.Label(CurrentTime.DateString(), KACResources.styleContent);
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
                    if (tmpAlarm.AlarmTime.UT > CurrentTime.UT && tmpAlarm.Enabled && !tmpAlarm.Actioned)
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
                        nextAlarm=tmpAlarm;
                }
            }

            if (nextAlarm == null)
            {
                GUILayout.Label("No Enabled Future Alarms in list");
            }
            else
            {
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
            if (Event.current.type==EventType.repaint)
                rectScrollview = new Rect (0,0,GUILayoutUtility.GetLastRect().width,GUILayoutUtility.GetLastRect().height);

        }

        private Boolean DrawAlarmLine(KACAlarm tmpAlarm)
        {
            Boolean blnReturn = false;

            GUILayout.BeginHorizontal();

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
            if ((Event.current.type == EventType.repaint) && rectScrollview.Contains(Event.current.mousePosition) && rectLabel.Contains(Event.current.mousePosition) && Input.GetMouseButtonDown(0))
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
                blnReturn=true;
            GUILayout.EndHorizontal();

            return blnReturn;
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
        public Boolean DrawToggle(ref Boolean blnVar, string ButtonText, GUIStyle style,params GUILayoutOption[] options)
        {
            Boolean blnReturn = GUILayout.Toggle(blnVar, ButtonText, style, options );
            return ToggleResult(ref blnVar, ref  blnReturn);
        }

        public Boolean DrawToggle(ref Boolean blnVar, Texture image , GUIStyle style, params GUILayoutOption[] options)
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
			Boolean blnReturn=false;
			Boolean blnToggleInitial = blnVar;

            GUILayout.BeginHorizontal();
            //Draw the radio
            DrawToggle(ref blnVar,"",KACResources.styleCheckbox,options);
			//Spacing
            GUILayout.Space(15);
            //and a label...
			GUILayout.Label(strText, KACResources.styleCheckboxLabel,options);
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
				blnReturn=true;
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


            return !(InitialChoice==Selected);
        }


        #endregion

        /// <summary>
        /// Some Structured logging to the debug file
        /// </summary>
        /// <param name="Message"></param>
        public static void DebugLogFormatted(string Message)
        {
            string strMessageLine = string.Format("{0},KerbalAlarmClock,{1}", DateTime.Now, Message);
            Debug.Log(strMessageLine);
        }
    }

}
