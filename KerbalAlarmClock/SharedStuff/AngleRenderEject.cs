using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;


namespace KerbalAlarmClock
{
    public class AngleRenderEject : MonoBehaviourExtended
    {
        public Boolean isDrawing { get; private set; }


        /// <summary>
        /// Is the angle drawn and visible on screen
        /// </summary>
        public Boolean isVisible { get { return isAngleVisible; } }


        public Boolean isAngleVisible { get; private set; }

        /// <summary>
        /// Is the angle in the process of becoming visible
        /// </summary>
        public Boolean isBecomingVisible
        {
            get { return _isBecomingVisible; }
        }
        private Boolean _isBecomingVisible = false;
        private Boolean _isBecomingVisible_LinesDone = false;
        private Boolean _isBecomingVisible_ArcDone = false;
        private Boolean _isBecomingVisible_VesselVectDone = false;

        private Boolean _isHiding = false;

        /// <summary>
        /// Is the angle in the process of being hidden
        /// </summary>
        public Boolean IsBecomingInvisible { get; private set; }
        private DateTime StartDrawing;

        /// <summary>
        /// The Body we are measuring from
        /// </summary>
        public CelestialBody bodyOrigin { get; set; }

        /// <summary>
        /// The Vessel we are using to display the eject against
        /// </summary>
        public Orbit VesselOrbit { get; set; }

        /// <summary>
        /// The target Angle to Draw - if we have a target
        /// </summary>
        public Double AngleTargetValue { get; set; }

        /// <summary>
        /// Is the angle drawing to the orbit retrograde
        /// </summary>
        public Boolean DrawToRetrograde { get; set; }

        internal Vector3d vectPosWorldPivot;
        internal Vector3d vectPosWorldOrigin;
        internal Vector3d vectPosWorldOrbitLabel;
        internal Vector3d vectPosWorldEnd;
        private Vector3d vectPosPivotWorking;
        private Vector3d vectPosEndWorking;

        private GameObject objLineStart;
        private GameObject objLineStartArrow1;
        private GameObject objLineStartArrow2;
        private GameObject objLineEnd;
        private GameObject objLineArc;
        private GameObject objLineVesselVect;
        private GameObject objLineVesselVectArrow1;
        private GameObject objLineVesselVectArrow2;

        private LineRenderer lineStart = null;
        private LineRenderer lineStartArrow1 = null;
        private LineRenderer lineStartArrow2 = null;
        private LineRenderer lineVesselVect = null;
        private LineRenderer lineVesselVectArrow1 = null;
        private LineRenderer lineVesselVectArrow2 = null;

        private LineRenderer lineEnd = null;
        internal LineRenderer lineArc = null;


        internal PlanetariumCamera cam;

        internal Int32 ArcPoints = 72;
        internal Int32 StartWidth = 10;
        internal Int32 EndWidth = 10;

        private GUIStyle styleLabelEnd;
        private GUIStyle styleLabelTarget;

