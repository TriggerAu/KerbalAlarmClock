using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

using UnityEngine;
using KSP;
using KSPPluginFramework;

namespace KerbalAlarmClock
{
    internal static class KACResources
    {
        #region "Textures"

        //Clock Icons
        internal static Texture2D iconNorm; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconNormShow; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconAlarm; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconAlarmShow; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpEffect100; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpEffect080; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpEffect060; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpEffect040; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpEffect020; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpEffect000; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseEffect100; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseEffect080; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseEffect060; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseEffect040; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseEffect020; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseEffect000; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        //Alarm List icons
        internal static Texture2D iconRaw; //  = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconMNode; //  = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconSOI; //  = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconAp; //  = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconPe; //  = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconAN; //  = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconDN; //  = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconXFer; //  = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconClosest; //  = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconLaunchRendezvous; //  = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconCrew; //  = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconEarth; //  = new Texture2D(18, 14, TextureFormat.ARGB32, false);

        internal static Texture2D iconNone; //  = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconEdit; //  = new Texture2D(16, 16, TextureFormat.ARGB32, false);

        internal static Texture2D iconWarpList100; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpList080; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpList060; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpList040; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpList020; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpList000; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        internal static Texture2D iconPauseList100; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseList080; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseList060; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseList040; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseList020; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseList000; //  = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        //internal static Texture2D iconstatusSOI; //  = new Texture2D(14, 11, TextureFormat.ARGB32, false);


        internal static Texture2D btnRaw; //  = new Texture2D(20, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnMNode; //  = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnAp; //  = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnPe; //  = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnApPe; //  = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnAN; //  = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnDN; //  = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnANDN; //  = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnSOI; //  = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnXfer; //  = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnClosest; //  = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnCrew; //  = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        
        internal static Texture2D btnChevronUp; //  = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnChevronDown; //  = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnChevLeft; //  = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnChevRight; //  = new Texture2D(17, 16, TextureFormat.ARGB32, false);

        internal static Texture2D btnRedCross; //  = new Texture2D(16, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnSettings; //  = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnSettingsAttention; //  = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnAdd; //  = new Texture2D(17, 16, TextureFormat.ARGB32, false);

        internal static Texture2D btnActionMsg;
        internal static Texture2D btnActionWarp;
        internal static Texture2D btnActionWarpMsg;
        internal static Texture2D btnActionPause;

        internal static Texture2D btnDropDown;
        internal static Texture2D btnPlay;
        internal static Texture2D btnStop;

        internal static Texture2D texBox;
        internal static Texture2D texBoxUnity;
        internal static Texture2D texTooltip;

        internal static Texture2D texSeparatorV;
        internal static Texture2D texSeparatorH;

        //internal static Texture2D txtTooltipBackground; //  = new Texture2D(9, 9); //, TextureFormat.ARGB32, false);
        //internal static Texture2D txtRedTint; //  = new Texture2D(16, 16); //, TextureFormat.ARGB32, false);
        //internal static Texture2D txtBlackSquare; //  = new Texture2D(5, 5); //, TextureFormat.ARGB32, false);
        //internal static Texture2D txtWhiteSquare; //  = new Texture2D(5, 5); //, TextureFormat.ARGB32, false);

