using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Services
{
    public class DbService
    {
        private readonly ProjectManagerDbContext _db = BaseDbService.Instance.Context;
        public ObservableCollection<User> Users { get; set; } = new();
        public ObservableCollection<Project> Projects { get; set; } = new();
        public ObservableCollection<ProjectUser> ProjectUsers { get; set; } = new();
        public ObservableCollection<Models.Task> Tasks { get; set; } = new();

        public DbService()
        {
            GetAll();
        }
      
        public int Commit() => _db.SaveChanges();
        public void GetAll()
        {
                var objects = _db.Users.ToList();
            Users.Clear();
            foreach (var obj in objects)
                Users.Add(obj);

            var objects1 = _db.ProjectUsers.ToList();
            ProjectUsers.Clear();
            foreach (var obj in objects1)
                ProjectUsers.Add(obj);

            var objects2 = _db.Projects.ToList();
            Projects.Clear();
            foreach (var obj in objects2)
                Projects.Add(obj);

            var objects3 = _db.Tasks.ToList();
            Tasks.Clear();
            foreach (var obj in objects3)
                Tasks.Add(obj);

        }

    }
}
