using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace Infrastructure.Service
{
    public static class PdfExtractor
    {
        /// <summary>
        /// Extraherar all text från en PDF-fil
        /// </summary>
        public static string ExtractTextFromPdf(byte[] pdfBytes)
        {
            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                throw new ArgumentException("PDF-bytes kan inte vara tom");
            }

            try
            {
                var text = new System.Text.StringBuilder();
                
                using (var memoryStream = new System.IO.MemoryStream(pdfBytes))
                using (var reader = new PdfReader(memoryStream))
                using (var document = new PdfDocument(reader))
                {
                    int pageCount = document.GetNumberOfPages();
                    Console.WriteLine($"📄 PDF har {pageCount} sidor");

                    for (int i = 1; i <= pageCount; i++)
                    {
                        try
                        {
                            var page = document.GetPage(i);
                            var strategy = new SimpleTextExtractionStrategy();
                            var pageText = PdfTextExtractor.GetTextFromPage(page, strategy);
                            text.AppendLine(pageText);
                            Console.WriteLine($"✅ Extraherade sida {i}");
                        }
                        catch (Exception pageEx)
                        {
                            Console.WriteLine($"⚠️ Fel på sida {i}: {pageEx.Message}");
                            // Fortsätt med nästa sida
                        }
                    }
                }
                
                var result = text.ToString();
                Console.WriteLine($"📊 Total text extraherad: {result.Length} tecken");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ PDF Extraction Fatal Error: {ex.GetType().Name}: {ex.Message}");
                throw new InvalidOperationException($"Kunde inte läsa PDF: {ex.Message}", ex);
            }
        }
    }
}