        internal static void loadGUIAssets()
        {
            MonoBehaviourExtended.LogFormatted("Loading Textures");

            try
            {
                KACUtils.LoadImageFromGameDB(ref iconNorm, "KACIcon-Norm.tga", KACUtils.DBPathToolbarIcons);
                KACUtils.LoadImageFromGameDB(ref iconNormShow, "KACIcon-NormShow.tga", KACUtils.DBPathToolbarIcons);
                KACUtils.LoadImageFromGameDB(ref iconAlarm, "KACIcon-Alarm.tga", KACUtils.DBPathToolbarIcons);
                KACUtils.LoadImageFromGameDB(ref iconAlarmShow, "KACIcon-AlarmShow.tga", KACUtils.DBPathToolbarIcons);
                KACUtils.LoadImageFromGameDB(ref iconWarpEffect100, "KACIcon-WarpEffect2_100.tga", KACUtils.DBPathToolbarIcons);
                KACUtils.LoadImageFromGameDB(ref iconWarpEffect080, "KACIcon-WarpEffect2_080.tga", KACUtils.DBPathToolbarIcons);
                KACUtils.LoadImageFromGameDB(ref iconWarpEffect060, "KACIcon-WarpEffect2_060.tga", KACUtils.DBPathToolbarIcons);
                KACUtils.LoadImageFromGameDB(ref iconWarpEffect040, "KACIcon-WarpEffect2_040.tga", KACUtils.DBPathToolbarIcons);
                KACUtils.LoadImageFromGameDB(ref iconWarpEffect020, "KACIcon-WarpEffect2_020.tga", KACUtils.DBPathToolbarIcons);
                KACUtils.LoadImageFromGameDB(ref iconWarpEffect000, "KACIcon-WarpEffect2_000.tga", KACUtils.DBPathToolbarIcons);
                KACUtils.LoadImageFromGameDB(ref iconPauseEffect100, "KACIcon-PauseEffect_100.tga", KACUtils.DBPathToolbarIcons);
                KACUtils.LoadImageFromGameDB(ref iconPauseEffect080, "KACIcon-PauseEffect_080.tga", KACUtils.DBPathToolbarIcons);
                KACUtils.LoadImageFromGameDB(ref iconPauseEffect060, "KACIcon-PauseEffect_060.tga", KACUtils.DBPathToolbarIcons);
                KACUtils.LoadImageFromGameDB(ref iconPauseEffect040, "KACIcon-PauseEffect_040.tga", KACUtils.DBPathToolbarIcons);
                KACUtils.LoadImageFromGameDB(ref iconPauseEffect020, "KACIcon-PauseEffect_020.tga", KACUtils.DBPathToolbarIcons);
                KACUtils.LoadImageFromGameDB(ref  iconPauseEffect000, "KACIcon-PauseEffect_000.tga", KACUtils.DBPathToolbarIcons);


                KACUtils.LoadImageFromGameDB(ref iconRaw, "img_listiconRaw.tga");
                KACUtils.LoadImageFromGameDB(ref iconSOI, "img_listiconSOI.tga");
                KACUtils.LoadImageFromGameDB(ref iconMNode, "img_listiconMNode.tga");
                KACUtils.LoadImageFromGameDB(ref iconAp, "img_listiconAp.tga");
                KACUtils.LoadImageFromGameDB(ref iconPe, "img_listiconPe.tga");
                KACUtils.LoadImageFromGameDB(ref iconAN, "img_listiconAN.tga");
                KACUtils.LoadImageFromGameDB(ref iconDN, "img_listiconDN.tga");
                KACUtils.LoadImageFromGameDB(ref iconXFer, "img_listiconXfer.tga");
                KACUtils.LoadImageFromGameDB(ref iconClosest, "img_listiconClosest.tga");
                KACUtils.LoadImageFromGameDB(ref iconCrew, "img_listiconCrew.tga");
                KACUtils.LoadImageFromGameDB(ref iconEarth, "img_listiconEarth.tga");
                KACUtils.LoadImageFromGameDB(ref iconLaunchRendezvous, "img_listiconLaunchRendezvous.tga");
                KACUtils.LoadImageFromGameDB(ref iconWarpList100, "img_listiconWarpList_100.tga");
                KACUtils.LoadImageFromGameDB(ref iconWarpList080, "img_listiconWarpList_080.tga");
                KACUtils.LoadImageFromGameDB(ref iconWarpList060, "img_listiconWarpList_060.tga");
                KACUtils.LoadImageFromGameDB(ref iconWarpList040, "img_listiconWarpList_040.tga");
                KACUtils.LoadImageFromGameDB(ref iconWarpList020, "img_listiconWarpList_020.tga");
                KACUtils.LoadImageFromGameDB(ref iconWarpList000, "img_listiconWarpList_000.tga");

                KACUtils.LoadImageFromGameDB(ref iconPauseList100, "img_listiconPauseList_100.tga");
                KACUtils.LoadImageFromGameDB(ref iconPauseList080, "img_listiconPauseList_080.tga");
                KACUtils.LoadImageFromGameDB(ref iconPauseList060, "img_listiconPauseList_060.tga");
                KACUtils.LoadImageFromGameDB(ref iconPauseList040, "img_listiconPauseList_040.tga");
                KACUtils.LoadImageFromGameDB(ref iconPauseList020, "img_listiconPauseList_020.tga");
                KACUtils.LoadImageFromGameDB(ref iconPauseList000, "img_listiconPauseList_000.tga");

                KACUtils.LoadImageFromGameDB(ref iconNone, "img_listiconNone.tga");
                KACUtils.LoadImageFromGameDB(ref iconEdit, "img_listiconEdit.tga");

                //KACUtils.LoadImageFromGameDB(ref iconstatusSOI, "img_statusiconSOI.png");

                KACUtils.LoadImageFromGameDB(ref btnRaw, "img_buttonTypeRaw.tga");
                KACUtils.LoadImageFromGameDB(ref btnMNode, "img_buttonTypeMNode.tga");
                KACUtils.LoadImageFromGameDB(ref btnAp, "img_buttonTypeAp.tga");
                KACUtils.LoadImageFromGameDB(ref btnPe, "img_buttonTypePe.tga");
                KACUtils.LoadImageFromGameDB(ref btnApPe, "img_buttonTypeApPe.tga");
                KACUtils.LoadImageFromGameDB(ref btnAN, "img_buttonTypeAN.tga");
                KACUtils.LoadImageFromGameDB(ref btnDN, "img_buttonTypeDN.tga");
                KACUtils.LoadImageFromGameDB(ref btnANDN, "img_buttonTypeANDN.tga");
                KACUtils.LoadImageFromGameDB(ref btnSOI, "img_buttonTypeSOI.tga");
                KACUtils.LoadImageFromGameDB(ref btnXfer, "img_buttonTypeXfer.tga");
                KACUtils.LoadImageFromGameDB(ref btnClosest, "img_buttonTypeClosest.tga");
                KACUtils.LoadImageFromGameDB(ref btnCrew, "img_buttonTypeCrew.tga");

                KACUtils.LoadImageFromGameDB(ref btnChevronUp, "img_buttonChevronUp.tga");
                KACUtils.LoadImageFromGameDB(ref btnChevronDown, "img_buttonChevronDown.tga");
                KACUtils.LoadImageFromGameDB(ref btnChevLeft, "img_buttonChevronLeft.tga");
                KACUtils.LoadImageFromGameDB(ref btnChevRight, "img_buttonChevronRight.tga");

                KACUtils.LoadImageFromGameDB(ref btnRedCross, "img_buttonRedCross.tga");
                KACUtils.LoadImageFromGameDB(ref btnSettings, "img_buttonSettings.tga");
                KACUtils.LoadImageFromGameDB(ref btnSettingsAttention, "img_buttonSettingsAttention.tga");
                KACUtils.LoadImageFromGameDB(ref btnAdd, "img_buttonAdd.tga");

                KACUtils.LoadImageFromGameDB(ref btnActionMsg, "img_buttonActionMsg.tga");
                KACUtils.LoadImageFromGameDB(ref btnActionWarp, "img_buttonActionWarp.tga");
                KACUtils.LoadImageFromGameDB(ref btnActionWarpMsg, "img_buttonActionWarpMsg.tga");
                KACUtils.LoadImageFromGameDB(ref btnActionPause, "img_buttonActionPause.tga");

                KACUtils.LoadImageFromGameDB(ref btnDropDown, "img_DropDown.tga");
                KACUtils.LoadImageFromGameDB(ref btnPlay, "img_Play.tga");
                KACUtils.LoadImageFromGameDB(ref btnStop, "img_Stop.tga");

                KACUtils.LoadImageFromGameDB(ref texBox, "tex_Box.tga");
                KACUtils.LoadImageFromGameDB(ref texBoxUnity, "tex_BoxUnity.tga");

                KACUtils.LoadImageFromGameDB(ref texSeparatorH, "img_SeparatorHorizontal.tga");
                KACUtils.LoadImageFromGameDB(ref texSeparatorV, "img_SeparatorVertical.tga");


                //KACUtils.LoadImageFromGameDB(ref txtRedTint, "Textures", "RedOverlay.png");

                //KACUtils.LoadImageFromGameDB(ref txtBlackSquare, "Textures", "BlackSquare.png");
                //KACUtils.LoadImageFromGameDB(ref txtWhiteSquare, "Textures", "WhiteSquare.png");

                MonoBehaviourExtended.LogFormatted("Loaded Textures");
            }
            catch (Exception)
            {
                MonoBehaviourExtended.LogFormatted("Failed to Load Textures - are you missing a file?");
            }


        }

