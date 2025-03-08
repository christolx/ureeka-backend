using System.ComponentModel.DataAnnotations;

namespace ureeka_backend.Models;

public class RegisterReq
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}