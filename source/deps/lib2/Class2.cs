namespace lib2;

public class Class2 : lib1.Class1
{
    public string GetVersion() => $"{GetType().Assembly.GetName().Version}";
}