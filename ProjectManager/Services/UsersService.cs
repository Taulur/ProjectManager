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
    public class UsersServices
    {
        private readonly ProjectManagerDbContext _db = BaseDbService.Instance.Context;
        public ObservableCollection<User> Users { get; set; } = new();
        public UsersServices()
        {
            GetAll();
        }
        public void Add(User user)
        {
            var _user = new User
            {
                Username = user.Username,
                Fullname = user.Fullname,
                Password = user.Password,
                CreatedAt = user.CreatedAt,
                SystemroleId = user.SystemroleId,
                Systemrole = user.Systemrole,
            };
            _db.Add<User>(_user);
            Commit();
        }
        public int Commit() => _db.SaveChanges();
        public void GetAll()
        {
            var users = _db.Users.ToList();
            Users.Clear();
            foreach (var user in users)
                Users.Add(user);
        }

        public void GetAll(Project project)
        {
            // Получаем ID всех пользователей, которые уже являются участниками проекта
            var memberUserIds = _db.ProjectUsers
                .Where(pu => pu.ProjectId == project.Id)
                .Select(pu => pu.UserId)
                .ToList();

            // Получаем пользователей, которые не входят в список участников
            var users = _db.Users
                .Where(u => !memberUserIds.Contains(u.Id))
                .ToList();

            Users.Clear();
            foreach (var user in users)
                Users.Add(user);
        }

        public void Remove(User user)
        {
            _db.Remove<User>(user);
            if (Commit() > 0)
                if (Users.Contains(user))
                    Users.Remove(user);
        }



        public void LoadRelation(User user, string relation)
        {
            var entry = _db.Entry(user);
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
