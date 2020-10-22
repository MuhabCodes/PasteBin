using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace PasteBin.Models
{
    public class TextHandler
    {
        public string Text { get; set; } = string.Empty;
        
        [Required]
        public string Title { get; set; } = string.Empty;
    }
}
