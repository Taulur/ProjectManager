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
        public AuthPage()
        {
            data.GetAll();
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            foreach (User user in data.Users)
            {
                if (usernameBox.Text == user.Username)
                {
                    if (passwordBox.Password == user.Password)
                    {
                       
                        CurrentUser.User = user;
                        NavigationService.Navigate(new MainPage());
                    }
                    else
                    {
                        MessageBox.Show("пароль неоч");
                    }
                }
              
            }
        }
    }
}
