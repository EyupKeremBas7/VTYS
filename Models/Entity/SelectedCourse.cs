using System;
using System.Collections.Generic;

namespace VTYS.Models.Entity;

public partial class SelectedCourse
{
    public int SelectionId { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public int? InstructorId { get; set; }

    public bool IsApproved { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Instructor Instructor { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
