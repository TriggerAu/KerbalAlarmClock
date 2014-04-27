using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        internal static KACTime CurrentTime = new KACTime();
        internal static KACTime LastTime = new KACTime();

        internal static Boolean CurrentlyUnderWarpInfluence = false;
        internal static DateTime CurrentWarpInfluenceStartTime;

        //Are we flying any ship?
        internal static Boolean IsFlightMode
        {
            get { return FlightGlobals.fetch != null && FlightGlobals.ActiveVessel != null; }
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

        //Does the active vessel have any manuever nodes
        internal static Boolean ManeuverNodeExists
        {
            get
            {
                Boolean blnReturn = false;
                if (IsFlightMode)
                {
                    if (FlightGlobals.ActiveVessel.patchedConicSolver != null)
                    {
                        if (FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes != null)
                        {
                            if (FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Count > 0)
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
                return FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.OrderBy(x => x.UT).FirstOrDefault(x => x.UT >KACWorkerGameState.CurrentTime.UT);
            }
        }

        internal static List<ManeuverNode> ManeuverNodesFuture
        {
            get
            {
                return ( FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.OrderBy(x => x.UT).SkipWhile(x => x.UT <KACWorkerGameState.CurrentTime.UT).ToList<ManeuverNode>());
            }
        }


        internal static Boolean SOIPointExists
        {
            get
            {
                Boolean blnReturn = false;

                if (FlightGlobals.ActiveVessel != null)
                {
                    if (FlightGlobals.ActiveVessel.orbit != null)
                    {
                        List<Orbit.PatchTransitionType> SOITransitions = new List<Orbit.PatchTransitionType> { Orbit.PatchTransitionType.ENCOUNTER, Orbit.PatchTransitionType.ESCAPE };
                        blnReturn = SOITransitions.Contains(FlightGlobals.ActiveVessel.orbit.patchEndTransition);
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

                if (FlightGlobals.ActiveVessel != null)
                {
                    if (FlightGlobals.ActiveVessel.orbit != null)
                    {
                        if (FlightGlobals.ActiveVessel.orbit.timeToAp > 0
                            && ((CurrentTime.UT + FlightGlobals.ActiveVessel.orbit.timeToAp) < FlightGlobals.ActiveVessel.orbit.EndUT))
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

                if (FlightGlobals.ActiveVessel != null)
                {
                    if (FlightGlobals.ActiveVessel.orbit != null)
                    {
                        if (FlightGlobals.ActiveVessel.orbit.timeToPe > 0
                            && ((CurrentTime.UT + FlightGlobals.ActiveVessel.orbit.timeToPe) < FlightGlobals.ActiveVessel.orbit.EndUT))
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

            try {KACWorkerGameState.CurrentTime.UT = Planetarium.GetUniversalTime(); }
            catch (Exception) { }
            //if (Planetarium.fetch!=null)KACWorkerGameState.CurrentTime.UT = Planetarium.GetUniversalTime();

            if (KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT)
            {
               KACWorkerGameState.CurrentVessel = FlightGlobals.ActiveVessel;
               KACWorkerGameState.CurrentSOIBody = FlightGlobals.ActiveVessel.mainBody;
               KACWorkerGameState.CurrentVesselTarget = FlightGlobals.fetch.VesselTarget;
            }
            else
            {
               KACWorkerGameState.CurrentVessel = null;
               KACWorkerGameState.CurrentSOIBody = null;
               KACWorkerGameState.CurrentVesselTarget = null;
            }
        }

        internal static void SetLastFlightStatesToCurrent()
        {
           KACWorkerGameState.LastSaveGameName =KACWorkerGameState.CurrentSaveGameName;
           KACWorkerGameState.LastTime =KACWorkerGameState.CurrentTime;
           KACWorkerGameState.LastVessel =KACWorkerGameState.CurrentVessel;
           KACWorkerGameState.LastSOIBody =KACWorkerGameState.CurrentSOIBody;
           KACWorkerGameState.LastVesselTarget =KACWorkerGameState.CurrentVesselTarget;
        }
    }
}
