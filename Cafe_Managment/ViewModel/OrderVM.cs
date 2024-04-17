using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Cafe_Managment.Model;
using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;

namespace Cafe_Managment.ViewModel
{
    internal class OrderVM : ViewModelBase
    {
        DishesRepository dishesRepository;
        int _selectedCategory;
        object _selectedDish;
        List<Category> _categoryList;
        List<DishData> _dishList;
        List<(int Id, int Count, string Title)> _selectedDishes;
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
        public object SelectedDish
        {
            get { return _selectedDish; }
            set { _selectedDish = value; OnPropertyChanged(nameof(SelectedDish));}
        }
        public List<DishData> DishList
        {
            get { return _dishList; }
            set { _dishList = value; OnPropertyChanged(nameof(DishList)); }
        }

        public List<(int Id, int Count, string Title)> SelectedDishes
        {
            get { return _selectedDishes; }
            set { _selectedDishes = value; OnPropertyChanged(nameof(SelectedDishes)); }
        }

        public ICommand SwitchToCategoryCommand { get; set; }
        public ICommand AddDishToOrderCommand { get; set; }

        public OrderVM() 
        {
            dishesRepository = new DishesRepository();

            SwitchToCategoryCommand = new RelayCommand(ExecuteSwitchToCategoryCommand);
            AddDishToOrderCommand = new RelayCommand(ExecuteAddDishToOrderCommand);

            CategoryList = new List<Category>();
            CategoryList.Add(new Category("Напитки", "/Images/Categories/Drinks.png"));
            CategoryList.Add(new Category("Закуски", "/Images/Categories/Snacks.png"));
            CategoryList.Add(new Category("Десерты", "/Images/Categories/Desserts.png"));
            CategoryList.Add(new Category("Гарниры", "/Images/Categories/Garnish.png"));
            CategoryList.Add(new Category("Супы", "/Images/Categories/Soup.png"));
            CategoryList.Add(new Category("Основные блюда", "/Images/Categories/MainDish.png"));
            CategoryList.Add(new Category("Салаты", "/Images/Categories/Salad.png"));
            CategoryList.Add(new Category("Завтраки", "/Images/Categories/Breakfast.png"));
        }

        private void ExecuteAddDishToOrderCommand(object obj)
        {
            DishData temp = SelectedDish as DishData;
        }

        private void ExecuteSwitchToCategoryCommand(object obj)
        {
            Debug.WriteLine(SelectedCategory.ToString());
            List<DishData> temp = dishesRepository.GetDishListByCategory(SelectedCategory+1);
            for (int i = 0; i < temp.Count; i++)
            {
                temp[i].ImageSource = CategoryList[SelectedCategory].ImagePath;
            }
            DishList = temp;
        }
    }
}
