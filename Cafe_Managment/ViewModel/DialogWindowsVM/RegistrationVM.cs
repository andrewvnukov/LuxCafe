using Cafe_Managment.Model;
using Cafe_Managment.Utilities;
using Cafe_Managment.View.DialogWindows.RegisterForms;
using Cafe_Managment.ViewModel.DialogWindowsVM.RegisterFormsVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows;
using System.Windows.Input;

namespace Cafe_Managment.ViewModel.DialogWindowsVM
{
    public class RegistrationVM : ViewModelBase
    {
        private bool _isViewVisible = true;
        private object _activePage;
        EmpData empData = new EmpData();
        RegisterFirstVM firstPage;
        RegisterSecondVM secondPage;

        public ICommand NavigateToPageCommand { get; }

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
            NavigateToPageCommand = new RelayCommand(NavigateToPage);

            ActivePage = new RegisterFirst();

            CloseWindowCommand = new RelayCommand(ExecuteCloseWindowCommand);
        }

        private void NavigateToPage(object obj)
        {
            ActivePage = obj;
        }

        private void ExecuteCloseWindowCommand(object obj)
        {
            IsViewVisible = false;
        }


    }
}
