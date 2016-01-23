//-----------------------------------------------------------------------
// <copyright file="ViewModelLocator.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace XportBand.ViewModel
{
    using GalaSoft.MvvmLight.Ioc;
    using GalaSoft.MvvmLight.Views;
    using Microsoft.Practices.ServiceLocation;
    using MSHealthAPI;

    /// <summary>
    /// Locator class for all ViewModels.
    /// </summary>
    /// <remarks>
    /// This class contains static references to all the ViewModels in the
    /// application and provides an entry point for the bindings.
    /// </remarks>
    public sealed class ViewModelLocator
    {

        #region Constants

        /// <summary>
        /// Client ID registered for XportBand app.
        /// </summary>
        public const string MSHEALTH_CLIENT_ID = "000000004017E7B0";

        /// <summary>
        /// Client Secret registered for XportBand app.
        /// </summary>
        public const string MSHEALTH_CLIENT_SECRET = "-Z0MoiAX96sx2aEAmhfpDfc4CoaKcxAL";

        /// <summary>
        /// Access types (scopes) required for XportBand app to work with.
        /// </summary>
        public const MSHealthScope MSHEALTH_SCOPE = MSHealthScope.ReadProfile |
                                                    MSHealthScope.ReadActivityHistory |
                                                    MSHealthScope.ReadActivityLocation |
                                                    MSHealthScope.OfflineAccess;

        #endregion

        #region ViewModels references

        /// <summary>
        /// ViewModel for Main view.
        /// </summary>
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        /// <summary>
        /// ViewModel for Settings view.
        /// </summary>
        public SettingsViewModel Settings
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); }
        }

        /// <summary>
        /// ViewModel for Activity Details view.
        /// </summary>
        public ActivityDetailsViewModel ActivityDetails
        {
            get { return ServiceLocator.Current.GetInstance<ActivityDetailsViewModel>(); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelLocator"/> class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            //if (ViewModelBase.IsInDesignModeStatic)
            //{
            //    // Create design time view services and models
            //    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            //}
            //else
            //{
            //    // Create run time view services and models
            //    SimpleIoc.Default.Register<IDataService, DataService>();
            //}
            // Register and configure services
            INavigationService loNavigationService = this.CreateNavigationService();
            SimpleIoc.Default.Register<INavigationService>(() => loNavigationService);
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            IMSHealthClient loMSHealthClient = new MSHealthClient(MSHEALTH_CLIENT_ID, MSHEALTH_CLIENT_SECRET, MSHEALTH_SCOPE);
            SimpleIoc.Default.Register<IMSHealthClient>(() => loMSHealthClient);
            // Register ViewModels
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<ActivityDetailsViewModel>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clears the ViewModels
        /// </summary>
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
            ServiceLocator.Current.GetInstance<MainViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<SettingsViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<ActivityDetailsViewModel>().Cleanup();
        }

        /// <summary>
        /// Creates and configures <see cref="GalaSoft.MvvmLight.Views.INavigationService"/> implementation.
        /// </summary>
        /// <returns>Instance of <see cref="GalaSoft.MvvmLight.Views.INavigationService"/> implementation.</returns>
        private INavigationService CreateNavigationService()
        {
            var loNavigationService = new NavigationService();
            loNavigationService.Configure("Main", typeof(View.MainView));
            loNavigationService.Configure("Settings", typeof(View.SettingsView));
            loNavigationService.Configure("ActivityDetails", typeof(View.ActivityDetailsView));
            return loNavigationService;
        }

        #endregion

    }

}
