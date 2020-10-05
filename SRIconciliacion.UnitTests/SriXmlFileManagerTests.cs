using System;
using NUnit.Framework;
using SRIconciliacion.Utils;

//install-package NUnit -Version 3.8.1
//install-package NUnit3TestAdapter -Version 3.8.0

namespace SRIconciliacion.UnitTests
{
    [TestFixture]
    public class SriXmlFileManagerTests
    {
        SriXmlFileManager fileManager;
        string defaultDate;
        Rutas paths;
        [SetUp]
        public void SetUp()
        {
            fileManager = new SriXmlFileManager();
            defaultDate = DateTime.Now.AddDays(1).ToString("yyyyMMdd");

            paths = new Rutas()
            {
                Fecha = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy")
            };
            paths.Paths.Add("CEP", @"C:\SRI\XML\CEP\S" + defaultDate);
            paths.Paths.Add("RISE", @"C:\SRI\XML\RISE\C" + defaultDate);
            paths.Paths.Add("MAT", @"C:\SRI\XML\MAT\M" + defaultDate);
        }
        [Test]
        public void CreateFolderOrPaths_CreateFolder_ReturnPaths()
        {
            // Arrange 
            // *ver SetUp function

            // Act
            var result = fileManager.CreateFolderOrPaths(DateTime.Now.AddDays(1).ToString("dd/MM/yyyy"), @"C:\SRI\XML\");
            // Assert

            Assert.AreEqual(paths.ToString(), result.ToString());
        }

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
    }
}
