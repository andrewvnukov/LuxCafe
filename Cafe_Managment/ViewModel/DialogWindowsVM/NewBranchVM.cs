using Cafe_Managment.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cafe_Managment.ViewModel.DialogWindowsVM
{
    class NewBranchVM : ViewModelBase
    {
        private bool _isViewVisible;

        public bool IsViewVisible
        {
            get { return _isViewVisible; }
            set
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }

        public ICommand SaveChanges { get; set; }

        public NewBranchVM()
        {
            _isViewVisible = true;

            SaveChanges = new RelayCommand(ExecuteSaveChanges);
        }

        private void ExecuteSaveChanges(object obj)
        {
            IsViewVisible = false;
        }

    }
}
