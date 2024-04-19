using Cafe_Managment.ViewModel;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для Order.xaml
    /// </summary>
    public partial class Order : UserControl
    {
        public Order()
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
                var viewModel = DataContext as OrderVM;
                if (viewModel != null)
                {
                    if (viewModel.SwitchToCategoryCommand.CanExecute(item))
                    {
                        viewModel.SwitchToCategoryCommand.Execute(item);
                    }
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
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
                var viewModel = DataContext as OrderVM;
                if (viewModel != null)
                {
                    if (viewModel.SwitchToCategoryCommand.CanExecute(item))
                    {
                        viewModel.AddDishToOrderCommand.Execute(item);
                    }
                }
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true; // Если не является числом или точкой, отменяем ввод
            }
            else
            {
                e.Handled = false;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
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
                var viewModel = DataContext as OrderVM;
                if (viewModel != null)
                {
                        viewModel.AddDishToOrderCommand.Execute(item);
                }
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var item = button.DataContext;
                // Выполнение команды
                var viewModel = DataContext as OrderVM;
                if (viewModel != null)
                {
                    viewModel.DescreaseDishCommand.Execute(item);
                }
            }
        }
    }
}
