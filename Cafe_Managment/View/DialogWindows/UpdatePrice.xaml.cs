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
using System.Windows.Shapes;

namespace Cafe_Managment.View.DialogWindows
{
    /// <summary>
    /// Логика взаимодействия для UpdatePrice.xaml
    /// </summary>
    public partial class UpdatePrice : Window
    {
        public UpdatePrice()
        {
            InitializeComponent();
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, является ли вводимый символ числом или точкой
            if (!char.IsDigit(e.Text, 0) && e.Text != ".")
            {
                e.Handled = true; // Если не является числом или точкой, отменяем ввод
            }
            else
            {
                // Если введена точка, проверяем, есть ли уже точка в текстовом поле
                if (e.Text == "." && ((TextBox)sender).Text.Contains("."))
                {
                    e.Handled = true; // Если точка уже есть, отменяем ввод
                }
            }
        }
        public string GetInput()
        {
            return newPrice.Text;
        }

        private void Button_CloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
