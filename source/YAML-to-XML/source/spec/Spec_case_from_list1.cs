using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public partial class ToXMLSpec
{
    [TestMethod]
    public void from_list1()
    {
        //Arrange
        var yamltext = @"---
- a
- b
- c
...";
        var map = yaml.deserial(yamltext);
        var expected_xmltext =
@"<yaml>
  <list>
    <entry>a</entry>
    <entry>b</entry>
    <entry>c</entry>
  </list>
</yaml>";

        //Act
        var xml = XDocument.Parse(yaml.AsXml_1dot0(map));

        //Assert
        Assert.AreEqual("yaml", xml.Root.Name);
        Assert.AreEqual(expected_xmltext, $"{xml}");
    }
}