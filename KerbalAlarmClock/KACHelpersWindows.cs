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
        /// <summary>
        /// Draw the icon on the screen
        /// </summary>
        public void DrawIcons()
        {
            if (!Settings.UseBlizzyToolbarIfAvailable || btnToolbar == null)
            {
                Texture2D iconToShow;
                //Replace this with workerstate object that can test for pause and catch errors - is it doing this in flight mode??
                if (!(KACGameState.CurrentGUIScene == GameScenes.FLIGHT) || (!KACGameState.PauseMenuOpen && !KACGameState.FlightResultsDialogOpen))
                {
                    if (FlightDriver.Pause)
                    {
                        iconToShow = Resources.GetPauseIcon();
                    }
                    else if (KACGameState.CurrentlyUnderWarpInfluence)
                    {
                        iconToShow = Resources.GetWarpIcon();
                    }
                    else
                    {
                        if (Settings.Alarms.ActiveEnabledFutureAlarms(HighLogic.CurrentGame.Title))
                        {
                            if (WindowVisibleByActiveScene)
                                iconToShow = Resources.iconAlarmShow;
                            else
                                iconToShow = Resources.iconAlarm;
                        }
                        else
                        {
                            if (WindowVisibleByActiveScene)
                                iconToShow = Resources.iconNormShow;
                            else
                                iconToShow = Resources.iconNorm;
                        }
                    }

                    //draw the icon button
                    if (IconShowByActiveScene)
                    {
                        if (GUI.Button(IconPosByActiveScene, new GUIContent(iconToShow, "Click to Toggle"), Styles.styleIconStyle))
                        {
                            //switch (KACWorkerGameState.CurrentGUIScene)
                            //{
                            //    case GameScenes.SPACECENTER: Settings.WindowVisible_SpaceCenter = !Settings.WindowVisible_SpaceCenter; break;
                            //    case GameScenes.TRACKSTATION: Settings.WindowVisible_TrackingStation = !Settings.WindowVisible_TrackingStation;break;
                            //    default: Settings.WindowVisible = !Settings.WindowVisible;break;
                            //}
                            WindowVisibleByActiveScene = !WindowVisibleByActiveScene;
                            Settings.Save();
                        }
                        //GUI.Label(new Rect(152,32,200,20), GUI.tooltip,KACStyles.styleTooltipStyle);
                    }
                }
            }
            else
            {
                //Do for Blizzies Toolbar
                if (btnToolbar != null)
                {
                    String TexturePath = "";
                    if (!(KACGameState.CurrentGUIScene == GameScenes.FLIGHT) || (!KACGameState.PauseMenuOpen && !KACGameState.FlightResultsDialogOpen))
                    {
                        if (FlightDriver.Pause)
                        {
                            TexturePath = Resources.GetPauseIconTexturePath();
                        }
                        else if (KACGameState.CurrentlyUnderWarpInfluence)
                        {
                            TexturePath = Resources.GetWarpIconTexturePath();
                        }
                        else
                        {
                            if (Settings.Alarms.ActiveEnabledFutureAlarms(HighLogic.CurrentGame.Title))
                            {
                                if (WindowVisibleByActiveScene)
                                    TexturePath = "TriggerTech/ToolbarIcons/KACIcon-AlarmShow";
                                else
                                    TexturePath = "TriggerTech/ToolbarIcons/KACIcon-Alarm";
                            }
                            else
                            {
                                if (WindowVisibleByActiveScene)
                                    TexturePath = "TriggerTech/ToolbarIcons/KACIcon-NormShow";
                                else
                                    TexturePath = "TriggerTech/ToolbarIcons/KACIcon-Norm";
                            }
                        }

                        btnToolbar.TexturePath = TexturePath;
                    }
                }
            }
        }

        //Window Size Constants
        Int32 intMainWindowWidth = 300;
        Int32 intMainWindowMinHeight = 114;
        Int32 intMainWindowBaseHeight = 114;

        Int32 intMainWindowAlarmListItemHeight = 26;
        Int32 intMainWindowAlarmListScrollPad = 3;
        Int32 intMainWindowEarthTimeHeight = 26;

        Int32 intPaneWindowWidth = 380;
        Int32 intAddPaneWindowWidth = 320;
        Int32 AddWindowHeight;

        Int32 EarthWindowHeight = 216;

        public void DrawWindows()
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
                MainWindowPos.height = intMainWindowMinHeight + 2;
            }
            else
            {
                MainWindowPos.height = intMainWindowBaseHeight;
                //Work out the number of alarms and therefore the height of the window
                if (Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title).Count > 1)
                {
                    if (Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title).Count < 2)
                        MainWindowPos.height = intMainWindowBaseHeight;
                    else if (Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title).Count < Settings.AlarmListMaxAlarmsInt)
                        MainWindowPos.height = intMainWindowBaseHeight + ((Settings.Alarms.BySaveName(HighLogic.CurrentGame.Title).Count - 1) * intMainWindowAlarmListItemHeight);
                    else
                        //this is scrolling
                        MainWindowPos.height = (intMainWindowBaseHeight - 3) + ((Settings.AlarmListMaxAlarmsInt32 - 1) * intMainWindowAlarmListItemHeight) + intMainWindowAlarmListScrollPad;
                }
                else MainWindowPos.height = intMainWindowBaseHeight + 2;
            }
            if (Settings.ShowEarthTime)
            {
                MainWindowPos.height += intMainWindowEarthTimeHeight;
            }

            //Now show the window
            WindowPosByActiveScene = GUILayout.Window(_WindowMainID, MainWindowPos, FillWindow, "Kerbal Alarm Clock - " + Settings.Version, Styles.styleWindow);

            //Do we have anything to show in the right pane
            if (_ShowSettings)
            {
                _WindowSettingsRect = GUILayout.Window(_WindowSettingsID, new Rect(WindowPosByActiveScene.x + WindowPosByActiveScene.width, WindowPosByActiveScene.y, intPaneWindowWidth, intSettingsHeight), FillSettingsWindow, "Settings and Globals", Styles.styleWindow);
            }
            else if (_ShowAddPane)
            {
                switch (AddType)
                {
                    case KACAlarm.AlarmType.Raw:
                        AddWindowHeight = 250; break;
                    case KACAlarm.AlarmType.Maneuver:
                    case KACAlarm.AlarmType.SOIChange:
                        AddWindowHeight = 182; break;
                    case KACAlarm.AlarmType.Apoapsis:
                    case KACAlarm.AlarmType.Periapsis:
                        AddWindowHeight = 208; break;
                    case KACAlarm.AlarmType.AscendingNode:
                    case KACAlarm.AlarmType.DescendingNode:
                    case KACAlarm.AlarmType.LaunchRendevous:
                        AddWindowHeight = 234; break;
                    case KACAlarm.AlarmType.Transfer:
                    case KACAlarm.AlarmType.TransferModelled:
                        AddWindowHeight = intAddXferHeight; break;
                    case KACAlarm.AlarmType.Closest:
                        AddWindowHeight = 230; break;
                    case KACAlarm.AlarmType.Distance:
                        AddWindowHeight = intAddDistanceHeight; break;
                    case KACAlarm.AlarmType.Crew:
                        AddWindowHeight = intAddCrewHeight; break;
                    default: AddWindowHeight = 250; break;
                }
                AddWindowHeight += intHeight_AddWindowCommon;
                _WindowAddRect = GUILayout.Window(_WindowAddID, new Rect(WindowPosByActiveScene.x + WindowPosByActiveScene.width, WindowPosByActiveScene.y, intAddPaneWindowWidth, AddWindowHeight), FillAddWindow, "Add New Alarm", Styles.styleWindow);                //switch (AddInterfaceType)

                if (_ShowAddMessages)
                {
                    _WindowAddMessagesRect = GUILayout.Window(_WindowAddMessagesID, new Rect(_WindowAddRect.x + _WindowAddRect.width, _WindowAddRect.y, 200, AddWindowHeight), FillAddMessagesWindow, "");
                }
            }
            else if (_ShowEarthAlarm)
            {
                float _WindowEarthTop = WindowPosByActiveScene.y + WindowPosByActiveScene.height - EarthWindowHeight;
                if (EarthWindowHeight > MainWindowPos.height) _WindowEarthTop = WindowPosByActiveScene.y;
                _WindowEarthAlarmRect = GUILayout.Window(_WindowEarthAlarmID, new Rect(WindowPosByActiveScene.x + WindowPosByActiveScene.width, _WindowEarthTop, intAddPaneWindowWidth, EarthWindowHeight), FillEarthAlarmWindow, "Add Earth Time Alarm", Styles.styleWindow);                //switch (AddInterfaceType)
                if (_ShowAddMessages)
                {
                    _WindowAddMessagesRect = GUILayout.Window(_WindowAddMessagesID, new Rect(_WindowEarthAlarmRect.x + _WindowEarthAlarmRect.width, _WindowEarthAlarmRect.y, 200, EarthWindowHeight), FillAddMessagesWindow, "");
                }
            }
            else if (_ShowEditPane)
            {
                _WindowEditRect = GUILayout.Window(_WindowEditID, new Rect(WindowPosByActiveScene.x + WindowPosByActiveScene.width, WindowPosByActiveScene.y, intPaneWindowWidth, intAlarmEditHeight), FillEditWindow, "Editing Alarm", Styles.styleWindow);
            }

            if (_ShowBackupFailedMessage)
            {
                _WindowBackupFailedRect = GUILayout.Window(_WindowBackupFailedID, _WindowBackupFailedRect, FillBackupFailedWindow, "Save Backup Failed", Styles.styleWindow);
                if (DateTime.Now.Subtract(_ShowBackupFailedMessageAt).Seconds > _ShowBackupFailedMessageForSecs)
                    ResetBackupFailedWindow();
            }

            DrawToolTip();
        }

        #region "Stuff to get icon and windows by scene"
        public Boolean IconShowByActiveScene
        {
            get
            {
                switch (KACGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: return Settings.IconShow_SpaceCenter;
                    case GameScenes.TRACKSTATION: return Settings.IconShow_TrackingStation;
                    default: return true;
                }
            }
            set
            {
                switch (KACGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: Settings.IconShow_SpaceCenter = value; break;
                    case GameScenes.TRACKSTATION: Settings.IconShow_TrackingStation = value; break;
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
                switch (KACGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: return Settings.IconPos_SpaceCenter;
                    case GameScenes.TRACKSTATION: return Settings.IconPos_TrackingStation;
                    default: return Settings.IconPos;
                }
            }
            set
            {
                switch (KACGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: Settings.IconPos_SpaceCenter = value; break;
                    case GameScenes.TRACKSTATION: Settings.IconPos_TrackingStation = value; break;
                    default: Settings.IconPos = value; break;
                }
            }
        }


        public Boolean WindowVisibleByActiveScene
        {
            get
            {
                switch (KACGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: return Settings.WindowVisible_SpaceCenter;
                    case GameScenes.TRACKSTATION: return Settings.WindowVisible_TrackingStation;
                    default: return Settings.WindowVisible;
                }
            }
            set
            {
                switch (KACGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: Settings.WindowVisible_SpaceCenter = value; break;
                    case GameScenes.TRACKSTATION: Settings.WindowVisible_TrackingStation = value; break;
                    default: Settings.WindowVisible = value; break;
                }
            }
        }

        public Boolean WindowMinimizedByActiveScene
        {
            get
            {
                switch (KACGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: return Settings.WindowMinimized_SpaceCenter;
                    case GameScenes.TRACKSTATION: return Settings.WindowMinimized_TrackingStation;
                    default: return Settings.WindowMinimized;
                }
            }
            set
            {
                switch (KACGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: Settings.WindowMinimized_SpaceCenter = value; break;
                    case GameScenes.TRACKSTATION: Settings.WindowMinimized_TrackingStation = value; break;
                    default: Settings.WindowMinimized = value; break;
                }
            }
        }

        public Rect WindowPosByActiveScene
        {
            get
            {
                switch (KACGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: return Settings.WindowPos_SpaceCenter;
                    case GameScenes.TRACKSTATION: return Settings.WindowPos_TrackingStation;
                    default: return Settings.WindowPos;
                }
            }
            set
            {
                switch (KACGameState.CurrentGUIScene)
                {
                    case GameScenes.SPACECENTER: Settings.WindowPos_SpaceCenter = value; break;
                    case GameScenes.TRACKSTATION: Settings.WindowPos_TrackingStation = value; break;
                    default: Settings.WindowPos = value; break;
                }
            }
        }
        #endregion

    }
}
