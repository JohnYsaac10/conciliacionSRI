using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SRIconciliacion.Utils
{
    public class SriXmlFileManager
    {
        public List<FileDetail> getAllFiles(Rutas rutas, string institucion, string directPath)
        {
            List<FileDetail> fileList = new List<FileDetail>();

            foreach (var path in rutas.Paths)
            {
                var files = Directory.GetFiles(String.IsNullOrEmpty(directPath) ? path.Value : directPath, institucion, SearchOption.TopDirectoryOnly)
                                      .Where(file => !file.Contains("_OUT")); //add param true (query == b)
                foreach (var filePath in files)
                {
                    var fileName = Path.GetFileName(filePath);
                    fileList.Add(new FileDetail
                    {
                        Fecha = rutas.Fecha,
                        NombreArchivo = fileName,
                        CodIFI = fileName.Contains("ofp") ? fileName.Split('-').Last().Split('.')[0] : fileName.Split('-')[1],
                        Institucion = "",
                        Servicio = path.Key == "MAT" ? "MATRICULACIÓN" : path.Key,
                        Usuario = "Admin"
                    });
                }

                if (!String.IsNullOrEmpty(directPath))
                    break;
            }

            return fileList;
        }


        public List<FileDetail> getFilesBy(Rutas rutas, string institucion, string servicio)
        {
            if (String.IsNullOrEmpty(institucion) && String.IsNullOrEmpty(servicio))      //00
            {
                return getAllFiles(rutas, "*.*", null);   // get all files
            }

            if (!String.IsNullOrEmpty(institucion) && String.IsNullOrEmpty(servicio))      //10
            {
                return getAllFiles(rutas, "*" + institucion + "*", null);   // get all files by institucion
            }

            if (String.IsNullOrEmpty(institucion) && !String.IsNullOrEmpty(servicio))      //01
            {
                return getAllFiles(rutas, "*.*", rutas.Paths[servicio]);  // get all files by servicio
            }

            if (!String.IsNullOrEmpty(institucion) && !String.IsNullOrEmpty(servicio))      //11
            {
                return getAllFiles(rutas, "*" + institucion + "*", rutas.Paths[servicio]);   // get all files by institucion and servicio
            }

            return new List<FileDetail>();
        }

        public Rutas CreateFolderOrPaths(string fecha, string baseUrl, bool folderToo)  // format 30/09/2020
        {
            var DateSplited = fecha.Split('/');
            var DateFormated = DateSplited[2] + DateSplited[1] + DateSplited[0];

            List<string> paths = new List<string>();
            paths.Add(@"CEP\S");
            paths.Add(@"RISE\C");
            paths.Add(@"MAT\M");

            var rutass = new Rutas
            {
                Fecha = fecha.Replace("/", "-")   //presentation Date
            };

            foreach (var folder in paths)
            {
                var absolutePath = baseUrl + folder + DateFormated;
                if (folderToo && !Directory.Exists(absolutePath))
                {
                    Directory.CreateDirectory(absolutePath);
                }

                rutass.Paths.Add(folder.Split('\\')[0], absolutePath);
            }

            return rutass;
        }

    }  

    

    public class Rutas
    {
        public string Fecha { get; set; }

        public Dictionary<string, string> Paths = new Dictionary<string, string>();
    }

    public class FileDetail
    {
        public string CodIFI { get; set; }
        public string Institucion { get; set; }
        public string Servicio { get; set; }
        public string NombreArchivo { get; set; }
        public string Usuario { get; set; }
        public string Fecha { get; set; }
    }
}