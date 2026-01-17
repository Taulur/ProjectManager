using Microsoft.EntityFrameworkCore;
using ProjectManager.Models;
using ProjectManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectManager.Services
{
    public class ProjectsServices
    {
        private readonly ProjectManagerDbContext _db = BaseDbService.Instance.Context;
        public ObservableCollection<Project> Projects { get; set; } = new();
        public ObservableCollection<ProjectsHistory> ProjectsHistories { get; set; } = new();
        public ObservableCollection<ProjectInformation> ProjectInformations { get; set; } = new();
        public ObservableCollection<Models.Task> Tasks { get; set; } = new();
        public ObservableCollection<ProjectUser> ProjectUsers { get; set; } = new();
        public ObservableCollection<Models.Action> Actions { get; set; } = new();
        public ProjectsServices()
        {
            GetAll();

        }
        public void Add(Project project,ProjectsHistory history)
        {

            var _history = new ProjectsHistory
            {
                ActionId = history.ActionId,
                Action = history.Action,
                Projectuser = history.Projectuser,
                ProjectuserId = history.ProjectuserId,
                Project = history.Project,
                ProjectId = history.ProjectId,
                Data = history.Data,
                DataId = history.DataId,
                CreatedAt = history.CreatedAt,

            };
            _db.Add<ProjectsHistory>(_history);

            var _project = new Project
            {
                CreatedAt = project.CreatedAt,
            };
            _db.Add<Project>(_project);

            Commit();
        }
        public void Add(ProjectsHistory history)
        {
            var _history = new ProjectsHistory
            {
                ActionId = history.ActionId,
                Action = history.Action,
                Projectuser = history.Projectuser,
                ProjectuserId = history.ProjectuserId,
                Project = history.Project,
                ProjectId = history.ProjectId,
                Data = history.Data,
                DataId = history.DataId,
                CreatedAt = history.CreatedAt,

            };
            _db.Add<ProjectsHistory>(_history);
            Commit();
        }


        public int Commit() => _db.SaveChanges();
        public void GetAll()
        {
            var projects = _db.Projects.ToList();
            Projects.Clear();
            foreach (var obj in projects)
                Projects.Add(obj);

            var projectsHistory = _db.ProjectsHistories.ToList();
            ProjectsHistories.Clear();
            foreach (var obj in projectsHistory)
                ProjectsHistories.Add(obj);

            var projectsInformations = _db.ProjectInformations.ToList();
            ProjectInformations.Clear();
            foreach (var obj in projectsInformations)
                ProjectInformations.Add(obj);

            var actions = _db.Actions.ToList();
            Actions.Clear();
            foreach (var obj in actions)
                Actions.Add(obj);

            var projectUsers = _db.ProjectUsers.ToList();
            ProjectUsers.Clear();
            foreach (var obj in projectUsers)
                ProjectUsers.Add(obj);


        }

        public void GetAll(User user)
        {
            Projects.Clear();
            ProjectsHistories.Clear();
            ProjectInformations.Clear();
            Actions.Clear();
            var projects = _db.Projects
        .Include(p => p.ProjectUsers) // Участники
            .ThenInclude(pu => pu.Role) // РОЛЬ участника!
        .Include(p => p.ProjectUsers) // Дважды Include для User
            .ThenInclude(pu => pu.User)
        .Include(p => p.Tasks)
            .ThenInclude(t => t.TasksHistories)
                .ThenInclude(th => th.Data)
        .Include(p => p.ProjectsHistories)
            .ThenInclude(ph => ph.Data)
        .Where(p => p.ProjectUsers.Any(pu => pu.UserId == user.Id))
        .ToList();

    foreach (var project in projects)
    {
      
        project.CurrentUser = user;
        Projects.Add(project);
    }

            // Загружаем истории только для этих проектов
            var projectIds = projects.Select(p => p.Id).ToList();
            var projectsHistory = _db.ProjectsHistories
                .Include(ph => ph.Data)
                .Include(ph => ph.Action)
                .Where(ph => projectIds.Contains(ph.ProjectId ?? 0))
                .ToList();

            ProjectsHistories.Clear();
            foreach (var obj in projectsHistory)
                ProjectsHistories.Add(obj);

           
            var historyDataIds = projectsHistory.Select(ph => ph.DataId ?? 0).ToList();
            var projectsInformations = _db.ProjectInformations
                .Where(pi => historyDataIds.Contains(pi.Id))
                .ToList();

            ProjectInformations.Clear();
            foreach (var obj in projectsInformations)
                ProjectInformations.Add(obj);

           
            var actions = _db.Actions.ToList();
            Actions.Clear();
            foreach (var obj in actions)
                Actions.Add(obj);
        }

        public void GetAllTasks(Project project)
        {
            var tasks = project.Tasks.ToList();
            Tasks.Clear();
            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }

            var projectusers = _db.ProjectUsers.ToList();
            ProjectUsers.Clear();
            foreach (var projectuser in projectusers)
                ProjectUsers.Add(projectuser);

        }

       

        public ProjectsHistory GetLastHistory(Project project)
        { 
            return project.ProjectsHistories.ToList().Last();
        }

        public void Remove(Project project)
        {
            // тут удаление

        }


        public void LoadRelation(ProjectUser project, string relation)
        {
            var entry = _db.Entry(project);
            var navigation = entry.Metadata.FindNavigation(relation)
            ?? throw new InvalidOperationException($"Navigation '{relation}' not found");

            if (navigation.IsCollection)
            {
                entry.Collection(relation).Load();
            }
            else
            {
                entry.Reference(relation).Load();
            }
        }




    }
}
