using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

using UnityEngine;
using KSP;

namespace KerbalAlarmClock
{
    public enum MiminalDisplayType
    {
        NextAlarm=0,
        OldestAlarm=1
    }

    /// <summary>
    /// Settings object
    /// </summary>
    public class KACSettings
    {
        public String Version="";
        public String VersionWeb="";
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
                        KACWorker.DebugLogFormatted("webversion: '{0}'", this.VersionWeb);
                        KACWorker.DebugLogFormatted("Unable to compare versions: {0}", ex.Message);
                        return false;
                    }

                //return ((this.VersionWeb != "") && (this.Version != this.VersionWeb));
            }
        }

        //Are we doing daily checks
        public Boolean DailyVersionCheck = true;
        public String VersionCheckResult = "";
        //attentionflag
        public Boolean VersionAttentionFlag = false;
        //When did we last check??
        public DateTime VersionCheckDate_Attempt;
        public String VersionCheckDate_AttemptString { get { return ConvertVersionCheckDateToString(this.VersionCheckDate_Attempt); } }
        public DateTime VersionCheckDate_Success;
        public String VersionCheckDate_SuccessString { get { return ConvertVersionCheckDateToString(this.VersionCheckDate_Success); } }

        private String ConvertVersionCheckDateToString(DateTime Date)
        {
            if (Date < DateTime.Now.AddYears(-10))
                return "No Date Recorded";
            else
                return String.Format("{0:yyyy-MM-dd}", Date);
        }

        public Boolean WindowVisible = false;
        public Boolean WindowMinimized = false;
        public Rect WindowPos = new Rect(3, 55, 300, 45);

        public Boolean WindowVisible_SpaceCenter = false;
        public Boolean WindowMinimized_SpaceCenter = false;
        public Rect WindowPos_SpaceCenter = new Rect(3, 36, 300, 45);

        public Boolean WindowVisible_TrackingStation = false;
        public Boolean WindowMinimized_TrackingStation = false;
        public Rect WindowPos_TrackingStation = new Rect(202, 45, 300, 45);

        public Rect IconPos;
        public Rect IconPos_SpaceCenter;
        public Boolean IconShow_SpaceCenter = true;
        public Rect IconPos_TrackingStation;
        public Boolean IconShow_TrackingStation = true;

        public Boolean UseBlizzyToolbarIfAvailable = true;
        public MiminalDisplayType WindowMinimizedType = MiminalDisplayType.NextAlarm;

        public int BehaviourChecksPerSec = 10;
        public int BehaviourChecksPerSec_Custom = 40;

        public KACAlarmList Alarms = new KACAlarmList();

        public Boolean AllowJumpFromViewOnly = true;
        public Boolean BackupSaves = true;
        public Boolean CancelFlightModeJumpOnBackupFailure = false;
        public int BackupSavesToKeep = 20;

        public String AlarmListMaxAlarms="10";
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

        public int AlarmDefaultAction = 1;
        public double AlarmDefaultMargin = 60;
        public int AlarmPosition = 1;
        public Boolean AlarmDeleteOnClose = false;
        public Boolean HideOnPause = true;
        //public Boolean TimeAsUT = false;
        public KACTime.PrintTimeFormat TimeFormat = KACTime.PrintTimeFormat.KSPString;
        public Boolean ShowTooltips = true;
        public Boolean ShowEarthTime = false;

        public Boolean AlarmXferRecalc = true;
        public double AlarmXferRecalcThreshold = 180;
        public Boolean AlarmXferDisplayList = false;
        public Boolean XferModelLoadData = true;
        public Boolean XferModelDataLoaded = false;
        public Boolean XferUseModelData = false;

        public Boolean AlarmNodeRecalc = false;
        public double AlarmNodeRecalcThreshold = 180;

        public Boolean AlarmAddSOIAuto = false;
        public double AlarmAddSOIAutoThreshold = 180;
        public double AlarmAutoSOIMargin = 900;

        public Boolean AlarmAddSOIAuto_ExcludeEVA = true;

        public Boolean AlarmSOIRecalc = false;
        public double AlarmSOIRecalcThreshold = 180;

        public Boolean AlarmAddManAuto = false;
        public Boolean AlarmAddManAuto_andRemove = false;
        public double AlarmAddManAutoMargin = 180;
        public double AlarmAddManAutoThreshold = 180;
        public int AlarmAddManAuto_Action = 1;

        //public double AlarmAddSOIMargin = 120;
        public Boolean AlarmCatchSOIChange = false;
        public int AlarmOnSOIChange_Action = 1;

        public Boolean AlarmCrewDefaultStoreNode = false;

        //Strings to store objects to reset after ship switch;
        public String LoadManNode = "";
        public string LoadVesselTarget = "";
        
        public List<GameScenes> DrawScenes = new List<GameScenes> { GameScenes.FLIGHT,GameScenes.SPACECENTER,GameScenes.TRACKSTATION };
        public List<GameScenes> BehaviourScenes = new List<GameScenes> { GameScenes.FLIGHT };
        public List<VesselType> VesselTypesForSOI = new List<VesselType>() { VesselType.Base, VesselType.Lander, VesselType.Probe, VesselType.Ship, VesselType.Station };
        public List<Orbit.PatchTransitionType> SOITransitions = new List<Orbit.PatchTransitionType> { Orbit.PatchTransitionType.ENCOUNTER, Orbit.PatchTransitionType.ESCAPE };

        public KACSettings()
        {
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //on each start set the attention flag to the property - should be on each program start
            VersionAttentionFlag=VersionAvailable;
        }

        public void Load()
        {
            try
            {
                KACWorker.DebugLogFormatted("Loading Config");
                 KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<KerbalAlarmClock>();
                configfile.load();

                this.DailyVersionCheck = configfile.GetValue("DailyUpdateCheck", true);

                try { this.VersionCheckDate_Attempt = DateTime.ParseExact(configfile.GetValue("VersionCheckDate_Attempt", ""), "yyyy-MM-dd", CultureInfo.CurrentCulture); }
                catch (Exception) { this.VersionCheckDate_Attempt = new DateTime();}

                try { this.VersionCheckDate_Success = DateTime.ParseExact(configfile.GetValue("VersionCheckDate_Success", ""), "yyyy-MM-dd", CultureInfo.CurrentCulture); }
                catch (Exception) { this.VersionCheckDate_Success = new DateTime(); }
                
                this.VersionWeb = configfile.GetValue("VersionWeb", "");

                this.WindowVisible = configfile.GetValue("WindowVisible", false);
                this.WindowMinimized = configfile.GetValue("WindowMinimized", false);
                this.WindowPos = configfile.GetValue<Rect>("WindowPos", new Rect(3, 55, 300, 45));

                this.WindowVisible_SpaceCenter = configfile.GetValue("WindowVisible_SpaceCenter", false);
                this.WindowMinimized_SpaceCenter = configfile.GetValue("WindowMinimized_SpaceCenter", false);
                this.WindowPos_SpaceCenter = configfile.GetValue<Rect>("WindowPos_SpaceCenter", new Rect(3, 36, 300, 45));

                this.WindowVisible_TrackingStation = configfile.GetValue("WindowVisible_TrackingStation", false);
                this.WindowMinimized_TrackingStation = configfile.GetValue("WindowMinimized_TrackingStation", false);
                this.WindowPos_TrackingStation = configfile.GetValue<Rect>("WindowPos_TrackingStation", new Rect(202, 45, 300, 45));

                this.IconPos = configfile.GetValue<Rect>("IconPos", new Rect(152, 0, 32, 32));
                this.IconPos.height = 32; this.IconPos.width = 32;
                this.IconPos_SpaceCenter = configfile.GetValue<Rect>("IconPos_SpaceCenter", new Rect(3, 3, 32, 32));
                this.IconPos_SpaceCenter.height = 32; this.IconPos_TrackingStation.width = 32;
                this.IconPos_TrackingStation = configfile.GetValue<Rect>("IconPos_TrackingStation", new Rect(202, 0, 32, 32));
                this.IconPos_TrackingStation.height = 32; this.IconPos_TrackingStation.width = 32;

                this.IconShow_SpaceCenter = configfile.GetValue("IconShow_SpaceCenter", true);
                this.IconShow_TrackingStation = configfile.GetValue("IconShow_TrackingStation", true);

                this.UseBlizzyToolbarIfAvailable = configfile.GetValue<Boolean>("UseBlizzyToolbarIfAvailable", true);
                this.WindowMinimizedType = (MiminalDisplayType) configfile.GetValue("WindowMinimizedType", 0);

                this.BehaviourChecksPerSec = configfile.GetValue("BehaviourChecksPerSec", 10);
                this.BehaviourChecksPerSec_Custom = configfile.GetValue("BehaviourChecksPerSecCustom",40);

                this.BackupSaves = configfile.GetValue("BackupSaves",true);
                this.BackupSavesToKeep = configfile.GetValue("BackupSavesToKeep",20);
                this.CancelFlightModeJumpOnBackupFailure = configfile.GetValue("CancelFlightModeJumpOnBackupFailure", false);
                
                this.AllowJumpFromViewOnly = configfile.GetValue("AllowJumpFromViewOnly", true);

                this.AlarmListMaxAlarms = configfile.GetValue("AlarmListMaxAlarms", "10");
                this.AlarmDefaultAction = configfile.GetValue<int>("AlarmDefaultAction", 1);
                this.AlarmDefaultMargin = configfile.GetValue<Double>("AlarmDefaultMargin", 60);
                this.AlarmPosition = configfile.GetValue<int>("AlarmPosition", 1);
                this.AlarmDeleteOnClose = configfile.GetValue("AlarmDeleteOnClose", false);
                this.ShowTooltips = configfile.GetValue("ShowTooltips", true);
                this.ShowEarthTime = configfile.GetValue("ShowEarthTime", false);
                this.HideOnPause = configfile.GetValue("HideOnPause", true);
                String strTimeFormat = configfile.GetValue("TimeFormat", "KSPString");
                //KACWorker.DebugLogFormatted("{0}",strTimeFormat);
                this.TimeFormat = (KACTime.PrintTimeFormat)Enum.Parse(typeof(KACTime.PrintTimeFormat), strTimeFormat);
                //this.TimeFormat = configfile.GetValue<KACTime.PrintTimeFormat>("TimeFormat", KACTime.PrintTimeFormat.KSPString);
                //KACWorker.DebugLogFormatted("{0}",this.TimeFormat.ToString());
                if (configfile.GetValue<bool>("TimeAsUT", false) == true)
                {
                    KACWorker.DebugLogFormatted("Forcing New Format");
                    this.TimeFormat = KACTime.PrintTimeFormat.TimeAsUT;
                    configfile.SetValue("TimeAsUT", false);
                    configfile.SetValue("TimeFormat", Enum.GetName(typeof(KACTime.PrintTimeFormat), this.TimeFormat));
                    configfile.save();
                }

                this.AlarmXferRecalc = configfile.GetValue("AlarmXferRecalc", true);
                this.AlarmXferRecalcThreshold = configfile.GetValue<Double>("AlarmXferRecalcThreshold", 180);
                this.AlarmXferDisplayList = configfile.GetValue("AlarmXferDisplayList", false);
                this.XferUseModelData = configfile.GetValue("XferUseModelData", false);

                this.AlarmNodeRecalc = configfile.GetValue("AlarmNodeRecalc", false);
                this.AlarmNodeRecalcThreshold = configfile.GetValue<Double>("AlarmNodeRecalcThreshold", 180);
                
                this.AlarmAddSOIAuto = configfile.GetValue("AlarmAddSOIAuto", false);
                this.AlarmAddSOIAutoThreshold = configfile.GetValue<Double>("AlarmAddSOIAutoThreshold", 180);
                //this.AlarmAddSOIMargin = configfile.GetValue("AlarmAddSOIMargin", 120);
                this.AlarmAutoSOIMargin = configfile.GetValue<Double>("AlarmAutoSOIMargin", 900);
                this.AlarmAddSOIAuto_ExcludeEVA = configfile.GetValue("AlarmAddSOIAuto_ExcludeEVA", true);
                this.AlarmCatchSOIChange = configfile.GetValue("AlarmOnSOIChange", false);
                this.AlarmOnSOIChange_Action = configfile.GetValue("AlarmOnSOIChange_Action", 1);

                this.AlarmSOIRecalc = configfile.GetValue("AlarmSOIRecalc", false);
                this.AlarmSOIRecalcThreshold = configfile.GetValue<Double>("AlarmSOIRecalcThreshold", 180);

                this.AlarmAddManAuto = configfile.GetValue("AlarmAddManAuto", false);
                this.AlarmAddManAuto_andRemove = configfile.GetValue("AlarmAddManAuto_andRemove", false);
                this.AlarmAddManAutoThreshold = configfile.GetValue("AlarmAddManAutoThreshold", 180);
                this.AlarmAddManAutoMargin = configfile.GetValue("AlarmAddManAutoMargin", 180);
                this.AlarmAddManAuto_Action = configfile.GetValue("AlarmAddManAuto_Action", 1);

                this.AlarmCrewDefaultStoreNode = configfile.GetValue("AlarmCrewDefaultStoreNode", false);

                this.LoadManNode = configfile.GetValue("LoadManNode", "");
                this.LoadVesselTarget = configfile.GetValue("LoadVesselTarget", "");

                //HIGHLOGIC IS NOT YET SET HERE!!!
                if (KSP.IO.File.Exists<KerbalAlarmClock>(String.Format("Alarms-{0}.txt", HighLogic.CurrentGame.Title)))
                {
                    KACWorker.DebugLogFormatted("Trying New Alarms file..."); 
                    LoadAlarms();
                }
                else
                {
                    //Loop through numbers to Load Alarms
                    Alarms = new KACAlarmList();
                    int intAlarm = 0;
                    String strAlarm = "";
                    do
                    {
                        strAlarm = configfile.GetValue("Alarm_" + intAlarm, "");
                        KACWorker.DebugLogFormatted(strAlarm);
                        if (strAlarm != "")
                        {
                            KACAlarm tmpAlarm = new KACAlarm();
                            tmpAlarm.LoadFromString(strAlarm);
                            Alarms.Add(tmpAlarm);
                            intAlarm++;
                        }
                    } while (strAlarm != "");
                }
                KACWorker.DebugLogFormatted("Config Loaded Successfully");
            }

            catch (Exception ex)
            {
                KACWorker.DebugLogFormatted("Failed To Load Config");
                KACWorker.DebugLogFormatted(ex.Message);
            }

        }

        private void LoadAlarms()
        {
            string AlarmsFileVersion = "2";
            Alarms = new KACAlarmList();
            KSP.IO.TextReader tr = KSP.IO.TextReader.CreateForType<KerbalAlarmClock>(String.Format("Alarms-{0}.txt", HighLogic.CurrentGame.Title));
            String strFile = tr.ReadToEnd();
            tr.Close();

            while (strFile.Contains("|<ENDLINE>"))
	        {
                String strAlarm = strFile.Substring(0, strFile.IndexOf("|<ENDLINE>"));
                strFile = strFile.Substring(strAlarm.Length + "|<ENDLINE>".Length).TrimStart("\r\n".ToCharArray());

                if (strAlarm.StartsWith("AlarmsFileVersion|"))
                {
                    AlarmsFileVersion = strAlarm.Split("|".ToCharArray())[1];
                    KACWorker.DebugLogFormatted("AlarmsFileVersion:{0}", AlarmsFileVersion);
                }
                else if (!strAlarm.StartsWith("VesselID|"))
                {
                    KACAlarm tmpAlarm = new KACAlarm();

                    switch (AlarmsFileVersion)
                    {
                        case "3":
                            tmpAlarm.LoadFromString3(strAlarm,KACWorkerGameState.CurrentTime.UT);
                            break;
                        default:
                            tmpAlarm.LoadFromString2(strAlarm);
                            break;
                    }
                    
                    Alarms.Add(tmpAlarm);
                }
	        }
        }

        public void Save()
        {

            SaveConfig();

            SaveLoadObjects();

            SaveAlarms();
        }

        public void SaveConfig()
        {
            KACWorker.DebugLogFormatted("Saving Config");

            KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<KerbalAlarmClock>();
            configfile.load();

            configfile.SetValue("DailyUpdateCheck", this.DailyVersionCheck);
            configfile.SetValue("VersionCheckDate_Attempt", this.VersionCheckDate_AttemptString);
            configfile.SetValue("VersionCheckDate_Success", this.VersionCheckDate_SuccessString);
            configfile.SetValue("VersionWeb", this.VersionWeb);

            configfile.SetValue("WindowVisible", this.WindowVisible);
            configfile.SetValue("WindowMinimized", this.WindowMinimized);
            configfile.SetValue("WindowPos", this.WindowPos);

            configfile.SetValue("WindowVisible_SpaceCenter", this.WindowVisible_SpaceCenter);
            configfile.SetValue("WindowMinimized_SpaceCenter", this.WindowMinimized_SpaceCenter);
            configfile.SetValue("WindowPos_SpaceCenter", this.WindowPos_SpaceCenter);

            configfile.SetValue("WindowVisible_TrackingStation", this.WindowVisible_TrackingStation);
            configfile.SetValue("WindowMinimized_TrackingStation", this.WindowMinimized_TrackingStation);
            configfile.SetValue("WindowPos_TrackingStation", this.WindowPos_TrackingStation);

            configfile.SetValue("IconPos", this.IconPos);
            configfile.SetValue("IconPos_SpaceCenter", this.IconPos_SpaceCenter);
            configfile.SetValue("IconShow_SpaceCenter", this.IconShow_SpaceCenter);
            configfile.SetValue("IconPos_TrackingStation", this.IconPos_TrackingStation);
            configfile.SetValue("IconShow_TrackingStation", this.IconShow_TrackingStation);

            configfile.SetValue("UseBlizzyToolbarIfAvailable", this.UseBlizzyToolbarIfAvailable);
            configfile.SetValue("WindowMinimizedType", (int)this.WindowMinimizedType);

            configfile.SetValue("BehaviourChecksPerSec", this.BehaviourChecksPerSec);

            configfile.SetValue("BackupSaves", this.BackupSaves);
            configfile.SetValue("BackupSavesToKeep", this.BackupSavesToKeep);
            configfile.SetValue("CancelFlightModeJumpOnBackupFailure", this.CancelFlightModeJumpOnBackupFailure);
            
            configfile.SetValue("AllowJumpFromViewOnly", this.AllowJumpFromViewOnly);

            configfile.SetValue("AlarmListMaxAlarms", this.AlarmListMaxAlarms);
            configfile.SetValue("AlarmPosition", this.AlarmPosition);
            configfile.SetValue("AlarmDefaultAction", this.AlarmDefaultAction);
            configfile.SetValue("AlarmDefaultMargin", this.AlarmDefaultMargin);
            configfile.SetValue("AlarmDeleteOnClose", this.AlarmDeleteOnClose);
            configfile.SetValue("ShowTooltips", this.ShowTooltips);
            configfile.SetValue("ShowEarthTime", this.ShowEarthTime);
            configfile.SetValue("HideOnPause", this.HideOnPause);
            configfile.SetValue("TimeFormat", Enum.GetName(typeof(KACTime.PrintTimeFormat), this.TimeFormat));

            configfile.SetValue("AlarmXferRecalc", this.AlarmXferRecalc);
            configfile.SetValue("AlarmXferRecalcThreshold", this.AlarmXferRecalcThreshold);
            configfile.SetValue("AlarmXferDisplayList", this.AlarmXferDisplayList);
            configfile.SetValue("XferUseModelData", this.XferUseModelData);

            configfile.SetValue("AlarmNodeRecalc", this.AlarmNodeRecalc);
            configfile.SetValue("AlarmNodeRecalcThreshold", this.AlarmNodeRecalcThreshold);

            configfile.SetValue("AlarmAddSOIAuto", this.AlarmAddSOIAuto);
            configfile.SetValue("AlarmAddSOIAutoThreshold", this.AlarmAddSOIAutoThreshold);
            //configfile.SetValue("AlarmAddSOIMargin", this.AlarmAddSOIMargin);
            configfile.SetValue("AlarmAutoSOIMargin", this.AlarmAutoSOIMargin);
            configfile.SetValue("AlarmAddSOIAuto_ExcludeEVA", this.AlarmAddSOIAuto_ExcludeEVA);
            configfile.SetValue("AlarmOnSOIChange", this.AlarmCatchSOIChange);
            configfile.SetValue("AlarmOnSOIChange_Action", this.AlarmOnSOIChange_Action);

            configfile.SetValue("AlarmSOIRecalc", this.AlarmSOIRecalc);
            configfile.SetValue("AlarmSOIRecalcThreshold", this.AlarmSOIRecalcThreshold);

            configfile.SetValue("AlarmAddManAuto", this.AlarmAddManAuto);
            configfile.SetValue("AlarmAddManAuto_andRemove", this.AlarmAddManAuto_andRemove);
            configfile.SetValue("AlarmAddManAutoThreshold", this.AlarmAddManAutoThreshold);
            configfile.SetValue("AlarmAddManAutoMargin", this.AlarmAddManAutoMargin);
            configfile.SetValue("AlarmAddManAuto_Action", this.AlarmAddManAuto_Action);

            configfile.SetValue("AlarmCrewDefaultStoreNode", this.AlarmCrewDefaultStoreNode);

            configfile.save();
            KACWorker.DebugLogFormatted("Saved Config");

        }

        public void SaveLoadObjects()
        {
            KACWorker.DebugLogFormatted("Saving Load Objects");
            KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<KerbalAlarmClock>();
            configfile.load();
            configfile.SetValue("LoadManNode", this.LoadManNode);
            configfile.SetValue("LoadVesselTarget", this.LoadVesselTarget);
            configfile.save();
            KACWorker.DebugLogFormatted("Saved Load Objects");
        }

        public void SaveAlarms2()
        {
            KACWorker.DebugLogFormatted("Saving Alarms");
            KSP.IO.TextWriter tw = KSP.IO.TextWriter.CreateForType<KerbalAlarmClock>(String.Format("Alarms-{0}.txt", HighLogic.CurrentGame.Title));
            //Write the header
            tw.WriteLine("VesselID|Name|Notes|AlarmTime.UT|AlarmMarginSecs|Type|Enabled|HaltWarp|PauseGame|Options-Manuever/Xfer/Target|<ENDLINE>");
            foreach (KACAlarm tmpAlarm in Alarms.BySaveName(HighLogic.CurrentGame.Title))
            {
                //Now Write Each alarm
                //tw.WriteLine(tmpAlarm.SerializeString2() + "|<ENDLINE>");
                tw.WriteLine(tmpAlarm.SerializeString3() + "|<ENDLINE>");
            }
            //And close the file
            tw.Close();
            KACWorker.DebugLogFormatted("Saved Alarms");
        }

        public void SaveAlarms()
        {
            KACWorker.DebugLogFormatted("Saving Alarms-v3");

            KSP.IO.TextWriter tw = KSP.IO.TextWriter.CreateForType<KerbalAlarmClock>(String.Format("Alarms-{0}.txt", HighLogic.CurrentGame.Title));
            //Write the header
            tw.WriteLine("AlarmsFileVersion|3|<ENDLINE>");
            tw.WriteLine("VesselID|Name|Notes|AlarmTime.UT|AlarmMarginSecs|Type|Enabled|HaltWarp|PauseGame|ActionedAt|Manuever|Xfer|Target|Options|<ENDLINE>");
            foreach (KACAlarm tmpAlarm in Alarms.BySaveName(HighLogic.CurrentGame.Title))
            {
                //Now Write Each alarm
                tw.WriteLine(tmpAlarm.SerializeString3() + "|<ENDLINE>");
            }
            //And close the file
            tw.Close();
        }

        public Boolean getLatestVersion()
        {
            Boolean blnReturn = false;
            try 
            {
                //Get the file from Codeplex
                this.VersionCheckResult = "Unknown - check again later";
                this.VersionCheckDate_Attempt = DateTime.Now;

                KACWorker.DebugLogFormatted("Reading version from Web");
                //Page content FormatException is |LATESTVERSION|1.2.0.0|LATESTVERSION|
                WWW www = new WWW("http://kerbalalarmclock.codeplex.com/wikipage?title=LatestVersion");
                while (!www.isDone) { }

                //Parse it for the version String
                String strFile = www.text;
                KACWorker.DebugLogFormatted("Response Length:" + strFile.Length);

                Match matchVersion;
                matchVersion = Regex.Match(strFile, "(?<=\\|LATESTVERSION\\|).+(?=\\|LATESTVERSION\\|)", System.Text.RegularExpressions.RegexOptions.Singleline);
                KACWorker.DebugLogFormatted("Got Version '" + matchVersion.ToString() + "'");

                String strVersionWeb = matchVersion.ToString();
                if (strVersionWeb != "")
                {
                    this.VersionCheckResult = "Success";
                    this.VersionCheckDate_Success = DateTime.Now;
                    this.VersionWeb = strVersionWeb;
                    blnReturn = true;
                } else
                {
                    this.VersionCheckResult = "Unable to parse web service";
                }
	        }
	        catch (Exception ex)
	        {
                KACWorker.DebugLogFormatted("Failed to read Version info from web");
                KACWorker.DebugLogFormatted(ex.Message);
                
	        }
            KACWorker.DebugLogFormatted("Version Check result:" + VersionCheckResult);
            return blnReturn;
        }

        /// <summary>
        /// Does some logic to see if a check is needed, and returns true if there is a different version
        /// </summary>
        /// <param name="ForceCheck">Ignore all logic and simply do a check</param>
        /// <returns></returns>
        public Boolean VersionCheck(Boolean ForceCheck)
        {
            Boolean blnReturn = false;
            Boolean blnDoCheck =false;

            try
            {
                if (ForceCheck)
                {
                    blnDoCheck = true;
                    KACWorker.DebugLogFormatted("Starting Version Check-Forced");
                } 
                else if (this.VersionWeb=="")
                {
                    blnDoCheck = true;
                    KACWorker.DebugLogFormatted("Starting Version Check-No current web version stored");
                }
                else if (this.VersionCheckDate_Success<DateTime.Now.AddYears(-9))
                {
                    blnDoCheck = true;
                    KACWorker.DebugLogFormatted("Starting Version Check-No current date stored");
                }
                else if (this.VersionCheckDate_Success.Date!=DateTime.Now.Date)
                {
                    blnDoCheck = true;
                    KACWorker.DebugLogFormatted("Starting Version Check-stored date is not today");
                }
                else
                    KACWorker.DebugLogFormatted("Skipping version check");
            

                if (blnDoCheck)
                {
                    getLatestVersion();
                    this.Save();
                    //if (getLatestVersion())
                    //{
                    //    //save all the details to the file
                    //    this.Save();
                    //}
                    //if theres a new version then set the flag
                    VersionAttentionFlag = VersionAvailable;
                }
                blnReturn = true;
            }
            catch (Exception ex)
            {
                KACWorker.DebugLogFormatted("Failed to run the update test");
                KACWorker.DebugLogFormatted(ex.Message);
            }
            return blnReturn;
        }
    }

}

