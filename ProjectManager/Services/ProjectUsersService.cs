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

namespace ProjectManager.Services
{
    public class ProjectUsersService
    {
        private readonly ProjectManagerDbContext _db = BaseDbService.Instance.Context;
        public ObservableCollection<ProjectUser> ProjectUsers { get; set; } = new();
        public ObservableCollection<Models.Task> Tasks { get; set; } = new();
        public ObservableCollection<Comment> Comments { get; set; } = new();
        public ObservableCollection<TasksHistory> TasksHistories { get; set; } = new();
        public ObservableCollection<TaskInformation> TasksInformations { get; set; } = new();
        public ProjectUsersService()
        {
            GetAll();
        }
        public void Add(ProjectUser project)
        {
            var _project = new ProjectUser
            {
                UserId = project.UserId,
                User = project.User,
                ProjectId = project.ProjectId,
                Project = project.Project,
                RoleId = project.RoleId,
                Role = project.Role,
                CreatedAt = project.CreatedAt,
            };
            _db.Add<ProjectUser>(_project);
            Commit();
        }
        public int Commit() => _db.SaveChanges();
        public void GetAll()
        {
            var projects = _db.ProjectUsers.ToList();
            ProjectUsers.Clear();
            foreach (var project in projects)
                ProjectUsers.Add(project);
        }

        public void GetAll(Project project)
        {

            // Загружаем ProjectUsers для проекта
            var projectUsers = _db.ProjectUsers
                .Where(pu => pu.ProjectId == project.Id)
                .ToList();

            // Загружаем Tasks для проекта
            var tasks = _db.Tasks
                .Where(t => t.ProjectId == project.Id)
                .Include(t => t.Createdby)
                .ToList();

            // Собираем ID всех задач проекта
            var taskIds = tasks.Select(t => t.Id).ToList();

            // Загружаем Comments для задач проекта
            var comments = _db.Comments
                .Where(c => taskIds.Contains(c.TaskId))
                .Include(c => c.Projectuser)
                .ToList();

            // Загружаем TasksHistories для задач проекта
            var tasksHistories = _db.TasksHistories
                .Where(th => taskIds.Contains(th.TaskId))
                .Include(th => th.Action)
                .Include(th => th.Projectuser)
                .Include(th => th.Data)
                .ToList();

            // Собираем ID всех TaskInformation из TasksHistories
            var taskInfoIds = tasksHistories
                .Select(th => th.DataId)
                .Distinct()
                .ToList();

            // Загружаем TasksInformations
            var tasksInformations = _db.TaskInformations
                .Where(ti => taskInfoIds.Contains(ti.Id))
                .ToList();

            ProjectUsers.Clear();
            foreach (var projectUser in projectUsers)
                ProjectUsers.Add(projectUser);

            // Обновляем Tasks
            Tasks.Clear();
            foreach (var task in tasks)
                Tasks.Add(task);

            // Обновляем Comments
            Comments.Clear();
            foreach (var comment in comments)
                Comments.Add(comment);

            // Обновляем TasksHistories
            TasksHistories.Clear();
            foreach (var history in tasksHistories)
                TasksHistories.Add(history);

            // Обновляем TasksInformations
            TasksInformations.Clear();
            foreach (var info in tasksInformations)
                TasksInformations.Add(info);
        }

        public void Remove(ProjectUser project)
        {
            _db.Remove<ProjectUser>(project);
            if (Commit() > 0)
                if (ProjectUsers.Contains(project))
                    ProjectUsers.Remove(project);
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
