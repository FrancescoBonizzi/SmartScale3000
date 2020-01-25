using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace SmartWeightDevice.Extensions
{
    public static class XamlExtensions
    {
        public static void Animate(
            this DependencyObject target,
            double? from,
            double to,
            string propertyPath,
            int duration = 400,
            int startTime = 0,
            EasingFunctionBase easing = null,
            Action completed = null)
        {
            if (easing == null)
            {
                easing = new ExponentialEase();
            }

            var db = new DoubleAnimation
            {
                To = to,
                From = from,
                EasingFunction = easing,
                Duration = TimeSpan.FromMilliseconds(duration)
            };
            Storyboard.SetTarget(db, target);
            Storyboard.SetTargetProperty(db, new PropertyPath(propertyPath));

            var sb = new Storyboard
            {
                BeginTime = TimeSpan.FromMilliseconds(startTime)
            };

            if (completed != null)
            {
                sb.Completed += (s, e) =>
                {
                    completed();
                };
            }

            sb.Children.Add(db);
            sb.Begin();
        }

    }
}

