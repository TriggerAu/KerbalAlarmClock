using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;

using UnityEngine;
using KSP;
using KSPPluginFramework;

namespace KerbalAlarmClock
{
    ///// <summary>
    ///// Have to do this behaviour or some of the textures are unloaded on first entry into flight mode
    ///// </summary>
    //[KSPAddon(KSPAddon.Startup.MainMenu, false)]
    //public class KerbalAlarmClockTextureLoader : MonoBehaviour
    //{
    //    //Awake Event - when the DLL is loaded
    //    public void Awake()
    //    {
    //        KACResources.loadGUIAssets();
    //    }
    //}

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KACFlight : KerbalAlarmClock
    {
        public override string MonoName { get { return this.name; } }
        public override bool ViewAlarmsOnly { get { return false; } }
    }
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class KACSpaceCenter : KerbalAlarmClock
    {
        public override string MonoName { get { return this.name; } }
        public override bool ViewAlarmsOnly { get { return true; } }
    }
    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class KACTrackingStation : KerbalAlarmClock
    {
        public override string MonoName { get { return this.name; } }
        public override bool ViewAlarmsOnly { get { return true; } }
    }

    /// <summary>
    /// This is the behaviour object that we hook events on to for flight
    /// </summary>
    public partial class KerbalAlarmClock : MonoBehaviourExtended
    {
        //Flags for scene behaviour
        public virtual String MonoName { get; set; }
        public virtual Boolean ViewAlarmsOnly { get; set; }

        //Global Settings
        internal static Settings settings;
        public static KACSettings Settings = new KACSettings();

        //Windows

        //Controllers
        internal static AudioController audioController;


        //GameState Objects for the monobehaviour
        private Boolean IsInPostDrawQueue = false;
        private Boolean ShouldBeInPostDrawQueue = false;
        private Double LastGameUT;
        private Vessel LastGameVessel;

        //All persistant stuff is stored in the settings object
        private long SecondsWarpLightIsShown = 3;

        //Constructor to set KACWorker parent object to this and access to the settings
        public KerbalAlarmClock()
        {
            //Set the saves path
            KACUtils.SavePath = string.Format("{0}saves/{1}", Resources.PathApp, HighLogic.SaveFolder);

        }


        //Awake Event - when the DLL is loaded
        internal override void Awake()
        {
            LogFormatted("Awakening the KerbalAlarmClock-{0}", MonoName);

            //Load the Settings values from the file
            LogFormatted("Loading Settings");
            settings = new Settings("settings.cfg");
            if (!settings.Load())
                LogFormatted("Settings Load Failed");

            //old settings
            Settings.Load();

            //get the sounds and set things up
            Resources.LoadSounds();
            InitAudio();

            //Get whether the toolbar is there
            settings.BlizzyToolbarIsAvailable = KACToolbarWrapper.ToolbarManager.ToolbarAvailable;
            //if requested use that button
            if (settings.BlizzyToolbarIsAvailable && settings.UseBlizzyToolbarIfAvailable)
                btnToolbar = InitToolbarButton();


            //Set up Variables




            //Set up Windows





            //Set initial GameState
            KACGameState.LastGUIScene = HighLogic.LoadedScene;

            //Load Hohmann modelling data - if in flight mode
            if ((KACGameState.LastGUIScene == GameScenes.FLIGHT) && Settings.XferModelLoadData)
                Settings.XferModelDataLoaded = Resources.LoadModelPoints();


            //plug us in to the draw queue and start the worker
            RenderingManager.AddToPostDrawQueue(1, DrawGUI);
            StartRepeatingWorker(Settings.BehaviourChecksPerSec);

            //do the daily version check if required
            if (settings.DailyVersionCheck)
                settings.VersionCheck(false);

            APIAwake();
        }

        //Destroy Event - when the DLL is loaded
        internal override void OnDestroy()
        {
            LogFormatted("Destroying the KerbalAlarmClock-{0}", MonoName);

            //tear down events

            //tear down the renderer
            RenderingManager.RemoveFromPostDrawQueue(1, DrawGUI);

            DestroyToolbarButton(btnToolbar);

            APIDestroy();
        }

        private void InitAudio()
        {
            audioController = AddComponent<AudioController>();
            audioController.mbKAC = this;
            audioController.Init();
        }




        internal override void OnGUIOnceOnly()
        {
            //Load Image resources
            Resources.LoadTextures();

            //Set the styles
            Styles.InitStyles();

            //Init Skins
            Styles.InitSkins();

            //Set the current skin
            SkinsLibrary.SetCurrent(settings.SelectedSkin.ToString());
        }

