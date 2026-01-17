using ProjectManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManager.Models;

public partial class ProjectUser
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ProjectId { get; set; }

    public int RoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<ProjectsHistory> ProjectsHistories { get; set; } = new List<ProjectsHistory>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<TaskInformation> TaskInformations { get; set; } = new List<TaskInformation>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<TasksHistory> TasksHistories { get; set; } = new List<TasksHistory>();

    public virtual User User { get; set; } = null!;

    [NotMapped]
    public int TotalAssignedTasks => Tasks?.Count ?? 0;

    /// <summary>
    /// Количество оставшихся задач, которые надо выполнить
    /// </summary>
    [NotMapped]
    public int RemainingTasksToComplete
    {
        get
        {
            if (TaskInformations == null) return 0;

            // Предполагаем, что статусы с определенным ID являются завершенными
            // Например, статус ID = 3 (или другой) означает "Выполнено"
            // Вам нужно адаптировать эту логику под вашу систему статусов
            const int completedStatusId = 6; // Измените на ваш ID статуса "Завершено"

            return TaskInformations
                .Where(ti => ti.StatusId != completedStatusId && ti.AssignedtoId == Id)
                .Count();
        }
    }
    [NotMapped]
    public string FirstWord
    {
        get
        {
            return User.Username[0].ToString();
        }
    }

    [NotMapped]
    public ObservableCollection<Role> Roles
    {
        get
        {
            RolesService service = new();
            return service.Roles;
        }
    }



}
