BeforeAll {
    . $PSScriptRoot\Symbol.ps1
}

Describe LoadCount1 {
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

Describe LoadCount2 {
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

Describe derivation {
    It counted1 {
        $asread = [PSCustomObject]@{Symbol='NAC/GCARSO A1'; Count=2}
        $symbol = [SymbolObject]::new($asread)
        $symbol.ID | Should -Be 'NAC/GCARSO A1'
        $symbol.StockSeries | Should -Be 'GCARSO A1'
        $symbol.Emisora | Should -Be 'GCARSO'
        $symbol.Serie | Should -Be 'A1'
        $symbol.Market | Should -Be 'NAC'
        $symbol.Counted | Should -Be 2
        $symbol.ConfigurableTopicSuffix | Should -Be 'NAC/GCARSO A1'
    }

    It historic1 {
        $asread = [PSCustomObject]@{
            Date='6/30/2020 5:14:01 PM'
            Symbol='MMM *'
            Market='SIC'
            QuoteCount=164
        }
        $symbol = [HistoricSymbol]::new($asread)
        $symbol.ID | Should -Be 'SIC/MMM *'
        $symbol.StockSeries | Should -Be 'MMM *'
        $symbol.Emisora | Should -Be 'MMM'
        $symbol.Serie | Should -Be '*'
        $symbol.Market | Should -Be 'SIC'
        $symbol.Counted | Should -Be 164
        $symbol.ConfigurableTopicSuffix | Should -Be 'SIC/MMM ~'
        '{0:s}' -f $symbol.When | Should -Be '2020-06-30T17:14:01'
    }
}

Describe LoadHistory1 {
    BeforeAll {
        $history = Read-SymbolHistory $home\Downloads\MD_Tracked-Symbol-Quote-Utilization_2024-07-31_ByDate.csv
    }

    It load1 {
        $history | Should -Not -BeNullOrEmpty
    }

    It historic1 {
        $symbol = $history | select -Last 1
        $symbol.ID | Should -Be 'SIC/TXN *'
        $symbol.StockSeries | Should -Be 'TXN *'
        $symbol.Emisora | Should -Be 'TXN'
        $symbol.Serie | Should -Be '*'
        $symbol.Market | Should -Be 'SIC'
        $symbol.Counted | Should -Be 1
        $symbol.ConfigurableTopicSuffix | Should -Be 'SIC/TXN ~'
        '{0:s}' -f $symbol.When | Should -Be '2024-07-31T17:14:00'
    }
}