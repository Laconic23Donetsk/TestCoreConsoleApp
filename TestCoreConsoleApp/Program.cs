using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;

namespace TestCoreConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var fs = new System.IO.FileStream(@"C:\Users\AndUser\Personal\tmp.pdf", System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                byte[] byteArray = CreatePdf();
                fs.Write(byteArray, 0, byteArray.Length);
            }
        }

        public static byte[] CreatePdf()
        {
            var stream = new System.IO.MemoryStream();
            var writer = new iText.Kernel.Pdf.PdfWriter(stream);
            var pdf = new iText.Kernel.Pdf.PdfDocument(writer);
            var document = new iText.Layout.Document(pdf);

            document.Add(new iText.Layout.Element.Paragraph("Shit, man!"));
            document.Close();

            return stream.ToArray();
        }
    }
}
