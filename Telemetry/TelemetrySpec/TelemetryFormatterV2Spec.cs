using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Collections.Generic;

namespace TelemetrySpec
{
  public interface ITelemetryFormatter
  {
    string Serialize(object x, string[] headers);
  }
  class Formatter1 : ITelemetryFormatter
  {
    public string Serialize(object x, string[] headers)
    {
      //var payload = new StringBuilder($"{message_type}");
      //for (int k = 0; k < values.Length; ++k)
      //{
      //  payload.AppendFormat("|{0}", values[k]);
      //}
      return "";// payload.ToString();
    }
  }
  public class TelemetryFormatterV2
  {
    private Dictionary<string, ITelemetryFormatter> formatters;
    public TelemetryFormatterV2()
    {
      formatters = new Dictionary<string, ITelemetryFormatter>();
      formatters.Add("S", new Formatter1());
    }
    public string GetStateNotice(string message_type)
    {
      //get headers=f(data_to_send)
      //formatter_key=f(headers)
      //formatter=formatters(formatter_key)
      //return formatter.Serialize(data_to_send)

      //string formatter_key = get_key(headers);
      //ITelemetryFormatter formatter = formatters[formatter_key];
      return "";// formatter.Serialize(payload, headers);
    }

    private string get_key(string[] headers)
    {
      return headers.Aggregate(new StringBuilder(), (whole, next) => whole.AppendFormat("{0}", next)).ToString();
    }
  }

  #region configs
  public class WnConfigurationSection : System.Configuration.ConfigurationSection
  {
    [System.Configuration.ConfigurationProperty("Parts", IsRequired = true)]
    [System.Configuration.ConfigurationCollection(typeof(PartCollection), AddItemName = "Part")]
    public PartCollection Parts { get { return (PartCollection)this["Parts"]; } }
  }

  public class PartCollection : System.Configuration.ConfigurationElementCollection
  {
    protected override System.Configuration.ConfigurationElement CreateNewElement()
    {
      return new PartCollectionElement();
    }
    protected override object GetElementKey(System.Configuration.ConfigurationElement element)
    {
      return ((PartCollectionElement)element).Name;
    }
  }

  public class PartCollectionElement : System.Configuration.ConfigurationElement
  {
    public const string DefaultProviderName = "Provider0";
    public const string DefaultProviderType = "ProviderAssembly.Provider0, ProviderAssembly";

    [System.Configuration.ConfigurationProperty("Name", IsRequired = true, DefaultValue = DefaultProviderName)]
    public string Name { get { return (string)this["Name"]; } set { this["Name"] = value; } }

    [System.Configuration.ConfigurationProperty("Provider", IsRequired = true, DefaultValue = DefaultProviderType)]
    public string Provider { get { return (string)this["Provider"]; } }
  }

  #endregion

  [TestClass]
  public class TelemetryFormatterV2Spec
  {
    [TestMethod]
    public void NotifySharedState()
    {
      //Arrange
      string shared_state1 = "shared_state1";
      var id = new Identity { ID = Guid.NewGuid().ToString(), Host = Environment.MachineName, Service = nameof(TelemetryFormatterV2), Name = nameof(NotifySharedState), SourceName = nameof(NotifySharedState), TargetName = nameof(Assert), State = "Arranged" };
      string expected_payload =
        string.Format("{0}\x0{1}\x0{2}\x0{3}\x0{4}\x0{5}\x0{6}\x0{7}",
          NotificationType.Status,
          id.ID,
          id.Host,
          id.Service,
          id.Name,
          id.SourceName,
          id.TargetName,
          shared_state1 == null ? id.State : shared_state1
        );

      var formatter = new TelemetryFormatterV2();

      //Act
      string payload = "";//formatter.GetStateNotice(NotificationType.Status, id.ID, id.Host, id.Service, id.Name, id.SourceName, id.TargetName, shared_state1);

      //Assert
      Assert.AreEqual<string>(expected_payload, payload);
    }

    [TestMethod]
    public void ConfigDefaults()
    {
      //Arrange
      WnConfigurationSection existing_section;

      //Act
      existing_section = System.Configuration.ConfigurationManager.GetSection("WnConfigurationSection") as WnConfigurationSection;
      var parts = existing_section.Parts.OfType<PartCollectionElement>();
      var non_existing_section = new WnConfigurationSection();
      var non_existing_part = new PartCollectionElement();

      //Assert
      Assert.IsNotNull(existing_section);
      Assert.IsNotNull(existing_section.Parts);
      Assert.AreEqual<int>(2, existing_section.Parts.Count);
      Assert.AreEqual<int>(2, parts.Count());
      Assert.AreEqual<string>("name1", parts.ElementAt(0).Name);
      Assert.AreEqual<string>("ProviderAssembly.Provider1, ProviderAssembly", parts.ElementAt(0).Provider);
      Assert.AreEqual<string>("name2", parts.ElementAt(1).Name);
      Assert.AreEqual<string>("Provider2, Provider2Assembly", parts.ElementAt(1).Provider);

      Assert.IsNotNull(non_existing_section);
      Assert.IsFalse(non_existing_section.ElementInformation.IsCollection);
      Assert.AreEqual<Type>(typeof(WnConfigurationSection), non_existing_section.ElementInformation.Type);

      Assert.IsNotNull(non_existing_part);
      Assert.AreEqual<string>(PartCollectionElement.DefaultProviderName, non_existing_part.Name);
      Assert.AreEqual<string>(PartCollectionElement.DefaultProviderType, non_existing_part.Provider);
    }

    IEnumerable<PartCollectionElement> GetParts()
    {
      WnConfigurationSection existing_section = System.Configuration.ConfigurationManager.GetSection("GettingPartsGroup/WnConfigurationSection") as WnConfigurationSection;
      if (existing_section != null)
      {
        foreach (var part in existing_section.Parts.OfType<PartCollectionElement>())
        {
          yield return part;
        }
      }
      else
      {
        yield return new PartCollectionElement();
      }
    }

    [TestMethod]
    public void GettingParts()
    {
      //Arrange
      IEnumerable<PartCollectionElement> parts;

      //Act
      parts = GetParts();

      //Assert
      Assert.IsNotNull(parts);
      switch (parts.Count())
      {
        case 1:
          Assert.AreEqual<string>(PartCollectionElement.DefaultProviderName, parts.ElementAt(0).Name);
          Assert.AreEqual<string>(PartCollectionElement.DefaultProviderType, parts.ElementAt(0).Provider);
          break;
        case 3:
          Assert.AreEqual<string>("name1", parts.ElementAt(0).Name);
          Assert.AreEqual<string>("ProviderAssembly.Provider1, ProviderAssembly", parts.ElementAt(0).Provider);
          Assert.AreEqual<string>("name2", parts.ElementAt(1).Name);
          Assert.AreEqual<string>("Provider2, Provider2Assembly", parts.ElementAt(1).Provider);
          Assert.AreEqual<string>("name3", parts.ElementAt(2).Name);
          Assert.AreEqual<string>("ProviderAssembly2.Provider3, ProviderAssembly2", parts.ElementAt(2).Provider);
          break;
        default:
          Assert.Fail("parts.Count={0}", parts.Count());
          break;
      }
    }
  }
}