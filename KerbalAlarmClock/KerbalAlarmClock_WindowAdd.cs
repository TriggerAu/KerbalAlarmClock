using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;

using UnityEngine;
using KSP;
using KSPPluginFramework;

using KAC_KERWrapper;
using KAC_VOIDWrapper;

using Contracts;

namespace KerbalAlarmClock
{
    public partial class KerbalAlarmClock
    {
        private KACAlarm.AlarmTypeEnum AddType = KACAlarm.AlarmTypeEnum.Raw;
        private KACAlarm.AlarmActionEnum AddAction = KACAlarm.AlarmActionEnum.MessageOnly;
        private AlarmActions AddActions = new AlarmActions() { Warp = AlarmActions.WarpEnum.KillWarp, Message = AlarmActions.MessageEnum.Yes, PlaySound = false, DeleteWhenDone = false };

        private KACTimeStringArray timeRaw = new KACTimeStringArray(600, KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeRepeatPeriod = new KACTimeStringArray(50 * KSPDateStructure.SecondsPerDay, KACTimeStringArray.TimeEntryPrecisionEnum.Days);

        private String strAlarmName = "";
        private String strAlarmNotes = "";
        //private String strAlarmNotes = "";
        //private String strAlarmDetail = "";
        private Boolean blnAlarmAttachToVessel=true;

        private String strAlarmDescSOI = "This will monitor the current active flight path for the next detected SOI change.\r\n\r\nIf the SOI Point changes the alarm will adjust until it is within {0} seconds of the Alarm time, at which point it just maintains the last captured time of the change.";
        private String strAlarmDescXfer = "This will check and recalculate the active transfer alarms for the correct phase angle - the math for these is based around circular orbits so the for any elliptical orbit these need to be recalculated over time.\r\n\r\nThe alarm will adjust until it is within {0} seconds of the target phase angle, at which point it just maintains the last captured time of the angle.\r\nI DO NOT RECOMMEND TURNING THIS OFF UNLESS THERE IS A MASSIVE PERFORMANCE GAIN";
        private String strAlarmDescNode = "This will check and recalculate the active orbit node alarms as the flight path changes. The alarm will adjust until it is within {0} seconds of the node.";

        private string strAlarmDescMan = "Will create an alarm whenever a maneuver node is detected on the vessels flight plan\r\n\r\nIf the Man Node is within {0} seconds of the current time it will not be created";

        /// <summary>
        /// Code to reset the settings etc when the new button is hit
        /// </summary>
        private void NewAddAlarm()
        {
            //Set time variables
            timeRaw.BuildFromUT(600);
            strRawUT = "";
            _ShowAddMessages = false;

            //option for xfer mode
            if (settings.XferUseModelData)
                intXferType = 0;
            else
                intXferType = 1;

            //default margin
            timeMargin.BuildFromUT(settings.AlarmDefaultMargin);

            //set default strings
            if (KACWorkerGameState.CurrentVessel != null)
                strAlarmName = KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName);
            else
                strAlarmName = "No Vessel";
            strAlarmNotes = "";
            AddNotesHeight = 100;

            AddActions = settings.AlarmDefaultAction.Duplicate();
            //blnHaltWarp = true;

            //set initial alarm type based on whats on the flight path
            if (KACWorkerGameState.ManeuverNodeExists)
                AddType = KACAlarm.AlarmTypeEnum.Maneuver;//AddAlarmType.Node;
            else if (KACWorkerGameState.SOIPointExists)
                AddType = KACAlarm.AlarmTypeEnum.SOIChange;//AddAlarmType.Node;
            else
                AddType = KACAlarm.AlarmTypeEnum.Raw;//AddAlarmType.Node;

            //trigger the work to set each type
            AddTypeChanged();

            //build the XFer parents list
            SetUpXferParents();
            intXferCurrentParent = 0;
            SetupXferOrigins();
            intXferCurrentOrigin = 0;

            if (KACWorkerGameState.CurrentVessel != null)
            {
                //if the craft is orbiting a body on the parents list then set it as the default
                if (XferParentBodies.Contains(KACWorkerGameState.CurrentVessel.mainBody.referenceBody))
                {
                    intXferCurrentParent = XferParentBodies.IndexOf(KACWorkerGameState.CurrentVessel.mainBody.referenceBody);
                    SetupXferOrigins();
                    intXferCurrentOrigin = XferOriginBodies.IndexOf(KACWorkerGameState.CurrentVessel.mainBody);
                }
            }
            //set initial targets
            SetupXFerTargets();
            intXferCurrentTarget = 0;

            intSelectedCrew = 0;
            intSelectedContract = 0;
            strCrewUT = "";

            ddlKERNodeMargin.SelectedIndex = (Int32)settings.DefaultKERMargin;

            //ddlAddAlarm.SelectedIndex = ddlAddAlarm.Items.IndexOf(settings.AlarmsSoundName);
            //PlaySound = settings.AlarmPlaySound;

        }

        List<KACAlarm.AlarmTypeEnum> AlarmsThatBuildStrings = new List<KACAlarm.AlarmTypeEnum>() {
            KACAlarm.AlarmTypeEnum.Raw,
            KACAlarm.AlarmTypeEnum.Transfer,
            KACAlarm.AlarmTypeEnum.TransferModelled,
            KACAlarm.AlarmTypeEnum.Crew,
            KACAlarm.AlarmTypeEnum.Contract,
            KACAlarm.AlarmTypeEnum.ContractAuto,
            KACAlarm.AlarmTypeEnum.ScienceLab
        };

        private String strAlarmEventName = "Alarm";
        internal void AddTypeChanged()
        {
            if (AddType == KACAlarm.AlarmTypeEnum.Transfer || AddType == KACAlarm.AlarmTypeEnum.TransferModelled)
                blnAlarmAttachToVessel = false;
            else
                blnAlarmAttachToVessel = true;

            //set strings, etc here for type changes
            switch (AddType)
            {
                case KACAlarm.AlarmTypeEnum.Raw: strAlarmEventName = "Alarm"; break;
                case KACAlarm.AlarmTypeEnum.Maneuver: strAlarmEventName = "Node"; break;
                case KACAlarm.AlarmTypeEnum.SOIChange: strAlarmEventName = "SOI"; break;
                case KACAlarm.AlarmTypeEnum.Transfer:
                case KACAlarm.AlarmTypeEnum.TransferModelled: strAlarmEventName = "Transfer"; break;
                case KACAlarm.AlarmTypeEnum.Apoapsis: strAlarmEventName = "Apoapsis"; break;
                case KACAlarm.AlarmTypeEnum.Periapsis: strAlarmEventName = "Periapsis"; break;
                case KACAlarm.AlarmTypeEnum.AscendingNode: strAlarmEventName = "Ascending"; break;
                case KACAlarm.AlarmTypeEnum.DescendingNode: strAlarmEventName = "Descending"; break;
                case KACAlarm.AlarmTypeEnum.LaunchRendevous: strAlarmEventName = "Launch Ascent"; break;
                case KACAlarm.AlarmTypeEnum.Closest: strAlarmEventName = "Closest"; break;
                case KACAlarm.AlarmTypeEnum.Distance: strAlarmEventName = "Target Distance"; break;
                case KACAlarm.AlarmTypeEnum.Crew: strAlarmEventName = "Crew"; break;
                case KACAlarm.AlarmTypeEnum.Contract: 
                case KACAlarm.AlarmTypeEnum.ContractAuto: strAlarmEventName = "Contract"; break;
                case KACAlarm.AlarmTypeEnum.ScienceLab: strAlarmEventName = "Science Lab"; break;
                default:
                    strAlarmEventName = "Alarm"; 
                    break;
            }

            //set strings, etc here for type changes
            strAlarmName = (KACWorkerGameState.CurrentVessel != null) ? KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) : "Alarm";
            strAlarmNotes = "";
            if (KACWorkerGameState.CurrentVessel != null)
            {
                switch (AddType)
                {
                    case KACAlarm.AlarmTypeEnum.Raw:
                        BuildRawStrings();
                        break;
                    case KACAlarm.AlarmTypeEnum.Maneuver:
                        strAlarmNotes = "Time to pay attention to\r\n    " + strAlarmName + "\r\nNearing Maneuver Node";
                        break;
                    case KACAlarm.AlarmTypeEnum.SOIChange:
                        if (KACWorkerGameState.SOIPointExists)
                            strAlarmNotes = "Time to pay attention to\r\n    " + KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + "\r\nNearing SOI Change\r\n" +
                                            "     Old SOI: " + KACWorkerGameState.CurrentVessel.orbit.referenceBody.bodyName + "\r\n" +
                                            "     New SOI: " + KACWorkerGameState.CurrentVessel.orbit.nextPatch.referenceBody.bodyName;
                        break;
                    case KACAlarm.AlarmTypeEnum.Transfer:
                    case KACAlarm.AlarmTypeEnum.TransferModelled:
                        BuildTransferStrings();
                        break;
                    case KACAlarm.AlarmTypeEnum.Apoapsis:
                        strAlarmNotes = "Time to pay attention to\r\n    " + KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + "\r\nNearing Apoapsis";
                        break;
                    case KACAlarm.AlarmTypeEnum.Periapsis:
                        strAlarmNotes = "Time to pay attention to\r\n    " + KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + "\r\nNearing Periapsis";
                        break;
                    case KACAlarm.AlarmTypeEnum.AscendingNode:
                        strAlarmNotes = "Time to pay attention to\r\n    " + KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + "\r\nNearing Ascending Node";
                        break;
                    case KACAlarm.AlarmTypeEnum.DescendingNode:
                        strAlarmNotes = "Time to pay attention to\r\n    " + KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + "\r\nNearing Descending Node";
                        break;
                    case KACAlarm.AlarmTypeEnum.LaunchRendevous:
                        strAlarmNotes = "Time to pay attention to\r\n    " + KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + "\r\nNearing Launch Rendevous";
                        break;
                    case KACAlarm.AlarmTypeEnum.Closest:
                        strAlarmNotes = "Time to pay attention to\r\n    " + KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + "\r\nNearing Closest Approach";
                        break;
                    case KACAlarm.AlarmTypeEnum.Distance:
                        strAlarmNotes = "Time to pay attention to\r\n    " + KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + "\r\nNearing Target Distance";
                        break;
                    case KACAlarm.AlarmTypeEnum.Crew:
                        BuildCrewStrings();
                        CrewAlarmStoreNode = settings.AlarmCrewDefaultStoreNode;
                        break;
                    case KACAlarm.AlarmTypeEnum.Contract:
                    case KACAlarm.AlarmTypeEnum.ContractAuto:
                        //BuildContractStringsAndMargin();
                        break;
                    case KACAlarm.AlarmTypeEnum.ScienceLab:
                        BuildScienceLabStrings();
                        break;
                    default:
                        break;
                }
            }

            //If the type is a contract then theres some margin adjustments to do
            if (AddType == KACAlarm.AlarmTypeEnum.Contract || AddType == KACAlarm.AlarmTypeEnum.ContractAuto)
                BuildContractStringsAndMargin(true);
            else
                timeMargin.BuildFromUT(settings.AlarmDefaultMargin);


            //Change Audio Sound?
        }

