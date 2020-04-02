using Dev2Be.Toolkit;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Office.Interop.Word;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using Word = Microsoft.Office.Interop.Word;

namespace SDLP
{
    public class OpenDocuments
    {
        [STAThread]
        public static MemoryStream GetTextFromWord(string fileName)
        {
            int processId = 0;

            try
            {
                foreach (Process item in Process.GetProcessesByName("winword"))
                    processId -= item.Id;

                Word.Application application = new Word.Application();

                foreach (Process item in Process.GetProcessesByName("winword"))
                    processId += item.Id;

                object path = fileName;
                object typeMissing = Type.Missing;

                Documents documents = application.Documents;

                _Document document = documents.Open(ref path, ref typeMissing, ref typeMissing, ref typeMissing, ref typeMissing, ref typeMissing, ref typeMissing, ref typeMissing, ref typeMissing, ref typeMissing, ref typeMissing, ref typeMissing, ref typeMissing, ref typeMissing, ref typeMissing, ref typeMissing);

                document.ActiveWindow.Selection.WholeStory();
                document.ActiveWindow.Selection.Copy();

                IDataObject data = Clipboard.GetDataObject();

                return new MemoryStream(Encoding.Default.GetBytes(data.GetData(DataFormats.Rtf).ToString()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, new AssemblyInformations(Assembly.GetExecutingAssembly().GetName().Name).Product, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);

                return default(MemoryStream);
            }
            finally
            {
                Process.GetProcessById(processId).Kill();
            }
        }

        public static string GetTextFromPdf(string fileName)
        {
            StringBuilder text = new StringBuilder();
            using (PdfReader reader = new PdfReader(fileName))
                for (int i = 1; i <= reader.NumberOfPages; i++)
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));

            return text.ToString();
        }
    }
}
