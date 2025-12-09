using System.ComponentModel.DataAnnotations;

namespace FreeBilling.Web.Models;

public class ContactViewModel
{
    [MaxLength(50), Required]
    public string Name { get; set; } = String.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = String.Empty;

    [MaxLength(50), Required]
    public string Subject { get; set; } = String.Empty;

    [MaxLength(500), MinLength(25), Required]
    public string Body { get; set; } = String.Empty;
}
