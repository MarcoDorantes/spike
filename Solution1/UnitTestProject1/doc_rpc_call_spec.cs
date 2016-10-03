using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using System.Collections.Generic;
using nutility;
using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;
using System.IO;

namespace UnitTestProject1
{
  [TestClass]
  public class RPC_As_An_AbstractDataType_Spec
  {
    #region SOAP
    //http://stackoverflow.com/questions/9062475/what-is-the-difference-between-document-style-and-rpc-style-communication
    //http://java.globinch.com/enterprise-java/web-services/soap-binding-document-rpc-style-web-services-difference/
    //https://msdn.microsoft.com/en-us/library/4cxy91t2(v=vs.100).aspx
    //https://msdn.microsoft.com/en-us/library/ms996466.aspx
    //https://msdn.microsoft.com/en-us/library/ms996486.aspx
    #endregion

    #region XML data type cases
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

    /*
     Inferring an XML Schema
     https://msdn.microsoft.com/en-us/library/b6kwb7fd(v=vs.110).aspx
     https://msdn.microsoft.com/en-us/library/system.xml.schema.xmlschemainference(v=vs.110).aspx

     Generating XML Documents from XML Schemas
     https://msdn.microsoft.com/en-us/library/aa302296.aspx

     XML Schema Object Model (SOM)
     https://msdn.microsoft.com/en-us/library/bs8hh90b(v=vs.110).aspx
     */
    [TestMethod, Description("https://msdn.microsoft.com/en-us/library/system.xml.schema.xmlschemainference(v=vs.110).aspx")]
    public void inferXSD_1()
    {
      var xml = @"<bookstore xmlns='http://www.contoso.com/books'>
  <book genre='autobiography' publicationdate='1981-03-22' ISBN='1-861003-11-0'>
    <title>The Autobiography of Benjamin Franklin</title>
    <author>
      <first-name>Benjamin</first-name>
      <last-name>Franklin</last-name>
    </author>
    <price>8.99</price>
  </book>
  <book genre='novel' publicationdate='1967-11-17' ISBN='0-201-63361-2'>
    <title>The Confidence Man</title>
    <author>
      <first-name>Herman</first-name>
      <last-name>Melville</last-name>
    </author>
    <price>11.99</price>
  </book>
  <book genre='philosophy' publicationdate='1991-02-15' ISBN='1-861001-57-6'>
    <title>The Gorgias</title>
    <author>
      <name>Plato</name>
    </author>
    <price>9.99</price>
  </book>
</bookstore>";
      XmlReader reader = XmlReader.Create(new System.IO.StringReader(xml));
      XmlSchemaSet schemaSet = new XmlSchemaSet();
      XmlSchemaInference schema = new XmlSchemaInference();

      schemaSet = schema.InferSchema(reader);
      var output = new System.IO.StringWriter();
      foreach (XmlSchema s in schemaSet.Schemas())
      {
        s.Write(output);
      }
      output.Flush();
      var xsd_text = output.GetStringBuilder().ToString();
      Trace.WriteLine(xsd_text);
      var xsd = XDocument.Parse(xsd_text);
      Assert.IsNotNull(xsd);
    }

