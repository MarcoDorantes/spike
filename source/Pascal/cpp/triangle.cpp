// https://qr.ae/pAExuz
// cl /EHsc triangle.cpp
/*
The Formula Behind the Numbers
Each number in Pascal's Triangle can be calculated directly using the binomial coefficient formula.
The number in the nth row and kth position (starting the count from 0) is given by the formula:

            n!
(n, k) = --------
         k!(n-k)!

function Factorial($n) { if($n -le 1) {return 1} else {return $n * (Factorial ($n-1))} }
function Pascal($n, $k){ return (Factorial $n) / ((Factorial $k)*(Factorial ($n-$k)))  }
*/
#include <iostream>
#include <vector>

void triangle_of_Pascal_v1(unsigned int rows)
{
  if(rows<1) return;
  std::cout<<"Pascal's triangle of "<<rows<<" rows:\n";
  if(rows==1){std::cout<<rows<<"\n";return;}
  int columns = (rows * 2) - 1;
  std::vector<std::vector<int>> grid(rows, std::vector<int>(columns, 0));

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
  
  /*for(int row=0; row<rows; ++row) {
    for(int column=0; column<columns; ++column) {
      int v=grid[row][column];
      if(v==0) std::cout<<"\t"; else std::cout<<v<<"\t";
    }
    std::cout<<"\n";
  }*/
  
  for(auto& row:grid) {
    for(auto& v:row) {
      //std::cout<<v<<"\t";
      if(v==0) std::cout<<"\t"; else std::cout<<v<<"\t";
    }
    std::cout<<"\n";
  }
}

int factorial(int n){if(n<=1) return 1; return n*factorial(n-1);}
int Pascal(int n,int k){return factorial(n) / (factorial(k) * factorial(n-k));}
void triangle_of_Pascal_v2(unsigned int rows)
{
  if(rows<1) return;
  std::cout<<"Pascal's triangle of "<<rows<<" rows:\n";
  if(rows==1){std::cout<<rows<<"\n";return;}
  std::vector<std::vector<int>> grid(rows);

  grid[0]=std::vector<int>(1, 1);
  for(int row=1, columns=2; row<rows; ++row, ++columns) {
    grid[row]=std::vector<int>(columns);
    for(int col=0; col<columns; ++col)
    {
      grid[row][col] = Pascal(row,col);
    }
  }

  for(int row=0, tabs=rows-1; row<rows; ++row, --tabs) {
    std::cout<<std::string(tabs, '\t');
    for(int col=0; col<grid[row].size(); ++col)
    {
      if(col>0) std::cout<<"\t\t";
      std::cout<<grid[row][col];
    }
    std::cout<<"\n";
  }
}

int main() {
  int return_code = 0;
  triangle_of_Pascal_v1(8U);//15 max Window
  triangle_of_Pascal_v2(8U);
  return return_code;
}