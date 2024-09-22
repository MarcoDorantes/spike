BeforeAll {
    . $PSScriptRoot/mastermind.ps1
}

Describe S {
    It X {
       Get-GameVersion | Should -Not -BeNullOrEmpty
    }

    It C {
        (Get-GameCode | sort | Get-Unique).Length | Should -Be 4
    }

    It G {
        $guess_input = '4 3,2 1'
        $guess_input -match '(?<digit1>\d)\W+(?<digit2>\d)\W+(?<digit3>\d)\W+(?<digit4>\d)'
        $Matches | Should -BeTrue
        $Matches.digit1 | Should -Be '4'
        $Matches.digit2 | Should -Be '3'
        $Matches.digit3 | Should -Be '2'
        $Matches.digit4 | Should -Be '1'
    }

    It G {
        $guess_input = '4 3,2 1'
        $guess_input -match '(?<digit1>\d)\W+(?<digit2>\d)\W+(?<digit3>\d)\W+(?<digit4>\d)'
        $Matches | Should -BeTrue
        $Matches.digit1 | Should -Be '4'
        $Matches.digit2 | Should -Be '3'
        $Matches.digit3 | Should -Be '2'
        $Matches.digit4 | Should -Be '1'
        $items = @($Matches.digit1, $Matches.digit2, $Matches.digit3, $Matches.digit4)
        $x = $null
        $digits = @($items | ?{[System.UInt32]::TryParse($_, [ref]$x) })
        $digits.Length | Should -Be 4
    }

    It G {
        $guess_input = '4 3,2'
        $comp = $guess_input -match '(?<digit1>\d)\W+(?<digit2>\d)\W+(?<digit3>\d)\W+(?<digit4>\d)'
        $comp | Should -BeFalse
    }

    It G {
        $guess_input = '4 3,2 1'
        (Get-Guess $guess_input | sort | Get-Unique).Length | Should -Be 4
    }

    It G {
        $toguess = Get-GameCode
        $x = $null
        $digits = @($toguess | ?{[System.UInt32]::TryParse($_, [ref]$x) })
        $digits.Length | Should -Be 4
    }

    It G {
        $toguess = Get-GameCode
        $guess = Get-Guess '4 3 2 1'
        [System.Linq.Enumerable]::SequenceEqual([object[]]$guess,[object[]]$toguess) | Should -BeFalse
    }

    It G {
        $hint = Get-Hint @(4,3,2,1) @(1,2,3,4)
        $hint.Whites | Should -Be 4
        $hint.Reds | Should -Be 0
    }

    It G {
        $hint = Get-Hint @(7,2,5,1) @(1,2,3,4)
        $hint.Whites | Should -Be 1
        $hint.Reds | Should -Be 1
    }
}