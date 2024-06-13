using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Eats_Tech.Providers
{
    public enum Folders
    {
        Platillos = 0, Images = 1, Users = 2, Temp = 3
    }

    public class PathProvider
    {
        private IWebHostEnvironment hostEnvironment;

        public PathProvider(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        public string MapPath(string fileName, Folders folder)
        {
            string path = "";
            if(folder == Folders.Platillos)
            {
                string carpeta = "Images/Platillos";
                path = Path.Combine(this.hostEnvironment.WebRootPath, carpeta, fileName);
            }
            else if(folder == Folders.Users)
            {
                string carpeta = "Images/Users";
                path = Path.Combine(this.hostEnvironment.WebRootPath, carpeta, fileName);
            }

            return path;
        }
    }
}
