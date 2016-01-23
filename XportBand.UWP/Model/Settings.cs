//-----------------------------------------------------------------------
// <copyright file="Settings.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace XportBand.Model
{
    using Windows.Storage;

    /// <summary>
    /// Model class to represent XportBand Settings.
    /// </summary>
    public static class Settings
    {

        #region Constants

        /// <summary>
        /// Period: Day.
        /// </summary>
        public const string PERIOD_DAY = "D";

        /// <summary>
        /// Period: Week,
        /// </summary>
        public const string PERIOD_WEEK = "W";

        /// <summary>
        /// Period: Month
        /// </summary>
        public const string PERIOD_MONTH = "M";

        /// <summary>
        /// Period: Year
        /// </summary>
        public const string PERIOD_YEAR = "Y";

        /// <summary>
        /// Period: All
        /// </summary>
        public const string PERIOD_ALL = "A";

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the <see cref="Settings"/> class.
        /// </summary>
        static Settings()
        {
            // Assign default Period value for Microsoft Health
            if (string.IsNullOrEmpty(MSHealthFilterPeriod))
                MSHealthFilterPeriod = PERIOD_WEEK;
            // Assign default Distance Unit value for Microsoft Health
            if (string.IsNullOrEmpty(MSHealthFilterDistance))
                MSHealthFilterDistance = DistanceUnit.DISTANCE_KILOMETER;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Microsoft Health access token.
        /// </summary>
        public static string MSHealthAccessToken
        {
            get { return ReadSettingValue<string>("MSHealthAccessToken"); }
            set { WriteSettingValue<string>("MSHealthAccessToken", value); }
        }

        /// <summary>
        /// Gets or sets the Microsoft Health refresh token.
        /// </summary>
        public static string MSHealthRefreshToken
        {
            get { return ReadSettingValue<string>("MSHealthRefreshToken"); }
            set { WriteSettingValue<string>("MSHealthRefreshToken", value); }
        }

        /// <summary>
        /// Gets or sets the Microsoft Health amount of time in seconds when the access token is valid.
        /// </summary>
        /// <see cref="MSHealthTokenCreationTime"/>.
        /// <see cref="MSHealthTokenExpirationTime"/>.
        public static long MSHealthExpiresIn
        {
            get { return ReadSettingValue<long>("MSHealthExpiresIn"); }
            set { WriteSettingValue<long>("MSHealthExpiresIn", value); }
        }

        /// <summary>
        /// Gets or sets the Microsoft Health token creation time.
        /// </summary>
        /// <see cref="MSHealthExpiresIn"/>.
        /// <see cref="MSHealthTokenExpirationTime"/>.
        public static long MSHealthTokenCreationTime
        {
            get { return ReadSettingValue<long>("MSHealthTokenCreationTime"); }
            set { WriteSettingValue<long>("MSHealthTokenCreationTime", value); }
        }

        /// <summary>
        /// Gets or sets the Microsoft Health token expiration time.
        /// </summary>
        /// <see cref="MSHealthExpiresIn"/>.
        /// <see cref="MSHealthTokenCreationTime"/>.
        public static long MSHealthTokenExpirationTime
        {
            get { return ReadSettingValue<long>("MSHealthTokenExpirationTime"); }
            set { WriteSettingValue<long>("MSHealthTokenExpirationTime", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter exercise activities.
        /// </summary>
        public static bool MSHealthFilterActivityExercise
        {
            get { return ReadSettingValue<bool>("MSHealthFilterActivityExercise"); }
            set { WriteSettingValue<bool>("MSHealthFilterActivityExercise", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter run activities.
        /// </summary>
        public static bool MSHealthFilterActivityRun
        {
            get { return ReadSettingValue<bool>("MSHealthFilterActivityRun"); }
            set { WriteSettingValue<bool>("MSHealthFilterActivityRun", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter bike activities.
        /// </summary>
        public static bool MSHealthFilterActivityBike
        {
            get { return ReadSettingValue<bool>("MSHealthFilterActivityBike"); }
            set { WriteSettingValue<bool>("MSHealthFilterActivityBike", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter golf activities.
        /// </summary>
        public static bool MSHealthFilterActivityGolf
        {
            get { return ReadSettingValue<bool>("MSHealthFilterActivityGolf"); }
            set { WriteSettingValue<bool>("MSHealthFilterActivityGolf", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter sleep activities.
        /// </summary>
        public static bool MSHealthFilterActivitySleep
        {
            get { return ReadSettingValue<bool>("MSHealthFilterActivitySleep"); }
            set { WriteSettingValue<bool>("MSHealthFilterActivitySleep", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter guided workout activities.
        /// </summary>
        public static bool MSHealthFilterActivityGuided
        {
            get { return ReadSettingValue<bool>("MSHealthFilterActivityGuided"); }
            set { WriteSettingValue<bool>("MSHealthFilterActivityGuided", value); }
        }

        /// <summary>
        /// Gets or sets the Microsoft Health filter period.
        /// </summary>
        public static string MSHealthFilterPeriod
        {
            get { return ReadSettingValue<string>("MSHealthFilterPeriod"); }
            set { WriteSettingValue<string>("MSHealthFilterPeriod", value); }
        }

        /// <summary>
        /// Gets or sets the Microsoft Health filter distance.
        /// </summary>
        public static string MSHealthFilterDistance
        {
            get { return ReadSettingValue<string>("MSHealthFilterDistance"); }
            set { WriteSettingValue<string>("MSHealthFilterDistance", value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reads value from settings.
        /// </summary>
        /// <typeparam name="T"><see cref="System.Type"/> parameter to read.</typeparam>
        /// <param name="key">Setting key to read.</param>
        /// <returns>Setting value read.</returns>
        private static T ReadSettingValue<T>(string key)
        {
            T loSettingValue = default(T);
            ApplicationDataContainer loLocalSettings = ApplicationData.Current.LocalSettings;
            object loValue = loLocalSettings.Values[key];
            if (loValue != null)
            {
                loSettingValue = (T)loValue;
            }
            return loSettingValue;
        }

        /// <summary>
        /// Writes value to settings.
        /// </summary>
        /// <typeparam name="T"><see cref="System.Type"/> parameter to write.</typeparam>
        /// <param name="key">Setting key.</param>
        /// <param name="value">Setting value.</param>
        private static void WriteSettingValue<T>(string key, T value)
        {
            ApplicationDataContainer loLocalSettings = ApplicationData.Current.LocalSettings;
            loLocalSettings.Values[key] = value;
        }

        /// <summary>
        /// Updates the Microsoft Health token settings.
        /// </summary>
        /// <param name="token"><see cref="MSHealthAPI.MSHealthToken"/> instance.</param>
        public static void UpdateMSHealthToken(MSHealthAPI.MSHealthToken token)
        {
            if (token != null)
            {
                // Token provided, persist values
                MSHealthAccessToken = token.AccessToken;
                MSHealthRefreshToken = token.RefreshToken;
                MSHealthExpiresIn = token.ExpiresIn;
                MSHealthTokenExpirationTime = token.ExpirationTime.Ticks;
                MSHealthTokenCreationTime = token.CreationTime.Ticks;
            }
            else
            {
                // Token not provided, reset values
                MSHealthAccessToken = null;
                MSHealthRefreshToken = null;
                MSHealthExpiresIn = 0;
                MSHealthTokenExpirationTime = 0;
                MSHealthTokenCreationTime = 0;
            }
        }

        #endregion

    }

}
