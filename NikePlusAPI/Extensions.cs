//-----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
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
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Extensions for NikePlusAPI objects.
    /// </summary>
    public static class Extensions
    {

        #region NikePlusActivity Extension Methods

        /// <summary>
        /// Exports <see cref="NikePlusAPI.NikePlusActivity"/> instance to GPX <see cref="System.Xml.Linq.XDocument"/>.
        /// </summary>
        /// <param name="activity"><see cref="NikePlusAPI.NikePlusActivity"/> instance to export.</param>
        /// <param name="creator">Creator name for activity.</param>
        /// <param name="name">Name for activity.</param>
        /// <returns><see cref="System.Xml.Linq.XDocument"/> with GPX information.</returns>
        public static XDocument ToGPX(this NikePlusActivity activity, string creator = null, string name = null)
        {
            XDocument loDocument = null;
            XElement loRoot = null;
            XNamespace loNamespaceDefault = "http://www.topografix.com/GPX/1/1";
            XNamespace loNamespaceXsi = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace loNamespaceGpxTpx = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1";
            XNamespace loNamespaceGpxx = "http://www.garmin.com/xmlschemas/GpxExtensions/v3";
            XNamespace loNamespaceSchema = "http://www.topografix.com/GPX/1/1 " +
                "http://www.topografix.com/GPX/1/1/gpx.xsd " +
                "http://www.garmin.com/xmlschemas/GpxExtensions/v3 " +
                "http://www.garmin.com/xmlschemas/GpxExtensionsv3.xsd " +
                "http://www.garmin.com/xmlschemas/TrackPointExtension/v1 " +
                "http://www.garmin.com/xmlschemas/TrackPointExtensionv1.xsd";
            XElement loMetadata = null;
            XElement loTrack = null;
            XElement loSegment = null;
            XElement loPoint = null;
            string lsCreator = creator;
            string lsName = name;
            DateTime ldtTime = DateTime.Now;
            double ldSeconds = 0;
            // Validate NikePlusActivity instance
            if (activity != null)
            {
                // Check activity GPS/WayPoints
                if (activity.GPSData != null &&
                    activity.GPSData.Waypoints != null &&
                    activity.GPSData.Waypoints.Any())
                {
                    // Get valid Waypoints (with Location Latitude and Longitude)
                    IEnumerable<NikePlusWaypoint> loWPs = activity.GPSData.Waypoints.Where(loWP => loWP.Latitude != null &&
                                                                                           loWP.Longitude != null);
                    // No valid Waypoints... throw exception
                    if (loWPs == null ||
                        !loWPs.Any())
                        throw new ArgumentNullException("Waypoints");
                }
                else
                {
                    // No Waypoints... throw exception
                    throw new ArgumentNullException("Waypoints");
                }
                // Validate Activity Properties (Start Time, Creator, Name)
                if (activity.StartTime != null &&
                    activity.StartTime.HasValue)
                    ldtTime = activity.StartTime.Value;
                if (string.IsNullOrEmpty(lsCreator))
                    lsCreator = "NikePlusAPI";
                if (string.IsNullOrEmpty(lsName))
                    lsName = string.Format("{0} {1:yyyy-MM-dd HH:mm:ss}", activity.Type, ldtTime);
                // Initialize GPX (XML Document), root with namespaces
                loRoot = new XElement(loNamespaceDefault + "gpx",
                                      new XAttribute("xmlns", loNamespaceDefault.NamespaceName),
                                      new XAttribute(XNamespace.Xmlns + "xsi", loNamespaceXsi.NamespaceName),
                                      new XAttribute(XNamespace.Xmlns + "gpxtpx", loNamespaceGpxTpx.NamespaceName),
                                      new XAttribute(XNamespace.Xmlns + "gpxx", loNamespaceGpxx.NamespaceName),
                                      new XAttribute(loNamespaceXsi + "schemaLocation", loNamespaceSchema.NamespaceName),
                                      new XAttribute("creator", lsCreator),
                                      new XAttribute("version", "1.1"));
                // Set GPX Metadata information
                loMetadata = new XElement(loNamespaceDefault + "metadata",
                                          new XElement(loNamespaceDefault + "name", lsName),
                                          new XElement(loNamespaceDefault + "time", string.Format("{0:O}", ldtTime.ToUniversalTime())));
                loRoot.Add(loMetadata);
                // Validate Activity MapPoints
                if (activity.GPSData != null &&
                    activity.GPSData.Waypoints != null &&
                    activity.GPSData.Waypoints.Any())
                {
                    // Initialize GPX Track/Segment node
                    loTrack = new XElement(loNamespaceDefault + "trk");
                    loSegment = new XElement(loNamespaceDefault + "trkseg");
                    // Process Activity MapPoints
                    foreach (NikePlusWaypoint loWaypoint in activity.GPSData.Waypoints)
                    {
                        // Append GPX TrackPoint
                        loPoint = new XElement(loNamespaceDefault + "trkpt");

                        // TrackPoint Longitude
                        if (loWaypoint.Longitude != null)
                            loPoint.Add(new XAttribute("lon", loWaypoint.Longitude));
                        // TrackPoint Latitude
                        if (loWaypoint.Latitude != null)
                            loPoint.Add(new XAttribute("lat", loWaypoint.Latitude));
                        // TrackPoint Elevation
                        if (loWaypoint.Elevation != null)
                            loPoint.Add(new XElement(loNamespaceDefault + "ele", loWaypoint.Elevation));

                        // TrackPoint Time
                        ldSeconds += activity.GPSData.IntervalMetric ?? 10d;
                        loPoint.Add(new XElement(loNamespaceDefault + "time", ldtTime.AddSeconds(ldSeconds).ToUniversalTime()));

                        loSegment.Add(loPoint);
                    }
                    loTrack.Add(loSegment);
                    loRoot.Add(loTrack);
                }
                loDocument = new XDocument(new XDeclaration("1.0", "utf-8", null), loRoot);
            }
            // Return GPX (XML Document)
            return loDocument;
        }

        #endregion

        #region DateTime Extension Methods

        /// <summary>
        /// Converts specific Date to Unix Timestamp (from epoch).
        /// </summary>
        /// <param name="datetime">Date to convert.</param>
        /// <returns>Unix Timestamp.</returns>
        public static double ToUnixTimestamp(this DateTime datetime)
        {
            DateTime ldtEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan ltsDiff = datetime.ToUniversalTime().Subtract(ldtEpoch);
            return ltsDiff.TotalMilliseconds;
        }

        /// <summary>
        /// Gets Time Zone (Google API based) for Date in specific Location (latitude, longitude).
        /// </summary>
        /// <param name="date">Date.</param>
        /// <param name="latitude">Location latitude.</param>
        /// <param name="longitude">Location longitude.</param>
        /// <returns>TimeZone details accordingly to Google API.</returns>
        public static async Task<GoogleTimeZone> ToGoogleTimeZoneAsync(this DateTime date, double latitude, double longitude)
        {
            GoogleTimeZone loGoogleTimeZone = null;
            StringBuilder loQuery = new StringBuilder();
            //loQuery.AppendFormat("location={0}", Uri.EscapeDataString(string.Format("{0},{1}", latitude, longitude)));
            loQuery.AppendFormat("location={0}", string.Format("{0},{1}", latitude, longitude));
            loQuery.AppendFormat("&timestamp={0}", Math.Floor(date.ToUnixTimestamp()));
            loQuery.AppendFormat("&sensor={0}", "false");
            UriBuilder loUriBuilder = new UriBuilder("https://maps.googleapis.com");
            loUriBuilder.Path += "/map/api/timezone/json";
            loUriBuilder.Query = loQuery.ToString();
            WebRequest loWebRequest = WebRequest.Create(loUriBuilder.Uri);
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
                            string lsResponse = await loStreamReader.ReadToEndAsync();
                            // Deserialize response string
                            loGoogleTimeZone = JsonConvert.DeserializeObject<GoogleTimeZone>(lsResponse);
                        }
                    }
                }
            }
            catch (Exception loException)
            {
                // (Just for debugging purposes: show exception).
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debug.WriteLine(loException.StackTrace);
                loGoogleTimeZone = null;
            }

            return loGoogleTimeZone;
        }

