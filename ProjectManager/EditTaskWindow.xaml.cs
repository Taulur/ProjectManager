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
        public ProjectUser projectUser { get; set; } = new();

        public string TitleWindow { get; set; }
        public string HintWindow { get; set; }

        public EditTaskWindow(Project _project, Models.Task? _task = null)
        {
            project = _project;
            projectUsersService.GetAll(project);

            projectUser = CurrentUser.ProjectUserByProject(project);
            taskHistory.Projectuser = projectUser;

         

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
                selectedUser = GetBestAssignedUser(5);
                taskHistory.Data = new();
                taskHistory.Data.DueDate = DateTime.Now;
                TitleWindow = "Создание задачи";
                HintWindow = $"Автоматически был выбран самый подходяший участник\nПользователь: {selectedUser.User.Username}\nФИО : {selectedUser.User.Fullname}";
            }

            InitializeComponent();
            
        }

       
        private ProjectUser GetBestAssignedUser(int targetPriorityId)
        {

            var candidates = project.ProjectUsers
                .Where(pu => pu.Id != projectUser.Id && pu.Role.Title != "manager") 
                .ToList();

            if (candidates.Count == 1)
            {
                return candidates[0];
            }

            if (!candidates.Any())
            {
                return null; // Нет кандидатов
                
            }

            int maxExperience = 0;
            double maxTimeSinceLast = 0;
            int maxLoad = 0;
            var candidateData = new Dictionary<ProjectUser, (int load, int experience, double timeSinceLast)>();

            DateTime now = DateTime.Now;

            foreach (var user in candidates)
            {
                int load = user.RemainingTasksToComplete;

                int experience = project.Tasks.Count(t =>
                {
                    var lastVersion = t.LastVersion?.Data;
                    return lastVersion != null &&
                           lastVersion.AssignedtoId == user.Id &&
                           lastVersion.StatusId == 6 && 
                           lastVersion.PriorityId == targetPriorityId;
                });

                DateTime? lastAssignmentTime = project.Tasks
                    .SelectMany(t => t.TasksHistories)
                    .Where(th => th.Data.AssignedtoId == user.Id)
                    .Max(th => (DateTime?)th.CreatedAt);

                double timeSinceLast = lastAssignmentTime.HasValue
                    ? (now - lastAssignmentTime.Value).TotalDays
                    : double.MaxValue / 2; 

                candidateData[user] = (load, experience, timeSinceLast);

                if (experience > maxExperience) maxExperience = experience;
                if (timeSinceLast > maxTimeSinceLast) maxTimeSinceLast = timeSinceLast;
                if (load > maxLoad) maxLoad = load;
            }

            const double weightLoad = 0.4;
            const double weightExp = 0.3;
            const double weightTime = 0.3;

            ProjectUser bestUser = null;
            double bestScore = double.MinValue;

            foreach (var kvp in candidateData)
            {
                var user = kvp.Key;
                var (load, experience, timeSinceLast) = kvp.Value;

                double loadScore = maxLoad > 0 ? 1.0 - (load / (double)maxLoad) : 1.0;

                double expScore = maxExperience > 0 ? experience / (double)maxExperience : 0.0;

                double timeScore = maxTimeSinceLast > 0 ? timeSinceLast / maxTimeSinceLast : 1.0; 

                double totalScore = (weightLoad * loadScore) + (weightExp * expScore) + (weightTime * timeScore);

                if (totalScore > bestScore)
                {
                    bestScore = totalScore;
                    bestUser = user;
                }
            }

            return bestUser;
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

   
            if (!isEdit && selectedUser == null && priorityId != -1)
            {
                selectedUser = GetBestAssignedUser(priorityId);
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
                    Data = data,
                    Projectuser = projectUser,
                    CreatedAt = DateTime.Now,
                };

                tasksService.Add(newTaskHistory);
            }
            else
            {

                if (statusId != -1 && priorityId != -1)
                {
                    taskHistory.Action = tasksService.Actions[0];
                    taskHistory.Task = task;
                    taskHistory.CreatedAt = DateTime.Now;
                    taskHistory.Data.Assignedto = selectedUser;
                    taskHistory.Data.Color = selectedColor;
                    task.Createdby = projectUser;
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