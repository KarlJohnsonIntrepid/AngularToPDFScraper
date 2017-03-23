using NReco.PhantomJS;
using System;
using System.Collections.Generic;
using System.Xml;

namespace AngularToPDFSiteScraper
{
    /// <summary>
    /// Uses NReco phantom JS ( .net wrapper for phantom),  phantom rasterize scripts are in the scripts folder if you need to customise the pdf output
    /// Pdfs are saved to the bin folder.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Generating PDF(s) to bin folder....");

            var convertor = new PdfConverter();

            //convert single page to XML
            convertor.GeneratePdf("scripts\\rasterizeCustom.js", "http://intrepidnomads.com/about/", "About.pdf", "A4");

            PrintAllFromXml(convertor);

        }

        /// <summary>
        /// Example loading from XML (just an example, if you are using this you will have to write your own method to work with the XML doc)
        /// </summary>
        /// <param name="convertor"></param>
        private static void PrintAllFromXml(PdfConverter convertor)
        {
            var doc = new XmlDocument();
            doc.Load("C:\\Users\\KarlJ\\Documents\\visual studio 2017\\Projects\\AngularToPDFSiteScraper\\AngularToPDFSiteScraper\\blogs.xml");

            XmlElement root = doc.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("row");

            var blogs = new List<Blog>(0);
            var baseUrl = "http://intrepidnomads.com/blog/";


            //Get all the links from the XML
            var count = 1;
            foreach (XmlElement el in nodes)
            {
                var blog = new Blog
                {
                    Url = baseUrl + el["TitleURL"].InnerText + "/",
                    Name = count + "." + el["Title"].InnerText
                };
                blogs.Add(blog);
                count++;
            }

            foreach (var blog in blogs)
            {
                convertor.GeneratePdf("scripts\\rasterize.js", blog.Url, blog.Name + ".pdf", "A4");
            }
        }
    }

    /// <summary>
    /// Converts a url to PDF, PDFs are saved in the BIN folder
    /// </summary>
    public class PdfConverter {

        private PhantomJS phantomJs;
        private int Count;

        public PdfConverter()
        {
            phantomJs = new PhantomJS();

            phantomJs.OutputReceived += (sender, e) => {
                Count++;
                Console.WriteLine("PDF " + Count + " generated");
                Console.Write("Generating next.....");
            };
        }

        public void GeneratePdf(string rasterizeScript, string url, string pdfName, string pageSize)
        {
            phantomJs.Run("scripts\\rasterizeCustom.js", new[] { url, pdfName, pageSize });
        }
    }

    public class Blog
    {
        public string Url { get; set; }
        public string Name { get; set; }
    }
}
