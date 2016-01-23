//-----------------------------------------------------------------------
// <copyright file="MainView.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace XportBand.View
{
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Main Page for XportBand.
    /// </summary>
    public sealed partial class MainView : Page
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView()
        {
            this.InitializeComponent();
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
