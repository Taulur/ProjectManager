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
using ProjectManager.Models;

namespace ProjectManager.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private DbService data { get; set; } = new();
        public string Username { get; set; }
        public string Password { get; set; }
        public AuthPage()
        {
            data.GetAll();
            InitializeComponent();
            DataContext = this;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           if (Username != null &&  Password != null)
            {
                foreach (User user in data.Users)
                {
                    if (Username == user.Username)
                    {
                        if (Password == user.Password)
                        {

                            CurrentUser.User = user;
                            NavigationService.Navigate(new MainPage());
                        }
                        else
                        {
                            Snackbar("Неверный пароль");
                        }
                    }
                    else
                    {
                        Snackbar("Пользователь не найден");
                    }

                }

            }
           else
            {
                Snackbar("Выполните все условия");
            }

            
        }
    }
}
