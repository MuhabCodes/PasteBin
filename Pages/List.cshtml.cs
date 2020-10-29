using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PasteBin.Models;

namespace PasteBin.Pages
{
    public class ListModel : PageModel
    {
        private readonly ILogger<ListModel> _logger;
        
        private readonly IHttpContextAccessor _httpContext;
        
        private string directoryPath;

        public List<string> fileList { get; set; } = new List<string>();
        
        public ListModel(ILogger<ListModel> logger, IHttpContextAccessor httpContext)
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
            foreach (string item in new List<string>(Directory.GetFiles(directoryPath)))
            {
                fileList.Add(Path.GetFileName(item));
            }
        }
        
        public IActionResult OnPostDelete(string FileName)
        {
            _logger.LogInformation(LogEvents.DeleteFileRequest, "User made a delete file request at {UtcNow}", DateTime.UtcNow);
            string filePath = Path.Combine(directoryPath, FileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                return RedirectToPage("List");
            }
            _logger.LogInformation(LogEvents.DeleteRequestError, "Error in delete request");
            return NotFound();
        }

        public IActionResult OnPostDeleteAll()
        {
            _logger.LogInformation(LogEvents.DeleteAllRequest, "User has made a delete all files request at {UtcNow}", DateTime.UtcNow);
            Directory.Delete(directoryPath, true);
            return RedirectToPage("Index");
        }
    }
}
