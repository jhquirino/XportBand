//-----------------------------------------------------------------------
// <copyright file="NikePlusClient.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace NikePlusAPI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Xml.Linq;
    using Newtonsoft.Json;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Converters;

    #region INikePlusClient interface

    /// <summary>
    /// Interface for Client to consume Nike+ API.
    /// </summary>
    public interface INikePlusClient
    {

        #region Properties

        /// <summary>
        /// Gets a value indicating whether API Client is Signed-in to Nike+.
        /// </summary>
        bool IsSignedIn { get; }

        /// <summary>
        /// Gets authorization token for Nike+ API (<see cref="NikePlusToken"/>).
        /// </summary>
        NikePlusToken Token { get; }

        #endregion

        #region Public methods (API v1)

        /// <summary>
        /// Sets the Nike+ credentials (user and password).
        /// </summary>
        /// <param name="user">Nike+ user (e-mail).</param>
        /// <param name="password">Nike+ password.</param>
        void SetCredentials(string user, string password);

        /// <summary>
        /// Signs-in to Nike+ service.
        /// </summary>
        /// <param name="forceRefreshToken">Flag to determine if force to get a new <see cref="Token"/>, even if one valid is available.</param>
        /// <returns><see langword="true"/> if signed-in successfully, otherwise, <see langword="false"/>.</returns>
        Task<bool> SignIn(bool forceRefreshToken = false);

        /// <summary>
        /// Signs-out from Nike+ service.
        /// </summary>
        /// <returns><see langword="true"/> if signed-out successfully, otherwise, <see langword="false"/>.</returns>
        Task<bool> SignOut();

        /// <summary>
        /// Gets a list of summary details of the user's Activities.
        /// </summary>
        /// <param name="startDate">Start date (required if endDate is used).</param>
        /// <param name="endDate">End date (required if startDate is used).</param>
        /// <param name="count">Pagination: number of records to retrieve (default 5)</param>
        /// <param name="offset">Pagination: first record to retrieve (starts at 1)</param>
        /// <param name="page">Pagination: Path to navigate to specific page.</param>
        /// <returns>Collection of activities</returns>
        Task<NikePlusActivities> ListActivities(DateTime? startDate = null, DateTime? endDate = null, int? count = null, int? offset = null, string page = null);

        /// <summary>
        /// Gets a summary details of the user's Activity.
        /// </summary>
        /// <param name="id">The unique Activity ID to retrieve.</param>
        /// <param name="includeGPS">Flag to determine whether to read GPS details.</param>
        /// <returns>Activity details.</returns>
        Task<NikePlusActivity> ReadActivity(string id, bool includeGPS = false);

        /// <summary>
        /// Gets GPS details of the user's Activity.
        /// </summary>
        /// <param name="id">The unique Activity ID to retrieve.</param>
        /// <returns>GPS details of Activity.</returns>
        Task<NikePlusGPS> ReadActivityGPS(string id);

        /// <summary>
        /// Adds Activity to user's history.
        /// </summary>
        /// <param name="activity">The activity details to add.</param>
        /// <returns>Metric Summary for activity added.</returns>
        /// <remarks>
        /// Though this method is implemented accordingly to Nike+ API documentation
        /// (https://developer.nike.com/documentation/api-docs/activity-services/add-activities.html)
        /// it doesn't work (returns a resource not available error), it seems that
        /// Nike doesn't expose this functionallity, the method is leaved to use when
        /// Nike finally expose this functionallity.
        /// If you want to "upload" activities, you can use the v2.0 approach
        /// (<see cref="SyncActivityV2"/>).
        /// </remarks>
        Task<NikePlusMetricSummary> AddActivity(NikePlusActivity activity);

        #endregion

        #region Public methods (API v2.0)

        /// <summary>
        /// Gets access token using Nike+ API v2.0.
        /// </summary>
        /// <returns>Access token.</returns>
        Task<string> GetAccessTokenV2();

        /// <summary>
        /// Syncs (upload) Nike+ Activity using Nike+ API v2.0.
        /// </summary>
        /// <param name="activity">Activity details to upload.</param>
        /// <param name="accessToken">Access token to connect Nike+.</param>
        /// <returns>ID of Activity successfully uploaded.</returns>
        Task<string> SyncActivityV2(NikePlusActivity activity, string accessToken = null);

        /// <summary>
        /// Ends Sync Nike+ Activity using Nike+ API v2.0.
        /// </summary>
        /// <param name="accessToken">Access token to connect Nike+.</param>
        Task EndSyncV2(string accessToken);

        #endregion

    }

    #endregion

    #region NikePlusClient class

    /// <summary>
    /// Client to consume Nike+ API.
    /// </summary>
    public sealed class NikePlusClient : INikePlusClient
    {

        #region Constants

        /// <summary>
        /// HACK: Nike+ API - URL for Login request (test developer console).
        /// </summary>
        private const string LOGIN_URI_DEV = "https://developer.nike.com/services/login";

        /// <summary>
        /// HACK: Nike+ API - User(e-mail) paremeter for Login request (test developer console).
        /// </summary>
        private const string USERNAME_PARAM = "username";

        /// <summary>
        /// HACK: Nike+ API - Password paremeter for Login request (test developer console).
        /// </summary>
        private const string PASSWORD_PARAM = "password";

        /// <summary>
        /// HACK: Nike+ API v2.0 - URL for Login request.
        /// </summary>
        private const string LOGIN_URI_V2 = "https://secure-nikeplus.nike.com/login/loginViaNike.do?mode=login";

        /// <summary>
        /// HACK: Nike+ API v2.0 - Domain for Login request.
        /// </summary>
        private const string LOGIN_DOMAIN_V2 = "secure-nikeplus.nike.com";

        /// <summary>
        /// HACK: Nike+ API v2.0 - E-mail paremeter for Login request.
        /// </summary>
        private const string EMAIL_PARAM = "email";

        /// <summary>
        /// HACK: Nike+ API v2.0 - Cookie name for App.
        /// </summary>
        private const string APP_COOKIE_V2 = "app";

        /// <summary>
        /// HACK: Nike+ API v2.0 - Cookie name for Client ID.
        /// </summary>
        private const string CLIENT_ID_COOKIE_V2 = "client_id";

        /// <summary>
        /// HACK: Nike+ API v2.0 - Cookie name for Client Secret.
        /// </summary>
        private const string CLIENT_SECRET_COOKIE_V2 = "client_secret";

        /// <summary>
        /// Nike+ API - Base URL for requests.
        /// </summary>
        private const string BASE_URI = "https://api.nike.com";

        /// <summary>
        /// Nike+ API v2.0 - Base URL for requests.
        /// </summary>
        private const string BASE_URI_V2 = "https://api.nike.com";

        /// <summary>
        /// Nike+ API - Path for Activities list.
        /// </summary>
        private const string ACTIVITIES_PATH = "/v1/me/sport/activities";

        /// <summary>
        /// Nike+ API - Path for single Activity details.
        /// </summary>
        private const string ACTIVITY_PATH = "/v1/me/sport/activities/{0}";

        /// <summary>
        /// Nike+ API - Path for single Activity GPS details.
        /// </summary>
        private const string ACTIVITY_GPS_PATH = "/v1/me/sport/activities/{0}/gps";

        /// <summary>
        /// Nike+ API - Path to Disconnect from service.
        /// </summary>
        private const string DISCONNECT_PATH = "/v1/me/app";

        /// <summary>
        /// Nike+ API v2.0 - User Agent to use for requests.
        /// </summary>
        private const string USER_AGENT_V2 = "NPConnect";

        /// <summary>
        /// Nike+ API v2.0 - Path to Sync Activity data.
        /// </summary>
        private const string SYNC_PATH_V2 = "/v2.0/me/sync";

        /// <summary>
        /// Nike+ API v2.0 - Path to Validate Sync Activity data Completion.
        /// </summary>
        private const string SYNC_COMPLETE_PATH_V2 = "/v2.0/me/sync/complete";

        /// <summary>
        /// URL for Password Pattern issue (contains a greater than symbol, ampersand or apostrophe).
        /// </summary>
        public const string URL_PASSWORD_PATTERN = "http://support-en-us.nikeplus.com/app/answers/detail/a_id/31352/p/3169,3195";

        /// <summary>
        /// URL (Pattern) for Activity details.
        /// </summary>
        public const string PATTERN_URL_ACTIVITY_DETAILS = "https://secure-nikeplus.nike.com/plus/activity/running/detail/{0}";

        #endregion

        #region Inner members

        /// <summary>
        /// Nike+ user name (e-mail) to sign-in.
        /// </summary>
        private string msUser;

        /// <summary>
        /// Nike+ password to sign-in.
        /// </summary>
        private string msPassword;

        /// <summary>
        /// The ID for registerd app.
        /// </summary>
        private readonly string msApp;

        /// <summary>
        /// The Client ID for registerd app.
        /// </summary>
        private readonly string msClientId;

        /// <summary>
        /// The Client Secret for registerd app.
        /// </summary>
        private readonly string msClientSecret;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether API Client is Signed-in to Nike+.
        /// </summary>
        public bool IsSignedIn { get; private set; }

        /// <summary>
        /// Gets authorization token for Nike+ API (<see cref="NikePlusToken"/>).
        /// </summary>
        public NikePlusToken Token { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NikePlusClient"/> class.
        /// </summary>
        public NikePlusClient() : this(null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NikePlusClient"/> class.
        /// </summary>
        /// <param name="app">The ID for registerd app.</param>
        /// <param name="clientId">The Client ID for registerd app.</param>
        /// <param name="clientSecret">The Client Secret for registerd app.</param>
        /// <remarks>
        /// Use this constructor to use API v2.0 methods (<see cref="GetAccessTokenV2"/> and <see cref="SyncActivityV2"/>).
        /// </remarks>
        public NikePlusClient(string app, string clientId, string clientSecret) : this(null, null, app, clientId, clientSecret) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NikePlusClient"/> class.
        /// </summary>
        /// <param name="user">Nike+ user (e-mail).</param>
        /// <param name="password">Nike+ password.</param>
        public NikePlusClient(string user, string password)
        {
            msUser = user;
            msPassword = password;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NikePlusClient"/> class.
        /// </summary>
        /// <param name="user">Nike+ user (e-mail).</param>
        /// <param name="password">Nike+ password.</param>
        /// <param name="app">The ID for registerd app.</param>
        /// <param name="clientId">The Client ID for registerd app.</param>
        /// <param name="clientSecret">The Client Secret for registerd app.</param>
        /// <remarks>
        /// Use this constructor to use API v2.0 methods (<see cref="GetAccessTokenV2"/> and <see cref="SyncActivityV2"/>).
        /// </remarks>
        public NikePlusClient(string user, string password, string app, string clientId, string clientSecret) : this(user, password)
        {
            msApp = app;
            msClientId = clientId;
            msClientSecret = clientSecret;
        }

        #endregion

        #region Public methods (API v1)

        /// <summary>
        /// Sets the Nike+ credentials (user and password).
        /// </summary>
        /// <param name="user">Nike+ user (e-mail).</param>
        /// <param name="password">Nike+ password.</param>
        public void SetCredentials(string user, string password)
        {
            Token = null;
            IsSignedIn = false;
            msUser = user;
            msPassword = password;
        }

        /// <summary>
        /// Signs-in to Nike+ service.
        /// </summary>
        /// <param name="forceRefreshToken">Flag to determine if force to get a new <see cref="Token"/>, even if one valid is available.</param>
        /// <returns><see langword="true"/> if signed-in successfully, otherwise, <see langword="false"/>.</returns>
        public async Task<bool> SignIn(bool forceRefreshToken = false)
        {
            bool lbSignedIn = false;
            bool lbValidToken = false;
            WebRequest loWebRequest = null;
            StringBuilder loPostData = new StringBuilder();
            string lsResponse;
            // Validate credentials
            if (string.IsNullOrEmpty(msUser) ||
                string.IsNullOrEmpty(msPassword))
                throw new NikePlusException("Invalid credentials, provide Nike+ user and password.", null, null, null);
            // Validate current Token
            lbSignedIn = false;
            lbValidToken = ValidateToken();
            // Check if refresh token must be forced
            if (forceRefreshToken)
                lbValidToken = false;
            lbSignedIn = lbValidToken;
            if (!lbSignedIn)
            {
                // Reset values
                Token = null;
                IsSignedIn = false;
                try
                {
                    // Build query data for user/password provided
                    loPostData.AppendFormat("{0}={1}", USERNAME_PARAM, Uri.EscapeDataString(msUser));
                    loPostData.AppendFormat("&{0}={1}", PASSWORD_PARAM, Uri.EscapeDataString(msPassword));
                    // Prepare request
                    loWebRequest = WebRequest.Create(LOGIN_URI_DEV);
                    //byte[] loPostContent = Encoding.Default.GetBytes(loPostData.ToString());
                    byte[] loPostContent = Encoding.UTF8.GetBytes(loPostData.ToString());
                    loWebRequest.Method = "POST";
                    loWebRequest.ContentType = "application/x-www-form-urlencoded";
#if DESKTOP_APP
					loWebRequest.ContentLength = loPostContent.Length;
#endif
                    // Post request
                    using (Stream loRequestStream = await loWebRequest.GetRequestStreamAsync())
                    {
                        loRequestStream.Write(loPostContent, 0, loPostContent.Length);
                        loRequestStream.Flush();
#if DESKTOP_APP
						loRequestStream.Close();
#endif
                    }
                    // Read response (Json formatted)
                    using (WebResponse loWebResponse = await loWebRequest.GetResponseAsync())
                    {
                        using (Stream loResponseStream = loWebResponse.GetResponseStream())
                        {
                            using (StreamReader loStreamReader = new StreamReader(loResponseStream))
                            {
                                lsResponse = loStreamReader.ReadToEnd();
                                // Deserialize Json response
                                Token = JsonConvert.DeserializeObject<NikePlusToken>(lsResponse);
                                IsSignedIn = true;
                                lbSignedIn = true;
                            }
                        }
                    }
                }
                catch (WebException loWebException)
                {
                    Token = null;
                    IsSignedIn = false;
                    lbSignedIn = false;
                    throw new NikePlusException(loWebException.Message, loWebException, LOGIN_URI_DEV, null);
                }
                catch (Exception loException)
                {
                    // (Just for debugging purposes: handle response exception).
                    if (System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debug.WriteLine(loException.StackTrace);
                    Token = null;
                    IsSignedIn = false;
                    lbSignedIn = false;
                    throw;
                }
            }
            return lbSignedIn;
        }

        /// <summary>
        /// Signs-out from Nike+ service.
        /// </summary>
        /// <returns><see langword="true"/> if signed-out successfully, otherwise, <see langword="false"/>.</returns>
        public async Task<bool> SignOut()
        {
            bool lbSignedOut = false;
            if (Token != null &&
                !string.IsNullOrEmpty(Token.AccessToken))
            {
                try
                {
                    await PerformRequest(DISCONNECT_PATH, null, "DELETE");
                    Token = null;
                    IsSignedIn = false;
                    lbSignedOut = true;
                }
                catch (Exception loException)
                {
                    // (Just for debugging purposes: handle response exception).
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debug.WriteLine(loException.StackTrace);
                    } // (Just for debugging purposes: handle response exception)
                    Token = null;
                    IsSignedIn = false;
                    lbSignedOut = true;
                }
            }
            else
            {
                Token = null;
                IsSignedIn = false;
                lbSignedOut = true;
            }
            return lbSignedOut;
        }

        /// <summary>
        /// Gets a list of summary details of the user's Activities.
        /// </summary>
        /// <param name="startDate">Start date (required if endDate is used).</param>
        /// <param name="endDate">End date (required if startDate is used).</param>
        /// <param name="count">Pagination: number of records to retrieve (default 5)</param>
        /// <param name="offset">Pagination: first record to retrieve (starts at 1)</param>
        /// <param name="page">Pagination: Path to navigate to specific page.</param>
        /// <returns>Collection of activities</returns>
        public async Task<NikePlusActivities> ListActivities(DateTime? startDate = null,
                                                             DateTime? endDate = null,
                                                             int? count = null,
                                                             int? offset = null,
                                                             string page = null)
        {
            NikePlusActivities loActivities = null;
            string lsResponse = null;
            StringBuilder loQuery = new StringBuilder();
            // Check if Page is provided
            if (!string.IsNullOrEmpty(page))
            {
                // Get query parameters from page
                UriBuilder loUriBuilder = new UriBuilder(string.Format("{0}{1}", BASE_URI, page));
                loQuery.Append(loUriBuilder.Query);
            }
            else
            {
                // Check StartDate/EndDate, and append to query if applies
                if (startDate != null &&
                    endDate != null)
                {
                    loQuery.AppendFormat("&startDate={0}", Uri.EscapeDataString(startDate.Value.ToString("yyyy-MM-dd")));
                    loQuery.AppendFormat("&endDate={0}", Uri.EscapeDataString(endDate.Value.ToString("yyyy-MM-dd")));
                }
                // Check pagination Count, and append to query if applies
                if (count != null)
                    loQuery.AppendFormat("&count={0}", count);
                // Check pagination Count, and append to query if applies
                if (offset != null)
                    loQuery.AppendFormat("&offset={0}", offset);
            }
            // Perform request using BASE_URI, ACTIVITIES_PATH and query string
            lsResponse = await PerformRequest(ACTIVITIES_PATH, loQuery.ToString().TrimStart(new char[] { '&', '?' }));
            // Deserialize Json response (use Converters for Enum and DateTime values)
            JsonSerializerSettings loSerializerSettings = new JsonSerializerSettings();
            loSerializerSettings.Converters.Add(new StringEnumConverter());
            loSerializerSettings.Converters.Add(new IsoDateTimeConverter());
            loSerializerSettings.Error = (sender, args) =>
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debug.WriteLine(args.ErrorContext.Error.Message);
                args.ErrorContext.Handled = true;
            };
            loActivities = JsonConvert.DeserializeObject<NikePlusActivities>(lsResponse, loSerializerSettings);

            return loActivities;
        }

        /// <summary>
        /// Gets a summary details of the user's Activity.
        /// </summary>
        /// <param name="id">The unique Activity ID to retrieve.</param>
        /// <param name="includeGPS">Flag to determine whether to read GPS details.</param>
        /// <returns>Activity details.</returns>
        public async Task<NikePlusActivity> ReadActivity(string id, bool includeGPS = false)
        {
            NikePlusActivity loActivity = null;
            string lsResponse = null;
            StringBuilder loQuery = new StringBuilder();
            // Perform request using BASE_URI, ACTIVITY_PATH and query string
            lsResponse = await PerformRequest(string.Format(ACTIVITY_PATH, id), loQuery.ToString().TrimStart(new char[] { '&' }));
            // Deserialize Json response (use Converters for Enum and DateTime values)
            JsonSerializerSettings loSerializerSettings = new JsonSerializerSettings();
            loSerializerSettings.Converters.Add(new StringEnumConverter());
            loSerializerSettings.Converters.Add(new IsoDateTimeConverter());
            loSerializerSettings.Error = (sender, args) =>
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debug.WriteLine(args.ErrorContext.Error.Message);
                args.ErrorContext.Handled = true;
            };
            loActivity = JsonConvert.DeserializeObject<NikePlusActivity>(lsResponse, loSerializerSettings);
            // Read GPS information
            if (includeGPS &&
                loActivity.IsGPSActivity != null &&
                loActivity.IsGPSActivity.Value)
            {
                loActivity.GPSData = await ReadActivityGPS(id);
            }

            return loActivity;
        }

        /// <summary>
        /// Gets GPS details of the user's Activity.
        /// </summary>
        /// <param name="id">The unique Activity ID to retrieve.</param>
        /// <returns>GPS details of Activity.</returns>
        public async Task<NikePlusGPS> ReadActivityGPS(string id)
        {
            NikePlusGPS loGPS = null;
            string lsResponse = null;
            StringBuilder loQuery = new StringBuilder();
            // Perform request using BASE_URI, ACTIVITY_PATH and query string
            lsResponse = await PerformRequest(string.Format(ACTIVITY_GPS_PATH, id), loQuery.ToString().TrimStart(new char[] { '&' }));
            // Deserialize Json response (use Converters for Enum and DateTime values)
            JsonSerializerSettings loSerializerSettings = new JsonSerializerSettings();
            loSerializerSettings.Converters.Add(new StringEnumConverter());
            loSerializerSettings.Converters.Add(new IsoDateTimeConverter());
            loSerializerSettings.Error = (sender, args) =>
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debug.WriteLine(args.ErrorContext.Error.Message);
                args.ErrorContext.Handled = true;
            };
            loGPS = JsonConvert.DeserializeObject<NikePlusGPS>(lsResponse, loSerializerSettings);

            return loGPS;
        }

        /// <summary>
        /// Adds Activity to user's history.
        /// </summary>
        /// <param name="activity">The activity details to add.</param>
        /// <returns>Metric Summary for activity added.</returns>
        /// <remarks>
        /// Though this method is implemented accordingly to Nike+ API documentation
        /// (https://developer.nike.com/documentation/api-docs/activity-services/add-activities.html)
        /// it doesn't work (returns a resource not available error), it seems that
        /// Nike doesn't expose this functionallity, the method is leaved to use when
        /// Nike finally expose this functionallity.
        /// If you want to "upload" activities, you can use the v2.0 approach
        /// (<see cref="SyncActivityV2"/>).
        /// </remarks>
        public async Task<NikePlusMetricSummary> AddActivity(NikePlusActivity activity)
        {
            NikePlusMetricSummary loMetricSummary = null;
            JsonSerializerSettings loSerializerSettings = null;
            string lsResponse = null;
            string lsPostData = null;
            byte[] loPostData = null;
            if (activity != null)
            {
                // Serialize Json request
                loSerializerSettings = new JsonSerializerSettings();
                loSerializerSettings.Converters.Add(new NikePlusActivityPostConverter());
                lsPostData = JsonConvert.SerializeObject(activity, loSerializerSettings);
                loPostData = Encoding.UTF8.GetBytes(lsPostData);
                // Perform request using BASE_URI and ACTIVITIES_PATH
                lsResponse = await PerformRequest(ACTIVITIES_PATH, null, "POST", loPostData);
                // Deserialize Json response (use Converters for Enum and DateTime values)
                loSerializerSettings = new JsonSerializerSettings();
                loSerializerSettings.Converters.Add(new StringEnumConverter());
                loSerializerSettings.Converters.Add(new IsoDateTimeConverter());
                loSerializerSettings.Error = (sender, args) =>
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debug.WriteLine(args.ErrorContext.Error.Message);
                    args.ErrorContext.Handled = true;
                };
                loMetricSummary = JsonConvert.DeserializeObject<NikePlusMetricSummary>(lsResponse, loSerializerSettings);
            }
            return loMetricSummary;
        }

        #endregion

        #region Public methods (API v2.0)

        /// <summary>
        /// Gets access token using Nike+ API v2.0.
        /// </summary>
        /// <returns>Access token.</returns>
        public async Task<string> GetAccessTokenV2()
        {
            string lsAccessToken = null;
            HttpWebRequest loWebRequest = null;
            StringBuilder loPostData = new StringBuilder();
            // Check for API settings
            if (string.IsNullOrEmpty(msApp))
                throw new NikePlusException("App not provided for API v2.0", null, null, null);
            if (string.IsNullOrEmpty(msClientId))
                throw new NikePlusException("ClientID not provided for API v2.0", null, null, null);
            if (string.IsNullOrEmpty(msClientSecret))
                throw new NikePlusException("ClientSecret not provided for API v2.0", null, null, null);
            // Check credentials
            if (string.IsNullOrEmpty(msUser) ||
                string.IsNullOrEmpty(msPassword))
                throw new NikePlusException("Invalid credentials, provide Nike+ user and password.", null, null, null);
            try
            {
                Uri loUri = new Uri(LOGIN_URI_V2);
                // Build query data for user/password provided
                loPostData.AppendFormat("{0}={1}", EMAIL_PARAM, Uri.EscapeDataString(msUser));
                loPostData.AppendFormat("&{0}={1}", PASSWORD_PARAM, Uri.EscapeDataString(msPassword));
                byte[] loPostContent = Encoding.UTF8.GetBytes(loPostData.ToString());
                // Cookies
                CookieContainer loCookieContainer = new CookieContainer();
                loCookieContainer.Add(loUri, new Cookie(APP_COOKIE_V2, msApp, "/", LOGIN_DOMAIN_V2));
                loCookieContainer.Add(loUri, new Cookie(CLIENT_ID_COOKIE_V2, msClientId, "/", LOGIN_DOMAIN_V2));
                loCookieContainer.Add(loUri, new Cookie(CLIENT_SECRET_COOKIE_V2, msClientSecret, "/", LOGIN_DOMAIN_V2));
                // Prepare request
                loWebRequest = WebRequest.Create(loUri) as HttpWebRequest;
                loWebRequest.CookieContainer = loCookieContainer;
#if DESKTOP_APP
				loWebRequest.UserAgent = USER_AGENT_V2;
#elif WINDOWS_UWP
                loWebRequest.Headers["user-agent"] = USER_AGENT_V2;
#endif
                loWebRequest.Method = "POST";
                loWebRequest.ContentType = "application/x-www-form-urlencoded";
#if DESKTOP_APP
				loWebRequest.ContentLength = loPostContent.Length;
#endif
                // Post request
                using (Stream loRequestStream = await loWebRequest.GetRequestStreamAsync())
                {
                    loRequestStream.Write(loPostContent, 0, loPostContent.Length);
                    loRequestStream.Flush();
#if DESKTOP_APP
					loRequestStream.Close();
#endif
                }
                // Read response
                using (WebResponse loWebResponse = await loWebRequest.GetResponseAsync())
                {
                    HttpWebResponse loHttpWebResponse = loWebResponse as HttpWebResponse;
                    if (loHttpWebResponse != null &&
                        loHttpWebResponse.Cookies != null)
                    {
                        // (Just for debugging purposes: show cookies).
                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            foreach (Cookie _cookie in loHttpWebResponse.Cookies)
                                System.Diagnostics.Debug.WriteLine("{0}={1}", _cookie.Name, _cookie.Value);
                        } // (Just for debugging purposes: show cookies).
                        Cookie loCookie = loHttpWebResponse.Cookies.Cast<Cookie>()
                            .FirstOrDefault(loItm => loItm.Name.Equals("access_token",
                                                                       StringComparison.OrdinalIgnoreCase));
                        if (loCookie != null)
                            lsAccessToken = loCookie.Value;
                    }
                    // (Just for debugging purposes: show response).
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        try
                        {
                            using (Stream loResponseStream = loWebResponse.GetResponseStream())
                            {
                                using (StreamReader loStreamReader = new StreamReader(loResponseStream))
                                {
                                    string lsResponse = loStreamReader.ReadToEnd();
                                    System.Diagnostics.Debug.WriteLine(lsResponse);
                                }
                            }
                        }
                        catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.StackTrace); }
                    } // (Just for debugging purposes: show response).
                }
            }
            catch (WebException loWebException)
            {
                lsAccessToken = null;
                throw new NikePlusException(loWebException.Message, loWebException, LOGIN_URI_DEV, null);
            }
            catch (Exception loException)
            {
                // (Just for debugging purposes: show exception).
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debug.WriteLine(loException.StackTrace);
                lsAccessToken = null;
                throw;
            }
            return lsAccessToken;
        }

        /// <summary>
        /// Syncs (upload) Nike+ Activity using Nike+ API v2.0.
        /// </summary>
        /// <param name="activity">Activity details to upload.</param>
        /// <param name="accessToken">Access token to connect Nike+.</param>
        /// <returns>ID of Activity successfully uploaded.</returns>
        public async Task<string> SyncActivityV2(NikePlusActivity activity, string accessToken = null)
        {
            string lsActivityID = null;
            string lsAccessToken = null;
            JsonSerializerSettings loSerializerSettings = null;
            string lsActivityJson = null;
            string lsBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            Dictionary<string, object> loContent = null;
            byte[] loPostContent = null;
            HttpWebRequest loWebRequest = null;
            XDocument loGPX = null;

            if (activity != null)
            {
                lsAccessToken = accessToken;
                if (string.IsNullOrEmpty(lsAccessToken))
                    lsAccessToken = await GetAccessTokenV2();
                if (!string.IsNullOrEmpty(lsAccessToken))
                {
                    try
                    {
                        // Prepare multipart content
                        loContent = new Dictionary<string, object>();
                        // Serialize Json request (use custom serializer)
                        loSerializerSettings = new JsonSerializerSettings();
                        loSerializerSettings.Converters.Add(new NikePlusActivityPostConverterV2());
                        lsActivityJson = JsonConvert.SerializeObject(activity, loSerializerSettings);
                        // (Just for debugging purposes: show Json request part)
                        if (System.Diagnostics.Debugger.IsAttached)
                            System.Diagnostics.Debug.WriteLine(lsActivityJson);
                        loContent.Add("run", new FileParameter(Encoding.UTF8.GetBytes(lsActivityJson), null, "application/json"));
                        // Serialize GPX request
                        loGPX = activity.GPX;
                        if (loGPX == null)
                        {
                            try { loGPX = activity.ToGPX(); }
                            catch
                            {
                                // Do nothing
                                if (System.Diagnostics.Debugger.IsAttached)
                                    System.Diagnostics.Debug.WriteLine("Nike+ Activity cannot be converted to GPX.");
                            }
                        }
                        if (loGPX != null)
                        {
                            // Convert XML formatted document to string
                            string lsGPX = null;
                            StringBuilder loStringBuilder = new StringBuilder();
                            using (TextWriter loTextWriter = new EncodingStringWriter(loStringBuilder, new UTF8Encoding(false)))
                            {
                                loGPX.Save(loTextWriter);
                                lsGPX = loStringBuilder.ToString();
                                // (Just for debugging purposes: show GPX request part)
                                if (System.Diagnostics.Debugger.IsAttached)
                                    System.Diagnostics.Debug.WriteLine(lsGPX);
                                loContent.Add("gpxXML", new FileParameter(Encoding.UTF8.GetBytes(lsGPX), null, "text/plain"));
                            }
                        }
                        loPostContent = BuildMultipartFormData(loContent, lsBoundary, Encoding.UTF8);
                        UriBuilder loUriBuilder = new UriBuilder(BASE_URI_V2);
                        loUriBuilder.Path = SYNC_PATH_V2;
                        loUriBuilder.Query = string.Format("access_token={0}", lsAccessToken);
                        // Cookies
                        CookieContainer loCookieContainer = new CookieContainer();
                        Uri loLoginUri = new Uri(LOGIN_URI_V2);
                        loCookieContainer.Add(loLoginUri, new Cookie(APP_COOKIE_V2, msApp, "/", LOGIN_DOMAIN_V2));
                        loCookieContainer.Add(loLoginUri, new Cookie(CLIENT_ID_COOKIE_V2, msClientId, "/", LOGIN_DOMAIN_V2));
                        loCookieContainer.Add(loLoginUri, new Cookie(CLIENT_SECRET_COOKIE_V2, msClientSecret, "/", LOGIN_DOMAIN_V2));
                        // Prepare request
                        loWebRequest = WebRequest.Create(loUriBuilder.Uri) as HttpWebRequest;
                        loWebRequest.CookieContainer = loCookieContainer;
#if DESKTOP_APP
						loWebRequest.UserAgent = USER_AGENT_V2;
#elif WINDOWS_UWP
                        loWebRequest.Headers["user-agent"] = USER_AGENT_V2;
#endif
                        loWebRequest.Headers["appId"] = msApp;
                        //loWebRequest.Accept = "application/json";
                        loWebRequest.Method = "POST";
                        loWebRequest.ContentType = String.Format("multipart/form-data; boundary={0}", lsBoundary);
#if DESKTOP_APP
						loWebRequest.ContentLength = loPostContent.Length;
#endif
                        // Post request
                        using (Stream loRequestStream = await loWebRequest.GetRequestStreamAsync())
                        {
                            loRequestStream.Write(loPostContent, 0, loPostContent.Length);
                            loRequestStream.Flush();
#if DESKTOP_APP
							loRequestStream.Close();
#endif
                        }
                        // Read response (XML formatted)
                        using (WebResponse loWebResponse = await loWebRequest.GetResponseAsync())
                        {
                            using (Stream loResponseStream = loWebResponse.GetResponseStream())
                            {
                                using (StreamReader loStreamReader = new StreamReader(loResponseStream))
                                {
                                    string lsResponse = loStreamReader.ReadToEnd();
                                    // (Just for debugging purposes: show response).
                                    if (System.Diagnostics.Debugger.IsAttached)
                                        System.Diagnostics.Debug.WriteLine(lsResponse);
                                    // Parse XML response
                                    XDocument loDocument = XDocument.Parse(lsResponse);
                                    lsActivityID = loDocument.Descendants("activityId").FirstOrDefault().Value;
                                    try { await EndSyncV2(lsAccessToken); }
                                    catch { /* Do nothing */ }
                                }
                            }
                        }
                    }
                    catch (WebException loWebException)
                    {
                        lsActivityID = null;
                        throw new NikePlusException(loWebException.Message, loWebException, LOGIN_URI_DEV, null);
                    }
                    catch (Exception loException)
                    {
                        // (Just for debugging purposes: show exception).
                        if (System.Diagnostics.Debugger.IsAttached)
                            System.Diagnostics.Debug.WriteLine(loException.StackTrace);
                        lsActivityID = null;
                        throw;
                    }
                }
            }
            return lsActivityID;
        }

        /// <summary>
        /// Ends Sync Nike+ Activity using Nike+ API v2.0.
        /// </summary>
        /// <param name="accessToken">Access token to connect Nike+.</param>
        public async Task EndSyncV2(string accessToken)
        {
            HttpWebRequest loWebRequest = null;
            // Check provided access token
            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentNullException("accessToken");

            try
            {
                UriBuilder loUriBuilder = new UriBuilder(BASE_URI_V2);
                loUriBuilder.Path = SYNC_COMPLETE_PATH_V2;
                loUriBuilder.Query = string.Format("access_token={0}", accessToken);
                //// Cookies
                //CookieContainer loCookieContainer = new CookieContainer();
                //Uri loLoginUri = new Uri(LOGIN_URI_V2);
                //loCookieContainer.Add(loLoginUri, new Cookie(APP_COOKIE_V2, msApp, "/", LOGIN_DOMAIN_V2));
                //loCookieContainer.Add(loLoginUri, new Cookie(CLIENT_ID_COOKIE_V2, msClientId, "/", LOGIN_DOMAIN_V2));
                //loCookieContainer.Add(loLoginUri, new Cookie(CLIENT_SECRET_COOKIE_V2, msClientSecret, "/", LOGIN_DOMAIN_V2));
                // Prepare request
                loWebRequest = WebRequest.Create(loUriBuilder.Uri) as HttpWebRequest;
                //loWebRequest.CookieContainer = loCookieContainer;
#if DESKTOP_APP
						loWebRequest.UserAgent = USER_AGENT_V2;
#elif WINDOWS_UWP
                loWebRequest.Headers["user-agent"] = USER_AGENT_V2;
#endif
                loWebRequest.Headers["appId"] = msApp;
                //loWebRequest.Accept = "application/json";
                loWebRequest.Method = "POST";
//#if DESKTOP_APP
//						loWebRequest.ContentLength = loPostContent.Length;
//#endif
//                // Post request
//                using (Stream loRequestStream = await loWebRequest.GetRequestStreamAsync())
//                {
//                    loRequestStream.Write(loPostContent, 0, loPostContent.Length);
//                    loRequestStream.Flush();
//#if DESKTOP_APP
//							loRequestStream.Close();
//#endif
//                }
                // Read response (XML formatted)
                using (WebResponse loWebResponse = await loWebRequest.GetResponseAsync())
                {
                    using (Stream loResponseStream = loWebResponse.GetResponseStream())
                    {
                        using (StreamReader loStreamReader = new StreamReader(loResponseStream))
                        {
                            string lsResponse = loStreamReader.ReadToEnd();
                            // (Just for debugging purposes: show response).
                            if (System.Diagnostics.Debugger.IsAttached)
                                System.Diagnostics.Debug.WriteLine(lsResponse);
                            //// Parse XML response
                            //XDocument loDocument = XDocument.Parse(lsResponse);
                            //lsActivityID = loDocument.Descendants("activityId").FirstOrDefault().Value;
                        }
                    }
                }
            }
            catch (WebException loWebException)
            {
                //lsActivityID = null;
                throw new NikePlusException(loWebException.Message, loWebException, LOGIN_URI_DEV, null);
            }
            catch (Exception loException)
            {
                // (Just for debugging purposes: show exception).
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debug.WriteLine(loException.StackTrace);
                //lsActivityID = null;
                throw;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Validates current authentication <see cref="Token"/>.
        /// </summary>
        /// <returns><see langword="true"/> if valid, otherwise, <see langword="false"/>.</returns>
        private bool ValidateToken()
        {
            bool lbValidToken = false;
            // Check if already signed-in and Token instance
            if (IsSignedIn &&
                Token != null)
            {
                lbValidToken = !string.IsNullOrEmpty(Token.AccessToken) &&
                    Token.ExpirationTime.CompareTo(DateTime.Now) > 0;
            }
            return lbValidToken;
        }

        /// <summary>
        /// Perform general Nike+ API requests using <see cref="BASE_URI"/>.
        /// </summary>
        /// <param name="path">Path to resource to request.</param>
        /// <param name="query">Query to resource to request.</param>
        /// <param name="method">Method to request.</param>
        /// <param name="postData">Data to post. (only when <paramref name="method"/> = POST).</param>
        /// <param name="contentType">Content type to post. (only when <paramref name="method"/> = POST)</param>
        /// <returns><see cref="string"/> response to request (generally is a Json string).</returns>
        private async Task<string> PerformRequest(string path, string query = null, string method = null, byte[] postData = null, string contentType = null)
        {
            string lsResponse = null;
            UriBuilder loUriBuilder = null;
            // Validate Token and Refresh if necessary
            if (await SignIn() &&
                Token != null &&
                !string.IsNullOrEmpty(Token.AccessToken))
            {
                // Check if query contains access_token
                if (string.IsNullOrEmpty(query))
                    query = string.Format("access_token={0}", Token.AccessToken);
                else
                {
                    if (!query.Contains("access_token="))
                        query += string.Format("&access_token={0}", Token.AccessToken);
                }
            }
            else
            {
                throw new NikePlusException("Invalid acces token", null, path, query);
            }
            // Prepare URL request
            loUriBuilder = new UriBuilder(BASE_URI);
            loUriBuilder.Path += path;
            loUriBuilder.Query = query;
            WebRequest loWebRequest = WebRequest.Create(loUriBuilder.Uri);
            if (!string.IsNullOrEmpty(method))
            {
                loWebRequest.Method = method;
                // Append data to Post
                if (method.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
                    postData != null)
                {
                    loWebRequest.ContentType = contentType ?? "application/json";
                    using (Stream loRequestStream = await loWebRequest.GetRequestStreamAsync())
                    {
                        loRequestStream.Write(postData, 0, postData.Length);
                        loRequestStream.Flush();
#if DESKTOP_APP
						loRequestStream.Close();
#endif
                    }
                }
            }
            try
            {
                // Perform request and handle response
                using (WebResponse loWebResponse = await loWebRequest.GetResponseAsync())
                {
                    // (Just for debugging purposes: show response status code).
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        HttpWebResponse loHttpWebResponse = loWebResponse as HttpWebResponse;
                        if (loHttpWebResponse != null)
                            System.Diagnostics.Debug.WriteLine("Response StatusCode: {0}", loHttpWebResponse.StatusCode);
                    } // (Just for debugging purposes: show response status code).
                    using (Stream loResponseStream = loWebResponse.GetResponseStream())
                    {
                        using (StreamReader loStreamReader = new StreamReader(loResponseStream))
                        {
                            // Get response as string
                            lsResponse = await loStreamReader.ReadToEndAsync();
                            // (Just for debugging purposes: show response).
                            if (System.Diagnostics.Debugger.IsAttached)
                                System.Diagnostics.Debug.WriteLine(lsResponse);
                        }
                    }
                }
            }
            catch (WebException loWebException)
            {
                throw new NikePlusException(loWebException.Message, loWebException, path, query);
            }
            catch (Exception loException)
            {
                // (Just for debugging purposes: show exception).
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debug.WriteLine(loException.StackTrace);
                throw;
            }
            return lsResponse;
        }

        /// <summary>
        /// Builds multi-part message for HTTP Posts.
        /// </summary>
        /// <param name="postParameters">Collection of parameters to post.</param>
        /// <param name="boundary">String to identify boundary of each parameter.</param>
        /// <param name="encoding">Encoding to use.</param>
        /// <returns>Byte array with multi-part message content.</returns>
        private static byte[] BuildMultipartFormData(Dictionary<string, object> postParameters, string boundary, Encoding encoding)
        {
            byte[] loFormData = null;
            Stream loStreamFormData = new MemoryStream();
            bool lbNeedsCLRF = false;
            foreach (var loParam in postParameters)
            {
                // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                // Skip it on the first parameter, add it to subsequent parameters.
                if (lbNeedsCLRF)
                    loStreamFormData.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));
                lbNeedsCLRF = true;
                FileParameter loFileToUpload = loParam.Value as FileParameter;
                if (loFileToUpload != null)
                {
                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string lsHeader = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                                                    boundary,
                                                    loParam.Key,
                                                    loFileToUpload.FileName ?? loParam.Key,
                                                    loFileToUpload.ContentType ?? "application/octet-stream");
                    loStreamFormData.Write(encoding.GetBytes(lsHeader), 0, encoding.GetByteCount(lsHeader));
                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    loStreamFormData.Write(loFileToUpload.File, 0, loFileToUpload.File.Length);
                }
                else
                {
                    string lsPostData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                                                      boundary,
                                                      loParam.Key,
                                                      loParam.Value);
                    loStreamFormData.Write(encoding.GetBytes(lsPostData), 0, encoding.GetByteCount(lsPostData));
                }
            }
            // Add the end of the request.  Start with a newline
            string lsFooter = "\r\n--" + boundary + "--\r\n";
            loStreamFormData.Write(encoding.GetBytes(lsFooter), 0, encoding.GetByteCount(lsFooter));
            // Dump the Stream into a byte[]
            loStreamFormData.Position = 0;
            loFormData = new byte[loStreamFormData.Length];
            loStreamFormData.Read(loFormData, 0, loFormData.Length);
