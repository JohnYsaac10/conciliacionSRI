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
using System.Xml.Serialization;
using SRIconciliacion.Models;

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
            _rutas = fileManager.CreateFolderOrPaths(DateTime.Now.AddDays(1).ToString("dd/MM/yyyy"), BASE_URL);
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        [WebMethod]
        public static string LoadFiles()
        {
            var res = fileManager.GetAllFiles(_rutas, "*.*", null);

            var json = new JavaScriptSerializer().Serialize(new { Ok = true, Data = res });

            //return json;

            /*string Path1 = @"C:\SRI\XML\MAT\\M20201001\mav-122-07-01-2015.xml";
            string Path2 = @"C:\SRI\XML\MAT\\M20201001\mav-132-07-01-2015.xml";

            XmlSerializer serializer = new XmlSerializer(typeof(recaudacionVehiculos));

            StreamReader reader = new StreamReader(Path1);
            var xmlObj = (recaudacionVehiculos)serializer.Deserialize(reader);
            reader.Close();  */

            /*ConciliaMat i = new ConciliaMat();
            i.conciliar();

            var t = 0;*/

            return json;
        }


        [WebMethod]
        public static string GetFilesBy(string fecha, string institucion, string servicio)
        {
            var newRuta = fileManager.CreateFolderOrPaths(fecha, BASE_URL, false);
            
            var res = fileManager.GetFilesBy(newRuta, institucion, servicio);

            var json = new JavaScriptSerializer().Serialize(new { Ok = true, Data = res });

            return json;
        }

    }
}