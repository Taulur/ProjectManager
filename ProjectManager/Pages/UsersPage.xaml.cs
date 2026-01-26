using ProjectManager.Models;
using ProjectManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class TeamPage : Page
    {
        public DbService DbService { get; set; } = new();
        public User? user { get; set; } = null;

        public string UsersSearchQuery { get; set; } = null!;
        public ICollectionView usersView { get; set; }
        public ObservableCollection<string> UsersFilter { get; set; } = new();

        public TeamPage()
        {
            DbService.GetAll();
            InitializeComponent();

            usersView = CollectionViewSource.GetDefaultView(DbService.Users);
            usersView.Filter = FilterUsers;

            UsersFilter.Add("По дате создания");
            UsersFilter.Add("По количеству проектов");
            DataContext = this;
        }
        public void add(object sender, EventArgs e)
        {
                var popupWindow = new EditUserWindow();
                popupWindow.Owner = Window.GetWindow(this);
                popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                popupWindow.ShowDialog();

                DbService.Commit();
            usersView.Refresh();
        }
        public void BackToMain(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
        public void edit(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
            {
                var popupWindow = new EditUserWindow(user);
                popupWindow.Owner = Window.GetWindow(this);
                popupWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                popupWindow.ShowDialog();

                DbService.Commit();
                usersView.Refresh();
            }
        }
        public void remove(object sender, EventArgs e)
        {
            var button = sender as Button;
            user = button?.Tag as User;
            if (user != null)
            {
                if (MessageBox.Show("Вы действительно хотите удалить запись?", "Удалить?",
           MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DbService.Users.Remove(user);
                }
            }
               
        }

        public bool FilterUsers(object obj)
        {
            if (obj is not User)
                return false;

            var temp = (User)obj;

            if (UsersSearchQuery != null && !temp.Fullname.Contains(UsersSearchQuery, StringComparison.CurrentCultureIgnoreCase))
                return false;

            return true;
        }

        private void usersBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            usersView.Refresh();
        }

        private void usersCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            usersView.SortDescriptions.Clear();
            var cb = (ComboBox)sender;
            if (cb.SelectedIndex != -1)
            {
                var selected = cb.SelectedItem.ToString();
                if (selected != null && selected != string.Empty)
                {
                    switch (selected)
                    {
                        case "По количеству проектов":
                            usersView.SortDescriptions.Add(new SortDescription("TotalProjects",
                            ListSortDirection.Descending));
                            break;
                        case "По дате создания":
                            usersView.SortDescriptions.Add(new SortDescription("CreatedAt",
                            ListSortDirection.Ascending));
                            break;
                    }
                }
            }
            usersView.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            usersBox.Text = string.Empty;
            usersCombo.SelectedIndex = -1;
            if (UsersSearchQuery != null)
                UsersSearchQuery = string.Empty;
            usersView.Refresh();
        }
    }
}
