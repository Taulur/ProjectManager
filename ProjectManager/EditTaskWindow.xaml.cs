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
using System.Windows.Shapes;

namespace ProjectManager
{
    /// <summary>
    /// Логика взаимодействия для EditTaskWindow.xaml
    /// </summary>
    public partial class EditTaskWindow : Window
    {
        public ObservableCollection<string> ColorsCollection { get; set; } = new ObservableCollection<string> { "Red", "Blue", "Green", "Yellow", "Orange", "DarkBlue", "DarkRed" };
        public ObservableCollection<ProjectUser> projectUsers { get; set; } = new();
        public ProjectUsersService projectUsersService { get; set; } = new();
        private Border prevBorderColor;
        private Border prevBorderUser;
        public Models.Task task { get; set; } = new();
        public Project project { get; set; } = new();

        public TasksService tasksService { get; set; } = new();

        public TasksHistory taskHistory { get; set; } = new();

        public int statusId { get; set; } = -1;
        public int priorityId { get; set; } = -1;
        bool isEdit = false;
        string selectedColor = "Red";
        public ProjectUser selectedUser { get; set; } = new();

        public string TitleWindow { get; set; }
        public string HintWindow { get; set; }

        public EditTaskWindow(Project _project, Models.Task? _task = null)
        {
            project = _project;
            projectUsersService.GetAll(project);
            
            if (_task != null)
            {

                task = _task;

                taskHistory.Data = new TaskInformation
                {
                    Title = task.LastVersion.Data.Title,
                    Description = task.LastVersion.Data.Description,
                    DueDate = task.LastVersion.Data.DueDate,
                    Status = task.LastVersion.Data.Status,
                    Priority = task.LastVersion.Data.Priority,
                    Assignedto = task.LastVersion.Data.Assignedto,
                    Color = task.LastVersion.Data.Color,
                };

                taskHistory.CreatedAt = task.LastVersion.CreatedAt;
                
               

                isEdit = true;

                TitleWindow = "Изменение задачи";

            }
            else
            {
                taskHistory.Data = new();
                taskHistory.Data.DueDate = DateTime.Now;
                TitleWindow = "Создание задачи";
                HintWindow = "";
            }




            InitializeComponent();
        }

        private void Execute(object sender, RoutedEventArgs e)
        {


            foreach (var status in tasksService.Statuses)
            {
                if (status.Id == statusId)
                {
                    taskHistory.Data.Status = status;
                    break;
                }
            }
            foreach (var priority in tasksService.Priorities)
            {
                if (priority.Id == priorityId)
                {
                    taskHistory.Data.Priority = priority;
                    break;
                }
            }

            if (isEdit)
            {



                TaskInformation data = new TaskInformation
                {
                    Title = taskHistory.Data.Title,
                    Description = taskHistory.Data.Description,
                    DueDate = taskHistory.Data.DueDate,
                    Status = taskHistory.Data.Status,
                    Priority = taskHistory.Data.Priority,
                    Assignedto = selectedUser,
                    Color = selectedColor,



                };

                TasksHistory newTaskHistory = new TasksHistory
                {
                    Action = tasksService.Actions[1],
                    Task = task,
                    Projectuser = projectUsersService.ProjectUsers[0],
                    Data = data,
                    CreatedAt = DateTime.Now,
                };

                tasksService.Add(newTaskHistory);
            }
            else
            {

                if (statusId != -1 && priorityId != -1)
                {

                    // В ИСТОРИЮ ИЗМЕНЕНИЯ ТОТ КТО ИЗМЕНИЛ ЗАНОСИТСЯ ПОКА ЧТО ПЕРВЫЙ УЧАСТНИК из всех ПРОЕКТов

                    taskHistory.Action = tasksService.Actions[0];
                    taskHistory.Projectuser = projectUsersService.ProjectUsers[0];
                    taskHistory.Task = task;
                    taskHistory.CreatedAt = DateTime.Now;
                    taskHistory.Data.Assignedto = selectedUser;
                    taskHistory.Data.Color = selectedColor;
                    task.Createdby = projectUsersService.ProjectUsers[0];
                    task.CreatedAt = DateTime.Now;
                    task.Project = project;


                    tasksService.Add(taskHistory);

                }

            }
            this.Close();

        }
          
           
        

        private void Back(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
    "Вы уверены? Все изменения сбросятся",
    "Подтверждение выхода",
    MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void EditChoseColor(object sender, MouseButtonEventArgs e)
        {
            if (prevBorderColor != null)
            {
                prevBorderColor.BorderThickness = new Thickness(0);
                prevBorderColor.BorderBrush = new SolidColorBrush(Colors.Black);
            }
            var border = sender as Border;
            prevBorderColor = border;
            border.BorderThickness = new Thickness(3);
            border.BorderBrush = new SolidColorBrush(Colors.AliceBlue);
            selectedColor = border?.Tag as string;

        }

        private void EditUserChose(object sender, MouseButtonEventArgs e)
        {
            if (prevBorderUser != null)
            {
                prevBorderUser.BorderThickness = new Thickness(1);
                prevBorderUser.BorderBrush = new SolidColorBrush(Colors.Black);
            }
            var border = sender as Border;
            prevBorderUser = border;
            border.BorderThickness = new Thickness(3);
            border.BorderBrush = new SolidColorBrush(Colors.Red);
            selectedUser = border?.Tag as ProjectUser;
        }
    }
}
