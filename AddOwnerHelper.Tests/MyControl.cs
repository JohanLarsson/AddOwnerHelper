namespace AddOwnerHelper.Tests
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public class MyControl : Control
    {
        /// <summary>
        /// DependencyProperty for the Stretch property.
        /// </summary>
        public static readonly DependencyProperty StretchProperty = Shape.StretchProperty.AddOwner(
            typeof(MyControl),
            new FrameworkPropertyMetadata(
                Stretch.None,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Identifies the <see cref="P:MyControl.IsReadOnly" /> dependency property. 
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:MyControl.IsReadOnly" /> dependency property.
        /// </returns>
        public static readonly DependencyProperty IsReadOnlyProperty = TextBoxBase.IsReadOnlyProperty.AddOwner(
            typeof(MyControl),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }
    }
}