    private XDocument InferXMLDataType(string xml_text)
    {
      var schema = new XmlSchemaInference();
      XmlSchemaSet schemaSet = schema.InferSchema(XmlReader.Create(new System.IO.StringReader(xml_text)));

      var output = new System.IO.StringWriter();
      foreach (XmlSchema s in schemaSet.Schemas())
      {
        s.Write(output);
      }
      output.Flush();
      var xsd_text = output.GetStringBuilder().ToString();
      Trace.WriteLine($"XML Instance:{xml_text}");
      Trace.WriteLine($"Inferred XSD:{xsd_text}");
      Trace.WriteLine("");
      return XDocument.Parse(xsd_text);
    }
    [TestMethod]
    public void inferXSD_2()
    {
      var xml_text0 = "<proc1><arg1>val1</arg1></proc1>";
      var xsd0 = InferXMLDataType(xml_text0);

      var xml_text1 = "<proc1><args><arg1>val1</arg1></args></proc1>";
      var xsd1 = InferXMLDataType(xml_text1);

      var xml_text2 = "<proc1><args><arg1>val1</arg1><arg1>val2</arg1></args></proc1>";
      var xsd2 = InferXMLDataType(xml_text2);

      var xml_text3 = "<proc><name>proc1</name><args><arg><name>name1</name><value>val1</value></arg></args></proc>";
      var xsd3 = InferXMLDataType(xml_text3);

      var xml_text4 = "<sp1><par1>val1</par1><par2>123.45</par2></sp1>";
      var xsd4 = InferXMLDataType(xml_text4);

      Assert.IsNotNull(xsd1);
      Assert.IsNotNull(xsd2);
      Assert.AreNotEqual<string>(xsd1.ToString(), xsd2.ToString());
    }
    [TestMethod, Description("https://msdn.microsoft.com/en-us/library/bb340331(v=vs.110).aspx")]
    public void validXML_1()
    {
      var xsd1_text = @"<?xml version='1.0' encoding='utf-16'?>
<xs:schema attributeFormDefault='unqualified' elementFormDefault='qualified' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xs:element name='sp1'>
    <xs:complexType>
      <xs:sequence>
        <xs:element name='par1' type='xs:string' />
        <xs:element name='par2' type='xs:decimal' />
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>";
      var schemas = new XmlSchemaSet();
      schemas.Add("", XmlReader.Create(new StringReader(xsd1_text)));

      //var xml_instance1 = "<sp1><par1>val1</par1><par2>123.45</par2></sp1>";
      var xml_instance1 = "<sp1><par1>val1</par1><par2>ABC</par2></sp1>";
      var xml1 = XDocument.Parse(xml_instance1);

      bool typemismatch = false;
      xml1.Validate(schemas, (_, e) => { Trace.WriteLine(e.Message); typemismatch = true; });

      Assert.IsTrue(typemismatch);
    }
    #endregion

    #region GetStoredProcedureCall as a first direct try
    interface IOperationCallBuilder
    {
      string GetRunStatement();
      string GetOperationName();
      string GetArguments();
    }
    static string GetStoredProcedureCall(nutility.ITypeClassMapper typemap)
    {
      var builder = typemap.GetService<IOperationCallBuilder>();
      return $"{builder.GetRunStatement()} {builder.GetOperationName()} {builder.GetArguments()}";
    }
    interface IParameterMetadataProvider
    {
      void LoadSchemaFor(string operation_name);
      string GetLiteralValue(string parameter_name, string parameter_value);
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
      public string GetArguments()
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

    [TestMethod, Description("No schema data")]
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

    [TestMethod, Description("Added simple schema data")]
    public void a_xml_call_4()
    {
      var xml = XDocument.Parse("<sp1><par1>val1</par1><par2>123.45</par2></sp1>");
      var schema = new Dictionary<string, Type> { { "par2", typeof(decimal) } };
      var typemap = new nutility.TypeClassMapper
      (
        new Dictionary<Type, Type>
        {
          { typeof(IOperationCallBuilder), typeof(OperationCallBuilder) },
          { typeof(IParameterMetadataProvider), typeof(ParameterMetadataProvider) }
        },
        new Dictionary<Type, object>
        {
          { typeof(XDocument), xml},
          { typeof(IDictionary<string, Type>), schema }
        }
      );

      var tsql = GetStoredProcedureCall(typemap);
      Assert.AreEqual<string>("EXECUTE sp1 @par1 = 'val1', @par2 = 123.45", tsql);
    }
    #endregion

