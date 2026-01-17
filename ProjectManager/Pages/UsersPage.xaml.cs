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
    /// Логика взаимодействия для TeamPage.xaml
    /// </summary>
    public partial class TeamPage : Page
    {
        public UsersServices UsersServices { get; set; } = new();
        public User? user { get; set; } = null;

        public TeamPage()
        {
            InitializeComponent();
        }
        public void add(object sender, EventArgs e)
        {
            NavigationService.Navigate(new EditUserPage());
        }
        public void back(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
        public void edit(object sender, EventArgs e)
        {
            var button = sender as Button;
            user = button?.Tag as User;
            if (user != null) 
            {
                NavigationService.Navigate(new EditUserPage(user));
            }
        }
        public void remove(object sender, EventArgs e)
        {
            var button = sender as Button;
            user = button?.Tag as User;
            if (user != null)
            {
                if (MessageBox.Show("Вы действительно хотите удалить запись?", "Удалить?",
           MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    UsersServices.Remove(user);
                }
            }
               
        }

    }
}
