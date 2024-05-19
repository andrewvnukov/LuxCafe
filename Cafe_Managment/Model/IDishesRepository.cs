using Cafe_Managment.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cafe_Managment.Model
{
    public interface IDishesRepository
    {
        List<string> GetAllCategories();
        DataTable GetAllDishesFromArchive();
        DataTable GetAllDishesFromMenu();
        void UpdateDish(DishData dish);
        void DeleteDish(DishData dish);
        void TransferDishToActiveMenu(DishData dish);
        List<DishData> GetDishListByCategory(int CatId);
        ObservableCollection<ChequeModel> GetActiveOrders();
        int CreateNewOrder(List<DishData> dishList, int spot, int guestCount, float totalPrice);
        void UpdateStatus(DishData dish);
        void IsOrderReady(ChequeModel cheque);
        DataTable RestoreDeletedDish(DishData dish);
        DataTable GetAllDeletedDishes();
        void AddDishToArchive(DishData dishData);

    }
}
