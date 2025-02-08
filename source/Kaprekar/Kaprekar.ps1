function GetSumA([int]$n)
{
    $sorted = ([System.Linq.Enumerable]::OrderDescending([System.Linq.Enumerable]::ToArray("$n"))) -join ''
    return [int]$sorted
}

function GetSumB([int]$n)
{
    $sorted = ([System.Linq.Enumerable]::Order([System.Linq.Enumerable]::ToArray("$n"))) -join ''
    return [int]$sorted
}

function GetSum([int]$n) { return (GetSumA $n) - (GetSumB $n) }