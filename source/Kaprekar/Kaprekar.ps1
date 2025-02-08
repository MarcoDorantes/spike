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

function GetSum   ([int]$n) { return (GetSumA $n) - (GetSumB $n) }
function GetSum_v1([int]$n) { return (GetSumA $n) - (GetSumB $n) }

function GetSum_v2([int]$n)
{
    [int]$a = GetSumA $n
    [int]$b = GetSumB $n
    if($a -gt $b) { return ($a - $b) }
    else { return ($b - $a) }
}

function GetSumCount
{
    [CmdletBinding()]
    param([Parameter(Mandatory,ValueFromPipeline)][int]$n, [int]$limit = 7)

    [int]$r = $n
    [int]$c = 0
    do
    {
        $r = GetSum_v1 $r
        ++$c
        if($c -ge $limit) { return [int](-1) }
    }
    while($r -ne 6174)
    return $c
}