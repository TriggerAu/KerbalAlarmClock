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

        public static String SepVariables(String separator, params object[] vars)
        {
            String strReturn = "";
            foreach (object tmpVar in vars)
            {
                if (strReturn != "") strReturn += separator;
                if (tmpVar == null)
                    strReturn += "";
                else
                {
                    strReturn = EncodeVarStrings(strReturn);
                    strReturn += tmpVar.ToString();
                }
            }
            return strReturn;
        }

        public static String EncodeVarStrings(String Input)
        {
            String strReturn = Input;
            //encode \r\t\n
            strReturn = strReturn.Replace("\r", "\\r");
            strReturn = strReturn.Replace("\n", "\\n");
            strReturn = strReturn.Replace("\t", "\\t");
            return strReturn;
        }

        public static String DecodeVarStrings(String Input)
        {
            String strReturn = Input;
            //encode \r\t\n
            strReturn = strReturn.Replace("\\r", "\r");
            strReturn = strReturn.Replace("\\n", "\n");
            strReturn = strReturn.Replace("\\t", "\t");
            return strReturn;
        }

        public static Byte[] LoadFileToArray(String Filename)
        {
            Byte[] arrBytes;

            arrBytes = KSP.IO.File.ReadAllBytes<KerbalAlarmClock>(Filename);

            return arrBytes;
        }

        public static void SaveFileFromArray(Byte[] data, String Filename)
        {
            KSP.IO.File.WriteAllBytes<KerbalAlarmClock>(data, Filename);
        }


        public static void LoadImageIntoTexture(ref Texture2D tex, String FileName)
        {

            try
            {
                //KACWorker.DebugLogFormatted("Loading: TriggerTech/Textures/KerbalAlarmClock/{0}", FileName);
                //tex = GameDatabase.Instance.GetTexture("TriggerTech/Textures/KerbalAlarmClock/" + FileName.Replace(".png", ""), false);
                //if (tex == null) KACWorker.DebugLogFormat GetTextureted("Textures Empty");

                tex.LoadImage(LoadFileToArray(FileName));
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

        public static RectOffset SetRectOffset(RectOffset tmpRectOffset, int Left, int Right, int Top, int Bottom)
        {
            tmpRectOffset.left = Left;
            tmpRectOffset.top = Top;
            tmpRectOffset.right = Right;
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
                //if (FlightGlobals.fetch.VesselTarget != null)
                if (KACWorkerGameState.CurrentVesselTarget is Vessel || KACWorkerGameState.CurrentVesselTarget is CelestialBody)
                {
                    Orbit oTarget = KACWorkerGameState.CurrentVesselTarget.GetOrbit();
                    Vector3d vectVesselPos = vessel.orbit.getRelativePositionAtUT(KACWorkerGameState.CurrentTime.UT);

                    blnReturn = CalcTimeToANorDN(vectVesselPos, vessel.orbit, oTarget, typeOfNode, out timeToNode);
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
                if (typeOfNode == ANDNNodeType.Ascending)
                    AngleToANDN = CalcAngleToAscendingNode(position, origin, target);
                else
                    AngleToANDN = CalcAngleToDescendingNode(position, origin, target);
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

        #region "timeOfClosestApproach Code - "
        public static Vector3d getAbsolutePositionAtUT(Orbit orbit, double UT)
        {
            Vector3d pos = orbit.getRelativePositionAtUT(UT);
            pos += orbit.referenceBody.position;
            return pos;
        }

        public static double timeOfClosestApproach(Orbit oOrig, Orbit oTgt, double timeStart, out double closestdistance)
        {
            return timeOfClosestApproach(oOrig, oTgt, timeStart, oOrig.period, 20, out closestdistance);
        }

        public static double timeOfClosestApproach(Orbit oOrig, Orbit oTgt, double timeStart, int orbit, out double closestdistance)
        {
            //return timeOfClosestApproach(a, b, time + ((orbit - 1) * a.period), (orbit * a.period), 20, out closestdistance);
            return timeOfClosestApproach(oOrig, oTgt, timeStart + ((orbit - 1) * oOrig.period), oOrig.period, 20, out closestdistance);
        }

        public static double timeOfClosestApproach(Orbit oOrig, Orbit oTgt, double timeStart, double periodtoscan, double numDivisions, out double closestdistance)
        {
            int intOrbit=0;
            return timeOfClosestApproach(oOrig, oTgt, timeStart, periodtoscan, numDivisions, out closestdistance,out intOrbit);
        }
        
        public static double timeOfClosestApproach(Orbit oOrig, Orbit oTgt, double timeStart, double periodtoscan, double numDivisions, out double closestdistance,out int ClosestDistanceOnOrbitNum)
        {
            double closestApproachTime = timeStart;
            double closestApproachDistance = Double.MaxValue;
            double minTime = timeStart;
            double maxTime = timeStart + periodtoscan;

            //work out iterations for precision - we only really need to within a second - so how many iterations do we actually need
            //Each iteration gets us 1/10th of the period to scan


            for (int iter = 0; iter < 8; iter++)
            {
                double dt = (maxTime - minTime) / numDivisions;
                for (int i = 0; i < numDivisions; i++)
                {
                    double t = minTime + i * dt;
                    double distance = (getAbsolutePositionAtUT(oOrig, t) - getAbsolutePositionAtUT(oTgt, t)).magnitude;
                    if (distance < closestApproachDistance)
                    {
                        closestApproachDistance = distance;
                        closestApproachTime = t;
                    }
                }
                minTime = KACUtils.Clamp(closestApproachTime - dt, timeStart, timeStart + periodtoscan);
                maxTime = KACUtils.Clamp(closestApproachTime + dt, timeStart, timeStart + periodtoscan);
            }

            closestdistance = closestApproachDistance;

            ClosestDistanceOnOrbitNum = (int)Math.Floor((closestApproachTime - timeStart) / oOrig.period) + 1;
            return closestApproachTime;
        }

        #endregion

        #endregion

    }
}
