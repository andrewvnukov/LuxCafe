using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe_Managment.Model
{
    public class UserAccountData
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public byte[] ProfileImage { get; set; }
        public string Post {  get; set; }
    }
}
