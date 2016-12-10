using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using UnityEngine;
using KSP;
using KSPPluginFramework;

using System.Xml.Serialization;

namespace KerbalAlarmClock
{
	public partial class KerbalAlarmClock 
	{
#if DEBUG
		//Debug Window
		private Boolean _ShowDebugPane = true;
		private static int _WindowDebugID = 123456;
		private static Rect _WindowDebugRect = new Rect(Screen.width - 400, 30, 400, 200);

		public void DebugActionTimed(GameScenes loadedscene)
		{
			LogFormatted("Timed Debug Action Initiated");
			//LogFormatted("Stuff Here");
			//LogFormatted(FlightGlobals.ActiveVessel.orbit.closestEncounterBody.bodyName);
			//LogFormatted(FlightGlobals.ActiveVessel.orbit.ClAppr.ToString());
			//LogFormatted(FlightGlobals.ActiveVessel.orbit.closestEncounterBody.sphereOfInfluence.ToString());

			// how to detect Escape - eg to Solar orbit

			//Detect SOI Change and time
			//if ((FlightGlobals.ActiveVessel.orbit.closestEncounterBody != null) && (FlightGlobals.ActiveVessel.orbit.ClAppr > 0))
			//{
			//    //Is the closest approach less than the size of the SOI
			//    LogFormatted(FlightGlobals.ActiveVessel.orbit.referenceBody + "," + FlightGlobals.ActiveVessel.orbit.closestEncounterBody + "," +
			//         FlightGlobals.ActiveVessel.orbit.nextPatch.referenceBody + "," + FlightGlobals.ActiveVessel.orbit.nextPatch.closestEncounterBody);
			//    if (FlightGlobals.ActiveVessel.orbit.ClAppr < FlightGlobals.ActiveVessel.orbit.closestEncounterBody.sphereOfInfluence)
			//    {
			//        LogFormatted("SOI Change in :" + (FlightGlobals.ActiveVessel.orbit.nextPatch.StartUT - Planetarium.GetUniversalTime()));
			//    }
			//    else
			//    {
			//        LogFormatted("Nextpatch in :" + (FlightGlobals.ActiveVessel.orbit.nextPatch.StartUT - Planetarium.GetUniversalTime()));
			//    }
			//}



			//looking for next action on the path use orbit.patchEndTransition - enum of Orbit.PatchTransitionType
			//  FINAL - fixed orbit no change
			//  ESCAPE - leaving SOI
			//  Intercept - entering new SOI inside current SOI
			//  INITIAL - ???
			//  MANEUVER - MAneuver Node
			//
			// orbit.UTsoi - time of next SOI change (base on above transition types - ie if type is final this time can be ignored)
			//orbit.nextpatch gives you the next orbit and you can read the new SOI!!!


			//Maneuver Node
			//To recreate should only need DeltaV, NodeRotation and UT of Node


			//write orbit, next orbit, patchedconicsolver nodes?
			//see what we need to store for a Maneuver node

			//FlightGlobals.Vessels[0].patchedConicSolver.maneuverNodes[0]

			//if (tmpVessel.orbit.nextPatch == null)
			//{
			//    LogFormatted(tmpVessel.name + "-No next Patch");
			//}
			//else
			//{
			//    LogFormatted(tmpVessel.name + "-Next patch @ " + (tmpVessel.orbit.nextPatch.StartUT-CurrentTime.UT));
			//    LogFormatted("Same orbit: " + (tmpVessel.orbit == tmpVessel.orbit.nextPatch));
			//}
		}

		Boolean blnTriggerFlag = false;
		public void DebugActionTriggered(GameScenes loadedscene)
		{
			LogFormatted("Manual Debug Action Initiated");

			blnTriggerFlag = true;

			//Kerbal[] KerbalObjects = FlightGlobals.FindObjectsOfType(typeof(Kerbal)) as Kerbal[];
			//LogFormatted("Kerbal Count: {0}", KerbalObjects.Length);
			//foreach (Kerbal k in KerbalObjects)
			//{
			//    LogFormatted("{0}", k.crewMemberName);
			//}

			//KerbalEVA[] KerbalEVAObjects = FlightGlobals.FindObjectsOfType(typeof(KerbalEVA)) as KerbalEVA[];
			//LogFormatted("Kerbal EVA Count: {0}", KerbalEVAObjects.Length);



			//LogFormatted("Vessels Count: {0}", FlightGlobals.Vessels.Count);
			//foreach (Vessel v in FlightGlobals.Vessels)
			//{
			//    List<ProtoCrewMember> pCM = v.GetVesselCrew();
			//    foreach (ProtoCrewMember CM in pCM)
			//    {
			//        LogFormatted("{2}({1})-{0}", CM.name,v.vesselType,v.vesselName);

			//    }
			//}

			//String strLine;
			//foreach (CelestialBody cbTemp in FlightGlobals.Bodies)
			//{
			//    strLine=String.Format("{0}({1}),", cbTemp.bodyName, Enum.GetName(typeof(CelestialBodyType), cbTemp.bodyType));
			//    try
			//    {
			//        strLine += String.Format("parent-{0},", cbTemp.referenceBody.bodyName);
			//    }
			//    catch (Exception) { }
			//    try
			//    {
			//        strLine += String.Format("radius-{0},", cbTemp.orbit.radius.ToString());
			//    }
			//    catch (Exception) { }
			//    try
			//    {
			//        strLine += String.Format("sma-{0},", cbTemp.orbit.semiMajorAxis.ToString());
			//    }
			//    catch (Exception) { }
			//    try
			//    {
			//        strLine += String.Format("period-{0},", cbTemp.orbit.period.ToString());
			//    }
			//    catch (Exception) { }

			//    try
			//    {
			//        strLine += String.Format("tA-{0},", cbTemp.orbit.trueAnomaly);
			//    }
			//    catch (Exception) { }
			//    try
			//    {
			//        strLine += String.Format("aOP-{0},", cbTemp.orbit.argumentOfPeriapsis);
			//    }
			//    catch (Exception) { }
			//    try
			//    {
			//        strLine += String.Format("LAN-{0},", cbTemp.orbit.LAN);
			//    }
			//    catch (Exception) { }

			//    LogFormatted(strLine);
			//}


			//LogFormatted("window textcolor r:{0}", KACResources.styleWindow.normal.textColor.r.ToString());
			//byte[] b = KSP.IO.IOUtils.SerializeToBinary(FlightGlobals.ActiveVessel.orbit);
			//bw.Write(b);
			//bw.Close();
			//KSP.IO.BinaryWriter bw = KSP.IO.BinaryWriter.CreateForType<KerbalAlarmClock>("testfile.bin");
			

			//LogFormatted(scrollPosition.ToString());
			//Print each of the vessels UTSOI
			//foreach (Vessel tmpVessel in FlightGlobals.Vessels)
			//{
			//    LogFormatted("{0}-{1}", tmpVessel.vesselName, tmpVessel.orbit.UTsoi.ToString());            
			//}

			
			//LogFormatted(FlightGlobals.ActiveVessel.id.ToString());

			//Orbit o=FlightGlobals.ActiveVessel.orbit;

			//WriteOrbitFile(o,"Debug/Orbit.txt");
			//if (o.nextPatch != null)
			//    WriteOrbitFile(o.nextPatch, "Debug/OrbitNext.txt");

			//WriteOrbitFile(tgtSelectedDistance.GetOrbit(),"Debug/OrbitTarget.txt");
			//WriteManeuverFile(FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes,"Debug/Nodes.txt");
		}



			//Orbit activeOrbit = FlightGlobals.fetch.activeVessel.orbit;
			//Orbit targetOrbit = (FlightGlobals.fetch.VesselTarget as Vessel).orbit;
			//Vector3d activePosition = activeOrbit.getRelativePositionAtUT(Planetarium.GetUniversalTime());
			//double ascendingNode = CalcAngleToAscendingNode(activePosition, activeOrbit, targetOrbit);
			//double timeToAN = CalcTimeToNode(activeOrbit, ascendingNode);

