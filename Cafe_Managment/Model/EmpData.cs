using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Cafe_Managment.Model
{
    public class EmpData
    {
        public string Role { get; set; }
        public string Status { get; set; }
        public string FullName {  get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string BirthDay { get; set; }
        public string Address { get; set; }
        public BitmapImage ProfileImage { get; set; }
    }
}
