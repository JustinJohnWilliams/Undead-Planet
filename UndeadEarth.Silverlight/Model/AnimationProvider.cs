using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace UndeadEarth.Silverlight.Model
{
    public class AnimationProvider
    {
        public Storyboard GetFadeInAnimation(UIElement target)
        {
            Storyboard storyBoard = new Storyboard();
            storyBoard.AutoReverse = false;

            // Create two DoubleAnimations and set their properties.
            DoubleAnimationUsingKeyFrames doubleAnimationOpacity = new DoubleAnimationUsingKeyFrames();
            doubleAnimationOpacity.BeginTime = TimeSpan.FromSeconds(0);
            Storyboard.SetTarget(doubleAnimationOpacity, target);
            Storyboard.SetTargetProperty(doubleAnimationOpacity, new PropertyPath("(UIElement.Opacity)"));
            storyBoard.Children.Add(doubleAnimationOpacity);

            EasingDoubleKeyFrame easingDoubleKeyFrame = new EasingDoubleKeyFrame();
            easingDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.5));
            easingDoubleKeyFrame.Value = 1;
            easingDoubleKeyFrame.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut };
            doubleAnimationOpacity.KeyFrames.Add(easingDoubleKeyFrame);

            return storyBoard;
        }

        public Storyboard GetFadeOutAnimation(UIElement target)
        {
            Storyboard storyBoard = new Storyboard();
            storyBoard.AutoReverse = false;

            // Create two DoubleAnimations and set their properties.
            DoubleAnimationUsingKeyFrames doubleAnimationOpacity = new DoubleAnimationUsingKeyFrames();
            doubleAnimationOpacity.BeginTime = TimeSpan.FromSeconds(0);
            Storyboard.SetTarget(doubleAnimationOpacity, target);
            Storyboard.SetTargetProperty(doubleAnimationOpacity, new PropertyPath("(UIElement.Opacity)"));
            storyBoard.Children.Add(doubleAnimationOpacity);

            EasingDoubleKeyFrame easingDoubleKeyFrame = new EasingDoubleKeyFrame();
            easingDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.5));
            easingDoubleKeyFrame.Value = 0;
            easingDoubleKeyFrame.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut };
            doubleAnimationOpacity.KeyFrames.Add(easingDoubleKeyFrame);

            return storyBoard;
        }
    }
}
