using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KerbalAlarmClock
{
    public partial class KerbalAlarmClock
    {
        void OnGUIAppLauncherReady()
        {
            MonoBehaviourExtended.LogFormatted_DebugOnly("AppLauncherReady");
            if (ApplicationLauncher.Ready)
            {
                if (settings.ButtonStyleChosen == Settings.ButtonStyleEnum.Launcher )
                {
                    btnAppLauncher = InitAppLauncherButton();
                    if (WindowVisibleByActiveScene)
                    {
                        LogFormatted("Setting Button True");
                        btnAppLauncher.SetTrue();
                    }
                }
            }
            else { LogFormatted("App Launcher-Not Actually Ready"); }
        }

        void OnGameSceneLoadRequestedForAppLauncher(GameScenes SceneToLoad)
        {
            LogFormatted_DebugOnly("GameSceneLoadRequest");
            DestroyAppLauncherButton();
        }
        internal ApplicationLauncherButton btnAppLauncher = null;

        internal ApplicationLauncherButton InitAppLauncherButton()
        {
            ApplicationLauncherButton retButton = null;

            ApplicationLauncherButton[] lstButtons = KerbalAlarmClock.FindObjectsOfType<ApplicationLauncherButton>();
            LogFormatted("AppLauncher: Creating Button-BEFORE", lstButtons.Length);
            try
            {
                retButton = ApplicationLauncher.Instance.AddModApplication(
                    onAppLaunchToggleOn, onAppLaunchToggleOff,
                    onAppLaunchHoverOn, onAppLaunchHoverOff,
                    null, null,
                    ApplicationLauncher.AppScenes.ALWAYS,
                    (Texture)KACResources.toolbariconNorm);

                //AppLauncherButtonMutuallyExclusive(settings.AppLauncherMutuallyExclusive);

                //appButton = ApplicationLauncher.Instance.AddApplication(
                //    onAppLaunchToggleOn, onAppLaunchToggleOff,
                //    onAppLaunchHoverOn, onAppLaunchHoverOff,
                //    null, null,
                //    (Texture)Resources.texAppLaunchIcon);
                //appButton.VisibleInScenes = ApplicationLauncher.AppScenes.FLIGHT;

            }
            catch (Exception ex)
            {
                MonoBehaviourExtended.LogFormatted("AppLauncher: Failed to set up App Launcher Button\r\n{0}", ex.Message);
                retButton = null;
            }
            lstButtons = KerbalAlarmClock.FindObjectsOfType<ApplicationLauncherButton>();
            LogFormatted("AppLauncher: Creating Button-AFTER", lstButtons.Length);

            return retButton;
        }


        internal void DestroyAppLauncherButton()
        {
            LogFormatted("AppLauncher: Destroying Button-BEFORE NULL CHECK");
            if (btnAppLauncher != null)
            {
                ApplicationLauncherButton[] lstButtons = KerbalAlarmClock.FindObjectsOfType<ApplicationLauncherButton>();
                LogFormatted("AppLauncher: Destroying Button-Button Count:{0}", lstButtons.Length);
                ApplicationLauncher.Instance.RemoveModApplication(btnAppLauncher);
                btnAppLauncher = null;
            }
            LogFormatted("AppLauncher: Destroying Button-AFTER NULL CHECK");
        }

        void onAppLaunchToggleOn() {
            MonoBehaviourExtended.LogFormatted_DebugOnly("TOn");

            WindowVisibleByActiveScene = true;
            settings.Save();
            MonoBehaviourExtended.LogFormatted_DebugOnly("{0}",WindowVisibleByActiveScene);
        }
        void onAppLaunchToggleOff() {
            MonoBehaviourExtended.LogFormatted_DebugOnly("TOff");

            WindowVisibleByActiveScene = false;
            settings.Save();
            MonoBehaviourExtended.LogFormatted_DebugOnly("{0}", WindowVisibleByActiveScene);
        }
        void onAppLaunchHoverOn() {
            MonoBehaviourExtended.LogFormatted_DebugOnly("HovOn");
            //MouseOverAppLauncherBtn = true;
        }
        void onAppLaunchHoverOff() {
            MonoBehaviourExtended.LogFormatted_DebugOnly("HovOff");
            //MouseOverAppLauncherBtn = false; 
        }

        //Boolean MouseOverAppLauncherBtn = false;
    }
}
