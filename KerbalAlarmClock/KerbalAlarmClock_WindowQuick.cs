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


        internal void SetupQuickList()
        {
            //what situation is the game in - and therefore what quick options are there?
            lstQuickButtons = new List<QuickAddItem>();
            if (KACWorkerGameState.CurrentVessel != null)
            {
                if (KACWorkerGameState.ManeuverNodeExists && KACWorkerGameState.ManeuverNodeFuture != null)
                    lstQuickButtons.Add(new QuickAddItem(String.Format("Maneuver Alarm ({0})", (new KSPTimeSpan(settings.AlarmAddManQuickMargin + GetBurnMarginSecs(settings.DefaultKERMargin)).ToString(6))), KACResources.iconMNode, QuickAddManNode));

                if (KACWorkerGameState.SOIPointExists )
                    lstQuickButtons.Add(new QuickAddItem(String.Format("SOI Change Alarm ({0})", (new KSPTimeSpan(settings.AlarmAddSOIQuickMargin).ToString(6))), KACResources.iconSOI, QuickAddSOI));

                if (KACWorkerGameState.ApPointExists && !KACWorkerGameState.CurrentVessel.LandedOrSplashed)
                    lstQuickButtons.Add(new QuickAddItem(String.Format("Apoapsis Alarm ({0})", (new KSPTimeSpan(settings.AlarmAddNodeQuickMargin).ToString(6))), KACResources.iconAp, QuickAddAp));

                if (KACWorkerGameState.PePointExists && !KACWorkerGameState.CurrentVessel.LandedOrSplashed)
                    lstQuickButtons.Add(new QuickAddItem(String.Format("Periapsis Alarm ({0})", (new KSPTimeSpan(settings.AlarmAddNodeQuickMargin).ToString(6))), KACResources.iconPe, QuickAddPe));

                if (KACWorkerGameState.CurrentVesselTarget != null) { 
                    if (KACWorkerGameState.CurrentVessel.orbit.AscendingNodeExists(KACWorkerGameState.CurrentVesselTarget.GetOrbit()))
                        lstQuickButtons.Add(new QuickAddItem(String.Format("Ascending Node Alarm ({0})", (new KSPTimeSpan(settings.AlarmAddNodeQuickMargin).ToString(6))), KACResources.iconAN, QuickAddAN));

                    if (KACWorkerGameState.CurrentVessel.orbit.AscendingNodeExists(KACWorkerGameState.CurrentVesselTarget.GetOrbit()))
                        lstQuickButtons.Add(new QuickAddItem(String.Format("Descending Node Alarm ({0})", (new KSPTimeSpan(settings.AlarmAddNodeQuickMargin).ToString(6))), KACResources.iconDN, QuickAddDN));
                }
            }
            lstQuickButtons.Add(new QuickAddItem("Raw Alarm (10 Min)", KACResources.iconRaw, QuickAddRaw));

            QuickAddItem qkEarth = new QuickAddItem("Earth Alarm (1 Hr)", KACResources.iconEarth, QuickAddEarth60);
            qkEarth.AllowAddAndWarp = false;
            lstQuickButtons.Add(qkEarth);

            QuickWindowHeight = 28 + (24 * lstQuickButtons.Count);
            if (settings.SelectedSkin != Settings.DisplaySkin.Default)
                QuickWindowHeight -= 8;
        }

        List<QuickAddItem> lstQuickButtons;

        /// <summary>
        /// Draw the Add Window contents
        /// </summary>
        /// <param name="WindowID"></param>
        internal void FillQuickWindow(int WindowID)
        {
            GUILayout.BeginVertical();

            foreach (QuickAddItem item in lstQuickButtons)
            {
                DrawQuickOption(item);
            }
            GUILayout.EndVertical();

            SetTooltipText();
        }


        private void DrawQuickOption(QuickAddItem item)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(item.Text, KACResources.styleQAListButton))
            {
                if (item.ActionToCall!=null){
                    item.ActionToCall.Invoke();
                }
                _ShowQuickAdd = false;
            }
            if (Event.current.type == EventType.Repaint)
                item.ButtonRect = GUILayoutUtility.GetLastRect();
            GUI.Box(new Rect(item.ButtonRect.x + 8, item.ButtonRect.y + 3, 18, 14), item.Icon, new GUIStyle());

            if (item.AllowAddAndWarp)
            {
                GUILayout.Space(-5);

                GUIContent contButton = new GUIContent(">>", "Warp to " + item.Text);
                if (GUILayout.Button(contButton, KACResources.styleQAListButton, GUILayout.Width(30)))
                {
                    if (item.ActionToCall != null)
                    {
                        KACAlarm newAlarm = item.ActionToCall.Invoke();

                        LogFormatted("Creating Alarm and setting warp rate-Remaining Time:{0}", newAlarm.Remaining.UT);

                        Int32 intRate = TimeWarp.fetch.warpRates.Length - 1;
                        while (intRate > 0 && (TimeWarp.fetch.warpRates[intRate] * 2) > newAlarm.Remaining.UT)
                        {
                            intRate -= 1;
                        }
                        LogFormatted("Setting Rate to {0}={1}x", intRate, TimeWarp.fetch.warpRates[intRate]);

                        TimeWarp.fetch.Mode = TimeWarp.Modes.HIGH;
                        //Make sure we cancel autowarp if its engaged
                        TimeWarp.fetch.CancelAutoWarp();
                        TimeWarp.SetRate(intRate, false);
                    }
                    _ShowQuickAdd = false;
                }
            }
            GUILayout.EndHorizontal();
        }

        private KACAlarm QuickAddRaw()
        {
            KACAlarm tmpAlarm = new KACAlarm(KACWorkerGameState.CurrentTime.UT + 600);
            tmpAlarm.TypeOfAlarm= KACAlarm.AlarmTypeEnum.Raw;
            tmpAlarm.Name = "Quick Raw";
            if (KACWorkerGameState.IsVesselActive)
                tmpAlarm.VesselID = KACWorkerGameState.CurrentVessel.id.ToString();

            
            alarms.Add(tmpAlarm);

            return tmpAlarm;
        }

        private KACAlarm QuickAddEarth60()
        {
            return QuickAddEarth(60);
        }
        private KACAlarm QuickAddEarth(Int32 Minutes)
        {
            KACAlarm tmpAlarm = new KACAlarm(EarthTimeEncode(DateTime.Now.AddMinutes(Minutes)));
            tmpAlarm.TypeOfAlarm = KACAlarm.AlarmTypeEnum.EarthTime;
            tmpAlarm.Name = "Quick Earth Alarm";

            alarms.Add(tmpAlarm);

            return tmpAlarm;
        }

        private KACAlarm QuickAddManNode()
        {
            KACAlarm tmpAlarm = new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(),
                KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + " Maneuver",
                "Quick Added Maneuver Alarm",
                KACWorkerGameState.ManeuverNodeFuture.UT - settings.AlarmAddManQuickMargin,
                settings.AlarmAddManQuickMargin,
                KACAlarm.AlarmTypeEnum.Maneuver,
                settings.AlarmAddManQuickAction,
                KACWorkerGameState.ManeuverNodesFuture);

            alarms.Add(tmpAlarm);

            return tmpAlarm;
        }

        private KACAlarm QuickAddSOI()
        {
            KACAlarm tmpAlarm = new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(),
                KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + " SOI Change",
                "Quick Added SOI Change Alarm",
                KACWorkerGameState.CurrentVessel.orbit.UTsoi - settings.AlarmAddSOIQuickMargin,
                settings.AlarmAddSOIQuickMargin,
                KACAlarm.AlarmTypeEnum.SOIChange,
                settings.AlarmAddSOIQuickAction);

            alarms.Add(tmpAlarm);

            return tmpAlarm;
        }

        private KACAlarm QuickAddAp()
        {
            KACAlarm tmpAlarm = new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(),
                KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + " Apopasis",
                "Quick Added Apoapsis Alarm",
                KACWorkerGameState.CurrentTime.UT + KACWorkerGameState.CurrentVessel.orbit.timeToAp - settings.AlarmAddNodeQuickMargin,
                settings.AlarmAddNodeQuickMargin,
                KACAlarm.AlarmTypeEnum.Apoapsis,
                settings.AlarmAddNodeQuickAction);
            alarms.Add(tmpAlarm);

            return tmpAlarm;
        }

        private KACAlarm QuickAddPe()
        {
            KACAlarm tmpAlarm = new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(),
                KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + " Periapsis",
                "Quick Added Periapsis Alarm",
                KACWorkerGameState.CurrentTime.UT + KACWorkerGameState.CurrentVessel.orbit.timeToPe - settings.AlarmAddNodeQuickMargin,
                settings.AlarmAddNodeQuickMargin,
                KACAlarm.AlarmTypeEnum.Periapsis,
                settings.AlarmAddNodeQuickAction);
            alarms.Add(tmpAlarm);

            return tmpAlarm;
        }

        private KACAlarm QuickAddAN()
        {
            KACAlarm tmpAlarm = new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(),
                KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + " Ascending",
                "Quick Added Ascending Node",
                KACWorkerGameState.CurrentVessel.orbit.TimeOfAscendingNode(KACWorkerGameState.CurrentVesselTarget.GetOrbit(), KACWorkerGameState.CurrentTime.UT) - settings.AlarmAddNodeQuickMargin,
                settings.AlarmAddNodeQuickMargin,
                KACAlarm.AlarmTypeEnum.AscendingNode,
                settings.AlarmAddNodeQuickAction);
            alarms.Add(tmpAlarm);

            return tmpAlarm;
        }

        private KACAlarm QuickAddDN()
        {
            KACAlarm tmpAlarm = new KACAlarm(KACWorkerGameState.CurrentVessel.id.ToString(),
                KSP.Localization.Localizer.Format(KACWorkerGameState.CurrentVessel.vesselName) + " Descending",
                "Quick Added Descending Node",
                KACWorkerGameState.CurrentVessel.orbit.TimeOfDescendingNode(KACWorkerGameState.CurrentVesselTarget.GetOrbit(), KACWorkerGameState.CurrentTime.UT) - settings.AlarmAddNodeQuickMargin,
                settings.AlarmAddNodeQuickMargin,
                KACAlarm.AlarmTypeEnum.DescendingNode,
                settings.AlarmAddNodeQuickAction);
            alarms.Add(tmpAlarm);

            return tmpAlarm;
        }




        //public class DropDownItemList : List<DropDownItem>
        //{
        //    public static DropDownItemList FromStringList(List<String> Items){
        //        DropDownItemList lstReturn = new DropDownItemList();
        //        foreach (String item in Items)
        //        {
        //            lstReturn.Add(new DropDownItem(item));
        //        }
        //        return lstReturn;
        //    } 

        //    public List<String> ToStringList()
        //    {
        //        return this.Select(x => x.Text).ToList();
        //    }
        //}

        internal class QuickAddItem
        {
            internal QuickAddItem(String Text, Texture2D Icon,Func<KACAlarm> FuncToCall)
                : this(Text,Icon)
            {
                this.ActionToCall = FuncToCall;
            }
            internal QuickAddItem(String Text, Texture2D Icon)
                : this(Text)
            {
                this.Icon = Icon;
            }
            internal QuickAddItem(String Text)
                : this()
            {
                this.Text = Text;
            }
            internal QuickAddItem(Texture2D Icon)
                : this()
            {
                this.Icon = Icon;
            }
            internal QuickAddItem() { }

            private String _Text;
            internal String Text
            {
                get { return _Text; }
                set { _Text = value; setContent(); }
            }
            private Texture2D _Icon;
            internal Texture2D Icon
            {
                get { return _Icon; }
                set { _Icon = value; setContent(); }
            }
            private void setContent()
            {
                if (Icon != null && Text != "")
                {
                    Content = new GUIContent(Text, Icon);
                }
                else if (Icon != null)
                {
                    Content = new GUIContent(Icon);
                }
                else
                {
                    Content = new GUIContent(Text);
                }
            }
            internal GUIContent Content { get; private set; }

            internal Rect ButtonRect;
            internal Boolean AllowAddAndWarp = true;

            internal Func<KACAlarm> ActionToCall;
        }

        //[Flags]
        //public enum DropDownListDisplayStyleEnum
        //{
        //    TextOnly = 1,
        //    ImageOnly = 2,
        //    Both = 3
        //}
    }
}
