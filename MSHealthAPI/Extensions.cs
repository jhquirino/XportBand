//-----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace MSHealthAPI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Extensions for MSHealthAPI objects.
    /// </summary>
    public static class Extensions
    {

        #region MSHealthActivity Extension Methods

        /// <summary>
        /// Exports <see cref="MSHealthAPI.MSHealthActivity"/> instance to GPX <see cref="System.Xml.Linq.XDocument"/>.
        /// </summary>
        /// <param name="activity"><see cref="MSHealthAPI.MSHealthActivity"/> instance to export.</param>
        /// <param name="creator">Creator name for activity.</param>
        /// <param name="name">Name for activity.</param>
        /// <param name="ignoreEmptyMapPoints">Flag to not throw exception if <paramref name="activity"/> has no MapPoints.</param>
        /// <returns><see cref="System.Xml.Linq.XDocument"/> with GPX information.</returns>
        public static XDocument ToGPX(this MSHealthActivity activity, string creator = null, string name = null, bool ignoreEmptyMapPoints = false)
        {
            XDocument loDocument = null;
            XElement loRoot = null;
            XNamespace loNamespaceDefault = "http://www.topografix.com/GPX/1/1";
            XNamespace loNamespaceXsi = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace loNamespaceGpxTpx = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1";
            XNamespace loNamespaceGpxx = "http://www.garmin.com/xmlschemas/GpxExtensions/v3";
            XNamespace loNamespaceSchema = "http://www.topografix.com/GPX/1/1 " +
                "http://www.topografix.com/GPX/1/1/gpx.xsd " +
                "http://www.garmin.com/xmlschemas/GpxExtensions/v3 " +
                "http://www.garmin.com/xmlschemas/GpxExtensionsv3.xsd " +
                "http://www.garmin.com/xmlschemas/TrackPointExtension/v1 " +
                "http://www.garmin.com/xmlschemas/TrackPointExtensionv1.xsd";
            XElement loMetadata = null;
            XElement loTrack = null;
            XElement loSegment = null;
            XElement loPoint = null;
            string lsCreator = creator;
            string lsName = name;
            DateTime ldtTime = DateTime.Now;
            // Validate MSHealthActivity instance
            if (activity != null)
            {
                // Check for MapPoints flag
                if (!ignoreEmptyMapPoints)
                {
                    // Check activity MapPoints
                    if (activity.MapPoints != null &&
                        activity.MapPoints.Any())
                    {
                        // Get valid MapPoints (with Location Latitude and Longitude)
                        IEnumerable<MSHealthMapPoint> loMPs = activity.MapPoints.Where(loMP => loMP.Location != null &&
                                                                                       loMP.Location.Latitude != null &&
                                                                                       loMP.Location.Longitude != null);
                        // No valid MapPoints... throw exception
                        if (loMPs == null ||
                            !loMPs.Any())
                            throw new ArgumentNullException("MapPoints");
                    }
                    else
                    {
                        // No MapPoints... throw exception
                        throw new ArgumentNullException("MapPoints");
                    }
                }
                // Validate Activity Properties (Start Time, Creator, Name)
                if (activity.StartTime != null &&
                    activity.StartTime.HasValue)
                    ldtTime = activity.StartTime.Value;
                if (string.IsNullOrEmpty(lsCreator))
                    lsCreator = "MSHealthAPI";
                if (string.IsNullOrEmpty(lsName))
                    lsName = string.Format("{0} {1:yyyy-MM-dd HH:mm:ss}", activity.Type, ldtTime);
                // Initialize GPX (XML Document), root with namespaces
                loRoot = new XElement(loNamespaceDefault + "gpx",
                                      new XAttribute("xmlns", loNamespaceDefault.NamespaceName),
                                      new XAttribute(XNamespace.Xmlns + "xsi", loNamespaceXsi.NamespaceName),
                                      new XAttribute(XNamespace.Xmlns + "gpxtpx", loNamespaceGpxTpx.NamespaceName),
                                      new XAttribute(XNamespace.Xmlns + "gpxx", loNamespaceGpxx.NamespaceName),
                                      new XAttribute(loNamespaceXsi + "schemaLocation", loNamespaceSchema.NamespaceName),
                                      new XAttribute("creator", lsCreator),
                                      new XAttribute("version", "1.1"));
                // Set GPX Metadata information
                loMetadata = new XElement(loNamespaceDefault + "metadata",
                                          new XElement(loNamespaceDefault + "name", lsName),
                                          new XElement(loNamespaceDefault + "time", string.Format("{0:O}", ldtTime.ToUniversalTime())));
                loRoot.Add(loMetadata);
                // Validate Activity MapPoints
                if (activity.MapPoints != null &&
                    activity.MapPoints.Any())
                {
                    // Initialize GPX Track/Segment node
                    loTrack = new XElement(loNamespaceDefault + "trk");
                    loSegment = new XElement(loNamespaceDefault + "trkseg");
                    List<MSHealthMapPoint> loMapPoints = new List<MSHealthMapPoint>();
                    if (ignoreEmptyMapPoints)
                        loMapPoints = activity.MapPoints.OrderBy(loMP => loMP.Ordinal).ToList();
                    else
                        loMapPoints = activity.MapPoints.Where(loMP => loMP.Location != null &&
                                                               loMP.Location.Latitude != null &&
                                                               loMP.Location.Longitude != null)
                                                        .OrderBy(loMP => loMP.Ordinal).ToList();
                    // Process Activity MapPoints
                    foreach (MSHealthMapPoint loMapPoint in loMapPoints)
                    {
                        // Append GPX TrackPoint
                        loPoint = new XElement(loNamespaceDefault + "trkpt");
                        if (loMapPoint.Location != null)
                        {
                            // TrackPoint Longitude
                            if (loMapPoint.Location.Latitude != null)
                                loPoint.Add(new XAttribute("lon", ((double)loMapPoint.Location.Longitude / MSHealthGPSPoint.LONGITUDE_FACTOR)));
                            // TrackPoint Latitude
                            if (loMapPoint.Location.Latitude != null)
                                loPoint.Add(new XAttribute("lat", ((double)loMapPoint.Location.Latitude / MSHealthGPSPoint.LATITUDE_FACTOR)));
                            // TrackPoint Elevation
                            if (loMapPoint.Location.ElevationFromMeanSeaLevel != null)
                                loPoint.Add(new XElement(loNamespaceDefault + "ele", (double)loMapPoint.Location.ElevationFromMeanSeaLevel / MSHealthGPSPoint.ELEVATION_FACTOR));
                        }
                        // TrackPoint Time
                        if (loMapPoint.SecondsSinceStart != null)
                        {
                            loPoint.Add(new XElement(loNamespaceDefault + "time", ldtTime.AddSeconds(loMapPoint.SecondsSinceStart.Value).ToUniversalTime()));
                        }
                        else
                        {
                            loPoint.Add(new XElement(loNamespaceDefault + "time", ldtTime.ToUniversalTime()));
                        }
                        // TrackPoint Extension HeartRate
                        if (loMapPoint.HeartRate != null)
                        {
                            loPoint.Add(new XElement(loNamespaceDefault + "extensions",
                                                     new XElement(loNamespaceGpxTpx + "TrackPointExtension",
                                                                  new XElement(loNamespaceGpxTpx + "hr", loMapPoint.HeartRate.Value))));
                        }
                        loSegment.Add(loPoint);
                    }
                    loTrack.Add(loSegment);
                    loRoot.Add(loTrack);
                }
                loDocument = new XDocument(new XDeclaration("1.0", "utf-8", null), loRoot);
            }
            // Return GPX (XML Document)
            return loDocument;
        }

        /// <summary>
        /// Exports <see cref="MSHealthAPI.MSHealthActivity"/> instance to TCX <see cref="System.Xml.Linq.XDocument"/>.
        /// </summary>
        /// <param name="activity"><see cref="MSHealthAPI.MSHealthActivity"/> instance to export.</param>
        /// <param name="creator">Creator name for activity.</param>
        /// <param name="ignoreEmptyMapPoints">Flag to not throw exception if <paramref name="activity"/> has no MapPoints.</param>
        /// <returns><see cref="System.Xml.Linq.XDocument"/> with TCX information.</returns>
        public static XDocument ToTCX(this MSHealthActivity activity, string creator = null, bool ignoreEmptyMapPoints = false)
        {
            XDocument loDocument = null;
            XElement loRoot = null;
            XNamespace loNamespaceDefault = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2";
            XNamespace loNamespaceXsi = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace loNamespaceSchema = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2 " +
                "http://www.garmin.com/xmlschemas/TrainingCenterDatabasev2.xsd";
            XNamespace loNamespace2 = "http://www.garmin.com/xmlschemas/UserProfile/v2";
            XNamespace loNamespace3 = "http://www.garmin.com/xmlschemas/ActivityExtension/v2";
            XNamespace loNamespace4 = "http://www.garmin.com/xmlschemas/ProfileExtension/v1";
            XNamespace loNamespace5 = "http://www.garmin.com/xmlschemas/ActivityGoals/v1";
            XElement loActivities = null;
            XElement loActivity = null;
            XElement loLap = null;
            XElement loTrack = null;
            XElement loTrackpoint = null;
            string lsCreator = creator;
            DateTime ldtTime = DateTime.Now;
            double ldDistanceInMeters = 0;
            // Validate MSHealthActivity instance
            if (activity != null)
            {
                // Check for MapPoints flag
                if (!ignoreEmptyMapPoints)
                {
                    // Check activity MapPoints
                    if (activity.MapPoints != null &&
                        activity.MapPoints.Any())
                    {
                        // Get valid MapPoints (with Location Latitude and Longitude)
                        IEnumerable<MSHealthMapPoint> loMPs = activity.MapPoints.Where(loMP => loMP.Location != null &&
                                                                                       loMP.Location.Latitude != null &&
                                                                                       loMP.Location.Longitude != null);
                        // No valid MapPoints... throw exception
                        if (loMPs == null ||
                            !loMPs.Any())
                            throw new ArgumentNullException("MapPoints");
                    }
                    else
                    {
                        // No MapPoints... throw exception
                        throw new ArgumentNullException("MapPoints");
                    }
                }
                // Validate Activity Properties (Start Time, Creator, Name)
                if (activity.StartTime != null &&
                    activity.StartTime.HasValue)
                    ldtTime = activity.StartTime.Value;
                if (string.IsNullOrEmpty(lsCreator))
                    lsCreator = "MSHealthAPI";
                // Initialize TCX (XML Document), root with namespaces
                loRoot = new XElement(loNamespaceDefault + "TrainingCenterDatabase",
                                      new XAttribute("xmlns", loNamespaceDefault.NamespaceName),
                                      new XAttribute(XNamespace.Xmlns + "xsi", loNamespaceXsi.NamespaceName),
                                      new XAttribute(XNamespace.Xmlns + "ns2", loNamespace2.NamespaceName),
                                      new XAttribute(XNamespace.Xmlns + "ns3", loNamespace3.NamespaceName),
                                      new XAttribute(XNamespace.Xmlns + "ns4", loNamespace4.NamespaceName),
                                      new XAttribute(XNamespace.Xmlns + "ns5", loNamespace5.NamespaceName),
                                      new XAttribute(loNamespaceXsi + "schemaLocation", loNamespaceSchema.NamespaceName),
                                      new XAttribute("creator", lsCreator),
                                      new XAttribute("version", "1.0"));
                // Lap (summary)
                loLap = new XElement(loNamespaceDefault + "Lap",
                                     new XAttribute("StartTime", ldtTime.ToUniversalTime()));
                // Lap duration
                if (activity.Duration != null)
                    loLap.Add(new XElement(loNamespaceDefault + "TotalTimeSeconds",
                                           activity.Duration.Value.TotalSeconds));
                // Lab distance
                if (activity.DistanceSummary != null &&
                    activity.DistanceSummary.TotalDistance != null &&
                    activity.SplitDistance != null)
                {
                    ldDistanceInMeters = activity.DistanceSummary.TotalDistance.Value / activity.SplitDistance.Value;
                    if (activity.SplitDistance.Value == MSHealthActivity.SPLIT_DISTANCE_KILOMETER)
                        ldDistanceInMeters = ldDistanceInMeters * 1000;
                    else if (activity.SplitDistance.Value == MSHealthActivity.SPLIT_DISTANCE_MILE)
                        ldDistanceInMeters = ldDistanceInMeters * 1609.344;
                    loLap.Add(new XElement(loNamespaceDefault + "DistanceMeters", ldDistanceInMeters));
                }
                // Lap calories
                if (activity.CaloriesBurnedSummary != null &&
                    activity.CaloriesBurnedSummary.TotalCalories != null)
                    loLap.Add(new XElement(loNamespaceDefault + "Calories", activity.CaloriesBurnedSummary.TotalCalories.Value));
                // Lap Heart Rate
                if (activity.HeartRateSummary != null)
                {
                    // Lap Heart Rate Average
                    if (activity.HeartRateSummary.AverageHeartRate != null)
                        loLap.Add(new XElement(loNamespaceDefault + "AverageHeartRateBpm",
                                               new XElement(loNamespaceDefault + "Value", activity.HeartRateSummary.AverageHeartRate.Value)));
                    // Lap Heart Rate Maximum
                    if (activity.HeartRateSummary.PeakHeartRate != null)
                        loLap.Add(new XElement(loNamespaceDefault + "MaximumHeartRateBpm",
                                               new XElement(loNamespaceDefault + "Value", activity.HeartRateSummary.PeakHeartRate.Value)));
                }
                loLap.Add(new XElement(loNamespaceDefault + "TriggerMethod", "Manual"));
                // Validate Activity MapPoints
                if (activity.MapPoints != null &&
                    activity.MapPoints.Any())
                {
                    // Initialize TCX Track
                    loTrack = new XElement(loNamespaceDefault + "Track");
                    List<MSHealthMapPoint> loMapPoints = new List<MSHealthMapPoint>();
                    if (ignoreEmptyMapPoints)
                        loMapPoints = activity.MapPoints.OrderBy(loMP => loMP.Ordinal).ToList();
                    else
                        loMapPoints = activity.MapPoints.Where(loMP => loMP.Location != null &&
                                                               loMP.Location.Latitude != null &&
                                                               loMP.Location.Longitude != null)
                                                        .OrderBy(loMP => loMP.Ordinal).ToList();
                    // Process Activity MapPoints
                    foreach (MSHealthMapPoint loMapPoint in loMapPoints)
                    {
                        loTrackpoint = new XElement(loNamespaceDefault + "Trackpoint");
                        // TrackPoint Time
                        if (loMapPoint.SecondsSinceStart != null)
                        {
                            loTrackpoint.Add(new XElement(loNamespaceDefault + "Time", ldtTime.AddSeconds(loMapPoint.SecondsSinceStart.Value).ToUniversalTime()));
                        }
                        else
                        {
                            loTrackpoint.Add(new XElement(loNamespaceDefault + "Time", ldtTime.ToUniversalTime()));
                        }
                        // TrackPoint Extension HeartRate
                        if (loMapPoint.HeartRate != null)
                        {
                            loTrackpoint.Add(new XElement(loNamespaceDefault + "HeartRateBpm",
                                                          new XElement(loNamespaceDefault + "Value", loMapPoint.HeartRate.Value)));
                        }
                        // Trackpoint Location
                        if (loMapPoint.Location != null)
                        {

                            // TrackPoint Elevation
                            if (loMapPoint.Location.ElevationFromMeanSeaLevel != null)
                                loTrackpoint.Add(new XElement(loNamespaceDefault + "AltitudeMeters", loMapPoint.Location.ElevationFromMeanSeaLevel / MSHealthGPSPoint.ELEVATION_FACTOR));
                            // TrackPoint Latitude/Longitude
                            if (loMapPoint.Location.Latitude != null &&
                                loMapPoint.Location.Latitude != null)
                            {
                                loTrackpoint.Add(new XElement(loNamespaceDefault + "Position",
                                                              new XElement(loNamespaceDefault + "LatitudeDegrees",
                                                                           loMapPoint.Location.Latitude / MSHealthGPSPoint.LATITUDE_FACTOR),
                                                              new XElement(loNamespaceDefault + "LongitudeDegrees",
                                                                           loMapPoint.Location.Longitude / MSHealthGPSPoint.LONGITUDE_FACTOR)));
                            }
                        }
                        // Trackpoint Distance
                        if (loMapPoint.TotalDistance != null)
                        {
                            ldDistanceInMeters = loMapPoint.TotalDistance.Value / activity.SplitDistance.Value;
                            if (activity.SplitDistance.Value == MSHealthActivity.SPLIT_DISTANCE_KILOMETER)
                                ldDistanceInMeters = ldDistanceInMeters * 1000;
                            else if (activity.SplitDistance.Value == MSHealthActivity.SPLIT_DISTANCE_MILE)
                                ldDistanceInMeters = ldDistanceInMeters * 1609.344;
                            loTrackpoint.Add(new XElement(loNamespaceDefault + "DistanceMeters", ldDistanceInMeters));
                        }
                        else
                        {
                            ldDistanceInMeters = 0;
                            loTrackpoint.Add(new XElement(loNamespaceDefault + "DistanceMeters", ldDistanceInMeters));
                        }
                        loTrack.Add(loTrackpoint);
                    }
                    loLap.Add(loTrack);
                }
                // Activity
                loActivity = new XElement(loNamespaceDefault + "Activity",
                                          new XAttribute("Sport", activity.Type == MSHealthActivityType.Run ? "Running" :
                                                         activity.Type == MSHealthActivityType.Bike ? "Biking" : "Other"),
                                          new XElement(loNamespaceDefault + "Id", ldtTime.ToUniversalTime()),
                                          loLap);
                loActivities = new XElement(loNamespaceDefault + "Activities",
                                            loActivity);
                loRoot.Add(loActivities);
                loDocument = new XDocument(new XDeclaration("1.0", "utf-8", null), loRoot);
            }
            // Return TCX (XML Document)
            return loDocument;
        }

        #endregion

    }

}
