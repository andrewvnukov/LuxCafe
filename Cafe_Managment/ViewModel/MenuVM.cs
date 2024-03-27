using Cafe_Managment.Model;
using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using Cafe_Managment.View.DialogWindows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Cafe_Managment.ViewModel
{
    internal class MenuVM : ViewModelBase
    {
        DishesRepository dishesRepository;
        private bool _isReadOnly;
        private int _selectedDish;


        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        public int SelectedDish
        {
            get { return _selectedDish; }
            set
            {
                _selectedDish = value;
                OnPropertyChanged(nameof(SelectedDish));
            }
        }

        private DataTable _menu;
        private DataTable _activemenu;


        public DataTable ActiveMenu
        {
            get { return _menu; }
            set { _menu = value; }
        }
        public DataTable Menu
        {
            get { return _activemenu; }
            set { _activemenu = value; }
        }
        public MenuVM()
        {
            dishesRepository = new DishesRepository();
            Menu = dishesRepository.GetAllDishesFromArchive();
            IsReadOnly = false;
            ActiveMenu = dishesRepository.GetAllDishesFromMenu();


            //HireCommand = new RelayCommand(ExecuteHireCommand, CanExecuteHireCommand);
            //FireCommand = new RelayCommand(ExecuteFireCommand, CanExecuteFireCommand);
        }
    }
}
