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

        public List<ChequeModel> Cheques
        {
            get { return _cheques; }
            set { _cheques = value; 
            OnPropertyChanged(nameof(Cheques));}
        }
        public int SelectedCheque
        {
            get { return _selectedCheque; }
            set { _selectedCheque = value;
            OnPropertyChanged(nameof(SelectedCheque));}
        }

        public string EmptyMessage
        {
            get { return _emptyMessage; }
            set { _emptyMessage = value;
            OnPropertyChanged(nameof(EmptyMessage));}
        }


        public ICommand ChangeDishStatusCommand { get; set; }
        public ICommand ReloadCommand { get; set; }

        public KitchenVM() 
        {
            _notifier = CreateNotifier();

            dishesRepository = new DishesRepository();

            ChangeDishStatusCommand = new RelayCommand(ExecuteChangeDishStatusCommand);
            ReloadCommand = new RelayCommand(ExecuteReloadCommand);
            UpdateLists(1);
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
                case 0://При выполнении простых штук
                    Cheques = new List<ChequeModel>();
                    Cheques = temp;
                    if(Cheques.Count == 0)
                    {
                        EmptyMessage = "Благодарим за работу!\nНо заказов пока что нет!";
                    }
                    else
                    {
                        EmptyMessage = string.Empty;
                    }
                    break;
                case 1://При связи с сервером
                    Cheques = new List<ChequeModel>();
                    Cheques = dishesRepository.GetActiveOrders();
                    temp = Cheques;
                    if (Cheques.Count == 0)
                    {
                        EmptyMessage = "Благодарим за работу!\nНо заказов пока что нет!";
                    }
                    else
                    {
                        EmptyMessage = string.Empty;
                    }
                    break;
            }
        }

        private void ExecuteChangeDishStatusCommand(object obj)
        {
            int dishIndex = (int)obj;

            // Обновляем статус блюда
            switch (temp[SelectedCheque].dishes[dishIndex].Status)
            {
                case 0:
                    temp[SelectedCheque].dishes[dishIndex].Status += 1;
                    dishesRepository.UpdateStatus(temp[SelectedCheque].dishes[dishIndex]);
                    UpdateLists(0);
                    break;

                case 1:
                    temp[SelectedCheque].dishes[dishIndex].Status += 1;

                    if (temp[SelectedCheque].dishes[dishIndex].Count > 1)
                    {
                        temp[SelectedCheque].dishes[dishIndex].Count -= 1;
                        temp[SelectedCheque].dishes[dishIndex].Status = 0;
                    }
                    else if (temp[SelectedCheque].dishes[dishIndex].Count == 1)
                    {
                        temp[SelectedCheque].dishes.RemoveAt(dishIndex);
                    }

                    // Проверяем, готов ли заказ
                    bool isChequeReady = true;

                    foreach (var item in temp[SelectedCheque].dishes)
                    {
                        if (item.Status != 2)
                        {
                            isChequeReady = false;
                            break;
                        }
                    }

                    if (isChequeReady) // Если заказ готов
                    {
                        var orderId = temp[SelectedCheque].Id; // Получаем Id заказа
                        dishesRepository.IsOrderReady(temp[SelectedCheque]);
                        _notifier.ShowSuccess($"Заказ №{orderId} готов к выдаче!"); // Уведомление с номером заказа
                        UpdateLists(1); // Обновление списков
                    }
                    else
                    {
                        UpdateLists(0); // Заказ еще не готов
                    }
                    break;

                case 2:
                    // Если статус 2, ничего не делаем
                    break;
            }
        }


    }
}
