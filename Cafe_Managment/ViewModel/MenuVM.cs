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
        private DataTable _menu;
        private DataTable _activemenu;

        public ICommand EditRowCommand { get; set; }
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

            tempArchvie = dishesRepository.GetAllDishesFromArchive();

            Menu = tempArchvie.Copy();

            Menu.Columns.Remove("Id");

            IsReadOnly = false;
            SelectedDish = -1;

            //EditRowCommand = new RelayCommand(EditRow);



            SaveRowCommand = new RelayCommand(ExecuteSaveRowCommand);
            EditRowCommand = new RelayCommand(ExecuteEditRowCommand);
            //DeleteRowCommand = new RelayCommand(ExecuteDeleteRowCommand);
            //TransferRowCommand = new RelayCommand(ExecuteTransferRowCommand);

            
        }
        private void ExecuteDeleteRowCommand()
        {
            throw new NotImplementedException();
        }
        private void ExecuteTransferRowCommand()
        {
            throw new NotImplementedException();
        }
        private void ExecuteSaveRowCommand(object obj)
        {
            MessageBox.Show(tempArchvie.Rows[SelectedDish][1].ToString());
        }

        private void ExecuteEditRowCommand(object obj)
        {
            IsReadOnly = !IsReadOnly;
        }
    }
}
