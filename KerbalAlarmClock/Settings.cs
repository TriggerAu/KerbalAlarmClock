using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KerbalAlarmClock
{
    internal class Settings : ConfigNodeStorage
    {
        internal Settings(String FilePath) : base(FilePath) {
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //on each start set the attention flag to the property - should be on each program start
            VersionAttentionFlag = VersionAvailable;

            OnEncodeToConfigNode();
        }

        //Windows and Visible Settings
        [Persistent] internal Boolean WindowVisible = false;
        [Persistent] internal Boolean WindowMinimized = false;
        internal Rect WindowPos = new Rect(3, 55, 340, 45);
        [Persistent] private RectStorage WindowPositionStored = new RectStorage();

        [Persistent] internal Boolean WindowVisible_SpaceCenter = false;
        [Persistent] internal Boolean WindowMinimized_SpaceCenter = false;
        internal Rect WindowPos_SpaceCenter = new Rect(3, 36, 340, 45);
        [Persistent] private RectStorage WindowPos_SpaceCenterStored = new RectStorage();

        [Persistent] internal Boolean WindowVisible_TrackingStation = false;
        [Persistent] internal Boolean WindowMinimized_TrackingStation = false;
        internal Rect WindowPos_TrackingStation = new Rect(196, 45, 340, 45);
        [Persistent] private RectStorage WindowPos_TrackingStationStored = new RectStorage();

        [Persistent] internal Boolean WindowVisible_Editor = false;
        [Persistent] internal Boolean WindowMinimized_Editor = false;
        internal Rect WindowPos_Editor = new Rect(270, 45, 340, 45);
        [Persistent] private RectStorage WindowPos_EditorStored = new RectStorage();

        [Persistent] internal Boolean WindowChildPosBelow = false;

        [Persistent] internal Rect IconPos =  new Rect(152, 0, 32, 32);
        [Persistent] internal Rect IconPos_SpaceCenter = new Rect(3, 3, 32, 32);
        [Persistent] internal Boolean IconShow_SpaceCenter = true;
        [Persistent] internal Rect IconPos_TrackingStation = new Rect(196, 0, 32, 32);
        [Persistent] internal Boolean IconShow_TrackingStation = true;
        [Persistent] internal Rect IconPos_Editor = new Rect(298, 0, 32, 32);
        [Persistent] internal Boolean IconShow_Editor = true;

        [Persistent] internal MiminalDisplayType WindowMinimizedType = MiminalDisplayType.NextAlarm;

        [Persistent] internal Boolean F11KeystrokeDisabled = true;
        [Persistent] internal Boolean KillWarpOnThrottleCutOffKeystroke = true;

        //Audio Volume
        [Persistent] internal Boolean AlarmsVolumeFromUI=true;
        [Persistent] internal Single AlarmsVolume=0.25f;

        //Visuals
        [Persistent] internal DisplaySkin SelectedSkin = DisplaySkin.Default;

        //Behaviours
        [Persistent] internal Int32 BehaviourChecksPerSec = 10;
        [Persistent] internal Int32 BehaviourChecksPerSec_Custom = 40;

        [Persistent] internal Boolean WarpTransitions_Instant = false;
        [Persistent] internal Int32 WarpTransitions_UpdateSecs = 5;
        [Persistent] internal Int32 WarpTransitions_UTToRateTimesOneTenths = 15;
        [Persistent] internal Int32 WarpTransitions_ShowIndicatorSecs = 4;

        [Persistent] internal Boolean WarpToEnabled = true;
        [Persistent] internal Boolean WarpToTipsHidden = false;
        [Persistent] internal Int32 WarpToTSIconDelayMSecs = 200;
        [Persistent] internal Int32 WarpToDupeProximitySecs = 60;
        [Persistent] internal Boolean WarpToRequiresConfirm = false;

        [Persistent] internal Boolean WarpToIncrease=false;
        //[Persistent] internal Int32 WarpToMaxRate = 100;

        [Persistent] internal Boolean WarpToAddMarginAp = false;
        [Persistent] internal Boolean WarpToAddMarginPe = false;
        [Persistent] internal Boolean WarpToAddMarginAN = false;
        [Persistent] internal Boolean WarpToAddMarginDN = false;
        [Persistent] internal Boolean WarpToAddMarginSOI = true;
        [Persistent] internal Boolean WarpToAddMarginManNode = true;
        //[Persistent] internal Boolean NewWarpBehaviour = true;

        [Persistent] internal Boolean AllowJumpFromViewOnly = true;
        [Persistent] internal Boolean AllowJumpToAsteroid = true;
        [Persistent] internal Boolean BackupSaves = true;
        [Persistent] internal Boolean CancelFlightModeJumpOnBackupFailure = false;
        [Persistent] internal int BackupSavesToKeep = 20;

        [Persistent] internal String AlarmListMaxAlarms = "10";
        public int AlarmListMaxAlarmsInt
        {
            get
            {
                try
                {
                    return Convert.ToInt32(this.AlarmListMaxAlarms);
                }
                catch (Exception)
                {
                    return 10;
                }

            }
        }
        [Persistent] internal Boolean ConfirmAlarmDeletes = false;


        [Persistent]
        internal KACAlarm.AlarmActionEnum AlarmDefaultAction = KACAlarm.AlarmActionEnum.KillWarp;
        [Persistent] internal Double AlarmDefaultMargin = 60;
        [Persistent] internal Int32 AlarmPosition = 1;
        [Persistent] internal Boolean AlarmDeleteOnClose = false;
        [Persistent] internal Boolean HideOnPause = true;
        //public Boolean TimeAsUT = false;
        [Persistent] internal OldPrintTimeFormat TimeFormat = OldPrintTimeFormat.KSPString;
        [Persistent] internal DateStringFormatsEnum DateTimeFormat = DateStringFormatsEnum.KSPFormatWithSecs;
        [Persistent] internal Boolean TimeFormatConverted = false;
        internal TimeSpanStringFormatsEnum TimeSpanFormat { get {
            switch (DateTimeFormat)
            {
                case DateStringFormatsEnum.TimeAsUT:
                    return TimeSpanStringFormatsEnum.TimeAsUT;
                case DateStringFormatsEnum.KSPFormat:
                    return TimeSpanStringFormatsEnum.KSPFormat;
                case DateStringFormatsEnum.KSPFormatWithSecs:
                    return TimeSpanStringFormatsEnum.KSPFormat;
                case DateStringFormatsEnum.DateTimeFormat:
                    return TimeSpanStringFormatsEnum.DateTimeFormat;
                default:
                    return TimeSpanStringFormatsEnum.KSPFormat;
            }
        } }


        [Persistent] internal Boolean ShowTooltips = true;
        [Persistent] internal Boolean ShowEarthTime = false;

        [Persistent] internal Boolean AlarmXferRecalc = true;
        [Persistent] internal Double AlarmXferRecalcThreshold = 180;
        [Persistent] internal Boolean AlarmXferDisplayList = false;
        [Persistent] internal Boolean XferModelLoadData = true;
        [Persistent] internal Boolean XferModelDataLoaded = false;
        [Persistent] internal Boolean XferUseModelData = false;

        [Persistent] internal Boolean AlarmNodeRecalc = false;
        [Persistent] internal Double AlarmNodeRecalcThreshold = 180;

        [Persistent] internal Boolean AlarmAddSOIAuto = false;
        [Persistent] internal Double AlarmAddSOIAutoThreshold = 180;
        [Persistent] internal Double AlarmAutoSOIMargin = 900;

        [Persistent] internal Boolean AlarmAddSOIAuto_ExcludeEVA = true;
        [Persistent] internal Boolean AlarmAddSOIAuto_ExcludeDebris = true;

        [Persistent] internal Boolean AlarmSOIRecalc = false;
        [Persistent] internal Double AlarmSOIRecalcThreshold = 180;

        [Persistent] internal Boolean AlarmAddManAuto = false;
        [Persistent] internal Boolean AlarmAddManAuto_andRemove = false;
        [Persistent] internal Double AlarmAddManAutoMargin = 180;
        [Persistent] internal Double AlarmAddManAutoThreshold = 180;
        [Persistent] internal KACAlarm.AlarmActionEnum AlarmAddManAuto_Action = KACAlarm.AlarmActionEnum.KillWarp;

        [Persistent] internal Double AlarmAddManQuickMargin = 180;
        [Persistent] internal KACAlarm.AlarmActionEnum AlarmAddManQuickAction = KACAlarm.AlarmActionEnum.KillWarp;
        [Persistent] internal Double AlarmAddSOIQuickMargin = 180;
        [Persistent] internal KACAlarm.AlarmActionEnum AlarmAddSOIQuickAction = KACAlarm.AlarmActionEnum.KillWarp;

        [Persistent] internal Double AlarmAddNodeQuickMargin = 30;
        [Persistent] internal KACAlarm.AlarmActionEnum AlarmAddNodeQuickAction = KACAlarm.AlarmActionEnum.KillWarp;


        [Persistent] internal Double AlarmOnContractExpireMargin = 3600;
        [Persistent] internal KACAlarm.AlarmActionEnum AlarmOnContractExpire_Action = KACAlarm.AlarmActionEnum.KillWarp;
        [Persistent] internal Double AlarmOnContractDeadlineMargin = 86400;
        [Persistent] internal KACAlarm.AlarmActionEnum AlarmOnContractDeadline_Action = KACAlarm.AlarmActionEnum.KillWarp;

        internal enum AutoContractBehaviorEnum
        {
            [Description("No Alarms")] None,
            [Description("Next Contract Only")] Next,
            [Description("All Contracts")] All,
        }
        [Persistent] internal AutoContractBehaviorEnum AlarmAddContractAutoOffered = AutoContractBehaviorEnum.None;
        [Persistent] internal AutoContractBehaviorEnum AlarmAddContractAutoActive = AutoContractBehaviorEnum.None;
        [Persistent] internal Boolean ContractDeadlineDelete = true;
        [Persistent] internal Boolean ContractExpireDelete = true;
        [Persistent] internal Boolean ContractDeadlineDontCreateInsideMargin = true;
        [Persistent] internal Boolean ContractExpireDontCreateInsideMargin = true;
        

        //[Persistent] internal Double AlarmAddApQuickMargin = 60;
        //[Persistent] internal KACAlarm.AlarmActionEnum AlarmAddApQuickAction = KACAlarm.AlarmActionEnum.KillWarp;
        //[Persistent] internal Double AlarmAddPeQuickMargin = 60;
        //[Persistent] internal KACAlarm.AlarmActionEnum AlarmAddPeQuickAction = KACAlarm.AlarmActionEnum.KillWarp;
        //[Persistent] internal Double AlarmAddANQuickMargin = 60;
        //[Persistent] internal KACAlarm.AlarmActionEnum AlarmAddANQuickAction = KACAlarm.AlarmActionEnum.KillWarp;
        //[Persistent] internal Double AlarmAddDNQuickMargin = 60;
        //[Persistent] internal KACAlarm.AlarmActionEnum AlarmAddDNQuickAction = KACAlarm.AlarmActionEnum.KillWarp;

        //public double AlarmAddSOIMargin = 120;
        //[Persistent] internal Boolean AlarmCatchSOIChange = false;
        [Persistent] internal KACAlarm.AlarmActionEnum AlarmOnSOIChange_Action = KACAlarm.AlarmActionEnum.KillWarp;

        [Persistent] internal Boolean AlarmCrewDefaultStoreNode = false;

        //Strings to store objects to reset after ship switch;
        [Persistent] internal String LoadManNode="";
        [Persistent] internal String LoadVesselTarget = "";

        //public KACAlarmList Alarms = new KACAlarmList();
        
        public List<GameScenes> DrawScenes = new List<GameScenes> { GameScenes.FLIGHT, GameScenes.SPACECENTER, GameScenes.TRACKSTATION, GameScenes.EDITOR };
        public List<GameScenes> BehaviourScenes = new List<GameScenes> { GameScenes.FLIGHT };
        public List<VesselType> VesselTypesForSOI = new List<VesselType>() { VesselType.Base, VesselType.Lander, VesselType.Probe, VesselType.Ship, VesselType.Station };
        public List<Orbit.PatchTransitionType> SOITransitions = new List<Orbit.PatchTransitionType> { Orbit.PatchTransitionType.ENCOUNTER, Orbit.PatchTransitionType.ESCAPE };
        

        //Toolbar Integration
        internal Boolean BlizzyToolbarIsAvailable = false;

        internal ButtonStyleEnum ButtonStyleToDisplay {
            get {
                if (BlizzyToolbarIsAvailable || ButtonStyleChosen != ButtonStyleEnum.Toolbar)
                    return ButtonStyleChosen;
                else
                    return ButtonStyleEnum.Launcher;
            }
        }
        [Persistent] internal ButtonStyleEnum ButtonStyleChosen = ButtonStyleEnum.Launcher;

        internal enum ButtonStyleEnum
        {
            [Description("Basic button")]                       Basic,
            [Description("Common Toolbar (by Blizzy78)")]       Toolbar,
            [Description("KSP App Launcher Button")]            Launcher,
        }


        //Click through protection
        [Persistent] internal Boolean ClickThroughProtect_KSC=true;
        [Persistent] internal Boolean ClickThroughProtect_Tracking=true;
        [Persistent] internal Boolean ClickThroughProtect_Editor=true;
        [Persistent] internal Boolean ClickThroughProtect_Flight=true;


        [Persistent] internal CalendarTypeEnum SelectedCalendar = CalendarTypeEnum.KSPStock;
        [Persistent] internal String EarthEpoch = "1951-01-01";

        [Persistent] internal Boolean ShowCalendarToggle = false;
        internal Boolean RSSActive = false;
        [Persistent] internal Boolean RSSShowCalendarToggled=false;

        [Persistent] internal Int32 AppLauncherSetTrueTimeOut= 6;

        //Version Stuff
        [Persistent] internal Boolean DailyVersionCheck = true;
        internal Boolean VersionAttentionFlag = false;
        //When did we last check??
        internal DateTime VersionCheckDate_Attempt = new DateTime();
        [Persistent] internal String VersionCheckDate_AttemptStored;
        public String VersionCheckDate_AttemptString { get { return ConvertVersionCheckDateToString(this.VersionCheckDate_Attempt); } }
        internal DateTime VersionCheckDate_Success = new DateTime();
        [Persistent] internal String VersionCheckDate_SuccessStored;
        public String VersionCheckDate_SuccessString { get { return ConvertVersionCheckDateToString(this.VersionCheckDate_Success); } }

        public override void OnEncodeToConfigNode()
        {
            WindowPositionStored = WindowPositionStored.FromRect(WindowPos);
            WindowPos_SpaceCenterStored = WindowPos_SpaceCenterStored.FromRect(WindowPos_SpaceCenter);
            WindowPos_TrackingStationStored = WindowPos_TrackingStationStored.FromRect(WindowPos_TrackingStation);
            WindowPos_EditorStored = WindowPos_EditorStored.FromRect(WindowPos_Editor);
            VersionCheckDate_AttemptStored = VersionCheckDate_AttemptString;
            VersionCheckDate_SuccessStored = VersionCheckDate_SuccessString;
        }
        public override void OnDecodeFromConfigNode()
        {
            WindowPos = WindowPositionStored.ToRect();
            WindowPos_SpaceCenter = WindowPos_SpaceCenterStored.ToRect();
            WindowPos_TrackingStation = WindowPos_TrackingStationStored.ToRect();
            WindowPos_Editor = WindowPos_EditorStored.ToRect();
            DateTime.TryParseExact(VersionCheckDate_AttemptStored, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out VersionCheckDate_Attempt);
            DateTime.TryParseExact(VersionCheckDate_SuccessStored, "yyyy-MM-dd", null ,System.Globalization.DateTimeStyles.None, out VersionCheckDate_Success);
        }

        internal enum DisplaySkin
        {
            [Description("KSP Style")]          Default,
            [Description("Unity Style")]        Unity,
            [Description("Unity/KSP Buttons")]  UnityWKSPButtons
        }

        #region Version Checks
        private String VersionCheckURL = "http://triggerau.github.io/KerbalAlarmClock/versioncheck.txt";
        //Could use this one to see usage, but need to be very aware of data connectivity if its ever used "http://bit.ly/KACVersion";

        private String ConvertVersionCheckDateToString(DateTime Date)
        {
            if (Date < DateTime.Now.AddYears(-10))
                return "No Date Recorded";
            else
                return String.Format("{0:yyyy-MM-dd}", Date);
        }

        public String Version = "";
        [Persistent] public String VersionWeb = "";
        public Boolean VersionAvailable
        {
            get
            {
                //todo take this out
                if (this.VersionWeb == "")
                    return false;
                else
                    try
                    {
                        //if there was a string and its version is greater than the current running one then alert
                        System.Version vTest = new System.Version(this.VersionWeb);
                        return (System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.CompareTo(vTest) < 0);
                    }
                    catch (Exception ex)
                    {
                        MonoBehaviourExtended.LogFormatted("webversion: '{0}'", this.VersionWeb);
                        MonoBehaviourExtended.LogFormatted("Unable to compare versions: {0}", ex.Message);
                        return false;
                    }

                //return ((this.VersionWeb != "") && (this.Version != this.VersionWeb));
            }
        }

        public String VersionCheckResult = "";


        /// <summary>
        /// Does some logic to see if a check is needed, and returns true if there is a different version
        /// </summary>
        /// <param name="ForceCheck">Ignore all logic and simply do a check</param>
        /// <returns></returns>
        public Boolean VersionCheck(Boolean ForceCheck)
        {
            Boolean blnReturn = false;
            Boolean blnDoCheck = false;

            try
            {
                if (ForceCheck)
                {
                    blnDoCheck = true;
                    MonoBehaviourExtended.LogFormatted("Starting Version Check-Forced");
                }
                //else if (this.VersionWeb == "")
                //{
                //    blnDoCheck = true;
                //    MonoBehaviourExtended.LogFormatted("Starting Version Check-No current web version stored");
                //}
                else if (this.VersionCheckDate_Attempt < DateTime.Now.AddYears(-9))
                {
                    blnDoCheck = true;
                    MonoBehaviourExtended.LogFormatted("Starting Version Check-No current date stored");
                }
                else if (this.VersionCheckDate_Attempt.Date != DateTime.Now.Date)
                {
                    blnDoCheck = true;
                    MonoBehaviourExtended.LogFormatted("Starting Version Check-stored date is not today");
                }
                else
                    MonoBehaviourExtended.LogFormatted("Skipping version check");


                if (blnDoCheck)
                {
                    //prep the background thread
                    bwVersionCheck = new BackgroundWorker();
                    bwVersionCheck.DoWork += bwVersionCheck_DoWork;
                    bwVersionCheck.RunWorkerCompleted += bwVersionCheck_RunWorkerCompleted;

                    //fire the worker
                    bwVersionCheck.RunWorkerAsync();
                }
                blnReturn = true;
            }
            catch (Exception ex)
            {
                MonoBehaviourExtended.LogFormatted("Failed to run the update test");
                MonoBehaviourExtended.LogFormatted(ex.Message);
            }
            return blnReturn;
        }

        internal Boolean VersionCheckRunning = false;
        BackgroundWorker bwVersionCheck;
        WWW wwwVersionCheck;

        void bwVersionCheck_DoWork(object sender, DoWorkEventArgs e)
        {
            //set initial stuff and save it
            VersionCheckRunning = true;
            this.VersionCheckResult = "Unknown - check again later";
            this.VersionCheckDate_Attempt = DateTime.Now;
            this.Save();

            //now do the download
            MonoBehaviourExtended.LogFormatted("Reading version from Web");
            wwwVersionCheck = new WWW(VersionCheckURL);
            while (!wwwVersionCheck.isDone) { }
            MonoBehaviourExtended.LogFormatted("Download complete:{0}", wwwVersionCheck.text.Length);
            VersionCheckRunning = false;
        }

        void bwVersionCheck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //get the response from the variable and work with it
                //Parse it for the version String
                String strFile = wwwVersionCheck.text;
                MonoBehaviourExtended.LogFormatted("Response Length:" + strFile.Length);
                MonoBehaviourExtended.LogFormatted("File:{0}" , strFile);

                Match matchVersion;
                matchVersion = Regex.Match(strFile, "(?<=\\|LATESTVERSION\\|).+(?=\\|LATESTVERSION\\|)", System.Text.RegularExpressions.RegexOptions.Singleline);
                MonoBehaviourExtended.LogFormatted("Got Version '" + matchVersion.ToString() + "'");

                String strVersionWeb = matchVersion.ToString();
                if (strVersionWeb != "")
                {
                    this.VersionCheckResult = "Success";
                    this.VersionCheckDate_Success = DateTime.Now;
                    this.VersionWeb = strVersionWeb;
                }
                else
                {
                    this.VersionCheckResult = "Unable to parse web service";
                }
            }
            catch (Exception ex)
            {
                MonoBehaviourExtended.LogFormatted("Failed to read Version info from web");
                MonoBehaviourExtended.LogFormatted(ex.Message);

            }
            MonoBehaviourExtended.LogFormatted("Version Check result:" + VersionCheckResult);

            this.Save();
            VersionAttentionFlag = VersionAvailable;
        }


        //public Boolean getLatestVersion()
        //{
        //    Boolean blnReturn = false;
        //    try
        //    {
        //        //Get the file from Codeplex
        //        this.VersionCheckResult = "Unknown - check again later";
        //        this.VersionCheckDate_Attempt = DateTime.Now;

        //        MonoBehaviourExtended.LogFormatted("Reading version from Web");
        //        //Page content FormatException is |LATESTVERSION|1.2.0.0|LATESTVERSION|
        //        //WWW www = new WWW("http://kerbalalarmclock.codeplex.com/wikipage?title=LatestVersion");
        //        WWW www = new WWW("https://sites.google.com/site/kerbalalarmclock/latestversion");
        //        while (!www.isDone) { }

        //        //Parse it for the version String
        //        String strFile = www.text;
        //        MonoBehaviourExtended.LogFormatted("Response Length:" + strFile.Length);

        //        Match matchVersion;
        //        matchVersion = Regex.Match(strFile, "(?<=\\|LATESTVERSION\\|).+(?=\\|LATESTVERSION\\|)", System.Text.RegularExpressions.RegexOptions.Singleline);
        //        MonoBehaviourExtended.LogFormatted("Got Version '" + matchVersion.ToString() + "'");

        //        String strVersionWeb = matchVersion.ToString();
        //        if (strVersionWeb != "")
        //        {
        //            this.VersionCheckResult = "Success";
        //            this.VersionCheckDate_Success = DateTime.Now;
        //            this.VersionWeb = strVersionWeb;
        //            blnReturn = true;
        //        }
        //        else
        //        {
        //            this.VersionCheckResult = "Unable to parse web service";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MonoBehaviourExtended.LogFormatted("Failed to read Version info from web");
        //        MonoBehaviourExtended.LogFormatted(ex.Message);

        //    }
        //    MonoBehaviourExtended.LogFormatted("Version Check result:" + VersionCheckResult);
        //    return blnReturn;
        //}
        #endregion
    }

    internal class RectStorage:ConfigNodeStorage
    {
        [Persistent] internal Single x,y,width,height;

        internal Rect ToRect() { return new Rect(x, y, width, height); }
        internal RectStorage FromRect(Rect rectToStore)
        {
            this.x = rectToStore.x;
            this.y = rectToStore.y;
            this.width = rectToStore.width;
            this.height = rectToStore.height;
            return this;
        }
    }

    internal enum MiminalDisplayType
    {
        NextAlarm = 0,
        OldestAlarm = 1
    }

}