        #region "Update Code"
        //Update Function - Happens on every frame - this is where behavioural stuff is typically done
        internal override void Update()
        {
            KACGameState.SetCurrentGUIStates();
            //if scene has changed
            if (KACGameState.ChangedGUIScene)
                LogFormatted("Scene Change from '{0}' to '{1}'", KACGameState.LastGUIScene.ToString(), KACGameState.CurrentGUIScene.ToString());

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
            KACGameState.SetLastGUIStatesToCurrent();
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
                WindowVisibleByActiveScene = !WindowVisibleByActiveScene;
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
                LogFormatted("Time Went Backwards - Load or restart - resetting inqueue flag");
                ShouldBeInPostDrawQueue = false;
                //                IsInPostDrawQueue = false;

                //Also, untrigger any alarms that we have now gone back past
                foreach (KACAlarm tmpAlarm in Settings.Alarms.Where(a => a.Triggered && (a.AlarmTime.UT > Planetarium.GetUniversalTime())))
                {
                    LogFormatted("Resetting Alarm Trigger for {0}({1})", tmpAlarm.Name, tmpAlarm.AlarmTime.UTString());
                    tmpAlarm.Triggered = false;
                    tmpAlarm.AlarmWindowID = 0;
                    tmpAlarm.AlarmWindowClosed = false;
                    tmpAlarm.Actioned = false;
                    tmpAlarm.ActionedAt = 0;
                }
            }
            else if (LastGameVessel == null)
            {
                LogFormatted("Active Vessel unreadable - resetting inqueue flag");
                ShouldBeInPostDrawQueue = false;
                //                IsInPostDrawQueue = false;
            }
            else if (LastGameVessel != FlightGlobals.ActiveVessel)
            {
                LogFormatted("Active Vessel changed - resetting inqueue flag");
                ShouldBeInPostDrawQueue = false;
                //                IsInPostDrawQueue = false;
            }
        }
        #endregion

