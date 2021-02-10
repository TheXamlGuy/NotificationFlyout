using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace NotificationFlyout.Uwp.UI.Controls
{
    internal class ContextMenuFlyoutHost : Control
    {
        private Grid _root;

        public ContextMenuFlyoutHost()
        {
            DefaultStyleKey = typeof(ContextMenuFlyoutHost);
        }

        public void ShowFlyout()
        {
            if (_root == null) return;
            var flyout = FlyoutBase.GetAttachedFlyout(_root);
            flyout.ShowAt(_root, new FlyoutShowOptions { Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft, ShowMode = FlyoutShowMode.TransientWithDismissOnPointerMoveAway });
        }

        public void HideFlyout()
        {
            if (_root == null) return;
            var flyout = FlyoutBase.GetAttachedFlyout(_root);
            flyout.Hide();
        }

        protected override void OnApplyTemplate()
        {
            _root = GetTemplateChild("Root") as Grid;
        }
    }
}