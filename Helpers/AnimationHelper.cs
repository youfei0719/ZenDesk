using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace ZenDesk.Helpers
{
    public static class AnimationHelper
    {
        // 阻尼缓动，符合 Windows 11 物理直觉
        public static IEasingFunction GetDampedEasingFunction()
        {
            return new CubicEase { EasingMode = EasingMode.EaseOut };
        }
        
        // 弹性缓动，适合弹窗或图标吸附
        public static IEasingFunction GetSpringEasingFunction()
        {
            return new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations = 1, Springiness = 5 };
        }

        // 淡入动画
        public static void FadeIn(UIElement element, double durationMs = 250)
        {
            var anim = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(durationMs),
                EasingFunction = GetDampedEasingFunction()
            };
            element.Visibility = Visibility.Visible;
            element.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        // 淡出动画
        public static void FadeOut(UIElement element, double durationMs = 200, Action onCompleted = null)
        {
            var anim = new DoubleAnimation
            {
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(durationMs),
                EasingFunction = GetDampedEasingFunction()
            };
            
            if (onCompleted != null)
            {
                anim.Completed += (s, e) =>
                {
                    element.Visibility = Visibility.Collapsed;
                    onCompleted();
                };
            }
            else
            {
                anim.Completed += (s, e) => element.Visibility = Visibility.Collapsed;
            }
            
            element.BeginAnimation(UIElement.OpacityProperty, anim);
        }
        
        // 垂直滑入 + 淡入 (类似 Windows 飞出菜单)
        public static void SlideAndFadeIn(UIElement element, double offsetDistance = 20, double durationMs = 300)
        {
            element.RenderTransform = new System.Windows.Media.TranslateTransform(0, offsetDistance);
            
            var slideAnim = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(durationMs),
                EasingFunction = GetDampedEasingFunction()
            };
            
            var fadeAnim = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(durationMs),
                EasingFunction = GetDampedEasingFunction()
            };

            element.Visibility = Visibility.Visible;
            element.RenderTransform.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, slideAnim);
            element.BeginAnimation(UIElement.OpacityProperty, fadeAnim);
        }
    }
}
