using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
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

        public FileHandler FileHandler;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public IndexModel(ILogger<IndexModel> logger, IHttpContextAccessor httpContext)
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

        public class InputModel
        {
            public string Text { get; set; } = string.Empty;

            [Required]
            [MaxLength(256)]
            public string Title { get; set; } = string.Empty;

            [Required]
            public int Duration { get; set; }

            [Required]
            public DateTime ExpireTime { get; set; } = DateTime.UtcNow;

            [Required]
            public bool IsEncrypted { get; set; }

            public string Password { get; set; } = string.Empty;

            public IFormFile? File { get; set; }

        }


        public void OnGet()
        {

        }
        
        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (Input.File == null)
            {
                ModelState.AddModelError(string.Empty, "Please select a file to upload.");
                _logger.LogInformation(LogEvents.FileUploadError, "No file selected");
                return Page();
            }

            // FileHandler = new(Input.File.FileName, DateTime.UtcNow.Add(TimeSpan.FromDays(365)), false);
            FileHandler = new();
            FileHandler.UploadFile(Input.File);

            _logger.LogInformation(LogEvents.FileUpload, "File uploaded successfully");
            return RedirectToPage("List");
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (Input.Title is not {Length: > 0})
            {
                ModelState.AddModelError(string.Empty, "Please make sure your text has a title");
                _logger.LogInformation(LogEvents.TextUploadError, "No title added to the text file error");
                return Page();
            }

            string filePath = Path.Combine(Locations.FileLocation, $"{Input.Title}.txt");

            int count = 1;
            while (System.IO.File.Exists(filePath))
            {
                filePath = Path.Combine(Locations.FileLocation, $"{Input.Title}-{count++}.txt");
            }


            DateTime expire;
            if (Input.Duration > 0)
            {
                expire = Input.ExpireTime.Add(TimeSpan.FromDays(Input.Duration));
            }
            else if (Input.Duration == 0)
            {
                expire = Input.ExpireTime.Add(TimeSpan.FromMinutes(1));
            }
            else 
            {
                expire = Input.ExpireTime.Add(TimeSpan.FromDays(3650));
            }

            FileHandler = new();
            FileHandler.FileName = Input.Title;
            FileHandler.ExpireTime = Input.ExpireTime.Add(TimeSpan.FromDays(Input.Duration));
            FileHandler.IsEncrypted = Input.IsEncrypted;

            if (Input.IsEncrypted)
            {
                FileHandler.WriteFileEncrypted(filePath, Input.Text, expire, true, Input.Password);
            }
            else 
            {
                FileHandler.WriteFile(filePath, Input.Text, expire, false);
            }

            _logger.LogInformation(LogEvents.TextUploaded, "Text has been uploaded successfully");

            return RedirectToPage("List");
        }
    }
}
