// https://qr.ae/pAExuz
// cl /EHsc triangle.cpp

#include <iostream>
#include <vector>

void triangle_of_Pascal(unsigned int rows)
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

int main() {
  int return_code = 0;
  triangle_of_Pascal(8U);//15 max Window
  return return_code;
}