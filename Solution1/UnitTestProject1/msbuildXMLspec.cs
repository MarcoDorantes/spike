﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

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

    [TestMethod]
    public void project_file()
    {
      //Arrange
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
      var proj = new MSBuildProjectFile($"{nameof(proj_xml)}", textReader);

      //Assert
      Assert.AreEqual<int>(7, proj.All().Count());
    }

    [TestMethod]
    public void emptyItemGroups()
    {
      //Arrange
      var proj_xml = @"<Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
<ItemGroup>
</ItemGroup>
<ItemGroup>
</ItemGroup>
<ItemGroup>
</ItemGroup>
</Project>";
      var textReader = new StringReader(proj_xml);

      //Ack
      var proj = new MSBuildProjectFile($"{nameof(proj_xml)}", textReader);

      //Assert
      Assert.AreEqual<int>(0, proj.All().Count());
    }

    [TestMethod]
    public void isInvalidXml()
    {
      var file = @"UnitTestProject1.dll";
      var reader=File.OpenText(file);
      try
      {
        var doc = XDocument.Load(reader);
      }
      catch (System.Xml.XmlException ex)
      {
        Assert.IsTrue(ex.Message.StartsWith("Data at the root level is invalid"));
      }
    }

    [TestMethod]
    public void to_assembly()
    {
      //Arrange
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
      var msbuild = new MSBuildProjectFile($"{nameof(proj_xml)}", textReader);

      //Ack
      bool ok = msbuild.IsWellFormedXML;
      string a_proj_ref = msbuild.All().First(d => d is MSBuildProjectFile.ProjectReference).Name;
      string a_ref = msbuild.All().First(d => d is MSBuildProjectFile.Reference).Name;
      string a_content = msbuild.All().First(d => d is MSBuildProjectFile.Content).Name;
      string last_ref = msbuild.All().Last(d => d is MSBuildProjectFile.Reference).Name;
      Assembly assembly = msbuild.GetProxyAssembly();

      Assert.IsTrue(ok);
      Assert.AreEqual<string>("Nasdaq.IBM.ServicesLib.csproj", a_proj_ref);
      Assert.AreEqual<string>("System", a_ref);
      Assert.AreEqual<string>("libclient_64", a_content);
      Assert.AreEqual<string>("Newtonsoft.Json", last_ref);
      Assert.IsNotNull(assembly);
    }

    [TestMethod]
    public void discriminated_dependencies()
    {
      //Arrange
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
      var msbuild = new MSBuildProjectFile($"{nameof(proj_xml)}", textReader);

      //Ack
      bool ok = msbuild.IsWellFormedXML;
      IEnumerable<MSBuildProjectFile.ProjectReference> proj_refs = msbuild.ProjectReferences();
      IEnumerable<MSBuildProjectFile.Reference> refs = msbuild.References();
      IEnumerable<MSBuildProjectFile.Content> contents = msbuild.Contents();

      Assert.IsTrue(ok);
      Assert.AreEqual<int>(2, proj_refs.Count());
      Assert.AreEqual<int>(3, refs.Count());
      Assert.AreEqual<int>(2, contents.Count());
    }
    public class MSBuildProjectFile
    {
      #region Nested classes
      public class Dependency
      {
        protected XElement xml;
        public Dependency(XElement source)
        {
          xml = source;
        }
        public virtual string Name { get { return xml.Name.LocalName; } }
      }
      public class Reference : Dependency
      {
        public Reference(XElement source) : base(source) { }
        public override string Name
        {
          get
          {
            var include = xml.Attribute("Include").Value;//TODO input validation
            int index = include.IndexOf(',');
            return index > 0 ? include.Substring(0, index) : include;//TODO option switch to Display Name instead of Simple Name
          }
        }
      }
      public class ProjectReference : Dependency
      {
        public ProjectReference(XElement source) : base(source) { }
        public override string Name { get { return Path.GetFileName(xml.Attribute("Include").Value); } }
      }
      public class Content : Dependency
      {
        public Content(XElement source) : base(source) { }
        public override string Name { get { return Path.GetFileNameWithoutExtension(xml.Attribute("Include").Value); } }
      }
      #endregion

      public static readonly XNamespace NS;

      static MSBuildProjectFile()
      {
        NS = "http://schemas.microsoft.com/developer/msbuild/2003";
      }

      private XDocument doc;
      private List<Dependency> efferent;
      public MSBuildProjectFile(string name, TextReader textReader)
      {
        Init(name, textReader);
      }

      public bool IsWellFormedXML { get; private set; }
      public string Name { get; private set; }

      public IEnumerable<Dependency> All()
      {
        IEnumerable<Dependency> result = efferent;
        return result;
      }
      public IEnumerable<MSBuildProjectFile.ProjectReference> ProjectReferences()
      {
        return efferent.OfType<MSBuildProjectFile.ProjectReference>();
      }
      public IEnumerable<MSBuildProjectFile.Reference> References()
      {
        return efferent.OfType<MSBuildProjectFile.Reference>();
      }
      public IEnumerable<MSBuildProjectFile.Content> Contents()
      {
        return efferent.OfType<MSBuildProjectFile.Content>();
      }

      public Assembly GetProxyAssembly()
      {
        if (!IsWellFormedXML)
        {
          throw new InvalidOperationException("The content is not well-formed XML.");
        }
        Assembly result = null;

        System.CodeDom.Compiler.CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
        var options = new System.CodeDom.Compiler.CompilerParameters();
        options.GenerateExecutable = false;
        options.IncludeDebugInformation = false;
        //foreach (string assemblyname in efferent.Select(e => e.Name))
        //{
        //  options.ReferencedAssemblies.Add(assemblyname);
        //}
        string GeneratedName = Name.Replace(' ', '_').Replace('.', '_');
        if (string.IsNullOrEmpty(GeneratedName))
        {
          options.GenerateInMemory = true;
        }
        else
        {
          options.GenerateInMemory = false;
          options.OutputAssembly = GeneratedName;
        }
        options.WarningLevel = 0;//0-4
        options.TreatWarningsAsErrors = false;
        options.CompilerOptions = "";
        options.TempFiles = new System.CodeDom.Compiler.TempFileCollection(".", false);
        string[] sources = new string[] { string.Format("namespace {0}_ns {{ public class {0}_type {{ }} }}", GeneratedName) };
        System.CodeDom.Compiler.CompilerResults results = provider.CompileAssemblyFromSource(options, sources);
        if (results.NativeCompilerReturnValue == 0)
        {
          result = results.CompiledAssembly;
        }
        else
        {
          var codelines = new System.Text.StringBuilder();
          foreach (System.CodeDom.Compiler.CompilerError err in results.Errors)
            codelines.AppendFormat("({0},{1}) {2}:{3} / ", err.Line, err.Column, err.ErrorNumber, err.ErrorText);
          throw new Exception(codelines.ToString());
        }
        return result;
      }

      private void Init(string name, TextReader textReader)
      {
        try
        {
          IsWellFormedXML = false;
          this.Name = name;
          doc = XDocument.Load(textReader);
          IsWellFormedXML = true;
          efferent = new List<Dependency>();
          doc.Root.Descendants(NS + "Reference").Aggregate(efferent, (whole, next) => { whole.Add(new Reference(next)); return whole; });
          doc.Root.Descendants(NS + "ProjectReference").Aggregate(efferent, (whole, next) => { whole.Add(new ProjectReference(next)); return whole; });
          doc.Root.Descendants(NS + "Content").Aggregate(efferent, (whole, next) => { whole.Add(new Content(next)); return whole; });
        }
        catch (System.Xml.XmlException)
        {
          //The specific supported use case will retry with another reader type different than XML.
        }
      }
    }
  }
}