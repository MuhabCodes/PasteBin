using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
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
        
        public List<string> FileList { set; get; } = new List<string>();
        
        public ListModel(ILogger<ListModel> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }

        public void OnGet()
        {
            string textDirectory = Path.Combine(_env.ContentRootPath, "Data", "Text"); 
            if (Directory.Exists(textDirectory))
            {
                foreach (string item in new List<string>(Directory.GetFiles(textDirectory)))
                {
                    FileList.Add(Path.GetFileName(item));
                }
            }
        }
    }
}
