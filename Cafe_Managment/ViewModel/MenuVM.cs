using Cafe_Managment.Model;
using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using Cafe_Managment.View;
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
        public UpdatePrice updatePrice;

        DishesRepository dishesRepository;
        public bool IsExitClicked = false;
        private bool _isReadOnly;
        private bool _isEnabled;
        private bool _isViewVisible;
        private int _selectedDish;
        private object _selectedItem;
        private object _selectedItemMenu;
        private string _newPrice;
        DataTable tempArchvie = new DataTable();
        DataTable tempMenu = new DataTable();
        private DataTable _menu;
        private DataTable _deletedDishes;
        private object _selectedItemDeletedDish;
        DataTable tempDelDish = new DataTable();


        public ICommand EditRowCommand { get; set; }
        public ICommand EditPriceCommandMenu { get; set; }
        public ICommand AddDishToArchiveCommand { get; set; }
        public ICommand SaveRowCommand { get; set; }
        public ICommand DeleteRowCommand { get; set; }
        public ICommand DeleteRowCommandMenu { get; set; }
        public ICommand TransferRowCommand { get; set; }
        public ICommand InfoCommandArchive { get; set; }
        public ICommand InfoCommandMenu { get; set; }
        public ICommand CloseWindowCommand { get; set; }
        public ICommand SavePriceCommand {  get; set; }
        public ICommand RestoreDishCommand { get; set; }


        public string NewPrice
        {
            get { return _newPrice; }
            set
            {
                _newPrice = value;
                OnPropertyChanged(nameof(NewPrice));
            }
        }

        public DataTable DeletedDishes
        {
            get { return _deletedDishes; }
            set { _deletedDishes = value; }
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

        public bool IsViewVisible
        {
            get { return _isViewVisible; }
            set { _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }
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
        public object SelectedItemMenu
        {
            get { return _selectedItemMenu; }
            set
            {
                _selectedItemMenu = value;
                OnPropertyChanged(nameof(SelectedItemMenu));
            }
        }

        private DataTable _activemenu;
        public DataTable ActiveMenu
        {
            get { return _activemenu; }
            set
            {
                _activemenu = value;
                OnPropertyChanged(nameof(ActiveMenu)); // Обязательно вызывать это после изменения свойства
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

            tempDelDish = dishesRepository.GetAllDeletedDishes();
            DeletedDishes = tempDelDish.Copy();
            DeletedDishes.Columns.Remove("Id");
            IsReadOnly = true;


            RestoreDishCommand = new RelayCommand(ExecuteRestoreDishCommand);


            tempArchvie = dishesRepository.GetAllDishesFromArchive();
            Menu = tempArchvie.Copy();
            Menu.Columns.Remove("Id");

            IsEnabled = false;
            IsReadOnly = true;
            SelectedDish = -1;
            IsViewVisible = true;

            //EditRowCommand = new RelayCommand(EditRow);

            SaveRowCommand = new RelayCommand(ExecuteSaveRowCommand);
            EditRowCommand = new RelayCommand(ExecuteEditRowCommand);
            AddDishToArchiveCommand = new RelayCommand(ExecuteAddDishToArchiveCommand);
            DeleteRowCommand = new RelayCommand(ExecuteDeleteRowCommand);
            TransferRowCommand = new RelayCommand(ExecuteTransferRowCommand);

            DeleteRowCommandMenu = new RelayCommand(ExecuteDeleteRowCommandMenu);
            EditPriceCommandMenu = new RelayCommand(ExecuteEditPriceCommandMenu);

            InfoCommandArchive = new RelayCommand(ExecuteInfoCommandArchive);
            InfoCommandMenu = new RelayCommand(ExecuteInfoCommandMenu);

            SavePriceCommand = new RelayCommand(ExecuteSavePriceCommand);
            CloseWindowCommand = new RelayCommand(ExecuteCloseDialogCommand);
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
                dishesRepository.RestoreDeletedDish(dishToRestore);

                // Обновляем интерфейс, если необходимо
                OnPropertyChanged(nameof(tempDelDish));
            }
        }

        private void ExecuteCloseDialogCommand(object obj)
        {
            NewPrice = "";
            IsViewVisible = false;

        }

        private void ExecuteDeleteRowCommandMenu(object obj)
        {
            DataRowView dataRowView = SelectedItemMenu as DataRowView;

            // Получаем ID выбранного блюда
            int dishId = int.Parse(tempMenu.Rows[int.Parse(dataRowView.Row[0].ToString()) - 1][1].ToString());
            

            // Отображаем диалоговое окно с вопросом
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить это блюдо?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // Если пользователь выбрал "Да", то удаляем блюдо
            if (result == MessageBoxResult.Yes)
            {
                // Создаем объект блюда для передачи в метод удаления
                DishData dishToDelete = new DishData
                {
                    Id = dishId
                };

                // Удаляем блюдо
                dishesRepository.DeleteDishMenu(dishToDelete);

                // Обновляем интерфейс, если необходимо
                RefreshMenu();

            }
        }

        private void ExecuteEditPriceCommandMenu(object obj)
        {
            updatePrice = new UpdatePrice();
            
            updatePrice.IsVisibleChanged += (s, ev) =>
            {
                if (!updatePrice.IsVisible && (updatePrice.GetInput()!=""))
                {
                    DataRowView dataRowView = SelectedItemMenu as DataRowView;
                    DishData data = new DishData
                    {
                        Id = int.Parse(tempMenu.Rows[int.Parse(dataRowView.Row[0].ToString()) - 1][1].ToString()),
                        Price = updatePrice.GetInput(),
                    };

                    dishesRepository.UpdateDishPrice(data);
                    MessageBox.Show($"Блюдо {dataRowView.Row[2].ToString()}\nБыло успешно изменено!");

                    tempMenu = dishesRepository.GetAllDishesFromMenu();
                    DataTable dt = tempMenu.Copy();
                    dt.Columns.Remove("Id");
                    ActiveMenu = dt.Copy();
                    

                    updatePrice.Close();
                }
                else
                {
                    if (!updatePrice.IsVisible)
                    {
                        updatePrice.Close();
                    }

                }
            };
            updatePrice.ShowDialog();
        }
        private void ExecuteSavePriceCommand(object obj)
        {
            IsViewVisible = false;
            RefreshMenu();
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

            if (dish != null)
            {
                dishesRepository.UpdateDish(dish);

                tempArchvie = dishesRepository.GetAllDishesFromArchive();
                DataTable dt = tempArchvie.Copy();
                dt.Columns.Remove("Id");
                Menu = dt.Copy();

                IsReadOnly = true;
                OnPropertyChanged(nameof(tempArchvie));
            }
        }

        private void ExecuteAddDishToArchiveCommand(object obj)
        {
            MessageBox.Show("Блюдо успешно добавлено!");
        }

        private void ExecuteDeleteRowCommand(object parameter)
        {
            // Получаем информацию о выбранной строке
            DataRowView dataRowView = SelectedItem as DataRowView;

            // Получаем ID выбранного блюда
            int dishId = int.Parse(tempArchvie.Rows[int.Parse(dataRowView.Row[0].ToString()) - 1][1].ToString());

            // Отображаем диалоговое окно с вопросом
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить это блюдо?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // Если пользователь выбрал "Да", то удаляем блюдо
            if (result == MessageBoxResult.Yes)
            {
                // Создаем объект блюда для передачи в метод удаления
                DishData dishToDelete = new DishData
                {
                    Id = dishId
                };

                // Удаляем блюдо
                dishesRepository.DeleteDish(dishToDelete);

                // Обновляем интерфейс, если необходимо
                OnPropertyChanged(nameof(tempArchvie));
            }
        }

        private void ExecuteTransferRowCommand(object parameter)
        {
            DataRowView dataRowView = SelectedItem as DataRowView;
            if (dataRowView == null) return;

            // Получение идентификатора блюда
            int dishId = int.Parse(tempArchvie.Rows[int.Parse(dataRowView.Row[0].ToString()) - 1][1].ToString());

            // Проверка, находится ли блюдо уже в активном меню
            bool dishExistsInMenu = dishesRepository.CheckDishInMenu(dishId, UserData.BranchId);

            if (dishExistsInMenu)
            {
                // Если блюдо уже в меню, выводим предупреждение
                MessageBox.Show("Это блюдо уже есть в активном меню.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Если блюдо отсутствует в меню, продолжаем и показываем окно для установки цены
            updatePrice = new UpdatePrice();

            // Обработчик события, когда окно установки цены закрывается
            updatePrice.IsVisibleChanged += (s, ev) =>
            {
                if (!updatePrice.IsVisible && updatePrice.GetInput() != "")
                {
                    // Создаем объект DishData и устанавливаем цену
                    DishData dishToMove = new DishData
                    {
                        Id = dishId,
                        Price = updatePrice.GetInput()  // Получаем введенную пользователем цену
                    };

                    // Перемещаем блюдо в активное меню
                    dishesRepository.TransferDishToActiveMenu(dishToMove);

                    // Обновляем данные активного меню после переноса блюда
                }
            };

            // Показываем окно для установки цены
            updatePrice.Show();
        }

        // Метод, который обновляет данные активного меню
        private void RefreshMenu()
        {
            tempMenu = dishesRepository.GetAllDishesFromMenu();
            ActiveMenu = tempMenu.Copy();
            ActiveMenu.Columns.Remove("Id");
        }


        private void ExecuteEditRowCommand(object obj)
        {
            Menu.AcceptChanges();
            IsReadOnly = !IsReadOnly;
        }

        private void ExecuteInfoCommandArchive(object obj)
        {
            MessageBox.Show("Поля №, Раздел, Дата изменения цены и Дата обновления \n" +
                "изменению  НЕ ПОДЛЕЖАТ!",
                "Внимание!!!", MessageBoxButton.YesNoCancel);
        }
        private void ExecuteInfoCommandMenu(object obj)
        {
            
            MessageBox.Show("Изменению подлежит только стоимость блюда\n" +
                "Остальные данные изменены не будут!!!",
                "Внимание!!!", MessageBoxButton.YesNoCancel);
        }
    }
}
