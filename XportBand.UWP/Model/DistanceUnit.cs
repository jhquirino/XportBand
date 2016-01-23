//-----------------------------------------------------------------------
// <copyright file="DistanceUnit.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace XportBand.Model
{

    /// <summary>
    /// Model class to represent Distance Unit.
    /// </summary>
    public sealed class DistanceUnit
    {

        #region Constants

        /// <summary>
        /// Key for Distance Unit: Miles
        /// </summary>
        public const string DISTANCE_MILE = "M";

        /// <summary>
        /// Key for Distance Unit: Kilometers
        /// </summary>
        public const string DISTANCE_KILOMETER = "K";

        #endregion

        #region Properties

        /// <summary>
        /// Key for current instance of <see cref="DistanceUnit"/>.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Description for current instance of <see cref="DistanceUnit"/>.
        /// </summary>
        public string Description { get; set; }

        #endregion

    }

}
