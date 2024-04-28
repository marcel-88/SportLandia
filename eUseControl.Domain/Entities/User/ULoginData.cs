using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eUseControl.Domain.Entities.User
{
    public class ULoginData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 30 characters.")]
    public string Username { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 50 characters.")]
    public string Password { get; set; }

    [Required]
    [StringLength(30, ErrorMessage = "Email must be up to 30 characters long.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }

    [DataType(DataType.Date)]
    public DateTime LastLogin { get; set; }

    [StringLength(30)]
    public string LasIp { get; set; }

    public URole Level {get; set; }
}
}
