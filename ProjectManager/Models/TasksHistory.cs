using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManager.Models;

public partial class TasksHistory
{
    public int Id { get; set; }

    public int ActionId { get; set; }

    public int ProjectuserId { get; set; }

    public int TaskId { get; set; }

    public int DataId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Action Action { get; set; } = null!;

    public virtual TaskInformation Data { get; set; } = null!;

    public virtual ProjectUser Projectuser { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;

    [NotMapped]
    public ObservableCollection<string> DisplayChanges
    {
        get
        {
            ObservableCollection<string> changes = new ObservableCollection<string>();

       
            var currentData = this.Data;

           
            var taskHistories = this.Task?.TasksHistories?.OrderBy(h => h.Id).ToList();
            if (taskHistories == null || taskHistories.Count == 0)
                return changes;

            var currentIndex = taskHistories.FindIndex(h => h.Id == this.Id);

           
            if (currentIndex == 0)
            {
                changes.Add($"Создано название: {currentData.Title}");
                changes.Add($"Создано описание: {currentData.Description}");
                changes.Add($"Создан цвет: {currentData.Color}");
                changes.Add($"Дата выполнения: {currentData.DueDate:dd.MM.yyyy}");
                if (currentData.Status != null)
                    changes.Add($"Статус: {currentData.Status.Title}");
                if (currentData.Priority != null)
                    changes.Add($"Приоритет: {currentData.Priority.Title}");
                if (currentData.Assignedto != null)
                    changes.Add($"Исполнитель: {currentData.Assignedto.User.Username}");
            }
            else
            {
              
                var previousData = taskHistories[currentIndex - 1].Data;

                if (previousData.Title != currentData.Title)
                    changes.Add($"Название: {previousData.Title} → {currentData.Title}");

                if (previousData.Description != currentData.Description)
                    changes.Add($"Описание: {previousData.Description} → {currentData.Description}");

                if (previousData.DueDate != currentData.DueDate)
                    changes.Add($"Дата выполнения: {previousData.DueDate:dd.MM.yyyy} → {currentData.DueDate:dd.MM.yyyy}");

                if (previousData.StatusId != currentData.StatusId)
                    changes.Add($"Статус: {previousData.Status?.Title} → {currentData.Status?.Title}");

                if (previousData.PriorityId != currentData.PriorityId)
                    changes.Add($"Приоритет: {previousData.Priority?.Title} → {currentData.Priority?.Title}");

                if (previousData.AssignedtoId != currentData.AssignedtoId)
                    changes.Add($"Исполнитель: {previousData.Assignedto?.User?.Username} → {currentData.Assignedto?.User?.Username}");

                if (previousData.Color != currentData.Color)
                    changes.Add($"Цвет: {previousData.Color} → {currentData.Color}");
            }

            return changes;
        }
    }

    [NotMapped]
    public string DisplayText
    {
        get
        {
            var username = Projectuser?.User?.Username ?? "Неизвестно";
            var actionName = Action?.Title ?? "Действие";
            var target = Data?.Title ?? "Задача";

            return $"🕐{TimeAgo} 👤{username} 🔧{actionName} 📋{target}";
        }
    }

    [NotMapped]
    public string DisplayTitle
    {
        get
        {
            return Task.LastVersion.Data.Title;
        }
    }

    [NotMapped]
    public string TimeAgo
    {
        get
        {
            var now = DateTime.Now;
            var timeSpan = now - CreatedAt;
            if (timeSpan.TotalSeconds < 0)
                return "только что";
            if (timeSpan.TotalSeconds < 60)
                return "только что";
            if (timeSpan.TotalMinutes < 60)
            {
                var minutes = (int)timeSpan.TotalMinutes;
                return $"{minutes}м. назад";
            }
            if (timeSpan.TotalHours < 24)
            {
                var hours = (int)timeSpan.TotalHours;
                return $"{hours}ч. назад";
            }
            if (timeSpan.TotalDays < 7)
            {
                var days = (int)timeSpan.TotalDays;
                return $"{days}д. назад";
            }
            if (timeSpan.TotalDays < 30)
            {
                var weeks = (int)(timeSpan.TotalDays / 7);
                return $"{weeks}нед. назад";
            }
            if (timeSpan.TotalDays < 365)
            {
                var months = (int)(timeSpan.TotalDays / 30);
                return $"{months}мес. назад";
            }
            var years = (int)(timeSpan.TotalDays / 365);
            return $"{years}г. назад";
        }
    }

   
}
