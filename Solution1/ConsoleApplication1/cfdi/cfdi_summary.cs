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
      var cfdi1 = new FileInfo(@"C:\Users\Marco\Documents\DEC_2018\gbm_2018\1414-3143-MES-2018-1-41477.xml");
      var cfdi2 = new FileInfo(@"C:\Users\Marco\Documents\DEC_2018\SAT_2018\ComprobanteCFDI.60aa3232-29c0-403c-8839-306a2cc7ea91.xml");

      var xml1 = XDocument.Load(cfdi1.FullName);
      var xml2 = XDocument.Load(cfdi2.FullName);

      //attr(xml.Root, a => WriteLine($"{a.Name}={a?.Value}"));
      //foreach (var a in read_attr(xml1.Root).OrderBy(x => x.Name.LocalName)) WriteLine($"{a.Name}={a?.Value}");
      var seq1 = read_attr(xml1.Root).OrderBy(x => x.Name.LocalName).Where(n => n.Name.LocalName != "schemaLocation").Select(y => $"[{y.Name}]=[{y?.Value}]");
      var seq2 = read_attr(xml2.Root).OrderBy(x => x.Name.LocalName).Where(n => n.Name.LocalName != "schemaLocation").Select(y => $"[{y.Name}]=[{y?.Value}]");
      seq1.Aggregate(Out,(w,n)=>{ w.WriteLine(n); return w; });
      WriteLine("\t\n*****\n");
      seq2.Aggregate(Out, (w, n) => { w.WriteLine(n); return w; });

      /*WriteLine($"{seq1.Count()}\t{seq2.Count()}");
      for (int k = 0; k < seq1.Count(); ++k)
      {
        if (string.Compare(seq1.ElementAt(k), seq2.ElementAt(k), false) != 0)
        {
          WriteLine($"{k}\n{seq1.ElementAt(k)}\n{seq2.ElementAt(k)}\n");
          var b1 = Encoding.ASCII.GetBytes(seq1.ElementAt(k));
          var b2 = Encoding.ASCII.GetBytes(seq2.ElementAt(k));
          WriteLine($"\n{b1.Count()}\t{b2.Count()}");
          for (int j = 0; j < b1.Length; ++j)
          {
            if(b1[j]!=b2[j]) WriteLine($"{j}\n{b1[j]}\n{b2[j]}\n");
          }
        }
      }*/

      WriteLine(seq1.SequenceEqual(seq2));
    }
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
      var folder = new DirectoryInfo(@"C:\Users\Marco\Documents\DEC_2018\SAT");
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
    public static void _Main(string[] args)
    {
      comp();
      //sat();
    }
  }
}
/*class exe
{
//[STAThread]
static void Main(string[] args) { ConsoleApplication1.cfdi_summary._Main(args); }
}*/
