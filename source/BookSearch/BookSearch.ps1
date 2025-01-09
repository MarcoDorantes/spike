[CmdletBinding()]
param(
    [Parameter(ParameterSetName='f', Mandatory, ValueFromPipeline, Position=0)][System.IO.FileInfo]$xlsx,
    [Parameter(ParameterSetName='p', Mandatory, ValueFromPipeline, Position=0)][string]$filepath
)

$dll = Join-Path $PSScriptRoot 'WritersControllerOpenXml\WritersControllerOpenXml.dll'
$dll
if(Test-Path $dll) { Add-Type -Path $dll } else { throw "No loaded $dll" }

class WnBookA
{
    [string]$Title
    [string]$Author
    [string]$ISBN
    [string]$Publisher 
}

class WnBook
{
    [string]$Title
    [string]$Subtitle
    [string]$Author
}

#$xlsx = ls \\tsclient\F\tep\BookIndex\BookIndex.xlsx
$excel = [WritersControllerOpenXml.Excel]::new($xlsx);
if($excel)
{
#var name = store.GetTabNames().First();
#var read = store.ReadTabObjects<Book0>(name).ToList();
    $names = $excel.GetTabNames()
    $index = $names | select -First 1
    "Tab: $index"
    $read = $excel.ReadTabObjects[WnBook]($index)
    $books = $read | ?{$_.Instance -ne $null} | select -expand Instance
    $books | ft
} else {'no-load'}