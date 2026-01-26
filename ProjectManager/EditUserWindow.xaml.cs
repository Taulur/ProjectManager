using ProjectManager.Models;
using ProjectManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectManager
{
    public partial class EditUserWindow : Window
    {
        public DbService DbService { get; set; } = new();
        public User EditUser { get; set; } = new();
        public User User { get; set; } = new();
        public string ConfirmPassword { get; set; }
        bool isEdit = false;

        public int SelectedRoleId
        {
            get => User?.SystemroleId ?? 0;
            set
            {
                if (User == null || value == User.SystemroleId) return;

                var newRole = DbService.SystemRoles.FirstOrDefault(r => r.Id == value);
                if (newRole != null)
                {
                    User.Systemrole = newRole;
                    User.SystemroleId = newRole.Id;
                    DbService.Commit();
                }
            }
        }

        public EditUserWindow(User? _editUser = null)
        {
            if (_editUser != null)
            {
                EditUser = _editUser;

                isEdit = true;

                User.Username = _editUser.Username;
                User.Fullname = _editUser.Fullname;
                User.Password = _editUser.Password;
                ConfirmPassword = _editUser.Password;
                User.CreatedAt = _editUser.CreatedAt;
                User.Systemrole = _editUser.Systemrole;


            }
            InitializeComponent();
            DbService.GetAll();
            DataContext = this;
        }
        private void Execute(object sender, RoutedEventArgs e)
        {
            bool isExist = false;
            if (User.Username != null &&  User.Password != null && User.Fullname != null && User.Systemrole != null)
            {
               
                foreach (User user in DbService.Users)
                {
                    if (User.Username == user.Username)
                    {
                        isExist = true;
                        break;
                    }
                }

                if (!isExist || isEdit)
                {

                    if (User.Password == ConfirmPassword)
                    {
                        if (isEdit)
                        {
                            EditUser.Username = User.Username;
                            EditUser.Fullname = User.Fullname;
                            EditUser.Password = User.Password;
                            EditUser.CreatedAt = User.CreatedAt;
                            EditUser.Systemrole = User.Systemrole;
                            DbService.Commit();

                            
                        }
                        else
                        {
                            User.CreatedAt = DateTime.Now;

                            DbService.Add(User);
                            DbService.Commit();
                       

                        }
                        this.Close();
                    }
                    else
                    {
                        Snackbar("Пароль не совпадают");
                    }
                }
                else
                {
                    Snackbar("Такой пользователь уже существует");
                }

            }
            else
            {
                Snackbar("Выполните все условия");
            }


        }

        void Snackbar(string message)
        {
            MainSnackbar.MessageQueue?.Enqueue(
                        message,
                        null,
                        null,
                        null,
                        false,
                        true,
                        TimeSpan.FromSeconds(0.5));
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
  "Вы уверены? Все изменения сбросятся",
  "Подтверждение выхода",
  MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

    }
}