        private TransferStrings BuildTransferStrings()
        {
            return BuildTransferStrings(intXferCurrentTarget,true);
        }
        private TransferStrings BuildTransferStrings(Int32 TargetIndex,Boolean SetAddVariables)
        {
            TransferStrings ret = new TransferStrings();
            
            String strWorking = "";
            if (blnAlarmAttachToVessel)
                strWorking = "Time to pay attention to\r\n    " + KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + "\r\nNearing Celestial Transfer:";
            else
                strWorking = "Nearing Celestial Transfer:";

            if (XferTargetBodies != null && TargetIndex < XferTargetBodies.Count)
                strWorking += "\r\n\tOrigin: " + XferTargetBodies[TargetIndex].Origin.bodyName + "\r\n\tTarget: " + XferTargetBodies[TargetIndex].Target.bodyName;
            ret.AlarmNotes = strWorking;

            strWorking = "";
            if (XferTargetBodies != null && TargetIndex < XferTargetBodies.Count)
                strWorking = XferTargetBodies[TargetIndex].Origin.bodyName + "->" + XferTargetBodies[TargetIndex].Target.bodyName;
            else
                strWorking = strWorking = "Nearing Celestial Transfer";
            ret.AlarmName = strWorking;
            
            strAlarmNotes = ret.AlarmNotes;
            strAlarmName = ret.AlarmName;

            return ret;
        }

        private class TransferStrings { internal string AlarmName; internal string AlarmNotes; }

