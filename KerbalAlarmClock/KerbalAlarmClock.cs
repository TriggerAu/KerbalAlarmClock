using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;

using UnityEngine;
using KSP;

namespace KerbalAlarmClock
{
    /// <summary>
    /// Have to do this behaviour or some of the textures are unloaded on first entry into flight mode
    /// </summary>
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class KerbalAlarmClockTextureLoader : MonoBehaviour
    {
        //Awake Event - when the DLL is loaded
        public void Awake()
        {
            KACResources.loadGUIAssets();
        }
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KACFlight : KerbalAlarmClock
    {
        public override string MonoName { get { return this.name; } }
        //public override bool ViewAlarmsOnly { get { return false; } }
    }
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class KACSpaceCenter : KerbalAlarmClock
    {
        public override string MonoName { get { return this.name; } }
        //public override bool ViewAlarmsOnly { get { return false; } }
    }
    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class KACTrackingStation : KerbalAlarmClock
    {
        public override string MonoName { get { return this.name; } }
        //public override bool ViewAlarmsOnly { get { return false; } }
    }

    /// <summary>
    /// This is the behaviour object that we hook events on to for flight
    /// </summary>
    public partial class KerbalAlarmClock : MonoBehaviour
    {
        //Global Settings
        public static KACSettings Settings = new KACSettings();
        public virtual String MonoName { get; set; }
        //public virtual Boolean ViewAlarmsOnly { get; set; }
        
        //GameState Objects for the monobehaviour
        private Boolean IsInPostDrawQueue=false ;
        private Boolean ShouldBeInPostDrawQueue = false;
        private Double LastGameUT;
        private Vessel LastGameVessel;

        //Worker and Settings objects
        public static KACWorker WorkerObjectInstance;
        public static float UpdateInterval = 0.1F;

        //Constructor to set KACWorker parent object to this and access to the settings
        public KerbalAlarmClock()
        {
            switch (KACWorkerGameState.CurrentGUIScene)
            {
                case GameScenes.SPACECENTER:
                case GameScenes.TRACKSTATION:
                    WorkerObjectInstance = new KACWorker_ViewOnly(this);
                    break;
                default:
                    WorkerObjectInstance = new KACWorker(this);
                    break;
            }
            //Set the saves path
            KACUtils.SavePath=string.Format("{0}saves/{1}",KACUtils.PathApp,HighLogic.SaveFolder);

        }

        //Awake Event - when the DLL is loaded
        public void Awake()
        {
            KACWorker.DebugLogFormatted("Awakening the KerbalAlarmClock-{0}", MonoName);
            KACWorker.DebugLogFormatted("PluginPath:{0}", KACUtils.PathPlugin);
            
            //Load Image resources
            KACResources.loadGUIAssets();

            //Load the Settings values from the file
            Settings.Load();

            //Set initial GameState
            KACWorkerGameState.LastGUIScene = HighLogic.LoadedScene;

            //Load Hohmann modelling data - if in flight mode
            //if ((KACWorkerGameState.LastGUIScene == GameScenes.FLIGHT) && Settings.XferModelLoadData)
            if (Settings.XferModelLoadData)
                Settings.XferModelDataLoaded = KACResources.LoadModelPoints();

            //Common Toolbar Code
            BlizzyToolbarIsAvailable = HookToolbar();

            if (BlizzyToolbarIsAvailable && Settings.UseBlizzyToolbarIfAvailable)
            {
                WorkerObjectInstance.btnToolbarKAC = InitToolbarButton();
            }

            //Set up the updating function - do this 5 times a sec not on every frame.
            KACWorker.DebugLogFormatted("Invoking Worker Function KerbalAlarmClock-{0}", MonoName);
            CancelInvoke();

            SetupRepeatingFunction_BehaviourUpdate(Settings.BehaviourChecksPerSec);
            
            //Reenable for during code debug
            //SetupRepeatingFunction("DebugWriter", 2F);
        }

        //public void Start()
        //{
        //    if (HighLogic.LoadedScene==GameScenes.SPACECENTER && Settings.UIBlockersEnabled)
        //    {
        //        WorkerObjectInstance.InitBlockers();
        //    }
        //}


        public void SetupRepeatingFunction_BehaviourUpdate(int ChecksPerSec)
        {
            SetupRepeatingFunction("BehaviourUpdate", ChecksPerSec);
        }
        public void SetupRepeatingFunction(String FunctionName, int ChecksPerSec)
        {
            float Interval = 1F / ChecksPerSec;
            SetupRepeatingFunction(FunctionName, Interval);
        }
        public void SetupRepeatingFunction(String FunctionName, float SecsInterval)
        {
            Debug.Log(SecsInterval);
            if (IsInvoking(FunctionName))
            {
                KACWorker.DebugLogFormatted("Cancelling repeating Behaviour({0}-{1})", FunctionName,MonoName);
                CancelInvoke(FunctionName);
            }
            KACWorker.DebugLogFormatted("Setting up repeating Behaviour({1}) every {0:0.00} Secs", SecsInterval, FunctionName);
            InvokeRepeating(FunctionName, SecsInterval, SecsInterval);
        }
 
        //Destroy Event - when the DLL is loaded
        public void OnDestroy()
        {
            KACWorker.DebugLogFormatted("Destroying the KerbalAlarmClock-{0}", MonoName);

            DestroyToolbarButton(WorkerObjectInstance.btnToolbarKAC);
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
                //switch (KACWorkerGameState.CurrentGUIScene)
                //{
                //    case GameScenes.SPACECENTER: Settings.WindowVisible_SpaceCenter = !Settings.WindowVisible_SpaceCenter; break;
                //    case GameScenes.TRACKSTATION: Settings.WindowVisible_TrackingStation = !Settings.WindowVisible_TrackingStation; break;
                //    default: Settings.WindowVisible = !Settings.WindowVisible; break;
                //}
                WorkerObjectInstance.WindowVisibleByActiveScene = !WorkerObjectInstance.WindowVisibleByActiveScene;
                Settings.Save();
            }

            //TODO:Disable this one
            //if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F8))
            //{
            //    WorkerObjectInstance.DebugActionTriggered(HighLogic.LoadedScene);
            //}

            //if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F8))
            //    Settings.Save();

            //if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F9)) 
            //    Settings.Load();

            //If we have paused the game via an alarm and the menu is not visible, then unpause so the menu will display
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

                //Also, untrigger any alarms that we have now gone back past
                foreach (KACAlarm tmpAlarm in Settings.Alarms.Where(a=>a.Triggered && (a.AlarmTime.UT>Planetarium.GetUniversalTime())))
                {
                    KACWorker.DebugLogFormatted("Resetting Alarm Trigger for {0}({1})", tmpAlarm.Name, tmpAlarm.AlarmTime.UTString());
                    tmpAlarm.Triggered = false;
                    tmpAlarm.AlarmWindowID = 0;
                    tmpAlarm.AlarmWindowClosed = false;
                    tmpAlarm.Actioned = false;
                    tmpAlarm.ActionedAt = 0;
                }
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
            //Only Do any of this work if we are in FlightMode - THIS IS NOW HANDLED INSIDE THE UPDATEDETAILS LOOK SO WE CAN DRAW ON OTHER SCREENS
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

            Boolean blnShowInterface = true;
            if (KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT)
            {
                if (Settings.HideOnPause && PauseMenu.isOpen)
                    blnShowInterface = false;
            }

            //If game has pause menu up see whether to display the interface
            if (blnShowInterface)
            {
                // look for passed alarms to display stuff
                if (WorkerObjectInstance.IconShowByActiveScene)
                    WorkerObjectInstance.TriggeredAlarms();

                //If the mainwindow is visible And no pause menu then draw it
                if (WorkerObjectInstance.WindowVisibleByActiveScene)
                {
                    WorkerObjectInstance.DrawWindows();

                }
            }

            ////Update Blocker behaviour in flight scene
            //if (KACWorkerGameState.CurrentGUIScene == GameScenes.SPACECENTER && Settings.UIBlockersRunning)
            //    WorkerObjectInstance.PositionBlockers();
            
            if (KACWorkerGameState.CurrentGUIScene == GameScenes.SPACECENTER)
            {
                WorkerObjectInstance.KSCInputLocking();
            }

            //If Game is paused then update Earth Alarms for list drawing
            if (WorkerObjectInstance.WindowVisibleByActiveScene && FlightDriver.Pause)
            {
                WorkerObjectInstance.UpdateEarthAlarms();
            }
        }

#if DEBUG
        public void DebugWriter()
        {
            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                WorkerObjectInstance.DebugActionTimed(HighLogic.LoadedScene);
            }
        }
#endif

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
        private KerbalAlarmClock parentBehaviour;
        private KACSettings Settings;


