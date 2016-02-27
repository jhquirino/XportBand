//-----------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace XportBand.ViewModel
{
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Views;
    using Model;
    using MSHealthAPI;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Windows.Input;
#if WINDOWS_UWP
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.UI.Core;
#endif    

    /// <summary>
    /// ViewModel for Main view.
    /// </summary>
    /// <seealso cref="GalaSoft.MvvmLight.ViewModelBase" />
    /// <seealso cref="XportBand.Model.INavigable" />
    public sealed class MainViewModel : ViewModelBase, INavigable
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
        /// Inner member for <see cref="IsRunningRequest"/> property.
        /// </summary>
        private bool mbIsRunningRequest = false;

        /// <summary>
        /// Inner member for <see cref="IsMSHealthSignedIn"/> property.
        /// </summary>
        private bool mbIsMSHealthSignedIn = false;

        /// <summary>
        /// Inner member for <see cref="Activities"/> property.
        /// </summary>
        private ObservableCollection<MSHealthActivity> moActivities;

        /// <summary>
        /// Inner member for <see cref="SelectedActivity"/> property.
        /// </summary>
        private MSHealthActivity moSelectedActivity;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is running request.
        /// </summary>
        public bool IsRunningRequest
        {
            get { return mbIsRunningRequest; }
            set { Set<bool>(() => IsRunningRequest, ref mbIsRunningRequest, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether user is signed-into Microsoft Health.
        /// </summary>
        public bool IsMSHealthSignedIn
        {
            get { return mbIsMSHealthSignedIn; }
            set { Set<bool>(() => IsMSHealthSignedIn, ref mbIsMSHealthSignedIn, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter activity type: Exercise.
        /// </summary>
        public bool FilterActivityExercise
        {
            get { return Settings.MSHealthFilterActivityExercise; }
            set { Settings.MSHealthFilterActivityExercise = value; RaisePropertyChanged<bool>(() => FilterActivityExercise); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter activity type: Run.
        /// </summary>
        public bool FilterActivityRun
        {
            get { return Settings.MSHealthFilterActivityRun; }
            set { Settings.MSHealthFilterActivityRun = value; RaisePropertyChanged<bool>(() => FilterActivityRun); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter activity type: Bike.
        /// </summary>
        public bool FilterActivityBike
        {
            get { return Settings.MSHealthFilterActivityBike; }
            set { Settings.MSHealthFilterActivityBike = value; RaisePropertyChanged<bool>(() => FilterActivityBike); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter activity type: Golf.
        /// </summary>
        public bool FilterActivityGolf
        {
            get { return Settings.MSHealthFilterActivityGolf; }
            set { Settings.MSHealthFilterActivityGolf = value; RaisePropertyChanged<bool>(() => FilterActivityGolf); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter activity type: Sleep.
        /// </summary>
        public bool FilterActivitySleep
        {
            get { return Settings.MSHealthFilterActivitySleep; }
            set { Settings.MSHealthFilterActivitySleep = value; RaisePropertyChanged<bool>(() => FilterActivitySleep); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter activity type: Guided Workout.
        /// </summary>
        public bool FilterActivityGuided
        {
            get { return Settings.MSHealthFilterActivityGuided; }
            set { Settings.MSHealthFilterActivityGuided = value; RaisePropertyChanged<bool>(() => FilterActivityGuided); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter period: Last Day.
        /// </summary>
        public bool FilterPeriodDay
        {
            get { return !string.IsNullOrEmpty(Settings.MSHealthFilterPeriod) && Settings.PERIOD_DAY.Equals(Settings.MSHealthFilterPeriod, StringComparison.OrdinalIgnoreCase); }
            set
            {
                Settings.MSHealthFilterPeriod = Settings.PERIOD_DAY;
                RaisePropertyChanged<bool>(() => FilterPeriodDay);
                RaisePropertyChanged<bool>(() => FilterPeriodWeek);
                RaisePropertyChanged<bool>(() => FilterPeriodMonth);
                RaisePropertyChanged<bool>(() => FilterPeriodYear);
                RaisePropertyChanged<bool>(() => FilterPeriodAll);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter period: Last Week.
        /// </summary>
        public bool FilterPeriodWeek
        {
            get { return !string.IsNullOrEmpty(Settings.MSHealthFilterPeriod) && Settings.PERIOD_WEEK.Equals(Settings.MSHealthFilterPeriod, StringComparison.OrdinalIgnoreCase); }
            set
            {
                Settings.MSHealthFilterPeriod = Settings.PERIOD_WEEK;
                RaisePropertyChanged<bool>(() => FilterPeriodDay);
                RaisePropertyChanged<bool>(() => FilterPeriodWeek);
                RaisePropertyChanged<bool>(() => FilterPeriodMonth);
                RaisePropertyChanged<bool>(() => FilterPeriodYear);
                RaisePropertyChanged<bool>(() => FilterPeriodAll);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter period: Last Month.
        /// </summary>
        public bool FilterPeriodMonth
        {
            get { return !string.IsNullOrEmpty(Settings.MSHealthFilterPeriod) && Settings.PERIOD_MONTH.Equals(Settings.MSHealthFilterPeriod, StringComparison.OrdinalIgnoreCase); }
            set
            {
                Settings.MSHealthFilterPeriod = Settings.PERIOD_MONTH;
                RaisePropertyChanged<bool>(() => FilterPeriodDay);
                RaisePropertyChanged<bool>(() => FilterPeriodWeek);
                RaisePropertyChanged<bool>(() => FilterPeriodMonth);
                RaisePropertyChanged<bool>(() => FilterPeriodYear);
                RaisePropertyChanged<bool>(() => FilterPeriodAll);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter period: Last Year.
        /// </summary>
        public bool FilterPeriodYear
        {
            get { return !string.IsNullOrEmpty(Settings.MSHealthFilterPeriod) && Settings.PERIOD_YEAR.Equals(Settings.MSHealthFilterPeriod, StringComparison.OrdinalIgnoreCase); }
            set
            {
                Settings.MSHealthFilterPeriod = Settings.PERIOD_YEAR;
                RaisePropertyChanged<bool>(() => FilterPeriodDay);
                RaisePropertyChanged<bool>(() => FilterPeriodWeek);
                RaisePropertyChanged<bool>(() => FilterPeriodMonth);
                RaisePropertyChanged<bool>(() => FilterPeriodYear);
                RaisePropertyChanged<bool>(() => FilterPeriodAll);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filter period: All.
        /// </summary>
        public bool FilterPeriodAll
        {
            get { return !string.IsNullOrEmpty(Settings.MSHealthFilterPeriod) && Settings.PERIOD_ALL.Equals(Settings.MSHealthFilterPeriod, StringComparison.OrdinalIgnoreCase); }
            set
            {
                Settings.MSHealthFilterPeriod = Settings.PERIOD_ALL;
                RaisePropertyChanged<bool>(() => FilterPeriodDay);
                RaisePropertyChanged<bool>(() => FilterPeriodWeek);
                RaisePropertyChanged<bool>(() => FilterPeriodMonth);
                RaisePropertyChanged<bool>(() => FilterPeriodYear);
                RaisePropertyChanged<bool>(() => FilterPeriodAll);
            }
        }

        /// <summary>
        /// Gets or sets the collection of Activities found by specified criteria.
        /// </summary>
        public ObservableCollection<MSHealthActivity> Activities
        {
            get { return moActivities; }
            set { Set<ObservableCollection<MSHealthActivity>>(() => Activities, ref moActivities, value); }
        }

        /// <summary>
        /// Gets or sets the Selected Activity.
        /// </summary>
        public MSHealthActivity SelectedActivity
        {
            get { return moSelectedActivity; }
            set
            {
                Set<MSHealthActivity>(() => SelectedActivity, ref moSelectedActivity, value);
                ((RelayCommand)ShowActivityDetailsCommand).RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command to execute: Goto Settings.
        /// </summary>
        public ICommand GoToSettingsCommand { get; private set; }

        /// <summary>
        /// Gets the command to execute: List Activities.
        /// </summary>
        public ICommand ListActivitiesCommand { get; private set; }

        /// <summary>
        /// Gets the command to execute: Show Activity Details.
        /// </summary>
        public ICommand ShowActivityDetailsCommand { get; set; }

        /// <summary>
        /// Gets the command to execute: Export Activities to CSV.
        /// </summary>
        public ICommand ExportActivitiesToCSVCommand { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service instance.</param>
        /// <param name="navigationService">Navigation service instance.</param>
        /// <param name="msHealthClient">Microsoft Health service instance.</param>
        public MainViewModel(IDialogService dialogService, INavigationService navigationService, IMSHealthClient msHealthClient)
        {
            moDialogService = dialogService;
            moNavigationService = navigationService;
            moMSHealthClient = msHealthClient;
            GoToSettingsCommand = new RelayCommand(GoToSettings, () => true);
            ListActivitiesCommand = new RelayCommand(ListActivities, () => true);
            ShowActivityDetailsCommand = new RelayCommand(ShowActivityDetails, CanShowActivityDetails);
            ExportActivitiesToCSVCommand = new RelayCommand(ExportActivitiesToCSV, () => true);
            //SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) =>
            //{
            //    e.Handled = true;
            //    App.Current.Exit();
            //};
        }

        #endregion

        #region Methods

        /// <summary>
        /// Displays Settings view.
        /// </summary>
        private void GoToSettings()
        {
            moNavigationService.NavigateTo("Settings");
        }

        /// <summary>
        /// Lists the activities that matches selected criteria (type/period).
        /// </summary>
        private async void ListActivities()
        {
            DateTime? ldtStart = null;
            DateTime? ldtEnd = null;
            MSHealthActivityType loActivityType = MSHealthActivityType.Unknown;
            MSHealthSplitDistanceType loDistance = MSHealthSplitDistanceType.None;

            try
            {
                IsRunningRequest = true;
                Activities = null;
                // Check if at least one Activity Type was selected
                bool lbFilterActivityType = FilterActivityExercise | FilterActivityRun | FilterActivityBike |
                                            FilterActivityGolf | FilterActivitySleep | FilterActivityGuided;
                if (!lbFilterActivityType)
                {
#if WINDOWS_UWP
                    await moDialogService.ShowMessage(Resources.Strings.MessageContentSelectActivityType,
                                                      Resources.Strings.MessageTitleFilterActivities);
#endif
                    return;
                }
                // Determine Activity Types to filter
                if (FilterActivityExercise)
                    loActivityType |= MSHealthActivityType.FreePlay;
                if (FilterActivityRun)
                    loActivityType |= MSHealthActivityType.Run;
                if (FilterActivityBike)
                    loActivityType |= MSHealthActivityType.Bike;
                if (FilterActivityGolf)
                    loActivityType |= MSHealthActivityType.Golf;
                if (FilterActivitySleep)
                    loActivityType |= MSHealthActivityType.Sleep;
                if (FilterActivityGuided)
                    loActivityType |= MSHealthActivityType.GuidedWorkout;
                // Determine Period to filter
                switch (Settings.MSHealthFilterPeriod)
                {
                    case Settings.PERIOD_DAY:
                        ldtEnd = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                        ldtStart = DateTime.Today.AddDays(-1);
                        break;
                    case Settings.PERIOD_WEEK:
                        ldtEnd = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                        ldtStart = DateTime.Today.AddDays(-7);
                        break;
                    case Settings.PERIOD_MONTH:
                        ldtEnd = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                        ldtStart = DateTime.Today.AddMonths(-1);
                        break;
                    case Settings.PERIOD_YEAR:
                        ldtEnd = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                        ldtStart = DateTime.Today.AddYears(-1);
                        break;
                }
                // Determine Distance Unit to retrieve
                switch (Settings.MSHealthFilterDistance)
                {
                    case DistanceUnit.DISTANCE_MILE:
                        loDistance = MSHealthSplitDistanceType.Mile;
                        break;
                    case DistanceUnit.DISTANCE_KILOMETER:
                        loDistance = MSHealthSplitDistanceType.Kilometer;
                        break;
                }
                // Find activities with selected criteria, and update MSHealth Token
                MSHealthActivities loActivities = await moMSHealthClient.ListActivities(startTime: ldtStart,
                                                                                        endTime: ldtEnd,
                                                                                        type: loActivityType,
                                                                                        splitDistanceType: loDistance);
                Settings.MSHealthToken = moMSHealthClient.Token;
                // Parse each separated activity list into one single activity list
                List<MSHealthActivity> loActivitiesList = new List<MSHealthActivity>();
                if (loActivities.BikeActivities != null &&
                    loActivities.BikeActivities.Any())
                    loActivitiesList.AddRange(loActivities.BikeActivities);
                if (loActivities.RunActivities != null &&
                    loActivities.RunActivities.Any())
                    loActivitiesList.AddRange(loActivities.RunActivities);
                if (loActivities.SleepActivities != null &&
                    loActivities.SleepActivities.Any())
                    loActivitiesList.AddRange(loActivities.SleepActivities);
                if (loActivities.FreePlayActivities != null &&
                    loActivities.FreePlayActivities.Any())
                    loActivitiesList.AddRange(loActivities.FreePlayActivities);
                if (loActivities.GolfActivities != null &&
                    loActivities.GolfActivities.Any())
                    loActivitiesList.AddRange(loActivities.GolfActivities);
                if (loActivities.GuidedWorkoutActivities != null &&
                    loActivities.GuidedWorkoutActivities.Any())
                    loActivitiesList.AddRange(loActivities.GuidedWorkoutActivities);
                // Sort descending by Start Time and append to Bindable property
                loActivitiesList = loActivitiesList.OrderByDescending(loAct => loAct.StartTime).ToList();
                Activities = new ObservableCollection<MSHealthActivity>(loActivitiesList);
            }
            catch (Exception loException)
            {
                // Handle exceptions (just for debugging purposes). TODO: Delete this code
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debug.WriteLine(loException.StackTrace);
                    System.Diagnostics.Debugger.Break();
                } // Handle exceptions (just for debugging purposes)
                  // Show error message
#if WINDOWS_UWP
                await moDialogService.ShowError(Resources.Strings.MessageContentErrorOperation,
                                                Resources.Strings.MessageTitleError,
                                                Resources.Strings.MessageButtonOK,
                                                null);
#endif
            }
            finally
            {
                IsRunningRequest = false;
            }
        }

        /// <summary>
        /// Shows the <see cref="SelectedActivity"/> details.
        /// </summary>
        private void ShowActivityDetails()
        {
            if (SelectedActivity != null)
                moNavigationService.NavigateTo("ActivityDetails", SelectedActivity.ID);
        }

        /// <summary>
        /// Determines whether this instance can show <see cref="SelectedActivity"/> details.
        /// </summary>
        /// <returns></returns>
        private bool CanShowActivityDetails()
        {
            return SelectedActivity != null;
        }

        /// <summary>
        /// Exports the activities to CSV.
        /// </summary>
        private async void ExportActivitiesToCSV()
        {
            StringBuilder loStrBuilder = new StringBuilder();
            if (Activities != null)
            {
                IsRunningRequest = true;
                try
                {
                    /*
                     * Prepare file contents
                     */
                    // Header
#if WINDOWS_UWP
                    loStrBuilder.AppendLine(Resources.Strings.FileCSVActivitiesHeader);
#endif
                    // Export Content
                    foreach (MSHealthActivity loActivity in Activities)
                    {
                        // StartTime, Type, Duration
                        loStrBuilder.AppendFormat("{0:yyyy-MM-dd HH:mm:ss}", loActivity.StartTime);
                        loStrBuilder.AppendFormat(",{0}", loActivity.Type);
                        loStrBuilder.AppendFormat(",{0:c}", loActivity.Duration);
                        // HeartRate (Avg, Max, Min)
                        if (loActivity.HeartRateSummary != null)
                        {
                            loStrBuilder.AppendFormat(",{0:#}", loActivity.HeartRateSummary.AverageHeartRate);
                            loStrBuilder.AppendFormat(",{0:#}", loActivity.HeartRateSummary.PeakHeartRate);
                            loStrBuilder.AppendFormat(",{0:#}", loActivity.HeartRateSummary.LowestHeartRate);
                        }
                        else
                            loStrBuilder.Append(",,,");
                        // Calories
                        if (loActivity.CaloriesBurnedSummary != null)
                            loStrBuilder.AppendFormat(",{0:#}", loActivity.CaloriesBurnedSummary.TotalCalories);
                        else
                            loStrBuilder.Append(",");
                        // Distances
                        if (loActivity.DistanceSummary != null)
                        {
                            // Distance
                            if (loActivity.SplitDistance != null &&
                                loActivity.DistanceSummary.TotalDistance != null)
                                loStrBuilder.AppendFormat(",{0:#.00}", (double)loActivity.DistanceSummary.TotalDistance / (double)loActivity.SplitDistance);
                            else
                                loStrBuilder.AppendFormat(",");
                            // Pace
                            if (loActivity.DistanceSummary.Pace != null)
                                loStrBuilder.AppendFormat(",{0:c}", TimeSpan.FromMilliseconds((double)loActivity.DistanceSummary.Pace));
                            else
                                loStrBuilder.AppendFormat(",");
                            // Elevation gain
                            if (loActivity.DistanceSummary.ElevationGain != null)
                                loStrBuilder.AppendFormat(",{0:#.00}", (double)loActivity.DistanceSummary.ElevationGain / MSHealthDistanceSummary.ELEVATION_FACTOR);
                            else
                                loStrBuilder.AppendFormat(",");
                            // Elevation loss
                            if (loActivity.DistanceSummary.ElevationLoss != null)
                                loStrBuilder.AppendFormat(",{0:#.00}", (double)loActivity.DistanceSummary.ElevationLoss / MSHealthDistanceSummary.ELEVATION_FACTOR);
                            else
                                loStrBuilder.AppendFormat(",");
                            // Elevation max
                            if (loActivity.DistanceSummary.MaxElevation != null)
                                loStrBuilder.AppendFormat(",{0:#.00}", (double)loActivity.DistanceSummary.MaxElevation / MSHealthDistanceSummary.ELEVATION_FACTOR);
                            else
                                loStrBuilder.AppendFormat(",");
                            // Elevation min
                            if (loActivity.DistanceSummary.MinElevation != null)
                                loStrBuilder.AppendFormat(",{0:#.00}", (double)loActivity.DistanceSummary.MinElevation / MSHealthDistanceSummary.ELEVATION_FACTOR);
                            else
                                loStrBuilder.AppendFormat(",");
                        }
                        // Heart Rate zones
                        if (loActivity.PerformanceSummary != null &&
                            loActivity.PerformanceSummary.HeartRateZones != null)
                        {
                            // HRZ: UnderHealthy
                            if (loActivity.PerformanceSummary.HeartRateZones.UnderHealthyHeart != null)
                                loStrBuilder.AppendFormat(",{0:#}", loActivity.PerformanceSummary.HeartRateZones.UnderHealthyHeart);
                            else
                                loStrBuilder.AppendFormat(",");
                            // HRZ: Healthy
                            if (loActivity.PerformanceSummary.HeartRateZones.HealthyHeart != null)
                                loStrBuilder.AppendFormat(",{0:#}", loActivity.PerformanceSummary.HeartRateZones.HealthyHeart);
                            else
                                loStrBuilder.AppendFormat(",");
                            // HRZ: Fitness
                            if (loActivity.PerformanceSummary.HeartRateZones.FitnessZone != null)
                                loStrBuilder.AppendFormat(",{0:#}", loActivity.PerformanceSummary.HeartRateZones.FitnessZone);
                            else
                                loStrBuilder.AppendFormat(",");
                            // HRZ: Aerobic
                            if (loActivity.PerformanceSummary.HeartRateZones.Aerobic != null)
                                loStrBuilder.AppendFormat(",{0:#}", loActivity.PerformanceSummary.HeartRateZones.Aerobic);
                            else
                                loStrBuilder.AppendFormat(",");
                            // HRZ: Anaerobic
                            if (loActivity.PerformanceSummary.HeartRateZones.Anaerobic != null)
                                loStrBuilder.AppendFormat(",{0:#}", loActivity.PerformanceSummary.HeartRateZones.Anaerobic);
                            else
                                loStrBuilder.AppendFormat(",");
                            // HRZ: Redline
                            if (loActivity.PerformanceSummary.HeartRateZones.Redline != null)
                                loStrBuilder.AppendFormat(",{0:#}", loActivity.PerformanceSummary.HeartRateZones.Redline);
                            else
                                loStrBuilder.AppendFormat(",");
                            // HRZ: OverRedline
                            if (loActivity.PerformanceSummary.HeartRateZones.OverRedline != null)
                                loStrBuilder.AppendFormat(",{0:#}", loActivity.PerformanceSummary.HeartRateZones.OverRedline);
                            else
                                loStrBuilder.AppendFormat(",");
                        }
                        loStrBuilder.AppendLine();
                    }
                    /*
                     * Pick file name to save, and write content
                     */
#if WINDOWS_UWP
                    FileSavePicker loFileSavePicker = new FileSavePicker();
                    //loFileSavePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    loFileSavePicker.FileTypeChoices.Add("CSV", new List<string>() { ".csv" });
                    loFileSavePicker.SuggestedFileName = string.Format("XportBandActivities{0:yyyyMMddHHmmss}", DateTime.Now);
                    StorageFile loStorageFile = await loFileSavePicker.PickSaveFileAsync();
                    if (loStorageFile != null)
                    {
                        CachedFileManager.DeferUpdates(loStorageFile);
                        await FileIO.WriteTextAsync(loStorageFile, loStrBuilder.ToString());
                        Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(loStorageFile);
                        if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                        {
                            await moDialogService.ShowMessage(string.Format(Resources.Strings.MessageContentExportCSVSuccess, loStorageFile.Name),
                                                              Resources.Strings.MessageTitleExportCSV);
                        }
                        else
                        {
                            await moDialogService.ShowError(string.Format(Resources.Strings.MessageContentExportCSVFail, loStorageFile.Name),
                                                            Resources.Strings.MessageTitleExportCSV,
                                                            Resources.Strings.MessageButtonOK,
                                                            null);
                        }
                    }
#endif
                }
                catch (Exception loException)
                {
                    // Handle exceptions (just for debugging purposes). TODO: Delete this code
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debug.WriteLine(loException.StackTrace);
                        System.Diagnostics.Debugger.Break();
                    } // Handle exceptions (just for debugging purposes)
#if WINDOWS_UWP
                    await moDialogService.ShowError(Resources.Strings.MessageContentErrorOperation,
                                                    Resources.Strings.MessageTitleExportCSV,
                                                    Resources.Strings.MessageButtonOK,
                                                    null);
#endif
                }
                finally
                {
                    IsRunningRequest = false;
                }
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
            // Set back button invisible (for Windows app)
#if WINDOWS_UWP
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
#endif
            // Check Microsoft Health refresh token
            if (Settings.MSHealthToken != null &&
                !string.IsNullOrEmpty(Settings.MSHealthToken.RefreshToken) &&
                !IsMSHealthSignedIn)
            {
                try
                {
                    IsRunningRequest = true;
                    // Microsoft Health refresh token is available, so, check validity
                    if (await moMSHealthClient.ValidateToken(Settings.MSHealthToken))
                    {
                        // Microsoft Health refresh token is valid, persist and set as signed-int
                        Settings.MSHealthToken = moMSHealthClient.Token;
                        IsMSHealthSignedIn = true;
                    }
                    else
                    {
                        // Microsoft Health refresh token is not valid, so, set as signed-out
                        IsMSHealthSignedIn = false;
                    }
                }
                catch { IsMSHealthSignedIn = false; }
                finally { IsRunningRequest = false; }
            }
            else
            {
                if (Settings.MSHealthToken == null ||
                    string.IsNullOrEmpty(Settings.MSHealthToken.RefreshToken))
                {
                    // Microsoft Health refresh token is not available, so, set as signed-out
                    IsMSHealthSignedIn = false;
                    Activities = null;
                }
            }
        }

        /// <summary>
        /// Handler for <see cref="Windows.UI.Xaml.Controls.Page.OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs)" />.
        /// </summary>
        /// <param name="parameter"><see cref="Windows.UI.Xaml.Navigation.NavigationEventArgs.Parameter" />.</param>
        public void Deactivate(object parameter)
        {
            // No implementation necessary
        }

        #endregion

    }

}
