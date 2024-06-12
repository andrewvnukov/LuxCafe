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
        bool IsOpen;
        private int _selectedDish;
        private object _selectedItem;
        private object _selectedItemMenu;
        private string _newPrice;
        private DishData _newDish;
        private int _selectedCategory;

        DataTable tempArchive = new DataTable();
        DataTable tempMenu = new DataTable();
        private object _selectedItemDeletedDish;
        DataTable tempDelDish = new DataTable();
        List<string> _categoties = new List<string>();

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
        public ICommand SavePriceCommand { get; set; }
        public ICommand RestoreDishCommand { get; set; }
        public ICommand RefreshDB { get; set; }

       
        public int SelectedCategory
        {
            get { return _selectedCategory; }
            set { _selectedCategory = value; 
            OnPropertyChanged(nameof(SelectedCategory));}
        }
        public List<string> Categories
        {
            get { return _categoties; }
            set { _categoties = value;
                OnPropertyChanged(nameof(_categoties));
            }
        } 
        public string NewPrice
        {
            get { return _newPrice; }
            set
            {
                _newPrice = value;
                OnPropertyChanged(nameof(NewPrice));
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set { _title= value;
            OnPropertyChanged(nameof(Title));}
        }
        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        private string _composition;
        public string Composition
        {
            get => _composition;
            set
            {
                _composition = value;
                OnPropertyChanged(nameof(Composition));
            }
        }

        public DishData NewDishAdd
        {
            get => _newDish;
            set
            {
                _newDish = value;
                OnPropertyChanged(nameof(NewDishAdd));
            }
        }

        private DataTable _deletedDishes;

        public DataTable DeletedDishes
        {
            get => _deletedDishes;
            set
            {
                if (_deletedDishes != value) // Проверяем, изменилось ли значение
                {
                    _deletedDishes = value;
                    AddIndexColumnToDelDish(_deletedDishes); // Добавляем индексы без вызова сеттера
                    OnPropertyChanged(nameof(DeletedDishes)); // Уведомляем об изменении
                }
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

        private DataTable _menu;

        public DataTable Menu
        {
            get => _menu;
            set
            {
                if (_menu != value) // Проверяем, изменилось ли значение
                {
                    _menu = value;
                    AddIndexColumnToArchive(_menu); // Добавляем индексы без вызова сеттера
                    OnPropertyChanged(nameof(Menu)); // Уведомляем об изменении
                }
            }
        }

        private DataTable _activemenu;

        public DataTable ActiveMenu
        {
            get => _activemenu;
            set
            {
                if (_activemenu != value)
                {
                    _activemenu = value;
                    AddIndexColumnToAm(_activemenu); // Добавляем индексы без вызова сеттера
                    OnPropertyChanged(nameof(ActiveMenu)); // Уведомляем об изменении
                }
            }
        }


        private bool _canEditColumns;
        public bool CanEditColumns
        {
            get { return _canEditColumns; }
            set
            {
                _canEditColumns = value;
                OnPropertyChanged(nameof(CanEditColumns));
            }
        }

        private int _index;

        public int Index
        {
            get => _index;
            set
            {
                if (_index != value) // Проверяем, изменилось ли значение
                {
                    _index = value;
                    OnPropertyChanged(nameof(Index)); // Уведомляем об изменении
                }
            }
        }
       

        public MenuVM()
        {

            dishesRepository = new DishesRepository();
            //tempArchive = dishesRepository.GetAllDishesFromArchive();

            // Создаем копию перед модификацией
            tempArchive = dishesRepository.GetAllDishesFromArchive();
            Menu = tempArchive.Copy();
            AddIndexColumnToArchive(_menu);

            //Menu.Columns.Remove("Id");

            IsReadOnly = true;

            _notifier = CreateNotifier();

            tempMenu = dishesRepository.GetAllDishesFromMenu();
            ActiveMenu = tempMenu.Copy();
            AddIndexColumnToAm(_activemenu);


            //ActiveMenu.Columns.Remove("Id");

            tempDelDish = dishesRepository.GetAllDeletedDishes();
            DeletedDishes = tempDelDish.Copy();
            AddIndexColumnToDelDish(_deletedDishes);

            //DeletedDishes.Columns.Remove("Id");
            //IsReadOnly = false;

            Categories = dishesRepository.GetAllCategories();



            IsEnabled = false;
            //IsReadOnly = false;
            SelectedDish = -1;
            IsViewVisible = true;

            //EditRowCommand = new RelayCommand(EditRow);

            SaveRowCommand = new RelayCommand(ExecuteSaveRowCommand, CanAdministrate);
            EditRowCommand = new RelayCommand(ExecuteEditRowCommand, CanAdministrate);
            AddDishToArchiveCommand = new RelayCommand(ExecuteAddDishToArchiveCommand, CanAdministrate);
            DeleteRowCommand = new RelayCommand(ExecuteDeleteRowCommand, CanAdministrate);
            TransferRowCommand = new RelayCommand(ExecuteTransferRowCommand, CanAdministrate);
            RestoreDishCommand = new RelayCommand(ExecuteRestoreDishCommand, CanAdministrate);
            DeleteRowCommandMenu = new RelayCommand(ExecuteDeleteRowCommandMenu, CanAdministrate);
            EditPriceCommandMenu = new RelayCommand(ExecuteEditPriceCommandMenu, CanAdministrate);

            InfoCommandArchive = new RelayCommand(ExecuteInfoCommandArchive);
            InfoCommandMenu = new RelayCommand(ExecuteInfoCommandMenu);

            SavePriceCommand = new RelayCommand(ExecuteSavePriceCommand, CanAdministrate);
            CloseWindowCommand = new RelayCommand(ExecuteCloseDialogCommand);


            ShowDishSuccessfullyDeletedCommand = new RelayCommand(ExecuteShowDishSuccessfullyDeletedCommand);
            ShowDataSuccessfullyUpdatedCommand = new RelayCommand(ExecuteShowDataSuccessfullyUpdatedCommand);
            ShowDishSuccessfullyTransferedCommand = new RelayCommand(ExecuteShowDishSuccessfullyTransferedCommand);
            ShowPriceSuccessfullyUpdatedCommand = new RelayCommand(ExecuteShowPriceSuccessfullyUpdatedCommand);
            RefreshDB = new RelayCommand(ExecuteRefreshDB);
        }

        private void ExecuteRefreshDB(object obj)
        {
            RefreshArchive();
        }

        public DataTable AddIndexColumnToAm(DataTable table)
        {
            // Проверяем, есть ли уже столбец индекса
            if (!table.Columns.Contains("Index"))
            {
                // Добавляем новый столбец
                DataColumn indexColumn = new DataColumn("Index", typeof(int));
                table.Columns.Add(indexColumn);
            }

            // Заполняем столбец индексами
            for (int i = 0; i < table.Rows.Count; i++)
            {
                table.Rows[i]["Index"] = i + 1;
            }

            return table;
        }


        public DataTable AddIndexColumnToArchive(DataTable table)
        {
            // Проверяем, есть ли уже столбец индекса
            if (!table.Columns.Contains("Index"))
            {
                // Добавляем новый столбец
                DataColumn indexColumn = new DataColumn("Index", typeof(int));
                table.Columns.Add(indexColumn);
            }

            // Заполняем столбец индексами
            for (int i = 0; i < table.Rows.Count; i++)
            {
                table.Rows[i]["Index"] = i + 1;
            }

            return table;
        }


        public DataTable AddIndexColumnToDelDish(DataTable table)
        {
            // Проверяем, есть ли уже столбец индекса
            if (!table.Columns.Contains("Index"))
            {
                // Добавляем новый столбец
                DataColumn indexColumn = new DataColumn("Index", typeof(int));
                table.Columns.Add(indexColumn);
            }

            // Заполняем столбец индексами
            for (int i = 0; i < table.Rows.Count; i++)
            {
                table.Rows[i]["Index"] = i + 1;
            }

            return table;
        }

        private bool CanAdministrate(object arg)
        {
            return (UserData.RoleId == 2 || UserData.RoleId == 7);
        }

        private Notifier CreateNotifier()
        {
            return new Notifier(cfg =>
            {
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    TimeSpan.FromSeconds(40),
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

        private void ExecuteShowDishSuccessfullyRestoredCommand(DataRowView dataRowView)
        {
            DishData data = new DishData
            {
                Id = int.Parse(dataRowView.Row[0].ToString())
            };

            _notifier.ShowSuccess($"Блюдо {dataRowView.Row[2].ToString()}\nуспешно восстановлено!");
        }
        private void ExecuteShowDishSuccessfullyDeletedCommand(object obj)
        {
            DataRowView dataRowView = SelectedItemMenu as DataRowView;
            DishData data = new DishData
            {
                Id = int.Parse(dataRowView.Row[0].ToString())
            };

            _notifier.ShowSuccess($"Блюдо {dataRowView.Row[2].ToString()}\nуспешно удалено!");
        }
        private void ExecuteShowDishSuccessfullyDeletedCommandArchive(DataRowView dataRowView)
        {
            DishData data = new DishData
            {
                Id = int.Parse(dataRowView.Row[0].ToString())
            };

            _notifier.ShowSuccess($"Блюдо {dataRowView.Row[2].ToString()}\nуспешно удалено!");
        }
        private void ExecuteShowDishSuccessfullyTransferedCommand(object obj)
        {
            DataRowView dataRowView = SelectedItemMenu as DataRowView;
            DishData data = new DishData
            {
                Id = int.Parse(dataRowView.Row[0].ToString())
            };
            
            _notifier.ShowSuccess($"Блюдо {dataRowView.Row[2].ToString()}\nуспешно перенесено в активное меню!");
        }
        private void ExecuteShowDataSuccessfullyUpdatedCommand(object obj)
        {
            DataRowView dataRowView = SelectedItem as DataRowView;
            DishData data = new DishData
            {
                Id = int.Parse(dataRowView.Row[0].ToString())
            };

            _notifier.ShowSuccess($"Блюдо {dataRowView.Row[2].ToString()}\nуспешно удалено!");

        }

        private void ExecuteShowPriceSuccessfullyUpdatedCommand(object obj)
        {
            DataRowView dataRowView = SelectedItemMenu as DataRowView;

            DishData data = new DishData
            {
                Id = int.Parse(dataRowView.Row[0].ToString()),
            };

            _notifier.ShowSuccess($"Цена блюда {dataRowView.Row[2].ToString()}\nуспешно изменена!");  
        }
       
        private void ExecuteRestoreDishCommand(object obj)
        {
            DataRowView dataRowView = SelectedItemDeletedDish as DataRowView;

            // Получаем ID выбранного блюда
            int dishId = int.Parse(dataRowView.Row[0].ToString());

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
                ExecuteShowDishSuccessfullyRestoredCommand(dataRowView);

                // Удаляем блюдо
                dishesRepository.RestoreDeletedDish(dishToRestore);
                RefreshDeletedDishes();
                RefreshArchive();
                tempMenu = dishesRepository.GetAllDishesFromMenu(); // Обновляем данные меню
                DataTable dtMenu = tempMenu.Copy();
                //dtMenu.Columns.Remove("Id");
                ActiveMenu = dtMenu.Copy();
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
            int dishId = int.Parse(dataRowView.Row[0].ToString());


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
            DataRowView dataRowView = SelectedItemMenu as DataRowView;

            updatePrice.IsVisibleChanged += (s, ev) =>
            {
                if (!updatePrice.IsVisible && !string.IsNullOrEmpty(updatePrice.GetInput()))
                {
                    // Создаем объект DishData
                    DishData dishData = new DishData
                    {
                        Id = int.Parse(dataRowView.Row[0].ToString()),
                        Price = updatePrice.GetInput() // Получаем введенную цену
                    };

                    try
                    {
                        // Обновляем цену блюда
                        dishesRepository.UpdateDishPrice(dishData);


                        tempMenu = dishesRepository.GetAllDishesFromMenu(); // Обновляем данные меню
                        DataTable dtMenu = tempMenu.Copy();
                        //dtMenu.Columns.Remove("Id");
                        ActiveMenu = dtMenu.Copy();

                        updatePrice.Close(); // Закрываем окно
                        ExecuteShowPriceSuccessfullyUpdatedCommand(null); // Показать уведомление о смене цены

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Произошла ошибка при обновлении цены: {ex.Message}");
                    }
                }
                else if (!updatePrice.IsVisible)
                {
                    updatePrice.Close(); // Закрываем окно, если оно невидимо
                }
            };

            updatePrice.ShowDialog(); // Открываем окно
        }
        private void ExecuteSavePriceCommand(object obj)
        {
            
            IsViewVisible = false;
            //updatePrice.Close();
            RefreshMenu();
            //tempMenu = ActiveMenu; // Сохранение текущего значения
            //ActiveMenu = null; // Сброс
            //ActiveMenu = tempMenu;
        }
        public void ExecuteSaveRowCommand(object parameter)
        {
            int temp = SelectedDish;

            
            DataRowView dataRowView = SelectedItem as DataRowView;

            Menu.AcceptChanges();
            //Получаем информацию о выбранной строке
            DishData dish = new DishData
            {
               Id = int.Parse(dataRowView.Row[0].ToString()),
               Title = dataRowView.Row[2].ToString(),
               Description = dataRowView.Row[3].ToString(),
               Composition = dataRowView.Row[4].ToString()
            };

            if (dish != null)
            {
                dishesRepository.UpdateDish(dish);

                tempArchive = dishesRepository.GetAllDishesFromArchive();
                DataTable dtArchive = tempArchive.Copy();
                //dtArchive.Columns.Remove("Id");
                Menu = dtArchive.Copy();

                IsReadOnly = true;
                OnPropertyChanged(nameof(tempArchive));
                RefreshMenu();

                _notifier.ShowSuccess($"Блюдо {dataRowView.Row[2].ToString()}\nуспешно изменено!");
            }

        }

        private void ExecuteAddDishToArchiveCommand(object obj)
        {
            
            if (Title != null && Description!=null && Composition!=null)
            {
                var window = obj as Window;
                NewDishAdd = new DishData
                {
                    Title = Title,
                    Description = Description,
                    Composition = Composition,
                    CategoryId = SelectedCategory
                };
                dishesRepository.AddDishToArchive(NewDishAdd);

                tempArchive = dishesRepository.GetAllDishesFromArchive();
                DataTable dtArchive = tempArchive.Copy();
                //dtArchive.Columns.Remove("Id");
                Menu = dtArchive.Copy();

                IsReadOnly = true;
                OnPropertyChanged(nameof(tempArchive));
                window.Close();

                _notifier.ShowSuccess($"Блюдо {NewDishAdd.Title.ToString()}\nуспешно добавлено в архив блюд!");

            }
            else
            {
                if (Title != null || Description != null || Composition != null)
                {
                    _notifier.ShowWarning("Не все поля заполнены!");
                }
                else
                {
                    NewDish newDish = new NewDish();
                    newDish.ShowDialog();
                }
            }
        }

        private void ExecuteDeleteRowCommand(object parameter)
        {
            DataRowView dataRowView = SelectedItem as DataRowView;

            if (dataRowView == null)
            {
                MessageBox.Show("Не выбран элемент для удаления.");
                return;
            }

            int dishId = int.Parse(dataRowView.Row[0].ToString());

            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить это блюдо?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DishData dishToDelete = new DishData
                {
                    Id = dishId
                };

                // Обновляем привязку данных
                dishesRepository.DeleteDish(dishToDelete);
                RefreshDeletedDishes();
                RefreshArchive();
                ExecuteShowDishSuccessfullyDeletedCommandArchive(dataRowView);

            }
        }

        private void ExecuteTransferRowCommand(object parameter)
        {
            DataRowView dataRowView = SelectedItem as DataRowView;
            if (dataRowView == null) return;

            // Получение идентификатора блюда
            int dishId = int.Parse(dataRowView.Row[0].ToString());

            // Проверка, находится ли блюдо уже в активном меню
            bool dishExistsInMenu = dishesRepository.CheckDishInMenu(dishId, UserData.BranchId);
            

            if (dishExistsInMenu)
            {
                // Если блюдо уже в меню, выводим предупреждение
                _notifier.ShowError($"{dataRowView.Row[2].ToString()}\nуже есть в активном меню");
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

        
                    RefreshMenu();
                    _notifier.ShowSuccess($"Блюдо {dataRowView.Row[2].ToString()}\nуспешно перенесено в активное меню!");

                }
            };

            // Показываем окно для установки цены
            updatePrice.ShowDialog();
        }

        // Метод, который обновляет данные активного меню
        private void RefreshMenu()
        {
            tempMenu = dishesRepository.GetAllDishesFromMenu();
            DataTable dt = tempMenu.Copy();

            //dt.Columns.Remove("Id");
            ActiveMenu = new DataTable();
            ActiveMenu = dt;
            AddIndexColumnToAm(ActiveMenu);

        }
        private void RefreshArchive()
        {
            tempArchive = dishesRepository.GetAllDishesFromArchive();
            DataTable dtArchive = tempArchive.Copy();
            Menu = new DataTable();

            Menu = dtArchive;
            AddIndexColumnToArchive(Menu);

        }
        private void RefreshDeletedDishes()
        {
            tempDelDish = dishesRepository.GetAllDeletedDishes();
            DataTable dtDelDishes = tempDelDish.Copy();
            //dtDelDishes.Columns.Remove("Id");
            DeletedDishes = dtDelDishes.Copy();
            AddIndexColumnToDelDish(_deletedDishes);

        }


        private void ExecuteEditRowCommand(object obj)
        {
            //Menu.AcceptChanges();
            //CanEditColumns = false; // Переключаем состояние

            //OnPropertyChanged(nameof(CanEditColumns)); // Убедитесь, что этот вызов здесь
            //Debug.WriteLine("ToggleEditMode called."); // Проверка
            //tempArchive = Menu; // Сохранение текущего значения
            //Menu = null; // Сброс
            //Menu = tempArchive; 
            IsReadOnly = !IsReadOnly;

        }

        private void ExecuteInfoCommandArchive(object obj)
        {
            _notifier.ShowInformation("№, Раздел, Дата добавления и Дата последнего обновления не подлежат изменению!");
                
        }
        private void ExecuteInfoCommandMenu(object obj)
        {
            _notifier.ShowInformation("Изменению подлежит только стоимость блюда");
                
        }
    }
}
