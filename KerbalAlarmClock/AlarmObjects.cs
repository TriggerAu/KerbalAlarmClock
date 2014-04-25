using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using UnityEngine;
using KSP;
using KSPPluginFramework;

namespace KerbalAlarmClock
{
    public class KACAlarm:ConfigNodeStorage
    {
        public enum AlarmType
        {
            Raw,
            Maneuver,
            ManeuverAuto,
            Apoapsis,
            Periapsis,
            AscendingNode,
            DescendingNode,
            LaunchRendevous,
            Closest,
            SOIChange,
            SOIChangeAuto,
            Transfer,
            TransferModelled,
            Distance,
            Crew,
            EarthTime
        }
        public static Dictionary<AlarmType, int> AlarmTypeToButton = new Dictionary<AlarmType, int>() {
            {AlarmType.Raw, 0},
            {AlarmType.Maneuver , 1},
            {AlarmType.ManeuverAuto , 1},
            {AlarmType.Apoapsis , 2},
            {AlarmType.Periapsis , 2},
            {AlarmType.AscendingNode , 3},
            {AlarmType.DescendingNode , 3},
            {AlarmType.LaunchRendevous , 3},
            {AlarmType.Closest , 4},
            {AlarmType.Distance , 4},
            {AlarmType.SOIChange , 5},
            {AlarmType.SOIChangeAuto , 5},
            {AlarmType.Transfer , 6},
            {AlarmType.TransferModelled , 6},
            {AlarmType.Crew , 7}
        };
        public static Dictionary<int, AlarmType> AlarmTypeFromButton = new Dictionary<int, AlarmType>() {
            {0,AlarmType.Raw},
            {1,AlarmType.Maneuver },
            {2,AlarmType.Apoapsis },
            {3,AlarmType.AscendingNode },
            {4,AlarmType.Closest },
            {5,AlarmType.SOIChange },
            {6,AlarmType.Transfer },
            {7,AlarmType.Crew }
        };




        public enum AlarmActionEnum
        {

            [Description("Message Only-No Affect on warp")]     MessageOnly,
            [Description("Kill Warp Only-No Message")]          KillWarpOnly,
            [Description("Kill Warp and Message")]              KillWarp,
            [Description("Pause Game and Message")]             PauseGame
        }


                #region "Constructors"
        public KACAlarm()
        {
        }
        public KACAlarm(double UT)
        {
            AlarmTime.UT = UT;
        }

        public KACAlarm(String vID, String NewName, String NewNotes, double UT, Double Margin, AlarmType atype, AlarmActionEnum aAction)
        {
            VesselID = vID;
            Name = NewName;
            Notes = NewNotes;
            AlarmTime.UT = UT;
            AlarmMarginSecs = Margin;
            TypeOfAlarm = atype;
            Remaining.UT = AlarmTime.UT - Planetarium.GetUniversalTime();
            AlarmAction = aAction;
        }

        public KACAlarm(String vID, String NewName, String NewNotes,  double UT, Double Margin, AlarmType atype, AlarmActionEnum aAction, List<ManeuverNode> NewManeuvers)
            : this(vID, NewName, NewNotes, UT, Margin, atype, aAction)
        {
            //set maneuver node
            ManNodes = NewManeuvers;
        }

        public KACAlarm(String vID, String NewName, String NewNotes, double UT, Double Margin, AlarmType atype, AlarmActionEnum aAction, KACXFerTarget NewTarget)
            : this(vID, NewName, NewNotes, UT, Margin, atype, aAction)
        {
            //Set target details
            XferOriginBodyName = NewTarget.Origin.bodyName;
            XferTargetBodyName = NewTarget.Target.bodyName;
        }

        public KACAlarm(String vID, String NewName, String NewNotes, double UT, Double Margin, AlarmType atype, AlarmActionEnum aAction, ITargetable NewTarget)
            : this(vID,NewName,NewNotes,UT,Margin,atype,aAction)
        {
            //Set the ITargetable proerty
            TargetObject = NewTarget;
        }
        #endregion




        [Persistent] public String VesselID="";
        [Persistent] public String Name = "";                                       //Name of Alarm
        public String Notes = "";                                      //Entered extra details
        [Persistent] private String NotesStorage = "";                                      //Entered extra details
        
        [Persistent] public AlarmType TypeOfAlarm = AlarmType.Raw;                  //What Type of Alarm

        public KACTime AlarmTime = new KACTime();                                   //UT of the alarm
        [Persistent] private Double AlarmTimeStorage;
        
        [Persistent] public Double AlarmMarginSecs = 0;                             //What the margin from the event was
        [Persistent] public Boolean Enabled = true;                                 //Whether it is enabled - not in use currently
        [Persistent] public AlarmActionEnum AlarmAction= AlarmActionEnum.KillWarp;
        
        //public ManeuverNode ManNode;                                              //Stored ManeuverNode attached to alarm
        public List<ManeuverNode> ManNodes = new List<ManeuverNode>();                                  //Stored ManeuverNode's attached to alarm
        [Persistent] String ManNodesStorage = "";

