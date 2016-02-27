//-----------------------------------------------------------------------
// <copyright file="ActivityDetailsViewModel.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2014-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace XportBand.ViewModel
{
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Views;
    using Model;
    using MSHealthAPI;
    using NikePlusAPI;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Input;
    using System.Xml.Linq;
    using Windows.Devices.Geolocation;
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.UI.Core;

    /// <summary>
    /// ViewModel for ActivityDetails view.
    /// </summary>
    /// <seealso cref="GalaSoft.MvvmLight.ViewModelBase" />
    /// <seealso cref="XportBand.Model.INavigable" />
    public sealed class ActivityDetailsViewModel : ViewModelBase, INavigable
    {

        #region Inner Members

        /// <summary>
        /// Dialog service instance.
        /// </summary>
        private readonly IDialogService moDialogService;

        /// <summary>
        /// Navigation service instance.
        /// </summary>
        private readonly INavigationService moNavigationService;

        /// <summary>
        /// Microsoft Health service instance.
        /// </summary>
        private readonly IMSHealthClient moMSHealthClient;

        /// <summary>
        /// Nike+ service instance.
        /// </summary>
        private readonly INikePlusClient moNikePlusClient;

        /// <summary>
        /// Inner member for <see cref="IsLoaded"/> property.
        /// </summary>
        private bool mbIsLoaded = false;

        /// <summary>
        /// Inner member for <see cref="IsRunningRequest"/> property.
        /// </summary>
        private bool mbIsRunningRequest = false;

        /// <summary>
        /// Inner member for <see cref="Activity"/> property.
        /// </summary>
        private MSHealthActivity moActivity = null;

        /// <summary>
        /// Inner member for <see cref="TotalDistance"/> property.
        /// </summary>
        private decimal? mdTotalDistance = null;

        /// <summary>
        /// Inner member for <see cref="DistanceUnitShort"/> property.
        /// </summary>
        private string msDistanceUnitShort;

        /// <summary>
        /// Inner member for <see cref="Pace"/> property.
        /// </summary>
        private TimeSpan? mtsPace = null;

        /// <summary>
        /// Inner member for <see cref="ElevationGain"/> property.
        /// </summary>
        private double? mdElevationGain = null;

        /// <summary>
        /// Inner member for <see cref="ElevationLoss"/> property.
        /// </summary>
        private double? mdElevationLoss = null;

        /// <summary>
        /// Inner member for <see cref="MaxElevation"/> property.
        /// </summary>
        private double? mdMaxElevation = null;

        /// <summary>
        /// Inner member for <see cref="MinElevation"/> property.
        /// </summary>
        private double? mdMinElevation = null;

        /// <summary>
        /// Inner member for <see cref="IsElevationAvailable"/> property.
        /// </summary>
        private bool mbIsElevationAvailable = false;

        /// <summary>
        /// Inner member for <see cref="HeartRateZones"/> property.
        /// </summary>
        private ObservableCollection<HeartRateZoneItem> moHeartRateZones;

        /// <summary>
        /// Inner member for <see cref="IsHeartRateZonesAvailable"/> property.
        /// </summary>
        private bool mbIsHeartRateZonesAvailable = false;

        /// <summary>
        /// Inner member for <see cref="Splits"/> property.
        /// </summary>
        private ObservableCollection<SplitItem> moSplits;

        /// <summary>
        /// Inner member for <see cref="MapPath"/> property.
        /// </summary>
        private Geopath moMapPath;

        /// <summary>
        /// Inner member for <see cref="IsNikePlusAvailable"/> property.
        /// </summary>
        private bool mbIsNikePlusAvailable;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loaded.
        /// </summary>
        public bool IsLoaded
        {
            get { return mbIsLoaded; }
            set { Set<bool>(() => IsLoaded, ref mbIsLoaded, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is running request.
        /// </summary>
        public bool IsRunningRequest
        {
            get { return mbIsRunningRequest; }
            set { Set<bool>(() => IsRunningRequest, ref mbIsRunningRequest, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="MSHealthActivity"/> details to show.
        /// </summary>
        public MSHealthActivity Activity
        {
            get { return moActivity; }
            set { Set<MSHealthActivity>(() => Activity, ref moActivity, value); }
        }

        /// <summary>
        /// Gets or sets the total distance (converted from <see cref="MSHealthDistanceSummary.TotalDistance"/> and 
        /// <see cref="MSHealthActivity.SplitDistance"/>).
        /// </summary>
        public decimal? TotalDistance
        {
            get { return mdTotalDistance; }
            set { Set<decimal?>(() => TotalDistance, ref mdTotalDistance, value); }
        }

        /// <summary>
        /// Gets or sets the distance unit in short name.
        /// </summary>
        public string DistanceUnitShort
        {
            get { return msDistanceUnitShort; }
            set { Set<string>(() => DistanceUnitShort, ref msDistanceUnitShort, value); }
        }

        /// <summary>
        /// Gets or sets the pace (converted from <see cref="MSHealthDistanceSummary.Pace"/>).
        /// </summary>
        public TimeSpan? Pace
        {
            get { return mtsPace; }
            set { Set<TimeSpan?>(() => Pace, ref mtsPace, value); }
        }

        /// <summary>
        /// Gets or sets the elevation gain (converted from <see cref="MSHealthDistanceSummary.ElevationGain"/> and 
        /// <see cref="MSHealthDistanceSummary.ELEVATION_FACTOR"/>).
        /// </summary>
        public double? ElevationGain
        {
            get { return mdElevationGain; }
            set { Set<double?>(() => ElevationGain, ref mdElevationGain, value); }
        }

        /// <summary>
        /// Gets or sets the elevation loss (converted from <see cref="MSHealthDistanceSummary.ElevationLoss"/> and 
        /// <see cref="MSHealthDistanceSummary.ELEVATION_FACTOR"/>).
        /// </summary>
        public double? ElevationLoss
        {
            get { return mdElevationLoss; }
            set { Set<double?>(() => ElevationLoss, ref mdElevationLoss, value); }
        }

        /// <summary>
        /// Gets or sets the maximum elevation (converted from <see cref="MSHealthDistanceSummary.MaxElevation"/> and 
        /// <see cref="MSHealthDistanceSummary.ELEVATION_FACTOR"/>).
        /// </summary>
        public double? MaxElevation
        {
            get { return mdMaxElevation; }
            set { Set<double?>(() => MaxElevation, ref mdMaxElevation, value); }
        }
        /// <summary>
        /// Gets or sets the maximum elevation (converted from <see cref="MSHealthDistanceSummary.MaxElevation"/> and 
        /// <see cref="MSHealthDistanceSummary.ELEVATION_FACTOR"/>).
        /// </summary>
        public double? MinElevation
        {
            get { return mdMinElevation; }
            set { Set<double?>(() => MinElevation, ref mdMinElevation, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether any Elevation value is available.
        /// </summary>
        public bool IsElevationAvailable
        {
            get { return mbIsElevationAvailable; }
            set { Set<bool>(() => IsElevationAvailable, ref mbIsElevationAvailable, value); }
        }

        /// <summary>
        /// Gets or sets the heart rate zones.
        /// </summary>
        public ObservableCollection<HeartRateZoneItem> HeartRateZones
        {
            get { return moHeartRateZones; }
            set { Set<ObservableCollection<HeartRateZoneItem>>(() => HeartRateZones, ref moHeartRateZones, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether any Heart Rate Zone value is available.
        /// </summary>
        public bool IsHeartRateZonesAvailable
        {
            get { return mbIsHeartRateZonesAvailable; }
            set { Set<bool>(() => IsHeartRateZonesAvailable, ref mbIsHeartRateZonesAvailable, value); }
        }

        /// <summary>
        /// Gets or sets the splits details.
        /// </summary>
        public ObservableCollection<SplitItem> Splits
        {
            get { return moSplits; }
            set { Set<ObservableCollection<SplitItem>>(() => Splits, ref moSplits, value); }
        }

        /// <summary>
        /// Gets or sets the Map Path (route) for GPS activities.
        /// </summary>
        public Geopath MapPath
        {
            get { return moMapPath; }
            set { Set<Geopath>(() => MapPath, ref moMapPath, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Nike+ is available.
        /// </summary>
        public bool IsNikePlusAvailable
        {
            get { return mbIsNikePlusAvailable; }
            set
            {
                Set<bool>(() => IsNikePlusAvailable, ref mbIsNikePlusAvailable, value);
                ((RelayCommand)SyncToNikePlusCommand).RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command to execute: Export to GPX.
        /// </summary>
        public ICommand ExportToGpxCommand { get; private set; }

        /// <summary>
        /// Gets the command to execute: Export to TCX.
        /// </summary>
        public ICommand ExportToTcxCommand { get; private set; }

        /// <summary>
        /// Gets the command to execute: Sync to Nike+.
        /// </summary>
        public ICommand SyncToNikePlusCommand { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityDetailsViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service instance.</param>
        /// <param name="navigationService">Navigation service instance.</param>
        /// <param name="msHealthClient">Microsoft Health service instance.</param>
        /// <param name="nikePlusClient">Nike+ service instance.</param>
        public ActivityDetailsViewModel(IDialogService dialogService, INavigationService navigationService, IMSHealthClient msHealthClient, INikePlusClient nikePlusClient)
        {
            moDialogService = dialogService;
            moNavigationService = navigationService;
            moMSHealthClient = msHealthClient;
            moNikePlusClient = nikePlusClient;
            ExportToGpxCommand = new RelayCommand(ExportToGpx, () => true);
            ExportToTcxCommand = new RelayCommand(ExportToTcx, () => true);
            SyncToNikePlusCommand = new RelayCommand(SyncToNikePlus, () => IsNikePlusAvailable);
            // Handle backbutton requests
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) =>
            {
                if (!IsRunningRequest)
                {
                    e.Handled = true;
                    moNavigationService.GoBack();
                }
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Exports current activity to GPX.
        /// </summary>
        private async void ExportToGpx()
        {
            if (Activity != null)
            {
                XDocument loGPX = null;
                try
                {
                    // Export Activity without force (if no MapPoints or GPSLocation, raises exception)
                    loGPX = Activity.ToGPX("XportBand");
                }
                catch (ArgumentNullException loArgumentNullException)
                {
                    loGPX = null;
                    // Check if exception was raised because of missing MapPoints/GPSLocation data
                    if (!string.IsNullOrEmpty(loArgumentNullException.ParamName) &&
                        loArgumentNullException.ParamName.Equals("MapPoints", StringComparison.OrdinalIgnoreCase))
                    {
                        // Ask user to confirm
                        await moDialogService.ShowMessage(Resources.Strings.MessageContentExportEmptyGPSData,
                                                          Resources.Strings.MessageTitleExportGPX,
                                                          Resources.Strings.MessageButtonYes,
                                                          Resources.Strings.MessageButtonNo,
                                                          (confirm) =>
                                                          {
                                                              if (confirm)
                                                              {
                                                                  loGPX = Activity.ToGPX("XportBand", null, confirm);
                                                              }
                                                          });
                    }
                    else
                    {
                        // Handle exceptions (just for debugging purposes). TODO: Delete this code
                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            System.Diagnostics.Debug.WriteLine(loArgumentNullException.StackTrace);
                            System.Diagnostics.Debugger.Break();
                        } // Handle exceptions (just for debugging purposes)
                        await moDialogService.ShowError(Resources.Strings.MessageContentErrorOperation,
                                                        Resources.Strings.MessageTitleExportGPX,
                                                        Resources.Strings.MessageButtonOK,
                                                        null);
                    }
                }
                catch (Exception loException)
                {
                    // Handle exceptions (just for debugging purposes). TODO: Delete this code
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debug.WriteLine(loException.StackTrace);
                        System.Diagnostics.Debugger.Break();
                    } // Handle exceptions (just for debugging purposes)
                    loGPX = null;
                    await moDialogService.ShowError(Resources.Strings.MessageContentErrorOperation,
                                                    Resources.Strings.MessageTitleExportGPX,
                                                    Resources.Strings.MessageButtonOK,
                                                    null);
                }

                // Check GPX (XML formatted document)
                if (loGPX != null)
                {
                    // Convert XML formatted document to string
                    string lsGPX = null;
                    StringBuilder loStringBuilder = new StringBuilder();
                    using (TextWriter loTextWriter = new EncodingStringWriter(loStringBuilder, new UTF8Encoding(false)))
                    {
                        loGPX.Save(loTextWriter);
                        lsGPX = loStringBuilder.ToString();
                    }
                    // Ask user for file destination
                    FileSavePicker loFileSavePicker = new FileSavePicker();
                    //loFileSavePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    loFileSavePicker.FileTypeChoices.Add("GPX", new List<string>() { ".gpx" });
                    loFileSavePicker.SuggestedFileName = string.Format("XportBand{0}{1:yyyyMMddHHmmss}", Activity.Type, Activity.StartTime);
                    StorageFile loStorageFile = await loFileSavePicker.PickSaveFileAsync();
                    if (loStorageFile != null)
                    {
                        // Save file
                        CachedFileManager.DeferUpdates(loStorageFile);
                        await FileIO.WriteTextAsync(loStorageFile, lsGPX);
                        Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(loStorageFile);
                        if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                        {
                            await moDialogService.ShowMessage(string.Format(Resources.Strings.MessageContentActivityExportSuccess, loStorageFile.Name),
                                                              Resources.Strings.MessageTitleExportGPX);
                        }
                        else
                        {
                            await moDialogService.ShowError(string.Format(Resources.Strings.MessageContentActivityExportFail, loStorageFile.Name),
                                                            Resources.Strings.MessageTitleExportGPX,
                                                            Resources.Strings.MessageButtonOK,
                                                            null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Exports current activity to TCX.
        /// </summary>
        private async void ExportToTcx()
        {
            if (Activity != null)
            {
                XDocument loTCX = null;
                try
                {
                    // Export Activity without force (if no MapPoints or GPSLocation, raises exception)
                    loTCX = Activity.ToTCX("XportBand");
                }
                catch (ArgumentNullException loArgumentNullException)
                {
                    loTCX = null;
                    // Check if exception was raised because of missing MapPoints/GPSLocation data
                    if (!string.IsNullOrEmpty(loArgumentNullException.ParamName) &&
                        loArgumentNullException.ParamName.Equals("MapPoints", StringComparison.OrdinalIgnoreCase))
                    {
                        // Ask user to confirm
                        await moDialogService.ShowMessage(Resources.Strings.MessageContentExportEmptyGPSData,
                                                          Resources.Strings.MessageTitleExportTCX,
                                                          Resources.Strings.MessageButtonYes,
                                                          Resources.Strings.MessageButtonNo,
                                                          (confirm) =>
                                                          {
                                                              if (confirm)
                                                              {
                                                                  loTCX = Activity.ToTCX("XportBand", confirm);
                                                              }
                                                          });
                    }
                    else
                    {
                        // Handle exceptions (just for debugging purposes). TODO: Delete this code
                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            System.Diagnostics.Debug.WriteLine(loArgumentNullException.StackTrace);
                            System.Diagnostics.Debugger.Break();
                        } // Handle exceptions (just for debugging purposes)
                        await moDialogService.ShowError(Resources.Strings.MessageContentErrorOperation,
                                                        Resources.Strings.MessageTitleExportTCX,
                                                        Resources.Strings.MessageButtonOK,
                                                        null);
                    }
                }
                catch (Exception loException)
                {
                    // Handle exceptions (just for debugging purposes). TODO: Delete this code
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debug.WriteLine(loException.StackTrace);
                        System.Diagnostics.Debugger.Break();
                    } // Handle exceptions (just for debugging purposes)
                    loTCX = null;
                    await moDialogService.ShowError(Resources.Strings.MessageContentErrorOperation,
                                                    Resources.Strings.MessageTitleExportTCX,
                                                    Resources.Strings.MessageButtonOK,
                                                    null);
                }

                // Check TCX (XML formatted document)
                if (loTCX != null)
                {
                    // Convert XML formatted document to string
                    string lsTCX = null;
                    StringBuilder loStringBuilder = new StringBuilder();
                    using (TextWriter loTextWriter = new EncodingStringWriter(loStringBuilder, new UTF8Encoding(false)))
                    {
                        loTCX.Save(loTextWriter);
                        lsTCX = loStringBuilder.ToString();
                    }
                    // Ask user for file destination
                    FileSavePicker loFileSavePicker = new FileSavePicker();
                    //loFileSavePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    loFileSavePicker.FileTypeChoices.Add("TCX", new List<string>() { ".tcx" });
                    loFileSavePicker.SuggestedFileName = string.Format("XportBand{0}{1:yyyyMMddHHmmss}", Activity.Type, Activity.StartTime);
                    StorageFile loStorageFile = await loFileSavePicker.PickSaveFileAsync();
                    if (loStorageFile != null)
                    {
                        // Save file
                        CachedFileManager.DeferUpdates(loStorageFile);
                        await FileIO.WriteTextAsync(loStorageFile, lsTCX);
                        Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(loStorageFile);
                        if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                        {
                            await moDialogService.ShowMessage(string.Format(Resources.Strings.MessageContentActivityExportSuccess, loStorageFile.Name),
                                                              Resources.Strings.MessageTitleExportTCX);
                        }
                        else
                        {
                            await moDialogService.ShowError(string.Format(Resources.Strings.MessageContentActivityExportFail, loStorageFile.Name),
                                                            Resources.Strings.MessageTitleExportTCX,
                                                            Resources.Strings.MessageButtonOK,
                                                            null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Synchronizes current activity to Nike+.
        /// </summary>
        private async void SyncToNikePlus()
        {
            try
            {
                IsRunningRequest = true;
                // Convert MSHealthActivity to NikePlusActivity
                NikePlusActivity loNActivity = Activity.ToNikePlusActicity();
                // Sync Activity
                string lsNActivityID = await moNikePlusClient.SyncActivityV2(loNActivity);
                if (!string.IsNullOrEmpty(lsNActivityID))
                {
                    string lsActivityUrl = string.Format(NikePlusClient.PATTERN_URL_ACTIVITY_DETAILS, lsNActivityID);
                    try
                    {
                        loNActivity = await moNikePlusClient.ReadActivity(lsNActivityID, false);
                        if (loNActivity != null)
                        {
                            // Report success sync with detailed NikeFuel gain
                            // Report with option to show sync activity
                            await moDialogService.ShowMessage(string.Format(Resources.Strings.MessageContentNikePlusSyncWithFuel, loNActivity.MetricSummary.Fuel),
                                                              Resources.Strings.MessageTitleNikePlus,
                                                              Resources.Strings.MessageButtonOK,
                                                              Resources.Strings.MessageButtonMoreDetails,
                                                              async (result) =>
                                                              {
                                                                  if (!result)
                                                                      await Windows.System.Launcher.LaunchUriAsync(new Uri(lsActivityUrl));
                                                              });
                        }
                        else
                        {
                            // Just report success sync
                            await moDialogService.ShowMessage(Resources.Strings.MessageContentNikePlusSync,
                                                              Resources.Strings.MessageTitleNikePlus,
                                                              Resources.Strings.MessageButtonOK,
                                                              Resources.Strings.MessageButtonMoreDetails,
                                                              async (result) =>
                                                              {
                                                                  if (!result)
                                                                      await Windows.System.Launcher.LaunchUriAsync(new Uri(lsActivityUrl));
                                                              });
                        }
                    }
                    catch (Exception loException2)
                    {
                        // Handle exceptions (just for debugging purposes)
                        if (System.Diagnostics.Debugger.IsAttached)
                            System.Diagnostics.Debug.WriteLine(loException2.StackTrace);
                        // Just report success sync
                        await moDialogService.ShowMessage(Resources.Strings.MessageContentNikePlusSync,
                                                          Resources.Strings.MessageTitleNikePlus,
                                                          Resources.Strings.MessageButtonOK,
                                                          Resources.Strings.MessageButtonMoreDetails,
                                                          async (result) =>
                                                          {
                                                              if (!result)
                                                                  await Windows.System.Launcher.LaunchUriAsync(new Uri(lsActivityUrl));
                                                          });
                    }
                }
                else
                {
                    // Report failed sync
                    await moDialogService.ShowMessage(Resources.Strings.MessageContentNikePlusSyncFail,
                                                      Resources.Strings.MessageTitleNikePlus,
                                                      Resources.Strings.MessageButtonOK,
                                                      null);
                }
                //
            }
            catch (Exception loException)
            {
                // Handle exceptions (just for debugging purposes)
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debug.WriteLine(loException.StackTrace);
                // Report failed sync
                await moDialogService.ShowMessage(Resources.Strings.MessageContentNikePlusSyncFail,
                                                  Resources.Strings.MessageTitleNikePlus,
                                                  Resources.Strings.MessageButtonOK,
                                                  null);
            }
            finally
            {
                IsRunningRequest = false;
            }
        }

        #endregion

        #region INavigable implementation

        /// <summary>
        /// Handler for <see cref="Page.OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs)" />.
        /// </summary>
        /// <param name="parameter"><see cref="Windows.UI.Xaml.Navigation.NavigationEventArgs.Parameter" />.</param>
        public async void Activate(object parameter)
        {
            string lsActivityID = parameter as string;
            MSHealthActivities loActivities = null;
            MSHealthSplitDistanceType loDistanceType = MSHealthSplitDistanceType.None;
            MSHealthActivityInclude loInclude = MSHealthActivityInclude.Details | MSHealthActivityInclude.MapPoints;
            IsLoaded = false;
            Activity = null;
            TotalDistance = null;
            DistanceUnitShort = null;
            Pace = null;
            Splits = null;
            HeartRateZones = null;
            MapPath = null;
            ElevationGain = null;
            ElevationLoss = null;
            MaxElevation = null;
            MinElevation = null;
            IsElevationAvailable = false;
            IsHeartRateZonesAvailable = false;
            IsNikePlusAvailable = false;
            List<HeartRateZoneItem> loHeartRateZones = null;
            // Set back button visible (for Windows app)
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            // Check parameter (Activity ID)
            if (!string.IsNullOrEmpty(lsActivityID))
            {
                IsRunningRequest = true;
                // Determine Distance Unit
                switch (Settings.MSHealthFilterDistance)
                {
                    case DistanceUnit.DISTANCE_MILE:
                        loDistanceType = MSHealthSplitDistanceType.Mile;
                        DistanceUnitShort = Resources.Strings.TextDistanceUnitShortMileText;
                        break;
                    case DistanceUnit.DISTANCE_KILOMETER:
                        loDistanceType = MSHealthSplitDistanceType.Kilometer;
                        DistanceUnitShort = Resources.Strings.TextDistanceUnitShortKilometerText;
                        break;
                }
                try
                {
                    // Get Activity details
                    loActivities = await moMSHealthClient.ListActivities(ids: lsActivityID,
                                                                         include: loInclude,
                                                                         splitDistanceType: loDistanceType);
                    // Check Activity details returned
                    if (loActivities.ItemCount > 0)
                    {
                        // Map from derivated activities to single instance activity
                        if (loActivities.FreePlayActivities != null &&
                            loActivities.FreePlayActivities.Any())
                            Activity = loActivities.FreePlayActivities.FirstOrDefault();
                        else if (loActivities.RunActivities != null &&
                                 loActivities.RunActivities.Any())
                            Activity = loActivities.RunActivities.FirstOrDefault();
                        else if (loActivities.BikeActivities != null &&
                                 loActivities.BikeActivities.Any())
                            Activity = loActivities.BikeActivities.FirstOrDefault();
                        else if (loActivities.GolfActivities != null &&
                                 loActivities.GolfActivities.Any())
                            Activity = loActivities.GolfActivities.FirstOrDefault();
                        else if (loActivities.GuidedWorkoutActivities != null &&
                                 loActivities.GuidedWorkoutActivities.Any())
                            Activity = loActivities.GuidedWorkoutActivities.FirstOrDefault();
                        else if (loActivities.SleepActivities != null &&
                                 loActivities.SleepActivities.Any())
                            Activity = loActivities.SleepActivities.FirstOrDefault();
                    }
                    // Check current activity instance
                    if (Activity != null)
                    {
                        // Calculate Total Distance
                        if (Activity.SplitDistance != null &&
                            Activity.SplitDistance.HasValue &&
                            Activity.SplitDistance.Value > 0 &&
                            Activity.DistanceSummary != null &&
                            Activity.DistanceSummary.TotalDistance != null &&
                            Activity.DistanceSummary.TotalDistance.HasValue)
                        {
                            TotalDistance = (decimal)Activity.DistanceSummary.TotalDistance / (decimal)Activity.SplitDistance;
                        }
                        // Calculate Pace
                        if (Activity.DistanceSummary != null &&
                            Activity.DistanceSummary.Pace != null &&
                            Activity.DistanceSummary.Pace.HasValue)
                        {
                            Pace = TimeSpan.FromMilliseconds((double)Activity.DistanceSummary.Pace);
                        }
                        // Calculate Elevation
                        if (Activity.DistanceSummary != null)
                        {
                            // Elevation Gain
                            if (Activity.DistanceSummary.ElevationGain != null)
                            {
                                ElevationGain = (double)Activity.DistanceSummary.ElevationGain / MSHealthDistanceSummary.ELEVATION_FACTOR;
                                IsElevationAvailable = true;
                            }
                            // Elevation Loss
                            if (Activity.DistanceSummary.ElevationLoss != null)
                            {
                                ElevationLoss = (double)Activity.DistanceSummary.ElevationLoss / MSHealthDistanceSummary.ELEVATION_FACTOR;
                                IsElevationAvailable = true;
                            }
                            // Max Elevation
                            if (Activity.DistanceSummary.MaxElevation != null)
                            {
                                MaxElevation = (double)Activity.DistanceSummary.MaxElevation / MSHealthDistanceSummary.ELEVATION_FACTOR;
                                IsElevationAvailable = true;
                            }
                            // Min Elevation
                            if (Activity.DistanceSummary.MinElevation != null)
                            {
                                MinElevation = (double)Activity.DistanceSummary.MinElevation / MSHealthDistanceSummary.ELEVATION_FACTOR;
                                IsElevationAvailable = true;
                            }
                        }
                        // Heart Rate Zones
                        if (Activity.PerformanceSummary != null &&
                            Activity.PerformanceSummary.HeartRateZones != null)
                        {
                            loHeartRateZones = new List<HeartRateZoneItem>();
                            // Underhealthy
                            loHeartRateZones.Add(new HeartRateZoneItem()
                            {
                                Key = HeartRateZoneItem.HRZONE_UNDER_HEALTHY,
                                Name = Resources.Strings.TextHeartRateZoneUnderHealthyText,
                                Value = Activity.PerformanceSummary.HeartRateZones.UnderHealthyHeart ?? 0,
                            });
                            // Healthy
                            loHeartRateZones.Add(new HeartRateZoneItem()
                            {
                                Key = HeartRateZoneItem.HRZONE_HEALTHY,
                                Name = Resources.Strings.TextHeartRateZoneHealthyText,
                                Value = Activity.PerformanceSummary.HeartRateZones.HealthyHeart ?? 0,
                            });
                            // Fitness
                            loHeartRateZones.Add(new HeartRateZoneItem()
                            {
                                Key = HeartRateZoneItem.HRZONE_FITNESS,
                                Name = Resources.Strings.TextHeartRateZoneFitnessText,
                                Value = Activity.PerformanceSummary.HeartRateZones.FitnessZone ?? 0,
                            });
                            // Aerobic
                            loHeartRateZones.Add(new HeartRateZoneItem()
                            {
                                Key = HeartRateZoneItem.HRZONE_AEROBIC,
                                Name = Resources.Strings.TextHeartRateZoneAerobicText,
                                Value = Activity.PerformanceSummary.HeartRateZones.Aerobic ?? 0,
                            });
                            // Anaerobic
                            loHeartRateZones.Add(new HeartRateZoneItem()
                            {
                                Key = HeartRateZoneItem.HRZONE_ANAEROBIC,
                                Name = Resources.Strings.TextHeartRateZoneAnaerobicText,
                                Value = Activity.PerformanceSummary.HeartRateZones.Anaerobic ?? 0,
                            });
                            // Redline
                            loHeartRateZones.Add(new HeartRateZoneItem()
                            {
                                Key = HeartRateZoneItem.HRZONE_REDLINE,
                                Name = Resources.Strings.TextHeartRateZoneRedlineText,
                                Value = Activity.PerformanceSummary.HeartRateZones.Redline ?? 0,
                            });
                            // OverRedline
                            loHeartRateZones.Add(new HeartRateZoneItem()
                            {
                                Key = HeartRateZoneItem.HRZONE_OVER_REDLINE,
                                Name = Resources.Strings.TextHeartRateZoneOverRedlineText,
                                Value = Activity.PerformanceSummary.HeartRateZones.OverRedline ?? 0,
                            });
                            HeartRateZones = new ObservableCollection<HeartRateZoneItem>(loHeartRateZones);
                            IsHeartRateZonesAvailable = true;
                        }
                        // Segments (splits)
                        if (Activity.ActivitySegments != null &&
                            Activity.ActivitySegments.Any() &&
                            TotalDistance != null)
                        {
                            // ActivitySegment to Split
                            double ldSplitValue = 0;
                            List<SplitItem> loSplits = new List<SplitItem>();
                            foreach (MSHealthActivitySegment loSegment in Activity.ActivitySegments.OrderBy(loSeg => loSeg.StartTime))
                            {
                                ldSplitValue++;
                                loSplits.Add(new SplitItem()
                                {
                                    Value = ldSplitValue > (double)TotalDistance.Value ? (double)TotalDistance.Value : ldSplitValue,
                                    Duration = loSegment.Duration.Value,
                                    AvgHeartRate = loSegment.HeartRateSummary != null ? loSegment.HeartRateSummary.AverageHeartRate.Value : 0,
                                });
                            }
                            // Get Max/Min Duration/HR, for complete splits only
                            try
                            {
                                loSplits.Where(loSplit => (loSplit.Value % 1) == 0).OrderBy(loSplit => loSplit.Duration).First().DurationMark = "↓";
                                loSplits.Where(loSplit => (loSplit.Value % 1) == 0).OrderByDescending(loSplit => loSplit.Duration).First().DurationMark = "↑";
                                loSplits.Where(loSplit => (loSplit.Value % 1) == 0).OrderBy(loSplit => loSplit.AvgHeartRate).First().HRMark = "↓";
                                loSplits.Where(loSplit => (loSplit.Value % 1) == 0).OrderByDescending(loSplit => loSplit.AvgHeartRate).First().HRMark = "↑";
                            }
                            catch { /* Do nothing */ }
                            // Sort by value and assign to instance
                            loSplits = loSplits.OrderBy(loSplit => loSplit.Value).ToList();
                            Splits = new ObservableCollection<SplitItem>(loSplits);
                        }
                        // MapPoints to MapPath
                        if (Activity.MapPoints != null &&
                            Activity.MapPoints.Any())
                        {
                            List<BasicGeoposition> loGeopositions = new List<BasicGeoposition>();
                            loGeopositions = (from loMapPoint in Activity.MapPoints
                                                                         .Where(loPoint => loPoint.Location != null &&
                                                                                           loPoint.Location.Latitude != null &&
                                                                                           loPoint.Location.Longitude != null)
                                                                         .OrderBy(loPoint => loPoint.Ordinal)
                                              select new BasicGeoposition()
                                              {
                                                  Latitude = (double)loMapPoint.Location.Latitude / MSHealthGPSPoint.LATITUDE_FACTOR,
                                                  Longitude = (double)loMapPoint.Location.Longitude / MSHealthGPSPoint.LONGITUDE_FACTOR,
                                                  Altitude = loMapPoint.Location.ElevationFromMeanSeaLevel != null ?
                                                                ((double)loMapPoint.Location.ElevationFromMeanSeaLevel / MSHealthGPSPoint.ELEVATION_FACTOR) : 0d,
                                              }).ToList();
                            //foreach (var loMapPoint in Activity.MapPoints)
                            //{
                            //    if (loMapPoint.Location != null &&
                            //        loMapPoint.Location.Latitude != null &&
                            //        loMapPoint.Location.Longitude != null)
                            //    {
                            //        loGeopositions.Add(new BasicGeoposition()
                            //        {
                            //            Latitude = (double)loMapPoint.Location.Latitude / 10000000d,
                            //            Longitude = (double)loMapPoint.Location.Longitude / 10000000d,
                            //        });
                            //    }
                            //}
                            if (loGeopositions.Any())
                            {
                                MapPath = new Geopath(loGeopositions);
                            }
                        }
                    }
                }
                catch (Exception loException)
                {
                    // Handle exceptions (just for debugging purposes)
                    if (System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debug.WriteLine(loException.StackTrace);
                    await moDialogService.ShowError(Resources.Strings.MessageContentErrorOperation,
                                                    Resources.Strings.MessageTitleError,
                                                    Resources.Strings.MessageButtonOK,
                                                    null);
                    // Return to main page
                    moNavigationService.GoBack();
                }
                finally
                {
                    Messenger.Default.Send<Geopath>(MapPath);
                    IsRunningRequest = false;
                }
                // Check for Nike+ Credentials
                if (Settings.NikePlusCredential != null)
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(Settings.NikePlusCredential.Password, "[\"&`'<>]"))
                    {
                        try
                        {
                            // Check for GPS data
                            if (Activity.MapPoints == null ||
                                !Activity.MapPoints.Any())
                            {
                                // Get Minute Summaries
                                loActivities = await moMSHealthClient.ListActivities(ids: lsActivityID,
                                                                             include: MSHealthActivityInclude.MinuteSummaries,
                                                                             splitDistanceType: loDistanceType);
                                // Check Activity details returned
                                if (loActivities.ItemCount > 0)
                                {
                                    // Map from derivated activities to single instance activity
                                    if (loActivities.FreePlayActivities != null &&
                                        loActivities.FreePlayActivities.Any())
                                        Activity.MinuteSummaries = loActivities.FreePlayActivities.FirstOrDefault().MinuteSummaries;
                                    else if (loActivities.RunActivities != null &&
                                             loActivities.RunActivities.Any())
                                        Activity.MinuteSummaries = loActivities.RunActivities.FirstOrDefault().MinuteSummaries;
                                    else if (loActivities.BikeActivities != null &&
                                             loActivities.BikeActivities.Any())
                                        Activity.MinuteSummaries = loActivities.BikeActivities.FirstOrDefault().MinuteSummaries;
                                    else if (loActivities.GolfActivities != null &&
                                             loActivities.GolfActivities.Any())
                                        Activity.MinuteSummaries = loActivities.GolfActivities.FirstOrDefault().MinuteSummaries;
                                    else if (loActivities.GuidedWorkoutActivities != null &&
                                             loActivities.GuidedWorkoutActivities.Any())
                                        Activity.MinuteSummaries = loActivities.GuidedWorkoutActivities.FirstOrDefault().MinuteSummaries;
                                    else if (loActivities.SleepActivities != null &&
                                             loActivities.SleepActivities.Any())
                                        Activity.MinuteSummaries = loActivities.SleepActivities.FirstOrDefault().MinuteSummaries;
                                }
                            }
                        }
                        catch (Exception loException)
                        {
                            // Handle exceptions (just for debugging purposes)
                            if (System.Diagnostics.Debugger.IsAttached)
                                System.Diagnostics.Debug.WriteLine(loException.StackTrace);
                        }
                        // Ensure Activity either has GPS or MinuteSummaries data
                        if ((Activity.MapPoints != null &&
                             Activity.MapPoints.Any()) ||
                             (Activity.MinuteSummaries != null &&
                              Activity.MinuteSummaries.Any()))
                        {
                            moNikePlusClient.SetCredentials(Settings.NikePlusCredential.UserName, Settings.NikePlusCredential.Password);
                            IsNikePlusAvailable = true;
                        }
                    }
                }
                IsLoaded = true;
            }
        }

        /// <summary>
        /// Handler for <see cref="Windows.UI.Xaml.Controls.Page.OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs)" />.
        /// </summary>
        /// <param name="parameter"><see cref="Windows.UI.Xaml.Navigation.NavigationEventArgs.Parameter" />.</param>
        public void Deactivate(object parameter)
        {
            IsLoaded = false;
            // No implementation necessary
        }

        #endregion

    }

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
