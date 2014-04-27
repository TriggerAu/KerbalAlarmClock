using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

// TODO: Change this namespace to something specific to your plugin here.
//EG:
// namespace MyPlugin_KACWrapper
namespace KACWrapper
{

    ///////////////////////////////////////////////////////////////////////////////////////////
    // BELOW HERE SHOULD NOT BE EDITED - this links to the loaded KAC module without requiring a Hard Dependancy
    ///////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The Wrapper class to access KAC from another plugin
    /// </summary>
    public class KACWrapper
    {
        protected static System.Type KACType;
        protected static System.Type KACAlarmType;
        //protected static System.Type KSPARPResourceListType;

        protected static Object actualKAC = null;

        /// <summary>
        /// This is the Alternate Resource Panel object
        /// 
        /// SET AFTER INIT
        /// </summary>
        public static KACAPI KAC = null;
        /// <summary>
        /// Whether we found the KerbalAlarmClock assembly in the loadedassemblies. 
        /// 
        /// SET AFTER INIT
        /// </summary>
        public static Boolean AssemblyExists { get { return (KACType != null); } }
        /// <summary>
        /// Whether we managed to hook the running Instance from the assembly. 
        /// 
        /// SET AFTER INIT
        /// </summary>
        public static Boolean InstanceExists { get { return (KAC != null); } }
        /// <summary>
        /// Whether we managed to wrap all the methods/functions from the instance. 
        /// 
        /// SET AFTER INIT
        /// </summary>
        private static Boolean _KACWrapped = false;

        /// <summary>
        /// Whether the object has been wrapped and the APIReady flag is set in the real ARP
        /// </summary>
        public static Boolean APIReady { get { return _KACWrapped && KAC.APIReady; } }


        /// <summary>
        /// This method will set up the KAC object and wrap all the methods/functions
        /// </summary>
        /// <param name="Force">This option will force the Init function to rebind everything</param>
        /// <returns></returns>
        public static Boolean InitKACWrapper()
        {
            //if (!_KSPARPWrapped )
            //{
            //reset the internal objects
            _KACWrapped = false;
            actualKAC = null;
            KAC = null;
            LogFormatted("Attempting to Grab KAC Types...");

            //find the base type
            KACType = AssemblyLoader.loadedAssemblies
                .Select(a => a.assembly.GetExportedTypes())
                .SelectMany(t => t)
                .FirstOrDefault(t => t.FullName == "KerbalAlarmClock.KerbalAlarmClock");

            if (KACType == null)
            {
                return false;
            }

            //now the resource Type
            KACAlarmType = AssemblyLoader.loadedAssemblies
                .Select(a => a.assembly.GetExportedTypes())
                .SelectMany(t => t)
                .FirstOrDefault(t => t.FullName == "KerbalAlarmClock.KACAlarm");

            if (KACAlarmType == null)
            {
                return false;
            }

            //now grab the running instance
            LogFormatted("Got Assembly Types, grabbing Instance");
            actualKAC = KACType.GetField("APIInstance", BindingFlags.Public | BindingFlags.Static).GetValue(null);

            if (actualKAC == null)
            {
                LogFormatted("Failed grabbing Instance");
                return false;
            }

            //If we get this far we can set up the local object and its methods/functions
            LogFormatted("Got Instance, Creating Wrapper Objects");
            KAC = new KACAPI(actualKAC);
            //}
            _KACWrapped = true;
            return true;
        }
                
        /// <summary>
        /// The Type that is an analogue of the real ARP. This lets you access all the API-able properties and Methods of the ARP
        /// </summary>
        public class KACAPI
        {

            internal KACAPI(Object KAC)
            {
                //store the actual object
                actualKAC = KAC;

                //these sections get and store the reflection info and actual objects where required. Later in the properties we then read the values from the actual objects
                //for events we also add a handler
                LogFormatted("Getting APIReady Object");
                APIReadyField = KACType.GetField("APIReady", BindingFlags.Public | BindingFlags.Static);
                LogFormatted("Success: " + (APIReadyField != null).ToString());
                
                //WORK OUT THE STUFF WE NEED TO HOOK FOR PEOPEL HERE

            }

            private Object actualKAC;

            private FieldInfo APIReadyField;
            /// <summary>
            /// Whether the APIReady flag is set in the real ARP
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