			//LogFormatted("AN:{0}", timeToAN.ToString());



		//private double CalcAngleToAscendingNode(Vector3d position, Orbit origin, Orbit target)
		//{
		//    double angleToNode = 0d;

		//    if (origin.inclination < 90)
		//    {
		//        angleToNode = CalcPhaseAngle(position, GetAscendingNode(origin, target));
		//    }
		//    else
		//    {
		//        angleToNode = 360 - CalcPhaseAngle(position, GetAscendingNode(origin, target));
		//    }

		//    return angleToNode;
		//}

		//private Vector3d GetAscendingNode(Orbit origin, Orbit target)
		//{
		//    return Vector3d.Cross(target.GetOrbitNormal(), origin.GetOrbitNormal());
		//}

		//private double CalcPhaseAngle(Vector3d origin, Vector3d target)
		//{
		//    double phaseAngle = Vector3d.Angle(target, origin);
		//    if (Vector3d.Angle(Quaternion.AngleAxis(90, Vector3d.forward) * origin, target) > 90)
		//    {
		//        phaseAngle = 360 - phaseAngle;
		//    }
		//    return (phaseAngle + 360) % 360;
		//}



		//private double CalcTimeToNode(Orbit origin, double angleToNode)
		//{
		//    return (origin.period / 360d) * angleToNode;
		//}


		Double UTStart = 0;
		DateTime TransStart;
		Boolean warptrans = false;

		Double[] UTChanges = new Double[8];

		void warpchange(){
			for (int i = 0; i < 8; i++)
			{
				UTChanges[i] = CalcUTTaken(TimeWarp.CurrentRateIndex,i);
			}
		}
		Double CalcUTTaken(Int32 StartIndex, Int32 EndIndex)
		{
			if (StartIndex == EndIndex)
				return 0;

			Double StartRate = TimeWarp.fetch.warpRates[StartIndex];
			Double EndRate = TimeWarp.fetch.warpRates[EndIndex];

			Double TimeAtEach = 1/((EndRate - 1) - StartRate);

			Double UT = 0;
			for (int interval = (Int32)StartRate + 1; interval < (Int32)EndRate; interval++)
			{
				UT += interval * TimeAtEach;
			}
			return UT;
		}

		int intTestheight = 500;
		int intTestheight2 = 0;
		int intTestheight3 = 0;
		static int intTestheight4 = 100;

		double dblTest = 12;

		int intTestDistance = 710000;

