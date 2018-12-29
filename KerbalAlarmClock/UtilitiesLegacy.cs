using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KerbalAlarmClock
{
    internal static class UtilitiesLegacy
    {


        internal static Boolean Loadv2Alarms(out String LoadMessage, out KACAlarmList oldAlarms)
        {
            oldAlarms = new KACAlarmList();
            Boolean blnReturn = false;

            try
            {
                //Find old files
                String[] AlarmFiles = System.IO.Directory.GetFiles(KACUtils.PathTriggerTech, "Alarms-*.txt", System.IO.SearchOption.AllDirectories);
                String FileToLoad = "";
                foreach (String item in AlarmFiles)
                {
                    System.IO.FileInfo File = new System.IO.FileInfo(item);
                    MonoBehaviourExtended.LogFormatted_DebugOnly("File:{0}", File.Name);

                    if (File.Name == String.Format("Alarms-{0}.txt", HighLogic.CurrentGame.Title))
                    {
                        FileToLoad = File.FullName;
                        break;
                    }
                }
                if (FileToLoad != "")
                {
                    //parse it to a new list
                    MonoBehaviourExtended.LogFormatted("Loading {0}", FileToLoad);
                    String strFile = System.IO.File.ReadAllText(FileToLoad);

                    String AlarmsFileVersion = "";

                    while (strFile.Contains("|<ENDLINE>"))
                    {
                        String strAlarm = strFile.Substring(0, strFile.IndexOf("|<ENDLINE>"));
                        strFile = strFile.Substring(strAlarm.Length + "|<ENDLINE>".Length).TrimStart("\r\n".ToCharArray());

                        if (strAlarm.StartsWith("AlarmsFileVersion|"))
                        {
                            AlarmsFileVersion = strAlarm.Split("|".ToCharArray())[1];
                            MonoBehaviourExtended.LogFormatted("AlarmsFileVersion:{0}", AlarmsFileVersion);
                        }
                        else if (!strAlarm.StartsWith("VesselID|"))
                        {
                            KACAlarm tmpAlarm;

                            switch (AlarmsFileVersion)
                            {
                                case "3":
                                    MonoBehaviourExtended.LogFormatted_DebugOnly("Loading Alarm via v3 loader");
                                    tmpAlarm = UtilitiesLegacy.LoadFromString3(strAlarm, KACWorkerGameState.CurrentTime.UT);
                                    break;
                                default:
                                    MonoBehaviourExtended.LogFormatted_DebugOnly("Loading Alarm via v2 loader");
                                    tmpAlarm = UtilitiesLegacy.LoadFromString2(strAlarm);
                                    break;
                            }

                            oldAlarms.Add(tmpAlarm);
                        }
                    }
                    blnReturn = true;
                    LoadMessage = "Successfully parsed Alarm File";
                }
                else
                {
                    MonoBehaviourExtended.LogFormatted("Could not find alarms file for: {0}", HighLogic.CurrentGame.Title);
                    LoadMessage = "File not found in TriggerTech Folder";
                }

            }
            catch (Exception ex)
            {
                MonoBehaviourExtended.LogFormatted("Error occured:{0}\r\n{1}", ex.Message,ex.StackTrace);
                LoadMessage = "Unknown error occured trying to load old file\r\n\r\nError details in output_log.txt";
            }
            return blnReturn;
        }

        // <summary>
        // Basically deserializing the alarm
        // </summary>
        // <param name="AlarmDetails"></param>
        //internal static KACAlarm LoadFromString(String AlarmDetails)
        //{
        //    String[] vars = AlarmDetails.Split("|".ToCharArray());
        //    String VesselID = "";
        //    String SaveName = vars[0];
        //    String Name = vars[1];
        //    Boolean Enabled = Convert.ToBoolean(vars[2]);
        //    Double UT = Convert.ToDouble(vars[3]);
        //    Boolean HaltWarp = Convert.ToBoolean(vars[4]);
        //    Boolean PauseGame = false;
        //    String Notes = "";
        //    if (vars.Length == 6)
        //        Notes = vars[5];
        //    else
        //    {
        //        PauseGame = Convert.ToBoolean(vars[5]);
        //        Notes = vars[6];
        //    }

        //    KACAlarm resultAlarm = new KACAlarm(UT);
        //    resultAlarm.Name = Name;
        //    resultAlarm.Enabled = Enabled;
        //    resultAlarm.Notes = Notes;
        //    resultAlarm.AlarmAction = KACAlarm.AlarmActionEnum.MessageOnly;
        //    if (HaltWarp)
        //        resultAlarm.AlarmAction = KACAlarm.AlarmActionEnum.KillWarp;
        //    else if (PauseGame)
        //        resultAlarm.AlarmAction = KACAlarm.AlarmActionEnum.PauseGame;

        //    return resultAlarm;
        //}

        internal static KACAlarm LoadFromString2(String AlarmDetails)
        {
            String[] vars = AlarmDetails.Split("|".ToCharArray(), StringSplitOptions.None);
            //String SaveName = HighLogic.CurrentGame.Title;    //Commented because usage removed
            String VesselID = vars[0];
            String Name = vars[1];
            String Notes = vars[2];
            Double UT = Convert.ToDouble(vars[3]);
            Double AlarmMarginSecs = Convert.ToDouble(vars[4]);
            KACAlarm.AlarmTypeEnum TypeOfAlarm = (KACAlarm.AlarmTypeEnum)Enum.Parse(typeof(KACAlarm.AlarmTypeEnum), vars[5]);
            Boolean Enabled = Convert.ToBoolean(vars[6]);
            Boolean HaltWarp = Convert.ToBoolean(vars[7]);
            Boolean PauseGame = Convert.ToBoolean(vars[8]);

            String strOptions = vars[9];

            List<ManeuverNode> ManNodes = null;
            String XferOriginBodyName="", XferTargetBodyName="";
            ITargetable TargetObject = null;
            String TargetLoader="";

            switch (TypeOfAlarm)
            {
                case KACAlarm.AlarmTypeEnum.Maneuver:
                    //Generate the Nodes List from the string
                    ManNodes = ManNodeDeserializeList(strOptions);
                    break;
                case KACAlarm.AlarmTypeEnum.Transfer:
                    try
                    {
                        String[] XferParts = strOptions.Split(",".ToCharArray());
                        XferOriginBodyName = XferParts[0];
                        XferTargetBodyName = XferParts[1];
                    }
                    catch (Exception ex)
                    {
                        MonoBehaviourExtended.LogFormatted("Unable to load transfer details for {0}", Name);
                        MonoBehaviourExtended.LogFormatted(ex.Message);
                    }
                    break;
                case KACAlarm.AlarmTypeEnum.AscendingNode:
                case KACAlarm.AlarmTypeEnum.DescendingNode:
                case KACAlarm.AlarmTypeEnum.LaunchRendevous:
                    if (strOptions != "")
                    {
                        //find the targetable object and set it
                        TargetObject = TargetDeserialize(strOptions);
                        if (TargetObject == null && strOptions.StartsWith("Vessel,"))
                            TargetLoader = strOptions;
                    }
                    break;
                default:
                    break;
            }

            KACAlarm resultAlarm = new KACAlarm(UT);
            resultAlarm.Name = Name;
            resultAlarm.VesselID = VesselID;
            resultAlarm.Enabled = Enabled;
            resultAlarm.Notes = Notes;
            resultAlarm.AlarmMarginSecs = AlarmMarginSecs;
            resultAlarm.TypeOfAlarm = TypeOfAlarm;
            if (HaltWarp)
                resultAlarm.AlarmActionConvert = KACAlarm.AlarmActionEnum.KillWarp;
            else if (PauseGame)
                resultAlarm.AlarmActionConvert = KACAlarm.AlarmActionEnum.PauseGame;

            if (ManNodes != null)
                resultAlarm.ManNodes = ManNodes;
            if (TargetObject != null)
                resultAlarm.TargetObject = TargetObject;
            resultAlarm.TargetLoader = TargetLoader;

            resultAlarm.XferOriginBodyName = XferOriginBodyName;
            resultAlarm.XferTargetBodyName = XferTargetBodyName;

            return resultAlarm;

        }

        internal static KACAlarm LoadFromString3(String AlarmDetails, Double CurrentUT)
        {
            //String is "VesselID|Name|Notes|AlarmTime.UT|AlarmMarginSecs|Type|Enabled|HaltWarp|PauseGame|ActionedAt|Maneuver|Xfer|Target|Options|<ENDLINE>");

            String[] vars = AlarmDetails.Split("|".ToCharArray(), StringSplitOptions.None);

            MonoBehaviourExtended.LogFormatted("AlarmExtract");
            for (int i = 0; i < vars.Length; i++) {
			    MonoBehaviourExtended.LogFormatted("{0}:{1}",i,vars[i]);
			}

            //String SaveName = HighLogic.CurrentGame.Title;    //Commented because usage removed
            String VesselID = vars[0];
            String Name = KACUtils.DecodeVarStrings(vars[1]);
            String Notes = KACUtils.DecodeVarStrings(vars[2]);
            Double UT = Convert.ToDouble(vars[3]);
            Double AlarmMarginSecs = Convert.ToDouble(vars[4]);
            KACAlarm.AlarmTypeEnum TypeOfAlarm = (KACAlarm.AlarmTypeEnum)Enum.Parse(typeof(KACAlarm.AlarmTypeEnum), vars[5]);
            Boolean Enabled = Convert.ToBoolean(vars[6]);
            Boolean HaltWarp = Convert.ToBoolean(vars[7]);
            Boolean PauseGame = Convert.ToBoolean(vars[8]);
            Double ActionedAt = Convert.ToDouble(vars[9]);

            List<ManeuverNode> ManNodes=null;
            String XferOriginBodyName="", XferTargetBodyName="";
            ITargetable TargetObject=null;
            String TargetLoader="";

            Boolean Triggered=false, Actioned=false, AlarmWindowClosed=false;
            
            if (vars[10] != "")
                ManNodes = ManNodeDeserializeList(vars[10]);

            if (vars[11] != "")
            {
                try
                {
                    MonoBehaviourExtended.LogFormatted("{0}", vars[11]);
                    String[] XferParts = vars[11].Split(",".ToCharArray());
                    XferOriginBodyName = XferParts[0];
                    XferTargetBodyName = XferParts[1];
                }
                catch (Exception ex)
                {
                    MonoBehaviourExtended.LogFormatted("Unable to load transfer details for {0}", Name);
                    MonoBehaviourExtended.LogFormatted(ex.Message);
                }
            }
            if (vars[12] != "")
            {
                //find the targetable object and set it
                TargetObject = TargetDeserialize(vars[12]);
                if (TargetObject == null && vars[12].StartsWith("Vessel,"))
                    TargetLoader = vars[12];
            }

            //Now do the work to set Actioned/triggered/etc if needed
            //LogFormatted("A:{0},T:{1:0},Act:{2:0}", this.Name, CurrentUT, this.ActionedAt);
            if (ActionedAt > 0 && CurrentUT > ActionedAt)
            {
                MonoBehaviourExtended.LogFormatted("Suppressing Alarm on Load:{0}", Name);
                Triggered = true;
                Actioned = true;
                AlarmWindowClosed = true;
            }
            else if (ActionedAt > CurrentUT)
            {
                MonoBehaviourExtended.LogFormatted("Reenabling Alarm on Load:{0}", Name);
                Triggered = false;
                Actioned = false;
                ActionedAt = 0;
                AlarmWindowClosed = false;
            }

            KACAlarm resultAlarm = new KACAlarm(UT);
            resultAlarm.Name = Name;
            resultAlarm.VesselID = VesselID;
            resultAlarm.Enabled = Enabled;
            resultAlarm.Notes = Notes;
            resultAlarm.AlarmMarginSecs = AlarmMarginSecs;
            resultAlarm.TypeOfAlarm = TypeOfAlarm;
            if (HaltWarp)
                resultAlarm.AlarmActionConvert = KACAlarm.AlarmActionEnum.KillWarp;
            else if (PauseGame)
                resultAlarm.AlarmActionConvert = KACAlarm.AlarmActionEnum.PauseGame;

            if (ManNodes != null)
                resultAlarm.ManNodes = ManNodes;
            if (TargetObject != null)
                resultAlarm.TargetObject = TargetObject;
            resultAlarm.TargetLoader = TargetLoader;

            resultAlarm.XferOriginBodyName = XferOriginBodyName;
            resultAlarm.XferTargetBodyName = XferTargetBodyName;

            resultAlarm.Triggered = Triggered;
            resultAlarm.Actioned = Actioned;
            resultAlarm.AlarmWindowClosed = AlarmWindowClosed;
            
            return resultAlarm;
        }

        private static ITargetable TargetDeserialize(String strInput)
        {
            ITargetable tReturn = null;
            String[] TargetParts = strInput.Split(",".ToCharArray());
            switch (TargetParts[0])
            {
                case "Vessel":
                    if (KerbalAlarmClock.StoredVesselExists(TargetParts[1]))
                        tReturn = KerbalAlarmClock.StoredVessel(TargetParts[1]);
                    break;
                case "CelestialBody":
                    if (KerbalAlarmClock.CelestialBodyExists(TargetParts[1]))
                        tReturn = KerbalAlarmClock.CelestialBody(TargetParts[1]);
                    break;
                default:
                    break;
            }
            return tReturn;
        }

        private static List<ManeuverNode> ManNodeDeserializeList(String strInput)
        {
            List<ManeuverNode> lstReturn = new List<ManeuverNode>();

            String[] strInputParts = strInput.Split(",".ToCharArray());
            MonoBehaviourExtended.LogFormatted("Found {0} Maneuver Nodes to deserialize", strInputParts.Length / 8);

            //There are 8 parts per mannode
            for (int iNode = 0; iNode < strInputParts.Length / 8; iNode++)
            {
                String strTempNode = String.Join(",", strInputParts.Skip(iNode * 8).Take(8).ToArray());
                lstReturn.Add(ManNodeDeserialize(strTempNode));
            }

            return lstReturn;
        }

        private static ManeuverNode ManNodeDeserialize(String strInput)
        {
            ManeuverNode mReturn =  new ManeuverNode();
            String[] manparts = strInput.Split(",".ToCharArray());
            mReturn.UT = Convert.ToDouble(manparts[0]);
            mReturn.DeltaV = new Vector3d(Convert.ToDouble(manparts[1]),
                                        Convert.ToDouble(manparts[2]),
                                        Convert.ToDouble(manparts[3])
                    );
            mReturn.nodeRotation = new Quaternion(Convert.ToSingle(manparts[4]),
                                                Convert.ToSingle(manparts[5]),
                                                Convert.ToSingle(manparts[6]),
                                                Convert.ToSingle(manparts[7])
                    );
            return mReturn;
        }
    }
}
