//-----------------------------------------------------------------------
// <copyright file="SettingsView.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace XportBand.View
{
    using GalaSoft.MvvmLight.Messaging;
    using System;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Settings Page for XportBand.
    /// </summary>
    public sealed partial class SettingsView : Page
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsView"/> class.
        /// </summary>
        public SettingsView()
        {
            this.InitializeComponent();
            // Register MVVM Message (handles URI navigation)
            Messenger.Default.Register<Uri>(this, (uri) => HandleNavigateUri(uri));
        }

        #endregion

        #region MVVM Messaging Handlers

        /// <summary>
        /// Handles MVVM Message for URI navigation.
        /// </summary>
        /// <param name="uri">The URI.</param>
        private void HandleNavigateUri(Uri uri)
        {
            // Navigate to URI on WebView
            wvwSignIn.Navigate(uri);
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
