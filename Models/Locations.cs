using System.IO;

namespace PasteBin.Models
{
    public class Locations
    {
        public static string FileLocation = Path.Combine("wwwroot","files");
        
        public static string UsersLocation = Path.Combine("wwwroot","users");

        public static string JsonLocation = Path.Combine("wwwroot", "json");
    }
}