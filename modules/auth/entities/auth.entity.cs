

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public enum RolesEnum{
    User
}
public class AuthEntity:BaseEntity
{
    [Required]
    public Guid userId{get;set;}

    [Required]
    [MaxLength(50)]
    public string username {get; set;}= string.Empty;

    [Required]
    [MaxLength(255)]
    public string email {get; set;}= string.Empty;

    [Required]
    public string hashedPassword {get;set;}=string.Empty;

    [MaxLength(1000)]
    public string? refreshToken{get; set;}=string.Empty;

    public DateTime? refreshExpiry{get;set;}

    public RolesEnum role {get;set;}

    [ForeignKey(nameof(userId))]
    public UserEntity user{get; set;}=null!; 
}