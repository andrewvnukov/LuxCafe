using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe_Managment.Model
{
    public class ChequeModel
    {
        public int Id { get; set; }
        public List<DishData> dishes { get; set; }
        public DateTime CreatedAt { get; set; } 
        public float TotalPrice { get; set; }
        public int SpotNumber { get; set; }
        public int GuestNumber { get; set;}
        public TimeSpan WaitingTime { get; internal set; }
    }
}
