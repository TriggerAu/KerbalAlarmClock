using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KerbalAlarmClock
{
    internal static class WarpTransitionCalculator
    {
        #region Warp Rates Stuff
        /// <summary>
        /// List of How long each transition takes - from index to index+1 and vice versa
        /// </summary>
        internal static List<WarpTransition> WarpRateTransitionPeriods = new List<WarpTransition>();

        internal static String WarpRateHash = "";
        internal static Boolean CheckForTransitionChanges()
        {
            if (TimeWarp.fetch == null)
            {
                return false;
            }
            //check to see if the warp rates are still the same
            String NewHash = "";

            for (int i = 0; i < TimeWarp.fetch.warpRates.Length; i++) {
                NewHash += TimeWarp.fetch.warpRates[i].ToString() + "|";
            }
            
            //if not then work out the time to change rates values
            if (NewHash == WarpRateHash) {
                return false;
            } else {
                WarpRateHash = NewHash;
                CalcWarpRateTransitions();
                return true;
            }
        }

        internal static void CalcWarpRateTransitions()
        {
            if (TimeWarp.fetch == null)
            {
                return;
            }
            MonoBehaviourExtended.LogFormatted("WarpRates:{0}", TimeWarp.fetch.warpRates.Length);
            WarpRateTransitionPeriods = new List<WarpTransition>();
            for (int i = 0; i < TimeWarp.fetch.warpRates.Length; i++)
            {
                WarpTransition newRate = new WarpTransition(i, TimeWarp.fetch.warpRates[i]);
                
                if (i>0) {
                    newRate.UTToRateDown = (TimeWarp.fetch.warpRates[i] + TimeWarp.fetch.warpRates[i-1]) / 2;
                }
                if (i<TimeWarp.fetch.warpRates.Length-1) {
                    newRate.UTToRateUp = (TimeWarp.fetch.warpRates[i] + TimeWarp.fetch.warpRates[i+1]) / 2;
                }

                WarpRateTransitionPeriods.Add(newRate);
            }

            foreach (WarpTransition wt in WarpRateTransitionPeriods)
            {
                List<WarpTransition> warpsTo1Times = WarpRateTransitionPeriods.Where(w => w.Index <= wt.Index).ToList();
                wt.UTTo1Times = warpsTo1Times.Sum(w => w.UTToRateDown);
            }

            //foreach (WarpTransition item in WarpRateTransitionPeriods.OrderBy(w => w.Index))
            //{
            //    MonoBehaviourExtended.LogFormatted_DebugOnly("{0}({1}):Up-{2} Down-{3} To0-{4}", item.Rate, item.Index, item.UTToRateUp, item.UTToRateDown, item.UTTo1Times);
            //}
        }

        internal static Double UTToRateDown(int FromIndex, int ToIndex)
        {
            return WarpRateTransitionPeriods.First(w => w.Index == FromIndex).UTTo1Times - WarpRateTransitionPeriods.First(w => w.Index == ToIndex).UTTo1Times;
        }
        internal static Double UTToRateDownOne
        {
            get
            {
                return UTToRateDown(TimeWarp.CurrentRateIndex, Math.Max(TimeWarp.CurrentRateIndex - 1, 0));
            }
        }
        internal static Double UTToRateTimesOne
        {
            get
            {
                return UTToRateDown(TimeWarp.CurrentRateIndex, 0);
            }
        }


        #endregion


    }

    internal class WarpTransition
    {
        internal WarpTransition(Int32 Index, Double Rate)
        {
            this.Index = Index;
            this.Rate = Rate;

            UTTo1Times = UTToRateDown = UTToRateUp = 0;
        }
        public Int32 Index { get; private set; }
        public Double Rate { get; private set; }
        public Double UTTo1Times { get; set; }
        public Double UTToRateDown { get; set; }
        public Double UTToRateUp { get; set; }
    }

}
