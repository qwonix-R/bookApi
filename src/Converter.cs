using Pandoc;
using System;
using System.IO;
using System.Threading.Tasks;
namespace bookApi
{
    static class PdfConverter
    {
        static private PandocEngine pandoc = new PandocEngine(@"/usr/bin/pandoc");

        public static async Task ConvertToPdf(Stream inputStream, Stream outputStream, string format)
        {

            if (inputStream == null || !inputStream.CanRead)
            {
                Console.WriteLine("Входной поток недоступен или пуст.");
                return;
            }

            
            var outformat = new PdfOut
            {
                Engine = PdfEngine.XeLatex
            };
            var options = new Options
            {
                CustomArguments = new[]
            {
                "-V", "mainfont=DejaVuSans", // Шрифт с поддержкой кириллицы
                "-V", "babel-lang=russian",  // Поддержка русского языка
                "-V", "inputenc=utf8",       // Кодировка UTF-8
                "-V", "header-includes=\\usepackage{ragged2e}",
                "--pdf-engine=xelatex"
                
            }
            };
            try
            {
                //await pandoc.Convert(inputStream, outputStream, outformat, null);
                switch (format.ToLower())
                {
                    case ".docx":
                        await pandoc.Convert(inputStream, outputStream, new DocxIn(), new PdfOut(), options);
                        break;
                    case ".txt":
                        await pandoc.Convert(inputStream, outputStream, new PandocMdIn(), new PdfOut(), options);
                        break;
                    case ".html":
                        await pandoc.Convert(inputStream, outputStream, new HtmlIn(), new PdfOut(), options);
                        break;
                    case ".epub":
                        await pandoc.Convert(inputStream, outputStream, new EpubIn(), new PdfOut(), options);
                        break;
                    case ".rtf":
                        await pandoc.Convert(inputStream, outputStream, new RtfIn(), new PdfOut(), options);
                        break;
                    case ".xml":
                        await pandoc.Convert(inputStream, outputStream, new DocBookIn(), new PdfOut(), options);
                        break;
                    default:
                        throw new ArgumentException($"Unsupported format: {format}");
                }
                Console.WriteLine("Конвертация в PDF успешно завершена.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при конвертации: {ex.Message}");
                return;
            }

        }
    }
}
