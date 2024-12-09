using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace VTYS.Models.Entity;

public partial class Student
{
    public int StudentId { get; set; }

    public string Fullname { get; set; } = null!;

    public string EMail { get; set; } = null!;

    public int? InstructorId { get; set; }

    public string Password { get; set; } = null!;

    public string Major { get; set; } = null!;

    public int? Class { get; set; }

    public virtual Instructor? Instructor { get; set; }

    public virtual ICollection<SelectedCourse> SelectedCourses { get; set; } = new List<SelectedCourse>();

    public async Task<List<Student>> GetAllStudentsAsync()
    {
        using (var context = new VtysContext())
        {
            // Tüm öğrencileri getir
            return await context.Students.ToListAsync();
        }
    }

    public async Task<Student?> GetStudentByIdAsync(int id)
    {
        using (var context = new VtysContext())
        {
            // Belirli bir öğrenciyi getir
            return await context.Students.FindAsync(id);
        }
    }

    public async Task AssignMandatoryCoursesAsync(VtysContext context)
        {
            var mandatoryCourses = await context.Courses
                .Where(c => c.Class == this.Class && c.IsMandatory)
                .ToListAsync();

            foreach (var course in mandatoryCourses)
            {
                var selectedCourse = new SelectedCourse
                {
                    StudentId = this.StudentId,
                    CourseId = course.CourseId,
                    InstructorId = course.InstructorId,
                    IsApproved = true
                };

                context.SelectedCourses.Add(selectedCourse);
            }

            await context.SaveChangesAsync();
        }

    public async Task<Student?> GetStudentWithInstructorAsync(int id)
    {
        using (var context = new VtysContext())
        {
            // İlişkili verileri include ederek getir
            return await context.Students
                .Include(s => s.Instructor)
                .Include(s => s.SelectedCourses)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }
    }
}
