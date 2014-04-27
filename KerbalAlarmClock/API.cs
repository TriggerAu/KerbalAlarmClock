using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalAlarmClock
{
    public partial class KerbalAlarmClock
    {

        //For API Access

        /// <summary>
        /// This is the Alternate Resource Panel object we hook from the wrapper
        /// </summary>
        public static KerbalAlarmClock APIInstance;
        public static Boolean APIReady = false;

        //Init the API Hooks
        private void APIAwake()
        {
            //set up the hookable object
            APIInstance = this;

            //set up the events we need

            //flag it ready
            LogFormatted("API Ready");
            APIReady = true;
        }

        private void APIDestroy()
        {
            //tear it down
            APIInstance = null;

            LogFormatted("API Cleaned up");
            APIReady = false;
        }



    }
}
