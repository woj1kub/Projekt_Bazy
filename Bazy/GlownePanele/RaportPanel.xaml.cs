using Microsoft.Win32;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Colors = MigraDoc.DocumentObjectModel.Colors;
using Paragraph = MigraDoc.DocumentObjectModel.Paragraph;
using Section = MigraDoc.DocumentObjectModel.Section;
using Style = MigraDoc.DocumentObjectModel.Style;
using MigraDoc.DocumentObjectModel.Tables;

namespace Bazy.GlownePanele
{
    /// <summary>
    /// Logika interakcji dla klasy RaportPanel.xaml
    /// </summary>
    public partial class RaportPanel : UserControl
    {

        ObservableCollection<Portfel> portfele_dane = new();
        private string? ActiveUser;

        public RaportPanel(ObservableCollection<Portfel> portfele, string activeuser)
        {
            portfele_dane = portfele;
            ActiveUser = activeuser;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                // Create a MigraDoc document
                Document document = CreateDocument();
                string ddl = MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToString(document);
                MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, "MigraDoc.mdddl");

                PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
                renderer.Document = document;
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                renderer.RenderDocument();


                // Save the document...
                renderer.PdfDocument.Save(saveFileDialog.FileName);
            }
        }

        Document CreateDocument()
        {
            // Create a new MigraDoc document
            Document document = new Document();
            document.Info.Title = $"Raport dla {ActiveUser}";
            document.Info.Subject = "Raport inwestycji personalnych wygenerowany na żądanie użytkownika";
            document.Info.Author = "Inwestycje personalne";

            DefineStyles(document);

            DefineCover(document);
            WyswietlLokaty(document);
            document.LastSection.AddPageBreak();
            WyswietlKontaOszczenosiowe(document);

            return document;
        }

        static void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Arial";

            // Heading1 to Heading9 are predefined styles with an outline level. An outline level
            // other than OutlineLevel.BodyText automatically creates the outline (or bookmarks) 
            // in PDF.

            style = document.Styles["Heading1"];
            style.Font.Name = "Tahoma";
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.Font.Color = Colors.DarkBlue;
            style.ParagraphFormat.PageBreakBefore = true;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles["Heading2"];
            style.Font.Size = 12;
            style.Font.Bold = true;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles["Heading3"];
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 3;

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called TextBox based on style Normal
            style = document.Styles.AddStyle("TextBox", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            style.ParagraphFormat.Borders.Width = 2.5;
            style.ParagraphFormat.Borders.Distance = "3pt";
            style.ParagraphFormat.Shading.Color = Colors.SkyBlue;

            // Create a new style called TOC based on style Normal
            style = document.Styles.AddStyle("TOC", "Normal");
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right, TabLeader.Dots);
            style.ParagraphFormat.Font.Color = Colors.Blue;
        }

        void DefineCover(Document document)
        {
            Section section = document.AddSection();

            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.SpaceAfter = "3cm";

            paragraph = section.AddParagraph("Raport inwestycji personalnych");
            paragraph.Format.Font.Size = 24;
            paragraph.Format.Font.Color = Colors.Black;
            paragraph.Format.SpaceBefore = "8cm";
            paragraph.Format.SpaceAfter = "3cm";

            paragraph = section.AddParagraph($"Użytkownik: {ActiveUser}");
            paragraph = section.AddParagraph("Data: ");
            paragraph.AddDateField();
            section.AddPageBreak();
        }

        void WyswietlLokaty(Document document)
        {
            Paragraph paragraph = document.LastSection.AddParagraph("Informacje na temat lokat", "Heading2");
            foreach (Portfel portfel in portfele_dane)
            {
                // pomiń jeżeli portfel nie zawiera żadnej lokaty
                if (portfel.Lokaties.Count == 0)
                    continue;
                document.LastSection.AddParagraph($"Portfel: {portfel.Nazwa}", "Heading3");
                MigraDoc.DocumentObjectModel.Tables.Table table = new MigraDoc.DocumentObjectModel.Tables.Table();
                table.Borders.Width = 0.75;

                Column column = table.AddColumn();
                column.Format.Alignment = ParagraphAlignment.Center;

                table.AddColumn();
                column = table.AddColumn();
                column = table.AddColumn();


                Row row = table.AddRow();
                row.Shading.Color = Colors.PaleGoldenrod;
                Cell cell = row.Cells[0];
                cell.AddParagraph("Nazwa");
                cell = row.Cells[1];
                cell.AddParagraph("Kwota");
                cell = row.Cells[2];
                cell.AddParagraph("Data założenia");
                cell = row.Cells[3];
                cell.AddParagraph("Data zakończenia");
                foreach (Lokaty lokata in portfel.Lokaties)
                {
                    row = table.AddRow();
                    cell = row.Cells[0];
                    cell.AddParagraph(lokata.Nazwa);
                    cell = row.Cells[1];
                    cell.AddParagraph(lokata.Kwota.ToString());
                    cell = row.Cells[2];
                    cell.AddParagraph(DateOnly.FromDateTime(lokata.Data_zakupu).ToString());
                    cell = row.Cells[3];
                    cell.AddParagraph(DateOnly.FromDateTime(lokata.Data_zakończenia).ToString());
                }
                document.LastSection.Add(table);
            }
        }

        void WyswietlKontaOszczenosiowe(Document document)
        {
            Paragraph paragraph = document.LastSection.AddParagraph("Informacje na temat kont oszczędnośiowych", "Heading2");
            foreach (Portfel portfel in portfele_dane)
            {
                // pomiń jeżeli portfel nie zawiera żadnej lokaty
                if (portfel.KontoOszczędnościowes.Count == 0)
                    continue;
                document.LastSection.AddParagraph($"Portfel: {portfel.Nazwa}", "Heading3");
                MigraDoc.DocumentObjectModel.Tables.Table table = new MigraDoc.DocumentObjectModel.Tables.Table();
                table.Borders.Width = 0.75;

                Column column = table.AddColumn();
                column.Format.Alignment = ParagraphAlignment.Center;

                table.AddColumn();
                column = table.AddColumn();
                column = table.AddColumn();


                Row row = table.AddRow();
                row.Shading.Color = Colors.PaleGoldenrod;
                Cell cell = row.Cells[0];
                cell.AddParagraph("Nazwa");
                cell = row.Cells[1];
                cell.AddParagraph("Kwota");
                cell = row.Cells[2];
                cell.AddParagraph("Oprocentowanie");
                cell = row.Cells[3];
                cell.AddParagraph("Data założenia");
                foreach (KontoOszczędnościowe konto in portfel.KontoOszczędnościowes)
                {
                    row = table.AddRow();
                    cell = row.Cells[0];
                    cell.AddParagraph(konto.Nazwa);
                    cell = row.Cells[1];
                    cell.AddParagraph(konto.Kwota.ToString());
                    cell = row.Cells[2];
                    cell.AddParagraph(konto.Oprecentowanie.ToString());
                    cell = row.Cells[3];
                    cell.AddParagraph(DateOnly.FromDateTime(konto.Data_Założenia).ToString());
                }
                document.LastSection.Add(table);
            }
        }
    }
}
