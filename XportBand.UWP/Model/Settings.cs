//-----------------------------------------------------------------------
// <copyright file="Settings.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace XportBand.Model
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MSHealthAPI;
#if WINDOWS_UWP
    using Windows.Storage;
    using Windows.Security.Credentials;
    using System.Collections.Generic;
#elif DESKTOP_APP
	using System.Configuration;
#endif

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

        /// <summary>
        /// ID for Nike+ Credential on XportBand.
        /// </summary>
        private const string NIKE_PLUS_CREDENTIAL = "NikePlusXportBand";

        #endregion

        #region Inner Members

        /// <summary>
        /// Password vault.
        /// </summary>
        private static PasswordVault moPasswordVault;

        /// <summary>
        /// Nike+ credential.
        /// </summary>
        private static PasswordCredential moNikePlusCredential;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the <see cref="Settings"/> class.
        /// </summary>
        static Settings()
        {
#if WINDOWS_UWP
            SetVersion(); // Ensure settings are on last version
            moPasswordVault = new PasswordVault();
#endif
            // Assign default Period value for Microsoft Health
            if (string.IsNullOrEmpty(MSHealthFilterPeriod))
                MSHealthFilterPeriod = PERIOD_WEEK;
            // Assign default Distance Unit value for Microsoft Health
            if (string.IsNullOrEmpty(MSHealthFilterDistance))
                MSHealthFilterDistance = DistanceUnit.DISTANCE_KILOMETER;
            // Assign default Activity Type value for Microsoft Health
            if (!(MSHealthFilterActivityBike ||
                MSHealthFilterActivityExercise ||
                MSHealthFilterActivityGolf ||
                MSHealthFilterActivityGuided ||
                MSHealthFilterActivityRun ||
                MSHealthFilterActivitySleep))
                MSHealthFilterActivityRun = true;
        }

        #endregion

        #region Properties

#if WINDOWS_UWP
        /// <summary>
        /// Gets or sets the Microsoft Health Authorization Token.
        /// </summary>
        public static MSHealthToken MSHealthToken
        {
            get { return ReadSettingValue<MSHealthToken>(null, "MSHealthToken"); }
            set { WriteSettingValue(null, value, "MSHealthToken"); }
        }

        /// <summary>
        /// Gets or sets the Nike+ credential.
        /// </summary>
        public static PasswordCredential NikePlusCredential
        {
            get
            {
                if (moNikePlusCredential == null)
                {
                    IReadOnlyList<PasswordCredential> loCredentialList = null;
                    loCredentialList = moPasswordVault.RetrieveAll();
                    if (loCredentialList != null &&
                       loCredentialList.Any())
                        moNikePlusCredential = loCredentialList.FirstOrDefault(loCred => loCred.Resource.Equals(NIKE_PLUS_CREDENTIAL, StringComparison.OrdinalIgnoreCase));
                    if (moNikePlusCredential != null)
                        moNikePlusCredential.RetrievePassword();
                }
                return moNikePlusCredential;
            }
            set
            {
                IReadOnlyList<PasswordCredential> loCredentialList = null;
                loCredentialList = moPasswordVault.RetrieveAll();
                if (value != null)
                {
                    if (loCredentialList != null &&
                        loCredentialList.Any())
                        moNikePlusCredential = loCredentialList.FirstOrDefault(loCred => loCred.Resource.Equals(NIKE_PLUS_CREDENTIAL, StringComparison.OrdinalIgnoreCase));
                    if (moNikePlusCredential != null)
                    {
                        moNikePlusCredential.RetrievePassword();
                        moPasswordVault.Remove(moNikePlusCredential);
                    }
                    value.Resource = NIKE_PLUS_CREDENTIAL;
                    moPasswordVault.Add(value);
                }
                else
                {
                    if (loCredentialList != null &&
                        loCredentialList.Any())
                    {
                        foreach (var loCred in loCredentialList)
                        {
                            loCred.RetrievePassword();
                            moPasswordVault.Remove(loCred);
                        }
                    }
                }
                moNikePlusCredential = value;
            }
        }

