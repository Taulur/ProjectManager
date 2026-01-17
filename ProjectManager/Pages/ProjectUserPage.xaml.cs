using ProjectManager.Models;
using ProjectManager.Services;
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

namespace ProjectManager.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProjectUserPage.xaml
    /// </summary>
    public partial class ProjectUserPage : Page
    {
 
        public ProjectUser projectUser { get; set; } = new();
        public RolesService rolesService { get; set; } = new();
        public ProjectUsersService projectUsersService { get; set; } = new();
        public ObservableCollection<TasksHistory> tasksHistories { get; set; } = new();
        public ObservableCollection<Comment> comments { get; set; } = new();
        public ObservableCollection<Models.Task> WaitingTasks { get; set; } = new();
        public ObservableCollection<Models.Task> ActiveTasks { get; set; } = new();
        public ObservableCollection<Models.Task> CompletedTasks { get; set; } = new();
        public int commentsCount { get { return comments.Count; } }
        public int RoleId { get; set; } = new();
        public ProjectUserPage(ProjectUser _projectUser)
        {

            projectUser = _projectUser;
            RoleId = projectUser.RoleId;
            var sortedHistories = projectUser.TasksHistories
      .OrderByDescending(h => h.CreatedAt)
      .ToList();

            foreach (var taskHistory in sortedHistories)
            {
                tasksHistories.Add(taskHistory);
            }
            foreach (var comment in projectUser.Comments)
            {
                comments.Add(comment);
            }
            TasksRefresh();

            InitializeComponent();
        }
        void TasksRefresh()
        {
            foreach (var task in projectUser.Tasks)
            {
                if (task.LastVersion.Data.Status.Title == "Waiting")
                {
                    WaitingTasks.Add(task);
                }
                else if (task.LastVersion.Data.Status.Title == "Active")
                {
                    ActiveTasks.Add(task);
                }
                else if (task.LastVersion.Data.Status.Title == "Completed")
                {
                    CompletedTasks.Add(task);
                }
            }
        }
        private void back(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProjectPage(projectUser.Project));
        }

        private void projects(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void users(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProjectUsersPage(projectUser.Project));
        }

        private void remove(object sender, RoutedEventArgs e)
        {
            projectUsersService.Remove(projectUser);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            foreach (var role in rolesService.Roles)
            {
                if (role.Id == RoleId)
                {
                    projectUser.Role = role;
                    break;
                }
            }
            projectUsersService.Commit();
        }

        private void leftHistory(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            TasksHistory tasksHistory = border?.Tag as TasksHistory;
            NavigationService.Navigate(new TaskPage(tasksHistory.Task));
        }

        private void leftComment(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            Comment comment = border?.Tag as Comment;
            NavigationService.Navigate(new TaskPage(comment.Task));
        }

        private void CardClick(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            Models.Task task = border?.Tag as Models.Task;
            NavigationService.Navigate(new TaskPage(task));
        }
    }


}
