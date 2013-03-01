using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;
using KSP;

namespace KerbalAlarmClock
{
    public static class KACUtils
    {
        //generic function
        public static String PipeSepVariables(params object[] vars)
        {
            String strReturn = "";
            foreach (object tmpVar in vars)
            {
                if (strReturn != "") strReturn += "|";
                if (tmpVar == null)
                    strReturn += "";
                else
                    strReturn += tmpVar.ToString();
            }
            return strReturn;
        }
        
        public static Byte[] LoadFileToArray(String Filename)
        {
            Byte[] arrBytes;

            arrBytes = KSP.IO.File.ReadAllBytes<KerbalAlarmClock>(Filename);

            return arrBytes;
        }

    }

    public static class KACResources
    {
        #region "Textures"
        public static Texture2D iconNorm = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconNormShow = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconAlarm = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconAlarmShow = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpEffect100 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpEffect080 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpEffect060 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpEffect040 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpEffect020 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpEffect000 = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        public static Texture2D iconPauseEffect100 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseEffect080 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseEffect060 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseEffect040 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseEffect020 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseEffect000 = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        public static Texture2D iconWarpList100 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpList080 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpList060 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpList040 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpList020 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpList000 = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        public static Texture2D iconPauseList100 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseList080 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseList060 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseList040 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseList020 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseList000 = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        public static Texture2D iconSOI = new Texture2D(18, 14, TextureFormat.ARGB32, false);