#elif DESKTOP_APP
        /// <summary>
        /// Gets or sets the Microsoft Health access token.
        /// </summary>
        public static string MSHealthAccessToken
        {
            get { return MSHealthToken != null ? MSHealthToken.AccessToken : null; }
            set { WriteSettingValue<string>("MSHealthToken", value, "accessToken"); }
        }

        /// <summary>
        /// Gets or sets the Microsoft Health refresh token.
        /// </summary>
        public static string MSHealthRefreshToken
        {
            get { return MSHealthToken != null ? MSHealthToken.RefreshToken : null; }
            set { WriteSettingValue<string>("MSHealthToken", value, "refreshToken"); }
        }

        /// <summary>
        /// Gets or sets the Microsoft Health amount of time in seconds when the access token is valid.
        /// </summary>
        /// <see cref="MSHealthTokenCreationTime"/>.
        /// <see cref="MSHealthTokenExpirationTime"/>.
        public static long MSHealthExpiresIn
        {
            get { return MSHealthToken != null ? MSHealthToken.ExpiresIn : 0; }
            set { WriteSettingValue("MSHealthToken", value, "expiresIn"); }
        }

        /// <summary>
        /// Gets or sets the Microsoft Health token creation time.
        /// </summary>
        /// <see cref="MSHealthExpiresIn"/>.
        /// <see cref="MSHealthTokenExpirationTime"/>.
        public static long MSHealthTokenCreationTime
        {
            get { return MSHealthToken != null ? MSHealthToken.CreationTime.Ticks : 0; }
            set { WriteSettingValue("MSHealthToken", value, "creationTime"); }
        }

        /// <summary>
        /// Gets or sets the Microsoft Health token expiration time.
        /// </summary>
        /// <see cref="MSHealthExpiresIn"/>.
        /// <see cref="MSHealthTokenCreationTime"/>.
        public static long MSHealthTokenExpirationTime
        {
            get { return MSHealthToken != null ? MSHealthToken.ExpirationTime.Ticks : 0; }
            set { WriteSettingValue("MSHealthToken", value, "expirationTime"); }
        }
