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
using ToastNotifications;
using ToastNotifications.Messages;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using System.Diagnostics;

namespace Cafe_Managment.ViewModel
{
    internal class MenuVM : ViewModelBase
    {

        private Notifier _notifier;

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
        DataTable tempArchive = new DataTable();
        DataTable tempMenu = new DataTable();
        private DataTable _menu;
        private DataTable _deletedDishes;
        private object _selectedItemDeletedDish;
        DataTable tempDelDish = new DataTable();

        public ICommand ShowDishSuccessfullyRestoredCommand { get; set; }
        public ICommand ShowDishSuccessfullyDeletedCommand { get; set; }
        public ICommand ShowDataSuccessfullyUpdatedCommand { get; set; }
        public ICommand ShowDishSuccessfullyTransferedCommand { get; set; }
        public ICommand ShowPriceSuccessfullyUpdatedCommand { get; set; }
        public ICommand ShowDishSuccessfullyDeletedCommandArchive { get; set; }
        public ICommand ShowErrorCommand { get; set; }
        public ICommand ShowWarningCommand { get; set; }
        public ICommand ShowInformationCommand { get; set; }
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
            _notifier = CreateNotifier();

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


            tempArchive = dishesRepository.GetAllDishesFromArchive();
            Menu = tempArchive.Copy();
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


            ShowDishSuccessfullyRestoredCommand = new RelayCommand(ExecuteShowDishSuccessfullyRestoredCommand);
            ShowDishSuccessfullyDeletedCommand = new RelayCommand(ExecuteShowDishSuccessfullyDeletedCommand);
            ShowDataSuccessfullyUpdatedCommand = new RelayCommand(ExecuteShowDataSuccessfullyUpdatedCommand);
            ShowDishSuccessfullyTransferedCommand = new RelayCommand(ExecuteShowDishSuccessfullyTransferedCommand);
            ShowPriceSuccessfullyUpdatedCommand = new RelayCommand(ExecuteShowPriceSuccessfullyUpdatedCommand);
            ShowDishSuccessfullyDeletedCommandArchive = new RelayCommand(ExecuteShowDishSuccessfullyDeletedCommandArchive);


            ShowErrorCommand = new RelayCommand(ExecuteShowErrorCommand);
            ShowWarningCommand = new RelayCommand(ExecuteShowWarningCommand);
            ShowInformationCommand = new RelayCommand(ExecuteShowInformationCommand);
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

        private void ExecuteShowDishSuccessfullyRestoredCommand(object obj)
        {
            DataRowView dataRowView = SelectedItemDeletedDish as DataRowView;
            DishData data = new DishData
            {
                Id = int.Parse(tempDelDish.Rows[int.Parse(dataRowView.Row[0].ToString()) - 1][1].ToString()),
            };

            _notifier.ShowSuccess($"Блюдо {dataRowView.Row[2].ToString()}\nуспешно восстановлено!");
        }
        private void ExecuteShowDishSuccessfullyDeletedCommand(object obj)
        {
            DataRowView dataRowView = SelectedItemMenu as DataRowView;
            DishData data = new DishData
            {
                Id = int.Parse(tempMenu.Rows[int.Parse(dataRowView.Row[0].ToString()) - 1][1].ToString()),
            };

            _notifier.ShowSuccess($"Блюдо {dataRowView.Row[2].ToString()}\nуспешно удалено!");
        }
        private void ExecuteShowDishSuccessfullyDeletedCommandArchive(object obj)
        {
            DataRowView dataRowView = SelectedItem as DataRowView;
            DishData data = new DishData
            {
                Id = int.Parse(tempArchive.Rows[int.Parse(dataRowView.Row[0].ToString()) - 1][1].ToString()),
            };

            _notifier.ShowSuccess($"Блюдо {dataRowView.Row[2].ToString()}\nуспешно удалено!");
        }
        private void ExecuteShowDishSuccessfullyTransferedCommand(object obj)
        {
            DataRowView dataRowView = SelectedItem as DataRowView;
            DishData data = new DishData
            {
                Id = int.Parse(tempArchive.Rows[int.Parse(dataRowView.Row[0].ToString()) - 1][1].ToString()),
            };
            
            _notifier.ShowSuccess($"Блюдо {dataRowView.Row[2].ToString()}\nуспешно перенесено в активное меню!");
        }
        private void ExecuteShowDataSuccessfullyUpdatedCommand(object obj)
        {
            DataRowView dataRowView = SelectedItem as DataRowView;
            DishData data = new DishData
            {
                Id = int.Parse(tempArchive.Rows[int.Parse(dataRowView.Row[0].ToString()) - 1][1].ToString()),
            };

            _notifier.ShowSuccess($"Блюдо {dataRowView.Row[2].ToString()}\nуспешно удалено!");

        }