		public void FillDebugWindow(int WindowID)
		{
            try { GUILayout.BeginVertical(); }
            catch (Exception) { LogFormatted("FillDebugWindow: GUILayout not ready yet", this._ClassName); return; }
			//GUILayout.BeginHorizontal();
			////GUILayout.Label("Alarm Add Interface:", KACResources.styleAddHeading, GUILayout.Width(90));
			////AddInterfaceType = Convert.ToInt32(GUILayout.TextField(AddInterfaceType.ToString()));
			//GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			GUILayout.Label("test1:");
			GUILayout.Label("test2:");
			GUILayout.Label("test3:");
			GUILayout.Label("test4:");


			GUILayout.Label("dblTest:");

			GUILayout.EndVertical();
			GUILayout.BeginVertical();

			intTestheight = Convert.ToInt32(GUILayout.TextField(intTestheight.ToString()));
			intTestheight2 = Convert.ToInt32(GUILayout.TextField(intTestheight2.ToString()));
			intTestheight3 = Convert.ToInt32(GUILayout.TextField(intTestheight3.ToString()));
			intTestheight4 = Convert.ToInt32(GUILayout.TextField(intTestheight4.ToString()));

			dblTest = Convert.ToDouble(GUILayout.TextField(dblTest.ToString()));



            GUILayout.Label("Screen.height:" + Screen.height);
            GUILayout.Label("AddXFER:" + intAddXferHeight);


            GUILayout.EndVertical();
			GUILayout.EndHorizontal();

            try
            {
                GUILayout.Label(String.Format("ManNodeExists:{0}",KACWorkerGameState.ManeuverNodeExists));
                GUILayout.Label(String.Format("ManNodeGizmoAttached:{0}",KACWorkerGameState.ManeuverNodeFuture.attachedGizmo!=null));

                GUILayout.Label(String.Format("AnyManNodeGizmo:{0}", KACWorkerGameState.ManeuverNodesAll.Any(n=>n.attachedGizmo!=null)));
            }
            catch (Exception)
            {
                
            }

            //try
            //{
            //    GUILayout.Label("Req: " + KAC_KERWrapper.KERWrapper.KER.bRequested);
            //    GUILayout.Label("Run: " + KAC_KERWrapper.KERWrapper.KER.bRunning);
            //}
            //catch (Exception) {
                                
            //}

            //if (GUILayout.Button("KER ReqSim"))
            //{
            //    KAC_KERWrapper.KERWrapper.KER.RequestSimlulation();
            //}
            //if (GUILayout.Button("KER Update"))
            //{
            //    KAC_KERWrapper.KERWrapper.KER.UpdateManNodeValues();
            //}

            //if (GUILayout.Button("KER ME"))
            //{
            //    LogFormatted("B:{0} - H:{1} - HASIT:{2}", KAC_KERWrapper.KERWrapper.KER.BurnTime, KAC_KERWrapper.KERWrapper.KER.HalfBurnTime, KAC_KERWrapper.KERWrapper.KER.HasDeltaV);
            //}
            //if (GUILayout.Button("VOID ME"))
            //{
            //    LogFormatted("B:{0} - H:{1} - HASIT:{2}", KAC_VOIDWrapper.VOIDWrapper.VOID.BurnTime, KAC_VOIDWrapper.VOIDWrapper.VOID.HalfBurnTime, KAC_VOIDWrapper.VOIDWrapper.VOID.HasDeltaV);

            //}
            //GUILayout.Label(WindowPosByActiveScene.ToString());
            //GUILayout.Label(Input.mousePosition.ToString());
            //GUILayout.Label(Event.current.mousePosition.ToString());

            //GUILayout.Label(dragHandleWidth.ToString());
            //GUILayout.Label(resizingWidth.ToString());


            //if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION && KACWorkerGameState.CurrentVessel != null){
            //    GUILayout.Label(KACWorkerGameState.CurrentVessel.id.ToString());
            //    GUILayout.Label(KACWorkerGameState.CurrentVessel.orbitRenderer.isFocused.ToString());
            //    GUILayout.Label(KACWorkerGameState.CurrentVessel.orbitRenderer.drawIcons.ToString());
            //    GUILayout.Label(KACWorkerGameState.CurrentVessel.orbitRenderer.drawMode.ToString());
            //    GUILayout.Label(KACWorkerGameState.CurrentVessel.PatchedConicsAttached.ToString());

            //    if (GUILayout.Button("Toggle"))
            //    {
            //        KACWorkerGameState.CurrentVessel.orbitRenderer.isFocused = !KACWorkerGameState.CurrentVessel.orbitRenderer.isFocused;
            //        KACWorkerGameState.CurrentVessel.AttachPatchedConicsSolver();
            //    }

            //}
            //if ((KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION || (KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT && MapView.MapIsEnabled)) && KACWorkerGameState.CurrentVessel != null)
            //{

            //    //GUILayout.Label(KACWorkerGameState.CurrentVessel.orbit.timeToAp.ToString());
            //    GUILayout.Label(MapView.MapCamera.camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(KACWorkerGameState.CurrentVessel.orbit.getPositionAtUT(KACWorkerGameState.CurrentVessel.orbit.timeToAp))).ToString());
                
            //    if (KACWorkerGameState.ManeuverNodeExists)
            //    {

            //        GUILayout.Label(MapView.MapCamera.camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(KACWorkerGameState.CurrentVessel.orbit.getPositionAtUT(KACWorkerGameState.ManeuverNodeFuture.UT))).ToString());
            //        GUILayout.Label(MapView.MapCamera.camera.transform.position.ToString());
            //        GUILayout.Label((MapView.MapCamera.camera.transform.position - KACWorkerGameState.CurrentVessel.orbit.getPositionAtUT(KACWorkerGameState.ManeuverNodeFuture.UT)).ToString());



            //        //GUILayout.Label(MapView.MapCamera.camera.WorldToScreenPoint(KACWorkerGameState.CurrentVessel.orbit.getPositionAtUT(KACWorkerGameState.ManeuverNodeFuture.UT)).ToString());
            //        //GUILayout.Label(MapView.MapCamera.camera.WorldToScreenPoint(KACWorkerGameState.CurrentVessel.orbit.getRelativePositionAtUT(KACWorkerGameState.ManeuverNodeFuture.UT)).ToString());
            //        if (KACWorkerGameState.ManeuverNodeFuture.attachedGizmo == null)
            //        {
            //            GUILayout.Label("Attached=NULL");
            //        }
            //        else
            //        {
            //            GUILayout.Label("Attached=YEP");
            //            GUILayout.Label(KACWorkerGameState.ManeuverNodeFuture.attachedGizmo.grabArea.position.ToString());
            //            GUILayout.Label(MapView.MapCamera.camera.WorldToScreenPoint(KACWorkerGameState.ManeuverNodeFuture.attachedGizmo.grabArea.position).ToString());
            //            GUILayout.Label(KACWorkerGameState.ManeuverNodeFuture.attachedGizmo.grabArea.localScale.ToString());
            //            GUILayout.Label(KACWorkerGameState.ManeuverNodeFuture.attachedGizmo.buttonRoot.position.ToString());

            //        }
            //    }

            //    //GUILayout.Label(KACWorkerGameState.CurrentVessel == null ? "NULL" : (KACWorkerGameState.CurrentVessel.vesselName + "-" + KACWorkerGameState.CurrentVessel.id.ToString()));
                



            //    //SpaceTracking st = (SpaceTracking)KACSpaceCenter.FindObjectOfType(typeof(SpaceTracking));
            //    //foreach (MapObject item in st.mainCamera.targets)
            //    //{
            //    //    GUILayout.Label(String.Format("{0}-{1}-{2}", item.GetName(), item.type.ToString(), item.type== MapObject.MapObjectType.VESSEL?item.vessel.id.ToString():""));
            //    //}
            //}


            //GUILayout.Label(KACWorkerGameState.CurrentVessel.orbit.ToString());
            //GUILayout.Label(KACWorkerGameState.CurrentTime.ToStringStandard(DateStringFormatsEnum.DateTimeFormat));
            //GUILayout.Label(KACWorkerGameState.CurrentTime.ToStringStandard(DateStringFormatsEnum.KSPFormat));
            //GUILayout.Label(KACWorkerGameState.CurrentTime.ToStringStandard(DateStringFormatsEnum.KSPFormatWithSecs));
            //GUILayout.Label(KACWorkerGameState.CurrentTime.ToStringStandard(DateStringFormatsEnum.TimeAsUT));

            //GUILayout.Label(rawEntry.UT.ToString());
            //GUILayout.Label(rawEntry.Years.ToString());
            //GUILayout.Label(timeDefaultMargin.UT.ToString());
            //GUILayout.Label(timeDefaultMargin.Years.ToString());
            //GUILayout.Label(timeDefaultMargin.Days.ToString());
            //GUILayout.Label(timeDefaultMargin.Hours.ToString());
            //GUILayout.Label(timeDefaultMargin.Minutes.ToString());



            //if (GUILayout.Button("KSP Cal"))
            //{
            //    KSPDateStructure.SetKSPStockCalendar();
            //}
            //if (GUILayout.Button("Earth Cal"))
            //{
            //    KSPDateStructure.SetEarthCalendar();
            //}

            //if (GUILayout.Button("Add Lock"))
            //{
            //    InputLockManager.SetControlLock(ControlTypes.All, "LockTest");
            //}
            //if (GUILayout.Button("Remove Lock"))
            //{
            //    InputLockManager.RemoveControlLock("LockTest");
            //}


			//GUILayout.Label(String.Format("S:{0}", intSelectedContract));
			//GUILayout.Label(String.Format("LastState:{0}", contractLastState));
            //if (Contracts.ContractSystem.Instance != null)
            //{
            //    foreach (Contracts.Contract c in Contracts.ContractSystem.Instance.Contracts)
            //    {
            //        GUILayout.Label(string.Format("{0}:{1}:{2}", c.ContractID, c.ContractGuid, c.Title.Substring(0, 20)));
            //    }

            //    GUILayout.Label("CONTRACTS Variable");
            //    foreach (Contracts.Contract c in lstContracts)
            //    {
            //        GUILayout.Label(string.Format("{0}:{1}:{2}", c.ContractID, c.ContractGuid, c.Title.Substring(0, 20)));
            //    }

			//    GUILayout.Label(String.Format("Title: {0}", lstContracts[intSelectedContract].Title));
			//    //GUILayout.Label(String.Format("Syn: {0}", lstContracts[intSelectedContract].Synopsys));
			//    //GUILayout.Label(String.Format("Desc: {0}", lstContracts[intSelectedContract].Description));

			//    //foreach (Contracts.ContractParameter cp in lstContracts[intSelectedContract].AllParameters)
			//    //{
			//    //    GUILayout.Label(String.Format("Param: {0}={1}", cp.Title,cp.Optional));
			//    //}
			//    //GUILayout.Label(String.Format("GUID: {0}", lstContracts[intSelectedContract].ContractGuid));
			//    //GUILayout.Label(String.Format("State: {0}", lstContracts[intSelectedContract].ContractState));
			//    //GUILayout.Label(String.Format("Notes: {0}", lstContracts[intSelectedContract].Notes));

			//    foreach (Contracts.Contract item in lstContracts.Where(ci => ci.ContractState == Contracts.Contract.State.Offered).OrderBy(ci => ci.DateNext()))
			//    {
			//        GUILayout.Label(String.Format("{0}\r\nDate:{1}, Date-margin:{2}", item.Title, item.DateNext(), item.DateNext()-settings.AlarmOnContractExpireMargin));
			//    //    //GUILayout.Label(String.Format("DateExpire:{0}DateDeadline:{1}", item.DateExpire,item.DateDeadline));
			//    }
            //}

			//GUILayout.Label(String.Format("{0}", WarpTransitionCalculator.WarpRateHash));
			//foreach (WarpTransition item in WarpTransitionCalculator.WarpRateTransitionPeriods.OrderBy(w => w.Index))
			//{
			//    GUILayout.Label(string.Format("{0}({1}):Up-{2} Down-{3} To0-{4}", item.Rate, item.Index, item.UTToRateUp, item.UTToRateDown, item.UTTo1Times));
			//}
			//for (int i = 0; i < TimeWarp.fetch.warpRates.Length; i++)
			//{
			//    GUILayout.Label(string.Format("{0}({1})", TimeWarp.fetch.warpRates[i], i));
			//}
		   
			////    btnAppLauncher = InitAppLauncherButton();
			
			//if (GUILayout.Button("Calc Rates"))
			//{
			//    WarpTransitionCalculator.CalcWarpRateTransitions();
			//}

			//if (GUILayout.Button("Set Rate to inttest4"))
			//{

			//    LogFormatted("StartTransition({2}->{3},UT:{0},Time:{1}",Planetarium.GetUniversalTime(),DateTime.Now,TimeWarp.CurrentRateIndex,intTestheight4);
			//    TransStart = DateTime.Now;
			//    UTStart = Planetarium.GetUniversalTime();
			//    warptrans = true;
			//    TimeWarp.SetRate(intTestheight4,false);

			//}

			//if (warptrans)
			//{
			//    if (TimeWarp.CurrentRate==TimeWarp.fetch.warpRates[intTestheight4]){
			//        warptrans = false;
			//        LogFormatted("EndTransition,UT:{0},Time:{1}", Planetarium.GetUniversalTime(), DateTime.Now);
			//        LogFormatted("Transition,UT:{0},Time:{1}", Planetarium.GetUniversalTime()-UTStart, (DateTime.Now-TransStart).TotalMilliseconds);
			//    }
			//}

			//for (int i = 0; i < 8; i++)
			//{
			//    GUILayout.Label(String.Format("{0}->{1} : {2}", TimeWarp.CurrentRateIndex,i,UTChanges[i]));
			//}

			//if (KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT)
			//{
			//    GUILayout.Label(String.Format("Rate: {0}", TimeWarp.CurrentRate));
			//    GUILayout.Label(String.Format("Text: {0}", TimeWarp.fetch.textDuration));

			//}


			//if (GUILayout.Button("Load Old Alarm List"))
			//{
			//    winAlarmImport.Visible = !winAlarmImport.Visible;
			//    //KACAlarmList oldAlarms = UtilitiesLegacy.Loadv2Alarms();

			//    //if (oldAlarms.Count>0)
			//    //{
			//    //    //write it out
			//    //    foreach (KACAlarm oldAlarm in oldAlarms)
			//    //    {
			//    //        LogFormatted("{0} @ {1}", oldAlarm.Name, oldAlarm.AlarmTime.UT);
			//    //    }
			//    //    LogFormatted("{0}", oldAlarms.EncodeToCN().ToString());

			//    //    alarms = oldAlarms;
			//    //}
			//    //else
			//    //{
			//    //    LogFormatted("Could not find alarms file for: {0}", HighLogic.CurrentGame.Title);
			//    //}

			//}

			//intMainWindowEarthTimeHeight = intTestheight4;

			//if (GUILayout.Button("Create"))
			//{
			//    CreateAlarm(KACAlarm.AlarmTypeEnum.Raw, "TEST");
			//}

			//if (GUILayout.Button("KSP"))
			//{
			//    settings.SelectedSkin = Settings.DisplaySkin.Default;
			//    KACResources.SetSkin(settings.SelectedSkin);
			//    settings.Save();
			//}
			//if (GUILayout.Button("Unity"))
			//{
			//    settings.SelectedSkin = Settings.DisplaySkin.Unity;
			//    KACResources.SetSkin(settings.SelectedSkin);
			//    settings.Save();
			//}
			//if (GUILayout.Button("Unity w KSP"))
			//{
			//    settings.SelectedSkin = Settings.DisplaySkin.UnityWKSPButtons;
			//    KACResources.SetSkin(settings.SelectedSkin);
			//    settings.Save();
			//}


			//GUILayout.Label("MouseAny: " + MouseOverAnyWindow.ToString());
			//GUILayout.Label("MouseSettings: " + MouseOverWindow(_WindowSettingsRect, WindowVisibleByActiveScene && _ShowSettings).ToString());
			//GUILayout.Label("MainRect: " + WindowPosByActiveScene.ToString());
			//GUILayout.Label("SettingsRect: " + _WindowSettingsRect.ToString());
			//GUILayout.Label("MousePos: " + Event.current.mousePosition.ToString());


			//GUILayout.Label("button: " + ddlSettingsSkin.rectButton.ToString());
			//GUILayout.Label("list: " + ddlSettingsSkin.rectListBox.ToString());

			//foreach (KACAlarm item in alarms)
			//{
			//    GUILayout.Label(String.Format("{0}:{1}", item.Name, item.AlarmAction));
			//}

			
			if (KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT)
			{
				//List<Double> LFs = new List<double>();
				//List<String> Ships=new List<string>();
				//Double LF = 0;
				//foreach (Part item in FlightGlobals.ActiveVessel.Parts)
				//{
				//    foreach (PartResource pr in item.Resources)
				//    {
				//        if (pr.info.name == "ElectricCharge")
				//            LF += pr.amount;
				//    }
				//}

				//foreach (Vessel v in FlightGlobals.Vessels.Where(x=>x.vesselType==VesselType.Ship))
				//{
				//    Double LFtemp = 0;
				//    foreach (Part item in v.Parts)
				//    {
				//        foreach (PartResource pr in item.Resources)
				//        {
				//            if (pr.info.name == "ElectricCharge")
				//                LFtemp += pr.amount;
				//        }
				//    }
				//    Ships.Add(v.vesselName);
				//    LFs.Add(LFtemp);
				//    GUILayout.Label(String.Format("{0}-{1:0.0}", v.vesselName, LFtemp));
				//}

								
				
				//GUILayout.Label (KACWorkerGameState.ManeuverNodeExists.ToString());
				//GUILayout.Label((KACWorkerGameState.ManeuverNodeFuture != null).ToString());



				
				//FlightState fs = new FlightState();
				//GUILayout.Label (fs.activeVesselIdx.ToString());

				//for (int i = 0; i < FlightGlobals.Vessels.Count; i++)
				//{
				//    GUILayout.Label(string.Format("{0}-{1}",i,FlightGlobals.Vessels[i].vesselName));
				//}
				//foreach (Vessel vt in FlightGlobals.Vessels)
				//{
					
				//}

				//if (FlightGlobals.fetch.VesselTarget != null)
				//{
				//    double time1 = LaunchTiming.TimeToPlane(KACWorkerGameState.CurrentVessel.mainBody,
				//        KACWorkerGameState.CurrentVessel.latitude, KACWorkerGameState.CurrentVessel.longitude, FlightGlobals.fetch.VesselTarget.GetOrbit());
				//    GUILayout.Label(time1.ToString());


				//}
				//if (KACWorkerGameState.CurrentVesselTarget != null)
				//{
				//    String strReturn = GUILayout.TextField(intTestDistance.ToString());
				//    Double dblTarget = Convert.ToDouble(strReturn);

				//    double dblClosestDistance;
				//    double dblTargetUT = KACUtils.timeOfTargetDistance(KACWorkerGameState.CurrentVessel.orbit,
				//                                                            KACWorkerGameState.CurrentVesselTarget.GetOrbit(),
				//                                                            KACWorkerGameState.CurrentTime.UT,
				//                                                            KACWorkerGameState.CurrentVessel.orbit.period * intOrbits,
				//                                                            20,
				//                                                            dblTarget,
				//                                                            out dblClosestDistance
				//                                                            );
				//    string strDisplay = string.Format("s:{0:0},e:{1:0},p:{2:0},dist:{3:0},Time:{4:0}",
				//            KACWorkerGameState.CurrentTime.UT,
				//           KACWorkerGameState.CurrentTime.UT + (KACWorkerGameState.CurrentVessel.orbit.period * intOrbits),
				//            KACWorkerGameState.CurrentVessel.orbit.period,
				//            dblClosestDistance,
				//            dblTargetUT);
				//    GUILayout.Label(strDisplay);
				//}

				//if (KACWorkerGameState.CurrentVesselTarget == null)
				//    GUILayout.Label("Equatorial Nodes (No Vessel Target)", KACResources.styleAddXferName, GUILayout.Height(18));
				//else
				//{
				//    if (KACWorkerGameState.CurrentVessel.orbit.referenceBody ==KACWorkerGameState.CurrentVesselTarget.GetOrbit().referenceBody)
				//    {
				//        if (KACWorkerGameState.CurrentVesselTarget is Vessel)
				//            GUILayout.Label("Target Vessel: " + KACWorkerGameState.CurrentVesselTarget.GetVessel().vesselName, KACResources.styleAddXferName, GUILayout.Height(18));
				//        else if (KACWorkerGameState.CurrentVesselTarget is CelestialBody)
				//            GUILayout.Label("Target Body: " + ((CelestialBody)KACWorkerGameState.CurrentVesselTarget).bodyName, KACResources.styleAddXferName, GUILayout.Height(18));
				//        else
				//            GUILayout.Label("Object Targeted", KACResources.styleAddXferName, GUILayout.Height(18));
				//        //GUILayout.Label("Target Vessel: " + KACWorkerGameState.CurrentVesselTarget.GetVessel().vesselName, KACResources.styleAddXferName, GUILayout.Height(18));
				//    }
				//    else
				//    {
				//        GUILayout.Label("Target Not Orbiting Same Parent", KACResources.styleAddXferName, GUILayout.Height(18));
				//    }
				//}
				//if (KACWorkerGameState.CurrentVesselTarget == null)
				//{
				//    GUILayout.Label(KACWorkerGameState.CurrentVessel.orbit.AscendingNodeEquatorialExists().ToString());
				//                                    //KACWorkerGameState.CurrentVessel.orbit.TimeOfAscendingNodeEquatorial(KACWorkerGameState.CurrentTime.UT) - KACWorkerGameState.CurrentTime.UT);
				//}
				//else
				//{
				//    GUILayout.Label(KACWorkerGameState.CurrentVessel.orbit.AscendingNodeExists(KACWorkerGameState.CurrentVesselTarget.GetOrbit()).ToString());
				//                                    //KACWorkerGameState.CurrentVessel.orbit.TimeOfAscendingNode(KACWorkerGameState.CurrentVesselTarget.GetOrbit(), KACWorkerGameState.CurrentTime.UT) - KACWorkerGameState.CurrentTime.UT);
				//}


				//GUILayout.Label(KACWorkerGameState.CurrentVessel.orbit.AscendingNodeEquatorialExists().ToString());
				//GUILayout.Label(KACWorkerGameState.CurrentVessel.orbit.TimeOfAscendingNodeEquatorial(KACWorkerGameState.CurrentTime.UT).ToString());
				//GUILayout.Label(KACWorkerGameState.CurrentVessel.orbit.patchEndTransition.ToString());
				//GUILayout.Label(KACWorkerGameState.CurrentVessel.orbit.UTsoi.ToString());


				//Settings.SOITransitions.Contains(KACWorkerGameState.CurrentVessel.orbit.patchEndTransition))
				//{
				//    timeSOIChange = KACWorkerGameState.CurrentVessel.orbit.UTsoi

				//if (FlightGlobals.fetch.VesselTarget != null)
				//{
				//    if (KACWorkerGameState.CurrentVessel.orbit.AscendingNodeExists(FlightGlobals.fetch.VesselTarget.GetOrbit()))
				//    {
				//        GUILayout.Label("AN:" + KACWorkerGameState.CurrentVessel.orbit.TimeOfAscendingNode(FlightGlobals.fetch.VesselTarget.GetOrbit(), KACWorkerGameState.CurrentTime.UT).ToString());
				//    }
				//    if (KACWorkerGameState.CurrentVessel.orbit.DescendingNodeExists(FlightGlobals.fetch.VesselTarget.GetOrbit()))
				//    {
				//        GUILayout.Label("DN:" + KACWorkerGameState.CurrentVessel.orbit.TimeOfDescendingNode(FlightGlobals.fetch.VesselTarget.GetOrbit(), KACWorkerGameState.CurrentTime.UT).ToString());
				//    }

				//    if (KACWorkerGameState.CurrentVessel.orbit.AscendingNodeExists(KACWorkerGameState.CurrentVesselTarget.GetOrbit()))
				//    {
				//        GUILayout.Label("AN:" + KACWorkerGameState.CurrentVessel.orbit.TimeOfAscendingNode(KACWorkerGameState.CurrentVesselTarget.GetOrbit(), KACWorkerGameState.CurrentTime.UT).ToString());
				//    }
				//    if (KACWorkerGameState.CurrentVessel.orbit.DescendingNodeExists(KACWorkerGameState.CurrentVesselTarget.GetOrbit()))
				//    {
				//        GUILayout.Label("DN:" + KACWorkerGameState.CurrentVessel.orbit.TimeOfDescendingNode(KACWorkerGameState.CurrentVesselTarget.GetOrbit(), KACWorkerGameState.CurrentTime.UT).ToString());
				//    }
				//}

				//GUILayout.Label(KACWorkerGameState.CurrentTime.UT.ToString());
				//GUILayout.Label(Planetarium.GetUniversalTime().ToString());

				//if (FlightGlobals.fetch.VesselTarget != null)
				//{
				//    GUILayout.Label(KACWorkerGameState.CurrentVesselTarget.GetOrbit().LAN.ToString());
				//    GUILayout.Label(FlightGlobals.fetch.VesselTarget.GetOrbit().LAN.ToString());
				//}
				//GUILayout.BeginVertical();
				//GUILayout.Label("Closest:");
				//GUILayout.Label("ClosestIn5:");
				//GUILayout.Label("test2:");
				//GUILayout.Label("test3:");
				//GUILayout.Label("test4:");

				//GUILayout.Label("AN1:");
				//GUILayout.Label("AN2:");
				//GUILayout.Label("Window Padding:", GUILayout.ExpandWidth(true));
				//GUILayout.Label("MainWindowWidth:", GUILayout.ExpandWidth(true));
				//GUILayout.Label("MainWindowMinHeight:", GUILayout.ExpandWidth(true));
				//GUILayout.Label("MainWindowBaseHeight:", GUILayout.ExpandWidth(true));
				//GUILayout.Label("MainWindowAlarmListItem:", GUILayout.ExpandWidth(true));
				//GUILayout.Label("MainWindowAlarmListScrollPad:", GUILayout.ExpandWidth(true));
				//GUILayout.Label("PaneWidth:", GUILayout.ExpandWidth(true));



				//GUILayout.Label("Moho", GUILayout.ExpandWidth(true));
				//GUILayout.Label("Eve", GUILayout.ExpandWidth(true));
				//GUILayout.Label("Duna", GUILayout.ExpandWidth(true));
				//GUILayout.Label("Dres", GUILayout.ExpandWidth(true));
				//GUILayout.Label("Jool", GUILayout.ExpandWidth(true));
				//GUILayout.Label("Eeloo", GUILayout.ExpandWidth(true));

				//GUILayout.EndVertical();


				//intTestheight = Convert.ToInt32(GUILayout.TextField(intTestheight.ToString()));
				//intTestheight2 = Convert.ToInt32(GUILayout.TextField(intTestheight2.ToString()));
				//intTestheight3 = Convert.ToInt32(GUILayout.TextField(intTestheight3.ToString()));
				//intTestheight4 = Convert.ToInt32(GUILayout.TextField(intTestheight4.ToString()));


				////Double timeToAN;
				////Boolean blnANExists = KACUtils.CalcTimeToANorDN(KACWorkerGameState.CurrentVessel, KACUtils.ANDNNodeType.Ascending, out timeToAN);
				//GUILayout.Label(Settings.XferUseModelData.ToString());
				//GUILayout.Label(Settings.XferModelDataLoaded.ToString());
				//if (blnANExists)
				//    GUILayout.Label(timeToAN.ToString());

				//intTestheight3 = Convert.ToInt32(GUILayout.TextField(intTestheight3.ToString()));
				////intSettingsPaneHeight = Convert.ToInt32(GUILayout.TextField(intSettingsPaneHeight.ToString()));
				//String strPadding = KACResources.styleWindow.padding.left.ToString();
				//int intPadding = Convert.ToInt32(GUILayout.TextField(strPadding));
				//KACResources.styleWindow.padding = KACUtils.SetWindowRectOffset(KACResources.styleWindow.padding, intPadding);
				//    //.left = intPadding;// = new RectOffset(intPadding, intPadding, intPadding, intPadding);
				//intMainWindowWidth = Convert.ToInt32(GUILayout.TextField(intMainWindowWidth.ToString()));
				//intMainWindowMinHeight = Convert.ToInt32(GUILayout.TextField(intMainWindowMinHeight.ToString()));
				//intMainWindowBaseHeight = Convert.ToInt32(GUILayout.TextField(intMainWindowBaseHeight.ToString()));
				//intMainWindowSOIAutoHeight = Convert.ToInt32(GUILayout.TextField(intMainWindowSOIAutoHeight.ToString()));
				//intMainWindowAlarmListItemHeight = Convert.ToInt32(GUILayout.TextField(intMainWindowAlarmListItemHeight.ToString()));
				//intMainWindowAlarmListScrollPad = Convert.ToInt32(GUILayout.TextField(intMainWindowAlarmListScrollPad.ToString()));

				//intPaneWindowWidth = Convert.ToInt32(GUILayout.TextField(intPaneWindowWidth.ToString()));

				//CurrentPhase (Desired Phase: diff)
				//GUILabelPhaseApproach(FlightGlobals.Bodies[1].orbit, FlightGlobals.Bodies[4].orbit);
				//GUILabelPhaseApproach(FlightGlobals.Bodies[1].orbit, FlightGlobals.Bodies[5].orbit);
				//GUILabelPhaseApproach(FlightGlobals.Bodies[1].orbit, FlightGlobals.Bodies[6].orbit);
				//GUILabelPhaseApproach(FlightGlobals.Bodies[1].orbit, FlightGlobals.Bodies[15].orbit);
				//GUILabelPhaseApproach(FlightGlobals.Bodies[1].orbit, FlightGlobals.Bodies[8].orbit);
				//GUILabelPhaseApproach(FlightGlobals.Bodies[1].orbit, FlightGlobals.Bodies[16].orbit);


				//if (FlightGlobals.fetch.VesselTarget != null)
				//{

				//    double dblClosestDistance;
				//    int intClosestOrbitPass=-1;
				//    double dblClosestUT = KACUtils.timeOfClosestApproach(KACWorkerGameState.CurrentVessel.orbit,
				//                                                            KACWorkerGameState.CurrentVesselTarget.GetOrbit(),
				//                                                            KACWorkerGameState.CurrentTime.UT,
				//                                                            KACWorkerGameState.CurrentVessel.orbit.period * intOrbits,
				//                                                            20,
				//                                                            out dblClosestDistance
				//                                                            );

				//    string strDisplay = string.Format("s:{0:0},e:{1:0},p:{2:0},dist:{3:0},Time:{4:0}",
				//            KACWorkerGameState.CurrentTime.UT,
				//           KACWorkerGameState.CurrentTime.UT + (KACWorkerGameState.CurrentVessel.orbit.period * intOrbits),
				//            KACWorkerGameState.CurrentVessel.orbit.period,
				//            dblClosestDistance,
				//            dblClosestUT);
				//    GUILayout.Label(strDisplay);
				//    GUILayout.Label(string.Format("Orbits:{0} - p:{1}", intOrbits, KACWorkerGameState.CurrentVessel.orbit.period));


				//    //KACTime ktmp;
				//    double closestdistance;


				////    ktmp = new KerbalTime(timeOfClosestApproach(KACWorkerGameState.CurrentVessel.orbit,
				////        FlightGlobals.fetch.VesselTarget.GetOrbit(),
				////        KACWorkerGameState.CurrentTime.UT,
				////            out closestdistance) - KACWorkerGameState.CurrentTime.UT);
				////    GUILayout.Label(KerbalTime.PrintInterval(ktmp, Settings.TimeFormat));

				////    //ktmp = new KerbalTime(timeOfClosestApproach(KACWorkerGameState.CurrentVessel.orbit,
				////    //    FlightGlobals.fetch.VesselTarget.GetOrbit(),
				////    //    KACWorkerGameState.CurrentTime.UT,
				////    //    KACWorkerGameState.CurrentVessel.orbit.period * 5,150) - KACWorkerGameState.CurrentTime.UT);
				////    //GUILayout.Label(KerbalTime.PrintInterval(ktmp, Settings.TimeFormat));


				//foreach (CelestialBody cbTemp in FlightGlobals.Bodies)
				//{
				//    GUILayout.Label(string.Format("{0}({2}): {1}",cbTemp.bodyName,cbTemp.maxAtmosphereAltitude.ToString(),cbTemp.atmosphere.ToString()));
				//}



				//int intClosestOrbit = 1;
				//double ClosestOverall = double.MaxValue;
				//double ClosestOverallTime = double.MaxValue;

				//double closestdistance;
				//double dblClosestUT;
				//string strDisplay;
				//Int32 intTestOrbits = 1;

				//intTestDistance = Convert.ToInt32(GUILayout.TextField(intTestDistance.ToString()));

				//GUILayout.Label("CUrrent");
				//GUILayout.Label((KACWorkerGameState.CurrentVessel.orbit.getRelativePositionAtUT(KACWorkerGameState.CurrentTime.UT).magnitude - KACWorkerGameState.CurrentVessel.orbit.referenceBody.Radius).ToString());

				//GUILayout.Label((KACWorkerGameState.CurrentVessel.orbit.getRelativePositionAtUT(KACWorkerGameState.CurrentTime.UT).magnitude - KACWorkerGameState.CurrentVessel.orbit.referenceBody.Radius).ToString());

				//GUILayout.Label(KACWorkerGameState.CurrentVessel.orbit.referenceBody.GetName());
				//GUILayout.Label(KACWorkerGameState.CurrentVessel.orbit.referenceBody.referenceBody.GetName());
				//if (KACWorkerGameState.CurrentVessel.orbit.referenceBody.referenceBody.referenceBody==null)
				//    GUILayout.Label("nulled");

				//dblClosestUT = KACUtils.timeOfTargetAltitude(KACWorkerGameState.CurrentVessel.orbit,
				//                                            KACWorkerGameState.CurrentTime.UT,
				//                                            20,
				//                                            out closestdistance,
				//                                            120000
				//                                            );
				//strDisplay = string.Format("s:{0:0},e:{1:0},p:{2:0},dist:{3},Time:{4:0}",
				//        KACWorkerGameState.CurrentTime.UT,
				//       KACWorkerGameState.CurrentVessel.orbit.EndUT,
				//        KACWorkerGameState.CurrentVessel.orbit.period,
				//        closestdistance,
				//        dblClosestUT);
				//GUILayout.Label(strDisplay);

				//GUILayout.Label((KACWorkerGameState.CurrentVessel.orbit.getRelativePositionAtUT(26349).magnitude - KACWorkerGameState.CurrentVessel.orbit.referenceBody.Radius).ToString());
				//GUILayout.Label((KACWorkerGameState.CurrentVessel.orbit.getRelativePositionAtUT(26350).magnitude - KACWorkerGameState.CurrentVessel.orbit.referenceBody.Radius).ToString());
				//GUILayout.Label((KACWorkerGameState.CurrentVessel.orbit.getRelativePositionAtUT(26351).magnitude - KACWorkerGameState.CurrentVessel.orbit.referenceBody.Radius).ToString());
				//GUILayout.Label((KACWorkerGameState.CurrentVessel.orbit.getRelativePositionAtUT(27030).magnitude - KACWorkerGameState.CurrentVessel.orbit.referenceBody.Radius).ToString());
				//GUILayout.Label((KACWorkerGameState.CurrentVessel.orbit.getRelativePositionAtUT(27031).magnitude - KACWorkerGameState.CurrentVessel.orbit.referenceBody.Radius).ToString());
				//GUILayout.Label((KACWorkerGameState.CurrentVessel.orbit.getRelativePositionAtUT(27032).magnitude - KACWorkerGameState.CurrentVessel.orbit.referenceBody.Radius).ToString());


				//dblClosestUT = KACUtils.timeOfTargetAltitude(KACWorkerGameState.CurrentVessel.orbit,
				//                                            KACWorkerGameState.CurrentTime.UT,
				//                                            KACWorkerGameState.CurrentVessel.orbit.EndUT - KACWorkerGameState.CurrentTime.UT,
				//                                            40,
				//                                            out closestdistance,
				//                                            120000,
				//                                            2
				//                                            );
				//strDisplay = string.Format("s:{0:0},e:{1:0},p:{2:0},dist:{3},Time:{4:0}",
				//        KACWorkerGameState.CurrentTime.UT,
				//       KACWorkerGameState.CurrentVessel.orbit.EndUT,
				//        KACWorkerGameState.CurrentVessel.orbit.period,
				//        closestdistance,
				//        dblClosestUT);
				//GUILayout.Label(strDisplay);


				//GUILayout.Label("Ref Body");
				//for (int i = 1; i <= intTestOrbits; i++)
				//{
				//    dblClosestUT = KACUtils.timeOfClosestApproach(KACWorkerGameState.CurrentVessel.orbit,
				//                                    KACWorkerGameState.CurrentVessel.orbit.referenceBody.orbit,
				//                                    KACWorkerGameState.CurrentTime.UT,
				//                                        i,
				//                                        out closestdistance
				//                                        );


				//    if (closestdistance < ClosestOverall)
				//    {
				//        intClosestOrbit = i;
				//        ClosestOverall = closestdistance;
				//        ClosestOverallTime = dblClosestUT;
				//    }
				//    //ktmp = new KACTime(KACUtils.timeOfClosestApproach(KACWorkerGameState.CurrentVessel.orbit,
				//    //    FlightGlobals.fetch.VesselTarget.GetOrbit(),
				//    //        KACWorkerGameState.CurrentTime.UT,
				//    //        i,
				//    //        out closestdistance)
				//    //        - KACWorkerGameState.CurrentTime.UT);
				//    strDisplay = string.Format("s:{0:0},e:{1:0},p:{2:0},dist:{3:0},Time:{4:0}",
				//        KACWorkerGameState.CurrentTime.UT + ((i - 1) * KACWorkerGameState.CurrentVessel.orbit.period),
				//       KACWorkerGameState.CurrentTime.UT + ((i) * KACWorkerGameState.CurrentVessel.orbit.period),
				//        KACWorkerGameState.CurrentVessel.orbit.period,
				//        closestdistance,
				//        dblClosestUT);
				//    GUILayout.Label(strDisplay);
				//    //GUILayout.Label(string.Format("\to:{0}-{1}", KACTime.PrintInterval(ktmp, Settings.TimeFormat), closestdistance));
				//}

				//if (KACWorkerGameState.SOIPointExists)
				//{
				//    GUILayout.Label("NextPatch");
				//    for (int i = 1; i <= intTestOrbits; i++)
				//    {
				//        dblClosestUT = KACUtils.timeOfClosestApproach(KACWorkerGameState.CurrentVessel.orbit,
				//                                        KACWorkerGameState.CurrentVessel.orbit.nextPatch.referenceBody.orbit,
				//                                        KACWorkerGameState.CurrentVessel.orbit.nextPatch.StartUT,
				//                                            i,
				//                                            out closestdistance
				//                                            );


				//        if (closestdistance < ClosestOverall)
				//        {
				//            intClosestOrbit = i;
				//            ClosestOverall = closestdistance;
				//            ClosestOverallTime = dblClosestUT;
				//        }
				//        //ktmp = new KACTime(KACUtils.timeOfClosestApproach(KACWorkerGameState.CurrentVessel.orbit,
				//        //    FlightGlobals.fetch.VesselTarget.GetOrbit(),
				//        //        KACWorkerGameState.CurrentTime.UT,
				//        //        i,
				//        //        out closestdistance)
				//        //        - KACWorkerGameState.CurrentTime.UT);
				//        strDisplay = string.Format("s:{0:0},e:{1:0},p:{2:0},dist:{3:0},Time:{4:0}",
				//            KACWorkerGameState.CurrentTime.UT + ((i - 1) * KACWorkerGameState.CurrentVessel.orbit.period),
				//           KACWorkerGameState.CurrentTime.UT + ((i) * KACWorkerGameState.CurrentVessel.orbit.period),
				//            KACWorkerGameState.CurrentVessel.orbit.period,
				//            closestdistance,
				//            dblClosestUT);
				//        GUILayout.Label(strDisplay);
				//        //GUILayout.Label(string.Format("\to:{0}-{1}", KACTime.PrintInterval(ktmp, Settings.TimeFormat), closestdistance));
				//    }
				//}

				//if (FlightGlobals.fetch.VesselTarget != null)
				//{


				//GUILayout.Label("Closest");
				//for (int i = 1; i <= intTestOrbits; i++)
				//{
				//    dblClosestUT = KACUtils.timeOfClosestApproach(KACWorkerGameState.CurrentVessel.orbit,
				//                                        KACWorkerGameState.CurrentVesselTarget.GetOrbit(),
				//                                        KACWorkerGameState.CurrentTime.UT,
				//                                        i,
				//                                        out closestdistance
				//                                        );

				//    if (closestdistance < ClosestOverall)
				//    {
				//        intClosestOrbit = i;
				//        ClosestOverall = closestdistance;
				//        ClosestOverallTime = dblClosestUT;
				//    }
				//    //ktmp = new KACTime(KACUtils.timeOfClosestApproach(KACWorkerGameState.CurrentVessel.orbit,
				//    //    FlightGlobals.fetch.VesselTarget.GetOrbit(),
				//    //        KACWorkerGameState.CurrentTime.UT,
				//    //        i,
				//    //        out closestdistance)
				//    //        - KACWorkerGameState.CurrentTime.UT);
				//    strDisplay = string.Format("s:{0:0},e:{1:0},p:{2:0},dist:{3:0},Time:{4:0}",
				//        KACWorkerGameState.CurrentTime.UT + ((i - 1) * KACWorkerGameState.CurrentVessel.orbit.period),
				//       KACWorkerGameState.CurrentTime.UT + ((i) * KACWorkerGameState.CurrentVessel.orbit.period),
				//        KACWorkerGameState.CurrentVessel.orbit.period,
				//        closestdistance,
				//        dblClosestUT);
				//    GUILayout.Label(strDisplay);
				//    //GUILayout.Label(string.Format("\to:{0}-{1}", KACTime.PrintInterval(ktmp, Settings.TimeFormat), closestdistance));
				//}
				//GUILayout.Label("Target");
				//GUILayout.Label(strNextPatch);
				//intTestDistance = Convert.ToInt32(GUILayout.TextField(intTestDistance.ToString()));

				//for (int i = 1; i <= intTestOrbits; i++)
				//{
				//    dblClosestUT = KACUtils.timeOfTargetDistance(VesselOrbitToCompare,
				//                                    tgtSelectedDistance.GetOrbit(),
				//                                    KACWorkerGameState.CurrentTime.UT,
				//                                        i,
				//                                        out closestdistance,
				//                                        intTestDistance
				//                                        );


				//    if (closestdistance < ClosestOverall)
				//    {
				//        intClosestOrbit = i;
				//        ClosestOverall = closestdistance;
				//        ClosestOverallTime = dblClosestUT;
				//    }
				//    //ktmp = new KACTime(KACUtils.timeOfClosestApproach(KACWorkerGameState.CurrentVessel.orbit,
				//    //    FlightGlobals.fetch.VesselTarget.GetOrbit(),
				//    //        KACWorkerGameState.CurrentTime.UT,
				//    //        i,
				//    //        out closestdistance)
				//    //        - KACWorkerGameState.CurrentTime.UT);
				//    strDisplay = string.Format("s:{0:0},e:{1:0},p:{2:0},dist:{3:0},Time:{4:0}",
				//        KACWorkerGameState.CurrentTime.UT + ((i - 1) * KACWorkerGameState.CurrentVessel.orbit.period),
				//       KACWorkerGameState.CurrentTime.UT + ((i) * KACWorkerGameState.CurrentVessel.orbit.period),
				//        KACWorkerGameState.CurrentVessel.orbit.period,
				//        closestdistance,
				//        dblClosestUT);
				//    GUILayout.Label(strDisplay);
				//    //GUILayout.Label(string.Format("\to:{0}-{1}", KACTime.PrintInterval(ktmp, Settings.TimeFormat), closestdistance));
				//}

				//}

				//    strDisplay = string.Format("dist:{0:0},Time:{1:0},Orbit:{2}",ClosestOverall,ClosestOverallTime,intClosestOrbit);
				//    GUILayout.Label(strDisplay);

				////    /////////////////////////////////////////////////////////////////////////
				////    //need to draw up start and end of each prbit times to see whats going on!!
				////    /////////////////////////////////////////////////////////////////////////


				////    //try doing same values as first orbit, but do start from end 1st orbit and go one orbit
				//}
				//else
				//{
				//    GUILayout.Label("NoTarget");
				////    GUILayout.Label("NoTarget");
				//}


				//GUILayout.Label(blnRecalc.ToString());
			}
			GUILayout.EndVertical();
			GUI.DragWindow();

		}
				