#endif

        /// <summary>
        /// Gets or sets a value indicating whether filter exercise activities.
        /// </summary>
        public static bool MSHealthFilterActivityExercise
        {
            get { return ReadSettingValue<bool>("exercise", "MSHealthActivityFilters"); }
            set { WriteSettingValue("exercise", value, "MSHealthActivityFilters"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter run activities.
        /// </summary>
        public static bool MSHealthFilterActivityRun
        {
            get { return ReadSettingValue<bool>("run", "MSHealthActivityFilters"); }
            set { WriteSettingValue("run", value, "MSHealthActivityFilters"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter bike activities.
        /// </summary>
        public static bool MSHealthFilterActivityBike
        {
            get { return ReadSettingValue<bool>("bike", "MSHealthActivityFilters"); }
            set { WriteSettingValue("bike", value, "MSHealthActivityFilters"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter golf activities.
        /// </summary>
        public static bool MSHealthFilterActivityGolf
        {
            get { return ReadSettingValue<bool>("golf", "MSHealthActivityFilters"); }
            set { WriteSettingValue("golf", value, "MSHealthActivityFilters"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter sleep activities.
        /// </summary>
        public static bool MSHealthFilterActivitySleep
        {
            get { return ReadSettingValue<bool>("sleep", "MSHealthActivityFilters"); }
            set { WriteSettingValue("sleep", value, "MSHealthActivityFilters"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter guided workout activities.
        /// </summary>
        public static bool MSHealthFilterActivityGuided
        {
            get { return ReadSettingValue<bool>("guided", "MSHealthActivityFilters"); }
            set { WriteSettingValue("guided", value, "MSHealthActivityFilters"); }
        }

        /// <summary>
        /// Gets or sets the Microsoft Health filter period.
        /// </summary>
        public static string MSHealthFilterPeriod
        {
            get { return ReadSettingValue<string>("period", "MSHealthActivityFilters"); }
            set { WriteSettingValue("period", value, "MSHealthActivityFilters"); }
        }

        /// <summary>
        /// Gets or sets the Microsoft Health filter distance.
        /// </summary>
        public static string MSHealthFilterDistance
        {
            get { return ReadSettingValue<string>("distance", "MSHealthActivityFilters"); }
            set { WriteSettingValue("distance", value, "MSHealthActivityFilters"); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reads value from settings.
        /// </summary>
        /// <typeparam name="T"><see cref="System.Type"/> parameter to read.</typeparam>
        /// <param name="key">Setting key to read.</param>
        /// <param name="composite">Setting composite to read.</param>
        /// <returns>Setting value.</returns>
        private static T ReadSettingValue<T>(string key, string composite = null)
        {
            SetVersion(); // Ensure settings are on last version
            T loSettingValue = default(T);
            object loValue = null;
#if WINDOWS_UWP
            ApplicationDataContainer loRoamingSettings = ApplicationData.Current.RoamingSettings;
            ApplicationDataCompositeValue loComposite = null;
            if (!string.IsNullOrEmpty(composite))
            {
                loComposite = loRoamingSettings.Values[composite] as ApplicationDataCompositeValue;
                if (loComposite != null)
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        loValue = loComposite[key];
                    }
                    else
                    {
                        if (typeof(T) == typeof(MSHealthToken))
                        {
                            loValue = loComposite.ToMSHealthToken();
                        }
                    }
                }
            }
            else
                loValue = loRoamingSettings.Values[key];
#elif DESKTOP_APP
			loValue = ConfigurationManager.AppSettings[key];
#endif
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
        /// <param name="composite">Setting composite.</param>
        private static void WriteSettingValue<T>(string key, T value, string composite = null)
        {
            SetVersion(); // Ensure settings are on last version
#if WINDOWS_UWP
            ApplicationDataContainer loRoamingSettings = ApplicationData.Current.RoamingSettings;
            ApplicationDataCompositeValue loComposite = null;
            MSHealthToken loMSHealthToken = null;
            if (!string.IsNullOrEmpty(composite))
            {
                loComposite = loRoamingSettings.Values[composite] as ApplicationDataCompositeValue;
                if (loComposite != null &&
                    !string.IsNullOrEmpty(key))
                {
                    if (value != null)
                        loComposite[key] = value;
                    else
                        loComposite.Values.Remove(key);
                    loRoamingSettings.Values[composite] = loComposite;
                }
                else
                {
                    if (value != null)
                    {
                        if (typeof(T) == typeof(MSHealthToken))
                        {
                            loMSHealthToken = value as MSHealthToken;
                            if (loMSHealthToken != null)
                                loRoamingSettings.Values[composite] = loMSHealthToken.ToComposite();
                        }
                        else
                        {
                            loComposite = new ApplicationDataCompositeValue();
                            loComposite[key] = value;
                            loRoamingSettings.Values[composite] = loComposite;
                        }
                    }
                    else
                    {
                        loRoamingSettings.Values.Remove(composite);
                    }
                }
            }
            else
            {
                if (value != null)
                    loRoamingSettings.Values[key] = value;
                else
                    loRoamingSettings.Values.Remove(key);
            }
#elif DESKTOP_APP
			if (value != null)
				ConfigurationManager.AppSettings[key] = value.ToString();
			else
				ConfigurationManager.AppSettings[key] = null;
#endif
        }

#if DESKTOP_APP
        /// <summary>
        /// Updates the Microsoft Health token settings.
        /// </summary>
        /// <param name="token"><see cref="MSHealthAPI.MSHealthToken"/> instance.</param>
        [Obsolete("Use instead MSHealthToken property")]
        public static void UpdateMSHealthToken(MSHealthToken token)
        {
            SetVersion(); // Ensure settings are on last version
            ApplicationDataContainer loRoamingSettings = ApplicationData.Current.RoamingSettings;
            if (token != null)
            {
                // Token provided, persist values
                loRoamingSettings.Values["MSHealthToken"] = token.ToComposite();
            }
            else
            {
                // Token not provided, reset values
                if (loRoamingSettings.Values["MSHealthToken"] != null)
                    loRoamingSettings.Values.Remove("MSHealthToken");
            }
        }
#endif

        #endregion

#if WINDOWS_UWP
        #region ApplicationData.Version Handlers

        /// <summary>
        /// Ensures settings version is up to date.
        /// </summary>
        private static void SetVersion()
        {
            ApplicationData loAppData = ApplicationData.Current;
            if (loAppData.Version < 1)
            {
                Task.Run(async () =>
                {
                    await loAppData.SetVersionAsync(1, new ApplicationDataSetVersionHandler((request) =>
                    {
                        SetVersionDeferral loDeferral = request.GetDeferral();
                        if (request.CurrentVersion < 1)
                        {
                            ApplicationDataContainer loLocalSettings = ApplicationData.Current.LocalSettings;
                            ApplicationDataContainer loRoamingSettings = ApplicationData.Current.RoamingSettings;
                            ApplicationDataCompositeValue loComposite = null;
                            // MSHealthToken
                            if (loLocalSettings.Values["MSHealthTokenCreationTime"] != null ||
                                loLocalSettings.Values["MSHealthExpiresIn"] != null ||
                                loLocalSettings.Values["MSHealthAccessToken"] != null ||
                                loLocalSettings.Values["MSHealthRefreshToken"] != null)
                            {
                                MSHealthToken loToken = new MSHealthToken();
                                if (loLocalSettings.Values["MSHealthTokenCreationTime"] != null)
                                    loToken.CreationTime = new DateTime((long)loLocalSettings.Values["MSHealthTokenCreationTime"]);
                                if (loLocalSettings.Values["MSHealthExpiresIn"] != null)
                                    loToken.ExpiresIn = (long)loLocalSettings.Values["MSHealthExpiresIn"];
                                if (loLocalSettings.Values["MSHealthAccessToken"] != null)
                                    loToken.AccessToken = (string)loLocalSettings.Values["MSHealthAccessToken"];
                                if (loLocalSettings.Values["MSHealthRefreshToken"] != null)
                                    loToken.RefreshToken = (string)loLocalSettings.Values["MSHealthRefreshToken"];
                                loRoamingSettings.Values["MSHealthToken"] = loToken.ToComposite();
                            }
                            // Activity Filters
                            if (loLocalSettings.Values["MSHealthFilterActivityExercise"] != null ||
                                loLocalSettings.Values["MSHealthFilterActivityRun"] != null ||
                                loLocalSettings.Values["MSHealthFilterActivityBike"] != null ||
                                loLocalSettings.Values["MSHealthFilterActivityGolf"] != null ||
                                loLocalSettings.Values["MSHealthFilterActivitySleep"] != null ||
                                loLocalSettings.Values["MSHealthFilterActivityGuided"] != null ||
                                loLocalSettings.Values["MSHealthFilterPeriod"] != null ||
                                loLocalSettings.Values["MSHealthFilterDistance"] != null)
                            {
                                loComposite = new ApplicationDataCompositeValue();
                                // Activity Type: Exercise
                                if (loLocalSettings.Values["MSHealthFilterActivityExercise"] != null)
                                    loComposite["exercise"] = loLocalSettings.Values["MSHealthFilterActivityExercise"];
                                // Activity Type: Run
                                if (loLocalSettings.Values["MSHealthFilterActivityRun"] != null)
                                    loComposite["run"] = loLocalSettings.Values["MSHealthFilterActivityRun"];
                                // Activity Type: Bike
                                if (loLocalSettings.Values["MSHealthFilterActivityBike"] != null)
                                    loComposite["bike"] = loLocalSettings.Values["MSHealthFilterActivityBike"];
                                // Activity Type: Golf
                                if (loLocalSettings.Values["MSHealthFilterActivityGolf"] != null)
                                    loComposite["golf"] = loLocalSettings.Values["MSHealthFilterActivityGolf"];
                                // Activity Type: Sleep
                                if (loLocalSettings.Values["MSHealthFilterActivitySleep"] != null)
                                    loComposite["sleep"] = loLocalSettings.Values["MSHealthFilterActivitySleep"];
                                // Activity Type: Guided Workout
                                if (loLocalSettings.Values["MSHealthFilterActivityGuided"] != null)
                                    loComposite["guided"] = loLocalSettings.Values["MSHealthFilterActivityGuided"];
                                // Period
                                if (loLocalSettings.Values["MSHealthFilterPeriod"] != null)
                                    loComposite["period"] = loLocalSettings.Values["MSHealthFilterPeriod"];
                                // Distance Unit
                                if (loLocalSettings.Values["MSHealthFilterDistance"] != null)
                                    loComposite["distance"] = loLocalSettings.Values["MSHealthFilterDistance"];
                                loRoamingSettings.Values["MSHealthActivityFilters"] = loComposite;
                            }
                        }
                        loDeferral.Complete();
                    }));
                }).Wait();
            }
        }

        #endregion
#endif

    }

}
