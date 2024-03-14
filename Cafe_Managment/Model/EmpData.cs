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
    public class EmpData
    {
        public int Branch { get; set; }
        public int Role { get; set; }
        public string Status { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name {  get; set; }
        public string Surname {  get; set; }
        public string Patronomic {  get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string BirthDay { get; set; }
        public string Address { get; set; }
        public BitmapImage ProfileImage { get; set; }
    }
}
