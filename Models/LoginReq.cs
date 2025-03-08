using System.ComponentModel.DataAnnotations;

namespace ureeka_backend.Models;

public class LoginReq
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}