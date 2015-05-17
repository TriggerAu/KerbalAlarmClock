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
        public static Boolean APIReady { get { return _KERWrapped && KER.APIReady ; } }

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


            ////
            //foreach(Type ass in  AssemblyLoader.loadedAssemblies
            //    .Select(a => a.assembly.GetExportedTypes())
            //    .SelectMany(t => t)){

            //        LogFormatted("{0}", ass.FullName);
            //}
                

            //find the base type
            KERManoeuvreProcessorType = AssemblyLoader.loadedAssemblies
                .Select(a => a.assembly.GetExportedTypes())
                .SelectMany(t => t)
                .FirstOrDefault(t => t.FullName == "KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode.ManoeuvreProcessor");

            if (KERManoeuvreProcessorType == null)
            {
                return false;
            }

            LogFormatted("KER Version:{0}", KERManoeuvreProcessorType.Assembly.GetName().Version.ToString());


            //try {
            //    actualKER = KERManoeuvreProcessorType.GetField("ManoeuvreProcessor", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            //} catch (Exception) {
            //}
            //if (actualKER == null)
            //{
            //    LogFormatted("Failed grabbing Instance");
            //    return false;
            //}
            




            
            ////now grab the running instance
            LogFormatted("Got Assembly Types, grabbing Instance");

            //try
            //{
            //    actualKAC = KACType.GetField("APIInstance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
            //}
            //catch (Exception)
            //{
            //    NeedUpgrade = true;
            //    LogFormatted("No APIInstance found - most likely you have KAC v2 installed");
            //    //throw;
            //}
            //if (actualKAC == null)
            //{
            //    LogFormatted("Failed grabbing Instance");
            //    return false;
            //}

            //If we get this far we can set up the local object and its methods/functions
            LogFormatted("Got Instance, Creating Wrapper Objects");
            KER = new KERAPI(actualKER);
            //}
            _KERWrapped = true;
            return true;
        }


        /// <summary>
        /// The Type that is an analogue of the real KAC. This lets you access all the API-able properties and Methods of the KAC
        /// </summary>
        public class KERAPI
        {
            private PropertyInfo BurnTimeProp, HalfBurnTimeProp;
            
            internal KERAPI(Object KER)
            {
                actualKER = KER;

                BurnTimeProp = KERManoeuvreProcessorType.GetProperty("BurnTime", BindingFlags.Public | BindingFlags.Static);
                LogFormatted("Success: " + (BurnTimeProp != null).ToString());
                //actualBurnTime = BurnTimeProp.GetValue(actualKER,null);

                HalfBurnTimeProp = KERManoeuvreProcessorType.GetProperty("HalfBurnTime", BindingFlags.Public | BindingFlags.Static);
                LogFormatted("Success: " + (HalfBurnTimeProp != null).ToString());
                //actualBurnTime = HalfBurnTimeProp.GetValue(actualKER,null));

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

            private FieldInfo APIReadyField;
            /// <summary>
            /// Whether the APIReady flag is set in the real KAC
            /// </summary>
            public Boolean APIReady
            {
                get
                {
                    if (APIReadyField == null)
                        return false;

                    return (Boolean)APIReadyField.GetValue(null);
                }
            }
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


