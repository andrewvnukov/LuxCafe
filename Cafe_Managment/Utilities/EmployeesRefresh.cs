using Cafe_Managment.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe_Managment.Utilities
{
    public class EmployeesRefresh 
    {
        public event Action OnDataRefresh;

        public void RaiseDataRefresh()
        {
            OnDataRefresh?.Invoke();
        }
    }
}