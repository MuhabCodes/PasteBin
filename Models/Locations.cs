using System.IO;
using Microsoft.AspNetCore.Hosting;
namespace PasteBin.Models
{
    public class Locations
   {
        public static string FileLocation(IWebHostEnvironment env) => 
           Path.Combine(env.ContentRootPath, "Data", "Text");
    }
}