using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace TestCoreConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Document document = new Document();

            Section section = document.AddSection();

            section.AddParagraph("Hello, World!");

            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.Font.Color = Color.FromCmyk(100, 30, 20, 50);
            paragraph.AddFormattedText("Hello, World!", TextFormat.Underline);

            FormattedText ft = paragraph.AddFormattedText("Small text", TextFormat.Bold);
            ft.Font.Size = 6;

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(false);
            pdfRenderer.Document = document;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            pdfRenderer.RenderDocument();

            string filename = "HelloWorld.pdf"; //otherwise throws an exception
            pdfRenderer.PdfDocument.Save(filename);
        }
    }
}
