using static System.Console;

namespace ConsoleApplication1
{
    class X
    {
        public X()
        {
            Name = "uno";
        }
        public string Name { get; }
        public int Age => Name.Length;
        public override string ToString() => $"({Name},{Age})";
    }
    class Program
    {
        static void Main(string[] args)
        {
            var x = new X();
            WriteLine($"X: {x.Name} {x.Age} | {x}");
            WriteLine("{0} {1}",nameof(x),nameof(X));
            X y = null;
            if (y?.Name?.Length > 0)
            {
                WriteLine("Name len is ok");
            }
        }
    }
}