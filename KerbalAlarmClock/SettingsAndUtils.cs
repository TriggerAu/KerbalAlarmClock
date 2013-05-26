using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

using UnityEngine;
using KSP;

namespace KerbalAlarmClock
{
    public static class KACUtils
    {
        public static String AppPath = KSPUtil.ApplicationRootPath.Replace("\\", "/");
        public static String PlugInPath = AppPath + "PluginData/KerbalAlarmClock/";

        //generic function
        public static String PipeSepVariables(params object[] vars)
        {
            return SepVariables("|", vars);
        }

        public static String CommaSepVariables(params object[] vars)
        {
            return SepVariables(",", vars);
        }

        public static String SepVariables(String separator,params object[] vars)
        {
            String strReturn = "";
            foreach (object tmpVar in vars)
            {
                if (strReturn != "") strReturn += separator;
                if (tmpVar == null)
                    strReturn += "";
                else
                    strReturn += tmpVar.ToString();
            }
            return strReturn;
        }

        public static Byte[] LoadFileToArray(String Filename)
        {
            Byte[] arrBytes;

            arrBytes = KSP.IO.File.ReadAllBytes<KerbalAlarmClock>(Filename);

            return arrBytes;
        }

        public static void SaveFileFromArray(Byte[] data,String Filename)
        {
            KSP.IO.File.WriteAllBytes<KerbalAlarmClock>(data,Filename);
        }


        public static void LoadImageIntoTexture(ref Texture2D tex, String FileName)
        {

            try
            {
                //KACWorker.DebugLogFormatted("Loading: TriggerTech/Textures/KerbalAlarmClock/{0}", FileName);
                tex = GameDatabase.Instance.GetTexture("TriggerTech/Textures/KerbalAlarmClock/" + FileName.Replace(".png", ""), false);
                //if (tex == null) KACWorker.DebugLogFormatted("Textures Empty");

                //tex.LoadImage(LoadFileToArray(FileName));
            }
            catch (Exception)
            {
                KACWorker.DebugLogFormatted("Failed to load (are you missing a file):{0}", FileName);
            }
        }

        //stop using unity www object as some clients get timeouts searching via the url address

        //public static void LoadImageIntoTexture(ref Texture2D tex, String FileName)
        //{
        //    WWW img1 = new WWW(String.Format("file://{0}Icons/{1}", PlugInPath, FileName));
        //    img1.LoadImageIntoTexture(tex);
        //}

        //public static void LoadImageIntoTexture(ref Texture2D tex, String FolderName, String FileName)
        //{
        //    WWW img1 = new WWW(String.Format("file://{0}{1}/{2}", PlugInPath, FolderName,FileName));
        //    img1.LoadImageIntoTexture(tex);
        //}

        #region "offset building"
        public static RectOffset SetWindowRectOffset(RectOffset tmpRectOffset, int intValue)
        {
            tmpRectOffset.left = intValue;
            //tmpRectOffset.top = Top;
            tmpRectOffset.right = intValue;
            tmpRectOffset.bottom = intValue;
            return tmpRectOffset;
        }

        public static RectOffset SetRectOffset(RectOffset tmpRectOffset, int intValue)
        {
            return SetRectOffset(tmpRectOffset, intValue, intValue, intValue, intValue);
        }

        public static RectOffset SetRectOffset(RectOffset tmpRectOffset, int Left,int Right,int Top, int Bottom)
        {
            tmpRectOffset.left = Left;
            tmpRectOffset.top = Top;
            tmpRectOffset.right = Right ;
            tmpRectOffset.bottom = Bottom;
            return tmpRectOffset;
        }
        #endregion

        #region "Math Stuff"
        public static double Clamp(double x, double min, double max)
        {
            return Math.Min(Math.Max(x, min), max);
        }

        //keeps angles in the range -180 to 180
        public static double clampDegrees(double angle)
        {
            angle = angle + ((int)(2 + Math.Abs(angle) / 360)) * 360.0; //should be positive
            angle = angle % 360.0;
            if (angle > 180.0) return angle - 360.0;
            else return angle;
        }

        //keeps angles in the range 0 to 360
        public static double clampDegrees360(double angle)
        {
            angle = angle % 360.0;
            if (angle < 0) return angle + 360.0;
            else return angle;
        }
#endregion

        #region "Orbital Math"

        //returns false if there is no AN/DN on the flight plan
        public static Boolean CalcTimeToANorDN(Vessel vessel, ANDNNodeType typeOfNode, out Double timeToNode)
        {
            Boolean blnReturn = false;
            timeToNode = 0;
            try 
	        {
                //work out the target type, and get the target orbit
                if (FlightGlobals.fetch.VesselTarget!=null)
                {

                    ITargetable target = FlightGlobals.fetch.VesselTarget;
                    Orbit oTarget = target.GetOrbit();
                    Vector3d vectVesselPos = vessel.orbit.getRelativePositionAtUT(KACWorkerGameState.CurrentTime.UT);

                    blnReturn = CalcTimeToANorDN(vectVesselPos,vessel.orbit,oTarget,typeOfNode,out timeToNode);
                }
	        }
	        catch (Exception)
	        {

            }
            return blnReturn;
        }



        #region "AN/DN Code - predominantly the functions from the Kerbal Engineer Redux by cybutek under Creative commons BY-NC-SA - http://creativecommons.org/licenses/by-nc-sa/3.0/deed.en_GB"
        public static Boolean CalcTimeToANorDN(Vector3d position, Orbit origin, Orbit target, ANDNNodeType typeOfNode, out Double timeToNode)
        {
            timeToNode = 0d;
            Boolean blnReturn = false;
            try
            {
                double AngleToANDN;
                if (typeOfNode== ANDNNodeType.Ascending)
                    AngleToANDN = CalcAngleToAscendingNode(position,origin,target);
                else
                    AngleToANDN = CalcAngleToDescendingNode(position,origin,target);
                timeToNode = CalcTimeToNode(origin, AngleToANDN);
                blnReturn = true;
            }
            catch (Exception)
            {
                //
            }
            return blnReturn;
        }

