using SRIconciliacion.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;

namespace SRIconciliacion.Views
{
    public partial class ConciliacionSRI : System.Web.UI.Page
    {
        private static ConciliacionService NegFac = new ConciliacionService();

        public static int ADD_DAYS = 1;  //dia de la capeta----- en este caso. maañana

        private static string BASE_URL = @"C:\SRI\XML\";  //ESTE VALOR DEBE QUITARSE EN PRODUCCION

        private static Rutas _rutas;

        private static SriFileManager fileManager;

        public ConciliacionSRI()
        {
            fileManager = new SriFileManager();
            //get path from dataBase
            var pathDB = NegFac.ConsultaPathFilesSRIConciliacionFacilitoSwitch();
            var objRes = new ConciliacionManager().XmlToObject("<Datos><Table><Path>Hola</Path></Table></Datos>");
            BASE_URL = string.IsNullOrEmpty(BASE_URL) ? objRes[0].Path : BASE_URL;
            //1. create folder if not exists
            _rutas = fileManager.CreateFolderOrPaths(DateTime.Now.AddDays(ADD_DAYS).ToString("dd/MM/yyyy"), BASE_URL, true);
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        [WebMethod(EnableSession = true)]
        public static string GetInstituciones()
        {
            /*Facilito.WEBUtil ObjWebUtil = new Facilito.WEBUtil();

            var instituciones = NegFac.Consultar_Instituciones_SRI();

            return ObjWebUtil.ConvertJson(instituciones);*/

            return "";

        }


        [WebMethod(EnableSession = true)]
        public static string GetProductos()
        {
            /*var productos = NegFac.Consultar_Productos_SRI();

            Facilito.WEBUtil ObjWebUtil = new Facilito.WEBUtil();

            return ObjWebUtil.ConvertJson(productos);*/

            return "";
        }



        [WebMethod(EnableSession = true)]
        public static string LoadFiles()
        {
            var res = fileManager.GetAllFiles(_rutas, "*.xml", null);

            var json = new JavaScriptSerializer().Serialize(new { Ok = true, Data = res });

            return json;
        }


        [WebMethod(EnableSession = true)]
        public static string GetFilesBy(string fecha, string institucion, string servicio)
        {
            //add one day to currente date
            string tomorrow = Convert.ToDateTime(fecha).AddDays(ADD_DAYS).ToString("dd/MM/yyyy");

            var newRuta = fileManager.CreateFolderOrPaths(tomorrow, BASE_URL, false);

            var res = fileManager.GetFilesBy(newRuta, institucion, servicio);

            var json = new JavaScriptSerializer().Serialize(new { Ok = true, Data = res });

            return json;
        }


        [WebMethod(EnableSession = true)]
        public static string Conciliar(string fecha, string institucion, string servicio, bool enviarEmail)
        {
            var newRuta = fileManager.CreateFolderOrPaths(Convert.ToDateTime(fecha).AddDays(ADD_DAYS).ToString("dd/MM/yyyy"), BASE_URL, false);

            var ArchivosConciliar = fileManager.GetFilesBy(newRuta, institucion, servicio);

            //llamar a la base. comparar. y crear los archivos _OUT.XML y Enviar correo
            //  ...
            //call function Conciliar
            ConciliacionManager conciliaManager = new ConciliacionManager();
            conciliaManager.Conciliar(newRuta, ArchivosConciliar, enviarEmail);
            //.....

            //listar los nuevos archivos
            fileManager.Select_OUT_XML = true;
            var res = fileManager.GetFilesBy(newRuta, institucion, servicio);

            return new JavaScriptSerializer().Serialize(new { Ok = true, Data = res });
        }



        [WebMethod(EnableSession = true)]  // WebMethod para descargar el archivo 
        public static void Download(string nombreArchivo, string servicio, string fecha)
        {

            var newRuta = fileManager.CreateFolderOrPaths(Convert.ToDateTime(fecha).AddDays(ADD_DAYS).ToString("dd/MM/yyyy"), BASE_URL, false);

            var rutaArchivoDownload = newRuta.Paths[servicio] + @"\" + nombreArchivo;

            var btFile = fileManager.FileToByteArray(rutaArchivoDownload);

            HttpContext.Current.Response.AddHeader("Content-disposition", "attachment; filename=" + nombreArchivo);
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.BinaryWrite(btFile);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

        }

    }


