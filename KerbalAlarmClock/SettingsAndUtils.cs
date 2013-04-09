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
    public static class KACUtils
    {
        public static String AppPath = KSPUtil.ApplicationRootPath.Replace("\\", "/");
        public static String PlugInPath = AppPath + "PluginData/KerbalAlarmClock/";

        //generic function
        public static String PipeSepVariables(params object[] vars)
        {
            return SepVariables("|", vars);
        }

        public static String CommaSepVariables(params object[] vars)
        {
            return SepVariables(",", vars);
        }

        public static String SepVariables(String separator,params object[] vars)
        {
            String strReturn = "";
            foreach (object tmpVar in vars)
            {
                if (strReturn != "") strReturn += separator;
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

        public static void SaveFileFromArray(Byte[] data,String Filename)
        {
            KSP.IO.File.WriteAllBytes<KerbalAlarmClock>(data,Filename);
        }

        public static void LoadImageIntoTexture(ref Texture2D tex, String FileName)
        {
            WWW img1 = new WWW(String.Format("file://{0}Icons/{1}", PlugInPath, FileName));
            img1.LoadImageIntoTexture(tex);
        }

        public static void LoadImageIntoTexture(ref Texture2D tex, String FolderName, String FileName)
        {
            WWW img1 = new WWW(String.Format("file://{0}{1}/{2}", PlugInPath, FolderName,FileName));
            img1.LoadImageIntoTexture(tex);
        }

        public static RectOffset SetWindowRectOffset(RectOffset tmpRectOffset, int intValue)
        {
            tmpRectOffset.left = intValue;
            //tmpRectOffset.top = Top;
            tmpRectOffset.right = intValue;
            tmpRectOffset.bottom = intValue;
            return tmpRectOffset;
        }

        public static RectOffset SetRectOffset(RectOffset tmpRectOffset, int intValue)
        {
            return SetRectOffset(tmpRectOffset, intValue, intValue, intValue, intValue);
        }

        public static RectOffset SetRectOffset(RectOffset tmpRectOffset, int Left,int Right,int Top, int Bottom)
        {
            tmpRectOffset.left = Left;
            tmpRectOffset.top = Top;
            tmpRectOffset.right = Right ;
            tmpRectOffset.bottom = Bottom;
            return tmpRectOffset;
        }

        public static double Clamp(double x, double min, double max)
        {
            return Math.Min(Math.Max(x, min), max);
        }

        //keeps angles in the range -180 to 180
        public static double clampDegrees(double angle)
        {
            angle = angle + ((int)(2 + Math.Abs(angle) / 360)) * 360.0; //should be positive
            angle = angle % 360.0;
            if (angle > 180.0) return angle - 360.0;
            else return angle;
        }

        //keeps angles in the range 0 to 360
        public static double clampDegrees360(double angle)
        {
            angle = angle % 360.0;
            if (angle < 0) return angle + 360.0;
            else return angle;
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

        public static Texture2D iconXFer = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconMNode = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconSOI = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconSOISmall = new Texture2D(14, 11, TextureFormat.ARGB32, false);
        public static Texture2D iconNone = new Texture2D(18, 14, TextureFormat.ARGB32, false);

        public static Texture2D iconEdit = new Texture2D(16, 16, TextureFormat.ARGB32, false);

        public static Texture2D btnRedCross = new Texture2D(16, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnSettings = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnSettingsAttention = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnMin = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnMax = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnAdd = new Texture2D(17, 16, TextureFormat.ARGB32, false);

        public static Texture2D txtTooltipBackground = new Texture2D(9, 9);//, TextureFormat.ARGB32, false);
        //public static Texture2D txtRedTint = new Texture2D(16, 16);//, TextureFormat.ARGB32, false);
        //public static Texture2D txtBlackSquare = new Texture2D(5, 5);//, TextureFormat.ARGB32, false);
        //public static Texture2D txtWhiteSquare = new Texture2D(5, 5);//, TextureFormat.ARGB32, false);
        
        public static void loadGUIAssets()
        {
            KACWorker.DebugLogFormatted("Loading Textures");

            try
            {
                KACUtils.LoadImageIntoTexture(ref iconNorm, "KACIcon-Norm.png");

                KACUtils.LoadImageIntoTexture(ref iconNormShow,"KACIcon-NormShow.png");
                KACUtils.LoadImageIntoTexture(ref iconAlarm,"KACIcon-Alarm.png");
                KACUtils.LoadImageIntoTexture(ref iconAlarmShow,"KACIcon-AlarmShow.png");
                
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect100,"KACIcon-WarpEffect2_100.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect080,"KACIcon-WarpEffect2_080.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect060,"KACIcon-WarpEffect2_060.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect040,"KACIcon-WarpEffect2_040.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect020,"KACIcon-WarpEffect2_020.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect000,"KACIcon-WarpEffect2_000.png");

                KACUtils.LoadImageIntoTexture(ref iconPauseEffect100,"KACIcon-PauseEffect_100.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseEffect080,"KACIcon-PauseEffect_080.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseEffect060,"KACIcon-PauseEffect_060.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseEffect040,"KACIcon-PauseEffect_040.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseEffect020,"KACIcon-PauseEffect_020.png");
                KACUtils.LoadImageIntoTexture(ref  iconPauseEffect000,"KACIcon-PauseEffect_000.png");

                KACUtils.LoadImageIntoTexture(ref iconWarpList100,"KACIcon-WarpList_100.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList080,"KACIcon-WarpList_080.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList060,"KACIcon-WarpList_060.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList040,"KACIcon-WarpList_040.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList020,"KACIcon-WarpList_020.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList000,"KACIcon-WarpList_000.png");

                KACUtils.LoadImageIntoTexture(ref iconPauseList100,"KACIcon-PauseList_100.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseList080,"KACIcon-PauseList_080.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseList060,"KACIcon-PauseList_060.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseList040,"KACIcon-PauseList_040.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseList020,"KACIcon-PauseList_020.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseList000,"KACIcon-PauseList_000.png");

                KACUtils.LoadImageIntoTexture(ref iconSOI, "KACIcon-SOI.png");
                KACUtils.LoadImageIntoTexture(ref iconSOISmall, "KACIcon-SOISmall.png");
                KACUtils.LoadImageIntoTexture(ref iconMNode, "KACIcon-MNode.png");
                KACUtils.LoadImageIntoTexture(ref iconXFer, "KACIcon-Xfer.png");
                KACUtils.LoadImageIntoTexture(ref iconNone, "KACIcon-None.png");

                KACUtils.LoadImageIntoTexture(ref iconEdit, "KACIcon-Edit.png");

                KACUtils.LoadImageIntoTexture(ref btnRedCross, "KACIcon-ButtonRedCross.png");
                KACUtils.LoadImageIntoTexture(ref btnSettings,"KACIcon-ButtonSettings.png");
                KACUtils.LoadImageIntoTexture(ref btnSettingsAttention, "KACIcon-ButtonSettingsAttention.png");
                KACUtils.LoadImageIntoTexture(ref btnMin, "KACIcon-ButtonMin.png");
                KACUtils.LoadImageIntoTexture(ref btnMax,"KACIcon-ButtonMax.png");
                KACUtils.LoadImageIntoTexture(ref btnAdd,"KACIcon-ButtonAdd.png");

                KACUtils.LoadImageIntoTexture(ref txtTooltipBackground, "Textures", "TooltipBackground.png");
                
                //KACUtils.LoadImageIntoTexture(ref txtRedTint, "Textures", "RedOverlay.png");
                
                //KACUtils.LoadImageIntoTexture(ref txtBlackSquare, "Textures", "BlackSquare.png");
                //KACUtils.LoadImageIntoTexture(ref txtWhiteSquare, "Textures", "WhiteSquare.png");

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
        
        public static GUIStyle styleWindow;
        public static GUIStyle styleTooltipStyle;

        public static GUIStyle styleIconStyle; 
        public static GUIStyle styleHeading;
        public static GUIStyle styleContent;
        public static GUIStyle styleButton;

        public static GUIStyle styleLabel;
        public static GUIStyle styleLabelWarning;
        public static GUIStyle styleLabelError;

        public static GUIStyle styleCheckbox;
        public static GUIStyle styleCheckboxLabel;

        public static GUIStyle styleSmallButton;

        public static GUIStyle styleFlagIcon;
        
        //List Styles
        public static GUIStyle styleAlarmListArea;
        public static GUIStyle styleAlarmText;
        public static GUIStyle styleAlarmTextGrayed;
        public static GUIStyle styleAlarmIcon;
        public static GUIStyle styleLabelWarp;
        public static GUIStyle styleLabelWarpGrayed;
        public static GUIStyle styleSOIIndicator;
        public static GUIStyle styleSOIIcon;
		
        //Add Alarm Styles
        public static GUIStyle styleAddSectionHeading;
        public static GUIStyle styleAddHeading;
        public static GUIStyle styleAddField;
        public static GUIStyle styleAddFieldError;
        //public static GUIStyle styleAddFieldErrorOverlay;
        public static GUIStyle styleAddFieldAreas;
        public static GUIStyle styleAddAlarmArea;
        public static GUIStyle styleAddXferName;
        public static GUIStyle styleAddXferButton;
        public static GUIStyle styleAddXferOriginButton;
        
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

            styleWindow = new GUIStyle(GUI.skin.window) ;
            styleWindow.padding = KACUtils.SetWindowRectOffset(styleWindow.padding, 4);
            //styleWindow.normal.background = KACResources.txtWhiteSquare;
            //styleWindow.normal.textColor = new Color32(183, 254, 0, 255);
            //styleWindow.normal.textColor = Color.red;

            styleTooltipStyle = new GUIStyle(styleDefLabel);
            styleTooltipStyle.fontSize = 12;
            styleTooltipStyle.normal.textColor = new Color32(207,207,207,255);
            styleTooltipStyle.stretchHeight = true;
            styleTooltipStyle.wordWrap = true;
            styleTooltipStyle.normal.background = txtTooltipBackground;
            //Extra border to prevent bleed of color - actual border is only 1 pixel wide
            styleTooltipStyle.border = new RectOffset(3, 3, 3, 3);
            styleTooltipStyle.padding = new RectOffset(4, 4, 6, 4);
            styleTooltipStyle.alignment = TextAnchor.MiddleCenter;

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

            styleLabel = new GUIStyle(styleDefLabel);

            styleLabelWarning = new GUIStyle(styleLabel);
            styleLabelWarning.normal.textColor = Color.yellow;

            styleLabelError = new GUIStyle(styleLabel);
            styleLabelError.normal.textColor = Color.red;


            styleCheckbox = new GUIStyle(styleDefToggle);
			//CHANGED
            styleCheckboxLabel = new GUIStyle(styleDefLabel);
            //styleCheckboxLabel.hover.textColor = Color.red;
            //styleCheckboxLabel.onHover.textColor = Color.red;

            styleSmallButton = new GUIStyle(GUI.skin.button);
            styleSmallButton.alignment = TextAnchor.MiddleCenter;
            styleSmallButton.fixedWidth = 30;
            styleSmallButton.fixedHeight = 20;
            styleSmallButton.fontSize = intFontSizeDefault;
            styleSmallButton.fontStyle = FontStyle.Normal;
            styleSmallButton.padding = KACUtils.SetRectOffset(styleSmallButton.padding, 0);

            styleFlagIcon = new GUIStyle(styleDefLabel);
            styleFlagIcon.padding = KACUtils.SetRectOffset(styleFlagIcon.padding, 0);
            styleFlagIcon.alignment = TextAnchor.MiddleLeft;
            styleFlagIcon.fixedWidth = 20;

            styleAlarmListArea = new GUIStyle(styleDefTextArea);
            styleAlarmListArea.padding =  KACUtils.SetRectOffset(styleAlarmListArea.padding, 0);
            styleAlarmListArea.margin = KACUtils.SetRectOffset(styleAlarmListArea.margin, 0);

            styleAlarmText = new GUIStyle(styleDefLabel);
            styleAlarmText.normal.textColor = Color.white;
            styleAlarmText.alignment = TextAnchor.MiddleLeft;
            styleAlarmText.stretchWidth = true;
            //this doesn't work unless you set the background texture apparently - without the stock backgrounds its a bit difficult to match graphically
            //styleAlarmText.hover.textColor = Color.red;

            styleAlarmTextGrayed = new GUIStyle(styleDefLabel);
            styleAlarmTextGrayed.normal.textColor = Color.gray;
            styleAlarmTextGrayed.alignment = TextAnchor.MiddleLeft;
            styleAlarmTextGrayed.stretchWidth = true;

            styleAlarmIcon = new GUIStyle(styleDefLabel);
            styleAlarmIcon.alignment = TextAnchor.UpperCenter;

            styleLabelWarp = new GUIStyle(styleDefLabel);
            styleLabelWarp.alignment = TextAnchor.MiddleRight;
            styleLabelWarpGrayed = new GUIStyle(styleLabelWarp);
            styleLabelWarpGrayed.normal.textColor = Color.gray;



            styleSOIIndicator = new GUIStyle(styleDefLabel);
            styleSOIIndicator.alignment = TextAnchor.MiddleLeft;
            //styleSOIIndicator.fontSize = 11;
            styleSOIIndicator.normal.textColor = new Color32(0, 112, 227, 255);
            styleSOIIndicator.padding = KACUtils.SetRectOffset(styleSOIIndicator.padding, 0);

            styleSOIIcon = new GUIStyle(styleSOIIndicator); 
            

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

            styleAddFieldError = new GUIStyle(styleAddField);
            styleAddFieldError.normal.textColor = Color.red;

            //styleAddFieldErrorOverlay = new GUIStyle(styleDefLabel);
            //styleAddFieldErrorOverlay.normal.background = txtRedTint;
            //styleAddFieldErrorOverlay.border = new RectOffset(6, 6, 6, 6);

            styleAddFieldAreas = new GUIStyle(styleDefTextArea);
            styleAddFieldAreas.padding = KACUtils.SetRectOffset(styleAddFieldAreas.padding,4);
            styleAddFieldAreas.margin.left = 0;
            styleAddFieldAreas.margin.right = 0;

            styleAddAlarmArea = new GUIStyle();
            styleAddAlarmArea.padding= KACUtils.SetRectOffset(styleAddAlarmArea.padding, 4);
            styleAddAlarmArea.margin.left = 0;
            styleAddAlarmArea.margin.right = 0;

            styleAddXferName = new GUIStyle(styleDefLabel);
            styleAddXferName.normal.textColor = Color.yellow;

            styleAddXferButton = new GUIStyle(styleDefButton);
            styleAddXferButton.fixedWidth = 40;
            styleAddXferButton.fixedHeight = 22;
            styleAddXferButton.fontSize = 11;
            styleAddXferButton.alignment = TextAnchor.MiddleCenter;

            styleAddXferOriginButton = new GUIStyle(styleDefButton);
            styleAddXferOriginButton.fixedWidth = 60;
            styleAddXferOriginButton.fixedHeight = 22;
            styleAddXferOriginButton.fontSize=11;
            styleAddXferOriginButton.alignment = TextAnchor.MiddleCenter;


            styleAlarmMessage = new GUIStyle(styleDefLabel);
            //styleAlarmMessage.normal.textColor = colLabelText;
            styleAlarmMessageTime = new GUIStyle(styleDefLabel);
            styleAlarmMessageTime.normal.textColor = Color.yellow;
            styleAlarmMessageAction = new GUIStyle(styleDefLabel);
            styleAlarmMessageAction.stretchWidth = true;
            styleAlarmMessageAction.stretchHeight = true;
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


        #region "Functions"
        //public static Color PulseColor(Color Start, Color Dest)
        //{
        //    Color colReturn = Start;
        //    Double intHundredth = Math.Truncate(DateTime.Now.Millisecond / 100d);
        //    switch (Convert.ToInt64(intHundredth))
        //    {
        //        case 0:
        //            colReturn=Start;
        //            break;
        //        case 1:
        //        case 9:
        //            colReturn.r = ((Dest.r - Start.r)*1/5) + Start.r;
        //            colReturn.g = ((Dest.g - Start.g)*1/5) + Start.g;
        //            colReturn.b = ((Dest.b - Start.b)*1/5) + Start.b;
        //            break;
        //        case 2:
        //        case 8:
        //            colReturn.r = ((Dest.r - Start.r)*2/5) + Start.r;
        //            colReturn.g = ((Dest.g - Start.g)*2/5) + Start.g;
        //            colReturn.b = ((Dest.b - Start.b)*2/5) + Start.b;
        //            break;
        //        case 3:
        //        case 7:
        //            colReturn.r = ((Dest.r - Start.r)*3/5) + Start.r;
        //            colReturn.g = ((Dest.g - Start.g)*3/5) + Start.g;
        //            colReturn.b = ((Dest.b - Start.b)*3/5) + Start.b;
        //            break;
        //        case 4:
        //        case 6:
        //            colReturn.r = ((Dest.r - Start.r)*4/5) + Start.r;
        //            colReturn.g = ((Dest.g - Start.g)*5/5) + Start.g;
        //            colReturn.b = ((Dest.b - Start.b)*4/5) + Start.b;
        //            break;
        //        case 5:
        //            colReturn=Dest;
        //            break;
        //        default:
        //            colReturn=Start;
        //            break;
        //    }
        //    return colReturn;
        //}
        #endregion

        #region "Data"
        public static List<KACXFerModelPoint> lstXferModelPoints;

        public static Boolean LoadModelPoints()
        {
            KACWorker.DebugLogFormatted("Loading Transfer Modelling Data");
            Boolean blnReturn = false;
            try
            {
                lstXferModelPoints = new List<KACXFerModelPoint>();

                WWW DataFile = new WWW(String.Format("file://{0}Data/TransferModelData.csv", KACUtils.PlugInPath));
                //loop to read file
                while (!DataFile.isDone) { } 
            
                //convert it
                String strData = DataFile.text;
                String[] strLines = strData.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                String[] strFields;
                for(int intLine=1;intLine<strLines.Length;intLine++)
        	    {
                    strFields = strLines[intLine].Split(",".ToCharArray());
                    lstXferModelPoints.Add(new KACXFerModelPoint(
                        Convert.ToDouble(strFields[0]),
                        Convert.ToInt32(strFields[1]),
                        Convert.ToInt32(strFields[2]),
                        Convert.ToDouble(strFields[3])
                        ));
	            }
                blnReturn = true;
                KACWorker.DebugLogFormatted("Transfer Modelling Data Load Complete");
            }
            catch (Exception)
            {
                KACWorker.DebugLogFormatted("Transfer Modelling Data Failed - is the data file there and correct");
            }
            return blnReturn;
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
        public Boolean TimeAsUT = false;
        public Boolean ShowTooltips = true;

        public Boolean AlarmXferRecalc = true;
        public double AlarmXferRecalcThreshold = 180;

        public Boolean XferModelLoadData = false;
        public Boolean XferModelDataLoaded = false;

        public Boolean AlarmAddSOIAuto = false;
        public double AlarmAddSOIAutoThreshold = 180;
        public double AlarmAutoSOIMargin = 900;
        //public double AlarmAddSOIMargin = 120;
        public Boolean AlarmCatchSOIChange = false;
        public int AlarmOnSOIChange_Action = 1;


        public List<GameScenes> DrawScenes = new List<GameScenes> { GameScenes.FLIGHT };
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
                this.WindowPos = configfile.GetValue<Rect>("WindowPos");
                this.WindowPos.height = 100;

                this.AlarmListMaxAlarms = configfile.GetValue("AlarmListMaxAlarms", "10");
                this.AlarmDefaultAction = configfile.GetValue<int>("AlarmDefaultAction", 1);
                this.AlarmDefaultMargin = configfile.GetValue<Double>("AlarmDefaultMargin", 60);
                this.AlarmPosition = configfile.GetValue<int>("AlarmPosition", 1);
                this.AlarmDeleteOnClose = configfile.GetValue("AlarmDeleteOnClose", false);
                this.ShowTooltips = configfile.GetValue("ShowTooltips", true);
                this.HideOnPause = configfile.GetValue("HideOnPause", true);
                this.TimeAsUT = configfile.GetValue("TimeAsUT", false);

                this.AlarmXferRecalc = configfile.GetValue("AlarmXferRecalc", true);
                this.AlarmXferRecalcThreshold = configfile.GetValue<Double>("AlarmXferRecalcThreshold", 180);

                this.AlarmAddSOIAuto = configfile.GetValue("AlarmAddSOIAuto", false);
                this.AlarmAddSOIAutoThreshold = configfile.GetValue<Double>("AlarmAddSOIAutoThreshold", 180);
                //this.AlarmAddSOIMargin = configfile.GetValue("AlarmAddSOIMargin", 120);
                this.AlarmAutoSOIMargin = configfile.GetValue<Double>("AlarmAutoSOIMargin", 900);
                this.AlarmCatchSOIChange = configfile.GetValue("AlarmOnSOIChange", false);
                this.AlarmOnSOIChange_Action = configfile.GetValue("AlarmOnSOIChange_Action", 1);

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
            Alarms = new KACAlarmList();
            KSP.IO.TextReader tr = KSP.IO.TextReader.CreateForType<KerbalAlarmClock>(String.Format("Alarms-{0}.txt", HighLogic.CurrentGame.Title));
            String strFile = tr.ReadToEnd();
            tr.Close();

            while (strFile.Contains("|<ENDLINE>"))
	        {
                String strAlarm = strFile.Substring(0,strFile.IndexOf("|<ENDLINE>"));
                strFile = strFile.Substring(strAlarm.Length + "|<ENDLINE>".Length).TrimStart("\r\n".ToCharArray());

                if(!strAlarm.StartsWith("VesselID|"))
                {
                    KACAlarm tmpAlarm = new KACAlarm();
                    tmpAlarm.LoadFromString2(strAlarm);
                    Alarms.Add(tmpAlarm);
                }
	        }
        }

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

            configfile.SetValue("AlarmListMaxAlarms", this.AlarmListMaxAlarms);
            configfile.SetValue("AlarmPosition", this.AlarmPosition);
            configfile.SetValue("AlarmDefaultAction", this.AlarmDefaultAction);
            configfile.SetValue("AlarmDefaultMargin", this.AlarmDefaultMargin);
            configfile.SetValue("AlarmDeleteOnClose", this.AlarmDeleteOnClose);
            configfile.SetValue("ShowTooltips", this.ShowTooltips);
            configfile.SetValue("HideOnPause", this.HideOnPause);
            configfile.SetValue("TimeAsUT", this.TimeAsUT);

            configfile.SetValue("AlarmXferRecalc", this.AlarmXferRecalc);
            configfile.SetValue("AlarmXferRecalcThreshold", this.AlarmXferRecalcThreshold);

            configfile.SetValue("AlarmAddSOIAuto", this.AlarmAddSOIAuto);
            configfile.SetValue("AlarmAddSOIAutoThreshold", this.AlarmAddSOIAutoThreshold);
            //configfile.SetValue("AlarmAddSOIMargin", this.AlarmAddSOIMargin);
            configfile.SetValue("AlarmAutoSOIMargin", this.AlarmAutoSOIMargin);
            configfile.SetValue("AlarmOnSOIChange", this.AlarmCatchSOIChange);
            configfile.SetValue("AlarmOnSOIChange_Action", this.AlarmOnSOIChange_Action);

            //for (int intAlarm = 0; intAlarm < Alarms.Count; intAlarm++)
            //{
            //    configfile.SetValue("Alarm_" + intAlarm.ToString(), Alarms[intAlarm].SerializeString());
            //}

            configfile.save();
            KACWorker.DebugLogFormatted("Saved Config");

            //Now Save the Alarms
            SaveAlarms();
            KACWorker.DebugLogFormatted("Saved Alarms");
        }

        private void SaveAlarms()
        {
            KSP.IO.TextWriter tw = KSP.IO.TextWriter.CreateForType<KerbalAlarmClock>(String.Format("Alarms-{0}.txt", HighLogic.CurrentGame.Title));
            //Write the header
            tw.WriteLine("VesselID|Name|Message|AlarmTime.UT|AlarmMarginSecs|Type|Enabled|HaltWarp|PauseGame|Options-Manuever/Xfer|<ENDLINE>");
            foreach (KACAlarm tmpAlarm in Alarms.BySaveName(HighLogic.CurrentGame.Title))
            {
                //Now Write Each alarm
                tw.WriteLine(tmpAlarm.SerializeString2() + "|<ENDLINE>");
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

