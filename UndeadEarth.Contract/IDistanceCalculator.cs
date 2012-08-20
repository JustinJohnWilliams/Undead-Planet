using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IDistanceCalculator
    {
        /// <summary>
        /// Calculates miles between two points on the earth given two sets of lattitude/longitude
        /// </summary>
        /// <param name="currentLattitude"></param>
        /// <param name="currentLongitude"></param>
        /// <param name="nextLattitude"></param>
        /// <param name="nextLongitude"></param>
        /// <returns></returns>
        double CalculateMiles(double currentLattitude, double currentLongitude, double nextLattitude, double nextLongitude);

        /// <summary>
        /// Returns Northern most point from a given center
        /// </summary>
        /// <param name="centerLatitude"></param>
        /// <param name="centerLongitude"></param>
        /// <param name="radiusInMiles"></param>
        /// <returns>Latitude and Longitude</returns>
        Tuple<double, double> GetNorthernPoint(double centerLatitude, double centerLongitude, double radiusInMiles);

        /// <summary>
        /// Returns Eastern most point from a given center
        /// </summary>
        /// <param name="centerLatitude"></param>
        /// <param name="centerLongitude"></param>
        /// <param name="radiusInMiles"></param>
        /// <returns>Latitude and Longitude</returns>
        Tuple<double, double> GetEasternPoint(double centerLatitude, double centerLongitude, double radiusInMiles);

        /// <summary>
        /// Returns Southern most point from a given center
        /// </summary>
        /// <param name="centerLatitude"></param>
        /// <param name="centerLongitude"></param>
        /// <param name="radiusInMiles"></param>
        /// <returns>Latitude and Longitude</returns>
        Tuple<double, double> GetSouthernPoint(double centerLatitude, double centerLongitude, double radiusInMiles);

        /// <summary>
        /// Returns Western most point from a given center
        /// </summary>
        /// <param name="centerLatitude"></param>
        /// <param name="centerLongitude"></param>
        /// <param name="radiusInMiles"></param>
        /// <returns>Latitude and Longitude</returns>
        Tuple<double, double> GetWesternPoint(double centerLatitude, double centerLongitude, double radiusInMiles);

        /// <summary>
        /// Gets the distance between to points.
        /// </summary>
        double DistanceBetweenPlaces(double startLatitudeInRadians, double startLongitudeInRadians, double endLatitudeInRadians, double endLongitudeInRadians);
    }
}
