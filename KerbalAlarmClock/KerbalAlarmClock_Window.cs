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
    public partial class KerbalAlarmClock
    {


        #region "OnGUI Stuff"
        System.Random rnd = new System.Random();
        //public void SetupDrawStuff()
        //{
        //    GUI.skin = HighLogic.Skin;
        //    if (KACResources.styleWindow == null)
        //    {
        //        KACResources.SetStyles();
        //        //styleWindow = new GUIStyle(GUI.skin.window);
        //    }
        //}

        #region "Tooltip Work"
        //Tooltip variables
        //Store the tooltip text from throughout the code
        String strToolTipText = "";
        String strLastTooltipText = "";
        //is it displayed and where
        Boolean blnToolTipDisplayed = false;
        Rect rectToolTipPosition;
        Int32 intTooltipVertOffset = 12;
        Int32 intTooltipMaxWidth = 250;
        //timer so it only displays for a preriod of time
        float fltTooltipTime = 0f;
        float fltMaxToolTipTime = 15f;


        private void DrawToolTip()
        {
            if (strToolTipText != "" && (fltTooltipTime < fltMaxToolTipTime))
            {
                GUIContent contTooltip = new GUIContent(strToolTipText);
                if (!blnToolTipDisplayed || (strToolTipText != strLastTooltipText))
                {
                    //reset display time if text changed
                    fltTooltipTime = 0f;
                    //Calc the size of the Tooltip
                    rectToolTipPosition = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y + intTooltipVertOffset, 0, 0);
                    float minwidth, maxwidth;
                    KACResources.styleTooltipStyle.CalcMinMaxWidth(contTooltip, out minwidth, out maxwidth); // figure out how wide one line would be
                    rectToolTipPosition.width = Math.Min(intTooltipMaxWidth - KACResources.styleTooltipStyle.padding.horizontal, maxwidth); //then work out the height with a max width
                    rectToolTipPosition.height = KACResources.styleTooltipStyle.CalcHeight(contTooltip, rectToolTipPosition.width); // heers the result
                    //Make sure its not off the right of the screen
                    if (rectToolTipPosition.x + rectToolTipPosition.width > Screen.width) rectToolTipPosition.x = Screen.width - rectToolTipPosition.width;
                }
                //Draw the Tooltip
                GUI.Label(rectToolTipPosition, contTooltip, KACResources.styleTooltipStyle);
                //On top of everything
                GUI.depth = 0;

                //update how long the tip has been on the screen and reset the flags
                fltTooltipTime += Time.deltaTime;
                blnToolTipDisplayed = true;
            }
            else
            {
                //clear the flags
                blnToolTipDisplayed = false;
            }
            if (strToolTipText != strLastTooltipText) fltTooltipTime = 0f;
            strLastTooltipText = strToolTipText;
        }

        internal void SetTooltipText()
        {
            if (Event.current.type == EventType.Repaint)
            {
                strToolTipText = GUI.tooltip;
            }
        }
        #endregion

        #region "Stuff to get icon and windows by scene"
        public Boolean IconShowByActiveScene
        {
            get
            {
                switch (KACWorkerGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: return settings.IconShow_SpaceCenter;
                    case GameScenes.TRACKSTATION: return settings.IconShow_TrackingStation;
                    default: return true;
                }
            }
            set
            {
                switch (KACWorkerGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: settings.IconShow_SpaceCenter = value; break;
                    case GameScenes.TRACKSTATION: settings.IconShow_TrackingStation = value; break;
                    default: 
                        //Settings.WindowVisible = value; 
                        break;
                }
            }
        }

        public Rect IconPosByActiveScene
        {
            get
            {
                switch (KACWorkerGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: return settings.IconPos_SpaceCenter;
                    case GameScenes.TRACKSTATION: return settings.IconPos_TrackingStation;
                    default: return settings.IconPos;
                }
            }
            set
            {
                switch (KACWorkerGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: settings.IconPos_SpaceCenter = value; break;
                    case GameScenes.TRACKSTATION: settings.IconPos_TrackingStation = value; break;
                    default: settings.IconPos = value; break;
                }
            }
        }


        public Boolean WindowVisibleByActiveScene
        {
            get
            {
                switch (KACWorkerGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: return settings.WindowVisible_SpaceCenter;
                    case GameScenes.TRACKSTATION: return settings.WindowVisible_TrackingStation;
                    default: return settings.WindowVisible;
                }
            }
            set
            {
                switch (KACWorkerGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: settings.WindowVisible_SpaceCenter = value; break;
                    case GameScenes.TRACKSTATION: settings.WindowVisible_TrackingStation = value; break;
                    default: settings.WindowVisible = value; break;
                }
            }
        }

        public Boolean WindowMinimizedByActiveScene
        {
            get
            {
                switch (KACWorkerGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: return settings.WindowMinimized_SpaceCenter;
                    case GameScenes.TRACKSTATION: return settings.WindowMinimized_TrackingStation;
                    default: return settings.WindowMinimized;
                }
            }
            set
            {
                switch (KACWorkerGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: settings.WindowMinimized_SpaceCenter = value; break;
                    case GameScenes.TRACKSTATION: settings.WindowMinimized_TrackingStation = value; break;
                    default: settings.WindowMinimized = value; break;
                }
            }
        }

        public Rect WindowPosByActiveScene
        {
            get {
                switch (KACWorkerGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER:    return settings.WindowPos_SpaceCenter;
                    case GameScenes.TRACKSTATION:   return settings.WindowPos_TrackingStation;
                    default:                        return settings.WindowPos;
                }
            }
            set {
                switch (KACWorkerGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER:    settings.WindowPos_SpaceCenter = value;         break;
                    case GameScenes.TRACKSTATION:   settings.WindowPos_TrackingStation = value;     break;
                    default:                        settings.WindowPos = value;                     break;
                }
            }
        }
        #endregion

        /// <summary>
        /// Draw the icon on the screen
        /// </summary>
        internal void DrawIcons()
        {
            //if (!settings.UseBlizzyToolbarIfAvailable || btnToolbarKAC == null)
            if (settings.ButtonStyleToDisplay == Settings.ButtonStyleEnum.Basic)
            {
            Texture2D iconToShow;
            //Replace this with workerstate object that can test for pause and catch errors - is it doing this in flight mode??
            if (!(KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT) || (!KACWorkerGameState.PauseMenuOpen && !KACWorkerGameState.FlightResultsDialogOpen))
            {
                if (FlightDriver.Pause)
                {
                    iconToShow = KACResources.GetPauseIcon();
                }
                else if (KACWorkerGameState.CurrentlyUnderWarpInfluence)
                {
                    iconToShow = KACResources.GetWarpIcon();
                }
                else
                {
                    if (alarms.ActiveEnabledFutureAlarms(HighLogic.CurrentGame.Title))
                    {
                        if (WindowVisibleByActiveScene)
                            iconToShow = KACResources.iconAlarmShow;
                        else
                            iconToShow = KACResources.iconAlarm;
                    }
                    else
                    {
                        if (WindowVisibleByActiveScene)
                            iconToShow = KACResources.iconNormShow;
                        else
                            iconToShow = KACResources.iconNorm;
                    }
                }

                //draw the icon button
                if (IconShowByActiveScene)
                {
                    if (GUI.Button(IconPosByActiveScene, new GUIContent(iconToShow, "Click to Toggle"), KACResources.styleIconStyle))
                    {
                        //switch (KACWorkerGameState.CurrentGUIScene)
                        //{
                        //    case GameScenes.SPACECENTER: Settings.WindowVisible_SpaceCenter = !Settings.WindowVisible_SpaceCenter; break;
                        //    case GameScenes.TRACKSTATION: Settings.WindowVisible_TrackingStation = !Settings.WindowVisible_TrackingStation;break;
                        //    default: Settings.WindowVisible = !Settings.WindowVisible;break;
                        //}
                        WindowVisibleByActiveScene = !WindowVisibleByActiveScene;
                        settings.Save();
                    }
                    //GUI.Label(new Rect(152,32,200,20), GUI.tooltip,KACResources.styleTooltipStyle);
                }
            }
        }
            else if (btnToolbarKAC != null) { 
            //{
                //Do for Blizzies Toolbar
                //if (btnToolbarKAC != null) { 
                    String TexturePath = "";
                    if (!(KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT) || (!KACWorkerGameState.PauseMenuOpen && !KACWorkerGameState.FlightResultsDialogOpen))
                    {
                        if (FlightDriver.Pause)
                        {
                            TexturePath = KACResources.GetPauseIconTexturePath();
                        }
                        else if (KACWorkerGameState.CurrentlyUnderWarpInfluence)
                        {
                            TexturePath = KACResources.GetWarpIconTexturePath();
                        }
                        else
                        {
                            if (alarms.ActiveEnabledFutureAlarms(HighLogic.CurrentGame.Title))
                            {
                                if (WindowVisibleByActiveScene)
                                    TexturePath = KACUtils.PathToolbarTexturePath + "/KACIcon-AlarmShow";
                                else
                                    TexturePath = KACUtils.PathToolbarTexturePath + "/KACIcon-Alarm";
                            }
                            else
                            {
                                if (WindowVisibleByActiveScene)
                                    TexturePath = KACUtils.PathToolbarTexturePath + "/KACIcon-NormShow";
                                else
                                    TexturePath = KACUtils.PathToolbarTexturePath + "/KACIcon-Norm";
                            }
                        }

                        btnToolbarKAC.TexturePath = TexturePath;
                    }
                //}
            }
            else if (btnAppLauncher != null)
            {
                Texture2D iconToShow;
                //Replace this with workerstate object that can test for pause and catch errors - is it doing this in flight mode??
                if (!(KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT) || (!KACWorkerGameState.PauseMenuOpen && !KACWorkerGameState.FlightResultsDialogOpen))
                {
                    if (FlightDriver.Pause)
                    {
                        iconToShow = KACResources.GetPauseIcon(true);
                    }
                    else if (KACWorkerGameState.CurrentlyUnderWarpInfluence)
                    {
                        iconToShow = KACResources.GetWarpIcon(true);
                    }
                    else
                    {
                        if (alarms.ActiveEnabledFutureAlarms(HighLogic.CurrentGame.Title))
                        {
                            if (WindowVisibleByActiveScene)
                                iconToShow = KACResources.toolbariconAlarmShow;
                            else
                                iconToShow = KACResources.toolbariconAlarm;
                        }
                        else
                        {
                            if (WindowVisibleByActiveScene)
                                iconToShow = KACResources.toolbariconNormShow;
                            else
                                iconToShow = KACResources.toolbariconNorm;
                        }
                    }
                    btnAppLauncher.SetTexture(iconToShow);
                }
            }
        }



        //Basic setup of draw stuff
        internal static Int32 _WindowMainID = 0;

        //is the add pane visible
        private Boolean _ShowAddPane = false;
        private Boolean _ShowAddPaneOnLeft = false;
        private static Int32 _WindowAddID = 0;
        private static Rect _WindowAddRect;

        //is the add pane messages pane visible
        private Boolean _ShowAddMessages = false;
        private static Int32 _WindowAddMessagesID = 0;
        private static Rect _WindowAddMessagesRect;

        //Settings Window
        private Boolean _ShowSettings = false;
        private Boolean _ShowShowSettingsOnLeft = false;
        private static Int32 _WindowSettingsID = 0;
        private static Rect _WindowSettingsRect;

        //Edit Window
        private Boolean _ShowEditPane = false;
        private Boolean _ShowEditPaneOnLeft = false;
        private static Int32 _WindowEditID = 0;
        private static Rect _WindowEditRect;

        //Earth Alarm Window
        private Boolean _ShowEarthAlarm = false;
        private Boolean _ShowEarthAlarmOnLeft = false;
        private static Int32 _WindowEarthAlarmID = 0;
        private static Rect _WindowEarthAlarmRect;

        //Earth Alarm Window
        private Boolean _ShowBackupFailedMessage = false;
        private DateTime _ShowBackupFailedMessageAt = DateTime.Now;
        private Int32 _ShowBackupFailedMessageForSecs=10;
        private static Int32 _WindowBackupFailedID = 0;
        private static Rect _WindowBackupFailedRect;

        private Boolean _ShowQuickAdd = false;
        private Boolean _ShowQuickAddOnLeft = false;
        private static Int32 _WindowQuickAddID = 0;
        private static Rect _WindowQuickAddRect;


        private static Windows.AlarmImport winAlarmImport = new Windows.AlarmImport();
        private static Windows.ConfirmAlarmDelete winConfirmAlarmDelete = new Windows.ConfirmAlarmDelete();

        //Window Size Constants
        private Int32 intMainWindowWidth = 340;
        private Int32 intMainWindowMinHeight = 110;
        private Int32 intMainWindowBaseHeight = 111;

        private Int32 intMainWindowAlarmListItemHeight = 26;
        private Int32 intMainWindowAlarmListScrollPad = 3;
        private Int32 intMainWindowEarthTimeHeight = 25;

        private Int32 intPaneWindowWidth = 380;
        private Int32 intAddPaneWindowWidth = 340;
        private Int32 AddWindowHeight;

        private Int32 EarthWindowHeight = 216;
        private Int32 QuickWindowHeight = 28;

        internal void DrawWindows()
        {
#if DEBUG
            if (_ShowDebugPane)
            {
                _WindowDebugRect = GUILayout.Window(_WindowDebugID, _WindowDebugRect, FillDebugWindow, "Debug");
            }
#endif
            //set initial values for rect from old ones - ignore old width
            Rect MainWindowPos = new Rect(WindowPosByActiveScene.x, WindowPosByActiveScene.y, intMainWindowWidth, WindowPosByActiveScene.height);
            
            //Min or normal window
            if (WindowMinimizedByActiveScene)
            {
                MainWindowPos.height = intMainWindowMinHeight-2;
            }
            else
            {
                MainWindowPos.height = intMainWindowBaseHeight;
                //Work out the number of alarms and therefore the height of the window
                if (alarms.Count > 1)
                {
                    //if (alarms.Count<2)
                    //    MainWindowPos.height = intMainWindowBaseHeight;
                    //else 
                    if (alarms.Count < settings.AlarmListMaxAlarmsInt)
                        MainWindowPos.height = intMainWindowBaseHeight + 
                            ((alarms.Count - 1) * intMainWindowAlarmListItemHeight) +
                            alarms.Sum(x=>x.AlarmLineHeightExtra);
                    else
                        //this is scrolling
                        MainWindowPos.height = (intMainWindowBaseHeight -3 )  +
                            ((settings.AlarmListMaxAlarmsInt - 1) * intMainWindowAlarmListItemHeight) + 
                            alarms.Take(settings.AlarmListMaxAlarmsInt).Sum(x=>x.AlarmLineHeightExtra) +
                            intMainWindowAlarmListScrollPad;
                }
                else MainWindowPos.height = intMainWindowBaseHeight;
            }
            if (KerbalAlarmClock.settings.SelectedSkin != Settings.DisplaySkin.Default)
                MainWindowPos.height -= 8;

            if (settings.ShowEarthTime)
            {
                MainWindowPos.height += intMainWindowEarthTimeHeight;
            }
            MainWindowPos = MainWindowPos.ClampToScreen(new RectOffset(0,0,-25,0));

            //Now show the window
            WindowPosByActiveScene = GUILayout.Window(_WindowMainID, MainWindowPos, FillWindow, "Kerbal Alarm Clock - " + settings.Version,KACResources.styleWindow);

            if (winAlarmImport.Visible)
                winAlarmImport.windowRect = GUILayout.Window(winAlarmImport.windowID, winAlarmImport.windowRect, winAlarmImport.FillWindow, "Import v2 Alarm File", KACResources.styleWindow);

            if (winConfirmAlarmDelete.Visible)
                winConfirmAlarmDelete.windowRect = GUILayout.Window(winConfirmAlarmDelete.windowID, 
                    new Rect(MainWindowPos.x + MainWindowPos.width,MainWindowPos.y,300,140),
                    winConfirmAlarmDelete.FillWindow, "Confirm Alarm Delete", KACResources.styleWindow);

            //Do we have anything to show in the right pane
            if (_ShowSettings)
            {
                _WindowSettingsRect = GUILayout.Window(_WindowSettingsID, WindowRectLeftRightToggle(WindowPosByActiveScene.x , WindowPosByActiveScene.width, WindowPosByActiveScene.y, intPaneWindowWidth, intSettingsHeight, ref _ShowShowSettingsOnLeft), FillSettingsWindow, "Settings and Globals", KACResources.styleWindow);
            }
            else if (_ShowAddPane)
            {
                switch (AddType)
                {
                    case KACAlarm.AlarmTypeEnum.Raw:
                        AddWindowHeight = 234; break; // 250;
                    case KACAlarm.AlarmTypeEnum.Maneuver:
                    case KACAlarm.AlarmTypeEnum.SOIChange:
                        AddWindowHeight = 170; break; // 182;
                    case KACAlarm.AlarmTypeEnum.Apoapsis:
                    case KACAlarm.AlarmTypeEnum.Periapsis:
                        AddWindowHeight = 200; break;// 208; 
                    case KACAlarm.AlarmTypeEnum.AscendingNode:
                    case KACAlarm.AlarmTypeEnum.DescendingNode:
                    case KACAlarm.AlarmTypeEnum.LaunchRendevous:
                        AddWindowHeight = 226; break;// 234; 
                    case KACAlarm.AlarmTypeEnum.Transfer:                        
                    case KACAlarm.AlarmTypeEnum.TransferModelled:
                        AddWindowHeight = intAddXferHeight; break;
                    case KACAlarm.AlarmTypeEnum.Closest:
                        AddWindowHeight = 252; break; //230;
                    case KACAlarm.AlarmTypeEnum.Distance:
                        AddWindowHeight = intAddDistanceHeight; break;
                    case KACAlarm.AlarmTypeEnum.Crew:
                        AddWindowHeight = intAddCrewHeight; break;
                    case KACAlarm.AlarmTypeEnum.Contract:
                    case KACAlarm.AlarmTypeEnum.ContractAuto:
                        AddWindowHeight = 400; break;
                    default: AddWindowHeight = 250; break;
                }
                AddWindowHeight += intHeight_AddWindowCommon;
                AddWindowHeight += intHeight_AddWindowRepeat;
                _WindowAddRect = GUILayout.Window(_WindowAddID, WindowRectLeftRightToggle(WindowPosByActiveScene.x , WindowPosByActiveScene.width, WindowPosByActiveScene.y, intAddPaneWindowWidth, AddWindowHeight,ref _ShowAddPaneOnLeft), FillAddWindow, "Add New Alarm", KACResources.styleWindow);                //switch (AddInterfaceType)

                if (_ShowAddMessages)
                {
                    _WindowAddMessagesRect = GUILayout.Window(_WindowAddMessagesID, new Rect(_WindowAddRect.x + _WindowAddRect.width, _WindowAddRect.y, 200, AddWindowHeight), FillAddMessagesWindow, "");
                }
            }
            else if (_ShowEarthAlarm)
            {
                float _WindowEarthTop = WindowPosByActiveScene.y + WindowPosByActiveScene.height - EarthWindowHeight;
                if (EarthWindowHeight > MainWindowPos.height) _WindowEarthTop = WindowPosByActiveScene.y;
                _WindowEarthAlarmRect = GUILayout.Window(_WindowEarthAlarmID, WindowRectLeftRightToggle(WindowPosByActiveScene.x, WindowPosByActiveScene.width, _WindowEarthTop, intAddPaneWindowWidth, EarthWindowHeight, ref _ShowEarthAlarmOnLeft), FillEarthAlarmWindow, "Add Earth Time Alarm", KACResources.styleWindow);                //switch (AddInterfaceType)
                if (_ShowAddMessages)
                {
                    _WindowAddMessagesRect = GUILayout.Window(_WindowAddMessagesID, new Rect(_WindowEarthAlarmRect.x + _WindowEarthAlarmRect.width, _WindowEarthAlarmRect.y, 200, EarthWindowHeight), FillAddMessagesWindow, "");
                }
            }
            else if (_ShowEditPane)
            {
                _WindowEditRect = GUILayout.Window(_WindowEditID, WindowRectLeftRightToggle(WindowPosByActiveScene.x ,WindowPosByActiveScene.width, WindowPosByActiveScene.y, intPaneWindowWidth, intAlarmEditHeight,ref _ShowEditPaneOnLeft), FillEditWindow, "Editing Alarm", KACResources.styleWindow);
            }
            else if (_ShowQuickAdd)
            {
                _WindowQuickAddRect = GUILayout.Window(_WindowQuickAddID, WindowRectLeftRightToggle(WindowPosByActiveScene.x , WindowPosByActiveScene.width, WindowPosByActiveScene.y, 300, QuickWindowHeight, ref _ShowQuickAddOnLeft), FillQuickWindow, "Quick Add", KACResources.styleWindow);
            }

            if (_ShowBackupFailedMessage)
            {
                _WindowBackupFailedRect = GUILayout.Window(_WindowBackupFailedID, _WindowBackupFailedRect, FillBackupFailedWindow, "Save Backup Failed", KACResources.styleWindow);
                if (DateTime.Now.Subtract(_ShowBackupFailedMessageAt).Seconds > _ShowBackupFailedMessageForSecs)
                    ResetBackupFailedWindow();
            }

            //Set DDL Window Positions
            SetDDLWindowPositions();

            if (settings.ShowTooltips)
                DrawToolTip();
        }

        /// <summary>
        /// Places the winodw on the left or right of the parent depending on the parents screen position
        /// </summary>
        /// <param name="leftParent">x of parent</param>
        /// <param name="widthParent">width of parent</param>
        /// <param name="top">y of both</param>
        /// <param name="width">width of child</param>
        /// <param name="height">height of child</param>
        /// <param name="ShowingOnLeft">Is it currently on the left?</param>
        /// <returns></returns>
        internal static Rect WindowRectLeftRightToggle(Single leftParent,Single widthParent, Single top, Single width, Single height, ref Boolean ShowingOnLeft)
        {
            //Default Rect
            Rect rectReturn = new Rect(leftParent + widthParent, top, width, height);

            //Is it on the right and going out of screen
            if (!ShowingOnLeft && ((leftParent + widthParent + width) > Screen.width))
            {
                //toggle side
                ShowingOnLeft=true;
            } else if (ShowingOnLeft && ((leftParent - width) < 0)) {
                //or on the left going going out then toggle side
                ShowingOnLeft = false;
            } 
            //if its on the left then change the left value
            if (ShowingOnLeft){
                rectReturn.x = leftParent - width;
            }
                //    LeftEdge = WindowPosByActiveScene.x - intPaneWindowWidth;)

            return rectReturn;
        }

        //Now the layout
        internal void FillWindow(Int32 intWindowID)
        {
            GUILayout.BeginVertical();

            //Heading Part
            GUILayout.BeginHorizontal();
            GUILayout.Label("Alarm List", KACResources.styleHeading, GUILayout.ExpandWidth(true));

            //No Longer Relevant
            //hide this stuff when not in alarm edit mode/flight mode
            //if (!ViewAlarmsOnly)
            //{
            if (settings.AlarmAddContractAutoOffered!= Settings.AutoContractBehaviorEnum.None || 
                settings.AlarmAddContractAutoActive!= Settings.AutoContractBehaviorEnum.None )
                {
                    String ContractTip = "Auto Contracts Enabled\r\n";
                    if (settings.AlarmAddContractAutoOffered == Settings.AutoContractBehaviorEnum.Next)
                        ContractTip += "Next Contract Offer";
                    else if (settings.AlarmAddContractAutoOffered == Settings.AutoContractBehaviorEnum.All)
                        ContractTip += "All Contract Offers";

                    if (settings.AlarmAddContractAutoActive == Settings.AutoContractBehaviorEnum.Next)
                        ContractTip += (ContractTip.Contains("\r\n")?" and ":"") + "Next Active Contract";
                    else if (settings.AlarmAddContractAutoActive == Settings.AutoContractBehaviorEnum.All)
                        ContractTip += (ContractTip.Contains("\r\n") ? " and " : "") + "All Active Contracts";
                    
                    GUIContent XferIcon = new GUIContent(KACResources.iconContract, ContractTip);
                    GUILayout.Label(XferIcon, KACResources.styleFlagIcon);
                }

                if (settings.AlarmNodeRecalc)
                {
                    GUIContent XferIcon = new GUIContent(KACResources.iconAp, "Orbit Node (Ap,Pe,AN,DN) Recalculation is enabled");
                    GUILayout.Label(XferIcon, KACResources.styleFlagIcon);
                }
                if (settings.AlarmXferRecalc)
                {
                    GUIContent XferIcon = new GUIContent(KACResources.iconXFer, "Transfer Recalculation is enabled");
                    GUILayout.Label(XferIcon, KACResources.styleFlagIcon);
                }

                //SOI AutoAlarm stuff
                if (settings.AlarmAddSOIAuto || settings.AlarmSOIRecalc)
                {
                    String SOITooltip = "";
                    if (settings.AlarmSOIRecalc)
                        SOITooltip = "SOI Recalculation is enabled";
                    if (settings.AlarmAddSOIAuto)
                    {
                        if (SOITooltip != "") SOITooltip += "\r\n";
                        SOITooltip += "SOI Auto Add Enabled";
                        //if (settings.AlarmCatchSOIChange)
                        //{
                        //    SOITooltip += "-plus catchall";
                        //}
                        if (settings.AlarmAddSOIAuto_ExcludeEVA)
                        {
                            SOITooltip += "-excluding EVA";
                        }
                        if (settings.AlarmAddSOIAuto_ExcludeDebris)
                        {
                            if (!SOITooltip.Contains("-excluding"))
                                SOITooltip += "-excluding Debris";
                            else
                                SOITooltip += "/Debris";
                        }
                        if (settings.AlarmOnSOIChange_Action == KACAlarm.AlarmActionEnum.PauseGame) SOITooltip += " (Pause Action)";
                        else if (settings.AlarmOnSOIChange_Action != KACAlarm.AlarmActionEnum.MessageOnly) SOITooltip += " (Warp Kill Action)";
                    }
                    GUIContent SOIIcon = new GUIContent(KACResources.iconSOI, SOITooltip);
                    GUILayout.Label(SOIIcon, KACResources.styleFlagIcon);
                }

                if (settings.AlarmAddManAuto)
                {
                    String strTooltip = "Man Node Auto Add Enabled";
                    if (settings.AlarmAddManAuto_andRemove)
                        strTooltip += " (and Removal)";

                    GUIContent ManIcon = new GUIContent(KACResources.iconMNode, strTooltip);
                    GUILayout.Label(ManIcon, KACResources.styleFlagIcon);
                }
            //}

            //Set a default for the MinMax button
            GUIContent contMaxMin = new GUIContent(KACResources.btnChevronUp, "Minimize");
            if (WindowMinimizedByActiveScene)
            {
                contMaxMin.image = KACResources.btnChevronDown;
                contMaxMin.tooltip = "Maximize";
            }
            //Draw the button
            if (GUILayout.Button(contMaxMin, KACResources.styleSmallButton))
            {
                WindowMinimizedByActiveScene = !WindowMinimizedByActiveScene;
                settings.Save();
            }

            GUIContent contSettings = new GUIContent(KACResources.GetSettingsButtonIcon(settings.VersionAttentionFlag), "Settings...");
            if (settings.VersionAvailable) contSettings.tooltip = "Updated Version Available - Settings...";
            if (DrawToggle(ref _ShowSettings, contSettings, KACResources.styleSmallButton) && _ShowSettings)
            {
                NewSettingsWindow();
                ResetPanes();
                    _ShowSettings = true;
            }
            //No longer relevant
            //if (!ViewAlarmsOnly)
            //{

                if (DrawToggle(ref _ShowAddPane, new GUIContent(KACResources.btnAdd, "Add New Alarm..."), KACResources.styleSmallButton) && _ShowAddPane)
                {
                    //reset the add stuff
                    NewAddAlarm();

                    ResetPanes();
                    _ShowAddPane = true;
                }
            //}
            //get this button right up against the add one
            GUILayout.Space(-5);
            if (DrawToggle(ref _ShowQuickAdd, new GUIContent("+", "Quick Add..."), KACResources.styleQAButton) && _ShowQuickAdd)
            {
                ResetPanes();
                _ShowQuickAdd = true;
                SetupQuickList();
            }

            GUILayout.EndHorizontal();

            //Text Area for content portion
            GUILayout.BeginVertical(KACResources.styleAlarmListArea);
            if (WindowMinimizedByActiveScene)
            {
                WindowLayout_Minimized();
            }
            else
            {
                WindowLayout_AlarmList();
            }
            
            GUILayout.EndVertical();

            //Current Game time at the botttom of the control 
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Current Time:", "Toggle Display of time from an alternate galaxy on a planet called \"Earth\""), KACResources.styleAddSectionHeading))
            {
                settings.ShowEarthTime = !settings.ShowEarthTime;
            }

            //Calendar toggle
            if (settings.ShowCalendarToggle)
            {
                if (GUILayout.Button(new GUIContent(KACResources.btnCalendar, "Toggle Calendar"), KACResources.styleSmallButton))
                {
                    if (settings.SelectedCalendar == CalendarTypeEnum.Earth) {
                        settings.SelectedCalendar = CalendarTypeEnum.KSPStock;
                        KSPDateStructure.SetKSPStockCalendar();
                    } else {
                        settings.SelectedCalendar = CalendarTypeEnum.Earth;
                        KSPDateStructure.SetEarthCalendar(settings.EarthEpoch);
                    }
                    settings.Save();
                }
            }

            //Work out the right text and tooltip and display the button as a label
            DateStringFormatsEnum MainClockFormat = DateStringFormatsEnum.DateTimeFormat;
            if (settings.DateTimeFormat == DateStringFormatsEnum.TimeAsUT) MainClockFormat = DateStringFormatsEnum.TimeAsUT;
            GUIContent contCurrentTime = new GUIContent(KACWorkerGameState.CurrentTime.ToStringStandard(MainClockFormat), "Click to toggle through time formats");
            if (GUILayout.Button(contCurrentTime, KACResources.styleContent))
            {
                switch (settings.DateTimeFormat)
                {
                    case DateStringFormatsEnum.TimeAsUT: settings.DateTimeFormat = DateStringFormatsEnum.KSPFormatWithSecs; break;
                    case DateStringFormatsEnum.KSPFormatWithSecs: settings.DateTimeFormat = DateStringFormatsEnum.DateTimeFormat; break;
                    case DateStringFormatsEnum.DateTimeFormat: settings.DateTimeFormat = DateStringFormatsEnum.TimeAsUT; break;
                    default: settings.DateTimeFormat = DateStringFormatsEnum.KSPFormatWithSecs; break;
                }
                settings.Save();
            }
            GUILayout.EndHorizontal();

            if (settings.ShowEarthTime)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("Earth Time:", "Hide Display of \"Real\" Time"), KACResources.styleHeadingEarth,GUILayout.Width(80)))
                {
                    settings.ShowEarthTime = !settings.ShowEarthTime;
                }
                GUILayout.Label(DateTime.Now.ToLongTimeString(), KACResources.styleContentEarth);
                if (DrawToggle(ref _ShowEarthAlarm, new GUIContent(KACResources.btnAdd, "Add New Alarm..."), KACResources.styleSmallButton) && _ShowEarthAlarm)
                {
                    //reset the add stuff
                    NewEarthAlarm();
                    ResetPanes();
                    _ShowEarthAlarm = true;
                }
                GUILayout.EndHorizontal();
            }


            GUILayout.EndVertical();
            SetTooltipText();
            GUI.DragWindow();
        }

        String strAlarmEarthHour="";
        String strAlarmEarthMin = "";

        //Display minimal info about the next alarm
        private void WindowLayout_Minimized()
        {
            KACAlarm nextAlarm = null;

            //Find the Alarm to display
            if (alarms != null)
            {
                if (settings.WindowMinimizedType == MiminalDisplayType.NextAlarm)
                {
                    foreach (KACAlarm tmpAlarm in alarms)
                    {
                        Boolean blnSwitch = false;
                        if (tmpAlarm.AlarmTime.UT > KACWorkerGameState.CurrentTime.UT && tmpAlarm.Enabled && !tmpAlarm.Actioned)
                        {
                            if (nextAlarm == null)
                            {
                                blnSwitch = true;
                            }
                            else
                            {
                                if (tmpAlarm.AlarmTime.UT < nextAlarm.AlarmTime.UT)
                                    blnSwitch = true;
                            }

                        }
                        if (blnSwitch)
                            nextAlarm = tmpAlarm;
                    }
                }
                else
                {
                    nextAlarm = alarms.OrderBy(a=>a.AlarmTime.UT).FirstOrDefault();
                }
            }

            if (nextAlarm == null)
            {
                GUILayout.Label("No Enabled Future Alarms in list");
            }
            else
            {
                if (Event.current.type == EventType.repaint)
                    rectScrollview = new Rect(0, 0, 0, 0);
                if (DrawAlarmLine(nextAlarm))
                    alarms.Remove(nextAlarm);
            }
        }

        //Display Full alarm list 
        internal static Rect rectScrollview;
        internal Vector2 scrollPosition = Vector2.zero;
        private void WindowLayout_AlarmList()
        {
            GUIStyle styleTemp = new GUIStyle();


            scrollPosition = GUILayout.BeginScrollView(scrollPosition, styleTemp);
            if (alarms.Count == 0)
            {
                GUILayout.Label("No Alarms in the List");
            }
            else
            {
                GUILayout.Space(4);
                List<KACAlarm> AlarmsToRemove = new List<KACAlarm>();
                foreach (KACAlarm tmpAlarm in alarms.OrderBy(a => a.AlarmTime.UT).ThenBy(a => a.ID.ToString()))
                {
                    //Draw a line for each alarm, returns true is person clicked delete
                    if (DrawAlarmLine(tmpAlarm))
                    {
                        if (!settings.ConfirmAlarmDeletes)
                            AlarmsToRemove.Add(tmpAlarm);
                        else
                        {
                            ResetPanes();
                            winConfirmAlarmDelete.AlarmToConfirm = tmpAlarm;
                            winConfirmAlarmDelete.Visible = true;
                        }
                    }
                }

                if (AlarmsToRemove.Count > 0)
                {
                    foreach (KACAlarm tmpAlarm in AlarmsToRemove)
                    {
                        alarms.Remove(tmpAlarm);
                        //settings.SaveAlarms();
                    }
                    //is the game paused, yet we deleted any active pause alarms??
                    if (alarms.FirstOrDefault(a => (a.AlarmWindowID != 0 && a.PauseGame == true)) == null)
                    {
                        if (FlightDriver.Pause)
                            FlightDriver.SetPause(false);
                    }
                }

            }
            GUILayout.EndScrollView();
            //Get the visible portion of the Scrollview and record it for hittesting later - needs to just be a box from the 0,0 point for the hit test
            // not sure why as the cursor test point is from the screen 0,0
            if (Event.current.type == EventType.repaint)
                rectScrollview = new Rect(0, scrollPosition.y, GUILayoutUtility.GetLastRect().width, GUILayoutUtility.GetLastRect().height);

        }

        private Boolean  DrawAlarmLine(KACAlarm tmpAlarm)
        {
            Boolean blnReturn = false;

            GUILayout.BeginHorizontal();

            switch (tmpAlarm.TypeOfAlarm)
            {
                case KACAlarm.AlarmTypeEnum.Raw:
                    GUILayout.Label(KACResources.iconRaw, KACResources.styleAlarmIcon);
                    break;
                case KACAlarm.AlarmTypeEnum.Maneuver:
                case KACAlarm.AlarmTypeEnum.ManeuverAuto:
                    GUILayout.Label(KACResources.iconMNode, KACResources.styleAlarmIcon);
                    break;
                case KACAlarm.AlarmTypeEnum.SOIChange:
                case KACAlarm.AlarmTypeEnum.SOIChangeAuto:
                    GUILayout.Label(KACResources.iconSOI, KACResources.styleAlarmIcon);
                    break;
                case KACAlarm.AlarmTypeEnum.Transfer:
                case KACAlarm.AlarmTypeEnum.TransferModelled:
                    GUILayout.Label(KACResources.iconXFer, KACResources.styleAlarmIcon);
                    break;
                case KACAlarm.AlarmTypeEnum.Apoapsis:
                    GUILayout.Label(KACResources.iconAp, KACResources.styleAlarmIcon);
                    break;
                case KACAlarm.AlarmTypeEnum.Periapsis:
                    GUILayout.Label(KACResources.iconPe, KACResources.styleAlarmIcon);
                    break;
                case KACAlarm.AlarmTypeEnum.AscendingNode:
                    GUILayout.Label(KACResources.iconAN, KACResources.styleAlarmIcon);
                    break;
                case KACAlarm.AlarmTypeEnum.DescendingNode:
                    GUILayout.Label(KACResources.iconDN, KACResources.styleAlarmIcon);
                    break;
                case KACAlarm.AlarmTypeEnum.LaunchRendevous:
                    GUILayout.Label(KACResources.iconLaunchRendezvous, KACResources.styleAlarmIcon);
                    break;
                case KACAlarm.AlarmTypeEnum.Closest:
                    GUILayout.Label(KACResources.iconClosest, KACResources.styleAlarmIcon);
                    break;
                case KACAlarm.AlarmTypeEnum.Distance:
                    //TODO - SOMETHING HERE MAYBE
                    GUILayout.Label(KACResources.iconClosest, KACResources.styleAlarmIcon);
                    break;
                case KACAlarm.AlarmTypeEnum.Crew:
                    GUILayout.Label(KACResources.iconCrew, KACResources.styleAlarmIcon);
                    break;
                case KACAlarm.AlarmTypeEnum.EarthTime:
                    GUILayout.Label(KACResources.iconEarth, KACResources.styleAlarmIcon);
                    break;
                case KACAlarm.AlarmTypeEnum.Contract:
                case KACAlarm.AlarmTypeEnum.ContractAuto:
                    GUILayout.Label(KACResources.iconContract, KACResources.styleAlarmIcon);
                    break;
                default:
                    GUILayout.Label(KACResources.iconNone, KACResources.styleAlarmIcon);
                    break;
            }

            //Set the Content up
            //Int32 intMaxWidth = intTestheight;
            //String strTimeToAlarm = String.Format(" ({0})",KerbalTime.PrintInterval(tmpAlarm.Remaining, Settings.TimeFormat));
            //float fTimeToAlarmWidth;
            //KACResources.styleAlarmText.CalcMinMaxWidth(new GUIContent(strTimeToAlarm),out fOutMin1,out fOutMax1);
            //fTimeToAlarmWidth = fOutMax1;

            //String strTextToDisplay = tmpAlarm.Name;
            //KACResources.styleAlarmText.CalcMinMaxWidth(new GUIContent(strTextToDisplay), out fOutMin, out fOutMax);
            //while (strTextToDisplay.Length>10 &&(  fOutMax+fTimeToAlarmWidth>intMaxWidth) )
            //{
            //    strTextToDisplay = strTextToDisplay.Remove(strTextToDisplay.Length - 2);
            //    KACResources.styleAlarmText.CalcMinMaxWidth(new GUIContent(strTextToDisplay), out fOutMin, out fOutMax);
            //}

            ////String strLabelText = strTextToDisplay + strTimeToAlarm;
            //String strTimeText = String.Format("({0})", KerbalTime.PrintInterval(tmpAlarm.Remaining, Settings.TimeFormat));
            //String strLabelText = tmpAlarm.Name;

            //GUIStyle styleLabel = new GUIStyle(KACResources.styleAlarmText);
            //if ((!tmpAlarm.Enabled || tmpAlarm.Actioned))
            //    styleLabel.normal.textColor=Color.gray;

            //GUIStyle styleTime = new GUIStyle(styleLabel);
            //styleTime.stretchWidth = true;

            //GUIContent contAlarmLabel = new GUIContent(strLabelText, tmpAlarm.Notes);
            //GUIContent contAlarmTime = new GUIContent(strTimeText, tmpAlarm.Notes);

            ////calc correct width for first part
            //KACResources.styleAlarmText.CalcMinMaxWidth(contAlarmTime, out fOutMin, out fOutMax);
            //styleLabel.CalcMinMaxWidth(contAlarmLabel, out fOutMin1, out fOutMax1);

            //Int32 intMaxWidth = intTestheight;
            //if (fOutMax1 + fOutMax > intMaxWidth)
            //    fOutMax1 = intMaxWidth - fOutMax;

            //if ((alarmEdit == tmpAlarm) && _ShowEditPane)
            //{
            //    intMaxWidth -= 20;
            //}

            //float width1 = fOutMin1;

            String strLabelText = "";
            strLabelText = String.Format("{0} ({1})", tmpAlarm.Name,tmpAlarm.Remaining.ToStringStandard(settings.TimeSpanFormat,3));

            GUIStyle styleLabel = new GUIStyle( KACResources.styleAlarmText);
            if ((!tmpAlarm.Enabled || tmpAlarm.Actioned))
                styleLabel.normal.textColor=Color.gray;
            GUIContent contAlarmLabel = new GUIContent(strLabelText, tmpAlarm.Notes);

            //Calc the line height
            Single sOutMin1, sOutMax1;
            styleLabel.CalcMinMaxWidth(contAlarmLabel, out sOutMin1, out sOutMax1);
            tmpAlarm.AlarmLineWidth = Convert.ToInt32(sOutMax1);
            Int32 intMaxwidth = 256;// 220;// 228;
            if (_ShowEditPane && (alarmEdit == tmpAlarm)) intMaxwidth = 235;// 198;// 216;
            tmpAlarm.AlarmLineHeight = Convert.ToInt32(styleLabel.CalcHeight(contAlarmLabel, intMaxwidth)); //218

            //Draw a button that looks like a label.
            if (GUILayout.Button(contAlarmLabel, styleLabel, GUILayout.MaxWidth(256)))
            {
                if (!_ShowSettings)
                {
                    if (alarmEdit == tmpAlarm)
                    {
                        //If there was an alarm open, then save em again
                        if (_ShowEditPane) settings.Save();
                        _ShowEditPane = !_ShowEditPane;
                    }
                    else
                    {
                        //If there was an alarm open, then save em again
                        if (_ShowEditPane) settings.Save();
                        alarmEdit = tmpAlarm;
                        _ShowEditPane = true;
                        _ShowSettings = false;
                        _ShowAddPane = false;
                        winConfirmAlarmDelete.Visible = false;
                    }
                }
            }

            if ((alarmEdit == tmpAlarm) && _ShowEditPane)
            {
                GUILayout.Label(new GUIContent(KACResources.iconEdit, "Editing..."), KACResources.styleLabelWarp);
            }
            if (tmpAlarm.PauseGame)
            {
                GUILayout.Label(new GUIContent(KACResources.GetPauseListIcon(tmpAlarm.WarpInfluence),"Pause"), KACResources.styleLabelWarp);
            }
            else if (tmpAlarm.HaltWarp)
            {
                if (tmpAlarm.AlarmAction== KACAlarm.AlarmActionEnum.KillWarp)
                    GUILayout.Label(new GUIContent(KACResources.GetWarpListIcon(tmpAlarm.WarpInfluence), "Kill Warp and Message"), KACResources.styleLabelWarp);
                else
                    GUILayout.Label(new GUIContent(KACResources.GetWarpListIcon(tmpAlarm.WarpInfluence), "Kill Warp Only"), KACResources.styleLabelWarp);
            }
            else
            {
                GUILayout.Label(new GUIContent(KACResources.iconNone), KACResources.styleLabelWarp);
            }

            if (GUILayout.Button(new GUIContent(KACResources.btnRedCross,"Delete Alarm"), GUI.skin.button, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                blnReturn = true;

            GUILayout.EndHorizontal();

            return blnReturn;
        }

        internal void ResetPanes()
        {
            _ShowAddPane = false;
            _ShowEarthAlarm = false;
            _ShowEditPane = false;
            _ShowSettings = false;
            _ShowQuickAdd = false;
            winConfirmAlarmDelete.Visible = false;
            winAlarmImport.Visible = false;
        }
        
        #endregion

        private void WindowLayout_CommonFields(ref String strName, ref String strMessage, ref KACAlarm.AlarmActionEnum Action, ref Double Margin, KACAlarm.AlarmTypeEnum TypeOfAlarm, Int32 WindowHeight)
        {
            KACTimeStringArray tmpTime = new KACTimeStringArray(Margin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
            WindowLayout_CommonFields(ref strName, ref strMessage, ref Action, ref tmpTime, TypeOfAlarm, WindowHeight);
            Margin = tmpTime.UT;
        }

        /// <summary>
        /// Layout of Common Parts of every alarm
        /// </summary>
        private void WindowLayout_CommonFields(ref String strName, ref String strMessage, ref KACAlarm.AlarmActionEnum Action, ref KACTimeStringArray Margin, KACAlarm.AlarmTypeEnum TypeOfAlarm, Int32 WindowHeight)
        {
            //Two Columns
            GUILayout.Label("Common Alarm Properties", KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas, GUILayout.Height(WindowHeight));

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(90));
            GUILayout.Label("Alarm Name:", KACResources.styleAddHeading);
            GUILayout.Label("Message:", KACResources.styleAddHeading);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(260), GUILayout.MaxWidth(260));
            strName = GUILayout.TextField(strName, KACResources.styleAddField).Replace("|", "");

            GUIStyle styleAddWrap = new GUIStyle(KACResources.styleAddField) { wordWrap = true };
            strMessage = GUILayout.TextArea(strMessage, styleAddWrap).Replace("|", "");
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            //Full width one under the two columns for the kill time warp
            DrawAlarmActionChoice3(ref Action, "On Alarm:", 100,40); //62

            if (TypeOfAlarm != KACAlarm.AlarmTypeEnum.Raw && TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime && TypeOfAlarm != KACAlarm.AlarmTypeEnum.Crew)
            {
                DrawTimeEntry(ref Margin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, "Alarm Margin:", 90);
            }

            GUILayout.EndVertical();

        }

        /// <summary>
        /// Layout of Common Parts of every alarm
        /// </summary>
        private void WindowLayout_CommonFields2(ref String strName, ref Boolean blnAttachVessel, ref KACAlarm.AlarmActionEnum Action, ref KACTimeStringArray Margin, KACAlarm.AlarmTypeEnum TypeOfAlarm, Int32 WindowHeight)
        {
            //Two Columns
            String strTitle = "";
            switch (TypeOfAlarm)
            {
                case KACAlarm.AlarmTypeEnum.Raw: strTitle = "Raw Time"; break;
                case KACAlarm.AlarmTypeEnum.Maneuver: strTitle = "Maneuver Node"; break;
                case KACAlarm.AlarmTypeEnum.SOIChange: strTitle = "SOI Change"; break;
                case KACAlarm.AlarmTypeEnum.Transfer: strTitle = "Transfer Window"; break;
                case KACAlarm.AlarmTypeEnum.TransferModelled: strTitle = "Transfer Window"; break;
                case KACAlarm.AlarmTypeEnum.Apoapsis: strTitle = "Apoapsis"; break;
                case KACAlarm.AlarmTypeEnum.Periapsis: strTitle = "Periapsis"; break;
                case KACAlarm.AlarmTypeEnum.AscendingNode: strTitle = "Ascending Node"; break;
                case KACAlarm.AlarmTypeEnum.DescendingNode: strTitle = "Descending Node"; break;
                case KACAlarm.AlarmTypeEnum.LaunchRendevous: strTitle = "Launch Ascent"; break;
                case KACAlarm.AlarmTypeEnum.Closest: strTitle = "Closest Approach"; break;
                case KACAlarm.AlarmTypeEnum.Distance: strTitle = "Target Distance"; break;
                case KACAlarm.AlarmTypeEnum.Crew: strTitle = "Crew"; break;
                case KACAlarm.AlarmTypeEnum.EarthTime: strTitle = "Earth Time"; break;
                case KACAlarm.AlarmTypeEnum.Contract: strTitle = "Contract"; break;
                default: strTitle = "Raw Time"; break;
            }
            strTitle += " Alarm - Common Properties";
            GUILayout.Label(strTitle, KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas, GUILayout.Height(WindowHeight));

            if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Selected Vessel:", KACResources.styleAddHeading);
                String strVesselName = "No Selected Vessel";
                if (KACWorkerGameState.CurrentVessel != null) strVesselName = KACWorkerGameState.CurrentVessel.vesselName;
                GUILayout.Label(strVesselName, KACResources.styleLabelWarning);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Alarm:", KACResources.styleAddHeading, GUILayout.Width(60));
            strName = GUILayout.TextField(strName, KACResources.styleAddField, GUILayout.MaxWidth(200)).Replace("|", "");

            GUIContent guiBtnMessages = new GUIContent(KACResources.btnChevRight, "Show Extra Details");
            if (_ShowAddMessages) guiBtnMessages = new GUIContent(KACResources.btnChevLeft, "Hide Details");
            if (GUILayout.Button(guiBtnMessages, KACResources.styleSmallButton))
                _ShowAddMessages = !_ShowAddMessages;
            GUILayout.EndHorizontal();


            if (ScenesForAttachOption.Contains(KACWorkerGameState.CurrentGUIScene) && TypesForAttachOption.Contains(TypeOfAlarm)
                && KACWorkerGameState.CurrentVessel != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                DrawCheckbox(ref blnAttachVessel, "Attach to Active Vessel");
                GUILayout.EndHorizontal();
            }

            //Full width one under the two columns for the kill time warp
            DrawAlarmActionChoice3(ref Action, "Action:", 70 ,38); //37

            if (TypeOfAlarm != KACAlarm.AlarmTypeEnum.Raw && TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime && TypeOfAlarm != KACAlarm.AlarmTypeEnum.Crew)
            {
                DrawTimeEntry(ref Margin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, "Margin:", 60);
            }
            GUILayout.EndVertical();
        }


        #region "Control Drawing"
        /// <summary>
        /// Draws a Toggle Button and sets the boolean variable to the state of the button
        /// </summary>
        /// <param name="blnVar">Boolean variable to set and store result</param>
        /// <param name="ButtonText"></param>
        /// <param name="style"></param>
        /// <param name="options"></param>
        /// <returns>True when the button state has changed</returns>
        internal Boolean DrawToggle(ref Boolean blnVar, String ButtonText, GUIStyle style, params GUILayoutOption[] options)
        {
            Boolean blnReturn = GUILayout.Toggle(blnVar, ButtonText, style, options);

            return ToggleResult(ref blnVar, ref  blnReturn);
        }

        internal Boolean DrawToggle(ref Boolean blnVar, Texture image, GUIStyle style, params GUILayoutOption[] options)
        {
            Boolean blnReturn = GUILayout.Toggle(blnVar, image, style, options);

            return ToggleResult(ref blnVar, ref blnReturn);
        }

        internal Boolean DrawToggle(ref Boolean blnVar, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            Boolean blnReturn = GUILayout.Toggle(blnVar, content, style, options);

            return ToggleResult(ref blnVar, ref blnReturn);
        }

        private Boolean ToggleResult(ref Boolean Old, ref Boolean New)
        {
            if (Old != New)
            {
                Old = New;
                LogFormatted("Toggle Changed:" + New.ToString());
                return true;
            }
            return false;
        }



        internal static Boolean DrawTextBox(ref String strVar, GUIStyle style, params GUILayoutOption[] options)
        {
            String strReturn = GUILayout.TextField(strVar, style, options);
            if (strReturn != strVar)
            {
                strVar = strReturn;
                LogFormatted("String Changed:" + strVar.ToString());
                return true;
            }
            return false;
        }


        internal static Boolean DrawTextField(ref String Value, String RegexValidator, Boolean RegexFailOnMatch, String LabelText = "", Int32 FieldWidth = 0, Int32 LabelWidth = 0, Boolean Locked = false)
        {
            GUIStyle styleTextBox = KACResources.styleAddField;
            if (Locked)
                styleTextBox = KACResources.styleAddFieldLocked;
            else if ((RegexFailOnMatch && System.Text.RegularExpressions.Regex.IsMatch(Value, RegexValidator, System.Text.RegularExpressions.RegexOptions.IgnoreCase)) ||
                (!RegexFailOnMatch && !System.Text.RegularExpressions.Regex.IsMatch(Value, RegexValidator, System.Text.RegularExpressions.RegexOptions.IgnoreCase)))
                styleTextBox = KACResources.styleAddFieldError;


            if (LabelText != "")
            {
                if (LabelWidth == 0)
                    GUILayout.Label(LabelText, KACResources.styleLabel);
                else
                    GUILayout.Label(LabelText, KACResources.styleLabel, GUILayout.Width(LabelWidth));
            }


            String textValue = Value;
            Boolean blnReturn = false;
            if (FieldWidth == 0)
                blnReturn = DrawTextBox(ref textValue, styleTextBox);
            else
                blnReturn = DrawTextBox(ref textValue, styleTextBox, GUILayout.Width(FieldWidth));

            if (!Locked) Value = textValue;
            return blnReturn;
        }

        internal static Boolean DrawYearDay(ref KSPDateTime dateToDraw)
        {
            String strYear = dateToDraw.Year.ToString();
            String strMonth = dateToDraw.Month.ToString();
            String strDay = dateToDraw.Day.ToString();

            //If the value changed
            Boolean blnReturn = false;

            if (KSPDateStructure.CalendarType == CalendarTypeEnum.Earth)
            {
                blnReturn = DrawYearMonthDay(ref strYear, ref strMonth, ref strDay);
                if (blnReturn)
                {
                    dateToDraw = KSPDateTime.FromEarthValues(strYear, strMonth, strDay);
                }
            }
            else
            {
                blnReturn = DrawYearDay(ref strYear, ref strDay);
                if (blnReturn)
                {
                    dateToDraw = new KSPDateTime(strYear, strDay);
                }
            }
            return blnReturn;
        }

        internal static Boolean DrawYearDay(ref String strYear, ref String strDay)
        {
            Boolean blnReturn = false;
            GUILayout.BeginHorizontal();
            blnReturn = blnReturn || DrawTextField(ref strYear, "[^\\d\\.]+", true, "Year:", 50, 40);
            blnReturn = blnReturn || DrawTextField(ref strDay, "[^\\d\\.]+", true, "Day:", 50, 40);
            GUILayout.EndHorizontal();
            return blnReturn;
        }

        internal static Boolean DrawYearMonthDay(ref String strYear, ref String strMonth, ref String strDay)
        {
            Boolean blnReturn = false;
            GUILayout.BeginHorizontal();
            blnReturn = blnReturn || DrawTextField(ref strYear, "[^\\d\\.]+", true, "Y:", 40, 20);
            blnReturn = blnReturn || DrawTextField(ref strMonth, "[^\\d\\.]+", true, "M:", 30, 20);
            blnReturn = blnReturn || DrawTextField(ref strDay, "[^\\d\\.]+", true, "D:", 30, 20);
            GUILayout.EndHorizontal();
            return blnReturn;
        }

        /// <summary>
        /// Draws a toggle button like a checkbox
        /// </summary>
        /// <param name="blnVar"></param>
        /// <returns>True when check state has changed</returns>
        internal Boolean DrawCheckbox(ref Boolean blnVar, String strText, params GUILayoutOption[] options)
        {
            return DrawCheckbox(ref blnVar, new GUIContent(strText),15, options);
        }
        internal Boolean DrawCheckbox(ref Boolean blnVar, GUIContent content, params GUILayoutOption[] options)
        {
            return DrawCheckbox(ref blnVar, content , 15, options);
        }
        internal Boolean DrawCheckbox(ref Boolean blnVar, String strText, Int32 CheckboxSpace, params GUILayoutOption[] options)
        {
            return DrawCheckbox(ref blnVar, new GUIContent(strText), CheckboxSpace, options);
        }
        //CHANGED
        /// <summary>
        /// Draws a toggle button like a checkbox
        /// </summary>
        /// <param name="blnVar"></param>
        /// <returns>True when check state has changed</returns>
        internal Boolean DrawCheckbox(ref Boolean blnVar, GUIContent content, Int32 CheckboxSpace, params GUILayoutOption[] options)
        {
            // return DrawToggle(ref blnVar, strText, KACResources.styleCheckbox, options);
            Boolean blnReturn = false;
            Boolean blnToggleInitial = blnVar;

            if (settings.SelectedSkin == Settings.DisplaySkin.Default)
                GUILayout.Space(-3);

            GUILayout.BeginHorizontal();
            //Draw the radio
            DrawToggle(ref blnVar, "", KACResources.styleCheckbox, options);
            //Spacing
            GUILayout.Space(CheckboxSpace);
            
            //And the button like a label
            if (GUILayout.Button(content,KACResources.styleCheckboxLabel, options))
            {
                //if its clicked then toggle the boolean
                blnVar = !blnVar;
                LogFormatted("Toggle Changed:" + blnVar);
            }

            GUILayout.EndHorizontal();
            if (settings.SelectedSkin == Settings.DisplaySkin.Default)
                GUILayout.Space(-3);

            //If output value doesnt = input value
            if (blnToggleInitial != blnVar)
            {
                //KACWorker.DebugLogFormatted("Toggle recorded:" + blnVar);
                blnReturn = true;
            }
            return blnReturn;
        }

        internal Boolean DrawRadioListVertical(ref Int32 Selected, params String[] Choices)
        {
            return DrawRadioList(false, ref Selected, Choices);
        }
        internal Boolean DrawRadioList(ref Int32 Selected, params String[] Choices)
        {
            return DrawRadioList(true, ref Selected, Choices);
        }
        internal Boolean DrawRadioList(Boolean Horizontal, ref Int32 Selected, params String[] Choices)
        {
            Int32 InitialChoice = Selected;

            if (Horizontal)
                GUILayout.BeginHorizontal();
            else
                GUILayout.BeginVertical();

            for (Int32 intChoice = 0; intChoice < Choices.Length; intChoice++)
            {
                //checkbox
                GUILayout.BeginHorizontal();
                if (GUILayout.Toggle((intChoice == Selected), "", KACResources.styleCheckbox))
                    Selected = intChoice;
                //button that looks like a label
                if (GUILayout.Button(Choices[intChoice], KACResources.styleCheckboxLabel))
                    Selected = intChoice;
                GUILayout.EndHorizontal();
            }
            if(Horizontal)
                GUILayout.EndHorizontal();
            else
                GUILayout.EndVertical();

            if (InitialChoice != Selected)
                LogFormatted(String.Format("Radio List Changed:{0} to {1}", InitialChoice, Selected));


            return !(InitialChoice == Selected);
        }

        //public Boolean DrawAlarmActionChoice(ref Int32 intChoice, String LabelText, Int32 LabelWidth)
        //{
        //    Boolean blnReturn = false;
        //    GUILayout.BeginHorizontal();
        //    GUILayout.Label(LabelText, KACResources.styleAddHeading, GUILayout.Width(LabelWidth));
        //    blnReturn = DrawRadioList(ref intChoice, "Message Only", "Kill Time Warp", "Pause Game");
        //    GUILayout.EndHorizontal();
        //    return blnReturn;
        //}
        //public Boolean DrawAlarmActionChoice2(ref KACAlarm.AlarmActionEnum Choice, String LabelText, Int32 LabelWidth)
        //{
        //    Boolean blnReturn = false;
        //    GUILayout.BeginHorizontal();
        //    GUILayout.Label(LabelText, KACResources.styleAddHeading, GUILayout.Width(LabelWidth-10));
        //    Int32 intChoice = (Int32)Choice;
        //    blnReturn = DrawRadioList(ref intChoice, "Message", "Kill Warp", "Pause");
        //    Choice = (KACAlarm.AlarmActionEnum)intChoice;
        //    GUILayout.EndHorizontal();
        //    return blnReturn;
        //}

        internal Boolean DrawAlarmActionChoice3(ref KACAlarm.AlarmActionEnum Choice, String LabelText, Int32 LabelWidth, Int32 ButtonWidth)
        {
            Boolean blnReturn = false;
            GUILayout.BeginHorizontal();
            GUILayout.Label(LabelText, KACResources.styleAddHeading, GUILayout.Width(LabelWidth - 10));
            Int32 intChoice = (Int32)Choice;
            GUIStyle styleButton = new GUIStyle(KACResources.styleButtonListAlarmActions) { fixedWidth = ButtonWidth };
            blnReturn = DrawButtonList(ref intChoice, styleButton, KACResources.lstAlarmChoices.ToArray());
            //blnReturn = DrawRadioList(ref intChoice, "Message", "Kill Warp", "Pause");
            Choice = (KACAlarm.AlarmActionEnum)intChoice;
            GUILayout.EndHorizontal();
            return blnReturn;
        }



        internal Boolean DrawTimeEntry(ref KACTimeStringArray time, KACTimeStringArray.TimeEntryPrecisionEnum Prec, params GUILayoutOption[] options)
        {
            return DrawTimeEntry(ref time, Prec, "", 0, 40,20);
        }

        internal Boolean DrawTimeEntry(ref KACTimeStringArray time, KACTimeStringArray.TimeEntryPrecisionEnum Prec, String LabelText, Int32 LabelWidth, params GUILayoutOption[] options)
        {
            return DrawTimeEntry(ref time, Prec, LabelText, LabelWidth, 40,20);
        }

        internal Boolean DrawTimeEntry(ref KACTimeStringArray time, KACTimeStringArray.TimeEntryPrecisionEnum Prec, String LabelText, Int32 LabelWidth, Int32 FieldWidth, Int32 SuffixWidth, params GUILayoutOption[] options)
        {
            Boolean blnReturn = false;

            GUILayout.BeginHorizontal();
            if (LabelText!="")
                GUILayout.Label(LabelText, KACResources.styleAddHeading, GUILayout.Width(LabelWidth));
            
            String strTemp;
            if (Prec >= KACTimeStringArray.TimeEntryPrecisionEnum.Years)
            {
                strTemp = time.Years;
                if (DrawTimeField(ref strTemp, "y", FieldWidth, SuffixWidth))
                {
                    blnReturn = true;
                    time.Years = strTemp;
                }
            }
            if (Prec >= KACTimeStringArray.TimeEntryPrecisionEnum.Days)
            {
                strTemp = time.Days;
                if (DrawTimeField(ref strTemp, "d", FieldWidth, SuffixWidth))
                {
                    blnReturn = true;
                    time.Days = strTemp;
                }
            }
            if (Prec >= KACTimeStringArray.TimeEntryPrecisionEnum.Hours)
            {
                strTemp = time.Hours;
                if (DrawTimeField(ref strTemp, "h", FieldWidth, SuffixWidth))
                {
                    blnReturn = true;
                    time.Hours = strTemp;
                }
            }
            if (Prec >= KACTimeStringArray.TimeEntryPrecisionEnum.Minutes)
            {
                strTemp = time.Minutes;
                if (DrawTimeField(ref strTemp, "m", FieldWidth, SuffixWidth))
                {
                    blnReturn = true;
                    time.Minutes = strTemp;
                }
            }
            if (Prec >= KACTimeStringArray.TimeEntryPrecisionEnum.Seconds)
            {
                strTemp = time.Seconds;
                if (DrawTimeField(ref strTemp, "s", FieldWidth, SuffixWidth))
                {
                    blnReturn = true;
                    time.Seconds = strTemp;
                }
            }
            //blnReturn = DrawTimeField(ref time.Seconds, "s", FieldWidth, SuffixWidth) && blnReturn;
            if (!time.Valid)
            {
                GUILayout.Label(new GUIContent("*","Invalid fields treated as 0"), KACResources.styleLabelError, GUILayout.Width(SuffixWidth));
            }
            GUILayout.EndHorizontal();

            return blnReturn;
        }

        internal Boolean DrawTimeField(ref String Value, String LabelText, Int32 FieldWidth, Int32 SuffixWidth)
        {
            Boolean blnReturn = false;
            Int32 intParse;
            GUIStyle styleTextBox = KACResources.styleAddField;
            GUIContent contText = new GUIContent(Value);
            Boolean BlnIsNum = Int32.TryParse(Value,out intParse);
            if (!BlnIsNum) styleTextBox = KACResources.styleAddFieldError;

            //styleTextBox.alignment = TextAnchor.MiddleRight;
            blnReturn = DrawTextBox(ref Value, styleTextBox, GUILayout.MaxWidth(FieldWidth));

            //String strReturn = GUILayout.TextField(Value, styleTextBox, GUILayout.MaxWidth(FieldWidth));
            //Attempt at fancy tint - looks weird
            //if(!BlnIsNum)
            //{
            //    Rect overlay = GUILayoutUtility.GetLastRect();
            //    overlay.x -= 1; overlay.y -= 2; overlay.width += 2; overlay.height += 5;
            //    GUI.depth--;
            //    GUI.Label(overlay,"", KACResources.styleAddFieldErrorOverlay);
            //}

            GUILayout.Label(LabelText,KACResources.styleAddHeading,GUILayout.Width(SuffixWidth));

            return blnReturn;
        }

        internal Boolean DrawButtonList(ref KACAlarm.AlarmTypeEnum selType, params GUIContent[] Choices)
        {
            //int Selection = (KACWorkerGameState.CurrentGUIScene != GameScenes.TRACKSTATION) ? KACAlarm.AlarmTypeToButton[selType] : KACAlarm.AlarmTypeToButtonTS[selType];
            int Selection = KACAlarm.AlarmTypeToButton[selType];
            if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION) Selection = KACAlarm.AlarmTypeToButtonTS[selType];
            else if (KACWorkerGameState.CurrentGUIScene == GameScenes.SPACECENTER) Selection = KACAlarm.AlarmTypeToButtonSC[selType];

            Boolean blnReturn = DrawButtonList(ref Selection, Choices);
            if (blnReturn)
            {
                if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION)
                    selType = KACAlarm.AlarmTypeFromButtonTS[Selection];
                else if (KACWorkerGameState.CurrentGUIScene == GameScenes.SPACECENTER)
                    selType = KACAlarm.AlarmTypeFromButtonSC[Selection];
                else
                    selType = KACAlarm.AlarmTypeFromButton[Selection];
            }
            return blnReturn;
        }

        internal Boolean DrawButtonList(ref Int32 Selected, params GUIContent[] Choices)
        {
            return DrawButtonList(ref Selected, KACResources.styleButtonList, Choices);
        }
        internal Boolean DrawButtonList(ref Int32 Selected, GUIStyle ButtonStyle, params GUIContent[] Choices)
        {
            Int32 InitialChoice = Selected;

            GUILayout.BeginHorizontal();

            for (Int32 intChoice = 0; intChoice < Choices.Length; intChoice++)
            {
                //button
                Boolean blnResult=(Selected==intChoice);
                if (DrawToggle(ref blnResult,Choices[intChoice],ButtonStyle))
                {
                    if (blnResult)
                        Selected=intChoice;
                }
            }
            GUILayout.EndHorizontal();

            if (InitialChoice != Selected)
                LogFormatted(String.Format("Button List Changed:{0} to {1}", InitialChoice, Selected));


            return !(InitialChoice == Selected);
        }


        #endregion


    }


}
