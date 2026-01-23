using ProjectManager.Models;
using ProjectManager.Services;
using System;
using System.Collections.Generic;
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

        public string UsersSearchQuery { get; set; } = null!;
        public ICollectionView usersView { get; set; }
        public ObservableCollection<string> UsersFilter { get; set; } = new();

        public string HistorySearchQuery { get; set; } = null!;
        public ICollectionView historiesView { get; set; }

        public string TasksSearchQuery { get; set; } = null!;
        public ICollectionView tasksWaitingView { get; set; }
        public ICollectionView tasksActiveView { get; set; }
        public ICollectionView tasksCompletedView { get; set; }
        public ObservableCollection<string> TasksFilter { get; set; } = new();


        public ProjectPage(Project project)
        {

            Project = project;

            InitializeComponent();

            ProjectUser currentUser = Models.CurrentUser.ProjectUserByProject(Project);
            if (currentUser.Role.Title != "manager")
            {
                AddButton.Visibility = Visibility.Collapsed;
                ProjectUsersButton.Visibility = Visibility.Collapsed;
            }

            DbService.LoadProjectTasks(Project);

         

            if (Project == null)
            {
                MessageBox.Show("Проект не найден.");
                NavigationService?.Navigate(new MainPage());
                return;
            }
            RefreshProjectData();

            UsersFilter.Add("По количеству задач");

            TasksFilter.Add("По приоритету");
            TasksFilter.Add("По дате выполнения");
            TasksFilter.Add("По дате изменения");
            TasksFilter.Add("По дате создания");

            DataContext = this;
            
        }

        public bool FilterTasks(object obj)
        {
            if (obj is not Models.Task)
                return false;

            var temp = (Models.Task)obj;

            if (TasksSearchQuery != null && !temp.LastVersion.DisplayTitle.Contains(TasksSearchQuery, StringComparison.CurrentCultureIgnoreCase))
                return false;

            return true;
        }

        private void tasksBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            tasksWaitingView.Refresh();
            tasksActiveView.Refresh();
            tasksCompletedView.Refresh();
        }

        private void tasksCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tasksWaitingView.SortDescriptions.Clear();
            tasksActiveView.SortDescriptions.Clear();
            tasksCompletedView.SortDescriptions.Clear();
            var cb = (ComboBox)sender;
            if (cb.SelectedIndex != -1)
            {
                var selected = cb.SelectedItem.ToString();
                if (selected != null && selected != string.Empty)
                {
                    switch (selected)
                    {
                        case "По приоритету":
                            tasksWaitingView.SortDescriptions.Add(new SortDescription("LastVersion.Data.Priority.Id",
                            ListSortDirection.Ascending));
                            tasksActiveView.SortDescriptions.Add(new SortDescription("LastVersion.Data.Priority.Id",
                           ListSortDirection.Ascending));
                            tasksCompletedView.SortDescriptions.Add(new SortDescription("LastVersion.Data.Priority.Id",
                          ListSortDirection.Ascending));
                            break;
                        case "По дате изменения":
                            tasksWaitingView.SortDescriptions.Add(new SortDescription("LastVersion.CreatedAt",
                            ListSortDirection.Descending));
                            tasksActiveView.SortDescriptions.Add(new SortDescription("LastVersion.CreatedAt",
                           ListSortDirection.Descending));
                            tasksCompletedView.SortDescriptions.Add(new SortDescription("LastVersion.CreatedAt",
                          ListSortDirection.Descending));
                            break;
                        case "По дате выполнения":
                            tasksWaitingView.SortDescriptions.Add(new SortDescription("LastVersion.Data.DueDate",
                           ListSortDirection.Ascending));
                            tasksActiveView.SortDescriptions.Add(new SortDescription("LastVersion.Data.DueDate",
                           ListSortDirection.Ascending));
                            tasksCompletedView.SortDescriptions.Add(new SortDescription("LastVersion.Data.DueDate",
                          ListSortDirection.Ascending));
                            break;
                        case "По дате создания":
                            tasksWaitingView.SortDescriptions.Add(new SortDescription("CreatedAt",
                           ListSortDirection.Ascending));
                            tasksActiveView.SortDescriptions.Add(new SortDescription("CreatedAt",
                           ListSortDirection.Ascending));
                            tasksCompletedView.SortDescriptions.Add(new SortDescription("CreatedAt",
                          ListSortDirection.Ascending));
                            break;
                    }
                }
            }

            tasksWaitingView.Refresh();
            tasksActiveView.Refresh();
            tasksCompletedView.Refresh();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            tasksBox.Text = string.Empty;
            tasksCombo.SelectedIndex = -1;
            if (TasksSearchQuery != null)
                TasksSearchQuery = string.Empty;
            tasksWaitingView.Refresh();
            tasksActiveView.Refresh();
            tasksCompletedView.Refresh();
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
                            usersView.SortDescriptions.Add(new SortDescription("Tasks.Count",
                            ListSortDirection.Descending));
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

        public bool FilterHistories(object obj)
        {
            if (obj is not TasksHistory)
                return false;

            var temp = (TasksHistory)obj;

            if (HistorySearchQuery != null && !temp.DisplayText.Contains(HistorySearchQuery, StringComparison.CurrentCultureIgnoreCase))
                return false;
            return true;
        }

        private void historytBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            historiesView.Refresh();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            historytBox.Text = string.Empty;
            if (HistorySearchQuery != null)
                HistorySearchQuery = string.Empty;
            historiesView.Refresh();
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
            foreach (var user in Project.ProjectUsers)
                ProjectUsers.Add(user);
            var histories = projectTasks
                .SelectMany(t => t.TasksHistories ?? new List<TasksHistory>())
                .OrderByDescending(h => h.CreatedAt)
                .ToList();
            TasksHistories.Clear();
            foreach (var h in histories)
                TasksHistories.Add(h);

            usersView = CollectionViewSource.GetDefaultView(ProjectUsers);
            usersView.Filter = FilterUsers;

            historiesView = CollectionViewSource.GetDefaultView(TasksHistories);
            historiesView.Filter = FilterHistories;

            tasksWaitingView = CollectionViewSource.GetDefaultView(WaitingTasks);
            tasksWaitingView.Filter = FilterTasks;

            tasksActiveView = CollectionViewSource.GetDefaultView(ActiveTasks);
            tasksActiveView.Filter = FilterTasks;

            tasksCompletedView = CollectionViewSource.GetDefaultView(CompletedTasks);
            tasksCompletedView.Filter = FilterTasks;

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