        internal static Texture2D GetSettingsButtonIcon(Boolean AttentionRequired)
        {
            Texture2D textureReturn;

            //Only flash if we need attention
            if (AttentionRequired && DateTime.Now.Millisecond < 500)
                textureReturn = btnSettingsAttention;
            else
                textureReturn = btnSettings;

            return textureReturn;
        }
        internal static Texture2D GetWarpIcon()
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

        internal static Texture2D GetPauseIcon()
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

        internal static String GetWarpIconTexturePath()
        {
            String textureReturn = "TriggerTech/ToolbarIcons/KACIcon-WarpEffect2_";

            textureReturn = GetIconPercentageFromTime(textureReturn);
            return textureReturn;
        }

        internal static String GetPauseIconTexturePath()
        {
            String textureReturn = "TriggerTech/ToolbarIcons/KACIcon-PauseEffect_";

            textureReturn = GetIconPercentageFromTime(textureReturn);
            return textureReturn;
        }

        private static string GetIconPercentageFromTime(String textureReturn)
        {
            Double intHundredth = Math.Truncate(DateTime.Now.Millisecond / 100d);
            switch (Convert.ToInt64(intHundredth))
            {
                case 0:
                    textureReturn += "100"; break;
                case 1:
                case 9:
                    textureReturn += "080"; break;
                case 2:
                case 8:
                    textureReturn += "060"; break;
                case 3:
                case 7:
                    textureReturn += "040"; break;
                case 4:
                case 6:
                    textureReturn += "020"; break;
                case 5:
                    textureReturn += "000"; break;
                default:
                    textureReturn += textureReturn += "100"; break;
            }
            return textureReturn;
        }

