param([Parameter(Mandatory, Position=0)][System.IO.FileInfo]$xlsx)

$dll = Join-Path $PSScriptRoot 'WritersControllerOpenXml\WritersControllerOpenXml.dll'
$dll
if(Test-Path $dll) { Add-Type -Path $dll } else { throw "No loaded $dll" }

class WnRow
{
    [DateTime]$When
    [string]$Outbound
    [string]$Inbound
    [string]$Balance
    [string]$DocNr
    [string]$Subject
}

function ReadRows([WritersControllerOpenXml.Excel]$excel)
{
    $names = $excel.GetTabNames()
    $index = $names | select -First 1
    "Tab: $index"
    $read = $excel.ReadTabObjects[WnRow]($index)
    $prob = $read | ?{$_.Errors -ne $null} | select -expand Errors
    if($prob) {"Problems: " + ($prob -join "`n")}
    $read | ?{$_.Instance -ne $null} | select -expand Instance
}

try
{
    [WritersControllerOpenXml.Excel]$excel = [WritersControllerOpenXml.Excel]::new($xlsx)
    $rows = ReadRows $excel
    $rows | ? Subject -match 'retiro' | ft
#   $rows | ? Subject -match 'telmex' | measure -Sum Outbound
}
catch
{
    "Problem: $_"
    $error | Get-Error
}
finally
{
    $excel.Dispose()
}