using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.IO;
using PasteBin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Text;

namespace PasteBin.Pages
{
    public class ViewTextModel : PageModel
    {
        private readonly ILogger<ViewTextModel> _logger;
        
        private readonly IHttpContextAccessor _httpContext;
        
        public TextHandler ViewedFile { get; set; } = new();

        public string FilePath;

        public ViewTextModel(ILogger<ViewTextModel> logger, IHttpContextAccessor httpContext)
        {
            _logger=logger;
            _httpContext = httpContext;
            

            if (!Directory.Exists(Locations.FileLocation))
            {
                Directory.CreateDirectory(Locations.FileLocation);
            }

            if (!Directory.Exists(Locations.JsonLocation))
            {
                Directory.CreateDirectory(Locations.JsonLocation);
            }
        }

        public IActionResult OnGet(string fileName)
        {
            _logger.LogInformation(LogEvents.ViewFileRequest, "User has made a view file request at {UtcNow}", DateTime.UtcNow);
            ViewedFile.Title = fileName;
            FilePath = Path.Combine(Locations.FileLocation, fileName);
            if (System.IO.File.Exists(FilePath))
            {
                if (!FileHandler.IfImage(fileName))
                {
                    ViewedFile.Text = System.IO.File.ReadAllText(FilePath);
                }
                return Page();
            }
            _logger.LogInformation(LogEvents.ViewRequestError, "Error in view file request at {UtcNow}", DateTime.UtcNow);
            return NotFound();
        }
    }
}