        internal static Texture2D GetWarpListIcon(Boolean blnWarpInfluence)
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

        internal static Texture2D GetPauseListIcon(Boolean blnPauseInfluence)
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


        #region Skins

        /// <summary>
        /// This is a copy of the default Unity skin
        /// </summary>
        internal static GUISkin DefUnitySkin { get; private set; }
        /// <summary>
        /// This is a copy of the default KSP skin
        /// </summary>
        internal static GUISkin DefKSPSkin { get; private set; }

        private static GUISkin _CurrentSkin;
        /// <summary>
        /// Will return the current Skin as controlled by the SetSkin() Methods
        /// </summary>
        internal static GUISkin CurrentSkin { get { return _CurrentSkin; } }


        internal static void InitSkins()
        {
            DefUnitySkin = GUI.skin;
            DefKSPSkin = HighLogic.Skin;

            SetSkin(KerbalAlarmClock.settings.SelectedSkin);
        }

        internal static void SetSkin(Settings.DisplaySkin SkinToSet)
        {
            switch (SkinToSet)
            {
                case Settings.DisplaySkin.Default:
                    _CurrentSkin = DefKSPSkin;
                    SetStyleDefaults();
                    SetKSPStyles();
                    SetKSPButtons();
                    break;
                case Settings.DisplaySkin.Unity:
                    _CurrentSkin = DefUnitySkin;
                    SetStyleDefaults(); //was 12
                    SetUnityStyles();
                    SetUnityButtons();
                    break;
                case Settings.DisplaySkin.UnityWKSPButtons:
                    _CurrentSkin = DefUnitySkin;
                    SetStyleDefaults();
                    SetUnityStyles();
                    SetKSPButtons();
                    break;
                default:
                    _CurrentSkin = DefKSPSkin;
                    SetStyleDefaults();
                    SetKSPStyles();
                    SetKSPButtons();
                    break;
            }

            SetStyles();

            //this throws an error
            if (OnSkinChanged!=null)
                OnSkinChanged();
        }

        internal delegate void SkinChangedEvent();
        internal static event SkinChangedEvent OnSkinChanged;

