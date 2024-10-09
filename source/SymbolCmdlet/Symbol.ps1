class ToSymbolConverter
{
    [PSCustomObject]$asread

    ToSymbolConverter($asread)
    {
        $this.asread = $asread
    }

    [string] ReadAsSymbolID($asread)
    {
        return $asread.Replace('~','*')
    }

    [string] ReadAsPlainSymbol($asread)
    {
        $c = $this.ReadAsSymbolID($asread)
        <# $p = $c -split '/'
        return .Replace('~','*') #>
        return $c
    }


    [PSCustomObject] GetSymbolObject()
    {
        return [PSCustomObject]@{SymbolID=$this.ReadAsSymbolID($this.asread.Symbol); Count=[System.UInt32]::Parse($this.asread.Count); Symbol=$this.ReadAsPlainSymbol($this.asread.Symbol)}
    }
}

function ConvertTo-SymbolObject($asread)
{
    $converter = [ToSymbolConverter]::new($asread)
    return $converter.GetSymbolObject()
}

function Read-SymbolTrace($file)
{
    Import-Csv $file | %{ ConvertTo-SymbolObject $_ }
}