#if DESKTOP_APP
        /// <summary>
        /// Gets Time Zone (Google API based) for Date in specific Location (latitude, longitude).
        /// </summary>
        /// <param name="date">Date.</param>
        /// <param name="latitude">Location latitude.</param>
        /// <param name="longitude">Location longitude.</param>
        /// <returns>TimeZone details accordingly to Google API.</returns>
        public static GoogleTimeZone ToGoogleTimeZone(this DateTime date, double latitude, double longitude)
        {
            GoogleTimeZone loGoogleTimeZone = null;
            StringBuilder loQuery = new StringBuilder();
            //loQuery.AppendFormat("location={0}", Uri.EscapeDataString(string.Format("{0},{1}", latitude, longitude)));
            loQuery.AppendFormat("location={0}", string.Format("{0},{1}", latitude, longitude));
            loQuery.AppendFormat("&timestamp={0}", Math.Floor(date.ToUnixTimestamp()));
            loQuery.AppendFormat("&sensor={0}", "false");
            UriBuilder loUriBuilder = new UriBuilder("https://maps.googleapis.com");
            loUriBuilder.Path += "/map/api/timezone/json";
            loUriBuilder.Query = loQuery.ToString();
            WebRequest loWebRequest = WebRequest.Create(loUriBuilder.Uri);
            try
            {
                // Perform request and handle response
                using (WebResponse loWebResponse = loWebRequest.GetResponse())
                {
                    using (Stream loResponseStream = loWebResponse.GetResponseStream())
                    {
                        using (StreamReader loStreamReader = new StreamReader(loResponseStream))
                        {
                            // Get response as string
                            string lsResponse = loStreamReader.ReadToEnd();
                            // Deserialize response string
                            loGoogleTimeZone = JsonConvert.DeserializeObject<GoogleTimeZone>(lsResponse);
                        }
                    }
                }
            }
            catch (Exception loException)
            {
                // (Just for debugging purposes: show exception).
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debug.WriteLine(loException.StackTrace);
                loGoogleTimeZone = null;
            }

            return loGoogleTimeZone;
        }
#endif

        #endregion

    }

    #region GoogleTimeZone class

    /// <summary>
    /// Represents Google Time Zone.
    /// </summary>
    public sealed class GoogleTimeZone
    {

        #region Properties

        /// <summary>
        /// Gets or sets the DST offset.
        /// </summary>
        public double dstOffset { get; set; }
        /// <summary>
        /// Gets or sets the raw offset.
        /// </summary>
        public double rawOffset { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// Gets or sets the time zone identifier.
        /// </summary>
        public string timeZoneId { get; set; }
        /// <summary>
        /// Gets or sets the name of the time zone.
        /// </summary>
        /// <value>
        /// The name of the time zone.
        /// </value>
        public string timeZoneName { get; set; }

        #endregion

    }

    #endregion

}
