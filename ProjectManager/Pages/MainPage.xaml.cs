using GalaSoft.MvvmLight.Command;
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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
   
        public DbService DbService { get; set; } = new DbService();

        public ObservableCollection<Project> Projects => DbService.Projects;

        public string ProjectSearchQuery { get; set; } = null!;
        public ICollectionView projectsView { get; set; }
        public ObservableCollection<string> ProjectsFilter { get; set; } = new();

        public string HistorySearchQuery { get; set; } = null!;
        public ICollectionView historiesView { get; set; }

        public ObservableCollection<ProjectsHistory> ProjectHistories => DbService.ProjectsHistories;




        public int ProjectsCount => Projects.Count;

        private User CurrentUser => Models.CurrentUser.User; 

        public MainPage()
        {
            InitializeComponent();

            if (CurrentUser.Systemrole.Title != "admin")
            {
                UsersButton.Visibility = Visibility.Collapsed;
                AddButton.Visibility = Visibility.Collapsed;
            }

            DbService.LoadUserProjects(CurrentUser);

            ProjectsFilter.Add("По дате изменения");
            ProjectsFilter.Add("По количеству задач");


            projectsView = CollectionViewSource.GetDefaultView(Projects);
            projectsView.Filter = FilterProjects;


            historiesView = CollectionViewSource.GetDefaultView(ProjectHistories);
            historiesView.Filter = FilterHistories;

            DataContext = this;

        }
        public bool FilterProjects(object obj)
        {
            if (obj is not Project)
                return false;

            var project = (Project)obj;

            if (ProjectSearchQuery != null && !project.LastVersion.DisplayTitle.Contains(ProjectSearchQuery, StringComparison.CurrentCultureIgnoreCase))
                return false;


            return true;
        }

        private void ProjectSearchQuery_Changed(object sender, TextChangedEventArgs e)
        {
            projectsView.Refresh();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            projectsView.SortDescriptions.Clear();
            var cb = (ComboBox)sender;
            if (cb.SelectedIndex != -1)
            {
                var selected = cb.SelectedItem.ToString();
                if (selected != null && selected != string.Empty)
                {
                    switch (selected)
                    {
                        case "По дате изменения":
                            projectsView.SortDescriptions.Add(new SortDescription("LastVersion.CreatedAt",
                            ListSortDirection.Descending));
                            break;
                        case "По количеству задач":
                            projectsView.SortDescriptions.Add(new SortDescription("Tasks.Count",
                            ListSortDirection.Descending));
                            break;
                    }
                }
            }
           

            projectsView.Refresh();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            projectBox.Text = string.Empty;
            projectCombo.SelectedIndex = -1;
            if (ProjectSearchQuery != null)
                ProjectSearchQuery = string.Empty;
            projectsView.Refresh();
        }

        public bool FilterHistories(object obj)
        {
            if (obj is not ProjectsHistory)
                return false;

            var temp = (ProjectsHistory)obj;

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

        private void ToAuth(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }
    }
}