        private void BuildRawStrings()
        {
            String strWorking = "";
            if (blnAlarmAttachToVessel && KACWorkerGameState.CurrentVessel != null)
                strWorking = "Time to pay attention to:\r\n    " + KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + "\r\nRaw Time Alarm";
            else
                strWorking = "Raw Time Alarm";
            strAlarmNotes = strWorking;

            strWorking = "";
            if (blnAlarmAttachToVessel && KACWorkerGameState.CurrentVessel != null)
                strWorking = KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName);
            else
                strWorking = "Raw Time Alarm";
            strAlarmName = strWorking;
        }

        private void BuildCrewStrings()
        {
            strAlarmEventName = "Crew";
            List<ProtoCrewMember> pCM = null;
            if (KACWorkerGameState.CurrentVessel != null)
                pCM = KACWorkerGameState.CurrentVessel.GetVesselCrew();
            if (pCM != null && pCM.Count == 0)
            {
                strAlarmName = "Crew member alarm";
                strAlarmNotes = "No Kerbals present in this vessel";
            } else {
                strAlarmName = pCM[intSelectedCrew].displayName;
                strAlarmNotes = string.Format("Alarm for {0}\r\nNot tied to any vessel - will follow the Kerbal", pCM[intSelectedCrew].name);
            }
        }

        private void BuildContractStringsAndMargin(Boolean ForceUpdateMargin=false)
        {
            strAlarmEventName = "Contract";
            if (ContractSystem.Instance == null || lstContracts.Count == 0)
            {
                strAlarmName = "Contract alarm";
                strAlarmNotes = "No Contracts Available";
            }
            else
            {
                GenerateContractStringsFromContract(lstContracts[intSelectedContract], out strAlarmName, out strAlarmNotes);


                if (ForceUpdateMargin || contractLastState != lstContracts[intSelectedContract].ContractState)
                {
                    if (lstContracts[intSelectedContract].ContractState == Contract.State.Active)
                        timeMargin.BuildFromUT(settings.AlarmOnContractDeadlineMargin);
                    else
                        timeMargin.BuildFromUT(settings.AlarmOnContractExpireMargin);

                    contractLastState = lstContracts[intSelectedContract].ContractState;
                }
            }
        }

        private void GenerateContractStringsFromContract(Contract c, out String AlarmName, out String AlarmNotes)
        {
            AlarmName = c.Title;
            AlarmNotes = String.Format("{0}\r\nName: {1}\r\nParameters:", c.AlarmType().Description(), c.Synopsys);
            foreach (ContractParameter cp in c.AllParameters)
            {
                AlarmNotes += String.Format("\r\n    * {0}", cp.Title);
            }
        }

        private void BuildScienceLabStrings()
        {
            string strVesselName = KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName);
            if (intSelectedScienceLab >= 0)
            {
                strAlarmName = strVesselName + " - Science Lab " + (intSelectedScienceLab + 1);
            }
            else
            {
                strAlarmName = strVesselName + " - Science Lab alarm";
            }
            strAlarmNotes = string.Format("Time to pay attention to\r\n    {0}\r\nNearing {1} science in Science Lab", strVesselName, intTargetScienceClamped);
        }

        //String[] strAddTypes = new String[] { "Raw", "Maneuver","SOI","Transfer" };
        private String[] strAddTypes = new String[] { "R", "M", "A", "P", "A", "D", "S", "X" };

        private GUIContent[] guiTypes = new GUIContent[]
            {
                new GUIContent(KACResources.btnRaw,"Raw Time Alarm"),
                new GUIContent(KACResources.btnMNode,"Maneuver Node"),
                new GUIContent(KACResources.btnApPe,"Apoapsis / Periapsis"),
                //new GUIContent(KACResources.btnAp,"Apoapsis"),
                //new GUIContent(KACResources.btnPe,"Periapsis"),
                new GUIContent(KACResources.btnANDN,"Ascending/Descending Node"),
                //new GUIContent(KACResources.btnAN,"Ascending Node"),
                //new GUIContent(KACResources.btnDN,"Descending Node"),
                new GUIContent(KACResources.btnClosest,"Closest/Target Distance"),
                new GUIContent(KACResources.btnSOI,"SOI Change"),
                new GUIContent(KACResources.btnXfer,"Transfer Window"),
                new GUIContent(KACResources.btnCrew,"Kerbal Crew"),
                new GUIContent(KACResources.btnContract,"Contract"),
                new GUIContent(KACResources.btnScienceLab,"Science Lab")
            };

        private GUIContent[] guiTypesView = new GUIContent[]
            {
                new GUIContent(KACResources.btnRaw,"Raw Time Alarm")
            };

        private GUIContent[] guiTypesSpaceCenter = new GUIContent[]
            {
                new GUIContent(KACResources.btnRaw,"Raw Time Alarm"),
                new GUIContent(KACResources.btnXfer,"Transfer Window"),
                new GUIContent(KACResources.btnContract,"Contract")
            };
        private GUIContent[] guiTypesTrackingStation = new GUIContent[]
            {
                new GUIContent(KACResources.btnRaw,"Raw Time Alarm"),
                new GUIContent(KACResources.btnMNode,"Maneuver Node"),
                new GUIContent(KACResources.btnApPe,"Apoapsis / Periapsis"),
                new GUIContent(KACResources.btnSOI,"SOI Change"),
                new GUIContent(KACResources.btnXfer,"Transfer Window"),
                new GUIContent(KACResources.btnCrew,"Kerbal Crew"),
                new GUIContent(KACResources.btnContract,"Contract")
            };
        private GUIContent[] guiTypesEditor = new GUIContent[]
            {
                new GUIContent(KACResources.btnRaw,"Raw Time Alarm"),
                new GUIContent(KACResources.btnXfer,"Transfer Window"),
                new GUIContent(KACResources.btnContract,"Contract")
            };

        private GameScenes[] ScenesForAttachOption = new GameScenes[] 
            { 
                GameScenes.FLIGHT, 
                GameScenes.TRACKSTATION, 
            };

        private KACAlarm.AlarmTypeEnum[] TypesForAttachOption = new KACAlarm.AlarmTypeEnum[] 
            { 
                KACAlarm.AlarmTypeEnum.Raw, 
                KACAlarm.AlarmTypeEnum.Transfer, 
                KACAlarm.AlarmTypeEnum.TransferModelled,
                KACAlarm.AlarmTypeEnum.Contract, 
                KACAlarm.AlarmTypeEnum.ContractAuto
            };

        private KACAlarm.AlarmTypeEnum[] TypesWithNoEvent = new KACAlarm.AlarmTypeEnum[] 
            { 
                KACAlarm.AlarmTypeEnum.Raw, 
                KACAlarm.AlarmTypeEnum.Transfer, 
                KACAlarm.AlarmTypeEnum.TransferModelled 
            };

        private int intHeight_AddWindowCommon;
        private int intHeight_AddWindowRepeat;
        private int intHeight_AddWindowKER;
        /// <summary>
        /// Draw the Add Window contents
        /// </summary>
        /// <param name="WindowID"></param>
        internal void FillAddWindow(int WindowID)
        {
            GUILayout.BeginVertical();

            //AddType =  (KACAlarm.AlarmType)GUILayout.Toolbar((int)AddType, strAddTypes,KACResources.styleButton);
            GUIContent[] guiButtons = guiTypes;
            //if (ViewAlarmsOnly) guiButtons = guiTypesView;
            switch (MonoName)
            {
                case "KACSpaceCenter":
                    guiButtons = guiTypesSpaceCenter; break;
                case "KACTrackingStation":
                    guiButtons = guiTypesTrackingStation; break;
                case "KACEditor":
                    guiButtons = guiTypesEditor; break;
                default:
                    break;
            }
            if (DrawButtonList(ref AddType,guiButtons))
            {
                //if the choice was the Ap/Pe one then work out the best next choice
                if(AddType== KACAlarm.AlarmTypeEnum.Apoapsis) {
                    
                    if(!KACWorkerGameState.ApPointExists && KACWorkerGameState.PePointExists)
                        AddType = KACAlarm.AlarmTypeEnum.Periapsis;
                    else if(KACWorkerGameState.ApPointExists && KACWorkerGameState.PePointExists &&
                            ((KACWorkerGameState.CurrentVessel == null) ? 0 : KACWorkerGameState.CurrentVessel.orbit.timeToAp) > ((KACWorkerGameState.CurrentVessel == null ) ? 0 : KACWorkerGameState.CurrentVessel.orbit.timeToPe))
                        AddType = KACAlarm.AlarmTypeEnum.Periapsis;
                }
                AddTypeChanged();
            }
            
            //if (AddType == KACAlarm.AlarmType.Apoapsis || AddType == KACAlarm.AlarmType.Periapsis)
            //    WindowLayout_AddTypeApPe();
            //if (AddType == KACAlarm.AlarmType.AscendingNode || AddType == KACAlarm.AlarmType.DescendingNode)
            //    WindowLayout_AddTypeANDN();

            //calc height for common stuff
            intHeight_AddWindowCommon = 71;
            if (AddType != KACAlarm.AlarmTypeEnum.Raw && AddType!= KACAlarm.AlarmTypeEnum.Crew && AddType != KACAlarm.AlarmTypeEnum.ScienceLab) //add stuff for margins
                intHeight_AddWindowCommon += 28;
            if (ScenesForAttachOption.Contains(KACWorkerGameState.CurrentGUIScene) && TypesForAttachOption.Contains(AddType) && KACWorkerGameState.CurrentVessel != null) //add stuff for attach to ship
                    intHeight_AddWindowCommon += 30;
            if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION)
                intHeight_AddWindowCommon += 18;

            //layout the right fields for the common components
            Boolean blnAttachPre = blnAlarmAttachToVessel;
            //WindowLayout_CommonFields2(ref strAlarmName, ref blnAlarmAttachToVessel, ref AddAction, ref timeMargin, AddType, intHeight_AddWindowCommon);
            WindowLayout_CommonFields3(ref strAlarmName, ref blnAlarmAttachToVessel, ref AddActions, ref timeMargin, AddType, intHeight_AddWindowCommon);
 
            Double dblTimeToPoint = 0;

            //layout the specific pieces for each type of alarm
            switch (AddType)
            {
                case KACAlarm.AlarmTypeEnum.Raw:
                    if (blnAttachPre!=blnAlarmAttachToVessel) BuildRawStrings();
                    WindowLayout_AddPane_Raw();
                    break;
                case KACAlarm.AlarmTypeEnum.Maneuver:
                    WindowLayout_AddPane_Maneuver();
                    break;
                case KACAlarm.AlarmTypeEnum.SOIChange:
                    dblTimeToPoint = (KACWorkerGameState.CurrentVessel==null) ? 0 : KACWorkerGameState.CurrentVessel.orbit.UTsoi - KACWorkerGameState.CurrentTime.UT;
                    WindowLayout_AddPane_NodeEvent(KACWorkerGameState.SOIPointExists,dblTimeToPoint);
                    //WindowLayout_AddPane_SOI2();
                    break;
                case KACAlarm.AlarmTypeEnum.Transfer:
                case KACAlarm.AlarmTypeEnum.TransferModelled:
                    if (blnAttachPre != blnAlarmAttachToVessel) BuildTransferStrings();
                    WindowLayout_AddPane_Transfer();
                    break;
                case KACAlarm.AlarmTypeEnum.Apoapsis:
                    WindowLayout_AddTypeApPe();

                    if (!KACWorkerGameState.ApPointExists && HighLogic.CurrentGame.Mode == Game.Modes.CAREER &&
                        GameVariables.Instance.GetOrbitDisplayMode(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.TrackingStation)) != GameVariables.OrbitDisplayMode.PatchedConics)
                    {
                        GUILayout.Label("Apoapsis Alarms cannot be set without upgrading the Tracking Station", GUILayout.ExpandWidth(true));
                    }
                    else
                    {
                        dblTimeToPoint = (KACWorkerGameState.CurrentVessel == null) ? 0 : KACWorkerGameState.CurrentVessel.orbit.timeToAp;
                        WindowLayout_AddPane_NodeEvent(KACWorkerGameState.ApPointExists && !KACWorkerGameState.CurrentVessel.LandedOrSplashed, dblTimeToPoint);
                    }
                    break;
                case KACAlarm.AlarmTypeEnum.Periapsis:
                    WindowLayout_AddTypeApPe();
                    if (!KACWorkerGameState.PePointExists && HighLogic.CurrentGame.Mode == Game.Modes.CAREER &&
                        GameVariables.Instance.GetOrbitDisplayMode(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.TrackingStation)) != GameVariables.OrbitDisplayMode.PatchedConics)
                    {
                        GUILayout.Label("Periapsis Alarms cannot be set without upgrading the Tracking Station", GUILayout.ExpandWidth(true));
                    }
                    else
                    {
                    dblTimeToPoint = (KACWorkerGameState.CurrentVessel == null ) ? 0 : KACWorkerGameState.CurrentVessel.orbit.timeToPe;
                    WindowLayout_AddPane_NodeEvent(KACWorkerGameState.PePointExists && !KACWorkerGameState.CurrentVessel.LandedOrSplashed, dblTimeToPoint);
                    }
                    break;
                case KACAlarm.AlarmTypeEnum.AscendingNode:
                    WindowLayout_AddTypeANDN();
                    if (KACWorkerGameState.CurrentVesselTarget==null)
                    {
                        WindowLayout_AddPane_AscendingNodeEquatorial();
                    }
                    else if (KACWorkerGameState.CurrentVessel.orbit.referenceBody == KACWorkerGameState.CurrentVesselTarget.GetOrbit().referenceBody) {
                        //Must be orbiting Same parent body for this to make sense
                        WindowLayout_AddPane_AscendingNode();
                    }
                    else
                    {
                        GUILayout.Label("Target Not Currently Orbiting Same Parent", KACResources.styleAddXferName, GUILayout.Height(18));
                        GUILayout.Label("", KACResources.styleAddXferName, GUILayout.Height(18));
                        GUILayout.Label("There is no Ascending Node between orbits", KACResources.styleAddXferName, GUILayout.Height(18));
                    }
                    break;
                case KACAlarm.AlarmTypeEnum.DescendingNode:
                    WindowLayout_AddTypeANDN();
                    if (KACWorkerGameState.CurrentVesselTarget==null)
                    {
                        WindowLayout_AddPane_DescendingNodeEquatorial();
                    }
                    else if (KACWorkerGameState.CurrentVessel.orbit.referenceBody == KACWorkerGameState.CurrentVesselTarget.GetOrbit().referenceBody)
                    {
                        //Must be orbiting Same parent body for this to make sense
                        WindowLayout_AddPane_DescendingNode();
                    }
                    else
                    {
                        GUILayout.Label("Target Not Currently Orbiting Same Parent", KACResources.styleAddXferName, GUILayout.Height(18));
                        GUILayout.Label("", KACResources.styleAddXferName, GUILayout.Height(18));
                        GUILayout.Label("There is no Ascending Node between orbits", KACResources.styleAddXferName, GUILayout.Height(18));
                    }
                    break;
                case KACAlarm.AlarmTypeEnum.LaunchRendevous:
                    WindowLayout_AddTypeANDN();
                    if (KACWorkerGameState.CurrentVessel.orbit.referenceBody == KACWorkerGameState.CurrentVesselTarget.GetOrbit().referenceBody)
                    {
                        //Must be orbiting Same parent body for this to make sense
                        WindowLayout_AddPane_LaunchRendevous();
                    }
                    else
                    {
                        GUILayout.Label("Target Not Currently Orbiting Same Parent", KACResources.styleAddXferName, GUILayout.Height(18));
                        GUILayout.Label("", KACResources.styleAddXferName, GUILayout.Height(18));
                        GUILayout.Label("There is no Ascending Node between orbits", KACResources.styleAddXferName, GUILayout.Height(18));
                    }

                    break;
                case KACAlarm.AlarmTypeEnum.Closest:
                    WindowLayout_AddTypeDistanceChoice();
                    WindowLayout_AddPane_ClosestApproach();
                    break;
                case KACAlarm.AlarmTypeEnum.Distance:
                    WindowLayout_AddTypeDistanceChoice();
                    WindowLayout_AddPane_TargetDistance();
                    break;
                case KACAlarm.AlarmTypeEnum.Crew:
                    WindowLayout_AddPane_Crew();
                    break;
                case KACAlarm.AlarmTypeEnum.Contract:
                case KACAlarm.AlarmTypeEnum.ContractAuto:
                    WindowLayout_AddPane_Contract();
                    break;
                case KACAlarm.AlarmTypeEnum.ScienceLab:
                    WindowLayout_AddPane_ScienceLab();
                    break;
                default:
                    break;
            }

            GUILayout.EndVertical();
            
            SetTooltipText();
        }

        internal void WindowLayout_AddTypeApPe()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("apsis Type:", KACResources.styleAddHeading);
            int intOption = 0;
            if (AddType != KACAlarm.AlarmTypeEnum.Apoapsis) intOption = 1;
            if (DrawRadioList(ref intOption, "Apoapsis", "Periapsis"))
            {
                if (intOption == 0)
                    AddType = KACAlarm.AlarmTypeEnum.Apoapsis;
                else
                {
                    AddType = KACAlarm.AlarmTypeEnum.Periapsis;
                }
                AddTypeChanged();

            }

            GUILayout.EndHorizontal();
        }


        ////Variabled for Raw Alarm screen
        //String strYears = "0", strDays = "0", strHours = "0", strMinutes = "0",
        private String strRawUT="0";
        private KSPDateTime rawTime = new KSPDateTime(600);
        private KSPTimeSpan rawTimeToAlarm = new KSPTimeSpan(0);
        //Boolean blnRawDate = false;
        //Boolean blnRawInterval = true;
        ///// <summary>
        ///// Layout the raw alarm screen inputs
        ///// </summary>
        private Int32 intRawType = 1;
        internal KACTimeStringArray rawEntry = new KACTimeStringArray(600, KACTimeStringArray.TimeEntryPrecisionEnum.Years);
        private void WindowLayout_AddPane_Raw()
        {
            GUILayout.Label("Enter Raw Time Values...", KACResources.styleAddSectionHeading);

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Time type:", KACResources.styleAddHeading, GUILayout.Width(90));
            if (DrawRadioList(ref intRawType, new string[] { "Date", "Time Interval" }))
            {
                if (intRawType == 0)
                {
                    rawEntry = new KACTimeStringArray(Planetarium.GetUniversalTime() + 600, KACTimeStringArray.TimeEntryPrecisionEnum.Years);
                }
            }
            GUILayout.EndHorizontal();

            if (intRawType == 0)
            {
                //date
                KACTimeStringArray rawDate = new KACTimeStringArray(rawEntry.UT + KSPDateStructure.EpochAsKSPDateTime.UT, KACTimeStringArray.TimeEntryPrecisionEnum.Years);
                if (DrawTimeEntry(ref rawDate, KACTimeStringArray.TimeEntryPrecisionEnum.Years, "Time:", 50, 35, 15))
                {
                    rawEntry.BuildFromUT(rawDate.UT - KSPDateStructure.EpochAsKSPDateTime.UT);
                }
            }
            else
            {
                //interval
                if (DrawTimeEntry(ref rawEntry, KACTimeStringArray.TimeEntryPrecisionEnum.Years, "Time:", 50, 35, 15))
                {

                }
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("UT (raw seconds):", KACResources.styleAddHeading,GUILayout.Width(100));
            strRawUT = GUILayout.TextField(strRawUT, KACResources.styleAddField);
            GUILayout.EndHorizontal();


            GUILayout.EndVertical();
            try
            {
                if (strRawUT != "")
                    rawTime.UT = Convert.ToDouble(strRawUT);
                else
                    rawTime.UT = rawEntry.UT;
        
                //If its an interval add the interval to the current time
                if (intRawType==1)
                    rawTime = new KSPDateTime(KACWorkerGameState.CurrentTime.UT + rawTime.UT);

                rawTimeToAlarm = new KSPTimeSpan(rawTime.UT - KACWorkerGameState.CurrentTime.UT);

                //Draw the Add Alarm details at the bottom
                if (DrawAddAlarm(rawTime,null,rawTimeToAlarm))
                {
                    //"VesselID, Name, Message, AlarmTime.UT, Type, Enabled,  HaltWarp, PauseGame, Maneuver"
                    String strVesselID = "";
                    if (KACWorkerGameState.CurrentVessel != null && blnAlarmAttachToVessel) strVesselID = KACWorkerGameState.CurrentVessel.id.ToString();
                    KACAlarm alarmNew = new KACAlarm(strVesselID, strAlarmName, (blnRepeatingAlarmFlag ? "Alarm Repeats\r\n" : "") + strAlarmNotes, rawTime.UT, 0, KACAlarm.AlarmTypeEnum.Raw,
                        AddActions);
                    alarmNew.RepeatAlarm = blnRepeatingAlarmFlag;
                    alarmNew.RepeatAlarmPeriod = new KSPTimeSpan(timeRepeatPeriod.UT);
                    alarms.Add(alarmNew);

                    //settings.Save();
                    _ShowAddPane = false;
                }
            }
            catch (Exception ex)
            {
                GUILayout.Label("Unable to combine all text fields to date", GUILayout.ExpandWidth(true));
                LogFormatted_DebugOnly("{0}\r\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private int intSelectedCrew = 0;
        //Do we do this in with the Raw alarm as we are gonna ask for almost the same stuff
        private String strCrewUT = "0";
        private KSPDateTime CrewTime = new KSPDateTime(600);
        private KSPTimeSpan CrewTimeToAlarm = new KSPTimeSpan(0);
        private Int32 intCrewType = 1;
        private KACTimeStringArray CrewEntry = new KACTimeStringArray(600, KACTimeStringArray.TimeEntryPrecisionEnum.Years);
        private Boolean CrewAlarmStoreNode = false;
        private  Int32 intAddCrewHeight = 322;
        private void WindowLayout_AddPane_Crew()
        {
            intAddCrewHeight = 304;// 322;
            GUILayout.Label("Select Crew...", KACResources.styleAddSectionHeading);
            if (KACWorkerGameState.CurrentVessel == null)
            {
                GUILayout.Label("No Active Vessel");
            }
            else
            {
                GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

                //get the kerbals in the current vessel
                List<ProtoCrewMember> pCM = KACWorkerGameState.CurrentVessel.GetVesselCrew();
                intAddCrewHeight += (pCM.Count * 30);
                if (pCM.Count == 0)
                {
                    //Draw something about no crew present
                    GUILayout.Label("No Kerbals present in this vessel", KACResources.styleContent, GUILayout.ExpandWidth(true));
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Kerbal Name", KACResources.styleAddSectionHeading, GUILayout.Width(267));
                    GUILayout.Label("Add", KACResources.styleAddSectionHeading);//, GUILayout.Width(30));
                    GUILayout.EndHorizontal();

                    for (int intTarget = 0; intTarget < pCM.Count; intTarget++)
                    {
                        //LogFormatted("{2}", pCM[intTarget].name);
                        GUILayout.BeginHorizontal();
                        //        //draw a line and a radio button for selecting Crew
                        GUILayout.Space(20);
                        GUILayout.Label(pCM[intTarget].name, KACResources.styleAddXferName, GUILayout.Width(240), GUILayout.Height(20));

                        //        //when they are selected adjust message to have a name of the crew member, and message of vessel when alarm was set
                        Boolean blnSelected = (intSelectedCrew == intTarget);
                        if (DrawToggle(ref blnSelected, "", KACResources.styleCheckbox, GUILayout.Width(40)))
                        {
                            if (blnSelected)
                            {
                                intSelectedCrew = intTarget;
                                BuildCrewStrings();
                            }
                        }
                        GUILayout.EndHorizontal();
                    }

                    DrawCheckbox(ref CrewAlarmStoreNode, "Store Man Node/Target with Crew Alarm");

                }
                GUILayout.EndVertical();

                if (pCM.Count > 0)
                {
                    //Now the time entry area
                    GUILayout.Label("Enter Time Values...", KACResources.styleAddSectionHeading);

                    GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Time type:", KACResources.styleAddHeading, GUILayout.Width(90));
                    if (DrawRadioList(ref intCrewType, new string[] { "Date", "Time Interval" }))
                    {
                        if (intRawType == 0)
                        {
                            rawEntry = new KACTimeStringArray(Planetarium.GetUniversalTime() + 600, KACTimeStringArray.TimeEntryPrecisionEnum.Years);
                        }
                    }
                    GUILayout.EndHorizontal();

                    if (intCrewType == 0)
                    {
                        //date
                        KACTimeStringArray CrewDate = new KACTimeStringArray(CrewEntry.UT + KSPDateStructure.EpochAsKSPDateTime.UT, KACTimeStringArray.TimeEntryPrecisionEnum.Years);
                        if (DrawTimeEntry(ref CrewDate, KACTimeStringArray.TimeEntryPrecisionEnum.Years, "Time:", 50, 35, 15))
                        {
                            rawEntry.BuildFromUT(CrewDate.UT - KSPDateStructure.EpochAsKSPDateTime.UT);
                        }
                    }
                    else
                    {
                        //interval
                        if (DrawTimeEntry(ref CrewEntry, KACTimeStringArray.TimeEntryPrecisionEnum.Years, "Time:", 50, 35, 15))
                        {

                        }
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("UT (raw seconds):", KACResources.styleAddHeading, GUILayout.Width(100));
                    strCrewUT = GUILayout.TextField(strCrewUT, KACResources.styleAddField);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();

                    try
                    {
                        if (strCrewUT != "")
                            CrewTime.UT = Convert.ToDouble(strCrewUT);
                        else
                            CrewTime.UT = CrewEntry.UT;

                        //If its an interval add the interval to the current time
                        if (intCrewType == 1)
                            CrewTime = new KSPDateTime(KACWorkerGameState.CurrentTime.UT + CrewTime.UT);

                        CrewTimeToAlarm = new KSPTimeSpan(CrewTime.UT - KACWorkerGameState.CurrentTime.UT);

                        //Draw the Add Alarm details at the bottom
                        if (DrawAddAlarm(CrewTime, null, CrewTimeToAlarm))
                        {
                            //"VesselID, Name, Message, AlarmTime.UT, Type, Enabled,  HaltWarp, PauseGame, Maneuver"
                            KACAlarm addAlarm = new KACAlarm(pCM[intSelectedCrew].name, strAlarmName, (blnRepeatingAlarmFlag ? "Alarm Repeats\r\n" : "") + strAlarmNotes, CrewTime.UT, 0, KACAlarm.AlarmTypeEnum.Crew,
                                AddActions);
                            if (CrewAlarmStoreNode)
                            {
                                if (KACWorkerGameState.ManeuverNodeExists) addAlarm.ManNodes = KACWorkerGameState.ManeuverNodesFuture;
                                if (KACWorkerGameState.CurrentVesselTarget != null) addAlarm.TargetObject = KACWorkerGameState.CurrentVesselTarget;
                            }
                            addAlarm.RepeatAlarm = blnRepeatingAlarmFlag;
                            addAlarm.RepeatAlarmPeriod = new KSPTimeSpan(timeRepeatPeriod.UT);

                            alarms.Add(addAlarm);
                            //settings.Save();
                            _ShowAddPane = false;
                        }
                    }
                    catch (Exception)
                    {
                        //    LogFormatted(ex.Message);
                        GUILayout.Label("Unable to combine all text fields to date", GUILayout.ExpandWidth(true));
                    }
                }
            }
        }

        Vector2 scrollContract = new Vector2(0, 0);
        Int32 intSelectedContract = 0;
        private KSPDateTime ContractTime = new KSPDateTime(600);
        private KSPTimeSpan ContractTimeToEvent = new KSPTimeSpan(0);
        private KSPTimeSpan ContractTimeToAlarm = new KSPTimeSpan(0);

        internal List<Contract> lstContracts;
        internal Contract.State contractLastState = Contract.State.Offered;
        
        private void WindowLayout_AddPane_Contract()
        {
            GUILayout.Label("Select Contract...", KACResources.styleAddSectionHeading);
            if (Contracts.ContractSystem.Instance == null)
            {
                GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
                GUILayout.Label("Jebediah digs around in the Sandbox, but can't\nfind any contracts in this game mode", KACResources.styleContent);
                GUILayout.EndVertical();
            }
            else
            {

                if (lstContracts.Count == 0)
                {
                    GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
                    GUILayout.Label("No Contracts Offered or Active", KACResources.styleContent);
                    GUILayout.EndVertical();
                }
                else
                {

                    scrollContract = GUILayout.BeginScrollView(scrollContract, KACResources.styleAddFieldAreas);

                    //If the selected contract is already an alarm then move the selected one
                    if (intSelectedContract == -1  || alarms.Any(a => a.ContractGUID == lstContracts[intSelectedContract].ContractGuid)){
                        intSelectedContract = -1;
                        for (int i = 0; i < lstContracts.Count; i++) {
                            if(!alarms.Any(a => a.ContractGUID == lstContracts[i].ContractGuid)){
                                intSelectedContract = i;
                                BuildContractStringsAndMargin();
                                break;
                            }
                        }
                    }
                    
                    //Loop through the contracts to draw the lines
                    for (Int32 intTarget = 0; intTarget < lstContracts.Count; intTarget++)
                    {
                        Contract c = lstContracts[intTarget];
                        Boolean AlarmExists = alarms.Any(a => a.ContractGUID == c.ContractGuid);

                        GUILayout.BeginHorizontal();
                        //Appropriate icon
                        GUILayout.Space(5);
                        if (AlarmExists) {
                            GUILayout.Label(new GUIContent(KACResources.iconRaw, "Alarm Exists"), GUILayout.Width(20), GUILayout.Height(25));
                        } else if (c.ContractState == Contract.State.Active) {
                            GUILayout.Label(new GUIContent(KACResources.iconContract, "Active"), GUILayout.Width(20), GUILayout.Height(25));
                        } else {
                            GUILayout.Space(24);
                        }

                        //What style should the name be
                        GUIStyle styleContLabel = KACResources.styleContractLabelOffer;
                        if (c.ContractState == Contract.State.Active)
                            styleContLabel = KACResources.styleContractLabelActive;
                        else if (AlarmExists)
                            styleContLabel = KACResources.styleContractLabelAlarmExists;

                        if (GUILayout.Button(c.Title, styleContLabel, GUILayout.Width(243)))
                        {
                            if (!AlarmExists)
                            {
                                intSelectedContract = intTarget;
                                BuildContractStringsAndMargin();
                            }
                        };

                        //Is the check box on?
                        Boolean blnSelected = (intSelectedContract == intTarget);

                        if (!AlarmExists)
                        {
                            if (DrawToggle(ref blnSelected, "", KACResources.styleCheckbox, GUILayout.Width(20)))
                            {
                                if (blnSelected)
                                {
                                    intSelectedContract = intTarget;
                                    BuildContractStringsAndMargin();
                                }
                            }
                        } GUILayout.EndHorizontal();
                    }
                    GUILayout.EndScrollView();

                    if (intSelectedContract < 0) {
                        GUILayout.Label("All Contracts Have alarms set already", KACResources.styleContent);
                    } else { 
                        //Draw the Add Alarm details at the bottom
                        //If its an interval add the interval to the current time
                        ContractTime = new KSPDateTime(lstContracts[intSelectedContract].DateNext());
                        ContractTimeToEvent = new KSPTimeSpan(lstContracts[intSelectedContract].DateNext() - KACWorkerGameState.CurrentTime.UT);
                        ContractTimeToAlarm = new KSPTimeSpan(ContractTimeToEvent.UT - timeMargin.UT);

                        if (DrawAddAlarm(ContractTime, ContractTimeToEvent, ContractTimeToAlarm))
                        {
                            //"VesselID, Name, Message, AlarmTime.UT, Type, Enabled,  HaltWarp, PauseGame, Maneuver"
                            String strVesselID = "";
                            if (KACWorkerGameState.CurrentVessel != null && blnAlarmAttachToVessel) strVesselID = KACWorkerGameState.CurrentVessel.id.ToString();
                            KACAlarm tmpAlarm = new KACAlarm(strVesselID, strAlarmName, strAlarmNotes, KACWorkerGameState.CurrentTime.UT + ContractTimeToAlarm.UT,
                                timeMargin.UT, KACAlarm.AlarmTypeEnum.Contract, AddActions);

                            tmpAlarm.ContractGUID = lstContracts[intSelectedContract].ContractGuid;
                            tmpAlarm.ContractAlarmType = lstContracts[intSelectedContract].AlarmType();

                            alarms.Add(tmpAlarm);
                            //settings.Save();
                            _ShowAddPane = false;
                        }
                    }
                }
            }
            
        }

        private int intSelectedScienceLab = 0;
        private string strTargetScience = "500";
        private int intAddScienceLabHeight = 150;
        private Part highlightedScienceLab;
        private bool blnClearScienceLabHighlight;
        private int intTargetScienceClamped = 500;
        private void WindowLayout_AddPane_ScienceLab()
        {
            intAddScienceLabHeight = 150;
            if (KACWorkerGameState.CurrentVessel == null)
            {
                GUILayout.Label("No Active Vessel.", KACResources.styleLabelWarning);
            }
            else
            {
                GUILayout.Label("Select Science Lab...", KACResources.styleAddSectionHeading);
                GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
                var lstScienceLabs = KACWorkerGameState.CurrentVessel.FindPartModulesImplementing<ModuleScienceLab>();
                if (lstScienceLabs.Count == 0)
                {
                    GUILayout.Label("No Science Labs on Active Vessel.", KACResources.styleLabelWarning);
                }
                else
                {
                    intAddScienceLabHeight += (lstScienceLabs.Count * 30);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label("Science Lab", KACResources.styleAddSectionHeading, GUILayout.Width(240));
                    GUILayout.Label("Select", KACResources.styleAddSectionHeading);
                    GUILayout.EndHorizontal();

                    for (var i = 0; i < lstScienceLabs.Count; ++i)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        GUILayout.Label(
                            string.Format(
                                "Science Lab {0} (Science: {1:0}, Data: {2:0})",
                                i + 1,
                                lstScienceLabs[i].storedScience,
                                lstScienceLabs[i].dataStored),
                            KACResources.styleAddXferName,
                            GUILayout.Width(240),
                            GUILayout.Height(20));

                        bool blnSelected = (intSelectedScienceLab == i);
                        if (DrawToggle(ref blnSelected, string.Empty, KACResources.styleCheckbox, GUILayout.Width(40)))
                        {
                            if (blnSelected)
                            {
                                intSelectedScienceLab = i;
                                BuildScienceLabStrings();
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();

                if (intSelectedScienceLab >= 0 && intSelectedScienceLab < lstScienceLabs.Count)
                {
                    blnClearScienceLabHighlight = false;
                    var partToHighlight = lstScienceLabs[intSelectedScienceLab].part;
                    if (partToHighlight != highlightedScienceLab && highlightedScienceLab != null && highlightedScienceLab.HighlightActive)
                    {
                        highlightedScienceLab.SetHighlightDefault();
                    }

                    if (!partToHighlight.HighlightActive)
                    {
                        partToHighlight.SetHighlight(true, false);
                    }

                    partToHighlight.highlightType = Part.HighlightType.AlwaysOn;
                    partToHighlight.SetHighlightColor(Color.yellow);
                    highlightedScienceLab = partToHighlight;

                    var lab = lstScienceLabs[intSelectedScienceLab];
                    var converter = lab.Converter;
                    if (!converter.IsActivated)
                    {
                        GUILayout.Label("Science Lab is not Active.", KACResources.styleLabelWarning);
                    }
                    else if (Mathf.Approximately(lab.dataStored, 0f))
                    {
                        GUILayout.Label("Science Lab has no Data.", KACResources.styleLabelWarning);
                    }
                    else if (!lab.part.protoModuleCrew.Any(c => c.trait == "Scientist"))
                    {
                        GUILayout.Label("Science Lab has no Scientists.", KACResources.styleLabelWarning);
                    }
                    else if (Mathf.Approximately(lab.storedScience, converter.scienceCap))
                    {
                        GUILayout.Label("Science Lab is already full.", KACResources.styleLabelWarning);
                    }
                    else
                    {
                        intAddScienceLabHeight += 232;
                        var fltMaxScience = Math.Min(converter.scienceCap, lab.storedScience + (lab.dataStored * converter.scienceMultiplier));
                        var intMinScience = (int)Math.Floor(lab.storedScience) + 1;
                        var intMaxScience = (int)Math.Floor(fltMaxScience);

                        GUILayout.Label("Choose Target Science Amount...", KACResources.styleAddSectionHeading);
                        GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Target Science:", KACResources.styleAddXferName);
                        DrawTextField(ref strTargetScience, "[^\\d\\.]+", true);
                        int intTargetScience;
                        if (!int.TryParse(strTargetScience, out intTargetScience))
                        {
                            intTargetScience = intMaxScience;
                            GUILayout.Label(new GUIContent("*", "Invalid fields treated as Max"), KACResources.styleLabelError, GUILayout.Width(8));
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Label(string.Format("Min Target Science: {0}", intMinScience), GUILayout.Width(160));
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(string.Format("Max Target Science: {0}", intMaxScience), GUILayout.Width(160));
                        if (GUILayout.Button("Set to Max"))
                        {
                            strTargetScience = intMaxScience.ToString();
                        }
                        GUILayout.EndHorizontal();
                        intTargetScienceClamped =
                            Math.Max((int)Math.Floor(lab.storedScience) + 1, Math.Min((int)Math.Floor(fltMaxScience), intTargetScience));
                        BuildScienceLabStrings();
                        if (intTargetScience != intTargetScienceClamped)
                        {
                            intAddScienceLabHeight += 25;
                            GUILayout.Label("Selected Target is out of reach. Target Amount changed.", KACResources.styleLabelWarning);
                        }
                        GUILayout.EndVertical();

                        var fltScienceNeeded = intTargetScienceClamped - lab.storedScience;
                        var fltDataNeeded = fltScienceNeeded / converter.scienceMultiplier;
                        var fltFinalData = lab.dataStored - fltDataNeeded;
                        var dblRateStart = converter.CalculateScienceRate(lab.dataStored);
                        var dblRateEnd = converter.CalculateScienceRate(fltFinalData);
                        var dblRateAvg = (dblRateStart + dblRateEnd) * 0.5d;
                        var dblDaysToProcess = fltScienceNeeded / dblRateAvg;
                        var totalResearchTime = KSPTimeSpan.FromDays(dblDaysToProcess);
                        // var totalResearchTime = new KSPTimeSpan(converter.CalculateResearchTime(fltDataNeeded));

                        GUILayout.Label("Target Details...", KACResources.styleAddSectionHeading);
                        GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Target Science Amount:", KACResources.styleAddHeading, GUILayout.Width(150));
                        GUILayout.Label(intTargetScienceClamped.ToString() + "/" + converter.scienceCap, KACResources.styleAddXferName);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Final Data Amount:", KACResources.styleAddHeading, GUILayout.Width(150));
                        GUILayout.Label(fltFinalData.ToString("0.00") + "/" + lab.dataStorage.ToString("0"), KACResources.styleAddXferName);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Final Data Rate:", KACResources.styleAddHeading, GUILayout.Width(150));
                        GUILayout.Label(dblRateEnd.ToString("0.00") + " sci/day", KACResources.styleAddXferName);
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();

                        var scienceLabTime = new KSPDateTime(KACWorkerGameState.CurrentTime.UT + totalResearchTime.UT);
                        var scienceLabToAlarm = new KSPTimeSpan(scienceLabTime.UT - KACWorkerGameState.CurrentTime.UT);
                        if (DrawAddAlarm(scienceLabTime, null, scienceLabToAlarm))
                        {
                            KACAlarm alarmNew = new KACAlarm(
                                KACWorkerGameState.CurrentVessel.id.ToString(),
                                strAlarmName,
                                strAlarmNotes,
                                KACWorkerGameState.CurrentTime.UT + scienceLabToAlarm.UT,
                                0,
                                KACAlarm.AlarmTypeEnum.ScienceLab,
                                AddActions);

                            alarms.Add(alarmNew);

                            //settings.Save();
                            _ShowAddPane = false;
                        }
                    }
                }
            }
        }

        private Boolean DrawAddAlarm(KSPDateTime AlarmDate, KSPTimeSpan TimeToEvent, KSPTimeSpan TimeToAlarm, Boolean ForceShowRepeat = false)
        {
            Boolean blnReturn = false;
            intHeight_AddWindowRepeat = 0;
            int intLineHeight = 18;

            GUILayout.BeginVertical();

            //Do we show repeating options
            if (KACAlarm.AlarmTypeSupportsRepeat.Contains(AddType) || ForceShowRepeat)
            {
                intHeight_AddWindowRepeat += 53;
                GUILayout.Label("Alarm Repeat Options...", KACResources.styleAddSectionHeading);
                GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
                DrawCheckbox(ref blnRepeatingAlarmFlag, new GUIContent("Make this alarm repeat when triggered", "If enabled then when one alarm fires another will be created based on the existing alarm"));
                if (KACAlarm.AlarmTypeSupportsRepeatPeriod.Contains(AddType))
                {
                    intHeight_AddWindowRepeat += 24;
                    DrawTimeEntry(ref timeRepeatPeriod, KACTimeStringArray.TimeEntryPrecisionEnum.Days, "Repeat After:", 90);
                }
                GUILayout.EndVertical();
            }
        
            //Now for the add area
            GUILayout.BeginHorizontal(KACResources.styleAddAlarmArea);
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Date:", KACResources.styleAddHeading, GUILayout.Height(intLineHeight), GUILayout.Width(40), GUILayout.MaxWidth(40));
            GUILayout.Label(AlarmDate.ToStringStandard(DateStringFormatsEnum.DateTimeFormat), KACResources.styleContent, GUILayout.Height(intLineHeight));
            GUILayout.EndHorizontal();
            if (TimeToEvent != null)
            {
                GUILayout.BeginHorizontal();
                //GUILayout.Label("Time to " + strAlarmEventName + ":", KACResources.styleAddHeading, GUILayout.Height(intLineHeight), GUILayout.Width(120), GUILayout.MaxWidth(120));
                GUILayout.Label("Time to " + strAlarmEventName + ":", KACResources.styleAddHeading, GUILayout.Height(intLineHeight));
                GUILayout.Label(TimeToEvent.ToStringStandard(settings.TimeSpanFormat), KACResources.styleContent, GUILayout.Height(intLineHeight));
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            //GUILayout.Label("Time to Alarm:", KACResources.styleAddHeading, GUILayout.Height(intLineHeight), GUILayout.Width(120), GUILayout.MaxWidth(120));
            GUILayout.Label("Time to Alarm:", KACResources.styleAddHeading, GUILayout.Height(intLineHeight));
            GUILayout.Label(TimeToAlarm.ToStringStandard(settings.TimeSpanFormat), KACResources.styleContent, GUILayout.Height(intLineHeight));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(10);
            int intButtonHeight = 36;
            if (TimeToEvent != null) intButtonHeight += 22;
            if (GUILayout.Button("Add Alarm", KACResources.styleButton, GUILayout.Width(75), GUILayout.Height(intButtonHeight)))
            {
                blnReturn = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            return blnReturn;
        }

        ////Variables for Node Alarms screen
        ////String strNodeMargin = "1";
        ///// <summary>
        ///// Screen Layout for adding Alarm from Maneuver Node
        ///// </summary>
        private void WindowLayout_AddPane_Maneuver()
        {
            if (HighLogic.LoadedScene == GameScenes.FLIGHT &&
                (KERWrapper.APIReady || VOIDWrapper.APIReady))
            {
                intHeight_AddWindowKER = 73;

                if (KACWorkerGameState.CurrentVessel == null)
                {
                    GUILayout.Label("No Active Vessel");
                }
                else
                {
                    if (!KACWorkerGameState.ManeuverNodeExists)
                    {
                        GUILayout.Label("No Maneuver Nodes Found", GUILayout.ExpandWidth(true));
                    }
                    else
                    {
                        if (KERWrapper.APIReady)
                        {
                            KERWrapper.KER.UpdateManNodeValues();
                            GUILayout.Label("Kerbal Engineer Node Margin", KACResources.styleAddSectionHeading);
                            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

                            GUILayout.BeginHorizontal();

                            GUILayout.Label("Add KER Burn Time: ", KACResources.styleAddHeading);
                            ddlKERNodeMargin.DrawButton();
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Enough Δv:", KACResources.styleAddHeading);
                            GUILayout.Label(KERWrapper.KER.HasDeltaV.ToString(), KACResources.styleAddXferName);
                            GUILayout.Label("   Burn:", KACResources.styleAddHeading);
                            GUILayout.Label(String.Format("{0:0.0}s", KERWrapper.KER.BurnTime), KACResources.styleAddXferName);
                            GUILayout.Label("   ½ Burn:", KACResources.styleAddHeading);
                            GUILayout.Label(String.Format("{0:0.0}s", KERWrapper.KER.HalfBurnTime), KACResources.styleAddXferName);
                            GUILayout.EndHorizontal();

                            GUILayout.EndVertical();
                        }
                        else if (VOIDWrapper.APIReady)
                        {
                            GUILayout.Label("VOID Node Margin", KACResources.styleAddSectionHeading);
                            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

                            GUILayout.BeginHorizontal();

                            GUILayout.Label("Add VOID Burn Time: ", KACResources.styleAddHeading);
                            ddlKERNodeMargin.DrawButton();
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Enough Δv:", KACResources.styleAddHeading);
                            GUILayout.Label(VOIDWrapper.VOID.HasDeltaV.ToString(), KACResources.styleAddXferName);
                            GUILayout.Label("   Burn:", KACResources.styleAddHeading);
                            GUILayout.Label(String.Format("{0:0.0}s", VOIDWrapper.VOID.BurnTime), KACResources.styleAddXferName);
                            GUILayout.Label("   ½ Burn:", KACResources.styleAddHeading);
                            GUILayout.Label(String.Format("{0:0.0}s", VOIDWrapper.VOID.HalfBurnTime), KACResources.styleAddXferName);
                            GUILayout.EndHorizontal();

                            GUILayout.EndVertical();
                        }
                    }
                }
            }
            else
            {
                intHeight_AddWindowKER = 0;
            }

            GUILayout.BeginVertical();
            GUILayout.Label("Node Details...", KACResources.styleAddSectionHeading);

            if (KACWorkerGameState.CurrentVessel == null)
            {
                GUILayout.Label("No Active Vessel");
            }
            else
            {
                if (!KACWorkerGameState.ManeuverNodeExists)
                {
                    GUILayout.Label("No Maneuver Nodes Found", GUILayout.ExpandWidth(true));
                }
                else
                {
                    Boolean blnFoundNode = false;
                    String strMarginConversion = "";
                    //loop to find the first future node
                    for (int intNode = 0; (intNode < KACWorkerGameState.CurrentVessel.patchedConicSolver.maneuverNodes.Count) && !blnFoundNode; intNode++)
                    {
                        KSPDateTime nodeTime = new KSPDateTime(KACWorkerGameState.CurrentVessel.patchedConicSolver.maneuverNodes[intNode].UT);
                        KSPTimeSpan nodeInterval = new KSPTimeSpan(nodeTime.UT - KACWorkerGameState.CurrentTime.UT);

                        KSPDateTime nodeAlarm;
                        KSPTimeSpan nodeAlarmInterval;

                        Double KERMarginAdd = GetBurnMarginSecs((Settings.BurnMarginEnum)ddlKERNodeMargin.SelectedIndex);

                        try
                        {
                            nodeAlarm = new KSPDateTime(nodeTime.UT - timeMargin.UT - KERMarginAdd);
                            nodeAlarmInterval = new KSPTimeSpan(nodeTime.UT - KACWorkerGameState.CurrentTime.UT - timeMargin.UT - KERMarginAdd);
                        }
                        catch (Exception)
                        {
                            nodeAlarm = null;
                            nodeAlarmInterval = null;
                            strMarginConversion = "Unable to Add the Margin Minutes";
                        }

                        if ((nodeTime.UT > KACWorkerGameState.CurrentTime.UT) && strMarginConversion == "")
                        {
                            if (DrawAddAlarm(nodeTime,nodeInterval,nodeAlarmInterval))
                            {
                                //Get a list of all future Maneuver Nodes - thats what the skip does
                                List<ManeuverNode> manNodesToStore = KACWorkerGameState.CurrentVessel.patchedConicSolver.maneuverNodes.Skip(intNode).ToList<ManeuverNode>();

                                alarms.Add(new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(), strAlarmName, strAlarmNotes, nodeAlarm.UT, timeMargin.UT + KERMarginAdd, KACAlarm.AlarmTypeEnum.Maneuver,
                                    AddActions, manNodesToStore));
                                //settings.Save();
                                _ShowAddPane = false;
                            }
                            blnFoundNode = true;
                        }
                    }

                    if (strMarginConversion != "")
                        GUILayout.Label(strMarginConversion, GUILayout.ExpandWidth(true));
                    else if (!blnFoundNode)
                        GUILayout.Label("No Future Maneuver Nodes Found", GUILayout.ExpandWidth(true));
                }
            }

            GUILayout.EndVertical();
        }

        internal double GetBurnMarginSecs(Settings.BurnMarginEnum KerMarginType)
        {
            Double retBurnMargin = 0;
            if (KERWrapper.APIReady)
            {
                switch (KerMarginType)
                {
                    case Settings.BurnMarginEnum.None: retBurnMargin = 0; break;
                    case Settings.BurnMarginEnum.Half: retBurnMargin = KERWrapper.KER.HalfBurnTime; break;
                    case Settings.BurnMarginEnum.Full: retBurnMargin = KERWrapper.KER.BurnTime; break;
                    default: retBurnMargin = 0; break;
                }
            }
            else if (VOIDWrapper.APIReady)
            {
                switch (KerMarginType)
                {
                    case Settings.BurnMarginEnum.None: retBurnMargin = 0; break;
                    case Settings.BurnMarginEnum.Half: retBurnMargin = VOIDWrapper.VOID.HalfBurnTime; break;
                    case Settings.BurnMarginEnum.Full: retBurnMargin = VOIDWrapper.VOID.BurnTime; break;
                    default: retBurnMargin = 0; break;
                }

            }
            return retBurnMargin;
        }


        private List<KACAlarm.AlarmTypeEnum> lstAlarmsWithTarget = new List<KACAlarm.AlarmTypeEnum> { KACAlarm.AlarmTypeEnum.AscendingNode, KACAlarm.AlarmTypeEnum.DescendingNode, KACAlarm.AlarmTypeEnum.LaunchRendevous };
        private void WindowLayout_AddPane_NodeEvent(Boolean PointFound,Double timeToPoint)
        {
            GUILayout.BeginVertical();
            GUILayout.Label(strAlarmEventName + " Details...", KACResources.styleAddSectionHeading);
            if (lstAlarmsWithTarget.Contains(AddType))
            {
                if (KACWorkerGameState.CurrentVesselTarget == null)
                    GUILayout.Label("Equatorial Nodes (No Valid Target)", KACResources.styleAddXferName, GUILayout.Height(18));
                else
                {
                    if (KACWorkerGameState.CurrentVesselTarget is Vessel)
                        GUILayout.Label("Target Vessel: " + KACWorkerGameState.CurrentVesselTarget.GetVessel().vesselName, KACResources.styleAddXferName,GUILayout.Height(18));
                    else if (KACWorkerGameState.CurrentVesselTarget is CelestialBody)
                        GUILayout.Label("Target Body: " + ((CelestialBody)KACWorkerGameState.CurrentVesselTarget).bodyName, KACResources.styleAddXferName,GUILayout.Height(18));
                    else
                        GUILayout.Label("Object Targeted", KACResources.styleAddXferName, GUILayout.Height(18));
                        //GUILayout.Label("Target Vessel: " + KACWorkerGameState.CurrentVesselTarget.GetVessel().vesselName, KACResources.styleAddXferName, GUILayout.Height(18));
                }
            }

            if (KACWorkerGameState.CurrentVessel == null)
                GUILayout.Label("No Active Vessel");
            else
            {
                if (!PointFound)
                {
                    GUILayout.Label("No " + strAlarmEventName + " Point Found on current plan", GUILayout.ExpandWidth(true));
                }
                else
                {
                    String strMarginConversion = "";
                    KSPDateTime eventTime = new KSPDateTime(KACWorkerGameState.CurrentTime.UT + timeToPoint);
                    KSPTimeSpan eventInterval = new KSPTimeSpan(timeToPoint);

                    KSPDateTime eventAlarm;
                    KSPTimeSpan eventAlarmInterval;
                    try
                    {
                        eventAlarm = new KSPDateTime(eventTime.UT - timeMargin.UT);
                        eventAlarmInterval = new KSPTimeSpan(eventTime.UT - KACWorkerGameState.CurrentTime.UT - timeMargin.UT);
                    }
                    catch (Exception)
                    {
                        eventAlarm = null;
                        eventAlarmInterval = null;
                        strMarginConversion = "Unable to Add the Margin Minutes";
                    }

                    if ((eventTime.UT > KACWorkerGameState.CurrentTime.UT) && strMarginConversion == "")
                    {
                        if (DrawAddAlarm(eventTime, eventInterval, eventAlarmInterval))
                        {
                            KACAlarm newAlarm = new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(), strAlarmName, strAlarmNotes, eventAlarm.UT, timeMargin.UT, AddType,
                                AddActions);
                            if (lstAlarmsWithTarget.Contains(AddType))
                                newAlarm.TargetObject = KACWorkerGameState.CurrentVesselTarget;

                            if (newAlarm.SupportsRepeat)
                                newAlarm.RepeatAlarm = blnRepeatingAlarmFlag;

                            alarms.Add(newAlarm);
                            //settings.Save();
                            _ShowAddPane = false;
                        }
                    }
                    else
                    {
                        strMarginConversion = "No Future " + strAlarmEventName + "Points found";
                    }

                    if (strMarginConversion != "")
                        GUILayout.Label(strMarginConversion, GUILayout.ExpandWidth(true));
                }
            }

            GUILayout.EndVertical();
        }

        private List<CelestialBody> XferParentBodies = new List<CelestialBody>();
        private List<CelestialBody> XferOriginBodies = new List<CelestialBody>();
        private List<KACXFerTarget> XferTargetBodies = new List<KACXFerTarget>();

        private static int SortByDistance(CelestialBody c1, CelestialBody c2)
        {
            Double f1 = c1.orbit.semiMajorAxis;
            double f2 = c2.orbit.semiMajorAxis;
            //LogFormatted("{0}-{1}", f1.ToString(), f2.ToString());
            return f1.CompareTo(f2);
        }


        private int intXferCurrentParent = 0;
        private int intXferCurrentOrigin = 0;
        private int intXferCurrentTarget = 0;
        //private KerbalTime XferCurrentTargetEventTime;
        private Boolean blnRepeatingAlarmFlag = false;

        private void SetUpXferParents()
        {
            XferParentBodies = new List<CelestialBody>();
            //Build a list of parents - Cant sort this normally as the Sun has no radius - duh!
            foreach (CelestialBody tmpBody in FlightGlobals.Bodies)
            {
                //add any body that has more than 1 child to the parents list
                if (tmpBody.orbitingBodies.Count > 1)
                    XferParentBodies.Add(tmpBody);
            }
        }

        private void SetupXferOrigins()
        {
            //set the possible origins to be all the orbiting bodies around the parent
            XferOriginBodies = new List<CelestialBody>();
            XferOriginBodies = XferParentBodies[intXferCurrentParent].orbitingBodies.OrderBy(b => b.orbit.semiMajorAxis).ToList<CelestialBody>();
            if (intXferCurrentOrigin > XferOriginBodies.Count)
                intXferCurrentOrigin = 0;

            if (AddType== KACAlarm.AlarmTypeEnum.Transfer || AddType== KACAlarm.AlarmTypeEnum.TransferModelled) BuildTransferStrings();
        }

        private void SetupXFerTargets()
        {
            XferTargetBodies = new List<KACXFerTarget>();

            //Loop through the Siblings of the origin planet
            foreach (CelestialBody bdyTarget in XferOriginBodies.OrderBy(b => b.orbit.semiMajorAxis))
            {
                //add all the other siblings as target possibilities
                if (bdyTarget != XferOriginBodies[intXferCurrentOrigin])
                {
                    KACXFerTarget tmpTarget = new KACXFerTarget();
                    tmpTarget.Origin = XferOriginBodies[intXferCurrentOrigin];
                    tmpTarget.Target = bdyTarget;
                    //tmpTarget.SetPhaseAngleTarget();
                    //add it to the list
                    XferTargetBodies.Add(tmpTarget);
                }
            }
            if (intXferCurrentTarget > XferTargetBodies.Count)
                intXferCurrentTarget = 0;

            if (AddType == KACAlarm.AlarmTypeEnum.Transfer || AddType == KACAlarm.AlarmTypeEnum.TransferModelled) BuildTransferStrings();
        }

        private int intAddXferHeight = 317;
        private int intXferType = 1;
        private Vector2 xferListScrollPosition = new Vector2();
        private void WindowLayout_AddPane_Transfer()
        {
            intAddXferHeight = 304;// 317;

            if (settings.RSSActive) {
                GUILayout.Label("RSS detected - it is recommended that you use the Transfer Window Planner Plugin to plan transfers", KACResources.styleAddXferName);
                GUILayout.Space(-8);
                if (GUILayout.Button("Click here to open the TWP forum thread", KACResources.styleContent))
                    Application.OpenURL("https://forum.kerbalspaceprogram.com/topic/84005-transfer-window-planner/");
                intAddXferHeight += 58;
            }

            KSPDateTime XferCurrentTargetEventTime = null;
            List<KSPDateTime> lstXferCurrentTargetEventTime = new List<KSPDateTime>();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Transfers", KACResources.styleAddSectionHeading, GUILayout.Width(60));
            //add something here to select the modelled or formula values for Solar orbiting bodies
            if (settings.XferModelDataLoaded)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Calc by:", KACResources.styleAddHeading);
                if (intXferCurrentParent == 0)
                {
                    //intAddXferHeight += 35;
                    if (DrawRadioList(ref intXferType, "Model", "Formula"))
                    {
                        settings.XferUseModelData = (intXferType == 0);
                        settings.Save();
                    }
                }
                else
                {
                    int zero = 0;
                    DrawRadioList(ref zero, "Formula");
                }
            }
            GUILayout.EndHorizontal();
            try
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Xfer Parent:", KACResources.styleAddHeading, GUILayout.Width(80), GUILayout.Height(20));
                GUILayout.Label(XferParentBodies[intXferCurrentParent].bodyName, KACResources.styleAddXferName, GUILayout.ExpandWidth(true), GUILayout.Height(20));
                if (GUILayout.Button(new GUIContent("Change", "Click to cycle through Parent Bodies"), KACResources.styleAddXferOriginButton))
                {
                    intXferCurrentParent += 1;
                    if (intXferCurrentParent >= XferParentBodies.Count) intXferCurrentParent = 0;
                    SetupXferOrigins();
                    intXferCurrentOrigin = 0;
                    SetupXFerTargets();
                    BuildTransferStrings();
                    //strAlarmNotesNew = String.Format("{0} Transfer", XferOriginBodies[intXferCurrentOrigin].bodyName);
                }
                GUILayout.Space(34);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Xfer Origin:", KACResources.styleAddHeading, GUILayout.Width(80),GUILayout.Height(20));
                GUILayout.Label(XferOriginBodies[intXferCurrentOrigin].bodyName, KACResources.styleAddXferName, GUILayout.ExpandWidth(true), GUILayout.Height(20));
                if (GUILayout.Button(new GUIContent("Change", "Click to cycle through Origin Bodies"), KACResources.styleAddXferOriginButton))
                {
                    intXferCurrentOrigin += 1;
                    if (intXferCurrentOrigin >= XferOriginBodies.Count) intXferCurrentOrigin = 0;
                    SetupXFerTargets();
                    BuildTransferStrings();
                    //strAlarmNotesNew = String.Format("{0} Transfer", XferOriginBodies[intXferCurrentOrigin].bodyName);
                }

                if (!settings.AlarmXferDisplayList)
                    GUILayout.Space(34);
                else
                    if (GUILayout.Button(new GUIContent(KACResources.btnChevronUp, "Hide Full List"), KACResources.styleSmallButton))
                    {
                        settings.AlarmXferDisplayList = !settings.AlarmXferDisplayList;
                        settings.Save();
                    }
                GUILayout.EndHorizontal();

                if (!settings.AlarmXferDisplayList)
                {
                    //Simple single chosen target
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Xfer Target:", KACResources.styleAddHeading, GUILayout.Width(80), GUILayout.Height(20));
                    GUILayout.Label(XferTargetBodies[intXferCurrentTarget].Target.bodyName, KACResources.styleAddXferName, GUILayout.ExpandWidth(true), GUILayout.Height(20));
                    if (GUILayout.Button(new GUIContent("Change", "Click to cycle through Target Bodies"), KACResources.styleAddXferOriginButton))
                    {
                        intXferCurrentTarget += 1;
                        if (intXferCurrentTarget >= XferTargetBodies.Count) intXferCurrentTarget = 0;
                        SetupXFerTargets();
                        BuildTransferStrings();
                        //strAlarmNotesNew = String.Format("{0} Transfer", XferTargetBodies[intXferCurrentTarget].Target.bodyName);
                    }
                    if (GUILayout.Button(new GUIContent(KACResources.btnChevronDown, "Show Full List"), KACResources.styleSmallButton))
                    {
                        settings.AlarmXferDisplayList = !settings.AlarmXferDisplayList;
                        settings.Save();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal(); 
                    GUILayout.Label("Phase Angle-Current:",  KACResources.styleAddHeading,GUILayout.Width(130));
                    GUILayout.Label(String.Format("{0:0.00}", XferTargetBodies[intXferCurrentTarget].PhaseAngleCurrent), KACResources.styleContent, GUILayout.Width(67));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Phase Angle-Target:", KACResources.styleAddHeading, GUILayout.Width(130));
                    if (intXferCurrentParent != 0 || (!settings.XferUseModelData && settings.XferModelDataLoaded))
                    {
                        //formula based
                        GUILayout.Label(String.Format("{0:0.00}", XferTargetBodies[intXferCurrentTarget].PhaseAngleTarget), KACResources.styleContent, GUILayout.Width(67));
                    }
                    else
                    {
                        //this is the modelled data, but only for Kerbol orbiting bodies
                        try
                        {
                            KACXFerModelPoint tmpModelPoint = KACResources.lstXferModelPoints.FirstOrDefault(
                            m => FlightGlobals.Bodies[m.Origin] == XferTargetBodies[intXferCurrentTarget].Origin &&
                                FlightGlobals.Bodies[m.Target] == XferTargetBodies[intXferCurrentTarget].Target &&
                                m.UT >= KACWorkerGameState.CurrentTime.UT);

                            if (tmpModelPoint != null)
                            {
                                GUILayout.Label(String.Format("{0:0.00}", tmpModelPoint.PhaseAngle), KACResources.styleContent, GUILayout.Width(67));
                                XferCurrentTargetEventTime = new KSPDateTime(tmpModelPoint.UT);
                            }
                            else
                            {
                                GUILayout.Label("No future model data available for this transfer", KACResources.styleContent, GUILayout.ExpandWidth(true));
                            }
                        }
                        catch (Exception ex)
                        {
                            GUILayout.Label("Unable to determine model data", KACResources.styleContent, GUILayout.ExpandWidth(true));
                            LogFormatted("Error determining model data: {0}", ex.Message);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {



                    //Build the list of model points for the add all button
                    for (int intTarget = 0; intTarget < XferTargetBodies.Count; intTarget++) {
                        if (!(intXferCurrentParent != 0 || (!settings.XferUseModelData && settings.XferModelDataLoaded))) {
                            try
                            {
                                KACXFerModelPoint tmpModelPoint = KACResources.lstXferModelPoints.FirstOrDefault(
                                m => FlightGlobals.Bodies[m.Origin] == XferTargetBodies[intTarget].Origin &&
                                    FlightGlobals.Bodies[m.Target] == XferTargetBodies[intTarget].Target &&
                                    m.UT >= KACWorkerGameState.CurrentTime.UT);

                                if (tmpModelPoint != null) {
                                    lstXferCurrentTargetEventTime.Add(new KSPDateTime(tmpModelPoint.UT));
                                }
                            }
                            catch (Exception ex)
                            {
                                LogFormatted("Error determining model data: {0}", ex.Message);
                            }
                        }
                    }

                    // Now do the add all buttons
                    intAddXferHeight += 28;
                    if (intXferCurrentParent != 0 || (!settings.XferUseModelData && settings.XferModelDataLoaded))
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent("Create Alarms for All", "Create Alarms for all listed transfers"), new GUIStyle(KACResources.styleAddXferOriginButton) { fixedWidth = 140 }))
                        {
                            for (int i = 0; i < XferTargetBodies.Count; i++)
                            {
                                String strVesselID = "";
                                if (blnAlarmAttachToVessel) strVesselID = KACWorkerGameState.CurrentVessel.id.ToString();

                                TransferStrings ts = BuildTransferStrings(i, false);
                                alarms.Add(new KACAlarm(strVesselID, ts.AlarmName, ts.AlarmNotes + "\r\n\tMargin: " + new KSPTimeSpan(timeMargin.UT).ToStringStandard(TimeSpanStringFormatsEnum.IntervalLongTrimYears),
                                (KACWorkerGameState.CurrentTime.UT + XferTargetBodies[i].AlignmentTime.UT - timeMargin.UT), timeMargin.UT, KACAlarm.AlarmTypeEnum.Transfer,
                                AddActions, XferTargetBodies[i]));
                            }
                            _ShowAddPane = false;
                        }
                        GUILayout.EndHorizontal();
                    } else {
                        //Model based

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent("Create Alarms for All", "Create Alarms for all listed transfers"), new GUIStyle(KACResources.styleAddXferOriginButton) { fixedWidth = 140 }))
                        {
                            for (int i = 0; i < XferTargetBodies.Count; i++)
                            {
                                String strVesselID = "";
                                if (blnAlarmAttachToVessel) strVesselID = KACWorkerGameState.CurrentVessel.id.ToString();

                                TransferStrings ts = BuildTransferStrings(i, false);
                                KACAlarm alarmNew = new KACAlarm(strVesselID, ts.AlarmName, (blnRepeatingAlarmFlag ? "Alarm Repeats\r\n" : "") + ts.AlarmNotes + "\r\n\tMargin: " + new KSPTimeSpan(timeMargin.UT).ToStringStandard(TimeSpanStringFormatsEnum.IntervalLongTrimYears),
                                    (lstXferCurrentTargetEventTime[i].UT - timeMargin.UT), timeMargin.UT, KACAlarm.AlarmTypeEnum.TransferModelled,
                                    AddActions, XferTargetBodies[i]);
                                alarmNew.RepeatAlarm = blnRepeatingAlarmFlag;
                                alarms.Add(alarmNew);
                            }
                            _ShowAddPane = false;
                        }
                        GUILayout.EndHorizontal();
                    }

                    GUIStyle styleTemp = new GUIStyle();
                    xferListScrollPosition = GUILayout.BeginScrollView(xferListScrollPosition, styleTemp);

                    // And now the table of results
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Target", KACResources.styleAddSectionHeading, GUILayout.Width(55));
                    GUILayout.Label(new GUIContent("Phase Angle", "Displayed as \"Current Angle (Target Angle)\""), KACResources.styleAddSectionHeading, GUILayout.Width(105));
                    GUILayout.Label("Time to Transfer", KACResources.styleAddSectionHeading, GUILayout.ExpandWidth(true));
                    //GUILayout.Label("Time to Alarm", KACResources.styleAddSectionHeading, GUILayout.ExpandWidth(true));
                    GUILayout.Label("Add", KACResources.styleAddSectionHeading, GUILayout.Width(30));
                    GUILayout.EndHorizontal();

                    for (int intTarget = 0; intTarget < XferTargetBodies.Count; intTarget++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(XferTargetBodies[intTarget].Target.bodyName, KACResources.styleAddXferName, GUILayout.Width(55), GUILayout.Height(20));
                        if (intXferCurrentParent != 0 || (!settings.XferUseModelData && settings.XferModelDataLoaded))
                        {
                            //formula based
                            String strPhase = String.Format("{0:0.00}({1:0.00})", XferTargetBodies[intTarget].PhaseAngleCurrent, XferTargetBodies[intTarget].PhaseAngleTarget);
                            GUILayout.Label(strPhase, KACResources.styleAddHeading, GUILayout.Width(105), GUILayout.Height(20));
                            GUILayout.Label(XferTargetBodies[intTarget].AlignmentTime.ToStringStandard(settings.TimeSpanFormat), KACResources.styleAddHeading, GUILayout.ExpandWidth(true), GUILayout.Height(20));
                        }
                        else 
                        { 
                            try
                            {
                                KACXFerModelPoint tmpModelPoint = KACResources.lstXferModelPoints.FirstOrDefault(
                                m => FlightGlobals.Bodies[m.Origin] == XferTargetBodies[intTarget].Origin &&
                                    FlightGlobals.Bodies[m.Target] == XferTargetBodies[intTarget].Target &&
                                    m.UT >= KACWorkerGameState.CurrentTime.UT);
                            
                                if (tmpModelPoint != null)
                                {
                                    String strPhase = String.Format("{0:0.00}({1:0.00})", XferTargetBodies[intTarget].PhaseAngleCurrent, tmpModelPoint.PhaseAngle);
                                    GUILayout.Label(strPhase, KACResources.styleAddHeading, GUILayout.Width(105), GUILayout.Height(20));
                                    KSPTimeSpan tmpTime = new KSPTimeSpan(tmpModelPoint.UT - KACWorkerGameState.CurrentTime.UT);
                                    GUILayout.Label(tmpTime.ToStringStandard(settings.TimeSpanFormat), KACResources.styleAddHeading, GUILayout.ExpandWidth(true), GUILayout.Height(20));                                

                                    if (intTarget==intXferCurrentTarget)
                                        XferCurrentTargetEventTime = new KSPDateTime(tmpModelPoint.UT);

                                    // Doing this at the top of the loop now
                                    //lstXferCurrentTargetEventTime.Add(new KSPDateTime(tmpModelPoint.UT));
                                }
                                else
                                {
                                    GUILayout.Label("No future model data", KACResources.styleContent, GUILayout.ExpandWidth(true));
                                }
                            }
                            catch (Exception ex)
                            {
                                GUILayout.Label("Unable to determine model data", KACResources.styleContent, GUILayout.ExpandWidth(true));
                                LogFormatted("Error determining model data: {0}", ex.Message);
                            }
                        }
                        Boolean blnSelected = (intXferCurrentTarget == intTarget);
                        if (DrawToggle(ref blnSelected, "", KACResources.styleCheckbox, GUILayout.Width(42)))
                        {
                            if (blnSelected)
                            {
                                intXferCurrentTarget = intTarget;
                                BuildTransferStrings();
                            }
                        }

                        GUILayout.EndHorizontal();
                    }

                    intAddXferHeight += -56 + ( XferTargetBodies.Count * 30);

                    GUILayout.EndScrollView();
                    intAddXferHeight += 2; //For the scroll bar
                }

                if(intXferCurrentParent != 0 || (!settings.XferUseModelData && settings.XferModelDataLoaded))
                {
                    ////Formula based - Add All Alarms
                    //if (settings.AlarmXferDisplayList)
                    //{
                    //    intAddXferHeight += 28;

                    //    GUILayout.BeginHorizontal();
                    //    GUILayout.FlexibleSpace();
                    //    if (GUILayout.Button(new GUIContent("Create Alarms for All", "Create Alarms for all listed transfers"), new GUIStyle(KACResources.styleAddXferOriginButton) {fixedWidth=140 }))
                    //    {
                    //        for (int i = 0; i < XferTargetBodies.Count; i++)
                    //        {
                    //            String strVesselID = "";
                    //            if (blnAlarmAttachToVessel) strVesselID = KACWorkerGameState.CurrentVessel.id.ToString();

                    //            TransferStrings ts = BuildTransferStrings(i, false);
                    //            alarms.Add(new KACAlarm(strVesselID, ts.AlarmName, ts.AlarmNotes + "\r\n\tMargin: " + new KSPTimeSpan(timeMargin.UT).ToStringStandard(TimeSpanStringFormatsEnum.IntervalLongTrimYears),
                    //            (KACWorkerGameState.CurrentTime.UT + XferTargetBodies[i].AlignmentTime.UT - timeMargin.UT), timeMargin.UT, KACAlarm.AlarmTypeEnum.Transfer,
                    //            AddActions, XferTargetBodies[i]));
                    //        }
                    //        _ShowAddPane = false;
                    //    }
                    //    GUILayout.EndHorizontal();
                    //}

                    //Formula based - add new alarm
                    if (DrawAddAlarm(new KSPDateTime(KACWorkerGameState.CurrentTime.UT + XferTargetBodies[intXferCurrentTarget].AlignmentTime.UT),
                                    XferTargetBodies[intXferCurrentTarget].AlignmentTime,
                                    new KSPTimeSpan(XferTargetBodies[intXferCurrentTarget].AlignmentTime.UT - timeMargin.UT)))
                    {
                        String strVesselID = "";
                        if (blnAlarmAttachToVessel) strVesselID = KACWorkerGameState.CurrentVessel.id.ToString();
                        alarms.Add(new KACAlarm(strVesselID, strAlarmName, strAlarmNotes + "\r\n\tMargin: " + new KSPTimeSpan(timeMargin.UT).ToStringStandard(TimeSpanStringFormatsEnum.IntervalLongTrimYears),
                            (KACWorkerGameState.CurrentTime.UT + XferTargetBodies[intXferCurrentTarget].AlignmentTime.UT - timeMargin.UT), timeMargin.UT, KACAlarm.AlarmTypeEnum.Transfer,
                            AddActions, XferTargetBodies[intXferCurrentTarget]));
                        //settings.Save();
                        _ShowAddPane = false;
                    }
                }
                else
                {

                    //Model based
                    if (XferCurrentTargetEventTime!=null)
                    {
                        ////Formula based - Add All Alarms
                        //if (settings.AlarmXferDisplayList)
                        //{
                        //    intAddXferHeight += 28;

                        //    GUILayout.BeginHorizontal();
                        //    GUILayout.FlexibleSpace();
                        //    if (GUILayout.Button(new GUIContent("Create Alarms for All", "Create Alarms for all listed transfers"), new GUIStyle(KACResources.styleAddXferOriginButton) { fixedWidth = 140 }))
                        //    {
                        //        for (int i = 0; i < XferTargetBodies.Count; i++)
                        //        {
                        //            String strVesselID = "";
                        //            if (blnAlarmAttachToVessel) strVesselID = KACWorkerGameState.CurrentVessel.id.ToString();

                        //            TransferStrings ts = BuildTransferStrings(i, false);

                        //            KACAlarm alarmNew = new KACAlarm(strVesselID, ts.AlarmName, (blnRepeatingAlarmFlag ? "Alarm Repeats\r\n" : "") + ts.AlarmNotes + "\r\n\tMargin: " + new KSPTimeSpan(timeMargin.UT).ToStringStandard(TimeSpanStringFormatsEnum.IntervalLongTrimYears),
                        //                (lstXferCurrentTargetEventTime[i].UT - timeMargin.UT), timeMargin.UT, KACAlarm.AlarmTypeEnum.TransferModelled,
                        //                AddActions, XferTargetBodies[i]); 
                        //            alarmNew.RepeatAlarm = blnRepeatingAlarmFlag;
                        //            alarms.Add(alarmNew);
                        //        }
                        //        _ShowAddPane = false;
                        //    }
                        //    GUILayout.EndHorizontal();
                        //}

                        if (DrawAddAlarm(XferCurrentTargetEventTime,
                                    new KSPTimeSpan(XferCurrentTargetEventTime.UT - KACWorkerGameState.CurrentTime.UT),
                                    new KSPTimeSpan(XferCurrentTargetEventTime.UT - KACWorkerGameState.CurrentTime.UT - timeMargin.UT),
                                    true))
                        {
                            String strVesselID = "";
                            if (blnAlarmAttachToVessel) strVesselID = KACWorkerGameState.CurrentVessel.id.ToString();
                            KACAlarm alarmNew = new KACAlarm(strVesselID, strAlarmName, (blnRepeatingAlarmFlag ? "Alarm Repeats\r\n" : "") + strAlarmNotes + "\r\n\tMargin: " + new KSPTimeSpan(timeMargin.UT).ToStringStandard(TimeSpanStringFormatsEnum.IntervalLongTrimYears),
                                (XferCurrentTargetEventTime.UT - timeMargin.UT), timeMargin.UT, KACAlarm.AlarmTypeEnum.TransferModelled,
                                AddActions, XferTargetBodies[intXferCurrentTarget]); 
                            alarmNew.RepeatAlarm = blnRepeatingAlarmFlag;
                            alarms.Add(alarmNew);
                            //settings.Save();
                            _ShowAddPane = false;
                        }
                    }
                    else{
                        GUILayout.Label("Selected a transfer with no event date",GUILayout.ExpandWidth(true));
                    }
                }
            }
            catch (Exception ex)
            {
                if (intXferCurrentTarget >= XferTargetBodies.Count) 
                    intXferCurrentTarget = 0;
                GUILayout.Label("Something weird has happened");
                LogFormatted(ex.Message);
                LogFormatted(ex.StackTrace);
            }


            //intAddXferHeight += intTestheight4;

            GUILayout.EndVertical();
        }



        private Int32 AddNotesHeight = 100;
        internal void FillAddMessagesWindow(int WindowID)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Vessel:", KACResources.styleAddHeading);
            String strVesselName = "Not Attached to Vessel";
            if (KACWorkerGameState.CurrentVessel != null && blnAlarmAttachToVessel) strVesselName = KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName);
            GUILayout.TextField(strVesselName, KACResources.styleAddFieldGreen);
            GUILayout.Label("Alarm:", KACResources.styleAddHeading);
            strAlarmName = GUILayout.TextField(strAlarmName, KACResources.styleAddField, GUILayout.MaxWidth(184)).Replace("|", "");
            GUILayout.Label("Notes:", KACResources.styleAddHeading);
            strAlarmNotes = GUILayout.TextArea(strAlarmNotes, KACResources.styleAddMessageField,
                                GUILayout.Height(AddNotesHeight), GUILayout.MaxWidth(184)
                                ).Replace("|", ""); 

            GUILayout.EndVertical();
        }
    }
}
