using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;

namespace PDFSplit
{
    class Program
    {

        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("输入源文件路径");
                var resource = Console.ReadLine();
                if (!File.Exists(resource))
                {
                    Console.WriteLine("文件不存在");
                }
                var fileinfo = new FileInfo(resource);
                string fileNameWithoutExt = fileinfo.Name.Replace(fileinfo.Extension, string.Empty);
                var dest = Path.Combine(fileinfo.Directory?.FullName, $"{fileNameWithoutExt}{{0}}{fileinfo.Extension}");
                ManipulatePdf(dest, resource ?? "");
            }
            finally
            {
                Console.ReadLine();            }
        }

        protected static void ManipulatePdf(string dest,string resource)
{
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(resource));

            IList<PdfDocument> splitDocuments = new CustomPdfSplitter(pdfDoc, dest).SplitByPageCount(1);

            foreach (PdfDocument doc in splitDocuments)
            {
                doc.Close();
            }

            pdfDoc.Close();
        }

        private class CustomPdfSplitter : PdfSplitter
        {
            private string dest;
            private int partNumber = 1;

            public CustomPdfSplitter(PdfDocument pdfDocument, string dest) : base(pdfDocument)
            {
                this.dest = dest;
            }

            protected override PdfWriter GetNextPdfWriter(PageRange documentPageRange)
            {
                return new PdfWriter(string.Format(dest, partNumber++));
            }
        }
    }
}
