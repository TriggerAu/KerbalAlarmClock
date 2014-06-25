using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalAlarmClock
{
    public static class KACWorkerGameState
    {
        public static String LastSaveGameName = "";
        public static GameScenes LastGUIScene = GameScenes.LOADING;
        public static Vessel LastVessel = null;
        public static CelestialBody LastSOIBody = null;
        public static ITargetable LastVesselTarget = null;

        public static String CurrentSaveGameName = "";
        public static GameScenes CurrentGUIScene = GameScenes.LOADING;
        public static Vessel CurrentVessel = null;
        public static CelestialBody CurrentSOIBody = null;
        public static ITargetable CurrentVesselTarget = null;

        public static Boolean ChangedSaveGameName { get { return (LastSaveGameName != CurrentSaveGameName); } }
        public static Boolean ChangedGUIScene { get { return (LastGUIScene != CurrentGUIScene); } }
        public static Boolean ChangedVessel { get { if (LastVessel == null) return true; else return (LastVessel != CurrentVessel); } }
        public static Boolean ChangedSOIBody { get { if (LastSOIBody == null) return true; else return (LastSOIBody != CurrentSOIBody); } }
        public static Boolean ChangedVesselTarget { get { if (LastVesselTarget == null) return true; else return (LastVesselTarget != CurrentVesselTarget); } }

        //The current UT time - for alarm comparison
        public static KACTime CurrentTime = new KACTime();
        public static KACTime LastTime = new KACTime();

        public static Boolean CurrentlyUnderWarpInfluence = false;
        public static DateTime CurrentWarpInfluenceStartTime;

        //Are we flying any ship?
        public static Boolean IsVesselActive
        {
            get { return FlightGlobals.fetch != null && CurrentVessel != null; }
        }

        public static Boolean PauseMenuOpen
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

        public static Boolean FlightResultsDialogOpen
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

        //Does the active vessel have any manuever nodes
        public static Boolean ManeuverNodeExists
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

        public static ManeuverNode ManeuverNodeFuture
        {
            get
            {
                return CurrentVessel.patchedConicSolver.maneuverNodes.OrderBy(x => x.UT).FirstOrDefault(x => x.UT > KACWorkerGameState.CurrentTime.UT);
            }
        }

        public static List<ManeuverNode> ManeuverNodesFuture
        {
            get
            {
                return ( CurrentVessel.patchedConicSolver.maneuverNodes.OrderBy(x => x.UT).SkipWhile(x => x.UT < KACWorkerGameState.CurrentTime.UT).ToList<ManeuverNode>());
            }
        }


        public static Boolean SOIPointExists
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

        public static Boolean ApPointExists
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
        public static Boolean PePointExists
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
        public static void SetCurrentGUIStates()
        {
            KACWorkerGameState.CurrentGUIScene = HighLogic.LoadedScene;
        }

        public static void SetLastGUIStatesToCurrent()
        {
            KACWorkerGameState.LastGUIScene = KACWorkerGameState.CurrentGUIScene;
        }

        public static void SetCurrentFlightStates()
        {
            if (HighLogic.CurrentGame != null)
                KACWorkerGameState.CurrentSaveGameName = HighLogic.CurrentGame.Title;
            else
                KACWorkerGameState.CurrentSaveGameName = "";

            try { KACWorkerGameState.CurrentTime.UT = Planetarium.GetUniversalTime(); }
            catch (Exception) { }
            //if (Planetarium.fetch!=null) KACWorkerGameState.CurrentTime.UT = Planetarium.GetUniversalTime();

            if (KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT)
            {
                KACWorkerGameState.CurrentVessel = FlightGlobals.ActiveVessel;
                KACWorkerGameState.CurrentSOIBody = CurrentVessel.mainBody;
                KACWorkerGameState.CurrentVesselTarget = CurrentVessel.targetObject;
            }
            else if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION)
            {
                SpaceTracking st = (SpaceTracking) KACSpaceCenter.FindObjectOfType(typeof(SpaceTracking));
                if (st.mainCamera.target != null && st.mainCamera.target.type== MapObject.MapObjectType.VESSEL) {
                    KACWorkerGameState.CurrentVessel = st.mainCamera.target.vessel;
                    KACWorkerGameState.CurrentSOIBody = CurrentVessel.mainBody;
                    KACWorkerGameState.CurrentVesselTarget = CurrentVessel.targetObject;
                } else {
                    KACWorkerGameState.CurrentVessel = null;
                    KACWorkerGameState.CurrentSOIBody = null;
                    KACWorkerGameState.CurrentVesselTarget = null;
                }
            }
            //else if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION && 
            //        MapView.MapCamera.target.type== MapObject.MapObjectType.VESSEL)
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

        public static void SetLastFlightStatesToCurrent()
        {
            KACWorkerGameState.LastSaveGameName = KACWorkerGameState.CurrentSaveGameName;
            KACWorkerGameState.LastTime = KACWorkerGameState.CurrentTime;
            if (LastVessel != CurrentVessel) { if (VesselChanged != null) VesselChanged(LastVessel, CurrentVessel); }
            KACWorkerGameState.LastVessel = KACWorkerGameState.CurrentVessel;
            KACWorkerGameState.LastSOIBody = KACWorkerGameState.CurrentSOIBody;
            KACWorkerGameState.LastVesselTarget = KACWorkerGameState.CurrentVesselTarget;
        }

        internal delegate void VesselChangedHandler(Vessel OldVessel, Vessel NewVessel);
        internal static event VesselChangedHandler VesselChanged;
    }
}
