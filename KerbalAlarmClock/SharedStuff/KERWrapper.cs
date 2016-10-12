using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KAC_KERWrapper
{
    class KERWrapper
    {
        //protected static System.Type KERType;
        protected static System.Type KERManoeuvreProcessorType;
        //protected static System.Type KERSimManagerType;

        protected static Object actualKER = null;

        /// <summary>
        /// This is the Kerbal Alarm Clock object
        /// 
        /// SET AFTER INIT
        /// </summary>
        public static KERAPI KER = null;
        /// <summary>
        /// Whether we found the KerbalAlarmClock assembly in the loadedassemblies. 
        /// 
        /// SET AFTER INIT
        /// </summary>
        public static Boolean AssemblyExists { get { return (KERManoeuvreProcessorType != null); } }
        /// <summary>
        /// Whether we managed to hook the running Instance from the assembly. 
        /// 
        /// SET AFTER INIT
        /// </summary>
        public static Boolean InstanceExists { get { return (KER != null); } }
        /// <summary>
        /// Whether we managed to wrap all the methods/functions from the instance. 
        /// 
        /// SET AFTER INIT
        /// </summary>
        private static Boolean _KERWrapped = false;

        /// <summary>
        /// Whether the object has been wrapped and the APIReady flag is set in the real KAC
        /// </summary>
        public static Boolean APIReady { get { return _KERWrapped ; } }

        /// <summary>
        /// This method will set up the KAC object and wrap all the methods/functions
        /// </summary>
        /// <param name="Force">This option will force the Init function to rebind everything</param>
        /// <returns></returns>
        public static Boolean InitKERWrapper()
        {
            //if (!_KACWrapped )
            //{
            //reset the internal objects
            _KERWrapped = false;
            actualKER = null;
            KER = null;
            LogFormatted("Attempting to Grab KER Types...");


            //find the base type
            AssemblyLoader.loadedAssemblies.TypeOperation(t =>
                {
                    if (t.FullName == "KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode.ManoeuvreProcessor")
                        KERManoeuvreProcessorType = t;
                });

            if (KERManoeuvreProcessorType == null)
            {
                return false;
            }

            LogFormatted("KER Version:{0}", KERManoeuvreProcessorType.Assembly.GetName().Version.ToString());


            //KERSimManagerType = AssemblyLoader.loadedAssemblies
            //    .Select(a => a.assembly.GetExportedTypes())
            //    .SelectMany(t => t)
            //    .FirstOrDefault(t => t.FullName == "KerbalEngineer.VesselSimulator.SimManager");

            //if (KERSimManagerType == null)
            //{
            //    LogFormatted("Cant grab SimManager");
            //    return false;
            //}

            LogFormatted("Creating Wrapper Objects");
            KER = new KERAPI(actualKER);
            //}
            _KERWrapped = true;
            return true;
        }


        /// <summary>
        /// The Type that is an analogue of the real KER. This lets you access all the API-able properties and Methods of the KER
        /// </summary>
        public class KERAPI
        {
            private PropertyInfo BurnTimeProp, HalfBurnTimeProp, HasDeltaVProp;
            private MethodInfo UpdateMethod;

            //private FieldInfo bRunningField, bRequestedField;

            internal KERAPI(Object KER)
            {
                PropertyInfo p = KERManoeuvreProcessorType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                if (p != null)
                {
                    LogFormatted("p not NULL");
                    actualKER = p.GetValue(null, null);
                }
                
                // actualKER = KER;

                BurnTimeProp = KERManoeuvreProcessorType.GetProperty("BurnTime", BindingFlags.Public | BindingFlags.Static);
                LogFormatted("BurnTimeProp Success: " + (BurnTimeProp != null).ToString());
                //actualBurnTime = BurnTimeProp.GetValue(actualKER,null);

                HalfBurnTimeProp = KERManoeuvreProcessorType.GetProperty("HalfBurnTime", BindingFlags.Public | BindingFlags.Static);
                LogFormatted("HalfBurnTimeProp Success: " + (HalfBurnTimeProp != null).ToString());
                //actualBurnTime = HalfBurnTimeProp.GetValue(actualKER,null));

                HasDeltaVProp = KERManoeuvreProcessorType.GetProperty("HasDeltaV", BindingFlags.Public | BindingFlags.Static);
                LogFormatted("HasDeltaVProp Success: " + (HasDeltaVProp != null).ToString());

                UpdateMethod = KERManoeuvreProcessorType.GetMethod("Update", BindingFlags.Public | BindingFlags.Instance);
                LogFormatted("UpdateMethod Success: " + (UpdateMethod != null).ToString());

                //bRunningField = KERSimManagerType.GetField("bRunning", BindingFlags.NonPublic | BindingFlags.Static);
                //LogFormatted("bRunningField Success: " + (bRunningField != null).ToString());

                //bRequestedField = KERSimManagerType.GetField("bRequested", BindingFlags.NonPublic | BindingFlags.Static);
                //LogFormatted("bRequestedField Success: " + (bRequestedField != null).ToString());

            }

            private Object actualKER;

            public Double BurnTime
            {
                get { return (Double)BurnTimeProp.GetValue(null, null); }

            }
            public Double HalfBurnTime
            {
                get { return (Double)HalfBurnTimeProp.GetValue(null, null); }

            }

            public Boolean HasDeltaV
            {
                get { return (Boolean)HasDeltaVProp.GetValue(null, null); }

            }
          
            public void UpdateManNodeValues()
            {
                UpdateMethod.Invoke(actualKER, null);
            }

            //public void RequestSimlulation()
            //{
            //    //RequestSimMethod.Invoke(null, null);
            //    RequestManMethod.Invoke(null, null);
            //}

            //public Boolean bRunning
            //{
            //    get { return (Boolean)bRunningField.GetValue(null); }
            //}
            //public Boolean bRequested
            //{
            //    get { return (Boolean)bRequestedField.GetValue(null); }
            //}

        }

        #region Logging Stuff
        /// <summary>
        /// Some Structured logging to the debug file - ONLY RUNS WHEN DLL COMPILED IN DEBUG MODE
        /// </summary>
        /// <param name="Message">Text to be printed - can be formatted as per String.format</param>
        /// <param name="strParams">Objects to feed into a String.format</param>
        [System.Diagnostics.Conditional("DEBUG")]
        internal static void LogFormatted_DebugOnly(String Message, params Object[] strParams)
        {
            LogFormatted(Message, strParams);
        }

        /// <summary>
        /// Some Structured logging to the debug file
        /// </summary>
        /// <param name="Message">Text to be printed - can be formatted as per String.format</param>
        /// <param name="strParams">Objects to feed into a String.format</param>
        internal static void LogFormatted(String Message, params Object[] strParams)
        {
            Message = String.Format(Message, strParams);
            String strMessageLine = String.Format("{0},{2}-{3},{1}",
                DateTime.Now, Message, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
            UnityEngine.Debug.Log(strMessageLine);
        }
        #endregion

    }


}


