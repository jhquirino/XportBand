//-----------------------------------------------------------------------
// <copyright file="HeartRateZoneToBrushConverter.cs" company="Jorge Alberto Hernández Quirino">
// Copyright (c) Jorge Alberto Hernández Quirino 2014-2016. All rights reserved.
// </copyright>
// <author>Jorge Alberto Hernández Quirino</author>
//-----------------------------------------------------------------------
namespace XportBand.Converters
{
    using System;
    using Windows.UI;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Converter class, Heart Rate Zone to <see cref="SolidColorBrush"/>.
    /// </summary>
    public sealed class HeartRateZoneToBrushConverter : IValueConverter
    {

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
            SolidColorBrush loBrush = new SolidColorBrush(Colors.Transparent);
            Model.HeartRateZoneItem loHRZ = value as Model.HeartRateZoneItem;
            if (loHRZ != null)
            {
                switch(loHRZ.Key)
                {
                    case Model.HeartRateZoneItem.HRZONE_UNDER_HEALTHY:
                        loBrush = new SolidColorBrush(Colors.LightGray);
                        break;
                    case Model.HeartRateZoneItem.HRZONE_HEALTHY:
                        loBrush = new SolidColorBrush(Colors.Green);
                        break;
                    case Model.HeartRateZoneItem.HRZONE_FITNESS:
                        loBrush = new SolidColorBrush(Colors.LimeGreen);
                        break;
                    case Model.HeartRateZoneItem.HRZONE_AEROBIC:
                        loBrush = new SolidColorBrush(Colors.Gold);
                        break;
                    case Model.HeartRateZoneItem.HRZONE_ANAEROBIC:
                        loBrush = new SolidColorBrush(Colors.Orange);
                        break;
                    case Model.HeartRateZoneItem.HRZONE_REDLINE:
                        loBrush = new SolidColorBrush(Colors.Red);
                        break;
                    case Model.HeartRateZoneItem.HRZONE_OVER_REDLINE:
                        loBrush = new SolidColorBrush(Colors.DarkRed);
                        break;
                    default:
                        loBrush = new SolidColorBrush(Colors.Gray);
                        break;
                }
            }
            return loBrush;
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
