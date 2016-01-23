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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Xml;
    using Windows.Devices.Geolocation;
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.UI.Core;

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
        /// Inner member for <see cref="IsRunningRequest"/> property.
        /// </summary>
        private bool mbIsRunningRequest = false;

        /// <summary>
        /// The mo activity
        /// </summary>
        private MSHealthActivity moActivity = null;

        /// <summary>
        /// The mi total distance
        /// </summary>
        private decimal? miTotalDistance = null;

        /// <summary>
        /// The MTS pace
        /// </summary>
        private TimeSpan? mtsPace = null;

        /// <summary>
        /// The mo heart rate zones
        /// </summary>
        private ObservableCollection<HeartRateZoneItem> moHeartRateZones;

        /// <summary>
        /// The mo map path
        /// </summary>
        private Geopath moMapPath;

        #endregion

        #region Properties

        public bool IsRunningRequest
        {
            get { return mbIsRunningRequest; }
            set { Set<bool>(() => IsRunningRequest, ref mbIsRunningRequest, value); }
        }

        public MSHealthActivity Activity
        {
            get { return moActivity; }
            set { Set<MSHealthActivity>(() => Activity, ref moActivity, value); }
        }

        public decimal? TotalDistance
        {
            get { return miTotalDistance; }
            set { Set<decimal?>(() => TotalDistance, ref miTotalDistance, value); }
        }

        public TimeSpan? Pace
        {
            get { return mtsPace; }
            set { Set<TimeSpan?>(() => Pace, ref mtsPace, value); }
        }

        public ObservableCollection<HeartRateZoneItem> HeartRateZones
        {
            get { return moHeartRateZones; }
            set { Set<ObservableCollection<HeartRateZoneItem>>(() => HeartRateZones, ref moHeartRateZones, value); }
        }

        public Geopath MapPath
        {
            get { return moMapPath; }
            set { Set<Geopath>(() => MapPath, ref moMapPath, value); }
        }

        public ICommand ExportToGpxCommand { get; private set; }

        #endregion

        #region Constructors

        public ActivityDetailsViewModel(IDialogService dialogService, INavigationService navigationService, IMSHealthClient msHealthClient)
        {
            moDialogService = dialogService;
            moNavigationService = navigationService;
            moMSHealthClient = msHealthClient;
            ExportToGpxCommand = new RelayCommand(ExportToGpx, () => true);
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

        private async void ExportToGpx()
        {
            if (Activity != null)
            {
                var loGPX = Activity.ToGPX("XportBand");
                if (loGPX != null)
                {
                    string lsGPX = null;
                    StringBuilder loStringBuilder = new StringBuilder();
                    using (TextWriter loTextWriter = new EncodingStringWriter(loStringBuilder, new UTF8Encoding(false)))// Encoding.UTF8))
                    {
                        loGPX.Save(loTextWriter);
                        lsGPX = loStringBuilder.ToString();
                    }

                    FileSavePicker loFileSavePicker = new FileSavePicker();
                    loFileSavePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    loFileSavePicker.FileTypeChoices.Add("GPX", new List<string>() { ".gpx" });
                    loFileSavePicker.SuggestedFileName = string.Format("XportBand{0}{1:yyyyMMddHHmmss}", Activity.Type, Activity.StartTime);
                    StorageFile loStorageFile = await loFileSavePicker.PickSaveFileAsync();
                    if (loStorageFile != null)
                    {
                        CachedFileManager.DeferUpdates(loStorageFile);
                        await FileIO.WriteTextAsync(loStorageFile, lsGPX);
                        Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(loStorageFile);
                        if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                        {
                            //this.textBlock.Text = "File " + file.Name + " was saved.";
                            await moDialogService.ShowMessage(string.Format("activity exported to: {0}", loStorageFile.Name), "export gpx");
                        }
                        else
                        {
                            //this.textBlock.Text = "File " + file.Name + " couldn't be saved.";
                            await moDialogService.ShowError(string.Format("activity couldn't be exported to: {0}", loStorageFile.Name), "export gpx", "ok", null);
                        }
                    }
                    else
                    {
                        await moDialogService.ShowMessage("operation cancelled.", "export gpx");
                    }
                }
            }
        }

        #endregion

        #region INavigable implementation

        public async void Activate(object parameter)
        {
            string lsActivityID = parameter as string;
            MSHealthActivities loActivities = null;
            MSHealthSplitDistanceType loDistanceType = MSHealthSplitDistanceType.None;
            MSHealthActivityInclude loInclude = MSHealthActivityInclude.Details | MSHealthActivityInclude.MapPoints | MSHealthActivityInclude.MinuteSummaries;
            Activity = null;
            TotalDistance = null;
            Pace = null;
            HeartRateZones = null;
            List<HeartRateZoneItem> loHeartRateZones = null;
            MapPath = null;
            
            // Set back button visible (for Windows app)
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            if (!string.IsNullOrEmpty(lsActivityID))
            {
                IsRunningRequest = true;
                switch (Settings.MSHealthFilterDistance)
                {
                    case DistanceUnit.DISTANCE_MILE:
                        loDistanceType = MSHealthSplitDistanceType.Mile;
                        break;
                    case DistanceUnit.DISTANCE_KILOMETER:
                        loDistanceType = MSHealthSplitDistanceType.Kilometer;
                        break;
                }
                try
                {
                    loActivities = await moMSHealthClient.ListActivities(ids: lsActivityID,
                                                                         include: loInclude,
                                                                         splitDistanceType: loDistanceType);
                    //Activity = await moMSHealthClient.ReadActivity(lsActivityID, loInclude, loDistanceType);
                    if (loActivities.ItemCount > 0)
                    {
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
                        // Heart Rate Zones
                        if (Activity.PerformanceSummary != null &&
                            Activity.PerformanceSummary.HeartRateZones != null)
                        {
                            loHeartRateZones = new List<HeartRateZoneItem>();
                            loHeartRateZones.Add(new HeartRateZoneItem()
                            {
                                Key = HeartRateZoneItem.HRZONE_UNDER_HEALTHY,
                                Name = Resources.Strings.TextHeartRateZoneUnderHealthyText,
                                Value = Activity.PerformanceSummary.HeartRateZones.UnderHealthyHeart ?? 0,
                            });
                            loHeartRateZones.Add(new HeartRateZoneItem()
                            {
                                Key = HeartRateZoneItem.HRZONE_HEALTHY,
                                Name = Resources.Strings.TextHeartRateZoneHealthyText,
                                Value = Activity.PerformanceSummary.HeartRateZones.HealthyHeart ?? 0,
                            });
                            loHeartRateZones.Add(new HeartRateZoneItem()
                            {
                                Key = HeartRateZoneItem.HRZONE_FITNESS,
                                Name = Resources.Strings.TextHeartRateZoneFitnessText,
                                Value = Activity.PerformanceSummary.HeartRateZones.FitnessZone ?? 0,
                            });
                            loHeartRateZones.Add(new HeartRateZoneItem()
                            {
                                Key = HeartRateZoneItem.HRZONE_AEROBIC,
                                Name = Resources.Strings.TextHeartRateZoneAerobicText,
                                Value = Activity.PerformanceSummary.HeartRateZones.Aerobic ?? 0,
                            });
                            loHeartRateZones.Add(new HeartRateZoneItem()
                            {
                                Key = HeartRateZoneItem.HRZONE_ANAEROBIC,
                                Name = Resources.Strings.TextHeartRateZoneAnaerobicText,
                                Value = Activity.PerformanceSummary.HeartRateZones.Anaerobic ?? 0,
                            });
                            loHeartRateZones.Add(new HeartRateZoneItem()
                            {
                                Key = HeartRateZoneItem.HRZONE_REDLINE,
                                Name = Resources.Strings.TextHeartRateZoneRedlineText,
                                Value = Activity.PerformanceSummary.HeartRateZones.Redline ?? 0,
                            });
                            loHeartRateZones.Add(new HeartRateZoneItem()
                            {
                                Key = HeartRateZoneItem.HRZONE_OVER_REDLINE,
                                Name = Resources.Strings.TextHeartRateZoneOverRedlineText,
                                Value = Activity.PerformanceSummary.HeartRateZones.OverRedline ?? 0,
                            });
                            HeartRateZones = new ObservableCollection<HeartRateZoneItem>(loHeartRateZones);
                        }
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
                    //throw;
                }
                finally
                {
                    Messenger.Default.Send<Geopath>(MapPath);
                    IsRunningRequest = false;
                }
            }
        }

        public void Deactivate(object parameter)
        {
            //throw new NotImplementedException();
        }

        #endregion

    }

    public class EncodingStringWriter : StringWriter
    {
        private readonly Encoding _encoding;

        public EncodingStringWriter(StringBuilder sb, Encoding encoding) : base(sb)
        {
            _encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return _encoding; }
        }
    }

}
