using System.ComponentModel.DataAnnotations;

namespace PasteBin.Models
{
    public class TextHandler
    {
        public string Text { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;
    }
}