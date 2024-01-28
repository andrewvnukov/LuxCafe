using Cafe_Managment.Model;
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
        private BitmapImage _profilePicture;

        public BitmapImage ProfilePicture
        {
            get { return _profilePicture; }
            set { _profilePicture = value; OnPropertyChanged(nameof(ProfilePicture)); }
        }


        public ProfileVM()
        {
            ProfilePicture = UserData.ProfileImage;
        }
    }
}
