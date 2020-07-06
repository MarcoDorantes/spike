using Xunit;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace sudoku.spec
{
  #region SUT
  class DigitSet
  {
    public const int MaxDigitCount = 9;

    public DigitSet(IEnumerable<Cell> cells)
    {
      if (cells == null)
      {
        throw new ArgumentNullException(nameof(cells), "Null is invalid.");
      }
      Cells = cells;
    }

    public IEnumerable<Cell> Cells { get; private set; }

    public int? this[int index]
    {
      get
      {
        int? result = null;
        if (index >= Cells.Count())
        {
          throw new IndexOutOfRangeException($"Cell {nameof(index)} ({index}) is out of range.");
        }
        result = Cells.ElementAt(index).Digit;
        return result;
      }
      set
      {
        CheckDigitValue(value);
        CheckDigitDuplicate(value);
        //CheckOverSize(Digits, n);
        if (index >= Cells.Count())
        {
          throw new IndexOutOfRangeException($"Cell {nameof(index)} ({index}) is out of range.");
        }
        Cells.ElementAt(index).Digit = value;
      }
    }
    public int Count { get => Cells.Count(c => c.Digit.HasValue); }
    public bool In(int x) => Cells.Any(c => c.Digit.HasValue && c.Digit.Value == x);

    public static bool IsValidDigit(int n) => n > 0 && n < 10;

    private void CheckDigitDuplicate(int? n)
    {
      if (n.HasValue == false || In(n.Value))//Any(x => x == n)
      {
        throw new ArgumentOutOfRangeException($"Invalid duplicate digit ({n}).", (Exception)null);
      }
    }
    private static void CheckDigitValue(int? n)
    {
      if (n.HasValue == false || !IsValidDigit(n.Value))
      {
        throw new ArgumentOutOfRangeException($"{n}", "Digit out of valid range (1-9).");
      }
    }
    /*private static void CheckOverSize(IEnumerable<int> digitset, int n)
    {
      if (digitset.Count() == MaxDigitCount)
      {
        throw new ArgumentOutOfRangeException($"Invalid digit count ({digitset.Count() + 1}) while adding '{n}'.", (Exception)null);
      }
    }*/
  }

  class Cell
  {
    public Cell(int row, int column, int square, Grid grid)
    {
      Row = row;
      Column = column;
      Square = square;
      Grid = grid;
      Digit = null;
    }
    public int? Digit { get; set; }
    public int Row { get; private set; }
    public int Column { get; private set; }
    public int Square { get; private set; }
    public Grid Grid { get; private set; }

    #region Identity
    public override bool Equals(object other) => (other as Cell)?.GetHashCode() == GetHashCode();
    public override int GetHashCode() =>
      Row.GetHashCode() +
      Column.GetHashCode() +
      Square.GetHashCode() +
      (Digit?.GetHashCode() ?? 0);
    #endregion
  }
  class SubGrid : DigitSet
  {
    public const int MaxSubGridCellCount = DigitSet.MaxDigitCount;

    public SubGrid(IEnumerable<Cell> cells) : base(cells) { }
  }
  class Row : SubGrid
  {
    public Row(IEnumerable<Cell> cells) : base(cells) { }
  }
  class Column : SubGrid
  {
    public Column(IEnumerable<Cell> cells) : base(cells) { }
  }
  class Square : SubGrid
  {
    public Square(IEnumerable<Cell> cells) : base(cells) { }
  }
  class Grid : List<Cell>
  {
    public const int MaxCellCount = SubGrid.MaxSubGridCellCount * SubGrid.MaxSubGridCellCount;
    public const int MaxRowCount = SubGrid.MaxSubGridCellCount;
    public const int MaxColumnCount = SubGrid.MaxSubGridCellCount;
    public const int MaxSquareCount = SubGrid.MaxSubGridCellCount;

    public Grid()
    {
      var rows = new List<SubGrid>();
      int square_side = MaxSquareCount / 3;
      for (int r = 0; r < MaxRowCount; ++r)
      {
        var row = new List<Cell>();
        for (int c = 0; c < MaxColumnCount; ++c)
        {
          var sindex = square_side * (r / square_side) + (c / square_side);
          var cell = new Cell(r, c, sindex, this);
          Add(cell);
          row.Add(cell);
        }
        rows.Add(new SubGrid(row));
      }
      Rows = rows;
      Columns = this.GroupBy(c => c.Column).Aggregate(new List<SubGrid>(), (w, n) => { w.Add(new SubGrid(n)); return w; });
      Squares = this.GroupBy(c => c.Square).Aggregate(new List<SubGrid>(), (w, n) => { w.Add(new SubGrid(n)); return w; });
    }

    public int? this[int row, int column] { get => Rows.ElementAt(row)[column]; }

    public IEnumerable<SubGrid> Rows { get; private set; }
    public IEnumerable<SubGrid> Columns { get; private set; }
    public IEnumerable<SubGrid> Squares { get; private set; }
  }
  #endregion

  public class DigitSpec
  {
    [Fact]
    public void ValidDigit()
    {
      Enumerable.Range(1, 9).ToList().ForEach(n => Assert.True(DigitSet.IsValidDigit(n)));
    }
    [Fact]
    public void InValidDigit()
    {
      Assert.False(DigitSet.IsValidDigit(0));
      Assert.False(DigitSet.IsValidDigit(10));
      Assert.False(DigitSet.IsValidDigit(-1));
    }
  }
  public class DigitSetSpec
  {
    [Fact]
    public void Empty()
    {
      var grid = new Grid();
      var @set = grid.Rows.First();
      Assert.False(@set.In(0));
      Assert.False(@set.In(1));
      Assert.False(@set.In(2));
      Assert.False(@set.In(3));
      Assert.False(@set.In(4));
    }
    [Fact]
    public void In()
    {
      var grid = new Grid();
      var @set = grid.Rows.First();
      @set[0] = 1;
      @set[1] = 2;
      @set[2] = 3;
      Assert.False(@set.In(0));
      Assert.True(@set.In(1));
      Assert.True(@set.In(2));
      Assert.True(@set.In(3));
      Assert.False(@set.In(4));
    }
    [Fact]
    public void NullDigits()
    {
      ArgumentNullException expected = null;
      try
      {
        new DigitSet(null);
      }
      catch (ArgumentNullException exception)
      {
        expected = exception;
      }
      Assert.NotNull(expected);
      Assert.Equal("Null is invalid. (Parameter 'cells')", expected.Message);
    }
    [Fact]
    public void InvalidLowerDigit()
    {
      ArgumentOutOfRangeException expected = null;
      var grid = new Grid();
      var @set = grid.Rows.First();
      try
      {
        @set[0] = 0;
      }
      catch (ArgumentOutOfRangeException exception)
      {
        expected = exception;
      }
      Assert.NotNull(expected);
      Assert.Equal("Digit out of valid range (1-9). (Parameter '0')", expected.Message);
    }
    [Fact]
    public void InvalidUpperNumber()
    {
      ArgumentOutOfRangeException expected = null;
      var grid = new Grid();
      var @set = grid.Rows.First();
      try
      {
        @set[0] = 10;
      }
      catch (ArgumentOutOfRangeException exception)
      {
        expected = exception;
      }
      Assert.NotNull(expected);
      Assert.Equal("Digit out of valid range (1-9). (Parameter '10')", expected.Message);
    }
    [Fact]
    public void Count()
    {
      var grid = new Grid();
      var @set = grid.Rows.First();
      Assert.Equal(0, @set.Count);
      @set[0] = 1;
      Assert.Equal(1, @set.Count);
    }
    [Fact]
    public void Add()
    {
      var grid = new Grid();
      var @set = grid.Rows.First();
      Assert.Equal(0, @set.Count);
      Assert.False(@set.In(1));
      @set[0] = 1;
      Assert.Equal(1, @set.Count);
      Assert.True(@set.In(1));
    }
    [Fact(Skip ="unscope")]
    public void DuplicateAtCtor()
    {
      ArgumentOutOfRangeException expected = null;
      try
      {
        new DigitSet(new[] { new Cell(0, 0, 0, null) { Digit = 1 }, new Cell(0, 0, 0, null) { Digit = 2 }, new Cell(0, 0, 0, null) { Digit = 1 } });
      }
      catch (ArgumentOutOfRangeException exception)
      {
        expected = exception;
      }
      Assert.NotNull(expected);
      Assert.Equal("Invalid duplicate digit (1).", expected.Message);
    }
    [Fact]
    public void DuplicateAtAdd()
    {
      var grid = new Grid();
      var @set = grid.Rows.First();
      Assert.Equal(0, @set.Count);
      Assert.False(@set.In(1));
      @set[0] = 1;
      Assert.Equal(1, @set.Count);
      Assert.True(@set.In(1));
      Assert.Throws<ArgumentOutOfRangeException>(() => @set[0] = 1);
    }
    [Fact]
    public void DuplicateAtIndexAdd()
    {
      var grid = new Grid();
      var @set = grid.Rows.First();
      Assert.Equal(0, @set.Count);
      Assert.False(@set.In(1));
      @set[3] = 1;
      Assert.Equal(1, @set.Count);
      Assert.True(@set.In(1));
      Assert.Throws<ArgumentOutOfRangeException>(() => @set[3] = 1);
    }
    [Fact]
    public void Index()
    {
      var grid = new Grid();
      var @set = grid.Rows.First();
      @set[0] = 8;
      Assert.Equal(8, @set[0].Value);
    }
    [Fact]
    public void NoIndex()
    {
      var grid = new Grid();
      var @set = grid.Rows.First();
      Assert.Throws<IndexOutOfRangeException>(() => @set[10]);
    }
  }
  public class CellSpec
  {
  }
  public class GridSpec
  {
    [Fact]
    public void Row()
    {
      var grid = new Grid();
      var row = grid.Rows.First();
      Assert.Equal(9, row.Cells.Count());
      Assert.Equal(0, row.Count);
    }
    [Fact]
    public void RowIndexedDigit()
    {
      var grid = new Grid();
      var row = grid.Rows.First();
      row[0] = 3;
      Assert.Equal(1, row.Count);
      Assert.True(row[0].HasValue);
      Assert.Equal(3, row[0].Value);
    }
    [Fact]
    public void RowIndexedCell()
    {
      var grid = new Grid();
      var row = grid.Rows.First();
      Assert.False(row[2].HasValue);
    }
    [Fact]
    public void RowNonIndexedCell()
    {
      var grid = new Grid();
      var row = grid.Rows.First();
      IndexOutOfRangeException expected = null;
      try
      {
        var x = row[12];
      }
      catch (IndexOutOfRangeException exception)
      {
        expected = exception;
      }
      Assert.NotNull(expected);
      Assert.Equal("Cell index (12) is out of range.", expected.Message);
    }
    [Fact]
    public void Column()
    {
      var grid = new Grid();
      var column = grid.Columns.First();
      Assert.Equal(9, column.Cells.Count());
      Assert.Equal(0, column.Count);
    }
    [Fact]
    public void ColumnIndexedDigit()
    {
      var grid = new Grid();
      var column = grid.Columns.First();
      column[0] = 3;
      var digit = column[0];
      Assert.Equal(1, column.Count);
      Assert.True(digit.HasValue);
      Assert.Equal(digit.Value, column[0].Value);
      Assert.Equal(digit.Value, grid[0, 0].Value);
    }
    [Fact]
    public void Square()
    {
      var grid = new Grid();
      var square = grid.Squares.First();
      Assert.Equal(9, square.Cells.Count());
      Assert.Equal(0, square.Count);
    }
    [Fact]
    public void Grid()
    {
      var grid = new Grid();
      Assert.Equal(9, grid.Rows.Count());
      Assert.Equal(9, grid.Columns.Count());
      Assert.Equal(9, grid.Squares.Count());
      var squares = grid.Select(c => c.Square);
      Assert.Equal(spec.Grid.MaxCellCount, squares.Count());
      Assert.Equal("0|1|2|3|4|5|6|7|8|", $"{squares.Distinct().Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n))}");
      Assert.Equal(9, squares.Distinct().Count());
      Assert.True(grid.All(c => c.Digit.HasValue == false));
    }
  }
  public class ContentSpec
  {
    [Fact]
    public void RowSquareOverlap()
    {
      var grid = new Grid();
      var column = grid.Columns.First();
      for (int k = 0; k < Grid.MaxRowCount; ++k)
      {
        column[k] = k + 1;
      }
      var square = grid.Squares.First();
      Assert.True(square.In(2));
    }
  }
}