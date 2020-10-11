using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using SRIconciliacion.Utils;
using SRIconciliacion.Views;

//install-package NUnit -Version 3.8.1
//install-package NUnit3TestAdapter -Version 3.8.0

namespace SRIconciliacion.UnitTests
{
    [TestFixture]
    public class SriXmlFileManagerTests
    {
        SriXmlFileManager fileManager;
        string defaultDate;
        Views.Rutas paths;
        [SetUp]
        public void SetUp()
        {
            fileManager = new SriXmlFileManager();
            defaultDate = DateTime.Now.AddDays(1).ToString("yyyyMMdd");

            paths = new Views.Rutas()
            {
                Fecha = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy")
            };
            paths.Paths.Add("CEP", @"C:\SRI\XML\CEP\S" + defaultDate);
            paths.Paths.Add("RISE", @"C:\SRI\XML\RISE\C" + defaultDate);
            paths.Paths.Add("MAT", @"C:\SRI\XML\MAT\M" + defaultDate);
        }
        /*[Test]
        public void CreateFolderOrPaths_CreateFolder_ReturnPaths()
        {
            // Arrange 
            // *ver SetUp function

            // Act
            var result = fileManager.CreateFolderOrPaths(DateTime.Now.AddDays(1).ToString("dd/MM/yyyy"), @"C:\SRI\XML\");
            // Assert

            Assert.AreEqual(paths.ToString(), result.ToString());
        }*/

        /*
        [TestCase("141", "MAT", 1)]  //1, 1
        [TestCase("", "RISE", 4)]  //1, 0
        [TestCase("074", "", 3)]  //0, 1
        [TestCase("", "", 9)]  //0, 0
        public void GetFilesBy_WhenCalled_ReturnFiles(string institucion, string servicio, int expectedResult)
        {
            // Arrange 
            // *ver SetUp function

            // Act
            var result = fileManager.GetFilesBy(paths, institucion, servicio);
            // Assert
            Assert.That(result.Count, Is.EqualTo(expectedResult));
        }
        */

        /*
        [Test]
        public void Concilia_WhenCalled_ReturnListFileOut()
        {
            var tomorrow = Convert.ToDateTime("29/09/2020").AddDays(1).ToString("dd/MM/yyyy");
            var newRuta = fileManager.CreateFolderOrPaths(tomorrow, @"C:\SRI\XML\", false);

            var ArchivosConciliar = fileManager.GetFilesBy(newRuta, "122", "MAT");

            //llamada a la accion
            ConciliacionManager objConciliar = new ConciliacionManager();
            objConciliar.Conciliar(newRuta, ArchivosConciliar, false);

            Assert.AreEqual("", "");
        }  */

        /*[Test]
        public void Concilia_ConciliaRISE_ReturnListFileOut()
        {
            var tomorrow = Convert.ToDateTime("29/09/2020").AddDays(1).ToString("dd/MM/yyyy");
            var newRuta = fileManager.CreateFolderOrPaths(tomorrow, @"C:\SRI\XML\", false);

            var ArchivosConciliar = fileManager.GetFilesBy(newRuta, "074", "RISE");

            //llamada a la accion
            Views.ConciliacionManager objConciliar = new Views.ConciliacionManager();
            objConciliar.Conciliar(newRuta, ArchivosConciliar, false);

            Assert.AreEqual("", "");
        }*/

        /*[Test]
        public void GetFechas_whenCall_ReturnFechas()
        {
            //arrange
            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:/Users/sanz9/Downloads/xml-toTest/conci/toTest.xml");    //mat
            XmlNodeList nl = doc.SelectNodes("recaudacionVehiculos/detallesPagos");    //mat

            //doc.Load(@"file:///C:/SRI/XML/RISE/C20200930/RISE-074-05012015-OFP.XML");    //rise
            //XmlNodeList nl = doc.SelectNodes("recaudacionDeuda/detalleRecaudacion");    //rise
            XmlNode root = nl[0];

            Views.ConciliacionManager c = new Views.ConciliacionManager();

            var start = DateTime.Now;
            // act
            var fechas = c.GetFechas(root.ChildNodes, "29-09-2020", "MAT");  //mat
            //var fechas = c.GetFechas(root.ChildNodes, "05/01/2015", "RISE");  //rise
            
            var end = DateTime.Now;

            var diff = end - start;
            // assert
            StringAssert.AreEqualIgnoringCase("07-01-2015", fechas);
        }*/

        /*[Test]
        public void RIseTest_WhenCall_Return()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"file:///C:/SRI/XML/RISE/C20200930/RISE-074-05012015-OFP.XML");

            XmlNodeList nl = doc.SelectNodes("recaudacionDeuda/detalleRecaudacion");
            XmlNode root = nl[0];
            foreach (XmlNode xnode in root.ChildNodes)
            {
                string estadoInicialDebito = xnode.Attributes["estadoDebito"].Value;
                string fechaPagoc = xnode.Attributes["fechaPago"].Value;
                var nl2 = xnode.SelectNodes("atributo");
                //XmlNode root2 = nl2[0];
                int i = 0;
                foreach (XmlNode xnode2 in nl2) {
                    var uu = xnode2;
                }
            }
        }*/

        [Test]
        public void TestXml()
        {
            //var xml = "<Datos><Table><lg_type_trn>010210</lg_type_trn><lg_fecha_trn>20200929</lg_fecha_trn><codLogMon>12229092020235589712</codLogMon><lg_referencia>PBC5928</lg_referencia></Table><Table><lg_type_trn>010210</lg_type_trn><lg_fecha_trn>20200929</lg_fecha_trn><codLogMon>12229092020235580663</codLogMon><lg_referencia>IS268U</lg_referencia></Table><Table><lg_type_trn>010210</lg_type_trn><lg_fecha_trn>20200929</lg_fecha_trn><codLogMon>12229092020235587665</codLogMon><lg_referencia>PVP0938</lg_referencia></Table></Datos>";

            //var xml = "<Datos/>";

            var xml = "<Datos><Table><lg_type_trn>010210</lg_type_trn></Table></Datos>";

            XmlSerializer serializer = new XmlSerializer(typeof(List<Table>), new XmlRootAttribute("Datos"));

            StringReader stringReader = new StringReader(xml);

            List<Table> productList = (List<Table>)serializer.Deserialize(stringReader);

            var single = productList[0].lg_type_trn;

            var res = productList.FirstOrDefault(elem => elem.lg_fecha_trn == "20200929");

            var t = productList;
        }
    }

    public class Table
    {
        public string lg_type_trn { get; set; }
        public string lg_fecha_trn { get; set; }
        public string codLogMon { get; set; }
        public string lg_referencia { get; set; }
    }
}
