using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KACToolbarWrapper;

namespace KerbalAlarmClock
{
    public partial class KerbalAlarmClock
    {
        internal IButton btnToolbarKAC = null;
        internal Boolean BlizzyToolbarIsAvailable = false;

        /// <summary>
        /// Check to see if the Toolbar is available
        /// </summary>
        /// <returns>True if the Toolbar.ToolbarManager class is loaded in an existing assembly</returns>
        internal Boolean HookToolbar()
        {
            //Is the Dll in memory
            Boolean blnReturn = ToolbarManager.ToolbarAvailable;
            LogFormatted("Blizzy's Toolbar Loaded:{0}", blnReturn);
            return blnReturn;
        }


        /// <summary>
        /// initialises a Toolbar Button for this mod
        /// </summary>
        /// <returns>The ToolbarButtonWrapper that was created</returns>
        internal IButton InitToolbarButton()
        {
            IButton btnReturn = null;
            try
            {
                LogFormatted("Initialising the Toolbar Icon");
                btnReturn = ToolbarManager.Instance.add("KerbalAlarmClock", "btnToolbarIcon");
                btnReturn.TexturePath = KACUtils.PathToolbarTexturePath + "/KACIcon-Norm";
                btnReturn.ToolTip = "Kerbal Alarm Clock";
                btnReturn.OnClick += (e) =>
                {
                    WindowVisibleByActiveScene = !WindowVisibleByActiveScene;
                    settings.Save();
                };
            }
            catch (Exception ex)
            {
                DestroyToolbarButton(btnReturn);
                LogFormatted("Error Initialising Toolbar Button: {0}", ex.Message);
            }
            return btnReturn;
        }

        /// <summary>
        /// Destroys theToolbarButtonWrapper object
        /// </summary>
        /// <param name="btnToDestroy">Object to Destroy</param>
        internal void DestroyToolbarButton(IButton btnToDestroy)
        {
            if (btnToDestroy != null)
            {
                LogFormatted("Destroying Toolbar Button");
                btnToDestroy.Destroy();
            }
            btnToDestroy = null;
        }

    }
}
