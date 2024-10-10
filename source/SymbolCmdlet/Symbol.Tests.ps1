BeforeAll {
    . $PSScriptRoot\Symbol.ps1
}

Describe Load1 {
    BeforeAll {
        $symbol = Read-SymbolTrace $PSScriptRoot\Symbols-20241007-171300.csv
    }

    It load1 {
        $symbol | Should -Not -BeNullOrEmpty
    }

    It size {
        $symbol.Count | Should -Be 1277
    }

    It symbol0 {
        $symbol[0].ID | Should -Be 'NAC/GCARSO A1'
        $symbol[0].StockSeries | Should -Be 'GCARSO A1'
        $symbol[0].Emisora | Should -Be 'GCARSO'
        $symbol[0].Serie | Should -Be 'A1'
        $symbol[0].Market | Should -Be 'NAC'
        $symbol[0].Counted | Should -Be 20480
        $symbol[0].ConfigurableTopicSuffix | Should -Be 'NAC/GCARSO A1'
    }

    It symbolx {
        $symbolx = $symbol | ? ID -eq 'NAC/ORBIA *'
        $symbolx.ID | Should -Be 'NAC/ORBIA *'
        $symbolx.StockSeries | Should -Be 'ORBIA *'
        $symbolx.Emisora | Should -Be 'ORBIA'
        $symbolx.Serie | Should -Be '*'
        $symbolx.Market | Should -Be 'NAC'
        $symbolx.Counted | Should -Be 111808
        $symbolx.ConfigurableTopicSuffix | Should -Be 'NAC/ORBIA ~'
    }
}

Describe Load2 {
    BeforeAll {
        $symbol = Read-SymbolTrace $PSScriptRoot\Symbols-20241008-171300.csv
    }

    It load1 {
        $symbol | Should -Not -BeNullOrEmpty
    }

    It size {
        $symbol.Count | Should -Be 1254
    }

    It symbol0 {
        $symbol[0].ID | Should -Be 'NAC/GCARSO A1'
        $symbol[0].StockSeries | Should -Be 'GCARSO A1'
        $symbol[0].Emisora | Should -Be 'GCARSO'
        $symbol[0].Serie | Should -Be 'A1'
        $symbol[0].Market | Should -Be 'NAC'
        $symbol[0].Counted | Should -Be 29232
        $symbol[0].ConfigurableTopicSuffix | Should -Be 'NAC/GCARSO A1'
    }

    It symbolx {
        $symbolx = $symbol | ? ID -eq 'NAC/ORBIA *'
        $symbolx.ID | Should -Be 'NAC/ORBIA *'
        $symbolx.StockSeries | Should -Be 'ORBIA *'
        $symbolx.Emisora | Should -Be 'ORBIA'
        $symbolx.Serie | Should -Be '*'
        $symbolx.Market | Should -Be 'NAC'
        $symbolx.Counted | Should -Be 101476
        $symbolx.ConfigurableTopicSuffix | Should -Be 'NAC/ORBIA ~'
    }
}