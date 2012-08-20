using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UndeadEarth.Web.Models
{
    public class DistanceAlgorithm
    {
        const double Pi = 3.141592653589793;
        const double RadiusOfEarthInMiles = 3958.75587;

        /// <summary>
        /// This class cannot be instantiated.
        /// </summary>
        private DistanceAlgorithm() { }

        /// <summary>
        /// Convert degrees to Radians
        /// </summary>
        /// <param name="x">Degrees</param>
        /// <returns>The equivalent in radians</returns>
        public static double Radians(double x)
        {
            return x * Pi / 180;
        }

        /// <summary>
        /// Calculate the distance between two places.
        /// </summary>
        /// <param name="startLongitude"></param>
        /// <param name="startLatitude"></param>
        /// <param name="endLongitude"></param>
        /// <param name="endLatitude"></param>
        /// <returns></returns>
        public static double DistanceBetweenPlaces(
            double startLongitude,
            double startLatitude,
            double endLongitude,
            double endLatitude)
        {
            double longitudeDifference = endLongitude - startLongitude;
            double latitudeDifference = endLatitude - startLatitude;

            double a = (Math.Sin(latitudeDifference / 2) * Math.Sin(latitudeDifference / 2)) + Math.Cos(startLatitude) * Math.Cos(endLatitude) * (Math.Sin(longitudeDifference / 2) * Math.Sin(longitudeDifference / 2));
            double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return angle * RadiusOfEarthInMiles;
        }
    }
}
