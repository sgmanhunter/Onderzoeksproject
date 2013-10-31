using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace TwitCrunch.Behaviors
{
    public class WindowDragBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.MouseDown += AssociatedObjectMouseDown;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseDown -= AssociatedObjectMouseDown;
            base.OnDetaching();
        }

        private void AssociatedObjectMouseDown(object sender, RoutedEventArgs routedEventArgs)
        {
            FrameworkElement f = sender as FrameworkElement;
            Window w = Window.GetWindow(f.Parent);
            w.DragMove();
        }
    }
}
