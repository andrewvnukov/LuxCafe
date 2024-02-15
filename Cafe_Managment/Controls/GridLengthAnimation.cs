using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;

namespace Cafe_Managment.Controls
{
    public class GridLengthAnimation : AnimationTimeline
    {
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));

        public override Type TargetPropertyType => typeof(GridLength);

        public override object GetCurrentValue(
            object defaultOriginValue, object defaultDestinationValue,
            AnimationClock animationClock)
        {
            double fromVal = ((GridLength)GetValue(FromProperty)).Value;
            double toVal = ((GridLength)GetValue(ToProperty)).Value;

            return new GridLength((1 - animationClock.CurrentProgress.Value) * fromVal +
                animationClock.CurrentProgress.Value * toVal, GridUnitType.Pixel);
        }

        protected override Freezable CreateInstanceCore() => new GridLengthAnimation();

        public GridLength From
        {
            get => (GridLength)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        public GridLength To
        {
            get => (GridLength)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }
    }
}
