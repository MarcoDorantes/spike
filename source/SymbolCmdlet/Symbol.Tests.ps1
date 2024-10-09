BeforeAll {
    . $PSScriptRoot\Symbol.ps1
}

Describe Load {
    BeforeAll {
        $symbol = Read-SymbolTrace $PSScriptRoot\Symbols-20241008-171300.csv
    }

    It load1 {
        $symbol | Should -Not -BeNullOrEmpty
    }

    It size {
        $symbol.Count | Should -Be 1254
    }

    It load1 {
        $symbol[0].ID | Should -Be 'NAC/GCARSO A1'
        $symbol[0].StockSeries | Should -Be 'GCARSO A1'
        $symbol[0].Emisora | Should -Be 'GCARSO'
        $symbol[0].Serie | Should -Be 'A1'
        $symbol[0].Market | Should -Be 'NAC'
        $symbol[0].Counted | Should -Be 29232
    }
}