#if DESKTOP_APP
			loStreamFormData.Close();
#endif
            return loFormData;
        }

        #endregion

    }

    #endregion

    #region NikePlusToken class

    /// <summary>
    /// Authentication Token to Nike+ API.
    /// </summary>
    [JsonObject]
    public sealed class NikePlusToken
    {

        #region Constants

        /// <summary>
        /// Default token type.
        /// </summary>
        public const string TOKEN_TYPE = "bearer";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or set authorization type (always "bearer").
        /// </summary>
        [JsonProperty(PropertyName = "token_type",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string TokenType { get; set; }

        /// <summary>
        /// Gets or sets the amount of time in seconds when the access token is valid.
        /// </summary>
        [JsonProperty(PropertyName = "expires_in",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public long ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets access token for use in Nike+ API calls.
        /// </summary>
        [JsonProperty(PropertyName = "access_token",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the URL to the user’s profile image (if it exists).
        /// </summary>
        [JsonProperty(PropertyName = "profile_img_url",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string ProfileImgUrl { get; set; }

        /// <summary>
        /// Gets or sets the time when current access token was created.
        /// </summary>
        [JsonIgnore]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets the expected time when current access token expires.
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
        /// Initializes a new instance of <see cref="NikePlusToken"/> class.
        /// </summary>
        public NikePlusToken()
        {
            TokenType = TOKEN_TYPE;
            CreationTime = DateTime.Now;
        }

        #endregion

    }

    #endregion

    #region NikePlusException class

    /// <summary>
    /// Exception for Nike+ requests.
    /// </summary>
    public sealed class NikePlusException : Exception
    {

        #region Properties

        /// <summary>
        /// Gets the error response for request.
        /// </summary>
        public NikePlusError Error { get; private set; }

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
        /// Initializes a new instance of the <see cref="NikePlusException"/> class.
        /// </summary>
        /// <param name="message">The error message, that explains the reason for exception.</param>
        /// <param name="innerException">The exception that is cause of the current exception.</param>
        /// <param name="path">The Nike+ path that generates current exception.</param>
        /// <param name="query">The Nike+ query parameters that generates current exception</param>
        internal NikePlusException(string message, WebException innerException, string path, string query) : base(message, innerException)
        {
            Path = path;
            Query = query;
            // Check for WebException
            WebException loWebException = innerException as WebException;
            if (loWebException != null)
            {
                // Check for Response
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
                            // Get response as string
                            string lsResponse = loStreamReader.ReadToEnd();
                            if (System.Diagnostics.Debugger.IsAttached)
                                System.Diagnostics.Debug.WriteLine(lsResponse);
                            // Deserialize
                            JsonSerializerSettings loSerializerSettings = new JsonSerializerSettings();
                            loSerializerSettings.Error = (sender, args) =>
                            {
                                if (System.Diagnostics.Debugger.IsAttached)
                                    System.Diagnostics.Debug.WriteLine(args.ErrorContext.Error.Message);
                                args.ErrorContext.Handled = true;
                            };
                            Error = JsonConvert.DeserializeObject<NikePlusError>(lsResponse, loSerializerSettings);
                        }
                    }
                }
            }
        }

        #endregion

    }

    #endregion

    #region NikePlusError class

    /// <summary>
    /// Nike+ Error Response.
    /// </summary>
    [JsonObject]
    public sealed class NikePlusError
    {

        #region Properties

        /// <summary>
        /// Gets or sets the ID for error.
        /// </summary>
        [JsonProperty(PropertyName = "error_id",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string ErrorId { get; set; }

        /// <summary>
        /// Gets or sets ID for request that generates the error.
        /// </summary>
        [JsonProperty(PropertyName = "requestId",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets the code for error.
        /// </summary>
        [JsonProperty(PropertyName = "error",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the description for error.
        /// </summary>
        [JsonProperty(PropertyName = "error_description",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets code for error.
        /// </summary>
        [JsonProperty(PropertyName = "code",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets description for error.
        /// </summary>
        [JsonProperty(PropertyName = "message",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets URL that describes deeply the error.
        /// </summary>
        [JsonProperty(PropertyName = "error_uri",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets source of error.
        /// </summary>
        [JsonProperty(PropertyName = "source",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the collection of error details.
        /// </summary>
        [JsonProperty(PropertyName = "errors",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default,
                      ItemIsReference = true)]
        public IList<NikePlusErrorItem> Errors { get; set; }

        /// <summary>
        /// Gets or sets the fault for error.
        /// </summary>
        [JsonProperty(PropertyName = "fault",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public Fault Fault { get; set; }

        #endregion

    }

    #endregion

    #region NikePlusErrorItem class

    /// <summary>
    /// Nike+ Error details.
    /// </summary>
    [JsonObject]
    public sealed class NikePlusErrorItem
    {

        #region Properties

        /// <summary>
        /// Gets or sets code for error.
        /// </summary>
        [JsonProperty(PropertyName = "code",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Code { get; set; }


        /// <summary>
        /// Gets or sets description for error.
        /// </summary>
        [JsonProperty(PropertyName = "message",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Message { get; set; }

        #endregion

    }

    #endregion

    #region Fault class

    /// <summary>
    /// Nike+ Fault.
    /// </summary>
    [JsonObject]
    public sealed class Fault
    {

        #region Properties

        /// <summary>
        /// Gets or sets description for fault.
        /// </summary>
        [JsonProperty(PropertyName = "faultstring",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets detail for fault.
        /// </summary>
        [JsonProperty(PropertyName = "detail",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public FaultDetail Detail { get; set; }

        #endregion

    }

    [JsonObject]
    public sealed class FaultDetail
    {

        /// <summary>
        /// Gets or sets error code for fault.
        /// </summary>
        [JsonProperty(PropertyName = "errorcode",
                      NullValueHandling = NullValueHandling.Ignore,
                      Required = Required.Default)]
        public string ErrorCode { get; set; }

    }

    #endregion

    #region FileParameter class

    /// <summary>
    /// Represents a file for Multi-part HTTP Posts.
    /// </summary>
    public class FileParameter
    {

        #region Properties

        /// <summary>
        /// Gets or sets the file contents (byte array).
        /// </summary>
        public byte[] File { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        public string ContentType { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="FileParameter"/> class.
        /// </summary>
        /// <param name="file">File contents (byte array).</param>
        public FileParameter(byte[] file) : this(file, null) { }

        /// <summary>
        /// Initializes a new instance of <see cref="FileParameter"/> class
        /// </summary>
        /// <param name="file">File contents (byte array).</param>
        /// <param name="filename">File name.</param>
        public FileParameter(byte[] file, string filename) : this(file, filename, null) { }

        /// <summary>
        /// Initializes a new instance of <see cref="FileParameter"/> class.
        /// </summary>
        /// <param name="file">File contents (byte array).</param>
        /// <param name="filename">File name.</param>
        /// <param name="contenttype">Content type.</param>
        public FileParameter(byte[] file, string filename, string contenttype)
        {
            File = file;
            FileName = filename;
            ContentType = contenttype;
        }

        #endregion

    }

    #endregion

    #region EncodingStringWriter class

    /// <summary>
    /// Implements a <see cref="TextWriter"/> for writing information to a string, with specific <see cref="T:System.Text.Encoding"/>.
    /// </summary>
    /// <seealso cref="System.IO.StringWriter" />
    public class EncodingStringWriter : StringWriter
    {

        #region Inner members

        /// <summary>
        /// <see cref="T:System.Text.Encoding"/> to use for writing string.
        /// </summary>
        private readonly Encoding _encoding;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="T:System.Text.Encoding" /> in which the output is written.
        /// </summary>
        public override Encoding Encoding
        {
            get { return _encoding; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodingStringWriter"/> class.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> object to write to.</param>
        /// <param name="encoding">The <see cref="System.Text.Encoding"/> to use for writing.</param>
        public EncodingStringWriter(StringBuilder sb, Encoding encoding) : base(sb)
        {
            _encoding = encoding;
        }

        #endregion

    }

    #endregion

}
