using Cafe_Managment.Model;
using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToastNotifications;
using ToastNotifications.Messages;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using System.Diagnostics;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Cafe_Managment.View;
using System.Windows.Navigation;

namespace Cafe_Managment.ViewModel
{
    internal class KitchenVM : ViewModelBase
    {
        private Notifier _notifier;

        DishesRepository dishesRepository;
        ObservableCollection<ChequeModel> temp;
        int _selectedCheque;
        string _emptyMessage;
        private bool _isOrderReady;
        private DispatcherTimer _timer;
        private OrderVM _currentOrder;
        private bool _isOrderVisible;
        private bool _isCequesVisible;
        private System.Timers.Timer _dishStatusCheckTimer;
        private System.Timers.Timer statusCheckTimer;

        public bool IsChequesVisible
        {
            get { return _isCequesVisible; }
            set { _isCequesVisible = value;
            OnPropertyChanged(nameof(IsChequesVisible));}
        }
        public bool IsOrderVisible
        {
            get { return _isOrderVisible; }
            set
            {
                if (_isOrderVisible != value)
                {
                    _isOrderVisible = value;
                    OnPropertyChanged(nameof(IsOrderVisible));
                }
            }
        }
        public bool IsOrderReady
        {
            get { return _isOrderReady; }
            set
            {
                if (_isOrderReady != value)
                {
                    _isOrderReady = value;
                    OnPropertyChanged(nameof(IsOrderReady));
                }
            }
        }
        private TimeSpan _waitingTime;

        public OrderVM CurrentOrder
        {
            get { return _currentOrder; }
            set { _currentOrder = value;
            OnPropertyChanged(nameof(CurrentOrder));}
        }

