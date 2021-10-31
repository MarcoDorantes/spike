using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public partial class ToXMLSpec
{
    [TestMethod]
    public void from_maplist1()
    {
        var yamltext = @"---
a: 
 id: 1
 keys:
  - A
  - B
...";
        var map = yaml.deserial(yamltext);
        var expected_xmltext =
@"<yaml>
  <map>
    <entry>
      <key>a</key>
      <value>
        <map>
          <entry>
            <key>id</key>
            <value>1</value>
          </entry>
          <entry>
            <key>keys</key>
            <value>
              <list>
                <entry>A</entry>
                <entry>B</entry>
              </list>
            </value>
          </entry>
        </map>
      </value>
    </entry>
  </map>
</yaml>";
        var xml = XDocument.Parse(yaml.AsXml_1dot0(map));
        Assert.AreEqual("yaml", xml.Root.Name);
        Assert.AreEqual(expected_xmltext, $"{xml}");
    }
}