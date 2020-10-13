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
using PasteBin.Models;

namespace PasteBin.Pages
{
    public class ViewTextModel : PageModel
    {
        private readonly ILogger<ViewTextModel> _logger;

        public TextHandler? ViewedFile { get; set; }

        public ViewTextModel(ILogger<ViewTextModel> logger)
        {
            _logger=logger;
        }

        public void OnGet()
        {

        }
    }
}