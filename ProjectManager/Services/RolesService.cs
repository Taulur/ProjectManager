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
    public class RolesService
    {
        private readonly ProjectManagerDbContext _db = BaseDbService.Instance.Context;
        public ObservableCollection<Role> Roles { get; set; } = new();
        public ObservableCollection<SystemRole> SystemRoles { get; set; } = new();
        public RolesService()
        {
            GetAll();
        }
        public void Add(Role role)
        {
            var _role = new Role
            {
                Title = role.Title,
            };
            _db.Add<Role>(_role);
            Commit();
        }
        public int Commit() => _db.SaveChanges();
        public void GetAll()
        {
            var roles = _db.Roles.ToList();
            Roles.Clear();
            foreach (var role in roles)
                Roles.Add(role);

            var systemroles = _db.SystemRoles.ToList();
            SystemRoles.Clear();
            foreach (var systemrole in systemroles)
                SystemRoles.Add(systemrole);
        }

        public void Remove(Role role)
        {
            _db.Remove<Role>(role);
            if (Commit() > 0)
                if (Roles.Contains(role))
                    Roles.Remove(role);
        }


        public void LoadRelation(Role role, string relation)
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
