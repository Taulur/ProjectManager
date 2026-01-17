using GalaSoft.MvvmLight.Views;
using ProjectManager.Models;
using ProjectManager.Services;
using System;
using System.Collections;
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
    /// Логика взаимодействия для ProjectPage.xaml
    /// </summary>
    public partial class ProjectPage : Page
    {
   

        public TasksService TasksService { get; set; } = new();
        public Project? Project { get; set; }
        public ObservableCollection<Models.Task> WaitingTasks { get; set; } = new();
        public ObservableCollection<Models.Task> ActiveTasks { get; set; } = new();
        public ObservableCollection<Models.Task> CompletedTasks { get; set; } = new();
        private Models.Task task { get; set; }
        public int tasksCount { get { return TasksService.Tasks.Count; } }
        public ObservableCollection<TasksHistory> TasksHistories { get; set; } = new();
        public ObservableCollection<ProjectUser> ProjectUsers { get; set; } = new();
        public int ProjectUsersCount {  get { return ProjectUsers.Count; } }

        public ProjectPage(Project _project)
        {
            Project = _project;

            TasksService.GetAll(Project);
            TasksRefresh();
            foreach (var user in Project.ProjectUsers)
            {
                ProjectUsers.Add(user);
            }
           
            var histories = TasksService.TasksHistories;
            var reversedList = histories.Reverse().ToList();
            TasksHistories = new ObservableCollection<TasksHistory>(reversedList);
            

            InitializeComponent();
        }

        void TasksRefresh()
        {
            foreach (var task in TasksService.Tasks)
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

        public void add(object sender, EventArgs e)
        {
            var popupWindow = new EditTaskWindow(Project);
            popupWindow.Owner = Window.GetWindow(this);
            popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            popupWindow.ShowDialog();
        }
        public void edit(object sender, EventArgs e)
        {
           
        }
        public void remove(object sender, EventArgs e)
        {
            var button = sender as Button;
            task = button?.Tag as Models.Task;
            if (MessageBox.Show("Вы действительно хотите удалить запись?", "Удалить?",
            MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                TasksService.Remove(task);
            }
        }
        public void projectUsers(object sender, EventArgs e)
        {
            NavigationService.Navigate(new ProjectUsersPage(Project));
        }
        public void back(object sender, EventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void projects(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void OnTaskClick(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            Models.Task task = border?.Tag as Models.Task;
            NavigationService.Navigate(new TaskPage(task));
        }

        private void OnUserClick(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            ProjectUser projectUser = border?.Tag as ProjectUser;
            NavigationService.Navigate(new ProjectUserPage(projectUser));
        }

        private void leftHistory(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            TasksHistory tasksHistory = border?.Tag as TasksHistory;
            NavigationService.Navigate(new TaskPage(tasksHistory.Task));
        }

        private void rightHistory(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            TasksHistory tasksHistory = border?.Tag as TasksHistory;
            NavigationService.Navigate(new ProjectUserPage(tasksHistory.Projectuser));
        }
    }
}
