using Cafe_Managment.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
namespace Cafe_Managment.Model
{
    public interface IDeletedDataRepository
    {
        DataTable GetDismissedEmployees();
        DataTable GetAllDeletedDishes();
        DataTable RestoreDeletedDish(DishData dish);
    }
}
