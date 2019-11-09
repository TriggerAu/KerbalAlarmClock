using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;


namespace KerbalAlarmClock
{
    public class AngleRenderPhase : MonoBehaviourExtended
    {
        public Boolean isDrawing { get; private set; }


        /// <summary>
        /// Is the angle drawn and visible on screen
        /// </summary>
        public Boolean isVisible { get { return isAngleVisible && (!ShowTargetAngle || isTargetVisible); } }


        public Boolean isAngleVisible { get; private set; }
        public Boolean isTargetVisible { get; private set; }

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
        private Boolean _isBecomingVisible_TargetArcDone = false;

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
        /// The Body we are measuring against
        /// </summary>
        public CelestialBody bodyTarget { get; set; }

        /// <summary>
        /// Are we drawing a Target as well
        /// </summary>
        public Boolean ShowTargetAngle { get; set; }

        /// <summary>
        /// The target Angle to Draw - if we have a target
        /// </summary>
        public Double AngleTargetValue { get; set; }


        internal Vector3d vectPosWorldPivot;
        internal Vector3d vectPosWorldOrigin;
        internal Vector3d vectPosWorldEnd;
        private Vector3d vectPosWorldTarget;
        private Vector3d vectPosPivotWorking;
        private Vector3d vectPosEndWorking;
        private Vector3d vectPosTargetWorking;

        private Double _PhaseAngleCurrent;


        private GameObject objLineStart;
        private GameObject objLineEnd;
        private GameObject objLineArc;
        private GameObject objLineTarget;
        private GameObject objLineTargetArc;

        private LineRenderer lineStart;
        private LineRenderer lineEnd;
        internal LineRenderer lineArc;
        private LineRenderer lineTarget;
        private LineRenderer lineTargetArc;


        internal PlanetariumCamera cam;

        internal Int32 ArcPoints = 72;
        internal Int32 StartWidth = 10;
        internal Int32 EndWidth = 10;

        private GUIStyle styleLabelEnd;
        private GUIStyle styleLabelTarget;

        internal override void OnAwake()
        {
            base.OnAwake();

            objLineStart = new GameObject("LineStart");
            objLineEnd = new GameObject("LineEnd");
            objLineArc = new GameObject("LineArc");
            objLineTarget = new GameObject("LineTarget");
            objLineTargetArc = new GameObject("LineTargetArc");

            lineStart = null;
            lineEnd = null;
            lineArc = null;
            lineTarget = null;
            lineTargetArc = null;
        }


        internal override void Start()
        {
            base.Start();

            if (!KerbalAlarmClock.lstScenesForAngles.Contains(HighLogic.LoadedScene)) { 
                this.enabled = false;
                return;
            }

            LogFormatted("Initializing Phase Angle Render");

            //Get the orbit lines material so things look similar
            Material orbitLines = ((MapView)GameObject.FindObjectOfType(typeof(MapView))).orbitLinesMaterial;

            //Material dottedLines = ((MapView)GameObject.FindObjectOfType(typeof(MapView))).dottedLineMaterial;    //Commented because usage removed

            //init all the lines
            lineStart = InitLine(objLineStart, Color.blue, 2, 10, orbitLines);
            lineEnd = InitLine(objLineEnd, Color.blue, 2, 10, orbitLines);
            lineArc = InitLine(objLineArc, Color.blue, ArcPoints, 10, orbitLines);
            lineTarget = InitLine(objLineTarget, Color.green, 2, 10, orbitLines);
            lineTargetArc = InitLine(objLineTargetArc, Color.green, ArcPoints, 10, orbitLines);

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
        private LineRenderer InitLine(GameObject objToAttach,Color lineColor,Int32 VertexCount, Int32 InitialWidth,Material linesMaterial)
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
            _isBecomingVisible_TargetArcDone = false;
            _isHiding = false;
            isDrawing = false;
            
            //Bin the objects
            lineStart = null;
            lineEnd = null;
            lineArc = null;
            lineTarget = null;
            lineTargetArc = null;

            objLineStart.DestroyGameObject();
            objLineEnd.DestroyGameObject();
            objLineArc.DestroyGameObject();
            objLineTarget.DestroyGameObject();
            objLineTargetArc.DestroyGameObject();
        }