    //manager class
    public class SriFileManager
    {
        public bool Select_OUT_XML = false;
        public List<FileDetail> GetAllFiles(Rutas rutas, string searchPattern, string servicio)
        {
            char criterioSplit = Select_OUT_XML ? '_' : '.';

            List<FileDetail> fileList = new List<FileDetail>();

            foreach (var path in rutas.Paths)
            {
                var files = Directory.GetFiles(String.IsNullOrEmpty(servicio) ? path.Value : rutas.Paths[servicio], searchPattern, SearchOption.TopDirectoryOnly)
                                      .Where(file => file.ToUpper().Contains("_OUT.XML") == Select_OUT_XML); //add bool param (query == b)
                foreach (var filePath in files)
                {
                    var fileName = Path.GetFileName(filePath);
                    //var keyServ = path.Key == "MAT" ? "MATRICULACIÓN" : path.Key;
                    fileList.Add(new FileDetail
                    {
                        Fecha = GetFileDate(fileName),
                        NombreArchivo = fileName,
                        CodIFI = fileName.Contains("ofp") ? fileName.Split('-').Last().Split(criterioSplit)[0] : fileName.Split('-')[1],
                        Institucion = "",
                        Servicio = String.IsNullOrEmpty(servicio) ? path.Key : servicio,
                        Usuario = "Admin"
                    });
                }

                if (!String.IsNullOrEmpty(servicio))
                    break;
            }

            return fileList;
        }


        public List<FileDetail> GetFilesBy(Rutas rutas, string institucion, string servicio)
        {
            if (String.IsNullOrEmpty(institucion))      //there're two options (service null, service NOT null)
            {
                return GetAllFiles(rutas, "*.xml", servicio);
            }

            if (!String.IsNullOrEmpty(institucion))    //there're two options (service null, service NOT null)
            {
                return GetAllFiles(rutas, "*" + institucion + "*.xml", servicio);
            }

            return new List<FileDetail>();
        }

