using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProjectManager.Services
{
    public class DbService
    {
        private readonly ProjectManagerDbContext _db = BaseDbService.Instance.Context;

        public ObservableCollection<Models.Action> Actions { get; set; } = new();
        public ObservableCollection<Comment> Comments { get; set; } = new();
        public ObservableCollection<Priority> Priorities { get; set; } = new();
        public ObservableCollection<Project> Projects { get; set; } = new();
        public ObservableCollection<ProjectInformation> ProjectInformations { get; set; } = new();
        public ObservableCollection<ProjectUser> ProjectUsers { get; set; } = new();
        public ObservableCollection<ProjectsHistory> ProjectsHistories { get; set; } = new();
        public ObservableCollection<Role> Roles { get; set; } = new();
        public ObservableCollection<Status> Statuses { get; set; } = new();
        public ObservableCollection<SystemRole> SystemRoles { get; set; } = new();
        public ObservableCollection<Models.Task> Tasks { get; set; } = new();
        public ObservableCollection<TaskInformation> TaskInformations { get; set; } = new();
        public ObservableCollection<TasksHistory> TasksHistories { get; set; } = new();
        public ObservableCollection<User> Users { get; set; } = new();

        public DbService()
        {
          
        }
        public int Commit() => _db.SaveChanges();
        public void Add(Comment comment)
        {
            var _comment = new Comment
            {
                TaskId = comment.TaskId,
                Task = comment.Task,
                ProjectuserId = comment.ProjectuserId,
                Projectuser = comment.Projectuser,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
            };
            _db.Add<Comment>(_comment);
            Commit();
        }
        public void Add(User user)
        {
            var _user = new User
            {
                Username = user.Username,
                Fullname = user.Fullname,
                Password = user.Password,
                CreatedAt = user.CreatedAt,
                SystemroleId = user.SystemroleId,
                Systemrole = user.Systemrole,
            };
            _db.Add<User>(_user);
            Commit();
        }

        public void RemoveProject(Project project)
        {
            if (project == null) return;

            var tasks = _db.Tasks.Where(t => t.ProjectId == project.Id).ToList();
            foreach (var task in tasks)
            {
                RemoveTask(task);
            }
            var projectHistories = _db.ProjectsHistories.Where(ph => ph.ProjectId == project.Id).ToList();
            foreach (var ph in projectHistories)
            {
                if (ph.Data != null)
                {
                    _db.ProjectInformations.Remove(ph.Data);
                    ProjectInformations.Remove(ph.Data);
                }
                _db.ProjectsHistories.Remove(ph);
                ProjectsHistories.Remove(ph);
            }
            var projectUsers = _db.ProjectUsers.Where(pu => pu.ProjectId == project.Id).ToList();
            foreach (var pu in projectUsers)
            {
                RemoveProjectUser(pu, false);
            }

            _db.Projects.Remove(project);
            Projects.Remove(project);
        }

        public void RemoveTask(Models.Task task)
        {
            if (task == null) return;

            var comments = _db.Comments.Where(c => c.TaskId == task.Id).ToList();
            _db.Comments.RemoveRange(comments);

            var histories = _db.TasksHistories
                .Include(th => th.Data)
                .Where(th => th.TaskId == task.Id)
                .ToList();

            _db.TasksHistories.RemoveRange(histories);
            var taskInformationsToDelete = histories
                .Where(h => h.Data != null)
                .Select(h => h.Data!)
                .Distinct()         
                .ToList();

            _db.TaskInformations.RemoveRange(taskInformationsToDelete);
            _db.Tasks.Remove(task);

            _db.SaveChanges();
        }


        public void RemoveProjectUser(ProjectUser projectUser, bool deleteDependencies = true)
        {
            if (projectUser == null) return;

            if (deleteDependencies)
            {
                var comments = _db.Comments.Where(c => c.ProjectuserId == projectUser.Id).ToList();
                foreach (var comment in comments)
                {
                    _db.Comments.Remove(comment);
                    Comments.Remove(comment);
                }
                var taskHistories = _db.TasksHistories.Where(th => th.ProjectuserId == projectUser.Id).ToList();
                foreach (var th in taskHistories)
                {
                    if (th.Data != null)
                    {
                        _db.TaskInformations.Remove(th.Data);
                        TaskInformations.Remove(th.Data);
                    }
                    _db.TasksHistories.Remove(th);
                    TasksHistories.Remove(th);
                }

                var projectHistories = _db.ProjectsHistories.Where(ph => ph.ProjectuserId == projectUser.Id).ToList();
                foreach (var ph in projectHistories)
                {
                    if (ph.Data != null)
                    {
                        _db.ProjectInformations.Remove(ph.Data);
                        ProjectInformations.Remove(ph.Data);
                    }
                    _db.ProjectsHistories.Remove(ph);
                    ProjectsHistories.Remove(ph);
                }
                var tasks = _db.Tasks.Where(t => t.CreatedbyId == projectUser.Id).ToList();
                foreach (var task in tasks)
                {
                    RemoveTask(task);
                }


                var assignedTaskInfos = _db.TaskInformations.Where(ti => ti.AssignedtoId == projectUser.Id).ToList();
                foreach (var ti in assignedTaskInfos)
                {
                    ti.AssignedtoId = null;
                    ti.Assignedto = null;
                }
            }

            _db.ProjectUsers.Remove(projectUser);
            ProjectUsers.Remove(projectUser);
        }

        public void LoadProjectTasks(Project project)
        {
         
            var tasks = _db.Tasks
                .Where(t => t.ProjectId == project.Id)
                .Include(t => t.Createdby)
                    .ThenInclude(cb => cb.User)
                .Include(t => t.Project)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Projectuser)
                        .ThenInclude(pu => pu.User)
                .ToList();
            Tasks.Clear();
            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }
        }

        public void LoadUserProjects(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userProjectIds = _db.ProjectUsers
                .Where(pu => pu.UserId == user.Id)
                .Select(pu => pu.ProjectId)
                .ToHashSet();

            var projects = _db.Projects
                .Where(p => userProjectIds.Contains(p.Id))
                .Include(p => p.ProjectUsers)
                    .ThenInclude(pu => pu.User)
                .Include(p => p.ProjectUsers)
                    .ThenInclude(pu => pu.Role)
                .Include(p => p.ProjectsHistories)
                    .ThenInclude(ph => ph.Data)
                .Include(p => p.ProjectsHistories)
                    .ThenInclude(ph => ph.Action)
                .Include(p => p.ProjectsHistories)
                    .ThenInclude(ph => ph.Projectuser)
                        .ThenInclude(pu => pu.User)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.Createdby)
                        .ThenInclude(cb => cb.User)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.Comments)
                        .ThenInclude(c => c.Projectuser)
                            .ThenInclude(pu => pu.User)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.TasksHistories)
                        .ThenInclude(th => th.Action)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.TasksHistories)
                        .ThenInclude(th => th.Data)
                            .ThenInclude(d => d.Assignedto)
                                .ThenInclude(a => a.User)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.TasksHistories)
                        .ThenInclude(th => th.Data)
                            .ThenInclude(d => d.Priority)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.TasksHistories)
                        .ThenInclude(th => th.Data)
                            .ThenInclude(d => d.Status)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.TasksHistories)
                        .ThenInclude(th => th.Projectuser)
                            .ThenInclude(pu => pu.User)
                .ToList();

            Projects.Clear();
            foreach (var obj in projects)
            {
                obj.CurrentUser = user;
                Projects.Add(obj);
                
            }

            var relevantHistoryIds = projects
        .SelectMany(p => p.ProjectsHistories)
        .Select(ph => ph.Id)
        .ToHashSet();

            ProjectsHistories.Clear();
            foreach (var history in _db.ProjectsHistories
                .Where(ph => relevantHistoryIds.Contains(ph.Id))
                .Include(ph => ph.Action)
                .Include(ph => ph.Data)
                .Include(ph => ph.Project)
                .Include(ph => ph.Projectuser).ThenInclude(pu => pu.User)
                .OrderByDescending(ph => ph.CreatedAt)
                .ToList())
            {
                ProjectsHistories.Add(history);
            }
        }

        public void GetAll()
        {
            var actions = _db.Actions.ToList();
            Actions.Clear();
            foreach (var obj in actions)
                Actions.Add(obj);
            var comments = _db.Comments
                .Include(c => c.Projectuser)
                    .ThenInclude(pu => pu.User)
                .Include(c => c.Task)
                .ToList();
            Comments.Clear();
            foreach (var obj in comments)
                Comments.Add(obj);

            var priorities = _db.Priorities.ToList();
            Priorities.Clear();
            foreach (var obj in priorities)
                Priorities.Add(obj);
            var projects = _db.Projects
                .Include(p => p.ProjectUsers)
                    .ThenInclude(pu => pu.User)
                .Include(p => p.ProjectUsers)
                    .ThenInclude(pu => pu.Role)
                .Include(p => p.ProjectsHistories)
                    .ThenInclude(ph => ph.Data)
                .Include(p => p.ProjectsHistories)
                    .ThenInclude(ph => ph.Action)
                .Include(p => p.ProjectsHistories)
                    .ThenInclude(ph => ph.Projectuser)
                        .ThenInclude(pu => pu.User)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.Createdby)
                        .ThenInclude(cb => cb.User)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.Comments)
                        .ThenInclude(c => c.Projectuser)
                            .ThenInclude(pu => pu.User)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.TasksHistories)
                        .ThenInclude(th => th.Action)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.TasksHistories)
                        .ThenInclude(th => th.Data)
                            .ThenInclude(d => d.Assignedto)
                                .ThenInclude(a => a.User)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.TasksHistories)
                        .ThenInclude(th => th.Data)
                            .ThenInclude(d => d.Priority)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.TasksHistories)
                        .ThenInclude(th => th.Data)
                            .ThenInclude(d => d.Status)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.TasksHistories)
                        .ThenInclude(th => th.Projectuser)
                            .ThenInclude(pu => pu.User)
                .ToList();
            Projects.Clear();
            foreach (var obj in projects)
                Projects.Add(obj);
            var projectInformations = _db.ProjectInformations
                .Include(pi => pi.ProjectsHistories)
                    .ThenInclude(ph => ph.Project)
                .ToList();
            ProjectInformations.Clear();
            foreach (var obj in projectInformations)
                ProjectInformations.Add(obj);
            var projectUsers = _db.ProjectUsers
                .Include(pu => pu.Project)
                .Include(pu => pu.Role)
                .Include(pu => pu.User)
                .Include(pu => pu.Comments)
                .Include(pu => pu.ProjectsHistories)
                    .ThenInclude(ph => ph.Data)
                .Include(pu => pu.Tasks)
                .Include(pu => pu.TaskInformations)
                    .ThenInclude(ti => ti.Priority)
                .Include(pu => pu.TaskInformations)
                    .ThenInclude(ti => ti.Status)
                .Include(pu => pu.TasksHistories)
                    .ThenInclude(th => th.Data)
                .ToList();
            ProjectUsers.Clear();
            foreach (var obj in projectUsers)
                ProjectUsers.Add(obj);
            var projectsHistories = _db.ProjectsHistories
                .Include(ph => ph.Action)
                .Include(ph => ph.Data)
                .Include(ph => ph.Project)
                .Include(ph => ph.Projectuser)
                    .ThenInclude(pu => pu.User)
                .ToList();
            ProjectsHistories.Clear();
            foreach (var obj in projectsHistories)
                ProjectsHistories.Add(obj);
            var roles = _db.Roles.ToList();
            Roles.Clear();
            foreach (var obj in roles)
                Roles.Add(obj);
            var statuses = _db.Statuses.ToList();
            Statuses.Clear();
            foreach (var obj in statuses)
                Statuses.Add(obj);

            var systemRoles = _db.SystemRoles
                .Include(sr => sr.Users)
                .ToList();
            SystemRoles.Clear();
            foreach (var obj in systemRoles)
                SystemRoles.Add(obj);

            var tasks = _db.Tasks
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Projectuser)
                        .ThenInclude(pu => pu.User)
                .Include(t => t.Createdby)
                    .ThenInclude(cb => cb.User)
                .Include(t => t.Project)
                .Include(t => t.TasksHistories)
                    .ThenInclude(th => th.Action)
                .Include(t => t.TasksHistories)
                    .ThenInclude(th => th.Data)
                        .ThenInclude(d => d.Assignedto)
                            .ThenInclude(a => a.User)
                .Include(t => t.TasksHistories)
                    .ThenInclude(th => th.Data)
                        .ThenInclude(d => d.Priority)
                .Include(t => t.TasksHistories)
                    .ThenInclude(th => th.Data)
                        .ThenInclude(d => d.Status)
                .Include(t => t.TasksHistories)
                    .ThenInclude(th => th.Projectuser)
                        .ThenInclude(pu => pu.User)
                .ToList();
            Tasks.Clear();
            foreach (var obj in tasks)
                Tasks.Add(obj);

            var taskInformations = _db.TaskInformations
                .Include(ti => ti.Assignedto)
                    .ThenInclude(a => a.User)
                .Include(ti => ti.Priority)
                .Include(ti => ti.Status)
                .Include(ti => ti.TasksHistories)
                    .ThenInclude(th => th.Task)
                .ToList();
            TaskInformations.Clear();
            foreach (var obj in taskInformations)
                TaskInformations.Add(obj);

            var tasksHistories = _db.TasksHistories
                .Include(th => th.Action)
                .Include(th => th.Data)
                    .ThenInclude(d => d.Assignedto)
                        .ThenInclude(a => a.User)
                .Include(th => th.Data)
                    .ThenInclude(d => d.Priority)
                .Include(th => th.Data)
                    .ThenInclude(d => d.Status)
                .Include(th => th.Projectuser)
                    .ThenInclude(pu => pu.User)
                .Include(th => th.Task)
                .ToList();
            TasksHistories.Clear();
            foreach (var obj in tasksHistories)
                TasksHistories.Add(obj);

            var users = _db.Users
                .Include(u => u.ProjectUsers)
                    .ThenInclude(pu => pu.Project)
                .Include(u => u.ProjectUsers)
                    .ThenInclude(pu => pu.Role)
                .Include(u => u.Systemrole)
                .ToList();
            Users.Clear();
            foreach (var obj in users)
                Users.Add(obj);
        }
    }
}