        public void DrawAngle(CelestialBody bodyOrigin, CelestialBody bodyTarget,Double angleTarget)
        {
            DrawAngle(bodyOrigin, bodyTarget);
            AngleTargetValue = angleTarget;
            ShowTargetAngle = true;
        }
        public void DrawAngle(CelestialBody bodyOrigin, CelestialBody bodyTarget)
        {
            ShowTargetAngle = false;
            this.bodyOrigin = bodyOrigin;
            this.bodyTarget = bodyTarget;

            isDrawing = true;
            StartDrawing = DateTime.Now;
            _isBecomingVisible = true;
            _isBecomingVisible_LinesDone = false;
            _isBecomingVisible_ArcDone = false;
            _isBecomingVisible_TargetArcDone = false;
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
            if (!KerbalAlarmClock.lstScenesForAngles.Contains(HighLogic.LoadedScene)) {
                return;
            }
            
            if (MapView.MapIsEnabled && isDrawing)
            {
                //Get the Vector of the Origin body from the ref point and its distance
                Vector3d vectStart = bodyOrigin.transform.position - bodyOrigin.referenceBody.transform.position;
                Double vectStartMag = vectStart.magnitude;
                
                //now work out the angle
                //Double _PhaseAngleCurrent = ClampDegrees180(LambertSolver.CurrentPhaseAngle(bodyOrigin.orbit,bodyTarget.orbit));
                _PhaseAngleCurrent = KACUtils.CurrentPhaseAngle(bodyOrigin.orbit, bodyTarget.orbit);
                if (bodyTarget.orbit.semiMajorAxis < bodyOrigin.orbit.semiMajorAxis)
                {
                    _PhaseAngleCurrent = _PhaseAngleCurrent - 360;
                }


                //And therefore the 2nd arm of the angle
                Vector3d vectEnd = Quaternion.AngleAxis(-(Single)_PhaseAngleCurrent, bodyOrigin.orbit.GetOrbitNormal().xzy) * vectStart;

                //make it 120% of the target bodies orbit
                vectEnd = vectEnd.normalized * bodyTarget.orbit.ApR * 1.2;
                Double vectEndMag = vectEnd.magnitude;

                Vector3d vectPointEnd = bodyOrigin.referenceBody.transform.position + vectEnd;

                //and heres the three points
                vectPosWorldPivot = bodyOrigin.referenceBody.transform.position;
                vectPosWorldOrigin = bodyOrigin.transform.position;
                vectPosWorldEnd = vectPointEnd;

                if (ShowTargetAngle) {
                    Vector3d vectTarget = Quaternion.AngleAxis(-(Single)AngleTargetValue, bodyOrigin.orbit.GetOrbitNormal().xzy) * vectStart;
                    vectPosWorldTarget = vectTarget.normalized * vectEndMag * 0.9;
                    vectPosWorldTarget += bodyOrigin.referenceBody.transform.position;
                }

                //Are we Showing, Hiding or Static State
                if (_isHiding) {
                    Single pctDone = (Single)(DateTime.Now - StartDrawing).TotalSeconds / 0.25f;
                    if (pctDone >= 1)
                    {
                        _isHiding = false;
                        isDrawing = false;
                    }
                    vectPosPivotWorking = bodyOrigin.transform.position - Mathf.Lerp(0, (Single)vectStartMag, Mathf.Clamp01(pctDone)) * vectStart.normalized;

                    DrawLine(lineStart, vectPosWorldPivot, vectPosWorldPivot + (vectPosWorldOrigin - vectPosWorldPivot).normalized * Mathf.Lerp((Single)vectStartMag,0,pctDone));
                    DrawLine(lineEnd, vectPosWorldPivot, vectPosWorldPivot + (vectPosWorldEnd - vectPosWorldPivot).normalized * Mathf.Lerp((Single)vectEndMag, 0, pctDone));

                    DrawArc(lineArc, vectStart, _PhaseAngleCurrent, Mathf.Lerp((Single)bodyOrigin.orbit.radius, 0, pctDone), Mathf.Lerp((Single)bodyTarget.orbit.radius, 0, pctDone));

                    if (ShowTargetAngle)
                    {
                        DrawLine(lineTarget, vectPosWorldPivot, vectPosWorldPivot + (vectPosWorldTarget - vectPosWorldPivot).normalized * Mathf.Lerp((Single)vectEndMag, 0, pctDone));
                        DrawArc(lineTargetArc, vectStart, AngleTargetValue, Mathf.Lerp((Single)bodyOrigin.orbit.radius * 0.9f, 0, pctDone), Mathf.Lerp((Single)bodyTarget.orbit.radius * 0.9f, 0, pctDone));
                    }

                } else if (isBecomingVisible) {
                    if (!_isBecomingVisible_LinesDone)
                    {
                        Single pctDone = (Single)(DateTime.Now - StartDrawing).TotalSeconds / 0.5f;
                        if (pctDone >= 1)
                        {
                            _isBecomingVisible_LinesDone = true;
                            StartDrawing = DateTime.Now;
                        }

                        vectPosPivotWorking = bodyOrigin.transform.position - Mathf.Lerp(0, (Single)vectStartMag, Mathf.Clamp01(pctDone)) * vectStart.normalized;

                        DrawLine(lineStart, vectPosPivotWorking, vectPosWorldOrigin);
                    }
                    else if (!_isBecomingVisible_ArcDone)
                    {
                        Single pctDone = (Single)(DateTime.Now - StartDrawing).TotalSeconds / 0.5f;
                        if (pctDone >= 1) { 
                            _isBecomingVisible_ArcDone = true;
                            StartDrawing = DateTime.Now;
                        }

                        Double vectEndMagWorking = Mathf.Lerp((Single)vectStartMag, (Single)vectEndMag, Mathf.Clamp01(pctDone));
                        Double PhaseAngleWorking = Mathf.Lerp(0, (Single)_PhaseAngleCurrent, Mathf.Clamp01(pctDone));
                        vectPosEndWorking = (Vector3d)(Quaternion.AngleAxis(-(Single)PhaseAngleWorking, bodyOrigin.orbit.GetOrbitNormal().xzy) * vectStart).normalized * vectEndMagWorking;
                        vectPosEndWorking += bodyOrigin.referenceBody.transform.position;

                        //draw the origin and end lines
                        DrawLine(lineStart, vectPosWorldPivot, vectPosWorldOrigin);
                        DrawLine(lineEnd, vectPosWorldPivot, vectPosEndWorking);
                        DrawArc(lineArc, vectStart, PhaseAngleWorking, bodyOrigin.orbit.radius, bodyTarget.orbit.radius);
                    }
                    else if(!_isBecomingVisible_TargetArcDone)
                    {
                        if (!ShowTargetAngle)
                        {
                            _isBecomingVisible_TargetArcDone = true;
                            _isBecomingVisible = false;
                            
                        } else
                        {
                            Single pctDone = (Single)(DateTime.Now - StartDrawing).TotalSeconds / 0.5f;
                            if (pctDone >= 1)
                            {
                                _isBecomingVisible_TargetArcDone = true;
                                _isBecomingVisible = false;
                            }

                            Double PhaseAngleWorking = Mathf.Lerp(0, (Single)AngleTargetValue, Mathf.Clamp01(pctDone));
                            vectPosTargetWorking = (Vector3d)(Quaternion.AngleAxis(-(Single)PhaseAngleWorking, bodyOrigin.orbit.GetOrbitNormal().xzy) * vectStart).normalized * vectEndMag * 0.9;
                            vectPosTargetWorking += bodyOrigin.referenceBody.transform.position;

                            DrawLine(lineStart, vectPosWorldPivot, vectPosWorldOrigin);
                            DrawLine(lineEnd, vectPosWorldPivot, vectPosWorldEnd);
                            DrawArc(lineArc, vectStart, _PhaseAngleCurrent, bodyOrigin.orbit.radius, bodyTarget.orbit.radius);
                            DrawLine(lineTarget, vectPosWorldPivot, vectPosTargetWorking );
                            DrawArc(lineTargetArc, vectStart, PhaseAngleWorking, bodyOrigin.orbit.radius * 0.9, bodyTarget.orbit.radius * 0.9);
                        }

                    }



                } else
                {
                    DrawLine(lineStart, vectPosWorldPivot, vectPosWorldOrigin);
                    DrawLine(lineEnd, vectPosWorldPivot, vectPosWorldEnd);

                    DrawArc(lineArc, vectStart, _PhaseAngleCurrent, bodyOrigin.orbit.radius, bodyTarget.orbit.radius);//  vectStartMag, vectEndMag);

                    if (ShowTargetAngle)
                    {
                        DrawLine(lineTarget, vectPosWorldPivot, vectPosWorldTarget);
                        DrawArc(lineTargetArc, vectStart , AngleTargetValue , bodyOrigin.orbit.radius * 0.9, bodyTarget.orbit.radius * 0.9);
                    }
                }
            }
            else
            {
                lineStart.enabled = false;
                lineEnd.enabled = false;
                lineArc.enabled = false;
                lineTarget.enabled = false;
                lineTargetArc.enabled = false;
            }

        }

