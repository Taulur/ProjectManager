using System;
using System.Collections.Generic;

namespace ProjectManager.Models;

public partial class Priority
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<TaskInformation> TaskInformations { get; set; } = new List<TaskInformation>();
}
