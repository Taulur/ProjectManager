using System;
using System.Collections.Generic;

namespace ProjectManager.Models;

public partial class SystemRole
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
