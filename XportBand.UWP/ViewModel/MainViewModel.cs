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
    using System.Windows.Input;
    using Windows.UI.Core;

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

        private readonly IMSHealthClient moMSHealthClient;

        private bool mbIsRunningRequest = false;

        private bool mbIsMSHealthSignedIn = false;

        private ObservableCollection<MSHealthActivity> moActivities;

        private MSHealthActivity moSelectedActivity;

        #endregion

        #region Properties

        public bool IsRunningRequest
        {
            get { return mbIsRunningRequest; }
            set { Set<bool>(() => IsRunningRequest, ref mbIsRunningRequest, value); }
        }

        public bool IsMSHealthSignedIn
        {
            get { return mbIsMSHealthSignedIn; }
            set { Set<bool>(() => IsMSHealthSignedIn, ref mbIsMSHealthSignedIn, value); }
        }

        public bool FilterActivityExercise
        {
            get { return Settings.MSHealthFilterActivityExercise; }
            set { Settings.MSHealthFilterActivityExercise = value; RaisePropertyChanged<bool>(() => FilterActivityExercise); }
        }

        public bool FilterActivityRun
        {
            get { return Settings.MSHealthFilterActivityRun; }
            set { Settings.MSHealthFilterActivityRun = value; RaisePropertyChanged<bool>(() => FilterActivityRun); }
        }

        public bool FilterActivityBike
        {
            get { return Settings.MSHealthFilterActivityBike; }
            set { Settings.MSHealthFilterActivityBike = value; RaisePropertyChanged<bool>(() => FilterActivityBike); }
        }

        public bool FilterActivityGolf
        {
            get { return Settings.MSHealthFilterActivityGolf; }
            set { Settings.MSHealthFilterActivityGolf = value; RaisePropertyChanged<bool>(() => FilterActivityGolf); }
        }

        public bool FilterActivitySleep
        {
            get { return Settings.MSHealthFilterActivitySleep; }
            set { Settings.MSHealthFilterActivitySleep = value; RaisePropertyChanged<bool>(() => FilterActivitySleep); }
        }

        public bool FilterActivityGuided
        {
            get { return Settings.MSHealthFilterActivityGuided; }
            set { Settings.MSHealthFilterActivityGuided = value; RaisePropertyChanged<bool>(() => FilterActivityGuided); }
        }

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

        public ObservableCollection<MSHealthActivity> Activities
        {
            get { return moActivities; }
            set { Set<ObservableCollection<MSHealthActivity>>(() => Activities, ref moActivities, value); }
        }

        public MSHealthActivity SelectedActivity
        {
            get { return moSelectedActivity; }
            set
            {
                Set<MSHealthActivity>(() => SelectedActivity, ref moSelectedActivity, value);
                ((RelayCommand)ShowActivityDetailsCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand GoToSettingsCommand { get; private set; }

        public ICommand ListActivitiesCommand { get; private set; }

        public ICommand ShowActivityDetailsCommand { get; set; }

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
            //SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) =>
            //{
            //    e.Handled = true;
            //    App.Current.Exit();
            //};
        }

        #endregion

        #region Methods

        private void GoToSettings()
        {
            moNavigationService.NavigateTo("Settings");
        }

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
                bool lbFilterActivityType = FilterActivityExercise | FilterActivityRun | FilterActivityBike |
                                            FilterActivityGolf | FilterActivitySleep | FilterActivityGuided;
                if (!lbFilterActivityType)
                {
                    await moDialogService.ShowMessage("select at least one activity type to filter.", "filter");
                    return;
                }
                if (FilterActivityExercise)
                    loActivityType |= MSHealthActivityType.FreePlay;
                //loActivityType |= MSHealthActivityType.CustomExercise;
                //loActivityType |= MSHealthActivityType.RegularExercise;
                //loActivityType |= MSHealthActivityType.CustomComposite;
                //loActivityType |= MSHealthActivityType.Custom;
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

                switch (Settings.MSHealthFilterPeriod)
                {
                    case Settings.PERIOD_DAY:
                        ldtEnd = DateTime.Now;
                        ldtStart = ldtEnd.Value.AddDays(-1);
                        break;
                    case Settings.PERIOD_WEEK:
                        ldtEnd = DateTime.Now;
                        ldtStart = ldtEnd.Value.AddDays(-7);
                        break;
                    case Settings.PERIOD_MONTH:
                        ldtEnd = DateTime.Now;
                        ldtStart = ldtEnd.Value.AddMonths(-1);
                        break;
                    case Settings.PERIOD_YEAR:
                        ldtEnd = DateTime.Now;
                        ldtStart = ldtEnd.Value.AddYears(-1);
                        break;
                }

                switch (Settings.MSHealthFilterDistance)
                {
                    case DistanceUnit.DISTANCE_MILE:
                        loDistance = MSHealthSplitDistanceType.Mile;
                        break;
                    case DistanceUnit.DISTANCE_KILOMETER:
                        loDistance = MSHealthSplitDistanceType.Kilometer;
                        break;
                }

                MSHealthActivities loActivities = await moMSHealthClient.ListActivities(startTime: ldtStart,
                                                                                        endTime: ldtEnd,
                                                                                        type: loActivityType,
                                                                                        splitDistanceType: loDistance);
                //include: MSHealthActivityInclude.Details | MSHealthActivityInclude.MapPoints);
                Settings.MSHealthAccessToken = moMSHealthClient.Token.AccessToken;
                Settings.MSHealthRefreshToken = moMSHealthClient.Token.RefreshToken;
                Settings.MSHealthExpiresIn = moMSHealthClient.Token.ExpiresIn;
                Settings.MSHealthTokenExpirationTime = moMSHealthClient.Token.ExpirationTime.Ticks;
                Settings.MSHealthTokenCreationTime = moMSHealthClient.Token.CreationTime.Ticks;

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
                loActivitiesList = loActivitiesList.OrderByDescending(loAct => loAct.StartTime).ToList();
                Activities = new ObservableCollection<MSHealthActivity>(loActivitiesList);

                var loSummaries = await moMSHealthClient.ListDailySummaries(startTime: ldtStart, endTime: ldtEnd);
                loSummaries = await moMSHealthClient.ListHourlySummaries(startTime: ldtStart, endTime: ldtEnd);
                if (loSummaries != null)
                {

                }
            }
            catch (MSHealthRequestException loRequestException)
            {
                if (loRequestException.ErrorResponse != null &&
                    loRequestException.ErrorResponse.Error != null)
                {
                    await moDialogService.ShowError(loRequestException.ErrorResponse.Error.Message, "error", "ok", null);
                }
                else
                {
                    await moDialogService.ShowError(loRequestException, "error", "ok", null);
                }

            }
            catch (Exception loException)
            {
                await moDialogService.ShowError(loException, "error", "ok", null);
            }
            finally
            {
                IsRunningRequest = false;
            }
        }

        private void ShowActivityDetails()
        {
            moNavigationService.NavigateTo("ActivityDetails", SelectedActivity.ID);
        }

        private bool CanShowActivityDetails()
        {
            return SelectedActivity != null;
        }

        #endregion

        #region INavigable implementation

        public async void Activate(object parameter)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            if (!string.IsNullOrEmpty(Settings.MSHealthRefreshToken) &&
                !IsMSHealthSignedIn)
            {
                try
                {
                    IsRunningRequest = true;
                    MSHealthToken loSettingsToken = new MSHealthToken()
                    {
                        AccessToken = Settings.MSHealthAccessToken,
                        RefreshToken = Settings.MSHealthRefreshToken,
                        ExpiresIn = Settings.MSHealthExpiresIn,
                        CreationTime = new DateTime(Settings.MSHealthTokenCreationTime),
                    };
                    if (await moMSHealthClient.ValidateToken(loSettingsToken))
                    //if (await moMSHealthClient.RefreshToken(Settings.MSHealthRefreshToken))
                    {
                        //MSHealthProfile = await moMSHealthClient.ReadProfile();
                        Settings.MSHealthAccessToken = moMSHealthClient.Token.AccessToken;
                        Settings.MSHealthRefreshToken = moMSHealthClient.Token.RefreshToken;
                        Settings.MSHealthExpiresIn = moMSHealthClient.Token.ExpiresIn;
                        Settings.MSHealthTokenExpirationTime = moMSHealthClient.Token.ExpirationTime.Ticks;
                        Settings.MSHealthTokenCreationTime = moMSHealthClient.Token.CreationTime.Ticks;
                        IsMSHealthSignedIn = true;
                    }
                    else
                    {
                        IsMSHealthSignedIn = false;
                    }
                }
                catch { IsMSHealthSignedIn = false; }
                finally { IsRunningRequest = false; }
            }
            else
            {
                if (string.IsNullOrEmpty(Settings.MSHealthRefreshToken))
                {
                    IsMSHealthSignedIn = false;
                    this.Activities = null;
                }
            }
        }

        public void Deactivate(object parameter)
        {
            //throw new NotImplementedException();
        }

        #endregion

    }

}
