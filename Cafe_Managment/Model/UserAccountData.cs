using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe_Managment.Model
{
    public class UserAccountData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronomic { get; set; }
        public string Adress {  get; set; }
        public string Post { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }
        public byte[] ProfileImage { get; set; }
        
    }
}
