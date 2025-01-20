using System.ComponentModel.DataAnnotations.Schema;

namespace Foxminded.Curriculum.Domain.Entities;

[Table("TEACHERS")]
public class Teachers : BaseEntity
{
    [Column("FIRST_NAME")]
    public required string First_Name { get; set; }

    [Column("LAST_NAME")]
    public required string Last_Name { get; set; }
    public string FullName { get { return $"{First_Name} {Last_Name}"; } private set { } }
}
