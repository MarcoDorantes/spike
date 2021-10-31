using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public partial class ToXMLSpec
{
    [TestMethod]
    public void from_value1()
    {
        var yamltext = @"---
a
...";
        var map = yaml.deserial(yamltext);
        var expected_xmltext =
@"<yaml>
  <value>a</value>
</yaml>";
        var xml = XDocument.Parse(yaml.AsXml_1dot0(map));
        Assert.AreEqual("yaml", xml.Root.Name);
        Assert.AreEqual(expected_xmltext, $"{xml}");
    }
}