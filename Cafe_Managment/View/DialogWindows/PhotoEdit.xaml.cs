﻿using System;
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
    /// Логика взаимодействия для PhotoEdit.xaml
    /// </summary>
    public partial class PhotoEdit : Window
    {
        public PhotoEdit(BitmapImage bitmapImage)
        {
            InitializeComponent();
            Profile.Source = bitmapImage;
        }
    }
}
