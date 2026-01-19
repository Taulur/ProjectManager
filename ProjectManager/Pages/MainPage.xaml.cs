using GalaSoft.MvvmLight.Command;
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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
   
        public DbService DbService { get; set; } = new DbService();

        public ObservableCollection<Project> Projects => DbService.Projects;


        public ObservableCollection<ProjectsHistory> ProjectHistories { get; set; } = new();


        public int ProjectsCount => Projects.Count;

        private User CurrentUser => Models.CurrentUser.User; 

        public MainPage()
        {
            InitializeComponent();
            DbService.LoadUserProjects(CurrentUser);

            var histories = DbService.ProjectsHistories
                .OrderByDescending(h => h.CreatedAt) 
                .ToList();

            ProjectHistories = new ObservableCollection<ProjectsHistory>(histories);

            DataContext = this;

        }

      


        private void OnCardClick(object sender, RoutedEventArgs e) 
        {
            if (sender is Border border && border.Tag is Project project)
            {
                NavigationService?.Navigate(new ProjectPage(project));
            }
        }

        private void AddProject(object sender, RoutedEventArgs e)
        {
            var popupWindow = new EditProjectWindow();
            popupWindow.Owner = Window.GetWindow(this);
            popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            popupWindow.ShowDialog();

            DbService.Commit();
            DbService.LoadUserProjects(CurrentUser);

            ProjectHistories.Clear();
            var histories = DbService.ProjectsHistories
                .OrderByDescending(h => h.CreatedAt)
                .ToList();
            foreach (var h in histories) ProjectHistories.Add(h);
        }

        private void EditProject(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Project project)
            {
                var popupWindow = new EditProjectWindow(project);
                popupWindow.Owner = Window.GetWindow(this);
                popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                popupWindow.ShowDialog();

                DbService.Commit();
                DbService.LoadUserProjects(CurrentUser);

                ProjectHistories.Clear();
                var histories = DbService.ProjectsHistories
                    .OrderByDescending(h => h.CreatedAt)
                    .ToList();
                foreach (var h in histories) ProjectHistories.Add(h);
            }
        }

        private void RemoveProject(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Project project)
            {
                var result = MessageBox.Show(
                    $"Удалить проект «{project.Title}»?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {

                    DbService.RemoveProject(project);
                    DbService.Commit();
                    DbService.LoadUserProjects(CurrentUser);

                    ProjectHistories.Clear();
                    var histories = DbService.ProjectsHistories
                        .OrderByDescending(h => h.CreatedAt)
                        .ToList();
                    foreach (var h in histories) ProjectHistories.Add(h);
                }
            }
        }

        private void ShowUsersPage(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new TeamPage());
        }

        private void LeftHistoryClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is ProjectsHistory history && history.Project != null)
            {
                NavigationService?.Navigate(new ProjectPage(history.Project));
            }
        }

        private void RightHistoryClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is ProjectsHistory history && history.Projectuser != null)
            {
                NavigationService?.Navigate(new ProjectUserPage(history.Projectuser));
            }
        }
    }
}