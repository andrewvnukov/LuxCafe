using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cafe_Managment.ViewModel
{
    public class LoadingVM : ViewModelBase
    {
        private bool _isVisible = true;

        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; OnPropertyChanged(nameof(IsVisible)); }
        }

        UserRepository userRepository;
        public LoadingVM() 
        {
            userRepository = new UserRepository();

            LoadCurrentUserData();
            
        }

        private void LoadCurrentUserData()
        {
            userRepository.GetById();
            MessageBox.Show("Yes");
        }
    }
}
