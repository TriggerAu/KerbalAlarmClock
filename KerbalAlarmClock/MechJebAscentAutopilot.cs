using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalAlarmClock
{
    static class LaunchTiming
	{
        //From MechJeb2
        //Computes the time required for the given launch location to rotate under the target orbital plane. 
        //If the latitude is too high for the launch location to ever actually rotate under the target plane,
        //returns the time of closest approach to the target plane.
        //I have a wonderful proof of this formula which this comment is too short to contain.
        internal static double TimeToPlane(CelestialBody launchBody, double launchLatitude, double launchLongitude, Orbit target)
        {
            double inc = Math.Abs(Vector3d.Angle(target.SwappedOrbitNormal(), launchBody.angularVelocity));
            Vector3d b = Vector3d.Exclude(launchBody.angularVelocity, target.SwappedOrbitNormal()).normalized; // I don't understand the sign here, but this seems to work
            b *= launchBody.Radius * Math.Sin(Math.PI / 180 * launchLatitude) / Math.Tan(Math.PI / 180 * inc);
            Vector3d c = Vector3d.Cross(target.SwappedOrbitNormal(), launchBody.angularVelocity).normalized;
            double cMagnitudeSquared = Math.Pow(launchBody.Radius * Math.Cos(Math.PI / 180 * launchLatitude), 2) - b.sqrMagnitude;
            if (cMagnitudeSquared < 0) cMagnitudeSquared = 0;
            c *= Math.Sqrt(cMagnitudeSquared);
            Vector3d a1 = b + c;
            Vector3d a2 = b - c;

            Vector3d longitudeVector = launchBody.GetSurfaceNVector(0, launchLongitude);

            double angle1 = Math.Abs(Vector3d.Angle(longitudeVector, a1));
            if (Vector3d.Dot(Vector3d.Cross(longitudeVector, a1), launchBody.angularVelocity) < 0) angle1 = 360 - angle1;
            double angle2 = Math.Abs(Vector3d.Angle(longitudeVector, a2));
            if (Vector3d.Dot(Vector3d.Cross(longitudeVector, a2), launchBody.angularVelocity) < 0) angle2 = 360 - angle2;

            double angle = Math.Min(angle1, angle2);
            return (angle / 360) * launchBody.rotationPeriod;
        }
	} 
}
