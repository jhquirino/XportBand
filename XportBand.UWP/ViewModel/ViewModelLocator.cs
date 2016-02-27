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
    using NikePlusAPI;
#if WINDOWS_UWP
    using DialogService = GalaSoft.MvvmLight.Views.DialogService;
    using NavigationService = GalaSoft.MvvmLight.Views.NavigationService;
#elif DESKTOP_APP
	using DialogService = XportBand.Services.DialogService;
	using NavigationService = XportBand.Services.NavigationService;
#endif

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

        /// <summary>
        /// HACK: ID for registered Nike+ App.
        /// </summary>
        public const string NIKEPLUS_APP = "NIKEPLUSGPS";

        /// <summary>
        /// HACK: Client ID for registered Nike+ App.
        /// </summary>
        public const string NIKEPLUS_CLIENT_ID = "9dfa1aef96a54441dfaac68c4410e063";

        /// <summary>
        /// HACK: Client Secret for registered Nike+ App.
        /// </summary>
        public const string NIKEPLUS_CLIENT_SECRET = "3cbd1f1908bc1553";

        #endregion

        #region ViewModels references

        /// <summary>
        /// ViewModel for Main view.
        /// </summary>
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

#if WINDOWS_UWP
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
#endif

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
            INikePlusClient loNikePlusClient = new NikePlusClient(NIKEPLUS_APP, NIKEPLUS_CLIENT_ID, NIKEPLUS_CLIENT_SECRET);
            SimpleIoc.Default.Register<INikePlusClient>(() => loNikePlusClient);
            // Register ViewModels
            SimpleIoc.Default.Register<MainViewModel>();
#if WINDOWS_UWP
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<ActivityDetailsViewModel>();
#endif
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
#if WINDOWS_UWP
            ServiceLocator.Current.GetInstance<SettingsViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<ActivityDetailsViewModel>().Cleanup();
#endif
        }

        /// <summary>
        /// Creates and configures <see cref="GalaSoft.MvvmLight.Views.INavigationService"/> implementation.
        /// </summary>
        /// <returns>Instance of <see cref="GalaSoft.MvvmLight.Views.INavigationService"/> implementation.</returns>
        private INavigationService CreateNavigationService()
        {
            var loNavigationService = new NavigationService();
#if WINDOWS_UWP
            loNavigationService.Configure("Main", typeof(View.MainView));
            loNavigationService.Configure("Settings", typeof(View.SettingsView));
            loNavigationService.Configure("ActivityDetails", typeof(View.ActivityDetailsView));
#endif
            return loNavigationService;
        }

        #endregion

    }

}
