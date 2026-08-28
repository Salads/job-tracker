using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace MainApp
{
    // Adorners must subclass the abstract base class Adorner.
    public class LocationAdorner : Adorner
    {
        // Be sure to call the base class constructor.
        public LocationAdorner(UIElement adornedElement)
          : base(adornedElement)
        {
            IsHitTestVisible = false;
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(LocationAdorner),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Brush TextColor { get; set; } = Brushes.Black;
        public double FontSize { get; set; } = 11.0;
        public string TypeFace { get; set; } = "Verdana";
        public TextTrimming Trimming { get; set; } = TextTrimming.None;

        // A common way to implement an adorner's rendering behavior is to override the OnRender
        // method, which is called by the layout system as part of a rendering pass.
        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            int topMargin = 2;

            FormattedText formattedText = new FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(TypeFace),
                FontSize,
                TextColor,
                VisualTreeHelper.GetDpi(this).PixelsPerDip
                );

            formattedText.MaxTextWidth = AdornedElement.RenderSize.Width;
            formattedText.MaxLineCount = 1;
            formattedText.Trimming = Trimming;

            drawingContext.DrawText(formattedText, new Point(0, adornedElementRect.Height + topMargin));
        }
    }
}
