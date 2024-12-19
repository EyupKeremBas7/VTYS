using System;
using System.Collections.Generic;

namespace VTYS.Models.Entity;

public partial class Instructor
{
    public int InstructorId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Title { get; set; }

    public string? Department { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<SelectedCourse> SelectedCourses { get; set; } = new List<SelectedCourse>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public string? EMail { get; set; }
}
