using ProjectManager.Models;
using ProjectManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ProjectManager.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProjectPage.xaml
    /// </summary>
    public partial class ProjectPage : Page
    {
        public DbService DbService { get; set; } = new DbService();

        public Project Project { get; set; }

        public ObservableCollection<Models.Task> WaitingTasks { get; set; } = new();
        public ObservableCollection<Models.Task> ActiveTasks { get; set; } = new();
        public ObservableCollection<Models.Task> CompletedTasks { get; set; } = new();

        public ObservableCollection<TasksHistory> TasksHistories { get; set; } = new();
        public ObservableCollection<ProjectUser> ProjectUsers { get; set; } = new();

        public int TasksCount => Project?.Tasks?.Count ?? 0;
        public int ProjectUsersCount => ProjectUsers.Count;

        private User CurrentUser => Models.CurrentUser.User; 

        public ProjectPage(Project project)
        {
            Project = project;
            InitializeComponent();
            DbService.LoadProjectTasks(Project);

            if (Project == null)
            {
                MessageBox.Show("Проект не найден.");
                NavigationService?.Navigate(new MainPage());
                return;
            }
            RefreshProjectData();
        }

        private void RefreshProjectData()
        {
            var projectTasks = Project.Tasks ?? new List<Models.Task>();
            WaitingTasks.Clear();
            foreach (var task in projectTasks.Where(t => t.LastVersion?.Data?.Status?.Title == "Waiting"))
                WaitingTasks.Add(task);

            ActiveTasks.Clear();
            foreach (var task in projectTasks.Where(t => t.LastVersion?.Data?.Status?.Title == "Active"))
                ActiveTasks.Add(task);

            CompletedTasks.Clear();
            foreach (var task in projectTasks.Where(t => t.LastVersion?.Data?.Status?.Title == "Completed"))
                CompletedTasks.Add(task);
            ProjectUsers.Clear();
            foreach (var user in Project.ProjectUsers ?? new List<ProjectUser>())
                ProjectUsers.Add(user);
            var histories = projectTasks
                .SelectMany(t => t.TasksHistories ?? new List<TasksHistory>())
                .OrderByDescending(h => h.CreatedAt)
                .ToList();
            TasksHistories.Clear();
            foreach (var h in histories)
                TasksHistories.Add(h);
        }

        private void AddTask(object sender, RoutedEventArgs e)
        {
            var popupWindow = new EditTaskWindow(Project);
            popupWindow.Owner = Window.GetWindow(this);
            popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            popupWindow.ShowDialog();

            DbService.Commit();
            DbService.LoadProjectTasks(Project);
            RefreshProjectData();

        }

        private void EditTask(object sender, RoutedEventArgs e)
        {
            if (sender is Border border && border.Tag is Models.Task task)
            {
                var popupWindow = new EditTaskWindow(Project);
                popupWindow.Owner = Window.GetWindow(this);
                popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                popupWindow.ShowDialog();

                DbService.Commit();
                DbService.LoadProjectTasks(Project);
                RefreshProjectData();
            }
        }

        private void RemoveTask(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Models.Task task)
            {
                if (MessageBox.Show("Вы действительно хотите удалить запись?", "Удалить?",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DbService.Tasks.Remove(task);
                    DbService.Commit();
                    DbService.LoadProjectTasks(Project);
                    RefreshProjectData();
                }
            }
        }

        private void ShowProjectUsers(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ProjectUsersPage(Project));
        }

        private void BackToMain(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void ProjectsPage(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MainPage());
        }

        private void OnTaskClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is Models.Task task)
            {
                NavigationService?.Navigate(new TaskPage(task));
            }
        }

        private void OnUserClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is ProjectUser projectUser)
            {
                NavigationService?.Navigate(new ProjectUserPage(projectUser));
            }
        }

        private void LeftHistoryClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is TasksHistory history && history.Task != null)
            {
                NavigationService?.Navigate(new TaskPage(history.Task));
            }
        }

        private void RightHistoryClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is TasksHistory history && history.Projectuser != null)
            {
                NavigationService?.Navigate(new ProjectUserPage(history.Projectuser));
            }
        }
    }
}