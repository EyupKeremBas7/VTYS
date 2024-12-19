using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VTYS.Models.Entity;

public partial class VtysContext : DbContext
{
    public VtysContext()
    {
    }

    public VtysContext(DbContextOptions<VtysContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; } = null!;

    public virtual DbSet<Instructor> Instructors { get; set; }  = null!;

    public virtual DbSet<SelectedCourse> SelectedCourses { get; set; } = null!;

    public virtual DbSet<Student> Students { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=DESKTOP-0H5KPU2;Database=VTYS;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Course__C92D7187756488EC");

            entity.ToTable("Course");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName).HasMaxLength(50);
            entity.Property(e => e.InstructorId).HasColumnName("InstructorID");
            entity.Property(e => e.IsMandatory).HasColumnName("isMandatory");
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.InstructorId).HasName("PK__Instruct__9D010B7B735385FC");

            entity.ToTable("Instructor");

            entity.Property(e => e.InstructorId).HasColumnName("InstructorID");
            entity.Property(e => e.Department).HasMaxLength(50);
            entity.Property(e => e.EMail)
                .HasMaxLength(50)
                .HasColumnName("EMail");
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
        });
        
        modelBuilder.Entity<SelectedCourse>(entity =>
        {
            entity.HasKey(e => e.SelectionId);

            entity.ToTable("SelectedCourse");

            entity.Property(e => e.SelectionId).HasColumnName("SelectionID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.InstructorId).HasColumnName("InstructorID");
            entity.Property(e => e.IsApproved).HasColumnName("isApproved");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Course).WithMany(p => p.SelectedCourses)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SelectedCourse_Course");

            entity.HasOne(d => d.Instructor).WithMany(p => p.SelectedCourses)
                .HasForeignKey(d => d.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SelectedCourse_Instructor");

            entity.HasOne(d => d.Student).WithMany(p => p.SelectedCourses)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SelectedCourse_Student");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__32C52A798905032A");

            entity.ToTable("Student");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.EMail)
                .HasMaxLength(100)
                .HasColumnName("E-mail");
            entity.Property(e => e.Fullname).HasMaxLength(50);
            entity.Property(e => e.InstructorId).HasColumnName("InstructorID");
            entity.Property(e => e.Major).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);

            entity.HasOne(d => d.Instructor).WithMany(p => p.Students)
                .HasForeignKey(d => d.InstructorId)
                .HasConstraintName("FK_Student_Instructor");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
