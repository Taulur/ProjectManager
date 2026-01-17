using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManager.Models;

public partial class Task
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int CreatedbyId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ProjectUser Createdby { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<TasksHistory> TasksHistories { get; set; } = new List<TasksHistory>();

    [NotMapped]
    public TasksHistory LastVersion =>
     TasksHistories?.OrderByDescending(th => th.CreatedAt).FirstOrDefault();

    [NotMapped]
    public string ColorByStatus
    {
        get
        {
            if (LastVersion.Data.Status.Title == "Waiting")
            {
                return "RoyalBlue";
            }
            else if (LastVersion.Data.Status.Title == "Active")
            {
                return "Orange";
            }
            else
            {
                return "Green";
            }
        }
    }

    [NotMapped]
    public string ColorByPriority
    {
        get
        {
            if (LastVersion.Data.Priority.Title == "Low")
            {
                return "Green";
            }
            else if (LastVersion.Data.Priority.Title == "Medium")
            {
                return "Orange";
            }
            else
            {
                return "Red";
            }
        }
    }

    [NotMapped]
    public int ProgressPercentage
    {
        get
        {
            if (LastVersion?.Data?.DueDate == null)
                return 0;

            DateTime startDate = CreatedAt; 
            DateTime dueDate = LastVersion.Data.DueDate;
            DateTime now = DateTime.Now;

            // Если задача уже просрочена
            if (now >= dueDate)
                return 100;

            // Если задача еще не началась
            if (now <= startDate)
                return 0;

            // Расчет прогресса
            double totalDays = (dueDate - startDate).TotalDays;
            double daysPassed = (now - startDate).TotalDays;

            int progress = (int)((daysPassed / totalDays) * 100);
            return Math.Clamp(progress, 0, 100);
        }
    }

    [NotMapped]
    public string TimeLeft
    {
        get
        {
            var now = DateTime.Now;
            var timeSpan = LastVersion.Data.DueDate - now;

            // Если дата в будущем (на всякий случай)
            if (timeSpan.TotalSeconds < 0)
                return "срок окончен";

            // Секунды
            if (timeSpan.TotalSeconds < 60)
                return "осталось меньше минуты";

            // Минуты
            if (timeSpan.TotalMinutes < 60)
            {
                var minutes = (int)timeSpan.TotalMinutes;
                return $"осталось {minutes} м.";
            }

            // Часы
            if (timeSpan.TotalHours < 24)
            {
                var hours = (int)timeSpan.TotalHours;
                return $"осталось {hours} ч.";
            }

            // Дни
            if (timeSpan.TotalDays < 7)
            {
                var days = (int)timeSpan.TotalDays;
                return $"осталось {days} д.";
            }

            // Недели (до ~30 дней)
            if (timeSpan.TotalDays < 30)
            {
                var weeks = (int)(timeSpan.TotalDays / 7);
                return $"осталось {weeks} нед.";
            }

            // Месяцы (до 365 дней)
            if (timeSpan.TotalDays < 365)
            {
                var months = (int)(timeSpan.TotalDays / 30);
                return $"осталось {months} мес.";
            }

            // Годы
            var years = (int)(timeSpan.TotalDays / 365);
            return $"осталось {years} г.";
        }
    }

    [NotMapped]
    public int CommentsCount
    {
        get
        {
            return Comments.Count;
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
}