		//bool blnRecalc = false;

		//private void GUILabelPhaseApproach(Orbit orbitOrig,Orbit orbitTarget)
		//{
		//    double angleTarget = KACUtils.clampDegrees360(180 * (1 - Math.Pow((orbitOrig.semiMajorAxis + orbitTarget.semiMajorAxis) / (2 * orbitTarget.semiMajorAxis), 1.5)));
		//    double angleCurrent = KACUtils.clampDegrees360(orbitTarget.trueAnomaly + orbitTarget.argumentOfPeriapsis + orbitTarget.LAN - (orbitOrig.trueAnomaly + orbitOrig.argumentOfPeriapsis + orbitOrig.LAN));
			
		//    double angleChangepersec = (360 / orbitTarget.period) - (360 / orbitOrig.period);

		//    double angleToMakeUp =angleCurrent-angleTarget;
		//    if (angleToMakeUp > 0 && angleChangepersec > 0)
		//        angleToMakeUp -= 360;
		//    if (angleToMakeUp < 0 && angleChangepersec < 0)
		//        angleToMakeUp += 360;

		//    double UTToTarget = Math.Floor(Math.Abs(angleToMakeUp / angleChangepersec));

		//    double UTTimeForAlarm = Math.Floor(KACWorkerGameState.CurrentTime.UT + Math.Abs(angleToMakeUp / angleChangepersec));
		//    KACAlarm alarmTarget = new KACAlarm(UTTimeForAlarm);

