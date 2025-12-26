namespace spec;

using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

using static System.Console;

/*
https://www.spinmaster.com/en-US/brands/rubiks/rubik-s-cube-gridlock

dotnet test .\source\gridlock\source\spec\ -c Release --logger "console;verbosity=detailed"
*/

class Grid
{
    public const byte Width = 8;
    public const byte Height = 8;
    public const char DOT = '*'; //'·';//U+00B7 MIDDLE DOT

    private char[,] grid;

    public Grid()
    {
        grid = new char[Width, Height];
        for (byte k = 0; k < Width; ++k) for (byte j = 0; j < Height; ++j) grid[k, j] = DOT;
    }

    public void Add(int row, int column, char[] inner)
    {
        if (grid == null) throw new InvalidOperationException("Grid not initialized.");
        if (!(row < Height && column < Width)) throw new ArgumentOutOfRangeException($"Invalid size ({row},{column}).");
        for (int c = 0, col = column; col < Width && c < inner.Length; ++c, ++col)
        {
            grid[row, col] = inner[c];
        }
    }
    public void Add(int row, int column, char[,] inner)
    {
        if (grid == null) throw new InvalidOperationException("Grid not initialized.");
        if (inner == null) throw new ArgumentNullException(nameof(inner));
        if (!(row < Height && column < Width)) throw new ArgumentOutOfRangeException($"Invalid size ({row},{column}).");
        int source_height = inner.GetLength(0);
        int source_width = inner.GetLength(1);
        for (int source_row = 0, target_row = row; source_row < source_height && target_row < Height; ++source_row, ++target_row)
        {
            for (int source_col = 0, target_col = column; target_col < Width && source_col < source_width; ++source_col, ++target_col)
            {
                grid[target_row, target_col] = inner[source_row, source_col];
            }
        }
    }
    public void Display()
    {
        if (grid == null) throw new InvalidOperationException("Grid not initialized.");
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);
        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < cols; ++c)
            {
                Write($" {grid[r, c]}");
            }
            WriteLine();
        }
    }

    public static char[] Init(int length, char id)
    {
        var result = new char[length];
        for (int k = 0; k < length; ++k)
        {
            result[k] = id;
        }
        return result;
    }
    public static char[,] Init(int height, int width, char id)
    {
        var result = new char[height, width];
        for (int k = 0; k < height; ++k)
        {
            for (int j = 0; j < width; ++j)
            {
                result[k, j] = id;
            }
        }
        return result;
    }
}

[TestClass]
public sealed class ExtantSpec
{
    void display(char[,] g)
    {
        int rows = g.GetLength(0);
        int cols = g.GetLength(1);
        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < cols; ++c)
            {
                Write(g[r, c]);
            }
            WriteLine();
        }
    }

    [TestMethod]
    public void array_type()
    {
        char[] singledimensional = new char[8];
        char[,] multidimensional = new char[8, 8];
        char[][] jagged = new char[3][];
        int singledimensional_length = singledimensional.Length;
        int multidimensional_length = multidimensional.Length;
        int jagged_length = jagged.Length;

        Assert.AreEqual("System.Char[]", singledimensional.GetType().FullName);
        Assert.AreEqual("System.Char[,]", multidimensional.GetType().FullName);
        Assert.AreEqual("System.Char[][]", jagged.GetType().FullName);
        Assert.HasCount(8, singledimensional);
        Assert.AreEqual(8, singledimensional_length);
        Assert.HasCount(64, multidimensional);
        Assert.AreEqual(64, multidimensional_length);
        Assert.HasCount(3, jagged);
        Assert.AreEqual(3, jagged_length);
    }

    [TestMethod]
    public void grid1()
    {
        const byte Width = 8;
        const byte Height = 8;
        const char DOT = '·';//U+00B7 MIDDLE DOT

        char[,] grid = new char[Width,Height];
        for (byte k = 0; k < Width; ++k) for (byte j = 0; j < Height; ++j) grid[k, j] = DOT;
        display(grid);
        Assert.AreEqual(DOT,grid[0,0]);

        int height = 4, width = 3;
        var inner = new char[height, width];
        int source_height = inner.GetLength(0);
        int source_width = inner.GetLength(1);
        Assert.AreEqual(height, source_height);
        Assert.AreEqual(width,source_width);
    }

    [TestMethod]
    public void grid2()
    {
        Grid g = new();
        var A = Grid.Init(3, 'A');
        var B = Grid.Init(2, 'B');
        var C = Grid.Init(4, 3, 'C');
        g.Add(0, 1, A);
        g.Add(4, 3, B);
        g.Add(1, 5, C);
        g.Display();
        Assert.HasCount(3, A);
        Assert.IsTrue(A.SequenceEqual(['A', 'A', 'A']));
        Assert.HasCount(2, B);
        Assert.IsTrue(B.SequenceEqual(['B', 'B']));
        Assert.HasCount(12, C);
    }
}