        public KACWorker(KerbalAlarmClock parent)
        {
            parentBehaviour = parent;
            Settings = KerbalAlarmClock.Settings;

            InitWorkerVariables();

            //Event for when the vessel changes (within a scene).
            KACWorkerGameState.VesselChanged += KACWorkerGameState_VesselChanged;
        }

        private void InitWorkerVariables()
        {
            _WindowAddID = rnd.Next(1000, 2000000);
            _WindowAddMessagesID = rnd.Next(1000, 2000000);
            _WindowMainID = rnd.Next(1000, 2000000);
            _WindowSettingsID = rnd.Next(1000, 2000000);
            _WindowEditID = rnd.Next(1000, 2000000);
            _WindowEarthAlarmID = rnd.Next(1000, 2000000);
            _WindowBackupFailedID = rnd.Next(1000, 2000000);
        }
        #endregion

        //Updates the variables that are used in the drawing - this is not on the OnGUI thread
        private Dictionary<String, KACVesselSOI> lstVessels = new Dictionary<String,KACVesselSOI>();
        int intPeriodicSaveCounter=0;
        public void UpdateDetails()
        {
            KACWorkerGameState.SetCurrentFlightStates();

            //NOT NEEDED NOW THAT IT ONLY LOADS IN FLIGHT MODE 
            //if (KACWorkerGameState.ChangedSaveGameName)
            //{
            //    DebugLogFormatted("SaveGame - {0} - {1}", KACWorkerGameState.LastSaveGameName, KACWorkerGameState.CurrentSaveGameName);
            //    if (KACWorkerGameState.CurrentSaveGameName != "")
            //        Settings.Load();
            //}

            if (KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT)
            {
                //if vessel has changed
                if (KACWorkerGameState.ChangedVessel)
                {
                    String strVesselName = "No Vessel";
                    if (KACWorkerGameState.LastVessel!=null) strVesselName=KACWorkerGameState.LastVessel.vesselName;
                    DebugLogFormatted("Vessel Change from '{0}' to '{1}'", strVesselName, KACWorkerGameState.CurrentVessel.vesselName);
                }

                // Do we need to restore a maneuverNode after a ship jump - give it 4 secs of attempts for changes to ship
                if (Settings.LoadManNode != "" && KACWorkerGameState.IsVesselActive)
                {
                    List<ManeuverNode> manNodesToRestore = KACAlarm.ManNodeDeserializeList(Settings.LoadManNode);
                    manToRestoreAttempts += 1;
                    DebugLogFormatted("Attempting to restore a maneuver node-Try {0}",manToRestoreAttempts.ToString());
                    FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Clear();
                    RestoreManeuverNodeList(manNodesToRestore);
                    if (KACWorkerGameState.ManeuverNodeExists)
                    {
                        Settings.LoadManNode = "";
                        Settings.SaveLoadObjects();
                        manNodesToRestore = null;
                        manToRestoreAttempts = 0;
                    }
                    if (manToRestoreAttempts > (5 / KerbalAlarmClock.UpdateInterval))
                    {
                        DebugLogFormatted("attempts adding Node failed over 5 secs - giving up");
                        Settings.LoadManNode = "";
                        Settings.SaveLoadObjects();
                        manNodesToRestore = null;
                        manToRestoreAttempts = 0;
                    }
                }

                // Do we need to restore a Target after a ship jump - give it 4 secs of attempts for changes to ship
                if (Settings.LoadVesselTarget != "" && KACWorkerGameState.IsVesselActive)
                {
                    ITargetable targetToRestore = KACAlarm.TargetDeserialize(Settings.LoadVesselTarget);
                    targetToRestoreAttempts += 1;
                    DebugLogFormatted("Attempting to restore a Target-Try {0}", targetToRestoreAttempts.ToString());

                    if (targetToRestore is Vessel)
                        FlightGlobals.fetch.SetVesselTarget(targetToRestore as Vessel);
                    else if (targetToRestore is CelestialBody)
                        FlightGlobals.fetch.SetVesselTarget(targetToRestore as CelestialBody);

                    if (FlightGlobals.fetch.VesselTarget!=null)
                    {
                        Settings.LoadVesselTarget="";
                        Settings.SaveLoadObjects();
                        targetToRestore = null;
                        targetToRestoreAttempts = 0;
                    }
                    if (targetToRestoreAttempts > (5 / KerbalAlarmClock.UpdateInterval))
                    {
                        DebugLogFormatted("attempts adding target over 5 secs failed - giving up");
                        Settings.LoadVesselTarget = "";
                        Settings.SaveLoadObjects();
                        targetToRestore = null;
                        targetToRestoreAttempts = 0;
                    }
                }

                //Are we adding SOI Alarms
                if (Settings.AlarmAddSOIAuto)
                {
                    MonitorSOIOnPath();                 
                    //Are we doing the base catchall
                    if (Settings.AlarmCatchSOIChange)
                    {
                        GlobalSOICatchAll(KerbalAlarmClock.UpdateInterval * TimeWarp.CurrentRate);
                    }
                }


                //Only do these recalcs at 1x or physwarp...
                if (TimeWarp.CurrentRate==1 || (TimeWarp.WarpMode==TimeWarp.Modes.LOW))
                {
                    if (Settings.AlarmSOIRecalc)
                    {
                        //Adjust any transfer window alarms until they hit the threshold
                        RecalcSOIAlarmTimes(false);
                    }

                    if (Settings.AlarmXferRecalc)
                    {
                        //Adjust any transfer window alarms until they hit the threshold
                        RecalcTransferAlarmTimes(false);
                    }

                    if (Settings.AlarmNodeRecalc)
                    {
                        //Adjust any Ap,Pe,AN,DNs as flight path changes
                        RecalcNodeAlarmTimes(false);
                    }
                }

                //Are we adding Man Node Alarms
                if (Settings.AlarmAddManAuto)
                {
                    MonitorManNodeOnPath();
                }


                //Periodically save the alarms list if any of the recalcs are on and the current vessel has alarms of that type
                //if its one in twenty then resave - every 5 secs
                intPeriodicSaveCounter++;
                if (intPeriodicSaveCounter > (5/KerbalAlarmClock.UpdateInterval))
                {
                    intPeriodicSaveCounter = 0;
                    Boolean blnPeriodicSave = false;
                    if (Settings.AlarmXferRecalc && Settings.Alarms.FirstOrDefault(a=>a.TypeOfAlarm== KACAlarm.AlarmType.Transfer)!=null)
                        blnPeriodicSave=true;
                    else if (Settings.AlarmAddSOIAuto && Settings.Alarms.FirstOrDefault(a => a.TypeOfAlarm == KACAlarm.AlarmType.SOIChangeAuto && a.VesselID == KACWorkerGameState.CurrentVessel.id.ToString()) != null)
                        blnPeriodicSave = true;
                    else if (Settings.AlarmSOIRecalc && Settings.Alarms.FirstOrDefault(a => a.TypeOfAlarm == KACAlarm.AlarmType.SOIChange && a.VesselID == KACWorkerGameState.CurrentVessel.id.ToString()) != null)
                        blnPeriodicSave = true;
                    else if (Settings.AlarmNodeRecalc && Settings.Alarms.FirstOrDefault(a => TypesToRecalc.Contains(a.TypeOfAlarm) && a.VesselID == KACWorkerGameState.CurrentVessel.id.ToString()) != null)
                        blnPeriodicSave = true;
                    else if (Settings.AlarmAddManAuto && Settings.Alarms.FirstOrDefault(a => a.TypeOfAlarm == KACAlarm.AlarmType.ManeuverAuto && a.VesselID == KACWorkerGameState.CurrentVessel.id.ToString()) != null)
                        blnPeriodicSave = true;

                    if (blnPeriodicSave)
                        Settings.SaveAlarms(true);
                }

            }

            //Do we need to turn off the global warp light
            if (KACWorkerGameState.CurrentWarpInfluenceStartTime == null)
                KACWorkerGameState.CurrentlyUnderWarpInfluence = false;
            else
                //has it been on long enough?
                if (KACWorkerGameState.CurrentWarpInfluenceStartTime.AddSeconds(SecondsWarpLightIsShown) < DateTime.Now)
                    KACWorkerGameState.CurrentlyUnderWarpInfluence = false;

            //Window Pos Movement Mechanic
            if (WindowPosLast.x != WindowPosByActiveScene.x || WindowPosLast.y != WindowPosByActiveScene.y)
            {
                //The window has moved;
                if (!WindowPosLastInited)
                    WindowPosLastInited = true;
                else
                {
                    WindowPosSaved = false;
                    WindowPosMoveDetectedAt = DateTime.Now;
                }
            }
            //Was it moved and has been stationary for the last second
            if (WindowPosLastInited && !WindowPosSaved && (DateTime.Now - WindowPosMoveDetectedAt).TotalSeconds > 1)
            {
                DebugLogFormatted("Saving Moved Window");
                Settings.SaveConfig();
                WindowPosSaved = true;
            }
            //Update the Last pos
            WindowPosLast.x = WindowPosByActiveScene.x;
            WindowPosLast.y = WindowPosByActiveScene.y;

            //Work out how many game seconds will pass till this runs again
            double SecondsTillNextUpdate;
            double dWarpRate = TimeWarp.CurrentRate;
            SecondsTillNextUpdate = KerbalAlarmClock.UpdateInterval * dWarpRate;

            //Loop through the alarms
            ParseAlarmsAndAffectWarpAndPause(SecondsTillNextUpdate);

            KACWorkerGameState.SetLastFlightStatesToCurrent();
        }

