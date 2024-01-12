using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
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

namespace Cafe_Managment.Controls
{
    /// <summary>
    /// Логика взаимодействия для PasswordBoxCustom.xaml
    /// </summary>
    public partial class PasswordBoxCustom : UserControl
    {
        public string Password { get; set; }
        public PasswordBoxCustom()
        {
            InitializeComponent();
        }

        

       

        private void PassBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PassBox.Text = Password;
        }

        private void PassBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Password = PassBox.Text;
            string temp = "";
            for (int i = 0; i < PassBox.Text.Length; i++)
            {
                temp += '●';
            }
            PassBox.Text = temp;
            PassBox.SelectionStart = PassBox.Text.Length;
        }
    }
}
