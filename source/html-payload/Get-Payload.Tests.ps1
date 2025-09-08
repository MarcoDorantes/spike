Describe x {
    It here-string {
        $email_body = @'
'@
        $email_body | Should -BeNullOrEmpty
        $email_body -is [string] | Should -BeTrue
        $email_body.Length | Should -Be 0
        [string]::Compare($email_body, [string]::Empty) -eq 0 | Should -BeTrue
    }
    It here-string {
        $email_body = @'
<html>
'@
        $email_body | Should -Be '<html>'
        $email_body -is [string] | Should -BeTrue
        $email_body.Length | Should -Be 6
        [string]::Compare($email_body, '<html>') -eq 0 | Should -BeTrue
    }
    It here-string {
        $email_body = @'
<html>

'@
        $email_body | Should -Be "<html>`r`n"
        $email_body -is [string] | Should -BeTrue
        $email_body.Length | Should -Be 8
        [string]::Compare($email_body, "<html>`r`n") -eq 0 | Should -BeTrue
    }
    It here-string-split {
        $email_body = @'
<html>

'@
        $lines = $email_body -split "`n"
        $lines -is [string[]] | Should -BeTrue
        $lines.Count| Should -Be 2
        $lines[0].Length | Should -Be 7
        [string]::Compare($lines[0], "<html>`r") -eq 0 | Should -BeTrue
    }
    It html {
        $email_body = @'
<html>

'@
        $match = Select-String -InputObject $email_body -Pattern '<html>' -SimpleMatch
        [bool]$match | Should -BeTrue
    }
    It HTML {
        $email_body = @'
<HTML>

'@
        $match = Select-String -InputObject $email_body -Pattern '<html>' -SimpleMatch
        [bool]$match | Should -BeTrue
    }
    It html-payload {
        $email_body = @'
<html><head></head><body>Get-Date</body></html>

'@
        $match = Select-String -InputObject $email_body -Pattern '<html>' -SimpleMatch
        [bool]$match | Should -BeTrue
    }
    It html-payload {
        $email_body = @'
<html><head>

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"></head><body><div><span style="font-size: 21.333334px;">Get-Date</span></div></body></html>

'@
        $match = Select-String -InputObject $email_body -Pattern '<html>' -SimpleMatch
        [bool]$match | Should -BeTrue
    }
    It body-payload {
        $email_body = @'
<html><head>

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"></head><body><div><span style="font-size: 21.333334px;">Get-Date</span></div></body></html>

'@
        $match = Select-String -InputObject $email_body -Pattern '<body>(?<inner>.*)</body></html>'
        [bool]$match | Should -BeTrue
        $match.Matches[0].Groups['inner'].Value | Should -Be '<div><span style="font-size: 21.333334px;">Get-Date</span></div>'
    }
    It xml-payload {
        $email_body = @'
<html><head>

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"></head><body><div><span style="font-size: 21.333334px;">Get-Date</span></div></body></html>

'@
        $match = Select-String -InputObject $email_body -Pattern '<body>(?<inner>.*)</body></html>'
        [bool]$match | Should -BeTrue
        $match.Matches[0].Groups['inner'].Value | Should -Be '<div><span style="font-size: 21.333334px;">Get-Date</span></div>'
        $xml1 = Select-Xml -Content ('<html><body>' + ($match.Matches[0].Groups['inner'].Value) + '</body></html>') -XPath '/'
        $xml1 | Should -Not -BeNullOrEmpty
        $payload = $xml1.Node.SelectSingleNode('//text()').InnerText
        $payload | Should -Be 'Get-Date'
        $payload -is [string]| Should -BeTrue
    }
    It xml-payload {
        $email_body = @'
<html><head>

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"></head><body><div><span style="font-size: 21.333334px;">Get-Date</span></div></body></html>

'@
        $match = Select-String -InputObject $email_body -Pattern '<body>(?<inner>.*)</body></html>'
        [bool]$match | Should -BeTrue
        $match.Matches[0].Groups['inner'].Value | Should -Be '<div><span style="font-size: 21.333334px;">Get-Date</span></div>'
        $xml1 = Select-Xml -Content ('<html><body>' + ($match.Matches[0].Groups['inner'].Value) + '</body></html>') -XPath '/'
        $xml1 | Should -Not -BeNullOrEmpty
        $payload = $xml1.Node.SelectNodes('//text()').InnerText
        $payload | Should -Be 'Get-Date'
        $payload -is [string]| Should -BeTrue
    }
#
    It xml-payload {
        $email_body = @'
<html><head>

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"></head><body><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;"><br></div><div id="ms-outlook-mobile-body-separator-line" dir="ltr"><div style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;">Get-Date</div><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;"><br></div><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;">Third line</div><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;">Get-Date</div><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;"><br></div></div><div id="ms-outlook-mobile-signature" dir="ltr"></div></body></html>

'@
        $email_body2 = $email_body.Replace('<br>','')
        $match = Select-String -InputObject $email_body2 -Pattern '<body>(?<inner>.*)</body></html>'
        [bool]$match | Should -BeTrue
        $match.Matches[0].Groups['inner'].Value | Should -Be '<div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;"></div><div id="ms-outlook-mobile-body-separator-line" dir="ltr"><div style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;">Get-Date</div><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;"></div><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;">Third line</div><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;">Get-Date</div><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;"></div></div><div id="ms-outlook-mobile-signature" dir="ltr"></div>'
        $xml1 = Select-Xml -Content ('<html><body>' + ($match.Matches[0].Groups['inner'].Value) + '</body></html>') -XPath '/'
        $xml1 | Should -Not -BeNullOrEmpty
        $payload = $xml1.Node.SelectSingleNode('//text()').InnerText
        $payload | Should -Be 'Get-Date'
        $payload -is [string]| Should -BeTrue
    }
    It xml-payload {
        $email_body = @'
<html><head>

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"></head><body><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;"><br></div><div id="ms-outlook-mobile-body-separator-line" dir="ltr"><div style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;">Get-Date</div><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;"><br></div><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;">Third line</div><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;">Get-Date</div><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;"><br></div></div><div id="ms-outlook-mobile-signature" dir="ltr"></div></body></html>

'@
        $email_body2 = $email_body.Replace('<br>','')
        $match = Select-String -InputObject $email_body2 -Pattern '<body>(?<inner>.*)</body></html>'
        [bool]$match | Should -BeTrue
        $match.Matches[0].Groups['inner'].Value | Should -Be '<div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;"></div><div id="ms-outlook-mobile-body-separator-line" dir="ltr"><div style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;">Get-Date</div><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;"></div><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;">Third line</div><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;">Get-Date</div><div dir="ltr" style="font-family: &quot;Segoe UI&quot;, &quot;Segoe UI_MSFontService&quot;, -apple-system, Roboto, Arial, Helvetica, sans-serif; font-size: 12pt;"></div></div><div id="ms-outlook-mobile-signature" dir="ltr"></div>'
        $xml1 = Select-Xml -Content ('<html><body>' + ($match.Matches[0].Groups['inner'].Value) + '</body></html>') -XPath '/'
        $xml1 | Should -Not -BeNullOrEmpty
        $payload = $xml1.Node.SelectNodes('//text()').InnerText
        $payload | Should -Be @('Get-Date','Third line','Get-Date')
        $payload -is [object[]]| Should -BeTrue
    }
}