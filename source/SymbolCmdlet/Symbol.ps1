class SymbolBase
{
    [PSCustomObject]$AsImported
    [string]$ID
    [string]$StockSeries
    [string]$Emisora
    [string]$Serie
    [string]$Market
    [System.UInt32]$Counted
    [string]$ConfigurableTopicSuffix

    SymbolBase([PSCustomObject]$asread)
    {
        $this.AsImported = $asread
        $this.ID = $this.AsImported.Symbol -replace '~$','*'
    }
}

class SymbolObject : SymbolBase
{
    SymbolObject([PSCustomObject]$asread) : base($asread)
    {
        $parts = Select-String -InputObject $this.ID -Pattern '^(?<market>\w+)/(?<emisora>.+) (?<serie>.+)$'
        if(!$parts) { throw "Invalid symbol format ($($this.AsImported.Symbol))." }
        $this.StockSeries = @($parts.Matches[0].Groups['emisora'].Value, $parts.Matches[0].Groups['serie'].Value) -join ' '
        $this.Emisora = $parts.Matches[0].Groups['emisora'].Value
        $this.Serie = $parts.Matches[0].Groups['serie'].Value
        $this.Market = $parts.Matches[0].Groups['market'].Value
        $this.Counted = [System.UInt32]::Parse($this.AsImported.Count)
        $this.ConfigurableTopicSuffix = $this.AsImported.Symbol
    }
}

class HistoricSymbol : SymbolBase
{
    [DateTime]$When

    HistoricSymbol([PSCustomObject]$asread) : base($asread)
    {
        $this.ID = @($this.AsImported.Market, $this.ID) -join '/'
        $this.StockSeries = $this.AsImported.Symbol
        $parts = Select-String -InputObject $this.AsImported.Symbol -Pattern '^(?<emisora>.+) (?<serie>.+)$'
        $this.Emisora = $parts.Matches[0].Groups['emisora'].Value
        $this.Serie = $parts.Matches[0].Groups['serie'].Value
        $this.Market = $this.AsImported.Market
        $this.Counted = [System.UInt32]::Parse($this.AsImported.QuoteCount)
        $this.ConfigurableTopicSuffix = $this.ID -replace '\*$','~'
        $this.When = [DateTime]$this.AsImported.Date
    }
}

function ConvertTo-SymbolObject
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, ValueFromPipeline)]
        [PSCustomObject]$AsImported
    )
    
    process
    {
        $result = [SymbolObject]::new($AsImported)
        $result
    }
}

function ConvertTo-HistoricSymbolObject
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, ValueFromPipeline)]
        [PSCustomObject]$AsImported
    )
    
    process
    {
        $result = [HistoricSymbol]::new($AsImported)
        $result
    }
}

function Read-SymbolTrace([Parameter(Mandatory)][System.IO.FileInfo]$file)
{
    Import-Csv $file | ConvertTo-SymbolObject
}

function Read-SymbolHistory([Parameter(Mandatory)][System.IO.FileInfo]$file)
{
    Import-Csv $file | ConvertTo-HistoricSymbolObject
}
<#
$ss = Read-SymbolTrace .\source\SymbolCmdlet\Symbols-20241007-171300.csv
$ss | sort -desc Counted -Stable| ? {$_.Market -eq 'NAC' -and $_.Emisora.StartsWith('O')}|select ID,Counted|measure -Sum Counted

cat $home\Downloads\MD_Tracked-Symbol-Quote-Utilization_2024-10-12_ByDate.csv -first 2
"Date","Symbol","Market","QuoteCount"
"6/30/2020 5:14:01 PM","MMM *","SIC","164"
#>