        public Vector3d WindowPosLast;
        public DateTime WindowPosMoveDetectedAt;
        public Boolean WindowPosSaved = true;
        public Boolean WindowPosLastInited = false;
        

        void KACWorkerGameState_VesselChanged(Vessel OldVessel, Vessel NewVessel)
        {
            if (_ShowAddPane)
            {
                //trigger the string builder to reset stuff
                AddTypeChanged();
            }
        }


        private void MonitorSOIOnPath()
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
            double timeSOIAlarm = 0;

            String strSOIAlarmName = "";
            String strSOIAlarmNotes = "";
            //double timeSOIAlarm = 0;

            if (Settings.AlarmAddSOIAuto_ExcludeEVA && KACWorkerGameState.CurrentVessel.vesselType == VesselType.EVA)
                return;

            if (Settings.SOITransitions.Contains(KACWorkerGameState.CurrentVessel.orbit.patchEndTransition))
            {
                timeSOIChange = KACWorkerGameState.CurrentVessel.orbit.UTsoi;
                //timeSOIAlarm = timeSOIChange - Settings.AlarmAddSOIMargin;
                //strOldAlarmNameSOI = KACWorkerGameState.CurrentVessel.vesselName + "";
                //strOldAlarmMessageSOI = KACWorkerGameState.CurrentVessel.vesselName + " - Nearing SOI Change\r\n" +
                //                "     Old SOI: " + KACWorkerGameState.CurrentVessel.orbit.referenceBody.bodyName + "\r\n" +
                //                "     New SOI: " + KACWorkerGameState.CurrentVessel.orbit.nextPatch.referenceBody.bodyName;
                strSOIAlarmName = KACWorkerGameState.CurrentVessel.vesselName;// + "-Leaving " + KACWorkerGameState.CurrentVessel.orbit.referenceBody.bodyName;
                strSOIAlarmNotes = KACWorkerGameState.CurrentVessel.vesselName + " - Nearing SOI Change\r\n" +
                                "     Old SOI: " + KACWorkerGameState.CurrentVessel.orbit.referenceBody.bodyName + "\r\n" +
                                "     New SOI: " + KACWorkerGameState.CurrentVessel.orbit.nextPatch.referenceBody.bodyName;
            }

