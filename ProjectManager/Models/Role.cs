using System;
using System.Collections.Generic;

namespace ProjectManager.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
}
