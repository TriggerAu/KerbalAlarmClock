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
	public partial class KerbalAlarmClock
	{
		private void NewEarthAlarm()
		{
			AddType = KACAlarm.AlarmTypeEnum.EarthTime;

			strAlarmName="Earth Calling...";
			strAlarmNotes="";

			AddNotesHeight = 50;

			strAlarmEarthHour = DateTime.Now.AddHours(2).Hour.ToString();
			strAlarmEarthMin = DateTime.Now.Minute.ToString();

			//AddAction = KACAlarm.AlarmActionEnum.PauseGame;
            AddActions = new AlarmActions(AlarmActions.WarpEnum.PauseGame, AlarmActions.MessageEnum.Yes, false, false);
            ///AddActionPlaySound = ??
		}

		internal void FillEarthAlarmWindow(int WindowID)
		{
			GUILayout.BeginVertical();

			intHeight_AddWindowCommon = 64;
			WindowLayout_CommonFields3(ref strAlarmName, ref blnAlarmAttachToVessel, ref AddActions, ref timeMargin, AddType, intHeight_AddWindowCommon);

			GUILayout.Label("Enter Time for reminder...", KACResources.styleAddSectionHeading);

			GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Time for Alarm (24 hour):",KACResources.styleAddHeading);
			GUILayout.FlexibleSpace();
			if (DrawTimeField(ref strAlarmEarthHour, "", 40, 0))
			{
				if (strAlarmEarthHour.Length > 2) strAlarmEarthHour = strAlarmEarthHour.Substring(strAlarmEarthHour.Length - 2, 2);
			}

			GUILayout.Label(":", KACResources.styleAlarmMessageTime, GUILayout.Width(3));
			//strAlarmEarthMin = GUILayout.TextField(strAlarmEarthMin, KACResources.styleAddField, GUILayout.Width(40));

			if (DrawTimeField(ref strAlarmEarthMin, "", 40, 0))
			{
				if (strAlarmEarthMin.Length > 2) strAlarmEarthMin = strAlarmEarthMin.Substring(strAlarmEarthMin.Length-2, 2);
			}

			GUILayout.EndHorizontal();
			GUILayout.EndVertical();

			try 
			{	        
				DateTime dteWorking;
				dteWorking=DateTime.ParseExact(strAlarmEarthHour + ":" + strAlarmEarthMin,"H:m",null);

				TimeSpan tmAlarm = (dteWorking.TimeOfDay - DateTime.Now.TimeOfDay);
				if (tmAlarm.TotalSeconds < 0) tmAlarm=tmAlarm.Add(new TimeSpan(24, 0, 0));

                KSPTimeSpan TimeToAlarm = new KSPTimeSpan(tmAlarm.TotalSeconds);

				//Bit at the bottom to add an alarm
				int intLineHeight = 18;
				GUILayout.BeginHorizontal(KACResources.styleAddAlarmArea);
				GUILayout.BeginVertical();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Date:", KACResources.styleAddHeading, GUILayout.Height(intLineHeight), GUILayout.Width(40), GUILayout.MaxWidth(40));
				GUILayout.Label(dteWorking.ToLongTimeString(), KACResources.styleContentEarth, GUILayout.Height(intLineHeight));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Time to Alarm:", KACResources.styleAddHeading, GUILayout.Height(intLineHeight), GUILayout.Width(100), GUILayout.MaxWidth(100));
                GUILayout.Label(TimeToAlarm.ToStringStandard(TimeSpanStringFormatsEnum.DateTimeFormatLong), KACResources.styleContentEarth, GUILayout.Height(intLineHeight));
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();

				GUILayout.Space(10);
				int intButtonHeight = 36;
				if (GUILayout.Button("Add Alarm", KACResources.styleButton, GUILayout.Width(90), GUILayout.Height(intButtonHeight)))
				{
					alarms.Add(
						new KACAlarm(null,strAlarmName,strAlarmNotes,
							EarthTimeEncode(DateTime.Now + tmAlarm),
							0, KACAlarm.AlarmTypeEnum.EarthTime,
							AddActions)
						);
					//settings.SaveAlarms();
					_ShowEarthAlarm = false;
				}
				GUILayout.EndHorizontal();
			}
			catch (Exception)
			{
				GUILayout.Label("Unable to determine Earth Time", GUILayout.ExpandWidth(true));
			}


			GUILayout.EndVertical();
			
			SetTooltipText();
		}


		private DateTime EarthTimeRoot = new DateTime(2013, 1, 1);

		internal Double EarthTimeEncode(DateTime Input)
		{
			Double dblReturn;

			dblReturn = (Input - EarthTimeRoot).TotalSeconds;

			return dblReturn;
		}

		internal DateTime EarthTimeDecode(Double Input)
		{
			DateTime dteReturn;

			dteReturn = EarthTimeRoot.AddSeconds(Input);

			return dteReturn;
		}
	}
}
