using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalAlarmClock
{
    public partial class KerbalAlarmClock
    {
        internal event APIAlarmStateChangedHandler AlarmStateChanged;
        internal delegate void APIAlarmStateChangedHandler(KACAlarm alarm, AlarmStateEventsEnum newstate);

        //For API Access

        /// <summary>
        /// This is the Kerbal Alarm Clock object we hook from the wrapper
        /// </summary>
        public static KerbalAlarmClock APIInstance;
        public static Boolean APIReady = false;

        //Init the API Hooks
        private void APIAwake()
        {
            //set up the hookable object
            APIInstance = this;

            //set up the events we need
            APIInstance.AlarmStateChanged += APIInstance_AlarmStateChanged;

            //flag it ready
            LogFormatted("API Ready");
            APIReady = true;
        }

        private void APIDestroy()
        {
            //tear it down
            APIInstance = null;

            try { APIInstance.AlarmStateChanged -= APIInstance_AlarmStateChanged; } catch (Exception) { }
       
            LogFormatted("API Cleaned up");
            APIReady = false;
        }

        void APIInstance_AlarmStateChanged(KACAlarm alarm, AlarmStateEventsEnum newstate)
        {
            if (onAlarmStateChanged != null)
                onAlarmStateChanged(new AlarmStateChangedEventArgs(alarm, newstate));
        }

        //public events
        public event AlarmStateChangedHandler onAlarmStateChanged;
        public delegate void AlarmStateChangedHandler(AlarmStateChangedEventArgs e);

        public class AlarmStateChangedEventArgs
        {
            public AlarmStateChangedEventArgs(KACAlarm sender,AlarmStateEventsEnum eventType)
            {
                this.sender = sender;
                this.eventType = eventType;
            }

            public KACAlarm sender;
            public AlarmStateEventsEnum eventType;
        }

        public enum AlarmStateEventsEnum {
            Created,
            Triggered,
            Closed,
            Deleted,
        }


        //methods
       // public KACAlarm CreateAlarm() { }


    }
}
