using Cafe_Managment.Model;
using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ToastNotifications;
using ToastNotifications.Messages;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Cafe_Managment.View.DialogWindows;

namespace Cafe_Managment.ViewModel
{
    internal class ProfileVM : ViewModelBase
    {
        UserRepository userRepository;
        private Notifier _notifier;
        public NewBranch newBranch;
        private Window currentWindow; // Свойство для хранения ссылки на текущее окно

        private string _previousPhoneNumber;
        private string _previousEmail;
        private string _newBranch;
        private bool _isViewVisible;

        private EmpData _currentData;
        private string _fullname;
        private bool _isAddressReadOnly;
        private bool _isEmailReadOnly;
        private bool _isNumberReadOnly;

        public ICommand EditAddress { get; set; }
        public ICommand EditEmail { get; set; }
        public ICommand EditNumber { get; set; }
        public ICommand EditPicture { get; set; }
        public ICommand FormatNumber { get; set; }
       
        public ICommand SaveBranchCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }
        public ICommand ShowAddBranchCommand { get; set; }


        public string Role {get; set;}
        public string Branch { get; set;}

        private string _phoneNumber;


        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != value) // Проверяем, изменилось ли значение
                {
                    _phoneNumber = value;
                    OnPropertyChanged(nameof(PhoneNumber)); // Уведомляем об изменении
                }
            }
        }
        public string NewBranch
        {
            get { return _newBranch; }
            set
            {
                _newBranch = value;
                OnPropertyChanged(nameof(NewBranch));
            }
        }
        public bool IsViewVisible
        {
            get { return _isViewVisible; }
            set
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }
        private string _email;

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value) // Проверяем, изменилось ли значение
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email)); // Уведомляем об изменении
                }
            }
        }

        public bool IsAddressReadOnly
        {
            get { return _isAddressReadOnly; }
            set { _isAddressReadOnly = value;
            OnPropertyChanged(nameof(IsAddressReadOnly));}
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

        private string newBranchName;
        public string NewBranchName
        {
            get { return newBranchName; }
            set
            {
                newBranchName = value;
                OnPropertyChanged(nameof(NewBranchName));
            }
        }

        public string Fullname
        {
            get { return _fullname; }
            set { _fullname = value; OnPropertyChanged(nameof(Fullname)); }
        }

        public ProfileVM()
        {
            userRepository = new UserRepository();
            _notifier = CreateNotifier();

            _isAddressReadOnly = true;
            _isEmailReadOnly=true;
            _isNumberReadOnly=true;
            IsViewVisible = true;

            CurrentData = new EmpData
            {
                Name = UserData.Name,
                Surname = UserData.Surname,
                Patronomic = UserData.Patronomic,
                Address = UserData.Address,
                Email = UserData.Email,
                BirthDay = UserData.BirthDay,
                PhoneNumber = UserData.PhoneNumber,
                CreatedAt = UserData.CreatedAt,
                ProfileImage = UserData.ProfileImage,
            };

            EditAddress = new RelayCommand(ExecuteEditAddress);
            EditEmail = new RelayCommand(ExecuteEditEmail); 
            EditNumber = new RelayCommand(ExecuteEditNumber);
            EditPicture = new RelayCommand(ExecuteEditPicture);
            FormatNumber = new RelayCommand(ExecuteFormatNumber);
            SaveBranchCommand = new RelayCommand(ExecuteSaveBranchCommand, CanExecuteSaveBranchCommand);
            CloseWindowCommand = new RelayCommand(ExecuteCloseDialogCommand);
            ShowAddBranchCommand = new RelayCommand(ExecuteShowAddBranchCommand, CanAdministrate);

            Role = userRepository.GetRoleById(UserData.RoleId);
            Branch = userRepository.GetBranchById(UserData.BranchId);
            Fullname = $"{UserData.Surname} {UserData.Name} {UserData.Patronomic}";

            CurrentData.PhoneNumber = FormatPhoneNumber(CurrentData.PhoneNumber);
            _previousPhoneNumber = CurrentData.PhoneNumber;

            _previousEmail = CurrentData.Email;
 
        }

        private void ExecuteShowAddBranchCommand(object obj)
        {
            var newBranchWindow = new NewBranch();
            newBranchWindow.DataContext = this;
            currentWindow = newBranchWindow; 
            newBranchWindow.ShowDialog();
        }

        private void ExecuteCloseDialogCommand(object obj)
        {
            NewBranch = "";
            IsViewVisible = false;
            //ExecuteShowDishSuccessfullyTransferedCommand(null);
        }
        private bool CanAdministrate(object arg)
        {
            return (UserData.RoleId == 2 || UserData.RoleId == 7);
        }
        private void ExecuteSaveBranchCommand(object obj)
        {
            try
            {
                userRepository.AddBranch(NewBranchName);
                _notifier.ShowSuccess($"Филиал '{NewBranchName}' успешно добавлен.");
                currentWindow.Close(); // Закрываем окно после добавления филиала

            }
            catch (Exception ex)
            {
                _notifier.ShowError(ex.Message);
            }
        }
        private bool CanExecuteSaveBranchCommand(object arg)
        {
            return !string.IsNullOrWhiteSpace(NewBranchName);
        }

        public void ExecuteFormatNumber(object parameter)
        {
            FormatPhoneNumber(null);
        }
        public void UpdatePhoneNumber(string newPhoneNumber)
        {
            PhoneNumber = newPhoneNumber; 
        }
        private Notifier CreateNotifier()
        {
            return new Notifier(cfg =>
            {
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    TimeSpan.FromSeconds(5),
                    MaximumNotificationCount.FromCount(5));

                cfg.PositionProvider = new PrimaryScreenPositionProvider(
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.DisplayOptions.TopMost = true;
                cfg.DisplayOptions.Width = 300;

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        private bool IsValidAddress(string address)
        {
            string pattern = @"^[\w\s,.-]*$";
            return Regex.IsMatch(address, pattern);
        }

        private void ExecuteEditAddress(object obj)
        {
            if (IsAddressReadOnly)
            {
                IsAddressReadOnly = !IsAddressReadOnly;
            }
            else
            {
                if (IsValidAddress(CurrentData.Address))
                {
                    userRepository.EditCurrentUser(nameof(EmpData.Address), CurrentData.Address);
                    IsAddressReadOnly = !IsAddressReadOnly;
                    userRepository.GetById();
                    _notifier.ShowSuccess("Адрес успешно изменен!");
                }
                else
                {
                    _notifier.ShowError("Неверный формат адреса! Адрес не должен включать в себя специальные символы.");
                    CurrentData.Address = string.Empty;
                }
            }
        }

        private void ExecuteEditEmail(object obj)
        {
            string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$"; 
            Regex emailRegex = new Regex(emailPattern); 

            if (IsEmailReadOnly)
            {
                IsEmailReadOnly = !IsEmailReadOnly;
            }
            else 
            {
                if (!emailRegex.IsMatch(CurrentData.Email)) 
                {
                    _notifier.ShowWarning("Некорректный формат электронной почты. Убедитесь, что введенная почта правильная.");
                    CurrentData.Email = _previousEmail; 
                    OnPropertyChanged(nameof(CurrentData.Email)); 
                    IsEmailReadOnly = !IsEmailReadOnly;

                    return; 
                }

                
                userRepository.EditCurrentUser(nameof(EmpData.Email), CurrentData.Email); 
                IsEmailReadOnly = !IsEmailReadOnly; 
                userRepository.GetById(); 
                _notifier.ShowSuccess("Email успешно изменен!");
            }
        }

        private string FormatPhoneNumber(string rawNumber)
        {
            string digitsOnly = new string(rawNumber.Where(char.IsDigit).ToArray());

            // Если длина 11 символов и начинается с 8 или 7, приводим к нужному формату
            if (digitsOnly.Length == 11 && (digitsOnly.StartsWith("7") || digitsOnly.StartsWith("8")))
            {
                return $"+7 ({digitsOnly.Substring(1, 3)}) {digitsOnly.Substring(4, 3)}-{digitsOnly.Substring(7, 2)}-{digitsOnly.Substring(9, 2)}";
            }

            return rawNumber;
        }
        private void ExecuteEditNumber(object obj)
        {
            string phonePattern = @"^(?:\+7|8)[-\s]?\(?\d{3}\)?[-\s]?\d{3}[-\s]?\d{2}[-\s]?\d{2}$";
            Regex phoneRegex = new Regex(phonePattern);

            if (IsNumberReadOnly)
            {
                IsNumberReadOnly = !IsNumberReadOnly;
            }
            else
            {
                // Проверяем формат номера телефона
                if (!phoneRegex.IsMatch(CurrentData.PhoneNumber))
                {
                    _notifier.ShowWarning("Некорректный формат номера телефона.");
                    //Debug.WriteLine("sdv",_previousPhoneNumber);

                    CurrentData.PhoneNumber = _previousPhoneNumber; 
                    OnPropertyChanged(nameof(CurrentData.PhoneNumber)); 
                    IsNumberReadOnly = !IsNumberReadOnly;

                    return;
                }

                // Сохраняем текущее значение как предыдущее перед обновлением
                _previousPhoneNumber = CurrentData.PhoneNumber;

                // Выполняем обновление
                userRepository.EditCurrentUser(nameof(EmpData.PhoneNumber), CurrentData.PhoneNumber);
                IsNumberReadOnly = !IsNumberReadOnly;
                userRepository.GetById();

                CurrentData.PhoneNumber = FormatPhoneNumber(CurrentData.PhoneNumber); 

                _notifier.ShowSuccess("Номер телефона успешно изменен!");
            }
        }


        private void ExecuteEditPicture(object obj)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image files (*.BMP, *.JPG, *.GIF, *.TIF, *.PNG, *.ICO, *.EMF, *.WMF)|*.bmp;*.jpg;*.gif; *.tif; *.png; *.ico; *.emf; *.wmf";
            if (fileDialog.ShowDialog() == true)
            {
                string fileName = fileDialog.FileName;
                CurrentData.ProfileImage = new BitmapImage(new Uri(fileName));
                byte[] photoData = File.ReadAllBytes(fileDialog.FileName);
                userRepository.UpdateCurrentUserPicture(photoData);
                userRepository.GetById();
                _notifier.ShowSuccess("Фотография профиля успешно изменена!");
            }
        }
    }
}
