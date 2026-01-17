using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManager.Models;

public partial class Comment
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public int ProjectuserId { get; set; }

    public string Text { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ProjectUser Projectuser { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;

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