    #region An inbound XML message processing second try
    static class RPC_Constant
    {
      public static string SerializedRPCDataType = "SerializedRPCDataType";
    }
    interface RPC_As_AbstractDataType
    {
      string GetRunStatement();
      string GetOperationName();
      string GetArguments();
    }
    interface RPC_DataType_To_OperationExecution
    {
      string GetOperationExecution(RPC_As_AbstractDataType calltype_instance);
    }
    class MessageToStoredProcedureCall : RPC_DataType_To_OperationExecution
    {
      public string GetOperationExecution(RPC_As_AbstractDataType calltype_instance) => GetStoredProcedureCall(calltype_instance);
      private string GetStoredProcedureCall(RPC_As_AbstractDataType calltype_instance)
      {
        return $"{calltype_instance.GetRunStatement()} {calltype_instance.GetOperationName()} {calltype_instance.GetArguments()}";
      }
    }
    class ProcedureCallAsXmlMessage : RPC_As_AbstractDataType
    {
      private XDocument xml;
      private IParameterMetadataProvider metadata;
      public ProcedureCallAsXmlMessage(nutility.ITypeClassMapper typemap)
      {
        xml = typemap.GetValue<XDocument>(RPC_Constant.SerializedRPCDataType);
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
      public string GetArguments()
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
    [TestMethod]
    public void a_xml_call_5()
    {
      //Arrange
      var schema = new Dictionary<string, Type> { { "par2", typeof(decimal) } };
      var typemap = new nutility.TypeClassMapper
      (
        new Dictionary<Type, Type>
        {
          { typeof(RPC_DataType_To_OperationExecution), typeof(MessageToStoredProcedureCall) },
          { typeof(RPC_As_AbstractDataType), typeof(ProcedureCallAsXmlMessage) },
          { typeof(IParameterMetadataProvider), typeof(ParameterMetadataProvider) }
        },
        new Dictionary<Type, object>
        {
          { typeof(IDictionary<string, Type>), schema }
        }
      );

      //Act
      var xml = XDocument.Parse("<sp1><par1>val1</par1><par2>123.45</par2></sp1>");
      typemap.SetValue<XDocument>(RPC_Constant.SerializedRPCDataType, xml);
      var inbound = typemap.GetService<RPC_As_AbstractDataType>();

      var transform = typemap.GetService<RPC_DataType_To_OperationExecution>();
      var execution = transform.GetOperationExecution(inbound);

      //Assert
      Assert.AreEqual<string>("EXECUTE sp1 @par1 = 'val1', @par2 = 123.45", execution);
    }

    [TestMethod]
    public void a_xml_call_6()
    {
      //Arrange
      var schema = new Dictionary<string, Type> { { "par2", typeof(decimal) } };
      var typemap = new nutility.TypeClassMapper
      (
        new Dictionary<Type, Type>
        {
          { typeof(RPC_DataType_To_OperationExecution), typeof(MessageToStoredProcedureCall) },
          { typeof(RPC_As_AbstractDataType), typeof(ProcedureCallAsXmlMessage) },
          { typeof(IParameterMetadataProvider), typeof(ParameterMetadataProvider) }
        },
        new Dictionary<Type, object>
        {
          { typeof(IDictionary<string, Type>), schema }
        }
      );

      //Act
      //Call 1
      var xml = XDocument.Parse("<sp1><par1>val1</par1><par2>123.45</par2></sp1>");
      typemap.SetValue<XDocument>(RPC_Constant.SerializedRPCDataType, xml);
      var inbound1 = typemap.GetService<RPC_As_AbstractDataType>();

      var transform = typemap.GetService<RPC_DataType_To_OperationExecution>();
      var execution1 = transform.GetOperationExecution(inbound1);

      //Call 2
      var xml2 = XDocument.Parse("<sp2><par1>val1</par1><par2>123.45</par2></sp2>");
      typemap.SetValue<XDocument>(RPC_Constant.SerializedRPCDataType, xml2);
      var inbound2 = typemap.GetService<RPC_As_AbstractDataType>();

      transform = typemap.GetService<RPC_DataType_To_OperationExecution>();
      var execution2 = transform.GetOperationExecution(inbound2);

      //Assert
      Assert.AreEqual<string>("EXECUTE sp1 @par1 = 'val1', @par2 = 123.45", execution1);
      Assert.AreEqual<string>("EXECUTE sp2 @par1 = 'val1', @par2 = 123.45", execution2);
    }
    #endregion
  }
}