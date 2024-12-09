using System;
using System.Collections.Generic;

namespace VTYS.Models.Entity;

public partial class Instructor
{
    public int InstructorId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Title { get; set; }

    public string EMail { get; set; } = null!;

    public string Department { get; set; } = null!;

    public virtual ICollection<SelectedCourse> SelectedCourses { get; set; } = new List<SelectedCourse>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
