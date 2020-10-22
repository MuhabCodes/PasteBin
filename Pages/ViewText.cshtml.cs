using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.IO;
using PasteBin.Models;
using Microsoft.AspNetCore.Hosting;

namespace PasteBin.Pages
{
    public class ViewTextModel : PageModel
    {
        private readonly ILogger<ViewTextModel> _logger;
        
        private readonly IWebHostEnvironment _env;

        public string TextDirectory { get; set; }

        public TextHandler ViewedFile { get; set; } = new();

        public ViewTextModel(ILogger<ViewTextModel> logger, IWebHostEnvironment env)
        {
            _logger=logger;
            _env = env;

            TextDirectory = Locations.FileLocation(env);

            if (!Directory.Exists(TextDirectory))
            {
                Directory.CreateDirectory(TextDirectory);
            }
        }

        public IActionResult OnGet(string FileName)
        {
            ViewedFile.Title = FileName;
            if (Directory.Exists(TextDirectory))
            {
                string FilePath = Path.Combine(TextDirectory, FileName);
                if (System.IO.File.Exists(FilePath))
                {
                    ViewedFile.Text = System.IO.File.ReadAllText(Path.Combine(TextDirectory, FileName));
                    return Page();
                }
            }
            return NotFound();
        }
    }
}