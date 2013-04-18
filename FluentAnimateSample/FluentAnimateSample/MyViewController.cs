using System;
using AccidentalFish.UIKit;
using MonoTouch.UIKit;
using System.Drawing;

namespace FluentAnimateSample
{
    public class MyViewController : UIViewController
    {
        UIButton _button;
        private const float ButtonWidth = 200;
        private const float ButtonHeight = 50;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.Frame = UIScreen.MainScreen.Bounds;
            View.BackgroundColor = UIColor.White;
            View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            _button = UIButton.FromType(UIButtonType.RoundedRect);

            _button.Frame = new RectangleF(
                View.Frame.Width / 2 - ButtonWidth / 2,
                View.Frame.Height / 2 - ButtonHeight / 2,
                ButtonWidth,
                ButtonHeight);

            _button.SetTitle("Click me", UIControlState.Normal);

            _button.TouchUpInside += (sender, e) => Animate();

            _button.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin |
                UIViewAutoresizing.FlexibleBottomMargin;

            View.AddSubview(_button);
        }

        private void Animate()
        {
            FluentAnimate
                .EaseIn(0.5, () => _button.Center = new PointF(ButtonWidth/2, ButtonHeight/2))
                .Then.EaseInOut(() => _button.Center = new PointF(ButtonWidth/2, View.Bounds.Height - ButtonHeight/2))
                .Then.EaseOut(() => _button.Center = new PointF(View.Bounds.Width - ButtonWidth / 2, View.Bounds.Height - ButtonHeight / 2))
                .Then.Linear(() => _button.Center = new PointF(View.Bounds.Width - ButtonWidth / 2, ButtonHeight / 2))
                .Then.After(3.0)
                .Repeat()
                .AllowUserInteraction()
                .Start();
        }
    }
}

