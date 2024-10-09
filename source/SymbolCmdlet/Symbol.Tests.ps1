BeforeAll {
    . $PSScriptRoot\Symbol.ps1
}

Describe Parse1 {
    It SymbolID {
        $asread = [PSCustomObject]@{Symbol='NAC/GCARSO A1'; Count=0}
        $symbol = [SymbolObject]::new($asread)
        $symbol.ID | Should -Be 'NAC/GCARSO A1'
    }

    It replace1 {
        'NAC/GCARSO A1' -replace '~$','*' | Should -Be 'NAC/GCARSO A1'
    }

    It replace2 {
        'NAC/GCARSO ~' -replace '~$','*' | Should -Be 'NAC/GCARSO *'
    }

    It parts1 {
        $parts = Select-String -InputObject 'NAC/GCARSO A1' -Pattern '^(?<market>\w+)/(?<emisora>\w+) (?<serie>.+)$'
        $parts.Matches[0].Groups['market'].Value | Should -Be 'NAC'
        $parts.Matches[0].Groups['emisora'].Value | Should -Be 'GCARSO'
        $parts.Matches[0].Groups['serie'].Value | Should -Be 'A1'
    }
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