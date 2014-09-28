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

        internal void APIInstance_AlarmStateChanged(KACAlarm alarm, AlarmStateEventsEnum newstate)
        {
            if (onAlarmStateChanged != null)
                onAlarmStateChanged(new AlarmStateChangedEventArgs(alarm, newstate));
        }

        //public events
        public event AlarmStateChangedHandler onAlarmStateChanged;
        public delegate void AlarmStateChangedHandler(AlarmStateChangedEventArgs e);

        public class AlarmStateChangedEventArgs
        {
            public AlarmStateChangedEventArgs(KACAlarm alarm,AlarmStateEventsEnum eventType)
            {
                this.alarm = alarm;
                this.eventType = eventType;
            }

            public KACAlarm alarm;
            public AlarmStateEventsEnum eventType;
        }

        public enum AlarmStateEventsEnum {
            Created,
            Triggered,
            Closed,
            Deleted,
        }


        //methods
        public void TestMethod()
        {
            LogFormatted("API Method Called");
        }

        public String CreateAlarm(KACAlarm.AlarmTypeEnum AlarmType, String Name,Double UT)
        //public KACAlarm CreateAlarm(String Name)
        {
            LogFormatted("B");
            KACAlarm tmpAlarm = new KACAlarm(UT);
            tmpAlarm.TypeOfAlarm=AlarmType;
            tmpAlarm.Name=Name;

            alarms.Add(tmpAlarm);

            return tmpAlarm.ID;
        }

        public Boolean DeleteAlarm(String AlarmID)
        {
            Boolean blnReturn = false;
            try
            {
                KACAlarm tmpAlarm = alarms.FirstOrDefault(a => a.ID == AlarmID);
                if (tmpAlarm != null)
                {
                    LogFormatted("API-DeleteAlarm-Deleting:{0}", AlarmID);
                    alarms.Remove(tmpAlarm);
                    blnReturn = true;
                }
                else
                {
                    LogFormatted("API-DeleteAlarm-ID Not Found:{0}", AlarmID);
                }
            }
            catch (Exception ex)
            {
                LogFormatted("API-DeleteAlarm-Error:{0}\r\n{1}", AlarmID,ex.Message);
            }
            return blnReturn;

        }
       
    }
}
