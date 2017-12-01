namespace nutility
{
    using System;
    using System.Xml;
    using System.Collections;
    using System.Collections.Generic;

    public class ExcelPage
    {
        public enum ColumnDataType { String, Number, Date, Time }
        public enum Alignment { Left, Center, Right }

        private const string props =
@"<WorksheetOptions xmlns='urn:schemas-microsoft-com:office:excel'>
  <Print>
  <ValidPrinterInfo/>
  <HorizontalResolution>600</HorizontalResolution>
  <VerticalResolution>600</VerticalResolution>
  </Print>
  <Selected/>
  <Panes>
  <Pane>
    <Number>3</Number>
    <ActiveRow>16</ActiveRow>
  </Pane>
  </Panes>
  <ProtectObjects>False</ProtectObjects>
  <ProtectScenarios>False</ProtectScenarios>
</WorksheetOptions>";

        private static Dictionary<ColumnDataType, Dictionary<Alignment, string>> styles;
        static ExcelPage()
        {
            styles = new Dictionary<ColumnDataType, Dictionary<Alignment, string>>()
            {
                {ColumnDataType.Number, new Dictionary<Alignment, string>{{Alignment.Left,"s25"},{Alignment.Center,"s26"},{Alignment.Right,"s27"}}},
                {ColumnDataType.String, new Dictionary<Alignment, string>{{Alignment.Left,"s25"},{Alignment.Center,"s26"},{Alignment.Right,"s27"}}},
                {ColumnDataType.Date, new Dictionary<Alignment, string>{{Alignment.Left,"s35"},{Alignment.Center,"s36"},{Alignment.Right,"s37"}}},
                {ColumnDataType.Time, new Dictionary<Alignment, string>{{Alignment.Left,"s45"},{Alignment.Center,"s46"},{Alignment.Right,"s47"}}}
            };
        }

        private string name;
        private List<PageColumn> columns;
        private List<PageRow> rows;
        private int ExpandedColumnCount;
        private int ExpandedRowCount
        {
            get { return rows.Count == 0 ? 1 : rows.Count + 2; }
        }

        public ExcelPage(string n)
        {
            name = n;
            columns = new List<PageColumn>();
            rows = new List<PageRow>();
            ExpandedColumnCount = 1;
        }

        public void AddColumn(string name, ColumnDataType datatype, ExcelPage.Alignment align)
        {
            columns.Add(new PageColumn(name, datatype, align));
        }

        public void AddColumn(string name)
        {
            columns.Add(new PageColumn(name, ExcelPage.ColumnDataType.Number, ExcelPage.Alignment.Center));
        }

        public void AddRow(params object[] values)
        {
            PageRow row = new PageRow(columns);
            row.SetValues(values);
            rows.Add(row);
            ExpandedColumnCount = Math.Max(ExpandedColumnCount, values.Length);
        }

        internal void Persist(XmlTextWriter writer)
        {
            writer.WriteStartElement("Worksheet");
            writer.WriteAttributeString("ss", "Name", null, name);
            writer.WriteStartElement("Table");
            writer.WriteAttributeString("ss", "ExpandedColumnCount", null, columns.Count.ToString());
            writer.WriteAttributeString("ss", "ExpandedRowCount", null, ExpandedRowCount.ToString());
            writer.WriteAttributeString("x", "FullColumns", null, "1");
            writer.WriteAttributeString("x", "FullRows", null, "1");

            foreach (PageColumn col in columns)
            {
                col.Persist(writer);
            }

            writer.WriteStartElement("Row");
            foreach (PageColumn col in columns)
            {
                writer.WriteStartElement("Cell");
                writer.WriteAttributeString("ss", "StyleID", null, "s21");
                writer.WriteStartElement("Data");
                writer.WriteAttributeString("ss", "Type", null, "String");
                writer.WriteRaw(col.name);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            foreach (PageRow row in rows)
            {
                row.Persist(writer);
            }
            writer.WriteEndElement();
            writer.WriteRaw(props);
            writer.WriteEndElement();
        }


        internal class PageColumn
        {
            internal string name;
            internal ColumnDataType datatype;
            internal Alignment align;

            public PageColumn(string n, ColumnDataType type, ExcelPage.Alignment a)
            {
                name = n;
                datatype = type;
                align = a;
            }
            internal void Persist(XmlTextWriter writer)
            {
                writer.WriteStartElement("Column");
                writer.WriteAttributeString("ss", "StyleID", null, "s21");
                writer.WriteEndElement();
/*
    <Column ss:StyleID='s23' ss:AutoFitWidth='0' ss:Width='24.75'/>
    <Column ss:StyleID='s23' ss:AutoFitWidth='0' ss:Width='15.75' ss:Span='1'/>
*/
            }
        }

        internal class PageRow
        {
            private IList<PageColumn> columns;
            private List<string> values;

            internal PageRow(IList<PageColumn> cols)
            {
                columns = cols;
                values = new List<string>();
            }

            internal void SetValues(params object[] vals)
            {
                if (vals.Length > columns.Count)
                    throw new Exception(string.Format("There are more SetValues({0}) than PageRow columns({1})", vals.Length, columns.Count));

                foreach (object x in vals)
                    values.Add(x != null ? x.ToString() : null);
            }
            internal void Persist(XmlTextWriter writer)
            {
                writer.WriteStartElement("Row");
                for (int k = 0; k < values.Count; ++k)
                {
                    string v = values[k];
                    PageColumn col = columns[k];
                    string styleID = styles[col.datatype][col.align];
                    System.Diagnostics.Contracts.Contract.Assert(string.IsNullOrWhiteSpace(styleID), "styleID is missing a proper value.");

                    writer.WriteStartElement("Cell");
                    writer.WriteAttributeString("ss", "StyleID", null, styleID);
                    if (v != null)
                    {
                        writer.WriteStartElement("Data");
                        writer.WriteAttributeString("ss", "Type", null, maptype(col.datatype));
                        writer.WriteRaw(v);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            private string maptype(ColumnDataType type)
            {
                switch (type)
                {
                    case ColumnDataType.Date:
                    case ColumnDataType.Time:
                        return "DateTime";
                    default:
                        return type.ToString();
                }
            }
        }
    }
}