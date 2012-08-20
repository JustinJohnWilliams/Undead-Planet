using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using UndeadEarth.Web.Models.Database;
using UndeadEarth.Web.Models;

namespace UndeadEarth.Web.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private Guid _userId = new Guid("AAAFCB18-3BBD-4842-A1D2-8D19D68EF52E");
        private int _userSpeed = 300; //mph
        private int _userZph = 25; //zombies that will get killed per hunt

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// TODO: Put in right place...mocks dont show me where this would go....The method is called by the javascript/UI every 3 seconds.  Before returning the
        /// User, the User's location is calcuated and then updated.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetUser()
        {
            UpdateUserLocation(_userId); //this is all the logic to determine the new location of the user getting the current time
            //...prolly not a good idea to put this in a "get" method since it's making a change
            //to data, but i didn't want the UI posting to a URL to update the user's position every so often.


            //all this needs to be abstracted.....
            UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(ConfigurationManager.ConnectionStrings["UndeadEarthConnectionString"].ConnectionString);
            UserDto userDto = undeadEarthDataContext.UserDtos.Single(s => s.Id == _userId);
            HotZoneDto hotZoneDto = undeadEarthDataContext.HotZoneDtos.SingleOrDefault(s => s.Id == userDto.LocationId);
            HotZoneDto nextHotZoneDto = undeadEarthDataContext.HotZoneDtos.SingleOrDefault(s => s.Id == (userDto.NextLocationId ?? Guid.Empty));
            InfoNodeDto nextInfoNodeDto = undeadEarthDataContext.InfoNodeDtos.SingleOrDefault(s => s.Id == (userDto.NextLocationId ?? Guid.Empty));

            UserNode userNode = new UserNode();
            userNode.Id = userDto.Id;
            userNode.Name = userDto.DisplayName;
            userNode.Latitude = userDto.TempLatitude ?? userDto.Latitude;
            userNode.Longitude = userDto.TempLongitude ?? userDto.Longitude;
            userNode.CanHunt = false;
            userNode.IsMoving = (userDto.MoveEndTime != null);
            userNode.HotZone = null;

            if (userNode.IsMoving)
            {
                //this is how you get the miles between two lat long positions...
                double milesAway = GetMiles(userDto.TempLatitude ?? userDto.Latitude,
                                    userDto.TempLongitude ?? userDto.Longitude,
                                    userDto.NextLatitude ?? userDto.Latitude,
                                    userDto.NextLongitude ?? userDto.Longitude);

                userNode.MinutesLeftInMove = GetMinutes(milesAway, _userSpeed);
                userNode.CurrentSpeed = _userSpeed;
                userNode.NextDestinationLatitude = userDto.NextLatitude ?? userDto.Latitude;
                userNode.NextDestinationLongitude = userDto.NextLongitude ?? userDto.Longitude;

                if (nextHotZoneDto != null)
                {
                    userNode.NextNodeName = nextHotZoneDto.Name;
                }

                if (nextInfoNodeDto != null)
                {
                    userNode.NextNodeName = nextInfoNodeDto.Name;
                }
            }

            //if the user can hunt, get hotzone information
            if (hotZoneDto != null && userDto.NextLocationId == null)
            {
                userNode.CanHunt = true;
                UserHotZoneProgressDto userHotZoneProgressDto = undeadEarthDataContext.UserHotZoneProgressDtos.Single(s => s.UserId == _userId && s.HotZoneId == hotZoneDto.Id);
                CalculateZombiesLeft(userHotZoneProgressDto);
                undeadEarthDataContext.SubmitChanges();
                userNode.HotZone = GetHotZoneNodeGivenDtos(userDto, hotZoneDto, userHotZoneProgressDto);
            }

            return Json(userNode);
        }

        /// <summary>
        /// TODO: Put in right place...mocks dont show me where this would go....This method gets all the hotzones on UndeadEarth...this probably 
        /// needs to change to take in the userid so that can be considered when retruning hotzones (there is no need to return all the hotzones...probably only 
        /// the ones in a given latitude/longitude radius.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetHotZones()
        {
            //all this needs to be abstracted.....
            UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(ConfigurationManager.ConnectionStrings["UndeadEarthConnectionString"].ConnectionString);
            UserDto currentUser = undeadEarthDataContext.UserDtos.Single(s => s.Id == _userId);

            List<HotZoneNode> nodes =
                (from hotZone in undeadEarthDataContext.HotZoneDtos
                 join userProgress in undeadEarthDataContext.UserHotZoneProgressDtos
                 on hotZone.Id equals userProgress.HotZoneId
                 where userProgress.IsDestroyed == false && userProgress.UserId == _userId
                 select new HotZoneNode
                 {
                     Id = hotZone.Id,
                     Latitude = hotZone.Latitude,
                     Longitude = hotZone.Longitude,
                     Name = hotZone.Name,
                     MaxZombies = userProgress.MaxZombies,
                     MinutesToNextRegen = Convert.ToInt32((userProgress.LastRegen.AddMinutes((double)userProgress.RegenMinuteTicks) - DateTime.Now).TotalMinutes),
                     RegenRate = userProgress.RegenZombieRate,
                     ZombiesLeft = userProgress.ZombiesLeft,
                     IsDestroyed = userProgress.IsDestroyed,
                     MinutesBetweenRegen = userProgress.RegenMinuteTicks
                 }).ToList();

            foreach (HotZoneNode hotZoneNode in nodes)
            {
                hotZoneNode.MilesAway = GetMiles(currentUser.TempLatitude ?? currentUser.Latitude,
                                            currentUser.TempLongitude ?? currentUser.Longitude,
                                            hotZoneNode.Latitude,
                                            hotZoneNode.Longitude);

                hotZoneNode.MinutesAway = GetMinutes(hotZoneNode.MilesAway, _userSpeed);

                if (hotZoneNode.MinutesToNextRegen < 0)
                {
                    hotZoneNode.MinutesToNextRegen = hotZoneNode.MinutesBetweenRegen;
                }
            }

            return Json(nodes);
        }

        /// <summary>
        /// TODO: Put in right place...mocks dont show me where this would go....Gets information with a particular hotzone.  
        /// This method gets called when a user hovers over a hotzone.  The updated time away from hotzone is updated based on users location.
        /// </summary>
        /// <param name="hotZoneId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetHotZone(Guid hotZoneId)
        {
            //all this needs to be abstracted.....
            UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(ConfigurationManager.ConnectionStrings["UndeadEarthConnectionString"].ConnectionString);

            UserDto currentUser = undeadEarthDataContext.UserDtos.Single(s => s.Id == _userId);
            HotZoneDto hotZoneDto = undeadEarthDataContext.HotZoneDtos.Single(s => s.Id == hotZoneId);
            UserHotZoneProgressDto userHotZoneProgressDto = undeadEarthDataContext.UserHotZoneProgressDtos.Single(s => s.UserId == _userId && s.HotZoneId == hotZoneId);

            //determine if zombie count needs to be increased
            CalculateZombiesLeft(userHotZoneProgressDto);
            undeadEarthDataContext.SubmitChanges();

            HotZoneNode hotZoneNode = GetHotZoneNodeGivenDtos(currentUser, hotZoneDto, userHotZoneProgressDto);
            return Json(hotZoneNode);
        }

        private void CalculateZombiesLeft(UserHotZoneProgressDto userHotZoneProgressDto)
        {
            int result = userHotZoneProgressDto.ZombiesLeft;
            DateTime nextRegenDate = userHotZoneProgressDto.LastRegen.AddMinutes((double)userHotZoneProgressDto.RegenMinuteTicks);
            DateTime now = DateTime.Now;
            if (now > nextRegenDate)
            {
                int multipler = (int)Math.Floor((now - nextRegenDate).TotalMinutes / (double)userHotZoneProgressDto.RegenMinuteTicks);
                int zombiesToAdd = multipler * userHotZoneProgressDto.RegenZombieRate;
                result = result + zombiesToAdd;
                if (result > userHotZoneProgressDto.MaxZombies)
                {
                    result = userHotZoneProgressDto.MaxZombies;
                }

                userHotZoneProgressDto.LastRegen = now;
                userHotZoneProgressDto.ZombiesLeft = result;
            }
        }

        /// <summary>
        /// TODO: Put in right place...mocks dont show me where this would go....Gets city information for undead earth.  I consider info nodes as zones.  Maybe we can associate
        /// an info node with a set of hotzones...so before a player can hunt in a city (an load hotzones in an area), you must FIRST move the info node.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetInfoNodes()
        {
            //all this needs to be abstracted.....
            UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(ConfigurationManager.ConnectionStrings["UndeadEarthConnectionString"].ConnectionString);
            List<DistanceNode> nodes = undeadEarthDataContext.InfoNodeDtos.Select(n => new DistanceNode
            {
                Id = n.Id,
                Latitude = n.Latitude,
                Longitude = n.Longitude,
                Name = n.Name
            }).ToList();

            UserDto currentUser = undeadEarthDataContext.UserDtos.Single(s => s.Id == _userId);

            foreach (DistanceNode infoNode in nodes)
            {
                infoNode.MilesAway = GetMiles(currentUser.TempLatitude ?? currentUser.Latitude,
                                         currentUser.TempLongitude ?? currentUser.Longitude,
                                         infoNode.Latitude,
                                         infoNode.Longitude);

                infoNode.MinutesAway = GetMinutes(infoNode.MilesAway, _userSpeed);
            }

            return Json(nodes);
        }

        /// <summary>
        /// TODO: Put in right place...mocks dont show me where this would go....Gets city information for a particular info id.  Method gets called when user hovers over an info node.
        /// </summary>
        /// <param name="infoNodeId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetInfoNode(Guid infoNodeId)
        {
            UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(ConfigurationManager.ConnectionStrings["UndeadEarthConnectionString"].ConnectionString);
            //all this needs to be abstracted.....
            UserDto currentUser = undeadEarthDataContext.UserDtos.Single(s => s.Id == _userId);
            InfoNodeDto infoNodeDto = undeadEarthDataContext.InfoNodeDtos.Single(s => s.Id == infoNodeId);
            DistanceNode infoNode = new DistanceNode
            {
                Id = infoNodeDto.Id,
                Name = infoNodeDto.Name,
                Latitude = infoNodeDto.Latitude,
                Longitude = infoNodeDto.Longitude
            };

            infoNode.MilesAway = GetMiles(currentUser.TempLatitude ?? currentUser.Latitude,
                                     currentUser.TempLongitude ?? currentUser.Longitude,
                                     infoNodeDto.Latitude,
                                     infoNodeDto.Longitude);

            infoNode.MinutesAway = GetMinutes(infoNode.MilesAway, _userSpeed);

            return Json(infoNode);
        }

        /// <summary>
        /// TODO: Put in right place...mocks dont show me where this would go....Moves a user to an info node.
        /// </summary>
        /// <param name="infoNodeId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MoveToInfoNode(Guid infoNodeId)
        {
            UpdateUserLocation(_userId);

            UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(ConfigurationManager.ConnectionStrings["UndeadEarthConnectionString"].ConnectionString);
            InfoNodeDto infoNodeDto = undeadEarthDataContext.InfoNodeDtos.Single(h => h.Id == infoNodeId);
            UserDto userDto = undeadEarthDataContext.UserDtos.Single(s => s.Id == _userId);

            userDto.Latitude = userDto.TempLatitude ?? userDto.Latitude;
            userDto.Longitude = userDto.TempLongitude ?? userDto.Longitude;
            userDto.NextLocationId = infoNodeDto.Id;
            userDto.NextLatitude = infoNodeDto.Latitude;
            userDto.NextLongitude = infoNodeDto.Longitude;
            userDto.MoveStartTime = DateTime.Now;

            double milesAway = GetMiles(userDto.Latitude, userDto.Longitude, infoNodeDto.Latitude, infoNodeDto.Longitude);
            double minutesAway = GetMinutes(milesAway, _userSpeed);

            userDto.MoveEndTime = DateTime.Now.AddMinutes(minutesAway);
            undeadEarthDataContext.SubmitChanges();

            return new EmptyResult();
        }

        /// <summary>
        /// TODO: Put in right place...mocks dont show me where this would go....Moves a user to a hot zone.
        /// </summary>
        /// <param name="infoNodeId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MoveToHotZone(Guid hotZoneId)
        {
            UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(ConfigurationManager.ConnectionStrings["UndeadEarthConnectionString"].ConnectionString);
            HotZoneDto hotZoneDto = undeadEarthDataContext.HotZoneDtos.Single(h => h.Id == hotZoneId);
            UserDto userDto = undeadEarthDataContext.UserDtos.Single(s => s.Id == _userId);

            userDto.Latitude = userDto.TempLatitude ?? userDto.Latitude;
            userDto.Longitude = userDto.TempLongitude ?? userDto.Longitude;

            userDto.NextLocationId = hotZoneDto.Id;
            userDto.NextLatitude = hotZoneDto.Latitude;
            userDto.NextLongitude = hotZoneDto.Longitude;
            userDto.MoveStartTime = DateTime.Now;

            double milesAway = GetMiles(userDto.Latitude,
                                   userDto.Longitude,
                                   hotZoneDto.Latitude,
                                   hotZoneDto.Longitude);

            double minutesAway = GetMinutes(milesAway, _userSpeed);

            userDto.MoveEndTime = DateTime.Now.AddMinutes(minutesAway);
            undeadEarthDataContext.SubmitChanges();

            return new EmptyResult();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Hunt()
        {
            //ensure that the user is at the hotzone
            UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(ConfigurationManager.ConnectionStrings["UndeadEarthConnectionString"].ConnectionString);
            UserDto userDto = undeadEarthDataContext.UserDtos.Single(s => s.Id == _userId);
            Guid possibleHotZoneId = userDto.LocationId;

            HotZoneDto hotZoneDto = undeadEarthDataContext.HotZoneDtos.SingleOrDefault(s => s.Id == userDto.LocationId);

            //ensure that the current node the user is on is indeed a hotzone node, and ensure that the user isn't moving
            if (hotZoneDto != null && userDto.NextLocationId == null)
            {
                //perform hunt given users zph
                UserHotZoneProgressDto userHotZoneProgressDto = undeadEarthDataContext.UserHotZoneProgressDtos.Single(s => s.UserId == _userId && s.HotZoneId == hotZoneDto.Id);
                userHotZoneProgressDto.ZombiesLeft = userHotZoneProgressDto.ZombiesLeft - _userZph;
                if (userHotZoneProgressDto.ZombiesLeft <= 0)
                {
                    userHotZoneProgressDto.ZombiesLeft = 0;
                    userHotZoneProgressDto.IsDestroyed = true;
                }
                userHotZoneProgressDto.LastHuntDate = DateTime.Now;
                undeadEarthDataContext.SubmitChanges();
            }

            return new EmptyResult();
        }

        private HotZoneNode GetHotZoneNodeGivenDtos(UserDto currentUser, HotZoneDto hotZoneDto, UserHotZoneProgressDto userHotZoneProgressDto)
        {
            HotZoneNode hotZoneNode = new HotZoneNode
                        {
                            Id = hotZoneDto.Id,
                            Name = hotZoneDto.Name,
                            Latitude = hotZoneDto.Latitude,
                            Longitude = hotZoneDto.Longitude,
                            MaxZombies = userHotZoneProgressDto.MaxZombies,
                            MinutesToNextRegen = Convert.ToInt32((userHotZoneProgressDto.LastRegen.AddMinutes((double)userHotZoneProgressDto.RegenMinuteTicks) - DateTime.Now).TotalMinutes),
                            RegenRate = userHotZoneProgressDto.RegenZombieRate,
                            ZombiesLeft = userHotZoneProgressDto.ZombiesLeft,
                            IsDestroyed = userHotZoneProgressDto.IsDestroyed,
                            MinutesBetweenRegen = userHotZoneProgressDto.RegenMinuteTicks
                        };

            hotZoneNode.MilesAway = GetMiles(currentUser.TempLatitude ?? currentUser.Latitude,
                                        currentUser.TempLongitude ?? currentUser.Longitude,
                                        hotZoneNode.Latitude,
                                        hotZoneNode.Longitude);

            hotZoneNode.MinutesAway = GetMinutes(hotZoneNode.MilesAway, _userSpeed);
            return hotZoneNode;
        }

        private double GetMinutes(double milesAway, int speed)
        {
            return Math.Round(Convert.ToDouble(Convert.ToDouble(milesAway) / speed) * 60.0, 4);
        }

        private double GetMiles(decimal fromLatitude, decimal fromLongitude, decimal toLatitude, decimal toLongitude)
        {
            return Math.Round(DistanceAlgorithm.DistanceBetweenPlaces(
                                    DistanceAlgorithm.Radians(Convert.ToDouble(fromLongitude)),
                                    DistanceAlgorithm.Radians(Convert.ToDouble(fromLatitude)),
                                    DistanceAlgorithm.Radians(Convert.ToDouble(toLongitude)),
                                    DistanceAlgorithm.Radians(Convert.ToDouble(toLatitude))), 4);
        }

        /// <summary>
        /// This method handles all the logic for updating a users location based on the current time.
        /// </summary>
        /// <param name="userId"></param>
        private void UpdateUserLocation(Guid userId)
        {
            UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(ConfigurationManager.ConnectionStrings["UndeadEarthConnectionString"].ConnectionString);
            UserDto userDto = undeadEarthDataContext.UserDtos.Single(s => s.Id == _userId);

            if (userDto.MoveEndTime != null)
            {
                DateTime currentTime = DateTime.Now;
                if (userDto.MoveEndTime <= currentTime) //if the end time is less then the current time, then assume that the person has moved to their location...reset all values DB values....need to abstract
                {
                    userDto.LocationId = userDto.NextLocationId ?? userDto.LocationId;
                    userDto.Latitude = userDto.NextLatitude ?? userDto.Latitude;
                    userDto.Longitude = userDto.NextLongitude ?? userDto.Latitude;
                    userDto.NextLocationId = null;
                    userDto.NextLatitude = null;
                    userDto.NextLongitude = null;
                    userDto.MoveEndTime = null;
                    userDto.MoveStartTime = null;
                    userDto.TempLatitude = null;
                    userDto.TempLongitude = null;
                    undeadEarthDataContext.SubmitChanges();
                }
                else if (userDto.MoveEndTime > currentTime) //if end time is less than current time, determine where the user is based on current time and end time and lat long positions
                {
                    DateTime startTime = userDto.MoveStartTime ?? DateTime.Today;
                    DateTime endTime = userDto.MoveEndTime ?? DateTime.Today;
                    double minuteDifferenceFromEndTime = Math.Abs((startTime - endTime).TotalMinutes); //visualizing 100 minutes
                    double minutesToGo = Math.Abs((currentTime - endTime).TotalMinutes); //visualizing 30 minutes
                    double percentageComplete = 1;

                    if (minuteDifferenceFromEndTime != 0)
                    {
                        percentageComplete = Convert.ToDouble(minuteDifferenceFromEndTime - minutesToGo) / Convert.ToDouble(minuteDifferenceFromEndTime); //answer should be 70%
                    }

                    if (percentageComplete >= 1) // if it's 100 complete then assume that user has completed the move
                    {
                        userDto.LocationId = userDto.NextLocationId ?? userDto.LocationId;
                        userDto.Latitude = userDto.NextLatitude ?? userDto.Latitude;
                        userDto.Longitude = userDto.NextLongitude ?? userDto.Latitude;
                        userDto.NextLocationId = null;
                        userDto.NextLatitude = null;
                        userDto.NextLongitude = null;
                        userDto.MoveEndTime = null;
                        userDto.MoveStartTime = null;
                        userDto.TempLatitude = null;
                        userDto.TempLongitude = null;
                        undeadEarthDataContext.SubmitChanges();
                    }
                    else
                    {
                        decimal difference = (userDto.NextLatitude ?? userDto.Latitude) - userDto.Latitude;
                        decimal newLatitude = userDto.Latitude + (difference * Convert.ToDecimal(percentageComplete));

                        difference = (userDto.NextLongitude ?? userDto.Longitude) - userDto.Longitude;
                        decimal newLongitude = userDto.Longitude + (difference * Convert.ToDecimal(percentageComplete));

                        //update the users position on every poll
                        userDto.TempLatitude = newLatitude;
                        userDto.TempLongitude = newLongitude;
                        undeadEarthDataContext.SubmitChanges();
                    }
                }
            }
        }
    }
}
