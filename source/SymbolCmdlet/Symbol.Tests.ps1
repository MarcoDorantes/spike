BeforeAll {
    . $PSScriptRoot\Symbol.ps1
}

Describe Parse1 {
    It SymbolID {
        $asread = 'NAC/GCARSO A1'
        $converter = [ToSymbolConverter]::new($null)
        $symbolid = $converter.ReadAsSymbolID($asread)
        $symbolid | Should -Be 'NAC/GCARSO A1'
    }
}

Describe Load {
    BeforeAll {
        $symbol = Read-SymbolTrace "$home\Downloads\Symbols-20241008-171300.csv"
    }

    It load1 {
        $symbol | Should -Not -BeNullOrEmpty
    }

    It load1 {
        $symbol[0].SymbolID | Should -Be 'NAC/GCARSO A1'
#        $symbol[0].StockSeries | Should -Be 'GCARSO A1'
#        $symbol[0].Emisora | Should -Be 'GCARSO'
#        $symbol[0].Serie | Should -Be 'A1'
#        $symbol[0].Market | Should -Be 'NAC'
        $symbol[0].Count | Should -Be 29232
    }
}