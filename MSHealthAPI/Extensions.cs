//-----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace MSHealthAPI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Extensions for MSHealthAPI objects.
    /// </summary>
    public static class Extensions
    {

        #region MSHealthActivity Extension Methods

        /// <summary>
        /// Exports <see cref="MSHealthAPI.MSHealthActivity"/> instance to GPX <see cref="System.Xml.Linq.XDocument"/>.
        /// </summary>
        /// <param name="activity"><see cref="MSHealthAPI.MSHealthActivity"/> instance to export.</param>
        /// <param name="creator">Creator name for activity.</param>
        /// <param name="name">Name for activity.</param>
        /// <param name="ignoreEmptyMapPoints">Flag to not throw exception if <paramref name="activity"/> has no MapPoints.</param>
        /// <returns><see cref="System.Xml.Linq.XDocument"/> with GPX information.</returns>
        public static XDocument ToGPX(this MSHealthActivity activity, string creator = null, string name = null, bool ignoreEmptyMapPoints = false)
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
            // Validate MSHealthActivity instance
            if (activity != null)
            {
                // Check for MapPoints flag
                if (!ignoreEmptyMapPoints)
                {
                    // Check activity MapPoints
                    if (activity.MapPoints != null &&
                        activity.MapPoints.Any())
                    {
                        // Get valid MapPoints (with Location Latitude and Longitude)
                        IEnumerable<MSHealthMapPoint> loMPs = activity.MapPoints.Where(loMP => loMP.Location != null &&
                                                                                               loMP.Location.Latitude != null &&
                                                                                               loMP.Location.Longitude != null);
                        // No valid MapPoints... throw exception
                        if (loMPs == null ||
                            !loMPs.Any())
                            throw new ArgumentNullException("MapPoints");
                    }
                    else
                    {
                        // No MapPoints... throw exception
                        throw new ArgumentNullException("MapPoints");
                    }
                }
                // Validate Activity Properties (Start Time, Creator, Name)
                if (activity.StartTime != null &&
                    activity.StartTime.HasValue)
                    ldtTime = activity.StartTime.Value;
                if (string.IsNullOrEmpty(lsCreator))
                    lsCreator = "MSHealthAPI";
                if (string.IsNullOrEmpty(lsName))
                    lsName = string.Format("{0} {1:yyyy-MM-dd HH:mm:ss}", activity.Type, ldtTime);
                // Initialize GPX (XML Document), root with namespaces
                loRoot = new XElement(loNamespaceDefault + "gpx",
                                      new XAttribute("xmlns", loNamespaceDefault.NamespaceName),
                                      //new XAttribute("xmlns", loNamespaceBase.NamespaceName),
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
                if (activity.MapPoints != null &&
                    activity.MapPoints.Any())
                {
                    // Initialize GPX Track/Segment node
                    loTrack = new XElement(loNamespaceDefault + "trk");
                    loSegment = new XElement(loNamespaceDefault + "trkseg");
                    List<MSHealthMapPoint> loMapPoints = activity.MapPoints.OrderBy(loMP => loMP.Ordinal).ToList();
                    // Process Activity MapPoints
                    foreach (MSHealthMapPoint loMapPoint in loMapPoints)
                    {
                        // Append GPX TrackPoint
                        loPoint = new XElement(loNamespaceDefault + "trkpt");
                        if (loMapPoint.Location != null)
                        {
                            // TrackPoint Longitude
                            if (loMapPoint.Location.Latitude != null)
                                loPoint.Add(new XAttribute("lon", ((double)loMapPoint.Location.Longitude / MSHealthGPSPoint.LONGITUDE_FACTOR)));
                            // TrackPoint Latitude
                            if (loMapPoint.Location.Latitude != null)
                                loPoint.Add(new XAttribute("lat", ((double)loMapPoint.Location.Latitude / MSHealthGPSPoint.LATITUDE_FACTOR)));
                            // TrackPoint Elevation
                            if (loMapPoint.Location.ElevationFromMeanSeaLevel != null)
                                loPoint.Add(new XElement(loNamespaceDefault + "ele", (double)loMapPoint.Location.ElevationFromMeanSeaLevel / MSHealthGPSPoint.ELEVATION_FACTOR));
                        }
                        // TrackPoint Time
                        if (loMapPoint.SecondsSinceStart != null)
                        {
                            //ldtTime = ldtTime.AddSeconds(loMapPoint.SecondsSinceStart.Value);
                            loPoint.Add(new XElement(loNamespaceDefault + "time", ldtTime.AddSeconds(loMapPoint.SecondsSinceStart.Value).ToUniversalTime()));
                        }
                        else
                        {
                            loPoint.Add(new XElement(loNamespaceDefault + "time", ldtTime.ToUniversalTime()));
                        }
                        // TrackPoint Extension HeartRate
                        if (loMapPoint.HeartRate != null)
                        {
                            loPoint.Add(new XElement(loNamespaceDefault + "extensions",
                                                     new XElement(loNamespaceGpxTpx + "TrackPointExtension",
                                                                  new XElement(loNamespaceGpxTpx + "hr", loMapPoint.HeartRate.Value))));
                        }
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

    }

}
