using Cafe_Managment.Model;
using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using Cafe_Managment.View.DialogWindows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Cafe_Managment.ViewModel
{

    internal class DeletedDataVM : ViewModelBase
    {
        private bool _isReadOnly;

        DeletedDataRepository deletedDataRepository;

        private DataTable _dismissedEmployees;

        public DataTable DismissedEmployees
        {
            get { return _dismissedEmployees; }
            set { _dismissedEmployees = value; }
        }
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        public DeletedDataVM()
        {
            deletedDataRepository = new DeletedDataRepository();

            deletedDataRepository.GetDismissedEmployees();

            IsReadOnly = false;

        }

    }
}
