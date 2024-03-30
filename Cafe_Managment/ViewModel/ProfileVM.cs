using Cafe_Managment.Model;
using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Cafe_Managment.ViewModel
{
    internal class ProfileVM : ViewModelBase
    {

        private EmpData _currentData;
        private string _fullname;
        private bool _isAdressReadOnly;
        private bool _isEmailReadOnly;
        private bool _isNumberReadOnly;

        public ICommand EditAdress { get; set; }
        public ICommand EditEmail { get; set; }
        public ICommand EditNumber { get; set; }
        public ICommand EditPicture { get; set; }


        public string Role {get; set;}
        public string Branch { get; set;}

        public bool IsAdressReadOnly
        {
            get { return _isAdressReadOnly; }
            set { _isAdressReadOnly = value;
            OnPropertyChanged(nameof(IsAdressReadOnly));}
        }

        public bool IsEmailReadOnly
        {
            get { return _isEmailReadOnly; }
            set
            {
                _isEmailReadOnly = value;
                OnPropertyChanged(nameof(IsEmailReadOnly));
            }
        }

        public bool IsNumberReadOnly
        {
            get { return _isNumberReadOnly; }
            set
            {
                _isNumberReadOnly = value;
                OnPropertyChanged(nameof(IsNumberReadOnly));
            }
        }

        public EmpData CurrentData
        {
            get { return _currentData; }
            set { _currentData = value; OnPropertyChanged(); }
        }
        
        public string Fullname
        {
            get { return _fullname; }
            set { _fullname = value; OnPropertyChanged(); }
        }


        public ProfileVM()
        {
            UserRepository userRepository = new UserRepository();

            _isAdressReadOnly = true;
            _isEmailReadOnly=true;
            _isNumberReadOnly=true;

            EditAdress = new RelayCommand(ExecuteEditAdress);
            EditEmail = new RelayCommand(ExecuteEditEmail); 
            EditNumber = new RelayCommand(ExecuteEditNumber);
            EditPicture = new RelayCommand(ExexuteEditPicture);
            
            CurrentData = new EmpData
            {
                Status = "Работает",
                Name = UserData.Name,
                Surname = UserData.Surname,
                Patronomic = UserData.Patronomic,
                PhoneNumber = UserData.PhoneNumber,
                Email = UserData.Email,
                BirthDay = UserData.BirthDay,
                Address = UserData.Address,
                CreatedAt = UserData.CreatedAt,
                ProfileImage = UserData.ProfileImage
            };
            Role = userRepository.GetRoleById(UserData.RoleId);
            Branch = userRepository.GetBranchById(UserData.BranchId);
            Fullname = $"{CurrentData.Surname} {CurrentData.Name} {CurrentData.Patronomic}";
        }

        private void ExecuteEditAdress(object obj)
        {
            if (IsAdressReadOnly)
            {
                IsAdressReadOnly = !IsAdressReadOnly;
            }
            else
            {
                MessageBox.Show(nameof(CurrentData.Address));
                IsAdressReadOnly = !IsAdressReadOnly;
            }
        
        }

        private void ExecuteEditEmail(object obj)
        {
            throw new NotImplementedException();
        }

        private void ExecuteEditNumber(object obj)
        {
            throw new NotImplementedException();
        }

        private void ExexuteEditPicture(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