        private void ExecuteShowPriceSuccessfullyUpdatedCommand(object obj)
        {
            DataRowView dataRowView = SelectedItemMenu as DataRowView;

            DishData data = new DishData
            {
                Id = int.Parse(tempMenu.Rows[int.Parse(dataRowView.Row[0].ToString()) - 1][1].ToString()),
            };

            _notifier.ShowSuccess($"Цена блюда {dataRowView.Row[2].ToString()}\nуспешно изменена!");  
        }
        private void ExecuteShowErrorCommand(object obj)
        {
            _notifier.ShowError("This is an error notification.");
        }

        private void ExecuteShowWarningCommand(object obj)
        {
            _notifier.ShowWarning("This is a warning notification.");
        }

        private void ExecuteShowInformationCommand(object obj)
        {
            _notifier.ShowInformation("This is an information notification.");
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
                ExecuteShowDishSuccessfullyRestoredCommand(dataRowView);
            }
        }

        private void ExecuteCloseDialogCommand(object obj)
        {
            NewPrice = "";
            IsViewVisible = false;
            ExecuteShowDishSuccessfullyTransferedCommand(null);
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
                _notifier.ShowSuccess($"Блюдо {dataRowView.Row[2].ToString()}\nуспешно удалено!");

            }
        }

        private void ExecuteEditPriceCommandMenu(object obj)
        {
            updatePrice = new UpdatePrice();
            
            updatePrice.IsVisibleChanged += (s, ev) =>
            {
                if (!updatePrice.IsVisible && (updatePrice.GetInput()!=""))
                {
                    ExecuteShowPriceSuccessfullyUpdatedCommand(null);

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
            ExecuteShowDishSuccessfullyTransferedCommand(Menu);
        }
        public void ExecuteSaveRowCommand(object parameter)
        {
            int temp = SelectedDish;

            
            DataRowView dataRowView = SelectedItem as DataRowView;

            Menu.AcceptChanges();
            //Получаем информацию о выбранной строке
            DishData dish = new DishData
            {
               Id = int.Parse(tempArchive.Rows[int.Parse(dataRowView.Row[0].ToString())-1][1].ToString()),
               Title = dataRowView.Row[2].ToString(),
               Description = dataRowView.Row[3].ToString(),
               Composition = dataRowView.Row[4].ToString(),
            };

            if (dish != null)
            {
                dishesRepository.UpdateDish(dish);

                tempArchive = dishesRepository.GetAllDishesFromArchive();
                DataTable dt = tempArchive.Copy();
                dt.Columns.Remove("Id");
                Menu = dt.Copy();

                IsReadOnly = true;
                OnPropertyChanged(nameof(tempArchive));
                _notifier.ShowSuccess($"Блюдо {dataRowView.Row[2].ToString()}\nуспешно изменено!");
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
            int dishId = int.Parse(tempArchive.Rows[int.Parse(dataRowView.Row[0].ToString()) - 1][1].ToString());

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
                OnPropertyChanged(nameof(tempArchive));
                ExecuteShowDishSuccessfullyDeletedCommandArchive(dataRowView);
            }
        }

        private void ExecuteTransferRowCommand(object parameter)
        {
            DataRowView dataRowView = SelectedItem as DataRowView;
            if (dataRowView == null) return;

            // Получение идентификатора блюда
            int dishId = int.Parse(tempArchive.Rows[int.Parse(dataRowView.Row[0].ToString()) - 1][1].ToString());

            // Проверка, находится ли блюдо уже в активном меню
            bool dishExistsInMenu = dishesRepository.CheckDishInMenu(dishId, UserData.BranchId);

            if (dishExistsInMenu)
            {
                // Если блюдо уже в меню, выводим предупреждение
                _notifier.ShowError($"{dataRowView.Row[2].ToString()}\nуже есть в активном меню");
                //MessageBox.Show("Это блюдо уже есть в активном меню.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    _notifier.ShowSuccess($"Блюдо {dataRowView.Row[2].ToString()}\nуспешно перенесено в активное меню!");

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
