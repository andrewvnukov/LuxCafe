using Cafe_Managment.Model;
using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe_Managment.ViewModel
{
    internal class KitchenVM : ViewModelBase
    {
        List<ChequeModel> _cheques;

        public List<ChequeModel> Cheques
        {
            get { return _cheques; }
            set { _cheques = value; 
            OnPropertyChanged(nameof(Cheques));}
        }

        public KitchenVM() 
        {
            DishesRepository dishesRepository = new DishesRepository();
            Cheques = dishesRepository.GetActiveOrders();
        }
    }
}
