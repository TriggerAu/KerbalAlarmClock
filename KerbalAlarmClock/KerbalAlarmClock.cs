using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;

using UnityEngine;
using KSP;
using KSPPluginFramework;
using Contracts;

using KACToolbarWrapper;
using KAC_KERWrapper;
using KAC_VOIDWrapper;

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
	
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class KACEditor : KerbalAlarmClock
	{
		public override string MonoName { get { return this.name; } }
		//public override bool ViewAlarmsOnly { get { return false; } }
	}

	/// <summary>
	/// This is the behaviour object that we hook events on to for flight
	/// </summary>
	public partial class KerbalAlarmClock : MonoBehaviourExtended
	{
		//Global Settings
		//public static KACSettings Settings = new KACSettings();
		internal static Settings settings;
		public static KACAlarmList alarms = new KACAlarmList();
		public static List<KACAlarm> alarmsDisplayed = new KACAlarmList();
		public virtual String MonoName { get; set; }
		//public virtual Boolean ViewAlarmsOnly { get; set; }
		
		//GameState Objects for the monobehaviour
		private Boolean IsInPostDrawQueue=false ;
		private Boolean ShouldBeInPostDrawQueue = false;
		private Double LastGameUT;
		private Vessel LastGameVessel;

		//Worker and Settings objects
		public static float UpdateInterval = 0.1F;


		internal static AudioController audioController;

		internal AngleRenderPhase PhaseAngle;
		internal AngleRenderEject EjectAngle;
		internal Boolean blnShowPhaseAngle;
		internal Boolean blnShowEjectAngle;

		internal static List<GameScenes> lstScenesForAngles = new List<GameScenes>() { GameScenes.TRACKSTATION, GameScenes.FLIGHT };

		//Constructor to set KACWorker parent object to this and access to the settings
		public KerbalAlarmClock()
		{
			//switch (KACWorkerGameState.CurrentGUIScene)
			//{
			//    case GameScenes.SPACECENTER:
			//    case GameScenes.TRACKSTATION:
			//        = new KACWorker_ViewOnly(this);
			//        break;
			//    default:
			//        = new KACWorker(this);
			//        break;
			//}
			////Set the saves path
			KACUtils.SavePath=string.Format("{0}saves/{1}",KACUtils.PathApp,HighLogic.SaveFolder);

		}

		//Awake Event - when the DLL is loaded
		internal override void OnAwake()
		{
			LogFormatted("Awakening the KerbalAlarmClock-{0}", MonoName);

			InitVariables();

			//Event for when the vessel changes (within a scene).
			KACWorkerGameState.VesselChanged += KACWorkerGameState_VesselChanged;

			//Load the Settings values from the file
			//Settings.Load();
			LogFormatted("Loading Settings");
			settings = new Settings("PluginData/settings.cfg");
			Boolean blnSettingsLoaded = settings.Load();
			if (!blnSettingsLoaded) {
				settings = new Settings("settings.cfg");
				blnSettingsLoaded = settings.Load();
				if (blnSettingsLoaded) {
					settings.FilePath = "PluginData/settings.cfg";
					System.IO.File.Move(KACUtils.PathPlugin + "/settings.cfg", KACUtils.PathPlugin + "/PluginData/settings.cfg");
				}
			}

			if (!blnSettingsLoaded) {
				settings.FilePath = "PluginData/settings.cfg";
				LogFormatted("Settings Load Failed");
			} else {
				if (!settings.TimeFormatConverted)
				{
					settings.TimeFormatConverted = true;
					switch (settings.TimeFormat)
					{
						case OldPrintTimeFormat.TimeAsUT: settings.DateTimeFormat = DateStringFormatsEnum.TimeAsUT; break;
						case OldPrintTimeFormat.KSPString: settings.DateTimeFormat = DateStringFormatsEnum.KSPFormatWithSecs; break;
						case OldPrintTimeFormat.DateTimeString: settings.DateTimeFormat = DateStringFormatsEnum.DateTimeFormat; break;
						default: settings.DateTimeFormat = DateStringFormatsEnum.KSPFormatWithSecs; break;
					}
					settings.Save();
				}

			}

			//Set up Default Sounds
			settings.VerifySoundsList();

			if (settings.SelectedCalendar == CalendarTypeEnum.Earth) {
				KSPDateStructure.SetEarthCalendar(settings.EarthEpoch);
			}

            KSPDateStructure.UseStockDateFormatters = settings.UseStockDateFormatters;

            //Set initial GameState
            KACWorkerGameState.LastGUIScene = HighLogic.LoadedScene;

			//Load Hohmann modelling data - if in flight mode
			//if ((KACWorkerGameState.LastGUIScene == GameScenes.FLIGHT) && settings.XferModelLoadData)
			if (settings.XferModelLoadData)
					settings.XferModelDataLoaded = KACResources.LoadModelPoints();

			//get the sounds and set things up
			KACResources.LoadSounds();
			this.ConfigSoundsDDLs();
			InitAudio();

			//Get whether the toolbar is there
			settings.BlizzyToolbarIsAvailable = ToolbarManager.ToolbarAvailable;

			//setup the Toolbar button if necessary
			if (settings.ButtonStyleToDisplay == Settings.ButtonStyleEnum.Toolbar)
			{
				btnToolbarKAC = InitToolbarButton();
			}
			GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
			GameEvents.onGUIApplicationLauncherDestroyed.Add(DestroyAppLauncherButton);
			GameEvents.onGameSceneLoadRequested.Add(OnGameSceneLoadRequestedForAppLauncher);
			GameEvents.Contract.onContractsLoaded.Add(ContractsReady);

            GameEvents.onGUIAdministrationFacilitySpawn.Add(EnterKSCFacility);
            GameEvents.onGUIAstronautComplexSpawn.Add(EnterKSCFacility);
            GameEvents.onGUIMissionControlSpawn.Add(EnterKSCFacility);
            GameEvents.onGUIRnDComplexSpawn.Add(EnterKSCFacility);
            GameEvents.onGUIAdministrationFacilityDespawn.Add(LeaveKSCFacility);
            GameEvents.onGUIAstronautComplexDespawn.Add(LeaveKSCFacility);
            GameEvents.onGUIMissionControlDespawn.Add(LeaveKSCFacility);
            GameEvents.onGUIRnDComplexDespawn.Add(LeaveKSCFacility);

            // Need this one to handle the hideUI cancelling via pause menu
            GameEvents.onShowUI.Add(OnShowUI);
            GameEvents.onHideUI.Add(OnHideUI);

            GameEvents.onUIScaleChange.Add(OnUIScaleChange);
            if (settings.UIScaleOverride)
            {
                guiScale = new Vector2(settings.UIScaleValue, settings.UIScaleValue);
            }
            else
            {
                guiScale = new Vector2(GameSettings.UI_SCALE, GameSettings.UI_SCALE);
            }

            blnFilterToVessel = false;
			if (HighLogic.LoadedScene == GameScenes.TRACKSTATION ||
				HighLogic.LoadedScene == GameScenes.FLIGHT)
				blnShowFilterToVessel = true;

			//Set up the updating function - do this 5 times a sec not on every frame.
			StartRepeatingWorker(settings.BehaviourChecksPerSec);

			InitDropDowns();

			winAlarmImport.KAC = this;
			winAlarmImport.Visible = false;
			winAlarmImport.InitWindow();

			winConfirmAlarmDelete.Visible = false;
			winConfirmAlarmDelete.InitWindow();

			WarpTransitionCalculator.CalcWarpRateTransitions();

			//Hook the Angle renderers
			if (lstScenesForAngles.Contains(HighLogic.LoadedScene))
			{
				PhaseAngle = MapView.MapCamera.gameObject.AddComponent<AngleRenderPhase>();
				EjectAngle = MapView.MapCamera.gameObject.AddComponent<AngleRenderEject>();
			}

			APIAwake();
		}

        private void InitAudio()
		{
			audioController = AddComponent<AudioController>();
			audioController.mbKAC = this;
			audioController.Init();
		}

		internal override void Start()
		{
			LogFormatted_DebugOnly("Start function");
			LogFormatted_DebugOnly("Alarms Length:{0}", alarms.Count);
			
			LogFormatted("Searching for RSS");
			AssemblyLoader.loadedAssemblies.TypeOperation(t =>
				{
					if (t.FullName == ("RealSolarSystem.RSSWatchDog"))
					{
						settings.RSSActive = true;
						if (!settings.RSSShowCalendarToggled)
						{
							settings.ShowCalendarToggle = true;
							settings.RSSShowCalendarToggled = true;
						}
					}
				});

			//Init the KER Integration
			LogFormatted("Searching for KER");
			KERWrapper.InitKERWrapper();
			if (KERWrapper.APIReady)
			{
				LogFormatted("Successfully Hooked KER");

			}
			//Init the VOID Integration
			LogFormatted("Searching for VOID");
			VOIDWrapper.InitVOIDWrapper();
			if (VOIDWrapper.APIReady)
			{
				LogFormatted("Successfully Hooked VOID");

			}

			RemoveInputLock();

			if (WindowVisibleByActiveScene && settings.ButtonStyleToDisplay==Settings.ButtonStyleEnum.Launcher)
			{
				AppLauncherToBeSetTrue = true;
				AppLauncherToBeSetTrueAttemptDate = DateTime.Now;
			}
		}

		//Destroy Event - when the DLL is loaded
		internal override void OnDestroy()
		{
			LogFormatted("Destroying the KerbalAlarmClock-{0}", MonoName);

			//Hook the App Launcher
			GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
			GameEvents.onGUIApplicationLauncherDestroyed.Remove(DestroyAppLauncherButton);
			GameEvents.onGameSceneLoadRequested.Remove(OnGameSceneLoadRequestedForAppLauncher);
			GameEvents.Contract.onContractsLoaded.Remove(ContractsReady);

            GameEvents.onGUIAdministrationFacilitySpawn.Remove(EnterKSCFacility);
            GameEvents.onGUIAstronautComplexSpawn.Remove(EnterKSCFacility);
            GameEvents.onGUIMissionControlSpawn.Remove(EnterKSCFacility);
            GameEvents.onGUIRnDComplexSpawn.Remove(EnterKSCFacility);
            GameEvents.onGUIAdministrationFacilityDespawn.Remove(LeaveKSCFacility);
            GameEvents.onGUIAstronautComplexDespawn.Remove(LeaveKSCFacility);
            GameEvents.onGUIMissionControlDespawn.Remove(LeaveKSCFacility);
            GameEvents.onGUIRnDComplexDespawn.Remove(LeaveKSCFacility);

            // Need this one to handle the hideUI cancelling via pause menu
            GameEvents.onShowUI.Remove(OnShowUI);
            GameEvents.onHideUI.Remove(OnHideUI);

            GameEvents.onUIScaleChange.Remove(OnUIScaleChange);

            Destroy(PhaseAngle);
			Destroy(EjectAngle);

			DestroyDropDowns();

			DestroyToolbarButton(btnToolbarKAC);

			DestroyAppLauncherButton();

			APIDestroy();
		}

		Boolean blnContractsSystemReady = false;
		void ContractsReady()
		{
			LogFormatted("Contracts System Ready");
			//update the list
			UpdateContractDetails();

			//set the flag to say we can start processing contracts
			blnContractsSystemReady = true;
		}

        //Hide the KAC when in SC overlay scenes
        private void EnterKSCFacility() { inAdminFacility = true; }
        private void LeaveKSCFacility() { inAdminFacility = false; }

        #region "Update Code"
        //Update Function - Happens on every frame - this is where behavioural stuff is typically done
        internal override void Update()
		{
			KACWorkerGameState.SetCurrentGUIStates();
			//if scene has changed
			if (KACWorkerGameState.ChangedGUIScene)
				LogFormatted("Scene Change from '{0}' to '{1}'", KACWorkerGameState.LastGUIScene.ToString(), KACWorkerGameState.CurrentGUIScene.ToString());

			HandleKeyStrokes();

            //Work out if we should be in the gui queue
            ShouldBeInPostDrawQueue = settings.DrawScenes.Contains(HighLogic.LoadedScene);
            
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
			if (!settings.F11KeystrokeDisabled)
			{
				if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F11))
				{
					//switch (KACWorkerGameState.CurrentGUIScene)
					//{
					//    case GameScenes.SPACECENTER: Settings.WindowVisible_SpaceCenter = !Settings.WindowVisible_SpaceCenter; break;
					//    case GameScenes.TRACKSTATION: Settings.WindowVisible_TrackingStation = !Settings.WindowVisible_TrackingStation; break;
					//    default: Settings.WindowVisible = !Settings.WindowVisible; break;
					//}
					WindowVisibleByActiveScene = !WindowVisibleByActiveScene;
					settings.Save();
				}
			}

			if (settings.KillWarpOnThrottleCutOffKeystroke)
			{
				if (Input.GetKeyDown(GameSettings.THROTTLE_CUTOFF.primary.code) || Input.GetKeyDown(GameSettings.THROTTLE_CUTOFF.secondary.code))
				{
                    //Make sure we cancel autowarp if its engaged
				    if (TimeWarp.fetch != null)
				    {
				        TimeWarp.fetch.CancelAutoWarp();
				        TimeWarp.SetRate(0, false);
				    }
				}
			}

			//TODO:Disable this one
			//if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F8))
			//{
			//    DebugActionTriggered(HighLogic.LoadedScene);
			//}

			//if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F8))
			//    Settings.Save();

			//if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F9)) 
			//    Settings.Load();

			//If we have paused the game via an alarm and the menu is not visible, then unpause so the menu will display
			try {
				if (Input.GetKey(KeyCode.Escape) && !PauseMenu.isOpen && FlightDriver.Pause)
				{
					FlightDriver.SetPause(false);
				}
			} catch (Exception) { }//LogFormatted("PauseMenu Object Not ready");
		}

		private void CheckForFlightChanges()
		{
			if ((LastGameUT != 0) && (LastGameUT > Planetarium.GetUniversalTime()))
			{
				LogFormatted("Time Went Backwards - Load or restart - resetting inqueue flag");
				ShouldBeInPostDrawQueue = false;
//                IsInPostDrawQueue = false;

				//Also, untrigger any alarms that we have now gone back past
				foreach (KACAlarm tmpAlarm in alarms.Where(a=>a.Triggered && (a.AlarmTime.UT>Planetarium.GetUniversalTime())))
				{
					LogFormatted("Resetting Alarm Trigger for {0}({1})", tmpAlarm.Name, tmpAlarm.AlarmTime.ToStringStandard(DateStringFormatsEnum.TimeAsUT));
					tmpAlarm.Triggered = false;
					tmpAlarm.AlarmWindowID = 0;
					tmpAlarm.AlarmWindowClosed = false;
					tmpAlarm.Actioned = false;
					//tmpAlarm.ActionedAt = 0;
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

        private void OnShowUI()
        {
            LogFormatted_DebugOnly("OnShowUI");
            GUIVisible = true;
        }
        private void OnHideUI()
        {
            LogFormatted_DebugOnly("OnHideUI");
            GUIVisible = false;
        }

        private Boolean GUIVisible = true;
        private Boolean inAdminFacility = false;

		//public void OnGUI()
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
					ResetPanes();
					
					IsInPostDrawQueue = true;

					//if we are adding the renderer and we are in flight then do the daily version check if required
					if (HighLogic.LoadedScene == GameScenes.FLIGHT && settings.DailyVersionCheck)
						settings.VersionCheck(this, false);

				}
				else
				{
					LogFormatted("Removing DrawGUI from PostRender Queue");
					IsInPostDrawQueue = false;
				}
			}

			if (GUIVisible && !inAdminFacility && !(HighLogic.LoadedScene == GameScenes.FLIGHT && KACWorkerGameState.PauseMenuOpen && settings.HideOnPause))
			{
				DrawGUI();
			}

            OnGUIMouseEvents();
		}

		private Int32 WarpRateWorkerCounter = 0;
		private Int32 WarpRateWorkerInitialPeriodCounter = 0;

        internal override void RepeatingWorker()
		{
			if (AppLauncherToBeSetTrue)
				SetAppButtonToTrue();

            if(vesselToJumpTo != null) {
                FlightGlobals.SetActiveVessel(vesselToJumpTo);
                vesselToJumpTo = null;
                return;
            }

            UpdateDetails();

			//Contract stuff
			if (Contracts.ContractSystem.Instance)
			{
				UpdateContractDetails();
			}

			//if we are using the transitions then periodically check the rates in case someone changed em
			if (!settings.WarpTransitions_Instant) {
				WarpRateWorkerCounter++;
				if (WarpRateWorkerCounter > settings.WarpTransitions_UpdateSecs / UpdateInterval) {
					WarpRateWorkerCounter = 0;
					WarpTransitionCalculator.CheckForTransitionChanges();
				}

				//Check more frequently for first period
				if (WarpRateWorkerInitialPeriodCounter < settings.WarpTransitions_UpdateSecs / UpdateInterval)
				{
					WarpRateWorkerInitialPeriodCounter++;
					WarpTransitionCalculator.CheckForTransitionChanges();
				}
			}
		}

		private void UpdateContractDetails()
		{
            //lstContracts = Contracts.ContractSystem.Instance.Contracts.Where(c => c.DateNext() > 0).OrderBy(c => c.DateNext()).ToList();

            // 0 GC Usage version below
            if (lstContracts == null)
                lstContracts = new List<Contract>();
            lstContracts.Clear();

            for (int i = 0,iContracts = ContractSystem.Instance.Contracts.Count; i < iContracts; i++)
            {
                if(ContractSystem.Instance.Contracts[i].DateNext() > 0)
                {
                    lstContracts.Add(ContractSystem.Instance.Contracts[i]);
                }
            }
            lstContracts.Sort(delegate (Contract a, Contract b) { return a.DateNext().CompareTo(b.DateNext()); });
		}

		internal override void OnGUIOnceOnly()
		{
			//Load Image resources
			KACResources.loadGUIAssets();

			KACResources.InitSkins();
			//Hook the App Launcher

			InitDDLStyles();
			
			//Called by SetSkin
			//KACResources.SetStyles();

		}

        internal Vector2 guiScale = Vector2.one;

        public void OnUIScaleChange()
        {
            guiScale.x = settings.UIScaleOverride ? settings.UIScaleValue : GameSettings.UI_SCALE;
            guiScale.y = settings.UIScaleOverride ? settings.UIScaleValue : GameSettings.UI_SCALE;
        }
        
		//This is what we do every frame when the object is being drawn 
		//We dont get here unless we are in the postdraw queue
		public void DrawGUI()
		{
			GUI.skin = KACResources.CurrentSkin;

			//Draw the icon that should be there all the time
			DrawIcons();

			// look for passed alarms to display stuff
			if (IconShowByActiveScene)
				TriggeredAlarms();

			//If the mainwindow is visible And no pause menu then draw it
			if (WindowVisibleByActiveScene)
			{
				GUIUtility.ScaleAroundPivot(guiScale, Vector2.zero);
				DrawWindowsPre();
				DrawWindows();
				DrawWindowsPost();
			}

			if (settings.WarpToEnabled)
			{
				DrawNodeButtons();
			}

			//Do the stuff to lock inputs of people have that turned on
			ControlInputLocks();

			//Now do the stuff to close the quick alarms window if you click off it
			if (_ShowQuickAdd && Event.current.type == EventType.MouseDown && !_WindowQuickAddRect.Contains(Event.current.mousePosition) && !WindowPosByActiveScene.Contains(Event.current.mousePosition))
				_ShowQuickAdd = false;

			//If Game is paused then update Earth Alarms for list drawing
			if (WindowVisibleByActiveScene && FlightDriver.Pause)
			{
				UpdateEarthAlarms();
			}
		}


		private Boolean drawingTrackStationButtons=false;
		private DateTime drawingTrackStationButtonsAt = DateTime.Now;
		private void DrawNodeButtons()
		{

			if (MapView.MapIsEnabled && KACWorkerGameState.CurrentVessel != null && !KACWorkerGameState.CurrentVessel.LandedOrSplashed)
			{
				//Dont draw these if there are any gizmos visible
				if (settings.WarpToHideWhenManGizmoShown && KACWorkerGameState.ManeuverNodesAll_GizmoShown)
					return;

				//Check if the focus just went off so the buttons still work
				if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION && KACWorkerGameState.CurrentVessel.orbitRenderer.isFocused) {
					drawingTrackStationButtons = true;
					drawingTrackStationButtonsAt = DateTime.Now;
				} else if(drawingTrackStationButtons) {
					if (drawingTrackStationButtonsAt.AddMilliseconds(settings.WarpToTSIconDelayMSecs) < DateTime.Now)
						drawingTrackStationButtons = false;
				}

				if (KACWorkerGameState.CurrentGUIScene != GameScenes.TRACKSTATION || drawingTrackStationButtons)
				DrawNodeWarpButton(KACWorkerGameState.ApPointExists,
					Planetarium.GetUniversalTime() + KACWorkerGameState.CurrentVessel.orbit.timeToAp,
					KACAlarm.AlarmTypeEnum.Apoapsis,
					"Ap",
					settings.WarpToAddMarginAp,
					settings.AlarmAddNodeQuickMargin
					);

				if (KACWorkerGameState.CurrentGUIScene != GameScenes.TRACKSTATION || drawingTrackStationButtons)
					DrawNodeWarpButton(KACWorkerGameState.PePointExists,
					Planetarium.GetUniversalTime() + KACWorkerGameState.CurrentVessel.orbit.timeToPe,
					KACAlarm.AlarmTypeEnum.Periapsis,
					"Pe", 
					settings.WarpToAddMarginPe,
					settings.AlarmAddNodeQuickMargin
					);


				if (KACWorkerGameState.CurrentGUIScene != GameScenes.TRACKSTATION || drawingTrackStationButtons)
					DrawNodeWarpButton(KACWorkerGameState.SOIPointExists,
					KACWorkerGameState.CurrentVessel.orbit.UTsoi,
					KACAlarm.AlarmTypeEnum.SOIChange,
					"SOI",
					settings.WarpToAddMarginSOI,
					settings.AlarmAddSOIQuickMargin
					);

				if (KACWorkerGameState.CurrentGUIScene != GameScenes.TRACKSTATION && KACWorkerGameState.ManeuverNodeExists &&
						KACWorkerGameState.ManeuverNodeFuture != null && KACWorkerGameState.ManeuverNodeFuture.attachedGizmo == null)
				{

					DrawNodeWarpButton(true,
						KACWorkerGameState.ManeuverNodeFuture.UT,
						KACAlarm.AlarmTypeEnum.Maneuver,
						"ManNode",
						settings.WarpToAddMarginManNode,
						settings.AlarmAddManQuickMargin + GetBurnMarginSecs(settings.DefaultKERMargin)
						);
				}
				if (KACWorkerGameState.CurrentVesselTarget != null && !KACWorkerGameState.ManeuverNodeExists && KACWorkerGameState.CurrentVesselTarget.GetOrbit()!=null)
				{
					if (KACWorkerGameState.CurrentVessel.orbit.AscendingNodeExists(KACWorkerGameState.CurrentVesselTarget.GetOrbit()))
					{
						Double tAN = KACWorkerGameState.CurrentVessel.orbit.TimeOfAscendingNode(KACWorkerGameState.CurrentVesselTarget.GetOrbit(), Planetarium.GetUniversalTime());
						if (tAN < KACWorkerGameState.CurrentVessel.orbit.EndUT)
						{
							DrawNodeWarpButton(true, tAN, KACAlarm.AlarmTypeEnum.AscendingNode, "AN",
								settings.WarpToAddMarginAN,
								settings.AlarmAddNodeQuickMargin
								);
						}
					}
					if (KACWorkerGameState.CurrentVessel.orbit.DescendingNodeExists(KACWorkerGameState.CurrentVesselTarget.GetOrbit()))
					{
						Double tDN = KACWorkerGameState.CurrentVessel.orbit.TimeOfDescendingNode(KACWorkerGameState.CurrentVesselTarget.GetOrbit(), Planetarium.GetUniversalTime());
						if (tDN < KACWorkerGameState.CurrentVessel.orbit.EndUT)
						{
							DrawNodeWarpButton(true, tDN, KACAlarm.AlarmTypeEnum.DescendingNode, "DN",
								settings.WarpToAddMarginDN,
								settings.AlarmAddNodeQuickMargin
								);
						}
					}
				}

				//if we had started a confirmation and the mouse leaves the button then turn off step 1
				if(WarpToArmed){
					Vector3 VectMouseflipped  = Input.mousePosition;
					VectMouseflipped.y = Screen.height - VectMouseflipped.y;
					if (!WarpToArmedButtonRect.Contains(VectMouseflipped)){
						WarpToArmed = false;
						LogFormatted_DebugOnly("Mouse position has Left WarpTo Button Rect");
					}
				}
			}
		}


		Boolean WarpToArmed = false;
		Rect WarpToArmedButtonRect;

		//Draw a single button near the correct node
		private Boolean DrawNodeWarpButton(Boolean Exists, Double UT,KACAlarm.AlarmTypeEnum aType, String NodeName, Boolean WithMargin, Double MarginSecs)
		{
			if (Exists)
			{
				//set the style basics
				GUIStyle styleWarpToButton = new GUIStyle();
				styleWarpToButton.fixedWidth = 20;
				styleWarpToButton.fixedHeight = 12;

				//set the default offset of the buttons top left from the node point
				// - these move around depending on the size of the node icon
				Int32 xOffset = 10;
				Int32 yOffset = -20;

				//set the specific normal and hover textures and any custom offsets
				switch (aType)
				{
					case KACAlarm.AlarmTypeEnum.Maneuver:
					case KACAlarm.AlarmTypeEnum.ManeuverAuto:
						if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION)
						{
							styleWarpToButton.normal.background = KACResources.iconWarpToTSManNode;
							if (!settings.WarpToRequiresConfirm || WarpToArmed)
								styleWarpToButton.hover.background = KACResources.iconWarpToTSManNodeOver;
						}
						else
						{
							styleWarpToButton.normal.background = KACResources.iconWarpToManNode;
							if (!settings.WarpToRequiresConfirm || WarpToArmed)
								styleWarpToButton.hover.background = KACResources.iconWarpToManNodeOver;
						}
						break;
					case KACAlarm.AlarmTypeEnum.Apoapsis:
					case KACAlarm.AlarmTypeEnum.Periapsis:
						if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION) {
							styleWarpToButton.normal.background = KACResources.iconWarpToTSApPe;
							if (!settings.WarpToRequiresConfirm || WarpToArmed)
								styleWarpToButton.hover.background = KACResources.iconWarpToTSApPeOver;
						}
						else { 
							styleWarpToButton.normal.background = KACResources.iconWarpToApPe;
							if (!settings.WarpToRequiresConfirm || WarpToArmed)
								styleWarpToButton.hover.background = KACResources.iconWarpToApPeOver;
						}
						break;
					case KACAlarm.AlarmTypeEnum.AscendingNode:
						styleWarpToButton.normal.background = KACResources.iconWarpToANDN;
						if (!settings.WarpToRequiresConfirm || WarpToArmed)
							styleWarpToButton.hover.background = KACResources.iconWarpToANDNOver;
						xOffset = 18;
						yOffset = -14;
						break;
					case KACAlarm.AlarmTypeEnum.DescendingNode:
						styleWarpToButton.normal.background = KACResources.iconWarpToANDN;
						if (!settings.WarpToRequiresConfirm || WarpToArmed)
							styleWarpToButton.hover.background = KACResources.iconWarpToANDNOver;
						xOffset = -1;
						yOffset = -16;
						break;
					case KACAlarm.AlarmTypeEnum.SOIChange:
					case KACAlarm.AlarmTypeEnum.SOIChangeAuto:
						xOffset = 6;
						yOffset = -16;
						if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION) {
							styleWarpToButton.normal.background = KACResources.iconWarpToTSApPe;
							if (!settings.WarpToRequiresConfirm || WarpToArmed)
								styleWarpToButton.hover.background = KACResources.iconWarpToTSApPeOver;
						}
						else { 
							styleWarpToButton.normal.background = KACResources.iconWarpToApPe;
							if (!settings.WarpToRequiresConfirm || WarpToArmed)
								styleWarpToButton.hover.background = KACResources.iconWarpToApPeOver;
						}
						break;
					default:
						styleWarpToButton.normal.background = KACResources.iconWarpToApPe;
						if (!settings.WarpToRequiresConfirm || WarpToArmed)
							styleWarpToButton.hover.background = KACResources.iconWarpToApPeOver;
						break;
				}

				//get the screen coordinates of the Point
				Vector3d screenPosNode = PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(KACWorkerGameState.CurrentVessel.orbit.getPositionAtUT(UT)));
				Rect rectNodeButton = new Rect((Int32)screenPosNode.x + xOffset, (Int32)(Screen.height - screenPosNode.y) + yOffset, 20, 12);
				if (GUI.Button(rectNodeButton, "", styleWarpToButton))
				{
					if (Event.current.button == 0)
					{
						if (settings.WarpToRequiresConfirm && !WarpToArmed)
						{
							LogFormatted_DebugOnly("Set confirmed and store Rect");
							WarpToArmed = true;
							WarpToArmedButtonRect = new Rect(rectNodeButton);
							//If in the TS then reset the orbit selection
							if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION)
							{
								lstOrbitRenderChanged.Add(KACWorkerGameState.CurrentVessel.id);
								KACWorkerGameState.CurrentVessel.orbitRenderer.isFocused = true;
								KACWorkerGameState.CurrentVessel.AttachPatchedConicsSolver();
							}
						}
						else
						{
							WarpToArmed = false;

							//Get any existing alarm for the same vessel/type and time - 
							//  Event Time NOT the alarm time
							KACAlarm aExisting = alarms.FirstOrDefault(a => a.VesselID == KACWorkerGameState.CurrentVessel.id.ToString()
								&& Math.Abs((a.AlarmTimeUT + a.AlarmMarginSecs) - UT) <= settings.WarpToDupeProximitySecs
								&& (a.TypeOfAlarm == aType 
									|| a.TypeOfAlarm == KACAlarm.AlarmTypeEnum.SOIChangeAuto && aType==KACAlarm.AlarmTypeEnum.SOIChange
									|| a.TypeOfAlarm == KACAlarm.AlarmTypeEnum.ManeuverAuto && aType==KACAlarm.AlarmTypeEnum.Maneuver
									)
								);

							//LogFormatted("UT:{0},Margin:{1},WUT:{2},Prox:{3}", alarms.First().AlarmTimeUT, alarms.First().AlarmMarginSecs, UT, settings.WarpToDupeProximitySecs);
							//LogFormatted("DIFF:{0}", Math.Abs((alarms.First().AlarmTimeUT + alarms.First().AlarmMarginSecs) - UT) <= settings.WarpToDupeProximitySecs);


							//if there aint one then add one
							if (aExisting == null)
							{
								KACAlarm newAlarm = new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(), "Warp to " + NodeName, "", UT - (WithMargin ? MarginSecs : 0), (WithMargin ? MarginSecs : 0), aType,
										AlarmActions.DefaultsKillWarpOnly());
								if (lstAlarmsWithTarget.Contains(aType))
									newAlarm.TargetObject = KACWorkerGameState.CurrentVesselTarget;
								if (KACWorkerGameState.ManeuverNodeExists)
									newAlarm.ManNodes = KACWorkerGameState.ManeuverNodesFuture;
								newAlarm.Actions.DeleteWhenDone = true;

								alarms.Add(newAlarm);
							}
							else
							{
								//else update the UT
								aExisting.AlarmTimeUT = UT;
							}

							//now accelerate time
							Double timeToEvent = UT - Planetarium.GetUniversalTime();
							Int32 rateToSet = WarpTransitionCalculator.WarpRateTransitionPeriods.Where(
													r => r.UTTo1Times < timeToEvent
														&& (!settings.WarpToLimitMaxWarp || r.Rate<=settings.WarpToMaxWarp)
													)
												.OrderBy(r => r.UTTo1Times)
												.Last().Index;
                            //Make sure we cancel autowarp if its engaged
						    if (TimeWarp.fetch != null)
						    {
						        TimeWarp.fetch.CancelAutoWarp();
						        TimeWarp.SetRate(rateToSet, false);
						    }



						    //If in the TS then reset the orbit selection
							if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION)
							{
								lstOrbitRenderChanged.Add(KACWorkerGameState.CurrentVessel.id);
								KACWorkerGameState.CurrentVessel.orbitRenderer.isFocused = true;
								KACWorkerGameState.CurrentVessel.AttachPatchedConicsSolver();
							}
						}
					}
				}

				if (settings.ShowTooltips && !settings.WarpToTipsHidden)
				{
					//work out where when the mouse is over the button
					Vector3 VectMouseflipped  = Input.mousePosition;
					VectMouseflipped.y = Screen.height - VectMouseflipped.y;
					if (rectNodeButton.Contains(VectMouseflipped))
					{
						//and draw some info bout it
						GUIStyle styleTip = new GUIStyle();
						styleTip.normal.textColor = Color.white;
						styleTip.fontSize = 12;

						String strArm = "";
						if (settings.WarpToRequiresConfirm) {
							if (!WarpToArmed)
								strArm = " (Unarmed)";
						}
						String strlabel = "Warp to " + NodeName + strArm + (WithMargin ? " (Margin=" + new KSPTimeSpan(MarginSecs).ToString(2) + ")" : "");
						GUI.Label(new Rect((Int32)screenPosNode.x + xOffset + 21, (Int32)(Screen.height - screenPosNode.y) + yOffset -2, 100, 12), strlabel, styleTip);
					}
				}
			}
			return false;
		}

		internal List<Guid> lstOrbitRenderChanged = new List<Guid>();


		internal Boolean MouseOverAnyWindow = false;
		internal Boolean InputLockExists = false;
		internal void ControlInputLocks()
		{
			//Do this for control Locks
			if (settings.ClickThroughProtect_KSC || settings.ClickThroughProtect_Tracking || settings.ClickThroughProtect_Flight || settings.ClickThroughProtect_Editor)
			{
				MouseOverAnyWindow = false;
				MouseOverAnyWindow = MouseOverAnyWindow || MouseOverWindow(WindowPosByActiveScene, WindowVisibleByActiveScene);
				MouseOverAnyWindow = MouseOverAnyWindow || MouseOverWindow(_WindowAddRect, WindowVisibleByActiveScene && _ShowAddPane);
				MouseOverAnyWindow = MouseOverAnyWindow || MouseOverWindow(_WindowSettingsRect, WindowVisibleByActiveScene && _ShowSettings);
				MouseOverAnyWindow = MouseOverAnyWindow || MouseOverWindow(_WindowAddMessagesRect, WindowVisibleByActiveScene && _ShowAddMessages);
				MouseOverAnyWindow = MouseOverAnyWindow || MouseOverWindow(_WindowEditRect, WindowVisibleByActiveScene && _ShowEditPane);
				MouseOverAnyWindow = MouseOverAnyWindow || MouseOverWindow(_WindowEarthAlarmRect, WindowVisibleByActiveScene && _ShowEarthAlarm);
				MouseOverAnyWindow = MouseOverAnyWindow || MouseOverWindow(_WindowQuickAddRect, WindowVisibleByActiveScene && _ShowQuickAdd);
#if DEBUG
				MouseOverAnyWindow = MouseOverAnyWindow || MouseOverWindow(_WindowDebugRect, WindowVisibleByActiveScene && _ShowDebugPane);
#endif
				foreach (KACAlarm tmpAlarm in alarms)
				{
					if (tmpAlarm.AlarmWindowID != 0 && tmpAlarm.Actioned)
					{
						MouseOverAnyWindow = MouseOverAnyWindow || MouseOverWindow(tmpAlarm.AlarmWindow, !tmpAlarm.AlarmWindowClosed);
					}
				}


				//If the setting is on and the mouse is over any window then lock it
				if (MouseOverAnyWindow && !InputLockExists)
				{
					Boolean AddLock = false;
					switch (HighLogic.LoadedScene)
					{
						case GameScenes.SPACECENTER: AddLock = settings.ClickThroughProtect_KSC && !(InputLockManager.GetControlLock("KACControlLock") != ControlTypes.None); break;
						case GameScenes.EDITOR: AddLock = settings.ClickThroughProtect_Editor && !(InputLockManager.GetControlLock("KACControlLock") != ControlTypes.None); break;
						case GameScenes.FLIGHT: AddLock = settings.ClickThroughProtect_Flight && !(InputLockManager.GetControlLock("KACControlLock") != ControlTypes.None); break;
						case GameScenes.TRACKSTATION: AddLock = settings.ClickThroughProtect_Tracking && !(InputLockManager.GetControlLock("KACControlLock") != ControlTypes.None); break;
						default:
							break;
					}
					if (AddLock)
					{
						//LogFormatted_DebugOnly("AddingLock-{0}", "KACControlLock");

						switch (HighLogic.LoadedScene)
						{
							case GameScenes.SPACECENTER: InputLockManager.SetControlLock(ControlTypes.KSC_FACILITIES, "KACControlLock"); break;
							case GameScenes.EDITOR:
								InputLockManager.SetControlLock((ControlTypes.EDITOR_LOCK | ControlTypes.EDITOR_GIZMO_TOOLS), "KACControlLock");
								break;
							case GameScenes.FLIGHT:
								InputLockManager.SetControlLock(ControlTypes.ALL_SHIP_CONTROLS, "KACControlLock");
								break;
							case GameScenes.TRACKSTATION:
								InputLockManager.SetControlLock(ControlTypes.TRACKINGSTATION_ALL | ControlTypes.MAP_UI, "KACControlLock");
								break;
							default:
								break;
						}
						InputLockExists = true;
					}
				}
				//Otherwise make sure the lock is removed
				else if (!MouseOverAnyWindow && InputLockExists)
				{
					RemoveInputLock();
				}
			}
		}

		internal void RemoveInputLock()
		{

			//if (InputLockManager.GetControlLock("KACControlLock") == ControlTypes.KSC_FACILITIES ||
			//    InputLockManager.GetControlLock("KACControlLock") == ControlTypes.TRACKINGSTATION_ALL ||
			//    InputLockManager.GetControlLock("KACControlLock") == ControlTypes.All)
			if (InputLockManager.GetControlLock("KACControlLock") != ControlTypes.None)
			{
				//LogFormatted_DebugOnly("Removing-{0}", "KACControlLock");
				InputLockManager.RemoveControlLock("KACControlLock");
			}
			InputLockExists = false;
		}

		private Boolean MouseOverWindow(Rect WindowRect, Boolean WindowVisible)
		{
			return WindowVisible && WindowRect.Contains(Event.current.mousePosition);
		}

