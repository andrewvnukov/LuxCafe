using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using ToastNotifications;
using ToastNotifications.Messages;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace Cafe_Managment.Utilities
{

    public class TextBoxInputValidationBehavior : Behavior<TextBox>
    {
        private Notifier _notifier;

        public TextBoxInputValidationBehavior()
        {
            _notifier = CreateNotifier();

        }
        private Notifier CreateNotifier()
        {
            return new Notifier(cfg =>
            {
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    TimeSpan.FromSeconds(5),
                    MaximumNotificationCount.FromCount(5));

                cfg.PositionProvider = new PrimaryScreenPositionProvider(
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.DisplayOptions.TopMost = true;
                cfg.DisplayOptions.Width = 300;

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        public static readonly DependencyProperty RegexPatternProperty =
        DependencyProperty.Register(nameof(RegexPattern), typeof(string), typeof(TextBoxInputValidationBehavior), new PropertyMetadata(default(string)));

        public string RegexPattern
        {
            get => (string)GetValue(RegexPatternProperty);
            set => SetValue(RegexPatternProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += OnPreviewTextInput;
            DataObject.AddPastingHandler(AssociatedObject, OnPaste);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
            DataObject.RemovePastingHandler(AssociatedObject, OnPaste);
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextValid(e.Text);
            if (e.Handled)
            {
               _notifier.ShowWarning("Недопустимый символ!");
            }
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var pastedText = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextValid(pastedText))
                {
                    e.CancelCommand();
                    _notifier.ShowWarning("Недопустимый символ!");
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private bool IsTextValid(string text)
        {
            return Regex.IsMatch(text, RegexPattern);
        }
    }
}
