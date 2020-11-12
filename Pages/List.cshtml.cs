using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PasteBin.Models;
using System.Text.Json;

namespace PasteBin.Pages
{
    public class ListModel : PageModel
    {
        private readonly ILogger<ListModel> _logger;
        
        private readonly IHttpContextAccessor _httpContext;

        public List<string> fileList { get; set; } = new List<string>();
        
        public ListModel(ILogger<ListModel> logger, IHttpContextAccessor httpContext)
        {
            _logger = logger;
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

        public void OnGet()
        {
            foreach (string item in new List<string>(Directory.GetFiles(Locations.FileLocation)))
            {
                string jsonFile = $"{Path.GetFileName(item)}.json";
                string json = System.IO.File.ReadAllText(Path.Combine(Locations.JsonLocation, jsonFile));
                FileHandler fileHandler = JsonSerializer.Deserialize<FileHandler>(json);
                if (DateTime.UtcNow >= fileHandler.ExpireTime)
                {
                    System.IO.File.Delete(item);
                    continue;
                }
                fileList.Add(Path.GetFileName(item));
            }
        }
        
        public IActionResult OnPostDelete(string fileName)
        {
            _logger.LogInformation(LogEvents.DeleteFileRequest, "User made a delete file request at {UtcNow}", DateTime.UtcNow);
            string filePath = Path.Combine(Locations.FileLocation, fileName);
            if (System.IO.File.Exists(filePath))
            {
                string jsonPath = Path.Combine(Locations.JsonLocation, $"{fileName}.json");
                if (System.IO.File.Exists(jsonPath))
                {
                    System.IO.File.Delete(jsonPath);
                }
                System.IO.File.Delete(filePath);
                return RedirectToPage("List");
            }
            _logger.LogInformation(LogEvents.DeleteRequestError, "Error in delete request");
            return NotFound();
        }

        public IActionResult OnPostDeleteAll()
        {
            _logger.LogInformation(LogEvents.DeleteAllRequest, "User has made a delete all files request at {UtcNow}", DateTime.UtcNow);
            Directory.Delete(Locations.FileLocation, true);
            Directory.Delete(Locations.JsonLocation, true);
            return RedirectToPage("Index");
        }
    }
}
