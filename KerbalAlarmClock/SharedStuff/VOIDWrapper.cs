using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KAC_VOIDWrapper
{
    class VOIDWrapper
    {
        //protected static System.Type VOIDType;
        protected static System.Type VOID_DataType;

        protected static Object actualVOID = null;

        /// <summary>
        /// This is the VOIDbal Alarm Clock object
        /// 
        /// SET AFTER INIT
        /// </summary>
        public static VOIDAPI VOID = null;
        /// <summary>
        /// Whether we found the VOIDbalAlarmClock assembly in the loadedassemblies. 
        /// 
        /// SET AFTER INIT
        /// </summary>
        public static Boolean AssemblyExists { get { return (VOID_DataType != null); } }
        /// <summary>
        /// Whether we managed to hook the running Instance from the assembly. 
        /// 
        /// SET AFTER INIT
        /// </summary>
        public static Boolean InstanceExists { get { return (VOID != null); } }
        /// <summary>
        /// Whether we managed to wrap all the methods/functions from the instance. 
        /// 
        /// SET AFTER INIT
        /// </summary>
        private static Boolean _VOIDWrapped = false;

        /// <summary>
        /// Whether the object has been wrapped and the APIReady flag is set in the real KAC
        /// </summary>
        public static Boolean APIReady { get { return _VOIDWrapped ; } }

        /// <summary>
        /// This method will set up the KAC object and wrap all the methods/functions
        /// </summary>
        /// <param name="Force">This option will force the Init function to rebind everything</param>
        /// <returns></returns>
        public static Boolean InitVOIDWrapper()
        {
            //if (!_KACWrapped )
            //{
            //reset the internal objects
            _VOIDWrapped = false;
            actualVOID = null;
            VOID = null;
            LogFormatted("Attempting to Grab VOID Types...");


            //find the base type
            AssemblyLoader.loadedAssemblies.TypeOperation(t =>
            {
                if (t.FullName == "VOID.VOID_Data")
                    VOID_DataType = t;
            });
            
            if (VOID_DataType == null)
            {
                return false;
            }

            LogFormatted("VOID Version:{0}", VOID_DataType.Assembly.GetName().Version.ToString());

            LogFormatted("Creating Wrapper Objects");
            VOID = new VOIDAPI(actualVOID);
            //}
            _VOIDWrapped = true;
            return true;
        }


        /// <summary>
        /// The Type that is an analogue of the real KAC. This lets you access all the API-able properties and Methods of the KAC
        /// </summary>
        public class VOIDAPI
        {
            private FieldInfo BurnTimeProp, HalfBurnTimeProp;
            private FieldInfo TotalDVProp, CurrManeuverDeltaVProp;
            //, HasDeltaVProp;
            
            internal VOIDAPI(Object VOID)
            {
                actualVOID = VOID;

                BurnTimeProp = VOID_DataType.GetField("currentNodeBurnDuration", BindingFlags.Public | BindingFlags.Static);
                LogFormatted("currentNodeBurnDuration Success: " + (BurnTimeProp != null).ToString());
                //actualBurnTime = BurnTimeProp.GetValue(actualVOID,null);

                HalfBurnTimeProp = VOID_DataType.GetField("currentNodeHalfBurnDuration", BindingFlags.Public | BindingFlags.Static);
                LogFormatted("currentNodeHalfBurnDuration Success: " + (HalfBurnTimeProp != null).ToString());
                //actualBurnTime = HalfBurnTimeProp.GetValue(actualVOID,null));

                TotalDVProp = VOID_DataType.GetField("totalDeltaV", BindingFlags.Public | BindingFlags.Static);
                LogFormatted("totalDeltaV Success: " + (TotalDVProp != null).ToString());

                CurrManeuverDeltaVProp = VOID_DataType.GetField("currManeuverDeltaV", BindingFlags.Public | BindingFlags.Static);
                LogFormatted("currManeuverDeltaV Success: " + (CurrManeuverDeltaVProp != null).ToString());

                //HasDeltaVProp = VOID_DataType.GetProperty("HasDeltaV", BindingFlags.Public | BindingFlags.Static);
                //LogFormatted("Success: " + (HasDeltaVProp != null).ToString());
            }

            private Object actualVOID;


            public Double BurnTime
            {
                get {
                    object BurnTimeVOID_DoubleValue = BurnTimeProp.GetValue(null);

                    //get the Value property off the Object
                    Double res = (Double)BurnTimeVOID_DoubleValue.GetType().GetProperty("Value").GetValue(BurnTimeVOID_DoubleValue, null);
                    
                    //if its infinity as theres not enough dV
                    if (Double.IsInfinity(res))
                    {
                        res = 0;
                    }
                    return res;
                }

            }
            public Double HalfBurnTime
            {
                get {
                    object HalfBurnTimeVOID_DoubleValue = HalfBurnTimeProp.GetValue(null);

                    //get the Value property off the Object
                    Double res = (Double)HalfBurnTimeVOID_DoubleValue.GetType().GetProperty("Value").GetValue(HalfBurnTimeVOID_DoubleValue, null);

                    //if its infinity as theres not enough dV
                    if (Double.IsInfinity(res))
                    {
                        res = 0;
                    }
                    return res;
                }
            }


            public Boolean HasDeltaV
            {
                get {

                    object TotalDVProp_DoubleValue = TotalDVProp.GetValue(null);
                    Double totalDeltaV = (Double)TotalDVProp_DoubleValue.GetType().GetProperty("Value").GetValue(TotalDVProp_DoubleValue, null);
                    object CurrManeuverDeltaVProp_DoubleValue = CurrManeuverDeltaVProp.GetValue(null);
                    Double currManeuverDeltaV = (Double)CurrManeuverDeltaVProp_DoubleValue.GetType().GetProperty("Value").GetValue(CurrManeuverDeltaVProp_DoubleValue, null);

                    return (totalDeltaV>=currManeuverDeltaV); 
                
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


