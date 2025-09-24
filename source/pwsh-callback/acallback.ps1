Set-StrictMode -Version 3.0

function FF([scriptblock]$f, [scriptblock]$g)
{
    'here'
    if($f)
    {
       #$f.GetType().FullName # System.Management.Automation.ScriptBlock
       $r = $f.Invoke($null)
       if($r){"`t`$f = $f"}else{"`t`$f = null"}
    }
    if($g)
    {

        $opt = @(1,'name1')
        $x = $g.Invoke($opt)
        if($x)
        {
            "`t`$x ($($x.Count)):`n`t`t$($x -join "`n`t`t")"
        }
    }
}

FF {Write-Host 'here2'} {param([int]$n, [string]$name) Write-Host "$n = $name"; return @(1..3)}