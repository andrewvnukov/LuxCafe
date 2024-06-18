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
    /// Логика взаимодействия для NewBranch.xaml
    /// </summary>
    public partial class NewBranch : Window
    {
        public NewBranch()
        {
            InitializeComponent();
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        public string GetInput()
        {
            return newBranch.Text;
        }

        private void Button_CloseClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
