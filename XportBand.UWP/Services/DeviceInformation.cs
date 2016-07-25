namespace XportBand.Services
{
    using Windows.Graphics.Display;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using static Windows.ApplicationModel.Resources.Core.ResourceContext;

    #region Enums

    /// <summary>
    /// Available device families.
    /// </summary>
    public enum DeviceFamily
    {
        Unknown,
        Desktop,
        Mobile,
        SurfaceHub,
        IoT,
        Xbox,
    }

    public enum DeviceOrientation
    {
        Portrait,
        Landscape,
    }

    public class DeviceInformation
    {
        public static DeviceOrientation Orientation =>
            DisplayInformation.GetForCurrentView().CurrentOrientation.ToString().Contains("Landscape") 
            ? DeviceOrientation.Landscape : DeviceOrientation.Portrait;

        public static DeviceFamily Family =>
             GetForCurrentView().QualifierValues["DeviceFamily"] == "Mobile" 
            ? DeviceFamily.Mobile : DeviceFamily.Desktop;

        public static DisplayInformation DisplayInformation =>
            DisplayInformation.GetForCurrentView();

        public static Frame DisplayFrame =>
                Window.Current.Content == null ? null : Window.Current.Content as Frame;

        public static string DeviceFamilyName => GetForCurrentView().QualifierValues["DeviceFamily"];

    }

    #endregion

}