            //is there an SOI alarm for this ship already that has not been triggered
            KACAlarm tmpSOIAlarm =
            Settings.Alarms.Find(delegate(KACAlarm a)
            {
                return
                    (a.VesselID == KACWorkerGameState.CurrentVessel.id.ToString())
                    && ((a.TypeOfAlarm == KACAlarm.AlarmType.SOIChangeAuto) || (a.TypeOfAlarm == KACAlarm.AlarmType.SOIChange))
                    && (a.Triggered == false);
            });

            //if theres a manual SOI alarm already then ignore it
            if ((tmpSOIAlarm != null) && tmpSOIAlarm.TypeOfAlarm == KACAlarm.AlarmType.SOIChange)
            {
                //Dont touch manually created SOI Alarms
            }
            else
            {
                //Is there an SOI point
                if (timeSOIChange != 0)
                {
                    timeSOIAlarm = timeSOIChange - Settings.AlarmAutoSOIMargin;
                    //and an existing alarm
                    if (tmpSOIAlarm != null)
                    {
                        //update the time (if more than threshold secs)
                        if ((timeSOIAlarm - KACWorkerGameState.CurrentTime.UT) > Settings.AlarmAddSOIAutoThreshold)
                        {
                            tmpSOIAlarm.AlarmTime.UT = timeSOIAlarm;
                        }
                    }
                    //Otherwise if its in the future add a new alarm
                    else if (timeSOIAlarm > KACWorkerGameState.CurrentTime.UT)
                    {
                        //Settings.Alarms.Add(new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(), strOldAlarmNameSOI, strOldAlarmMessageSOI, timeSOIAlarm, Settings.AlarmAutoSOIMargin,
                        //    KACAlarm.AlarmType.SOIChange, (Settings.AlarmOnSOIChange_Action > 0), (Settings.AlarmOnSOIChange_Action > 1)));
                        Settings.Alarms.Add(new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(), strSOIAlarmName, strSOIAlarmNotes, timeSOIAlarm, Settings.AlarmAutoSOIMargin,
                            KACAlarm.AlarmType.SOIChangeAuto, (Settings.AlarmOnSOIChange_Action > 0), (Settings.AlarmOnSOIChange_Action > 1)));
                        Settings.SaveAlarms();
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

            }
        }