        static GUIStyle styleDefLabel, styleDefTextField, styleDefTextArea, styleDefToggle, styleDefButton;
        static int intFontSizeDefault;
        private static void SetStyleDefaults(Int32 FontSize=13)
        {
            Color32 colLabelText = new Color32(220, 220, 220, 255);
            intFontSizeDefault = FontSize;

            //Common starting points
            styleDefLabel = new GUIStyle(CurrentSkin.label);
            styleDefLabel.fontSize = intFontSizeDefault;
            styleDefLabel.fontStyle = FontStyle.Normal;
            styleDefLabel.normal.textColor = colLabelText;
            styleDefLabel.hover.textColor = Color.blue;

            styleDefTextField = new GUIStyle(CurrentSkin.textField);
            styleDefTextField.fontSize = intFontSizeDefault;
            styleDefTextField.fontStyle = FontStyle.Normal;
            styleDefTextArea = new GUIStyle(CurrentSkin.textArea);
            styleDefTextArea.fontSize = intFontSizeDefault;
            styleDefTextArea.fontStyle = FontStyle.Normal;
            styleDefToggle = new GUIStyle(CurrentSkin.toggle);
            styleDefToggle.fontSize = intFontSizeDefault;
            styleDefToggle.fontStyle = FontStyle.Normal;
            styleDefToggle.stretchWidth = false;

            styleWindow = new GUIStyle(CurrentSkin.window);
            styleWindow.padding = KACUtils.SetWindowRectOffset(styleWindow.padding, 4);
            //styleWindow.normal.background = KACResources.txtWhiteSquare;
            //styleWindow.normal.textColor = new Color32(183, 254, 0, 255);
            //styleWindow.normal.textColor = Color.red;

        }
        private static void SetKSPStyles()
        {
            texTooltip = texBox;

            styleDropDownListBox = new GUIStyle();
            styleDropDownListBox.normal.background = texBox;
            //Extra border to prevent bleed of color - actual border is only 1 pixel wide
            styleDropDownListBox.border = new RectOffset(3, 3, 3, 3);

        }
        private static void SetUnityStyles()
        {
            texTooltip = texBoxUnity;

            styleDropDownListBox = new GUIStyle();
            styleDropDownListBox.normal.background = texBoxUnity;
            //Extra border to prevent bleed of color - actual border is only 1 pixel wide
            styleDropDownListBox.border = new RectOffset(3, 3, 3, 3);
        }
        private static void SetKSPButtons()
        {
            styleDefButton = new GUIStyle(DefKSPSkin.button);
            styleDefToggle.fontSize = intFontSizeDefault;
            styleDefToggle.fontStyle = FontStyle.Normal;

            styleDropDownButton = new GUIStyle(styleDefButton);
            styleDropDownButton.fontSize = intFontSizeDefault;
            styleDropDownButton.fixedHeight = 20;
            if (KerbalAlarmClock.settings.SelectedSkin== Settings.DisplaySkin.UnityWKSPButtons)
                styleDropDownButton.padding.top = 4;
            else
                styleDropDownButton.padding.top = 8;
            styleDropDownButton.padding.right = 20;

        }
        private static void SetUnityButtons()
        {
            styleDefButton = new GUIStyle(DefUnitySkin.button);
            styleDefToggle.fontSize = intFontSizeDefault;
            styleDefToggle.fontStyle = FontStyle.Normal;

            styleDropDownButton = new GUIStyle(styleDefButton);
            styleDropDownButton.fontSize = intFontSizeDefault;
            styleDropDownButton.fixedHeight = 20;
            styleDropDownButton.padding.top = 4;
            styleDropDownButton.padding.right = 20;
        }


        #endregion

        #region "Styles"
        //Styles for windows - Cant initialize the objects here as the GUIStyle Constructor cannot be called outside of OnGUI

        internal static GUIStyle styleWindow;
        internal static GUIStyle styleTooltipStyle;

        internal static GUIStyle styleIconStyle;
        internal static GUIStyle styleHeading;
        internal static GUIStyle styleContent;

        internal static GUIStyle styleHeadingEarth;
        internal static GUIStyle styleContentEarth;

        internal static GUIStyle styleButton;

        internal static GUIStyle styleLabel;
        internal static GUIStyle styleLabelWarning;
        internal static GUIStyle styleLabelError;

        internal static GUIStyle styleCheckbox;
        internal static GUIStyle styleCheckboxLabel;

        internal static GUIStyle styleButtonList;
        internal static GUIStyle styleButtonListAlarmActions;

        internal static GUIStyle styleSmallButton;

        internal static GUIStyle styleFlagIcon;

        //List Styles
        internal static GUIStyle styleAlarmListArea;
        internal static GUIStyle styleAlarmText;
        //internal static GUIStyle styleAlarmTextGrayed;
        internal static GUIStyle styleAlarmIcon;
        internal static GUIStyle styleLabelWarp;
        internal static GUIStyle styleLabelWarpGrayed;
        internal static GUIStyle styleSOIIndicator;
        internal static GUIStyle styleSOIIcon;

