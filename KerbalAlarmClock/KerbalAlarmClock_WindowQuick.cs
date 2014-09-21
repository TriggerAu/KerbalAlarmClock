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

            lstQuickButtons.Add(new QuickAddItem("Raw Alarm (10 Min)", KACResources.iconRaw,QuickAddRaw));
            lstQuickButtons.Add(new QuickAddItem("Earth Alarm (1 Hr)", KACResources.iconEarth));
        }

        List<QuickAddItem> lstQuickButtons;

        private int intHeight_QuickWindow;
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
        }


        private void DrawQuickOption(QuickAddItem item)
        {
            if (GUILayout.Button(item.Text, KACResources.styleQAListButton))
            {
                if (item.ActionToCall!=null){
                    item.ActionToCall.Invoke();
                }
                _ShowQuickAdd = false;
            }
            if (Event.current.type == EventType.repaint)
                item.ButtonRect = GUILayoutUtility.GetLastRect();
            GUI.Box(new Rect(item.ButtonRect.x + 8, item.ButtonRect.y + 3, 18, 14), item.Icon, new GUIStyle());
        }

        private void QuickAddRaw()
        {
            KACAlarm tmpAlarm = new KACAlarm();
            tmpAlarm.TypeOfAlarm= KACAlarm.AlarmType.Raw;
            tmpAlarm.AlarmTime = new KACTime(KACWorkerGameState.CurrentTime.UT + 600);
            tmpAlarm.Name = "Quick Raw";
            //if (KACWorkerGameState.IsVesselActive)
            //    tmpAlarm.VesselID = KACWorkerGameState.CurrentVessel.id.ToString();

            
            alarms.Add(tmpAlarm);
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
            internal QuickAddItem(String Text, Texture2D Icon,Action FuncToCall)
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
            internal Action ActionToCall;
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
