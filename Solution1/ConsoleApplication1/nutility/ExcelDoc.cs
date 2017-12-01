namespace nutility
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Collections;
    using System.Collections.Generic;

    public class ExcelDoc : IDisposable
    {
        private XmlTextWriter writer;
        private List<ExcelPage> pages;

        private const string styles =
@"
  <Style ss:ID='Default' ss:Name='Normal'>
   <Alignment ss:Vertical='Center'/>
   <Borders/>
   <Font ss:FontName='Calibri' x:Family='Swiss' ss:Size='11' ss:Color='#000000'/>
   <Interior/>
   <NumberFormat/>
   <Protection/>
  </Style>
  <Style ss:ID='s21'>
   <Font ss:FontName='Calibri' x:Family='Swiss' ss:Size='11' ss:Color='#000000' ss:Bold='1'/>
   <Alignment ss:Horizontal='Center' ss:Vertical='Center' ss:WrapText='1'/>
  </Style>
  <Style ss:ID='s23'>
   <Font ss:Size='11'/>
  </Style>
  <Style ss:ID='s24'>
   <Alignment ss:Vertical='Bottom' ss:Rotate='90'/>
   <Font ss:Size='11'/>
  </Style>

  <Style ss:ID='s25'>
   <Font ss:FontName='Calibri' x:Family='Swiss' ss:Size='11'/>
   <Alignment ss:Horizontal='Left' ss:Vertical='Center'/>
  </Style>
  <Style ss:ID='s26'>
   <Font ss:FontName='Calibri' x:Family='Swiss' ss:Size='11'/>
   <Alignment ss:Horizontal='Center' ss:Vertical='Center'/>
  </Style>
  <Style ss:ID='s27'>
   <Font ss:FontName='Calibri' x:Family='Swiss' ss:Size='11'/>
   <Alignment ss:Horizontal='Right' ss:Vertical='Center'/>
  </Style>

  <Style ss:ID='s35'>
   <Font ss:FontName='Calibri' x:Family='Swiss' ss:Size='11'/>
   <Alignment ss:Horizontal='Left' ss:Vertical='Center'/>
   <NumberFormat ss:Format='Short Date'/>
  </Style>
  <Style ss:ID='s36'>
   <Font ss:FontName='Calibri' x:Family='Swiss' ss:Size='11'/>
   <Alignment ss:Horizontal='Center' ss:Vertical='Center'/>
   <NumberFormat ss:Format='Short Date'/>
  </Style>
  <Style ss:ID='s37'>
   <Font ss:FontName='Calibri' x:Family='Swiss' ss:Size='11'/>
   <Alignment ss:Horizontal='Right' ss:Vertical='Center'/>
   <NumberFormat ss:Format='Short Date'/>
  </Style>

  <Style ss:ID='s45'>
   <Font ss:FontName='Calibri' x:Family='Swiss' ss:Size='11'/>
   <Alignment ss:Horizontal='Left' ss:Vertical='Center'/>
   <NumberFormat ss:Format='hh:mm:ss;@'/>
  </Style>
  <Style ss:ID='s46'>
   <Font ss:FontName='Calibri' x:Family='Swiss' ss:Size='11'/>
   <Alignment ss:Horizontal='Center' ss:Vertical='Center'/>
   <NumberFormat ss:Format='hh:mm:ss;@'/>
  </Style>
  <Style ss:ID='s47'>
   <Font ss:FontName='Calibri' x:Family='Swiss' ss:Size='11'/>
   <Alignment ss:Horizontal='Right' ss:Vertical='Center'/>
   <NumberFormat ss:Format='hh:mm:ss;@'/>
  </Style>
";
        public ExcelDoc(TextWriter w)
        {
            writer = new XmlTextWriter(w);
            pages = new List<ExcelPage>();
        }

        public void AddPage(ExcelPage page)
        {
            pages.Add(page);
        }

        public void Persist()
        {
            writer.WriteProcessingInstruction("xml", "version='1.0'");
            writer.WriteProcessingInstruction("mso-application", "progid='Excel.Sheet'");

            writer.WriteStartElement("Workbook");
            writer.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:spreadsheet");
            writer.WriteAttributeString("xmlns", "o", null, "urn:schemas-microsoft-com:office:office");
            writer.WriteAttributeString("xmlns", "x", null, "urn:schemas-microsoft-com:office:excel");
            writer.WriteAttributeString("xmlns", "ss", null, "urn:schemas-microsoft-com:office:spreadsheet");
            writer.WriteAttributeString("xmlns", "html", null, "http://www.w3.org/TR/REC-html40");
            writer.WriteStartElement("DocumentProperties");
            writer.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:office");
            writer.WriteElementString("LastAuthor", "commandline_utility tool");
            writer.WriteElementString("Created", DateTime.Now.ToString("s"));
            writer.WriteElementString("LastSaved", DateTime.Now.ToString("s"));
            writer.WriteElementString("Version", "11.6360");
            writer.WriteEndElement();

            writer.WriteStartElement("ExcelWorkbook");
            writer.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:excel");
            writer.WriteElementString("WindowHeight", "9345");
            writer.WriteElementString("WindowWidth", "15180");
            writer.WriteElementString("WindowTopX", "120");
            writer.WriteElementString("WindowTopY", "60");
            writer.WriteElementString("ProtectStructure", "False");
            writer.WriteElementString("ProtectWindows", "False");
            writer.WriteEndElement();

            writer.WriteStartElement("Styles");
            writer.WriteRaw(styles);
            writer.WriteEndElement();
            foreach (ExcelPage p in pages)
                p.Persist(writer);
            writer.WriteEndElement();
            writer.Flush();
        }

        #region IDisposable Members
        public void Dispose()
        {
            writer.BaseStream.Flush();
            IDisposable d = writer.BaseStream as IDisposable;
            d.Dispose();
        }
        #endregion
    }
}