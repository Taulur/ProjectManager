using GalaSoft.MvvmLight.Command;
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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static MaterialDesignThemes.Wpf.Theme.ComboBox;

namespace ProjectManager.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public DbService DbService { get; set; } = new();
        public ProjectsServices ProjectsServices { get; set; } = new();
        public UsersServices UsersServices { get; set; } = new();
        public ObservableCollection<Project> Projects { get; set; } = new();
        public Project? project { get; set; } = null;
        public User user { get; set; } = null;
        public ObservableCollection<ProjectsHistory> ProjectHistories { get; set; } = new();

        public int projectsCount
        {
            get
            {
                return Projects.Count;
            }
        }

        public MainPage()
        {

            user = UsersServices.Users.First();
            ProjectsServices.GetAll(user);
            Projects = ProjectsServices.Projects;
            var histories = ProjectsServices.ProjectsHistories;
            var reversedList = histories.Reverse().ToList();

            // Создаем новую ObservableCollection
            ProjectHistories = new ObservableCollection<ProjectsHistory>(reversedList);
            InitializeComponent();
        }

        public void OnCardClick(object sender, EventArgs e)
        {
            
            var border = sender as Border;
            Project project = border?.Tag as Project;
            if (project != null)
            {
                NavigationService.Navigate(new ProjectPage(project));
            }
           
        }

        public void add(object sender, EventArgs e)
        {
            var popupWindow = new EditProjectWindow();
            popupWindow.Owner = Window.GetWindow(this);
            popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            popupWindow.ShowDialog(); // или Show() для неблокирующего
        }
        public void edit(object sender, EventArgs e)
        {
            var button = sender as Button;
            Project project = button?.Tag as Project;
            var popupWindow = new EditProjectWindow(project);
            popupWindow.Owner = Window.GetWindow(this);
            popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            popupWindow.ShowDialog(); // или Show() для неблокирующего

        }
       


        public void remove(object sender, EventArgs e)
        {
            var button = sender as Button;
            Project project = button?.Tag as Project;
          
                if (MessageBox.Show("При удалении проекта вы также удаляете\nучастников, задачи и всю связанную историю, удалить?", "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    ProjectsServices.Remove(project);
                }
           
        }
        public void users(object sender, EventArgs e)
        {
            NavigationService.Navigate(new TeamPage());
        }

        private void leftHistory(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            ProjectsHistory projectHistory = border?.Tag as ProjectsHistory;
            NavigationService.Navigate(new ProjectPage(projectHistory.Project));
        }

        private void rightHistory(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            ProjectsHistory projectHistory = border?.Tag as ProjectsHistory;
            NavigationService.Navigate(new ProjectUserPage(projectHistory.Projectuser));
        }
    }
}
