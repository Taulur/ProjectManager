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
using ProjectManager.Services;
using ProjectManager.Models;

namespace ProjectManager.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditUserPage.xaml
    /// </summary>
    public partial class EditUserPage : Page
    {
        private UsersServices _service = new();
        public User user { get; set; } = new();
        public int roleId { get; set; } = new();
        public SystemRolesService RolesService { get; set; } = new();
        bool isEdit = false;
        public EditUserPage(User? _editUser = null)
        {

            if (_editUser != null)
            {
                user = _editUser;
                isEdit = true;
            }
            if (user.Systemrole == null)
                user.Systemrole = new();
            InitializeComponent();
        }
        private void save(object sender, RoutedEventArgs e)
        {
            if (isEdit)
                _service.Commit();
            else
            {

                user.CreatedAt = DateTime.Now;


                foreach (var role in RolesService.SystemRoles)
                {
                    if (role.Id == roleId)
                    {
                        user.Systemrole = role;
                        break;
                    }
                }
             
                _service.Add(user);
            }
            NavigationService.GoBack();
        }
        private void back(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
