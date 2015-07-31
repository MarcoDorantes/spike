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
            WriteLine("X: {0} {1} | {2}",x.Name, x.Age, x);
        }
    }
}