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

    #region An inbound XML message processing third try
    /*
        --TSQLPayloadTarget.cs
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using SqlWriterAgent;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Xml.Linq;

    namespace SqlTargetStoredProcedureExecutor
    {
        internal static class RPC_Constant
        {
            public static string SerializedRPCDataType = "SerializedRPCDataType";
        }

        //maybe this is, in fact, an internal Target or a PreparedSqlStoredProcedureBase-derivation ,maybe (check quasi-TSQL integration)? -instead of a hosted Processor-, as it is closely related to SP execution.
        public class TSQLPayloadTarget : SqlWriterAgent.ITargetHostedProcessor
        {
            private string connection_string;
            private ulong message_count;
            private Stopwatch elapsed;

            #region SqlWriterAgent.ITargetHostedProcessor
            public System.Threading.CancellationToken Cancellation { get; set; }
            public IDictionary<string, object> Configuration { get; set; }
            public ITargetProcessorHost Host { get; set; }
            public string Name
            {
                get
                {
                    var type = GetType();
                    return $"{type.FullName} v{type.Assembly.GetName().Version}";
                }
            }

            public void Setup()
            {
                message_count = 0UL;
                elapsed = new Stopwatch();
                connection_string = System.Configuration.ConfigurationManager.ConnectionStrings[Configuration[ConfigurationKey.ConnectionStringKey].ToString()].ConnectionString;
                Host.Information($"{nameof(TSQLPayloadTarget)}.Setup ConnectionString = {connection_string}");
                CheckDBConnection();
            }

            public void Start()
            {
                Host.Information($"{nameof(TSQLPayloadTarget)}.Start()");
            }

            public void Start(string initialization_message)
            {
                Host.Information($"{nameof(TSQLPayloadTarget)}.Start({initialization_message})");
            }

            public void Start(IDictionary<string, object> initialization_message)
            {
                Host.Information($"{nameof(TSQLPayloadTarget)}.Start({initialization_message})");
            }

            public void Stop()
            {
                Host.Information($"{nameof(TSQLPayloadTarget)}.Stop()");
            }

            public void ProcessMessage(IDictionary<string, object> message)
            {
                ExecutePayload(message);
            }
            #endregion

            private void CheckDBConnection()
            {
                using (var con = new SqlConnection(connection_string))
                {
                    con.Open();
                }
            }
            private void ExecutePayload(IDictionary<string, object> message)
            {
                string payload = null;
                try
                {
                    if (!message.ContainsKey(Constant.SolaceMessagePayloadKey))
                    {
                        throw new Exception($"{nameof(Constant.SolaceMessagePayloadKey)} key with the payload is missing in the incoming message.");
                    }
                    payload = message[Constant.SolaceMessagePayloadKey].ToString();
                    if (string.IsNullOrWhiteSpace(payload))
                    {
                        throw new Exception($"Received payload is null or empty.");
                    }
                    if (IsXML(payload))
                    {
                        ExecutePayloadAsXMLSerializedDataType(payload, message);
                    }
                    else
                    {
                        ExecutePayloadAsTSQL(payload, message);
                    }
                }
                catch (Exception ex)
                {
                    Exception ex2 = ex;
                    var exs = new StringBuilder();
                    int count = 0;
                    while (ex2 != null)
                    {
                        exs.AppendLine($"[Level {++count}]: {ex2}");
                        ex2 = ex2.InnerException;
                    }
                    Host.Error(ex, $"payload: {payload}\r\n{exs}");
                    //There is no 'throw' statement here because this Processor logs and reports all exceptions: the Solace ACK must be sent for all cases as the related queue has no TTL.
                }
            }
            private bool IsXML(string text)
            {
                try
                {
                    XDocument.Parse(text);
                    return true;
                }
                catch (System.Xml.XmlException)
                {
                    return false;
                }
            }
            private void ExecutePayloadAsXMLSerializedDataType(string xmltext, IDictionary<string, object> message)
            {
                Host.Information($"Received payload (XML): [{xmltext}]");
                OperationCallPreparation preparation = GetSqlCommandPreparation(xmltext, CreateTypeMapForSqlCommandPreparation());
                Host.Information($"SP to be prepared: [{preparation?.OperationName}]");

                string GUID = null;
                string MESSAGE = null;

                var getSPCommand = new Func<SqlConnection, SqlCommand>(connection =>
                {
                    var command = connection.CreateCommand();
                    command.CommandText = preparation?.OperationName;
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlCommandBuilder.DeriveParameters(command);
                    preparation.SetInboundValues(command);
                    return command;
                });

                DateTime sp_begin = DateTime.Now;
                try
                {
                    elapsed.Restart();
                    ExecuteSqlCommand(getSPCommand, out GUID, out MESSAGE);
                    CheckTSQLResults(preparation?.OperationName, GUID, MESSAGE, message);
                }
                finally
                {
                    elapsed.Stop();
                }
                DateTime sp_end = DateTime.Now;
                Host.InformationMessageProcessed(this.message_count, GUID, MESSAGE, sp_begin, sp_end, elapsed.ElapsedMilliseconds);
            }
            private void ExecutePayloadAsTSQL(string received_tsql, IDictionary<string, object> message)
            {
                Host.Information($"Received payload (T-SQL): [{received_tsql}]");

                string GUID = null;
                string MESSAGE = null;

                var getTSQLCommand = new Func<SqlConnection, SqlCommand>(connection =>
                {
                    var command = connection.CreateCommand();
                    command.CommandText = received_tsql;
                    command.CommandType = System.Data.CommandType.Text;
                    return command;
                });

                DateTime sp_begin = DateTime.Now;
                try
                {
                    elapsed.Restart();
                    ExecuteSqlCommand(getTSQLCommand, out GUID, out MESSAGE);
                    CheckTSQLResults(received_tsql, GUID, MESSAGE, message);
                }
                finally
                {
                    elapsed.Stop();
                }
                DateTime sp_end = DateTime.Now;
                Host.InformationMessageProcessed(this.message_count, GUID, MESSAGE, sp_begin, sp_end, elapsed.ElapsedMilliseconds);
            }
            private void CheckTSQLResults(string received_tsql, string GUID, string MESSAGE, IDictionary<string, object> message)
            {
                string GUID_problem = string.IsNullOrWhiteSpace(GUID) ? "GUID returned empty." : null;
                string MESSAGE_problem = string.IsNullOrWhiteSpace(MESSAGE) ? "MESSAGE returned empty." : null;
                if (string.IsNullOrWhiteSpace(MESSAGE_problem))
                {
                    MESSAGE_problem = MESSAGE.EndsWith("ERROR", StringComparison.OrdinalIgnoreCase) ? MESSAGE : null;
                }
                if (GUID_problem != null || MESSAGE_problem != null)
                {
                    Host.ErrorMessageProcessed(message_count, GUID, MESSAGE, "SqlResultset exception:" + (!string.IsNullOrWhiteSpace(GUID_problem) ? " GUID:" + GUID_problem : "") + (!string.IsNullOrWhiteSpace(MESSAGE_problem) ? " MESSAGE:" + MESSAGE_problem : ""), message, GetCurrentCallContextParameterValues(received_tsql));
                }
            }
            private IDictionary<string, object> GetCurrentCallContextParameterValues(string received_tsql)
            {
                var result = new Dictionary<string, object>();
                var call_context = new StoredProcedureProxy.SqlCallDataContext()
                {
                    MessageCount = message_count.ToString(),
                    WriterName = Name,

                    Timepoint = DateTime.Now.ToString("s"),
                    Description = "",
                    GUID_MESSAGE = "",
                    InboundMessage = received_tsql,
                    SQL_Parameters = "",
                    QuasiTSQL = received_tsql
                };
                result[StoredProcedureProxy.Constant.SqlCallDataContextKey] = call_context;
                result[nameof(TSQLPayloadTarget)] = Name;
                return result;
            }
            private void ExecuteSqlCommand(Func<SqlConnection, SqlCommand> CreateAndSetupCommand, out string id, out string message)
            {
                ++this.message_count;
                using (var connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    using (var command = CreateAndSetupCommand(connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            ProcessSQLResultset(reader, out id, out message);
                        }
                    }
                }
            }

            public void ProcessSQLResultset(System.Data.SqlClient.SqlDataReader reader, out string id, out string result)
            {
                id = null;
                result = null;

                int count = 0;
                var loglines = new System.Text.StringBuilder(string.Format("[{0}] thread: {1} SQL Resultsets:", DateTime.Now.ToString("s"), Environment.CurrentManagedThreadId));
                var line = " (no SQL Resultset)";
                do
                {
                    if (reader.Read())
                    {
                        int fieldCount = reader.FieldCount;
                        int visibleFieldCount = reader.VisibleFieldCount;
                        bool hasRows = reader.HasRows;
                        var values = new object[Math.Max(3, reader.FieldCount)];
                        int value_count = reader.GetValues(values);
                        string values_text = "(no values)";
                        if (value_count > 0)
                        {
                            var text = new System.Text.StringBuilder();
                            for (int k = 0; k < fieldCount; ++k)
                            {
                                var field_name = reader.GetName(k);
                                object field_value = k < value_count ? values[k] : string.Format("(no value at {0})", k);
                                text.AppendFormat("[{0}={1}] ", field_name, field_value);
                                if (k < value_count && string.Compare(field_name, "GUID", true) == 0)
                                {
                                    id = field_value.ToString();
                                }
                                else if (k < value_count && string.Compare(field_name, "MESSAGE", true) == 0)
                                {
                                    result = field_value.ToString();
                                }
                            }
                            values_text = text.ToString();
                        }
                        line = string.Format("\n\tResultset {0}: FieldCount: {1} VisibleFieldCount: {2} HasRows: {3} values: {4}", ++count, fieldCount, visibleFieldCount, hasRows, values_text);
                    }
                    loglines.Append(line);
                } while (reader.NextResult());

                if (id == null || result == null)
                {
                    string logline = loglines.ToString();
                    //System.Diagnostics.Trace.WriteLine(logline);
                    throw new Exception(logline);
                }
            }


            #region Generated TSQL
            private string GetGenerated_TSQL(string xmltext, utility.ITypeClassMapper typemap)
            {
                var xml = XDocument.Parse(xmltext);
                typemap.SetValue<XDocument>(RPC_Constant.SerializedRPCDataType, xml);
                var inbound = typemap.GetService<RPC_As_AbstractDataType>();

                var transform = typemap.GetService<RPC_DataType_To_OperationExecution>();
                return transform.GetOperationExecution(inbound);
            }
            private utility.ITypeClassMapper CreateTypeMapForTSQLGeneration()
            {
                var result = new utility.TypeClassMapper
                (
                  new Dictionary<Type, Type>
                  {
                      { typeof(RPC_DataType_To_OperationExecution), typeof(MessageToStoredProcedureCall) },
                      { typeof(RPC_As_AbstractDataType), typeof(TSQLCallAsXmlMessage) },
                      { typeof(IParameterMetadataProvider), typeof(ParameterMetadataProvider) }
                  }
                );
                result.SetValue<string>(nameof(connection_string), connection_string);
                return result;
            }
            #endregion
            #region SP Preparation
            private OperationCallPreparation GetSqlCommandPreparation(string xmltext, utility.ITypeClassMapper typemap)
            {
                var xml = XDocument.Parse(xmltext);
                typemap.SetValue<XDocument>(RPC_Constant.SerializedRPCDataType, xml);
                var inbound = typemap.GetService<RPC_As_AbstractDataType>();

                var transform = typemap.GetService<RPC_DataType_To_OperationExecution>();
                return transform.GetOperationExecutionObject<SqlTargetStoredProcedureExecutor.OperationCallPreparation>(inbound);
            }
            private utility.ITypeClassMapper CreateTypeMapForSqlCommandPreparation()
            {
                var result = new utility.TypeClassMapper
                (
                  new Dictionary<Type, Type>
                  {
                      { typeof(RPC_DataType_To_OperationExecution), typeof(MessageToStoredProcedureCall) },
                      { typeof(RPC_As_AbstractDataType), typeof(SqlCommandAsXmlMessage) },
                      //{ typeof(IParameterMetadataProvider), typeof(ParameterMetadataProvider) }
                  }
                );
                result.SetValue<string>(nameof(connection_string), connection_string);
                return result;
            }

            #endregion

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // dispose managed state (managed objects).
                    }

                    // free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // set large fields to null.

                    disposedValue = true;
                }
            }

            // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~TSQLPayloadTarget() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion
        }

        class RPC_Argument
        {
            public string Name;
            public string Value;
            public string LiteralValue;
        }
        internal interface RPC_As_AbstractDataType
        {
            string GetRunStatement();
            string GetOperationName();
            IEnumerable<RPC_Argument> GetArguments();
        }
        internal interface RPC_DataType_To_OperationExecution
        {
            string GetOperationExecution(RPC_As_AbstractDataType calltype_instance);
            T GetOperationExecutionObject<T>(RPC_As_AbstractDataType calltype_instance) where T : OperationCallPreparation;
        }
        internal interface IParameterMetadataProvider
        {
            void LoadSchemaFor(string operation_name);
            string GetLiteralValue(string parameter_name, string parameter_value);
        }

        internal class MessageToStoredProcedureCall : RPC_DataType_To_OperationExecution
        {
            string RPC_DataType_To_OperationExecution.GetOperationExecution(RPC_As_AbstractDataType calltype_instance) => GetStoredProcedureCall(calltype_instance);
            T RPC_DataType_To_OperationExecution.GetOperationExecutionObject<T>(RPC_As_AbstractDataType calltype_instance) => (T)GetPreparedStoredProcedure(calltype_instance);

            private string GetStoredProcedureCall(RPC_As_AbstractDataType calltype_instance)
            {
                string arguments = null;
                var args = new List<RPC_Argument>(calltype_instance.GetArguments());
                if (args.Count > 0)
                {
                    arguments = args.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat(", @{0} = {1}", n.Name, n.LiteralValue)).ToString().Substring(2);
                }
                return $"{calltype_instance.GetRunStatement()} {calltype_instance.GetOperationName()} {arguments}";
            }
            private OperationCallPreparation GetPreparedStoredProcedure(RPC_As_AbstractDataType calltype_instance)
            {
                OperationCallPreparation result = new SqlCommandPreparation(calltype_instance);
                return result;
            }
        }
        internal class TSQLCallAsXmlMessage : RPC_As_AbstractDataType
        {
            private XDocument xml;
            private IParameterMetadataProvider metadata;
            public TSQLCallAsXmlMessage(utility.ITypeClassMapper typemap)
            {
                xml = typemap.GetValue<XDocument>(SqlTargetStoredProcedureExecutor.RPC_Constant.SerializedRPCDataType);
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
            public IEnumerable<RPC_Argument> GetArguments()
            {
                if (xml.Root.Elements().Count() > 0)
                {
                    foreach (var element in xml.Root.Elements())
                    {
                        yield return new RPC_Argument() { Name = element.Name.LocalName, Value = element.Value, LiteralValue = metadata.GetLiteralValue(element.Name.LocalName, element.Value) };
                    }
                }
            }
        }
        internal class SqlCommandAsXmlMessage : RPC_As_AbstractDataType
        {
            private XDocument xml;
            //private IParameterMetadataProvider metadata;
            public SqlCommandAsXmlMessage(utility.ITypeClassMapper typemap)
            {
                xml = typemap.GetValue<XDocument>(SqlTargetStoredProcedureExecutor.RPC_Constant.SerializedRPCDataType);
                //metadata = typemap.GetService<IParameterMetadataProvider>();
                //metadata.LoadSchemaFor(GetOperationName());
            }
            public string GetRunStatement()
            {
                throw new NotImplementedException();
            }
            public string GetOperationName() => xml.Root.Name.LocalName;
            public IEnumerable<RPC_Argument> GetArguments()
            {
                if (xml.Root.Elements().Count() > 0)
                {
                    foreach (var element in xml.Root.Elements())
                    {
                        //yield return new RPC_Argument() { Name = element.Name.LocalName, Value = element.Value, LiteralValue = metadata.GetLiteralValue(element.Name.LocalName, element.Value) };
                        yield return new RPC_Argument() { Name = element.Name.LocalName, Value = element.Value };
                    }
                }
            }
        }
        internal class ParameterMetadataProvider : IParameterMetadataProvider
        {
            private string connection_string;
            public ParameterMetadataProvider(utility.ITypeClassMapper typemap)
            {
                connection_string = typemap.GetValue<string>(nameof(connection_string));
            }
            public void LoadSchemaFor(string sp)
            {
                using (var connection = new SqlConnection(connection_string))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = sp;
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    connection.Open();
                    SqlCommandBuilder.DeriveParameters(command);
                    Schema = new Dictionary<string, System.Data.SqlDbType>();
                    for (int k = 0; k < command.Parameters.Count; ++k)
                    {
                        SqlParameter p = command.Parameters[k];
                        if (p.Direction == System.Data.ParameterDirection.Input)
                        {
                            Schema[p.ParameterName.Replace("@", "")] = p.SqlDbType;
                        }
                    }
                }
            }
            public string GetLiteralValue(string parameter_name, string parameter_value)
            {
                if (Schema != null && Schema.ContainsKey(parameter_name))
                {
                    return $"{GetSqlTextValue(parameter_value, Schema[parameter_name])}";
                }
                return $"'{parameter_value}'";
            }
            public Dictionary<string, System.Data.SqlDbType> Schema;

            private string GetSqlTextValue(object parameter_value, System.Data.SqlDbType parameter_type)
            {
                string result = "";
                if (parameter_value == null)
                {
                    result = "default";
                }
                else if (Convert.IsDBNull(parameter_value))
                {
                    result = "NULL";
                }
                else
                {
                    var literal_value = parameter_value.ToString().Replace("'", "''");
                    switch (parameter_type)
                    {
                        case System.Data.SqlDbType.Char:
                        case System.Data.SqlDbType.NChar:
                        case System.Data.SqlDbType.NText:
                        case System.Data.SqlDbType.NVarChar:
                        case System.Data.SqlDbType.Text:
                        case System.Data.SqlDbType.VarChar:
                        case System.Data.SqlDbType.Xml:
                        case System.Data.SqlDbType.DateTimeOffset:
                        case System.Data.SqlDbType.UniqueIdentifier:
                            result = "'" + literal_value + "'";
                            break;

                        case System.Data.SqlDbType.Time:
                        case System.Data.SqlDbType.Date:
                        case System.Data.SqlDbType.DateTime:
                        case System.Data.SqlDbType.DateTime2:
                            TimeSpan time;
                            DateTime datetime;
                            string datetime_literal_value = literal_value;
                            if (TimeSpan.TryParse(literal_value, out time))
                            {
                                datetime_literal_value = time.ToString();
                            }
                            else if (DateTime.TryParse(literal_value, out datetime))
                            {
                                datetime_literal_value = datetime.ToString("yyyy/MM/dd HH:mm:ss.fff");
                            }
                            result = "'" + datetime_literal_value + "'";
                            break;

                        case System.Data.SqlDbType.Image:
                        case System.Data.SqlDbType.Structured:
                        case System.Data.SqlDbType.Udt:
                        case System.Data.SqlDbType.Variant:
                            throw new Exception($"Not supported SqlDbType: {parameter_type}");

                        default:
                            result = literal_value;
                            break;
                    }
                }
                return result;
            }
        }
        internal interface OperationCallPreparation
        {
            string OperationName { get; }
            void SetInboundValues<T>(T command);
        }
        internal class SqlCommandPreparation : OperationCallPreparation
        {
            private RPC_As_AbstractDataType calltype_instance;
            public SqlCommandPreparation(RPC_As_AbstractDataType calltype_instance)
            {
                this.calltype_instance = calltype_instance;
            }
            public string OperationName => calltype_instance.GetOperationName();
            public void SetInboundValues<T>(T operation)
            {
                SqlCommand command = operation as SqlCommand;
                if (command == null)
                {
                    throw new ArgumentException($"{nameof(SetInboundValues)} can only work with a parameter of {nameof(SqlCommand)} type.");
                }
                foreach (var inbound_argument in calltype_instance.GetArguments())
                {
                    command.Parameters[$"@{inbound_argument.Name}"].Value = inbound_argument.Value;
                }
            }
        }


        #region class spiked
        public class SPCallByXMLTarget : SqlWriterAgent.ITargetHostedProcessor //TODO would be better as different modes of SP execution from the same Processors instead of separated Processor classes?
        {
            private string connection_string;

            #region SqlWriterAgent.ITargetHostedProcessor
            public System.Threading.CancellationToken Cancellation { get; set; }
            public IDictionary<string, object> Configuration { get; set; }
            public ITargetProcessorHost Host { get; set; }
            public string Name
            {
                get
                {
                    var type = GetType();
                    return $"{type.FullName} v{type.Assembly.GetName().Version}";
                }
            }

            public void Setup()
            {
                connection_string = System.Configuration.ConfigurationManager.ConnectionStrings[Configuration[ConfigurationKey.ConnectionStringKey].ToString()].ConnectionString;
                Host.Information($"{nameof(SPCallByXMLTarget)}.Setup ConnectionString = {connection_string}");
                CheckDBConnection();
            }

            public void Start()
            {
            }

            public void Start(string initialization_message)
            {
                Host.Information($"{nameof(SPCallByXMLTarget)}.Start({initialization_message})");
            }

            public void Start(IDictionary<string, object> initialization_message)
            {
                Host.Information($"{nameof(SPCallByXMLTarget)}.Start({initialization_message})");
            }

            public void Stop()
            {
                Host.Information($"{nameof(SPCallByXMLTarget)}.Stop()");
            }

            public void ProcessMessage(IDictionary<string, object> message)
            {
                ExecutePayloadAsTSQL(message);
            }
            #endregion

            private void CheckDBConnection()
            {
                using (var con = new SqlConnection(connection_string))
                {
                    con.Open();
                }
            }
            private void ExecutePayloadAsTSQL(IDictionary<string, object> message)
            {
                try
                {
                    if (!message.ContainsKey(Constant.SolaceMessagePayloadKey))
                    {
                        throw new Exception($"{nameof(Constant.SolaceMessagePayloadKey)} key with the payload is missing in the incoming message.");
                    }
                    ExecuteTSQL(message[Constant.SolaceMessagePayloadKey].ToString());
                }
                catch (Exception ex)
                {
                    Host.Error(ex, "");
                }
            }

            private void ExecuteTSQL(string tsql)
            {
                using (var con = new SqlConnection(connection_string))
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = tsql;
                    cmd.CommandType = System.Data.CommandType.Text;
                    con.Open();
                    int result = cmd.ExecuteNonQuery();
                    Host.Information($"T-SQL Execute: {tsql} |Result: {result}");

                }
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // dispose managed state (managed objects).
                    }

                    // free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // set large fields to null.

                    disposedValue = true;
                }
            }

            // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~SPCallByXMLTarget() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion
        }
        #endregion
    }
        --EfectivoTSQLGenerationSpec.cs
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Xml.Linq;
    using System.Collections.Generic;
    using System.Xml;
    using System.Linq;
    using System.Text;
    using System.Xml.Schema;
    using System.Diagnostics;
    using System.IO;
    using System.Data.SqlClient;

    namespace SqlWriterSpecs
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

                var xml_text5 = "<CapitalMarket.uspLoadCapitalMarketCashFixWriter xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'><SideID>1</SideID><TradeID>12</TradeID><TradeOrderID>2164110050017</TradeOrderID><TransactionTypeID>1967</TransactionTypeID><CustodyTypeID>2</CustodyTypeID><ClientLegacyContractID>153641</ClientLegacyContractID><ClientSubAccountID>609</ClientSubAccountID><ClientPortfolioID>39415</ClientPortfolioID><AmountBeforeCommissionAndTax>26001.000000</AmountBeforeCommissionAndTax><CommissionAmount>1950.075000</CommissionAmount><TaxAmount>312.012000</TaxAmount><AllocationAndSettlementDay>4</AllocationAndSettlementDay><MarketID>8427</MarketID><MarketPartyID>GBM     </MarketPartyID></CapitalMarket.uspLoadCapitalMarketCashFixWriter>";
                var xsd5 = InferXMLDataType(xml_text5);

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
            static string GetStoredProcedureCall(utility.ITypeClassMapper typemap)
            {
                var builder = typemap.GetService<IOperationCallBuilder>();
                return $"{builder.GetRunStatement()} {builder.GetOperationName()} {builder.GetArguments()}";
            }
            class OperationCallBuilder : IOperationCallBuilder
            {
                private XDocument xml;
                private SqlTargetStoredProcedureExecutor.IParameterMetadataProvider metadata;
                public OperationCallBuilder(utility.ITypeClassMapper typemap)
                {
                    xml = typemap.GetService<XDocument>();
                    metadata = typemap.GetService<SqlTargetStoredProcedureExecutor.IParameterMetadataProvider>();
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
            class ParameterMetadataProvider : SqlTargetStoredProcedureExecutor.IParameterMetadataProvider
            {
                private utility.ITypeClassMapper typemap;
                private IDictionary<string, Type> Schema;
                public ParameterMetadataProvider(utility.ITypeClassMapper typemap)
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
                var typemap = new utility.TypeClassMapper
                (
                  new Dictionary<Type, Type>
                  {
                      { typeof(IOperationCallBuilder), typeof(OperationCallBuilder) },
                      { typeof(SqlTargetStoredProcedureExecutor.IParameterMetadataProvider), typeof(ParameterMetadataProvider) }
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
                var typemap = new utility.TypeClassMapper
                (
                  new Dictionary<Type, Type>
                  {
                      { typeof(IOperationCallBuilder), typeof(OperationCallBuilder) },
                      { typeof(SqlTargetStoredProcedureExecutor.IParameterMetadataProvider), typeof(ParameterMetadataProvider) }
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

            [TestMethod]
            public void a_xml_call_5()
            {
                //Arrange
                var schema = new Dictionary<string, Type> { { "par2", typeof(decimal) } };
                var typemap = new utility.TypeClassMapper
                (
                  new Dictionary<Type, Type>
                  {
                      { typeof(SqlTargetStoredProcedureExecutor.RPC_DataType_To_OperationExecution), typeof(SqlTargetStoredProcedureExecutor.MessageToStoredProcedureCall) },
                      { typeof(SqlTargetStoredProcedureExecutor.RPC_As_AbstractDataType), typeof(SqlTargetStoredProcedureExecutor.TSQLCallAsXmlMessage) },
                      { typeof(SqlTargetStoredProcedureExecutor.IParameterMetadataProvider), typeof(ParameterMetadataProvider) }
                  },
                  new Dictionary<Type, object>
                  {
                      { typeof(IDictionary<string, Type>), schema }
                  }
                );

                //Act
                var xml = XDocument.Parse("<sp1><par1>val1</par1><par2>123.45</par2></sp1>");
                typemap.SetValue<XDocument>(SqlTargetStoredProcedureExecutor.RPC_Constant.SerializedRPCDataType, xml);
                var inbound = typemap.GetService<SqlTargetStoredProcedureExecutor.RPC_As_AbstractDataType>();

                var transform = typemap.GetService<SqlTargetStoredProcedureExecutor.RPC_DataType_To_OperationExecution>();
                var execution = transform.GetOperationExecution(inbound);

                //Assert
                Assert.AreEqual<string>("EXECUTE sp1 @par1 = 'val1', @par2 = 123.45", execution);
            }

            [TestMethod]
            public void a_xml_call_6()
            {
                //Arrange
                var schema = new Dictionary<string, Type> { { "par2", typeof(decimal) } };
                var typemap = new utility.TypeClassMapper
                (
                  new Dictionary<Type, Type>
                  {
                      { typeof(SqlTargetStoredProcedureExecutor.RPC_DataType_To_OperationExecution), typeof(SqlTargetStoredProcedureExecutor.MessageToStoredProcedureCall) },
                      { typeof(SqlTargetStoredProcedureExecutor.RPC_As_AbstractDataType), typeof(SqlTargetStoredProcedureExecutor.TSQLCallAsXmlMessage) },
                      { typeof(SqlTargetStoredProcedureExecutor.IParameterMetadataProvider), typeof(ParameterMetadataProvider) }
                  },
                  new Dictionary<Type, object>
                  {
                      { typeof(IDictionary<string, Type>), schema }
                  }
                );

                //Act
                //Call 1
                var xml = XDocument.Parse("<sp1><par1>val1</par1><par2>123.45</par2></sp1>");
                typemap.SetValue<XDocument>(SqlTargetStoredProcedureExecutor.RPC_Constant.SerializedRPCDataType, xml);
                var inbound1 = typemap.GetService<SqlTargetStoredProcedureExecutor.RPC_As_AbstractDataType>();

                var transform = typemap.GetService<SqlTargetStoredProcedureExecutor.RPC_DataType_To_OperationExecution>();
                var execution1 = transform.GetOperationExecution(inbound1);

                //Call 2
                var xml2 = XDocument.Parse("<sp2><par1>val1</par1><par2>123.45</par2></sp2>");
                typemap.SetValue<XDocument>(SqlTargetStoredProcedureExecutor.RPC_Constant.SerializedRPCDataType, xml2);
                var inbound2 = typemap.GetService<SqlTargetStoredProcedureExecutor.RPC_As_AbstractDataType>();

                transform = typemap.GetService<SqlTargetStoredProcedureExecutor.RPC_DataType_To_OperationExecution>();
                var execution2 = transform.GetOperationExecution(inbound2);

                //Assert
                Assert.AreEqual<string>("EXECUTE sp1 @par1 = 'val1', @par2 = 123.45", execution1);
                Assert.AreEqual<string>("EXECUTE sp2 @par1 = 'val1', @par2 = 123.45", execution2);
            }
            #endregion

            [TestMethod]
            public void sit_xml_call_5()
            {
                //Arrange
                var typemap = new utility.TypeClassMapper
                (
                  new Dictionary<Type, Type>
                  {
                      { typeof(SqlTargetStoredProcedureExecutor.RPC_DataType_To_OperationExecution), typeof(SqlTargetStoredProcedureExecutor.MessageToStoredProcedureCall) },
                      { typeof(SqlTargetStoredProcedureExecutor.RPC_As_AbstractDataType), typeof(SqlTargetStoredProcedureExecutor.TSQLCallAsXmlMessage) },
                      { typeof(SqlTargetStoredProcedureExecutor.IParameterMetadataProvider), typeof(SqlTargetStoredProcedureExecutor.ParameterMetadataProvider) }
                  }
                );
                var connection_string = "Data Source=GBMDESA01;Initial Catalog=NoeticODS_Dev;Integrated Security=SSPI;Persist Security Info=False";
                typemap.SetValue<string>(nameof(connection_string), connection_string);

                //Act
                var xml = XDocument.Parse("<CapitalMarket.uspLoadCapitalMarketCashFixWriter xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'><SideID>2</SideID><TradeID>3</TradeID><TradeOrderID>2164110050002</TradeOrderID><TransactionTypeID>2017</TransactionTypeID><CustodyTypeID>2</CustodyTypeID><ClientLegacyContractID>148448</ClientLegacyContractID><ClientSubAccountID>45074</ClientSubAccountID><ClientPortfolioID>39415</ClientPortfolioID><AmountBeforeCommissionAndTax>24764.000000</AmountBeforeCommissionAndTax><CommissionAmount>1857.300000</CommissionAmount><TaxAmount>297.168000</TaxAmount><AllocationAndSettlementDay>4</AllocationAndSettlementDay><MarketID>8427</MarketID><MarketPartyID>GBM     </MarketPartyID></CapitalMarket.uspLoadCapitalMarketCashFixWriter>");
                typemap.SetValue<XDocument>(SqlTargetStoredProcedureExecutor.RPC_Constant.SerializedRPCDataType, xml);
                var inbound = typemap.GetService<SqlTargetStoredProcedureExecutor.RPC_As_AbstractDataType>();

                var transform = typemap.GetService<SqlTargetStoredProcedureExecutor.RPC_DataType_To_OperationExecution>();
                var execution = transform.GetOperationExecution(inbound);

                //Assert
                Assert.AreEqual<string>("EXECUTE CapitalMarket.uspLoadCapitalMarketCashFixWriter @SideID = 2, @TradeID = 3, @TradeOrderID = 2164110050002, @TransactionTypeID = 2017, @CustodyTypeID = 2, @ClientLegacyContractID = '148448', @ClientSubAccountID = 45074, @ClientPortfolioID = 39415, @AmountBeforeCommissionAndTax = 24764.000000, @CommissionAmount = 1857.300000, @TaxAmount = 297.168000, @AllocationAndSettlementDay = 4, @MarketID = 8427, @MarketPartyID = 'GBM     '", execution);
            }
            [TestMethod, Ignore]//, Ignore
            public void dev_xml_call_5_a()
            {
                //Arrange
                var typemap = new utility.TypeClassMapper
                (
                  new Dictionary<Type, Type>
                  {
                      { typeof(SqlTargetStoredProcedureExecutor.RPC_DataType_To_OperationExecution), typeof(SqlTargetStoredProcedureExecutor.MessageToStoredProcedureCall) },
                      { typeof(SqlTargetStoredProcedureExecutor.RPC_As_AbstractDataType), typeof(SqlTargetStoredProcedureExecutor.TSQLCallAsXmlMessage) },
                      { typeof(SqlTargetStoredProcedureExecutor.IParameterMetadataProvider), typeof(SqlTargetStoredProcedureExecutor.ParameterMetadataProvider) }
                  }
                );

                var connection_string = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=ODSDB1;Data Source=mx-cb-mdorantes";
                typemap.SetValue<string>(nameof(connection_string), connection_string);

                //Act
                var xml = XDocument.Parse("<sp1 xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'><par1>1</par1><par2>2</par2><par4>4</par4><par5>5</par5><par6>6</par6><par8>8</par8><par9>9</par9><par11>11</par11><par12>12</par12></sp1>");
                typemap.SetValue<XDocument>(SqlTargetStoredProcedureExecutor.RPC_Constant.SerializedRPCDataType, xml);
                var inbound = typemap.GetService<SqlTargetStoredProcedureExecutor.RPC_As_AbstractDataType>();

                var transform = typemap.GetService<SqlTargetStoredProcedureExecutor.RPC_DataType_To_OperationExecution>();
                var execution = transform.GetOperationExecution(inbound);

                //Assert
                Assert.AreEqual<string>("EXECUTE sp1 @par1 = 1, @par2 = 2, @par4 = 4, @par5 = 5, @par6 = 6, @par8 = 8, @par9 = 9, @par11 = '11', @par12 = 12", execution);
            }
            [TestMethod]
            public void sit_xml_call_5_b()
            {
                //Arrange
                var typemap = new utility.TypeClassMapper
                (
                  new Dictionary<Type, Type>
                  {
                      { typeof(SqlTargetStoredProcedureExecutor.RPC_DataType_To_OperationExecution), typeof(SqlTargetStoredProcedureExecutor.MessageToStoredProcedureCall) },
                      { typeof(SqlTargetStoredProcedureExecutor.RPC_As_AbstractDataType), typeof(SqlTargetStoredProcedureExecutor.SqlCommandAsXmlMessage ) }
                  }
                );

                var connection_string = "Data Source=GBMDESA01;Initial Catalog=NoeticODS_Dev;Integrated Security=SSPI;Persist Security Info=False";
                typemap.SetValue<string>(nameof(connection_string), connection_string);
                using (var con = new SqlConnection(connection_string))
                using (var cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = "CapitalMarket.uspLoadCapitalMarketCashFixWriter";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlCommandBuilder.DeriveParameters(cmd);

                    //Act
                    var xml = XDocument.Parse("<CapitalMarket.uspLoadCapitalMarketCashFixWriter xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'><SideID>2</SideID><TradeID>3</TradeID><TradeOrderID>2164110050002</TradeOrderID><TransactionTypeID>2017</TransactionTypeID><CustodyTypeID>2</CustodyTypeID><ClientLegacyContractID>148448</ClientLegacyContractID><ClientSubAccountID>45074</ClientSubAccountID><ClientPortfolioID>39415</ClientPortfolioID><AmountBeforeCommissionAndTax>24764.000000</AmountBeforeCommissionAndTax><CommissionAmount>1857.300000</CommissionAmount><TaxAmount>297.168000</TaxAmount><AllocationAndSettlementDay>4</AllocationAndSettlementDay><MarketID>8427</MarketID><MarketPartyID>GBM     </MarketPartyID></CapitalMarket.uspLoadCapitalMarketCashFixWriter>");
                    typemap.SetValue<XDocument>(SqlTargetStoredProcedureExecutor.RPC_Constant.SerializedRPCDataType, xml);
                    var inbound = typemap.GetService<SqlTargetStoredProcedureExecutor.RPC_As_AbstractDataType>();

                    var transform = typemap.GetService<SqlTargetStoredProcedureExecutor.RPC_DataType_To_OperationExecution>();
                    var execution = transform.GetOperationExecutionObject<SqlTargetStoredProcedureExecutor.OperationCallPreparation>(inbound);
                    execution.SetInboundValues(cmd);

                    //Assert
                    Assert.IsNotNull(execution);
                    Assert.AreEqual<string>(cmd.CommandText, execution.OperationName);
                    Assert.AreEqual<int>(15, cmd.Parameters.Count);
                    Assert.AreEqual("2", cmd.Parameters["@SideID"].Value);
                    Assert.AreEqual("3", cmd.Parameters["@TradeID"].Value);
                    //...
                    Assert.AreEqual("8427", cmd.Parameters["@MarketID"].Value);
                    Assert.AreEqual("GBM     ", cmd.Parameters["@MarketPartyID"].Value);
                }
            }
        }
    }
    */

    #endregion
  }
}