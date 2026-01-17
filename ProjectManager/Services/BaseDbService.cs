
using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Services
{
    internal class BaseDbService
    {


        private BaseDbService()
        {
            context = new ProjectManagerDbContext();
        }

        private static BaseDbService? instance;
        public static BaseDbService Instance
        {
            get
            {
                if (instance == null)
                    instance = new BaseDbService();
                return instance;
            }
        }
        private ProjectManagerDbContext context;
        public ProjectManagerDbContext Context => context;

    }
}
