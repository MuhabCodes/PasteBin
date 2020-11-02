using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace PasteBin.Models
{
    public class ApplicationUser
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

    }
}