//-----------------------------------------------------------------------
// <copyright file="MSHealthDevice.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace MSHealthAPI
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    #region MSHealthDevices class

    /// <summary>
    /// Represents details about the devices associated with user's Microsoft Health profile.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthDevices
    {

        #region Properties

        /// <summary>
        /// Gets or sets the collection of devices details.
        /// </summary>
        [JsonProperty(PropertyName = "deviceProfiles",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<MSHealthDevice> Devices { get; set; }

        /// <summary>
        /// Gets or sets the number of <see cref="Devices"/> returned.
        /// </summary>
        [JsonProperty(PropertyName = "itemCount",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int ItemCount { get; set; }

        #endregion

    }

    #endregion

    #region MSHealthDevice class

    /// <summary>
    /// Represents details about device.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthDevice
    {

        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier of the device.
        /// </summary>
        [JsonProperty(PropertyName = "id",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the name the user has given the device.
        /// </summary>
        /// <remarks>
        /// Not available in the Developer Preview.
        /// </remarks>
        [JsonProperty(PropertyName = "displayName",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the date the device was last synced with the service.
        /// </summary>
        /// <remarks>
        /// Not available in the Developer Preview.
        /// </remarks>
        [JsonProperty(PropertyName = "lastSuccessfulSync",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public DateTime LastSuccessfulSync { get; set; }

        /// <summary>
        /// Gets or sets the family.
        /// </summary>
        [JsonProperty(PropertyName = "deviceFamily",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthDeviceFamily Family { get; set; }

        /// <summary>
        /// Gets or sets the device's hardware version.
        /// </summary>
        [JsonProperty(PropertyName = "hardwareVersion",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string HardwareVersion { get; set; }

        /// <summary>
        /// Gets or sets the device's software version.
        /// </summary>
        [JsonProperty(PropertyName = "softwareVersion",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string SoftwareVersion { get; set; }

        /// <summary>
        /// Gets or sets the name of the model of the device.
        /// </summary>
        [JsonProperty(PropertyName = "modelName",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string ModelName { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer of the device.
        /// </summary>
        [JsonProperty(PropertyName = "manufacturer",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the current status of the device.
        /// </summary>
        [JsonProperty(PropertyName = "deviceStatus",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthDeviceStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the date the device was first registered.
        /// </summary>
        /// <remarks>
        /// Not available in the Developer Preview.
        /// </remarks>
        [JsonProperty(PropertyName = "createdDate",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public DateTime? CreatedDate { get; set; }

        #endregion

    }

    #endregion

    #region MSHealthDeviceFamily enum

    /// <summary>
    /// Represents the device family.
    /// </summary>
    public enum MSHealthDeviceFamily
    {
        /// <summary>
        /// Unknown family device.
        /// </summary>
        Unknown,
        /// <summary>
        /// Microsoft Band device.
        /// </summary>
        Band,
        /// <summary>
        /// Windows device.
        /// </summary>
        Windows,
        /// <summary>
        /// Android device.
        /// </summary>
        Android,
        /// <summary>
        /// iOS device.
        /// </summary>
        IOS,
    }

    #endregion

    #region MSHealthDeviceStatus enum

    /// <summary>
    /// Represents the status device.
    /// </summary>
    public enum MSHealthDeviceStatus
    {
        /// <summary>
        /// Unknown status.
        /// </summary>
        Unknown,
        /// <summary>
        /// Device is Active.
        /// </summary>
        Active,
        /// <summary>
        /// Device is Inactive.
        /// </summary>
        Inactive,
    }

    #endregion

}
