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
        int _spotNumber;
        int _guestCount;
        List<Category> _categoryList;
        List<DishData> _dishList;
        List<DishData> _selectedDishes;
        List<DishData> tempL;
        float _totalPrice;
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
        public float TotalPrice
        {
            get { return _totalPrice; }
            set { _totalPrice = value;
            OnPropertyChanged(nameof(TotalPrice));}
        }
        public int SpotNumber
        {
            get { return _spotNumber; }
            set { _spotNumber = value;
            OnPropertyChanged(nameof(SpotNumber));}
        }
        public int GuestCount
        {
            get { return _guestCount; }
            set { _guestCount = value;
            OnPropertyChanged(nameof(GuestCount));}
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

        public List<DishData> SelectedDishes
        {
            get { return _selectedDishes; }
            set { _selectedDishes = value; OnPropertyChanged(nameof(SelectedDishes)); }
        }

        public ICommand SwitchToCategoryCommand { get; set; }
        public ICommand AddDishToOrderCommand { get; set; }
        public ICommand DescreaseDishCommand { get; set; }

        public OrderVM() 
        {
            dishesRepository = new DishesRepository();

            SwitchToCategoryCommand = new RelayCommand(ExecuteSwitchToCategoryCommand);
            AddDishToOrderCommand = new RelayCommand(ExecuteAddDishToOrderCommand);
            DescreaseDishCommand = new RelayCommand(ExecuteDescreaseDishCommand);

            TotalPrice = 0;
            CategoryList = new List<Category>();
            SelectedDishes = new List<DishData>();
            tempL = new List<DishData>();

            CategoryList.Add(new Category("Напитки", "/Images/Categories/Drinks.png"));
            CategoryList.Add(new Category("Закуски", "/Images/Categories/Snacks.png"));
            CategoryList.Add(new Category("Десерты", "/Images/Categories/Desserts.png"));
            CategoryList.Add(new Category("Гарниры", "/Images/Categories/Garnish.png"));
            CategoryList.Add(new Category("Супы", "/Images/Categories/Soup.png"));
            CategoryList.Add(new Category("Основные блюда", "/Images/Categories/MainDish.png"));
            CategoryList.Add(new Category("Салаты", "/Images/Categories/Salad.png"));
            CategoryList.Add(new Category("Завтраки", "/Images/Categories/Breakfast.png"));
        }

        private void ExecuteDescreaseDishCommand(object obj)
        {
            DishData temp = obj as DishData;
            foreach (DishData dish in tempL)
            {
                if (dish.Id == temp.Id)
                {
                    if (dish.Count == 1)
                    {
                        tempL.Remove(dish);
                        TotalPrice -= float.Parse(dish.Price);
                        break;
                    }
                    else
                    {
                        dish.Count -= 1;
                        TotalPrice -= float.Parse(dish.Price);
                        break;
                    }
                }
            }
            SelectedDishes = new List<DishData>();
            SelectedDishes = tempL;
        }

        private void ExecuteAddDishToOrderCommand(object obj)
        {
            bool IsInCart = false;
            DishData temp = obj as DishData;
            Debug.WriteLine(temp.Title);
            foreach (DishData dish in tempL)
            {
                if (dish.Id == temp.Id) 
                {   
                    dish.Count += 1; 
                    IsInCart = true;
                    TotalPrice += float.Parse(dish.Price);
                    break;
                }
                
            }
            if (!IsInCart)
            {
                tempL.Add(new DishData
                {
                    Id = temp.Id,
                    Price = temp.Price,
                    Title = temp.Title,
                    Count = 1,
                });
                TotalPrice += float.Parse(temp.Price);
            }
            SelectedDishes = new List<DishData>();
            SelectedDishes = tempL;
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
