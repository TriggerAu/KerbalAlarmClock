using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

using System.IO;

using UnityEngine;
using KSP;
using KSPPluginFramework;

namespace KerbalAlarmClock
{
    internal static class KACResources
    {
        #region "Textures"

        //Clock Icons
        internal static Texture2D iconNorm = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconNormShow = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconAlarm = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconAlarmShow = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpEffect100 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpEffect080 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpEffect060 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpEffect040 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpEffect020 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpEffect000 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseEffect100 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseEffect080 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseEffect060 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseEffect040 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseEffect020 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseEffect000 = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        //Clock toolbaricons
        internal static Texture2D toolbariconNorm = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D toolbariconNormShow = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D toolbariconAlarm = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D toolbariconAlarmShow = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D toolbariconWarpEffect100 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D toolbariconWarpEffect080 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D toolbariconWarpEffect060 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D toolbariconWarpEffect040 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D toolbariconWarpEffect020 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D toolbariconWarpEffect000 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D toolbariconPauseEffect100 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D toolbariconPauseEffect080 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D toolbariconPauseEffect060 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D toolbariconPauseEffect040 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D toolbariconPauseEffect020 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D toolbariconPauseEffect000 = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        //Alarm List icons
        internal static Texture2D iconRaw = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconMNode = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconSOI = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconAp = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconPe = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconAN = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconDN = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconXFer = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconClosest = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconLaunchRendezvous = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconCrew = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconContract = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconScienceLab = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconEarth = new Texture2D(18, 14, TextureFormat.ARGB32, false);

        internal static Texture2D iconNone = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        internal static Texture2D iconEdit = new Texture2D(16, 16, TextureFormat.ARGB32, false);

