﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
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
    /// Логика взаимодействия для BindblePassBox.xaml
    /// </summary>
    public partial class BindblePassBox : UserControl
    {
        public static readonly DependencyProperty PasswordProperty =
     DependencyProperty.Register("Password", typeof(SecureString), typeof(BindblePassBox));

        public SecureString Password
        {
            get { return (SecureString)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public BindblePassBox()
        {
            InitializeComponent();
            txtPassword.PasswordChanged += OnPasswordChanged;

        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = txtPassword.SecurePassword;
        }
    }
}
