param([switch]$startmode, [uint]$trylimit = 9)

$MASTERMIND_VERSION = New-Object -TypeName System.Version -ArgumentList '1.0.0'

function Get-GameVersion
{
    $MASTERMIND_VERSION
}

function mainmaster
{
    $MASTERMIND_VERSION
}

function Get-GameCode
{
    do
    {
        $result = Get-SecureRandom -Minimum 0 -Maximum 9 -Count 4
        $diff_count = $result|sort|Get-Unique|measure|select -expand Count
    }while($diff_count -ne 4)
    return $result
}

function Get-Guess($guess_input)
{
    $result = $null
    if($guess_input -match '(?<digit1>\d)\W+(?<digit2>\d)\W+(?<digit3>\d)\W+(?<digit4>\d)')
    {
        $items = @($Matches.digit1, $Matches.digit2, $Matches.digit3, $Matches.digit4)
        $x = $null
        $digits = @($items | ?{[System.UInt32]::TryParse($_, [ref]$x) })
        if($digits.Length -eq 4 -and (($digits|sort|Get-Unique).Length -eq 4))
        {
            $result = $digits
        }
    }
    return $result
}

function Get-Hint($guess, $toguess)
{
    $white = 0
    $red = 0
    for($k = 0; $k -lt 4; ++$k)
    {
        if($guess[$k] -eq $toguess[$k]) { ++$red }
        elseif($guess[$k] -in $toguess) { ++$white }
    }
    return [PSCustomObject]@{Whites=$white; Reds=$red}
}

function TopEntryPoint
{
    $toguess = Get-GameCode
#$toguesspattern = $toguess -join '|'
#$toguess -join ' '
    [int]$trycount = 0
    do {
        ++$trycount
        $guess_input = Read-Host -Prompt "[Your try count: $($trycount)] Which are the four numbers?"
        $guess = Get-Guess $guess_input
        if(!$guess) { continue }
        if([System.Linq.Enumerable]::SequenceEqual([object[]]$guess,[object[]]$toguess)) { "`nYou won in $trycount tries!!!`n"; break; }
        if($trycount -eq $trylimit) { "`nYou reached the try limit of $($trylimit): The game won!!!`nThe number were $($toguess -join ' ')`n"; break;  }
        $hint = Get-Hint $guess $toguess
        "`nHint> Whites: $($hint.Whites) Reds: $($hint.Reds)`n"
<#
        [System.Linq.Enumerable]::Intersect([object[]]$guess,[object[]]$toguess)

        $guesspattern = $guess -join '|'
        $comp = [System.Text.RegularExpressions.Regex]::Matches($guesspattern, $toguesspattern)
#>
    }while($true)
}

if($startmode)
{
    TopEntryPoint
}