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
        UserRepository userRepository;

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

        public EmpData CurrentData { get { return _currentData; }
            set { _currentData = value; OnPropertyChanged(nameof(CurrentData)); }
        }



        public string Fullname
        {
            get { return _fullname; }
            set { _fullname = value; OnPropertyChanged(nameof(Fullname)); }
        }


        public ProfileVM()
        {
            userRepository = new UserRepository();

            _isAdressReadOnly=false;
            _isEmailReadOnly=false;
            _isNumberReadOnly=true;

            CurrentData = new EmpData
            {
                Address = UserData.Address,
                Email = UserData.Email,
                BirthDay = UserData.BirthDay,
                PhoneNumber = UserData.PhoneNumber,
                CreatedAt = UserData.CreatedAt,
                ProfileImage = UserData.ProfileImage,
            };

            EditAdress = new RelayCommand(ExecuteEditAdress);
            EditEmail = new RelayCommand(ExecuteEditEmail); 
            EditNumber = new RelayCommand(ExecuteEditNumber);
            EditPicture = new RelayCommand(ExexuteEditPicture);
            
            Role = userRepository.GetRoleById(UserData.RoleId);
            Branch = userRepository.GetBranchById(UserData.BranchId);
            Fullname = $"{UserData.Surname} {UserData.Name} {UserData.Patronomic}";
        }

        private void ExecuteEditAdress(object obj)
        {
            //if (IsAdressReadOnly)
            //{
            //    IsAdressReadOnly = !IsAdressReadOnly;
            //}
            //else
            //{
            //userRepository.EditCurrentUser(nameof(EmpData.Address), UserData.Address);
               MessageBox.Show(CurrentData.Address);
            //    IsAdressReadOnly = !IsAdressReadOnly;
            //}
        
        }

        private void ExecuteEditEmail(object obj)
        {
            MessageBox.Show(CurrentData.Email);
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
