using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.IO;
using PasteBin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace PasteBin.Pages
{
    public class ViewTextModel : PageModel
    {
        private readonly ILogger<ViewTextModel> _logger;
        
        private readonly IHttpContextAccessor _httpContext;
        
        private string directoryPath;
        
        public TextHandler ViewedFile { get; set; } = new();

        public ViewTextModel(ILogger<ViewTextModel> logger, IHttpContextAccessor httpContext)
        {
            _logger=logger;
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

        public IActionResult OnGet(string fileName)
        {
            ViewedFile.Title = fileName;
            string filePath = Path.Combine(directoryPath, fileName);
            if (System.IO.File.Exists(filePath))
            {
                ViewedFile.Text = System.IO.File.ReadAllText(filePath);
                return Page();
            }
            return NotFound();
        }
    }
}