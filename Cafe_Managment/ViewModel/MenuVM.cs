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
        private int _selectedDish;
        DataTable tempArchvie = new DataTable();
        DataTable tempMenu = new DataTable();
        private DataTable _menu;
        private DataTable _activemenu;

        public ICommand EditRowCommand { get; set; }
        public ICommand AddDishToArchiveCommand { get; set; }
        public ICommand SaveRowCommand { get; set; }
        public ICommand DeleteRowCommand { get; set; }
        public ICommand TransferRowCommand { get; set; }


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

        

        //private void EditRow(object parameter)
        //{
        //    // Получаем выбранный элемент (предполагается, что элемент содержит свойство IsEditable)
        //    var selectedItem = parameter as SelectedRow;

        //    if (selectedItem != null)
        //    {
        //        // Устанавливаем редактируемость только для выбранного элемента
        //        selectedItem.IsEditable = true;

        //        // Вызываем событие PropertyChanged для обновления интерфейса
        //        OnPropertyChanged(nameof(ActiveMenu));
        //    }
        //}

        //public event PropertyChangedEventHandler PropertyChanged;
        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}   



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
            // Получаем информацию о выбранной строке
            DishData dish = parameter as DishData;

            if (dish != null)
            {
                // Выполняем операции обновления данных в базе данных
                // Например, вызываем метод вашего репозитория, который обновляет данные в БД
                dishesRepository.UpdateDish(dish);

                // После обновления данных в базе данных можно выполнить какие-то дополнительные действия, если это необходимо

                // Например, можно обновить интерфейс, чтобы отобразить изменения
                OnPropertyChanged(nameof(tempArchvie));
            }
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
            IsReadOnly = !IsReadOnly;
        }
    }
}
