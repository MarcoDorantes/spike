﻿using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using System.Collections.Generic;
using nutility;

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
          new XElement("arg1", "val1")
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
    interface IParameterMetadataProvider
    {
      void LoadSchemaFor(string operation_name);
      string GetLiteralValue(string parameter_name, string parameter_value);
    }
    static string GetStoredProcedureCall(nutility.ITypeClassMapper typemap)
    {
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
        metadata.LoadSchemaFor(GetOperationName());
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
      private nutility.ITypeClassMapper typemap;
      private IDictionary<string, Type> Schema;
      public ParameterMetadataProvider(nutility.ITypeClassMapper typemap)
      {
        this.typemap = typemap;
      }
      public void LoadSchemaFor(string operation_name)
      {
        Schema = typemap.GetService<IDictionary<string, Type>>();
      }

      public string GetLiteralValue(string parameter_name, string parameter_value)
      {
        if (Schema != null && Schema.ContainsKey(parameter_name))
        {
          //TODO Check Get_Quasi_T_SQL
          return Schema[parameter_name] == typeof(decimal) ? $"{parameter_value}" : $"'{parameter_value}'";
        }
        return $"'{parameter_value}'";
      }
    }

    [TestMethod]
    public void a_xml_call_3()
    {
      var xml = XDocument.Parse("<sp1><par1>val1</par1><par2>123.45</par2></sp1>");
      var typemap = new nutility.TypeClassMapper
      (
        new Dictionary<Type, Type>
        {
          { typeof(IOperationCallBuilder), typeof(OperationCallBuilder) },
          { typeof(IParameterMetadataProvider), typeof(ParameterMetadataProvider) }
        },
        new Dictionary<Type, object>
        {
          { typeof(XDocument), xml },
          { typeof(IDictionary<string, Type>), null }
        }
      );
      var tsql = GetStoredProcedureCall(typemap);
      Assert.AreEqual<string>("EXECUTE sp1 @par1 = 'val1', @par2 = '123.45'", tsql);
    }
    [TestMethod]
    public void a_xml_call_4()
    {
      var xml = XDocument.Parse("<sp1><par1>val1</par1><par2>123.45</par2></sp1>");
      var schema = new Dictionary<string, Type> { { "par2", typeof(decimal) } };
      var typemap = new nutility.TypeClassMapper
      (
        new Dictionary<Type, Type> {
          { typeof(IOperationCallBuilder), typeof(OperationCallBuilder) },
          { typeof(IParameterMetadataProvider), typeof(ParameterMetadataProvider) }
        },
        new Dictionary<Type, object> {
          { typeof(IDictionary<string, Type>), schema },
          { typeof(XDocument), xml},
        }
      );

      var tsql = GetStoredProcedureCall(typemap);
      Assert.AreEqual<string>("EXECUTE sp1 @par1 = 'val1', @par2 = 123.45", tsql);
    }
    interface IDataType
    {
      void SetSerializedDataTypeAsXDocument(XDocument xml);
      XDocument GetXDocument();
    }
    interface IDataTypeToOperationExecution { string GetOperationExecution(IDataType data, nutility.ITypeClassMapper typemap); }
    class DataTypeToStoredProcedureCall : IDataTypeToOperationExecution
    {
      public string GetOperationExecution(IDataType data, ITypeClassMapper typemap)
      {
        typemap.AddMapping<XDocument>(data.GetXDocument());
        var tsql = GetStoredProcedureCall(typemap);
        return tsql;
      }
    }
    class DataType : IDataType
    {
      private XDocument xml;

      public void SetSerializedDataTypeAsXDocument(XDocument xml) { this.xml = xml; }
      public XDocument GetXDocument() => xml;
    }
    [TestMethod]
    public void a_xml_call_5()
    {
      var xml = XDocument.Parse("<sp1><par1>val1</par1><par2>123.45</par2></sp1>");
      var schema = new Dictionary<string, Type> { { "par2", typeof(decimal) } };
      var typemap = new nutility.TypeClassMapper
      (
        new Dictionary<Type, Type>
        {
          { typeof(IOperationCallBuilder), typeof(OperationCallBuilder) },
          { typeof(IDataTypeToOperationExecution), typeof(DataTypeToStoredProcedureCall) },
          { typeof(IDataType), typeof(DataType) },
          { typeof(IParameterMetadataProvider), typeof(ParameterMetadataProvider) }
        },
        new Dictionary<Type, object>
        {
          { typeof(IDictionary<string, Type>), schema }
        }
      );
      var inbound = typemap.GetService<IDataType>();
      inbound.SetSerializedDataTypeAsXDocument(xml);
      var transform = typemap.GetService<IDataTypeToOperationExecution>();
      var execution = transform.GetOperationExecution(inbound, typemap);
      Assert.AreEqual<string>("EXECUTE sp1 @par1 = 'val1', @par2 = 123.45", execution);
    }
  }
}