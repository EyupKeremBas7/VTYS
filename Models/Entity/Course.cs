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

    public int? InstructorId { get; set; }

    public virtual ICollection<SelectedCourse> SelectedCourses { get; set; } = new List<SelectedCourse>();
    

}
