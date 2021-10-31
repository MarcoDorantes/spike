class Host
{
    static void Main(string[] args) => System.Console.WriteLine(yaml.AsXml_1dot0(yaml.deserial(System.IO.File.ReadAllText(args[0]))));
}