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
    public class TasksService
    {
        private readonly ProjectManagerDbContext _db = BaseDbService.Instance.Context;
        public ObservableCollection<Models.Task> Tasks { get; set; } = new();
        public ObservableCollection<TaskInformation> TasksInformations  { get; set; } = new();
        public ObservableCollection<TasksHistory> TasksHistories { get; set; } = new();
        public ObservableCollection<Priority> Priorities { get; set; } = new();
        public ObservableCollection<Status> Statuses { get; set; } = new();
        public ObservableCollection<Models.Action> Actions { get; set; } = new();
        public ObservableCollection<Comment> Comments { get; set; } = new();

        public TasksService()
        {
            GetAll();
        }
        public void Add(TasksHistory history)
        {
            var _history = new TasksHistory
            {
                ActionId = history.ActionId,
                Action = history.Action,
                Projectuser = history.Projectuser,
                ProjectuserId = history.ProjectuserId,
                Task = history.Task,
                TaskId = history.TaskId,
                Data = history.Data,
                DataId = history.DataId,
                CreatedAt = history.CreatedAt,

            };
            _db.Add<TasksHistory>(_history);
            Commit();
        }
        public void Add(Comment comment)
        {
            var _comment = new Comment
            {
               TaskId = comment.TaskId,
               Task = comment.Task,
               ProjectuserId= comment.ProjectuserId,
               Projectuser = comment.Projectuser,
               Text = comment.Text,
               CreatedAt= comment.CreatedAt,
            };
            _db.Add<Comment>(_comment);
            Commit();
        }
        public int Commit() => _db.SaveChanges();

        public void GetAll()
        {
            var tasks = _db.Tasks.ToList();
            Tasks.Clear();
            foreach (var obj in tasks)
                Tasks.Add(obj);

            var tasksInformations = _db.TaskInformations.ToList();
            TasksInformations.Clear();
            foreach (var obj in tasksInformations)
                TasksInformations.Add(obj);

            var tasksHistories = _db.TasksHistories.ToList();
            TasksHistories.Clear();
            foreach (var obj in tasksHistories)
                TasksHistories.Add(obj);

            var priorities = _db.Priorities.ToList();
            Priorities.Clear();
            foreach (var obj in priorities)
                Priorities.Add(obj);

            var statuses = _db.Statuses.ToList();
            Statuses.Clear();
            foreach (var obj in statuses)
                Statuses.Add(obj);

            var actions = _db.Actions.ToList();
            Actions.Clear();
            foreach (var obj in actions)
                Actions.Add(obj);

            var comments = _db.Comments.ToList();
            Comments.Clear();
            foreach (var obj in comments)
                Comments.Add(obj);
        }

        public void GetAll(Project project)
        {
            // Загружаем задачи проекта
            var tasks = _db.Tasks
                .Where(t => t.ProjectId == project.Id)
                .ToList();

            Tasks.Clear();
            foreach (var task in tasks)
                Tasks.Add(task);

            // Получаем ID всех задач проекта
            var taskIds = tasks.Select(t => t.Id).ToList();

            // Загружаем TaskInformations для задач проекта с включением связанных данных
            var tasksInformations = _db.TaskInformations
                .Where(ti => taskIds.Contains(ti.Id))
                .Include(ti => ti.Priority)
                .Include(ti => ti.Status)
                .Include(ti => ti.Assignedto)
                .ToList();

            TasksInformations.Clear();
            foreach (var info in tasksInformations)
                TasksInformations.Add(info);

            // Загружаем комментарии для задач проекта
            var comments = _db.Comments
                .Where(c => taskIds.Contains(c.TaskId))
                .Include(c => c.Projectuser)  // Подгружаем данные пользователя
                .Include(c => c.Task)         // Подгружаем данные задачи
                .OrderBy(c => c.CreatedAt)    // Сортируем по дате создания
                .ToList();

            Comments.Clear();
            foreach (var comment in comments)
                Comments.Add(comment);

            // Загружаем TasksHistories для задач проекта
            var tasksHistories = _db.TasksHistories
                .Where(th => taskIds.Contains(th.TaskId))
                .Include(th => th.Action)
                .Include(th => th.Data)
                .Include(th => th.Projectuser)
                .Include(th => th.Task)
                .ToList();

            TasksHistories.Clear();
            foreach (var history in tasksHistories)
                TasksHistories.Add(history);

            // Загружаем приоритеты
            var priorityIds = tasksInformations
                .Where(ti => ti.PriorityId.HasValue)
                .Select(ti => ti.PriorityId.Value)
                .Distinct()
                .ToList();

            var priorities = _db.Priorities
                .Where(p => priorityIds.Contains(p.Id))
                .ToList();

            Priorities.Clear();
            foreach (var priority in priorities)
                Priorities.Add(priority);

            // Загружаем статусы
            var statusIds = tasksInformations
                .Where(ti => ti.StatusId.HasValue)
                .Select(ti => ti.StatusId.Value)
                .Distinct()
                .ToList();

            var statuses = _db.Statuses
                .Where(s => statusIds.Contains(s.Id))
                .ToList();

            Statuses.Clear();
            foreach (var status in statuses)
                Statuses.Add(status);

            // Загружаем действия
            var actionIds = tasksHistories
                .Select(th => th.ActionId)
                .Distinct()
                .ToList();

            var actions = _db.Actions
                .Where(a => actionIds.Contains(a.Id))
                .ToList();

            Actions.Clear();
            foreach (var action in actions)
                Actions.Add(action);

            // Загружаем ProjectUsers для назначенных пользователей
            var assignedUserIds = tasksInformations
                .Where(ti => ti.AssignedtoId.HasValue)
                .Select(ti => ti.AssignedtoId.Value)
                .Distinct()
                .ToList();

            // Загружаем ProjectUsers для истории изменений
            var projectUserIdsFromHistory = tasksHistories
                .Select(th => th.ProjectuserId)
                .Distinct()
                .ToList();

            // Загружаем ProjectUsers для комментариев
            var projectUserIdsFromComments = comments
                .Select(c => c.ProjectuserId)
                .Distinct()
                .ToList();

            // Объединяем все ID пользователей
            var allUserIds = assignedUserIds
                .Union(projectUserIdsFromHistory)
                .Union(projectUserIdsFromComments)
                .Distinct()
                .ToList();

            var projectUsers = _db.ProjectUsers
                .Where(pu => allUserIds.Contains(pu.Id))
                .Include(pu => pu.User)  // Подгружаем данные пользователя
                .ToList();

          

        }

        public void Remove(Models.Task task)
        {
            _db.Remove<Models.Task>(task);
            if (Commit() > 0)
                if (Tasks.Contains(task))
                    Tasks.Remove(task);
        }


        public void LoadRelation(Models.Task task, string relation)
        {
            var entry = _db.Entry(task);
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
