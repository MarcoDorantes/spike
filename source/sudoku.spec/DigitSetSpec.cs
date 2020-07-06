using Xunit;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace sudoku.spec
{
  #region SUT
  interface IDigitStore
  {
    int Count { get; }
    bool Contains(int x);
    int this[int index] { get; set; }
    void Add(int x);
  }
  class ListDigitStore : IDigitStore
  {
    private List<int> store;
    public ListDigitStore()
    {
      store = new List<int>();
    }
    #region IDigitStore
    public int Count { get => store.Count; }
    public bool Contains(int x)=>store.Any(n => n == x);
    public int this[int index] { get => store[index]; set => store[index] = value; }
    public void Add(int x) => store.Add(x);
    #endregion
  }
  class DigitSet
  {
    public const int MaxDigitCount = 9;

    private IDigitStore Digits;

    public DigitSet(IDigitStore store, int[] digits = null)
    {
      /*if (digits == null)
      {
        throw new ArgumentNullException(nameof(digits), "Null is invalid.");
      }*/
      Digits = store;// new List<int>(Enumerable.Repeat(0, MaxDigitCount));
      if (digits != null)
        foreach (var n in digits)
        {
          Add(n);
        }
    }

    public int this[int index] { get => Digits[index]; }
    public int Count { get => Digits.Count; }
    public bool In(int x) => Digits.Contains(x);//Any(n => n == x);
    public void Add(int n)
    {
      CheckDigitValue(n);
      CheckDigitDuplicate(Digits, n);
      //CheckOverSize(Digits, n);
      Digits.Add(n);
    }
    public void Add(int n, int index)
    {
      CheckDigitValue(n);
      CheckDigitDuplicate(Digits, n);
      //CheckOverSize(Digits, n);
      Digits[index] = n;
    }
    public static bool IsValidDigit(int n) => n > 0 && n < 10;

    private static void CheckDigitDuplicate(IDigitStore digitset, int n)
    {
      if (digitset.Contains(n))//Any(x => x == n)
      {
        throw new ArgumentOutOfRangeException($"Invalid duplicate digit ({n}).", (Exception)null);
      }
    }
    private static void CheckDigitValue(int n)
    {
      if (!IsValidDigit(n))
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
  class SubGrid : IDigitStore
  {
    public const int MaxSubGridCellCount = DigitSet.MaxDigitCount;

    private DigitSet digiset;

    public SubGrid(IEnumerable<Cell> cells)
    {
      Cells = cells;
      digiset = new DigitSet(this);
    }

    #region IDigitStore
    int IDigitStore.Count { get => Cells.Count(c => c.Digit.HasValue); }
    bool IDigitStore.Contains(int x) => Cells.Any(c => c.Digit.HasValue && c.Digit.Value == x);
    int IDigitStore.this[int index] { get => this[index].Value; set { }/* => this[index].Value = value;*/ }
    void IDigitStore.Add(int x) { Cells.ElementAt(Count + 1).Digit = x; }// => store.Add(x);
    #endregion

    public int Count { get => ((IDigitStore)this).Count; }
    public void Add(int n)
    {
      digiset.Add(n);
    }
    public int? this[int index]
    {
      get
      {
        int? result = null;
        if (index >= Cells.Count())
        {
          throw new IndexOutOfRangeException($"Cell {nameof(index)} ({index}) is out of range.");
        }
        if (index < Count)//if (index < ((IDigitStore)this).Count)
        {
          result = digiset[index];
        }
        return result;
      }
    }
    public IEnumerable<Cell> Cells { get; private set; }
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
      var @set = new DigitSet(new ListDigitStore());
      Assert.False(@set.In(0));
      Assert.False(@set.In(1));
      Assert.False(@set.In(2));
      Assert.False(@set.In(3));
      Assert.False(@set.In(4));
    }
    [Fact]
    public void In()
    {
      var @set = new DigitSet(new ListDigitStore(), new[] { 1, 2, 3 });
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
      Assert.Equal("Null is invalid. (Parameter 'digits')", expected.Message);
    }
    [Fact]
    public void InvalidLowerDigit()
    {
      ArgumentOutOfRangeException expected = null;
      try
      {
        new DigitSet(new ListDigitStore(), Enumerable.Range(0, 9).ToArray());
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
      try
      {
        new DigitSet(new ListDigitStore(), Enumerable.Range(1, 10).ToArray());
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
      var @set = new DigitSet(new ListDigitStore());
      Assert.Equal(0, @set.Count);
      @set.Add(1);
      Assert.Equal(1, @set.Count);
    }
    [Fact]
    public void Add()
    {
      var @set = new DigitSet(new ListDigitStore());
      Assert.Equal(0, @set.Count);
      Assert.False(@set.In(1));
      @set.Add(1);
      Assert.Equal(1, @set.Count);
      Assert.True(@set.In(1));
    }
    [Fact]
    public void DuplicateAtCtor()
    {
      ArgumentOutOfRangeException expected = null;
      try
      {
        new DigitSet(new ListDigitStore(), new[] { 1, 2, 1 });
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
      var @set = new DigitSet(new ListDigitStore());
      Assert.Equal(0, @set.Count);
      Assert.False(@set.In(1));
      @set.Add(1);
      Assert.Equal(1, @set.Count);
      Assert.True(@set.In(1));
      Assert.Throws<ArgumentOutOfRangeException>(() => @set.Add(1));
    }
    [Fact]
    public void DuplicateAtIndexAdd()
    {
      var @set = new DigitSet(new ListDigitStore());
      Assert.Equal(0, @set.Count);
      Assert.False(@set.In(1));
      @set.Add(1, 3);
      Assert.Equal(1, @set.Count);
      Assert.True(@set.In(1));
      Assert.Throws<ArgumentOutOfRangeException>(() => @set.Add(1, 3));
    }
    [Fact]
    public void Index()
    {
      var @set = new DigitSet(new ListDigitStore());
      @set.Add(8);
      Assert.Equal(8, @set[0]);
    }
    [Fact]
    public void NoIndex()
    {
      var @set = new DigitSet(new ListDigitStore());
      Assert.Throws<ArgumentOutOfRangeException>(() => @set[1]);
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
      row.Add(3);
      Assert.Equal(1, row.Count);
      Assert.Equal(3, row[0]);
    }
    [Fact]
    public void RowIndexedCell()
    {
      var grid = new Grid();
      var row = grid.Rows.First();
      Assert.Equal((int?)null, row[2]);
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
      column.Add(3);
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
    [Fact(Skip ="wip")]
    public void Grid1()
    {
      var g = new Grid();
      
    }
  }
}