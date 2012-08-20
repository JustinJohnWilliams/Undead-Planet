using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Model
{
    public class DistanceCalculator : IDistanceCalculator
    {
        //DI DA
        public DistanceCalculator() { }

        double IDistanceCalculator.CalculateMiles(double currentLattitude, double currentLongitude, double nextLattitude, double nextLongitude)
        {
            var thisAsDistanceCalculator = this as IDistanceCalculator;

            return Math.Round(thisAsDistanceCalculator.DistanceBetweenPlaces(
                                    Radians(currentLattitude),
                                    Radians(currentLongitude),
                                    Radians(nextLattitude),
                                    Radians(nextLongitude)), 4);
        }


        UndeadEarth.Contract.Tuple<double, double> IDistanceCalculator.GetNorthernPoint(double centerLatitude, double centerLongitude, double radiusInMiles)
        {
            return GetLocation(centerLatitude, centerLongitude, radiusInMiles, 0);
        }

        UndeadEarth.Contract.Tuple<double, double> IDistanceCalculator.GetEasternPoint(double centerLatitude, double centerLongitude, double radiusInMiles)
        {
            return GetLocation(centerLatitude, centerLongitude, radiusInMiles, 90);
        }

        UndeadEarth.Contract.Tuple<double, double> IDistanceCalculator.GetSouthernPoint(double centerLatitude, double centerLongitude, double radiusInMiles)
        {
            return GetLocation(centerLatitude, centerLongitude, radiusInMiles, 180);
        }

        UndeadEarth.Contract.Tuple<double, double> IDistanceCalculator.GetWesternPoint(double centerLatitude, double centerLongitude, double radiusInMiles)
        {
            return GetLocation(centerLatitude, centerLongitude, radiusInMiles, 270);
        }

        private UndeadEarth.Contract.Tuple<double, double> GetLocation(double centerLatitude, double centerLongitude, double radiusInMiles, int degree)
        {
            double earthRadius = 3958.75587;
            double latitude = centerLatitude * Math.PI / 180;
            double longitude = centerLongitude * Math.PI / 180;
            double distanceRatio = radiusInMiles / earthRadius;
            double radian = degree * Math.PI / 180;

            UndeadEarth.Contract.Tuple<double, double> point = new UndeadEarth.Contract.Tuple<double, double>(0, 0);

            point.Item1 = Math.Asin(Math.Sin(latitude) * Math.Cos(distanceRatio) + Math.Cos(latitude) * Math.Sin(distanceRatio * Math.Cos(radian)));
            point.Item2 = (((longitude + Math.Atan2(Math.Sin(radian) * Math.Sin(distanceRatio) * Math.Cos(latitude), Math.Cos(distanceRatio) - Math.Sin(latitude) * Math.Sin(point.Item1))) * 180) / Math.PI);
            point.Item1 = (point.Item1 * 180) / Math.PI;

            return point;
        }

        const double Pi = 3.141592653589793;
        const double RadiusOfEarthInMiles = 3958.75587;

        /// <summary>
        /// Convert degrees to Radians
        /// </summary>
        /// <param name="x">Degrees</param>
        /// <returns>The equivalent in radians</returns>
        private double Radians(double x)
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
        double IDistanceCalculator.DistanceBetweenPlaces(
            double startLatitude,
            double startLongitude,
            double endLatitude,
            double endLongitude)
        {
            double longitudeDifference = endLongitude - startLongitude;
            double latitudeDifference = endLatitude - startLatitude;

            double x = Math.Sin(latitudeDifference / 2);
            x = x * Math.Sin(latitudeDifference / 2);

            double y = Math.Cos(startLatitude);
            y = y * Math.Cos(endLatitude);

            double z = Math.Sin(longitudeDifference / 2);
            z = z * Math.Sin(longitudeDifference / 2);

            double a = x + y * z;
            double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            angle = angle * RadiusOfEarthInMiles;

            return angle;
        }
    }
}
