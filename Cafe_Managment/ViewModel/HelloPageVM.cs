using Cafe_Managment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe_Managment.ViewModel
{
    public class HelloPageVM
    {
        private string _name;

        public string Name 
        { 
            get { return _name; } 
            set { _name = value; } 
        }

        public HelloPageVM() 
        {
            Name = UserData.Name + "!";
        }
    }

}