        internal override void Start()
        {
            base.Start();

            if (!KerbalAlarmClock.lstScenesForAngles.Contains(HighLogic.LoadedScene))
            {
                this.enabled = false;
                return;
            }

            LogFormatted("Initializing EjectAngle Render");
            objLineStart = new GameObject("LineStart");
            objLineStartArrow1 = new GameObject("LineStartArrow1");
            objLineStartArrow2 = new GameObject("LineStartArrow2");
            objLineEnd = new GameObject("LineEnd");
            objLineArc = new GameObject("LineArc");
            objLineVesselVect = new GameObject("LineVesselVect");
            objLineVesselVectArrow1 = new GameObject("LineVesselVectArrow1");
            objLineVesselVectArrow2 = new GameObject("LineVesselVectArrow2");

            //Get the orbit lines material so things look similar
            Material orbitLines = ((MapView)GameObject.FindObjectOfType(typeof(MapView))).orbitLinesMaterial;

            //Material dottedLines = ((MapView)GameObject.FindObjectOfType(typeof(MapView))).dottedLineMaterial;    //Commented because usage removed

            //init all the lines
            lineStart = InitLine(objLineStart, Color.blue, 2, 10, orbitLines);
            lineStartArrow1 = InitLine(objLineStartArrow1, Color.blue, 2, 10, orbitLines);
            lineStartArrow2 = InitLine(objLineStartArrow2, Color.blue, 2, 10, orbitLines);

            lineEnd = InitLine(objLineEnd, Color.blue, 2, 10, orbitLines);
            lineArc = InitLine(objLineArc, Color.blue, ArcPoints, 10, orbitLines);

            lineVesselVect = InitLine(objLineVesselVect, Color.green, 2, 10, orbitLines);
            lineVesselVectArrow1 = InitLine(objLineVesselVectArrow1, Color.green, 2, 10, orbitLines);
            lineVesselVectArrow2 = InitLine(objLineVesselVectArrow2, Color.green, 2, 10, orbitLines);

            styleLabelEnd = new GUIStyle();
            styleLabelEnd.normal.textColor = Color.white;
            styleLabelEnd.alignment = TextAnchor.MiddleCenter;
            styleLabelTarget = new GUIStyle();
            styleLabelTarget.normal.textColor = Color.white;
            styleLabelTarget.alignment = TextAnchor.MiddleCenter;

            //get the map camera - well need this for distance/width calcs
            cam = (PlanetariumCamera)GameObject.FindObjectOfType(typeof(PlanetariumCamera));
        }

        /// <summary>
        /// Initialise a LineRenderer with some baseic values
        /// </summary>
        /// <param name="objToAttach">GameObject that renderer is attached to - one linerenderer per object</param>
        /// <param name="lineColor">Draw this color</param>
        /// <param name="VertexCount">How many vertices make up the line</param>
        /// <param name="InitialWidth">line width</param>
        /// <param name="linesMaterial">Line material</param>
        /// <returns></returns>
        private LineRenderer InitLine(GameObject objToAttach, Color lineColor, Int32 VertexCount, Int32 InitialWidth, Material linesMaterial)
        {
            objToAttach.layer = 9;
            LineRenderer lineReturn = objToAttach.AddComponent<LineRenderer>();

            lineReturn.material = linesMaterial;
            //lineReturn.SetColors(lineColor, lineColor);
            lineReturn.startColor = lineColor;
            lineReturn.endColor = lineColor;
            lineReturn.transform.parent = null;
            lineReturn.useWorldSpace = true;
            //lineReturn.SetWidth(InitialWidth, InitialWidth);
            lineReturn.startWidth = InitialWidth;
            lineReturn.endWidth = InitialWidth;
            //lineReturn.SetVertexCount(VertexCount);
            lineReturn.positionCount = VertexCount;
            lineReturn.enabled = false;

            return lineReturn;
        }



        internal override void OnDestroy()
        {
            base.OnDestroy();

            _isBecomingVisible = false;
            _isBecomingVisible_LinesDone = false;
            _isBecomingVisible_ArcDone = false;
            _isBecomingVisible_VesselVectDone = false;
            _isHiding = false;
            isDrawing = false;

            //Bin the objects
            lineStart = null;
            lineStartArrow1 = null;
            lineStartArrow2 = null;
            lineEnd = null;
            lineArc = null;
            lineVesselVect = null;
            lineVesselVectArrow1 = null;
            lineVesselVectArrow2 = null;

            objLineStart.DestroyGameObject();
            objLineEnd.DestroyGameObject();
            objLineArc.DestroyGameObject();
        }


        public void DrawAngle(CelestialBody bodyOrigin, Double angleTarget, Boolean ToRetrograde)
        {
            this.VesselOrbit = null;
            this.bodyOrigin = bodyOrigin;
            this.AngleTargetValue = angleTarget;
            this.DrawToRetrograde = ToRetrograde;

            isDrawing = true;
            StartDrawing = DateTime.Now;
            _isBecomingVisible = true;
            _isBecomingVisible_LinesDone = false;
            _isBecomingVisible_ArcDone = false;
            _isBecomingVisible_VesselVectDone = false;
            _isHiding = false;
        }

