Imports System

Module TriangleOfPascal

Function Factorial(ByVal n As Integer) As Integer
    if n<=1 Then
      Return 1
    End If
    Return n*Factorial(n-1)
End Function

Function Pascal(ByVal n As Integer, ByVal k As Integer) As Integer
  Return Cint(Math.Floor(Factorial(n) / (Factorial(k) * Factorial(n-k))))
End Function

Sub triangle_of_Pascal_v2(rows as Integer)
	If rows < 1 Then
		Return
	End If
  Console.WriteLine("Pascal's triangle of " & rows & " rows:")
  If rows = 1 Then
    Console.WriteLine(rows)
    Return
  End If
  Dim grid As New List(Of List(Of Integer))

  grid.Add(New List(Of Integer))
  grid(0).Add(1)
  Dim row as Integer = 1
  Dim columns as Integer = 2
  While row<rows
    grid.Add(New List(Of Integer))
    Dim col as Integer = 0
    While col<columns
      grid(row).Add(Pascal(row,col))
      col = col+1
    End While
    row = row+1
    columns = columns+1
  End While

  row=0
  Dim tabs as Integer = rows-1
  While row<rows
    Console.Write(New String(Chr(9), tabs))
    Dim col as Integer = 0
    While col<grid(row).Count
      If col>0 Then
        Console.Write(New String(Chr(9), 2))
      End If
      Console.Write(grid(row)(col))
      col = col+1
    End While
    Console.WriteLine
    row = row+1
    tabs = tabs-1
  End While
End Sub

Sub triangle_of_Pascal_v1(rows as Integer)
	If rows < 1 Then
		Return
	End If
  Console.WriteLine("Pascal's triangle of " & rows & " rows:")
  If rows = 1 Then
    Console.WriteLine(rows)
    Return
  End If
  Dim columns as Integer = (rows * 2) - 1
  Dim add = Function(whole, n)
    whole.Add(new List(Of Integer)(System.Linq.Enumerable.Repeat(0,columns)))
    Return whole
  End Function
  Dim grid As List(Of List(Of Integer)) = System.Linq.Enumerable.Range(1, rows).Aggregate(new List(Of List(Of Integer)), add )

  Dim row_start_index as Integer = Cint(Math.Floor(columns/2))
  grid(0)(row_start_index)=1
  grid(1)(row_start_index+1)=1
  row_start_index = row_start_index-1
  grid(1)(row_start_index)=1
  Dim row as Integer = 2
  While row<rows
    Dim row_value_count as Integer = row+1
    Dim row_sums as Integer = row_value_count-2
    row_start_index = row_start_index-1
    grid(row)(row_start_index)=1
    Dim col as Integer = row_start_index+2
    Dim sums_count as Integer=0
    While col<columns And sums_count<row_sums
      grid(row)(col)=grid(row-1)(col-1) + grid(row-1)(col+1)
      col=col+2
      sums_count=sums_count+1
    End While
    grid(row)(col)=1
    row = row+1
  End While
 
  For Each rowline as List(Of Integer) in grid
    For Each v as Integer in rowline
      if v=0 Then
        Console.Write(Chr(9))
      Else
        Console.Write(v & Chr(9))
      End If
    Next
    Console.WriteLine()
  Next
End Sub

Sub found1(rows As Integer)
		Dim triangle As New List(Of List(Of Integer))()

		' Initialize the triangle
		For i As Integer = 0 To rows - 1
				Dim row As New List(Of Integer)()
				For j As Integer = 0 To i
						If j = 0 Or j = i Then
								row.Add(1) ' The first and last elements of each row are 1
						Else
								row.Add(triangle(i - 1)(j - 1) + triangle(i - 1)(j)) ' Sum of the two elements above
						End If
				Next
				triangle.Add(row)
		Next

		' Print the triangle
		For i As Integer = 0 To rows - 1
			Console.Write(New String(" "c, (rows - i - 1) * 2)) ' Add leading spaces
			Console.WriteLine(String.Join(" ", triangle(i)))
		Next
End Sub
Sub found2(rows As Integer)
		Dim triangle(rows - 1)() As Integer

		' Initialize the triangle
		For i As Integer = 0 To rows - 1
				triangle(i) = New Integer(i) {}
		Next

		' Build the triangle
		For i As Integer = 0 To rows - 1
				triangle(i)(0) = 1
				triangle(i)(i) = 1
				For j As Integer = 1 To i - 1
						triangle(i)(j) = triangle(i - 1)(j - 1) + triangle(i - 1)(j)
				Next
		Next

		' Display the triangle
		For i As Integer = 0 To rows - 1
				' Print leading spaces for symmetry
				Console.Write(New String(" "c, (rows - i - 1) * 2))
				For j As Integer = 0 To i
						Console.Write(triangle(i)(j) & " ")
				Next
				Console.WriteLine()
		Next
End Sub

Sub Main(args As String())
  triangle_of_Pascal_v1(8)
  triangle_of_Pascal_v2(8)
  'found1(8)
  'found2(8)
End Sub

End Module