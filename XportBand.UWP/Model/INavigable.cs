//-----------------------------------------------------------------------
// <copyright file="INavigable.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2015-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace XportBand.Model
{

    /// <summary>
    /// Interface to define Navigable View Models.
    /// </summary>
    public interface INavigable
    {

        #region Methods

        /// <summary>
        /// Handler for <see cref="Windows.UI.Xaml.Controls.Page.OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs)"/>.
        /// </summary>
        /// <param name="parameter"><see cref="Windows.UI.Xaml.Navigation.NavigationEventArgs.Parameter"/>.</param>
        void Activate(object parameter);

        /// <summary>
        /// Handler for <see cref="Windows.UI.Xaml.Controls.Page.OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs)"/>.
        /// </summary>
        /// <param name="parameter"><see cref="Windows.UI.Xaml.Navigation.NavigationEventArgs.Parameter"/>.</param>
        void Deactivate(object parameter);

        #endregion

    }

}
