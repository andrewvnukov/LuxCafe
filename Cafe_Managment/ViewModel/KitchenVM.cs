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

namespace Cafe_Managment.ViewModel
{
    internal class KitchenVM : ViewModelBase
    {
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
            dishesRepository = new DishesRepository();

            ChangeDishStatusCommand = new RelayCommand(ExecuteChangeDishStatusCommand);
            ReloadCommand = new RelayCommand(ExecuteReloadCommand);
            UpdateLists(1);
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
            if (temp[SelectedCheque].dishes[dishIndex].Status == 0)
            {
                temp[SelectedCheque].dishes[dishIndex].Status += 1;
                dishesRepository.UpdateStatus(temp[SelectedCheque].dishes[dishIndex]);
                UpdateLists(0);
                
            }
            else
            {
                temp[SelectedCheque].dishes[dishIndex].Status += 1;
                bool IsChequeReady = true;
                foreach (var item in temp[SelectedCheque].dishes) 
                {
                    if (item.Status == 2) IsChequeReady = true;
                    else { IsChequeReady = false; break; }
                }
                if (IsChequeReady)
                {
                    dishesRepository.DoOrderReady(temp[SelectedCheque]);

                    UpdateLists(1);
                }
                else
                {
                    UpdateLists(0);
                }
            }
        }
    }
}
