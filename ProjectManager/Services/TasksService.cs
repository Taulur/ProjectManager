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
