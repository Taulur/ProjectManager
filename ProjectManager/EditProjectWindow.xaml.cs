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

namespace ProjectManager
{
    /// <summary>
    /// Логика взаимодействия для EditProjectWindow.xaml
    /// </summary>
    public partial class EditProjectWindow : Window
    {
        public Project currentProject { get; set; } = new();
        public ObservableCollection<string> ColorsCollection { get; set; } = new ObservableCollection<string> { "Red", "Blue", "Green", "Yellow", "Orange", "DarkBlue", "DarkRed" };
        private Border prevBorder;
        public Project project { get; set; } = new();
        private ProjectsServices projectsServices = new();
        private RolesService rolesService = new();
        private UsersServices usersServices = new();
        public ProjectsHistory projectHistory { get; set; } = new();
        public ProjectUser projectUser { get; set; } = new();
        bool isEdit = false;
        string selectedColor = "Red";

        public string TitleWindow { get; set; }
        public string HintWindow { get; set; }

        public EditProjectWindow(Project? _project = null)
        {
            if (_project != null)
            {

                project = _project;


                projectUser = CurrentUser.ProjectUserByProject(project);

                projectHistory.Data = new ProjectInformation
                {
                    Title = project.LastVersion.Data.Title,
                    Description = project.LastVersion.Data.Description,
                    Color = project.LastVersion.Data.Color,
                };

                projectHistory.CreatedAt = project.LastVersion.CreatedAt;
                projectHistory.Projectuser = project.LastVersion.Projectuser;

                isEdit = true;

                TitleWindow = "Изменение проекта";

            }
            else
            {
                projectHistory.Data = new();

                TitleWindow = "Создание проекта";
                HintWindow = "При создании проекта вы становитесь его менеджером.";
            }




            InitializeComponent();
        }

        private void Execute(object sender, RoutedEventArgs e)
        {
            if (isEdit)
            {

                ProjectInformation data = new ProjectInformation
                {
                    Title = projectHistory.Data.Title,
                    Description = projectHistory.Data.Description,
                    Color = selectedColor,
                };

                ProjectsHistory tempProjectHistory = new ProjectsHistory
                {
                    Action = projectsServices.Actions[1],
                    Project = project,
                    Projectuser = projectUser,
                    Data = data,
                    CreatedAt = DateTime.Now,
                };

                projectsServices.Add(tempProjectHistory);
            }
            else
            {



                projectUser.Project = project;
                projectUser.Role = rolesService.Roles[1];
                projectUser.User = CurrentUser.User;
                projectUser.CreatedAt = DateTime.Now;


                project.CreatedAt = DateTime.Now;
                projectHistory.Data.Color = selectedColor;
                projectHistory.Action = projectsServices.Actions[0];
                projectHistory.Projectuser = projectUser;
                projectHistory.Project = project;
                projectHistory.CreatedAt = DateTime.Now;

                projectsServices.Add(projectHistory);


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
            if (prevBorder != null)
            {
                prevBorder.BorderThickness = new Thickness(0);
                prevBorder.BorderBrush = new SolidColorBrush(Colors.Black);
            }
            var border = sender as Border;
            prevBorder = border;
            border.BorderThickness = new Thickness(5);
            border.BorderBrush = new SolidColorBrush(Colors.AliceBlue);
            selectedColor = border?.Tag as string;

        }
    }
}