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
    public int RemainingTasksToComplete
    {
        get
        {
            if (TaskInformations == null) return 0;
            return TaskInformations
                .GroupBy(ti => ti.TasksHistories?.FirstOrDefault()?.TaskId ?? 0)
                .Select(g => g
                    .OrderByDescending(ti => ti.TasksHistories?.Max(h => h.CreatedAt) ?? DateTime.MinValue)
                    .FirstOrDefault()
                )
                .Count(ti => ti != null && ti.AssignedtoId == Id);
        }
    }

    [NotMapped]
    public int TotalAssignedTasks
    {
        get
        {
            if (TaskInformations == null) return 0;
            var uniqueTasksLatestInfo = TaskInformations
                .GroupBy(ti => ti.TasksHistories.FirstOrDefault()?.Task?.Id ?? 0)
                .Select(group => group
                    .OrderByDescending(ti => ti.TasksHistories.Max(h => (DateTime?)h.CreatedAt) ?? DateTime.MinValue)
                    .First()
                )
                .Where(ti => ti.AssignedtoId == Id)
                .ToList();

            const int COMPLETED = 6;

            return uniqueTasksLatestInfo.Count(ti => ti.StatusId == COMPLETED);
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