        [Persistent] public String XferOriginBodyName = "";                         //Stored orbital transfer details
        [Persistent] public String XferTargetBodyName = "";

        //Have to generate these details when the target object is set
        private ITargetable _TargetObject = null;                                   //Stored Target Details
        [Persistent] private String TargetObjectStorage;

        //Vessel Target - needs the fancy get routine as the alarms load before the vessels are loaded.
        //This means that each time the object is accessed if its not yet loaded it trys again
        public ITargetable TargetObject
        {
            get
            {
                if (_TargetObject != null)
                    return _TargetObject;
                else
                {
                    //is there something to load here from the string
                    if (TargetLoader != "")
                    {
                        String[] TargetParts = TargetLoader.Split(",".ToCharArray());
                        switch (TargetParts[0])
                        {
                            case "Vessel":
                                if (KerbalAlarmClock.StoredVesselExists(TargetParts[1]))
                                    _TargetObject = KerbalAlarmClock.StoredVessel(TargetParts[1]);
                                break;
                            case "CelestialBody":
                                if (KerbalAlarmClock.CelestialBodyExists(TargetParts[1]))
                                    TargetObject = KerbalAlarmClock.CelestialBody(TargetParts[1]);
                                break;
                            default:
                                MonoBehaviourExtended.LogFormatted("No Target Found:{0}", TargetLoader);
                                break;
                        }
                    }
                    return _TargetObject;
                }
            }
            set { _TargetObject = value; }
        }
        //Need this one as some vessels arent loaded when the config comes in
        public String TargetLoader = "";


        [Persistent] internal Boolean DeleteOnClose;                                //Whether the checkbox is on or off for this
        [Persistent] internal Boolean Triggered = false;                               //Has this alarm been triggered
        [Persistent] internal Boolean Actioned = false;                                //Has the code actioned th alarm - ie. displayed its message


        //Dynamic props down here
        public KACTime Remaining = new KACTime();                        //UT value of how long till the alarm fires
        public Boolean WarpInfluence = false;                           //Whether the Warp setting is being influenced by this alarm

        //Details of the alarm message window
        public Rect AlarmWindow;
        public int AlarmWindowID = 0;
        public int AlarmWindowHeight = 148;
        [Persistent] internal Boolean AlarmWindowClosed = false;

        //Details of the alarm message
        public Boolean EditWindowOpen = false;


        public Boolean PauseGame { get { return AlarmAction == AlarmActionEnum.PauseGame; } }
        public Boolean HaltWarp { get { return (AlarmAction == AlarmActionEnum.KillWarp || AlarmAction == AlarmActionEnum.KillWarpOnly); } }
                
        public override void OnEncodeToConfigNode()
        {
            NotesStorage = KACUtils.EncodeVarStrings(Notes);
            AlarmTimeStorage = AlarmTime.UT;
            TargetObjectStorage = TargetSerialize(TargetObject);
            ManNodesStorage = ManNodeSerializeList(ManNodes);
        }
        public override void OnDecodeFromConfigNode()
        {
            Notes = KACUtils.DecodeVarStrings(NotesStorage);
            AlarmTime=new KACTime(AlarmTimeStorage);
            _TargetObject = TargetDeserialize(TargetObjectStorage);
            ManNodes = ManNodeDeserializeList( ManNodesStorage);
        }
        