        public void HideAngle()
        {
            StartDrawing = DateTime.Now;
            _isHiding = true;
            //isDrawing = false;
        }

        //keeps angles in the range 0 to 360
        internal static double ClampDegrees360(double angle)
        {
            angle = angle % 360.0;
            if (angle < 0) return angle + 360.0;
            else return angle;
        }

        //keeps angles in the range -180 to 180
        internal static double ClampDegrees180(double angle)
        {
            angle = ClampDegrees360(angle);
            if (angle > 180) angle -= 360;
            return angle;
        }


        internal override void OnPreCull()
        {
            base.OnPreCull();

            //not sure if this is right - but its working
            if (!KerbalAlarmClock.lstScenesForAngles.Contains(HighLogic.LoadedScene))
            {
                return;
            }

            if (MapView.MapIsEnabled && isDrawing)
            {
                //Get the Planets Velocity Vector
                Vector3d vectOrbitPrograde = bodyOrigin.orbit.getOrbitalVelocityAtUT(Planetarium.GetUniversalTime()).xzy.normalized * bodyOrigin.Radius * 5;
                Vector3d vectStart;

                if (DrawToRetrograde)
                    vectStart = -vectOrbitPrograde;
                else
                    vectStart = vectOrbitPrograde;

                //Get the Vector of the Origin body from the ref point and its distance
                //Vector3d vectStart = bodyOrigin.transform.position - bodyOrigin.referenceBody.transform.position;
                Double vectStartMag = vectStart.magnitude;

                //Work out the Eject Angle Vector
                Vector3d vectEnd = Quaternion.AngleAxis((Single)AngleTargetValue, bodyOrigin.orbit.GetOrbitNormal().xzy) * vectStart;
                Double vectEndMag = vectEnd.magnitude;

                //and heres the three points
                vectPosWorldPivot = bodyOrigin.transform.position;
                vectPosWorldOrigin = bodyOrigin.transform.position + vectStart;
                vectPosWorldEnd = bodyOrigin.transform.position + vectEnd;

                Vector3d vectPosWorldOrbitArrow = bodyOrigin.transform.position + vectOrbitPrograde;
                vectPosWorldOrbitLabel = bodyOrigin.transform.position + (vectOrbitPrograde * 3 / 4);

                if (this.VesselOrbit != null)
                {
                    //now work out the angle
                    //Double _PhaseAngleCurrent = ClampDegrees180(LambertSolver.CurrentPhaseAngle(bodyOrigin.orbit,bodyTarget.orbit));
                    //_PhaseAngleCurrent = LambertSolver.CurrentPhaseAngle(bodyOrigin.orbit, bodyTarget.orbit);
                    //if (bodyTarget.orbit.semiMajorAxis < bodyOrigin.orbit.semiMajorAxis)
                    //{
                    //    _PhaseAngleCurrent = _PhaseAngleCurrent - 360;
                    //}
                }

                //Are we Showing, Hiding or Static State
                if (_isHiding)
                {
                    Single pctDone = (Single)(DateTime.Now - StartDrawing).TotalSeconds / 0.25f;
                    if (pctDone >= 1)
                    {
                        _isHiding = false;
                        isDrawing = false;
                    }
                    vectPosPivotWorking = bodyOrigin.transform.position - Mathf.Lerp(0, (Single)vectStartMag, Mathf.Clamp01(pctDone)) * vectStart.normalized;

                    DrawLine(lineStart, vectPosWorldPivot + (DrawToRetrograde ? vectOrbitPrograde.normalized * Mathf.Lerp((Single)vectStartMag, 0, pctDone) : new Vector3d()), vectPosWorldPivot + (vectPosWorldOrigin - vectPosWorldPivot).normalized * Mathf.Lerp((Single)vectStartMag, 0, pctDone));
                    DrawLine(lineEnd, vectPosWorldPivot, vectPosWorldPivot + (vectPosWorldEnd - vectPosWorldPivot).normalized * Mathf.Lerp((Single)vectEndMag, 0, pctDone));

                    DrawArc(lineArc, vectStart, AngleTargetValue, Mathf.Lerp((Single)bodyOrigin.Radius * 3, 0, pctDone), Mathf.Lerp((Single)bodyOrigin.Radius * 3, 0, pctDone));

                    Vector3d vectVesselStart = bodyOrigin.transform.position + (vectEnd * 3 / 4);
                    Vector3d vectVesselEnd = (Vector3d)(Quaternion.AngleAxis(-(Single)90, bodyOrigin.orbit.GetOrbitNormal().xzy) * vectEnd).normalized * (Mathf.Lerp((Single)bodyOrigin.Radius * 3f, 0, Mathf.Clamp01(pctDone)));
                    vectVesselEnd += vectVesselStart;
                    DrawLine(lineVesselVect, vectVesselStart, vectVesselEnd);

                    lineStartArrow1.enabled = false;
                    lineStartArrow2.enabled = false;
                    lineVesselVectArrow1.enabled = false;
                    lineVesselVectArrow2.enabled = false;
                }
                else if (isBecomingVisible)
                {
                    if (!_isBecomingVisible_LinesDone)
                    {
                        Single pctDone = (Single)(DateTime.Now - StartDrawing).TotalSeconds / 0.5f;
                        if (pctDone >= 1)
                        {
                            _isBecomingVisible_LinesDone = true;
                            StartDrawing = DateTime.Now;
                        }

                        vectPosPivotWorking = vectPosWorldPivot + (DrawToRetrograde ? vectOrbitPrograde.normalized * Mathf.Lerp(0, (Single)vectStartMag, pctDone) : new Vector3d()) + Mathf.Lerp((Single)vectStartMag, 0, Mathf.Clamp01(pctDone)) * vectStart.normalized;

                        DrawLine(lineStart, vectPosPivotWorking, vectPosWorldOrigin);
                    }
                    else if (!_isBecomingVisible_ArcDone)
                    {
                        Single pctDone = (Single)(DateTime.Now - StartDrawing).TotalSeconds / 0.5f;
                        if (pctDone >= 1)
                        {
                            _isBecomingVisible_ArcDone = true;
                            StartDrawing = DateTime.Now;
                        }

                        Double vectEndMagWorking = Mathf.Lerp((Single)vectStartMag, (Single)vectEndMag, Mathf.Clamp01(pctDone));
                        Double AngleTargetWorking = ClampDegrees180(Mathf.Lerp(0, (Single)AngleTargetValue, Mathf.Clamp01(pctDone)));
                        vectPosEndWorking = (Vector3d)(Quaternion.AngleAxis((Single)AngleTargetWorking, bodyOrigin.orbit.GetOrbitNormal().xzy) * vectStart).normalized * vectEndMagWorking;
                        vectPosEndWorking += bodyOrigin.transform.position;

                        //draw the origin and end lines
                        DrawLine(lineStart, vectPosWorldPivot + (DrawToRetrograde ? vectOrbitPrograde : new Vector3d()), vectPosWorldOrigin);
                        DrawLine(lineEnd, vectPosWorldPivot, vectPosEndWorking);
                        DrawArc(lineArc, vectStart, AngleTargetWorking, bodyOrigin.Radius * 3, bodyOrigin.Radius * 3);
                    }
                    else if (!_isBecomingVisible_VesselVectDone)
                    {
                        Single pctDone = (Single)(DateTime.Now - StartDrawing).TotalSeconds / 0.5f;
                        if (pctDone >= 1)
                        {
                            _isBecomingVisible_VesselVectDone = true;
                            _isBecomingVisible = false;
                        }

                        //draw the origin and end lines
                        DrawLine(lineStart, vectPosWorldPivot + (DrawToRetrograde ? vectOrbitPrograde : new Vector3d()), vectPosWorldOrigin);
                        //Arrow heads
                        DrawLineArrow(lineStartArrow1, lineStartArrow2, vectPosWorldPivot, vectPosWorldOrbitArrow, bodyOrigin.orbit.GetOrbitNormal().xzy, (bodyOrigin.Radius * 2 / 3));

                        DrawLine(lineEnd, vectPosWorldPivot, vectPosWorldEnd);
                        DrawArc(lineArc, vectStart, AngleTargetValue, bodyOrigin.Radius * 3, bodyOrigin.Radius * 3);//  vectStartMag, vectEndMag);

                        Vector3d vectVesselStart = bodyOrigin.transform.position + (vectEnd * 3 / 4);
                        Vector3d vectVesselEnd = (Vector3d)(Quaternion.AngleAxis(-(Single)90, bodyOrigin.orbit.GetOrbitNormal().xzy) * vectEnd).normalized * (Mathf.Lerp(0, (Single)bodyOrigin.Radius * 3f, Mathf.Clamp01(pctDone)));
                        vectVesselEnd += vectVesselStart;
                        DrawLine(lineVesselVect, vectVesselStart, vectVesselEnd);

                    }
                }
                else
                {
                    DrawLine(lineStart, vectPosWorldPivot + (DrawToRetrograde ? vectOrbitPrograde : new Vector3d()), vectPosWorldOrigin);
                    //Arrow heads
                    DrawLineArrow(lineStartArrow1, lineStartArrow2, vectPosWorldPivot, vectPosWorldOrbitArrow, bodyOrigin.orbit.GetOrbitNormal().xzy, (bodyOrigin.Radius * 2 / 3));

                    DrawLine(lineEnd, vectPosWorldPivot, vectPosWorldEnd);
                    DrawArc(lineArc, vectStart, AngleTargetValue, bodyOrigin.Radius * 3, bodyOrigin.Radius * 3);//  vectStartMag, vectEndMag);


                    Vector3d vectVesselStart = bodyOrigin.transform.position + (vectEnd * 3 / 4);
                    Vector3d vectVesselEnd = (Vector3d)(Quaternion.AngleAxis(-(Single)90, bodyOrigin.orbit.GetOrbitNormal().xzy) * vectEnd).normalized * bodyOrigin.Radius * 3;
                    vectVesselEnd += vectVesselStart;
                    DrawLine(lineVesselVect, vectVesselStart, vectVesselEnd);
                    DrawLineArrow(lineVesselVectArrow1, lineVesselVectArrow2, vectVesselStart, vectVesselEnd, bodyOrigin.orbit.GetOrbitNormal().xzy, (bodyOrigin.Radius * 2 / 3));
                }
            }
            else
            {
                lineStart.enabled = false;
                lineStartArrow1.enabled = false;
                lineStartArrow2.enabled = false;
                lineEnd.enabled = false;
                lineArc.enabled = false;
                lineVesselVect.enabled = false;
                lineVesselVectArrow1.enabled = false;
                lineVesselVectArrow2.enabled = false;
            }

        }

