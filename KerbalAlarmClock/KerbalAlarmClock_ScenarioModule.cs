using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KerbalAlarmClock
{

    class KerbalAlarmClockScenario : ScenarioModule
    {
        public override void OnLoad(ConfigNode gameNode)
        {
            base.OnLoad(gameNode);
            MonoBehaviourExtended.LogFormatted_DebugOnly("OnLoad: ");
            MonoBehaviourExtended.LogFormatted_DebugOnly("{0}",gameNode);
            //if(gameNode.HasNodeID("KACAlarmListStorage"))
            //{MonoBehaviourExtended.LogFormatted_DebugOnly("A");} else {MonoBehaviourExtended.LogFormatted_DebugOnly("B");}
            //KerbalAlarmClock.alarms.DecodeFromCN(gameNode.GetNode(this.GetType().Name));
        }

        public override void OnSave(ConfigNode gameNode)
        {
            base.OnSave(gameNode);
            MonoBehaviourExtended.LogFormatted_DebugOnly("OnSave: ");
            MonoBehaviourExtended.LogFormatted_DebugOnly("{0}", gameNode);
            gameNode.AddNode(KerbalAlarmClock.alarms.EncodeToCN());
        }
    }
}
