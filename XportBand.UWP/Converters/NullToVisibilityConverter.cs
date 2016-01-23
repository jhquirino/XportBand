//-----------------------------------------------------------------------
// <copyright file="NullToVisibilityConverter.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2014-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace XportBand.Converters
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Converter class, validates <see cref="object"/> to <see cref="Windows.UI.Xaml.Visibility"/>.
    /// </summary>
    public sealed class NullToVisibilityConverter : IValueConverter
    {

        #region Properties

        /// <summary>
        /// If set to <see langword="true"/>, conversion is reversed: <see langword="true"/> become <see cref="Windows.UI.Xaml.Visibility.Collapsed"/>.
        /// </summary>
        public bool IsReversed { get; set; }

        #endregion

        #region IValueConverter implementation

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Visibility loVisibility = Visibility.Collapsed;
            bool lbValue = value != null;
            if (this.IsReversed) lbValue = !lbValue;
            if (lbValue) loVisibility = Visibility.Visible;
            return loVisibility;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}
