using System;
using System.Collections.Generic;

namespace ProjectManager.Models;

public partial class ProjectInformation
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Color { get; set; }

    public virtual ICollection<ProjectsHistory> ProjectsHistories { get; set; } = new List<ProjectsHistory>();
}
