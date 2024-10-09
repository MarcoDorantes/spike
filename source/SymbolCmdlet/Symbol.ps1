class SymbolObject
{
    [PSCustomObject]$AsImported
    [string]$ID
    [string]$StockSeries
    [string]$Emisora
    [string]$Serie
    [string]$Market
    [System.UInt32]$Counted

    SymbolObject([PSCustomObject]$asread)
    {
        $this.AsImported = $asread
        $this.ID = $this.AsImported.Symbol -replace '~$','*'
        $parts = Select-String -InputObject $this.ID -Pattern '^(?<market>\w+)/(?<emisora>.+) (?<serie>.+)$'
        if(!$parts) { throw "Invalid symbol format ($($this.AsImported.Symbol))." }
        $this.StockSeries = @($parts.Matches[0].Groups['emisora'].Value, $parts.Matches[0].Groups['serie'].Value) -join ' '
        $this.Emisora = $parts.Matches[0].Groups['emisora'].Value
        $this.Serie = $parts.Matches[0].Groups['serie'].Value
        $this.Market = $parts.Matches[0].Groups['market'].Value
        $this.Counted = [System.UInt32]::Parse($this.AsImported.Count)
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

function Read-SymbolTrace($file)
{
    Import-Csv $file | ConvertTo-SymbolObject
}