using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KerbalAlarmClock
{
    public partial class KerbalAlarmClock
    {
        internal DropDownListManager ddlManager = new DropDownListManager();

        private DropDownList ddlChecksPerSec;
        private DropDownList ddlSettingsSkin;
        private DropDownList ddlSettingsButtonStyle;

        private DropDownList ddlSettingsAlarmSpecs;
        private DropDownList ddlSettingsCalendar;

        private DropDownList ddlSettingsContractAutoOffered;
        private DropDownList ddlSettingsContractAutoActive;

        private DropDownList ddlKERNodeMargin;
        private DropDownList ddlSettingsKERNodeMargin;

        //private DropDownList ddlAddAlarm;

        private SettingsAlarmSpecsEnum SettingsAlarmSpecSelected = SettingsAlarmSpecsEnum.Default;
        internal enum SettingsAlarmSpecsEnum
        {
            [Description("All Alarm Defaults")] Default,
            [Description("Maneuver Node Alarms")] ManNode,
            [Description("Sphere Of Influence Alarms")] SOI,
            [Description("Contract Alarms")] Contract,
            [Description("Warp To Alarms")] WarpTo,
            [Description("Other Alarms")]Other
        }

        internal void InitDropDowns()
        {
            String[] strChecksPerSecChoices =  { "10","20","50","100","Custom (" + settings.BehaviourChecksPerSec_Custom.ToString() + ")"};

            ddlChecksPerSec = new DropDownList(strChecksPerSecChoices,_WindowSettingsRect);
            ddlChecksPerSec.OnSelectionChanged += ddlChecksPerSec_OnSelectionChanged;

            ddlSettingsSkin = new DropDownList(KSPPluginFramework.EnumExtensions.ToEnumDescriptions<Settings.DisplaySkin>(), (Int32)settings.SelectedSkin, _WindowSettingsRect);
            ddlSettingsSkin.OnSelectionChanged += ddlSettingsSkin_OnSelectionChanged;

            ddlSettingsButtonStyle = new DropDownList(KSPPluginFramework.EnumExtensions.ToEnumDescriptions<Settings.ButtonStyleEnum>(), (Int32)settings.ButtonStyleChosen, _WindowSettingsRect);
            ddlSettingsButtonStyle.OnSelectionChanged += ddlSettingsButtonStyle_OnSelectionChanged;

            ddlSettingsAlarmSpecs = new DropDownList(KSPPluginFramework.EnumExtensions.ToEnumDescriptions<SettingsAlarmSpecsEnum>(), (int)SettingsAlarmSpecSelected, _WindowSettingsRect);
            ddlSettingsAlarmSpecs.OnSelectionChanged += ddlSettingsAlarmSpecs_OnSelectionChanged;

            ddlSettingsContractAutoOffered = new DropDownList(KSPPluginFramework.EnumExtensions.ToEnumDescriptions<Settings.AutoContractBehaviorEnum>(), (Int32)settings.AlarmAddContractAutoOffered, _WindowSettingsRect);
            ddlSettingsContractAutoOffered.OnSelectionChanged += ddlSettingsContractAutoOffered_OnSelectionChanged;
            ddlSettingsContractAutoActive = new DropDownList(KSPPluginFramework.EnumExtensions.ToEnumDescriptions<Settings.AutoContractBehaviorEnum>(), (Int32)settings.AlarmAddContractAutoActive, _WindowSettingsRect);
            ddlSettingsContractAutoActive.OnSelectionChanged += ddlSettingsContractAutoActive_OnSelectionChanged;

            ddlSettingsCalendar = new DropDownList(KSPPluginFramework.EnumExtensions.ToEnumDescriptions<CalendarTypeEnum>(), (Int32)settings.SelectedCalendar,_WindowSettingsRect );
            //NOTE:Pull out the custom option for now
            ddlSettingsCalendar.Items.Remove(CalendarTypeEnum.Custom.Description());
            ddlSettingsCalendar.OnSelectionChanged += ddlSettingsCalendar_OnSelectionChanged;

            ddlKERNodeMargin = new DropDownList(KSPPluginFramework.EnumExtensions.ToEnumDescriptions<Settings.BurnMarginEnum>(), _WindowAddRect);
            ddlSettingsKERNodeMargin = new DropDownList(KSPPluginFramework.EnumExtensions.ToEnumDescriptions<Settings.BurnMarginEnum>(), (int)settings.DefaultKERMargin, _WindowSettingsRect);
            ddlSettingsKERNodeMargin.OnSelectionChanged += ddlSettingsKERNodeMargin_OnSelectionChanged;


            //ddlAddAlarm = LoadSoundsListForAdd(KACResources.clipAlarms.Keys.ToArray(), settings.AlarmsSoundName);
            //ddlAddAlarm.OnSelectionChanged += ddlAddAlarm_OnSelectionChanged;

            ddlManager.AddDDL(ddlChecksPerSec);
            ddlManager.AddDDL(ddlSettingsSkin);
            ddlManager.AddDDL(ddlSettingsButtonStyle);
            ddlManager.AddDDL(ddlSettingsAlarmSpecs);
            ddlManager.AddDDL(ddlSettingsContractAutoOffered);
            ddlManager.AddDDL(ddlSettingsContractAutoActive);
            ddlManager.AddDDL(ddlSettingsCalendar);
            ddlManager.AddDDL(ddlKERNodeMargin);
            ddlManager.AddDDL(ddlSettingsKERNodeMargin);
            //ddlManager.AddDDL(ddlAddAlarm);
        }

        internal void ConfigSoundsDDLs()
        {
            foreach (AlarmSound s in settings.AlarmSounds)
            {
                s.ddl = LoadSoundsListForDDL(KACResources.clipAlarms.Keys.ToArray(), s.SoundName);
                s.ddl.OnSelectionChanged+=ddlSettingsSound_OnSelectionChanged;
                ddlManager.AddDDL(s.ddl);
            }
        }


        internal void DestroyDropDowns()
        {
            ddlChecksPerSec.OnSelectionChanged -= ddlChecksPerSec_OnSelectionChanged;
            ddlSettingsSkin.OnSelectionChanged -= ddlSettingsSkin_OnSelectionChanged;
            ddlSettingsButtonStyle.OnSelectionChanged -= ddlSettingsButtonStyle_OnSelectionChanged;
            ddlSettingsAlarmSpecs.OnSelectionChanged -= ddlSettingsAlarmSpecs_OnSelectionChanged;
            ddlSettingsContractAutoOffered.OnSelectionChanged -= ddlSettingsContractAutoOffered_OnSelectionChanged;
            ddlSettingsContractAutoActive.OnSelectionChanged -= ddlSettingsContractAutoActive_OnSelectionChanged;
            ddlSettingsCalendar.OnSelectionChanged -= ddlSettingsCalendar_OnSelectionChanged;
            ddlSettingsKERNodeMargin.OnSelectionChanged -= ddlSettingsKERNodeMargin_OnSelectionChanged;

            foreach (AlarmSound s in settings.AlarmSounds)
            {
                if (s.ddl != null)
                    s.ddl.OnSelectionChanged -= ddlSettingsSound_OnSelectionChanged;
            }
        }

        internal void SetDDLWindowPositions()
        {
            ddlChecksPerSec.WindowRect = _WindowSettingsRect;
            ddlSettingsSkin.WindowRect = _WindowSettingsRect;
            ddlSettingsButtonStyle.WindowRect = _WindowSettingsRect;
            ddlSettingsAlarmSpecs.WindowRect = _WindowSettingsRect;
            ddlSettingsContractAutoOffered.WindowRect = _WindowSettingsRect;
            ddlSettingsContractAutoActive.WindowRect = _WindowSettingsRect;
            ddlSettingsCalendar.WindowRect = _WindowSettingsRect;
            ddlKERNodeMargin.WindowRect = _WindowAddRect;
            ddlSettingsKERNodeMargin.WindowRect = _WindowSettingsRect;
            

            foreach (AlarmSound s in settings.AlarmSounds)
            {
                if (s.ddl != null)
                    s.ddl.WindowRect = _WindowSettingsRect;
            }
        }

        #region DDLEvents code

        void ddlChecksPerSec_OnSelectionChanged(KerbalAlarmClock.DropDownList sender, int OldIndex, int NewIndex)
        {
            switch (NewIndex)
            {
                case 0: settings.BehaviourChecksPerSec = 10; break;
                case 1: settings.BehaviourChecksPerSec = 20; break;
                case 2: settings.BehaviourChecksPerSec = 50; break;
                case 3: settings.BehaviourChecksPerSec = 100; break;
                default: settings.BehaviourChecksPerSec = settings.BehaviourChecksPerSec_Custom; break;
            }
            StartRepeatingWorker(settings.BehaviourChecksPerSec);
            settings.Save();
        }

        void ddlSettingsSkin_OnSelectionChanged(KerbalAlarmClock.DropDownList sender, int OldIndex, int NewIndex)
        {
            settings.SelectedSkin = (Settings.DisplaySkin)NewIndex;
            KACResources.SetSkin(settings.SelectedSkin);
            settings.Save();
        }
        void ddlSettingsButtonStyle_OnSelectionChanged(DropDownList sender, int OldIndex, int NewIndex)
        {
            settings.ButtonStyleChosen = (Settings.ButtonStyleEnum)NewIndex;
            settings.Save();

            //destroy Old Objects
            switch ((Settings.ButtonStyleEnum)OldIndex)
            {
                case Settings.ButtonStyleEnum.Toolbar:
                    DestroyToolbarButton(btnToolbarKAC);
                    break;
                case Settings.ButtonStyleEnum.Launcher:
                    DestroyAppLauncherButton();
                    break;
            }

            //Create New ones
            switch ((Settings.ButtonStyleEnum)NewIndex)
            {
                case Settings.ButtonStyleEnum.Toolbar:
                    btnToolbarKAC = InitToolbarButton();
                    break;
                case Settings.ButtonStyleEnum.Launcher:
                    btnAppLauncher = InitAppLauncherButton();
                    if (WindowVisibleByActiveScene) {
                        AppLauncherToBeSetTrueAttemptDate = DateTime.Now;
                        AppLauncherToBeSetTrue = true;
                    }
                        
                    break;
            }
        }
        void ddlSettingsAlarmSpecs_OnSelectionChanged(KerbalAlarmClock.DropDownList sender, int OldIndex, int NewIndex)
        {
            SettingsAlarmSpecSelected = (SettingsAlarmSpecsEnum)NewIndex;
        }

        void ddlSettingsContractAutoActive_OnSelectionChanged(KerbalAlarmClock.DropDownList sender, int OldIndex, int NewIndex)
        {
            settings.AlarmAddContractAutoActive =  (Settings.AutoContractBehaviorEnum)NewIndex;
            settings.Save();
        }

        void ddlSettingsContractAutoOffered_OnSelectionChanged(KerbalAlarmClock.DropDownList sender, int OldIndex, int NewIndex)
        {
            settings.AlarmAddContractAutoOffered = (Settings.AutoContractBehaviorEnum)NewIndex;
            settings.Save();
        }

        void ddlSettingsCalendar_OnSelectionChanged(KerbalAlarmClock.DropDownList sender, int OldIndex, int NewIndex)
        {
            settings.SelectedCalendar = (CalendarTypeEnum)NewIndex;
            settings.Save();
            switch (settings.SelectedCalendar)
            {
                case CalendarTypeEnum.KSPStock: KSPDateStructure.SetKSPStockCalendar(); break;
                case CalendarTypeEnum.Earth:
                    KSPDateStructure.SetEarthCalendar(settings.EarthEpoch);
                    break;
                case CalendarTypeEnum.Custom:
                    KSPDateStructure.SetCustomCalendar();
                    break;
                default: KSPDateStructure.SetKSPStockCalendar(); break;
            }
        }

        void ddlSettingsKERNodeMargin_OnSelectionChanged(KerbalAlarmClock.DropDownList sender, int OldIndex, int NewIndex)
        {
            settings.DefaultKERMargin = (Settings.BurnMarginEnum)ddlSettingsKERNodeMargin.SelectedIndex;
            settings.Save();
        }


        void ddlSettingsSound_OnSelectionChanged(KerbalAlarmClock.DropDownList sender, int OldIndex, int NewIndex)
        {
            foreach (AlarmSound s in settings.AlarmSounds) {
                if (s.ddl.SelectedValue != s.SoundName) {
                    s.SoundName = s.ddl.SelectedValue;
                    settings.Save();
                }
            }
        }


        #endregion






        internal void InitDDLStyles()
        {
            ddlManager.DropDownGlyphs = new GUIContentWithStyle(KACResources.btnDropDown, KACResources.styleDropDownGlyph);
            ddlManager.DropDownSeparators = new GUIContentWithStyle("", KACResources.styleSeparatorV);
        }

        internal void DrawWindowsPre()
        {
            ddlManager.DrawBlockingSelectors();
        }

        internal void DrawWindowsPost()
        {
            ddlManager.DrawDropDownLists();
        }



        public class DropDownListManager : List<DropDownList>
        {
            public DropDownListManager()
            {

            }

            internal void AddDDL(DropDownList NewDDL)
            {
                this.Add(NewDDL);
                NewDDL.OnListVisibleChanged += NewDDL_OnListVisibleChanged;
            }

            void NewDDL_OnListVisibleChanged(DropDownList sender, bool VisibleState)
            {
                if (VisibleState)
                {
                    foreach (DropDownList ddlTemp in this)
                    {
                        if (sender != ddlTemp)
                            ddlTemp.ListVisible = false;
                    }
                }
            }

            internal void DrawBlockingSelectors()
            {
                //this is too slow
                //foreach (DropDownList ddlTemp in this.Where(x=>x.ListVisible))
                foreach (DropDownList ddlTemp in this)
                {
                    ddlTemp.DrawBlockingSelector();
                }
            }

            internal void DrawDropDownLists()
            {
                //foreach (DropDownList ddlTemp in this.Where(x=>x.ListVisible))
                foreach (DropDownList ddlTemp in this)
                {
                    ddlTemp.DrawDropDownList();
                }
            }

            internal void CloseOnOutsideClicks()
            {
                //foreach (DropDownList ddlTemp in this.Where(x=>x.ListVisible))
                foreach (DropDownList ddlTemp in this)
                {
                    ddlTemp.CloseOnOutsideClick();
                }
            }

            internal void SetListBoxOffset(Vector2 WrapperOffset)
            {
                foreach (DropDownList ddlTemp in this)
                {
                    ddlTemp.SetListBoxOffset(WrapperOffset);
                }
            }

            internal GUIContentWithStyle DropDownGlyphs
            {
                set
                {
                    foreach (DropDownList ddlTemp in this)
                    {
                        ddlTemp.DropDownGlyph = value;
                    }
                }
            }
            internal GUIContentWithStyle DropDownSeparators
            {
                set
                {
                    foreach (DropDownList ddlTemp in this)
                    {
                        ddlTemp.DropDownSeparator = value;
                    }
                }
            }

        }

        public class DropDownList
        {
            //Constructors
            public DropDownList(IEnumerable<String> Items, Int32 Selected, Rect WindowRect)
                : this(Items, WindowRect)
            {
                SelectedIndex = Selected;
            }
            public DropDownList(IEnumerable<String> Items, Rect WindowRect)
                : this(WindowRect)
            {
                this.Items = Items.ToList<String>();
            }

            //public DropDownList(Enum Items)
            //    : this()
            //{
            //    this.Items = EnumExtensions.ToEnumDescriptions(Items);
            //}
            public DropDownList(Rect WindowRect)
            {
                this.WindowRect = WindowRect;
                //set internal variable so we dont trigger the event before the object exists
                _ListVisible = false;
                SelectedIndex = 0;

                KACResources.OnSkinChanged += SkinsLibrary_OnSkinChanged;
            }

            //properties to use
            internal List<String> Items { get; set; }
            internal Int32 SelectedIndex { get; set; }
            internal String SelectedValue { get { return Items[SelectedIndex]; } }

            private Boolean _ListVisible;
            internal Boolean ListVisible
            {
                get { return _ListVisible; }
                set
                {
                    _ListVisible = value;
                    if (_ListVisible)
                        CalcPagesAndSizes();
                    if (OnListVisibleChanged != null)
                        OnListVisibleChanged(this, _ListVisible);
                }
            }

            internal Rect rectButton;
            internal Rect rectListBox;
            /// <summary>
            /// This is for DropDowns inside extra containers like scrollviews - where getlastrect does not cater to the scrollview position 
            /// </summary>
            private Vector2 vectListBoxOffset = new Vector2(0, 0);
            internal void SetListBoxOffset(Vector2 WrapperOffset)
            {
                vectListBoxOffset = WrapperOffset;
            }

            private GUIStyle stylePager;
            private GUIStyle _styleButton = null;
            internal GUIStyle styleButton
            {
                get { return _styleButton; }
                set { _styleButton = value; SkinsLibrary_OnSkinChanged(); }
            }

            private GUIStyle _styleListItem;
            public GUIStyle styleListItem
            {
                get { return _styleListItem; }
                set { _styleListItem = value; SkinsLibrary_OnSkinChanged(); }
            }

            private GUIStyle _styleListBox;
            public GUIStyle styleListBox
            {
                get { return _styleListBox; }
                set { _styleListBox = value; SkinsLibrary_OnSkinChanged(); }
            }

            internal GUIStyle styleListBlocker = new GUIStyle();

            internal GUIContentWithStyle DropDownSeparator = null;
            internal GUIContentWithStyle DropDownGlyph = null;

            internal Int32 ListItemHeight = 20;
            internal RectOffset ListBoxPadding = new RectOffset(1, 1, 1, 1);

            //internal MonoBehaviourWindow Window;
            internal Rect WindowRect = new Rect();
            internal Int32 ListPageLength = 0;
            internal Int32 ListPageNum = 0;
            internal Boolean ListPageOverflow = false;
            internal Boolean ListPagingRequired = false;


            //event for changes
            public delegate void SelectionChangedEventHandler(DropDownList sender, Int32 OldIndex, Int32 NewIndex);
            public event SelectionChangedEventHandler OnSelectionChanged;
            public delegate void ListVisibleChangedHandler(DropDownList sender, Boolean VisibleState);
            public event ListVisibleChangedHandler OnListVisibleChanged;

            private GUIStyle styleButtonToDraw = null;
            private GUIStyle styleListBoxToDraw = null;
            private GUIStyle styleListItemToDraw = null;

            //Event Handler for SkinChanges
            void SkinsLibrary_OnSkinChanged()
            {
                //check the user provided style
                styleButtonToDraw = KACResources.styleDropDownButton;// CombineSkinStyles(_styleButton, "DropDownButton");
                styleListBoxToDraw = KACResources.styleDropDownListBox;// CombineSkinStyles(_styleListBox, "DropDownListBox");
                styleListItemToDraw = KACResources.styleDropDownListItem;// CombineSkinStyles(_styleListItem, "DropDownListItem");
            }
            //private GUIStyle CombineSkinStyles(GUIStyle UserStyle, String StyleID)
            //{
            //    GUIStyle retStyle;
            //    if (UserStyle == null)
            //    {
            //        //then look in the skinslibrary
            //        if (KACResources.CurrentSkin.customStyles.Any(x => x.name == StyleID))
            //            retStyle = KACResources.CurrentSkin.customStyles.First(x => x.name == StyleID);
            //        //if (SkinsLibrary.StyleExists(SkinsLibrary.CurrentSkin, StyleID))
            //        //{
            //        //    retStyle = SkinsLibrary.GetStyle(SkinsLibrary.CurrentSkin, StyleID);
            //        //}
            //        else
            //        {
            //            retStyle = null;
            //        }
            //    }
            //    else
            //    {
            //        retStyle = UserStyle;
            //    }
            //    return retStyle;
            //}

            //Draw the button behind everything else to catch the first mouse click
            internal void DrawBlockingSelector()
            {
                //do we need to draw the blocker
                if (ListVisible)
                {
                    //This will collect the click event before any other controls under the listrect
                    if (GUI.Button(rectListBox, "", styleListBlocker))
                    {
                        Int32 oldIndex = SelectedIndex;

                        if (!ListPageOverflow)
                            SelectedIndex = (Int32)Math.Floor((Event.current.mousePosition.y - rectListBox.y) / (rectListBox.height / Items.Count));
                        else
                        {
                            //do some maths to work out the actual index - Page Length + 1 for the pager row
                            Int32 SelectedRow = (Int32)Math.Floor((Event.current.mousePosition.y - rectListBox.y) / (rectListBox.height / (ListPageLength + 1)));
                            //Old one - Int32 SelectedRow = (Int32)Math.Floor((Event.current.mousePosition.y - rectListBox.y) / (rectListBox.height / ListPageLength));

                            if (SelectedRow == 0)
                            {
                                //this is the paging row...
                                if (Event.current.mousePosition.x > (rectListBox.x + rectListBox.width - 40 - ListBoxPadding.right))
                                    ListPageNum++;
                                else if (Event.current.mousePosition.x > (rectListBox.x + rectListBox.width - 80 - ListBoxPadding.right))
                                    ListPageNum--;
                                if (ListPageNum < 0) ListPageNum = (Int32)Math.Floor((Single)Items.Count / ListPageLength);
                                if (ListPageNum * ListPageLength > Items.Count) ListPageNum = 0;
                                return;
                            }
                            else
                            {
                                SelectedIndex = (ListPageNum * ListPageLength) + (SelectedRow - 1);
                            }
                            if (SelectedIndex >= Items.Count)
                            {
                                SelectedIndex = oldIndex;
                                return;
                            }
                        }
                        //Throw an event or some such from here
                        if (OnSelectionChanged != null)
                            OnSelectionChanged(this, oldIndex, SelectedIndex);
                        ListVisible = false;
                    }
                }
            }

            //Draw the actual button for the list
            internal Boolean DrawButton()
            {
                Boolean blnReturn = false;

                if (styleButtonToDraw == null)
                    blnReturn = GUILayout.Button(SelectedValue);
                else
                    blnReturn = GUILayout.Button(SelectedValue, styleButtonToDraw);

                if (blnReturn) ListVisible = !ListVisible;

                //get the drawn button rectangle
                if (Event.current.type == EventType.Repaint)
                {
                    rectButton = GUILayoutUtility.GetLastRect();
                }
                //draw a dropdown symbol on the right edge
                if (DropDownGlyph != null)
                {
                    Rect rectDropIcon = new Rect(rectButton) { x = (rectButton.x + rectButton.width - 20), width = 20 };
                    if (DropDownSeparator != null)
                    {
                        Rect rectDropSep = new Rect(rectDropIcon) { x = (rectDropIcon.x - DropDownSeparator.CalcWidth), width = DropDownSeparator.CalcWidth };
                        if (DropDownSeparator.Style == null)
                        {
                            GUI.Box(rectDropSep, DropDownSeparator.Content);
                        }
                        else
                        {
                            GUI.Box(rectDropSep, DropDownSeparator.Content, DropDownSeparator.Style);
                        }
                    }
                    if (DropDownGlyph.Style == null)
                    {
                        GUI.Box(rectDropIcon, DropDownGlyph.Content);
                    }
                    else
                    {
                        GUI.Box(rectDropIcon, DropDownGlyph.Content, DropDownGlyph.Style);
                    }

                }

                return blnReturn;
            }

            private void CalcPagesAndSizes()
            {
                //raw box size
                rectListBox = new Rect(rectButton)
                {
                    x = rectButton.x + WindowRect.x + vectListBoxOffset.x,
                    y = rectButton.y + WindowRect.y + rectButton.height + vectListBoxOffset.y,
                    height = (Items.Count * ListItemHeight) + (ListBoxPadding.top + ListBoxPadding.bottom)
                };

                //if it doesnt fit below the list
                if ((rectListBox.y + rectListBox.height) > WindowRect.y + WindowRect.height)
                {
                    if (rectListBox.height < WindowRect.height - 8)
                    {
                        //move the top up so that the full list is visible
                        ListPageOverflow = false;
                        rectListBox.y = WindowRect.y + WindowRect.height - rectListBox.height - 4;
                    }
                    else
                    {
                        //Need multipages for this list
                        ListPageOverflow = true;
                        rectListBox.y = 4;
                        rectListBox.height = (Single)(ListItemHeight * Math.Floor((WindowRect.height - 8) / ListItemHeight));
                        ListPageLength = (Int32)(Math.Floor((WindowRect.height - 8) / ListItemHeight) - 1);
                        ListPageNum = (Int32)Math.Floor((Double)SelectedIndex / ListPageLength);
                    }
                }
                else
                {
                    ListPageOverflow = false;
                }

                stylePager = new GUIStyle(KACResources.CurrentSkin.label) { fontStyle = FontStyle.Italic };
            }

            //Draw the hovering dropdown
            internal void DrawDropDownList()
            {
                if (ListVisible)
                {
                    GUI.depth = 0;

                    if (styleListBoxToDraw == null) styleListBoxToDraw = GUI.skin.box;
                    if (styleListItemToDraw == null) styleListItemToDraw = GUI.skin.label;

                    //and draw it
                    GUI.Box(rectListBox, "", styleListBoxToDraw);

                    Int32 iStart = 0, iEnd = Items.Count, iPad = 0;
                    if (ListPageOverflow)
                    {
                        //calc the size of each page
                        iStart = ListPageLength * ListPageNum;

                        if (ListPageLength * (ListPageNum + 1) < Items.Count)
                            iEnd = ListPageLength * (ListPageNum + 1);

                        //this moves us down a row to fit the paging buttons in the main loop
                        iPad = 1;

                        //Draw paging buttons
                        GUI.Label(new Rect(rectListBox) { x = rectListBox.x + ListBoxPadding.left, height = 20 }, String.Format("Page {0}/{1:0}", ListPageNum + 1, Math.Floor((Single)Items.Count / ListPageLength) + 1), stylePager);
                        GUI.Button(new Rect(rectListBox) { x = rectListBox.x + rectListBox.width - 80 - ListBoxPadding.right, y = rectListBox.y + 2, width = 40, height = 16 }, "Prev");
                        GUI.Button(new Rect(rectListBox) { x = rectListBox.x + rectListBox.width - 40 - ListBoxPadding.right, y = rectListBox.y + 2, width = 40, height = 16 }, "Next");
                    }

                    //now draw each listitem
                    for (int i = iStart; i < iEnd; i++)
                    {
                        Rect ListButtonRect = new Rect(rectListBox)
                        {
                            x = rectListBox.x + ListBoxPadding.left,
                            width = rectListBox.width - ListBoxPadding.left - ListBoxPadding.right,
                            y = rectListBox.y + ((i - iStart + iPad) * ListItemHeight) + ListBoxPadding.top,
                            height = 20
                        };

                        if (GUI.Button(ListButtonRect, Items[i], styleListItemToDraw))
                        {
                            ListVisible = false;
                            SelectedIndex = i;
                        }
                        if (i == SelectedIndex)
                            GUI.Label(new Rect(ListButtonRect) { x = ListButtonRect.x + ListButtonRect.width - 20 }, "✔");
                    }

                    CloseOnOutsideClick();
                }

            }

            internal Boolean CloseOnOutsideClick()
            {
                if (ListVisible && Event.current.type == EventType.MouseDown && !rectListBox.Contains(Event.current.mousePosition))
                {
                    ListVisible = false;
                    return true;
                }
                else { return false; }
            }
        }
    }
    public class GUIContentWithStyle
    {
        internal GUIContentWithStyle(String text, GUIStyle Style)
        {
            this.Content = new GUIContent(text);
            this.Style = new GUIStyle(Style);
        }
        internal GUIContentWithStyle(GUIContent src, GUIStyle Style)
        {
            this.Content = new GUIContent(src);
            this.Style = new GUIStyle(Style);
        }
        internal GUIContentWithStyle(Texture image, GUIStyle Style)
        {
            this.Content = new GUIContent(image);
            this.Style = new GUIStyle(Style);
        }
        internal GUIContentWithStyle(String text, Texture image, GUIStyle Style)
        {
            this.Content = new GUIContent(text, image);
            this.Style = new GUIStyle(Style);
        }
        internal GUIContentWithStyle(String text)
        {
            this.Content = new GUIContent(text);
        }
        internal GUIContentWithStyle(GUIContent src)
        {
            this.Content = new GUIContent(src);
        }
        internal GUIContentWithStyle(Texture image)
        {
            this.Content = new GUIContent(image);
        }
        internal GUIContentWithStyle(String text, Texture image)
        {
            this.Content = new GUIContent(text, image);
        }

        internal GUIContentWithStyle() { }


        internal GUIContent Content = null;
        internal GUIStyle Style = null;

        internal Single CalcWidth
        {
            get
            {
                Single RunningTotal = 0;
                if (Style != null)
                    RunningTotal = Style.CalcSize(Content).x;
                if (Content.image != null)
                    RunningTotal += Content.image.width;
                if (RunningTotal == 0)
                    RunningTotal = 10;
                return RunningTotal;
            }
        }
        internal Single CalcHeight { get { if (Style != null) return Style.CalcSize(Content).x; else return 20; } }
    }

}