        internal override void OnGUIEvery()
        {
            if (MapView.MapIsEnabled && isDrawing && !_isBecomingVisible && !_isHiding)
            {
                GUI.Label(new Rect(PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(vectPosWorldEnd)).x - 50, Screen.height - PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(vectPosWorldEnd)).y - 15, 100, 30), String.Format("{0:0.00}°", _PhaseAngleCurrent),styleLabelEnd);

                if(ShowTargetAngle)
                    GUI.Label(new Rect(PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(vectPosWorldTarget)).x - 50, Screen.height - PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(vectPosWorldTarget)).y - 15, 100, 30), String.Format("{0:0.00}°", AngleTargetValue),styleLabelTarget);
            }
        }

        private void DrawArc(LineRenderer line, Vector3d vectStart, Double Angle, Double StartLength, Double EndLength)
        {
            Double ArcRadius = Math.Min(StartLength, EndLength) * 0.9;
            for (int i = 0; i < ArcPoints; i++)
            {
                Vector3d vectArc = Quaternion.AngleAxis(-(Single)Angle / (ArcPoints - 1) * i, bodyOrigin.orbit.GetOrbitNormal().xzy) * vectStart;
                vectArc = vectArc.normalized * ArcRadius;
                vectArc = bodyOrigin.referenceBody.transform.position + vectArc;

                line.SetPosition(i, ScaledSpace.LocalToScaledSpace(vectArc));
            }
            //line.SetWidth((float)10 / 1000 * cam.Distance, (float)10 / 1000 * cam.Distance);
            line.startWidth = 10f / 1000f * cam.Distance;
            line.endWidth = 10f / 1000f * cam.Distance;
            line.enabled = true;
        }

        private void DrawLine(LineRenderer line, Vector3d pointStart, Vector3d pointEnd )
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

    }
}
