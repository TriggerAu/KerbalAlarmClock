using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using KSP.UI;
using KSP.UI.Screens;
using KSP.UI.Dialogs;
using KSPPluginFramework;

namespace KerbalAlarmClock
{
    internal static class KACWorkerGameState
    {
        internal static String LastSaveGameName = "";
        internal static GameScenes LastGUIScene = GameScenes.LOADING;
        internal static Vessel LastVessel = null;
        internal static CelestialBody LastSOIBody = null;
        internal static ITargetable LastVesselTarget = null;

        internal static String CurrentSaveGameName = "";
        internal static GameScenes CurrentGUIScene = GameScenes.LOADING;
        internal static Vessel CurrentVessel = null;
        internal static CelestialBody CurrentSOIBody = null;
        internal static ITargetable CurrentVesselTarget = null;

        internal static Boolean ChangedSaveGameName { get { return (LastSaveGameName != CurrentSaveGameName); } }
        internal static Boolean ChangedGUIScene { get { return (LastGUIScene != CurrentGUIScene); } }
        internal static Boolean ChangedVessel { get { if (LastVessel == null) return true; else return (LastVessel != CurrentVessel); } }
        internal static Boolean ChangedSOIBody { get { if (LastSOIBody == null) return true; else return (LastSOIBody != CurrentSOIBody); } }
        internal static Boolean ChangedVesselTarget { get { if (LastVesselTarget == null) return true; else return (LastVesselTarget != CurrentVesselTarget); } }

        //The current UT time - for alarm comparison
        internal static KSPDateTime CurrentTime = new KSPDateTime(0);
        internal static KSPDateTime LastTime = new KSPDateTime(0);

        internal static Boolean CurrentlyUnderWarpInfluence = false;
        internal static DateTime CurrentWarpInfluenceStartTime;

        //Are we flying any ship?
        internal static Boolean IsVesselActive
        {
            get { return FlightGlobals.fetch != null && CurrentVessel != null; }
        }

        internal static Boolean PauseMenuOpen
        {
            get
            {
                try { return PauseMenu.isOpen; }
                catch (Exception)
                {
                    //if we cant read it it cant be open.
                    return false;
                }
            }
        }

