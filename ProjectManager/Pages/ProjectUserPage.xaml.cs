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
    public partial class ProjectUserPage : Page
    {
        public DbService DbService { get; set; } = new DbService();

        public ProjectUser ProjectUser { get; set; }
        public ObservableCollection<TasksHistory> TasksHistories { get; set; } = new();
        public ObservableCollection<Comment> Comments { get; set; } = new();
        public ObservableCollection<Models.Task> WaitingTasks { get; set; } = new();
        public ObservableCollection<Models.Task> ActiveTasks { get; set; } = new();
        public ObservableCollection<Models.Task> CompletedTasks { get; set; } = new();

        public string HistorySearchQuery { get; set; } = null!;
        public ICollectionView historiesView { get; set; }

        public int CommentsCount => Comments?.Count ?? 0;

        public int SelectedRoleId
        {
            get => ProjectUser?.RoleId ?? 0;
            set
            {
                if (ProjectUser == null || value == ProjectUser.RoleId) return;

                var newRole = DbService.Roles.FirstOrDefault(r => r.Id == value);
                if (newRole != null)
                {
                    ProjectUser.Role = newRole;
                    ProjectUser.RoleId = newRole.Id;

                    DbService.Commit();
                }
            }
        }

        public ProjectUserPage(ProjectUser projectUser)
        {
            DbService.GetAll();

            if (projectUser == null)
                throw new ArgumentNullException(nameof(projectUser));

            ProjectUser = projectUser;

            var sortedHistories = projectUser.TasksHistories
                .OrderByDescending(h => h.CreatedAt)
                .ToList();

            TasksHistories.Clear();
            foreach (var h in sortedHistories)
                TasksHistories.Add(h);
            Comments.Clear();
            foreach (var c in projectUser.Comments)
                Comments.Add(c);

            RefreshTasks();

            historiesView = CollectionViewSource.GetDefaultView(TasksHistories);
            historiesView.Filter = FilterHistories;

            InitializeComponent();

            ProjectUser currentUser = Models.CurrentUser.ProjectUserByProject(projectUser.Project);
            if (currentUser.Role.Title != "manager")
            {
                ActionGrid.Visibility = Visibility.Collapsed;
            }

            DataContext = this;
        }

        public bool FilterHistories(object obj)
        {
            if (obj is not TasksHistory)
                return false;

            var temp = (TasksHistory)obj;

            if (HistorySearchQuery != null && !temp.DisplayText.Contains(HistorySearchQuery, StringComparison.CurrentCultureIgnoreCase))
                return false;
            return true;
        }

        private void RefreshTasks()
        {
            WaitingTasks.Clear();
            ActiveTasks.Clear();
            CompletedTasks.Clear();

            var assignedTasks = ProjectUser.TaskInformations
                .Select(ti => ti.TasksHistories
                    .OrderByDescending(h => h.CreatedAt)
                    .FirstOrDefault())
                .Where(th => th != null)
                .Select(th => th.Task)
                .Distinct()                     
                .ToList();

            foreach (var task in assignedTasks)
            {
                var status = task.LastVersion?.Data?.Status?.Title;

                if (status == "Waiting") WaitingTasks.Add(task);
                else if (status == "Active") ActiveTasks.Add(task);
                else if (status == "Completed") CompletedTasks.Add(task);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void GoToProjects_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MainPage());
        }

        private void GoToProjectUsers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ProjectUsersPage(ProjectUser.Project));
        }

        private void RemoveUserFromProject_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(
                "Удалить пользователя из проекта?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            DbService.ProjectUsers.Remove(ProjectUser);
            DbService.Commit();

            NavigationService?.Navigate(new ProjectUsersPage(ProjectUser.Project));
        }

        private void HistoryItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border border) return;
            if (border.Tag is not TasksHistory history) return;

            NavigationService?.Navigate(new TaskPage(history.Task));
        }

        private void CommentItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border border) return;
            if (border.Tag is not Comment comment) return;

            NavigationService?.Navigate(new TaskPage(comment.Task));
        }

        private void TaskCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border border) return;
            if (border.Tag is not Models.Task task) return;

            NavigationService?.Navigate(new TaskPage(task));
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //foreach (var role in DbService.Roles)
            //{
            //    if (role.Id == RoleId)
            //    {
            //        projectUser.Role = role;
            //        break;
            //    }
            //}
            //projectUsersService.Commit();
        }

        private void historytBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            historiesView.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            historytBox.Text = string.Empty;
            if (HistorySearchQuery != null)
                HistorySearchQuery = string.Empty;
            historiesView.Refresh();
        }
    }
}