        //Add Alarm Styles
        internal static GUIStyle styleAddSectionHeading;
        internal static GUIStyle styleAddHeading;
        internal static GUIStyle styleAddField;
        internal static GUIStyle styleAddFieldError;
        //internal static GUIStyle styleAddFieldErorOverlay;
        internal static GUIStyle styleAddFieldGreen;
        internal static GUIStyle styleAddFieldAreas;
        internal static GUIStyle styleAddAlarmArea;
        internal static GUIStyle styleAddXferName;
        internal static GUIStyle styleAddXferButton;
        internal static GUIStyle styleAddXferOriginButton;
        internal static GUIStyle styleAddMessageField;

        //AlarmMessage Styles
        internal static GUIStyle styleAlarmMessage;
        internal static GUIStyle styleAlarmMessageTime;
        internal static GUIStyle styleAlarmMessageAction;
        internal static GUIStyle styleAlarmMessageActionPause;

        internal static GUIStyle styleVersionHighlight;

        #region DropdownStuff
        internal static GUIStyle styleDropDownButton;
        internal static GUIStyle styleDropDownListBox;
        internal static GUIStyle styleDropDownListItem;

        internal static GUIStyle styleDropDownGlyph;

        internal static GUIStyle styleSeparatorV;
        internal static GUIStyle styleSeparatorH;
        #endregion

        internal static List<GUIContent> lstAlarmChoices;

