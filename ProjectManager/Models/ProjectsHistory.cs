using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManager.Models;

public partial class ProjectsHistory
{
    public int Id { get; set; }

    public int? ActionId { get; set; }

    public int? ProjectId { get; set; }

    public int? ProjectuserId { get; set; }

    public int? DataId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Action? Action { get; set; }

    public virtual ProjectInformation? Data { get; set; }

    public virtual Project? Project { get; set; }

    public virtual ProjectUser? Projectuser { get; set; }

    [NotMapped]
    public ObservableCollection<string> DisplayChanges
    {
        get
        {
            ObservableCollection<string> changes = new ObservableCollection<string>();
            var currentData = this.Data;

          
            var projectHistories = this.Project?.ProjectsHistories?
                .OrderBy(h => h.CreatedAt)  
                .ToList();

            if (projectHistories == null || projectHistories.Count == 0)
                return changes;

  
            var currentIndex = projectHistories.FindIndex(h => h.Id == this.Id);

            if (currentIndex == 0)
            {
                changes.Add($"Создано название: {currentData.Title}");
                changes.Add($"Создано описание: {currentData.Description}");
                changes.Add($"Создан цвет: {currentData.Color}");
            }
            else
            {
         
                var previousData = projectHistories[currentIndex - 1].Data;

                if (previousData.Title != currentData.Title)
                    changes.Add($"Название: {previousData.Title} → {currentData.Title}");

                if (previousData.Description != currentData.Description)
                    changes.Add($"Описание: {previousData.Description} → {currentData.Description}");

                if (previousData.Color != currentData.Color)
                    changes.Add($"Цвет: {previousData.Color} → {currentData.Color}");

            }

            return changes;
        }
    }

    [NotMapped]
    public string TimeAgo
    {
        get
        {
            var now = DateTime.Now;
            var timeSpan = now - CreatedAt;

            // Если дата в будущем (на всякий случай)
            if (timeSpan.TotalSeconds < 0)
                return "только что";

            // Секунды
            if (timeSpan.TotalSeconds < 60)
                return "только что";

            // Минуты
            if (timeSpan.TotalMinutes < 60)
            {
                var minutes = (int)timeSpan.TotalMinutes;
                return $"{minutes}м. назад";
            }

            // Часы
            if (timeSpan.TotalHours < 24)
            {
                var hours = (int)timeSpan.TotalHours;
                return $"{hours}ч. назад";
            }

            // Дни
            if (timeSpan.TotalDays < 7)
            {
                var days = (int)timeSpan.TotalDays;
                return $"{days}д. назад";
            }

            // Недели (до ~30 дней)
            if (timeSpan.TotalDays < 30)
            {
                var weeks = (int)(timeSpan.TotalDays / 7);
                return $"{weeks}нед. назад";
            }

            // Месяцы (до 365 дней)
            if (timeSpan.TotalDays < 365)
            {
                var months = (int)(timeSpan.TotalDays / 30);
                return $"{months}мес. назад";
            }

            // Годы
            var years = (int)(timeSpan.TotalDays / 365);
            return $"{years}г. назад";
        }
    }

    [NotMapped]
    public string DisplayText
    {
        get
        {
            var username = Projectuser?.User?.Username ?? "Неизвестно";
            var actionName = Action?.Title ?? "Действие";
            var target = Data?.Title ?? "Проект";

            return $"📅{TimeAgo} 👤{username} 🔧{actionName} 📁{target}";
        }
    }

    [NotMapped]
    public string DisplayTitle
    {
        get
        {
            return Project.LastVersion.Data.Title;
        }
    }
}
