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
    public static class KACResources
    {
        #region "Textures"

        //Clock Icons
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

        //Alarm List icons
        public static Texture2D iconMNode = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconSOI = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconAp = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconPe = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconAN = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconDN = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconXFer = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconClosest = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconlaunchRendezvous = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconCrew = new Texture2D(18, 14, TextureFormat.ARGB32, false);

        public static Texture2D iconNone = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconEdit = new Texture2D(16, 16, TextureFormat.ARGB32, false);

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

        //public static Texture2D iconstatusSOI = new Texture2D(14, 11, TextureFormat.ARGB32, false);


        public static Texture2D btnRaw = new Texture2D(20, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnMNode = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnAp = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnPe = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnApPe = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnAN = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnDN = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnANDN = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnSOI = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnXfer = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnClosest = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnCrew = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        
        public static Texture2D btnChevronUp = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnChevronDown = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnChevLeft = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnChevRight = new Texture2D(17, 16, TextureFormat.ARGB32, false);

        public static Texture2D btnRedCross = new Texture2D(16, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnSettings = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnSettingsAttention = new Texture2D(17, 16, TextureFormat.ARGB32, false);
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
                KACUtils.LoadImageIntoTexture(ref iconNorm, "img_iconNorm.png");
                KACUtils.LoadImageIntoTexture(ref iconNormShow, "img_iconNormShow.png");
                KACUtils.LoadImageIntoTexture(ref iconAlarm, "img_iconAlarm.png");
                KACUtils.LoadImageIntoTexture(ref iconAlarmShow, "img_iconAlarmShow.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect100, "img_iconWarpEffect2_100.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect080, "img_iconWarpEffect2_080.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect060, "img_iconWarpEffect2_060.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect040, "img_iconWarpEffect2_040.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect020, "img_iconWarpEffect2_020.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect000, "img_iconWarpEffect2_000.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseEffect100, "img_iconPauseEffect_100.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseEffect080, "img_iconPauseEffect_080.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseEffect060, "img_iconPauseEffect_060.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseEffect040, "img_iconPauseEffect_040.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseEffect020, "img_iconPauseEffect_020.png");
                KACUtils.LoadImageIntoTexture(ref  iconPauseEffect000, "img_iconPauseEffect_000.png");


                KACUtils.LoadImageIntoTexture(ref iconSOI, "img_listiconSOI.png");
                KACUtils.LoadImageIntoTexture(ref iconMNode, "img_listiconMNode.png");
                KACUtils.LoadImageIntoTexture(ref iconAp, "img_listiconAp.png");
                KACUtils.LoadImageIntoTexture(ref iconPe, "img_listiconPe.png");
                KACUtils.LoadImageIntoTexture(ref iconAN, "img_listiconAN.png");
                KACUtils.LoadImageIntoTexture(ref iconDN, "img_listiconDN.png");
                KACUtils.LoadImageIntoTexture(ref iconXFer, "img_listiconXfer.png");
                KACUtils.LoadImageIntoTexture(ref iconClosest, "img_listiconClosest.png");
                KACUtils.LoadImageIntoTexture(ref iconCrew, "img_listiconCrew.png");
                KACUtils.LoadImageIntoTexture(ref iconlaunchRendezvous, "img_listiconlaunchRendezvous.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList100, "img_listiconWarpList_100.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList080, "img_listiconWarpList_080.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList060, "img_listiconWarpList_060.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList040, "img_listiconWarpList_040.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList020, "img_listiconWarpList_020.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList000, "img_listiconWarpList_000.png");

                KACUtils.LoadImageIntoTexture(ref iconPauseList100, "img_listiconPauseList_100.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseList080, "img_listiconPauseList_080.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseList060, "img_listiconPauseList_060.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseList040, "img_listiconPauseList_040.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseList020, "img_listiconPauseList_020.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseList000, "img_listiconPauseList_000.png");

                KACUtils.LoadImageIntoTexture(ref iconNone, "img_listiconNone.png");
                KACUtils.LoadImageIntoTexture(ref iconEdit, "img_listiconEdit.png");

                //KACUtils.LoadImageIntoTexture(ref iconstatusSOI, "img_statusiconSOI.png");

                KACUtils.LoadImageIntoTexture(ref btnRaw, "img_buttonTypeRaw.png");
                KACUtils.LoadImageIntoTexture(ref btnMNode, "img_buttonTypeMNode.png");
                KACUtils.LoadImageIntoTexture(ref btnAp, "img_buttonTypeAp.png");
                KACUtils.LoadImageIntoTexture(ref btnPe, "img_buttonTypePe.png");
                KACUtils.LoadImageIntoTexture(ref btnApPe, "img_buttonTypeApPe.png");
                KACUtils.LoadImageIntoTexture(ref btnAN, "img_buttonTypeAN.png");
                KACUtils.LoadImageIntoTexture(ref btnDN, "img_buttonTypeDN.png");
                KACUtils.LoadImageIntoTexture(ref btnANDN, "img_buttonTypeANDN.png");
                KACUtils.LoadImageIntoTexture(ref btnSOI, "img_buttonTypeSOI.png");
                KACUtils.LoadImageIntoTexture(ref btnXfer, "img_buttonTypeXfer.png");
                KACUtils.LoadImageIntoTexture(ref btnClosest, "img_buttonTypeClosest.png");
                KACUtils.LoadImageIntoTexture(ref btnCrew, "img_buttonTypeCrew.png");

                KACUtils.LoadImageIntoTexture(ref btnChevronUp, "img_buttonChevronUp.png");
                KACUtils.LoadImageIntoTexture(ref btnChevronDown, "img_buttonChevronDown.png");
                KACUtils.LoadImageIntoTexture(ref btnChevLeft, "img_buttonChevronLeft.png");
                KACUtils.LoadImageIntoTexture(ref btnChevRight, "img_buttonChevronRight.png");

                KACUtils.LoadImageIntoTexture(ref btnRedCross, "img_buttonRedCross.png");
                KACUtils.LoadImageIntoTexture(ref btnSettings, "img_buttonSettings.png");
                KACUtils.LoadImageIntoTexture(ref btnSettingsAttention, "img_buttonSettingsAttention.png");
                KACUtils.LoadImageIntoTexture(ref btnAdd, "img_buttonAdd.png");

                KACUtils.LoadImageIntoTexture(ref txtTooltipBackground, "txt_TooltipBackground.png");


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

        public static GUIStyle styleHeadingEarth;
        public static GUIStyle styleContentEarth;

        public static GUIStyle styleButton;

        public static GUIStyle styleLabel;
        public static GUIStyle styleLabelWarning;
        public static GUIStyle styleLabelError;

        public static GUIStyle styleCheckbox;
        public static GUIStyle styleCheckboxLabel;

        public static GUIStyle styleButtonList;

        public static GUIStyle styleSmallButton;

        public static GUIStyle styleFlagIcon;

        //List Styles
        public static GUIStyle styleAlarmListArea;
        public static GUIStyle styleAlarmText;
        //public static GUIStyle styleAlarmTextGrayed;
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
        //public static GUIStyle styleAddFieldErorOverlay;
        public static GUIStyle styleAddFieldGreen;
        public static GUIStyle styleAddFieldAreas;
        public static GUIStyle styleAddAlarmArea;
        public static GUIStyle styleAddXferName;
        public static GUIStyle styleAddXferButton;
        public static GUIStyle styleAddXferOriginButton;
        public static GUIStyle styleAddMessageField;

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
            styleWindow.padding = KACUtils.SetWindowRectOffset(styleWindow.padding, 4);
            //styleWindow.normal.background = KACResources.txtWhiteSquare;
            //styleWindow.normal.textColor = new Color32(183, 254, 0, 255);
            //styleWindow.normal.textColor = Color.red;

            styleTooltipStyle = new GUIStyle(styleDefLabel);
            styleTooltipStyle.fontSize = 12;
            styleTooltipStyle.normal.textColor = new Color32(207, 207, 207, 255);
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

            styleHeadingEarth = new GUIStyle(styleHeading);
            styleHeadingEarth.normal.textColor = new Color32(0 , 173, 236, 255);
            styleContentEarth = new GUIStyle(styleContent);
            styleContentEarth.normal.textColor = new Color32(0, 173, 236, 255);

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

            styleButtonList = new GUIStyle(styleDefButton);
            styleButtonList.fixedHeight = 26;
            styleButtonList.padding = KACUtils.SetRectOffset(styleButtonList.padding, 0);

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
            styleAlarmListArea.padding = KACUtils.SetRectOffset(styleAlarmListArea.padding, 0);
            styleAlarmListArea.margin = KACUtils.SetRectOffset(styleAlarmListArea.margin, 0);

            styleAlarmText = new GUIStyle(styleDefLabel);
            styleAlarmText.normal.textColor = Color.white;
            styleAlarmText.alignment = TextAnchor.MiddleLeft;
            styleAlarmText.wordWrap = true;
            styleAlarmText.stretchWidth = true;
            //styleAlarmText.wordWrap = false;
            //styleAlarmText.stretchWidth = false;
            //styleAlarmText.clipping = TextClipping.Clip;

            //this doesn't work unless you set the background texture apparently - without the stock backgrounds its a bit difficult to match graphically
            //styleAlarmText.hover.textColor = Color.red;

            //styleAlarmTextGrayed = new GUIStyle(styleAlarmText);
            //styleAlarmTextGrayed.normal.textColor = Color.gray;

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
            styleAddField.alignment = TextAnchor.UpperLeft;
            styleAddField.normal.textColor = Color.yellow;

            styleAddFieldError = new GUIStyle(styleAddField);
            styleAddFieldError.normal.textColor = Color.red;

            styleAddFieldGreen = new GUIStyle(styleAddField);
            styleAddFieldGreen.normal.textColor = Color.green;

            styleAddMessageField = new GUIStyle(styleAddField);
            styleAddMessageField.wordWrap = true;
            styleAddMessageField.stretchHeight = true;
            styleAddMessageField.stretchWidth = false;

            //styleAddFieldErrorOverlay = new GUIStyle(styleDefLabel);
            //styleAddFieldErrorOverlay.normal.background = txtRedTint;
            //styleAddFieldErrorOverlay.border = new RectOffset(6, 6, 6, 6);

            styleAddFieldAreas = new GUIStyle(styleDefTextArea);
            styleAddFieldAreas.padding = KACUtils.SetRectOffset(styleAddFieldAreas.padding, 4);
            styleAddFieldAreas.margin.left = 0;
            styleAddFieldAreas.margin.right = 0;

            styleAddAlarmArea = new GUIStyle();
            styleAddAlarmArea.padding = KACUtils.SetRectOffset(styleAddAlarmArea.padding, 4);
            styleAddAlarmArea.margin.left = 0;
            styleAddAlarmArea.margin.right = 0;

            styleAddXferName = new GUIStyle(styleDefLabel);
            styleAddXferName.normal.textColor = Color.yellow;

            styleAddXferButton = new GUIStyle(styleDefButton);
            styleAddXferButton.fixedWidth = 40;
            styleAddXferButton.fixedHeight = 20;
            styleAddXferButton.fontSize = 11;
            styleAddXferButton.alignment = TextAnchor.MiddleCenter;

            styleAddXferOriginButton = new GUIStyle(styleDefButton);
            styleAddXferOriginButton.fixedWidth = 60;
            styleAddXferOriginButton.fixedHeight = 20;
            styleAddXferOriginButton.fontSize = 11;
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

                //read in the data file
                String strData = KSP.IO.File.ReadAllText<KerbalAlarmClock>("data_TransferModelData.csv");
                //split to lines
                String[] strLines = strData.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                String[] strFields;
                for (int intLine = 1; intLine < strLines.Length; intLine++)
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
            catch (Exception ex)
            {
                KACWorker.DebugLogFormatted("Transfer Modelling Data Failed - is the data file there and correct\r\n{0}", ex.Message);
            }
            return blnReturn;
        }
        #endregion
    }
}