        internal override void OnGUIEvery()
        {
            if (MapView.MapIsEnabled && isDrawing && !_isBecomingVisible && !_isHiding)
            {
                GUI.Label(new Rect(PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(vectPosWorldEnd)).x - 50, Screen.height - PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(vectPosWorldEnd)).y - 15, 100, 30), String.Format("{0:0.00}°\r\n{1}", AngleTargetValue, DrawToRetrograde ? "to retrograde" : "to prograde"), styleLabelEnd);

                GUI.Label(new Rect(PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(vectPosWorldOrbitLabel)).x - 50, Screen.height - PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(vectPosWorldOrbitLabel)).y - 15, 100, 30), "Orbit", styleLabelTarget);

                if (VesselOrbit != null)
                {
                    //GUI.Label(new Rect(PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(vectPosWorldTarget)).x - 50, Screen.height - PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(vectPosWorldTarget)).y - 15, 100, 30), String.Format("{0:0.00}°", AngleTargetValue),styleLabelTarget);
                }
            }
        }

        private void DrawArc(LineRenderer line, Vector3d vectStart, Double Angle, Double StartLength, Double EndLength)
        {
            Double ArcRadius = Math.Min(StartLength, EndLength);
            for (int i = 0; i < ArcPoints; i++)
            {
                Vector3d vectArc = Quaternion.AngleAxis((Single)Angle / (ArcPoints - 1) * i, bodyOrigin.orbit.GetOrbitNormal().xzy) * vectStart;
                vectArc = vectArc.normalized * ArcRadius;
                vectArc = bodyOrigin.transform.position + vectArc;

                line.SetPosition(i, ScaledSpace.LocalToScaledSpace(vectArc));
            }
            //line.SetWidth((float)10 / 1000 * cam.Distance, (float)10 / 1000 * cam.Distance);
            line.startWidth = 10f / 1000f * cam.Distance;
            line.endWidth = 10f / 1000f * cam.Distance;
            line.enabled = true;
        }

        private void DrawLine(LineRenderer line, Vector3d pointStart, Vector3d pointEnd)
        {
            line.SetPosition(0, ScaledSpace.LocalToScaledSpace(pointStart));
            line.SetPosition(1, ScaledSpace.LocalToScaledSpace(pointEnd));
            //line.SetWidth((Single)StartWidth / 1000 * (Single)(cam.transform.position - pointStart).magnitude, (Single)EndWidth / 1000 * (Single)(cam.transform.position - pointEnd).magnitude);
            //line.SetWidth((float)10 / 1000 * cam.Distance, (float)10 / 1000 * cam.Distance);
            line.startWidth = 10f / 1000f * cam.Distance;
            line.endWidth = 10f / 1000f * cam.Distance;

            //Double distToStart = ScaledSpace.LocalToScaledSpace(pointStart).magnitude;
            //Double distToEnd = ScaledSpace.LocalToScaledSpace(pointEnd).magnitude;
            //line.SetWidth((float)StartWidth / 10000f * (Single)distToStart, (float)EndWidth / 10000f * (Single)distToEnd);

            line.enabled = true;
        }

        //internal Int32 ArrowMult = 1000
        private void DrawLineArrow(LineRenderer line1, LineRenderer line2, Vector3d pointStart, Vector3d pointEnd, Vector3d vectPlaneNormal, Double ArrowArmLength)
        {
            Vector3d vectArrow = (pointEnd - pointStart).normalized * ArrowArmLength;  //400000;
            Vector3d vectArrow1 = Quaternion.AngleAxis((Single)30, vectPlaneNormal) * vectArrow;
            Vector3d vectArrow2 = Quaternion.AngleAxis(-(Single)30, vectPlaneNormal) * vectArrow;
            line1.SetPosition(0, ScaledSpace.LocalToScaledSpace(pointEnd - vectArrow1));
            line1.SetPosition(1, ScaledSpace.LocalToScaledSpace(pointEnd));
            line2.SetPosition(0, ScaledSpace.LocalToScaledSpace(pointEnd - vectArrow2));
            line2.SetPosition(1, ScaledSpace.LocalToScaledSpace(pointEnd));
            //line.SetWidth((Single)StartWidth / 1000 * (Single)(cam.transform.position - pointStart).magnitude, (Single)EndWidth / 1000 * (Single)(cam.transform.position - pointEnd).magnitude);
            //line1.SetWidth((float)10 / 1000 * cam.Distance, (float)10 / 1000 * cam.Distance);
            line1.startWidth = 10f / 1000f * cam.Distance;
            line1.endWidth = 10f / 1000f * cam.Distance;
            //line2.SetWidth((float)10 / 1000 * cam.Distance, (float)10 / 1000 * cam.Distance);
            line2.startWidth = 10f / 1000f * cam.Distance;
            line2.endWidth = 10f / 1000f * cam.Distance;

            //Double distToStart = ScaledSpace.LocalToScaledSpace(pointStart).magnitude;
            //Double distToEnd = ScaledSpace.LocalToScaledSpace(pointEnd).magnitude;
            //line.SetWidth((float)StartWidth / 10000f * (Single)distToStart, (float)EndWidth / 10000f * (Single)distToEnd);

            line1.enabled = true;
            line2.enabled = true;
        }

        internal override void FixedUpdate()
        {
            base.FixedUpdate();


        }
    }
}
