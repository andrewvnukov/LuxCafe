using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace Cafe_Managment.Model
{
    public class DishData
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Composition { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string BranchId { get; set; }
        public string Price { get; set; }
        public string ImageSource { get; set; }
        public int Count { get; set; }
        public int Status { get; set; }

    }
}
