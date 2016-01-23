//-----------------------------------------------------------------------
// <copyright file="NameValueItem.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2014-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace XportBand.Model
{
    using GalaSoft.MvvmLight;

    #region NameValueItem class

    /// <summary>
    /// Model class to represent Name-Value items.
    /// </summary>
    /// <typeparam name="T"><see cref="System.Type"/> for value.</typeparam>
    /// <seealso cref="GalaSoft.MvvmLight.ObservableObject" />
    public class NameValueItem<T> : ObservableObject
    {

        #region Inner members

        /// <summary>
        /// Inner member for <see cref="Name"/> property.
        /// </summary>
        private string msName;

        /// <summary>
        /// Inner member for <see cref="Value"/> property.
        /// </summary>
        private T moValue;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the item name.
        /// </summary>
        public string Name
        {
            get { return msName; }
            set { Set<string>(() => Name, ref msName, value); }
        }
        /// <summary>
        /// Gets or sets the item value.
        /// </summary>
        public T Value
        {
            get { return moValue; }
            set { Set<T>(() => Value, ref moValue, value); }
        }

        #endregion

    }

    #endregion

    #region HeartRateZoneItem class

    /// <summary>
    /// Model class to represent Heart Rate Zone item.
    /// </summary>
    /// <seealso cref="XportBand.Model.NameValueItem{System.Int32}" />
    public class HeartRateZoneItem : NameValueItem<int>
    {

        #region Constants

        /// <summary>
        /// Key for UnderHealthy Heart Rate Zone.
        /// </summary>
        public const string HRZONE_UNDER_HEALTHY = "UH";

        /// <summary>
        /// Key for Healthy Heart Rate Zone.
        /// </summary>
        public const string HRZONE_HEALTHY = "H";

        /// <summary>
        /// Key for Fitness Heart Rate Zone.
        /// </summary>
        public const string HRZONE_FITNESS = "F";

        /// <summary>
        /// Key for Aerobic Heart Rate Zone.
        /// </summary>
        public const string HRZONE_AEROBIC = "A";

        /// <summary>
        /// Key for Anaerobic Heart Rate Zone.
        /// </summary>
        public const string HRZONE_ANAEROBIC = "AN";

        /// <summary>
        /// Key for Redline Heart Rate Zone.
        /// </summary>
        public const string HRZONE_REDLINE = "R";

        /// <summary>
        /// Key for OverRedline Heart Rate Zone.
        /// </summary>
        public const string HRZONE_OVER_REDLINE = "OR";

        #endregion

        #region Inner members

        /// <summary>
        /// Inner member for <see cref="Key"/> property.
        /// </summary>
        private string msKey;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the key for current heart rate zone.
        /// </summary>
        public string Key
        {
            get { return msKey; }
            set { Set<string>(() => Key, ref msKey, value); }
        }

        #endregion

    }

    #endregion

}
