//-----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace XportBand.Model
{
    using MSHealthAPI;
    using NikePlusAPI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Windows.Storage;

    /// <summary>
    /// Extensions for cross objects.
    /// </summary>
    public static class Extensions
    {

        #region MSHealthActivity Extension Methods

        /// <summary>
        /// Maps <see cref="MSHealthAPI.MSHealthActivity"/> instance to <see cref="NikePlusAPI.NikePlusActivity"/> instance.
        /// </summary>
        /// <param name="activityIn"><see cref="MSHealthAPI.MSHealthActivity"/> instance to map.</param>
        /// <returns><see cref="NikePlusAPI.NikePlusActivity"/> instance.</returns>
        public static NikePlusActivity ToNikePlusActicity(this MSHealthActivity activityIn)
        {
            NikePlusActivity loActivityOut = null;
            if (activityIn != null)
            {
                loActivityOut = new NikePlusActivity();
                // Distance
                double ldDistance = 0;
                if (activityIn.SplitDistance != null &&
                    activityIn.DistanceSummary != null &&
                    activityIn.DistanceSummary.TotalDistance != null)
                {
                    ldDistance = activityIn.DistanceSummary.TotalDistance.Value / activityIn.SplitDistance.Value;
                    if (activityIn.SplitDistance.Value == MSHealthActivity.SPLIT_DISTANCE_MILE)
                        ldDistance = ldDistance * 1.609344; //1609.344;
                    if (loActivityOut.MetricSummary == null)
                        loActivityOut.MetricSummary = new NikePlusMetricSummary();
                    loActivityOut.MetricSummary.Distance = ldDistance;
                }
                // Duration
                if (activityIn.Duration != null)
                {
                    loActivityOut.Duration = activityIn.Duration.Value.TotalMilliseconds;
                    if (loActivityOut.MetricSummary == null)
                        loActivityOut.MetricSummary = new NikePlusMetricSummary();
                    loActivityOut.MetricSummary.Duration = activityIn.Duration;
                }
                // Start Time
                if (activityIn.StartTime != null &&
                    activityIn.StartTime.HasValue)
                    loActivityOut.StartTime = activityIn.StartTime.Value;
                // Status
                loActivityOut.Status = NikePlusActivityStatus.COMPLETE;
                // Type
                switch (activityIn.Type)
                {
                    case MSHealthActivityType.Run:
                        loActivityOut.Type = NikePlusActivityType.RUN;
                        break;
                    case MSHealthActivityType.Bike:
                        loActivityOut.Type = NikePlusActivityType.CYCLE;
                        break;
                    case MSHealthActivityType.Sleep:
                        loActivityOut.Type = NikePlusActivityType.SLEEPING;
                        break;
                    default:
                        loActivityOut.Type = NikePlusActivityType.OTHER;
                        break;
                }
                // Calories
                if (activityIn.CaloriesBurnedSummary != null &&
                    activityIn.CaloriesBurnedSummary.TotalCalories != null)
                {
                    if (loActivityOut.MetricSummary == null)
                        loActivityOut.MetricSummary = new NikePlusMetricSummary();
                    loActivityOut.MetricSummary.Calories = activityIn.CaloriesBurnedSummary.TotalCalories;
                }
                // Device
                loActivityOut.DeviceType = NikePlusDeviceType.WATCH; // Nike doesn't recognize MSBand as valid device, so, set as WATCH
                                                                     // Get valid MapPoints (with Location Latitude and Longitude)
                IEnumerable<MSHealthMapPoint> loMapPoints = null;
                if (activityIn.MapPoints != null &&
                    activityIn.MapPoints.Any())
                {
                    loMapPoints = activityIn.MapPoints
                        .Where(loMP => loMP.IsPaused != null &&
                               !loMP.IsPaused.Value &&
                               loMP.SecondsSinceStart != null)
                        .OrderBy(loMP => loMP.Ordinal);
                }
                // Metrics
                List<NikePlusMetric> loMetrics = new List<NikePlusMetric>();
                NikePlusMetric loMetricDistance = new NikePlusMetric() { Type = NikePlusMetricType.DISTANCE };
                NikePlusMetric loMetricHeartRate = new NikePlusMetric() { Type = NikePlusMetricType.HEARTRATE };
                NikePlusMetric loMetricSpeed = new NikePlusMetric { Type = NikePlusMetricType.SPEED };
                NikePlusMetric loMetricCalories = new NikePlusMetric { Type = NikePlusMetricType.CALORIES };
                SortedDictionary<float, float> loDistanceToDurationNodes = new SortedDictionary<float, float>();
                SortedDictionary<float, float> loHeartRateToDurationNodes = new SortedDictionary<float, float>();
                SortedDictionary<float, float> loSpeedToDurationNodes = new SortedDictionary<float, float>();
                SortedDictionary<float, float> loCaloriesToDurationNodes = new SortedDictionary<float, float>();
                SortedDictionary<float, float> loDurationToDistanceNodes = new SortedDictionary<float, float>();
                double ldTotalDistance = 0;
                float lfTotalSeconds = 0;
                if (loMapPoints != null &&
                    loMapPoints.Any())
                {
                    foreach (MSHealthMapPoint loMapPoint in loMapPoints)
                    {
                        // Location
                        if (loMapPoint.Location != null)
                        {
                            // Latitude/Longitude
                            if (loMapPoint.Location.Latitude != null &&
                                loMapPoint.Location.Longitude != null)
                            {
                                if (loActivityOut.GPSData == null)
                                    loActivityOut.GPSData = new NikePlusGPS();
                                if (loActivityOut.GPSData.Waypoints == null)
                                    loActivityOut.GPSData.Waypoints = new List<NikePlusWaypoint>();
                                NikePlusWaypoint loWaypoint = new NikePlusWaypoint()
                                {
                                    Latitude = loMapPoint.Location.Latitude.Value / MSHealthGPSPoint.LATITUDE_FACTOR,
                                    Longitude = loMapPoint.Location.Longitude.Value / MSHealthGPSPoint.LONGITUDE_FACTOR,
                                };
                                // Elevation
                                if (loMapPoint.Location.ElevationFromMeanSeaLevel != null)
                                {
                                    loWaypoint.Elevation = loMapPoint.Location.ElevationFromMeanSeaLevel.Value / MSHealthGPSPoint.ELEVATION_FACTOR;
                                }
                                loActivityOut.GPSData.Waypoints.Add(loWaypoint);
                            }
                        }
                        // Distance
                        if (loMapPoint.TotalDistance != null &&
                            loMapPoint.SecondsSinceStart != null)
                        {
                            ldDistance = loMapPoint.TotalDistance.Value / activityIn.SplitDistance.Value;
                            if (activityIn.SplitDistance.Value == MSHealthActivity.SPLIT_DISTANCE_MILE)
                                ldDistance = ldDistance * 1.609344;
                            if (!loDistanceToDurationNodes.ContainsKey(loMapPoint.SecondsSinceStart.Value))
                                loDistanceToDurationNodes.Add(loMapPoint.SecondsSinceStart.Value, (float)ldDistance);
                            if (!loDurationToDistanceNodes.ContainsKey((float)ldDistance))
                                loDurationToDistanceNodes.Add((float)ldDistance, loMapPoint.SecondsSinceStart.Value);
                            ldTotalDistance = ldDistance;
                            lfTotalSeconds = loMapPoint.SecondsSinceStart.Value;
                        }
                        // Heart rate
                        if (loMapPoint.HeartRate != null)
                        {
                            if (!loHeartRateToDurationNodes.ContainsKey(loMapPoint.SecondsSinceStart.Value))
                                loHeartRateToDurationNodes.Add(loMapPoint.SecondsSinceStart.Value, loMapPoint.HeartRate.Value);
                        }
                        // Speed
                        if (loMapPoint.Speed != null)
                        {
                            // TODO: Determine if conversion is necessary
                            if (!loSpeedToDurationNodes.ContainsKey(loMapPoint.SecondsSinceStart.Value))
                                loSpeedToDurationNodes.Add(loMapPoint.SecondsSinceStart.Value, (float)loMapPoint.Speed.Value);
                        }
                    }
                    MSHealthMapPoint loLastMapPoint = loMapPoints.LastOrDefault();
                    if (loLastMapPoint != null &&
                        loLastMapPoint.SecondsSinceStart != null)
                        lfTotalSeconds = loLastMapPoint.SecondsSinceStart.Value;
                }
                else
                {
                    if (activityIn.MinuteSummaries != null &&
                        activityIn.MinuteSummaries.Any())
                    {
                        lfTotalSeconds = 0;
                        ldTotalDistance = 0;
                        IEnumerable<MSHealthSummary> loSummaries = activityIn.MinuteSummaries.OrderBy(loSumm => loSumm.StartTime);
                        foreach (MSHealthSummary loSummary in loSummaries)
                        {
                            lfTotalSeconds += 60;
                            // Calories
                            if (loSummary.CaloriesBurnedSummary != null &&
                                loSummary.CaloriesBurnedSummary.TotalCalories != null)
                            {
                                if (!loCaloriesToDurationNodes.ContainsKey(lfTotalSeconds))
                                    loCaloriesToDurationNodes.Add(lfTotalSeconds, loSummary.CaloriesBurnedSummary.TotalCalories.Value);
                            }
                            // Heart Rate
                            if (loSummary.HeartRateSummary != null &&
                                loSummary.HeartRateSummary.AverageHeartRate != null)
                            {
                                if (!loHeartRateToDurationNodes.ContainsKey(lfTotalSeconds))
                                    loHeartRateToDurationNodes.Add(lfTotalSeconds, loSummary.HeartRateSummary.AverageHeartRate.Value);
                            }
                            // Distance
                            if (activityIn.SplitDistance != null &&
                                loSummary.DistanceSummary != null &&
                                loSummary.DistanceSummary.TotalDistance != null)
                            {
                                ldDistance = loSummary.DistanceSummary.TotalDistance.Value / activityIn.SplitDistance.Value;
                                if (activityIn.SplitDistance.Value == MSHealthActivity.SPLIT_DISTANCE_MILE)
                                    ldDistance = ldDistance * 1.609344;
                                ldTotalDistance += ldDistance;
                                if (!loDistanceToDurationNodes.ContainsKey(lfTotalSeconds))
                                    loDistanceToDurationNodes.Add(lfTotalSeconds, (float)ldTotalDistance);
                                if (!loDurationToDistanceNodes.ContainsKey((float)ldDistance))
                                    loDurationToDistanceNodes.Add((float)ldDistance, lfTotalSeconds);
                            }
                        }
                    }
                    loActivityOut.IsGPSActivity = false;
                }
                // Interpolate metrics to each 10 seconds.
                List<float> loIntervals = new List<float>();
                for (float lfInterval = 10; lfInterval < lfTotalSeconds; lfInterval += 10)
                    loIntervals.Add(lfInterval);
                CubicSpline loCubicSpline = null;
                float[] loValues = null;
                /*
				 * GPS
				 */
                if (loActivityOut.GPSData != null &&
                    loActivityOut.GPSData.Waypoints != null &&
                    loActivityOut.GPSData.Waypoints.Any())
                {
                    // Configure GPS Data
                    loActivityOut.IsGPSActivity = true;
                    loActivityOut.GPSData.IntervalUnit = NikePlusIntervalUnit.SEC;
                    loActivityOut.GPSData.IntervalMetric = Math.Floor((double)lfTotalSeconds / (double)loActivityOut.GPSData.Waypoints.Count);
                    // Elevation
                    if (activityIn.DistanceSummary != null)
                    {
                        if (activityIn.DistanceSummary.ElevationGain != null)
                            loActivityOut.GPSData.ElevationGain = activityIn.DistanceSummary.ElevationGain / MSHealthDistanceSummary.ELEVATION_FACTOR;
                        if (activityIn.DistanceSummary.ElevationLoss != null)
                            loActivityOut.GPSData.ElevationLoss = activityIn.DistanceSummary.ElevationLoss / MSHealthDistanceSummary.ELEVATION_FACTOR;
                        if (activityIn.DistanceSummary.MaxElevation != null)
                            loActivityOut.GPSData.ElevationMax = activityIn.DistanceSummary.MaxElevation / MSHealthDistanceSummary.ELEVATION_FACTOR;
                        if (activityIn.DistanceSummary.MinElevation != null)
                            loActivityOut.GPSData.ElevationMin = activityIn.DistanceSummary.MinElevation / MSHealthDistanceSummary.ELEVATION_FACTOR;
                    }
                    // GPS can be loaded using GPX format
                    try { loActivityOut.GPX = activityIn.ToGPX(); }
                    catch { /* Do nothing */ }
                }
                /*
				 * Metrics
				 */
                // Distance
                if (loDistanceToDurationNodes.Any())
                {
                    if (!loDistanceToDurationNodes.ContainsKey(0))
                        loDistanceToDurationNodes.Add(0, 0);
                    loCubicSpline = new CubicSpline(loDistanceToDurationNodes.Keys.ToArray(), loDistanceToDurationNodes.Values.ToArray());
                    loValues = loCubicSpline.Eval(loIntervals.ToArray());
                    // Configure Metric Intervals and Values
                    loMetricDistance.Values = (from lfValue in loValues select lfValue < 0 ? "0" : lfValue.ToString()).ToList();
                    loMetricDistance.IntervalUnit = NikePlusIntervalUnit.SEC;
                    loMetricDistance.IntervalMetric = 10;
                    loMetrics.Add(loMetricDistance);
                }
                // Heart Rate
                if (loHeartRateToDurationNodes.Any())
                {
                    if (!loHeartRateToDurationNodes.ContainsKey(0))
                        loHeartRateToDurationNodes.Add(0, 0);
                    loCubicSpline = new CubicSpline(loHeartRateToDurationNodes.Keys.ToArray(), loHeartRateToDurationNodes.Values.ToArray());
                    loValues = loCubicSpline.Eval(loIntervals.ToArray());
                    // Configure Metric Intervals and Values
                    loMetricHeartRate.Values = (from lfValue in loValues select lfValue < 0 ? "0" : ((int)lfValue).ToString()).ToList();
                    loMetricHeartRate.IntervalUnit = NikePlusIntervalUnit.SEC;
                    loMetricHeartRate.IntervalMetric = 10;
                    loMetrics.Add(loMetricHeartRate);
                }
                // Speed
                if (loSpeedToDurationNodes.Any())
                {
                    if (!loSpeedToDurationNodes.ContainsKey(0))
                        loSpeedToDurationNodes.Add(0, 0);
                    loCubicSpline = new CubicSpline(loSpeedToDurationNodes.Keys.ToArray(), loSpeedToDurationNodes.Values.ToArray());
                    loValues = loCubicSpline.Eval(loIntervals.ToArray());
                    // Configure Metric Intervals and Values
                    loMetricSpeed.Values = (from lfValue in loValues select lfValue < 0 ? "0" : lfValue.ToString()).ToList();
                    loMetricSpeed.IntervalUnit = NikePlusIntervalUnit.SEC;
                    loMetricSpeed.IntervalMetric = 10;
                    //loMetrics.Add(loMetricSpeed);
                }
                // Calories
                if (loCaloriesToDurationNodes.Any())
                {
                    if (!loCaloriesToDurationNodes.ContainsKey(0))
                        loCaloriesToDurationNodes.Add(0, 0);
                    loCubicSpline = new CubicSpline(loCaloriesToDurationNodes.Keys.ToArray(), loCaloriesToDurationNodes.Values.ToArray());
                    loValues = loCubicSpline.Eval(loIntervals.ToArray());
                    // Configure Metric Intervals and Values
                    loMetricCalories.Values = (from lfValue in loValues select lfValue < 0 ? "0" : lfValue.ToString()).ToList();
                    loMetricCalories.IntervalUnit = NikePlusIntervalUnit.SEC;
                    loMetricCalories.IntervalMetric = 10;
                    loMetrics.Add(loMetricCalories);
                }
                // Snapshots
                if (loDurationToDistanceNodes != null &&
                    loDurationToDistanceNodes.Any())
                {
                    loActivityOut.SummaryV2 = new NikePlusActivitySummaryV2();
                    loActivityOut.SummaryV2.Snapshots = new List<NikePlusSnapshotV2>();
                    NikePlusSnapshotV2 loSnapshot = null;
                    // Kilometers
                    int liTotalDistance = (int)(ldTotalDistance / 1d);
                    loIntervals = new List<float>();
                    for (float lfInterval = 1; lfInterval <= liTotalDistance; lfInterval++)
                        loIntervals.Add(lfInterval);
                    loCubicSpline = null;
                    loValues = null;
                    if (!loDurationToDistanceNodes.ContainsKey(0))
                        loDurationToDistanceNodes.Add(0, 0);
                    loCubicSpline = new CubicSpline(loDurationToDistanceNodes.Keys.ToArray(), loDurationToDistanceNodes.Values.ToArray());
                    loValues = loCubicSpline.Eval(loIntervals.ToArray());
                    loSnapshot = new NikePlusSnapshotV2();
                    loSnapshot.Name = "kmSplit";
                    loSnapshot.DataSeries = new List<NikePlusDataSerieV2>();
                    for (int liValue = 0; liValue < loValues.Length; liValue++)
                    {
                        NikePlusDataSerieV2 loDataSerie = new NikePlusDataSerieV2();
                        loDataSerie.Metrics = new NikePlusMetricV2()
                        {
                            Distance = (liValue + 1),
                            Duration = loValues[liValue] * 1000, // Seconds to Milliseconds
                        };
                        loDataSerie.ObjectType = "dataPoint";
                        loSnapshot.DataSeries.Add(loDataSerie);
                    }
                    loActivityOut.SummaryV2.Snapshots.Add(loSnapshot);
                    // Miles (1.609344)
                    liTotalDistance = (int)(ldTotalDistance / 1.609344d);
                    loIntervals = new List<float>();
                    for (float lfInterval = 1; lfInterval <= liTotalDistance; lfInterval++)
                        loIntervals.Add(lfInterval * 1.609344f);
                    loCubicSpline = null;
                    loValues = null;
                    if (!loDurationToDistanceNodes.ContainsKey(0))
                        loDurationToDistanceNodes.Add(0, 0);
                    loCubicSpline = new CubicSpline(loDurationToDistanceNodes.Keys.ToArray(), loDurationToDistanceNodes.Values.ToArray());
                    loValues = loCubicSpline.Eval(loIntervals.ToArray());
                    loSnapshot = new NikePlusSnapshotV2();
                    loSnapshot.Name = "mileSplit";
                    loSnapshot.DataSeries = new List<NikePlusDataSerieV2>();
                    for (int liValue = 0; liValue < loValues.Length; liValue++)
                    {
                        NikePlusDataSerieV2 loDataSerie = new NikePlusDataSerieV2();
                        loDataSerie.Metrics = new NikePlusMetricV2()
                        {
                            Distance = (liValue + 1),
                            Duration = loValues[liValue] * 1000, // Seconds to Milliseconds
                        };
                        loDataSerie.ObjectType = "dataPoint";
                        loSnapshot.DataSeries.Add(loDataSerie);
                    }
                    loActivityOut.SummaryV2.Snapshots.Add(loSnapshot);
                }
                // Set metrics
                if (loMetrics != null &&
                    loMetrics.Any())
                    loActivityOut.Metrics = loMetrics;
                /*
				Newtonsoft.Json.JsonSerializerSettings loSerializerSettings = null;
				loSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings();
				loSerializerSettings.Converters.Add(new NikePlusActivityPostConverterV2());
				string lsActivityOutJson = Newtonsoft.Json.JsonConvert.SerializeObject(loActivityOut, loSerializerSettings);
				System.Diagnostics.Debug.WriteLine(lsActivityOutJson);
				//loActivityOut = null;//*/
            }
            return loActivityOut;
        }

        #endregion

#if WINDOWS_UWP
        #region MSHealthToken Extension Methods

        /// <summary>
        /// Converts <see cref="MSHealthToken"/> instance to <see cref="ApplicationDataCompositeValue"/>.
        /// </summary>
        /// <param name="token"><see cref="MSHealthToken"/> instance.</param>
        /// <returns><see cref="ApplicationDataCompositeValue"/> instance.</returns>
        public static ApplicationDataCompositeValue ToComposite(this MSHealthToken token)
        {
            ApplicationDataCompositeValue loComposite = null;
            if (token != null)
            {
                loComposite = new ApplicationDataCompositeValue();
                loComposite["creationTime"] = token.CreationTime.Ticks;
                loComposite["expiresIn"] = token.ExpiresIn;
                loComposite["expirationTime"] = token.ExpirationTime.Ticks;
                if (!string.IsNullOrEmpty(token.AccessToken))
                    loComposite["accessToken"] = token.AccessToken;
                if (!string.IsNullOrEmpty(token.RefreshToken))
                    loComposite["refreshToken"] = token.RefreshToken;
            }

            return loComposite;
        }

        /// <summary>
        /// Converts <see cref="ApplicationDataCompositeValue"/> instance to <see cref="MSHealthToken"/>.
        /// </summary>
        /// <param name="composite"><see cref="ApplicationDataCompositeValue"/> instance.</param>
        /// <returns><see cref="MSHealthToken"/> instancel.</returns>
        public static MSHealthToken ToMSHealthToken(this ApplicationDataCompositeValue composite)
        {
            MSHealthToken loToken = null;
            if (composite != null)
            {
                loToken = new MSHealthToken();
                if (composite["creationTime"] != null)
                    loToken.CreationTime = new DateTime((long)composite["creationTime"]);
                if (composite["expiresIn"] != null)
                    loToken.ExpiresIn = (long)composite["expiresIn"];
                if (composite["accessToken"] != null)
                    loToken.AccessToken = (string)composite["accessToken"];
                if (composite["refreshToken"] != null)
                    loToken.RefreshToken = (string)composite["refreshToken"];
            }
            return loToken;
        }

        #endregion
#endif

    }

}
