using Cafe_Managment.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Cafe_Managment.ViewModel
{
    internal class ProfileVM
    {
        private BitmapImage _profilePicture;

        public BitmapImage ProfilePicture
        {
            get { return _profilePicture; }
            set { _profilePicture = value; }
        }


        public ProfileVM()
        {
            ProfilePicture = UserData.ProfileImage;
        }
    }
}
