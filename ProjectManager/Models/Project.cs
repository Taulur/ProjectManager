using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManager.Models;

public partial class Project
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();

    public virtual ICollection<ProjectsHistory> ProjectsHistories { get; set; } = new List<ProjectsHistory>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    [NotMapped]
    public int TotalTasksCount => Tasks?.Count ?? 0;

    [NotMapped]
    public int CompletedTasksCount
    {
        get
        {
            if (Tasks == null) return 0;

            return Tasks.Count(t =>
                t.TasksHistories != null &&
                t.TasksHistories.Any() &&
                t.TasksHistories.OrderByDescending(th => th.CreatedAt)
                               .FirstOrDefault()?.Data?.StatusId == 6);
        }
    }

    [NotMapped]
    public ProjectsHistory LastVersion =>
     ProjectsHistories?.OrderByDescending(ph => ph.CreatedAt).FirstOrDefault();

    [NotMapped]
    public string Title => LastVersion?.Data?.Title ?? "Без названия";

    [NotMapped]
    public string Description => LastVersion?.Data?.Description ?? string.Empty;

    [NotMapped]
    public string Color => LastVersion?.Data?.Color ?? "#808080"; // Серый по умолчанию

    [NotMapped]
    public DateTime? LastModifiedAt => LastVersion?.CreatedAt;

    [NotMapped]
    private User _currentUser;

    [NotMapped]
    public User CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            // Если реализуете INotifyPropertyChanged, вызовите OnPropertyChanged
        }
    }

   

    [NotMapped]
    public string UserRoleInProject
    {
        get
        {
            if (CurrentUser == null || ProjectUsers == null)
                return "Не назначена";

            var projectUser = ProjectUsers
                .FirstOrDefault(pu => pu.UserId == CurrentUser.Id);

            return projectUser?.Role?.Title ?? "Не назначена";
        }
    }
}