        public enum ANDNNodeType
        {
            Ascending,
            Descending
        }


        public static double CalcAngleToAscendingNode(Vector3d position, Orbit origin, Orbit target)
        {
            double angleToNode = 0d;

            if (origin.inclination < 90)
            {
                angleToNode = CalcPhaseAngle(position, GetAscendingNode(origin, target));
            }
            else
            {
                angleToNode = 360 - CalcPhaseAngle(position, GetAscendingNode(origin, target));
            }

            return angleToNode;
        }

        public static double CalcAngleToDescendingNode(Vector3d position, Orbit origin, Orbit target)
        {
            double angleToNode = 0d;

            if (origin.inclination < 90)
            {
                angleToNode = CalcPhaseAngle(position, GetDescendingNode(origin, target));
            }
            else
            {
                angleToNode = 360 - CalcPhaseAngle(position, GetDescendingNode(origin, target));
            }

            return angleToNode;
        }

        public static Vector3d GetAscendingNode(Orbit origin, Orbit target)
        {
            //get the vector at 90 degrees to the two orbits normal so we see the cross over AN
            return Vector3d.Cross(target.GetOrbitNormal(), origin.GetOrbitNormal());
        }
        public static Vector3d GetDescendingNode(Orbit origin, Orbit target)
        {
            //get the vector at 90 degrees to the two orbits normal so we see the cross over AN
            return Vector3d.Cross(origin.GetOrbitNormal(), target.GetOrbitNormal());
        }

        public static double CalcPhaseAngle(Vector3d origin, Vector3d target)
        {
            //angle between the two vectors
            double phaseAngle = Vector3d.Angle(target, origin);
            if (Vector3d.Angle(Quaternion.AngleAxis(90, Vector3d.forward) * origin, target) > 90)
            {
                phaseAngle = 360 - phaseAngle;
            }
            return (phaseAngle + 360) % 360;
        }
        public static double CalcTimeToNode(Orbit origin, double angleToNode)
        {
            return (origin.period / 360d) * angleToNode;
        }
        #endregion


        #endregion

    }

    public static class KACResources
    {
        #region "Textures"
        
        //Clock Icons
        public static Texture2D iconNorm = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconNormShow = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconAlarm = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconAlarmShow = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpEffect100 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpEffect080 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpEffect060 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpEffect040 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpEffect020 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpEffect000 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseEffect100 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseEffect080 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseEffect060 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseEffect040 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseEffect020 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseEffect000 = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        //Alarm List icons
        public static Texture2D iconMNode = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconSOI = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconAp = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconPe = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconAN = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconDN = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconXFer = new Texture2D(18, 14, TextureFormat.ARGB32, false);

        public static Texture2D iconNone = new Texture2D(18, 14, TextureFormat.ARGB32, false);
        public static Texture2D iconEdit = new Texture2D(16, 16, TextureFormat.ARGB32, false);

        public static Texture2D iconWarpList100 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpList080 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpList060 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpList040 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpList020 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconWarpList000 = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        public static Texture2D iconPauseList100 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseList080 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseList060 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseList040 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseList020 = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D iconPauseList000 = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        //public static Texture2D iconstatusSOI = new Texture2D(14, 11, TextureFormat.ARGB32, false);


