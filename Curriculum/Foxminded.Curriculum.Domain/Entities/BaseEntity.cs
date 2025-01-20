using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foxminded.Curriculum.Domain.Entities;

public abstract class BaseEntity
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
}
