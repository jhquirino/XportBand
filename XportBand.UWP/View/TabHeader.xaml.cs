namespace XportBand.View
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Custom Header (glyph and label) for <see cref="Pivot"/> control.
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.UserControl" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    public sealed partial class TabHeader : UserControl
    {

        #region DependencyProperties

        /// <summary>
        /// <see cref="Glyph"/> property.
        /// </summary>
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(TabHeader), null);

        /// <summary>
        /// <see cref="Label"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(TabHeader), null);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the glyph.
        /// </summary>
        public string Glyph
        {
            get { return GetValue(GlyphProperty) as string; }
            set { SetValue(GlyphProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label
        {
            get { return GetValue(LabelProperty) as string; }
            set { SetValue(LabelProperty, value); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TabHeader"/> class.
        /// </summary>
        public TabHeader()
        {
            InitializeComponent();
            DataContext = this;
        }

        #endregion

    }

}
