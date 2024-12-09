using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VTYS.Models.Entity;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public bool IsMandatory { get; set; }

    public int Credit { get; set; }

    public int Class { get; set; }

    public int InstructorId { get; set; }

    public virtual ICollection<SelectedCourse> SelectedCourses { get; set; } = new List<SelectedCourse>();
    
        public async Task AssignMandatoryCoursesToStudentsAsync(VtysContext context)
    {
    
        var students = await context.Students.Where(s => s.Class.HasValue).ToListAsync();
        var mandatoryCourses = await context.Courses.Where(c => c.IsMandatory).ToListAsync();

        var selectedCourses = new List<SelectedCourse>();

        foreach (var student in students)
        {
    
            var coursesForStudent = mandatoryCourses.Where(c => c.Class == student.Class);

            foreach (var course in coursesForStudent)
            {
    
                var selectedCourse = new SelectedCourse
                {
                    StudentId = student.StudentId,
                    CourseId = course.CourseId,
                    InstructorId = course.InstructorId,
                    IsApproved = true
                };

                selectedCourses.Add(selectedCourse);
            }
        }
        context.SelectedCourses.AddRange(selectedCourses);
        await context.SaveChangesAsync();
    }

}