        public TimeSpan WaitingTime
        {
            get => _waitingTime;
            set
            {
                if (_waitingTime != value) // Проверяем, изменилось ли значение
                {
                    _waitingTime = value;
                    OnPropertyChanged(nameof(WaitingTime));
                    // Уведомляем об изменении
                }
            }
        }

        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(nameof(CurrentView)); }
        }

        private ObservableCollection<ChequeModel> _cheques;

        public ObservableCollection<ChequeModel> Cheques
        {
            get { return _cheques; }
            set { _cheques = value; OnPropertyChanged(nameof(Cheques)); }
        }
        public int SelectedCheque
        {
            get { return _selectedCheque; }
            set { _selectedCheque = value;
                OnPropertyChanged(nameof(SelectedCheque)); }
        }

        public string EmptyMessage
        {
            get { return _emptyMessage; }
            set { _emptyMessage = value;
                OnPropertyChanged(nameof(EmptyMessage)); }
        }

        private DateTime _creationTime;
        private DispatcherTimer _otherTimer;

        public DateTime CreationTime
        {
            get { return _creationTime; }
            set { _creationTime = value; OnPropertyChanged(nameof(CreationTime)); }
        }


        public ICommand ChangeDishStatusCommand { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand CloseOrderCommand { get; set; }
        public ICommand OpenOrderPageCommand { get; set; }
        public ICommand OrderToInvis { get; set; }

        //public ICommand OpenOrderPageCommand { get; set; }

        public KitchenVM()
        {
            _notifier = CreateNotifier();

            IsChequesVisible = true;
            IsOrderVisible = false;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick; // Добавляем обработчик события таймера
            _timer.Start();

            _otherTimer = new DispatcherTimer();
            _otherTimer.Interval = TimeSpan.FromSeconds(7);
            _otherTimer.Tick += OtherTimer_Tick; // Подписываемся на событие Tick
            _otherTimer.Start();

            StartDishStatusCheckTimer();

            statusCheckTimer = new System.Timers.Timer(5000); // Интервал 5 секунд
            statusCheckTimer.Elapsed += OnStatusCheckTimerElapsed;
            statusCheckTimer.AutoReset = true;
            statusCheckTimer.Enabled = true;

            dishesRepository = new DishesRepository();

            OpenOrderPageCommand = new RelayCommand(OpenOrderPage);
            ChangeDishStatusCommand = new RelayCommand(ExecuteChangeDishStatusCommand);
            ReloadCommand = new RelayCommand(ExecuteReloadCommand);
            CloseOrderCommand = new RelayCommand(ExecuteCloseOrderCommand);
            OrderToInvis = new RelayCommand(ExecuteOrderToInvis);
            //OpenOrderPageCommand = new RelayCommand(ExecuteOpenOrderPageCommand);

            UpdateLists(1);
        }
        public void StartDishStatusCheckTimer()
        {
            _dishStatusCheckTimer = new System.Timers.Timer();
            _dishStatusCheckTimer.Interval = 7000; // Интервал в миллисекундах (например, 5 секунд)
            _dishStatusCheckTimer.Elapsed += OnDishStatusCheckTimerElapsed;
            _dishStatusCheckTimer.AutoReset = true;
            _dishStatusCheckTimer.Enabled = true;
        }
       
        private void CheckDishStatusUpdates()
        {
            // Получаем обновленные заказы
            var updatedOrders = dishesRepository.GetActiveOrders();

            // Перебираем каждый заказ
            foreach (var order in updatedOrders)
            {
                // Перебираем каждое блюдо в заказе
                foreach (var updatedDish in order.dishes)
                {
                    // Находим текущий заказ в локальном списке
                    var currentOrder = temp.FirstOrDefault(o => o.Id == order.Id);
                    if (currentOrder != null)
                    {
                        // Находим текущее блюдо в локальном заказе
                        var currentDish = currentOrder.dishes.FirstOrDefault(d => d.Id == updatedDish.Id);
                        if (currentDish != null && currentDish.Status != updatedDish.Status)
                        {
                            // Проверка статуса и роли пользователя
                            if (updatedDish.Status == 1 && UserData.RoleId == 7 || UserData.RoleId == 6)
                            {
                                _notifier.ShowInformation($"Блюдо {updatedDish.Title} из заказа №{order.Id} готово к выдаче!");
                            }

                            // Для отладки: выводим информацию о блюде и его статусе
                            //Debug.WriteLine($"Dish: {updatedDish.Title}, Old Status: {currentDish.Status}, New Status: {updatedDish.Status}");

                            // Обновляем статус блюда в локальном кэше
                            currentDish.Status = updatedDish.Status;
                        }
                    }
                }
            }

            // Обновляем списки, чтобы отобразить изменения в UI
            UpdateLists(0);
        }

        private void CheckOrderAndDishStatus()
        {
            var updatedOrders = dishesRepository.GetActiveOrders();

            foreach (var order in updatedOrders)
            {
                // Проверяем, изменился ли статус заказа на "готово"
                var currentOrder = temp.FirstOrDefault(o => o.Id == order.Id);
                if (currentOrder != null)
                {
                    bool orderWasReady = currentOrder.dishes.All(d => d.Status == 2);
                    bool orderIsNowReady = order.dishes.All(d => d.Status == 2);

                    // Если заказ стал готовым
                    if (!orderWasReady && orderIsNowReady)
                    {
                        _notifier.ShowSuccess($"Все блюда в заказе № {order.Id} готовы и выданы!");
                    }
                    // Если все блюда в заказе готовы и выданы
                    else if (!orderWasReady && orderIsNowReady)
                    {
                        _notifier.ShowSuccess($"Все блюда в заказе № {order.Id} готовы и выданы!");
                    }
                } 
            }

            // Обновляем списки, чтобы отобразить актуальную информацию
            UpdateLists(0);
        }

        private void OnDishStatusCheckTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckDishStatusUpdates();
        }
        private void OnStatusCheckTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckOrderAndDishStatus();
        }

        private void OtherTimer_Tick(object sender, EventArgs e)
        {
            UpdateLists(1);
        }

        public void ExecuteOrderToInvis(object obj)
        {
            // Переключаем видимость заказов
            IsOrderVisible = !IsOrderVisible;
            IsChequesVisible = true;
            UpdateLists(1); 
        }


        private void OpenOrderPage(object obj)
        {
            // Проверяем, является ли пользователь поваром или администратором
            bool isWaiterOrAdmin = UserData.RoleId == 6
                || UserData.RoleId == 7
                ;

            if (isWaiterOrAdmin)
            {
                UserData.OrderId = temp[SelectedCheque].Id;
                CurrentOrder = new OrderVM();

                IsOrderVisible = !IsOrderVisible;
                IsChequesVisible = !IsChequesVisible;
            }
            else
            {
                // Если пользователь не является поваром или администратором, выдаем сообщение об ошибке
                _notifier.ShowError("Только официанты могут добавлять блюда в заказ.");
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (var cheque in Cheques)
            {
                // Определяем время, с которого следует начинать отсчет
                var startTime = cheque.UpdatedAt != null && cheque.UpdatedAt != DateTime.MinValue ? cheque.UpdatedAt : cheque.CreatedAt;

                // Вычисляем время, прошедшее с указанного момента
                var elapsed = DateTime.Now - startTime;

                // Вычисляем оставшееся время (45 минут минус время, прошедшее с указанного момента)
                var remaining = TimeSpan.FromMinutes(45) - elapsed;

                // Если оставшееся время меньше нуля, устанавливаем его в ноль
                cheque.WaitingTime = (TimeSpan)(remaining < TimeSpan.Zero ? TimeSpan.Zero : remaining);

                 //Debug.WriteLine($"Cheque Id: {cheque.Id}, WaitingTime: {cheque.WaitingTime}");
            }
        }

        private void ExecuteCloseOrderCommand(object obj)
        {
            var selectedCheque = temp[SelectedCheque];

            // Проверяем, есть ли не готовые блюда
            bool hasUnreadyDishes = selectedCheque.dishes.Any(d => d.Status != 2);

            // Проверяем, является ли пользователь поваром
            bool isWaiter = UserData.RoleId == 6
                || UserData.RoleId == 7
                ;

            if (hasUnreadyDishes)
            {
                // Если есть не готовые блюда, выдаем сообщение и не закрываем заказ
                _notifier.ShowError($"Нельзя закрыть заказ № {selectedCheque.Id}, пока не все блюда готовы и выданы!");
            }
            else if (!isWaiter)
            {
                // Если пользователь не является поваром, выдаем сообщение об ошибке
                _notifier.ShowError("Только официанты могут закрывать заказ.");
            }
            else
            {
                // Иначе, все блюда готовы, можно закрыть заказ
                dishesRepository.IsOrderReady(selectedCheque);
                Cheques = dishesRepository.GetActiveOrders();
                UpdateLists(1);
                _notifier.ShowSuccess($"Заказ № {selectedCheque.Id} успешно выполнен и закрыт!");
            }
        }

        private Notifier CreateNotifier()
        {
            return new Notifier(cfg =>
            {
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    TimeSpan.FromSeconds(12),
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

        private void ExecuteReloadCommand(object obj)
        {
            UpdateLists(1);
        }

        void UpdateLists(int v)
        {
            switch (v)
            {
                case 0: // При выполнении простых штук
                    Cheques = new ObservableCollection<ChequeModel>(temp);
                    if (Cheques.Count == 0)
                    {
                        EmptyMessage = "Благодарим за работу!\nОднако заказов пока что нет.";
                    }
                    else
                    {
                        EmptyMessage = string.Empty;
                    }
                    break;
                case 1: // При связи с сервером
                    Cheques = dishesRepository.GetActiveOrders();
                    temp = new ObservableCollection<ChequeModel>(Cheques);

                    // Разбиваем каждое блюдо на отдельные элементы, если их количество больше одного
                    SplitDishes();

                    if (Cheques.Count == 0)
                    {
                        EmptyMessage = "Благодарим за работу!\nОднако заказов пока что нет.";
                    }
                    else
                    {
                        EmptyMessage = string.Empty;
                    }
                    break;
            }
        }

        private void SplitDishes()
        {
            ObservableCollection<ChequeModel> updatedCheques = new ObservableCollection<ChequeModel>();

            foreach (var cheque in Cheques)
            {
                List<DishData> splittedDishes = new List<DishData>();

                foreach (var dish in cheque.dishes)
                {
                    if (dish.Count > 1)
                    {
                        // Если блюдо в заказе больше одного, разбиваем их на отдельные элементы
                        for (int i = 0; i < dish.Count; i++)
                        {
                            splittedDishes.Add(new DishData
                            {
                                Title = dish.Title,
                                Count = 1,
                                Status = dish.Status,
                                Id = dish.Id // Если нужен идентификатор, необходимо уникальное значение
                            });
                        }
                    }
                    else
                    {
                        // Если блюдо в заказе одно, оставляем его без изменений
                        splittedDishes.Add(dish);
                    }
                }

                // Обновляем список блюд в заказе
                cheque.dishes = splittedDishes;
                updatedCheques.Add(cheque);
            }

            // Обновляем список заказов
            Cheques = updatedCheques;
        }

        private void ExecuteChangeDishStatusCommand(object obj)
        {
            int dishIndex = (int)obj;

            // Получаем ссылку на текущее блюдо
            var currentDish = temp[SelectedCheque].dishes[dishIndex];

            // Обновляем статус блюда
            switch (currentDish.Status)
            {
                case 0:
                    if (UserData.RoleId == 5 || UserData.RoleId == 7)
                    {
                        currentDish.Status += 1;
                        dishesRepository.UpdateStatus(currentDish);
                        UpdateLists(0);
                        //_notifier.ShowInformation($"Блюдо {currentDish.Title} из заказа №{temp[SelectedCheque].Id} готово к выдаче!");

                    }
                    else
                    {
                        _notifier.ShowError("Только повара могут отмечать факт готовности блюда.");
                    }

                    break;

                case 1:
                    if (UserData.RoleId == 6 || UserData.RoleId == 7)
                    {
                        currentDish.Status += 1;

                        if (currentDish.Count > 1)
                        {
                            currentDish.Count -= 1;
                            currentDish.Status = 0;

                        }
                        else if (currentDish.Count == 1)
                        {
                            currentDish.Status = 2; // Устанавливаем статус "готово"
                                                    // Обновляем статус блюда в базе данных
                            dishesRepository.UpdateStatus(currentDish);
                            UpdateLists(0);
                        }
                    }
                    else
                    {
                        _notifier.ShowError("Только официанты могут отмечать факт выдачи блюда.");
                    }
                    break;

                case 2:
                    // Если статус 2, ничего не делаем
                    break;
            }

            // Проверяем, все ли блюда в заказе готовы
            bool allDishesReady = temp[SelectedCheque].dishes.All(d => d.Status == 2);

            // Если все блюда готовы, выдаем уведомление
            if (allDishesReady)
            {
                _notifier.ShowSuccess($"Все блюда в заказе №{temp[SelectedCheque].Id} готовы и выданы!");
            }

            // Обновляем списки
            UpdateLists(0);
        }

    }
}
