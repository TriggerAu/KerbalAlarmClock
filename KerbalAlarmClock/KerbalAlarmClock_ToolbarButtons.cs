using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalAlarmClock
{
    public partial class KACWorker
    {
        internal ToolbarButtonWrapper btnToolbarKAC = null;
    }

    public partial class KerbalAlarmClock
    {
        internal Boolean BlizzyToolbarIsAvailable = false;

        /// <summary>
        /// Check to see if the Toolbar is available
        /// </summary>
        /// <returns>True if the Toolbar.ToolbarManager class is loaded in an existing assembly</returns>
        internal Boolean HookToolbar()
        {
            //Is the Dll in memory
            Boolean blnReturn = ToolbarDLL.Loaded;
            KACWorker.DebugLogFormatted("Blizzy's Toolbar Loaded:{0}", blnReturn);
            return blnReturn;
        }


        /// <summary>
        /// initialises a Toolbar Button for this mod
        /// </summary>
        /// <returns>The ToolbarButtonWrapper that was created</returns>
        internal ToolbarButtonWrapper InitToolbarButton()
        {
            ToolbarButtonWrapper btnReturn=null;
            try
            {
                KACWorker.DebugLogFormatted("Initialising the Toolbar Icon");
                btnReturn = new ToolbarButtonWrapper("KerbalAlarmClock", "btnToolbarIcon");
                btnReturn.TexturePath = "TriggerTech/ToolbarIcons/KACIcon-Norm";
                btnReturn.ToolTip = "Kerbal Alarm Clock";
                btnReturn.AddButtonClickHandler((e) =>
                {
                    WorkerObjectInstance.WindowVisibleByActiveScene = !WorkerObjectInstance.WindowVisibleByActiveScene;
                    Settings.Save();
                });
            }
            catch (Exception ex)
            {
                DestroyToolbarButton(btnReturn);
                KACWorker.DebugLogFormatted("Error Initialising Toolbar Button: {0}", ex.Message);
            }
            return btnReturn;
        }

        /// <summary>
        /// Destroys theToolbarButtonWrapper object
        /// </summary>
        /// <param name="btnToDestroy">Object to Destroy</param>
        internal void DestroyToolbarButton(ToolbarButtonWrapper btnToDestroy)
        {
            if (btnToDestroy != null)
            {
                KACWorker.DebugLogFormatted("Destroying Toolbar Button");
                btnToDestroy.Destroy();
            }
            btnToDestroy = null;
        }

    }
}
