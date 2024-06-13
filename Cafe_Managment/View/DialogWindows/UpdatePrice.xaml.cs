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
            TextBox textBox = (TextBox)sender;

            // Проверяем, является ли вводимый символ числом или точкой
            if (!char.IsDigit(e.Text, 0) && e.Text != "." && e.Text != ",")
            {
                e.Handled = true; // Если не является числом или точкой, отменяем ввод
            }
            else if (e.Text == "." || e.Text == ",")
            {
                // Если введена точка, показываем сообщение об ошибке
                MessageBox.Show("Можно вводить только целые числа.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = true; // Отменяем ввод точки
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