        internal static Boolean FlightResultsDialogOpen
        {
            get
            {
                try { return FlightResultsDialog.isDisplaying; }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        //Does the active vessel have any Maneuver nodes
        internal static Boolean ManeuverNodeExists
        {
            get
            {
                Boolean blnReturn = false;
                if (IsVesselActive)
                {
                    if (CurrentVessel.patchedConicSolver != null)
                    {
                        if (CurrentVessel.patchedConicSolver.maneuverNodes != null)
                        {
                            if (CurrentVessel.patchedConicSolver.maneuverNodes.Count > 0)
                            {
                                blnReturn = true;
                            }
                        }
                    }
                }
                return blnReturn;
            }
        }

        internal static ManeuverNode ManeuverNodeFuture
        {
            get
            {
                return CurrentVessel.patchedConicSolver.maneuverNodes.OrderBy(x => x.UT).FirstOrDefault(x => x.UT > KACWorkerGameState.CurrentTime.UT);
            }
        }

        internal static List<ManeuverNode> ManeuverNodesFuture
        {
            get
            {
                return (CurrentVessel.patchedConicSolver.maneuverNodes.OrderBy(x => x.UT).SkipWhile(x => x.UT < KACWorkerGameState.CurrentTime.UT).ToList<ManeuverNode>());
            }
        }

        internal static List<ManeuverNode> ManeuverNodesAll
        {
            get
            {
                return (CurrentVessel.patchedConicSolver.maneuverNodes.OrderBy(x => x.UT).ToList<ManeuverNode>());
            }
        }
        internal static Boolean ManeuverNodesAll_GizmoShown
        {
            get
            {
                return KACWorkerGameState.ManeuverNodeExists && KACWorkerGameState.ManeuverNodesAll.Any(n => n.attachedGizmo != null);
            }
        }

        internal static Boolean SOIPointExists
        {
            get
            {
                Boolean blnReturn = false;

                if (CurrentVessel != null)
                {
                    if (CurrentVessel.orbit != null)
                    {
                        List<Orbit.PatchTransitionType> SOITransitions = new List<Orbit.PatchTransitionType> { Orbit.PatchTransitionType.ENCOUNTER, Orbit.PatchTransitionType.ESCAPE };
                        blnReturn = SOITransitions.Contains(CurrentVessel.orbit.patchEndTransition);
                    }
                }

                return blnReturn;
            }
        }

        internal static Boolean ApPointExists
        {
            get
            {
                Boolean blnReturn = false;

                if (CurrentVessel != null)
                {
                    if (CurrentVessel.orbit != null)
                    {
                        if (CurrentVessel.orbit.timeToAp > 0
                            && ((CurrentTime.UT + CurrentVessel.orbit.timeToAp) < CurrentVessel.orbit.EndUT))
                            blnReturn = true;
                    }
                }
                return blnReturn;
            }
        }
        internal static Boolean PePointExists
        {
            get
            {
                Boolean blnReturn = false;

                if (CurrentVessel != null)
                {
                    if (CurrentVessel.orbit != null)
                    {
                        if (CurrentVessel.orbit.timeToPe > 0
                            && ((CurrentTime.UT + CurrentVessel.orbit.timeToPe) < CurrentVessel.orbit.EndUT))
                            blnReturn = true;
                    }
                }
                return blnReturn;
            }
        }

        //do null checks on all these!!!!!
        internal static void SetCurrentGUIStates()
        {
           KACWorkerGameState.CurrentGUIScene = HighLogic.LoadedScene;
        }

        internal static void SetLastGUIStatesToCurrent()
        {
           KACWorkerGameState.LastGUIScene =KACWorkerGameState.CurrentGUIScene;
        }

        internal static void SetCurrentFlightStates()
        {
            if (HighLogic.CurrentGame != null)
               KACWorkerGameState.CurrentSaveGameName = HighLogic.CurrentGame.Title;
            else
               KACWorkerGameState.CurrentSaveGameName = "";

            try {
				if (HighLogic.LoadedSceneIsEditor) {
					KACWorkerGameState.CurrentTime.UT = HighLogic.CurrentGame.flightState.universalTime;
				} else {
					KACWorkerGameState.CurrentTime.UT = Planetarium.GetUniversalTime();
				}
			} catch (Exception) { }
            //if (Planetarium.fetch!=null)KACWorkerGameState.CurrentTime.UT = Planetarium.GetUniversalTime();

            try
            {
                if (KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT && FlightGlobals.ActiveVessel != null)
                {
                    KACWorkerGameState.CurrentVessel = FlightGlobals.ActiveVessel;
                    KACWorkerGameState.CurrentSOIBody = CurrentVessel.mainBody;
                    KACWorkerGameState.CurrentVesselTarget = CurrentVessel.targetObject;
                }
                else if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION)
                {
                    SpaceTracking st = (SpaceTracking)KACSpaceCenter.FindObjectOfType(typeof(SpaceTracking));
                    PlanetariumCamera c = PlanetariumCamera.fetch;
                    if (c.target != null && c.target.type == MapObject.ObjectType.Vessel)
                    {
                        KACWorkerGameState.CurrentVessel = c.target.vessel;
                        KACWorkerGameState.CurrentSOIBody = CurrentVessel.mainBody;
                        KACWorkerGameState.CurrentVesselTarget = CurrentVessel.targetObject;
                    }
                    else
                    {
                        KACWorkerGameState.CurrentVessel = null;
                        KACWorkerGameState.CurrentSOIBody = null;
                        KACWorkerGameState.CurrentVesselTarget = null;
                    }
                }
                //else if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION &&
                //        MapView.MapCamera.target.type == MapObject.MapObjectType.VESSEL)
                //{
                //    KACWorkerGameState.CurrentVessel = MapView.MapCamera.target.vessel;
                //    KACWorkerGameState.CurrentSOIBody = CurrentVessel.mainBody;
                //    KACWorkerGameState.CurrentVesselTarget = CurrentVessel.targetObject;
                //}
                else
                {
                    KACWorkerGameState.CurrentVessel = null;
                    KACWorkerGameState.CurrentSOIBody = null;
                    KACWorkerGameState.CurrentVesselTarget = null;
                }
            }
            catch (Exception)
            {
                
                
            }
        }

        internal static void SetLastFlightStatesToCurrent()
        {
           KACWorkerGameState.LastSaveGameName =KACWorkerGameState.CurrentSaveGameName;
           KACWorkerGameState.LastTime =KACWorkerGameState.CurrentTime;
           if (LastVessel != CurrentVessel) { if (VesselChanged != null) VesselChanged(LastVessel, CurrentVessel); }
           KACWorkerGameState.LastVessel = KACWorkerGameState.CurrentVessel;
           KACWorkerGameState.LastSOIBody =KACWorkerGameState.CurrentSOIBody;
           KACWorkerGameState.LastVesselTarget =KACWorkerGameState.CurrentVesselTarget;
        }

        internal delegate void VesselChangedHandler(Vessel OldVessel, Vessel NewVessel);
        internal static event VesselChangedHandler VesselChanged;
    }
}
