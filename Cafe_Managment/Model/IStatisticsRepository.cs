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
    public interface IStatisticsRepository
    {
        Dictionary<DateTime, double> GetProfitForTimePeriod(DateTime startDate, DateTime endDate);
        Dictionary<string, double> GetDishDataFromDatabase();
        Dictionary<string, double> GetUnpopularDishes();
        Dictionary<string, double> GetPopularDishes();
        IEnumerable<DishData> GetAllDishes();
    }
}
