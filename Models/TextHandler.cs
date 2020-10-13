using System.ComponentModel.DataAnnotations;

namespace PasteBin.Models
{
    public class TextHandler
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public string Title { get; set; }
    }
}