#if DEBUG
		public void DebugWriter()
		{
			if (HighLogic.LoadedScene == GameScenes.FLIGHT)
			{
				DebugActionTimed(HighLogic.LoadedScene);
			}
		}
#endif


		//All persistant stuff is stored in the settings object
		//private long SecondsWarpLightIsShown = 3;

		//Constructor - link to parent and set up time
		//#region "Constructor"
		//private KerbalAlarmClock 
		//private KACSettings Settings;


		//public KACWorker(KerbalAlarmClock parent)
		//{
		//    = parent;
		//    Settings = KerbalAlarmClock.Settings;

		//    InitWorkerVariables();
		//}

		private void InitVariables()
		{
			_WindowAddID = UnityEngine.Random.Range(1000, 2000000) + _AssemblyName.GetHashCode();
			_WindowAddMessagesID = UnityEngine.Random.Range(1000, 2000000) + _AssemblyName.GetHashCode();
			_WindowMainID = UnityEngine.Random.Range(1000, 2000000) + _AssemblyName.GetHashCode();
			_WindowSettingsID = UnityEngine.Random.Range(1000, 2000000) + _AssemblyName.GetHashCode();
			_WindowEditID = UnityEngine.Random.Range(1000, 2000000) + _AssemblyName.GetHashCode();
			_WindowEarthAlarmID = UnityEngine.Random.Range(1000, 2000000) + _AssemblyName.GetHashCode();
			_WindowBackupFailedID = UnityEngine.Random.Range(1000, 2000000) + _AssemblyName.GetHashCode();
			_WindowQuickAddID = UnityEngine.Random.Range(1000, 2000000) + _AssemblyName.GetHashCode();

			blnContractsSystemReady = false;
		}
		//#endregion

		//Updates the variables that are used in the drawing - this is not on the OnGUI thread
		private Dictionary<String, KACVesselSOI> lstVessels = new Dictionary<String,KACVesselSOI>();
		public void UpdateDetails()
		{
			KACWorkerGameState.SetCurrentFlightStates();

			//Turn off the rendering of orbits for vessels we may have adjusted by the warpto code
			if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION)
			{
				if (KACWorkerGameState.ChangedVessel && KACWorkerGameState.LastVessel!=null)
				{
					if (lstOrbitRenderChanged.Contains(KACWorkerGameState.LastVessel.id))
					{
						KACWorkerGameState.LastVessel.orbitRenderer.isFocused = false;
						KACWorkerGameState.LastVessel.DetachPatchedConicsSolver();
						lstOrbitRenderChanged.Clear();
					}
				}
			}

			if (KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT)
			{
				//if vessel has changed
				if (KACWorkerGameState.ChangedVessel)
				{
					String strVesselName = "No Vessel";
					if (KACWorkerGameState.LastVessel!=null) strVesselName = KSP.Localization.Localizer.Format(KACWorkerGameState.LastVessel.vesselName);
					LogFormatted("Vessel Change from '{0}' to '{1}'", strVesselName, KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName));
				}

                // Do we need to clear any highlighted science labs?
                if (blnClearScienceLabHighlight)
                {
                    if (highlightedScienceLab != null && highlightedScienceLab.HighlightActive)
                    {
                        highlightedScienceLab.SetHighlightDefault();
                    }
                    blnClearScienceLabHighlight = false;
                    highlightedScienceLab = null;
                }
                else if (highlightedScienceLab != null)
                {
                    blnClearScienceLabHighlight = true;
                }

				// Do we need to restore a maneuverNode after a ship jump - give it 5 secs of attempts for changes to ship
				if (settings.LoadManNode != null && settings.LoadManNode != "" && KACWorkerGameState.IsVesselActive)
				{
					List<ManeuverNode> manNodesToRestore = KACAlarm.ManNodeDeserializeList(settings.LoadManNode);
					manToRestoreAttempts += 1;
					LogFormatted("Attempting to restore a maneuver node-Try {0}-{1}", manToRestoreAttempts.ToString(), manNodesToRestore.Count);
					FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Clear();
					RestoreManeuverNodeList(manNodesToRestore);
					if (KACWorkerGameState.ManeuverNodeExists)
					{
						settings.LoadManNode = null;
						settings.Save();
						manNodesToRestore = null;
						manToRestoreAttempts = 0;
					}
					if (manToRestoreAttempts > (5 / KerbalAlarmClock.UpdateInterval))
					{
						LogFormatted("attempts adding Node failed over 5 secs - giving up");
						settings.LoadManNode = null;
						settings.Save();
						manNodesToRestore = null;
						manToRestoreAttempts = 0;
					}
				}

				// Do we need to restore a Target after a ship jump - give it 4 secs of attempts for changes to ship
				if (settings.LoadVesselTarget != "" && KACWorkerGameState.IsVesselActive)
				{
					ITargetable targetToRestore = KACAlarm.TargetDeserialize(settings.LoadVesselTarget);
					targetToRestoreAttempts += 1;
					LogFormatted("Attempting to restore a Target-Try {0}", targetToRestoreAttempts.ToString());

					if (targetToRestore is Vessel)
						FlightGlobals.fetch.SetVesselTarget(targetToRestore as Vessel);
					else if (targetToRestore is CelestialBody)
						FlightGlobals.fetch.SetVesselTarget(targetToRestore as CelestialBody);

					if (FlightGlobals.fetch.VesselTarget!=null)
					{
						settings.LoadVesselTarget="";
						settings.Save();
						targetToRestore = null;
						targetToRestoreAttempts = 0;
					}
					if (targetToRestoreAttempts > (5 / KerbalAlarmClock.UpdateInterval))
					{
						LogFormatted("attempts adding target over 5 secs failed - giving up");
						settings.LoadVesselTarget = "";
						settings.Save();
						targetToRestore = null;
						targetToRestoreAttempts = 0;
					}
				}

				//Are we adding SOI Alarms
				if (settings.AlarmAddSOIAuto)
				{
					MonitorSOIOnPath();                 
					//Are we doing the base catchall
					//if (settings.AlarmCatchSOIChange)
					//{
					//    GlobalSOICatchAll(KerbalAlarmClock.UpdateInterval * TimeWarp.CurrentRate);
					//}
				}


				//Only do these recalcs at 1x or physwarp...
			    if (TimeWarp.fetch != null)
			    {
			        if (TimeWarp.CurrentRate == 1 || (TimeWarp.WarpMode == TimeWarp.Modes.LOW))
			        {
			            if (settings.AlarmSOIRecalc)
			            {
			                //Adjust any transfer window alarms until they hit the threshold
			                RecalcSOIAlarmTimes(false);
			            }

			            if (settings.AlarmXferRecalc)
			            {
			                //Adjust any transfer window alarms until they hit the threshold
			                RecalcTransferAlarmTimes(false);
			            }

			            if (settings.AlarmNodeRecalc)
			            {
			                //Adjust any Ap,Pe,AN,DNs as flight path changes
			                RecalcNodeAlarmTimes(false);
			            }
			        }
			    }

			    //Are we adding Man Node Alarms
				if (settings.AlarmAddManAuto)
				{
					MonitorManNodeOnPath();
				}
			}

			// Check for contract adjustments
			MonitorContracts();


			//Do we need to turn off the global warp light
			if (KACWorkerGameState.CurrentWarpInfluenceStartTime == null)
				KACWorkerGameState.CurrentlyUnderWarpInfluence = false;
			else
				//has it been on long enough?
				if (KACWorkerGameState.CurrentWarpInfluenceStartTime.AddSeconds(settings.WarpTransitions_ShowIndicatorSecs) < DateTime.Now)
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
				LogFormatted("Saving Moved Window");
				settings.Save();
				WindowPosSaved = true;
			}
			//Update the Last pos
			WindowPosLast.x = WindowPosByActiveScene.x;
			WindowPosLast.y = WindowPosByActiveScene.y;

			//Work out how many game seconds will pass till this runs again
			double SecondsTillNextUpdate;
			double dWarpRate = 1;
		    if (TimeWarp.fetch != null)
		    {
		        dWarpRate = TimeWarp.CurrentRate;
		    }
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

			if (settings.AlarmAddSOIAuto_ExcludeEVA && KACWorkerGameState.CurrentVessel.vesselType == VesselType.EVA)
				return;

			if (settings.AlarmAddSOIAuto_ExcludeDebris && KACWorkerGameState.CurrentVessel.vesselType == VesselType.Debris)
				return;

			if (settings.SOITransitions.Contains(KACWorkerGameState.CurrentVessel.orbit.patchEndTransition))
			{
				timeSOIChange = KACWorkerGameState.CurrentVessel.orbit.UTsoi;
				//timeSOIAlarm = timeSOIChange - Settings.AlarmAddSOIMargin;
				//strOldAlarmNameSOI = KACWorkerGameState.CurrentVessel.vesselName + "";
				//strOldAlarmMessageSOI = KACWorkerGameState.CurrentVessel.vesselName + " - Nearing SOI Change\r\n" +
				//                "     Old SOI: " + KACWorkerGameState.CurrentVessel.orbit.referenceBody.bodyName + "\r\n" +
				//                "     New SOI: " + KACWorkerGameState.CurrentVessel.orbit.nextPatch.referenceBody.bodyName;
				strSOIAlarmName = KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName);// + "-Leaving " + KACWorkerGameState.CurrentVessel.orbit.referenceBody.bodyName;
				strSOIAlarmNotes = KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + " - Nearing SOI Change\r\n" +
								"     Old SOI: " + KACWorkerGameState.CurrentVessel.orbit.referenceBody.bodyName + "\r\n" +
								"     New SOI: " + KACWorkerGameState.CurrentVessel.orbit.nextPatch.referenceBody.bodyName;
			}

			//is there an SOI alarm for this ship already that has not been triggered
			KACAlarm tmpSOIAlarm =
			alarms.Find(delegate(KACAlarm a)
			{
				return
					(a.VesselID == KACWorkerGameState.CurrentVessel.id.ToString())
					&& ((a.TypeOfAlarm == KACAlarm.AlarmTypeEnum.SOIChangeAuto) || (a.TypeOfAlarm == KACAlarm.AlarmTypeEnum.SOIChange))
					&& (a.Triggered == false);
			});

			//if theres a manual SOI alarm already then ignore it
			if ((tmpSOIAlarm != null) && tmpSOIAlarm.TypeOfAlarm == KACAlarm.AlarmTypeEnum.SOIChange)
			{
				//Dont touch manually created SOI Alarms
			}
			else
			{
				//Is there an SOI point
				if (timeSOIChange != 0)
				{
					timeSOIAlarm = timeSOIChange - settings.AlarmAutoSOIMargin;
					//and an existing alarm
					if (tmpSOIAlarm != null)
					{
						//update the time (if more than threshold secs)
						if ((timeSOIAlarm - KACWorkerGameState.CurrentTime.UT) > settings.AlarmAddSOIAutoThreshold)
						{
							tmpSOIAlarm.AlarmTime.UT = timeSOIAlarm;
						}
					}
					//Otherwise if its in the future add a new alarm
					else if (timeSOIAlarm > KACWorkerGameState.CurrentTime.UT)
					{
						//alarms.Add(new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(), strOldAlarmNameSOI, strOldAlarmMessageSOI, timeSOIAlarm, Settings.AlarmAutoSOIMargin,
						//    KACAlarm.AlarmType.SOIChange, (Settings.AlarmOnSOIChange_Action > 0), (Settings.AlarmOnSOIChange_Action > 1)));
						alarms.Add(new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(), strSOIAlarmName, strSOIAlarmNotes, timeSOIAlarm, settings.AlarmAutoSOIMargin,
							KACAlarm.AlarmTypeEnum.SOIChangeAuto, settings.AlarmOnSOIChange_Action));
						//settings.SaveAlarms();
					}
				}
				else
				{
					//remove any existing alarm - if less than threshold - this means old alarms not touched
					if (tmpSOIAlarm != null && (tmpSOIAlarm.Remaining.UT > settings.AlarmAddSOIAutoThreshold))
					{
						alarms.Remove(tmpSOIAlarm);
					}
				}

			}
		}

		private void RecalcSOIAlarmTimes(Boolean OverrideDriftThreshold)
		{
			foreach (KACAlarm tmpAlarm in alarms.Where(a => a.TypeOfAlarm == KACAlarm.AlarmTypeEnum.SOIChange && a.VesselID == KACWorkerGameState.CurrentVessel.id.ToString()))
			{
				if (tmpAlarm.Remaining.UT > settings.AlarmSOIRecalcThreshold)
				{
					//do the check/update on these
					if (settings.SOITransitions.Contains(KACWorkerGameState.CurrentVessel.orbit.patchEndTransition))
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
			foreach (KACAlarm tmpAlarm in alarms.Where(a => a.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Transfer))
			{
				if (tmpAlarm.Remaining.UT > settings.AlarmXferRecalcThreshold)
				{
					KACXFerTarget tmpTarget = new KACXFerTarget();
					tmpTarget.Origin = FlightGlobals.Bodies.Single(b => b.bodyName == tmpAlarm.XferOriginBodyName);
					tmpTarget.Target = FlightGlobals.Bodies.Single(b => b.bodyName == tmpAlarm.XferTargetBodyName);

					//LogFormatted("{0}+{1}-{2}", KACWorkerGameState.CurrentTime.UT.ToString(), tmpTarget.AlignmentTime.UT.ToString(), tmpAlarm.AlarmMarginSecs.ToString());
					//recalc the transfer spot, but dont move it if the difference is more than the threshold value
					if (Math.Abs(KACWorkerGameState.CurrentTime.UT - tmpTarget.AlignmentTime.UT) < settings.AlarmXferRecalcThreshold || OverrideDriftThreshold)
						tmpAlarm.AlarmTime.UT = KACWorkerGameState.CurrentTime.UT - tmpAlarm.AlarmMarginSecs + tmpTarget.AlignmentTime.UT;
				}
			}
		}

		List<KACAlarm.AlarmTypeEnum> TypesToRecalc = new List<KACAlarm.AlarmTypeEnum>() {KACAlarm.AlarmTypeEnum.Apoapsis,KACAlarm.AlarmTypeEnum.Periapsis,
																				KACAlarm.AlarmTypeEnum.AscendingNode,KACAlarm.AlarmTypeEnum.DescendingNode};
		private void RecalcNodeAlarmTimes(Boolean OverrideDriftThreshold)
		{
			//only do these recalcs for the current flight plan
			foreach (KACAlarm tmpAlarm in alarms.Where(a => TypesToRecalc.Contains(a.TypeOfAlarm) && a.VesselID==KACWorkerGameState.CurrentVessel.id.ToString()))
			{
				if (tmpAlarm.Remaining.UT > settings.AlarmNodeRecalcThreshold)
				{
					switch (tmpAlarm.TypeOfAlarm)
					{
						case KACAlarm.AlarmTypeEnum.Apoapsis:
							if (KACWorkerGameState.ApPointExists &&
								((Math.Abs(KACWorkerGameState.CurrentVessel.orbit.timeToAp) > settings.AlarmNodeRecalcThreshold) || OverrideDriftThreshold))
								tmpAlarm.AlarmTime.UT = KACWorkerGameState.CurrentTime.UT - tmpAlarm.AlarmMarginSecs + KACWorkerGameState.CurrentVessel.orbit.timeToAp;
							break;
						case KACAlarm.AlarmTypeEnum.Periapsis:
							if (KACWorkerGameState.PePointExists &&
								((Math.Abs(KACWorkerGameState.CurrentVessel.orbit.timeToPe) > settings.AlarmNodeRecalcThreshold) || OverrideDriftThreshold))
								tmpAlarm.AlarmTime.UT = KACWorkerGameState.CurrentTime.UT - tmpAlarm.AlarmMarginSecs + KACWorkerGameState.CurrentVessel.orbit.timeToPe;
							break;
						case KACAlarm.AlarmTypeEnum.AscendingNode:
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
								((Math.Abs(timeToAN) > settings.AlarmNodeRecalcThreshold) || OverrideDriftThreshold))
								tmpAlarm.AlarmTime.UT = KACWorkerGameState.CurrentTime.UT - tmpAlarm.AlarmMarginSecs + timeToAN;
							break;

						case KACAlarm.AlarmTypeEnum.DescendingNode:
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
								((Math.Abs(timeToDN) > settings.AlarmNodeRecalcThreshold) || OverrideDriftThreshold))
								tmpAlarm.AlarmTime.UT = KACWorkerGameState.CurrentTime.UT - tmpAlarm.AlarmMarginSecs + timeToDN;
							break;
						default:
							break;
					}
				}
			}
		}

		//private void GlobalSOICatchAll(double SecondsTillNextUpdate)
		//{
		//    foreach (Vessel tmpVessel in FlightGlobals.Vessels)
		//    {
		//        //only track vessels, not debris, EVA, etc
		//        //and not the current vessel
		//        //and no SOI alarm for it within the threshold - THIS BIT NEEDS TUNING
		//        if (settings.VesselTypesForSOI.Contains(tmpVessel.vesselType) && (tmpVessel!=KACWorkerGameState.CurrentVessel) &&
		//            (alarms.FirstOrDefault(a => 
		//                (a.VesselID == tmpVessel.id.ToString() && 
		//                (a.TypeOfAlarm == KACAlarm.AlarmType.SOIChange) &&
		//                (Math.Abs(a.Remaining.UT) < SecondsTillNextUpdate + settings.AlarmAddSOIAutoThreshold)
		//                )) == null)
		//            )

		//        {
		//            if (lstVessels.ContainsKey(tmpVessel.id.ToString()) == false)
		//            {
		//                //Add new Vessels
		//                LogFormatted(String.Format("Adding {0}-{1}-{2}-{3}", tmpVessel.id, tmpVessel.vesselName, tmpVessel.vesselType, tmpVessel.mainBody.bodyName));
		//                lstVessels.Add(tmpVessel.id.ToString(), new KACVesselSOI(tmpVessel.vesselName, tmpVessel.mainBody.bodyName));
		//            }
		//            else
		//            {
		//                //get this vessel from the memory array we are keeping and compare to its SOI
		//                if (lstVessels[tmpVessel.id.ToString()].SOIName != tmpVessel.mainBody.bodyName)
		//                {
		//                    //Set a new alarm to display now
		//                    KACAlarm newAlarm = new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(), tmpVessel.vesselName + "- SOI Catch",
		//                        tmpVessel.vesselName + " Has entered a new Sphere of Influence\r\n" +
		//                        "     Old SOI: " + lstVessels[tmpVessel.id.ToString()].SOIName + "\r\n" +
		//                        "     New SOI: " + tmpVessel.mainBody.bodyName,
		//                         KACWorkerGameState.CurrentTime.UT, 0, KACAlarm.AlarmType.SOIChange,
		//                        settings.AlarmOnSOIChange_Action );
		//                    alarms.Add(newAlarm);

		//                    LogFormatted("Triggering SOI Alarm - " + newAlarm.Name);
		//                    newAlarm.Triggered = true;
		//                    newAlarm.Actioned = true;
		//                    if (settings.AlarmOnSOIChange_Action == KACAlarm.AlarmActionEnum.PauseGame)
		//                    {
		//                        LogFormatted(String.Format("{0}-Pausing Game", newAlarm.Name));
		//                        TimeWarp.SetRate(0, true);
		//                        FlightDriver.SetPause(true);
		//                    }
		//                    else if (settings.AlarmOnSOIChange_Action != KACAlarm.AlarmActionEnum.MessageOnly)
		//                    {
		//                        LogFormatted(String.Format("{0}-Halt Warp", newAlarm.Name));
		//                        TimeWarp.SetRate(0, true);
		//                    }

		//                    //reset the name String for next check
		//                    lstVessels[tmpVessel.id.ToString()].SOIName = tmpVessel.mainBody.bodyName;
		//                }
		//            }
		//        }
		//    }
		//}

		private void MonitorManNodeOnPath()
		{
			//is there an alarm
			KACAlarm tmpAlarm = alarms.FirstOrDefault(a => a.TypeOfAlarm == KACAlarm.AlarmTypeEnum.ManeuverAuto && a.VesselID == KACWorkerGameState.CurrentVessel.id.ToString());

			//is there an alarm and no man node?
			if (KACWorkerGameState.ManeuverNodeExists && (KACWorkerGameState.ManeuverNodeFuture != null))
			{
				KSPDateTime nodeAutoAlarm;
				nodeAutoAlarm = new KSPDateTime(KACWorkerGameState.ManeuverNodeFuture.UT - settings.AlarmAddManAutoMargin - GetBurnMarginSecs(settings.DefaultKERMargin));
				
				List<ManeuverNode> manNodesToStore = KACWorkerGameState.ManeuverNodesFuture;

				String strManNodeAlarmName = KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName);
				String strManNodeAlarmNotes = "Time to pay attention to\r\n    " + KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + "\r\nNearing Maneuver Node";

				//Are we updating an alarm
				if (tmpAlarm != null)
				{
					//update the margin
					tmpAlarm.AlarmMarginSecs = settings.AlarmAddManAutoMargin + GetBurnMarginSecs(settings.DefaultKERMargin);
					//and the UT
					tmpAlarm.AlarmTime.UT = new KSPDateTime(KACWorkerGameState.ManeuverNodeFuture.UT - tmpAlarm.AlarmMarginSecs).UT;
					tmpAlarm.ManNodes = manNodesToStore;
				}
				else 
				{
					//dont add an alarm if we are within the threshold period
					if (nodeAutoAlarm.UT + settings.AlarmAddManAutoMargin - settings.AlarmAddManAutoThreshold > KACWorkerGameState.CurrentTime.UT)
					{
						//or are we setting a new one
						alarms.Add(new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(), strManNodeAlarmName, strManNodeAlarmNotes, nodeAutoAlarm.UT, settings.AlarmAddManAutoMargin + GetBurnMarginSecs(settings.DefaultKERMargin), KACAlarm.AlarmTypeEnum.ManeuverAuto,
							settings.AlarmAddManAuto_Action , manNodesToStore));
						//settings.Save();
					}
				}
			}
			else if (tmpAlarm!=null && settings.AlarmAddManAuto_andRemove && !KACWorkerGameState.ManeuverNodeExists)
			{
				alarms.Remove(tmpAlarm);
			}
		}

		internal void MonitorContracts()
		{
			if(lstContracts==null) return;

			//check the ready flag
			if (!blnContractsSystemReady) return;

			//check for expired/dead contracts
			if (settings.ContractExpireDelete)
			{
				List<KACAlarm> ToDelete = new List<KACAlarm>();
				foreach (KACAlarm tmpAlarm in alarms.Where(a => (a.TypeOfAlarm== KACAlarm.AlarmTypeEnum.Contract || a.TypeOfAlarm== KACAlarm.AlarmTypeEnum.ContractAuto) && 
																a.ContractAlarmType == KACAlarm.ContractAlarmTypeEnum.Expire))
				{
					if (!lstContracts.Any(c => c.ContractGuid == tmpAlarm.ContractGUID))
					{
						LogFormatted("Found an Expired Contract Alarm to Delete:{0}", tmpAlarm.Name);
						ToDelete.Add(tmpAlarm);
					}
				}
				foreach (KACAlarm a in ToDelete)
				{
					alarms.Remove(a);
				}
			}
			
			if (settings.ContractDeadlineDelete)
			{
				List<KACAlarm> ToDelete = new List<KACAlarm>();
				foreach (KACAlarm tmpAlarm in alarms.Where(a => (a.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Contract || a.TypeOfAlarm == KACAlarm.AlarmTypeEnum.ContractAuto) &&
																a.ContractAlarmType == KACAlarm.ContractAlarmTypeEnum.Deadline))
				{
					if (!lstContracts.Any(c => c.ContractGuid == tmpAlarm.ContractGUID))
					{
						LogFormatted("Found a Completed/Failed Contract Alarm to Delete:{0}", tmpAlarm.Name);
						ToDelete.Add(tmpAlarm);
					}
				}
				foreach (KACAlarm a in ToDelete)
				{
					alarms.Remove(a);
				}
			}

			//Now are we monitoring for alarms

			if (settings.AlarmAddContractAutoOffered != Settings.AutoContractBehaviorEnum.None)
			{
				CreateAutoContracts(settings.AlarmAddContractAutoOffered,Contract.State.Offered,settings.ContractExpireDontCreateInsideMargin,settings.AlarmOnContractExpireMargin,settings.AlarmOnContractExpire_Action);
			}
			if (settings.AlarmAddContractAutoActive != Settings.AutoContractBehaviorEnum.None)
			{
				CreateAutoContracts(settings.AlarmAddContractAutoActive, Contract.State.Active, settings.ContractDeadlineDontCreateInsideMargin, settings.AlarmOnContractDeadlineMargin, settings.AlarmOnContractDeadline_Action);
			}
		}

		private void CreateAutoContracts(Settings.AutoContractBehaviorEnum TypeOfAuto, Contract.State state, Boolean DontCreateAlarmsInsideMargin, Double margin, AlarmActions action)
		{
			if (TypeOfAuto == Settings.AutoContractBehaviorEnum.Next && DontCreateAlarmsInsideMargin)
			{
				//find the next valid contract to have an alarm for
				Contract conNext = lstContracts.Where(ci => ci.ContractState == state &&
											ci.DateNext() > KACWorkerGameState.CurrentTime.UT + margin)
											.OrderBy(ci => ci.DateNext())
											.FirstOrDefault();

				if (conNext != null)
				{
					AddContractAutoAlarm(conNext, margin, action);
				}
			}
			else if (TypeOfAuto == Settings.AutoContractBehaviorEnum.All || TypeOfAuto == Settings.AutoContractBehaviorEnum.Next)
			{
				Boolean FirstOutsideMargin = false;
				foreach (Contract c in lstContracts.Where(ci => ci.ContractState == state &&
											ci.DateNext() > KACWorkerGameState.CurrentTime.UT).OrderBy(ci => ci.DateNext()))
				{
					AddContractAutoAlarm(c, margin, action);

					FirstOutsideMargin = (c.DateNext() > KACWorkerGameState.CurrentTime.UT + margin);

					if (TypeOfAuto == Settings.AutoContractBehaviorEnum.Next && FirstOutsideMargin)
						break;
				}
			}
		}

		private void AddContractAutoAlarm(Contract contract, Double margin, AlarmActions action)
		{
			//If theres already an alarm then leave it alone
			if (alarms.Any(a => a.ContractGUID == contract.ContractGuid))
				return;

			//log
			LogFormatted("Adding new {3} Contract Alarm for: {0}({1})-{2}", contract.Title, contract.ContractGuid, contract.DateExpire, contract.ContractState);

			//gen the text
			String AlarmName, AlarmNotes;
			GenerateContractStringsFromContract(contract, out AlarmName, out AlarmNotes);

			//create the alarm
			KACAlarm tmpAlarm = new KACAlarm("", AlarmName, AlarmNotes, contract.DateNext() - margin, margin,
				KACAlarm.AlarmTypeEnum.ContractAuto, action);

			//add the contract specifics
			tmpAlarm.ContractGUID = contract.ContractGuid;
			tmpAlarm.ContractAlarmType = contract.AlarmType();

			//add it to the list
			alarms.Add(tmpAlarm);
		}

		/// <summary>
		/// Only called when game is in paused state
		/// </summary>
		public void UpdateEarthAlarms()
		{
			foreach (KACAlarm tmpAlarm in alarms.Where(a=>a.TypeOfAlarm== KACAlarm.AlarmTypeEnum.EarthTime))
			{
				tmpAlarm.Remaining.UT = (EarthTimeDecode(tmpAlarm.AlarmTime.UT) - DateTime.Now).TotalSeconds;
			}
		}

        private KACAlarmList alarmsToAdd;

        private void ParseAlarmsAndAffectWarpAndPause(double SecondsTillNextUpdate)
		{
            if(alarmsToAdd== null)
            {
                alarmsToAdd = new KACAlarmList();
            }
            else
            {
                alarmsToAdd.Clear();
            }
			
			KACAlarm alarmAddTemp;

            for (int i = 0,iAlarms = alarms.Count; i < iAlarms; i++)
            {
                KACAlarm tmpAlarm = alarms[i];

				//reset each alarms WarpInfluence flag
				if (KACWorkerGameState.CurrentWarpInfluenceStartTime == null)
					tmpAlarm.WarpInfluence = false;
				else
					//if the lights been on long enough
					if (KACWorkerGameState.CurrentWarpInfluenceStartTime.AddSeconds(settings.WarpTransitions_ShowIndicatorSecs) < DateTime.Now)
						tmpAlarm.WarpInfluence = false;

                //Update Remaining interval for each alarm
                if (tmpAlarm.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime)
                    //tmpAlarm.Remaining.UT = tmpAlarm.AlarmTime.UT - KACWorkerGameState.CurrentTime.UT;
                    tmpAlarm.UpdateRemaining(tmpAlarm.AlarmTime.UT - KACWorkerGameState.CurrentTime.UT);
                else
                    tmpAlarm.UpdateRemaining((EarthTimeDecode(tmpAlarm.AlarmTime.UT) - DateTime.Now).TotalSeconds);
				
				//set triggered for passed alarms so the OnGUI part can draw the window later
				//if ((KACWorkerGameState.CurrentTime.UT >= tmpAlarm.AlarmTime.UT) && (tmpAlarm.Enabled) && (!tmpAlarm.Triggered))
				if ((tmpAlarm.Remaining.UT<=0) && (tmpAlarm.Enabled) && (!tmpAlarm.Triggered))
				{
					//if (tmpAlarm.ActionedAt > 0)
					//{
					//    LogFormatted("Suppressing Alarm due to Actioned At being set:{0}", tmpAlarm.Name);
					//    tmpAlarm.Triggered = true;
					//    tmpAlarm.Actioned = true;
					//    tmpAlarm.AlarmWindowClosed = true;
					//}
					//else
					//{

						LogFormatted("Triggering Alarm - " + tmpAlarm.Name);
						tmpAlarm.Triggered = true;


						if (CreateAlarmRepeats(tmpAlarm, out alarmAddTemp))
							alarmsToAdd.Add(alarmAddTemp);

						try {
							APIInstance_AlarmStateChanged(tmpAlarm, AlarmStateEventsEnum.Triggered);
						} catch (Exception ex) {
							LogFormatted("Error Raising API Event-Triggered Alarm: {0}\r\n{1}", ex.Message, ex.StackTrace);
						} 

						//If we are simply past the time make sure we halt the warp
						//only do this in flight mode
						//if (!ViewAlarmsOnly)
						//{
							if (tmpAlarm.PauseGame)
							{
								LogFormatted(String.Format("{0}-Pausing Game", tmpAlarm.Name));
								if (tmpAlarm.Actions.Message != AlarmActions.MessageEnum.Yes)
									tmpAlarm.Actions.Message = AlarmActions.MessageEnum.Yes;

                                //Make sure we cancle autowarp if its engaged
                                TimeWarp.fetch.CancelAutoWarp();
                                TimeWarp.SetRate(0, true);
								FlightDriver.SetPause(true);
							}
							else if (tmpAlarm.HaltWarp)
							{
								if (!FlightDriver.Pause)
								{
									LogFormatted(String.Format("{0}-Halt Warp", tmpAlarm.Name));
                                    //Make sure we cancle autowarp if its engaged
                                    TimeWarp.fetch.CancelAutoWarp();
                                    TimeWarp.SetRate(0, true);
								}
								else
								{
									LogFormatted(String.Format("{0}-Game paused, skipping Halt Warp", tmpAlarm.Name));
								}
							}
						//}
					//}
				}


				//skip this if we aren't in flight mode
				//if (!ViewAlarmsOnly)
				//{
					//if in the next two updates we would pass the alarm time then slow down the warp
					if (!tmpAlarm.Actioned && tmpAlarm.Enabled && (tmpAlarm.HaltWarp || tmpAlarm.PauseGame))
					{
						if (settings.WarpTransitions_Instant)
						{
							Double TimeNext = KACWorkerGameState.CurrentTime.UT + SecondsTillNextUpdate * 2;
							//LogFormatted(CurrentTime.UT.ToString() + "," + TimeNext.ToString());
							if (TimeNext > tmpAlarm.AlarmTime.UT)
							{
								tmpAlarm.WarpInfluence = true;
								KACWorkerGameState.CurrentlyUnderWarpInfluence = true;
								KACWorkerGameState.CurrentWarpInfluenceStartTime = DateTime.Now;

								TimeWarp w = TimeWarp.fetch;
								if (w.current_rate_index > 0)
								{
									LogFormatted("Reducing Warp");
                                    //Make sure we cancel autowarp if its engaged
                                    TimeWarp.fetch.CancelAutoWarp();
                                    TimeWarp.SetRate(w.current_rate_index - 1, true);
								}
							}
						}
						else
						{
							if (WarpTransitionCalculator.UTToRateTimesOne * settings.WarpTransitions_UTToRateTimesOneTenths / 10 > tmpAlarm.AlarmTime.UT - KACWorkerGameState.CurrentTime.UT)
							{
								tmpAlarm.WarpInfluence = true;
								KACWorkerGameState.CurrentlyUnderWarpInfluence = true;
								KACWorkerGameState.CurrentWarpInfluenceStartTime = DateTime.Now;

								TimeWarp w = TimeWarp.fetch;
								if (w.current_rate_index > 0 && WarpTransitionCalculator.UTToRateTimesOne > (tmpAlarm.AlarmTime.UT - KACWorkerGameState.CurrentTime.UT))
								{
									LogFormatted("Reducing Warp-Transition Instant");
                                    //Make sure we cancel autowarp if its engaged
                                    TimeWarp.fetch.CancelAutoWarp();
                                    TimeWarp.SetRate(w.current_rate_index - 1, true);
								}
								else if (w.current_rate_index > 0)
								{
									LogFormatted("Reducing Warp-Transition");
                                    //Make sure we cancel autowarp if its engaged
                                    TimeWarp.fetch.CancelAutoWarp();
									TimeWarp.SetRate(w.current_rate_index - 1, false);
								}
							}
						}
						
					}
				//}

					if (tmpAlarm.Triggered && !tmpAlarm.Actioned)
					{
						tmpAlarm.Actioned = true;


						//Play the sounds if necessary
						if (tmpAlarm.Actions.PlaySound) {
							//first get the right sound
							AlarmSound s = settings.AlarmSounds.FirstOrDefault(st => st.Types.Contains(tmpAlarm.TypeOfAlarm));

							if (s == null || s.Enabled == false) {
								s = settings.AlarmSounds[0];
							}

							if (!tmpAlarm.ShowMessage && s.RepeatCount > 5)
							{
								audioController.Play(KACResources.clipAlarms[s.SoundName], 5);
							}
							else
							{
								audioController.Play(KACResources.clipAlarms[s.SoundName], s.RepeatCount);
							}
						}


						//if (tmpAlarm.AlarmActionConvert == KACAlarm.AlarmActionEnum.KillWarpOnly 
						//    || tmpAlarm.AlarmActionConvert == KACAlarm.AlarmActionEnum.DoNothingDeleteWhenPassed
						//    || tmpAlarm.AlarmActionConvert == KACAlarm.AlarmActionEnum.DoNothing)
						if((tmpAlarm.Actions.Message==AlarmActions.MessageEnum.No) ||
							(
								tmpAlarm.Actions.Message == AlarmActions.MessageEnum.YesIfOtherVessel &&
								KACWorkerGameState.CurrentVessel!=null &&
								tmpAlarm.VesselID == KACWorkerGameState.CurrentVessel.id.ToString()
							))
						{
							tmpAlarm.AlarmWindowClosed = true;
							try
							{
								APIInstance_AlarmStateChanged(tmpAlarm, AlarmStateEventsEnum.Closed);
							}
							catch (Exception ex)
							{
								LogFormatted("Error Raising API Event-Closed Alarm: {0}\r\n{1}", ex.Message, ex.StackTrace);
							}

						}

						LogFormatted("Actioning Alarm");
						LogFormatted_DebugOnly("{0}",tmpAlarm.Actions);
					}

			}

			//Add any extra alarms that were created in the parse loop
			foreach (KACAlarm a in alarmsToAdd)
			{
				alarms.Add(a);
			}

			// Delete the do nothing/delete alarms - One loop to find the ones to delete - cant delete inside the foreach or it breaks the iterator
			List<KACAlarm> ToDelete = new List<KACAlarm>();
            //foreach (KACAlarm tmpAlarm in alarms.Where(a => (a.AlarmActionConvert == KACAlarm.AlarmActionEnum.DoNothingDeleteWhenPassed) || (a.ActionDeleteWhenDone)))
            //foreach (KACAlarm tmpAlarm in alarms.Where(a => a.Actions.Warp == AlarmActions.WarpEnum.DoNothing && a.Actions.DeleteWhenDone))
            foreach (KACAlarm tmpAlarm in alarms.Where(a => (!a.ShowMessage || a.Actions.Warp == AlarmActions.WarpEnum.DoNothing) && a.Actions.DeleteWhenDone))
            {
                if (tmpAlarm.Triggered && tmpAlarm.Actioned)
    				ToDelete.Add(tmpAlarm);
			}
			foreach (KACAlarm a in ToDelete)
			{
				alarms.Remove(a);
			}
		}


		private Boolean CreateAlarmRepeats(KACAlarm alarmToCheck, out KACAlarm alarmToAdd)
		{
			if (alarmToCheck.RepeatAlarm){
				if (alarmToCheck.TypeOfAlarm == KACAlarm.AlarmTypeEnum.TransferModelled)
				{
					try
					{
						LogFormatted("Adding repeat alarm for ({0}->{1})", alarmToCheck.XferOriginBodyName, alarmToCheck.XferTargetBodyName);
						//find the next transfer from the modelled data
						KACXFerModelPoint tmpModelPoint = KACResources.lstXferModelPoints.FirstOrDefault(
								   m => FlightGlobals.Bodies[m.Origin].bodyName == alarmToCheck.XferOriginBodyName &&
									   FlightGlobals.Bodies[m.Target].bodyName == alarmToCheck.XferTargetBodyName &&
									   m.UT > alarmToCheck.AlarmTime.UT + alarmToCheck.AlarmMarginSecs);

						if (tmpModelPoint != null)
						{
							KSPDateTime XferNextTargetEventTime = new KSPDateTime(tmpModelPoint.UT);

							if (!alarms.Any(a => a.TypeOfAlarm == KACAlarm.AlarmTypeEnum.TransferModelled &&
											a.XferOriginBodyName == alarmToCheck.XferOriginBodyName &&
											a.XferTargetBodyName == alarmToCheck.XferTargetBodyName &&
											a.AlarmTime.UT == tmpModelPoint.UT))
							{
								alarmToAdd=alarmToCheck.Duplicate(XferNextTargetEventTime.UT - alarmToCheck.AlarmMarginSecs);
								return true;
							}
							else
							{
								LogFormatted("Alarm already exists, not adding repeat({0}->{1}): UT={2}", alarmToCheck.XferOriginBodyName, alarmToCheck.XferTargetBodyName, XferNextTargetEventTime.UT);
							}
						}
						else
						{
							LogFormatted("Unable to find a future model data point for this transfer({0}->{1})", alarmToCheck.XferOriginBodyName, alarmToCheck.XferTargetBodyName);
						}

					}
					catch (Exception ex)
					{
						LogFormatted("Unable to find a future model data point for this transfer({0}->{1})\r\n{2}", alarmToCheck.XferOriginBodyName, alarmToCheck.XferTargetBodyName, ex.Message);
					}
				}
				else if (alarmToCheck.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Apoapsis || alarmToCheck.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Periapsis)
				{
					try
					{
						Vessel v = FindVesselForAlarm(alarmToCheck);

						if(v == null)
						{
							LogFormatted("Unable to find the vessel to work out the repeat ({0})", alarmToCheck.VesselID);
						}
						else
						{

							LogFormatted("Adding repeat alarm for Ap/Pe ({0})", alarmToCheck.VesselID);

							//get the time of the next node if the margin is greater than 0
							Double nextApPe = Planetarium.GetUniversalTime() + v.orbit.timeToAp + (alarmToCheck.AlarmMarginSecs>0?v.orbit.period:0);
							if (alarmToCheck.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Periapsis)
								nextApPe = Planetarium.GetUniversalTime() + v.orbit.timeToPe + +(alarmToCheck.AlarmMarginSecs > 0 ? v.orbit.period : 0);

							if (!alarms.Any(a => a.TypeOfAlarm == alarmToCheck.TypeOfAlarm &&
									a.AlarmTime.UT == nextApPe))
							{
								alarmToAdd = alarmToCheck.Duplicate(nextApPe);
								return true;
							}
							else
							{
								LogFormatted("Alarm already exists, not adding repeat ({0}): UT={1}", alarmToCheck.VesselID, nextApPe);
							}
							
							alarmToAdd = alarmToCheck.Duplicate(nextApPe);
							return true;
						}

					}
					catch (Exception ex)
					{
						LogFormatted("Unable to add a repeat alarm ({0})\r\n{1}", alarmToCheck.VesselID, ex.Message);
					}
				}
				else if (alarmToCheck.RepeatAlarmPeriod.UT > 0)
				{
					LogFormatted("Adding repeat alarm for {0}:{1}-{2}+{3}", alarmToCheck.TypeOfAlarm, alarmToCheck.Name, alarmToCheck.AlarmTime.UT, alarmToCheck.RepeatAlarmPeriod.UT);
					alarmToAdd = alarmToCheck.Duplicate(alarmToCheck.AlarmTime.UT + alarmToCheck.RepeatAlarmPeriod.UT);
					return true;
				}
			}
			alarmToAdd = null;
			return false;
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

	}



