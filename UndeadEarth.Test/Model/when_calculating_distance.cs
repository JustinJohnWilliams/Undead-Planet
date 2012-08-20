using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using UndeadEarth.Model;

namespace UndeadEarth.Test.Model
{
    /// <summary>
    /// Summary description for when_calculating_distance
    /// </summary>
    [TestClass]
    public class when_calculating_distance
    {
        private IDistanceCalculator _distanceCalculator;
        public when_calculating_distance()
        {
            _distanceCalculator = new DistanceCalculator();
        }

        [TestMethod]
        public void should_calculate_miles_between_two_points()
        {
            double miles = _distanceCalculator.CalculateMiles(32.73, 96.97, 40.77, 73.98);
            Assert.AreEqual(1383.3047, miles);
        }

        [TestMethod]
        public void should_calculate_northern_point()
        {
            UndeadEarth.Contract.Tuple<double, double> northPoint = _distanceCalculator.GetNorthernPoint(32.73, 96.97, 5);
            Assert.AreEqual(32.8024, Math.Round(northPoint.Item1, 4));
            Assert.AreEqual(96.9700, Math.Round(northPoint.Item2, 4));
        }

        [TestMethod]
        public void should_calculate_southern_point()
        {
            UndeadEarth.Contract.Tuple<double, double> southernPoint = _distanceCalculator.GetSouthernPoint(32.73, 96.97, 5);
            Assert.AreEqual(32.6576, Math.Round(southernPoint.Item1, 4));
            Assert.AreEqual(96.9700, Math.Round(southernPoint.Item2, 4));
        }

        [TestMethod]
        public void should_calculate_eastern_point()
        {
            UndeadEarth.Contract.Tuple<double, double> easternPoint = _distanceCalculator.GetEasternPoint(32.73, 96.97, 5);
            Assert.AreEqual(32.7300, Math.Round(easternPoint.Item1, 4));
            Assert.AreEqual(97.0560, Math.Round(easternPoint.Item2, 4));
        }

        [TestMethod]
        public void should_calculate_western_point()
        {
            UndeadEarth.Contract.Tuple<double, double> westernPoint = _distanceCalculator.GetWesternPoint(32.73, 96.97, 5);
            Assert.AreEqual(32.7300, Math.Round(westernPoint.Item1, 4));
            Assert.AreEqual(96.8840, Math.Round(westernPoint.Item2, 4));
        }
    }
}
