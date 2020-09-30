using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SRIconciliacion.Views
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private readonly string BASE_URL = @"C:\SRI\XML\";

        private readonly Rutas _rutas;
        public WebForm1()
        {
            _rutas = CreateFolders();
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //1. create folder if not exists
        public Rutas CreateFolders()
        {
            var current_date = DateTime.Now.ToString("yyyy/MM/dd/");
            current_date = current_date.Replace("/", "");

            var pathCEP = BASE_URL + @"CEP\S" + current_date;
            if (!Directory.Exists(pathCEP))
            {
                Directory.CreateDirectory(pathCEP);
            }

            var pathRISE = BASE_URL + @"RISE\C" + current_date;
            if (!Directory.Exists(pathRISE))
            {
                Directory.CreateDirectory(pathRISE);
            }


            var pathMAT = BASE_URL + @"MAT\M" + current_date;
            if (!Directory.Exists(pathMAT))
            {
                Directory.CreateDirectory(pathMAT);
            }

            return new Rutas {
                pathMAT = pathMAT,
                pathRISE = pathRISE,
                pathCEP = pathCEP
            };
        }

        public List<string> getFiles()
        {
            //could filter -out.xml or only .xml
            var filesCEP = Directory.GetFiles(_rutas.pathCEP, "*.*", SearchOption.TopDirectoryOnly);

            var filesRISE = Directory.GetFiles(_rutas.pathRISE, "*.*", SearchOption.TopDirectoryOnly);

            var filesMAT = Directory.GetFiles(_rutas.pathMAT, "*.*", SearchOption.TopDirectoryOnly);

            var merge1 = filesCEP.Concat(filesRISE).ToArray();
            var merge2 = merge1.Concat(filesMAT);

            return merge2.ToList();
        }


        //[WebMethod]


        public class Rutas
        {
            public string pathMAT { get; set; }
            public string pathRISE { get; set; }
            public string pathCEP { get; set; }
        }

    }
}