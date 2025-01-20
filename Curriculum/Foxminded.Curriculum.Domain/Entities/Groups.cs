using System.ComponentModel.DataAnnotations.Schema;

namespace Foxminded.Curriculum.Domain.Entities;

[Table("GROUPS", Schema = "dbo")]
public class Groups : BaseEntity
{
    [Column("NAME")]
    public required string Name { get; set; }

    [Column("COURSE_ID")]
    [ForeignKey(nameof(Course))]
    public required int Course_ID { get; set; }

    [Column("TEACHER_ID")]
    [ForeignKey(nameof(Teacher))]
    public int Teacher_ID { get; set; }

    public virtual required Courses Course { get; set; }
    public virtual Teachers? Teacher { get; set; }
}
