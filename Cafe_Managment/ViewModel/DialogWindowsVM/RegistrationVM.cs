using Cafe_Managment.Model;
using Cafe_Managment.Utilities;
using Cafe_Managment.View.DialogWindows.RegisterForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Cafe_Managment.ViewModel.DialogWindowsVM
{
    public class RegistrationVM : ViewModelBase
    {
        private bool _isViewVisible = true;
        private object _activePage;
        private EmpData _empdata;

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

        public EmpData Empdata
        {
            get { return _empdata; }
            set { _empdata = value; OnPropertyChanged(nameof(Empdata)); }
        }

        public ICommand CloseWindowCommand { get; set; }
        public ICommand GoNextCommand { get; set; }


        public RegistrationVM() 
        {
            ActivePage = new RegisterFirst();

            GoNextCommand = new RelayCommand(ExecuteGoNextCommand);

            CloseWindowCommand = new RelayCommand(ExecuteCloseWindowCommand);
        }

        private void ExecuteGoNextCommand(object obj)
        {
            try
            {
                MessageBox.Show(Empdata.Name + "\n" + Empdata.Surname + "\n" + Empdata.Patronomic + "\n" + Empdata.PhoneNumber);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            } 
     
        }

        private void ExecuteCloseWindowCommand(object obj)
        {
            IsViewVisible = false;
        }
    }
}