		//    GUILayout.Label(String.Format("{0:0.00} ({1:0.00})-{2:0}-{3:0}-{4}",
		//        angleTarget,
		//        angleToMakeUp,
		//        UTToTarget,
		//        UTTimeForAlarm,
		//        alarmTarget.Remaining.IntervalString(2)
		//        ));
		//}

		//public void WriteManeuverFile(List<ManeuverNode> m, String FileName)
		//{
		//    if (m.Count > 0)
		//    {
		//        KSP.IO.TextWriter tw = KSP.IO.TextWriter.CreateForType<KerbalAlarmClock>(FileName);
		//        String strInfo="";
		//        foreach (ManeuverNode mNode in m)
		//        {
		//            strInfo += WriteNodeDetails(mNode);
		//        }
		//        strInfo+= KACAlarm.ManNodeSerializeList(FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes) + "\r\n";
		//        //strInfo+= KACAlarm.ManNodeSerializeList(tmpAlarm.ManNodes)+ "\r\n";
		//        tw.WriteLine(strInfo);
		//        tw.Close();
		//    }
		//}

		//private static string WriteNodeDetails( ManeuverNode mNode)
		//{
		//    String strInfo = "";
		//    strInfo += "attachedGizmo:" + mNode.attachedGizmo + "\r\n";
		//    strInfo += "DeltaV:" + mNode.DeltaV + "\r\n";
		//    strInfo += "nextPatch:" + mNode.nextPatch + "\r\n";
		//    strInfo += "nodeRotation:" + mNode.nodeRotation + "\r\n";
		//    strInfo += "patch:" + mNode.patch + "\r\n";
		//    strInfo += "scaledSpaceTarget:" + mNode.scaledSpaceTarget + "\r\n";
		//    strInfo += "solver:" + mNode.solver + "\r\n";
		//    strInfo += "UT:" + mNode.UT + "\r\n";
		//    return strInfo;
		//}