        public Rutas CreateFolderOrPaths(string fecha, string baseUrl, bool folderToo = true)  // format 30/09/2020
        {
            /*
                 Las transacciones del viernes se concilian el lunes.
                 Las transacciones del sábado, domingo y lunes se concilia el martes
                 Las transacciones de feriados se concilian el siguiente día hábil de conciliación  **
             */

            var today = Convert.ToDateTime(fecha).AddDays(-ConciliacionSRI.ADD_DAYS);
            if (today.DayOfWeek == DayOfWeek.Friday)
            {
                string monday = today.AddDays(3).ToString("dd/MM/yyyy");
                fecha = monday;
            }

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


        public byte[] FileToByteArray(string fileName)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(fileName,
                                           FileMode.Open,
                                           FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            buff = br.ReadBytes((int)numBytes);
            return buff;
        }

        public string GetFileDate(string fileName)
        {
            if (fileName.Contains("mav"))
            {
                var splited = fileName.Split('-');
                return (splited[2] + "-" + splited[3] + "-" + splited[4]).Split('.')[0].Split('_')[0];
            }
            if (fileName.Contains("RISE"))
            {
                var splited = fileName.Split('-');
                return splited[2].Substring(0, 2) + "-" + splited[2].Substring(2, 2) + "-" + splited[2].Substring(4, 4);
            }
            if (fileName.Contains("ofp"))
            {
                var splited = fileName.Split('-');
                var dia = int.Parse(splited[1]) < 10 ? "0" + splited[1] : splited[1];
                var mes = int.Parse(splited[2]) < 10 ? "0" + splited[2] : splited[2];
                return dia + "-" + mes + "-" + splited[3].Split('.')[0].Split('_')[0];
            }
            return "";
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

    public class Cons { public string Producto { get; set; } public string Cabecera { get; set; }}


    ///conciliacion Manager
    public class ConciliacionManager
    {

        public void Conciliar(Rutas rutas, List<FileDetail> archivosConciliar, bool envioCorreo)
        {
            ConciliacionService NegFac = new ConciliacionService();
            foreach (var archivo in archivosConciliar)
            {
                var cons = GetCons(archivo.Servicio);

                var pathFile = rutas.Paths[archivo.Servicio] + @"\" + archivo.NombreArchivo;

                XmlDocument doc = new XmlDocument();
                doc.Load(pathFile);

                XmlNodeList nl0 = doc.SelectNodes(cons.Cabecera);
                XmlNode node0 = nl0[0];

                var fechaPagoCabecera = "";
                foreach (XmlNode xnode3 in node0.ChildNodes)
                {
                    if (xnode3.Name == "fechaRecaudacion")
                    {
                        fechaPagoCabecera = xnode3.InnerText;
                        break;
                    }
                }
                //--------------


                if (archivo.Servicio == "MAT")
                {
                    XmlNodeList nl = doc.SelectNodes("recaudacionVehiculos/detallesPagos");
                    XmlNode root = nl[0];
                    var fechasParaConsulta = GetFechas(root.ChildNodes, fechaPagoCabecera, archivo.Servicio);

                    //llamar al SP
                    var res = NegFac.ConsultaReversosRiseCepMat(fechasParaConsulta, cons.Producto);
                    var  objRes = XmlToObject(res.Data);

                    foreach (XmlNode xnode in root.ChildNodes)
                    {
                        var tipo = GetTipoPagoMat(xnode.Attributes["tipoPago"].Value);

                        var nl2 = xnode.SelectNodes("pago");


                        foreach (XmlNode xnode2 in nl2)
                        {
                            var estadoInicialDebito = xnode2.Attributes["estadoDebito"].Value;
                            var placaCpnCamv = xnode2.Attributes["placaCpnCamv"].Value;
                            var fecha = GetFechaSinDelimitadores(xnode2.Attributes["fechaPago"].Value, "MAT");
                            var codLogMon = xnode2.Attributes["codLogMon"].Value;
                            var valorDebitar = float.Parse(xnode2.Attributes["valorDebitar"].Value, CultureInfo.InvariantCulture.NumberFormat);


                            var transaccion = objRes.FirstOrDefault(elem => elem.lg_fecha_trn == fecha && elem.lg_type_trn == tipo.Value &&
                                                elem.codLogMon == codLogMon && elem.lg_referencia == placaCpnCamv);

                            xnode2.Attributes["codTranIFIRev"].Value = "";
                            if (transaccion != null && estadoInicialDebito == "SI")
                            {
                                xnode2.Attributes["estadoDebito"].Value = "NO";

                                ChangeCabecera(node0, valorDebitar, tipo.Key);
                            }
                        }

                    }

                }


                if (archivo.Servicio == "RISE")  //cep
                {
                    XmlNodeList nl = doc.SelectNodes("recaudacionDeuda/detalleRecaudacion");
                    XmlNode root = nl[0];

                    var fechasParaConsulta = GetFechas(root.ChildNodes, fechaPagoCabecera, archivo.Servicio);

                    var res = NegFac.ConsultaReversosRiseCepMat(fechasParaConsulta, cons.Producto);
                    var objRes = XmlToObject(res.Data);

                    foreach (XmlNode xnode in root.ChildNodes)
                    {
                        string estadoInicialDebito = xnode.Attributes["estadoDebito"].Value;
                        string fecha = GetFechaSinDelimitadores(xnode.Attributes["fechaPago"].Value, "RISE");
                        var valorDebitar = float.Parse(xnode.Attributes["valorDebitar"].Value, CultureInfo.InvariantCulture.NumberFormat);
                        var numCompSRI = xnode.Attributes["numCompSRI"].Value;
                        var valorReferencia = "";
                        var tipo = "";

                        var nl2 = xnode.SelectNodes("atributo");
                        //XmlNode root2 = nl2[0];
                        int i = 0;
                        foreach (XmlNode xnode2 in nl2)
                        {
                            i++;
                            if (i == 1)
                            {
                                valorReferencia = xnode2.Attributes["valor"].Value;
                            }
                            if (i == 2)
                            {                                                                       //GLOBAL
                                tipo = xnode2.Attributes["valor"].Value == "A_LA_FECHA" ? "010207" : "010107";

                                var transaccion = objRes.FirstOrDefault(elem => elem.lg_fecha_trn == fecha && elem.lg_type_trn == tipo
                                                                        && elem.lg_referencia == valorReferencia && elem.lg_numero_factura == numCompSRI);
                                

                                if (transaccion != null && estadoInicialDebito == "SI")
                                {
                                    xnode2.Attributes["estadoDebito"].Value = "NO";  ///// pilasssss comentar

                                    ChangeCabecera(node0, valorDebitar, "");
                                }
                            }
                        }
                        i = 0;
                    }
                }

                if (archivo.Servicio == "CEP")  //ofp
                {
                    XmlNodeList nl = doc.SelectNodes("OtrasFormasDePago/declaracionesSRI_IFI");
                    XmlNode root = nl[0];

                    var fechasParaConsulta = GetFechas(root.ChildNodes, fechaPagoCabecera, archivo.Servicio);

                    var res = NegFac.ConsultaReversosRiseCepMat(fechasParaConsulta, cons.Producto);
                    var objRes = XmlToObject(res.Data);

                    foreach (XmlNode xnode in root.ChildNodes)
                    {
                        string fechaSW = xnode.Attributes["fechaPago"].Value.Replace("-", "");
                        string numeroRuc = xnode.Attributes["numeroRuc"].Value;
                        string adhesivo = xnode.Attributes["adhesivo"].Value;
                        string codigoLogMonitor = xnode.Attributes["codigoLogMonitor"].Value;
                        string estadoInicialDebito = xnode.Attributes["estadoDebito"].Value;
                        var valorDebitar = float.Parse(xnode.Attributes["valorDebitar"].Value, CultureInfo.InvariantCulture.NumberFormat);

                        var transacciones = objRes.FirstOrDefault(elem => elem.lg_fecha_trn == fechaSW && elem.lg_type_trn == "010011" && elem.lg_new_cedruc == numeroRuc
                                                                    && elem.lg_referencia == adhesivo && elem.codLogMon == codigoLogMonitor);
                        
                        if (transacciones != null && estadoInicialDebito == "SI")
                        {
                            xnode.Attributes["estadoDebito"].Value = "NO";

                            ChangeCabecera(node0, valorDebitar, "");
                        }
                    }
                }


                doc.Save(rutas.Paths[archivo.Servicio] + @"\" + archivo.NombreArchivo.Split('.')[0] + "_OUT.xml");
                //enviar correo
                //NegFac.EnviarCorreoConciliacion("asunto", "mensaje", "adjunto", "correos separados por coma");
            }


        }

        public KeyValuePair<string, string> GetTipoPagoMat(string tipo) {
            if (tipo == "TRANSF_DOM") return new KeyValuePair<string, string>(tipo, "010310");
            if (tipo == "AJUSTES") return new KeyValuePair<string, string>(tipo, "010110");
            else return new KeyValuePair<string, string>(tipo, "010210");  //"MATRICULA"
        }

        public string GetFechaSinDelimitadores(string fecha, string servicio)
        {
            if (servicio == "MAT" || servicio == "RISE") return fecha.Substring(6, 4) + fecha.Substring(3, 2) + fecha.Substring(0, 2);
            else return fecha.Replace("-", "");
        }

        public void ChangeCabecera(XmlNode root3, float valorDebitar, string tipo)
        {
            foreach (XmlNode xnode3 in root3.ChildNodes)
            {
                if (xnode3.Name == "numeroTransaccionesDirectas" + tipo || xnode3.Name == "numeroDeclaracionesPorTxPago")
                {
                    xnode3.InnerText = (int.Parse(xnode3.InnerText) - 1).ToString();
                }
                if (xnode3.Name == "numeroTransaccionesReversadas" + tipo || xnode3.Name == "numeroDeclaracionesPorTxReverso")
                {
                    xnode3.InnerText = (int.Parse(xnode3.InnerText) + 1).ToString();
                }
                if (xnode3.Name == "montoTransaccionesDirectas" + tipo || xnode3.Name == "montoRecaudadoPorTxPago")
                {
                    var val = float.Parse(xnode3.InnerText, CultureInfo.InvariantCulture.NumberFormat);
                    xnode3.InnerText = (val - valorDebitar).ToString().Replace(',', '.');
                }
                if (xnode3.Name == "montoTransaccionesReversas" + tipo || xnode3.Name == "montoRecaudadoPorTxReverso")
                {
                    var val = float.Parse(xnode3.InnerText, CultureInfo.InvariantCulture.NumberFormat);
                    xnode3.InnerText = (val + valorDebitar).ToString().Replace(',', '.');
                }
            }
        }

        
        public class Table
        {
            public string lg_type_trn { get; set; }
            public string lg_fecha_trn { get; set; }
            public string codLogMon { get; set; }
            public string lg_referencia { get; set; }
            public string lg_numero_factura { get; set; }
            public string lg_new_cedruc { get; set; }
            public string Path { get; set; }
        }

        public List<Table> XmlToObject(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Table>), new XmlRootAttribute("Datos"));

            StringReader stringReader = new StringReader(xml);

            return (List<Table>)serializer.Deserialize(stringReader);
        }

        public string GetFechas(XmlNodeList nl, string fechaCabecera, string servicio)
        {
            List<string> fechas = new List<string>();
            List<string> fechasConFormato = new List<string>();


            foreach (XmlNode xnode in nl)
            {
                var nl2 = servicio=="MAT"? xnode.SelectNodes("pago"): nl;
                foreach (XmlNode xnode2 in nl2)
                {
                    var fechaPago = xnode2.Attributes["fechaPago"].Value;

                    if (fechaCabecera != fechaPago)
                    {
                        var find = fechas.FirstOrDefault(elem => elem == fechaPago);
                        if (find == null)
                        {
                            fechas.Add(fechaPago);
                        }
                    }
                }
                if (servicio != "MAT") break;
            }
            
            fechas.Add(fechaCabecera);

            foreach (var fecha in fechas)
            {
                var nuevaFecha = GetFechaSinDelimitadores(fecha, servicio);
                fechasConFormato.Add(nuevaFecha);
            }

            return string.Join(",", fechasConFormato.ToArray());
        }

        public Cons GetCons(string servicio)
        {
            if (servicio == "MAT") return new Cons { Producto = "0010181010", Cabecera = "recaudacionVehiculos/cabecera" };
            if (servicio == "RISE") return new Cons { Producto = "0010011007", Cabecera = "recaudacionDeuda/cabecera" };
            else return new Cons { Producto = "0010191011", Cabecera = "OtrasFormasDePago/cabecera" };
        }
    }
}