        public static Texture2D btnRedCross = new Texture2D(16, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnSettings = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnSettingsAttention = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnMin = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnMax = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnAdd = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        
        public static void loadGUIAssets()
        {
            KACWorker.DebugLogFormatted("Loading Textures");

            try
            {
                iconNorm.LoadImage(KACUtils.LoadFileToArray("KACIcon-Norm.png"));
                iconNormShow.LoadImage(KACUtils.LoadFileToArray("KACIcon-NormShow.png"));
                iconAlarm.LoadImage(KACUtils.LoadFileToArray("KACIcon-Alarm.png"));
                iconAlarmShow.LoadImage(KACUtils.LoadFileToArray("KACIcon-AlarmShow.png"));
                iconWarpEffect100.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpEffect2_100.png"));
                iconWarpEffect080.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpEffect2_080.png"));
                iconWarpEffect060.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpEffect2_060.png"));
                iconWarpEffect040.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpEffect2_040.png"));
                iconWarpEffect020.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpEffect2_020.png"));
                iconWarpEffect000.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpEffect2_000.png"));

                iconPauseEffect100.LoadImage(KACUtils.LoadFileToArray("KACIcon-PauseEffect_100.png"));
                iconPauseEffect080.LoadImage(KACUtils.LoadFileToArray("KACIcon-PauseEffect_080.png"));
                iconPauseEffect060.LoadImage(KACUtils.LoadFileToArray("KACIcon-PauseEffect_060.png"));
                iconPauseEffect040.LoadImage(KACUtils.LoadFileToArray("KACIcon-PauseEffect_040.png"));
                iconPauseEffect020.LoadImage(KACUtils.LoadFileToArray("KACIcon-PauseEffect_020.png"));
                iconPauseEffect000.LoadImage(KACUtils.LoadFileToArray("KACIcon-PauseEffect_000.png"));

                iconWarpList100.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpList_100.png"));
                iconWarpList080.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpList_080.png"));
                iconWarpList060.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpList_060.png"));
                iconWarpList040.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpList_040.png"));
                iconWarpList020.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpList_020.png"));
                iconWarpList000.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpList_000.png"));

                iconPauseList100.LoadImage(KACUtils.LoadFileToArray("KACIcon-PauseList_100.png"));
                iconPauseList080.LoadImage(KACUtils.LoadFileToArray("KACIcon-PauseList_080.png"));
                iconPauseList060.LoadImage(KACUtils.LoadFileToArray("KACIcon-PauseList_060.png"));
                iconPauseList040.LoadImage(KACUtils.LoadFileToArray("KACIcon-PauseList_040.png"));
                iconPauseList020.LoadImage(KACUtils.LoadFileToArray("KACIcon-PauseList_020.png"));
                iconPauseList000.LoadImage(KACUtils.LoadFileToArray("KACIcon-PauseList_000.png"));

                //try
                //{
                //    iconSOI.LoadImage(KACUtils.LoadFileToArray("Icons\\KACIcon-SOI.png"));
                //}
                //catch (Exception ex)
                //{
                //    KACWorker.DebugLogFormatted("Failed Touch load");
                //    KACWorker.DebugLogFormatted(ex.Message);
                //}
                iconSOI.LoadImage(KACUtils.LoadFileToArray("KACIcon-SOI.png"));

                btnRedCross.LoadImage(KACUtils.LoadFileToArray("KACIcon-ButtonRedCross.png"));
                btnSettings.LoadImage(KACUtils.LoadFileToArray("KACIcon-ButtonSettings.png"));
                btnSettingsAttention.LoadImage(KACUtils.LoadFileToArray("KACIcon-ButtonSettingsAttention.png"));
                btnMin.LoadImage(KACUtils.LoadFileToArray("KACIcon-ButtonMin.png"));
                btnMax.LoadImage(KACUtils.LoadFileToArray("KACIcon-ButtonMax.png"));
                btnAdd.LoadImage(KACUtils.LoadFileToArray("KACIcon-ButtonAdd.png"));
                
                KACWorker.DebugLogFormatted("Loaded Textures");
            }
            catch (Exception)
            {
                KACWorker.DebugLogFormatted("Failed to Load Textures - are you missing a file?");
            }


        }

        public static Texture2D GetSettingsButtonIcon(Boolean AttentionRequired)
        {
            Texture2D textureReturn;

            //Only flash if we need attention
            if (AttentionRequired && DateTime.Now.Millisecond < 500)
                textureReturn = btnSettingsAttention;
            else
                textureReturn = btnSettings;

            return textureReturn;
        }
        public static Texture2D GetWarpIcon()
        {
            Texture2D textureReturn;

            Double intHundredth = Math.Truncate(DateTime.Now.Millisecond / 100d);
            switch (Convert.ToInt64(intHundredth))
            {
                case 0:
                    textureReturn = KACResources.iconWarpEffect100;
                    break;
                case 1:
                case 9:
                    textureReturn = KACResources.iconWarpEffect080;
                    break;
                case 2:
                case 8:
                    textureReturn = KACResources.iconWarpEffect060;
                    break;
                case 3:
                case 7:
                    textureReturn = KACResources.iconWarpEffect040;
                    break;
                case 4:
                case 6:
                    textureReturn = KACResources.iconWarpEffect020;
                    break;
                case 5:
                    textureReturn = KACResources.iconWarpEffect000;
                    break;
                default:
                    textureReturn = KACResources.iconWarpEffect100;
                    break;
            }
            return textureReturn;
        }

        public static Texture2D GetPauseIcon()
        {
            Texture2D textureReturn;

            Double intHundredth = Math.Truncate(DateTime.Now.Millisecond / 100d);
            switch (Convert.ToInt64(intHundredth))
            {
                case 0:
                    textureReturn = KACResources.iconPauseEffect100;
                    break;
                case 1:
                case 9:
                    textureReturn = KACResources.iconPauseEffect080;
                    break;
                case 2:
                case 8:
                    textureReturn = KACResources.iconPauseEffect060;
                    break;
                case 3:
                case 7:
                    textureReturn = KACResources.iconPauseEffect040;
                    break;
                case 4:
                case 6:
                    textureReturn = KACResources.iconPauseEffect020;
                    break;
                case 5:
                    textureReturn = KACResources.iconPauseEffect000;
                    break;
                default:
                    textureReturn = KACResources.iconPauseEffect100;
                    break;
            }
            return textureReturn;
        }

        public static Texture2D GetWarpListIcon(Boolean blnWarpInfluence)
        {
            Texture2D textureReturn;

            if (blnWarpInfluence)
            {
                Double intHundredth = Math.Truncate(DateTime.Now.Millisecond / 100d);
                switch (Convert.ToInt64(intHundredth))
                {
                    case 0:
                        textureReturn = KACResources.iconWarpList100;
                        break;
                    case 1:
                    case 9:
                        textureReturn = KACResources.iconWarpList080;
                        break;
                    case 2:
                    case 8:
                        textureReturn = KACResources.iconWarpList060;
                        break;
                    case 3:
                    case 7:
                        textureReturn = KACResources.iconWarpList040;
                        break;
                    case 4:
                    case 6:
                        textureReturn = KACResources.iconWarpList020;
                        break;
                    case 5:
                        textureReturn = KACResources.iconWarpList000;
                        break;
                    default:
                        textureReturn = KACResources.iconWarpList100;
                        break;
                }
            }
            else
            {
                textureReturn = KACResources.iconWarpList000;
            }
            return textureReturn;
        }

        public static Texture2D GetPauseListIcon(Boolean blnPauseInfluence)
        {
            Texture2D textureReturn;

            if (blnPauseInfluence)
            {
                Double intHundredth = Math.Truncate(DateTime.Now.Millisecond / 100d);
                switch (Convert.ToInt64(intHundredth))
                {
                    case 0:
                        textureReturn = KACResources.iconPauseList100;
                        break;
                    case 1:
                    case 9:
                        textureReturn = KACResources.iconPauseList080;
                        break;
                    case 2:
                    case 8:
                        textureReturn = KACResources.iconPauseList060;
                        break;
                    case 3:
                    case 7:
                        textureReturn = KACResources.iconPauseList040;
                        break;
                    case 4:
                    case 6:
                        textureReturn = KACResources.iconPauseList020;
                        break;
                    case 5:
                        textureReturn = KACResources.iconPauseList000;
                        break;
                    default:
                        textureReturn = KACResources.iconPauseList100;
                        break;
                }
            }
            else
            {
                textureReturn = KACResources.iconPauseList000;
            }
            return textureReturn;
        }
        #endregion


        #region "Styles"
        //Styles for windows - Cant initialize the objects here as the GUIStyle Constructor cannot be called outside of OnGUI
        public static GUIStyle styleIconStyle;
        public static GUIStyle styleWindow;
        public static GUIStyle styleHeading;
        public static GUIStyle styleContent;
        public static GUIStyle styleButton;

        public static GUIStyle styleCheckbox;
        public static GUIStyle styleCheckboxLabel;

        public static GUIStyle styleSmallButton;

        //List Styles
        public static GUIStyle styleAlarmListArea;
        public static GUIStyle styleAlarmText;
        public static GUIStyle styleAlarmTextGrayed;
        public static GUIStyle styleLabelWarp;
        public static GUIStyle styleLabelWarpGrayed;
        public static GUIStyle styleSOIIndicator;
		
        //Add Alarm Styles
        public static GUIStyle styleAddSectionHeading;
        public static GUIStyle styleAddHeading;
        public static GUIStyle styleAddField;
        public static GUIStyle styleAddFieldAreas;
        public static GUIStyle styleAddAlarmArea;

        //AlarmMessage Styles
        public static GUIStyle styleAlarmMessage;
        public static GUIStyle styleAlarmMessageTime;
        public static GUIStyle styleAlarmMessageAction;
        public static GUIStyle styleAlarmMessageActionPause;

        public static GUIStyle styleVersionHighlight;

        /// <summary>
        /// Sets up the styles for the different parts of the drawing
        /// Should only be called once
        /// </summary>
        public static void SetStyles()
        {
            Color32 colLabelText = new Color32(220, 220, 220, 255);
            int intFontSizeDefault = 13;

            //Common starting points
            GUIStyle styleDefLabel = new GUIStyle(GUI.skin.label);
            styleDefLabel.fontSize = intFontSizeDefault;
            styleDefLabel.fontStyle = FontStyle.Normal;
            styleDefLabel.normal.textColor = colLabelText;
            styleDefLabel.hover.textColor = Color.blue;

            GUIStyle styleDefTextField = new GUIStyle(GUI.skin.textField);
            styleDefTextField.fontSize = intFontSizeDefault;
            styleDefTextField.fontStyle = FontStyle.Normal;
            GUIStyle styleDefTextArea = new GUIStyle(GUI.skin.textArea);
            styleDefTextArea.fontSize = intFontSizeDefault;
            styleDefTextArea.fontStyle = FontStyle.Normal;
            GUIStyle styleDefToggle = new GUIStyle(GUI.skin.toggle);
            styleDefToggle.fontSize = intFontSizeDefault;
            styleDefToggle.fontStyle = FontStyle.Normal;
            GUIStyle styleDefButton = new GUIStyle(GUI.skin.button);
            styleDefToggle.fontSize = intFontSizeDefault;
            styleDefToggle.fontStyle = FontStyle.Normal;

            //Set up the used styles
            styleIconStyle = new GUIStyle();

            styleWindow = new GUIStyle(GUI.skin.window);
            styleWindow.fixedWidth = 250;

            styleHeading = new GUIStyle(styleDefLabel);
            styleHeading.normal.textColor = Color.white;
            styleHeading.fontStyle = FontStyle.Bold;

            styleContent = new GUIStyle(styleDefLabel);
            styleContent.normal.textColor = new Color32(183, 254, 0, 255);
            styleContent.alignment = TextAnchor.MiddleRight;
            styleContent.stretchWidth = true;

            styleButton = new GUIStyle(styleDefButton);
            styleButton.hover.textColor = Color.yellow;
            styleButton.fontSize = intFontSizeDefault;
            
            styleCheckbox = new GUIStyle(styleDefToggle);
			//CHANGED
            styleCheckboxLabel = new GUIStyle(styleDefLabel);
            styleCheckboxLabel.hover.textColor = Color.red;
            styleCheckboxLabel.onHover.textColor = Color.red;

            styleSmallButton = new GUIStyle(GUI.skin.button);
            styleSmallButton.alignment = TextAnchor.MiddleCenter;
            styleSmallButton.fixedWidth = 30;
            styleSmallButton.fixedHeight = 20;
            styleSmallButton.fontSize = intFontSizeDefault;
            styleSmallButton.fontStyle = FontStyle.Normal;
            styleSmallButton.padding.top = 0;
            styleSmallButton.padding.bottom = 0;
            styleSmallButton.padding.left = 0;
            styleSmallButton.padding.right = 0;


            styleAlarmListArea = new GUIStyle(styleDefTextArea);
            styleAlarmListArea.padding = new RectOffset(0, 0, 0, 0);
            styleAlarmListArea.margin = new RectOffset(0, 0, 0, 0);

            styleAlarmText = new GUIStyle(styleDefLabel);
            styleAlarmText.normal.textColor = Color.white;
            styleAlarmText.alignment = TextAnchor.MiddleLeft;
            styleAlarmText.stretchWidth = true;

            styleAlarmTextGrayed = new GUIStyle(styleDefLabel);
            styleAlarmTextGrayed.normal.textColor = Color.gray;
            styleAlarmTextGrayed.alignment = TextAnchor.MiddleLeft;
            styleAlarmTextGrayed.stretchWidth = true;

            styleLabelWarp = new GUIStyle(styleDefLabel);
            styleLabelWarp.alignment = TextAnchor.MiddleRight;
            styleLabelWarpGrayed = new GUIStyle(styleLabelWarp);
            styleLabelWarpGrayed.normal.textColor = Color.gray;

            styleSOIIndicator = new GUIStyle(styleDefLabel);
            styleSOIIndicator.alignment = TextAnchor.MiddleRight;
            styleSOIIndicator.normal.textColor = new Color32(0, 112, 227, 255);
            styleSOIIndicator.padding = new RectOffset(0, 0, 0, 0);

            styleAddSectionHeading = new GUIStyle(styleDefLabel);
            styleAddSectionHeading.normal.textColor = Color.white;
            styleAddSectionHeading.fontStyle = FontStyle.Bold;
            styleAddSectionHeading.padding.bottom = 0;
            styleAddSectionHeading.margin.bottom = 0;

            styleAddHeading = new GUIStyle(styleDefLabel);
            //styleAddHeading.normal.textColor = colLabelText;
            styleAddHeading.stretchWidth = false;
            styleAddHeading.alignment = TextAnchor.MiddleLeft;

            styleAddField = new GUIStyle(styleDefTextField);
            styleAddField.stretchWidth = true;
            styleAddField.alignment = TextAnchor.MiddleLeft;
            styleAddField.normal.textColor = Color.yellow;

            styleAddFieldAreas = new GUIStyle(styleDefTextArea);
            styleAddFieldAreas.padding.top = 4;
            styleAddFieldAreas.padding.bottom = 4;
            styleAddFieldAreas.padding.left = 4;
            styleAddFieldAreas.padding.right = 4;
            styleAddFieldAreas.margin.left = 0;
            styleAddFieldAreas.margin.right = 0;

            styleAddAlarmArea = new GUIStyle();
            styleAddAlarmArea.padding.top = 4;
            styleAddAlarmArea.padding.bottom = 4;
            styleAddAlarmArea.padding.left = 4;
            styleAddAlarmArea.padding.right = 4;
            styleAddAlarmArea.margin.left = 0;
            styleAddAlarmArea.margin.right = 0;

            styleAlarmMessage = new GUIStyle(styleDefLabel);
            //styleAlarmMessage.normal.textColor = colLabelText;
            styleAlarmMessageTime = new GUIStyle(styleDefLabel);
            styleAlarmMessageTime.normal.textColor = Color.yellow;
            styleAlarmMessageAction = new GUIStyle(styleDefLabel);
            styleAlarmMessageAction.stretchWidth = true;
            styleAlarmMessageAction.alignment = TextAnchor.MiddleRight;
            styleAlarmMessageAction.normal.textColor = Color.yellow;

            styleAlarmMessageActionPause = new GUIStyle(styleAlarmMessageAction);
            styleAlarmMessageActionPause.normal.textColor = Color.red;
 

            styleVersionHighlight = new GUIStyle(styleDefLabel);
            styleVersionHighlight.normal.textColor = Color.yellow;
            styleVersionHighlight.fontStyle = FontStyle.Bold;
            styleVersionHighlight.alignment = TextAnchor.MiddleRight;
            styleVersionHighlight.stretchWidth = true;
        }
        #endregion

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
                return ((this.VersionWeb != "") && (this.Version != this.VersionWeb));
                //Boolean blnReturn = false;
                //if ((this.VersionWeb!="") && (this.Version != this.VersionWeb))
                //    blnReturn = true;
                //return blnReturn;
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
        public Rect WindowPos;

        public KACAlarmList Alarms = new KACAlarmList();

        public string AlarmListMaxAlarms="10";
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
                    return 5;
                }

            }
        }
        public int AlarmPosition = 1;
        public Boolean HideOnPause = true;
        public Boolean TimeAsUT = false;
		
        public Boolean AlarmOnSOIChange = false;
        public int AlarmOnSOIChange_Action=1;


        public KACSettings()
        {
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //on each start set the attention flag to the property - should be on each program start
            VersionAttentionFlag=VersionAvailable;
        }

		//CHANGED
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
                this.WindowPos = configfile.GetValue<Rect>("WindowPos");
                this.WindowPos.height = 100;
 
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
                this.AlarmListMaxAlarms = configfile.GetValue("AlarmListMaxAlarms", "10");
                this.AlarmPosition = configfile.GetValue("AlarmPosition", 1);
                this.HideOnPause = configfile.GetValue("HideOnPause", true);
                this.TimeAsUT = configfile.GetValue("TimeAsUT", false );

                this.AlarmOnSOIChange = configfile.GetValue("AlarmOnSOIChange", false);
                this.AlarmOnSOIChange_Action = configfile.GetValue("AlarmOnSOIChange_Action", 1);

                KACWorker.DebugLogFormatted("Config Loaded Successfully");
            }

            catch (Exception ex)
            {
                KACWorker.DebugLogFormatted("Failed To Load Config");
                KACWorker.DebugLogFormatted(ex.Message);
            }

        }

		//CHANGED
        public void Save()
        {

            KACWorker.DebugLogFormatted("Saving Config");

            KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<KerbalAlarmClock>();

            configfile.SetValue("DailyUpdateCheck", this.DailyVersionCheck);
            configfile.SetValue("VersionCheckDate_Attempt", this.VersionCheckDate_AttemptString);
            configfile.SetValue("VersionCheckDate_Success", this.VersionCheckDate_SuccessString);
            configfile.SetValue("VersionWeb", this.VersionWeb);

            configfile.SetValue("WindowVisible", this.WindowVisible);
            configfile.SetValue("WindowMinimized", this.WindowMinimized);
            configfile.SetValue("WindowPos", this.WindowPos);

            for (int intAlarm = 0; intAlarm < Alarms.Count; intAlarm++)
            {
                configfile.SetValue("Alarm_" + intAlarm.ToString(), Alarms[intAlarm].SerializeString());
            }

            configfile.SetValue("AlarmListMaxAlarms", this.AlarmListMaxAlarms);
            configfile.SetValue("AlarmPosition", this.AlarmPosition);
            configfile.SetValue("HideOnPause", this.HideOnPause);
            configfile.SetValue("TimeAsUT", this.TimeAsUT);

            configfile.SetValue("AlarmOnSOIChange", this.AlarmOnSOIChange);
            configfile.SetValue("AlarmOnSOIChange_Action", this.AlarmOnSOIChange_Action);

            configfile.save();
            KACWorker.DebugLogFormatted("Saved Config");
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

                //Parse it for the version string
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
