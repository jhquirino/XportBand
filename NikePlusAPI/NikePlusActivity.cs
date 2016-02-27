//-----------------------------------------------------------------------
// <copyright file="NikePlusActivity.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace NikePlusAPI
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;
    using NodaTime.TimeZones;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    #region NikePlusActivities class

    /// <summary>
    /// Represents a collection of user's Activities.
    /// </summary>
    [JsonObject]
    public sealed class NikePlusActivities
    {

        #region Properties

        /// <summary>
        /// Gets or sets the collection of activities.
        /// </summary>
        [JsonProperty(PropertyName = "data",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<NikePlusActivity> Activities { get; set; }

        /// <summary>
        /// Gets or sets the paging data for activities.
        /// </summary>
        [JsonProperty(PropertyName = "paging",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public NikePlusPaging Paging { get; set; }

        #endregion

    }

    #endregion

    #region NikePlusActivity class

    /// <summary>
    /// Represents a single user's Activity.
    /// </summary>
    [JsonObject]
    public sealed class NikePlusActivity
    {

        #region Properties

        /// <summary>
        /// Gets or sets the list of links to related resources.
        /// </summary>
        [JsonProperty(PropertyName = "links",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<NikePlusActivityLink> Links { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the activity.
        /// </summary>
        [JsonProperty(PropertyName = "activityId",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the type of activity performed.
        /// </summary>
        [JsonProperty(PropertyName = "activityType",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        [JsonConverter(typeof(StringEnumConverter))]
        public NikePlusActivityType Type { get; set; }

        /// <summary>
        /// Gets or sets the start time of the activity.
        /// </summary>
        [JsonProperty(PropertyName = "startTime",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the time zone name in which the activity occurred.
        /// </summary>
        /// <remarks>
        /// Nike+ supports political time zones found in the IANA Time Zone Database.
        /// </remarks>
        [JsonProperty(PropertyName = "activityTimeZone",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string ActivityTimeZone { get; set; }

        /// <summary>
        /// Gets or sets the status of the activity.
        /// </summary>
        [JsonProperty(PropertyName = "status",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        [JsonConverter(typeof(StringEnumConverter))]
        public NikePlusActivityStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the device used to record the activity.
        /// </summary>
        [JsonProperty(PropertyName = "deviceType",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        [JsonConverter(typeof(StringEnumConverter))]
        public NikePlusDeviceType DeviceType { get; set; }

        /// <summary>
        /// Gets or sets the duration of the activity.
        /// </summary>
        [JsonProperty(PropertyName = "duration",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? Duration { get; set; }

        /// <summary>
        /// Gets or sets the device identifier used to record the activity.
        /// </summary>
        [JsonProperty(PropertyName = "deviceName",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string DeviceName { get; set; }


        /// <summary>
        /// Gets or sets the summary metric data for the activity.
        /// </summary>
        [JsonProperty(PropertyName = "metricSummary",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public NikePlusMetricSummary MetricSummary { get; set; }

        /// <summary>
        /// Gets or sets the list of additional details the user entered about the activity.
        /// </summary>
        [JsonProperty(PropertyName = "tags",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<NikePlusActivityTag> Tags { get; set; }

        /// <summary>
        /// Gets or sets the list of metric data (interval unit, interval value, and Metric Type) and metric values.
        /// </summary>
        [JsonProperty(PropertyName = "metrics",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<NikePlusMetric> Metrics { get; set; }

        /// <summary>
        /// Gets or sets a value to determine whether this activity contains GPS data.
        /// </summary>
        [JsonProperty(PropertyName = "isGpsActivity",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public bool? IsGPSActivity { get; set; }

        /// <summary>
        /// Gets or sets the summary of activity.
        /// </summary>
        /// <remarks>
        /// Only useful for Nike+ API v2.0 calls.
        /// </remarks>
        [JsonProperty(PropertyName = "summary",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public NikePlusActivitySummaryV2 SummaryV2 { get; set; }


        /// <summary>
        /// Gets or sets the GPS data for the user's Activities.
        /// </summary>
        [JsonIgnore]
        public NikePlusGPS GPSData { get; set; }

        /// <summary>
        /// Gets or sets the GPX document for location
        /// </summary>
        [JsonIgnore]
        public XDocument GPX { get; set; }

        #endregion

    }

    #endregion

    #region NikePlusActivityLink class

    /// <summary>
    /// Represents links to related resources in HATEOAS format.
    /// </summary>
    /// <remarks>
    /// HATEOAS (Hypermedia as the Engine of Application State) is a constraint of the REST application architecture.
    /// </remarks>
    [JsonObject]
    public sealed class NikePlusActivityLink
    {

        #region Properties

        /// <summary>
        /// Gets or sets the relationship of the resource.
        /// </summary>
        [JsonProperty(PropertyName = "rel",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Relationship { get; set; }

        /// <summary>
        /// Gets or sets the complete URL that uniquely defines the resource.
        /// </summary>
        [JsonProperty(PropertyName = "href",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string URL { get; set; }

        #endregion

    }

    #endregion

    #region NikePlusMetricSummary class

    /// <summary>
    /// Represents summary metric data for Activity.
    /// </summary>
    [JsonObject]
    public sealed class NikePlusMetricSummary
    {

        #region Properties

        /// <summary>
        /// Gets or sets the number of calories burned during the activity.
        /// </summary>
        [JsonProperty(PropertyName = "calories",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? Calories { get; set; }

        /// <summary>
        /// Gets or sets the NikeFuel earned during the activity.
        /// </summary>
        [JsonProperty(PropertyName = "fuel",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? Fuel { get; set; }

        /// <summary>
        /// Gets or sets the distance traveled during the activity.
        /// </summary>
        [JsonProperty(PropertyName = "distance",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? Distance { get; set; }

        /// <summary>
        /// Gets or sets the Number of steps taken during the activity.
        /// </summary>
        [JsonProperty(PropertyName = "steps",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? Steps { get; set; }

        /// <summary>
        /// Gets or sets the amount of time the activity lasted.
        /// </summary>
        [JsonProperty(PropertyName = "duration",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public TimeSpan? Duration { get; set; }

        #endregion

    }

    #endregion

    #region NikePlusActivityTag class

    /// <summary>
    /// Represents additional details the user entered about an activity.
    /// </summary>
    [JsonObject]
    public sealed class NikePlusActivityTag
    {

        #region Properties

        /// <summary>
        /// Gets or sets the type of tag.
        /// </summary>
        [JsonProperty(PropertyName = "tagType",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value of tag.
        /// </summary>
        [JsonProperty(PropertyName = "tagValue",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Value { get; set; }

        #endregion

    }

    #endregion

    #region NikePlusMetric class

    /// <summary>
    /// Represents Metric data (interval unit, interval value, and Metric Type) and the array of metric values.
    /// </summary>
    [JsonObject]
    public sealed class NikePlusMetric
    {

        #region Properties

        /// <summary>
        /// Gets or sets interval in which the metric's values were sampled.
        /// </summary>
        /// <remarks>
        /// Time interval value in seconds: 1, 5, 10.
        /// </remarks>
        [JsonProperty(PropertyName = "intervalMetric",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public long? IntervalMetric { get; set; }

        /// <summary>
        /// Gets or sets the unit of the metric's interval value (<see cref="IntervalMetric"/>).
        /// </summary>
        [JsonProperty(PropertyName = "intervalUnit",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public NikePlusIntervalUnit IntervalUnit { get; set; }

        /// <summary>
        /// Gets or sets the type of the metric.
        /// </summary>
        [JsonProperty(PropertyName = "metricType",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        [JsonConverter(typeof(StringEnumConverter))]
        public NikePlusMetricType Type { get; set; }

        /// <summary>
        /// Gets or sets the list of values for the metric.
        /// </summary>
        [JsonProperty(PropertyName = "values",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<string> Values { get; set; }

        #endregion

    }

    #endregion

    #region NikePlusGPS class

    /// <summary>
    /// Represents GPS data for the user's Activities.
    /// </summary>
    [JsonObject]
    public sealed class NikePlusGPS
    {

        #region Properties

        /// <summary>
        /// Gets or sets the list of links to related resources.
        /// </summary>
        [JsonProperty(PropertyName = "links",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<NikePlusActivityLink> Links { get; set; }

        /// <summary>
        /// Gets or sets the total elevation loss over all the provided coordinates.
        /// </summary>
        [JsonProperty(PropertyName = "elevationLoss",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? ElevationLoss { get; set; }

        /// <summary>
        /// Gets or sets the total elevation gain over all the provided coordinates.
        /// </summary>
        [JsonProperty(PropertyName = "elevationGain",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? ElevationGain { get; set; }

        /// <summary>
        /// Gets or sets the maximum elevation value.
        /// </summary>
        [JsonProperty(PropertyName = "elevationMax",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? ElevationMax { get; set; }

        /// <summary>
        /// Gets or sets the minimum elevation value.
        /// </summary>
        [JsonProperty(PropertyName = "elevationMin",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? ElevationMin { get; set; }

        /// <summary>
        /// Gets or sets the interval in which the GPS coordinate values were sampled.
        /// </summary>
        [JsonProperty(PropertyName = "intervalMetric",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? IntervalMetric { get; set; }

        /// <summary>
        /// Gets or sets the unit of the metric's interval value.
        /// </summary>
        [JsonProperty(PropertyName = "intervalUnit",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public NikePlusIntervalUnit IntervalUnit { get; set; }

        /// <summary>
        /// Gets or sets the GPS coordinates.
        /// </summary>
        [JsonProperty(PropertyName = "waypoints",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<NikePlusWaypoint> Waypoints { get; set; }

        #endregion

    }

    #endregion

    #region NikePlusWaypoint class

    /// <summary>
    /// Represents GPS coordinates.
    /// </summary>
    [JsonObject]
    public sealed class NikePlusWaypoint
    {

        #region Properties

        /// <summary>
        /// Gets or sets the latitude value of this GPS coordinate.
        /// </summary>
        [JsonProperty(PropertyName = "latitude",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude value of this GPS coordinate.
        /// </summary>
        [JsonProperty(PropertyName = "longitude",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the elevation value of this GPS coordinate.
        /// </summary>
        [JsonProperty(PropertyName = "elevation",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? Elevation { get; set; }

        #region Not API properties to map with MSHealth

        #endregion

        #endregion

    }

    #endregion

    #region NikePlusPaging class

    /// <summary>
    /// Represents paging information for Nike+ responses.
    /// </summary>
    public sealed class NikePlusPaging
    {

        #region Properties

        /// <summary>
        /// Gets or sets the URL for next page.
        /// </summary>
        [JsonProperty(PropertyName = "next",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Next { get; set; }

        /// <summary>
        /// Gets or sets the URL for previous page.
        /// </summary>
        [JsonProperty(PropertyName = "previous",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Previous { get; set; }

        #endregion

    }

    #endregion

    #region NikePlusActivityType enum

    /// <summary>
    /// Classification of the kind of activity a user performs.
    /// </summary>
    public enum NikePlusActivityType
    {
        /// <summary>
        /// Other activities.
        /// </summary>
        OTHER,
        /// <summary>
        /// Badminton activity.
        /// </summary>
        BADMINTON,
        /// <summary>
        /// Baseball activity.
        /// </summary>
        BASEBALL,
        /// <summary>
        /// Basketball activity.
        /// </summary>
        BASKETBALL,
        /// <summary>
        /// Beach Volleyball activity.
        /// </summary>
        BEACH_VOLLEYBALL,
        /// <summary>
        /// Bootcamp activity.
        /// </summary>
        BOOTCAMP,
        /// <summary>
        /// Bowling activity.
        /// </summary>
        BOWLING,
        /// <summary>
        /// Boxing activity.
        /// </summary>
        BOXING,
        /// <summary>
        /// Calisthenics activity.
        /// </summary>
        CALISTHENICS,
        /// <summary>
        /// Circuit Training activity.
        /// </summary>
        CIRCUIT_TRAINING,
        /// <summary>
        /// Cleaning activity.
        /// </summary>
        CLEANING,
        /// <summary>
        /// Clubbing activity.
        /// </summary>
        CLUBBING,
        /// <summary>
        /// Cooking activity.
        /// </summary>
        COOKING,
        /// <summary>
        /// Cricket activity.
        /// </summary>
        CRICKET,
        /// <summary>
        /// Cross Country activity.
        /// </summary>
        CROSS_COUNTRY,
        /// <summary>
        /// Cross Training activity.
        /// </summary>
        CROSS_TRAINING,
        /// <summary>
        /// Curling activity.
        /// </summary>
        CURLING,
        /// <summary>
        /// Cycle activity.
        /// </summary>
        CYCLE,
        /// <summary>
        /// Cyclocross activity.
        /// </summary>
        CYCLOCROSS,
        /// <summary>
        /// Dancing activity.
        /// </summary>
        DANCING,
        /// <summary>
        /// Djing activity.
        /// </summary>
        DJING,
        /// <summary>
        /// Dodgeball activity.
        /// </summary>
        DODGEBALL,
        /// <summary>
        /// Dog Walking activity.
        /// </summary>
        DOG_WALKING,
        /// <summary>
        /// Drumming activity.
        /// </summary>
        DRUMMING,
        /// <summary>
        /// Elliptical activity.
        /// </summary>
        ELLIPTICAL,
        /// <summary>
        /// Fencing activity.
        /// </summary>
        FENCING,
        /// <summary>
        /// Field Hockey activity.
        /// </summary>
        FIELD_HOCKEY,
        /// <summary>
        /// Fishing activity.
        /// </summary>
        FISHING,
        /// <summary>
        /// Flag Football activity.
        /// </summary>
        FLAG_FOOTBALL,
        /// <summary>
        /// Fly Fishing activity.
        /// </summary>
        FLY_FISHING,
        /// <summary>
        /// Football activity.
        /// </summary>
        FOOTBALL,
        /// <summary>
        /// Gaming activity.
        /// </summary>
        GAMING,
        /// <summary>
        /// Golf activity.
        /// </summary>
        GOLF,
        /// <summary>
        /// Gymnastics activity.
        /// </summary>
        GYMNASTICS,
        /// <summary>
        /// Handball activity.
        /// </summary>
        HANDBALL,
        /// <summary>
        /// Hiit activity.
        /// </summary>
        HIIT,
        /// <summary>
        /// Hiking activity.
        /// </summary>
        HIKING,
        /// <summary>
        /// Horseback Riding activity.
        /// </summary>
        HORSEBACK_RIDING,
        /// <summary>
        /// Ice Climbing activity.
        /// </summary>
        ICE_CLIMBING,
        /// <summary>
        /// Ice Hockey activity.
        /// </summary>
        ICE_HOCKEY,
        /// <summary>
        /// Ice Skating activity.
        /// </summary>
        ICE_SKATING,
        /// <summary>
        /// Ironing activity.
        /// </summary>
        IRONING,
        /// <summary>
        /// Jogging activity.
        /// </summary>
        JOGGING,
        /// <summary>
        /// Karate activity.
        /// </summary>
        KARATE,
        /// <summary>
        /// Kayaking activity.
        /// </summary>
        KAYAKING,
        /// <summary>
        /// Kickball activity.
        /// </summary>
        KICKBALL,
        /// <summary>
        /// Kickboxing activity.
        /// </summary>
        KICKBOXING,
        /// <summary>
        /// Kite Flying activity.
        /// </summary>
        KITE_FLYING,
        /// <summary>
        /// Martial Arts activity.
        /// </summary>
        MARTIAL_ARTS,
        /// <summary>
        /// Mini Golf activity.
        /// </summary>
        MINI_GOLF,
        /// <summary>
        /// Mixed Martial Arts activity.
        /// </summary>
        MIXED_MARTIAL_ARTS,
        /// <summary>
        /// Mountain Biking activity.
        /// </summary>
        MOUNTAIN_BIKING,
        /// <summary>
        /// Mountaineering activity.
        /// </summary>
        MOUNTAINEERING,
        /// <summary>
        /// Mowing activity.
        /// </summary>
        MOWING,
        /// <summary>
        /// Paddling activity.
        /// </summary>
        PADDLING,
        /// <summary>
        /// Pilates activity.
        /// </summary>
        PILATES,
        /// <summary>
        /// Pool activity.
        /// </summary>
        POOL,
        /// <summary>
        /// Racquetball activity.
        /// </summary>
        RACQUETBALL,
        /// <summary>
        /// Rock Climbing activity.
        /// </summary>
        ROCK_CLIMBING,
        /// <summary>
        /// Rollerblading activity.
        /// </summary>
        ROLLERBLADING,
        /// <summary>
        /// Rollerskating activity.
        /// </summary>
        ROLLERSKATING,
        /// <summary>
        /// Rowing activity.
        /// </summary>
        ROWING,
        /// <summary>
        /// Rugby activity.
        /// </summary>
        RUGBY,
        /// <summary>
        /// Run activity.
        /// </summary>
        RUN,
        /// <summary>
        /// Sailing activity.
        /// </summary>
        SAILING,
        /// <summary>
        /// Shopping activity.
        /// </summary>
        SHOPPING,
        /// <summary>
        /// Shoveling activity.
        /// </summary>
        SHOVELING,
        /// <summary>
        /// Shuffleboard activity.
        /// </summary>
        SHUFFLEBOARD,
        /// <summary>
        /// Skateboarding activity.
        /// </summary>
        SKATEBOARDING,
        /// <summary>
        /// Skiing activity.
        /// </summary>
        SKIING,
        /// <summary>
        /// Sleeping activity.
        /// </summary>
        SLEEPING,
        /// <summary>
        /// Snowboarding activity.
        /// </summary>
        SNOWBOARDING,
        /// <summary>
        /// Soccer activity.
        /// </summary>
        SOCCER,
        /// <summary>
        /// Softball activity.
        /// </summary>
        SOFTBALL,
        /// <summary>
        /// Spinning activity.
        /// </summary>
        SPINNING,
        /// <summary>
        /// Squash activity.
        /// </summary>
        SQUASH,
        /// <summary>
        /// Stair Climber activity.
        /// </summary>
        STAIR_CLIMBER,
        /// <summary>
        /// Stationary Biking activity.
        /// </summary>
        STATIONARY_BIKING,
        /// <summary>
        /// Swimming activity.
        /// </summary>
        SWIMMING,
        /// <summary>
        /// Table Tennis activity.
        /// </summary>
        TABLE_TENNIS,
        /// <summary>
        /// Tennis activity.
        /// </summary>
        TENNIS,
        /// <summary>
        /// Training activity.
        /// </summary>
        TRAINING,
        /// <summary>
        /// Volleyball activity.
        /// </summary>
        VOLLEYBALL,
        /// <summary>
        /// Walk activity.
        /// </summary>
        WALK,
        /// <summary>
        /// Weight Lifting activity.
        /// </summary>
        WEIGHT_LIFTING,
        /// <summary>
        /// Weight Training activity.
        /// </summary>
        WEIGHT_TRAINING,
        /// <summary>
        /// Yoga activity.
        /// </summary>
        YOGA,
        /// <summary>
        /// HeartRate-Only activity.
        /// </summary>
        HEARTRATE_ONLY,
        /// <summary>
        /// All Day activity.
        /// </summary>
        ALL_DAY,
    }

    #endregion

    #region NikePlusActivityStatus enum

    /// <summary>
    /// Status of an activity.
    /// </summary>
    public enum NikePlusActivityStatus
    {
        /// <summary>
        /// None (unknown) Status.
        /// </summary>
        NONE,
        /// <summary>
        /// Activity in progress.
        /// </summary>
        IN_PROGRESS,
        /// <summary>
        /// Activity completed.
        /// </summary>
        COMPLETE,
    }

    #endregion

    #region NikePlusDeviceType enum

    /// <summary>
    /// Device type used for an activity.
    /// </summary>
    public enum NikePlusDeviceType
    {
        /// <summary>
        /// Other device.
        /// </summary>
        OTHER,
        /// <summary>
        /// Bike.
        /// </summary>
        BIKE,
        /// <summary>
        /// Elliptical.
        /// </summary>
        ELLIPTICAL,
        /// <summary>
        /// Heart Rate.
        /// </summary>
        HEARTRATE,
        /// <summary>
        /// Pedometer.
        /// </summary>
        PEDOMETER,
        /// <summary>
        /// Phone.
        /// </summary>
        PHONE,
        /// <summary>
        /// Stepper.
        /// </summary>
        STEPPER,
        /// <summary>
        /// Tablet.
        /// </summary>
        TABLET,
        /// <summary>
        /// Treadmill.
        /// </summary>
        TREADMILL,
        /// <summary>
        /// Watch.
        /// </summary>
        WATCH,
        /// <summary>
        /// Wearable
        /// </summary>
        WEARABLE,
        /* Custom */
        /// <summary>
        /// iPod.
        /// </summary>
        IPOD,
        /// <summary>
        /// iPhone.
        /// </summary>
        IPHONE,
        /// <summary>
        /// Microsoft Band.
        /// </summary>
        MSBAND,
    }

    #endregion

    #region NikePlusMetricType enum

    /// <summary>
    /// 
    /// </summary>
    public enum NikePlusMetricType
    {
        /// <summary>
        /// Elevation of the user for a single data point in meters.
        /// </summary>
        ELEVATION,
        /// <summary>
        /// Strength of GPS signal (value between 0 and 100).
        /// </summary>
        GPSSIGNALSTRENGTH,
        /// <summary>
        /// Latitude of the user for a single data point.
        /// </summary>
        LATITUDE,
        /// <summary>
        /// Longitude of the user for a single data point.
        /// </summary>
        LONGITUDE,
        /// <summary>
        /// Calories the user burned.
        /// </summary>
        CALORIES,
        /// <summary>
        /// Distance the user traveled.
        /// </summary>
        DISTANCE,
        /// <summary>
        /// Duration of Activity.
        /// </summary>
        DURATION,
        /// <summary>
        /// NikeFuel the user earned.
        /// </summary>
        FUEL,
        /// <summary>
        /// Ratio of rise over run (rise/run).
        /// </summary>
        GRADE,
        /// <summary>
        /// Heart rate of the user.
        /// </summary>
        HEARTRATE,
        /// <summary>
        /// METS are metabolic equivalents.
        /// </summary>
        /// <remarks>
        /// Currently unavailable. One MET is defined as the energy it takes to
        /// sit quietly. These MET estimates are for healthy adults.
        /// </remarks>
        METS,
        /// <summary>
        /// Speed of the user.
        /// </summary>
        SPEED,
        /// <summary>
        /// "Hours Won" value for the Activity.
        /// </summary>
        STARS,
        /// <summary>
        /// Steps the user took.
        /// </summary>
        STEPS,
        /// <summary>
        /// Maximum rate of oxygen consumption by user.
        /// </summary>
        /// <remarks>
        /// Currently unavailable.
        /// </remarks>
        VO2,
        /// <summary>
        /// Watts generated by user.
        /// </summary>
        WATTS,
        /// <summary>
        /// Age of the user.
        /// </summary>
        /// <remarks>
        /// Currently unavailable.
        /// </remarks>
        AGE,
        /// <summary>
        /// Resting heart rate of user.
        /// </summary>
        /// <remarks>
        /// Currently unavailable.
        /// </remarks>
        RESTINGHEARTRATE,
        /// <summary>
        /// Weight of the user.
        /// </summary>
        /// <remarks>
        /// Currently unavailable.
        /// </remarks>
        WEIGHT,
    }

    #endregion

    #region NikePlusIntervalUnit enum

    /// <summary>
    /// Unit of the time interval for an activity.
    /// </summary>
    public enum NikePlusIntervalUnit
    {
        /// <summary>
        /// Interval: none.
        /// </summary>
        NONE,
        /// <summary>
        /// Interval: seconds.
        /// </summary>
        SEC,
        /// <summary>
        /// Interval: minutes.
        /// </summary>
        MIN,
    }

    #endregion

    #region NikePlusActivitySummaryV2 class

    /// <summary>
    /// Represents summary for activity.
    /// </summary>
    /// <remarks>
    /// Only useful when using Nike+ API v2.0
    /// </remarks>
    public sealed class NikePlusActivitySummaryV2
    {

        #region Properties

        /// <summary>
        /// Gets or sets the collection of snapshots.
        /// </summary>
        [JsonProperty(PropertyName = "snapshots",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<NikePlusSnapshotV2> Snapshots { get; set; }

        /// <summary>
        /// Gets or sets the collection of device configurations.
        /// </summary>
        [JsonProperty(PropertyName = "deviceConfig",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<NikePlusDeviceConfigV2> DeviceConfig { get; set; }

        #endregion

    }

    #endregion

    #region NikePlusSnapshotV2 class

    /// <summary>
    /// Represents a snapshot for activity summary.
    /// </summary>
    public sealed class NikePlusSnapshotV2
    {

        #region Properties

        /// <summary>
        /// Gets or sets the name of snapshot
        /// </summary>
        [JsonProperty(PropertyName = "name",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the collection of data series for snapshot.
        /// </summary>
        [JsonProperty(PropertyName = "dataSeries",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<NikePlusDataSerieV2> DataSeries { get; set; }

        #endregion

    }

    #endregion

    #region NikePlusDataSerieV2 class

    /// <summary>
    /// Represents a data serie for snapshot.
    /// </summary>
    public sealed class NikePlusDataSerieV2
    {

        #region Properties

        /// <summary>
        /// Gets or sets the collection of metrics for data serie.
        /// </summary>
        [JsonProperty(PropertyName = "metrics",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public NikePlusMetricV2 Metrics { get; set; }

        /// <summary>
        /// Gets or sets the object type for data serie.
        /// </summary>
        [JsonProperty(PropertyName = "objType",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string ObjectType { get; set; }

        #endregion

    }

    #endregion

    #region NikePlusMetricV2 class

    /// <summary>
    /// Represents metric for snapshot data serie.
    /// </summary>
    public sealed class NikePlusMetricV2
    {

        #region Properties

        /// <summary>
        /// Gets or sets the distance for metric.
        /// </summary>
        [JsonProperty(PropertyName = "distance",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? Distance { get; set; }

        /// <summary>
        /// Gets or sets the duration for metric.
        /// </summary>
        [JsonProperty(PropertyName = "duration",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public double? Duration { get; set; }

        #endregion

    }

    #endregion

    #region NikePlusDeviceConfigV2 class

    /// <summary>
    /// Represents device configuration for activity summary.
    /// </summary>
    public sealed class NikePlusDeviceConfigV2
    {

        #region Properties

        /// <summary>
        /// Gets or sets the component for device configuration.
        /// </summary>
        [JsonProperty(PropertyName = "component",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public NikePlusDeviceComponentV2 Component { get; set; }

        #endregion

    }

    #endregion

    #region NikePlusDeviceComponentV2 class

    /// <summary>
    /// Represents device component for activity summary.
    /// </summary>
    public sealed class NikePlusDeviceComponentV2
    {

        #region Properties

        /// <summary>
        /// Gets or sets the device type.
        /// </summary>
        [JsonProperty(PropertyName = "type",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public NikePlusDeviceType Type { get; set; }

        /// <summary>
        /// Gets or sets the device category.
        /// </summary>
        [JsonProperty(PropertyName = "category",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Category { get; set; }

        #endregion

    }

    #endregion

    #region NikePlusActivityPostConverter class

    /// <summary>
    /// Serializer to Post (v1) Nike+ Activity.
    /// </summary>
    public sealed class NikePlusActivityPostConverter : JsonConverter
    {

        #region Properties Overriding

        /// <summary>
        /// Gets a value indicating whether this JsonConverter can read JSON.
        /// </summary>
        /// <remarks>
        /// Since current class only works as Serializer (writer), it always returns <see langword="false"/>.
        /// </remarks>
        public override bool CanRead
        {
            get { return false; }
        }

        #endregion

        #region Methods Overriding

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><see langword="true"/> if this instance can convert the specified object type; otherwise, <see langword="false"/>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(NikePlusActivity);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        /// <remarks>
        /// Since current class only works as Serializer (writer), it always throws <see cref="NotImplementedException"/>.
        /// </remarks>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The <see cref="NikePlusActivity"/> to write.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            NikePlusActivity loActivity = value as NikePlusActivity;
            if (loActivity != null)
            {
                /*
				 * Activity type, device name/type, start time, duration
				 */
                JObject loJObjectRoot = new JObject();
                loJObjectRoot.Add(new JProperty("activityType", Enum.GetName(typeof(NikePlusActivityType), loActivity.Type)));
                if (!string.IsNullOrEmpty(loActivity.DeviceName))
                    loJObjectRoot.Add(new JProperty("deviceName", loActivity.DeviceName));
                loJObjectRoot.Add(new JProperty("deviceType", Enum.GetName(typeof(NikePlusDeviceType), loActivity.DeviceType)));
                loJObjectRoot.Add(new JProperty("startTime", (long)loActivity.StartTime
                                                .Value
                                                .ToUnixTimestamp()));
                if (loActivity.Duration != null)
                    loJObjectRoot.Add(new JProperty("duration", (long)loActivity.Duration));
                /* 
				 * Timezone
				 */
                // Get using Location and GoogleAPI
                if (string.IsNullOrEmpty(loActivity.ActivityTimeZone) &&
                    loActivity.GPSData != null &&
                    loActivity.GPSData.Waypoints != null &&
                    loActivity.GPSData.Waypoints.Any())
                {
                    // Get from first waypoint
                    NikePlusWaypoint loWaypoint = loActivity.GPSData.Waypoints.FirstOrDefault();
                    if (loWaypoint != null &&
                        loWaypoint.Latitude != null &&
                        loWaypoint.Longitude != null)
                    {
                        GoogleTimeZone loGoogleTimeZone = null;
#if WINDOWS_UWP
                        Task.Run(async () => loGoogleTimeZone = await DateTime.UtcNow.ToGoogleTimeZoneAsync(loWaypoint.Latitude.Value, loWaypoint.Longitude.Value)).Wait();
#elif DESKTOP_APP
                        loGoogleTimeZone = DateTime.UtcNow.ToGoogleTimeZone(loWaypoint.Latitude.Value, loWaypoint.Longitude.Value);
#endif
                        if (loGoogleTimeZone != null)
                            loActivity.ActivityTimeZone = loGoogleTimeZone.timeZoneId;
                    }
                }
                // Get using UTC/Local TimeZone
                if (string.IsNullOrEmpty(loActivity.ActivityTimeZone))
                {
                    TzdbDateTimeZoneSource loTzdbSource = TzdbDateTimeZoneSource.Default;
                    switch (loActivity.StartTime.Value.Kind)
                    {
                        case DateTimeKind.Utc:
                            loActivity.ActivityTimeZone = "Etc/UTC";
                            break;
                        default:
                            loActivity.ActivityTimeZone = loTzdbSource.MapTimeZoneId(TimeZoneInfo.Local);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(loActivity.ActivityTimeZone))
                    loJObjectRoot.Add(new JProperty("activityTimeZone", loActivity.ActivityTimeZone));
                /*
				 * Metrics
				 */
                if (loActivity.Metrics != null &&
                    loActivity.Metrics.Any())
                {
                    IList<NikePlusMetric> loMetrics = (from loMetric in loActivity.Metrics
                                                       where loMetric.IntervalUnit == NikePlusIntervalUnit.SEC &&
                                                       loMetric.IntervalMetric == 10 &&
                                                       loMetric.Values != null
                                                       select loMetric).ToList();
                    if (loMetrics.Any())
                    {
                        IList<string> loMetricTypes = (from loMetric in loMetrics
                                                       select Enum.GetName(typeof(NikePlusMetricType), loMetric.Type)).ToList();
                        IList<int> loMetricValuesCount = (from loMetric in loMetrics
                                                          select loMetric.Values.Count).ToList();
                        bool lbValidMetricDataCount = loMetricValuesCount.GroupBy(loCount => loCount).Count() == 1;

                        bool lbHasGPSData = loActivity.GPSData != null &&
                            loActivity.GPSData.IntervalUnit == NikePlusIntervalUnit.SEC &&
                            loActivity.GPSData.IntervalMetric == 10 &&
                            loActivity.GPSData.Waypoints != null &&
                            loActivity.GPSData.Waypoints.Any();
                        //loActivity.GPSData.Waypoints.Count == loMetricValuesCount[0] &&
                        //lbValidMetricDataCount;
                        if (lbHasGPSData)
                        {
                            loMetricTypes.Add(Enum.GetName(typeof(NikePlusMetricType), NikePlusMetricType.LATITUDE));
                            loMetricTypes.Add(Enum.GetName(typeof(NikePlusMetricType), NikePlusMetricType.LONGITUDE));
                            loMetricTypes.Add(Enum.GetName(typeof(NikePlusMetricType), NikePlusMetricType.ELEVATION));
                        }

                        if (lbValidMetricDataCount)
                        {
                            JObject loJObjectMetrics = new JObject();
                            loJObjectMetrics.Add(new JProperty("intervalUnit", "SEC"));
                            loJObjectMetrics.Add(new JProperty("intervalValue", 10));
                            loJObjectMetrics.Add(new JProperty("metricTypes", new JArray(loMetricTypes)));

                            List<JArray> loMetricData = new List<JArray>();
                            for (int liValueIndex = 0; liValueIndex < loMetrics[0].Values.Count; liValueIndex++)
                            {
                                List<double> loMetricDataItem = new List<double>();
                                foreach (NikePlusMetric loMetric in loMetrics)
                                {
                                    double ldDataValue = 0;
                                    double.TryParse(loMetric.Values[liValueIndex], out ldDataValue);
                                    loMetricDataItem.Add(ldDataValue);
                                }
                                if (lbHasGPSData &&
                                    loActivity.GPSData.Waypoints.Count > liValueIndex)
                                {
                                    loMetricDataItem.Add(loActivity.GPSData.Waypoints[liValueIndex].Latitude ?? 0);
                                    loMetricDataItem.Add(loActivity.GPSData.Waypoints[liValueIndex].Longitude ?? 0);
                                    loMetricDataItem.Add(loActivity.GPSData.Waypoints[liValueIndex].Elevation ?? 0);
                                }
                                loMetricData.Add(new JArray(loMetricDataItem));
                            }
                            loJObjectMetrics.Add(new JProperty("data", new JArray(loMetricData)));

                            loJObjectRoot.Add(new JProperty("metrics", loJObjectMetrics));
                        }
                    }
                }
                loJObjectRoot.WriteTo(writer);
            }
        }

        #endregion

    }

    #endregion

    #region NikePlusActivityPostConverterV2 class

    /// <summary>
    /// Serializer to Post (v2.0) Nike+ Activity.
    /// </summary>
    public sealed class NikePlusActivityPostConverterV2 : JsonConverter
    {

        #region Properties Overriding

        /// <summary>
        /// Gets a value indicating whether this JsonConverter can read JSON.
        /// </summary>
        /// <remarks>
        /// Since current class only works as Serializer (writer), it always returns <see langword="false"/>.
        /// </remarks>
        public override bool CanRead
        {
            get { return false; }
        }

        #endregion

        #region Methods Overriding

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><see langword="true"/> if this instance can convert the specified object type; otherwise, <see langword="false"/>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(NikePlusActivity);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        /// <remarks>
        /// Since current class only works as Serializer (writer), it always throws <see cref="NotImplementedException"/>.
        /// </remarks>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The <see cref="NikePlusActivity"/> to write.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            NikePlusActivity loActivity = value as NikePlusActivity;
            if (loActivity != null)
            {
                JObject loJObjectRoot = new JObject();
                /*
				 * Distance, Duration, StartTime, Status, Type, Calories
				 */
                if (loActivity.MetricSummary != null &&
                    loActivity.MetricSummary.Distance != null)
                    loJObjectRoot.Add(new JProperty("distance", loActivity.MetricSummary.Distance));
                if (loActivity.Duration != null)
                    loJObjectRoot.Add(new JProperty("duration", loActivity.Duration));
                else if (loActivity.MetricSummary != null &&
                         loActivity.MetricSummary.Duration != null)
                    loJObjectRoot.Add(new JProperty("duration", loActivity.MetricSummary.Duration.Value.TotalMilliseconds));
                loJObjectRoot.Add(new JProperty("startTime", loActivity.StartTime.Value.ToUnixTimestamp()));
                loJObjectRoot.Add(new JProperty("status", Enum.GetName(typeof(NikePlusActivityStatus), loActivity.Status)));
                loJObjectRoot.Add(new JProperty("type", Enum.GetName(typeof(NikePlusActivityType), loActivity.Type)));
                if (loActivity.MetricSummary != null &&
                    loActivity.MetricSummary.Calories != null)
                    loJObjectRoot.Add(new JProperty("calories", loActivity.MetricSummary.Calories));
                /* 
				 * Timezone
				 */
                // Get using Location and GoogleAPI
                if (string.IsNullOrEmpty(loActivity.ActivityTimeZone) &&
                    loActivity.GPSData != null &&
                    loActivity.GPSData.Waypoints != null &&
                    loActivity.GPSData.Waypoints.Any())
                {
                    // Get from first waypoint
                    NikePlusWaypoint loWaypoint = loActivity.GPSData.Waypoints.FirstOrDefault();
                    if (loWaypoint != null &&
                        loWaypoint.Latitude != null &&
                        loWaypoint.Longitude != null)
                    {
                        GoogleTimeZone loGoogleTimeZone = null;
#if WINDOWS_UWP
                        Task.Run(async () => loGoogleTimeZone = await DateTime.UtcNow.ToGoogleTimeZoneAsync(loWaypoint.Latitude.Value, loWaypoint.Longitude.Value)).Wait();
#elif DESKTOP_APP
                        loGoogleTimeZone = DateTime.UtcNow.ToGoogleTimeZone(loWaypoint.Latitude.Value, loWaypoint.Longitude.Value);
#endif
                        if (loGoogleTimeZone != null)
                            loActivity.ActivityTimeZone = loGoogleTimeZone.timeZoneId;
                    }
                }
                // Get using UTC/Local TimeZone
                if (string.IsNullOrEmpty(loActivity.ActivityTimeZone))
                {
                    TzdbDateTimeZoneSource loTzdbSource = TzdbDateTimeZoneSource.Default;
                    switch (loActivity.StartTime.Value.Kind)
                    {
                        case DateTimeKind.Utc:
                            loActivity.ActivityTimeZone = "Etc/UTC";
                            break;
                        default:
                            loActivity.ActivityTimeZone = loTzdbSource.MapTimeZoneId(TimeZoneInfo.Local);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(loActivity.ActivityTimeZone))
                    loJObjectRoot.Add(new JProperty("timeZoneId", loActivity.ActivityTimeZone));
                /*
				 * Device
				 */
                List<JObject> loDeviceConfigs = new List<JObject>();
                JObject loComponent = new JObject();
                loComponent.Add(new JProperty("type", Enum.GetName(typeof(NikePlusDeviceType), loActivity.DeviceType)));
                loComponent.Add(new JProperty("category", "device"));
                JObject loDeviceConfig = new JObject();
                loDeviceConfig.Add(new JProperty("component", loComponent));
                loDeviceConfigs.Add(loDeviceConfig);
                JObject loSummary = new JObject();
                loSummary.Add(new JProperty("deviceConfig", new JArray(loDeviceConfigs)));
                /*
				 * Snapshots
				 */
                if (loActivity.SummaryV2 != null &&
                    loActivity.SummaryV2.Snapshots != null &&
                    loActivity.SummaryV2.Snapshots.Any())
                {
                    IEnumerable<NikePlusMetricV2> loValidMetrics = (from loSnapshot in loActivity.SummaryV2.Snapshots
                                                                    from loDataSerie in loSnapshot.DataSeries
                                                                    where loSnapshot != null &&
                                                                    loSnapshot.DataSeries != null &&
                                                                    loDataSerie.Metrics != null &&
                                                                    loDataSerie.Metrics.Duration != null &&
                                                                    loDataSerie.Metrics.Distance != null
                                                                    select loDataSerie.Metrics);
                    if (loValidMetrics != null && loValidMetrics.Any())
                    {
                        List<JObject> loSnapshots = new List<JObject>();
                        foreach (NikePlusSnapshotV2 loSnapshotV2 in loActivity.SummaryV2.Snapshots)
                        {
                            JObject loSnapshot = new JObject();
                            if (loSnapshotV2.DataSeries != null &&
                                loSnapshotV2.DataSeries.Any())
                            {
                                List<JObject> loDataSeries = new List<JObject>();
                                foreach (NikePlusDataSerieV2 loDataSerieV2 in loSnapshotV2.DataSeries)
                                {
                                    JObject loDataSerie = new JObject();
                                    if (loDataSerieV2.Metrics != null &&
                                        loDataSerieV2.Metrics.Distance != null &&
                                        loDataSerieV2.Metrics.Duration != null)
                                    {
                                        JObject loMetric = new JObject();
                                        loMetric.Add(new JProperty("distance", loDataSerieV2.Metrics.Distance));
                                        loMetric.Add(new JProperty("duration", loDataSerieV2.Metrics.Duration));
                                        loDataSerie.Add(new JProperty("metrics", loMetric));
                                    }
                                    if (!string.IsNullOrEmpty(loDataSerieV2.ObjectType))
                                        loDataSerie.Add(new JProperty("objType", loDataSerieV2.ObjectType));
                                    loDataSeries.Add(loDataSerie);
                                }
                                loSnapshot.Add(new JProperty("dataSeries", new JArray(loDataSeries)));
                            }
                            if (!string.IsNullOrEmpty(loSnapshotV2.Name))
                                loSnapshot.Add(new JProperty("name", loSnapshotV2.Name));
                            loSnapshots.Add(loSnapshot);
                        }
                        loSummary.Add(new JProperty("snapshots", new JArray(loSnapshots)));
                    }
                }
                loJObjectRoot.Add(new JProperty("summary", loSummary));
                /*
				 * Detail
				 */
                if (loActivity.Metrics != null &&
                    loActivity.Metrics.Any())
                {
                    IList<NikePlusMetric> loMetrics = (from loMetric in loActivity.Metrics
                                                       where loMetric.Values != null
                                                       select loMetric).ToList();
                    if (loMetrics.Any())
                    {
                        List<JObject> loDetails = new List<JObject>();
                        foreach (NikePlusMetric loMetric in loMetrics)
                        {
                            JObject loDetail = new JObject();
                            loDetail.Add(new JProperty("startTimeOffset", 0));
                            loDetail.Add(new JProperty("intervalType", "time"));
                            loDetail.Add(new JProperty("intervalUnit", Enum.GetName(typeof(NikePlusIntervalUnit), loMetric.IntervalUnit)));
                            loDetail.Add(new JProperty("intervalMetric", loMetric.IntervalMetric));
                            loDetail.Add(new JProperty("metricType", Enum.GetName(typeof(NikePlusMetricType), loMetric.Type)));
                            loDetail.Add(new JProperty("value", new JArray(loMetric.Values)));
                            loDetail.Add(new JProperty("objType", "dataStream"));
                            loDetails.Add(loDetail);
                        }
                        loJObjectRoot.Add(new JProperty("detail", new JArray(loDetails)));
                    }
                }
                loJObjectRoot.WriteTo(writer);
            }
        }

        #endregion

    }

    #endregion

}
