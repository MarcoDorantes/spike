using System;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Console;

namespace ConsoleApplication1
{
  class cfdi_summary
  {
    /*
http://www.sat.gob.mx/cfd/3
http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd

http://www.sat.gob.mx/nomina12
http://www.sat.gob.mx/sitio_internet/cfd/nomina/nomina12.xsd

$ns = @{cfdi = "http://www.sat.gob.mx/cfd/3"; xsi = "http://www.w3.org/2001/XMLSchema-instance"; nomina12 = "http://www.sat.gob.mx/nomina12"}

dir -path C:\Users\Marco\Documents\DEC_2020\SAT -filter *.xml -recurse| %{select-xml -Path $_.FullName -Namespace $ns -XPath '/cfdi:Comprobante/@Version'}|select -ExpandProperty Node
dir -path C:\Users\Marco\Documents\DEC_2020\SAT -filter *.xml -recurse| %{select-xml -Path $_.FullName -Namespace $ns -XPath '/cfdi:Comprobante/@xsi:schemaLocation'}|select -ExpandProperty Node
|%{$_.value} | out-file -FilePath C:\temp\xdslocs2020.txt
dir -path C:\Users\Marco\Documents\DEC_2020\SAT -filter *.xml -recurse| %{select-xml -Path $_.FullName -Namespace $ns -XPath '/cfdi:Comprobante/@Fecha'}|select -ExpandProperty Node
|%{[datetime]$_.value} | sort | %{"{0:s}" -f $_}
			      | %{"{0:MM}" -f $_}
dir -path C:\Users\Marco\Documents\DEC_2020\SAT -filter *.xml -recurse| %{select-xml -Path $_.FullName -Namespace $ns -XPath '/cfdi:Comprobante/@Folio'}|select -ExpandProperty Node|%{$_.value}|sort
    */
    public static IEnumerable<XAttribute> read_attr(XElement e)
    {
      if (e.HasAttributes) foreach (var a in e.Attributes()) yield return a;
      if (e.HasElements) foreach (var s in e.Elements()) foreach (var x in read_attr(s)) yield return x;
    }
    public static void attr(XElement e, Action<XAttribute> visit)
    {
      if (e.HasAttributes) foreach (var a in e.Attributes()) visit(a);
      if (e.HasElements) foreach (var s in e.Elements()) attr(s, visit);
    }
    public static void comp()
    {
      XNamespace cfdi_ns = "http://www.sat.gob.mx/cfd/3";
      XNamespace nomina12_ns = "http://www.sat.gob.mx/nomina12";

      var add_cfdi = new Func<Dictionary<string, XDocument>, FileInfo, Dictionary<string, XDocument>>((w, n) =>
      {
        var xml = XDocument.Load(n.FullName);
        var key = $"{xml.Root.Attribute("Folio").Value}|{Math.Abs(double.Parse(xml.Root.Element(cfdi_ns + "Complemento").Element(nomina12_ns + "Nomina").Attribute("NumDiasPagados").Value))}|{xml.Root.Attribute("Sello").Value}";
        w.Add(key, xml);
        return w;
      });

      var cfdi1_file = (new DirectoryInfo(@"C:\Users\Marco\Documents\DEC_2020\GBM")).EnumerateFiles("*.xml");
      var cfdi2_file = (new DirectoryInfo(@"C:\Users\Marco\Documents\DEC_2020\SAT")).EnumerateFiles("*.xml");
      if (cfdi1_file.Count() != cfdi2_file.Count()) throw new ArgumentOutOfRangeException($"CFDI file number must be the same ({cfdi1_file.Count()} != {cfdi2_file.Count()})");
      var cfdi_set1 = cfdi1_file.Aggregate(new Dictionary<string, XDocument>(), (w, n) => add_cfdi(w, n));
      var cfdi_set2 = cfdi2_file.Aggregate(new Dictionary<string, XDocument>(), (w, n) => add_cfdi(w, n));

      foreach (var key in cfdi_set1.Keys)
      {
        var xml1 = cfdi_set1[key];
        var xml2 = cfdi_set2[key];

        var seq1 = read_attr(xml1.Root).OrderBy(x => x.Name.LocalName).Where(n => n.Name.LocalName != "schemaLocation").Select(y => $"[{y.Name}]=[{y?.Value}]");
        var seq2 = read_attr(xml2.Root).OrderBy(x => x.Name.LocalName).Where(n => n.Name.LocalName != "schemaLocation").Select(y => $"[{y.Name}]=[{y?.Value}]");

        WriteLine($"{key.Substring(0, 30)} {seq1.SequenceEqual(seq2)}");
      }
    }
    public static void sat()
    {
      var getconcept = new Func<string, string>(n =>
      {
        var match = System.Text.RegularExpressions.Regex.Match(n, "[a-zA-Z]");
        var result = new StringBuilder();
        for (; match.Success; match = match.NextMatch()) result.Append(match.Value);
        return $"{result}";
      });
      var folder = new DirectoryInfo(@"C:\Users\Marco\Documents\DEC_2020\SAT");
      //var folder = new DirectoryInfo(@"C:\Users\Marco\Documents\DEC_2020\GBM");

      int count = 0;
      XNamespace cfdi_ns = "http://www.sat.gob.mx/cfd/3";
      XNamespace nomina12_ns = "http://www.sat.gob.mx/nomina12";
      WriteLine("#,Fecha,Folio,FechaPago,Días,TotalPercepciones,TotalDeducciones,TotalSueldos,TotalGravado,TotalExento,PercepcionesConceptos,PercepcionesImportesExentos,PercepcionesImportesGravados,DeduccionesConceptos,DeduccionesImportes,SubTotal,Descuento,Total,Concepto,CFDI");
      foreach (var cfdi in folder.EnumerateFiles("*.xml").OrderByDescending(y => y.LastWriteTime))
      {
        var xml = XDocument.Load(cfdi.FullName);
        var Nomina = xml.Root.Element(cfdi_ns + "Complemento")?.Element(nomina12_ns + "Nomina");
        var Percepciones = Nomina?.Element(nomina12_ns + "Percepciones");
        var Deducciones = Nomina?.Element(nomina12_ns + "Deducciones");

        var Fecha = xml.Root.Attribute("Fecha").Value;
        var Folio = xml.Root.Attribute("Folio").Value;
        var FechaPago = Nomina?.Attribute("FechaPago")?.Value;
        var NumDiasPagados = Math.Abs(double.Parse(Nomina?.Attribute("NumDiasPagados")?.Value ?? "0"));
        var TotalPercepciones = Nomina?.Attribute("TotalPercepciones")?.Value;
        var TotalDeducciones = Nomina?.Attribute("TotalDeducciones")?.Value;
        var TotalSueldos = Percepciones?.Attribute("TotalSueldos")?.Value;
        var TotalGravado = Percepciones?.Attribute("TotalGravado")?.Value;
        var TotalExento = Percepciones?.Attribute("TotalExento")?.Value;
        var PercepcionesConceptos = $"{Percepciones?.Elements(nomina12_ns + "Percepcion")?.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0} ", n?.Attribute("Concepto")?.Value))}";
        var PercepcionesImportesExentos = $"{Percepciones?.Elements(nomina12_ns + "Percepcion")?.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("+{0}", n?.Attribute("ImporteExento")?.Value))}";
        var PercepcionesImportesGravados = $"{Percepciones?.Elements(nomina12_ns + "Percepcion")?.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("+{0}", n?.Attribute("ImporteGravado")?.Value))}";
        var DeduccionesConceptos = $"{Deducciones?.Elements(nomina12_ns + "Deduccion")?.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0} ", n?.Attribute("Concepto")?.Value))}";
        var DeduccionesImportes = $"{Deducciones?.Elements(nomina12_ns + "Deduccion")?.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("+{0}", n?.Attribute("Importe")?.Value))}";

        var SubTotal = xml.Root.Attribute("SubTotal").Value;
        var Descuento = xml.Root.Attribute("Descuento").Value;
        var Total = xml.Root.Attribute("Total").Value;
        var Concepto = getconcept(Path.GetFileNameWithoutExtension(cfdi.Name));
        var CFDI = cfdi.Name;
        WriteLine($"{++count},{Fecha},{Folio},=\"{FechaPago}\",{NumDiasPagados},{TotalPercepciones},{TotalDeducciones},{TotalSueldos},{TotalGravado},{TotalExento},{PercepcionesConceptos},={PercepcionesImportesExentos},={PercepcionesImportesGravados},{DeduccionesConceptos},={DeduccionesImportes},{SubTotal},{Descuento},{Total},{Concepto},{CFDI}");
      }
    }
    public static void _Main(string[] args)
    {
      //comp();
      sat();
    }
  }
}
/*class exe
{
//[STAThread]
static void Main(string[] args) { ConsoleApplication1.cfdi_summary._Main(args); }
}*/