        internal static ITargetable TargetDeserialize(String strInput)
        {
            ITargetable tReturn = null;
            if (strInput == "") return null;

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

        internal static String TargetSerialize(ITargetable tInput)
        {
            string strReturn = "";

            if (tInput == null) return "";
            strReturn += tInput.GetType();
            strReturn += ",";

            if (tInput is Vessel)
            {
                Vessel tmpVessel = tInput as Vessel;
                strReturn += tmpVessel.id.ToString();
            }
            else if (tInput is CelestialBody)
            {
                CelestialBody tmpBody = tInput as CelestialBody;
                strReturn += tmpBody.bodyName;
            }

            return strReturn;

        }

        public static List<ManeuverNode> ManNodeDeserializeList(String strInput)
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

        public static ManeuverNode ManNodeDeserialize(String strInput)
        {
            ManeuverNode mReturn = new ManeuverNode();
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

        public static string ManNodeSerializeList(List<ManeuverNode> mInput)
        {
            String strReturn = "";
            foreach (ManeuverNode tmpMNode in mInput)
            {
                strReturn += ManNodeSerialize(tmpMNode);
                strReturn += ",";
            }
            strReturn = strReturn.TrimEnd(",".ToCharArray());
            return strReturn;
        }

        public static string ManNodeSerialize(ManeuverNode mInput)
        {
            String strReturn = mInput.UT.ToString();
            strReturn += "," + KACUtils.CommaSepVariables(mInput.DeltaV.x, mInput.DeltaV.y, mInput.DeltaV.z);
            strReturn += "," + KACUtils.CommaSepVariables(mInput.nodeRotation.x, mInput.nodeRotation.y, mInput.nodeRotation.z, mInput.nodeRotation.w);
            return strReturn;
        }

        public static Boolean CompareManNodeListSimple(List<ManeuverNode> l1, List<ManeuverNode> l2)
        {
            Boolean blnReturn = true;

            if (l1.Count != l2.Count)
                blnReturn = false;
            else
            {
                for (int i = 0; i < l1.Count; i++)
                {
                    if (l1[i].UT != l2[i].UT)
                        blnReturn = false;
                    else if (l1[i].DeltaV != l2[i].DeltaV)
                        blnReturn = false;
                }
            }

            return blnReturn;
        }
        public static int SortByUT(KACAlarm c1, KACAlarm c2)
        {
            return c1.Remaining.UT.CompareTo(c2.Remaining.UT);
        }

    }

    //public class ManeuverNodeStorageList:List<ManeuverNodeStorage>
    //{
    //    public List<ManeuverNode> ToManNodeList()
    //    {
    //        List<ManeuverNode> lstReturn = new List<ManeuverNode>();
    //        foreach (ManeuverNodeStorage item in this)
    //        {
    //            lstReturn.Add(item.ToManeuverNode());
    //        }
    //        return lstReturn;
    //    }

    //    public ManeuverNodeStorageList FromManNodeList(List<ManeuverNode> ManNodesToStore)
    //    {
    //        this.Clear();
    //        MonoBehaviourExtended.LogFormatted("{0}", ManNodesToStore.Count);
    //        if (ManNodesToStore == null) return this;
    //        foreach (ManeuverNode item in ManNodesToStore)
    //        {
    //            this.Add(new ManeuverNodeStorage(item));
    //        }
    //        return this;
    //    }
    //}

    //public class ManeuverNodeStorage
    //{
    //    public ManeuverNodeStorage() { }
    //    public ManeuverNodeStorage(ManeuverNode newManNode) 
    //    {
    //        FromManeuverNode(newManNode);
    //    }

    //    [Persistent] Vector3 DeltaV;
    //    [Persistent] Quaternion NodeRotation;
    //    [Persistent] Double UT;

    //    public ManeuverNode ToManeuverNode()
    //    {
    //        ManeuverNode retManNode = new ManeuverNode();
    //        retManNode.DeltaV = DeltaV;
    //        retManNode.nodeRotation = NodeRotation;
    //        retManNode.UT = UT;
    //        return retManNode;
    //    }
    //    public ManeuverNodeStorage FromManeuverNode(ManeuverNode ManNodeToStore)
    //    {
    //        this.DeltaV = ManNodeToStore.DeltaV;
    //        this.NodeRotation = ManNodeToStore.nodeRotation;
    //        this.UT = ManNodeToStore.UT;
    //        return this;
    //    }
    //}

    public class KACAlarmList: List<KACAlarm>
    {


        public ConfigNode EncodeToCN()
        {
            KACAlarmListStorage lstTemp = new KACAlarmListStorage();
            lstTemp.list = this;
            //MonoBehaviourExtended.LogFormatted("{0}", lstTemp.list.Count);
            //foreach (KACAlarm item in lstTemp.list)
            //{
            //    MonoBehaviourExtended.LogFormatted("{0}", item.AsConfigNode);
            //}
            ConfigNode cnReturn = lstTemp.AsConfigNode;
            MonoBehaviourExtended.LogFormatted_DebugOnly("Encoding:{0}", cnReturn);
            //MonoBehaviourExtended.LogFormatted("{0}", cnReturn.GetNode("list"));
            return cnReturn;
        }

        public void DecodeFromCN(ConfigNode AlarmListNode)
        {
            try
            {
                MonoBehaviourExtended.LogFormatted_DebugOnly("Decoding:{0}", AlarmListNode);
                KACAlarmListStorage lstTemp = new KACAlarmListStorage();
                ConfigNode.LoadObjectFromConfig(lstTemp, AlarmListNode);
                this.Clear();
                this.AddRange(lstTemp.list);

            }
            catch (Exception ex)
            {
                MonoBehaviourExtended.LogFormatted("Failed to Load Alarms from Save File");
                MonoBehaviourExtended.LogFormatted_DebugOnly(ex.Message);
            }
        }

        /// <summary>
        /// Get the Alarm object from the Unity Window ID
        /// </summary>
        /// <param name="windowID"></param>
        /// <returns></returns>
        public KACAlarm GetByWindowID(Int32 windowID)
        {
            return this.FirstOrDefault(x => x.AlarmWindowID == windowID);
        }

        /// <summary>
        /// Are there any alarms for this save file that are in the future and not already actioned
        /// </summary>
        /// <param name="SaveName"></param>
        /// <returns></returns>
        public Boolean ActiveEnabledFutureAlarms(String SaveName)
        {
            Boolean blnReturn = false;

            foreach (KACAlarm tmpAlarm in this)
            {
                if (tmpAlarm.AlarmTime.UT > Planetarium.GetUniversalTime() && tmpAlarm.Enabled && !tmpAlarm.Actioned )
                {
                    blnReturn = true;
                }
            }
            return blnReturn;
        }
    }

    public class KACAlarmListStorage : ConfigNodeStorage
    {
        [Persistent]
        public List<KACAlarm> list;
    }
}
