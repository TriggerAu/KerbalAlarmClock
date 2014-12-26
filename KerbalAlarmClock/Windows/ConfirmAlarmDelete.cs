using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;

using KSPPluginFramework;

namespace KerbalAlarmClock.Windows
{
    class ConfirmAlarmDelete
    {

        internal Int32 windowID;
        internal Rect windowRect;
        internal Boolean Visible;

        internal KACAlarm AlarmToConfirm;

        internal void InitWindow()
        {
            windowID = UnityEngine.Random.Range(1000, 2000000) + KerbalAlarmClock._AssemblyName.GetHashCode();
            windowRect = new Rect(0, 0, 400, 200);
            Visible = false;
        }
        internal void FillWindow(Int32 windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Are you sure you want to delete?");
            GUILayout.Label(AlarmToConfirm.Name);
            GUILayout.Label(AlarmToConfirm.Remaining.ToStringStandard(TimeSpanStringFormatsEnum.IntervalLongTrimYears));
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel")) {
                Visible = false;
            }
            if (GUILayout.Button(new GUIContent("Delete", KACResources.btnRedCross))) {
                Visible = false;
                KerbalAlarmClock.alarms.Remove(AlarmToConfirm);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

    }
}
