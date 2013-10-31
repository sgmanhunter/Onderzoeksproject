using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace TwitCrunch.Behaviors
{
    public class WindowMinimizeBehavior : Behavior<Button>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Click += AssociatedObjectOnClick;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Click -= AssociatedObjectOnClick;
            base.OnDetaching();
        }

        private void AssociatedObjectOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Button b = sender as Button;
            Window w = Window.GetWindow(b.Parent);
            w.WindowState = WindowState.Minimized;
        }
    }
}
