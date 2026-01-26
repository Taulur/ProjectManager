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
    public partial class TaskPage : Page
    {
        public DbService DbService { get; set; } = new DbService();
        public Models.Task Task { get; set; }
        public ObservableCollection<TasksHistory> TaskHistories { get; set; } = new();
        public ObservableCollection<Comment> Comments { get; set; } = new();

        public Comment NewComment { get; set; } = new() { Text = "" };

        public string HistorySearchQuery { get; set; } = null!;
        public ICollectionView historiesView { get; set; }

        private ProjectUser CurrentUser => Models.CurrentUser.ProjectUserByProject(Task.Project);

        public TaskPage(Models.Task task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            Task = task;

            var sortedHistories = task.TasksHistories
                .OrderByDescending(h => h.CreatedAt)
                .ToList();

            TaskHistories.Clear();
            foreach (var h in sortedHistories)
            {
                TaskHistories.Add(h);
            }
            Comments.Clear();
            foreach (var c in task.Comments)
            {
                Comments.Add(c);
            }

            historiesView = CollectionViewSource.GetDefaultView(TaskHistories);
            historiesView.Filter = FilterHistories;

            InitializeComponent();
            DataContext = this;

            DbService.GetAll();
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

        private void AddComment_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewComment.Text))
            {
                MessageBox.Show("Введите текст комментария", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CurrentUser == null)
            {
                MessageBox.Show("Не удалось определить текущего участника проекта", "Ошибка");
                return;
            }

            var comment = new Comment
            {
                TaskId = Task.Id,
                Task = Task,
                ProjectuserId = CurrentUser.Id,
                Projectuser = CurrentUser,
                Text = NewComment.Text.Trim(),
                CreatedAt = DateTime.Now
            };

            DbService.Add(comment);
            Comments.Add(comment);

            NewComment.Text = "";
        }


        private void EditTask_Click(object sender, RoutedEventArgs e)
        {
            var window = new EditTaskWindow(Task.Project, Task)
            {
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.ShowDialog();
            DbService.Commit();
            Task = DbService.Tasks.FirstOrDefault(t => t.Id == Task.Id);
            NavigationService.Navigate(new ProjectPage(Task.Project));
            NavigationService.Navigate(new TaskPage(Task));
        }

        private void RemoveTask_Click(object sender, RoutedEventArgs e)
        {
          
                var result = MessageBox.Show(
                    $"Удалить задачу «{Task.LastVersion.Data.Title}»?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {

                DbService.RemoveTask(Task);
                DbService.Commit();
                NavigationService?.Navigate(new ProjectPage(Task.Project));
            }
           
        }

        private void GoToProject_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ProjectPage(Task.Project));
        }

        private void GoToProjectUsers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ProjectUsersPage(Task.Project));
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void NavigateToUser_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border border) return;

            if (border.Tag is ProjectUser pu)
            {
                NavigationService?.Navigate(new ProjectUserPage(pu));
            }
            else if (border.Tag is TasksHistory history && history.Projectuser != null)
            {
                NavigationService?.Navigate(new ProjectUserPage(history.Projectuser));
            }
            else if (border.Tag is Comment comment && comment.Projectuser != null)
            {
                NavigationService?.Navigate(new ProjectUserPage(comment.Projectuser));
            }
        }

        private void Creator_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Task?.Createdby != null)
            {
                NavigationService?.Navigate(new ProjectUserPage(Task.Createdby));
            }
        }

        private void Assignee_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var assignee = Task?.LastVersion?.Data?.Assignedto;
            if (assignee != null)
            {
                NavigationService?.Navigate(new ProjectUserPage(assignee));
            }
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