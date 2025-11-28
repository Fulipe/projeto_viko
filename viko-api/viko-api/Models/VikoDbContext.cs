using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using viko_api.Models.Entities;

namespace viko_api.Models;

public partial class VikoDbContext : DbContext
{
    public VikoDbContext(DbContextOptions<VikoDbContext> options)
        : base(options)
    {
    }
    public DbSet<Entity> Entities { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<EventRegistration> EventRegistrations { get; set; }
    public DbSet<EventStatus> EventStatuses { get; set; }
    public DbSet<User> Users { get; set; }

    // Method that maps tables to create the classes 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Entity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Entities");

            //Collumn definition
            entity.Property(e => e.Image)
                .HasColumnType("VARCHAR(MAX)")
                .IsUnicode(false);
            entity.Property(e => e.Languages)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        // === Roles ===
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.Id).HasName("PK_Roles");
            entity.Property(r => r.Name).HasMaxLength(50).IsUnicode(false).IsRequired();

            // Seed inicial (para login e permissões)
            entity.HasData(
                new Role { Id = 1, Name = "Student" },
                new Role { Id = 2, Name = "Teacher" },
                new Role { Id = 3, Name = "Administrator" }
            );
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Events");
            
            entity.HasIndex(e => e.EventGuid)
                .IsUnique();

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.EntityId).HasColumnName("Entity_Id");
            entity.Property(e => e.EventStatusId).HasColumnName("Event_Status_Id");
            
            entity.Property(e => e.FinishDate)
                .HasColumnType("datetime")
                .HasColumnName("Finish_Date");
            entity.Property(e => e.RegistrationDeadline)
                .HasColumnType("datetime")
                .HasColumnName("Registration_Deadline");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.TeacherId).HasColumnName("Teacher_Id");

            entity.HasOne(d => d.Entity)
                .WithMany(p => p.Events)
                .HasForeignKey(d => d.EntityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Event_Entity");

            entity.HasOne(d => d.EventStatus)
                .WithMany(p => p.Events)
                .HasForeignKey(d => d.EventStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Status");

            entity.HasOne(d => d.Teacher)
                .WithMany(p => p.Events)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Teacher");
        });

        modelBuilder.Entity<EventRegistration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_EventRegistrations");

            //entity.Property(e => e.EventId).HasColumnName("Event_Id");
            entity.Property(e => e.RegistrationDate).HasColumnType("datetime");

            entity.HasOne(d => d.Event)
                .WithMany(p => p.EventRegistrations)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Registration_Event");

            entity.HasOne(d => d.Student)
                .WithMany(p => p.EventRegistrations)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Registration_Student");
        });

        modelBuilder.Entity<EventStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_EventStatus");

            entity.ToTable("EventStatus");

            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            // Seed inicial (para login e permissões)
            entity.HasData(
                new EventStatus { Id = 1, Status = "Open" },
                new EventStatus { Id = 2, Status = "Closed" },
                new EventStatus { Id = 3, Status = "Finished" }
            );
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Users");

            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .IsRequired();

            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .IsRequired();

            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsRequired();

            entity.Property(e => e.EntityId).HasColumnName("Entity_Id");

            entity.HasOne(d => d.Entity)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.EntityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Entities");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    

}
