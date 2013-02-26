using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;

using UnityEngine;
using KSP;


namespace KerbalAlarmClock
{

    /// <summary>
    /// Basic Part piece that creates the Classes that run in the background at all times
    /// </summary>
    public class KerbalAlarmClock : PartModule
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
        public void OnGUI()
        {
            //Do the GUI Stuff
            if (IsFlightMode)
            {
                //If time goes backwards assume a reload/restart and readd to rendering queue
                if ((GameUT != 0) && (GameUT > Planetarium.GetUniversalTime()))
                {
                    KACWorker.DebugLogFormatted("Time Went Backwards - Load or restart");
                    KACWorker.DebugLogFormatted("Readding DrawGUI to PostRender Queue");
                    RenderingManager.AddToPostDrawQueue(5, DrawGUI);
                    blnInPostQueue = true;
                }
                GameUT = Planetarium.GetUniversalTime();
                
                //if the variable is false assume starting flight mode and add to rendering queue
                if (!blnInPostQueue)
                {
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
        Boolean UpdateChecked = false;
        public void DrawGUI()
        {
            if (!UpdateChecked)
            {
                UpdateChecked = true;
                if (Settings.CheckForUpdatesOnStart)
                {
                    KACWorker.DebugLogFormatted("Checking for update");
                    KACUtils.getLatestVersion();
                }
            }

            WorkerObjectInstance.SetupDrawStuff();

            //If in flight mode then look for passed alarms to display stuff
            WorkerObjectInstance.TriggeredAlarms();

            //Draw the icon that should be there all the time
            WorkerObjectInstance.DrawIcons();

            //If the mainwindow is visible then draw it
            if (Settings.WindowVisible)
            {
                WorkerObjectInstance.DrawWindow();
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
    public class KACWorker
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


        private int intMainWindowID;
        private int intAddWindowID;
        
        public KACWorker(MonoBehaviour parent)
        {
            parentBehaviour = parent;
            Settings = KerbalAlarmClock.Settings;

            intMainWindowID = rnd.Next(1, 2000000);
            intAddWindowID = rnd.Next(1, 2000000);
        }
        #endregion

        //Updates the variables that are used in the drawing - this is not on the OnGUI thread
        public void UpdateDetails()
        {
            CurrentTime.UT = Planetarium.GetUniversalTime();

            if (dteWarpInfluence == null)
                blnWarpInfluence = false;
            else
                if (dteWarpInfluence.AddSeconds(lngSecondsWarpLight) < DateTime.Now)
                    blnWarpInfluence = false;

            double SecondsTillNextUpdate;
            double dWarpRate = TimeWarp.CurrentRate;

            SecondsTillNextUpdate = KACBehaviour.UpdateInterval * dWarpRate;
            //DebugLogFormatted(dWarpRate.ToString());


            //Loop through the alarms
            foreach (KACAlarm tmpAlarm in Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title))
            {
                //reset each alarms WarpInfluence flag
                if (dteWarpInfluence == null)
                    tmpAlarm.WarpInfluence= false;
                else
                    if (dteWarpInfluence.AddSeconds(lngSecondsWarpLight) < DateTime.Now)
                        tmpAlarm.WarpInfluence = false;

                //Update Remaining interval
                tmpAlarm.Remaining.UT = tmpAlarm.AlarmTime.UT - CurrentTime.UT;
                //DebugLogFormatted(string.Format("{0}-{1}-{2}", CurrentTime.UT, tmpAlarm.AlarmTime.UT, tmpAlarm.Remaining.UT));

                //set triggered for passed alarms so the OnGUI part can draw the window later
                if ((CurrentTime.UT >= tmpAlarm.AlarmTime.UT) && (tmpAlarm.Enabled) && (!tmpAlarm.Triggered))
                {
                    DebugLogFormatted("Triggering Alarm - " + tmpAlarm.Name);
                    tmpAlarm.Triggered = true;

                    if (tmpAlarm.HaltWarp)
                    {
                        DebugLogFormatted("Halt Warp");
                        TimeWarp.SetRate(0, true);
                    }
                }

                //test for alarms nearing their time and slow warp
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
        }

        #region "OnGUI Stuff"
        public void SetupDrawStuff()
        {
            GUI.skin = HighLogic.Skin;
            if (KACResources.styleWindow == null)
            {
                KACResources.SetStyles();
            }
        }

        //On OnGUI - draw alarms if needed
        System.Random rnd = new System.Random();
        public void TriggeredAlarms()
        {
            foreach (KACAlarm tmpAlarm in Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title))
            {
                if (tmpAlarm.Enabled)
                {
                 //also test triggered and actioned
                    if (CurrentTime.UT>= tmpAlarm.AlarmTime.UT  )
                    {
                        if (tmpAlarm.Triggered && !tmpAlarm.Actioned)
                        {
                            tmpAlarm.Actioned = true;
                            DebugLogFormatted("Actioning Alarm");


                            //PopupDialog.SpawnPopupDialog("Kerbal Alarm Clock", tmpAlarm.Message, "Close", false, GUI.skin);
                        }
                        if (tmpAlarm.Actioned && !tmpAlarm.AlarmWindowClosed)
                        {
                            if (tmpAlarm.AlarmWindowID == 0)
                            {
                                tmpAlarm.AlarmWindowID = rnd.Next(1, 2000000);
                                tmpAlarm.AlarmWindow = new Rect((Screen.width/2)-160,(Screen.height/2)-100,320,100);
                            }
                            tmpAlarm.AlarmWindow = GUILayout.Window(tmpAlarm.AlarmWindowID, tmpAlarm.AlarmWindow, FillAlarmWindow, tmpAlarm.Name,GUILayout.MinWidth(320));
                        }
                    }
                }
            }
        }

        public void FillAlarmWindow(int windowID)
        {
            KACAlarm tmpAlarm = Settings.Alarms.GetByWindowID(windowID);

            GUILayout.BeginVertical();

            GUILayout.BeginVertical(GUI.skin.textArea);
            GUILayout.Label(tmpAlarm.AlarmTime.DateString(), KACResources.styleAlarmMessageTime);
            GUILayout.Label(tmpAlarm.Message, KACResources.styleAlarmMessage);
            GUILayout.EndVertical();
            if( GUILayout.Button("Close Alarm"))
            {
                tmpAlarm.AlarmWindowClosed=true;
            }

            GUILayout.EndVertical();
            GUI.DragWindow();

        }


        /// <summary>
        /// Draw the icon on the screen
        /// </summary>
        public void DrawIcons()
        {
            Texture2D iconToShow;

            if (blnWarpInfluence)
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


        //Basic setup of draw stuff
        int intLeftWindowWidth = 295;
        int intLeftWindowMinHeight = 129;
        int intLeftWindowBaseHeight = 129;
        int intRightWindowWidth = 388;


        //is the add pane visible
        private Boolean _ShowAddPane = false;
        static Rect _WindowPosAdd;
        //Settings Window
        private Boolean _ShowSettings = false;
        private static Rect _WindowPosSettings;
        private static int _WindowSettingsID=0;

        public void DrawWindow()
        {
            //if (_ShowSettings)
            //{
            //    if (_WindowSettingsID == 0) _WindowSettingsID = rnd.Next(1,2000000);
            //    _WindowPosSettings.x = Settings.WindowPos.x;
            //    _WindowPosSettings.y = Settings.WindowPos.y;
            //    _WindowPosSettings = GUILayout.Window(_WindowSettingsID, _WindowPosSettings, FillSettingsWindow, "Settings for Kerbal Alarm Clock - " + Settings.Version);
            //}
            //else
            //{
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
                        MainWindowPos.height = intLeftWindowBaseHeight + ((Settings.Alarms.Count - 1) * 27);
                    }
                }
                Settings.WindowPos = GUILayout.Window(123546, MainWindowPos, FillWindow, "Kerbal Alarm Clock - " + Settings.Version, GUILayout.MinWidth(intLeftWindowWidth), GUILayout.MaxWidth(intLeftWindowWidth));

                if (_ShowAddPane)
                {
                    long AddWindowHeight;
                    switch (intAddType)
                    {
                        case 0: AddWindowHeight = 397; break;
                        case 1: AddWindowHeight = 370; break;
                        default: AddWindowHeight = 397; break;
                    }
                    _WindowPosAdd = GUILayout.Window(1235467, new Rect(Settings.WindowPos.x + Settings.WindowPos.width, Settings.WindowPos.y, intRightWindowWidth, AddWindowHeight), FillAddWindow, "Add New Alarm", GUILayout.MinWidth(intRightWindowWidth), GUILayout.MaxWidth(intRightWindowWidth), GUILayout.ExpandWidth(false));
                }
            //}
        }

        //Now the layout
        public void FillWindow(int intWindowID)
        {
            //GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();

            //Heading Part
            GUILayout.BeginHorizontal();
            GUILayout.Label("Alarm List", KACResources.styleHeading, GUILayout.ExpandWidth(true));

            //If the settings pane is visible then dont show the rest
            string strMinimizedText = "_";
            if (Settings.WindowMinimized) strMinimizedText = "+";

            if (GUILayout.Button(strMinimizedText, KACResources.styleSmallButton))
            {
                Settings.WindowMinimized = !Settings.WindowMinimized;
                Settings.Save();
            }
            
            if (DrawToggle(ref _ShowAddPane, "Add", KACResources.styleSmallButton))
            {
                //reset the add stuff
                NewAddAlarm();
            }

            GUILayout.EndHorizontal();

            //Text Area for content portion
            GUILayout.BeginVertical(GUI.skin.textArea);
            if (Settings.WindowMinimized)
            {
                WindowLayout_Minimized();
            }
            else
            {
                WindowLayout_AlarmList();
            }
            GUILayout.EndVertical();

            //Current Game time at the botttom of the control
            GUILayout.BeginHorizontal();
            GUILayout.Label("Current Time:", KACResources.styleHeading);
            GUILayout.Label(CurrentTime.DateString(), KACResources.styleContent);
            GUILayout.EndHorizontal();


            GUILayout.EndVertical();

            //GUILayout.EndHorizontal();

            GUI.DragWindow();
        }
        
        //Display minimal info about the next alarm
        private void WindowLayout_Minimized()
        {
            KACAlarm nextAlarm = null;

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
                GUILayout.BeginHorizontal();

                string strLabelText = "";
                strLabelText = string.Format("{0} ({1})", nextAlarm.Name, nextAlarm.Remaining.IntervalString(3));
                GUIStyle styleLabel = KACResources.styleAlarmText;

                GUILayout.Label(strLabelText, styleLabel);
                if (nextAlarm.HaltWarp)
                {
                    GUILayout.Label(KACResources.GetWarpListIcon(nextAlarm.WarpInfluence), KACResources.styleLabelWarp);
                }

                if (GUILayout.Button(KACResources.btnRedCross, GUI.skin.button, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                    Settings.Alarms.Remove(nextAlarm); 
                GUILayout.EndHorizontal();
            }
        }

        //Display Full alarm list - Sort by Date/Time????
        private void WindowLayout_AlarmList()
        {
            if (Settings.Alarms.CountInSave(HighLogic.CurrentGame.Title) == 0)
            {
                GUILayout.Label("No Alarms in the List");
            }
            else
            {
                List<KACAlarm> AlarmsToRemove = new List<KACAlarm>();
                foreach (KACAlarm tmpAlarm in Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title))
                {
                    GUILayout.BeginHorizontal();

                    string strLabelText = "";
                    strLabelText = string.Format("{0} ({1})", tmpAlarm.Name, tmpAlarm.Remaining.IntervalString(3));

                    GUIStyle styleLabel = KACResources.styleAlarmText;
                    if ((!tmpAlarm.Enabled || tmpAlarm.Actioned))
                        styleLabel = KACResources.styleAlarmTextGrayed;
                    GUIStyle styleLabelWarpWorking = KACResources.styleLabelWarp;
                    if ((!tmpAlarm.Enabled || tmpAlarm.Actioned))
                        styleLabelWarpWorking = KACResources.styleLabelWarpGrayed;


                    GUILayout.Label(strLabelText, styleLabel);
                    //GUILayout.Button("E", GUI.skin.button, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20));
                    if (tmpAlarm.HaltWarp)
                    {
                        GUILayout.Label(KACResources.GetWarpListIcon(tmpAlarm.WarpInfluence), KACResources.styleLabelWarp);
                    }

                    if (GUILayout.Button(KACResources.btnRedCross, GUI.skin.button, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                        AlarmsToRemove.Add(tmpAlarm);
                    GUILayout.EndHorizontal();
                }

                foreach (KACAlarm tmpAlarm in AlarmsToRemove)
                {
                    Settings.Alarms.Remove(tmpAlarm);
                    Settings.Save();
                }

            }
        }

        int intAddType = 0;
        private string strAlarmName = "";
        private string strAlarmMessage = "";
        private string strAlarmNameNode = "";
        private string strAlarmMessageNode = "";
        //private string strAlarmNameXFER = "";
        //private string strAlarmMessageXFER = "";
        private Boolean blnHaltWarp = true;

        /// <summary>
        /// Code to reset the settings etc when athe new button is hit
        /// </summary>
        private void NewAddAlarm()
        {
            strYears = "0";
            strDays = "0";
            strHours = "0";
            strMinutes = "10";

            strAlarmName = FlightGlobals.ActiveVessel.vesselName + " Alarm";
            strAlarmNameNode = FlightGlobals.ActiveVessel.vesselName + " Node";
            strAlarmMessage = "Time to pay attention to " + FlightGlobals.ActiveVessel.vesselName + "\r\nManual Alarm";
            strAlarmMessageNode = "Time to pay attention to " + FlightGlobals.ActiveVessel.vesselName + "\r\nNearing Maneuvering Node";
            blnHaltWarp = true;

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

            GUILayout.BeginVertical(GUILayout.Width(5));
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            //string[] strAddTypes = new string[] { "Raw", "Maneuver Node", "Transfer Window" };
            string[] strAddTypes = new string[] { "Raw", "Maneuver Node"};
            intAddType = GUILayout.Toolbar((int)intAddType, strAddTypes);

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
            
            GUILayout.BeginVertical(GUILayout.Width(260),GUILayout.MaxWidth(260));
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
            GUILayout.Space(55);
            DrawCheckbox(ref blnHaltWarp,"Kill Time Warp");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

        }


        //Variabled for Raw Alarm screen
        string strYears = "0", strDays = "0", strHours = "0", strMinutes = "0";
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
            DrawCheckbox(ref blnRawDate, "Date", GUILayout.Width(80));
            DrawCheckbox(ref blnRawInterval, "Time Interval", GUILayout.Width(110));
            if (blnRawDateStart!= blnRawDate)
            {
                blnRawInterval = !blnRawDate;
            } else {
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

            GUILayout.EndVertical();
            try
            {
                rawTime.BuildUT(Convert.ToDouble(strYears),
                                Convert.ToDouble(strDays),
                                Convert.ToDouble(strHours),
                                Convert.ToDouble(strMinutes),
                                0);
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
                GUILayout.Label(rawTime.DateString(), KACResources.styleContent);
                GUILayout.Label(rawTimeToAlarm.IntervalString(), KACResources.styleContent);
                GUILayout.EndVertical();
                GUILayout.Space(15);
                if (GUILayout.Button("Add Alarm", GUILayout.Width(90), GUILayout.Height(40))) 
                {
                    Settings.Alarms.Add(new KACAlarm(rawTime.UT, strAlarmName, strAlarmMessage, blnHaltWarp));
                    Settings.Save();
                    _ShowAddPane = false;
                }
                GUILayout.EndHorizontal();


            }
            catch (Exception)
            {
                GUILayout.Label("Unable to combine all text fields to date",GUILayout.ExpandWidth(true));
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
                    GUILayout.Label("No Maneuver Nodes Found",GUILayout.ExpandWidth(true));
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
                            lngMarginMinutes =Convert.ToInt64(strNodeMargin);
                            nodeAlarm = new KerbalTime(nodeTime.UT - (lngMarginMinutes * 60));
                            nodeAlarmInterval = new KerbalTime(nodeTime.UT - CurrentTime.UT - (lngMarginMinutes * 60));
                        }
	                    catch (Exception)
	                    {
                            nodeAlarm = null;
                            nodeAlarmInterval = null;
		                    strMarginConversion = "Unable to Add the Margin Minutes";
	                    }

                        if ((nodeTime.UT > CurrentTime.UT) && strMarginConversion=="")
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.BeginVertical(GUILayout.Width(100), GUILayout.MaxWidth(100));
                            GUILayout.Label("Node Date:", KACResources.styleAddHeading);
                            GUILayout.Label("Time to Node:", KACResources.styleAddHeading);
                            GUILayout.Label("Time to Alarm:", KACResources.styleAddHeading);
                            GUILayout.EndVertical();
                            GUILayout.BeginVertical(GUILayout.Width(150), GUILayout.MaxWidth(150));
                            GUILayout.Label(nodeTime.DateString(), KACResources.styleContent);
                            GUILayout.Label(nodeInterval.IntervalString(), KACResources.styleContent);
                            GUILayout.Label(nodeAlarmInterval.IntervalString(), KACResources.styleContent);
                            GUILayout.EndVertical();
                            GUILayout.Space(15);

                            if (GUILayout.Button("Add Alarm", GUILayout.Width(90), GUILayout.Height(70)))
                            {
                                Settings.Alarms.Add(new KACAlarm(nodeAlarm.UT, strAlarmNameNode, strAlarmMessageNode, blnHaltWarp));
                                Settings.Save();
                                _ShowAddPane = false;
                            }
                            GUILayout.EndHorizontal();
                            
                            blnFoundNode=true;
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

        #endregion

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
            if (blnReturn != blnVar)
            {
                blnVar = blnReturn;
                DebugLogFormatted("Toggle Changed:" + blnVar.ToString());
                return true;
            }
            return false;
        }


        public Boolean DrawToggle(ref Boolean blnVar, Texture image , GUIStyle style, params GUILayoutOption[] options)
        {
            Boolean blnReturn = GUILayout.Toggle(blnVar, image, style, options);
            if (blnReturn != blnVar)
            {
                blnVar = blnReturn;
                DebugLogFormatted("Toggle Changed:" + blnVar.ToString());
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
