using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Cafe_Managment.Utilities
{
    internal class ButtonControl : RadioButton
    {
        static ButtonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonControl),
                new FrameworkPropertyMetadata(typeof(ButtonControl)));
        }
    }
}
