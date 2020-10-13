using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PasteBin.Models;
using Microsoft.AspNetCore.Hosting;

namespace PasteBin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly IWebHostEnvironment _env;

        [BindProperty]
        public TextHandler TextHandler { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (TextHandler.Title == "" || TextHandler.Text == "")
            {
                return RedirectToPage("Index");
            }
            string directoryPath = Path.Combine(_env.ContentRootPath, "Data", "Text");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, $"{TextHandler.Title}.txt");
            int count = 1;
            while (System.IO.File.Exists(filePath))
            {
                filePath = Path.Combine(directoryPath, $"{TextHandler.Title}-({count++}).txt");
            }
            await System.IO.File.WriteAllTextAsync(filePath,TextHandler.Text);
            return RedirectToPage("List");
        }
    }
}
