using System;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;

public static class yaml
{
    #region Deserialization
    public static object deserial(string yaml) => deserial(new StringReader(yaml));
    public static object deserial(TextReader r)
    {
        var d = new YamlDotNet.Serialization.Deserializer(); //Provided <PackageReference Include="YamlDotNet" Version="9.1.0" />
        return d.Deserialize(r);
    }
    #endregion

    #region to XML
    public static string AsXml_1dot0(object graph)
    {
        var xml = parse(graph);

        var root = new XDocument(new XElement("yaml"));
        root.Root.Add(xml);
        return $"{root}";
    }
    public static object parse(object x)
    {
        object result = default;
        switch (x)
        {
            case null: result = null; break;
            case string s: result = new XElement("value", s); break;
            case IList<object> _l: result = parse_list(_l); break;
            case IDictionary<object, object> d: result = parse(d); break;
            default: throw new Exception($"*** unsupported default ({x},{x?.GetType().FullName}) ***");
        }
        return result;
    }
    public static object parse(IDictionary<object, object> _m)
    {
        var result = new XElement("map");
        foreach (var pair in _m)
        {
            var entry = new XElement("entry");
            result.Add(entry);
            switch (pair.Key)
            {
                case string s: entry.Add(new XElement("key", s)); break;
                default: throw new Exception($"*** unsupported key default {pair.Key} : {pair.Key.GetType().FullName} ***");
            }
            switch (pair.Value)
            {
                case null: entry.Add(new XElement("value")); break;
                case string s: entry.Add(new XElement("value", s)); break;
                case IList<object> _l: entry.Add(new XElement("value", parse_list(_l))); break;
                case IDictionary<object, object> _c: entry.Add(new XElement("value", parse(_c))); break;
                default: throw new Exception($"*** unsupported default ({pair.Value},{pair.Value?.GetType().FullName}) ***");
            }
        }
        return result;
    }
    public static object parse_list(IList<object> _L)
    {
        var result = new XElement("list");
        foreach (var x in _L)
        {
            switch (x)
            {
                case null: result.Add(new XElement("entry")); break;
                case string s: result.Add(new XElement("entry", s)); break;
                case IList<object> _l: result.Add(new XElement("entry", parse_list(_l))); break;
                case IDictionary<object, object> _c: result.Add(new XElement("entry", parse(_c))); break;
                default: throw new Exception($"*** unsupported default ({x},{x?.GetType().FullName}) ***");
            }
        }
        return result;
    }
    #endregion
}