param([switch]$startmode, [switch]$separator, [uint]$trylimit = 12)

$MASTERMIND_VERSION = New-Object -TypeName System.Version -ArgumentList '1.0.0'
$VALID_INPUT_KEYS_ARRAY = @([System.Enum]::GetNames([System.ConsoleKey]) | ? {$_ -match 'NumPad' -or $_ -match '^D\d' -or $_ -eq 'Spacebar'})

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

function Get-Guess($guess_input, [switch]$separator)
{
    $result = $null
    if($separator.IsPresent) { $guess_text = $guess_input }
    else { $guess_text = "$($guess_input)".GetEnumerator() | Join-String -Separator ' ' }
    if($guess_text -match '(?<digit1>\d)\W+(?<digit2>\d)\W+(?<digit3>\d)\W+(?<digit4>\d)')
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

function Read-GuessKeys
{
    $result = $null
    $keys = New-Object -TypeName System.Collections.ArrayList
    do
    {
        $k = [console]::ReadKey()
        if($k.Key -eq [System.ConsoleKey]::Enter)
        {
            $text = New-Object -TypeName System.Text.StringBuilder
            foreach($c in $keys) {$text.Append($c); Write-Host $c}
            $result = "$($text)" 
            break
        }
        if("$($k.Key)" -in $VALID_INPUT_KEYS_ARRAY -and $_.KeyChar -notin $keys) { $keys.Add($k.KeyChar) | Out-Null }
    }while($true)
    return $result
}

function Read-Guess($Prompt)
{
    $result = $null
    Write-Host -NoNewline $Prompt
    $result = Read-GuessKeys
    <#
    $text = New-Object -TypeName System.Text.StringBuilder
    do
    {
        $k = [console]::ReadKey()
        if($k.Key -eq [System.ConsoleKey]::Enter)
        {
            $result = "$($text)"
            break
        }
        if("$($k.Key)" -in $VALID_INPUT_KEYS_ARRAY) { $text.Append($k.KeyChar) }
    }while($true)
    #>
    return $result
}

function TopEntryPoint
{
    "`n"
    $toguess = Get-GameCode
#$toguess -join ' '
    $askguess = 'Which are the four numbers? : '
    $askguesslength = New-Object -TypeName System.String -ArgumentList @(' ', $askguess.Length)
    [int]$trycount = 0
    do {
        if($trycount -ge $trylimit) { "`nYou reached the attempts limit of $($trylimit): The game won!!!`nThe numbers were $($toguess -join ' ')`n"; break;  }
#        $guess_input = Read-Host -Prompt $askguess
        $guess_input = Read-Guess -Prompt $askguess
#        Write-Host "[$($guess_input)]"
#        return
        $guess = Get-Guess $guess_input $separator
        ++$trycount
        if(!$guess) { continue }
        if([System.Linq.Enumerable]::SequenceEqual([object[]]$guess,[object[]]$toguess)) { "`nYou won in $trycount attempts!!!`n"; break; }
        $trylabel = "Attempt #$('{0,-2}' -f $trycount)>"
        $hint = Get-Hint $guess $toguess
#        "$($askguesslength)       $($trylabel) Misplaced: $($hint.Whites) Exact: $($hint.Reds)"
        Write-Host "`r$($askguess)$($guess_input) $($trylabel) Misplaced: $($hint.Whites) Exact: $($hint.Reds)"
    }while($true)
}

if($startmode)
{
    TopEntryPoint
}