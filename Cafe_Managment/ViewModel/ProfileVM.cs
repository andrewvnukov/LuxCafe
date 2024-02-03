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

        public string Fullname;

        public EmpData CurrentData
        {
            get { return _currentData; }
            set { _currentData = value; OnPropertyChanged(); }
        }
        


        public ProfileVM()
        {
            UserRepository userRepository = new UserRepository();

            CurrentData = new EmpData
            {
                Role = userRepository.GetRoleById(UserData.RoleId),
                Status = "Работает",
                Patronomic = UserData.Patronomic,
                PhoneNumber = UserData.PhoneNumber,
                Email = UserData.Email,
                BirthDay = UserData.BirthDay,
                Address = UserData.Address,
                ProfileImage = UserData.ProfileImage
            };
        }
    }
}
