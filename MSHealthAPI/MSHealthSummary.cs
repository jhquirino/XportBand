//-----------------------------------------------------------------------
// <copyright file="MSHealthSummary.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace MSHealthAPI
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    #region MSHealthSummaries class

    /// <summary>
    /// Represents details about the summaries associated with user's Microsoft Health profile.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthSummaries
    {

        #region Properties

        /// <summary>
        /// Gets or sets the collection of summaries.
        /// </summary>
        [JsonProperty(PropertyName = "summaries",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<MSHealthSummary> Summaries { get; set; }

        /// <summary>
        /// Gets or sets the URI for the next page of data.
        /// </summary>
        [JsonProperty(PropertyName = "nextPage",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string NextPage { get; set; }

        /// <summary>
        /// Gets or sets the number of <see cref="Summaries"/> returned.
        /// </summary>
        [JsonProperty(PropertyName = "itemCount",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int ItemCount { get; set; }

        #endregion

    }

    #endregion

    #region MSHealthSummary class

    /// <summary>
    /// Represents details about a summary.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthSummary
    {

        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        [JsonProperty(PropertyName = "userId",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string UserID { get; set; }

        /// <summary>
        /// Gets or sets the start time of the period.
        /// </summary>
        [JsonProperty(PropertyName = "startTime",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the period.
        /// </summary>
        [JsonProperty(PropertyName = "endTime",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Gets or sets the parent day of the period.
        /// </summary>
        [JsonProperty(PropertyName = "parentDay",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string ParentDay { get; set; }

        /// <summary>
        /// Gets or sets the is transit day.
        /// </summary>
        /// <remarks>
        /// <see langword="true"/> if the user transitioned time zones during this period, else <see langword="false"/>.
        /// </remarks>
        [JsonProperty(PropertyName = "isTransitDay",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public bool? IsTransitDay { get; set; }

        /// <summary>
        /// Gets or sets the length of the time bucket for which the summary is calculated.
        /// </summary>
        [JsonProperty(PropertyName = "period",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthPeriod Period { get; set; }

        /// <summary>
        /// Gets or sets the duration of the period.
        /// </summary>
        [JsonProperty(PropertyName = "duration",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Gets or sets the steps during the period.
        /// </summary>
        [JsonProperty(PropertyName = "stepsTaken",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? Steps { get; set; }

        /// <summary>
        /// Gets or sets the active hours during the period.
        /// </summary>
        [JsonProperty(PropertyName = "activeHours",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? ActiveHours { get; set; }

        /// <summary>
        /// Gets or sets the UV exposure as time in the sun.
        /// </summary>
        [JsonProperty(PropertyName = "uvExposure",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string UVExposure { get; set; }

        /// <summary>
        /// Gets or sets the summary of the calories burned during the period.
        /// </summary>
        [JsonProperty(PropertyName = "caloriesBurnedSummary",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthCaloriesBurnedSummary CaloriesBurnedSummary { get; set; }

        /// <summary>
        /// Gets or sets the heart rate data during the period.
        /// </summary>
        [JsonProperty(PropertyName = "heartRateSummary",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthHeartRateSummary HeartRateSummary { get; set; }

        /// <summary>
        /// Gets or sets the summary of the distance data during the period.
        /// </summary>
        [JsonProperty(PropertyName = "distanceSummary",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthDistanceSummary DistanceSummary { get; set; }

        #endregion

    }

    #endregion

    #region MSHealthCaloriesBurnedSummary class

    /// <summary>
    /// Represents the summary of calories burned during a period.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthCaloriesBurnedSummary
    {

        #region Properties

        /// <summary>
        /// Gets or sets the length of the time bucket for which the summary is calculated.
        /// </summary>
        [JsonProperty(PropertyName = "period",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthPeriod Period { get; set; }

        /// <summary>
        /// Gets or sets the total calories burned during the period.
        /// </summary>
        [JsonProperty(PropertyName = "totalCalories",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? TotalCalories { get; set; }

        #endregion

    }

    #endregion

    #region MSHealthHeartRateSummary class

    /// <summary>
    /// Represents the summary of heart rate data during a period.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthHeartRateSummary
    {

        #region Properties

        /// <summary>
        /// Gets or sets the length of the time bucket for which the summary is calculated.
        /// </summary>
        [JsonProperty(PropertyName = "period",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthPeriod Period { get; set; }

        /// <summary>
        /// Gets or sets the average heart rate achieved during the period.
        /// </summary>
        [JsonProperty(PropertyName = "averageHeartRate",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? AverageHeartRate { get; set; }

        /// <summary>
        /// Gets or sets the peak heart rate achieved during the period.
        /// </summary>
        [JsonProperty(PropertyName = "peakHeartRate",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? PeakHeartRate { get; set; }

        /// <summary>
        /// Gets or sets the lowest heart rate achieved during the period.
        /// </summary>
        [JsonProperty(PropertyName = "lowestHeartRate",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? LowestHeartRate { get; set; }

        #endregion

    }

    #endregion

    #region MSHealthPerformanceSummary class

    /// <summary>
    /// Represents the performance summary data during a period.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthPerformanceSummary
    {

        #region Properties

        /// <summary>
        /// Gets or sets the heart rate when the user finished the exercise.
        /// </summary>
        [JsonProperty(PropertyName = "finishHeartRate",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? FinishHeartRate { get; set; }

        /// <summary>
        /// Gets or sets the heart rate one minute after the user finished the exercise.
        /// </summary>
        [JsonProperty(PropertyName = "recoveryHeartRateAt1Minute",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? RecoveryHeartRateAt1Minute { get; set; }

        /// <summary>
        /// Gets or sets the heart rate two minutes after the user finished the exercise.
        /// </summary>
        [JsonProperty(PropertyName = "recoveryHeartRateAt2Minutes",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? RecoveryHeartRateAt2Minutes { get; set; }

        /// <summary>
        /// Gets or sets the breakdown of the heart rate zones during the exercise.
        /// </summary>
        [JsonProperty(PropertyName = "heartRateZones",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthHeartRateZones HeartRateZones { get; set; }

        #endregion

    }

    #endregion

    #region MSHealthDistanceSummary class

    /// <summary>
    /// Represents the summary of distance data during a period.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthDistanceSummary
    {

        #region Constants

        /// <summary>
        /// Factor to convert (divide by) elevation values returned to meters.
        /// </summary>
        public const double ELEVATION_FACTOR = 100d;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the length of the time bucket for which the summary is calculated.
        /// </summary>
        [JsonProperty(PropertyName = "period",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthPeriod Period { get; set; }

        /// <summary>
        /// Gets or sets the total distance during the period.
        /// </summary>
        /// <remarks>
        /// If this is a time-based summary, e.g. hourly or daily, then this is the total 
        /// distance of the period. If this is an activity split summary, e.g. splits of a 
        /// run, then this is the split distance, e.g. 1 mile, 1 kilometer. For the last  
        /// split of the activity, this value may be less than the full split distance.
        /// </remarks>
        [JsonProperty(PropertyName = "totalDistance",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? TotalDistance { get; set; }

        /// <summary>
        /// Gets or sets the total distance covered on foot during the period.
        /// </summary>
        [JsonProperty(PropertyName = "totalDistanceOnFoot",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? TotalDistanceOnFoot { get; set; }

        /// <summary>
        /// Gets or sets the absolute distance including any paused time distance during the period.
        /// </summary>
        [JsonProperty(PropertyName = "actualDistance",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? ActualDistance { get; set; }

        /// <summary>
        /// Gets or sets the cumulative elevation gain accrued during the period in cm.
        /// </summary>
        [JsonProperty(PropertyName = "elevationGain",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? ElevationGain { get; set; }

        /// <summary>
        /// Gets or sets the cumulative elevation loss accrued during this period in cm.
        /// </summary>
        [JsonProperty(PropertyName = "elevationLoss",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? ElevationLoss { get; set; }

        /// <summary>
        /// Gets or sets the maximum elevation during this period in cm.
        /// </summary>
        [JsonProperty(PropertyName = "maxElevation",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? MaxElevation { get; set; }

        /// <summary>
        /// Gets or sets the minimum elevation during this period in cm.
        /// </summary>
        [JsonProperty(PropertyName = "minElevation",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? MinElevation { get; set; }

        /// <summary>
        /// Gets or sets the distance in cm between recorded GPS points.
        /// </summary>
        [JsonProperty(PropertyName = "waypointDistance",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? WaypointDistance { get; set; }

        /// <summary>
        /// Gets or sets the average speed during the period.
        /// </summary>
        [JsonProperty(PropertyName = "speed",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? Speed { get; set; }

        /// <summary>
        /// Gets or sets the average pace during the period.
        /// </summary>
        [JsonProperty(PropertyName = "pace",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? Pace { get; set; }

        /// <summary>
        /// Gets or sets the total distance to the end of this period divided by total time to the end of this period.
        /// </summary>
        [JsonProperty(PropertyName = "overallPace",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? OverallPace { get; set; }

        #endregion

    }

    #endregion

    #region MSHealthHeartRateZones class

    /// <summary>
    /// Represents the mapping of the amount of time spent in a given heart rate zone during a segment.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthHeartRateZones
    {

        #region Properties

        /// <summary>
        /// Gets or sets the number of minutes where the HR was below 50% of the user’s max HR.
        /// </summary>
        [JsonProperty(PropertyName = "underHealthyHeart",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? UnderHealthyHeart { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes where the HR was below 50% of the user’s max HR.
        /// </summary>
        /// <remarks>
        /// This field is deprecated. The correct field name is now <see cref="UnderHealthyHeart"/>. 
        /// Populated for backwards compatibility until V2.
        /// </remarks>
        [JsonProperty(PropertyName = "underAerobic",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? UnderAerobic { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes where the HR was between 70-80% of the user’s max HR.
        /// </summary>
        [JsonProperty(PropertyName = "aerobic",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? Aerobic { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes where the HR was between 80-90% of the user’s max HR.
        /// </summary>
        [JsonProperty(PropertyName = "anaerobic",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? Anaerobic { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes where the HR was between 60-70% of the user’s max HR.
        /// </summary>
        [JsonProperty(PropertyName = "fitnessZone",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? FitnessZone { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes where the HR was between 50-60% of the user’s max HR.
        /// </summary>
        [JsonProperty(PropertyName = "healthyHeart",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? HealthyHeart { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes where the HR was between 90-100% of the user’s max HR.
        /// </summary>
        [JsonProperty(PropertyName = "redline",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? Redline { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes above the user’s max HR.
        /// </summary>
        [JsonProperty(PropertyName = "overRedline",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? OverRedline { get; set; }

        #endregion

    }

    #endregion

    #region MSHealthPeriod enum

    /// <summary>
    /// Represents the length of the time bucket for which a summary is calculated.
    /// </summary>
    public enum MSHealthPeriod
    {
        /// <summary>
        /// Unknown period.
        /// </summary>
        Unknown,
        /// <summary>
        /// Activity period
        /// </summary>
        Activity,
        /// <summary>
        /// Minute period.
        /// </summary>
        Minute,
        /// <summary>
        /// Quarter-hourly period.
        /// </summary>
        QuarterHourly,
        /// <summary>
        /// Half-hourly period.
        /// </summary>
        HalfHourly,
        /// <summary>
        /// Hourly period.
        /// </summary>
        Hourly,
        /// <summary>
        /// Daily period.
        /// </summary>
        Daily,
        /// <summary>
        /// Weekly period.
        /// </summary>
        Weekly,
        /// <summary>
        /// Last 30 days period.
        /// </summary>
        Last30Days,
        /// <summary>
        /// Calendar month period.
        /// </summary>
        CalendarMonth,
        /// <summary>
        /// Last 90 days period.
        /// </summary>
        Last90Days,
        /// <summary>
        /// Calendar year period.
        /// </summary>
        CalendarYear,
        /// <summary>
        /// Last 365 days period.
        /// </summary>
        Last365Days,
        /// <summary>
        /// Segment period.
        /// </summary>
        Segment,
    }

    #endregion

}
