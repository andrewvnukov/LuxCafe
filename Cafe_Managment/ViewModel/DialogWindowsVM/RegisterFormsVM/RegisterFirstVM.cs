using Cafe_Managment.Model;
using Cafe_Managment.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Cafe_Managment.ViewModel.DialogWindowsVM.RegisterFormsVM
{
    public class RegisterFirstVM : ViewModelBase
    {
        private bool _isVisible = true;
        private EmpData _regData;

        public delegate void EventDelegate(bool s);
        public event EventDelegate IfFinished=null;

        public EmpData RegData
        {
            get { return _regData; }
            set { _regData = value; OnPropertyChanged(nameof(RegData)); }
        }
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; OnPropertyChanged(); }
        }

        public ICommand GoNextCommand { get; set; }

        public RegisterFirstVM() 
        {
            GoNextCommand = new RelayCommand(ExecuteGoNextCommand);

        }
        public RegisterFirstVM(ref EmpData tempData) 
        {
            RegData = tempData;
            GoNextCommand = new RelayCommand(ExecuteGoNextCommand);

        }

        private void ExecuteGoNextCommand(object obj)
        {
            IfFinished.Invoke(true);
        }

        
    }
}
