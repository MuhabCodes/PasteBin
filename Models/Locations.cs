using System.IO;

namespace PasteBin.Models
{
    public class Locations
    {
        public static string FileLocation = Path.Combine("Data","Text");
        
        public static string UsersLocation = Path.Combine("Data","Users");

        public static string UserFilesLocation(string username) =>
            Path.Combine(FileLocation,username);
    }
}