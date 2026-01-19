using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Models
{
    public static class CurrentUser
    {
        static public User User { get; set; } = new();

        static public ProjectUser ProjectUserByProject(Project project)
        {
           foreach (ProjectUser projectUser in User.ProjectUsers)
            {
                if (projectUser.ProjectId == project.Id)
                    return projectUser;
            }
            return null;
        }
    }
}