        internal static Texture2D iconWarpToApPe = new Texture2D(20, 12, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpToApPeOver = new Texture2D(20, 12, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpToManNode = new Texture2D(20, 12, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpToManNodeOver = new Texture2D(20, 12, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpToANDN = new Texture2D(20, 12, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpToANDNOver = new Texture2D(20, 12, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpToTSApPe = new Texture2D(20, 12, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpToTSApPeOver = new Texture2D(20, 12, TextureFormat.ARGB32, false);
        //internal static Texture2D iconWarpToTSApPeOverConfirm = new Texture2D(20, 12, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpToTSManNode = new Texture2D(20, 12, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpToTSManNodeOver = new Texture2D(20, 12, TextureFormat.ARGB32, false);


        internal static Texture2D iconWarpList100 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpList080 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpList060 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpList040 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpList020 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconWarpList000 = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        internal static Texture2D iconPauseList100 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseList080 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseList060 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseList040 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseList020 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        internal static Texture2D iconPauseList000 = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        //internal static Texture2D iconstatusSOI = new Texture2D(14, 11, TextureFormat.ARGB32, false);


        internal static Texture2D btnRaw = new Texture2D(20, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnMNode = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnAp = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnPe = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnApPe = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnAN = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnDN = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnANDN = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnSOI = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnXfer = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnClosest = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnCrew = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnContract = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        internal static Texture2D btnScienceLab = new Texture2D(20, 20, TextureFormat.ARGB32, false);
        
        internal static Texture2D btnChevronUp = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnChevronDown = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnChevLeft = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnChevRight = new Texture2D(17, 16, TextureFormat.ARGB32, false);

        internal static Texture2D btnRedCross = new Texture2D(16, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnSettings = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnSettingsAttention = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnAdd = new Texture2D(17, 16, TextureFormat.ARGB32, false);

        internal static Texture2D btnRocket = new Texture2D(16, 16, TextureFormat.ARGB32, false);

        internal static Texture2D btnCalendar = new Texture2D(17, 16, TextureFormat.ARGB32, false);

        internal static Texture2D btnActionNothing = new Texture2D(28, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnActionWarp = new Texture2D(28, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnActionPause = new Texture2D(28, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnActionNoMsg = new Texture2D(28, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnActionMsg = new Texture2D(28, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnActionMsgVessel = new Texture2D(28, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnActionDelete = new Texture2D(28, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnActionSound = new Texture2D(28, 16, TextureFormat.ARGB32, false);

        //Older ones
        internal static Texture2D btnActionWarpMsg = new Texture2D(32, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnActionNothingAndDelete = new Texture2D(32, 16, TextureFormat.ARGB32, false);
        
        internal static Texture2D btnDropDown = new Texture2D(10,10, TextureFormat.ARGB32, false);
        internal static Texture2D btnPlay = new Texture2D(10, 10, TextureFormat.ARGB32, false);
        internal static Texture2D btnStop = new Texture2D(10, 10, TextureFormat.ARGB32, false);

        internal static Texture2D texBox = new Texture2D(9,9, TextureFormat.ARGB32, false);
        internal static Texture2D texBoxUnity = new Texture2D(9, 9, TextureFormat.ARGB32, false);
        internal static Texture2D texTooltip = new Texture2D(9, 9, TextureFormat.ARGB32, false);

        internal static Texture2D texSeparatorV = new Texture2D(6, 2, TextureFormat.ARGB32, false);
        internal static Texture2D texSeparatorH = new Texture2D(2, 20, TextureFormat.ARGB32, false);

        internal static Texture2D curResizeWidth = new Texture2D(23, 23, TextureFormat.ARGB32, false);
        internal static Texture2D curResizeHeight = new Texture2D(23, 23, TextureFormat.ARGB32, false);
        internal static Texture2D curResizeBoth = new Texture2D(23, 23, TextureFormat.ARGB32, false);

        //internal static Texture2D txtTooltipBackground = new Texture2D(9, 9); //, TextureFormat.ARGB32, false);
        //internal static Texture2D txtRedTint = new Texture2D(16, 16); //, TextureFormat.ARGB32, false);
        //internal static Texture2D txtBlackSquare = new Texture2D(5, 5); //, TextureFormat.ARGB32, false);
        //internal static Texture2D txtWhiteSquare = new Texture2D(5, 5); //, TextureFormat.ARGB32, false);

        internal static void loadGUIAssets()
        {
            MonoBehaviourExtended.LogFormatted("Loading Textures");

            try
            {
                KACUtils.LoadImageFromFile(ref iconNorm, "KACIcon-Norm.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref iconNormShow, "KACIcon-NormShow.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref iconAlarm, "KACIcon-Alarm.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref iconAlarmShow, "KACIcon-AlarmShow.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref iconWarpEffect100, "KACIcon-WarpEffect2_100.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref iconWarpEffect080, "KACIcon-WarpEffect2_080.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref iconWarpEffect060, "KACIcon-WarpEffect2_060.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref iconWarpEffect040, "KACIcon-WarpEffect2_040.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref iconWarpEffect020, "KACIcon-WarpEffect2_020.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref iconWarpEffect000, "KACIcon-WarpEffect2_000.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref iconPauseEffect100, "KACIcon-PauseEffect_100.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref iconPauseEffect080, "KACIcon-PauseEffect_080.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref iconPauseEffect060, "KACIcon-PauseEffect_060.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref iconPauseEffect040, "KACIcon-PauseEffect_040.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref iconPauseEffect020, "KACIcon-PauseEffect_020.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref iconPauseEffect000, "KACIcon-PauseEffect_000.png", KACUtils.PathToolbarIcons);

                KACUtils.LoadImageFromFile(ref toolbariconNorm, "KACIconBig-Norm.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref toolbariconNormShow, "KACIconBig-NormShow.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref toolbariconAlarm, "KACIconBig-Alarm.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref toolbariconAlarmShow, "KACIconBig-AlarmShow.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref toolbariconWarpEffect100, "KACIconBig-WarpEffect2_100.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref toolbariconWarpEffect080, "KACIconBig-WarpEffect2_080.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref toolbariconWarpEffect060, "KACIconBig-WarpEffect2_060.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref toolbariconWarpEffect040, "KACIconBig-WarpEffect2_040.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref toolbariconWarpEffect020, "KACIconBig-WarpEffect2_020.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref toolbariconWarpEffect000, "KACIconBig-WarpEffect2_000.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref toolbariconPauseEffect100, "KACIconBig-PauseEffect_100.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref toolbariconPauseEffect080, "KACIconBig-PauseEffect_080.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref toolbariconPauseEffect060, "KACIconBig-PauseEffect_060.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref toolbariconPauseEffect040, "KACIconBig-PauseEffect_040.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref toolbariconPauseEffect020, "KACIconBig-PauseEffect_020.png", KACUtils.PathToolbarIcons);
                KACUtils.LoadImageFromFile(ref toolbariconPauseEffect000, "KACIconBig-PauseEffect_000.png", KACUtils.PathToolbarIcons);


                KACUtils.LoadImageFromFile(ref iconRaw, "img_listiconRaw.png");
                KACUtils.LoadImageFromFile(ref iconSOI, "img_listiconSOI.png");
                KACUtils.LoadImageFromFile(ref iconMNode, "img_listiconMNode.png");
                KACUtils.LoadImageFromFile(ref iconAp, "img_listiconAp.png");
                KACUtils.LoadImageFromFile(ref iconPe, "img_listiconPe.png");
                KACUtils.LoadImageFromFile(ref iconAN, "img_listiconAN.png");
                KACUtils.LoadImageFromFile(ref iconDN, "img_listiconDN.png");
                KACUtils.LoadImageFromFile(ref iconXFer, "img_listiconXfer.png");
                KACUtils.LoadImageFromFile(ref iconClosest, "img_listiconClosest.png");
                KACUtils.LoadImageFromFile(ref iconCrew, "img_listiconCrew.png");
                KACUtils.LoadImageFromFile(ref iconContract, "img_listiconContract.png");
                KACUtils.LoadImageFromFile(ref iconScienceLab, "img_listiconScienceLab.png");
                KACUtils.LoadImageFromFile(ref iconEarth, "img_listiconEarth.png");
                KACUtils.LoadImageFromFile(ref iconLaunchRendezvous, "img_listiconLaunchRendezvous.png");
                KACUtils.LoadImageFromFile(ref iconWarpList100, "img_listiconWarpList_100.png");
                KACUtils.LoadImageFromFile(ref iconWarpList080, "img_listiconWarpList_080.png");
                KACUtils.LoadImageFromFile(ref iconWarpList060, "img_listiconWarpList_060.png");
                KACUtils.LoadImageFromFile(ref iconWarpList040, "img_listiconWarpList_040.png");
                KACUtils.LoadImageFromFile(ref iconWarpList020, "img_listiconWarpList_020.png");
                KACUtils.LoadImageFromFile(ref iconWarpList000, "img_listiconWarpList_000.png");

                KACUtils.LoadImageFromFile(ref iconPauseList100, "img_listiconPauseList_100.png");
                KACUtils.LoadImageFromFile(ref iconPauseList080, "img_listiconPauseList_080.png");
                KACUtils.LoadImageFromFile(ref iconPauseList060, "img_listiconPauseList_060.png");
                KACUtils.LoadImageFromFile(ref iconPauseList040, "img_listiconPauseList_040.png");
                KACUtils.LoadImageFromFile(ref iconPauseList020, "img_listiconPauseList_020.png");
                KACUtils.LoadImageFromFile(ref iconPauseList000, "img_listiconPauseList_000.png");

                KACUtils.LoadImageFromFile(ref iconNone, "img_listiconNone.png");
                KACUtils.LoadImageFromFile(ref iconEdit, "img_listiconEdit.png");

                KACUtils.LoadImageFromFile(ref iconWarpToApPe, "img_buttonWarpToApPe.png");
                KACUtils.LoadImageFromFile(ref iconWarpToApPeOver, "img_buttonWarpToApPeOver.png");
                KACUtils.LoadImageFromFile(ref iconWarpToManNode, "img_buttonWarpToManNode.png");
                KACUtils.LoadImageFromFile(ref iconWarpToManNodeOver, "img_buttonWarpToManNodeOver.png");
                KACUtils.LoadImageFromFile(ref iconWarpToANDN, "img_buttonWarpToANDN.png");
                KACUtils.LoadImageFromFile(ref iconWarpToANDNOver, "img_buttonWarpToANDNOver.png");
                KACUtils.LoadImageFromFile(ref iconWarpToTSApPe, "img_buttonWarpToTSApPe.png");
                KACUtils.LoadImageFromFile(ref iconWarpToTSApPeOver, "img_buttonWarpToTSApPeOver.png");
                //KACUtils.LoadImageFromFile(ref iconWarpToTSApPeOverConfirm, "img_buttonWarpToTSApPeOverConfirm.png");
                KACUtils.LoadImageFromFile(ref iconWarpToTSManNode, "img_buttonWarpToTSManNode.png");
                KACUtils.LoadImageFromFile(ref iconWarpToTSManNodeOver, "img_buttonWarpToTSManNodeOver.png");

                //KACUtils.LoadImageFromFile(ref iconstatusSOI, "img_statusiconSOI.png");

                KACUtils.LoadImageFromFile(ref btnRaw, "img_buttonTypeRaw.png");
                KACUtils.LoadImageFromFile(ref btnMNode, "img_buttonTypeMNode.png");
                KACUtils.LoadImageFromFile(ref btnAp, "img_buttonTypeAp.png");
                KACUtils.LoadImageFromFile(ref btnPe, "img_buttonTypePe.png");
                KACUtils.LoadImageFromFile(ref btnApPe, "img_buttonTypeApPe.png");
                KACUtils.LoadImageFromFile(ref btnAN, "img_buttonTypeAN.png");
                KACUtils.LoadImageFromFile(ref btnDN, "img_buttonTypeDN.png");
                KACUtils.LoadImageFromFile(ref btnANDN, "img_buttonTypeANDN.png");
                KACUtils.LoadImageFromFile(ref btnSOI, "img_buttonTypeSOI.png");
                KACUtils.LoadImageFromFile(ref btnXfer, "img_buttonTypeXfer.png");
                KACUtils.LoadImageFromFile(ref btnClosest, "img_buttonTypeClosest.png");
                KACUtils.LoadImageFromFile(ref btnCrew, "img_buttonTypeCrew.png");
                KACUtils.LoadImageFromFile(ref btnContract, "img_buttonTypeContract.png");
                KACUtils.LoadImageFromFile(ref btnScienceLab, "img_buttonTypeScienceLab.png");

                KACUtils.LoadImageFromFile(ref btnChevronUp, "img_buttonChevronUp.png");
                KACUtils.LoadImageFromFile(ref btnChevronDown, "img_buttonChevronDown.png");
                KACUtils.LoadImageFromFile(ref btnChevLeft, "img_buttonChevronLeft.png");
                KACUtils.LoadImageFromFile(ref btnChevRight, "img_buttonChevronRight.png");

                KACUtils.LoadImageFromFile(ref btnRedCross, "img_buttonRedCross.png");
                KACUtils.LoadImageFromFile(ref btnSettings, "img_buttonSettings.png");
                KACUtils.LoadImageFromFile(ref btnSettingsAttention, "img_buttonSettingsAttention.png");
                KACUtils.LoadImageFromFile(ref btnAdd, "img_buttonAdd.png");

                KACUtils.LoadImageFromFile(ref btnRocket, "img_buttonRocket.png");

                KACUtils.LoadImageFromFile(ref btnCalendar, "img_buttonCalendar.png");

                KACUtils.LoadImageFromFile(ref btnActionNothing, "img_buttonActionNothing.png");
                KACUtils.LoadImageFromFile(ref btnActionWarp, "img_buttonActionWarp.png");
                KACUtils.LoadImageFromFile(ref btnActionPause, "img_buttonActionPause.png");
                KACUtils.LoadImageFromFile(ref btnActionNoMsg, "img_buttonActionNoMsg.png");
                KACUtils.LoadImageFromFile(ref btnActionMsg, "img_buttonActionMsg.png");
                KACUtils.LoadImageFromFile(ref btnActionMsgVessel, "img_buttonActionMsgVessel.png");
                KACUtils.LoadImageFromFile(ref btnActionSound, "img_buttonActionSound.png");
                KACUtils.LoadImageFromFile(ref btnActionDelete, "img_buttonActionDelete.png");

                KACUtils.LoadImageFromFile(ref btnActionWarpMsg, "img_buttonActionWarpMsg.png");
                KACUtils.LoadImageFromFile(ref btnActionNothingAndDelete, "img_buttonActionNothingAndDelete.png");

                KACUtils.LoadImageFromFile(ref btnDropDown, "img_DropDown.png");
                KACUtils.LoadImageFromFile(ref btnPlay, "img_Play.png");
                KACUtils.LoadImageFromFile(ref btnStop, "img_Stop.png");

                KACUtils.LoadImageFromFile(ref curResizeWidth, "cur_ResizeWidth.png");
                KACUtils.LoadImageFromFile(ref curResizeHeight, "cur_ResizeHeight.png");
                KACUtils.LoadImageFromFile(ref curResizeBoth, "cur_ResizeBoth.png");

                KACUtils.LoadImageFromFile(ref texBox, "tex_Box.png");
                KACUtils.LoadImageFromFile(ref texBoxUnity, "tex_BoxUnity.png");

                KACUtils.LoadImageFromFile(ref texSeparatorH, "img_SeparatorHorizontal.png");
                KACUtils.LoadImageFromFile(ref texSeparatorV, "img_SeparatorVertical.png");


                //KACUtils.LoadImageFromFile(ref txtRedTint, "Textures", "RedOverlay.png");

                //KACUtils.LoadImageFromFile(ref txtBlackSquare, "Textures", "BlackSquare.png");
                //KACUtils.LoadImageFromFile(ref txtWhiteSquare, "Textures", "WhiteSquare.png");

                MonoBehaviourExtended.LogFormatted("Loaded Textures");
            }
            catch (Exception)
            {
                MonoBehaviourExtended.LogFormatted("Failed to Load Textures - are you missing a file?");
            }


        }

        internal static Texture2D GetSettingsButtonIcon(Boolean AttentionRequired)
        {
            Texture2D textureReturn;

            //Only flash if we need attention
            if (AttentionRequired && DateTime.Now.Millisecond < 500)
                textureReturn = btnSettingsAttention;
            else
                textureReturn = btnSettings;

            return textureReturn;
        }
        internal static Texture2D GetWarpIcon(Boolean AppLauncherVersion = false)
        {
            Texture2D textureReturn;

            Double intHundredth = Math.Truncate(DateTime.Now.Millisecond / 100d);
            switch (Convert.ToInt64(intHundredth))
            {
                case 0:
                    textureReturn = AppLauncherVersion ? KACResources.toolbariconWarpEffect100 : KACResources.iconWarpEffect100;
                    break;
                case 1:
                case 9:
                    textureReturn = AppLauncherVersion ? KACResources.toolbariconWarpEffect080 : KACResources.iconWarpEffect080;
                    break;
                case 2:
                case 8:
                    textureReturn = AppLauncherVersion ? KACResources.toolbariconWarpEffect060 : KACResources.iconWarpEffect060;
                    break;
                case 3:
                case 7:
                    textureReturn = AppLauncherVersion ? KACResources.toolbariconWarpEffect040 : KACResources.iconWarpEffect040;
                    break;
                case 4:
                case 6:
                    textureReturn = AppLauncherVersion ? KACResources.toolbariconWarpEffect020 : KACResources.iconWarpEffect020;
                    break;
                case 5:
                    textureReturn = AppLauncherVersion ? KACResources.toolbariconWarpEffect000 : KACResources.iconWarpEffect000;
                    break;
                default:
                    textureReturn = AppLauncherVersion ? KACResources.toolbariconWarpEffect100 : KACResources.iconWarpEffect100;
                    break;
            }
            return textureReturn;
        }

        internal static Texture2D GetPauseIcon(Boolean AppLauncherVersion=false)
        {
            Texture2D textureReturn;

            Double intHundredth = Math.Truncate(DateTime.Now.Millisecond / 100d);
            switch (Convert.ToInt64(intHundredth))
            {
                case 0:
                    textureReturn = AppLauncherVersion ? KACResources.toolbariconPauseEffect100 : KACResources.iconPauseEffect100;
                    break;
                case 1:
                case 9:
                    textureReturn = AppLauncherVersion ? KACResources.toolbariconPauseEffect080 : KACResources.iconPauseEffect080;
                    break;
                case 2:
                case 8:
                    textureReturn = AppLauncherVersion ? KACResources.toolbariconPauseEffect060 : KACResources.iconPauseEffect060;
                    break;
                case 3:
                case 7:
                    textureReturn = AppLauncherVersion ? KACResources.toolbariconPauseEffect040 : KACResources.iconPauseEffect040;
                    break;
                case 4:
                case 6:
                    textureReturn = AppLauncherVersion ? KACResources.toolbariconPauseEffect020 : KACResources.iconPauseEffect020;
                    break;
                case 5:
                    textureReturn = AppLauncherVersion ? KACResources.toolbariconPauseEffect000 : KACResources.iconPauseEffect000;
                    break;
                default:
                    textureReturn = KACResources.iconPauseEffect100;
                    break;
            }
            return textureReturn;
        }

        internal static String GetWarpIconTexturePath()
        {
            String textureReturn = KACUtils.PathToolbarTexturePath + "/KACIcon-WarpEffect2_";

            textureReturn = GetIconPercentageFromTime(textureReturn);
            return textureReturn;
        }

        internal static String GetPauseIconTexturePath()
        {
            String textureReturn = KACUtils.PathToolbarTexturePath + "/KACIcon-PauseEffect_";

            textureReturn = GetIconPercentageFromTime(textureReturn);
            return textureReturn;
        }

        private static string GetIconPercentageFromTime(String textureReturn)
        {
            Double intHundredth = Math.Truncate(DateTime.Now.Millisecond / 100d);
            switch (Convert.ToInt64(intHundredth))
            {
                case 0:
                    textureReturn += "100"; break;
                case 1:
                case 9:
                    textureReturn += "080"; break;
                case 2:
                case 8:
                    textureReturn += "060"; break;
                case 3:
                case 7:
                    textureReturn += "040"; break;
                case 4:
                case 6:
                    textureReturn += "020"; break;
                case 5:
                    textureReturn += "000"; break;
                default:
                    textureReturn += textureReturn += "100"; break;
            }
            return textureReturn;
        }

        internal static Texture2D GetWarpListIcon(Boolean blnWarpInfluence)
        {
            Texture2D textureReturn;

            if (blnWarpInfluence)
            {
                Double intHundredth = Math.Truncate(DateTime.Now.Millisecond / 100d);
                switch (Convert.ToInt64(intHundredth))
                {
                    case 0:
                        textureReturn = KACResources.iconWarpList100;
                        break;
                    case 1:
                    case 9:
                        textureReturn = KACResources.iconWarpList080;
                        break;
                    case 2:
                    case 8:
                        textureReturn = KACResources.iconWarpList060;
                        break;
                    case 3:
                    case 7:
                        textureReturn = KACResources.iconWarpList040;
                        break;
                    case 4:
                    case 6:
                        textureReturn = KACResources.iconWarpList020;
                        break;
                    case 5:
                        textureReturn = KACResources.iconWarpList000;
                        break;
                    default:
                        textureReturn = KACResources.iconWarpList100;
                        break;
                }
            }
            else
            {
                textureReturn = KACResources.iconWarpList000;
            }
            return textureReturn;
        }

        internal static Texture2D GetPauseListIcon(Boolean blnPauseInfluence)
        {
            Texture2D textureReturn;

            if (blnPauseInfluence)
            {
                Double intHundredth = Math.Truncate(DateTime.Now.Millisecond / 100d);
                switch (Convert.ToInt64(intHundredth))
                {
                    case 0:
                        textureReturn = KACResources.iconPauseList100;
                        break;
                    case 1:
                    case 9:
                        textureReturn = KACResources.iconPauseList080;
                        break;
                    case 2:
                    case 8:
                        textureReturn = KACResources.iconPauseList060;
                        break;
                    case 3:
                    case 7:
                        textureReturn = KACResources.iconPauseList040;
                        break;
                    case 4:
                    case 6:
                        textureReturn = KACResources.iconPauseList020;
                        break;
                    case 5:
                        textureReturn = KACResources.iconPauseList000;
                        break;
                    default:
                        textureReturn = KACResources.iconPauseList100;
                        break;
                }
            }
            else
            {
                textureReturn = KACResources.iconPauseList000;
            }
            return textureReturn;
        }
        #endregion

        #region Audio Stuff

        //Alarm Library
        internal static Dictionary<String, AudioClip> clipAlarms;

        internal static void LoadSounds()
        {
            MonoBehaviourExtended.LogFormatted("Loading Sounds");

            clipAlarms = new Dictionary<string, AudioClip>();
            clipAlarms.Add("None", null);
            if (Directory.Exists(KACUtils.PathPluginSounds))
            {
                //get all the png and tga's
                FileInfo[] fileClips = new System.IO.DirectoryInfo(KACUtils.PathPluginSounds).GetFiles("*.wav");

                foreach (FileInfo fileClip in fileClips)
                {
                    try
                    {
                        //load the file from the GameDB
                        AudioClip clipLoading = null;
                        if (LoadAudioClipFromGameDB(ref clipLoading, fileClip.Name))
                        {
                            String ClipKey = fileClip.Name;
                            if (ClipKey.ToLower().EndsWith(".wav"))
                                ClipKey = ClipKey.Substring(0, ClipKey.Length - 4);
                            clipAlarms.Add(ClipKey, clipLoading);
                        }
                    }
                    catch (Exception)
                    {
                        //MonoBehaviourExtended.LogFormatted("Unable to load AudioClip from GameDB:{0}/{1}", PathPluginSounds,fileClip.Name);
                    }
                }
            }

        }

        internal static Boolean LoadAudioClipFromGameDB(ref AudioClip clip, String FileName, String FolderPath = "")
        {
            Boolean blnReturn = false;
            try
            {
                //trim off the tga and png extensions
                if (FileName.ToLower().EndsWith(".wav")) FileName = FileName.Substring(0, FileName.Length - 4);
                //default folder
                if (FolderPath == "") FolderPath = KACUtils.DBPathPluginSounds;

                //Look for case mismatches
                if (!GameDatabase.Instance.ExistsAudioClip(String.Format("{0}/{1}", FolderPath, FileName)))
                    throw new Exception();

                //now load it
                clip = GameDatabase.Instance.GetAudioClip(String.Format("{0}/{1}", FolderPath, FileName));
                blnReturn = true;
            }
            catch (Exception)
            {
                MonoBehaviourExtended.LogFormatted("Failed to load (are you missing a file - and check case):{0}/{1}", FolderPath, FileName);
            }
            return blnReturn;
        }


        #endregion

        #region Skins

        /// <summary>
        /// This is a copy of the default Unity skin
        /// </summary>
        internal static GUISkin DefUnitySkin { get; private set; }
        /// <summary>
        /// This is a copy of the default KSP skin
        /// </summary>
        internal static GUISkin DefKSPSkin { get; private set; }

        private static GUISkin _CurrentSkin;
        /// <summary>
        /// Will return the current Skin as controlled by the SetSkin() Methods
        /// </summary>
        internal static GUISkin CurrentSkin { get { return _CurrentSkin; } }


        internal static void InitSkins()
        {
            DefUnitySkin = GUI.skin;
            DefKSPSkin = (GUISkin)GUISkin.Instantiate(HighLogic.Skin);

            SetSkin(KerbalAlarmClock.settings.SelectedSkin);
        }

        internal static void SetSkin(Settings.DisplaySkin SkinToSet)
        {
            switch (SkinToSet)
            {
                case Settings.DisplaySkin.Default:
                    _CurrentSkin = DefKSPSkin;
                    _CurrentSkin.font = DefUnitySkin.font;
                    SetStyleDefaults();
                    SetKSPStyles();
                    SetKSPButtons();
                    break;
                case Settings.DisplaySkin.Unity:
                    _CurrentSkin = DefUnitySkin;
                    SetStyleDefaults(); //was 12
                    SetUnityStyles();
                    SetUnityButtons();
                    break;
                case Settings.DisplaySkin.UnityWKSPButtons:
                    _CurrentSkin = DefUnitySkin;
                    SetStyleDefaults();
                    SetUnityStyles();
                    SetKSPButtons();
                    break;
                default:
                    _CurrentSkin = DefKSPSkin;
                    SetStyleDefaults();
                    SetKSPStyles();
                    SetKSPButtons();
                    break;
            }

            SetStyles();

            //this throws an error
            if (OnSkinChanged!=null)
                OnSkinChanged();
        }

        internal delegate void SkinChangedEvent();
        internal static event SkinChangedEvent OnSkinChanged;

        static GUIStyle styleDefLabel, styleDefTextField, styleDefTextArea, styleDefToggle, styleDefButton;
        static int intFontSizeDefault;
        private static void SetStyleDefaults(Int32 FontSize=12)
        {
            Color32 colLabelText = new Color32(220, 220, 220, 255);
            intFontSizeDefault = FontSize;

            //Common starting points
            styleDefLabel = new GUIStyle(CurrentSkin.label);
            styleDefLabel.fontSize = intFontSizeDefault;
            styleDefLabel.fontStyle = FontStyle.Normal;
            styleDefLabel.normal.textColor = colLabelText;
            styleDefLabel.hover.textColor = Color.blue;

            styleDefTextField = new GUIStyle(CurrentSkin.textField);
            styleDefTextField.fontSize = intFontSizeDefault;
            styleDefTextField.fontStyle = FontStyle.Normal;
            styleDefTextArea = new GUIStyle(CurrentSkin.textArea);
            styleDefTextArea.fontSize = intFontSizeDefault;
            styleDefTextArea.fontStyle = FontStyle.Normal;
            styleDefToggle = new GUIStyle(CurrentSkin.toggle);
            styleDefToggle.fontSize = intFontSizeDefault;
            styleDefToggle.fontStyle = FontStyle.Normal;
            styleDefToggle.stretchWidth = false;

            styleWindow = new GUIStyle(CurrentSkin.window);
            styleWindow.padding = KACUtils.SetWindowRectOffset(styleWindow.padding, 4);


            //styleWindowQuickAdd = new GUIStyle(styleWindow);
            //styleWindow.normal.background = KACResources.txtWhiteSquare;
            //styleWindow.normal.textColor = new Color32(183, 254, 0, 255);
            //styleWindow.normal.textColor = Color.red;

        }
        private static void SetKSPStyles()
        {
            texTooltip = texBox;

            styleDropDownListBox = new GUIStyle();
            styleDropDownListBox.normal.background = texBox;
            //Extra border to prevent bleed of color - actual border is only 1 pixel wide
            styleDropDownListBox.border = new RectOffset(3, 3, 3, 3);

        }
        private static void SetUnityStyles()
        {
            texTooltip = texBoxUnity;

            styleDropDownListBox = new GUIStyle();
            styleDropDownListBox.normal.background = texBoxUnity;
            //Extra border to prevent bleed of color - actual border is only 1 pixel wide
            styleDropDownListBox.border = new RectOffset(3, 3, 3, 3);
        }
        private static void SetKSPButtons()
        {
            styleDefButton = new GUIStyle(DefKSPSkin.button);
            styleDefToggle.fontSize = intFontSizeDefault;
            styleDefToggle.fontStyle = FontStyle.Normal;

            styleDropDownButton = new GUIStyle(styleDefButton);
            styleDropDownButton.fontSize = intFontSizeDefault;
            styleDropDownButton.fixedHeight = 20;
            if (KerbalAlarmClock.settings.SelectedSkin== Settings.DisplaySkin.UnityWKSPButtons)
                styleDropDownButton.padding.top = 4;
            else
                styleDropDownButton.padding.top = 8;
            styleDropDownButton.padding.right = 20;

        }
        private static void SetUnityButtons()
        {
            styleDefButton = new GUIStyle(DefUnitySkin.button);
            styleDefToggle.fontSize = intFontSizeDefault;
            styleDefToggle.fontStyle = FontStyle.Normal;

            styleDropDownButton = new GUIStyle(styleDefButton);
            styleDropDownButton.fontSize = intFontSizeDefault;
            styleDropDownButton.fixedHeight = 20;
            styleDropDownButton.padding.top = 4;
            styleDropDownButton.padding.right = 20;
        }


        #endregion

        #region "Styles"
        //Styles for windows - Cant initialize the objects here as the GUIStyle Constructor cannot be called outside of OnGUI

        internal static GUIStyle styleWindow;//, styleWindowQuickAdd;
        internal static GUIStyle styleTooltipStyle;

        internal static GUIStyle styleIconStyle;
        internal static GUIStyle styleHeading;
        internal static GUIStyle styleContent;

        internal static GUIStyle styleHeadingEarth;
        internal static GUIStyle styleContentEarth;

        internal static GUIStyle styleButton;

        internal static GUIStyle styleLabel;
        internal static GUIStyle styleLabelWarning;
        internal static GUIStyle styleLabelError;

        internal static GUIStyle styleCheckbox;
        internal static GUIStyle styleCheckboxLabel;

        internal static GUIStyle styleButtonList;
        internal static GUIStyle styleButtonListAlarmActions;

        internal static GUIStyle styleSmallButton, styleQAButton, styleQAListButton;

        internal static GUIStyle styleFlagIcon;

        //List Styles
        internal static GUIStyle styleAlarmListArea;
        internal static GUIStyle styleAlarmText;
        //internal static GUIStyle styleAlarmTextGrayed;
        internal static GUIStyle styleAlarmIcon;
        internal static GUIStyle styleLabelWarp;
        internal static GUIStyle styleLabelWarpGrayed;
        //internal static GUIStyle styleSOIIndicator;
        //internal static GUIStyle styleSOIIcon;

        //Add Alarm Styles
        internal static GUIStyle styleAddSectionHeading;
        internal static GUIStyle styleAddHeading;
        internal static GUIStyle styleAddField;
        internal static GUIStyle styleAddFieldError;
        internal static GUIStyle styleAddFieldLocked;
        
        //internal static GUIStyle styleAddFieldErorOverlay;
        internal static GUIStyle styleAddFieldGreen;
        internal static GUIStyle styleAddFieldAreas;
        internal static GUIStyle styleAddAlarmArea;
        internal static GUIStyle styleAddXferName;
        internal static GUIStyle styleAddXferButton;
        internal static GUIStyle styleAddXferOriginButton;
        internal static GUIStyle styleAddMessageField;

        internal static GUIStyle styleContractLabelOffer;
        internal static GUIStyle styleContractLabelActive;
        internal static GUIStyle styleContractLabelAlarmExists;

        //AlarmMessage Styles
        internal static GUIStyle styleAlarmMessage;
        internal static GUIStyle styleAlarmMessageTime;
        internal static GUIStyle styleAlarmMessageAction;
        internal static GUIStyle styleAlarmMessageActionPause;

        internal static GUIStyle styleVersionHighlight;

        #region DropdownStuff
        internal static GUIStyle styleDropDownButton;
        internal static GUIStyle styleDropDownListBox;
        internal static GUIStyle styleDropDownListItem;

        internal static GUIStyle styleDropDownGlyph;

        internal static GUIStyle styleSeparatorV;
        internal static GUIStyle styleSeparatorH;
        #endregion

        internal static List<GUIContent> lstAlarmChoices;
        internal static List<GUIContent> lstAlarmWarpChoices;
        internal static List<GUIContent> lstAlarmMessageChoices;

        /// <summary>
        /// Sets up the styles for the different parts of the drawing
        /// Should only be called once
        /// </summary>
        internal static void SetStyles()
        {
            //Color32 colLabelText = new Color32(220, 220, 220, 255);
            //int intFontSizeDefault = 13;

            ////Common starting points
            //GUIStyle styleDefLabel = new GUIStyle(CurrentSkin.label);
            //styleDefLabel.fontSize = intFontSizeDefault;
            //styleDefLabel.fontStyle = FontStyle.Normal;
            //styleDefLabel.normal.textColor = colLabelText;
            //styleDefLabel.hover.textColor = Color.blue;

            //GUIStyle styleDefTextField = new GUIStyle(CurrentSkin.textField);
            //styleDefTextField.fontSize = intFontSizeDefault;
            //styleDefTextField.fontStyle = FontStyle.Normal;
            //GUIStyle styleDefTextArea = new GUIStyle(CurrentSkin.textArea);
            //styleDefTextArea.fontSize = intFontSizeDefault;
            //styleDefTextArea.fontStyle = FontStyle.Normal;
            //GUIStyle styleDefToggle = new GUIStyle(CurrentSkin.toggle);
            //styleDefToggle.fontSize = intFontSizeDefault;
            //styleDefToggle.fontStyle = FontStyle.Normal;
            //GUIStyle styleDefButton = new GUIStyle(CurrentSkin.button);
            //styleDefToggle.fontSize = intFontSizeDefault;
            //styleDefToggle.fontStyle = FontStyle.Normal;

            //Set up the used styles
            styleIconStyle = new GUIStyle(styleDefButton);
            styleIconStyle.fixedHeight = 32;
            styleIconStyle.fixedWidth = 32;
            styleIconStyle.padding = new RectOffset(0, 0, 0, 0);

            //styleWindow = new GUIStyle(CurrentSkin.window);
            //styleWindow.padding = KACUtils.SetWindowRectOffset(styleWindow.padding, 4);
            ////styleWindow.normal.background = KACResources.txtWhiteSquare;
            ////styleWindow.normal.textColor = new Color32(183, 254, 0, 255);
            ////styleWindow.normal.textColor = Color.red;

            styleTooltipStyle = new GUIStyle(styleDefLabel);
            styleTooltipStyle.fontSize = 12;
            styleTooltipStyle.normal.textColor = new Color32(207, 207, 207, 255);
            styleTooltipStyle.stretchHeight = true;
            styleTooltipStyle.wordWrap = true;
            styleTooltipStyle.normal.background = texTooltip;
            //Extra border to prevent bleed of color - actual border is only 1 pixel wide
            styleTooltipStyle.border = new RectOffset(3, 3, 3, 3);
            styleTooltipStyle.padding = new RectOffset(4, 4, 6, 4);
            styleTooltipStyle.alignment = TextAnchor.MiddleCenter;

            styleHeading = new GUIStyle(styleDefLabel);
            styleHeading.fontSize = styleDefLabel.fontSize + 1;
            styleHeading.normal.textColor = Color.white;
            styleHeading.fontStyle = FontStyle.Bold;

            styleContent = new GUIStyle(styleDefLabel);
            styleContent.normal.textColor = new Color32(183, 254, 0, 255);
            styleContent.alignment = TextAnchor.MiddleRight;
            styleContent.stretchWidth = true;

            styleButton = new GUIStyle(styleDefButton);
            styleButton.hover.textColor = Color.yellow;
            styleButton.fontSize = intFontSizeDefault;

            styleLabel = new GUIStyle(styleDefLabel);

            styleLabelWarning = new GUIStyle(styleLabel);
            styleLabelWarning.normal.textColor = Color.yellow;

            styleLabelError = new GUIStyle(styleLabel);
            styleLabelError.normal.textColor = Color.red;


            styleCheckbox = new GUIStyle(styleDefToggle);
            //CHANGED
            styleCheckboxLabel = new GUIStyle(styleDefLabel);
            //styleCheckboxLabel.hover.textColor = Color.red;
            //styleCheckboxLabel.onHover.textColor = Color.red;
            styleCheckboxLabel.stretchWidth=false;
            styleCheckboxLabel.alignment = TextAnchor.MiddleLeft;

            styleButtonList = new GUIStyle(styleDefButton);
            styleButtonList.fixedHeight = 26;
            styleButtonList.padding = KACUtils.SetRectOffset(styleButtonList.padding, 0);
            styleButtonList.onNormal.background = styleButtonList.active.background;

            styleButtonListAlarmActions = new GUIStyle(styleDefButton);
            styleButtonListAlarmActions.fixedHeight = 22;
            styleButtonListAlarmActions.fixedWidth = 40;
            styleButtonListAlarmActions.padding = KACUtils.SetRectOffset(styleButtonList.padding, 0);
            styleButtonListAlarmActions.onNormal.background = styleButtonListAlarmActions.active.background;

            styleSmallButton = new GUIStyle(styleDefButton);
            styleSmallButton.alignment = TextAnchor.MiddleCenter;
            styleSmallButton.fixedWidth = 30;
            styleSmallButton.fixedHeight = 20;
            styleSmallButton.fontSize = intFontSizeDefault;
            styleSmallButton.fontStyle = FontStyle.Normal;
            styleSmallButton.padding = KACUtils.SetRectOffset(styleSmallButton.padding, 0);
            styleSmallButton.onNormal.background = styleSmallButton.active.background;

            styleQAButton = new GUIStyle(styleSmallButton);
            if (KerbalAlarmClock.settings.SelectedSkin == Settings.DisplaySkin.Default)
            {
                styleQAButton.padding.left = -1;
                styleQAButton.padding.top = 1;
            }
            else
            {
                styleQAButton.padding.right = 1;
                styleQAButton.padding.top = 1;
            }
            styleQAButton.fixedWidth = 18;
            styleQAButton.normal.textColor = new Color32(177, 193, 205, 255);
            styleQAButton.fontStyle = FontStyle.Bold;
            styleQAButton.fontSize=16;

            styleQAListButton = new GUIStyle(styleDefButton);
            styleQAListButton.normal.textColor = new Color32(177, 193, 205, 255);
            styleQAListButton.fixedHeight = 20;
            styleQAListButton.alignment = TextAnchor.MiddleCenter;
            if (KerbalAlarmClock.settings.SelectedSkin== Settings.DisplaySkin.Default)
                styleQAListButton.padding = new RectOffset(4, 4, 4, 6);
            else
                styleQAListButton.padding = styleDefButton.padding;

            styleFlagIcon = new GUIStyle(styleDefLabel);
            styleFlagIcon.padding = KACUtils.SetRectOffset(styleFlagIcon.padding, 0,0,4,0);
            styleFlagIcon.alignment = TextAnchor.MiddleLeft;
            styleFlagIcon.fixedWidth = 20;

            styleAlarmListArea = new GUIStyle(styleDefTextArea);
            styleAlarmListArea.padding = KACUtils.SetRectOffset(styleAlarmListArea.padding, 0);
            styleAlarmListArea.margin = KACUtils.SetRectOffset(styleAlarmListArea.margin, 0);

            styleAlarmText = new GUIStyle(styleDefLabel);
            styleAlarmText.normal.textColor = Color.white;
            styleAlarmText.alignment = TextAnchor.MiddleLeft;
            styleAlarmText.wordWrap = true;
            styleAlarmText.stretchWidth = true;
            //styleAlarmText.wordWrap = false;
            //styleAlarmText.stretchWidth = false;
            //styleAlarmText.clipping = TextClipping.Clip;

            //this doesn't work unless you set the background texture apparently - without the stock backgrounds its a bit difficult to match graphically
            //styleAlarmText.hover.textColor = Color.red;

            //styleAlarmTextGrayed = new GUIStyle(styleAlarmText);
            //styleAlarmTextGrayed.normal.textColor = Color.gray;

            styleAlarmIcon = new GUIStyle(styleDefLabel);
            styleAlarmIcon.alignment = TextAnchor.UpperCenter;
            styleAlarmIcon.fixedWidth = 18;

            styleLabelWarp = new GUIStyle(styleDefLabel);
            styleLabelWarp.alignment = TextAnchor.MiddleRight;
            styleLabelWarp.fixedWidth = 18;
            styleLabelWarpGrayed = new GUIStyle(styleLabelWarp);
            styleLabelWarpGrayed.normal.textColor = Color.gray;



            //styleSOIIndicator = new GUIStyle(styleDefLabel);
            //styleSOIIndicator.alignment = TextAnchor.MiddleLeft;
            ////styleSOIIndicator.fontSize = 11;
            //styleSOIIndicator.normal.textColor = new Color32(0, 112, 227, 255);
            //styleSOIIndicator.padding = KACUtils.SetRectOffset(styleSOIIndicator.padding, 0);

            //styleSOIIcon = new GUIStyle(styleSOIIndicator);


            styleAddSectionHeading = new GUIStyle(styleDefLabel);
            styleAddSectionHeading.normal.textColor = Color.white;
            styleAddSectionHeading.fontStyle = FontStyle.Bold;
            styleAddSectionHeading.padding.bottom = 0;
            styleAddSectionHeading.margin.bottom = 0;

            styleAddHeading = new GUIStyle(styleDefLabel);
            //styleAddHeading.normal.textColor = colLabelText;
            styleAddHeading.stretchWidth = false;
            styleAddHeading.alignment = TextAnchor.MiddleLeft;

            styleAddField = new GUIStyle(styleDefTextField);
            styleAddField.stretchWidth = true;
            styleAddField.alignment = TextAnchor.UpperLeft;
            styleAddField.normal.textColor = Color.yellow;

            styleAddFieldError = new GUIStyle(styleAddField);
            styleAddFieldError.normal.textColor = Color.red;

            styleAddFieldLocked = new GUIStyle(styleAddField);
            styleAddFieldLocked.normal.textColor = Color.gray;
            styleAddFieldLocked.fontStyle = FontStyle.Italic;

            styleAddFieldGreen = new GUIStyle(styleAddField);
            styleAddFieldGreen.normal.textColor = Color.green;

            styleAddMessageField = new GUIStyle(styleAddField);
            styleAddMessageField.wordWrap = true;
            styleAddMessageField.stretchHeight = true;
            styleAddMessageField.stretchWidth = false;

            //styleAddFieldErrorOverlay = new GUIStyle(styleDefLabel);
            //styleAddFieldErrorOverlay.normal.background = txtRedTint;
            //styleAddFieldErrorOverlay.border = new RectOffset(6, 6, 6, 6);

            styleAddFieldAreas = new GUIStyle(styleDefTextArea);
            styleAddFieldAreas.padding = KACUtils.SetRectOffset(styleAddFieldAreas.padding, 4);
            styleAddFieldAreas.margin.left = 0;
            styleAddFieldAreas.margin.right = 0;

            styleAddAlarmArea = new GUIStyle();
            styleAddAlarmArea.padding = KACUtils.SetRectOffset(styleAddAlarmArea.padding, 4);
            styleAddAlarmArea.margin.left = 0;
            styleAddAlarmArea.margin.right = 0;

            styleAddXferName = new GUIStyle(styleDefLabel);
            styleAddXferName.normal.textColor = Color.yellow;

            styleAddXferButton = new GUIStyle(styleDefButton);
            styleAddXferButton.fixedWidth = 40;
            styleAddXferButton.fixedHeight = 20;
            styleAddXferButton.fontSize = 11;
            styleAddXferButton.alignment = TextAnchor.MiddleCenter;

            styleAddXferOriginButton = new GUIStyle(styleDefButton);
            styleAddXferOriginButton.fixedWidth = 60;
            styleAddXferOriginButton.fixedHeight = 20;
            styleAddXferOriginButton.fontSize = 11;
            styleAddXferOriginButton.alignment = TextAnchor.MiddleCenter;


            styleContractLabelOffer = new GUIStyle(styleLabel);
            //styleContractLabelOffer.normal.textColor = Color.yellow;
            styleContractLabelActive = new GUIStyle(styleLabel);
            styleContractLabelActive.normal.textColor = new Color32(183, 254, 0, 255);
            styleContractLabelAlarmExists = new GUIStyle(styleLabel);
            styleContractLabelAlarmExists.normal.textColor = new Color32(128,128,128,255);


            styleAlarmMessage = new GUIStyle(styleDefLabel);
            //styleAlarmMessage.normal.textColor = colLabelText;
            styleAlarmMessageTime = new GUIStyle(styleDefLabel);
            styleAlarmMessageTime.normal.textColor = Color.yellow;
            styleAlarmMessageAction = new GUIStyle(styleDefLabel);
            styleAlarmMessageAction.stretchWidth = true;
            styleAlarmMessageAction.stretchHeight = true;
            styleAlarmMessageAction.alignment = TextAnchor.MiddleRight;
            styleAlarmMessageAction.normal.textColor = Color.yellow;

            styleAlarmMessageActionPause = new GUIStyle(styleAlarmMessageAction);
            styleAlarmMessageActionPause.normal.textColor = Color.red;

            styleVersionHighlight = new GUIStyle(styleDefLabel);
            styleVersionHighlight.normal.textColor = Color.yellow;
            styleVersionHighlight.fontStyle = FontStyle.Bold;
            styleVersionHighlight.alignment = TextAnchor.MiddleRight;
            styleVersionHighlight.stretchWidth = true;


            styleHeadingEarth = new GUIStyle(styleHeading);
            styleHeadingEarth.normal.textColor = new Color32(0, 173, 236, 255);
            styleContentEarth = new GUIStyle(styleContent);
            styleContentEarth.normal.textColor = new Color32(0, 173, 236, 255);


            styleDropDownListItem = new GUIStyle();
            styleDropDownListItem.normal.textColor = new Color(207, 207, 207);
            Texture2D texBack = CreateColorPixel(new Color(207, 207, 207));
            styleDropDownListItem.hover.background = texBack;
            styleDropDownListItem.onHover.background = texBack;
            styleDropDownListItem.hover.textColor = Color.black;
            styleDropDownListItem.onHover.textColor = Color.black;
            styleDropDownListItem.padding = new RectOffset(4, 4, 3, 4);

            styleDropDownGlyph = new GUIStyle();
            styleDropDownGlyph.alignment = TextAnchor.MiddleCenter;

            styleSeparatorV = new GUIStyle();
            styleSeparatorV.normal.background = texSeparatorV;
            styleSeparatorV.border = new RectOffset(0, 0, 6, 6);
            styleSeparatorV.fixedWidth = 2;

            styleSeparatorH = new GUIStyle();
            styleSeparatorH.normal.background = texSeparatorH;
            styleSeparatorH.border = new RectOffset(6, 6, 0, 0);
            styleSeparatorH.fixedHeight = 2;

            lstAlarmChoices = new List<GUIContent>();
            lstAlarmChoices.Add(new GUIContent(btnActionNothingAndDelete, KACAlarm.AlarmActionEnum.DoNothingDeleteWhenPassed.Description()));
            lstAlarmChoices.Add(new GUIContent(btnActionNothing, KACAlarm.AlarmActionEnum.DoNothing.Description()));
            lstAlarmChoices.Add(new GUIContent(btnActionMsg, KACAlarm.AlarmActionEnum.MessageOnly.Description()));
            lstAlarmChoices.Add(new GUIContent(btnActionWarp, KACAlarm.AlarmActionEnum.KillWarpOnly.Description()));
            lstAlarmChoices.Add(new GUIContent(btnActionWarpMsg, KACAlarm.AlarmActionEnum.KillWarp.Description()));
            lstAlarmChoices.Add(new GUIContent(btnActionPause, KACAlarm.AlarmActionEnum.PauseGame.Description()));


            lstAlarmWarpChoices = new List<GUIContent>();
            lstAlarmWarpChoices.Add(new GUIContent(btnActionNothing, AlarmActions.WarpEnum.DoNothing.Description()));
            lstAlarmWarpChoices.Add(new GUIContent(btnActionWarp, AlarmActions.WarpEnum.KillWarp.Description()));
            lstAlarmWarpChoices.Add(new GUIContent(btnActionPause, AlarmActions.WarpEnum.PauseGame.Description()));

            lstAlarmMessageChoices = new List<GUIContent>();
            lstAlarmMessageChoices.Add(new GUIContent(btnActionNoMsg, AlarmActions.MessageEnum.No.Description()));
            lstAlarmMessageChoices.Add(new GUIContent(btnActionMsg, AlarmActions.MessageEnum.Yes.Description()));
            lstAlarmMessageChoices.Add(new GUIContent(btnActionMsgVessel, AlarmActions.MessageEnum.YesIfOtherVessel.Description()));

        }

        /// <summary>
        /// Creates a 1x1 texture
        /// </summary>
        /// <param name="Background">Color of the texture</param>
        /// <returns></returns>
        internal static Texture2D CreateColorPixel(Color32 Background)
        {
            Texture2D retTex = new Texture2D(1, 1);
            retTex.SetPixel(0, 0, Background);
            retTex.Apply();
            return retTex;
        }

        #endregion


        #region "Functions"
        //internal static Color PulseColor(Color Start, Color Dest)
        //{
        //    Color colReturn = Start;
        //    Double intHundredth = Math.Truncate(DateTime.Now.Millisecond / 100d);
        //    switch (Convert.ToInt64(intHundredth))
        //    {
        //        case 0:
        //            colReturn=Start;
        //            break;
        //        case 1:
        //        case 9:
        //            colReturn.r = ((Dest.r - Start.r)*1/5) + Start.r;
        //            colReturn.g = ((Dest.g - Start.g)*1/5) + Start.g;
        //            colReturn.b = ((Dest.b - Start.b)*1/5) + Start.b;
        //            break;
        //        case 2:
        //        case 8:
        //            colReturn.r = ((Dest.r - Start.r)*2/5) + Start.r;
        //            colReturn.g = ((Dest.g - Start.g)*2/5) + Start.g;
        //            colReturn.b = ((Dest.b - Start.b)*2/5) + Start.b;
        //            break;
        //        case 3:
        //        case 7:
        //            colReturn.r = ((Dest.r - Start.r)*3/5) + Start.r;
        //            colReturn.g = ((Dest.g - Start.g)*3/5) + Start.g;
        //            colReturn.b = ((Dest.b - Start.b)*3/5) + Start.b;
        //            break;
        //        case 4:
        //        case 6:
        //            colReturn.r = ((Dest.r - Start.r)*4/5) + Start.r;
        //            colReturn.g = ((Dest.g - Start.g)*5/5) + Start.g;
        //            colReturn.b = ((Dest.b - Start.b)*4/5) + Start.b;
        //            break;
        //        case 5:
        //            colReturn=Dest;
        //            break;
        //        default:
        //            colReturn=Start;
        //            break;
        //    }
        //    return colReturn;
        //}
        #endregion

        #region "Data"
        internal static List<KACXFerModelPoint> lstXferModelPoints;

        internal static Boolean LoadModelPoints()
        {
            MonoBehaviourExtended.LogFormatted("Loading Transfer Modelling Data");
            Boolean blnReturn = false;
            try
            {
                lstXferModelPoints = new List<KACXFerModelPoint>();

                //read in the data file
                //String strData = KSP.IO.File.ReadAllText<KerbalAlarmClock>("data_TransferModelData.csv");
                String strData = System.IO.File.ReadAllText(KACUtils.PathPluginData + "/data_TransferModelData.csv");
                //split to lines
                String[] strLines = strData.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                String[] strFields;
                for (int intLine = 1; intLine < strLines.Length; intLine++)
                {
                    strFields = strLines[intLine].Split(",".ToCharArray());
                    lstXferModelPoints.Add(new KACXFerModelPoint(
                        Convert.ToDouble(strFields[0]),
                        Convert.ToInt32(strFields[1]),
                        Convert.ToInt32(strFields[2]),
                        Convert.ToDouble(strFields[3])
                        ));
                }
                blnReturn = true;
                MonoBehaviourExtended.LogFormatted("Transfer Modelling Data Load Complete");
            }
            catch (Exception ex)
            {
                MonoBehaviourExtended.LogFormatted("Transfer Modelling Data Failed - is the data file there and correct\r\n{0}", ex.Message);
            }
            return blnReturn;
        }
        #endregion
    }
}
