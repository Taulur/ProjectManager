using System;
using System.Collections.Generic;

namespace ProjectManager.Models;

    public partial class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public string Fullname { get; set; } = null!;

        public string Password { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public int SystemroleId { get; set; }

        public virtual ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();

        public virtual SystemRole Systemrole { get; set; } = null!;

    public string FirstWord
    {
        get
        {
            return Username[0].ToString();
        }
    }
}
