using ProjectManager.Models;
using ProjectManager.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ProjectManager.Pages
{
    public partial class ProjectUsersPage : Page
    {
        public DbService DbService { get; set; } = new DbService();

        public Project Project { get; set; }
        public ObservableCollection<ProjectUser> ProjectUsers { get; set; } = new();
        public ObservableCollection<User> AvailableUsers { get; set; } = new();

        public string UsersSearchQuery { get; set; } = null!;
        public ICollectionView usersView { get; set; }
        public ObservableCollection<string> UsersFilter { get; set; } = new();

        public string Users1SearchQuery { get; set; } = null!;
        public ICollectionView usersView1 { get; set; }

        public ProjectUsersPage(Project project)
        {
            DbService.GetAll();

            if (project == null)
                throw new ArgumentNullException(nameof(project));

            Project = project;

            RefreshProjectUsers();

            RefreshAvailableUsers();

            InitializeComponent();

            DataContext = this;
        }

        private void RefreshProjectUsers()
        {
            ProjectUsers.Clear();
            foreach (var pu in Project.ProjectUsers)
            {
                ProjectUsers.Add(pu);
            }

            UsersFilter.Add("По дате присоединения");
            UsersFilter.Add("По количеству задач");

            usersView = CollectionViewSource.GetDefaultView(ProjectUsers);
            usersView.Filter = FilterUsers;
        }

        public bool FilterUsers(object obj)
        {
            if (obj is not ProjectUser)
                return false;

            var temp = (ProjectUser)obj;

            if (UsersSearchQuery != null && !temp.User.Fullname.Contains(UsersSearchQuery, StringComparison.CurrentCultureIgnoreCase))
                return false;

            return true;
        }

        public bool FilterUsers1(object obj)
        {
            if (obj is not User)
                return false;

            var temp = (User)obj;

            if (UsersSearchQuery != null && !temp.Fullname.Contains(UsersSearchQuery, StringComparison.CurrentCultureIgnoreCase))
                return false;

            return true;
        }

        private void RefreshAvailableUsers()
        {
            var alreadyInProject = Project.ProjectUsers
                .Select(pu => pu.UserId)
                .ToHashSet();

            AvailableUsers.Clear();
            foreach (var user in DbService.Users)
            {
                if (!alreadyInProject.Contains(user.Id))
                {
                    AvailableUsers.Add(user);
                }
            }

            usersView1 = CollectionViewSource.GetDefaultView(AvailableUsers);
            usersView1.Filter = FilterUsers1;
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ComboBox comboBox) return;
            if (comboBox.Tag is not ProjectUser projectUser) return;
            if (e.AddedItems.Count == 0) return;

            if (comboBox.SelectedValue is int roleId)
            {
                var newRole = DbService.Roles.FirstOrDefault(r => r.Id == roleId);
                if (newRole != null && newRole != projectUser.Role)
                {
                    projectUser.Role = newRole;
                    projectUser.RoleId = newRole.Id;
                    DbService.Commit();

                }
            }
        }

        private void RemoveUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;
            if (button.Tag is not ProjectUser projectUser) return;

            if (MessageBox.Show(
                $"Удалить {projectUser.User?.Fullname ?? "пользователя"} из проекта?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            DbService.RemoveProjectUser(projectUser);
            DbService.Commit();

            RefreshProjectUsers();
            RefreshAvailableUsers(); 
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;
            if (button.Tag is not User user) return;

            if (Project.ProjectUsers.Any(pu => pu.UserId == user.Id))
                return;

            var defaultRole = DbService.Roles.FirstOrDefault(r => r.Title?.ToLower().Contains("member") == true)
                           ?? DbService.Roles.FirstOrDefault(); 

            var newProjectUser = new ProjectUser
            {
                UserId = user.Id,
                User = user,
                ProjectId = Project.Id,
                Project = Project,
                RoleId = defaultRole?.Id ?? 0,
                Role = defaultRole,
                CreatedAt = DateTime.UtcNow
            };

            Project.ProjectUsers.Add(newProjectUser);
            DbService.Commit();

            RefreshProjectUsers();
            RefreshAvailableUsers(); 
        }

        private void GoToProjects_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ProjectPage(Project));
        }

        private void usersBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            usersView.Refresh();
        }

        private void usersCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            usersView.SortDescriptions.Clear();
            var cb = (ComboBox)sender;
            if (cb.SelectedIndex != -1)
            {
                var selected = cb.SelectedItem.ToString();
                if (selected != null && selected != string.Empty)
                {
                    switch (selected)
                    {
                        case "По количеству задач":
                            usersView.SortDescriptions.Add(new SortDescription("TotalAssignedTasks",
                            ListSortDirection.Descending));
                            break;
                        case "По дате присоединения":
                            usersView.SortDescriptions.Add(new SortDescription("CreatedAt",
                            ListSortDirection.Ascending));
                            break;
                    }
                }
            }
            usersView.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            usersBox.Text = string.Empty;
            usersCombo.SelectedIndex = -1;
            if (UsersSearchQuery != null)
                UsersSearchQuery = string.Empty;
            usersView.Refresh();
        }

        private void usersBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            usersView1.Refresh();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            usersBox1.Text = string.Empty;
            if (UsersSearchQuery != null)
                UsersSearchQuery = string.Empty;
            usersView1.Refresh();
        }
    }
}