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

    internal class DeletedDataVM : ViewModelBase
    {
        private bool _isReadOnly;

        DeletedDataRepository deletedDataRepository;

        private DataTable _dismissedEmployees;

        private DataTable _deletedDishes;
        private object _selectedItemDeletedDish;


        DataTable tempDelDish = new DataTable();



        public ICommand RestoreDishCommand { get; set; }



        public DataTable DismissedEmployees
        {
            get { return _dismissedEmployees; }
            set { _dismissedEmployees = value; }
        }
        public DataTable DeletedDishes
        {
            get { return _deletedDishes; }
            set { _deletedDishes = value; }
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

        public object SelectedItemDeletedDish
        {
            get { return _selectedItemDeletedDish; }
            set
            {
                _selectedItemDeletedDish = value;
                OnPropertyChanged(nameof(SelectedItemDeletedDish));
            }
        }

        public DeletedDataVM()
        {
            deletedDataRepository = new DeletedDataRepository();
            //DeletedDishes = deletedDataRepository.GetAllDeletedDishes();

            tempDelDish = deletedDataRepository.GetAllDeletedDishes();
            DeletedDishes = tempDelDish.Copy();
            DeletedDishes.Columns.Remove("Id");


            //dishesRepository = new DishesRepository();
            //Menu = dishesRepository.GetAllDishesFromArchive();
            //IsReadOnly = true;

            //tempMenu = dishesRepository.GetAllDishesFromMenu();
            //ActiveMenu = tempMenu.Copy();
            //ActiveMenu.Columns.Remove("Id");


            DismissedEmployees = deletedDataRepository.GetDismissedEmployees();
            IsReadOnly = true;


            DeletedDishes = deletedDataRepository.GetAllDeletedDishes();
            IsReadOnly = true;



            RestoreDishCommand = new RelayCommand(ExecuteRestoreDishCommand);


        }

        private void ExecuteRestoreDishCommand(object obj)
        {
            DataRowView dataRowView = SelectedItemDeletedDish as DataRowView;

            // Получаем ID выбранного блюда
            int dishId = int.Parse(tempDelDish.Rows[int.Parse(dataRowView.Row[0].ToString()) - 1][1].ToString());

            // Отображаем диалоговое окно с вопросом
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите восстановить это блюдо?", "Подтверждение восстановления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // Если пользователь выбрал "Да", то удаляем блюдо
            if (result == MessageBoxResult.Yes)
            {
                // Создаем объект блюда для передачи в метод удаления
                DishData dishToRestore = new DishData
                {
                    Id = dishId
                };

                // Удаляем блюдо
                deletedDataRepository.RestoreDeletedDish(dishToRestore);

                // Обновляем интерфейс, если необходимо
                OnPropertyChanged(nameof(tempDelDish));
            }
        }
    }
}
