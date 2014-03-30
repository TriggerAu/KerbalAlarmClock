using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;

using UnityEngine;
using KSP;
using KSPPluginFramework;

namespace KerbalAlarmClock
{
    [WindowInitials(ClampToScreen=true,
                    DragEnabled=true,
                    TooltipsEnabled=true,
                    Caption="Kerbal Alarm Clock")]
    class KACWindow:MonoBehaviourWindowPlus
    {
        internal KerbalAlarmClock mbKAC;
        private Settings settings;
        
        internal override void DrawWindow(int id)
        {
            


        }
    }
}