        private void RecalcSOIAlarmTimes(Boolean OverrideDriftThreshold)
        {
            foreach (KACAlarm tmpAlarm in Settings.Alarms.Where(a => a.TypeOfAlarm == KACAlarm.AlarmType.SOIChange && a.VesselID == KACWorkerGameState.CurrentVessel.id.ToString()))
            {
                if (tmpAlarm.Remaining.UT > Settings.AlarmSOIRecalcThreshold)
                {
                    //do the check/update on these
                    if (Settings.SOITransitions.Contains(KACWorkerGameState.CurrentVessel.orbit.patchEndTransition))
                    {
                        double timeSOIChange = 0;
                        timeSOIChange = KACWorkerGameState.CurrentVessel.orbit.UTsoi;
                        tmpAlarm.AlarmTime.UT = KACWorkerGameState.CurrentVessel.orbit.UTsoi - tmpAlarm.AlarmMarginSecs;
                    }
                }
            }
        }

        private void RecalcTransferAlarmTimes(Boolean OverrideDriftThreshold)
        {
            foreach (KACAlarm tmpAlarm in Settings.Alarms.Where(a => a.TypeOfAlarm == KACAlarm.AlarmType.Transfer))
            {
                if (tmpAlarm.Remaining.UT > Settings.AlarmXferRecalcThreshold)
                {
                    KACXFerTarget tmpTarget = new KACXFerTarget();
                    tmpTarget.Origin = FlightGlobals.Bodies.Single(b => b.bodyName == tmpAlarm.XferOriginBodyName);
                    tmpTarget.Target = FlightGlobals.Bodies.Single(b => b.bodyName == tmpAlarm.XferTargetBodyName);

                    //DebugLogFormatted("{0}+{1}-{2}", KACWorkerGameState.CurrentTime.UT.ToString(), tmpTarget.AlignmentTime.UT.ToString(), tmpAlarm.AlarmMarginSecs.ToString());
                    //recalc the transfer spot, but dont move it if the difference is more than the threshold value
                    if (Math.Abs(KACWorkerGameState.CurrentTime.UT - tmpTarget.AlignmentTime.UT) < Settings.AlarmXferRecalcThreshold || OverrideDriftThreshold)
                        tmpAlarm.AlarmTime.UT = KACWorkerGameState.CurrentTime.UT - tmpAlarm.AlarmMarginSecs + tmpTarget.AlignmentTime.UT;
                }
            }
        }

