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
    public class Alarm:ConfigNodeStorage
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
        public Alarm()
        {
        }
        public Alarm(double UT)
        {
            AlarmTime.UT = UT;
        }

        public Alarm(String vID, String NewName, String NewNotes, double UT, Double Margin, AlarmType atype, AlarmActionEnum aAction)
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

        public Alarm(String vID, String NewName, String NewNotes,  double UT, Double Margin, AlarmType atype, AlarmActionEnum aAction, List<ManeuverNode> NewManeuvers)
            : this(vID, NewName, NewNotes, UT, Margin, atype, aAction)
        {
            //set maneuver node
            ManNodes = NewManeuvers;
        }

        public Alarm(String vID, String NewName, String NewNotes, double UT, Double Margin, AlarmType atype, AlarmActionEnum aAction, KACXFerTarget NewTarget)
            : this(vID, NewName, NewNotes, UT, Margin, atype, aAction)
        {
            //Set target details
            XferOriginBodyName = NewTarget.Origin.bodyName;
            XferTargetBodyName = NewTarget.Target.bodyName;
        }

        public Alarm(String vID, String NewName, String NewNotes, double UT, Double Margin, AlarmType atype, AlarmActionEnum aAction, ITargetable NewTarget)
            : this(vID,NewName,NewNotes,UT,Margin,atype,aAction)
        {
            //Set the ITargetable proerty
            TargetObject = NewTarget;
        }
        #endregion




        [Persistent] public String VesselID="";
        [Persistent] public String Name = "";                                       //Name of Alarm
        [Persistent] public String Notes = "";                                      //Entered extra details
        
        [Persistent] public AlarmType TypeOfAlarm = AlarmType.Raw;                  //What Type of Alarm

        public KACTime AlarmTime = new KACTime();                                   //UT of the alarm
        [Persistent] private Double AlarmTimeStorage;
        
        [Persistent] public Double AlarmMarginSecs = 0;                             //What the margin from the event was
        [Persistent] public Boolean Enabled = true;                                 //Whether it is enabled - not in use currently
        [Persistent] public AlarmActionEnum AlarmAction= AlarmActionEnum.KillWarp;
        
        //public ManeuverNode ManNode;                                              //Stored ManeuverNode attached to alarm
        public List<ManeuverNode> ManNodes = null;                                  //Stored ManeuverNode's attached to alarm

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

        //Details of the alarm message
        public Boolean EditWindowOpen = false;                                        

        /// <summary>
        /// NEED YTP DO SOME MAN NODE SERIALISE STUFF HERE
        /// </summary>

        public override void OnEncodeToConfigNode()
        {
            AlarmTimeStorage = AlarmTime.UT;
            TargetObjectStorage = TargetSerialize(TargetObject);
        }
        public override void OnDecodeFromConfigNode()
        {
            AlarmTime=new KACTime(AlarmTimeStorage);
            _TargetObject = TargetDeserialize(TargetObjectStorage);
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

        private static String TargetSerialize(ITargetable tInput)
        {
            string strReturn = "";

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


    }

    public class AlarmList: List<Alarm>
    {

    }
}
