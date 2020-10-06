using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Xml;

namespace SRIconciliacion.Utils
{
    public class ConciliacionManager
    {
        public void Conciliar(Rutas rutas, List<FileDetail> archivosConciliar, bool envioCorreo)
        {
            foreach (var archivo in archivosConciliar)
            {
                var producto = "";
                var cabecera = "";
                if (archivo.Servicio == "MAT")
                {
                    producto = "0010181010";
                    cabecera = "recaudacionVehiculos/cabecera";
                }
                if (archivo.Servicio == "RISE")
                {
                    producto = "0010011007";
                    cabecera = "recaudacionDeuda/cabecera";
                }
                if (archivo.Servicio == "CEP")
                    producto = "0010191011";
                //var key = archivo.Servicio.Substring(0, 3);
                //var serv = archivo.Servicio == "MATRICULACIÓN" ? archivo.Servicio.Substring(0, 3) : archivo.Servicio;
                var pathFile = rutas.Paths[archivo.Servicio] + @"\" + archivo.NombreArchivo;

                XmlDocument doc = new XmlDocument();
                doc.Load(pathFile);


                XmlNodeList xxx = doc.SelectNodes(cabecera);
                XmlNode yyy = xxx[0];

                var fechaPago = "";
                foreach (XmlNode xnode3 in yyy.ChildNodes)
                {
                    if (xnode3.Name == "fechaRecaudacion")
                    {
                        if (archivo.Servicio == "MAT")
                        {
                            var splitedFechaPago = xnode3.InnerText.Split('-');
                            fechaPago = splitedFechaPago[2] + splitedFechaPago[1] + splitedFechaPago[0];
                            break;
                        }

                        if (archivo.Servicio == "RISE")
                        {
                            var splitedFechaPago = xnode3.InnerText.Split('/');
                            fechaPago = splitedFechaPago[2] + splitedFechaPago[1] + splitedFechaPago[0];
                            break;
                        }
                    }
                }

                //--------------


                if (archivo.Servicio == "MAT")
                {
                    XmlNodeList nl = doc.SelectNodes("recaudacionVehiculos/detallesPagos");
                    XmlNode root = nl[0];

                    foreach (XmlNode xnode in root.ChildNodes)
                    {
                        string tipo = xnode.Attributes["tipoPago"].Value;

                        if (tipo == "TRANSF_DOM")
                        {
                            tipo = "010310";
                        }
                        if (tipo == "AJUSTES")
                        {
                            tipo = "010110";
                        }
                        if (tipo == "MATRICULA")
                        {
                            tipo = "010210";
                        }

                        var nl2 = xnode.SelectNodes("pago");
                        
                        foreach (XmlNode xnode2 in nl2)
                        {
                            var estadoInicialDebito = xnode2.Attributes["estadoDebito"].Value; 
                            var estadoDebito = "";
                            
                            var placaCpnCamv = xnode2.Attributes["placaCpnCamv"].Value;
                            var codLogMon = xnode2.Attributes["codLogMon"].Value;
                            var valorDebitar = float.Parse(xnode2.Attributes["valorDebitar"].Value, CultureInfo.InvariantCulture.NumberFormat);

                            ConciliacionService cs = new ConciliacionService();
                            var res = cs.Sp_consulta_conciliacion_mat_getAdquirente(fechaPago, tipo, codLogMon, producto);

                            //return  si($ok), lg_sec_adquirente($secuencial)

                            if (res.OK > 1 || res.OK == 0)
                            {
                                estadoDebito = "NO";
                            }

                            if (res.OK == 1)  ///si
                            {
                                ConciliacionService cs2 = new ConciliacionService();
                                var exrev = cs2.Sp_consult_conciliacion_mat_getExrev("", tipo, res.Secuencial);

                                // return exrev

                                if (exrev == 1)
                                {
                                    estadoDebito = "NO";
                                }

                            }

                            xnode2.Attributes["codTranIFIRev"].Value = "";
                            if (estadoDebito == "NO" && estadoInicialDebito == "SI")
                            {
                                xnode2.Attributes["estadoDebito"].Value = estadoDebito;

                                XmlNodeList nl3 = doc.SelectNodes("recaudacionVehiculos/cabecera");
                                XmlNode root3 = nl3[0];

                                if (tipo == "010210")  //MATRICULA
                                {
                                    ChangeCabecera(root3, valorDebitar, "MATRICULA");
                                }

                                if (tipo == "010310")  //TRANSF_DOM
                                {
                                    ChangeCabecera(root3, valorDebitar, "TRANSF_DOM");
                                }

                                if (tipo == "010110")  //AJUSTES
                                {
                                    ChangeCabecera(root3, valorDebitar, "JUSTES");
                                }

                            }
                        }

                    }

                }


                if (archivo.Servicio == "RISE")
                {
                    XmlNodeList nl = doc.SelectNodes("recaudacionDeuda/detalleRecaudacion");
                    XmlNode root = nl[0];
                    
                    foreach (XmlNode xnode in root.ChildNodes)
                    {
                        string estadoInicialDebito = xnode.Attributes["estadoDebito"].Value;
                        var valorDebitar = float.Parse(xnode.Attributes["valorDebitar"].Value, CultureInfo.InvariantCulture.NumberFormat);
                        var numCompSRI = xnode.Attributes["numCompSRI"].Value;
                        var estadoDebito = "";
                        var valorReferencia = "";
                        var tipo = "";

                        var nl2 = xnode.SelectNodes("atributo");
                         //XmlNode root2 = nl2[0];
                        int i = 0;
                        foreach (XmlNode xnode2 in nl2)
                        {
                            //var estadoInicialDebito = xnode2.Attributes["estadoDebito"].Value;
                            //var estadoDebito = "";
                            i++;
                            if (i == 1)
                            {
                                valorReferencia = xnode2.Attributes["valor"].Value;
                            }
                            if (i == 2)
                            {
                                if (xnode2.Attributes["valor"].Value == "A_LA_FECHA")
                                {
                                    tipo = "010207";
                                }
                                if (xnode2.Attributes["valor"].Value == "GLOBAL")
                                {
                                    tipo = "010107";
                                }

                                ConciliacionService ffff = new ConciliacionService();
                                var res = ffff.Sp_consulta_conciliacion_rise_getAdquirente(fechaPago, tipo, valorReferencia, numCompSRI, producto);

                                // return  OK (si), $secuencial(lg_sec_adquirente) 
                                
                                if (res.OK > 1 || res.OK == 0)
                                {
                                    estadoDebito = "NO";
                                }

                                if (res.OK == 1)
                                {
                                    ConciliacionService ttt = new ConciliacionService();
                                    var exrev = ttt.Sp_consulta_conciliacion_rise_getExrev(fechaPago, valorReferencia, res.Secuencial);
                                    // return exrev

                                    if (exrev == 1)
                                    {
                                        estadoDebito = "NO";
                                    }

                                }

                                if (estadoDebito == "NO" && estadoInicialDebito == "SI")
                                {

                                    xnode2.Attributes["estadoDebito"].Value = estadoDebito;  ///// pilasssss comentar

                                    XmlNodeList nl3 = doc.SelectNodes("recaudacionVehiculos/cabecera");
                                    XmlNode root3 = nl3[0];

                                    ChangeCabecera(root3, valorDebitar, "");

                                }
                            }
                        }
                        i = 0;
                    }
                }

                
                doc.Save(rutas.Paths[archivo.Servicio] + @"\" + archivo.NombreArchivo.Split('.')[0] + "_OUT.xml");
            }

            
        }

        public void ChangeCabecera(XmlNode root3, float valorDebitar, string tipo)
        {
            foreach (XmlNode xnode3 in root3.ChildNodes)
            {
                if (xnode3.Name == "numeroTransaccionesDirectas" + tipo)
                {
                    xnode3.InnerText = (int.Parse(xnode3.InnerText) - 1).ToString();
                }
                if (xnode3.Name == "numeroTransaccionesReversadas" + tipo)
                {
                    xnode3.InnerText = (int.Parse(xnode3.InnerText) + 1).ToString();
                }
                if (xnode3.Name == "montoTransaccionesDirectas" + tipo)
                {
                    var val = float.Parse(xnode3.InnerText, CultureInfo.InvariantCulture.NumberFormat);
                    xnode3.InnerText = (val - valorDebitar).ToString().Replace(',', '.');
                }
                if (xnode3.Name == "montoTransaccionesReversas" + tipo)
                {
                    var val = float.Parse(xnode3.InnerText, CultureInfo.InvariantCulture.NumberFormat);
                    xnode3.InnerText = (val + valorDebitar).ToString().Replace(',', '.');
                }
            }
        }
    }
}