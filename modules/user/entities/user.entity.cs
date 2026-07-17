

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UserEntity : BaseEntity
{

    [Required]
    public string username { get; set; }=String.Empty;

    public string? bio { get; set; }

    public string? pronouns { get; set; }

    public Guid? profileMediaId { get; set; }


}