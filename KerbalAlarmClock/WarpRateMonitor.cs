using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalAlarmClock
{
    static class WarpToMonitor
    {

        internal static Int32 LastWarpRate = 0;
        internal static WarpChangeEnum LastWarpChange;
        internal static void onTimeWarpRateChanged()
        {
            LastWarpChange = TimeWarp.CurrentRateIndex < LastWarpRate ? WarpChangeEnum.Slower : WarpChangeEnum.Faster;

            // if warp rate is slowing
            if (LastWarpChange == WarpChangeEnum.Slower) {
                //and it wasnt slowed by KAC WarpTo
                if (true) {
                    //turn off WarpTo behaviour
                    KerbalAlarmClock.WarpToActive = false;
                }
            }

            LastWarpRate = TimeWarp.CurrentRateIndex;
        }

        internal enum WarpChangeEnum
	    {
                Faster,
                Slower
	    }


        internal static void RepeatingWorkerCheck()
        {
            //if in the next x secs we will be below a certain altitude then slow the warp rate and set a flag so the transition event doesnt koll the increase


            //if we can increase spead then lets go baby!
        }
    }
}