        public static Texture2D btnRaw = new Texture2D(20, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnMNode = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnAp = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnPe = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnAN = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnDN = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnSOI = new Texture2D(25, 20, TextureFormat.ARGB32, false);
        public static Texture2D btnXfer = new Texture2D(25, 20, TextureFormat.ARGB32, false);

        public static Texture2D btnChevronUp = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnChevronDown = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnChevLeft = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnChevRight = new Texture2D(17,16, TextureFormat.ARGB32, false);

        public static Texture2D btnRedCross = new Texture2D(16, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnSettings = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnSettingsAttention = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnAdd = new Texture2D(17, 16, TextureFormat.ARGB32, false);

        public static Texture2D txtTooltipBackground = new Texture2D(9, 9);//, TextureFormat.ARGB32, false);
        //public static Texture2D txtRedTint = new Texture2D(16, 16);//, TextureFormat.ARGB32, false);
        //public static Texture2D txtBlackSquare = new Texture2D(5, 5);//, TextureFormat.ARGB32, false);
        //public static Texture2D txtWhiteSquare = new Texture2D(5, 5);//, TextureFormat.ARGB32, false);
        
        public static void loadGUIAssets()
        {
            KACWorker.DebugLogFormatted("Loading Textures");

            try
            {
                KACUtils.LoadImageIntoTexture(ref iconNorm, "img_iconNorm.png");
                KACUtils.LoadImageIntoTexture(ref iconNormShow,"img_iconNormShow.png");
                KACUtils.LoadImageIntoTexture(ref iconAlarm,"img_iconAlarm.png");
                KACUtils.LoadImageIntoTexture(ref iconAlarmShow,"img_iconAlarmShow.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect100,"img_iconWarpEffect2_100.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect080,"img_iconWarpEffect2_080.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect060,"img_iconWarpEffect2_060.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect040,"img_iconWarpEffect2_040.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect020,"img_iconWarpEffect2_020.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpEffect000,"img_iconWarpEffect2_000.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseEffect100,"img_iconPauseEffect_100.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseEffect080,"img_iconPauseEffect_080.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseEffect060,"img_iconPauseEffect_060.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseEffect040,"img_iconPauseEffect_040.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseEffect020,"img_iconPauseEffect_020.png");
                KACUtils.LoadImageIntoTexture(ref  iconPauseEffect000,"img_iconPauseEffect_000.png");


                KACUtils.LoadImageIntoTexture(ref iconSOI, "img_listiconSOI.png");
                KACUtils.LoadImageIntoTexture(ref iconMNode, "img_listiconMNode.png");
                KACUtils.LoadImageIntoTexture(ref iconAp, "img_listiconAp.png");
                KACUtils.LoadImageIntoTexture(ref iconPe, "img_listiconPe.png");
                KACUtils.LoadImageIntoTexture(ref iconAN, "img_listiconAN.png");
                KACUtils.LoadImageIntoTexture(ref iconDN, "img_listiconDN.png");
                KACUtils.LoadImageIntoTexture(ref iconXFer, "img_listiconXfer.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList100, "img_listiconWarpList_100.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList080,"img_listiconWarpList_080.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList060,"img_listiconWarpList_060.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList040,"img_listiconWarpList_040.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList020,"img_listiconWarpList_020.png");
                KACUtils.LoadImageIntoTexture(ref iconWarpList000,"img_listiconWarpList_000.png");

                KACUtils.LoadImageIntoTexture(ref iconPauseList100,"img_listiconPauseList_100.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseList080,"img_listiconPauseList_080.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseList060,"img_listiconPauseList_060.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseList040,"img_listiconPauseList_040.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseList020,"img_listiconPauseList_020.png");
                KACUtils.LoadImageIntoTexture(ref iconPauseList000,"img_listiconPauseList_000.png");

                KACUtils.LoadImageIntoTexture(ref iconNone, "img_listiconNone.png");
                KACUtils.LoadImageIntoTexture(ref iconEdit, "img_listiconEdit.png");

                //KACUtils.LoadImageIntoTexture(ref iconstatusSOI, "img_statusiconSOI.png");

                KACUtils.LoadImageIntoTexture(ref btnRaw, "img_buttonTypeRaw.png");
                KACUtils.LoadImageIntoTexture(ref btnMNode, "img_buttonTypeMNode.png");
                KACUtils.LoadImageIntoTexture(ref btnAp, "img_buttonTypeAp.png");
                KACUtils.LoadImageIntoTexture(ref btnPe, "img_buttonTypePe.png");
                KACUtils.LoadImageIntoTexture(ref btnAN, "img_buttonTypeAN.png");
                KACUtils.LoadImageIntoTexture(ref btnDN, "img_buttonTypeDN.png");
                KACUtils.LoadImageIntoTexture(ref btnSOI, "img_buttonTypeSOI.png");
                KACUtils.LoadImageIntoTexture(ref btnXfer, "img_buttonTypeXfer.png");


                KACUtils.LoadImageIntoTexture(ref btnChevronUp, "img_buttonChevronUp.png");
                KACUtils.LoadImageIntoTexture(ref btnChevronDown, "img_buttonChevronDown.png");
                KACUtils.LoadImageIntoTexture(ref btnChevLeft, "img_buttonChevronLeft.png");
                KACUtils.LoadImageIntoTexture(ref btnChevRight, "img_buttonChevronRight.png");

                KACUtils.LoadImageIntoTexture(ref btnRedCross, "img_buttonRedCross.png");
                KACUtils.LoadImageIntoTexture(ref btnSettings, "img_buttonSettings.png");
                KACUtils.LoadImageIntoTexture(ref btnSettingsAttention, "img_buttonSettingsAttention.png");
                KACUtils.LoadImageIntoTexture(ref btnAdd, "img_buttonAdd.png");

                KACUtils.LoadImageIntoTexture(ref txtTooltipBackground, "txt_TooltipBackground.png");


                //KACUtils.LoadImageIntoTexture(ref txtRedTint, "Textures", "RedOverlay.png");
                
                //KACUtils.LoadImageIntoTexture(ref txtBlackSquare, "Textures", "BlackSquare.png");
                //KACUtils.LoadImageIntoTexture(ref txtWhiteSquare, "Textures", "WhiteSquare.png");

                KACWorker.DebugLogFormatted("Loaded Textures");
            }
            catch (Exception)
            {
                KACWorker.DebugLogFormatted("Failed to Load Textures - are you missing a file?");
            }


        }

        public static Texture2D GetSettingsButtonIcon(Boolean AttentionRequired)
        {
            Texture2D textureReturn;

            //Only flash if we need attention
            if (AttentionRequired && DateTime.Now.Millisecond < 500)
                textureReturn = btnSettingsAttention;
            else
                textureReturn = btnSettings;

            return textureReturn;
        }
        public static Texture2D GetWarpIcon()
        {
            Texture2D textureReturn;

            Double intHundredth = Math.Truncate(DateTime.Now.Millisecond / 100d);
            switch (Convert.ToInt64(intHundredth))
            {
                case 0:
                    textureReturn = KACResources.iconWarpEffect100;
                    break;
                case 1:
                case 9:
                    textureReturn = KACResources.iconWarpEffect080;
                    break;
                case 2:
                case 8:
                    textureReturn = KACResources.iconWarpEffect060;
                    break;
                case 3:
                case 7:
                    textureReturn = KACResources.iconWarpEffect040;
                    break;
                case 4:
                case 6:
                    textureReturn = KACResources.iconWarpEffect020;
                    break;
                case 5:
                    textureReturn = KACResources.iconWarpEffect000;
                    break;
                default:
                    textureReturn = KACResources.iconWarpEffect100;
                    break;
            }
            return textureReturn;
        }

        public static Texture2D GetPauseIcon()
        {
            Texture2D textureReturn;

            Double intHundredth = Math.Truncate(DateTime.Now.Millisecond / 100d);
            switch (Convert.ToInt64(intHundredth))
            {
                case 0:
                    textureReturn = KACResources.iconPauseEffect100;
                    break;
                case 1:
                case 9:
                    textureReturn = KACResources.iconPauseEffect080;
                    break;
                case 2:
                case 8:
                    textureReturn = KACResources.iconPauseEffect060;
                    break;
                case 3:
                case 7:
                    textureReturn = KACResources.iconPauseEffect040;
                    break;
                case 4:
                case 6:
                    textureReturn = KACResources.iconPauseEffect020;
                    break;
                case 5:
                    textureReturn = KACResources.iconPauseEffect000;
                    break;
                default:
                    textureReturn = KACResources.iconPauseEffect100;
                    break;
            }
            return textureReturn;
        }

        public static Texture2D GetWarpListIcon(Boolean blnWarpInfluence)
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

        public static Texture2D GetPauseListIcon(Boolean blnPauseInfluence)
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


        #region "Styles"
        //Styles for windows - Cant initialize the objects here as the GUIStyle Constructor cannot be called outside of OnGUI
        
        public static GUIStyle styleWindow;
        public static GUIStyle styleTooltipStyle;

        public static GUIStyle styleIconStyle; 
        public static GUIStyle styleHeading;
        public static GUIStyle styleContent;
        public static GUIStyle styleButton;

        public static GUIStyle styleLabel;
        public static GUIStyle styleLabelWarning;
        public static GUIStyle styleLabelError;

        public static GUIStyle styleCheckbox;
        public static GUIStyle styleCheckboxLabel;

        public static GUIStyle styleButtonList;

        public static GUIStyle styleSmallButton;

        public static GUIStyle styleFlagIcon;
        
        //List Styles
        public static GUIStyle styleAlarmListArea;
        public static GUIStyle styleAlarmText;
        //public static GUIStyle styleAlarmTextGrayed;
        public static GUIStyle styleAlarmIcon;
        public static GUIStyle styleLabelWarp;
        public static GUIStyle styleLabelWarpGrayed;
        public static GUIStyle styleSOIIndicator;
        public static GUIStyle styleSOIIcon;
		
        //Add Alarm Styles
        public static GUIStyle styleAddSectionHeading;
        public static GUIStyle styleAddHeading;
        public static GUIStyle styleAddField;
        public static GUIStyle styleAddFieldError;
        //public static GUIStyle styleAddFieldErorOverlay;
        public static GUIStyle styleAddFieldGreen;
        public static GUIStyle styleAddFieldAreas;
        public static GUIStyle styleAddAlarmArea;
        public static GUIStyle styleAddXferName;
        public static GUIStyle styleAddXferButton;
        public static GUIStyle styleAddXferOriginButton;
        public static GUIStyle styleAddMessageField;
        
        //AlarmMessage Styles
        public static GUIStyle styleAlarmMessage;
        public static GUIStyle styleAlarmMessageTime;
        public static GUIStyle styleAlarmMessageAction;
        public static GUIStyle styleAlarmMessageActionPause;

        public static GUIStyle styleVersionHighlight;

        /// <summary>
        /// Sets up the styles for the different parts of the drawing
        /// Should only be called once
        /// </summary>
        public static void SetStyles()
        {
            Color32 colLabelText = new Color32(220, 220, 220, 255);
            int intFontSizeDefault = 13;

            //Common starting points
            GUIStyle styleDefLabel = new GUIStyle(GUI.skin.label);
            styleDefLabel.fontSize = intFontSizeDefault;
            styleDefLabel.fontStyle = FontStyle.Normal;
            styleDefLabel.normal.textColor = colLabelText;
            styleDefLabel.hover.textColor = Color.blue;

            GUIStyle styleDefTextField = new GUIStyle(GUI.skin.textField);
            styleDefTextField.fontSize = intFontSizeDefault;
            styleDefTextField.fontStyle = FontStyle.Normal;
            GUIStyle styleDefTextArea = new GUIStyle(GUI.skin.textArea);
            styleDefTextArea.fontSize = intFontSizeDefault;
            styleDefTextArea.fontStyle = FontStyle.Normal;
            GUIStyle styleDefToggle = new GUIStyle(GUI.skin.toggle);
            styleDefToggle.fontSize = intFontSizeDefault;
            styleDefToggle.fontStyle = FontStyle.Normal;
            GUIStyle styleDefButton = new GUIStyle(GUI.skin.button);
            styleDefToggle.fontSize = intFontSizeDefault;
            styleDefToggle.fontStyle = FontStyle.Normal;

            //Set up the used styles
            styleIconStyle = new GUIStyle();

            styleWindow = new GUIStyle(GUI.skin.window) ;
            styleWindow.padding = KACUtils.SetWindowRectOffset(styleWindow.padding, 4);
            //styleWindow.normal.background = KACResources.txtWhiteSquare;
            //styleWindow.normal.textColor = new Color32(183, 254, 0, 255);
            //styleWindow.normal.textColor = Color.red;

            styleTooltipStyle = new GUIStyle(styleDefLabel);
            styleTooltipStyle.fontSize = 12;
            styleTooltipStyle.normal.textColor = new Color32(207,207,207,255);
            styleTooltipStyle.stretchHeight = true;
            styleTooltipStyle.wordWrap = true;
            styleTooltipStyle.normal.background = txtTooltipBackground;
            //Extra border to prevent bleed of color - actual border is only 1 pixel wide
            styleTooltipStyle.border = new RectOffset(3, 3, 3, 3);
            styleTooltipStyle.padding = new RectOffset(4, 4, 6, 4);
            styleTooltipStyle.alignment = TextAnchor.MiddleCenter;

            styleHeading = new GUIStyle(styleDefLabel);
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

            styleButtonList = new GUIStyle(styleDefButton);
            styleButtonList.fixedHeight = 26;
            styleButtonList.padding = KACUtils.SetRectOffset(styleButtonList.padding, 0);

            styleSmallButton = new GUIStyle(GUI.skin.button);
            styleSmallButton.alignment = TextAnchor.MiddleCenter;
            styleSmallButton.fixedWidth = 30;
            styleSmallButton.fixedHeight = 20;
            styleSmallButton.fontSize = intFontSizeDefault;
            styleSmallButton.fontStyle = FontStyle.Normal;
            styleSmallButton.padding = KACUtils.SetRectOffset(styleSmallButton.padding, 0);

            styleFlagIcon = new GUIStyle(styleDefLabel);
            styleFlagIcon.padding = KACUtils.SetRectOffset(styleFlagIcon.padding, 0);
            styleFlagIcon.alignment = TextAnchor.MiddleLeft;
            styleFlagIcon.fixedWidth = 20;

            styleAlarmListArea = new GUIStyle(styleDefTextArea);
            styleAlarmListArea.padding =  KACUtils.SetRectOffset(styleAlarmListArea.padding, 0);
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

            styleLabelWarp = new GUIStyle(styleDefLabel);
            styleLabelWarp.alignment = TextAnchor.MiddleRight;
            styleLabelWarpGrayed = new GUIStyle(styleLabelWarp);
            styleLabelWarpGrayed.normal.textColor = Color.gray;



            styleSOIIndicator = new GUIStyle(styleDefLabel);
            styleSOIIndicator.alignment = TextAnchor.MiddleLeft;
            //styleSOIIndicator.fontSize = 11;
            styleSOIIndicator.normal.textColor = new Color32(0, 112, 227, 255);
            styleSOIIndicator.padding = KACUtils.SetRectOffset(styleSOIIndicator.padding, 0);

            styleSOIIcon = new GUIStyle(styleSOIIndicator); 
            

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
            styleAddFieldAreas.padding = KACUtils.SetRectOffset(styleAddFieldAreas.padding,4);
            styleAddFieldAreas.margin.left = 0;
            styleAddFieldAreas.margin.right = 0;

            styleAddAlarmArea = new GUIStyle();
            styleAddAlarmArea.padding= KACUtils.SetRectOffset(styleAddAlarmArea.padding, 4);
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
            styleAddXferOriginButton.fontSize=11;
            styleAddXferOriginButton.alignment = TextAnchor.MiddleCenter;


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
        }
        #endregion


        #region "Functions"
        //public static Color PulseColor(Color Start, Color Dest)
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
        public static List<KACXFerModelPoint> lstXferModelPoints;

        public static Boolean LoadModelPoints()
        {
            KACWorker.DebugLogFormatted("Loading Transfer Modelling Data");
            Boolean blnReturn = false;
            try
            {
                lstXferModelPoints = new List<KACXFerModelPoint>();

                //read in the data file
                String strData = KSP.IO.File.ReadAllText<KerbalAlarmClock>("data_TransferModelData.csv");
                //split to lines
                String[] strLines = strData.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                String[] strFields;
                for(int intLine=1;intLine<strLines.Length;intLine++)
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
                KACWorker.DebugLogFormatted("Transfer Modelling Data Load Complete");
            }
            catch (Exception ex)
            {
                KACWorker.DebugLogFormatted("Transfer Modelling Data Failed - is the data file there and correct\r\n{0}",ex.Message);
            }
            return blnReturn;
        }
        #endregion
    }

    /// <summary>
    /// Settings object
    /// </summary>
    public class KACSettings
    {
        public String Version="";
        public String VersionWeb="";
        public Boolean VersionAvailable
        {
            get
            {
                if (this.VersionWeb == "")
                    return false;
                else
                    try
                    {
                        //if there was a string and its version is greater than the current running one then alert
                        Version vTest = new Version(this.VersionWeb);
                        return (System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.CompareTo(vTest) < 0);
                    }
                    catch (Exception ex)
                    {
                        KACWorker.DebugLogFormatted("webversion: '{0}'", this.VersionWeb);
                        KACWorker.DebugLogFormatted("Unable to compare versions: {0}", ex.Message);
                        return false;
                    }

                //return ((this.VersionWeb != "") && (this.Version != this.VersionWeb));
            }
        }

        //Are we doing daily checks
        public Boolean DailyVersionCheck = true;
        public String VersionCheckResult = "";
        //attentionflag
        public Boolean VersionAttentionFlag = false;
        //When did we last check??
        public DateTime VersionCheckDate_Attempt;
        public String VersionCheckDate_AttemptString { get { return ConvertVersionCheckDateToString(this.VersionCheckDate_Attempt); } }
        public DateTime VersionCheckDate_Success;
        public String VersionCheckDate_SuccessString { get { return ConvertVersionCheckDateToString(this.VersionCheckDate_Success); } }

        private String ConvertVersionCheckDateToString(DateTime Date)
        {
            if (Date < DateTime.Now.AddYears(-10))
                return "No Date Recorded";
            else
                return String.Format("{0:yyyy-MM-dd}", Date);
        }

        public Boolean WindowVisible = false;
        public Boolean WindowMinimized = false;
        public Rect WindowPos;

        public Rect IconPos;

        public KACAlarmList Alarms = new KACAlarmList();

        public String AlarmListMaxAlarms="10";
        public int AlarmListMaxAlarmsInt
        {
            get
            {
                try
                {
                    return Convert.ToInt32(this.AlarmListMaxAlarms);
                }
                catch (Exception)
                {
                    return 10;
                }

            }
        }
        public int AlarmDefaultAction = 1;
        public double AlarmDefaultMargin = 60;
        public int AlarmPosition = 1;
        public Boolean AlarmDeleteOnClose = false;
        public Boolean HideOnPause = true;
        //public Boolean TimeAsUT = false;
        public KerbalTime.PrintTimeFormat TimeFormat = KerbalTime.PrintTimeFormat.KSPString;
        public Boolean ShowTooltips = true;

        public Boolean AlarmXferRecalc = true;
        public double AlarmXferRecalcThreshold = 180;
        public Boolean AlarmXferDisplayList = false;
        public Boolean XferModelLoadData = true;
        public Boolean XferModelDataLoaded = false;
        public Boolean XferUseModelData = false;

        public Boolean AlarmNodeRecalc = false;
        public double AlarmNodeRecalcThreshold = 180;

        public Boolean AlarmAddSOIAuto = false;
        public double AlarmAddSOIAutoThreshold = 180;
        public double AlarmAutoSOIMargin = 900;

        public Boolean AlarmSOIRecalc = false;
        public double AlarmSOIRecalcThreshold = 180;

        //public double AlarmAddSOIMargin = 120;
        public Boolean AlarmCatchSOIChange = false;
        public int AlarmOnSOIChange_Action = 1;


        public List<GameScenes> DrawScenes = new List<GameScenes> { GameScenes.FLIGHT };
        public List<GameScenes> BehaviourScenes = new List<GameScenes> { GameScenes.FLIGHT };
        public List<VesselType> VesselTypesForSOI = new List<VesselType>() { VesselType.Base, VesselType.Lander, VesselType.Probe, VesselType.Ship, VesselType.Station };
        public List<Orbit.PatchTransitionType> SOITransitions = new List<Orbit.PatchTransitionType> { Orbit.PatchTransitionType.ENCOUNTER, Orbit.PatchTransitionType.ESCAPE };

        public KACSettings()
        {
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //on each start set the attention flag to the property - should be on each program start
            VersionAttentionFlag=VersionAvailable;
        }

        public void Load()
        {
            try
            {
                KACWorker.DebugLogFormatted("Loading Config");
                 KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<KerbalAlarmClock>();
                configfile.load();

                this.DailyVersionCheck = configfile.GetValue("DailyUpdateCheck", true);

                try { this.VersionCheckDate_Attempt = DateTime.ParseExact(configfile.GetValue("VersionCheckDate_Attempt", ""), "yyyy-MM-dd", CultureInfo.CurrentCulture); }
                catch (Exception) { this.VersionCheckDate_Attempt = new DateTime();}

                try { this.VersionCheckDate_Success = DateTime.ParseExact(configfile.GetValue("VersionCheckDate_Success", ""), "yyyy-MM-dd", CultureInfo.CurrentCulture); }
                catch (Exception) { this.VersionCheckDate_Success = new DateTime(); }
                
                this.VersionWeb = configfile.GetValue("VersionWeb", "");
				
				this.WindowVisible = configfile.GetValue("WindowVisible", false);
                this.WindowMinimized = configfile.GetValue("WindowMinimized", false);
                this.WindowPos = configfile.GetValue<Rect>("WindowPos");
                this.WindowPos.height = 100;

                this.IconPos = configfile.GetValue<Rect>("IconPos", new Rect(152, 0, 32, 32));
                this.IconPos.height = 32; this.IconPos.width = 32;

                this.AlarmListMaxAlarms = configfile.GetValue("AlarmListMaxAlarms", "10");
                this.AlarmDefaultAction = configfile.GetValue<int>("AlarmDefaultAction", 1);
                this.AlarmDefaultMargin = configfile.GetValue<Double>("AlarmDefaultMargin", 60);
                this.AlarmPosition = configfile.GetValue<int>("AlarmPosition", 1);
                this.AlarmDeleteOnClose = configfile.GetValue("AlarmDeleteOnClose", false);
                this.ShowTooltips = configfile.GetValue("ShowTooltips", true);
                this.HideOnPause = configfile.GetValue("HideOnPause", true);
                this.TimeFormat = configfile.GetValue<KerbalTime.PrintTimeFormat>("TimeFormat", KerbalTime.PrintTimeFormat.KSPString);
                if (configfile.GetValue<bool>("TimeAsUT", false) == true)
                {
                    this.TimeFormat = KerbalTime.PrintTimeFormat.TimeAsUT;
                    configfile.SetValue("TimeAsUT", false);
                    configfile.SetValue("TimeFormat", Enum.GetName(typeof(KerbalTime.PrintTimeFormat), this.TimeFormat));
                    configfile.save();
                }

                this.AlarmXferRecalc = configfile.GetValue("AlarmXferRecalc", true);
                this.AlarmXferRecalcThreshold = configfile.GetValue<Double>("AlarmXferRecalcThreshold", 180);
                this.AlarmXferDisplayList = configfile.GetValue("AlarmXferDisplayList", false);
                this.XferUseModelData = configfile.GetValue("XferUseModelData", false);

                this.AlarmNodeRecalc = configfile.GetValue("AlarmNodeRecalc", false);
                this.AlarmNodeRecalcThreshold = configfile.GetValue<Double>("AlarmNodeRecalcThreshold", 180);
                
                this.AlarmAddSOIAuto = configfile.GetValue("AlarmAddSOIAuto", false);
                this.AlarmAddSOIAutoThreshold = configfile.GetValue<Double>("AlarmAddSOIAutoThreshold", 180);
                //this.AlarmAddSOIMargin = configfile.GetValue("AlarmAddSOIMargin", 120);
                this.AlarmAutoSOIMargin = configfile.GetValue<Double>("AlarmAutoSOIMargin", 900);
                this.AlarmCatchSOIChange = configfile.GetValue("AlarmOnSOIChange", false);
                this.AlarmOnSOIChange_Action = configfile.GetValue("AlarmOnSOIChange_Action", 1);

                this.AlarmSOIRecalc = configfile.GetValue("AlarmSOIRecalc", false);
                this.AlarmSOIRecalcThreshold = configfile.GetValue<Double>("AlarmSOIRecalcThreshold", 180);

                //HIGHLOGIC IS NOT YET SET HERE!!!
                if (KSP.IO.File.Exists<KerbalAlarmClock>(String.Format("Alarms-{0}.txt", HighLogic.CurrentGame.Title)))
                {
                    KACWorker.DebugLogFormatted("Trying New Alarms file..."); 
                    LoadAlarms();
                }
                else
                {
                    //Loop through numbers to Load Alarms
                    Alarms = new KACAlarmList();
                    int intAlarm = 0;
                    String strAlarm = "";
                    do
                    {
                        strAlarm = configfile.GetValue("Alarm_" + intAlarm, "");
                        KACWorker.DebugLogFormatted(strAlarm);
                        if (strAlarm != "")
                        {
                            KACAlarm tmpAlarm = new KACAlarm();
                            tmpAlarm.LoadFromString(strAlarm);
                            Alarms.Add(tmpAlarm);
                            intAlarm++;
                        }
                    } while (strAlarm != "");
                }
                KACWorker.DebugLogFormatted("Config Loaded Successfully");
            }

            catch (Exception ex)
            {
                KACWorker.DebugLogFormatted("Failed To Load Config");
                KACWorker.DebugLogFormatted(ex.Message);
            }

        }

        private void LoadAlarms()
        {
            string SettingsVersion = "2";
            Alarms = new KACAlarmList();
            KSP.IO.TextReader tr = KSP.IO.TextReader.CreateForType<KerbalAlarmClock>(String.Format("Alarms-{0}.txt", HighLogic.CurrentGame.Title));
            String strFile = tr.ReadToEnd();
            tr.Close();

            while (strFile.Contains("|<ENDLINE>"))
	        {
                String strAlarm = strFile.Substring(0,strFile.IndexOf("|<ENDLINE>"));
                strFile = strFile.Substring(strAlarm.Length + "|<ENDLINE>".Length).TrimStart("\r\n".ToCharArray());

                if (strAlarm.StartsWith("SettingsVersion|"))
                {
                    SettingsVersion = strAlarm.Split("|".ToCharArray())[1];
                }
                else if (!strAlarm.StartsWith("VesselID|"))
                {
                    KACAlarm tmpAlarm = new KACAlarm();

                    switch (SettingsVersion)
                    {
                        //case "3":
                        //    tmpAlarm.LoadFromString3(strAlarm);
                        //    break;
                        default:
                            tmpAlarm.LoadFromString2(strAlarm);
                            break;
                    }
                    
                    Alarms.Add(tmpAlarm);
                }
	        }
        }

        public void Save()
        {

            KACWorker.DebugLogFormatted("Saving Config");

            KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<KerbalAlarmClock>();

            configfile.SetValue("DailyUpdateCheck", this.DailyVersionCheck);
            configfile.SetValue("VersionCheckDate_Attempt", this.VersionCheckDate_AttemptString);
            configfile.SetValue("VersionCheckDate_Success", this.VersionCheckDate_SuccessString);
            configfile.SetValue("VersionWeb", this.VersionWeb);

            configfile.SetValue("WindowVisible", this.WindowVisible);
            configfile.SetValue("WindowMinimized", this.WindowMinimized);
            configfile.SetValue("WindowPos", this.WindowPos);

            configfile.SetValue("IconPos", this.IconPos);

            configfile.SetValue("AlarmListMaxAlarms", this.AlarmListMaxAlarms);
            configfile.SetValue("AlarmPosition", this.AlarmPosition);
            configfile.SetValue("AlarmDefaultAction", this.AlarmDefaultAction);
            configfile.SetValue("AlarmDefaultMargin", this.AlarmDefaultMargin);
            configfile.SetValue("AlarmDeleteOnClose", this.AlarmDeleteOnClose);
            configfile.SetValue("ShowTooltips", this.ShowTooltips);
            configfile.SetValue("HideOnPause", this.HideOnPause);
            configfile.SetValue("TimeFormat", Enum.GetName(typeof(KerbalTime.PrintTimeFormat),this.TimeFormat));

            configfile.SetValue("AlarmXferRecalc", this.AlarmXferRecalc);
            configfile.SetValue("AlarmXferRecalcThreshold", this.AlarmXferRecalcThreshold);
            configfile.SetValue("AlarmXferDisplayList", this.AlarmXferDisplayList);
            configfile.SetValue("XferUseModelData", this.XferUseModelData);

            configfile.SetValue("AlarmNodeRecalc", this.AlarmNodeRecalc);
            configfile.SetValue("AlarmNodeRecalcThreshold", this.AlarmNodeRecalcThreshold);

            configfile.SetValue("AlarmAddSOIAuto", this.AlarmAddSOIAuto);
            configfile.SetValue("AlarmAddSOIAutoThreshold", this.AlarmAddSOIAutoThreshold);
            //configfile.SetValue("AlarmAddSOIMargin", this.AlarmAddSOIMargin);
            configfile.SetValue("AlarmAutoSOIMargin", this.AlarmAutoSOIMargin);
            configfile.SetValue("AlarmOnSOIChange", this.AlarmCatchSOIChange);
            configfile.SetValue("AlarmOnSOIChange_Action", this.AlarmOnSOIChange_Action);

            configfile.SetValue("AlarmSOIRecalc", this.AlarmSOIRecalc);
            configfile.SetValue("AlarmSOIRecalcThreshold", this.AlarmSOIRecalcThreshold);

            //for (int intAlarm = 0; intAlarm < Alarms.Count; intAlarm++)
            //{
            //    configfile.SetValue("Alarm_" + intAlarm.ToString(), Alarms[intAlarm].SerializeString());
            //}

            configfile.save();
            KACWorker.DebugLogFormatted("Saved Config");

            //Now Save the Alarms
            //SaveAlarms2();
            SaveAlarms2();
            KACWorker.DebugLogFormatted("Saved Alarms");
        }

        private void SaveAlarms2()
        {
            KSP.IO.TextWriter tw = KSP.IO.TextWriter.CreateForType<KerbalAlarmClock>(String.Format("Alarms-{0}.txt", HighLogic.CurrentGame.Title));
            //Write the header
            tw.WriteLine("VesselID|Name|Notes|AlarmTime.UT|AlarmMarginSecs|Type|Enabled|HaltWarp|PauseGame|Options-Manuever/Xfer|<ENDLINE>");
            foreach (KACAlarm tmpAlarm in Alarms.BySaveName(HighLogic.CurrentGame.Title))
            {
                //Now Write Each alarm
                tw.WriteLine(tmpAlarm.SerializeString2() + "|<ENDLINE>");
            }
            //And close the file
            tw.Close();
        }

        //private void SaveAlarms3()
        //{
        //    KSP.IO.TextWriter tw = KSP.IO.TextWriter.CreateForType<KerbalAlarmClock>(String.Format("Alarms-{0}.txt", HighLogic.CurrentGame.Title));
        //    //Write the header
        //    tw.WriteLine("SettingsVersion|3|<ENDLINE>");
        //    tw.WriteLine("VesselID|Name|Notes|Details|AlarmTime.UT|AlarmMarginSecs|Type|Enabled|HaltWarp|PauseGame|Options-Manuever/Xfer|<ENDLINE>");
        //    foreach (KACAlarm tmpAlarm in Alarms.BySaveName(HighLogic.CurrentGame.Title))
        //    {
        //        //Now Write Each alarm
        //        tw.WriteLine(tmpAlarm.SerializeString3() + "|<ENDLINE>");
        //    }
        //    //And close the file
        //    tw.Close();
        //}

        public Boolean getLatestVersion()
        {
            Boolean blnReturn = false;
            try 
            {
                //Get the file from Codeplex
                this.VersionCheckResult = "Unknown - check again later";
                this.VersionCheckDate_Attempt = DateTime.Now;

                KACWorker.DebugLogFormatted("Reading version from Web");
                //Page content FormatException is |LATESTVERSION|1.2.0.0|LATESTVERSION|
                WWW www = new WWW("http://kerbalalarmclock.codeplex.com/wikipage?title=LatestVersion");
                while (!www.isDone) { }

                //Parse it for the version String
                String strFile = www.text;
                KACWorker.DebugLogFormatted("Response Length:" + strFile.Length);

                Match matchVersion;
                matchVersion = Regex.Match(strFile, "(?<=\\|LATESTVERSION\\|).+(?=\\|LATESTVERSION\\|)", System.Text.RegularExpressions.RegexOptions.Singleline);
                KACWorker.DebugLogFormatted("Got Version '" + matchVersion.ToString() + "'");

                String strVersionWeb = matchVersion.ToString();
                if (strVersionWeb != "")
                {
                    this.VersionCheckResult = "Success";
                    this.VersionCheckDate_Success = DateTime.Now;
                    this.VersionWeb = strVersionWeb;
                    blnReturn = true;
                } else
                {
                    this.VersionCheckResult = "Unable to parse web service";
                }
	        }
	        catch (Exception ex)
	        {
                KACWorker.DebugLogFormatted("Failed to read Version info from web");
                KACWorker.DebugLogFormatted(ex.Message);
                
	        }
            KACWorker.DebugLogFormatted("Version Check result:" + VersionCheckResult);
            return blnReturn;
        }

        /// <summary>
        /// Does some logic to see if a check is needed, and returns true if there is a different version
        /// </summary>
        /// <param name="ForceCheck">Ignore all logic and simply do a check</param>
        /// <returns></returns>
        public Boolean VersionCheck(Boolean ForceCheck)
        {
            Boolean blnReturn = false;
            Boolean blnDoCheck =false;

            try
            {
                if (ForceCheck)
                {
                    blnDoCheck = true;
                    KACWorker.DebugLogFormatted("Starting Version Check-Forced");
                } 
                else if (this.VersionWeb=="")
                {
                    blnDoCheck = true;
                    KACWorker.DebugLogFormatted("Starting Version Check-No current web version stored");
                }
                else if (this.VersionCheckDate_Success<DateTime.Now.AddYears(-9))
                {
                    blnDoCheck = true;
                    KACWorker.DebugLogFormatted("Starting Version Check-No current date stored");
                }
                else if (this.VersionCheckDate_Success.Date!=DateTime.Now.Date)
                {
                    blnDoCheck = true;
                    KACWorker.DebugLogFormatted("Starting Version Check-stored date is not today");
                }
                else
                    KACWorker.DebugLogFormatted("Skipping version check");
            

                if (blnDoCheck)
                {
                    getLatestVersion();
                    this.Save();
                    //if (getLatestVersion())
                    //{
                    //    //save all the details to the file
                    //    this.Save();
                    //}
                    //if theres a new version then set the flag
                    VersionAttentionFlag = VersionAvailable;
                }
                blnReturn = true;
            }
            catch (Exception ex)
            {
                KACWorker.DebugLogFormatted("Failed to run the update test");
                KACWorker.DebugLogFormatted(ex.Message);
            }
            return blnReturn;
        }
    }

}

