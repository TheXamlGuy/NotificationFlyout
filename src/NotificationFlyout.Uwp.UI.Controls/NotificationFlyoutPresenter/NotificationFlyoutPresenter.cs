using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace NotificationFlyout.Uwp.UI.Controls
{
    public class NotificationFlyoutPresenter : ContentControl
    {
        public NotificationFlyoutPresenter()
        {
            DefaultStyleKey = typeof(NotificationFlyoutPresenter);
        }

        protected override void OnApplyTemplate()
        {
            if (GetTemplateChild("ContentPresenter") is ContentControl contentPresenter)
            {
                BindingOperations.SetBinding(this, RequestedThemeProperty, new Binding
                {
                    Source = contentPresenter.Content,
                    Path = new PropertyPath(nameof(RequestedTheme)),
                    Mode = BindingMode.TwoWay
                });
            }
        }
    }
}