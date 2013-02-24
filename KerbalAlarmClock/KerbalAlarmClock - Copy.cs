using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KSP;


namespace KerbalAlarmClock
{
    public class KerbalAlarmClock : PartModule
    {
        public Settings settings = new Settings();

        public override void OnAwake()
        {
            if (KACBehaviour.GameObjectInstance == null)
                KACBehaviour.GameObjectInstance = GameObject.Find("KACBehaviour") ?? new GameObject("KACBehaviour", typeof(KACBehaviour));

        }

        public override void OnLoad(ConfigNode node)
        {
            settings.Load();
        }
        public override void OnSave(ConfigNode node)
        {
            settings.Save();
        }
    }



    public class KACBehaviour : MonoBehaviour
    {
        public static GameObject GameObjectInstance;
        private static Boolean _WindowVisible;
        private static KACWorker WorkerObjectInstance = new KACWorker();

        public void Awake()
        {
            Debug.Log("####### Awakening the KerbalAlarmClock #######");
            //Keep the Behaviour active even on scene Loads
            DontDestroyOnLoad(this);

            //Debug.Log("####### KerbalAlarmClock-ADDING RENDERER #######");
            //if (IsFlightMode)
            //{
            //    Debug.Log("####### KerbalAlarmClock-ADDING RENDERER #######");
            //    RenderingManager.AddToPostDrawQueue(1, TimeToDoStuff);
            //}

            Debug.Log("####### Invoking Worker Function KerbalAlarmClock #######");
            CancelInvoke();
            InvokeRepeating("BehaviourUpdate", 0.25F, 0.25F);
            //Debug.Log("####### KerbalAlarmClock-ADDED RENDERER #######");
        }

        //**** - fps counter - **** //


        //OnGUI is where any drawing must happen - you cant do this in the update functions
        //Do we just pass off to the worker and do all the code in there, maybe separate file??
        public void BehaviourUpdate()
        {
            if (IsFlightMode)
            {
                //Move this to be a repeating invoke every 1/2 second?
                //TODO: Count how often we are doing these ONGUIs
                WorkerObjectInstance.UpdateDetails();
            }
        }

        
        public void OnGUI()
        {
            //Only Do any of this work if we are in FlightMode
            if (IsFlightMode && _WindowVisible)
            {
                //Move this to be a repeating invoke every 1/2 second?
                //TODO: Count how often we are doing these ONGUIs
                //WorkerObjectInstance.UpdateDetails();

                //Do the GUI Stuff
                WorkerObjectInstance.DrawWindow();
            }
        }
        
        //Update Function - Happens on every frame - this is where behavioural stuff is typically done
        public void Update()
        {
            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F11))
                _WindowVisible = !_WindowVisible;
        }

        //Are we flying any ship - do we move this down below
        private static bool IsFlightMode
        {
            get { return FlightGlobals.fetch != null && FlightGlobals.ActiveVessel != null; }
        }

    }

    //Guts of code
    public class KACWorker
    {
        //Styles for window
        public static GUIStyle styleWindow;
        public static GUIStyle styleHeading;
        public static GUIStyle styleContent;

        //The alarms we have set
        public List<KACAlarm> Alarms;

        //The current UT time - for alarm comparison
        private KerbalTime CurrentTime =new KerbalTime();

        //Do I still need this??
        public void TimeToDoStuff()
        {
            Debug.Log("####### KerbalAlarmClock-TimeToDoStuff #######");

            UpdateDetails();
    
            //KACBehaviour.
        
        }

        //Updates the variables that are used in the drawing
        public void UpdateDetails()
        {
            //year is 0 here - need to look at how to use time - maybe have a setdate and a setduration or something - date give year 1, etc, duration would be year 0 - ie without all teh +1s in teh math
            CurrentTime.UT = Planetarium.GetUniversalTime();
        }


        //Basic setup of draw stuff
        private static Rect _windowPos;
        public void DrawWindow()
        {
            if (styleWindow == null) {
                SetStyles();
            }

            GUI.skin = HighLogic.Skin;
            _windowPos = GUILayout.Window(12356, _windowPos, FillWindow, "Kerbal Alarm Clock", GUILayout.Width(275));

        }

        //Now teh layout
        public void FillWindow(int intWindowID)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Current Time:", styleHeading);
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical();
            GUILayout.Label(CurrentTime.ToString(), styleContent);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        //Should only be called once
        public void SetStyles()
        {
            styleWindow = new GUIStyle(GUI.skin.window);
            styleWindow.fixedWidth = 250;

            styleHeading = new GUIStyle(GUI.skin.label);
            styleHeading.normal.textColor = Color.white;
            styleHeading.fontStyle = FontStyle.Bold;

            styleContent = new GUIStyle(GUI.skin.label);
            styleContent.normal.textColor = Color.green;
            styleContent.alignment = TextAnchor.UpperRight;
            styleContent.stretchWidth = true;
        }
    }


    public class KACAlarm
    {
        public Boolean Enabled;
        public KerbalTime AlarmTime;
        public string Message;
    }

    //KerbalTime calculation object - set UT and get the rest back
    //Put in something to get a short string
    //Add a time type - Date or TimeSpan
    public class KerbalTime
    {
        const double HoursPerYearKerbal = 2556.5402;
        const double HoursPerYearEarth = 8766.1527121;

        public double UT;

        public KerbalTime()
        { }
        public KerbalTime(double NewUT)
        {
            UT = NewUT;
        }


        public long Second
        {
            get { return Convert.ToInt64(UT % 60); }
        }
        public long Minute
        {
            get { return Convert.ToInt64((UT / 60) % 60); }
        }

        private double HourRaw { get { return UT / 60 / 60; } }
        public long HourEarth
        {
            get { return Convert.ToInt64(HourRaw / 24); }
        }
        public long HourKerbal
        {
            get { return Convert.ToInt64(HourRaw / 6); }
        }

        public long DayEarth
        {
            get { return Convert.ToInt64(((HourRaw % HoursPerYearEarth)/ 24)+1); }
        }
        public long DayKerbal
        {
            get { return Convert.ToInt64(((HourRaw % HoursPerYearKerbal) / 24) + 1); }
        }

        public long YearEarth
        {
            get { return Convert.ToInt64((HourRaw / HoursPerYearEarth) + 1); }
        }
        public long YearKerbal
        {
            get { return Convert.ToInt64((HourRaw % HoursPerYearKerbal) + 1); }
        }

        public string StringLong
        {
            get { return string.Format("Year {0}, Day {1}, {2:00}:{3:00}:{4:00}", YearEarth, DayEarth, HourEarth, Minute, Second); }
        }
        //public string StringShort
        //{
        //    get {
        //        string strReturn = "";
                
        //        return string.Format("{0}y, {1}d, {2}h, {3}m, {4}s", YearEarth, DayEarth, HourEarth, Minute, Second); 
        //    }
        //}

        public override string ToString()
        {
            return string.Format("Year {0},Day {1}, {2:00}:{3:00}:{4:00}", YearEarth, DayEarth, HourEarth, Minute, Second);
        }
    }

    public class Settings
    {
        private string _settingsFile;
        
        

        public void New()
        {
            _settingsFile = "KerbalAlarmClock_Settings.cfg";
        }
        public void New(string SettingsFile)
        {
            _settingsFile = SettingsFile;
        }

        public void Load()
        {

        }

        public void Save()
        {

        }

    }
}
