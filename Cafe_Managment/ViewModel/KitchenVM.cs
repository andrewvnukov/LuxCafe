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

namespace Cafe_Managment.ViewModel
{
    internal class KitchenVM : ViewModelBase
    {
        private Notifier _notifier;

        DishesRepository dishesRepository;
        List<ChequeModel> _cheques;
        List<ChequeModel> temp;
        int _selectedCheque;
        string _emptyMessage;
        private bool _isOrderReady;
        private DispatcherTimer _timer;

        // Свойство для привязки в XAML
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

        public List<ChequeModel> Cheques
        {
            get { return _cheques; }
            set { _cheques = value;
                OnPropertyChanged(nameof(Cheques)); }
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

        public DateTime CreationTime
        {
            get { return _creationTime; }
            set { _creationTime = value; OnPropertyChanged(nameof(CreationTime)); }
        }


        public ICommand ChangeDishStatusCommand { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand CloseOrderCommand { get; set; }
        public ICommand OpenOrderCommand { get; set; }



        public KitchenVM()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1); // Интервал таймера - 1 секунда
            _timer.Tick += Timer_Tick; // Добавляем обработчик события таймера
            _timer.Start(); // Запускаем таймер

            _notifier = CreateNotifier();

            dishesRepository = new DishesRepository();

            ChangeDishStatusCommand = new RelayCommand(ExecuteChangeDishStatusCommand);
            ReloadCommand = new RelayCommand(ExecuteReloadCommand);
            CloseOrderCommand = new RelayCommand(ExecuteCloseOrderCommand);
            OpenOrderCommand = new RelayCommand(Order);

            UpdateLists(1);
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Обновляем время ожидания для каждого заказа
            foreach (var cheque in Cheques)
            {
                cheque.WaitingTime = DateTime.Now - cheque.CreatedAt;
                Debug.WriteLine($"Cheque Id: {cheque.Id}, WaitingTime: {cheque.WaitingTime}");
            }
        }

        private void Order(object obj) => new OrderVM();

        


        private void ExecuteCloseOrderCommand(object obj)
        {
            var selectedCheque = temp[SelectedCheque];

            // Проверяем, есть ли не готовые блюда
            bool hasUnreadyDishes = selectedCheque.dishes.Any(d => d.Status != 2);

            if (hasUnreadyDishes)
            {
                // Если есть не готовые блюда, выдаем сообщение и не закрываем заказ
                _notifier.ShowError($"Нельзя закрыть заказ № {selectedCheque.Id}, пока не все блюда готовы и выданы!");
            }
            else
            {
                // Иначе, все блюда готовы, можно закрыть заказ
                dishesRepository.IsOrderReady(selectedCheque);
                Cheques = dishesRepository.GetActiveOrders();
                _timer.Stop();
                UpdateLists(1);
                _notifier.ShowSuccess($"Заказ № {selectedCheque.Id} успешно выполнен и закрыт!");
            }
        }

        public bool IsOrderActiveForTooLong(ChequeModel cheque)
        {
            return (DateTime.Now - cheque.CreatedAt).TotalMinutes > 1;
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

        private void ExecuteReloadCommand(object obj)
        {
            UpdateLists(1);
        }

        void UpdateLists(int v)
        {
            switch (v)
            {
                case 0: // При выполнении простых штук
                    Cheques = new List<ChequeModel>();
                    Cheques = temp;
                    if (Cheques.Count == 0)
                    {
                        EmptyMessage = "Благодарим за работу!\nОднако заказов пока что нет!";
                    }
                    else
                    {
                        EmptyMessage = string.Empty;
                    }
                    break;
                case 1: // При связи с сервером
                    Cheques = new List<ChequeModel>();
                    Cheques = dishesRepository.GetActiveOrders();
                    temp = Cheques;

                    // Разбиваем каждое блюдо на отдельные элементы, если их количество больше одного
                    SplitDishes();

                    if (Cheques.Count == 0)
                    {
                        EmptyMessage = "Благодарим за работу!\nОднако заказов пока что нет!";
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
            List<ChequeModel> updatedCheques = new List<ChequeModel>();

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
                    currentDish.Status += 1;
                    dishesRepository.UpdateStatus(currentDish);

                    break;

                case 1:
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
