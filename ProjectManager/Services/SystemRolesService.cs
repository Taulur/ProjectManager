using Microsoft.EntityFrameworkCore;
using ProjectManager.Models;
using ProjectManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Services
{
    public class SystemRolesService
    {
        private readonly ProjectManagerDbContext _db = BaseDbService.Instance.Context;
        public ObservableCollection<SystemRole> SystemRoles { get; set; } = new();
        public SystemRolesService()
        {
            GetAll();
        }
        public void Add(SystemRole role)
        {
            var _role = new SystemRole
            {
                Title = role.Title,
            };
            _db.Add<SystemRole>(_role);
            Commit();
        }
        public int Commit() => _db.SaveChanges();
        public void GetAll()
        {
            var roles = _db.SystemRoles.ToList();
            SystemRoles.Clear();
            foreach (var role in roles)
                SystemRoles.Add(role);
        }

        public void Remove(SystemRole role)
        {
            _db.Remove<SystemRole>(role);
            if (Commit() > 0)
                if (SystemRoles.Contains(role))
                    SystemRoles.Remove(role);
        }


        public void LoadRelation(SystemRole role, string relation)
        {
            var entry = _db.Entry(role);
            var navigation = entry.Metadata.FindNavigation(relation)
            ?? throw new InvalidOperationException($"Navigation '{relation}' not found");

            if (navigation.IsCollection)
            {
                entry.Collection(relation).Load();
            }
            else
            {
                entry.Reference(relation).Load();
            }
        }




    }
}
