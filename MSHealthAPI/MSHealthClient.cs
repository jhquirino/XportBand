//-----------------------------------------------------------------------
// <copyright file="MSHealthClient.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace MSHealthAPI
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
#if WINDOWS_UWP
    using Windows.Data.Json;
    using Windows.Foundation;
    using Windows.UI.Xaml.Controls;
#elif DESKTOP_APP
	using System.Collections.Specialized;
	using System.Web;
	using System.Windows.Navigation;
#endif

    #region IMSHealthClient interface

    /// <summary>
    /// Interface for Client to consume Microsoft Health Cloud API.
    /// </summary>
    public interface IMSHealthClient
    {

        #region Properties

        /// <summary>
        /// Gets a value indicating whether API Client is Signed-in to Microsoft Health.
        /// </summary>
        bool IsSignedIn { get; }

        /// <summary>
        /// Gets <see cref="MSHealthAPI.MSHealthToken"/> instance
        /// </summary>
        MSHealthToken Token { get; }

        /// <summary>
        /// Gets URL to request Sign-in.
        /// </summary>
        Uri SignInUri { get; }

        /// <summary>
        /// Gets URL to request Sign-out.
        /// </summary>
        Uri SignOutUri { get; }

        #endregion

        #region Methods

#if WINDOWS_UWP
        /// <summary>
        /// Handles <see cref="WebView.NavigationCompleted"/> event, to determine if Sign-in/Sign-out
        /// process was successfull.
        /// </summary>
        /// <param name="args">The event data</param>
        /// <returns><see cref="NavigationResult"/> of Sign-in/Sign-out process.</returns>
        Task<MSHealthNavigationResult> HandleNavigationCompleted(WebViewNavigationCompletedEventArgs args);
#elif DESKTOP_APP
		/// <summary>
		/// Handles <see cref="System.Windows.Controls.WebBrowser.Navigated"/> event, to determine if Sign-in/Sign-out
		/// process was successfull.
		/// </summary>
		/// <param name="args">The event data</param>
		/// <returns><see cref="MSHealthNavigationResult"/> of Sign-in/Sign-out process.</returns>
		Task<MSHealthNavigationResult> HandleNavigationCompleted(NavigationEventArgs args);
#endif

        /// <summary>
        /// Gets a list of activities, that match specified parameters, associated with this user's Microsoft Health profile.
        /// </summary>
        /// <param name="startTime">Filters the set of returned activities to those starting after the specified <see cref="DateTime"/>, inclusive.</param>
        /// <param name="endTime">Filters the set of returned activities to those starting before the specified <see cref="DateTime"/>, exclusive. </param>
        /// <param name="ids">The comma-separated list of activity ids to return.</param>
        /// <param name="type">The <see cref="MSHealthActivityType"/> to return (supports multi-values).</param>
        /// <param name="include">The <see cref="MSHealthActivityInclude"/> properties to return: Details, MinuteSummaries, MapPoints  (supports multi-values).</param>
        /// <param name="deviceIds">Filters the set of returned activities based on the comma-separated list of device ids provided.</param>
        /// <param name="splitDistanceType">The length of splits (<see cref="MSHealthSplitDistanceType"/>) used in each activity.</param>
        /// <param name="maxPageSize">The maximum number of entries to return per page. Defaults to 1000.</param>
        /// <returns>Instance of <see cref="MSHealthActivities"/> with collection of activities that matched specified parameters.</returns>
        Task<MSHealthActivities> ListActivities(DateTime? startTime = default(DateTime?),
                                                DateTime? endTime = default(DateTime?),
                                                string ids = null,
                                                MSHealthActivityType type = MSHealthActivityType.Unknown,
                                                MSHealthActivityInclude include = MSHealthActivityInclude.None,
                                                string deviceIds = null,
                                                MSHealthSplitDistanceType splitDistanceType = MSHealthSplitDistanceType.None,
                                                int? maxPageSize = default(int?));

        /// <summary>
        /// Lists daily summary data for this user by date range.
        /// </summary>
        /// <param name="startTime">Filters the set of returned summaries to those starting after the specified <see cref="DateTime"/>, inclusive.</param>
        /// <param name="endTime">Filters the set of returned summaries to those starting before the specified <see cref="DateTime"/>, exclusive. </param>
        /// <param name="deviceIds">Filters the set of returned summaries based on the comma-separated list of device ids provided.</param>
        /// <param name="maxPageSize">The maximum number of entries to return per page. Defaults to 48 for hourly and 31 for daily.</param>
        /// <returns></returns>
        Task<MSHealthSummaries> ListDailySummaries(DateTime? startTime = default(DateTime?),
                                                   DateTime? endTime = default(DateTime?),
                                                   string deviceIds = null,
                                                   int? maxPageSize = default(int?));

        /// <summary>
        /// Lists hourly summary data for this user by date range.
        /// </summary>
        /// <param name="startTime">Filters the set of returned summaries to those starting after the specified <see cref="DateTime"/>, inclusive.</param>
        /// <param name="endTime">Filters the set of returned summaries to those starting before the specified <see cref="DateTime"/>, exclusive. </param>
        /// <param name="deviceIds">Filters the set of returned summaries based on the comma-separated list of device ids provided.</param>
        /// <param name="maxPageSize">The maximum number of entries to return per page. Defaults to 48 for hourly and 31 for daily.</param>
        /// <returns></returns>
        Task<MSHealthSummaries> ListHourlySummaries(DateTime? startTime = default(DateTime?),
                                                    DateTime? endTime = default(DateTime?),
                                                    string deviceIds = null,
                                                    int? maxPageSize = default(int?));

        /// <summary>
        /// Get the details about the devices associated with this user's Microsoft Health profile.
        /// </summary>
        /// <returns>Instance of <see cref="MSHealthAPI.MSHealthDevices"/> with devices details.</returns>
        Task<MSHealthDevices> ListDevices();

        /// <summary>
        /// Get the details of an activity by its id.
        /// </summary>
        /// <param name="id">The id of the activity to get.</param>
        /// <param name="include">The <see cref="MSHealthAPI.MSHealthActivityInclude"/> properties to return: Details, MinuteSummaries, MapPoints  (supports multi-values).</param>
        /// <returns><see cref="MSHealthAPI.MSHealthActivity"/> instance with activity details.</returns>
        Task<MSHealthActivity> ReadActivity(string id, MSHealthActivityInclude include = MSHealthActivityInclude.None);

        /// <summary>
        /// Get the details about the requested device associated with this user's Microsoft Health profile.
        /// </summary>
        /// <param name="id">The id of the device</param>
        /// <returns><see cref="MSHealthDevice"/> instance with device details.</returns>
        Task<MSHealthDevice> ReadDevice(string id);

        /// <summary>
        /// Get the details about this user from their Microsoft Health profile.
        /// </summary>
        /// <returns><see cref="MSHealthProfile"/> instance with profile details.</returns>
        Task<MSHealthProfile> ReadProfile();

        /// <summary>
        /// Refresh current <see cref="MSHealthClient.Token"/>.
        /// </summary>
        /// <returns><see langword="true"/> if refresh successfull, otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// It only works if <see cref="MSHealthToken.RefreshToken"/> is available, to obtain it,
        /// it's necessary to set use <see cref="MSHealthScope.OfflineAccess"/>.
        /// </remarks>
        Task<bool> RefreshToken();

        /// <summary>
        /// Verifies <paramref name="token"/>instance validity (<see cref="MSHealthToken.ExpirationTime"/>),
        /// replaces current <see cref="MSHealthClient.Token"/> and if <paramref name="refreshIfInvalid"/>
        /// is <see langword="true"/>, calls <see cref="MSHealthClient.RefreshToken"/>.
        /// </summary>
        /// <param name="token">Instance of <see cref="MSHealthToken"/> to validate.</param>
        /// <param name="refreshIfInvalid">Flag to enforce Token refresh if is not valid.</param>
        /// <returns><see langword="true"/> if specified token is valid or has been refresh successfull, otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// It only works if <see cref="MSHealthToken.RefreshToken"/> is available, to obtain it,
        /// it's necessary to set use <see cref="MSHealthScope.OfflineAccess"/>.
        /// </remarks>
        Task<bool> ValidateToken(MSHealthToken token, bool refreshIfInvalid = true);

        #endregion

    }

    #endregion

    #region MSHealthClient class

    /// <summary>
    /// Client to consume Microsoft Health Cloud API.
    /// </summary>
    public sealed class MSHealthClient : IMSHealthClient
    {

        #region Constants

        /// <summary>
        /// Microsoft Health Cloud API - URL for Sign-in requests.
        /// </summary>
        private const string SIGNIN_URI = "https://login.live.com/oauth20_authorize.srf";

        /// <summary>
        /// Microsoft Health Cloud API - URL for Sign-out requests.
        /// </summary>
        private const string SIGNOUT_URI = "https://login.live.com/oauth20_logout.srf";

        /// <summary>
        /// Microsoft Health Cloud API - Base URL for requests.
        /// </summary>
        private const string BASE_URI = "https://api.microsofthealth.net";

        /// <summary>
        /// Microsoft Health Cloud API - URL for redirection response on authentication requests.
        /// </summary>
        private const string REDIRECT_URI = "https://login.live.com/oauth20_desktop.srf";

        /// <summary>
        /// Microsoft Health Cloud API - URL for Token requests.
        /// </summary>
        private const string TOKEN_URI = "https://login.live.com/oauth20_token.srf";

        /// <summary>
        /// Microsoft Health Cloud API - Path for authentication requests.
        /// </summary>
        private const string AUTH_PATH = "/oauth20_desktop.srf";

        /// <summary>
        /// Microsoft Health Cloud API - Read Profile Scope.
        /// </summary>
        private const string SCOPE_READ_PROFILE = "mshealth.ReadProfile";

        /// <summary>
        /// Microsoft Health Cloud API - Read Activity History Scope.
        /// </summary>
        private const string SCOPE_READ_ACTIVITY_HISTORY = "mshealth.ReadActivityHistory";

        /// <summary>
        /// Microsoft Health Cloud API - Read Devices Scope.
        /// </summary>
        private const string SCOPE_READ_DEVICES = "mshealth.ReadDevices";

        /// <summary>
        /// Microsoft Health Cloud API - Read Activity Location Scope.
        /// </summary>
        private const string SCOPE_READ_ACTIVITY_LOCATION = "mshealth.ReadActivityLocation";

        /// <summary>
        /// Microsoft Health Cloud API - Offline Access Scope.
        /// </summary>
        private const string SCOPE_OFFLINE_ACCESS = "offline_access";

        /// <summary>
        /// Microsoft Health Cloud API - Path for Profile Details.
        /// </summary>
        private const string PROFILE_PATH = "/v1/me/Profile";

        /// <summary>
        /// Microsoft Health Cloud API - Path for Devices Collection Details.
        /// </summary>
        private const string DEVICES_PATH = "/v1/me/Devices";

        /// <summary>
        /// Microsoft Health Cloud API - Path for Device Details.
        /// </summary>
        private const string DEVICE_PATH = "/v1/me/Devices/{0}";

        /// <summary>
        /// Microsoft Health Cloud API - Path for Daily Summaries Details.
        /// </summary>
        private const string SUMMARIES_DAILY_PATH = "/v1/me/Summaries/Daily";

        /// <summary>
        /// Microsoft Health Cloud API - Path for Hourly Summaries Details.
        /// </summary>
        private const string SUMMARIES_HOURLY_PATH = "/v1/me/Summaries/Hourly";

        /// <summary>
        /// Microsoft Health Cloud API - Path for Activities Collection Details.
        /// </summary>
        private const string ACTIVITIES_PATH = "/v1/me/Activities";

        /// <summary>
        /// Microsoft Health Cloud API - Path for Activity Details.
        /// </summary>
        private const string ACTIVITY_PATH = "/v1/me/Activities/{0}";

        #endregion

        #region Inner Members

        /// <summary>
        /// The Client ID for registerd app.
        /// </summary>
        private readonly string msClientId;

        /// <summary>
        /// The Client Secret for registerd app.
        /// </summary>
        private readonly string msClientSecret;

        /// <summary>
        /// The "list" of authorization scopes that app requires.
        /// </summary>
        private readonly MSHealthScope moScope;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether API Client is Signed-in to Microsoft Health.
        /// </summary>
        public bool IsSignedIn { get; private set; }

        /// <summary>
        /// Gets <see cref="MSHealthAPI.MSHealthToken"/> instance
        /// </summary>
        public MSHealthToken Token { get; private set; }

        /// <summary>
        /// Gets URL to request Sign-in.
        /// </summary>
        public Uri SignInUri
        {
            get
            {
                UriBuilder loUri = new UriBuilder(SIGNIN_URI);
                StringBuilder loQuery = new StringBuilder();
                string lsScopes = string.Empty;
                // Build query
                loQuery.AppendFormat("redirect_uri={0}", Uri.EscapeDataString(REDIRECT_URI));
                loQuery.AppendFormat("&client_id={0}", Uri.EscapeDataString(msClientId));
                // Append required scopes
                if (moScope != MSHealthScope.None)
                {
                    if (moScope.HasFlag(MSHealthScope.ReadProfile) || moScope.HasFlag(MSHealthScope.All))
                        lsScopes += " " + SCOPE_READ_PROFILE;
                    if (moScope.HasFlag(MSHealthScope.ReadActivityHistory) || moScope.HasFlag(MSHealthScope.All))
                        lsScopes += " " + SCOPE_READ_ACTIVITY_HISTORY;
                    if (moScope.HasFlag(MSHealthScope.ReadDevices) || moScope.HasFlag(MSHealthScope.All))
                        lsScopes += " " + SCOPE_READ_DEVICES;
                    if (moScope.HasFlag(MSHealthScope.ReadActivityLocation) || moScope.HasFlag(MSHealthScope.All))
                        lsScopes += " " + SCOPE_READ_ACTIVITY_LOCATION;
                    if (moScope.HasFlag(MSHealthScope.OfflineAccess) || moScope.HasFlag(MSHealthScope.All))
                        lsScopes += " " + SCOPE_OFFLINE_ACCESS;
                    lsScopes = lsScopes.Trim();
                    loQuery.AppendFormat("&scope={0}", Uri.EscapeDataString(lsScopes));
                }
                loQuery.Append("&response_type=code");
                loUri.Query = loQuery.ToString();
                // Return URL
                return loUri.Uri;
            }
        }

        /// <summary>
        /// Gets URL to request Sign-out.
        /// </summary>
        public Uri SignOutUri
        {
            get
            {
                UriBuilder loUri = new UriBuilder(SIGNOUT_URI);
                StringBuilder loQuery = new StringBuilder();
                // Build query
                loQuery.AppendFormat("redirect_uri={0}", Uri.EscapeDataString(REDIRECT_URI));
                loQuery.AppendFormat("&client_id={0}", Uri.EscapeDataString(msClientId));
                loUri.Query = loQuery.ToString();
                // Return URL
                return loUri.Uri;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="MSHealthAPI.MSHealthClient"/> class.
        /// </summary>
        /// <param name="clientId">The Client ID for registerd app.</param>
        /// <param name="clientSecret">The Client Secret for registerd app.</param>
        /// <param name="scope">The "list" of authorization scopes that app requires.</param>
        public MSHealthClient(string clientId, string clientSecret, MSHealthScope scope)
        {
            msClientId = clientId;
            msClientSecret = clientSecret;
            moScope = scope;
        }

        #endregion

        #region Public Methods

#if WINDOWS_UWP
        /// <summary>
        /// Handles <see cref="WebView.NavigationCompleted"/> event, to determine if Sign-in/Sign-out
        /// process was successfull.
        /// </summary>
        /// <param name="args">The event data</param>
        /// <returns><see cref="MSHealthNavigationResult"/> of Sign-in/Sign-out process.</returns>
        public async Task<MSHealthNavigationResult> HandleNavigationCompleted(WebViewNavigationCompletedEventArgs args)
        {
            MSHealthNavigationResult loResult = MSHealthNavigationResult.None;
            // Check if URL has Authentication path
            if (args.Uri.LocalPath.StartsWith(AUTH_PATH, StringComparison.OrdinalIgnoreCase))
            {
                WwwFormUrlDecoder loDecoder = new WwwFormUrlDecoder(args.Uri.Query);
                // Read Authentication Code
                IWwwFormUrlDecoderEntry loCode = loDecoder.FirstOrDefault((entry) => entry.Name.Equals("code", StringComparison.OrdinalIgnoreCase));
                // Read Authentication Errors
                IWwwFormUrlDecoderEntry loError = loDecoder.FirstOrDefault((entry) => entry.Name.Equals("error", StringComparison.OrdinalIgnoreCase));
                IWwwFormUrlDecoderEntry loErrorDesc = loDecoder.FirstOrDefault((entry) => entry.Name.Equals("error_description", StringComparison.OrdinalIgnoreCase));
                // Check the code to see if this is sign-in or sign-out
                if (loCode != null)
                {
                    // Check error and throw Exception
                    if (loError != null)
                        throw new Exception(string.Format("{0}\r\n{1}", loError.Value, loErrorDesc.Value));
                    // Get Token
                    try
                    {
                        // Signed-in
                        Token = await GetToken(loCode.Value, false);
                        IsSignedIn = true;
                        loResult = MSHealthNavigationResult.SignIn;
                    }
                    catch
                    {
                        // Error
                        Token = null;
                        IsSignedIn = false;
                        loResult = MSHealthNavigationResult.Error;
                        throw;
                    }
                }
                else
                {
                    // Signed-out
                    Token = null;
                    IsSignedIn = false;
                    loResult = MSHealthNavigationResult.SignOut;
                }
            }
            return loResult;
        }
#elif DESKTOP_APP
		/// <summary>
		/// Handles <see cref="System.Windows.Controls.WebBrowser.Navigated"/> event, to determine if Sign-in/Sign-out
		/// process was successfull.
		/// </summary>
		/// <param name="args">The event data</param>
		/// <returns><see cref="MSHealthNavigationResult"/> of Sign-in/Sign-out process.</returns>
		public async Task<MSHealthNavigationResult> HandleNavigationCompleted(NavigationEventArgs args)
		{
			MSHealthNavigationResult loResult = MSHealthNavigationResult.None;
			// Check if URL has Authentication path
			if (args.Uri.LocalPath.StartsWith(AUTH_PATH, StringComparison.OrdinalIgnoreCase))
			{
				//HttpUtility
				NameValueCollection loDecoder = HttpUtility.ParseQueryString(args.Uri.Query);
				// Read Authentication Code
				string lsCode = loDecoder["code"];
				// Read Authentication Errors
				string lsError = loDecoder["error"];
				string lsErrorDesc = loDecoder["error_description"];
				// Check the code to see if this is sign-in or sign-out
				if (!string.IsNullOrEmpty(lsCode))
				{
					// Check error and throw Exception
					if (!string.IsNullOrEmpty(lsError))
						throw new Exception(string.Format("{0}\r\n{1}", lsError, lsErrorDesc));
					// Get Token
					try
					{
						// Signed-in
						Token = await GetToken(lsCode, false);
						IsSignedIn = true;
						loResult = MSHealthNavigationResult.SignIn;
					}
					catch
					{
						// Error
						Token = null;
						IsSignedIn = false;
						loResult = MSHealthNavigationResult.Error;
						throw;
					}
				}
				else
				{
					// Signed-out
					Token = null;
					IsSignedIn = false;
					loResult = MSHealthNavigationResult.SignOut;
				}
			}
			return loResult;
		}
#endif

        /// <summary>
        /// Refresh current <see cref="Token"/>.
        /// </summary>
        /// <returns><see langword="true"/> if refresh successfull, otherwise, <see langword="false"/>.</returns>
        public async Task<bool> RefreshToken()
        {
            bool lbRefresh = false;
            // Check current token
            if (Token != null)
            {
                // Get Token
                Token = await GetToken(Token.RefreshToken, true);
                lbRefresh = true;
            }
            return lbRefresh;
        }

        /// <summary>
        /// Verifies <paramref name="token"/>instance validity (<see cref="MSHealthToken.ExpirationTime"/>),
        /// replaces current <see cref="MSHealthClient.Token"/> and if <paramref name="refreshIfInvalid"/>
        /// is <see langword="true"/>, calls <see cref="MSHealthClient.RefreshToken"/>.
        /// </summary>
        /// <param name="token">Instance of <see cref="MSHealthToken"/> to validate.</param>
        /// <param name="refreshIfInvalid">Flag to enforce Token refresh if is not valid.</param>
        /// <returns><see langword="true"/> if specified token is valid or has been refresh successfull, otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// It only wworks if <see cref="MSHealthToken.RefreshToken"/> is available, to obtain it,
        /// it's necessary to set use <see cref="MSHealthScope.OfflineAccess"/>.
        /// </remarks>
        public async Task<bool> ValidateToken(MSHealthToken token, bool refreshIfInvalid = true)
        {
            bool lbValid = false;
            // Check input token
            if (token != null)
            {
                // Check instance token
                if (Token != null)
                {
                    // Compare AccessToken for input vs instance
                    if (!string.IsNullOrEmpty(token.AccessToken) &&
                        !token.AccessToken.Equals(Token.AccessToken, StringComparison.OrdinalIgnoreCase))
                    {
                        // Different AccessToken, so, asign input to instance
                        Token = token;
                    }
                }
                else
                {
                    // No instance token, so, asign input
                    Token = token;
                }
                // Check Token's ExpirationTime
                if (Token.ExpirationTime == null ||
                    Token.ExpirationTime.CompareTo(DateTime.Now) <= 0)
                {
                    // Already Expired, so, refresh Token
                    if (refreshIfInvalid)
                        lbValid = await RefreshToken();
                }
                else
                {
                    lbValid = true;
                }
            }
            return lbValid;
        }

        /// <summary>
        /// Get the details about this user from their Microsoft Health profile.
        /// </summary>
        /// <returns><see cref="MSHealthProfile"/> instance with profile details.</returns>
        public async Task<MSHealthProfile> ReadProfile()
        {
            MSHealthProfile loProfile = null;
            // Perform request using BASE_URI and PROFILE_PATH
            string lsResponse = await PerformRequest(PROFILE_PATH);
            // Deserialize Json response
            loProfile = JsonConvert.DeserializeObject<MSHealthProfile>(lsResponse);
            return loProfile;
        }

        /// <summary>
        /// Get the details about the devices associated with this user's Microsoft Health profile.
        /// </summary>
        /// <returns>Instance of <see cref="MSHealthAPI.MSHealthDevices"/> with devices details.</returns>
        public async Task<MSHealthDevices> ListDevices()
        {
            MSHealthDevices loDevices = null;
            // Perform request using BASE_URI and DEVICES_PATH
            string lsResponse = await PerformRequest(DEVICES_PATH);
            // Deserialize Json response
            loDevices = JsonConvert.DeserializeObject<MSHealthDevices>(lsResponse, new StringEnumConverter());
            return loDevices;
        }

        /// <summary>
        /// Get the details about the requested device associated with this user's Microsoft Health profile.
        /// </summary>
        /// <param name="id">The id of the device</param>
        /// <returns><see cref="MSHealthDevice"/> instance with device details.</returns>
        public async Task<MSHealthDevice> ReadDevice(string id)
        {
            MSHealthDevice loDevice = null;
            // Perform request using BASE_URI, DEVICE_PATH and id
            string lsResponse = await PerformRequest(string.Format(DEVICE_PATH, id));
            // Deserialize Json response
            loDevice = JsonConvert.DeserializeObject<MSHealthDevice>(lsResponse, new StringEnumConverter());
            return loDevice;
        }

        /// <summary>
        /// Gets a list of activities, that match specified parameters, associated with this user's Microsoft Health profile.
        /// </summary>
        /// <param name="startTime">Filters the set of returned activities to those starting after the specified <see cref="DateTime"/>, inclusive.</param>
        /// <param name="endTime">Filters the set of returned activities to those starting before the specified <see cref="DateTime"/>, exclusive. </param>
        /// <param name="ids">The comma-separated list of activity ids to return.</param>
        /// <param name="type">The <see cref="MSHealthActivityType"/> to return (supports multi-values).</param>
        /// <param name="include">The <see cref="MSHealthActivityInclude"/> properties to return: Details, MinuteSummaries, MapPoints  (supports multi-values).</param>
        /// <param name="deviceIds">Filters the set of returned activities based on the comma-separated list of device ids provided.</param>
        /// <param name="splitDistanceType">The length of splits (<see cref="MSHealthSplitDistanceType"/>) used in each activity.</param>
        /// <param name="maxPageSize">The maximum number of entries to return per page.</param>
        /// <returns>Instance of <see cref="MSHealthActivities"/> with collection of activities that matched specified parameters.</returns>
        public async Task<MSHealthActivities> ListActivities(DateTime? startTime = null,
                                                             DateTime? endTime = null,
                                                             string ids = null,
                                                             MSHealthActivityType type = MSHealthActivityType.Unknown,
                                                             MSHealthActivityInclude include = MSHealthActivityInclude.None,
                                                             string deviceIds = null,
                                                             MSHealthSplitDistanceType splitDistanceType = MSHealthSplitDistanceType.None,
                                                             int? maxPageSize = null)
        {
            MSHealthActivities loActivities = null;
            StringBuilder loQuery = new StringBuilder();
            string lsResponse;
            string lsParamValue;
            // Check StartTime, and append to query if applies
            if (startTime != null && startTime.HasValue)
                loQuery.AppendFormat("&startTime={0}", Uri.EscapeDataString(startTime.Value.ToUniversalTime().ToString("O")));
            // Check EndTime, and append to query if applies
            if (endTime != null && endTime.HasValue)
                loQuery.AppendFormat("&endTime={0}", Uri.EscapeDataString(endTime.Value.ToUniversalTime().ToString("O")));
            // Check ActivityIds, and append to query if applies
            if (!string.IsNullOrEmpty(ids))
                loQuery.AppendFormat("&activityIds={0}", Uri.EscapeDataString(ids));
            // Check ActivityTypes, and append to query if applies
            if (type != MSHealthActivityType.Unknown)
            {
                lsParamValue = string.Empty;
                if (type.HasFlag(MSHealthActivityType.Custom))
                    lsParamValue += string.Format(",{0}", MSHealthActivityType.Custom);
                if (type.HasFlag(MSHealthActivityType.CustomExercise))
                    lsParamValue += string.Format(",{0}", MSHealthActivityType.CustomExercise);
                if (type.HasFlag(MSHealthActivityType.CustomComposite))
                    lsParamValue += string.Format(",{0}", MSHealthActivityType.CustomComposite);
                if (type.HasFlag(MSHealthActivityType.Run))
                    lsParamValue += string.Format(",{0}", MSHealthActivityType.Run);
                if (type.HasFlag(MSHealthActivityType.Sleep))
                    lsParamValue += string.Format(",{0}", MSHealthActivityType.Sleep);
                if (type.HasFlag(MSHealthActivityType.FreePlay))
                    lsParamValue += string.Format(",{0}", MSHealthActivityType.FreePlay);
                if (type.HasFlag(MSHealthActivityType.GuidedWorkout))
                    lsParamValue += string.Format(",{0}", MSHealthActivityType.GuidedWorkout);
                if (type.HasFlag(MSHealthActivityType.Bike))
                    lsParamValue += string.Format(",{0}", MSHealthActivityType.Bike);
                if (type.HasFlag(MSHealthActivityType.Golf))
                    lsParamValue += string.Format(",{0}", MSHealthActivityType.Golf);
                if (type.HasFlag(MSHealthActivityType.RegularExercise))
                    lsParamValue += string.Format(",{0}", MSHealthActivityType.RegularExercise);
                if (type.HasFlag(MSHealthActivityType.Hike))
                    lsParamValue += string.Format(",{0}", MSHealthActivityType.Hike);
                lsParamValue = lsParamValue.TrimStart(new char[] { ',' });
                loQuery.AppendFormat("&activityTypes={0}", lsParamValue);
            }
            // Check ActivityIncludes, and append to query if applies
            if (include != MSHealthActivityInclude.None)
            {
                lsParamValue = string.Empty;
                if (include.HasFlag(MSHealthActivityInclude.Details))
                    lsParamValue += string.Format(",{0}", MSHealthActivityInclude.Details);
                if (include.HasFlag(MSHealthActivityInclude.MinuteSummaries))
                    lsParamValue += string.Format(",{0}", MSHealthActivityInclude.MinuteSummaries);
                if (include.HasFlag(MSHealthActivityInclude.MapPoints))
                    lsParamValue += string.Format(",{0}", MSHealthActivityInclude.MapPoints);
                lsParamValue = lsParamValue.TrimStart(new char[] { ',' });
                loQuery.AppendFormat("&activityIncludes={0}", lsParamValue);
            }
            // Check DeviceIds, and append to query if applies
            if (!string.IsNullOrEmpty(deviceIds))
                loQuery.AppendFormat("&deviceIds={0}", Uri.EscapeDataString(deviceIds));
            // Check SplitDistanceType, and append to query if applies
            switch (splitDistanceType)
            {
                case MSHealthSplitDistanceType.Mile:
                    loQuery.AppendFormat("&splitDistanceType={0}", MSHealthSplitDistanceType.Mile);
                    break;
                case MSHealthSplitDistanceType.Kilometer:
                    loQuery.AppendFormat("&splitDistanceType={0}", MSHealthSplitDistanceType.Kilometer);
                    break;
                case MSHealthSplitDistanceType.None:
                default:
                    break;
            }
            // Check MaxPageSize, and append to query if applies
            if (maxPageSize != null && maxPageSize.HasValue && maxPageSize.Value > 0)
                loQuery.AppendFormat("&maxPageSize={0}", maxPageSize.Value);

            // Perform request using BASE_URI, ACTIVITIES_PATH and query string
            lsResponse = await PerformRequest(ACTIVITIES_PATH, loQuery.ToString().TrimStart(new char[] { '&' }));
            // Deserialize Json response (use Converters for Enum, DateTime and TimeSpan values)
            JsonSerializerSettings loSerializerSettings = new JsonSerializerSettings();
            loSerializerSettings.Converters.Add(new StringEnumConverter());
            loSerializerSettings.Converters.Add(new IsoDateTimeConverter());
            loSerializerSettings.Converters.Add(new TimeSpanConverter());
            loActivities = JsonConvert.DeserializeObject<MSHealthActivities>(lsResponse, loSerializerSettings);

            return loActivities;
        }

        /// <summary>
        /// Get the details of an activity by its id.
        /// </summary>
        /// <param name="id">The id of the activity to get.</param>
        /// <param name="include">The <see cref="MSHealthAPI.MSHealthActivityInclude"/> properties to return: Details, MinuteSummaries, MapPoints  (supports multi-values).</param>
        /// <returns><see cref="MSHealthAPI.MSHealthActivity"/> instance with activity details.</returns>
        public async Task<MSHealthActivity> ReadActivity(string id,
                                                         MSHealthActivityInclude include = MSHealthActivityInclude.None)
        {
            MSHealthActivity loActivity = null;
            StringBuilder loQuery = new StringBuilder();
            string lsResponse;
            string lsParamValue;

            // Check ActivityIncludes, and append to query if applies
            if (include != MSHealthActivityInclude.None)
            {
                lsParamValue = string.Empty;
                if (include.HasFlag(MSHealthActivityInclude.Details))
                    lsParamValue += string.Format(",{0}", MSHealthActivityInclude.Details);
                if (include.HasFlag(MSHealthActivityInclude.MinuteSummaries))
                    lsParamValue += string.Format(",{0}", MSHealthActivityInclude.MinuteSummaries);
                if (include.HasFlag(MSHealthActivityInclude.MapPoints))
                    lsParamValue += string.Format(",{0}", MSHealthActivityInclude.MapPoints);
                lsParamValue = lsParamValue.TrimStart(new char[] { ',' });
                loQuery.AppendFormat("&activityIncludes={0}", lsParamValue);
            }
            // Perform request using BASE_URI, ACTIVITY_PATH, id and query string
            lsResponse = await PerformRequest(string.Format(ACTIVITY_PATH, id), loQuery.ToString().TrimStart(new char[] { '&' }));
            // Deserialize Json response (use Converters for Enum, DateTime and TimeSpan values)
            JsonSerializerSettings loSerializerSettings = new JsonSerializerSettings();
            loSerializerSettings.Converters.Add(new StringEnumConverter());
            loSerializerSettings.Converters.Add(new IsoDateTimeConverter());
            loSerializerSettings.Converters.Add(new TimeSpanConverter());
            loActivity = JsonConvert.DeserializeObject<MSHealthActivity>(lsResponse, loSerializerSettings);

            return loActivity;
        }



        /// <summary>
        /// Lists daily summary data for this user by date range.
        /// </summary>
        /// <param name="startTime">Filters the set of returned summaries to those starting after the specified <see cref="DateTime"/>, inclusive.</param>
        /// <param name="endTime">Filters the set of returned summaries to those starting before the specified <see cref="DateTime"/>, exclusive. </param>
        /// <param name="deviceIds">Filters the set of returned summaries based on the comma-separated list of device ids provided.</param>
        /// <param name="maxPageSize">The maximum number of entries to return per page. Defaults to 48 for hourly and 31 for daily.</param>
        /// <returns></returns>
        public async Task<MSHealthSummaries> ListDailySummaries(DateTime? startTime = default(DateTime?),
                                                                DateTime? endTime = default(DateTime?),
                                                                string deviceIds = null,
                                                                int? maxPageSize = default(int?))
        {
            MSHealthSummaries loSummaries = null;
            StringBuilder loQuery = new StringBuilder();
            string lsResponse;

            // Check StartTime, and append to query if applies
            if (startTime != null && startTime.HasValue)
                loQuery.AppendFormat("&startTime={0}", Uri.EscapeDataString(startTime.Value.ToUniversalTime().ToString("O")));
            // Check EndTime, and append to query if applies
            if (endTime != null && endTime.HasValue)
                loQuery.AppendFormat("&endTime={0}", Uri.EscapeDataString(endTime.Value.ToUniversalTime().ToString("O")));
            // Check DeviceIds, and append to query if applies
            if (!string.IsNullOrEmpty(deviceIds))
                loQuery.AppendFormat("&deviceIds={0}", Uri.EscapeDataString(deviceIds));
            // Check MaxPageSize, and append to query if applies
            if (maxPageSize != null && maxPageSize.HasValue && maxPageSize.Value > 0)
                loQuery.AppendFormat("&maxPageSize={0}", maxPageSize.Value);

            // Perform request using BASE_URI, SUMMARIES_DAILY_PATH and query string
            lsResponse = await PerformRequest(SUMMARIES_DAILY_PATH, loQuery.ToString().TrimStart(new char[] { '&' }));
            // Deserialize Json response (use Converters for Enum, DateTime and TimeSpan values)
            JsonSerializerSettings loSerializerSettings = new JsonSerializerSettings();
            loSerializerSettings.Converters.Add(new StringEnumConverter());
            loSerializerSettings.Converters.Add(new IsoDateTimeConverter());
            loSerializerSettings.Converters.Add(new TimeSpanConverter());
            loSummaries = JsonConvert.DeserializeObject<MSHealthSummaries>(lsResponse, loSerializerSettings);

            return loSummaries;
        }

        /// <summary>
        /// Lists hourly summary data for this user by date range.
        /// </summary>
        /// <param name="startTime">Filters the set of returned summaries to those starting after the specified <see cref="DateTime"/>, inclusive.</param>
        /// <param name="endTime">Filters the set of returned summaries to those starting before the specified <see cref="DateTime"/>, exclusive. </param>
        /// <param name="deviceIds">Filters the set of returned summaries based on the comma-separated list of device ids provided.</param>
        /// <param name="maxPageSize">The maximum number of entries to return per page. Defaults to 48 for hourly and 31 for daily.</param>
        /// <returns></returns>
        public async Task<MSHealthSummaries> ListHourlySummaries(DateTime? startTime = default(DateTime?),
                                                                 DateTime? endTime = default(DateTime?),
                                                                 string deviceIds = null,
                                                                 int? maxPageSize = default(int?))
        {
            MSHealthSummaries loSummaries = null;
            StringBuilder loQuery = new StringBuilder();
            string lsResponse;

            // Check StartTime, and append to query if applies
            if (startTime != null && startTime.HasValue)
                loQuery.AppendFormat("&startTime={0}", Uri.EscapeDataString(startTime.Value.ToUniversalTime().ToString("O")));
            // Check EndTime, and append to query if applies
            if (endTime != null && endTime.HasValue)
                loQuery.AppendFormat("&endTime={0}", Uri.EscapeDataString(endTime.Value.ToUniversalTime().ToString("O")));
            // Check DeviceIds, and append to query if applies
            if (!string.IsNullOrEmpty(deviceIds))
                loQuery.AppendFormat("&deviceIds={0}", Uri.EscapeDataString(deviceIds));
            // Check MaxPageSize, and append to query if applies
            if (maxPageSize != null && maxPageSize.HasValue && maxPageSize.Value > 0)
                loQuery.AppendFormat("&maxPageSize={0}", maxPageSize.Value);

            // Perform request using BASE_URI, SUMMARIES_HOURLY_PATH and query string
            lsResponse = await PerformRequest(SUMMARIES_HOURLY_PATH, loQuery.ToString().TrimStart(new char[] { '&' }));
            // Deserialize Json response (use Converters for Enum, DateTime and TimeSpan values)
            JsonSerializerSettings loSerializerSettings = new JsonSerializerSettings();
            loSerializerSettings.Converters.Add(new StringEnumConverter());
            loSerializerSettings.Converters.Add(new IsoDateTimeConverter());
            loSerializerSettings.Converters.Add(new TimeSpanConverter());
            loSummaries = JsonConvert.DeserializeObject<MSHealthSummaries>(lsResponse, loSerializerSettings);

            return loSummaries;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets Authentication Token for Microsoft Health Cloud API.
        /// </summary>
        /// <param name="code">Authentication Code or Refresh Token.</param>
        /// <param name="isRefresh">Flag to determine if is a Refresh Token request.</param>
        /// <returns><see cref="MSHealthToken"/> instance.</returns>
        private async Task<MSHealthToken> GetToken(string code, bool isRefresh)
        {
            MSHealthToken loToken = null;
            UriBuilder loUri = new UriBuilder(TOKEN_URI);
            StringBuilder loQuery = new StringBuilder();
            WebRequest loWebRequest;
            // Build base query
            loQuery.AppendFormat("redirect_uri={0}", Uri.EscapeDataString(REDIRECT_URI));
            loQuery.AppendFormat("&client_id={0}", Uri.EscapeDataString(msClientId));
            loQuery.AppendFormat("&client_secret={0}", Uri.EscapeDataString(msClientSecret));
            // Check if is refresh request
            if (isRefresh)
            {
                // Build refresh query
                loQuery.AppendFormat("&refresh_token={0}", Uri.EscapeDataString(code));
                loQuery.Append("&grant_type=refresh_token");
            }
            else
            {
                // Build new token query
                loQuery.AppendFormat("&code={0}", Uri.EscapeDataString(code));
                loQuery.Append("&grant_type=authorization_code");
            }
            // Prepare complete URL
            loUri.Query = loQuery.ToString();
            loWebRequest = HttpWebRequest.Create(loUri.Uri);
            try
            {
                // Perform request and handle response
                using (WebResponse loWebResponse = await loWebRequest.GetResponseAsync())
                {
                    using (Stream loResponseStream = loWebResponse.GetResponseStream())
                    {
                        using (StreamReader loStreamReader = new StreamReader(loResponseStream))
                        {
                            string lsResponse = loStreamReader.ReadToEnd();
#if DESKTOP_APP
#elif WINDOWS_UWP
                            JsonObject loJsonResponse = JsonObject.Parse(lsResponse);
                            IJsonValue loJsonValue = null;
                            string lsError = null;
                            // Check for error
                            if (loJsonResponse.TryGetValue("error", out loJsonValue) && loJsonValue != null)
                                lsError = loJsonValue.GetString();
                            if (!string.IsNullOrEmpty(lsError))
                                throw new Exception(lsError);
#endif
                            // Deserialize Json response
                            loToken = JsonConvert.DeserializeObject<MSHealthToken>(lsResponse);
                            if (string.IsNullOrEmpty(loToken.RefreshToken))
                                loToken.RefreshToken = code;
                        }
                    }
                }
            }
            catch (Exception loException)
            {
                throw new MSHealthException(loException.Message, loException, loUri.Path, loUri.Query);
            }

            return loToken;
        }

        /// <summary>
        /// Perform general Microsoft Health API requests using <see cref="BASE_URI"/>.
        /// </summary>
        /// <param name="path">Path to resource to request.</param>
        /// <param name="query">Query to resource to request.</param>
        /// <returns><see cref="string"/> response to request (generally is a Json string).</returns>
        private async Task<string> PerformRequest(string path, string query = null)
        {
            string lsResponse = null;
            UriBuilder loUriBuilder = null;

            // Validate Token and Refresh if necessary
            if (moScope.HasFlag(MSHealthScope.OfflineAccess))
            {
                if (!(await ValidateToken(Token, true)))
                {
                    throw new ArgumentNullException("Token");
                }
            }
            // Prepare URL request
            loUriBuilder = new UriBuilder(BASE_URI);
            loUriBuilder.Path += path;
            loUriBuilder.Query = query;
            WebRequest loWebRequest = HttpWebRequest.Create(loUriBuilder.Uri);
            loWebRequest.Headers[HttpRequestHeader.Authorization] = string.Format("{0} {1}", Token.TokenType, Token.AccessToken);
            try
            {
                // Perform request and handle response
                using (WebResponse loWebResponse = await loWebRequest.GetResponseAsync())
                {
                    using (Stream loResponseStream = loWebResponse.GetResponseStream())
                    {
                        using (StreamReader loStreamReader = new StreamReader(loResponseStream))
                        {
                            // Get response as string
                            lsResponse = await loStreamReader.ReadToEndAsync();
                        }
                    }
                }
            }
            catch (Exception loException)
            {
                throw new MSHealthException(loException.Message, loException, path, query);
            }

            return lsResponse;
        }

        #endregion

    }

    #endregion

    #region MSHealthToken class

    /// <summary>
    /// Authentication Token to Microsoft Health Cloud API.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthToken
    {

        #region Constants

        /// <summary>
        /// Default token type.
        /// </summary>
        public const string TOKEN_TYPE = "bearer";

        #endregion

        #region Properties

        /// <summary>
        /// Authorization type: "Bearer" in this case.
        /// </summary>
        [JsonProperty(PropertyName = "token_type",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string TokenType { get; set; } //= TOKEN_TYPE;

        /// <summary>
        /// The amount of time in seconds when the access token is valid.
        /// </summary>
        /// <remarks>
        /// You can request a new access token by using the refresh token (if available),
        /// or by repeating the authentication request from the beginning.
        /// </remarks>
        [JsonProperty(PropertyName = "expires_in",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public long ExpiresIn { get; set; }

        /// <summary>
        /// A space-separated list of scopes that your app requires.
        /// </summary>
        [JsonProperty(PropertyName = "scope",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Scope { get; set; }

        /// <summary>
        /// Access token to authenticate against Microsoft Health Cloud APIs
        /// </summary>
        [JsonProperty(PropertyName = "access_token",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string AccessToken { get; set; }

        /// <summary>
        /// The refresh token received previously.
        /// </summary>
        [JsonProperty(PropertyName = "refresh_token",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Time when current access token was created.
        /// </summary>
        [JsonIgnore]
        public DateTime CreationTime { get; set; } //= DateTime.Now;

        /// <summary>
        /// Expected Time when current access token expires.
        /// </summary>
        /// <remarks>
        /// This value is calculated using <see cref="CreationTime"/> and
        /// <see cref="ExpiresIn"/> values.
        /// </remarks>
        [JsonIgnore]
        public DateTime ExpirationTime
        {
            get { return CreationTime.AddSeconds(ExpiresIn); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="MSHealthToken"/> class.
        /// </summary>
        public MSHealthToken()
        {
            TokenType = TOKEN_TYPE;
            CreationTime = DateTime.Now;
        }

        #endregion

    }

    #endregion

    #region MSHealthProfile class

    /// <summary>
    /// General profile of the person using Microsoft Health.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthProfile
    {

        #region Properties

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        [JsonProperty(PropertyName = "firstName",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's middle name.
        /// </summary>
        [JsonProperty(PropertyName = "middleName",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        [JsonProperty(PropertyName = "lastName",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the last update time of the user's profile record.
        /// </summary>
        [JsonProperty(PropertyName = "lastUpdateTime",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// Gets or sets the user's birth date.
        /// </summary>
        [JsonProperty(PropertyName = "birthdate",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public DateTime Birthdate { get; set; }

        /// <summary>
        /// Gets or sets the user's postal code.
        /// </summary>
        [JsonProperty(PropertyName = "postalCode",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the user's gender.
        /// </summary>
        [JsonProperty(PropertyName = "gender",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the user's current height.
        /// </summary>
        [JsonProperty(PropertyName = "height",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the user's current weight.
        /// </summary>
        [JsonProperty(PropertyName = "weight",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public int Weight { get; set; }

        /// <summary>
        /// Gets or sets the user's preferred locale.
        /// </summary>
        [JsonProperty(PropertyName = "preferredLocale",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string PreferredLocale { get; set; }

        #endregion

    }

    #endregion

    #region MSHealthRequestException class

    /// <summary>
    /// Represents error that occur during <see cref="MSHealthClient"/> operations execution.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public sealed class MSHealthException : Exception
    {

        #region Properties

        /// <summary>
        /// Gets the error response for request.
        /// </summary>
        public MSHealthError Error { get; private set; }

        /// <summary>
        /// Gets the error <see cref="HttpWebResponse"/> for request.
        /// </summary>
        public HttpWebResponse HttpWebResponse { get; private set; }

        /// <summary>
        /// Gets the path for the request that raises the error.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the query for the request that raises the error.
        /// </summary>
        public string Query { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MSHealthException"/> class.
        /// </summary>
        /// <param name="message"><see cref="Exception.Message"/>.</param>
        /// <param name="innerException"><see cref="Exception.InnerException"/>.</param>
        /// <param name="path">Path for the request that raises the error.</param>
        /// <param name="query">Query for the request that raises the error.</param>
        public MSHealthException(string message, Exception innerException, string path, string query) :
            base(message, innerException)
        {
            Path = path;
            Query = query;
            // Check if inner exception is a WebException
            WebException loWebException = innerException as WebException;
            if (loWebException != null)
            {
                // Get WebResponse for inner Exception, and handle it
                if (loWebException.Response != null)
                {
                    HttpWebResponse = loWebException.Response as HttpWebResponse;
                    if (HttpWebResponse != null &&
                        System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debug.WriteLine("Exception StatusCode: {0}", HttpWebResponse.StatusCode);
                    // Get response details
                    using (Stream loResponseStream = loWebException.Response.GetResponseStream())
                    {
                        using (StreamReader loStreamReader = new StreamReader(loResponseStream))
                        {
                            // Read response as string (must be a Json string)
                            string lsErrorResponse = loStreamReader.ReadToEnd();
                            if (!string.IsNullOrEmpty(lsErrorResponse))
                            {
                                // Deserialize response (Json)
                                JsonSerializerSettings loSerializerSettings = new JsonSerializerSettings();
                                loSerializerSettings.Error = (sender, args) =>
                                {
                                    if (System.Diagnostics.Debugger.IsAttached)
                                        System.Diagnostics.Debug.WriteLine(args.ErrorContext.Error.Message);
                                    args.ErrorContext.Handled = true;
                                };
                                Error = JsonConvert.DeserializeObject<MSHealthError>(lsErrorResponse, loSerializerSettings);
                            }
                        }
                    }
                }
            }
        }

        #endregion

    }

    #endregion

    #region MSHealthError class

    /// <summary>
    /// Represents error reponse to Microsoft Health Cloud API requests.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthError
    {

        #region Properties

        /// <summary>
        /// Gets or sets the error information.
        /// </summary>
        [JsonProperty(PropertyName = "error",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthErrorInformation Error { get; set; }

        #endregion

    }

    #endregion

    #region MSHealthErrorInformation class

    /// <summary>
    /// Represents error information to Microsoft Health Cloud API requests.
    /// </summary>
    [JsonObject]
    public sealed class MSHealthErrorInformation
    {

        #region Properties

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [JsonProperty(PropertyName = "code",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [JsonProperty(PropertyName = "message",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the error target.
        /// </summary>
        [JsonProperty(PropertyName = "target",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Target { get; set; }

        /// <summary>
        /// Gets or sets the error details.
        /// </summary>
        [JsonProperty(PropertyName = "details",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<MSHealthErrorInformation> Details { get; set; }

        /// <summary>
        /// Gets or sets the inner error.
        /// </summary>
        [JsonProperty(PropertyName = "innererror",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public MSHealthErrorInformation InnerError { get; set; }

        #endregion

    }

    #endregion

    #region MSHealthScope enum

    /// <summary>
    /// Scopes (or access types) for different types of user data on Microsoft Health.
    /// </summary>
    [Flags]
    public enum MSHealthScope
    {
        /// <summary>
        /// No access.
        /// </summary>
        None = 0,
        /// <summary>
        /// Access to profile data.
        /// </summary>
        /// <remarks>
        /// Profile includes things like name, gender, weight, and age.
        /// Email address will not be shared.
        /// </remarks>
        ReadProfile = 1,
        /// <summary>
        /// Access to daily and historical activity information.
        /// </summary>
        /// <remarks>
        /// Activity history includes things like runs, workouts, sleep, and daily steps.
        /// </remarks>
        ReadActivityHistory = 2,
        /// <summary>
        /// Access information about the devices associated with used Microsoft Health account.
        /// </summary>
        ReadDevices = 4,
        /// <summary>
        /// Access location information for activities.
        /// </summary>
        ReadActivityLocation = 8,
        /// <summary>
        /// Receive a refresh token so it can work offline even when the user isn't active.
        /// </summary>
        OfflineAccess = 16,
        /// <summary>
        /// All previous access listed (except <see cref="MSHealthScope.None"/>).
        /// </summary>
        All = 32,
    }

    #endregion

    #region MSHealthNavigationResult enum

    /// <summary>
    /// Result for <see cref="IMSHealthClient.HandleNavigationCompleted(WebViewNavigationCompletedEventArgs)"/>.
    /// </summary>
    public enum MSHealthNavigationResult
    {
        /// <summary>
        /// No relevant navigation was handled.
        /// </summary>
        None,
        /// <summary>
        /// Navigation handled successfully Sing-in request.
        /// </summary>
        SignIn,
        /// <summary>
        /// Navigation handled successfully Sing-out request.
        /// </summary>
        SignOut,
        /// <summary>
        /// Navigation failed to handle request.
        /// </summary>
        Error,
    }

    #endregion

}
