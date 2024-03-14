using Cafe_Managment.Model;
using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Cafe_Managment.ViewModel
{
    internal class ProfileVM : ViewModelBase
    {

        private EmpData _currentData;

        private string _fullname;

        public string Role;
        public EmpData CurrentData
        {
            get { return _currentData; }
            set { _currentData = value; OnPropertyChanged(); }
        }
        
        public string Fullname
        {
            get { return _fullname; }
            set { _fullname = value; OnPropertyChanged(); }
        }


        public ProfileVM()
        {
            UserRepository userRepository = new UserRepository();
            Role = userRepository.GetRoleById(UserData.RoleId);
            CurrentData = new EmpData
            {
                Status = "Работает",
                Name = UserData.Name,
                Surname = UserData.Surname,
                Patronomic = UserData.Patronomic,

                PhoneNumber = UserData.PhoneNumber,

                Email = UserData.Email,
                BirthDay = UserData.BirthDay,

                Address = UserData.Address,

                ProfileImage = UserData.ProfileImage
            };

            Fullname = $"{CurrentData.Surname} {CurrentData.Name} {CurrentData.Patronomic}";
        }
    }
}
