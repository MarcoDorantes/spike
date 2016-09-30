using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using System.Collections.Generic;

namespace UnitTestProject1
{
  [TestClass]
  public class doc_rpc_call_spec
  {
    //http://stackoverflow.com/questions/9062475/what-is-the-difference-between-document-style-and-rpc-style-communication
    //http://java.globinch.com/enterprise-java/web-services/soap-binding-document-rpc-style-web-services-difference/
    //https://msdn.microsoft.com/en-us/library/4cxy91t2(v=vs.100).aspx
    //https://msdn.microsoft.com/en-us/library/ms996466.aspx
    //https://msdn.microsoft.com/en-us/library/ms996486.aspx
    [TestMethod]
    public void a_xml_call()
    {
      var xml = new XDocument(
        new XElement("proc1",
          new XElement("arg1","val1")
        )
      );

      Assert.AreEqual<string>("<proc1><arg1>val1</arg1></proc1>", xml.ToString(SaveOptions.DisableFormatting));
    }

    [TestMethod]
    public void a_xml_call_2()
    {
      var xml = new XDocument(
        new XElement("proc",
          new XElement("name", "proc1"),
          new XElement("args",
            new XElement("arg", new XElement("name", "name1"), new XElement("value", "val1"))
          )
        )
      );

      Assert.AreEqual<string>("<proc><name>proc1</name><args><arg><name>name1</name><value>val1</value></arg></args></proc>", xml.ToString(SaveOptions.DisableFormatting));
    }

    interface IOperationCallBuilder
    {
      string GetRunStatement();
      string GetOperationName();
      string RenderArguments();
    }
    interface IParameterMetadataProvider { string GetLiteralValue(string parameter_name, string parameter_value); }
    string GetOperationCall(XDocument xml, nutility.ITypeClassMapper typemap)
    {
      typemap.AddMapping<XDocument>(xml);
      var builder = typemap.GetService<IOperationCallBuilder>();

      return $"{builder.GetRunStatement()} {builder.GetOperationName()} {builder.RenderArguments()}";
    }
    class OperationCallBuilder : IOperationCallBuilder
    {
      private XDocument xml;
      private IParameterMetadataProvider metadata;
      public OperationCallBuilder(nutility.ITypeClassMapper typemap)
      {
        xml = typemap.GetService<XDocument>();
        metadata = typemap.GetService<IParameterMetadataProvider>();
      }
      public string GetRunStatement()
      {
        return "EXECUTE";
      }
      public string GetOperationName()
      {
        return xml.Root.Name.LocalName;
      }
      public string RenderArguments()
      {
        if (xml.Root.Elements().Count() > 0)
        {
          return xml.Root.Elements().Aggregate(new StringBuilder(), (w, n) => w.AppendFormat(", @{0} = {1}", n.Name.LocalName, metadata.GetLiteralValue(n.Name.LocalName, n.Value))).ToString().Substring(2);
        }
        else
        {
          return string.Empty;
        }
      }
    }
    class ParameterMetadataProvider : IParameterMetadataProvider
    {
      public string GetLiteralValue(string parameter_name, string parameter_value)
      {
        if (Schema != null && Schema.ContainsKey(parameter_name))
        {
          //TODO Check Get_Quasi_T_SQL
          return  Schema[parameter_name] == typeof(decimal) ? $"{parameter_value}" : $"'{parameter_value}'";
        }
        return $"'{parameter_value}'";
      }
      public Dictionary<string, Type> Schema;
    }

    [TestMethod]
    public void a_xml_call_3()
    {
      var typemap = new nutility.TypeClassMapper(new Dictionary<Type, Type>
      {
        { typeof(IOperationCallBuilder), typeof(OperationCallBuilder) },
        { typeof(IParameterMetadataProvider), typeof(ParameterMetadataProvider) },
      });
      var xml = XDocument.Parse("<sp1><par1>val1</par1><par2>123.45</par2></sp1>");
      var tsql = GetOperationCall(xml,typemap);
      Assert.AreEqual<string>("EXECUTE sp1 @par1 = 'val1', @par2 = '123.45'", tsql);
    }
    [TestMethod]
    public void a_xml_call_4()
    {
      var metadata = new ParameterMetadataProvider() { Schema = new Dictionary<string, Type> { { "par2", typeof(decimal) } } };
      var typemap = new nutility.TypeClassMapper(
        new Dictionary<Type, Type> { { typeof(IOperationCallBuilder), typeof(OperationCallBuilder) } },
        new Dictionary<Type, object> { { typeof(IParameterMetadataProvider), metadata } }
      );

      var xml = XDocument.Parse("<sp1><par1>val1</par1><par2>123.45</par2></sp1>");
      var tsql = GetOperationCall(xml, typemap);
      Assert.AreEqual<string>("EXECUTE sp1 @par1 = 'val1', @par2 = 123.45", tsql);
    }
  }
}