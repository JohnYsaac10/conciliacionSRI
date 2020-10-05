using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;

namespace SRIconciliacion.Utils
{
    public class ConciliaMat
    {

        public void conciliar()
        {
            string Path1 = @"C:\SRI\XML\MAT\\M20201001\mav-122-07-01-2015.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(Path1);

            XmlNode oNode = doc.SelectSingleNode(".//detallesPagos");
            XmlNodeList Stay = oNode.SelectNodes("detallePago");

            foreach (XmlNode rmxn in Stay)
            {
                var t = rmxn.InnerText;
                var fd = rmxn.Attributes["pago"].Value;
                var rt = rmxn.Name;
                var cvv = rmxn.NamespaceURI;
                var tt = rmxn.ChildNodes;
                var nn = rmxn.InnerText;
            }

            var u = "";

        }
    }
}