using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PasteBin.Models;

namespace PasteBin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly IHttpContextAccessor _httpContext;
        
        private string directoryPath;

        [BindProperty]
        public TextHandler TextHandler { get; set; } = new();

        public IndexModel(ILogger<IndexModel> logger, IHttpContextAccessor httpContext)
        {
            _logger = logger;
            _httpContext = httpContext;


            if (_httpContext.HttpContext.User.Identity.IsAuthenticated)
            {
                directoryPath = Locations.UserFilesLocation(_httpContext.HttpContext.User.Identity.Name);
            }
            else
            {
                directoryPath = Locations.FileLocation;
            }

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            } 
        }

        public void OnGet()
        {

        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (String.IsNullOrWhiteSpace(TextHandler.Title))
            {
                ModelState.AddModelError(string.Empty, "Please make sure your text has a title");
                _logger.LogInformation(LogEvents.TextUploadError, "No title added to the text file error");
                return Page();
            }

            string filePath = Path.Combine(directoryPath, $"{TextHandler.Title}.txt");

            int count = 1;
            while (System.IO.File.Exists(filePath))
            {
                filePath = Path.Combine(directoryPath, $"{TextHandler.Title}-{count++}.txt");
            }
            await System.IO.File.WriteAllTextAsync(filePath, TextHandler.Text);
            _logger.LogInformation(LogEvents.TextUploaded, "Text has been uploaded successfully");
            return RedirectToPage("List");
        }
    }
}
