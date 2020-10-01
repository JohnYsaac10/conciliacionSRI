using SRIconciliacion.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SRIconciliacion.Views
{
    public partial class ConciliacionSRI : System.Web.UI.Page
    {
        private static string BASE_URL = @"C:\SRI\XML\";

        private static Rutas _rutas;

        private static SriXmlFileManager fileManager;
        public ConciliacionSRI()
        {
            fileManager = new SriXmlFileManager();
            //1. create folder if not exists
            _rutas = fileManager.CreateFolderOrPaths(DateTime.Now.ToString("dd/MM/yyyy"), BASE_URL, true);
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        [WebMethod]
        public static string LoadFiles()
        {
            var res = fileManager.getAllFiles(_rutas, "*.*", null);

            var json = new JavaScriptSerializer().Serialize(new { Ok = true, Data = res });

            return json;
        }


        [WebMethod]
        public static string GetFilesBy(string fecha, string institucion, string servicio)
        {
            var newRuta = fileManager.CreateFolderOrPaths(fecha, BASE_URL, false);
            
            var res = fileManager.getFilesBy(newRuta, institucion, servicio);

            var json = new JavaScriptSerializer().Serialize(new { Ok = true, Data = res });

            return json;
        }

    }
}