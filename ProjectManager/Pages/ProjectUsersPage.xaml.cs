using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ProjectManager.Services;

namespace ProjectManager.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProjectUsersPage.xaml
    /// </summary>
    public partial class ProjectUsersPage : Page
    {
        public Project project { get; set; } = new();
        public ObservableCollection<ProjectUser> projectUsers { get; set; } = new();
        public ObservableCollection<User> users { get; set; } = new();
        public UsersServices usersServices { get; set; } = new();
        public ProjectUsersService projectUsersService { get; set; } = new();
        public RolesService rolesService { get; set; } = new();
        public int RoleId { get; set; } = new();
        public ProjectUsersPage(Project _project)
        {
            project = _project;
            usersServices.GetAll(project);
            foreach (var projectUser in project.ProjectUsers)
            {
                projectUsers.Add(projectUser);
            }
            InitializeComponent();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;
            ProjectUser temp = combobox.Tag as ProjectUser;


            foreach (var role in rolesService.Roles)
            {
                if (role.Id == RoleId)
                {
                    temp.Role = role;
                    break;
                }
            }
            usersServices.Commit();
        }

        private void projects(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void back(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProjectPage(project));
        }

        private void remove(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            ProjectUser temp = button.Tag as ProjectUser;
            projectUsersService.Remove(temp);
            projectUsersService.GetAll(project);
            projectUsers.Clear();
            foreach (var projectUser in project.ProjectUsers)
            {
                projectUsers.Add(projectUser);
            }
            usersServices.GetAll(project);
        }

        private void add(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            User temp = button.Tag as User;

            ProjectUser newUser = new ProjectUser
            {
                User = temp,
                Project = project,
                Role = rolesService.Roles[0],
                CreatedAt = DateTime.Now,
            };

            projectUsersService.Add(newUser);
            projectUsersService.GetAll(project);
            projectUsers.Clear();
            foreach (var projectUser in project.ProjectUsers)
            {
                projectUsers.Add(projectUser);
            }
            usersServices.GetAll(project);

        }
    }
}