        List<KACAlarm.AlarmType> TypesToRecalc = new List<KACAlarm.AlarmType>() {KACAlarm.AlarmType.Apoapsis,KACAlarm.AlarmType.Periapsis,
                                                                                KACAlarm.AlarmType.AscendingNode,KACAlarm.AlarmType.DescendingNode};
        private void RecalcNodeAlarmTimes(Boolean OverrideDriftThreshold)
        {
            //only do these recalcs for the current flight plan
            foreach (KACAlarm tmpAlarm in Settings.Alarms.Where(a => TypesToRecalc.Contains(a.TypeOfAlarm) && a.VesselID==KACWorkerGameState.CurrentVessel.id.ToString()))
            {
                if (tmpAlarm.Remaining.UT > Settings.AlarmNodeRecalcThreshold)
                {
                    switch (tmpAlarm.TypeOfAlarm)
	                {
                        case KACAlarm.AlarmType.Apoapsis:
                            if (KACWorkerGameState.ApPointExists &&
                                ((Math.Abs(KACWorkerGameState.CurrentVessel.orbit.timeToAp) > Settings.AlarmNodeRecalcThreshold) || OverrideDriftThreshold))
                                tmpAlarm.AlarmTime.UT = KACWorkerGameState.CurrentTime.UT - tmpAlarm.AlarmMarginSecs + KACWorkerGameState.CurrentVessel.orbit.timeToAp;
                            break;
                        case KACAlarm.AlarmType.Periapsis:
                            if (KACWorkerGameState.PePointExists &&
                                ((Math.Abs(KACWorkerGameState.CurrentVessel.orbit.timeToPe) > Settings.AlarmNodeRecalcThreshold) || OverrideDriftThreshold))
                                tmpAlarm.AlarmTime.UT = KACWorkerGameState.CurrentTime.UT - tmpAlarm.AlarmMarginSecs + KACWorkerGameState.CurrentVessel.orbit.timeToPe;
                            break;
                        case KACAlarm.AlarmType.AscendingNode:
                            Double timeToAN;
                            //Boolean blnANExists = KACUtils.CalcTimeToANorDN(KACWorkerGameState.CurrentVessel, KACUtils.ANDNNodeType.Ascending, out timeToAN);
                            Boolean blnANExists;
                            if (KACWorkerGameState.CurrentVesselTarget == null)
                            {
                                blnANExists = KACWorkerGameState.CurrentVessel.orbit.AscendingNodeEquatorialExists();
                                timeToAN = KACWorkerGameState.CurrentVessel.orbit.TimeOfAscendingNodeEquatorial(KACWorkerGameState.CurrentTime.UT) - KACWorkerGameState.CurrentTime.UT;
                            }
                            else
                            {
                                blnANExists= KACWorkerGameState.CurrentVessel.orbit.AscendingNodeExists(KACWorkerGameState.CurrentVesselTarget.GetOrbit());
                                timeToAN = KACWorkerGameState.CurrentVessel.orbit.TimeOfAscendingNode(KACWorkerGameState.CurrentVesselTarget.GetOrbit(), KACWorkerGameState.CurrentTime.UT) - KACWorkerGameState.CurrentTime.UT;
                            }

                            if (blnANExists &&
                                ((Math.Abs(timeToAN) > Settings.AlarmNodeRecalcThreshold) || OverrideDriftThreshold))
                                tmpAlarm.AlarmTime.UT = KACWorkerGameState.CurrentTime.UT - tmpAlarm.AlarmMarginSecs + timeToAN;
                            break;

                        case KACAlarm.AlarmType.DescendingNode:
                            Double timeToDN;
                            //Boolean blnDNExists = KACUtils.CalcTimeToANorDN(KACWorkerGameState.CurrentVessel, KACUtils.ANDNNodeType.Descending, out timeToDN);
                            Boolean blnDNExists;
                            if (KACWorkerGameState.CurrentVesselTarget == null)
                            {
                                blnDNExists = KACWorkerGameState.CurrentVessel.orbit.DescendingNodeEquatorialExists();
                                timeToDN = KACWorkerGameState.CurrentVessel.orbit.TimeOfDescendingNodeEquatorial(KACWorkerGameState.CurrentTime.UT - KACWorkerGameState.CurrentTime.UT);
                            }
                            else
                            {
                                blnDNExists = KACWorkerGameState.CurrentVessel.orbit.DescendingNodeExists(KACWorkerGameState.CurrentVesselTarget.GetOrbit());
                                timeToDN = KACWorkerGameState.CurrentVessel.orbit.TimeOfDescendingNode(KACWorkerGameState.CurrentVesselTarget.GetOrbit(), KACWorkerGameState.CurrentTime.UT) - KACWorkerGameState.CurrentTime.UT;
                            }

                            if (blnDNExists &&
                                ((Math.Abs(timeToDN) > Settings.AlarmNodeRecalcThreshold) || OverrideDriftThreshold))
                                tmpAlarm.AlarmTime.UT = KACWorkerGameState.CurrentTime.UT - tmpAlarm.AlarmMarginSecs + timeToDN;
                            break;
                        default:
                            break;
	                }
                }
            }
        }

