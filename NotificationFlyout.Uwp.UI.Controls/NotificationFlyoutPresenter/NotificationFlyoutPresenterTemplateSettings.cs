using Windows.UI.Xaml;

namespace NotificationFlyout.Uwp.UI.Controls
{
    public class NotificationFlyoutPresenterTemplateSettings : DependencyObject
    {
        public static readonly DependencyProperty FromHorizontalOffsetProperty =
            DependencyProperty.Register(nameof(FromHorizontalOffset),
                typeof(double), typeof(NotificationFlyoutPresenterTemplateSettings),
                new PropertyMetadata(0d));

        public static readonly DependencyProperty FromVerticalOffsetProperty =
            DependencyProperty.Register(nameof(FromVerticalOffset),
                typeof(double), typeof(NotificationFlyoutPresenterTemplateSettings),
                new PropertyMetadata(0d));

        public double FromHorizontalOffset
        {
            get => (double)GetValue(FromHorizontalOffsetProperty);
            set => SetValue(FromHorizontalOffsetProperty, value);
        }

        public double FromVerticalOffset
        {
            get => (double)GetValue(FromVerticalOffsetProperty);
            set => SetValue(FromVerticalOffsetProperty, value);
        }
    }
}