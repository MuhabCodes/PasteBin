using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace PasteBin.Models
{
    public static class Locations
    {
        public static string FileLocation(IWebHostEnvironment env) =>
            Path.Combine(env.ContentRootPath, "Data", "Text");
    }
}