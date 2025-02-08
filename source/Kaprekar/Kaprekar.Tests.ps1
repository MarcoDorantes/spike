BeforeAll {
    . $PSScriptRoot\Kaprekar.ps1
}

Describe addends {
    BeforeAll {
        [int]$n = 1234
    }
    It x {
        $n -is [int] | Should -BeTrue
    }
    It sumA {
        GetSumA $n | Should -Be 4321
    }
    It sumA {
        GetSumA 4321 | Should -Be 4321
    }
    It sumA {
        GetSumA 6174 | Should -Be 7641
    }
    It sumA {
        (GetSumA $n) -is [int] | Should -BeTrue
    }

    It sumB {
        GetSumB $n | Should -Be 1234
    }
    It sumB {
        GetSumB 4321 | Should -Be 1234
    }
    It sumB {
        GetSumB 6174 | Should -Be 1467
    }
    It sumB {
        (GetSumB $n) -is [int] | Should -BeTrue
    }
}

Describe sums {
    It sum1 {
        GetSum 6174 | Should -Be 6174
    }
    It sum1 {
        GetSum 1234 | Should -Be 3087
    }
    It sum1 {
        GetSum 3087 | Should -Be 8352
    }
    It sum1 {
        GetSum 8352 | Should -Be 6174
    }
    It sumcount {
        GetSumCount 1234 | Should -Be 3
    }
    It sumcount {
        GetSumCount 3087 | Should -Be 2
    }
    It sumcount {
        GetSumCount 8352 | Should -Be 1
    }
    It sumcount {
        GetSumCount 1000 7000 | Should -Be -1
    }
    It sumcount {
        GetSumCount 9998 7000 | Should -Be -1
    }
    It sumcount {
        1000 | GetSumCount | Should -Be -1
    }
    It sumcount {
        1000..9999 | ?{ (GetSumCount $_) -eq -1} | measure | select -expand Count | Should -Be 2057
    }
    It sumcount {
        1000..9999 | ?{ (GetSumCount $_) -ne -1} | measure | select -expand Count | Should -Be (9000-2057)
    }
    It sumcount {
        1000..9999 | ?{ (GetSumCount $_) -ne -1} | select -last 3| Join-String -Separator '|' | Should -Be '9995|9996|9997'
    }
    It sumcount {
        1..1000 | ?{ (GetSumCount $_) -ne -1} | select -last 3| Join-String -Separator '|' | Should -Be ''
    }
}