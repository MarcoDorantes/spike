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
      foreach (var cell in cells)
      {
        if (!cell.Digit.HasValue) continue;
        CheckDigitValue(cell.Digit.Value);
        CheckDigitDuplicate(cell.Digit.Value);
      }
      Cells = cells;
    }

    public Grid Grid { get => Cells.First().Grid; }
    public IEnumerable<Cell> Cells { get; private set; }

    public int? this[int index]
    {
      get
      {
        if (index >= Cells.Count())
        {
          throw new IndexOutOfRangeException($"Cell {nameof(index)} ({index}) is out of range.");
        }
        return Cells.ElementAt(index).Digit;
      }
      set
      {
        if (index >= Cells.Count())
        {
          throw new IndexOutOfRangeException($"Cell {nameof(index)} ({index}) is out of range.");
        }
        CheckDigitValue(value);
        CheckDigitDuplicate(value);
        var cell = Cells.ElementAt(index);
        CheckDigitDuplicate(cell, value);
        cell.Digit = value;
      }
    }
    public int Count { get => Cells.Count(c => c.Digit.HasValue); }
    public bool In(int x) => Cells.Any(c => c.Digit.HasValue && c.Digit.Value == x);

    public void Fill()
    {
      if (Grid.All(c => c.Digit.HasValue == false))
      {
        var d = GetSeries(1);
        for (int k = 0; k < MaxDigitCount; ++k)
        {
          this[k] = d.ElementAt(k);
        }
      }
      else if (Cells.Count(c => c.Digit.HasValue) == 1)
      {
        var d = GetSeries(2);
        var r = d.Except(Cells.Where(c => c.Digit.HasValue).Select(c => c.Digit.Value));
        int index = 1;
        foreach (var n in r)
        {
          this[index++] = n;
        }
      }
      else
      {
        var remain_index = Cells.Select((c, index) => c.Digit.HasValue ? -1 : index).Where(i => i != -1).OrderBy(i => i).ToArray();
        var remain_digit = GetSeries(3).Except(Cells.Where(c => c.Digit.HasValue).Select(c => c.Digit.Value)).OrderBy(i => i).ToArray();
        if (remain_index.Count() != remain_digit.Count()) throw new Exception($"{nameof(remain_index)} ({remain_index.Count()}) != {nameof(remain_digit)} ({remain_digit.Count()})");
        for (int k = 0; k < remain_index.Length; ++k)
        {
          this[remain_index[k]] = remain_digit[k];
        }
      }
    }

    public static bool IsValidDigit(int n) => n > 0 && n <= MaxDigitCount;
    public static IEnumerable<int> GetSeries(int rowtype, int maxcount = MaxDigitCount)
    {
      var tries = new[] { (1, 25), (2, 30), (3,25), (4, 30), (5, 30), (6, 25), (7, 35), (8, 25), (9, 30) };
      var try_count = tries.FirstOrDefault(t => t.Item1 == rowtype).Item2;
      if (try_count <= 0) throw new ArgumentOutOfRangeException(nameof(rowtype), $"Unsupported {nameof(rowtype)} ({rowtype}).");
      var rnd = new Random(rowtype);
      var set = new List<int>();
      for (int k = 0; k < try_count; ++k)
      {
        if (set.Count == maxcount) break;
        var n = rnd.Next(1, maxcount + 1);
        if (set.Contains(n)) continue;
        set.Add(n);
      }
      return set;
    }
    public static IEnumerable<int> Shuffle(IEnumerable<int> n, int maxtries = 10)
    {
      IEnumerable<int> result = null;
      if (n != null)
      {
        int trycount = 0;
        while (result == null && trycount < maxtries)
        {
          var tryresult = try_shuffle(n);
          if (!n.SequenceEqual(tryresult))
          {
            result = tryresult;
          }
          else ++trycount;
        }
        if (result != null && trycount == maxtries)
        {
          throw new ApplicationException($"Unable to shuffle after {maxtries} retries.");
        }
      }
      return result;

      IEnumerable<int> try_shuffle(IEnumerable<int> _n)
      {
        var _result = _n.ToArray();
        int length = _result.Count();
        var rowtype = (new Random()).Next(1, 10);
        var serie = GetSeries(rowtype, length);
        for (int k = 0; k < length; ++k)
        {
          swap(_result, k, serie.ElementAt(k) - 1);
        }
        return _result;
      }
      void swap(int[] _array, int _a, int _b)
      {
        if (_a < 0 || _a >= _array.Length) throw new ArgumentOutOfRangeException(nameof(_a), $"{_a}");
        if (_b < 0 || _b >= _array.Length) throw new ArgumentOutOfRangeException(nameof(_b), $"{_b}");
        int t = _array[_a];
        _array[_a] = _array[_b];
        _array[_b] = t;
      }
    }

    private void CheckDigitDuplicate(int? n)
    {
      if (n.HasValue == true && In(n.Value))
      {
        throw new ArgumentOutOfRangeException($"Invalid duplicate digit ({n}).", (Exception)null);
      }
    }
    private static void CheckDigitDuplicate(Cell cell, int? value)
    {
      if (value.HasValue &&
        (
        cell.Grid.Rows.ElementAt(cell.Row).In(value.Value) ||
        cell.Grid.Columns.ElementAt(cell.Column).In(value.Value) ||
        cell.Grid.Squares.ElementAt(cell.Square).In(value.Value)
        )
      )
      {
        throw new ArgumentOutOfRangeException($"Cell {nameof(value)} ({value.Value}) is already taken.", (Exception)null);
      }
    }
    private static void CheckDigitValue(int? n)
    {
      if (n.HasValue == true && !IsValidDigit(n.Value))
      {
        throw new ArgumentOutOfRangeException($"{n}", "Digit out of valid range (1-9).");
      }
    }
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
        rows.Add(new Row(row));
      }
      Rows = rows;
      Columns = this.GroupBy(c => c.Column).Aggregate(new List<SubGrid>(), (w, n) => { w.Add(new Column(n)); return w; });
      Squares = this.GroupBy(c => c.Square).Aggregate(new List<SubGrid>(), (w, n) => { w.Add(new Square(n)); return w; });
    }

    public int? this[int row, int column] { get => Rows.ElementAt(row)[column]; }

    public IEnumerable<SubGrid> Rows { get; private set; }
    public IEnumerable<SubGrid> Columns { get; private set; }
    public IEnumerable<SubGrid> Squares { get; private set; }

    public override string ToString()
    {
      var result = new StringBuilder();
      foreach (var row in Rows)
      {
        foreach (var cell in row.Cells)
        {
          result.Append("\t" + (cell.Digit.HasValue ? $"{cell.Digit.Value}" : "."));
        }
        result.AppendLine();
      }
      return $"{result}";
    }
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
    [Fact]
    public void FillRow()
    {
      var d = Enumerable.Range(1, 9);
      var grid = new Grid();
      var row = grid.Rows.First();
      for (int k = 0; k < SubGrid.MaxSubGridCellCount; ++k)
      {
        row[k] = d.ElementAt(k);
      }
      Assert.Equal(SubGrid.MaxSubGridCellCount, row.Count);
    }
    [Fact]
    public void same_random_sequence()
    {
      var rnd = new Random(1);
      Assert.Equal("3|1|5|7|6|4|4|9|1|6|", $"{Enumerable.Range(1, 10).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", rnd.Next(1, 10)))}");
    }
    [Fact]
    public void semi_row_series1()
    {
      var set = DigitSet.GetSeries(1);
      Assert.Equal("3|1|5|7|6|4|9|2|8|", $"{set.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n))}");
    }
    [Fact]
    public void semi_row_series2()
    {
      var set = DigitSet.GetSeries(2);
      Assert.Equal("7|4|2|9|1|3|8|5|6|", $"{set.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n))}");
    }
    [Fact]
    public void semi_row_series3()
    {
      var set = DigitSet.GetSeries(3);
      Assert.Equal("3|7|8|2|6|9|4|5|1|", $"{set.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n))}");
    }
    [Fact]
    public void semi_row_series4()
    {
      var set = DigitSet.GetSeries(4);
      Assert.Equal("8|9|6|4|1|7|5|3|2|", $"{set.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n))}");
    }
    [Fact]
    public void semi_row_series5()
    {
      var set = DigitSet.GetSeries(5);
      Assert.Equal("4|3|6|5|9|2|1|8|7|", $"{set.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n))}");
    }
    [Fact]
    public void semi_row_series6_9()
    {
      Assert.Equal("8|6|9|5|7|2|4|3|1|", $"{DigitSet.GetSeries(6).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n))}");
      Assert.Equal("4|8|6|1|7|9|5|2|3|", $"{DigitSet.GetSeries(7).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n))}");
      Assert.Equal("9|2|4|3|8|5|7|6|1|", $"{DigitSet.GetSeries(8).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n))}");
      Assert.Equal("4|5|1|3|9|6|8|2|7|", $"{DigitSet.GetSeries(9).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n))}");
    }
    [Fact]
    public void shuffle()
    {
      var n = new[] { 1, 2, 3, 4 };
      var s = DigitSet.Shuffle(n);
      Assert.Equal(s.Count(), n.Count());
      Assert.False(s.SequenceEqual(n));
      Assert.False(s.SequenceEqual(n.Reverse()));
    }
  }
  public class CellSpec
  {
    [Fact]
    public void Index()
    {
      var grid = new Grid();
      foreach (var row in grid.Rows)
      {
        Assert.True(row.Cells.Select((c, index) => index).SequenceEqual(Enumerable.Range(0, DigitSet.MaxDigitCount)));
      }
      foreach (var column in grid.Columns)
      {
        Assert.True(column.Cells.Select((c, index) => index).SequenceEqual(Enumerable.Range(0, DigitSet.MaxDigitCount)));
      }
      foreach (var square in grid.Squares)
      {
        Assert.True(square.Cells.Select((c, index) => index).SequenceEqual(Enumerable.Range(0, DigitSet.MaxDigitCount)));
      }
    }
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
    [Fact]
    public void OneDigit()
    {
      var grid = new Grid();
      Assert.True(grid.All(c => c.Digit.HasValue == false));
      grid.Rows.First()[0] = 9;
      Assert.Equal(1, grid.Count(c => c.Digit.HasValue == true));
    }
    [Fact]
    public void OneDigitTwice()
    {
      ArgumentOutOfRangeException expected = null;
      var grid = new Grid();
      Assert.True(grid.All(c => c.Digit.HasValue == false));
      grid.Rows.First()[0] = 9;
      Assert.Equal(1, grid.Count(c => c.Digit.HasValue == true));
      try
      {
        grid.Rows.Last()[0] = 9;
      }
      catch (ArgumentOutOfRangeException exception)
      {
        expected = exception;
      }
      Assert.NotNull(expected);
      Assert.Equal("Cell value (9) is already taken.", expected.Message);
    }
    [Fact]
    public void OneUndo()
    {
      var grid = new Grid();
      Assert.True(grid.All(c => c.Digit.HasValue == false));
      grid.Rows.First()[0] = 9;
      Assert.False(grid.All(c => c.Digit.HasValue == false));
      Assert.Equal(1, grid.Count(c => c.Digit.HasValue == true));
      grid.Rows.First()[0] = null;
      Assert.True(grid.All(c => c.Digit.HasValue == false));
      Assert.Equal(0, grid.Count(c => c.Digit.HasValue == true));
    }
    [Fact]
    public void RemainIndexes()
    {
      var grid = new Grid();
      grid.Rows.First().Fill();
      grid.Columns.First().Fill();
      var s = grid.Squares.First();
      var remain_index = s.Cells.Select((c, index) => c.Digit.HasValue ? -1 : index).Where(i => i != -1).OrderBy(i => i);
      Assert.Equal("4|5|7|8|",$"{remain_index.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n))}");
      Assert.True(new[] { 4, 5, 7, 8 }.SequenceEqual(remain_index));
    }
  }
  public class ContentSpec
  {
    [Fact]
    public void Overlap()
    {
      var grid = new Grid();

      var column = grid.Columns.First();
      for (int k = 0; k < Grid.MaxRowCount; ++k)
      {
        column[k] = k + 1;
      }
      Enumerable.Range(1, SubGrid.MaxSubGridCellCount).ToList().ForEach(n => Assert.True(column.In(n)));

      var square = grid.Squares.First();
      Assert.True(square.In(1));
      Assert.True(square.In(2));
      Assert.True(square.In(3));
      Enumerable.Range(4, SubGrid.MaxSubGridCellCount - 4).ToList().ForEach(n => Assert.False(square.In(n)));

      var row = grid.Rows.First();
      Assert.True(row.In(1));
      Enumerable.Range(2, SubGrid.MaxSubGridCellCount - 2).ToList().ForEach(n => Assert.False(row.In(n)));

      Assert.Equal(
"\t1\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t2\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t3\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t4\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t5\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t6\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t7\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t8\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t9\t.\t.\t.\t.\t.\t.\t.\t.\r\n", $"{grid}");
    }
    [Fact]
    public void Complete_a()
    {
      var grid = new Grid();
      var square = grid.Squares.ElementAt(grid.Squares.Count() / 2);
      for (int k = 0; k < SubGrid.MaxSubGridCellCount; ++k)
      {
        square[k] = k + 1;
      }
      Enumerable.Range(1, SubGrid.MaxSubGridCellCount).ToList().ForEach(n => Assert.True(square.In(n)));

      Assert.Equal(
"\t.\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t.\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t.\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t.\t.\t.\t1\t2\t3\t.\t.\t.\r\n" +
"\t.\t.\t.\t4\t5\t6\t.\t.\t.\r\n" +
"\t.\t.\t.\t7\t8\t9\t.\t.\t.\r\n" +
"\t.\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t.\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t.\t.\t.\t.\t.\t.\t.\t.\t.\r\n", $"{grid}");
    }
    [Fact]
    public void Complete_b()
    {
      var grid = new Grid();

      IEnumerator<SubGrid> rows = null;
      IEnumerator<SubGrid> columns = null;
      IEnumerator<SubGrid> squares = null;

      var row = next(grid.Rows, ref rows);
      row.Fill();
      var column = next(grid.Columns, ref columns);
      column.Fill();
      var square = next(grid.Squares, ref squares);
      square.Fill();

      //row = next(grid.Rows, ref rows);
      //row.Fill();

      Assert.Equal(SubGrid.MaxSubGridCellCount * 2 + 3, grid.Count(c => c.Digit.HasValue == true));

      Assert.Equal(
"\t3\t1\t5\t7\t6\t4\t9\t2\t8\r\n" +
"\t7\t2\t6\t.\t.\t.\t.\t.\t.\r\n" +
"\t4\t8\t9\t.\t.\t.\t.\t.\t.\r\n" +
"\t2\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t9\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t1\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t8\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t5\t.\t.\t.\t.\t.\t.\t.\t.\r\n" +
"\t6\t.\t.\t.\t.\t.\t.\t.\t.\r\n", $"{grid}");

      static SubGrid next(IEnumerable<SubGrid> all, ref IEnumerator<SubGrid> itr)
      {
        if (itr?.MoveNext() == true)
        {
          return itr.Current;
        }
        else
        {
          itr = all.GetEnumerator();
          itr.MoveNext();
          return itr.Current;
        }
      }
    }
  }
}