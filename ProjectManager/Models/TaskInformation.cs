using System;
using System.Collections.Generic;

namespace ProjectManager.Models;

public partial class TaskInformation
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    public int? StatusId { get; set; }

    public int? PriorityId { get; set; }

    public int? AssignedtoId { get; set; }

    public string? Color { get; set; }

    public virtual ProjectUser? Assignedto { get; set; }

    public virtual Priority? Priority { get; set; }

    public virtual Status? Status { get; set; }

    public virtual ICollection<TasksHistory> TasksHistories { get; set; } = new List<TasksHistory>();
}
