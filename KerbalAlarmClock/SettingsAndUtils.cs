using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;

using UnityEngine;
using KSP;


namespace KerbalAlarmClock
{
    public static class KACUtils
    {
        //generic function
        public static string PipeSepVariables(params object[] vars)
        {
            string strReturn = "";
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
        
        public static Byte[] LoadFileToArray(string Filename)
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

        public static Texture2D iconWarpList100 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpList080 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpList060 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpList040 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpList020 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpList000 = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        public static Texture2D btnRedCross = new Texture2D(16, 16, TextureFormat.ARGB32, false);
        
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

                iconWarpList100.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpList_100.png"));
                iconWarpList080.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpList_080.png"));
                iconWarpList060.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpList_060.png"));
                iconWarpList040.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpList_040.png"));
                iconWarpList020.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpList_020.png"));
                iconWarpList000.LoadImage(KACUtils.LoadFileToArray("KACIcon-WarpList_000.png"));

                btnRedCross.LoadImage(KACUtils.LoadFileToArray("KACIcon-ButtonRedCross.png"));

                KACWorker.DebugLogFormatted("Loaded Textures");
            }
            catch (Exception)
            {
                KACWorker.DebugLogFormatted("Failed to Load Textures - are you missing a file?");
            }


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
        #endregion


        #region "Styles"
        //Styles for windows - Cant initialize the objects here as the GUIStyle Constructor cannot be called outside of OnGUI
        public static GUIStyle styleIconStyle;
        public static GUIStyle styleWindow;
        public static GUIStyle styleHeading;
        public static GUIStyle styleContent;

        public static GUIStyle styleSmallButton;

        //List Styles
        public static GUIStyle styleAlarmText;
        public static GUIStyle styleAlarmTextGrayed;
        public static GUIStyle styleLabelWarp;
        public static GUIStyle styleLabelWarpGrayed;


        //Add Alarm Styles
        public static GUIStyle styleCheckbox;
        public static GUIStyle styleAddSectionHeading;
        public static GUIStyle styleAddHeading;
        public static GUIStyle styleAddField;
        public static GUIStyle styleAddFieldAreas;
        public static GUIStyle styleAddAlarmArea;

        //AlarmMessage Styles
        public static GUIStyle styleAlarmMessage;
        public static GUIStyle styleAlarmMessageTime;

        /// <summary>
        /// Sets up the styles for the different parts of the drawing
        /// Should only be called once
        /// </summary>
        public static void SetStyles()
        {
            Color32 colLabelText = new Color32(224, 224, 224, 255);
            int intFontSizeDefault = 13;

            //Common starting points
            GUIStyle styleDefLabel = new GUIStyle(GUI.skin.label);
            styleDefLabel.fontSize = intFontSizeDefault;
            GUIStyle styleDefTextField = new GUIStyle(GUI.skin.textField);
            styleDefTextField.fontSize = intFontSizeDefault;
            GUIStyle styleDefTextArea = new GUIStyle(GUI.skin.textArea);
            styleDefTextArea.fontSize = intFontSizeDefault;
            GUIStyle styleDefToggle = new GUIStyle(GUI.skin.toggle);
            styleDefToggle.fontSize = intFontSizeDefault;


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

            styleSmallButton = new GUIStyle(GUI.skin.button);
            styleSmallButton.alignment = TextAnchor.MiddleCenter;

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

            styleCheckbox = new GUIStyle(styleDefToggle);
            styleCheckbox.normal.textColor = Color.white;
            styleCheckbox.fixedWidth = 24;
            styleCheckbox.fixedHeight = 24;

            styleAddSectionHeading = new GUIStyle(styleDefLabel);
            styleAddSectionHeading.normal.textColor = Color.white;
            styleAddSectionHeading.fontStyle = FontStyle.Bold;
            styleAddSectionHeading.padding.bottom = 0;
            styleAddSectionHeading.margin.bottom = 0;

            styleAddHeading = new GUIStyle(styleDefLabel);
            styleAddHeading.normal.textColor = colLabelText;
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
            styleAlarmMessageTime = new GUIStyle(styleDefLabel);
            styleAlarmMessageTime.normal.textColor = Color.yellow;

        }
        #endregion

    }

    /// <summary>
    /// Settings object
    /// </summary>
    public class KACSettings
    {
        public string Version;

        public Boolean WindowVisible = false;
        public Boolean WindowMinimized = false;
        public Rect WindowPos;

        public KACAlarmList Alarms = new KACAlarmList();

        public KACSettings()
        {
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public void Load()
        {
            try
            {
                KACWorker.DebugLogFormatted("Loading Config");
                KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<KerbalAlarmClock>();
                configfile.load();
                this.WindowVisible = configfile.GetValue("WindowVisible", false);
                this.WindowMinimized = configfile.GetValue("WindowMinimized", false);
                this.WindowPos = configfile.GetValue<Rect>("WindowPos");

                this.WindowPos.height = 100;
 
                //Loop through numbers to Load Alarms
                Alarms = new KACAlarmList();
                int intAlarm = 0;
                string strAlarm = "";
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

                KACWorker.DebugLogFormatted("Config Loaded Successfully");
            }

            catch (Exception ex)
            {
                KACWorker.DebugLogFormatted("Failed To Load Config");
                KACWorker.DebugLogFormatted(ex.Message);
            }

        }

        public void Save()
        {

            KACWorker.DebugLogFormatted("Saving Config");

            KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<KerbalAlarmClock>();

            configfile.SetValue("WindowVisible", this.WindowVisible);
            configfile.SetValue("WindowMinimized", this.WindowMinimized);
            configfile.SetValue("WindowPos", this.WindowPos);

            for (int intAlarm = 0; intAlarm < Alarms.Count; intAlarm++)
            {
                configfile.SetValue("Alarm_" + intAlarm.ToString(), Alarms[intAlarm].SerializeString());
            }

            configfile.save();
            KACWorker.DebugLogFormatted("Saved Config");
        }

    }


}