#if DEBUG
//	//This will kick us into the save called default and set the first vessel active
//	[KSPAddon(KSPAddon.Startup.MainMenu, false)]
//	public class Debug_AutoLoadPersistentSaveOnStartup : MonoBehaviour
//	{
//		//use this variable for first run to avoid the issue with when this is true and multiple addons use it
//		public static bool first = true;
//		public void Start()
//		{
//			//only do it on the first entry to the menu
//			if (first)
//			{
//				first = false;
//				HighLogic.SaveFolder = "default";
////				HighLogic.SaveFolder = "Career";
//				Game game = GamePersistence.LoadGame("persistent", HighLogic.SaveFolder, true, false);

//				if (game != null && game.flightState != null && game.compatible)
//				{
//					//straight to spacecenter
//					HighLogic.CurrentGame = game;
//					//HighLogic.LoadScene(GameScenes.SPACECENTER);
//					HighLogic.LoadScene(GameScenes.TRACKSTATION);
//					return;

//					Int32 FirstVessel;
//					Boolean blnFoundVessel = false;
//					for (FirstVessel = 0; FirstVessel < game.flightState.protoVessels.Count; FirstVessel++)
//					{
//						if (game.flightState.protoVessels[FirstVessel].vesselType != VesselType.SpaceObject &&
//							game.flightState.protoVessels[FirstVessel].vesselType != VesselType.Unknown)
//						{
//							blnFoundVessel = true;
//							break;
//						}
//					}
//					if (!blnFoundVessel)
//						FirstVessel = 0;
//					FlightDriver.StartAndFocusVessel(game, FirstVessel);
//				}

//				//CheatOptions.InfiniteFuel = true;
//			}
//		}
//	}
#endif
}