        internal override void OnGUIEvery()
        {
            //Do the GUI Stuff - basically get the workers draw stuff into the postrendering queue
            //If the two flags are different are we going in or out of the queue
            if (ShouldBeInPostDrawQueue != IsInPostDrawQueue)
            {
                if (ShouldBeInPostDrawQueue && !IsInPostDrawQueue)
                {
                    LogFormatted("Adding DrawGUI to PostRender Queue");

                    //reset any existing pane display
                    WorkerObjectInstance.ResetPanes();

                    //Add to the queue
                    RenderingManager.AddToPostDrawQueue(5, DrawGUI);
                    IsInPostDrawQueue = true;

                }
                else
                {
                    LogFormatted("Removing DrawGUI from PostRender Queue");
                    RenderingManager.RemoveFromPostDrawQueue(5, DrawGUI);
                    IsInPostDrawQueue = false;
                }
            }
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
            if (KACGameState.CurrentGUIScene == GameScenes.FLIGHT)
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

            //If Game is paused then update Earth Alarms for list drawing
            if (WindowVisibleByActiveScene && FlightDriver.Pause)
            {
                UpdateEarthAlarms();
            }
        }
        internal override void RepeatingWorker()
        {
            KACGameState.SetCurrentFlightStates();

            if (KACGameState.CurrentGUIScene == GameScenes.FLIGHT)
            {
                //if vessel has changed
                if (KACGameState.ChangedVessel)
                {
                    String strVesselName = "No Vessel";
                    if (KACGameState.LastVessel != null) strVesselName = KACGameState.LastVessel.vesselName;
                    LogFormatted("Vessel Change from '{0}' to '{1}'", strVesselName, KACGameState.CurrentVessel.vesselName);
                }

                // Do we need to restore a maneuverNode after a ship jump - give it 4 secs of attempts for changes to ship
                if (Settings.LoadManNode != "" && KACGameState.IsFlightMode)
                {
                    List<ManeuverNode> manNodesToRestore = KACAlarm.ManNodeDeserializeList(Settings.LoadManNode);
                    manToRestoreAttempts += 1;
                    LogFormatted("Attempting to restore a maneuver node-Try {0}", manToRestoreAttempts.ToString());
                    RestoreManeuverNodeList(manNodesToRestore);
                    if (KACGameState.ManeuverNodeExists)
                    {
                        Settings.LoadManNode = "";
                        Settings.SaveLoadObjects();
                        manNodesToRestore = null;
                        manToRestoreAttempts = 0;
                    }
                    if (manToRestoreAttempts > (5 / (1/Settings.BehaviourChecksPerSec)))
                    {
                        LogFormatted("attempts adding Node failed over 5 secs - giving up");
                        Settings.LoadManNode = "";
                        Settings.SaveLoadObjects();
                        manNodesToRestore = null;
                        manToRestoreAttempts = 0;
                    }
                }

                // Do we need to restore a Target after a ship jump - give it 4 secs of attempts for changes to ship
                if (Settings.LoadVesselTarget != "" && KACGameState.IsFlightMode)
                {
                    ITargetable targetToRestore = KACAlarm.TargetDeserialize(Settings.LoadVesselTarget);
                    targetToRestoreAttempts += 1;
                    LogFormatted("Attempting to restore a Target-Try {0}", targetToRestoreAttempts.ToString());

                    if (targetToRestore is Vessel)
                        FlightGlobals.fetch.SetVesselTarget(targetToRestore as Vessel);
                    else if (targetToRestore is CelestialBody)
                        FlightGlobals.fetch.SetVesselTarget(targetToRestore as CelestialBody);

                    if (FlightGlobals.fetch.VesselTarget != null)
                    {
                        Settings.LoadVesselTarget = "";
                        Settings.SaveLoadObjects();
                        targetToRestore = null;
                        targetToRestoreAttempts = 0;
                    }
                    if (targetToRestoreAttempts > (5 / (1 / Settings.BehaviourChecksPerSec)))
                    {
                        LogFormatted("attempts adding target over 5 secs failed - giving up");
                        Settings.LoadVesselTarget = "";
                        Settings.SaveLoadObjects();
                        targetToRestore = null;
                        targetToRestoreAttempts = 0;
                    }
                }


                //Do we need to turn off the global warp light
                if (KACGameState.CurrentWarpInfluenceStartTime == null)
                    KACGameState.CurrentlyUnderWarpInfluence = false;
                else
                    //has it been on long enough?
                    if (KACGameState.CurrentWarpInfluenceStartTime.AddSeconds(SecondsWarpLightIsShown) < DateTime.Now)
                        KACGameState.CurrentlyUnderWarpInfluence = false;

                //Are we adding SOI Alarms
                if (Settings.AlarmAddSOIAuto)
                {
                    MonitorSOIOnPath();
                    //Are we doing the base catchall
                    if (Settings.AlarmCatchSOIChange)
                    {
                        GlobalSOICatchAll((1 / Settings.BehaviourChecksPerSec) * TimeWarp.CurrentRate);
                    }
                }


                //Only do these recalcs at 1x or physwarp...
                if (TimeWarp.CurrentRate == 1 || (TimeWarp.WarpMode == TimeWarp.Modes.LOW))
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
                if (intPeriodicSaveCounter > (5 / (1 / Settings.BehaviourChecksPerSec)))
                {
                    intPeriodicSaveCounter = 0;
                    Boolean blnPeriodicSave = false;
                    if (Settings.AlarmXferRecalc && Settings.Alarms.FirstOrDefault(a => a.TypeOfAlarm == KACAlarm.AlarmType.Transfer) != null)
                        blnPeriodicSave = true;
                    else if (Settings.AlarmAddSOIAuto && Settings.Alarms.FirstOrDefault(a => a.TypeOfAlarm == KACAlarm.AlarmType.SOIChangeAuto && a.VesselID == KACGameState.CurrentVessel.id.ToString()) != null)
                        blnPeriodicSave = true;
                    else if (Settings.AlarmSOIRecalc && Settings.Alarms.FirstOrDefault(a => a.TypeOfAlarm == KACAlarm.AlarmType.SOIChange && a.VesselID == KACGameState.CurrentVessel.id.ToString()) != null)
                        blnPeriodicSave = true;
                    else if (Settings.AlarmNodeRecalc && Settings.Alarms.FirstOrDefault(a => TypesToRecalc.Contains(a.TypeOfAlarm) && a.VesselID == KACGameState.CurrentVessel.id.ToString()) != null)
                        blnPeriodicSave = true;
                    else if (Settings.AlarmAddManAuto && Settings.Alarms.FirstOrDefault(a => a.TypeOfAlarm == KACAlarm.AlarmType.ManeuverAuto && a.VesselID == KACGameState.CurrentVessel.id.ToString()) != null)
                        blnPeriodicSave = true;

                    if (blnPeriodicSave)
                        Settings.SaveAlarms();
                }

            }
            //Work out how many game seconds will pass till this runs again
            double SecondsTillNextUpdate;
            double dWarpRate = TimeWarp.CurrentRate;
            SecondsTillNextUpdate = (1 / Settings.BehaviourChecksPerSec) * dWarpRate;

            //Loop through the alarms
            ParseAlarmsAndAffectWarpAndPause(SecondsTillNextUpdate);

            KACGameState.SetLastFlightStatesToCurrent();
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
}
