namespace XportBand.StateTriggers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.UI.Xaml;
    using Services;
    using static Windows.ApplicationModel.DesignMode;
    using Windows.Graphics.Display;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// The device trigger.
    /// </summary>
    public class DeviceTrigger : StateTriggerBase
    {

        #region Familiy
        public DeviceFamily Family
        {
            get { return (DeviceFamily)GetValue(FamilyProperty); }
            set { SetValue(FamilyProperty, value); }
        }
        public static readonly DependencyProperty FamilyProperty =
            DependencyProperty.Register(nameof(Family), typeof(DeviceFamily), typeof(DeviceTrigger),
                new PropertyMetadata(DeviceFamily.Desktop));
        #endregion

        #region Orientation
        public DeviceOrientation Orientation
        {
            get { return (DeviceOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(DeviceOrientation), typeof(DeviceTrigger),
                new PropertyMetadata(DeviceOrientation.Portrait));
        #endregion

        public DeviceTrigger()
        {
            Initialize();
        }

        /// <summary>
        /// If we call SetTrigger in the Constructor, it won't affect to the UI, to make
        /// a call for the first time in the best place, it is in the Navigated Event of the Frame
        /// </summary>
        private void Initialize()
        {
            if (!DesignModeEnabled)
            {
                //Initial Trigger 
                NavigatedEventHandler framenavigated = null;
                framenavigated = (s, e) =>
                {
                    DeviceInformation.DisplayFrame.Navigated -= framenavigated;
                    SetTrigger();
                };
                DeviceInformation.DisplayFrame.Navigated += framenavigated;

                //Orientation Trigger
                DeviceInformation.DisplayInformation.OrientationChanged += (s, e) => SetTrigger();
            }
        }

        private void SetTrigger()
        {
            //SetValue(FamilyProperty, DeviceInformation.Family);
            //SetValue(OrientationProperty, DeviceInformation.Orientation);
            SetActive(Orientation == DeviceInformation.Orientation && Family == DeviceInformation.Family);
        }

    }

}