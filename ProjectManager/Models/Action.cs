using System;
using System.Collections.Generic;

namespace ProjectManager.Models;

public partial class Action
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<ProjectsHistory> ProjectsHistories { get; set; } = new List<ProjectsHistory>();

    public virtual ICollection<TasksHistory> TasksHistories { get; set; } = new List<TasksHistory>();
}
