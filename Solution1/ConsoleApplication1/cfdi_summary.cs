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
    public static void sat()
    {
      var getconcept = new Func<string, string>(n =>
      {
        System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(n, "[a-zA-Z]");
        var result = new StringBuilder();
        for (; match.Success; match = match.NextMatch()) { result.Append(match.Value); }
        return $"{result}";
      });
      //WriteLine(getconcept("1414-3143-BON-2018-12-41477"));return;
      var folder = new DirectoryInfo(@"C:\path");
      int count = 0;
      XNamespace cfdi_ns = "http://www.sat.gob.mx/cfd/3";
      XNamespace nomina12_ns = "http://www.sat.gob.mx/nomina12";
      WriteLine("#,Fecha,Folio,FechaPago,Días,TotalPercepciones,TotalDeducciones,TotalSueldos,TotalGravado,TotalExento,PercepcionesConceptos,PercepcionesImportesExentos,PercepcionesImportesGravados,DeduccionesConceptos,DeduccionesImportes,SubTotal,Descuento,Total,Concepto,CFDI");
      //foreach (var cfdi in folder.EnumerateFiles("*.xml").Where(x => x.FullName.Contains("MES-2018")).OrderByDescending(y => y.LastWriteTime))
      foreach (var cfdi in folder.EnumerateFiles("*.xml").OrderByDescending(y => y.LastWriteTime))
      {
        //WriteLine(cfdi.FullName);
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
    public static void _Main(string[] args) { sat(); }
  }
}
/*class exe
{
//[STAThread]
static void Main(string[] args) { ConsoleApplication1.cfdi_summary._Main(args); }
}*/
