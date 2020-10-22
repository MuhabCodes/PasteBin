using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PasteBin.Models;

namespace PasteBin.Pages
{
    public class ListModel : PageModel
    {
        private readonly ILogger<ListModel> _logger;

        private readonly IWebHostEnvironment _env;
        
        public string TextDirectory { get; set; }

        public List<string> FileList { get; set; } = new List<string>();
        
        public ListModel(ILogger<ListModel> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            
            TextDirectory = Locations.FileLocation(env);

            if (!Directory.Exists(TextDirectory))
            {
                Directory.CreateDirectory(TextDirectory);
            }
        }

        public void OnGet()
        {
            if (Directory.Exists(TextDirectory))
            {
                foreach (string item in new List<string>(Directory.GetFiles(TextDirectory)))
                {
                    FileList.Add(Path.GetFileName(item));
                }
            }
        }
        
        public IActionResult OnPostDelete(string FileName)
        {
            if (Directory.Exists(TextDirectory))
            {
                string FilePath = Path.Combine(TextDirectory, FileName);
                if (System.IO.File.Exists(FilePath))
                {
                    System.IO.File.Delete(FilePath);
                    return RedirectToPage("List");
                }
            }
            return NotFound();
        }

        public IActionResult OnPostDeleteAll()
        {
            if (Directory.Exists(TextDirectory))
            {
                Directory.Delete(TextDirectory, true);
                Directory.CreateDirectory(TextDirectory);
            }
            return RedirectToPage("Index");
        }
    }
}
