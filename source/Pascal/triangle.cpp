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
    int row_calcs = row_value_count-2;
    grid[row][--row_start_index]=1;
    int calcs_count=0;
    int column=row_start_index+2;
    for(; column<columns; column+=2) {
      if(calcs_count>=row_calcs) break;
      grid[row][column] = grid[row-1][column-1] + grid[row-1][column+1];
      ++calcs_count;
    }
    grid[row][column]=1;
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
  triangle_of_Pascal(6U);//15 max Window
  return return_code;
}