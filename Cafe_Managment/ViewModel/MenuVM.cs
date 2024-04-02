using Cafe_Managment.Model;
using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using Cafe_Managment.View.DialogWindows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private bool _isEnabled;
        private int _selectedDish;
        private object _selectedItem;
        DataTable tempArchvie = new DataTable();
        DataTable tempMenu = new DataTable();
        private DataTable _menu;
        private DataTable _activemenu;

        public ICommand EditRowCommand { get; set; }
        public ICommand AddDishToArchiveCommand { get; set; }
        public ICommand SaveRowCommand { get; set; }
        public ICommand DeleteRowCommand { get; set; }
        public ICommand TransferRowCommand { get; set; }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; OnPropertyChanged(nameof(IsEnabled)); } 
        }

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

        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public DataTable ActiveMenu
        {
            get { return _activemenu; }
            set { _activemenu = value;
                OnPropertyChanged(nameof(ActiveMenu));
            }
        }
        public DataTable Menu
        {
            get { return _menu; }
            set { _menu = value;
                OnPropertyChanged(nameof(Menu));
            }
        }
        public MenuVM()
        {
            dishesRepository = new DishesRepository();
            Menu = dishesRepository.GetAllDishesFromArchive();
            IsReadOnly = true;

            tempMenu = dishesRepository.GetAllDishesFromMenu();
            ActiveMenu = tempMenu.Copy();
            ActiveMenu.Columns.Remove("Id");


            tempArchvie = dishesRepository.GetAllDishesFromArchive();
            Menu = tempArchvie.Copy();
            Menu.Columns.Remove("Id");

            IsEnabled = false;
            IsReadOnly = false;
            SelectedDish = -1;

            //EditRowCommand = new RelayCommand(EditRow);

            SaveRowCommand = new RelayCommand(ExecuteSaveRowCommand);
            EditRowCommand = new RelayCommand(ExecuteEditRowCommand);
            AddDishToArchiveCommand = new RelayCommand(ExecuteAddDishToArchiveCommand);
            //DeleteRowCommand = new RelayCommand(ExecuteDeleteRowCommand);
            //TransferRowCommand = new RelayCommand(ExecuteTransferRowCommand);
        }

        public void ExecuteSaveRowCommand(object parameter)
        {
            int temp = SelectedDish;

            
            DataRowView dataRowView = SelectedItem as DataRowView;

            Menu.AcceptChanges();
            //Получаем информацию о выбранной строке
            DishData dish = new DishData
            {
               Id = int.Parse(tempArchvie.Rows[int.Parse(dataRowView.Row[0].ToString())-1][1].ToString()),
               Title = dataRowView.Row[2].ToString(),
               Description = dataRowView.Row[3].ToString(),
               Composition = dataRowView.Row[4].ToString(),
            };
            
            MessageBox.Show($"{dish.Id.ToString()}\n{dish.Title}");
            //if (dish != null)
            //{

            //    // Выполняем операции обновления данных в базе данных
            //    // Например, вызываем метод вашего репозитория, который обновляет данные в БД
            //    dishesRepository.UpdateDish(dish);

            //    // После обновления данных в базе данных можно выполнить какие-то дополнительные действия, если это необходимо

            //    // Например, можно обновить интерфейс, чтобы отобразить изменения
            //    OnPropertyChanged(nameof(tempArchvie));

            //}
        }

        private void ExecuteAddDishToArchiveCommand(object obj)
        {
            MessageBox.Show("Блюдо успешно добавлено!");
        }

        private void ExecuteDeleteRowCommand()
        {
            throw new NotImplementedException();
        }
        private void ExecuteTransferRowCommand()
        {
            throw new NotImplementedException();
        }
        //private void ExecuteSaveRowCommand(object obj)
        //{
        //    MessageBox.Show(SelectedDish.ToString());
        //}

        private void ExecuteEditRowCommand(object obj)
        {
            Menu.AcceptChanges();
            IsReadOnly = !IsReadOnly;
        }
    }
}
