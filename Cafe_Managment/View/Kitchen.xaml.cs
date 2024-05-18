using Cafe_Managment.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cafe_Managment.View
{
    /// <summary>
    /// Логика взаимодействия для Kitchen.xaml
    /// </summary>
    public partial class Kitchen : UserControl
    {
        public Kitchen()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var item = button.DataContext;
                var listBox = FindParent<ListBox>(button);
                if (listBox != null)
                {
                    listBox.SelectedItem = item;
                    listBox.SelectedIndex = listBox.Items.IndexOf(item);
                }
                // Выполнение команды
                var viewModel = DataContext as KitchenVM;
                if (viewModel != null)
                {
                    viewModel.ChangeDishStatusCommand.Execute(listBox.SelectedIndex);
                }
            }
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            T parent = parentObject as T;
            return parent ?? FindParent<T>(parentObject);
        }

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = sender as Border;
            if (button != null)
            {
                var item = button.DataContext;

                var listBox = FindParent<ListBox>(button);
                if (listBox != null)
                {
                    listBox.SelectedItem = item;
                    listBox.SelectedIndex = listBox.Items.IndexOf(item);
                }
            }
        }
    }
}
