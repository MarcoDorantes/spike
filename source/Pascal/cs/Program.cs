// See https://aka.ms/new-console-template for more information
using static System.Console;

void triangle_of_Pascal(int rows)//uint rows)
{
  if(rows<1) return;
  WriteLine($"Pascal's triangle of {rows} rows:\n");
  if(rows==1){WriteLine(rows);return;}
  int columns = (rows * 2) - 1;
  List<List<int>> grid = Enumerable.Range(1, rows).Aggregate(new List<List<int>>(), (whole, next) => {whole.Add(new List<int>(Enumerable.Repeat(0,columns))); return whole;});

  int row_start_index=columns/2;
  grid[0][row_start_index]=1;
  grid[1][row_start_index+1]=1;
  grid[1][--row_start_index]=1;
  for(int row=2; row<rows; ++row) {
    int row_value_count = row+1;
    int row_sums = row_value_count-2;
    grid[row][--row_start_index]=1;
    int col=row_start_index+2;
    for(
      int sums_count=0;
      col<columns && sums_count<row_sums;
      col+=2, ++sums_count)
    {
      grid[row][col]=grid[row-1][col-1] + grid[row-1][col+1];
    }
    grid[row][col]=1;
  }

  foreach(var row in grid) {
    foreach(var v in row) {
      if(v==0) Write("\t"); else Write($"{v}\t");
    }
    WriteLine();
  }
}

triangle_of_Pascal(8);//8U//15 max Window