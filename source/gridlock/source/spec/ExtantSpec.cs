namespace spec;

using static System.Console;

/*
https://www.spinmaster.com/en-US/brands/rubiks/rubik-s-cube-gridlock

dotnet test .\source\gridlock\source\spec\ -c Release --logger "console;verbosity=detailed"
*/

[TestClass]
public sealed class ExtantSpec
{
    void display(char[,] g)
    {
        char[][] _ = new char[1][];
        WriteLine(_.GetType().FullName);//System.Char[][]

        WriteLine(g.GetType().FullName);//System.Char[,]
        WriteLine(g[0,0]);
    }
    [TestMethod]
    public void grid1()
    {
        char[,] grid = new char[8,8];
        display(grid);
        Assert.AreEqual(0,grid[0,0]);
    }
}