        /// <summary>
        /// Sets up the styles for the different parts of the drawing
        /// Should only be called once
        /// </summary>
        internal static void SetStyles()
        {
            //Color32 colLabelText = new Color32(220, 220, 220, 255);
            //int intFontSizeDefault = 13;

            ////Common starting points
            //GUIStyle styleDefLabel = new GUIStyle(CurrentSkin.label);
            //styleDefLabel.fontSize = intFontSizeDefault;
            //styleDefLabel.fontStyle = FontStyle.Normal;
            //styleDefLabel.normal.textColor = colLabelText;
            //styleDefLabel.hover.textColor = Color.blue;

            //GUIStyle styleDefTextField = new GUIStyle(CurrentSkin.textField);
            //styleDefTextField.fontSize = intFontSizeDefault;
            //styleDefTextField.fontStyle = FontStyle.Normal;
            //GUIStyle styleDefTextArea = new GUIStyle(CurrentSkin.textArea);
            //styleDefTextArea.fontSize = intFontSizeDefault;
            //styleDefTextArea.fontStyle = FontStyle.Normal;
            //GUIStyle styleDefToggle = new GUIStyle(CurrentSkin.toggle);
            //styleDefToggle.fontSize = intFontSizeDefault;
            //styleDefToggle.fontStyle = FontStyle.Normal;
            //GUIStyle styleDefButton = new GUIStyle(CurrentSkin.button);
            //styleDefToggle.fontSize = intFontSizeDefault;
            //styleDefToggle.fontStyle = FontStyle.Normal;

            //Set up the used styles
            styleIconStyle = new GUIStyle(styleDefButton);
            styleIconStyle.fixedHeight = 32;
            styleIconStyle.fixedWidth = 32;
            styleIconStyle.padding = new RectOffset(0, 0, 0, 0);

            //styleWindow = new GUIStyle(CurrentSkin.window);
            //styleWindow.padding = KACUtils.SetWindowRectOffset(styleWindow.padding, 4);
            ////styleWindow.normal.background = KACResources.txtWhiteSquare;
            ////styleWindow.normal.textColor = new Color32(183, 254, 0, 255);
            ////styleWindow.normal.textColor = Color.red;

            styleTooltipStyle = new GUIStyle(styleDefLabel);
            styleTooltipStyle.fontSize = 12;
            styleTooltipStyle.normal.textColor = new Color32(207, 207, 207, 255);
            styleTooltipStyle.stretchHeight = true;
            styleTooltipStyle.wordWrap = true;
            styleTooltipStyle.normal.background = texTooltip;
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
            styleCheckboxLabel.stretchWidth=false;
            styleCheckboxLabel.alignment = TextAnchor.MiddleLeft;

            styleButtonList = new GUIStyle(styleDefButton);
            styleButtonList.fixedHeight = 26;
            styleButtonList.padding = KACUtils.SetRectOffset(styleButtonList.padding, 0);
            styleButtonList.onNormal.background = styleButtonList.active.background;

            styleButtonListAlarmActions = new GUIStyle(styleDefButton);
            styleButtonListAlarmActions.fixedHeight = 22;
            styleButtonListAlarmActions.fixedWidth = 40;
            styleButtonListAlarmActions.padding = KACUtils.SetRectOffset(styleButtonList.padding, 0);
            styleButtonListAlarmActions.onNormal.background = styleButtonListAlarmActions.active.background;

            styleSmallButton = new GUIStyle(styleDefButton);
            styleSmallButton.alignment = TextAnchor.MiddleCenter;
            styleSmallButton.fixedWidth = 30;
            styleSmallButton.fixedHeight = 20;
            styleSmallButton.fontSize = intFontSizeDefault;
            styleSmallButton.fontStyle = FontStyle.Normal;
            styleSmallButton.padding = KACUtils.SetRectOffset(styleSmallButton.padding, 0);
            styleSmallButton.onNormal.background = styleSmallButton.active.background;

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

            styleDropDownListItem = new GUIStyle();
            styleDropDownListItem.normal.textColor = new Color(207, 207, 207);
            Texture2D texBack = CreateColorPixel(new Color(207, 207, 207));
            styleDropDownListItem.hover.background = texBack;
            styleDropDownListItem.onHover.background = texBack;
            styleDropDownListItem.hover.textColor = Color.black;
            styleDropDownListItem.onHover.textColor = Color.black;
            styleDropDownListItem.padding = new RectOffset(4, 4, 3, 4);

            styleDropDownGlyph = new GUIStyle();
            styleDropDownGlyph.alignment = TextAnchor.MiddleCenter;

            styleSeparatorV = new GUIStyle();
            styleSeparatorV.normal.background = texSeparatorV;
            styleSeparatorV.border = new RectOffset(0, 0, 6, 6);
            styleSeparatorV.fixedWidth = 2;

            styleSeparatorH = new GUIStyle();
            styleSeparatorH.normal.background = texSeparatorH;
            styleSeparatorH.border = new RectOffset(6, 6, 0, 0);
            styleSeparatorH.fixedHeight = 2;

            lstAlarmChoices = new List<GUIContent>();
            lstAlarmChoices.Add(new GUIContent(btnActionMsg, KACAlarm.AlarmActionEnum.MessageOnly.Description()));
            lstAlarmChoices.Add(new GUIContent(btnActionWarp, KACAlarm.AlarmActionEnum.KillWarpOnly.Description()));
            lstAlarmChoices.Add(new GUIContent(btnActionWarpMsg, KACAlarm.AlarmActionEnum.KillWarp.Description()));
            lstAlarmChoices.Add(new GUIContent(btnActionPause, KACAlarm.AlarmActionEnum.PauseGame.Description()));
        }

        /// <summary>
        /// Creates a 1x1 texture
        /// </summary>
        /// <param name="Background">Color of the texture</param>
        /// <returns></returns>
        internal static Texture2D CreateColorPixel(Color32 Background)
        {
            Texture2D retTex = new Texture2D(1, 1);
            retTex.SetPixel(0, 0, Background);
            retTex.Apply();
            return retTex;
        }

        #endregion


        #region "Functions"
        //internal static Color PulseColor(Color Start, Color Dest)
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
        internal static List<KACXFerModelPoint> lstXferModelPoints;

        internal static Boolean LoadModelPoints()
        {
            MonoBehaviourExtended.LogFormatted("Loading Transfer Modelling Data");
            Boolean blnReturn = false;
            try
            {
                lstXferModelPoints = new List<KACXFerModelPoint>();

                //read in the data file
                //String strData = KSP.IO.File.ReadAllText<KerbalAlarmClock>("data_TransferModelData.csv");
                String strData = System.IO.File.ReadAllText(KACUtils.PathPluginData + "/data_TransferModelData.csv");
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
                MonoBehaviourExtended.LogFormatted("Transfer Modelling Data Load Complete");
            }
            catch (Exception ex)
            {
                MonoBehaviourExtended.LogFormatted("Transfer Modelling Data Failed - is the data file there and correct\r\n{0}", ex.Message);
            }
            return blnReturn;
        }
        #endregion
    }
}
