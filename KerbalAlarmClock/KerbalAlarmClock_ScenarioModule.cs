using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KerbalAlarmClock
{

    internal class KerbalAlarmClockScenario : ScenarioModule
    {
        public override void OnLoad(ConfigNode gameNode)
        {
            //reset the list here
            //KerbalAlarmClock.alarms = new KACAlarmList();
            KerbalAlarmClock.alarms.RemoveRange(0,KerbalAlarmClock.alarms.Count);

            base.OnLoad(gameNode);
            MonoBehaviourExtended.LogFormatted("BaseLoadDone. Alarms Count (Should be 0):{0}", KerbalAlarmClock.alarms.Count);

            MonoBehaviourExtended.LogFormatted_DebugOnly("OnLoad: ");
            MonoBehaviourExtended.LogFormatted_DebugOnly("{0}",gameNode);

            if (gameNode.HasNode("KerbalAlarmClockScenario")) MonoBehaviourExtended.LogFormatted_DebugOnly("Found {0}","KerbalAlarmClockScenario");
            if (gameNode.HasNode("KACAlarmListStorage")) MonoBehaviourExtended.LogFormatted_DebugOnly("Found {0}", "KACAlarmListStorage");
            if(gameNode.HasNode("KACAlarmListStorage"))
            {
                KerbalAlarmClock.alarms.DecodeFromCN(gameNode.GetNode("KACAlarmListStorage"));
            }

            MonoBehaviourExtended.LogFormatted("ScenarioLoadDone. Alarms Count:{0}", KerbalAlarmClock.alarms.Count);
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
