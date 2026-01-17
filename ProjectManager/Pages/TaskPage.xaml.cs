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
    /// Логика взаимодействия для TaskPage.xaml
    /// </summary>
    public partial class TaskPage : Page
    {
        public TasksService tasksService { get; set; } = new();
        public ProjectsServices projectsServices { get; set; } = new();
        public ObservableCollection<TasksHistory> tasksHistories { get; set; } = new();
        public Models.Task task { get; set; } = new();
        public ObservableCollection<Comment> comments { get; set; } = new();
        public Comment comment { get; set; } = new();

        public TaskPage(Models.Task _task)
        {
            task = _task;
            var sortedHistories = task.TasksHistories
      .OrderByDescending(h => h.CreatedAt)
      .ToList();

            foreach (var taskHistory in sortedHistories)
            {
                tasksHistories.Add(taskHistory);
            }
            foreach (var _comment in task.Comments)
                comments.Add(_comment);
            InitializeComponent();
        }

        private void AddComment(object sender, RoutedEventArgs e)
        {
            Comment temp = new Comment
            {
                Task = task,
                Projectuser = projectsServices.ProjectUsers.ToList().First(),
                Text = comment.Text,
                CreatedAt = DateTime.Now,
            };
            tasksService.Add(temp);

            comments.Add(temp);

        }

        private void edit(object sender, RoutedEventArgs e)
        {
            var popupWindow = new EditTaskWindow(task.Project,task);
            popupWindow.Owner = Window.GetWindow(this);
            popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            popupWindow.ShowDialog();
        }

        private void remove(object sender, RoutedEventArgs e)
        {
           
        }

        private void projects(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void users(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProjectUsersPage(task.Project));
        }

        private void back(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProjectPage(task.Project));
        }

        private void leftHistory(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            TasksHistory tasksHistory = border?.Tag as TasksHistory;
            NavigationService.Navigate(new ProjectUserPage(tasksHistory.Projectuser));
        }

        private void leftComment(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            Comment comment = border?.Tag as Comment;
            NavigationService.Navigate(new ProjectUserPage(comment.Projectuser));
        }

        private void creatorTask(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new ProjectUserPage(task.Createdby));
        }

        private void assignedTask(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new ProjectUserPage(task.LastVersion.Data.Assignedto));
        }
    }
}
