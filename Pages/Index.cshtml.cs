using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using PasteBin.Models;
using Microsoft.AspNetCore.Hosting;

namespace PasteBin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly IWebHostEnvironment _env;

        public string TextDirectory { get; set; }

        [BindProperty]
        public TextHandler TextHandler { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
           
            TextDirectory = Locations.FileLocation(env);

            if (!Directory.Exists(TextDirectory))
            {
                Directory.CreateDirectory(TextDirectory);
            }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (String.IsNullOrWhiteSpace(TextHandler.Title))
            {
                return RedirectToPage("Index");
            }

            string FilePath = Path.Combine(TextDirectory, $"{TextHandler.Title}.txt");
            int Count = 1;
            while (System.IO.File.Exists(FilePath))
            {
                FilePath = Path.Combine(TextDirectory, $"{TextHandler.Title}-{Count++}.txt");
            }
            await System.IO.File.WriteAllTextAsync(FilePath,TextHandler.Text);
            return RedirectToPage("List");
        }
    }
}
