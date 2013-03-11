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

        //public override void OnLoad(ConfigNode node)
        //{
        //    //Load the Settings File
        //    //Settings.Load();
        //}
        //public override void OnSave(ConfigNode node)
        //{
        //    //Save the Settings File
        //    //Settings.Save();

        //}

    }


    public static class KACWorkerGameState
    {
        public static string LastSaveGameName = "";
        public static GameScenes LastGUIScene=GameScenes.SPLASHSCREEN;
        public static Vessel LastVessel=null;
        public static CelestialBody LastSOIBody=null;

        public static string CurrentSaveGameName = "";
        public static GameScenes CurrentGUIScene=GameScenes.SPLASHSCREEN;
        public static Vessel CurrentVessel=null;
        public static CelestialBody CurrentSOIBody=null;

        public static Boolean ChangedSaveGameName { get { return (LastSaveGameName != CurrentSaveGameName); } }
        public static Boolean ChangedGUIScene { get { return (LastGUIScene != CurrentGUIScene); } }
        public static Boolean ChangedVessel { get { if(LastVessel==null) return true; else return (LastVessel != CurrentVessel); } }
        public static Boolean ChangedSOIBody { get { if (LastSOIBody == null) return true; else return (LastSOIBody != CurrentSOIBody); } }


        //The current UT time - for alarm comparison
        public static KerbalTime CurrentTime = new KerbalTime();
        public static KerbalTime LastTime = new KerbalTime();

        public static Boolean CurrentlyUnderWarpInfluence = false;
        public static DateTime CurrentWarpInfluenceStartTime;


        //Are we flying any ship?
        public static bool IsFlightMode
        {
            get { return FlightGlobals.fetch != null && FlightGlobals.ActiveVessel != null; }
        }

        public static bool PauseMenuOpen
        {
            get
            {
                try { return PauseMenu.isOpen; }
                catch (Exception)
                {
                    //if we cant read it it cant be open.
                    return false;
                }
            }
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

        public static Boolean SOIPointExists
        {
            get
            {
                Boolean blnReturn = false;

                if (FlightGlobals.ActiveVessel != null)
                {
                    if (FlightGlobals.ActiveVessel.orbit != null)
                    {
                        List<Orbit.PatchTransitionType> SOITransitions = new List<Orbit.PatchTransitionType> { Orbit.PatchTransitionType.ENCOUNTER, Orbit.PatchTransitionType.ESCAPE };
                        blnReturn = SOITransitions.Contains(FlightGlobals.ActiveVessel.orbit.patchEndTransition);
                    }
                }

                return blnReturn;
            }
        }

        //do null checks on all these!!!!!
        public static void SetCurrentGUIStates()
        {
            KACWorkerGameState.CurrentGUIScene = HighLogic.LoadedScene;
        }

        public static void SetLastGUIStatesToCurrent()
        {
            KACWorkerGameState.LastGUIScene = KACWorkerGameState.CurrentGUIScene;
        }
        
        public static void SetCurrentFlightStates()
        {
            if (HighLogic.CurrentGame != null)
                KACWorkerGameState.CurrentSaveGameName = HighLogic.CurrentGame.Title;
            else
                KACWorkerGameState.CurrentSaveGameName = "";
            
            try { KACWorkerGameState.CurrentTime.UT = Planetarium.GetUniversalTime(); }
            catch (Exception) { }
            //if (Planetarium.fetch!=null) KACWorkerGameState.CurrentTime.UT = Planetarium.GetUniversalTime();
            
            if (KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT)
            {
                KACWorkerGameState.CurrentVessel = FlightGlobals.ActiveVessel;
                KACWorkerGameState.CurrentSOIBody = FlightGlobals.ActiveVessel.mainBody;
            }
            else
            {
                KACWorkerGameState.CurrentVessel = null;
                KACWorkerGameState.CurrentSOIBody = null;
            }
        }

        public static void SetLastFlightStatesToCurrent()
        {
            KACWorkerGameState.LastSaveGameName = KACWorkerGameState.CurrentSaveGameName;
            KACWorkerGameState.LastTime = KACWorkerGameState.CurrentTime;
            KACWorkerGameState.LastVessel = KACWorkerGameState.CurrentVessel;
            KACWorkerGameState.LastSOIBody = KACWorkerGameState.CurrentSOIBody;
        }
    }

    /// <summary>
    /// This is the behaviour object that we hook events on to 
    /// </summary>
    public class KACBehaviour : MonoBehaviour
    {
        //Game object that keeps us running
        public static GameObject GameObjectInstance;
        
        //GameState Objects for the monobehaviour
        private Boolean IsInPostDrawQueue=false ;
        private Boolean ShouldBeInPostDrawQueue = false;
        private Double LastGameUT;
        private Vessel LastGameVessel;

        //Worker and Settings objects
        private KACWorker WorkerObjectInstance;
        private KACSettings Settings;
        public static float UpdateInterval = 0.2F;

        //Constructor to set KACWorker parent object to this and access to the settings
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
            
            //Set initial GameState
            KACWorkerGameState.LastGUIScene = HighLogic.LoadedScene;

            //Set up the updating function - do this 5 times a sec not on every frame.
            KACWorker.DebugLogFormatted("Invoking Worker Function KerbalAlarmClock");
            CancelInvoke();
            InvokeRepeating("BehaviourUpdate", UpdateInterval, UpdateInterval);
            InvokeRepeating("DebugWriter", 2F, 2F);
        }

        #region "Update Code"
        //Update Function - Happens on every frame - this is where behavioural stuff is typically done
        public void Update()
        {
            KACWorkerGameState.SetCurrentGUIStates();
            //if scene has changed
            if (KACWorkerGameState.ChangedGUIScene)
                KACWorker.DebugLogFormatted("Scene Change from '{0}' to '{1}'", KACWorkerGameState.LastGUIScene.ToString(), KACWorkerGameState.CurrentGUIScene.ToString());

            HandleKeyStrokes();

            //Work out if we should be in the gui queue
            ShouldBeInPostDrawQueue = Settings.DrawScenes.Contains(HighLogic.LoadedScene);

            //Fix the issues with Flight SCene Stuff
            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                //If time goes backwards assume a reload/restart/ if vessel changes and readd to rendering queue
                CheckForFlightChanges();

                //tag these for next time round
                LastGameUT = Planetarium.GetUniversalTime();
                LastGameVessel = FlightGlobals.ActiveVessel;
            }
            KACWorkerGameState.SetLastGUIStatesToCurrent();
        }
        
        private void HandleKeyStrokes()
        {
            //Look for key inputs to change settings
            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F11))
            {
                Settings.WindowVisible = !Settings.WindowVisible;
                Settings.Save();
            }

            //if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F10))
            //{
            //    WorkerObjectInstance.DebugActionTriggered(HighLogic.LoadedScene);
            //}

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

        private void CheckForFlightChanges()
        {
            if ((LastGameUT != 0) && (LastGameUT > Planetarium.GetUniversalTime()))
            {
                KACWorker.DebugLogFormatted("Time Went Backwards - Load or restart - resetting inqueue flag");
                ShouldBeInPostDrawQueue = false;
//                IsInPostDrawQueue = false;
            }
            else if (LastGameVessel == null)
            {
                KACWorker.DebugLogFormatted("Active Vessel unreadable - resetting inqueue flag");
                ShouldBeInPostDrawQueue = false;
                //                IsInPostDrawQueue = false;
            }
            else if (LastGameVessel != FlightGlobals.ActiveVessel)
            {
                KACWorker.DebugLogFormatted("Active Vessel changed - resetting inqueue flag");
                ShouldBeInPostDrawQueue = false;
                //                IsInPostDrawQueue = false;
            }
        }
        #endregion

        public void OnGUI()
        {
            //Do the GUI Stuff - basically get the workers draw stuff into the postrendering queue
            //If the two flags are different are we going in or out of the queue
            if (ShouldBeInPostDrawQueue != IsInPostDrawQueue)
            {
                if (ShouldBeInPostDrawQueue && !IsInPostDrawQueue)
                {
                    KACWorker.DebugLogFormatted("Adding DrawGUI to PostRender Queue");

                    //reset any existing pane display
                    WorkerObjectInstance.ResetPanes();
                    
                    //Add to the queue
                    RenderingManager.AddToPostDrawQueue(5, DrawGUI);
                    IsInPostDrawQueue = true;

                    //if we are adding the renderer and we are in flight then do the daily version check if required
                    if (HighLogic.LoadedScene == GameScenes.FLIGHT && Settings.DailyVersionCheck)
                        Settings.VersionCheck(false);

                }
                else
                {
                    KACWorker.DebugLogFormatted("Removing DrawGUI from PostRender Queue");
                    RenderingManager.RemoveFromPostDrawQueue(5, DrawGUI);
                    IsInPostDrawQueue = false;
                }
            }
        }

        public void BehaviourUpdate()
        {
            //Only Do any of this work if we are in FlightMode
            //if (Settings.BehaviourScenes.Contains(HighLogic.LoadedScene))
            //{
                WorkerObjectInstance.UpdateDetails();
            //}
        }

        //This is what we do every frame when the object is being drawn 
        //We dont get here unless we are in the postdraw queue
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

        public void DebugWriter()
        {
            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                WorkerObjectInstance.DebugActionTimed(HighLogic.LoadedScene);
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
        private long SecondsWarpLightIsShown = 3;

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
        }
        #endregion

        //Updates the variables that are used in the drawing - this is not on the OnGUI thread
        private Dictionary<String, KACVesselSOI> lstVessels = new Dictionary<String,KACVesselSOI>();
        
        public void UpdateDetails()
        {
            KACWorkerGameState.SetCurrentFlightStates();

            if (KACWorkerGameState.ChangedSaveGameName)
            {
                DebugLogFormatted("SaveGame - {0} - {1}", KACWorkerGameState.LastSaveGameName, KACWorkerGameState.CurrentSaveGameName);
                if (KACWorkerGameState.CurrentSaveGameName != "")
                    Settings.Load();
            }

            if (KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT)
            {
                //if vessel has changed
                if (KACWorkerGameState.ChangedVessel)
                {
                    string strVesselName = "No Vessel";
                        if (KACWorkerGameState.LastVessel!=null) strVesselName=KACWorkerGameState.LastVessel.vesselName;
                    DebugLogFormatted("Vessel Change from '{0}' to '{1}'", strVesselName, KACWorkerGameState.CurrentVessel.vesselName);
                }

                // Do we need to restore a maneuverNode after a ship jump - give it 4 secs of attempts for changes to ship
                if (manToRestore != null && KACWorkerGameState.IsFlightMode)
                {
                    manToRestoreAttempts += 1;
                    DebugLogFormatted("Attempting to restore a maneuver node-Try {0}",manToRestoreAttempts.ToString());
                    RestoreManeuverNode(manToRestore);
                    if (KACWorkerGameState.ManeuverNodeExists)
                    {
                        manToRestore = null;
                        manToRestoreAttempts = 0;
                    }
                    if (manToRestoreAttempts > 19)
                    {
                        DebugLogFormatted("20 attempts adding Node failed - giving up");
                        manToRestore = null;
                        manToRestoreAttempts = 0;
                    }
                }


                //Do we need to turn off the global warp light
                if (KACWorkerGameState.CurrentWarpInfluenceStartTime == null)
                    KACWorkerGameState.CurrentlyUnderWarpInfluence = false;
                else
                    //has it been on long enough?
                    if (KACWorkerGameState.CurrentWarpInfluenceStartTime.AddSeconds(SecondsWarpLightIsShown) < DateTime.Now)
                        KACWorkerGameState.CurrentlyUnderWarpInfluence = false;

                //Are we adding SOI Alarms
                if (Settings.AlarmAddSOIAuto)
                {
                    //Is there an SOI Point on the path - looking for next action on the path use orbit.patchEndTransition - enum of Orbit.PatchTransitionType
                    //  FINAL - fixed orbit no change
                    //  ESCAPE - leaving SOI
                    //  ENCOUNTER - entering new SOI inside current SOI
                    //  INITIAL - ???
                    //  MANEUVER - Maneuver Node
                    // orbit.UTsoi - time of next SOI change (base on above transition types - ie if type is final this time can be ignored)
                    //orbit.nextpatch gives you the next orbit and you can read the new SOI!!!

                    double timeSOIChange = 0;
                    //double timeSOIAlarm = 0;
                    if (Settings.SOITransitions.Contains(KACWorkerGameState.CurrentVessel.orbit.patchEndTransition))
                    {
                        timeSOIChange = KACWorkerGameState.CurrentVessel.orbit.UTsoi;
                        //timeSOIAlarm = timeSOIChange - Settings.AlarmAddSOIMargin;
                        strAlarmNameSOI = KACWorkerGameState.CurrentVessel.vesselName + "";
                        strAlarmMessageSOI = KACWorkerGameState.CurrentVessel.vesselName + " - Nearing SOI Change\r\n" +
                                        "     Old SOI: " + KACWorkerGameState.CurrentVessel.orbit.referenceBody.bodyName + "\r\n" +
                                        "     New SOI: " + KACWorkerGameState.CurrentVessel.orbit.nextPatch.referenceBody.bodyName;
                    }
                                

                    //is there an SOI alarm for this ship already that has not been triggered
                    KACAlarm tmpSOIAlarm =
                    Settings.Alarms.Find(delegate(KACAlarm a) {
                                            return
                                                (a.VesselID == KACWorkerGameState.CurrentVessel.id.ToString())
                                                && (a.TypeOfAlarm==KACAlarm.AlarmType.SOIChange)
                                                && (a.Triggered==false);
                    });
                    //Is there an SOI point
                    if (timeSOIChange != 0)
                    {
                        //and an existing alarm
                        if (tmpSOIAlarm != null)
                        {
                            //update the time (if more than threshold secs)
                            if (tmpSOIAlarm.Remaining.UT>Settings.AlarmAddSOIAutoThreshold)
                                tmpSOIAlarm.AlarmTime.UT = timeSOIChange;
                        }
                            //Otherwise if its in the future add a new alarm
                        else if (timeSOIChange > KACWorkerGameState.CurrentTime.UT)
                        {
                            Settings.Alarms.Add(new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(), strAlarmNameSOI, strAlarmMessageSOI, timeSOIChange, 0,
                                KACAlarm.AlarmType.SOIChange, (Settings.AlarmOnSOIChange_Action > 0), (Settings.AlarmOnSOIChange_Action > 1)));
                        }
                    }
                    else
                    {
                        //remove any existing alarm - if less than threshold - this means old alarms not touched
                        if (tmpSOIAlarm != null && (tmpSOIAlarm.Remaining.UT > Settings.AlarmAddSOIAutoThreshold))
                        {
                            Settings.Alarms.Remove(tmpSOIAlarm);
                        }
                    }

                                        
                    //Are we doing the base catchall
                    if (Settings.AlarmCatchSOIChange)
                    {
                        GlobalSOICatchAll();
                    }
                }

                //Work out how many game seconds will pass till this runs again
                double SecondsTillNextUpdate;
                double dWarpRate = TimeWarp.CurrentRate;
                SecondsTillNextUpdate = KACBehaviour.UpdateInterval * dWarpRate;

                //Loop through the alarms
                ParseAlarmsAndAffectWarpAndPause(SecondsTillNextUpdate);
            }

            KACWorkerGameState.SetLastFlightStatesToCurrent();
        }

        

        private void GlobalSOICatchAll()
        {
            foreach (Vessel tmpVessel in FlightGlobals.Vessels)
            {
                //only track vessels, not debris, EVA, etc
                //and not the current vessel
                //and no SOI alarm for it within the threshold - THIS BIT NEEDS TUNING
                if (Settings.VesselTypesForSOI.Contains(tmpVessel.vesselType) && (tmpVessel!=KACWorkerGameState.CurrentVessel) && 
                    !(Settings.Alarms.Exists(delegate(KACAlarm a) {return
                                                (a.VesselID == tmpVessel.id.ToString())
                                                && (a.TypeOfAlarm==KACAlarm.AlarmType.SOIChange)
                                                && (Math.Abs(a.Remaining.UT)<=Settings.AlarmAddSOIAutoThreshold)
                                                ;})
                                            ))
                {
                    if (lstVessels.ContainsKey(tmpVessel.id.ToString()) == false)
                    {
                        //Add new Vessels
                        DebugLogFormatted(String.Format("Adding {0}-{1}-{2}-{3}", tmpVessel.id, tmpVessel.vesselName, tmpVessel.vesselType, tmpVessel.mainBody.bodyName));
                        lstVessels.Add(tmpVessel.id.ToString(), new KACVesselSOI(tmpVessel.vesselName, tmpVessel.mainBody.bodyName));
                    }
                    else
                    {
                        //get this vessel from the memory array we are keeping and compare to its SOI
                        if (lstVessels[tmpVessel.id.ToString()].SOIName != tmpVessel.mainBody.bodyName)
                        {
                            //Set a new alarm to display now
                            KACAlarm newAlarm = new KACAlarm(FlightGlobals.ActiveVessel.id.ToString(), tmpVessel.vesselName + "- SOI Catch",
                                tmpVessel.vesselName + " Has entered a new Sphere of Influence\r\n" +
                                "     Old SOI: " + lstVessels[tmpVessel.id.ToString()].SOIName + "\r\n" +
                                "     New SOI: " + tmpVessel.mainBody.bodyName,
                                 KACWorkerGameState.CurrentTime.UT, 0, KACAlarm.AlarmType.SOIChange,
                                (Settings.AlarmOnSOIChange_Action > 0), (Settings.AlarmOnSOIChange_Action > 1));
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

        private void ParseAlarmsAndAffectWarpAndPause(double SecondsTillNextUpdate)
        {
            foreach (KACAlarm tmpAlarm in Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title))
            {
                //reset each alarms WarpInfluence flag
                if (KACWorkerGameState.CurrentWarpInfluenceStartTime == null)
                    tmpAlarm.WarpInfluence = false;
                else
                    //if the lights been on long enough
                    if (KACWorkerGameState.CurrentWarpInfluenceStartTime.AddSeconds(SecondsWarpLightIsShown) < DateTime.Now)
                        tmpAlarm.WarpInfluence = false;

                //Update Remaining interval for each alarm
                tmpAlarm.Remaining.UT = tmpAlarm.AlarmTime.UT - KACWorkerGameState.CurrentTime.UT;

                //set triggered for passed alarms so the OnGUI part can draw the window later
                if ((KACWorkerGameState.CurrentTime.UT >= tmpAlarm.AlarmTime.UT) && (tmpAlarm.Enabled) && (!tmpAlarm.Triggered))
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
                        DebugLogFormatted(string.Format("{0}-Halt Warp", tmpAlarm.Name));
                        TimeWarp.SetRate(0, true);
                    }
                }

                //if in the next two updates we would pass the alarm time then slow down the warp
                if (!tmpAlarm.Actioned && tmpAlarm.Enabled && tmpAlarm.HaltWarp)
                {
                    Double TimeNext = KACWorkerGameState.CurrentTime.UT + SecondsTillNextUpdate * 2;
                    //DebugLogFormatted(CurrentTime.UT.ToString() + "," + TimeNext.ToString());
                    if (TimeNext > tmpAlarm.AlarmTime.UT)
                    {
                        tmpAlarm.WarpInfluence = true;
                        KACWorkerGameState.CurrentlyUnderWarpInfluence = true;
                        KACWorkerGameState.CurrentWarpInfluenceStartTime = DateTime.Now;

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

        private ManeuverNode manToRestore = null;
        private Int32 manToRestoreAttempts = 0;
        public void RestoreManeuverNode(ManeuverNode newManNode)
        {
            ManeuverNode tmpNode = FlightGlobals.ActiveVessel.patchedConicSolver.AddManeuverNode(newManNode.UT);
            tmpNode.DeltaV = newManNode.DeltaV;
            tmpNode.nodeRotation = newManNode.nodeRotation;
            FlightGlobals.ActiveVessel.patchedConicSolver.UpdateFlightPlan();
        }

        /// <summary>
        /// Some Structured logging to the debug file
        /// </summary>
        /// <param name="Message"></param>
        public static void DebugLogFormatted(string Message, params string[] strParams )
        {
            Message = string.Format(Message, strParams);
            string strMessageLine = string.Format("{0},KerbalAlarmClock,{1}", DateTime.Now, Message);
            Debug.Log(strMessageLine);
        }
    }

}