		//public void WriteOrbitFile(Orbit o, String FileName)
		//{

		//    KSP.IO.TextWriter tw = KSP.IO.TextWriter.CreateForType<KerbalAlarmClock>(FileName);
		//    tw.WriteLine("ClAppr:" + o.ClAppr);
		//    tw.WriteLine("ClEctr1:" + o.ClEctr1);
		//    tw.WriteLine("ClEctr2:" + o.ClEctr2);
		//    tw.WriteLine("closestEncounterBody:" + o.closestEncounterBody);
		//    tw.WriteLine("closestEncounterLevel:" + o.closestEncounterLevel);
		//    tw.WriteLine("closestEncounterPatch:" + o.closestEncounterPatch);
		//    tw.WriteLine("closestTgtApprUT:" + o.closestTgtApprUT);
		//    tw.WriteLine("CrAppr:" + o.CrAppr);

		//    tw.WriteLine("EndUT:" + o.EndUT);
		//    tw.WriteLine("epoch:" + o.epoch);
		//    tw.WriteLine("FEVp:" + o.FEVp);
		//    tw.WriteLine("FEVs:" + o.FEVs);
		//    tw.WriteLine("fromE:" + o.fromE);
		//    tw.WriteLine("fromV:" + o.fromV);
		//    tw.WriteLine("h:" + o.h);
		//    tw.WriteLine("inclination:" + o.inclination);
		//    tw.WriteLine("LAN:" + o.LAN);
		//    tw.WriteLine("mag:" + o.mag);
		//    tw.WriteLine("meanAnomaly:" + o.meanAnomaly);
		//    tw.WriteLine("meanAnomalyAtEpoch:" + o.meanAnomalyAtEpoch);
		//    tw.WriteLine("nearestTT:" + o.nearestTT);
		//    tw.WriteLine("nextPatch:" + o.nextPatch);
		//    tw.WriteLine("nextTT:" + o.nextTT);
		//    tw.WriteLine("nextPatch:" + o.nextPatch);
		//    tw.WriteLine("ObT:" + o.ObT);
		//    tw.WriteLine("ObTAtEpoch:" + o.ObTAtEpoch);
		//    tw.WriteLine("orbitalEnergy:" + o.orbitalEnergy);
		//    tw.WriteLine("orbitalSpeed:" + o.orbitalSpeed);
		//    tw.WriteLine("orbitPercent:" + o.orbitPercent);
		//    tw.WriteLine("patchEndTransition:" + o.patchEndTransition);
		//    tw.WriteLine("patchStartTransition:" + o.patchStartTransition);
		//    tw.WriteLine("period:" + o.period);
		//    tw.WriteLine("pos:" + o.pos);
		//    tw.WriteLine("previousPatch:" + o.previousPatch);
		//    tw.WriteLine("radius:" + o.radius);
		//    tw.WriteLine("referenceBody:" + o.referenceBody);
		//    tw.WriteLine("SEVp:" + o.SEVp);
		//    tw.WriteLine("SEVs:" + o.SEVs);
		//    tw.WriteLine("StartUT:" + o.StartUT);
		//    tw.WriteLine("timeToTransition1:" + o.timeToTransition1);
		//    tw.WriteLine("timeToTransition2:" + o.timeToTransition2);
		//    tw.WriteLine("toE:" + o.toE);
		//    tw.WriteLine("toV:" + o.toV);
		//    tw.WriteLine("trueAnomaly:" + o.trueAnomaly);
		//    tw.WriteLine("UTappr:" + o.UTappr);
		//    tw.WriteLine("UTsoi:" + o.UTsoi);
		//    tw.WriteLine("V:" + o.V);
		//    tw.WriteLine("vel:" + o.vel);
		//    tw.Close();
		//}
		
		
#endif
	}
}
