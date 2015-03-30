using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSPPluginFramework;

namespace KerbalAlarmClock
{
    internal class WarpToMonitor
    {
        internal static Double dblLeadSecs = 3;

        internal static Int32 LastWarpRate = 0;
        internal static WarpChangeEnum LastWarpChange;
        internal void onTimeWarpRateChanged()
        {
            MonoBehaviourExtended.LogFormatted_DebugOnly("Caught Warp Change. Last:{0} New:{1}", LastWarpRate, TimeWarp.CurrentRateIndex);
            LastWarpChange = TimeWarp.CurrentRateIndex < LastWarpRate ? WarpChangeEnum.Slower : WarpChangeEnum.Faster;

            // if warp rate is slowing
            if (LastWarpChange == WarpChangeEnum.Slower) {
                //and it wasnt slowed by KAC WarpTo
                if (!DownShiftTriggered_WarpTo) {
                    ScreenMessages.PostScreenMessage("Downshift Detected");
                    //turn off WarpTo behaviour
                    KerbalAlarmClock.WarpToActive = false;
                } else {
                    //Clear the downshift flag
                    DownShiftTriggered_WarpTo = false;
                    ScreenMessages.PostScreenMessage("WarpTo Downshift Detected");
                }
            }

            LastWarpRate = TimeWarp.CurrentRateIndex;
        }

        internal enum WarpChangeEnum
	    {
                Faster,
                Slower
	    }

        internal static Boolean DownShiftTriggered_WarpTo = false;
        internal static Boolean DownShiftTriggered_Remaining = false;
        internal static void RepeatingWorkerCheck()
        {
            Double AltitudeFuture = FlightGlobals.ActiveVessel.orbit.getRelativePositionAtUT(Planetarium.GetUniversalTime()+dblLeadSecs).magnitude -
                                    FlightGlobals.ActiveVessel.orbit.referenceBody.Radius;

            //if in the next x secs we will be below a certain altitude then slow the warp rate and set a flag so the transition event doesnt koll the increase
            if (AltitudeFuture < TimeWarp.fetch.altitudeLimits[TimeWarp.CurrentRateIndex]) {
                DownShiftTriggered_WarpTo = true;
                ScreenMessages.PostScreenMessage("WarpTo Downshift Triggered");
                TimeWarp.SetRate(TimeWarp.CurrentRateIndex - 1, false);
            }
            //if we can increase spead then lets go baby!
            else if (TimeWarp.CurrentRateIndex<TimeWarp.fetch.warpRates.GetUpperBound(0) &&
                FlightGlobals.ActiveVessel.orbit.altitude > TimeWarp.fetch.altitudeLimits[TimeWarp.CurrentRateIndex+1] &&
                AltitudeFuture > TimeWarp.fetch.altitudeLimits[TimeWarp.CurrentRateIndex+1]) {

                ScreenMessages.PostScreenMessage("WarpTo Upshift Triggered");

                TimeWarp.SetRate(TimeWarp.CurrentRateIndex + 1, false);
            }
        }
    }
}
