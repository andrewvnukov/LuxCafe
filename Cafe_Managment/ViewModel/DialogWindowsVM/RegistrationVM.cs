using Cafe_Managment.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cafe_Managment.ViewModel.DialogWindowsVM
{
    public class RegistrationVM : ViewModelBase
    {
        private bool _isViewVisible = true;
        private object _activePage;

        public bool IsViewVisible
        {
            get { return _isViewVisible; }
            set { _isViewVisible = value; OnPropertyChanged(); }
        }

        public object ActivePage
        {
            get => _activePage;
            set
            {
                _activePage = value;
                OnPropertyChanged();
            }
        }

        public ICommand CloseWindowCommand { get; set; }


        public RegistrationVM() 
        {
            CloseWindowCommand = new RelayCommand(ExecuteCloseWindowCommand);
        }

        private void ExecuteCloseWindowCommand(object obj)
        {
            IsViewVisible = false;
        }
    }
}
