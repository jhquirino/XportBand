//-----------------------------------------------------------------------
// <copyright file="MSHealthActivity.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace MSHealthAPI
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    #region MSHealthActivities class

    /// <summary>
    /// Represents details about the activities associated with users Microsoft Health profile.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthActivities
    {

        #region Properties

        /// <summary>
        /// Gets or sets the collection of bike activities.
        /// </summary>
        [JsonProperty(PropertyName = "bikeActivities",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<MSHealthActivity> BikeActivities { get; set; }

        /// <summary>
        /// Gets or sets the collection of free play activities.
        /// </summary>
        [JsonProperty(PropertyName = "freePlayActivities",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<MSHealthActivity> FreePlayActivities { get; set; }

        /// <summary>
        /// Gets or sets the collection of golf activities.
        /// </summary>
        [JsonProperty(PropertyName = "golfActivities",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<MSHealthActivity> GolfActivities { get; set; }

        /// <summary>
        /// Gets or sets the collection of guided workout activities.
        /// </summary>
        [JsonProperty(PropertyName = "guidedWorkoutActivities",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<MSHealthActivity> GuidedWorkoutActivities { get; set; }

        /// <summary>
        /// Gets or sets the collection of run activities.
        /// </summary>
        [JsonProperty(PropertyName = "runActivities",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<MSHealthActivity> RunActivities { get; set; }

        /// <summary>
        /// Gets or sets the collection of sleep activities.
        /// </summary>
        [JsonProperty(PropertyName = "sleepActivities",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<MSHealthActivity> SleepActivities { get; set; }

        /// <summary>
        /// Gets or sets the URI for the next page of data.
        /// </summary>
        [JsonProperty(PropertyName = "nextPage",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string NextPage { get; set; }

        /// <summary>
        /// Gets or sets the number of activities returned.
        /// </summary>
        [JsonProperty(PropertyName = "itemCount",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int ItemCount { get; set; }

        #endregion

    }

    #endregion

    #region MSHealthActivity class

    /// <summary>
    /// Represents details about an activity.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthActivity
    {

        #region Constants

        /// <summary>
        /// Split Distance value for Kilometers.
        /// </summary>
        public const double SPLIT_DISTANCE_KILOMETER = 100000;

        /// <summary>
        /// Split Distance value for Miles.
        /// </summary>
        public const double SPLIT_DISTANCE_MILE = 160934;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier of the activity (unique by user).
        /// </summary>
        [JsonProperty(PropertyName = "id",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who completed the activity.
        /// </summary>
        [JsonProperty(PropertyName = "userId",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string UserID { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the device which generated the activity.
        /// </summary>
        [JsonProperty(PropertyName = "deviceId",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string DeviceID { get; set; }

        /// <summary>
        /// Gets or sets the start time of the activity.
        /// </summary>
        [JsonProperty(PropertyName = "startTime",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the activity.
        /// </summary>
        [JsonProperty(PropertyName = "endTime",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Gets or sets the mapping of an event to a logical date.
        /// </summary>
        /// <remarks>
        /// For most events, other than sleep, the default assignment is based on the event's start time. 
        /// This is subject to change in the future. For example, if a sleep activity starts before 5 AM, 
        /// the DayId is the previous day.
        /// </remarks>
        [JsonProperty(PropertyName = "dayId",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public DateTime? DayId { get; set; }

        /// <summary>
        /// Gets or sets the time the activity was created.
        /// </summary>
        [JsonProperty(PropertyName = "createdTime",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// Gets or sets the app that created the activity.
        /// </summary>
        [JsonProperty(PropertyName = "createdBy",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the type of the activity.
        /// </summary>
        [JsonProperty(PropertyName = "activityType",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthActivityType Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the activity.
        /// </summary>
        [JsonProperty(PropertyName = "name",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the duration of the activity.
        /// </summary>
        [JsonProperty(PropertyName = "duration",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Gets or sets the UV exposure as time in the sun.
        /// </summary>
        [JsonProperty(PropertyName = "uvExposure",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string UVExposure { get; set; }

        /// <summary>
        /// Gets or sets the summaries associated with this activity.
        /// </summary>
        [JsonProperty(PropertyName = "minuteSummaries",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<MSHealthSummary> MinuteSummaries { get; set; }

        /// <summary>
        /// Gets or sets the summary of calories burned during the activity.
        /// </summary>
        [JsonProperty(PropertyName = "caloriesBurnedSummary",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthCaloriesBurnedSummary CaloriesBurnedSummary { get; set; }

        /// <summary>
        /// Gets or sets the heart rate summary for the activity.
        /// </summary>
        [JsonProperty(PropertyName = "heartRateSummary",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthHeartRateSummary HeartRateSummary { get; set; }

        /// <summary>
        /// Gets or sets the performance summary for the activity.
        /// </summary>
        [JsonProperty(PropertyName = "performanceSummary",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthPerformanceSummary PerformanceSummary { get; set; }

        /// <summary>
        /// Gets or sets the summary of distance data during the activity.
        /// </summary>
        [JsonProperty(PropertyName = "distanceSummary",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthDistanceSummary DistanceSummary { get; set; }

        /// <summary>
        /// Gets or sets the  length of time the user was paused during the activity.
        /// </summary>
        [JsonProperty(PropertyName = "pausedDuration",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public TimeSpan? PausedDuration { get; set; }

        /// <summary>
        /// Gets or sets the split distance used for the activity.
        /// </summary>
        [JsonProperty(PropertyName = "splitDistance",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? SplitDistance { get; set; }

        /// <summary>
        /// Gets or sets the map points for the activity.
        /// </summary>
        [JsonProperty(PropertyName = "mapPoints",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<MSHealthMapPoint> MapPoints { get; set; }

        /// <summary>
        /// Gets or sets the segments associated with this activity.
        /// </summary>
        [JsonProperty(PropertyName = "activitySegments",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<MSHealthActivitySegment> ActivitySegments { get; set; }

        /// <summary>
        /// Gets or sets the number of complete circuit rounds actually performed.
        /// </summary>
        /// <remarks>
        /// A round is the repetition of a circuit of exercises. 
        /// Partial circuits are not counted.
        /// Only available on <see cref="MSHealthActivityType.GuidedWorkout"/>
        /// </remarks>
        [JsonProperty(PropertyName = "roundsPerformed",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? RoundsPerformed { get; set; }

        /// <summary>
        /// Gets or sets the number of exercise repetitions actually performed.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.GuidedWorkout"/>
        /// </remarks>
        [JsonProperty(PropertyName = "repetitionsPerformed",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? RepetitionsPerformed { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the workout plan.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.GuidedWorkout"/>
        /// </remarks>
        [JsonProperty(PropertyName = "workoutPlanId",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string WorkoutPlanId { get; set; }

        /// <summary>
        /// Gets or sets the length of time the user was awake during the activity.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.Sleep"/>
        /// </remarks>
        [JsonProperty(PropertyName = "awakeDuration",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public TimeSpan? AwakeDuration { get; set; }

        /// <summary>
        /// Gets or sets the total length of time the user was asleep during the activity.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.Sleep"/>
        /// </remarks>
        [JsonProperty(PropertyName = "sleepDuration",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public TimeSpan? SleepDuration { get; set; }

        /// <summary>
        /// Gets or sets the number of times the user woke up during the activity,
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.Sleep"/>
        /// </remarks>
        [JsonProperty(PropertyName = "numberOfWakeups",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? NumberOfWakeups { get; set; }

        /// <summary>
        /// Gets or sets the length of time it took the user to fall asleep.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.Sleep"/>
        /// </remarks>
        [JsonProperty(PropertyName = "fallAsleepDuration",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public TimeSpan? FallAsleepDuration { get; set; }

        /// <summary>
        /// Gets or sets the ratio of sleep duration to total duration of the activity,
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.Sleep"/>
        /// </remarks>
        [JsonProperty(PropertyName = "sleepEfficiencyPercentage",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? SleepEfficiencyPercentage { get; set; }

        /// <summary>
        /// Gets or sets the length of time the user was in a restless sleep state.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.Sleep"/>
        /// </remarks>
        [JsonProperty(PropertyName = "totalRestlessSleepDuration",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public TimeSpan? TotalRestlessSleepDuration { get; set; }

        /// <summary>
        /// Gets or sets the length of time the user was in a restful sleep state.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.Sleep"/>
        /// </remarks>
        [JsonProperty(PropertyName = "totalRestfulSleepDuration",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public TimeSpan? TotalRestfulSleepDuration { get; set; }

        /// <summary>
        /// Gets or sets the resting heart rate during the activity.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.Sleep"/>
        /// </remarks>
        [JsonProperty(PropertyName = "restingHeartRate",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? RestingHeartRate { get; set; }

        /// <summary>
        /// Gets or sets the time the user fell asleep.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.Sleep"/>
        /// </remarks>
        [JsonProperty(PropertyName = "fallAsleepTime",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public DateTime? FallAsleepTime { get; set; }

        /// <summary>
        /// Gets or sets the time the user woke up,.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.Sleep"/>
        /// </remarks>
        [JsonProperty(PropertyName = "wakeupTime",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public DateTime? WakeupTime { get; set; }

        /// <summary>
        /// Gets or sets the total number of steps a user took during the activity.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.Golf"/>
        /// </remarks>
        [JsonProperty(PropertyName = "totalStepCount",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? TotalStepCount { get; set; }

        /// <summary>
        /// Gets or sets the total distance a user walked during the activity.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.Golf"/>
        /// </remarks>
        [JsonProperty(PropertyName = "totalDistanceWalked",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? TotalDistanceWalked { get; set; }

        /// <summary>
        /// Gets or sets the number of holes played where the user scored par or better during the activity.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.Golf"/>
        /// </remarks>
        [JsonProperty(PropertyName = "parOrBetterCount",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? ParOrBetterCount { get; set; }

        /// <summary>
        /// Gets or sets the distance of the longest drive hit by the user during the activity.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.Golf"/>
        /// </remarks>
        [JsonProperty(PropertyName = "longestDriveDistance",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? LongestDriveDistance { get; set; }

        /// <summary>
        /// Gets or sets the distance of the longest stroke hit by the user during the activity.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.Golf"/>
        /// </remarks>
        [JsonProperty(PropertyName = "longestStrokeDistance",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? LongestStrokeDistance { get; set; }

        /// <summary>
        /// Gets or sets the list of child activities.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthActivityType.Golf"/>
        /// </remarks>
        [JsonProperty(PropertyName = "childActivities",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<MSHealthActivity> ChildActivities { get; set; }

        #endregion

    }
    #endregion

    #region MSHealthMapPoint class

    /// <summary>
    /// Represents map point details for activity.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthMapPoint
    {

        #region Properties

        /// <summary>
        /// Gets or sets the number of seconds that have elapsed since mapping began, typically the start of a run or other activity.
        /// </summary>
        [JsonProperty(PropertyName = "secondsSinceStart",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? SecondsSinceStart { get; set; }

        /// <summary>
        /// Gets or sets the type of map point.
        /// </summary>
        [JsonProperty(PropertyName = "mapPointType",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthMapPointType Type { get; set; }

        /// <summary>
        /// Gets or sets the absolute ordering of this point relative to the others in its set, starting from 0.
        /// </summary>
        [JsonProperty(PropertyName = "ordinal",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public long? Ordinal { get; set; }

        /// <summary>
        /// Gets or sets the distance not including distance traveled while paused, 
        /// it is the distance that splits are based off of, since splits ignore 
        /// paused time.
        /// </summary>
        [JsonProperty(PropertyName = "actualDistance",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? ActualDistance { get; set; }

        /// <summary>
        /// Gets or sets the total distance from the start point to this map point.
        /// </summary>
        [JsonProperty(PropertyName = "totalDistance",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? TotalDistance { get; set; }

        /// <summary>
        /// Gets or sets the heart rate at the time of this map point.
        /// </summary>
        [JsonProperty(PropertyName = "heartRate",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? HeartRate { get; set; }

        /// <summary>
        /// Gets or sets the pace.
        /// </summary>
        [JsonProperty(PropertyName = "pace",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? Pace { get; set; }

        /// <summary>
        /// Gets or sets a number between 0 and 100 that denotes the pace/speed between the 
        /// slowest and fastest instantaneous pace for the overall route.
        /// </summary>
        /// <remarks>
        /// Slowest segment in the route (highest pace, lowest speed) is 0 and fastest segment 
        /// (lowest pace, highest speed) is 100. Only makes sense in the context of the set of 
        /// all map points.
        /// </remarks>
        [JsonProperty(PropertyName = "scaledPace",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? ScaledPace { get; set; }

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        [JsonProperty(PropertyName = "speed",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? Speed { get; set; }

        /// <summary>
        /// Gets or sets the GPS location for this map point.
        /// </summary>
        [JsonProperty(PropertyName = "location",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthGPSPoint Location { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this map point occurred during paused time.
        /// </summary>
        [JsonProperty(PropertyName = "isPaused",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public bool? IsPaused { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this map point is the first one since the activity resumed.
        /// </summary>
        [JsonProperty(PropertyName = "isResume",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public bool? IsResume { get; set; }

        #endregion

    }

    #endregion

    #region MSHealthGPSPoint class

    /// <summary>
    /// Represents a segment segments associated with this activity.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthGPSPoint
    {

        #region Constants

        /// <summary>
        /// Factor to convert (divide by) latitud value returned to degrees.
        /// </summary>
        public const double LATITUDE_FACTOR = 10000000d;

        /// <summary>
        /// Factor to convert (divide by) longitude value returned to degrees.
        /// </summary>
        public const double LONGITUDE_FACTOR = 10000000d;

        /// <summary>
        /// Factor to convert (divide by) elevation values returned to meters.
        /// </summary>
        public const double ELEVATION_FACTOR = 100d;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the speed over ground for the GPS point in m/s * 100.
        /// </summary>
        [JsonProperty(PropertyName = "speedOverGround",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? SpeedOverGround { get; set; }

        /// <summary>
        /// Gets or sets the latitude for the GPS point in degrees * 10^7 (+ = North).
        /// </summary>
        [JsonProperty(PropertyName = "latitude",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude for the GPS point in degrees * 10^7 (+ = East).
        /// </summary>
        [JsonProperty(PropertyName = "longitude",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the elevation from mean sea level in m * 100.
        /// </summary>
        [JsonProperty(PropertyName = "elevationFromMeanSeaLevel",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? ElevationFromMeanSeaLevel { get; set; }

        /// <summary>
        /// Gets or sets the estimated horizontal error in m * 100.
        /// </summary>
        [JsonProperty(PropertyName = "estimatedHorizontalError",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? EstimatedHorizontalError { get; set; }

        /// <summary>
        /// Gets or sets the estimated vertical error in m * 100.
        /// </summary>
        [JsonProperty(PropertyName = "estimatedVerticalError",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? EstimatedVerticalError { get; set; }

        #endregion

    }

    #endregion

    #region MSHealthActivitySegment class

    /// <summary>
    /// Represents a segment associated with an activity.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthActivitySegment
    {

        #region Properties

        /// <summary>
        /// Gets or sets the segment type.
        /// </summary>
        [JsonProperty(PropertyName = "segmentType",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthSegmentType Type { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the segment.
        /// </summary>
        [JsonProperty(PropertyName = "segmentId",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the start time of the segment.
        /// </summary>
        [JsonProperty(PropertyName = "startTime",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the segment.
        /// </summary>
        [JsonProperty(PropertyName = "endTime",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Gets or sets the duration of the segment.
        /// </summary>
        [JsonProperty(PropertyName = "duration",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Gets or sets the summary of calories burned during the segment.
        /// </summary>
        [JsonProperty(PropertyName = "caloriesBurnedSummary",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthCaloriesBurnedSummary CaloriesBurnedSummary { get; set; }

        /// <summary>
        /// Gets or sets the heart rate summary for the segment.
        /// </summary>
        [JsonProperty(PropertyName = "heartRateSummary",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthHeartRateSummary HeartRateSummary { get; set; }

        /// <summary>
        /// Gets or sets the breakdown of the heart rate zones during the segment.
        /// </summary>
        [JsonProperty(PropertyName = "heartRateZones",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthHeartRateZones HeartRateZones { get; set; }

        /// <summary>
        /// Gets or sets the split distance used for the segment.
        /// </summary>
        [JsonProperty(PropertyName = "splitDistance",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? SplitDistance { get; set; }

        /// <summary>
        /// Gets or sets the summary of distance data during the segment.
        /// </summary>
        [JsonProperty(PropertyName = "distanceSummary",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthDistanceSummary DistanceSummary { get; set; }

        /// <summary>
        /// Gets or sets the length of time the user was paused during the segment.
        /// </summary>
        [JsonProperty(PropertyName = "pausedDuration",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public TimeSpan? PausedDuration { get; set; }

        /// <summary>
        /// Gets or sets the ordinal of the circuit within the workout.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthSegmentType.GuidedWorkout"/>.
        /// </remarks>
        [JsonProperty(PropertyName = "circuitOrdinal",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? CircuitOrdinal { get; set; }

        /// <summary>
        /// Gets or sets the type of the circuit.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthSegmentType.GuidedWorkout"/>.
        /// </remarks>
        [JsonProperty(PropertyName = "circuitType",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? CircuitType { get; set; }

        /// <summary>
        /// Gets or sets the length of time in minutes the user was asleep during the segment.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthSegmentType.Sleep"/>.
        /// </remarks>
        [JsonProperty(PropertyName = "sleepTime",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? SleepTime { get; set; }

        /// <summary>
        /// Gets or sets the mapping of an event to a logical date.
        /// </summary>
        /// <remarks>
        /// This is the same as the DayId for the sleep activity.
        /// Only available on <see cref="MSHealthSegmentType.Sleep"/>.
        /// </remarks>
        [JsonProperty(PropertyName = "dayId",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public DateTime? DayId { get; set; }

        /// <summary>
        /// Gets or sets the sleep state.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthSegmentType.Sleep"/>.
        /// </remarks>
        [JsonProperty(PropertyName = "sleepType",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthSleepType SleepType { get; set; }

        /// <summary>
        /// Gets or sets the hole number on the golf course.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthSegmentType.GolfHole"/>.
        /// </remarks>
        [JsonProperty(PropertyName = "holeNumber",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? HoleNumber { get; set; }

        /// <summary>
        /// Gets or sets the steps taken by the user for the hole.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthSegmentType.GolfHole"/>.
        /// </remarks>
        [JsonProperty(PropertyName = "stepCount",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? StepCount { get; set; }

        /// <summary>
        /// Gets or sets the distance walked by the user for the hole.
        /// </summary>
        /// <remarks>
        /// Only available on <see cref="MSHealthSegmentType.GolfHole"/>.
        /// </remarks>
        [JsonProperty(PropertyName = "distanceWalked",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int? DistanceWalked { get; set; }

        #endregion

    }

    #endregion

    #region MSHealthActivityType enum

    /// <summary>
    /// Represents type of activity registered.
    /// </summary>
    [Flags]
    public enum MSHealthActivityType
    {
        /// <summary>
        /// Unknown activity.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Custom activity.
        /// </summary>
        Custom = 1,
        /// <summary>
        /// Custom exercise activity.
        /// </summary>
        CustomExercise = 2,
        /// <summary>
        /// Custom composite activity.
        /// </summary>
        CustomComposite = 4,
        /// <summary>
        /// Run activity.
        /// </summary>
        Run = 8,
        /// <summary>
        /// Sleep activity.
        /// </summary>
        Sleep = 16,
        /// <summary>
        /// Free play activity.
        /// </summary>
        FreePlay = 32,
        /// <summary>
        /// Guided workout activity.
        /// </summary>
        GuidedWorkout = 64,
        /// <summary>
        /// Bike activity.
        /// </summary>
        Bike = 128,
        /// <summary>
        /// Golf activity.
        /// </summary>
        Golf = 256,
        /// <summary>
        /// Regular exercise activity.
        /// </summary>
        RegularExercise = 512,
    }

    #endregion

    #region MSHealthActivityInclude enum

    /// <summary>
    /// Represents details of activity to include in requests to Microsoft Health Cloud API.
    /// </summary>
    [Flags]
    public enum MSHealthActivityInclude
    {
        /// <summary>
        /// No details
        /// </summary>
        None = 0,
        /// <summary>
        /// Include details.
        /// </summary>
        Details = 1,
        /// <summary>
        /// Include minute summaries.
        /// </summary>
        MinuteSummaries = 2,
        /// <summary>
        /// Include map points
        /// </summary>
        MapPoints = 4,
    }

    #endregion

    #region MSHealthSplitDistanceType enum

    /// <summary>
    /// Represents the length of splits used in each activity.
    /// </summary>
    public enum MSHealthSplitDistanceType
    {
        /// <summary>
        /// No length.
        /// </summary>
        None,
        /// <summary>
        /// Distance length in miles.
        /// </summary>
        Mile,
        /// <summary>
        /// Distance length in kilometers.
        /// </summary>
        Kilometer,
    }

    #endregion

    #region MSHealthMapPointType enum

    /// <summary>
    /// Represents type of map point in activity.-
    /// </summary>
    public enum MSHealthMapPointType
    {
        /// <summary>
        /// Unknown map point type.
        /// </summary>
        Unknown,
        /// <summary>
        /// Start of activity.
        /// </summary>
        Start,
        /// <summary>
        /// End of activity.
        /// </summary>
        End,
        /// <summary>
        /// Split of activity.
        /// </summary>
        Split,
        /// <summary>
        /// Waypoint of activity.
        /// </summary>
        Waypoint,
    }

    #endregion

    #region MSHealthSegmentType enum

    /// <summary>
    /// Represents the segment type of an activity.
    /// </summary>
    public enum MSHealthSegmentType
    {
        /// <summary>
        /// Unknown segment type.
        /// </summary>
        Unknown,
        /// <summary>
        /// Run segment type.
        /// </summary>
        Run,
        /// <summary>
        /// Free play segment type.
        /// </summary>
        FreePlay,
        /// <summary>
        /// Doze segment type.
        /// </summary>
        Doze,
        /// <summary>
        /// Sleep segment type.
        /// </summary>
        Sleep,
        /// <summary>
        /// Snooze segment type.
        /// </summary>
        Snooze,
        /// <summary>
        /// Awake segment type.
        /// </summary>
        Awake,
        /// <summary>
        /// Guided workout segment type.
        /// </summary>
        GuidedWorkout,
        /// <summary>
        /// Bike segment type.
        /// </summary>
        Bike,
        /// <summary>
        /// Pause segment type.
        /// </summary>
        Pause,
        /// <summary>
        /// Resume segment type.
        /// </summary>
        Resume,
        /// <summary>
        /// Distance based interval segment type.
        /// </summary>
        DistanceBasedInterval,
        /// <summary>
        /// Time based interval segment type.
        /// </summary>
        TimeBasedInterval,
        /// <summary>
        /// Golf hole segment type.
        /// </summary>
        GolfHole,
        /// <summary>
        /// Golf shot segment type.
        /// </summary>
        GolfShot,
        /// <summary>
        /// Not worn segment type.
        /// </summary>
        NotWorn,
    }

    #endregion

    #region MSHealthSleepType enum

    /// <summary>
    /// Represents the sleep state.
    /// </summary>
    public enum MSHealthSleepType
    {
        /// <summary>
        /// Unknown sleep state.
        /// </summary>
        Unknown,
        /// <summary>
        /// Undifferentiated sleep state.
        /// </summary>
        UndifferentiatedSleep,
        /// <summary>
        /// Restless sleep state.
        /// </summary>
        RestlessSleep,
        /// <summary>
        /// Restful sleep state.
        /// </summary>
        RestfulSleep
    }

    #endregion

}
