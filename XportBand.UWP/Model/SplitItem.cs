//-----------------------------------------------------------------------
// <copyright file="SplitItem.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace XportBand.Model
{
    using GalaSoft.MvvmLight;
    using System;

    /// <summary>
    /// Model class to represents Split details.
    /// </summary>
    /// <seealso cref="GalaSoft.MvvmLight.ObservableObject" />
    public sealed class SplitItem : ObservableObject
    {

        #region Inner members

        /// <summary>
        /// Inner member for <see cref="SplitAt"/> property.
        /// </summary>
        private double mdValue;

        /// <summary>
        /// Inner member for <see cref="Duration"/> property.
        /// </summary>
        private TimeSpan mtsDuration;

        /// <summary>
        /// Inner member for <see cref="DurationMark"/> property.
        /// </summary>
        private string msDurationMark;

        /// <summary>
        /// Inner member for <see cref="AvgHeartRate"/> property.
        /// </summary>
        private int miAvgHeartRate;

        /// <summary>
        /// Inner member for <see cref="HRMark"/> property.
        /// </summary>
        private string msHRMark;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets value for current split.
        /// </summary>
        public double Value
        {
            get { return mdValue; }
            set { Set<double>(() => Value, ref mdValue, value); }
        }

        /// <summary>
        /// Gets or sets the duration for current split.
        /// </summary>
        public TimeSpan Duration
        {
            get { return mtsDuration; }
            set { Set<TimeSpan>(() => Duration, ref mtsDuration, value); }
        }

        /// <summary>
        /// Gets or sets a mark for current split.
        /// </summary>
        public string DurationMark
        {
            get { return msDurationMark; }
            set { Set<string>(() => DurationMark, ref msDurationMark, value); }
        }

        /// <summary>
        /// Gets or sets the average heart rate for current split.
        /// </summary>
        public int AvgHeartRate
        {
            get { return miAvgHeartRate; }
            set { Set<int>(() => AvgHeartRate, ref miAvgHeartRate, value); }
        }

        /// <summary>
        /// Gets or sets a mark for current split.
        /// </summary>
        public string HRMark
        {
            get { return msHRMark; }
            set { Set<string>(() => HRMark, ref msHRMark, value); }
        }

        #endregion

    }

}
