using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace ProjectManager.Models;

public partial class ProjectManagerDbContext : DbContext
{
    public ProjectManagerDbContext()
    {
    }

    public ProjectManagerDbContext(DbContextOptions<ProjectManagerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Action> Actions { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Priority> Priorities { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectInformation> ProjectInformations { get; set; }

    public virtual DbSet<ProjectUser> ProjectUsers { get; set; }

    public virtual DbSet<ProjectsHistory> ProjectsHistories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<SystemRole> SystemRoles { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskInformation> TaskInformations { get; set; }

    public virtual DbSet<TasksHistory> TasksHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connString = ConfigurationManager.ConnectionStrings["ProjectManagerDb"].ConnectionString;
            optionsBuilder.UseSqlServer(connString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Action>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Actions__3213E83F0DFED6AF");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comments__3213E83F545D949D");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ProjectuserId).HasColumnName("projectuser_id");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.Text).HasColumnName("text");

            entity.HasOne(d => d.Projectuser).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ProjectuserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_ProjectUsers");

            entity.HasOne(d => d.Task).WithMany(p => p.Comments)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Tasks");
        });

        modelBuilder.Entity<Priority>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Prioriti__3213E83F4F3AD4ED");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Projects__3213E83F702D1329");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
        });

        modelBuilder.Entity<ProjectInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectD__3213E83FFA64B9D5");

            entity.ToTable("ProjectInformation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Color).HasColumnName("color");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<ProjectUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectU__3213E83FB4C8F675");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectUsers)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectUsers_Projects");

            entity.HasOne(d => d.Role).WithMany(p => p.ProjectUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectUsers_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectUsers_Users");
        });

        modelBuilder.Entity<ProjectsHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Projects__3213E83F7AFE9787");

            entity.ToTable("ProjectsHistory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActionId).HasColumnName("action_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DataId).HasColumnName("data_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.ProjectuserId).HasColumnName("projectuser_id");

            entity.HasOne(d => d.Action).WithMany(p => p.ProjectsHistories)
                .HasForeignKey(d => d.ActionId)
                .HasConstraintName("FK_ProjectsHistory_Actions");

            entity.HasOne(d => d.Data).WithMany(p => p.ProjectsHistories)
                .HasForeignKey(d => d.DataId)
                .HasConstraintName("FK_ProjectsHistory_ProjectData");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectsHistories)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_ProjectsHistory_Projects");

            entity.HasOne(d => d.Projectuser).WithMany(p => p.ProjectsHistories)
                .HasForeignKey(d => d.ProjectuserId)
                .HasConstraintName("FK_ProjectsHistory_ProjectUsers");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3213E83F6A71CB86");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Statuses__3213E83FFA196F18");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<SystemRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SystemRo__3213E83F8ACFA4BD");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tasks__3213E83F31374102");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedbyId).HasColumnName("createdby_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");

            entity.HasOne(d => d.Createdby).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.CreatedbyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tasks_ProjectUsers");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tasks_Projects");
        });

        modelBuilder.Entity<TaskInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TaskData__3213E83F7F7B7ED2");

            entity.ToTable("TaskInformation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedtoId).HasColumnName("assignedto_id");
            entity.Property(e => e.Color).HasColumnName("color");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DueDate)
                .HasColumnType("datetime")
                .HasColumnName("due_date");
            entity.Property(e => e.PriorityId).HasColumnName("priority_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Title).HasColumnName("title");

            entity.HasOne(d => d.Assignedto).WithMany(p => p.TaskInformations)
                .HasForeignKey(d => d.AssignedtoId)
                .HasConstraintName("FK_TaskInformation_ProjectUsers");

            entity.HasOne(d => d.Priority).WithMany(p => p.TaskInformations)
                .HasForeignKey(d => d.PriorityId)
                .HasConstraintName("FK_TaskData_Priorities");

            entity.HasOne(d => d.Status).WithMany(p => p.TaskInformations)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_TaskData_Statuses");
        });

        modelBuilder.Entity<TasksHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History__3213E83F78B2CE91");

            entity.ToTable("TasksHistory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActionId).HasColumnName("action_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DataId).HasColumnName("data_id");
            entity.Property(e => e.ProjectuserId).HasColumnName("projectuser_id");
            entity.Property(e => e.TaskId).HasColumnName("task_id");

            entity.HasOne(d => d.Action).WithMany(p => p.TasksHistories)
                .HasForeignKey(d => d.ActionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_History_Actions");

            entity.HasOne(d => d.Data).WithMany(p => p.TasksHistories)
                .HasForeignKey(d => d.DataId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TasksHistory_TaskData");

            entity.HasOne(d => d.Projectuser).WithMany(p => p.TasksHistories)
                .HasForeignKey(d => d.ProjectuserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TasksHistory_ProjectUsers");

            entity.HasOne(d => d.Task).WithMany(p => p.TasksHistories)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TasksHistory_Tasks");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83F0B306EF4");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Fullname).HasColumnName("fullname");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.SystemroleId).HasColumnName("systemrole_id");
            entity.Property(e => e.Username).HasColumnName("username");

            entity.HasOne(d => d.Systemrole).WithMany(p => p.Users)
                .HasForeignKey(d => d.SystemroleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_SystemRoles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
