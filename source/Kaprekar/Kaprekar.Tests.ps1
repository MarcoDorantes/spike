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
}