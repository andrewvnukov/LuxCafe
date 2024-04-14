using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Cafe_Managment.Controls;
using Cafe_Managment.Utilities;

namespace Cafe_Managment.ViewModel
{
    internal class OrderVM : ViewModelBase
    {
        int _selectedCategory;
        List<Category> _categoryList;
        List<Dish> _dishList;
        public List<Category> CategoryList
        {
            get { return _categoryList; }
            set { _categoryList = value; 
                OnPropertyChanged(nameof(CategoryList));
            }
        }
        public int SelectedCategory
        {
            get { return _selectedCategory; }
            set { _selectedCategory = value; OnPropertyChanged(nameof(SelectedCategory)); }
        }

        public List<Dish> DishList
        {
            get { return _dishList; }
            set { _dishList = value; OnPropertyChanged(nameof(DishList)); }
        }

        public ICommand SwitchToCategoryCommand { get; set; }

        public OrderVM() 
        {
            SwitchToCategoryCommand = new RelayCommand(ExecuteSwitchToCategoryCommand);

            CategoryList = new List<Category>();
            CategoryList.Add(new Category("Завтраки", "/Images/Categories/Breakfast.png"));
            CategoryList.Add(new Category("Завтраки", "/Images/Categories/Breakfast.png"));
            CategoryList.Add(new Category("Завтраки", "/Images/Categories/Breakfast.png"));
            CategoryList.Add(new Category("Завтраки", "/Images/Categories/Breakfast.png"));
        }

        private void ExecuteSwitchToCategoryCommand(object obj)
        {
            MessageBox.Show(SelectedCategory.ToString());
        }
    }
}
