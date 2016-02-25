using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using System.IO;

namespace UnitTestProject1
{
  [TestClass]
  public class msbuildXMLspec
  {
    [TestMethod]
    public void proj_compiletime_dependencies()
    {
      //Arrange
      XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";
      var proj_xml = @"<Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
<ItemGroup>
  <Reference Include='System' />
  <Reference Include='System.Data' />
  <Reference Include='Newtonsoft.Json, Version=4.5.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed, processorArchitecture=MSIL'>
    <SpecificVersion>False</SpecificVersion>
    <HintPath>..\..\Lib\Json.NET\Net40\Newtonsoft.Json.dll</HintPath>
  </Reference>
</ItemGroup>
<ItemGroup>
  <ProjectReference Include='..\..\Services\Nasdaq.IBM.ServicesLib.csproj'>
    <Project>{AB38AF66-37C4-4FE8-B44E-DC26849131A6}</Project>
    <Name>Nasdaq.IBM.ServicesLib</Name>
  </ProjectReference>
    <ProjectReference Include='..\Parser\Parser.csproj'>
      <Project>{8D940AEC-ECFD-49D0-AF41-29D048A9751C}</Project>
      <Name>Parser</Name>
    </ProjectReference>
</ItemGroup>
  <ItemGroup>
    <Content Include='..\..\..\..\Solution1\Main\Lib\Nasdaq 1.2\win64\libclient_64.dll'>
      <Link>libclient_64.dll</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
    <Content Include='..\..\Lib\Microsoft.Exchange.WebServices.dll'>
      <Link>Microsoft.Exchange.WebServices.dll</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>
</Project>";
      var textReader = new StringReader(proj_xml);

      //Ack
      var doc = XDocument.Load(textReader);

      //Assert
      Assert.IsNotNull(doc);
      Assert.AreEqual<string>("Project", doc.Root.Name.LocalName);
      Assert.AreEqual<int>(3, doc.Root.Descendants(ns + "Reference").Count());
      Assert.AreEqual<int>(2, doc.Root.Descendants(ns + "ProjectReference").Count());
      Assert.AreEqual<int>(2, doc.Root.Descendants(ns + "Content").Count());
    }
  }
}