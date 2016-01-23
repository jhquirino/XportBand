//-----------------------------------------------------------------------
// <copyright file="ActivityDetailsView.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace XportBand.View
{
    using GalaSoft.MvvmLight.Messaging;
    using System;
    using System.Linq;
    using Windows.Devices.Geolocation;
    using Windows.Foundation;
    using Windows.Storage.Streams;
    using Windows.UI;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Maps;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ActivityDetailsView : Page
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityDetailsView"/> class.
        /// </summary>
        public ActivityDetailsView()
        {
            this.InitializeComponent();
            // Register MVVM Message (handles Map path)
            Messenger.Default.Register<Geopath>(this, (path) => HandleMapPath(path));
        }

        #endregion

        #region MVVM Messaging Handlers

        /// <summary>
        /// Handles MVVM Message for Map path.
        /// </summary>
        /// <param name="path">The path.</param>
        private void HandleMapPath(Geopath path)
        {
            // Remove previous paths from MapControl
            mapActivity.MapElements.Clear();
            // Validate input path
            if (path != null &&
                path.Positions.Any())
            {
                // Configure path to draw with polyline and assign path to MapControl
                MapPolyline loMapPolyline = new MapPolyline();
                loMapPolyline.Path = path;
                loMapPolyline.StrokeColor = (Color)Resources["SystemAccentColor"];
                loMapPolyline.StrokeThickness = 3;
                mapActivity.MapElements.Add(loMapPolyline);
                // Configure start position icon and assign path to MapControl
                BasicGeoposition loStartPosition = path.Positions[0];
                MapIcon loStartIcon = new MapIcon();
                loStartIcon.Location = new Geopoint(loStartPosition);
                loStartIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                loStartIcon.Title = XportBand.Resources.Strings.MapPositionStartText;
                loStartIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/LocationDarkGreen.png"));
                mapActivity.MapElements.Add(loStartIcon);
                // Configure end position icon and assign path to MapControl
                BasicGeoposition loEndPosition = path.Positions[path.Positions.Count - 1];
                MapIcon loEndIcon = new MapIcon();
                loEndIcon.Location = new Geopoint(loEndPosition);
                loEndIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                loEndIcon.Title = XportBand.Resources.Strings.MapPositionEndText;
                loEndIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/LocationDarkRed.png"));
                mapActivity.MapElements.Add(loEndIcon);
                // Center map to start position and assign default zoom level to 15 (TODO: auto-zoom)
                mapActivity.Center = new Geopoint(loStartPosition);
                mapActivity.ZoomLevel = 15;
            }
        }

        #endregion

        #region Overriding

        /// <summary>
        /// Raises the <see cref="E:NavigatedTo" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // Determine if context (ViewModel) implements INavigable interface, if so, call Activate.
            Model.INavigable loNavigableVM = DataContext as Model.INavigable;
            if (loNavigableVM != null)
                loNavigableVM.Activate(e.Parameter);
        }

        /// <summary>
        /// Raises the <see cref="E:NavigatedFrom" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            // Determine if context (ViewModel) implements INavigable interface, if so, call Deactivate.
            Model.INavigable loNavigableVM = DataContext as Model.INavigable;
            if (loNavigableVM != null)
                loNavigableVM.Deactivate(e.Parameter);
        }

        #endregion

    }

}
