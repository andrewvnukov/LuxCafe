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
using ToastNotifications;
using ToastNotifications.Messages;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
namespace Cafe_Managment.ViewModel
{
    internal class OrderVM : ViewModelBase
    {
        private Notifier _notifier;

        DishesRepository dishesRepository;
        int _selectedCategory;
        object _selectedDish;
        int _spotNumber;
        int _guestCount;
        private List<int> _tableNumbers;
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
        public List<int> TableNumbers
        {
            get { return _tableNumbers; }
            set
            {
                _tableNumbers = value;
                OnPropertyChanged(nameof(TableNumbers));
            }
        }

        public ICommand SwitchToCategoryCommand { get; set; }
        public ICommand AddDishToOrderCommand { get; set; }
        public ICommand DescreaseDishCommand { get; set; }
        public ICommand CreateOrderCommand { get; set; }

        public OrderVM() 
        {
            _tableNumbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }; // Пример списка номеров столиков

            _notifier = CreateNotifier();

            dishesRepository = new DishesRepository();

            SwitchToCategoryCommand = new RelayCommand(ExecuteSwitchToCategoryCommand);
            AddDishToOrderCommand = new RelayCommand(ExecuteAddDishToOrderCommand);
            DescreaseDishCommand = new RelayCommand(ExecuteDescreaseDishCommand);
            CreateOrderCommand = new RelayCommand(ExecuteCreateOrderCommand, CanExecuteCreateOrderCommand);

            SpotNumber = 0;
            GuestCount = 0;
            TotalPrice = 0;
            CategoryList = new List<Category>();
            SelectedDishes = new List<DishData>();
            tempL = new List<DishData>();

            CategoryList.Add(new Category("Напитки", "/Images/Categories/Drinks.png"));
            CategoryList.Add(new Category("Закуски", "/Images/Categories/Snacks.png"));
            CategoryList.Add(new Category("Десерты", "/Images/Categories/Desserts.png"));
            CategoryList.Add(new Category("Гарниры", "/Images/Categories/Garnish.png"));
            CategoryList.Add(new Category("Супы", "/Images/Categories/Soup.png"));
            CategoryList.Add(new Category("Второе", "/Images/Categories/MainDish.png"));
            CategoryList.Add(new Category("Салаты", "/Images/Categories/Salad.png"));
            CategoryList.Add(new Category("Завтраки", "/Images/Categories/Breakfast.png"));
        }

        private Notifier CreateNotifier()
        {
            return new Notifier(cfg =>
            {
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    TimeSpan.FromSeconds(5),
                    MaximumNotificationCount.FromCount(5));

                cfg.PositionProvider = new PrimaryScreenPositionProvider(
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.DisplayOptions.TopMost = true;
                cfg.DisplayOptions.Width = 300;

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        private bool CanExecuteCreateOrderCommand(object arg)
        {
            return (GuestCount >0 && SpotNumber >0);
        }

        private void ExecuteCreateOrderCommand(object obj)
        {
            switch(dishesRepository.CreateNewOrder(tempL, SpotNumber, GuestCount, TotalPrice)){
                case 0:
                    _notifier.ShowSuccess("Заказ успешно оформлен!");
                    SelectedDishes = new List<DishData>();
                    tempL = new List<DishData>();
                    SpotNumber = 0;
                    GuestCount = 0;
                    TotalPrice = 0;
                    break;
                case 1:
                    _notifier.ShowError("Ошибка при оформлении заказа!");
                    break;
            }
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
            //Debug.WriteLine(temp.Title);
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
            //Debug.WriteLine(SelectedCategory.ToString());
            List<DishData> temp = dishesRepository.GetDishListByCategory(SelectedCategory+1);
            for (int i = 0; i < temp.Count; i++)
            {
                temp[i].ImageSource = CategoryList[SelectedCategory].ImagePath;
            }
            DishList = temp;
        }
    }
}