        private void GlobalSOICatchAll(double SecondsTillNextUpdate)
        {
            foreach (Vessel tmpVessel in FlightGlobals.Vessels)
            {
                //only track vessels, not debris, EVA, etc
                //and not the current vessel
                //and no SOI alarm for it within the threshold - THIS BIT NEEDS TUNING
                if (Settings.VesselTypesForSOI.Contains(tmpVessel.vesselType) && (tmpVessel!=KACWorkerGameState.CurrentVessel) &&
                    (Settings.Alarms.FirstOrDefault(a => 
                        (a.VesselID == tmpVessel.id.ToString() && 
                        (a.TypeOfAlarm == KACAlarm.AlarmType.SOIChange) &&
                        (Math.Abs(a.Remaining.UT) < SecondsTillNextUpdate + Settings.AlarmAddSOIAutoThreshold)
                        )) == null)
                    )

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
                            KACAlarm newAlarm = new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(), tmpVessel.vesselName + "- SOI Catch",
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
                                DebugLogFormatted(String.Format("{0}-Pausing Game", newAlarm.Name));
                                TimeWarp.SetRate(0, true);
                                FlightDriver.SetPause(true);
                            }
                            else if (Settings.AlarmOnSOIChange_Action > 0)
                            {
                                DebugLogFormatted(String.Format("{0}-Halt Warp", newAlarm.Name));
                                TimeWarp.SetRate(0, true);
                            }

                            //reset the name String for next check
                            lstVessels[tmpVessel.id.ToString()].SOIName = tmpVessel.mainBody.bodyName;
                        }
                    }
                }
            }
        }

        private void MonitorManNodeOnPath()
        {
            //is there an alarm
            KACAlarm tmpAlarm = Settings.Alarms.FirstOrDefault(a => a.TypeOfAlarm == KACAlarm.AlarmType.ManeuverAuto && a.VesselID == KACWorkerGameState.CurrentVessel.id.ToString());

            //is there an alarm and no man node?
            if (KACWorkerGameState.ManeuverNodeExists && (KACWorkerGameState.ManeuverNodeFuture != null))
            {
                KACTime nodeAutoAlarm;
                nodeAutoAlarm = new KACTime(KACWorkerGameState.ManeuverNodeFuture.UT - Settings.AlarmAddManAutoMargin);
                
                List<ManeuverNode> manNodesToStore = KACWorkerGameState.ManeuverNodesFuture;

                String strManNodeAlarmName = KACWorkerGameState.CurrentVessel.vesselName;
                String strManNodeAlarmNotes = "Time to pay attention to\r\n    " + KACWorkerGameState.CurrentVessel.vesselName + "\r\nNearing Maneuver Node";

                //Are we updating an alarm
                if (tmpAlarm != null)
                {
                    tmpAlarm.AlarmTime.UT = nodeAutoAlarm.UT;
                    tmpAlarm.ManNodes = manNodesToStore;
                }
                else 
                {
                    //dont add an alarm if we are within the threshold period
                    if (nodeAutoAlarm.UT + Settings.AlarmAddManAutoMargin - Settings.AlarmAddManAutoThreshold > KACWorkerGameState.CurrentTime.UT)
                    {
                        //or are we setting a new one
                        Settings.Alarms.Add(new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(), strManNodeAlarmName, strManNodeAlarmNotes, nodeAutoAlarm.UT, Settings.AlarmAddManAutoMargin, KACAlarm.AlarmType.ManeuverAuto,
                            (Settings.AlarmAddManAuto_Action == (int)KACAlarm.AlarmAction.KillWarp), (Settings.AlarmAddManAuto_Action == (int)KACAlarm.AlarmAction.PauseGame), manNodesToStore));
                        Settings.Save();
                    }
                }
            }
            else if (Settings.AlarmAddManAuto_andRemove && !KACWorkerGameState.ManeuverNodeExists)
            {
                Settings.Alarms.Remove(tmpAlarm);
            }
        }

        /// <summary>
        /// Only called when game is in paused state
        /// </summary>
        public void UpdateEarthAlarms()
        {
            foreach (KACAlarm tmpAlarm in Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title).Where(a=>a.TypeOfAlarm== KACAlarm.AlarmType.EarthTime))
            {
                tmpAlarm.Remaining.UT = (EarthTimeDecode(tmpAlarm.AlarmTime.UT) - DateTime.Now).TotalSeconds;
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
                if (tmpAlarm.TypeOfAlarm != KACAlarm.AlarmType.EarthTime)
                    tmpAlarm.Remaining.UT = tmpAlarm.AlarmTime.UT - KACWorkerGameState.CurrentTime.UT;
                else
                    tmpAlarm.Remaining.UT = (EarthTimeDecode(tmpAlarm.AlarmTime.UT) - DateTime.Now).TotalSeconds;
                
                //set triggered for passed alarms so the OnGUI part can draw the window later
                //if ((KACWorkerGameState.CurrentTime.UT >= tmpAlarm.AlarmTime.UT) && (tmpAlarm.Enabled) && (!tmpAlarm.Triggered))
                if ((tmpAlarm.Remaining.UT<=0) && (tmpAlarm.Enabled) && (!tmpAlarm.Triggered))
                {
                    if (tmpAlarm.ActionedAt > 0)
                    {
                        KACWorker.DebugLogFormatted("Suppressing Alarm due to Actioned At being set:{0}", tmpAlarm.Name);
                        tmpAlarm.Triggered = true;
                        tmpAlarm.Actioned = true;
                        tmpAlarm.AlarmWindowClosed = true;
                    }
                    else
                    {

                        DebugLogFormatted("Triggering Alarm - " + tmpAlarm.Name);
                        tmpAlarm.Triggered = true;

                        //If we are simply past the time make sure we halt the warp
                        //only do this in flight mode
                        //if (!parentBehaviour.ViewAlarmsOnly)
                        //{
                            if (tmpAlarm.PauseGame)
                            {
                                DebugLogFormatted(String.Format("{0}-Pausing Game", tmpAlarm.Name));
                                TimeWarp.SetRate(0, true);
                                FlightDriver.SetPause(true);
                            }
                            else if (tmpAlarm.HaltWarp)
                            {
                                if (!FlightDriver.Pause)
                                {
                                    DebugLogFormatted(String.Format("{0}-Halt Warp", tmpAlarm.Name));
                                    TimeWarp.SetRate(0, true);
                                }
                                else
                                {
                                    DebugLogFormatted(String.Format("{0}-Game paused, skipping Halt Warp", tmpAlarm.Name));
                                }
                            }
                        //}
                    }
                }


                //skip this if we aren't in flight mode
                //if (!parentBehaviour.ViewAlarmsOnly)
                //{
                    //if in the next two updates we would pass the alarm time then slow down the warp
                    if (!tmpAlarm.Actioned && tmpAlarm.Enabled && (tmpAlarm.HaltWarp || tmpAlarm.PauseGame))
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
                //}
            }
        }

        private Int32 targetToRestoreAttempts = 0;
        private Int32 manToRestoreAttempts = 0;

        public void RestoreManeuverNodeList(List<ManeuverNode> newManNodes)
        {
            
            foreach (ManeuverNode tmpMNode in newManNodes)
            {
                RestoreManeuverNode(tmpMNode);
            }
        }

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
        public static void DebugLogFormatted(String Message, params object[] strParams )
        {
            Message = String.Format(Message, strParams);
            String strMessageLine = String.Format("{0},KerbalAlarmClock,{1}", DateTime.Now, Message);
            Debug.Log(strMessageLine);
        }
    }

