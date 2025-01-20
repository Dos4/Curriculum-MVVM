using System.ComponentModel.DataAnnotations.Schema;

namespace Foxminded.Curriculum.Domain.Entities;

[Table("COURSES")]
public class Courses : BaseEntity
{
    [Column("NAME")]
    public required string Name { get; set; }

    [Column("DESCRIPTION")]
    public string? Description { get; set; }
}
