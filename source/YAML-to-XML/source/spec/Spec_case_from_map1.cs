using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public partial class ToXMLSpec
{
    [TestMethod]
    public void from_map1()
    {
        //Arrange
        var map = yaml.deserial("hello: world");
        var expected_xmltext =
@"<yaml>
  <map>
    <entry>
      <key>hello</key>
      <value>world</value>
    </entry>
  </map>
</yaml>";

        //Act
        var xml = XDocument.Parse(yaml.AsXml_1dot0(map));
        
        //Assert
        Assert.AreEqual("yaml", xml.Root.Name);
        Assert.AreEqual(expected_xmltext, $"{xml}");
    }
}