//#if DEBUG
//    //This will kick us into the save called default and set the first vessel active
//    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
//    public class Debug_AutoLoadPersistentSaveOnStartup : MonoBehaviour
//    {
//        //use this variable for first run to avoid the issue with when this is true and multiple addons use it
//        public static bool first = true;
//        public void Start()
//        {
//            //only do it on the first entry to the menu
//            if (first)
//            {
//                first = false;
//                HighLogic.SaveFolder = "default";
//                Game game = GamePersistence.LoadGame("persistent", HighLogic.SaveFolder, true, false);

//                if (game != null && game.flightState != null && game.compatible)
//                {
//                    Int32 FirstVessel;
//                    Boolean blnFoundVessel = false;
//                    for (FirstVessel = 0; FirstVessel < game.flightState.protoVessels.Count; FirstVessel++)
//                    {
//                        if (game.flightState.protoVessels[FirstVessel].vesselType != VesselType.SpaceObject &&
//                            game.flightState.protoVessels[FirstVessel].vesselType != VesselType.Unknown)
//                        {
//                            blnFoundVessel = true;
//                            break;
//                        }
//                    }
//                    if (!blnFoundVessel)
//                        FirstVessel = 0;
//                    FlightDriver.StartAndFocusVessel(game, FirstVessel);
//                }

//                //CheatOptions.InfiniteFuel = true;
//            }
//        }
//    }
//#endif
    //[KSPAddon(KSPAddon.Startup.EveryScene, false)]
    //class TriggerUIButtonTest : MonoBehaviour
    //{
    //    public static UIButton blocker {get;set;}

    //    public static void SetPosition(float a, float b, float c){
    //        blocker.transform.position = new Vector3(a, b, c);

    //    }
    //    void Start()
    //    {
    //        var uiCamera = Camera.allCameras.ToList().Find(c => c.name == "UI camera");
    //        KACWorker.DebugLogFormatted("A");

    //        Vector3 v = uiCamera.ScreenToWorldPoint(new Vector3(0f, Screen.height, 0f));
    //        KACWorker.DebugLogFormatted(v.ToString());

    //        KACWorker.DebugLogFormatted("AA");
    //        blocker = UIButton.Create("ButtonTest.Blocker", v);
    //        blocker.SetAnchor(SpriteRoot.ANCHOR_METHOD.UPPER_LEFT);
    //        blocker.renderCamera = uiCamera;
    //        blocker.gameObject.layer = LayerMask.NameToLayer("EzGUI_UI");

    //        KACWorker.DebugLogFormatted("AAA");
    //        var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
    //        texture.SetPixels(new Color[] { new Color(0f, 0f, 1f, 0.1f) }); // nice peaceful blue for debug purposes
    //        texture.Apply();

    //        KACWorker.DebugLogFormatted("AAAA");
    //        blocker.Setup(200, 200, new Material(Shader.Find("Sprite/Vertex Colored")) { mainTexture = texture });


    //        KACWorker.DebugLogFormatted("AAAAA");
    //        // just cover the screen with it
    //        var buttonPosition = uiCamera.ScreenToWorldPoint(new Vector3(0f, Screen.height, 0f));

    //        KACWorker.DebugLogFormatted("AAAAAA");
    //        buttonPosition.z = 0f;
    //        blocker.transform.position = buttonPosition;

    //        KACWorker.DebugLogFormatted("AAAAAAA");
    //        blocker.AddValueChangedDelegate(delegate(IUIObject obj) { ScreenMessages.PostScreenMessage("You clicked the background button", 3f); });
    //    }
    //}
}
