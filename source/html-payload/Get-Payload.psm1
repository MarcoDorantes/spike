function GetPayload($email_body)
{
    $result = $email_body
    if(Select-String -InputObject $email_body -Pattern '^<html><head>')
    {
        $email_body2 = $email_body.Replace('<br>','')
        $match = Select-String -InputObject $email_body2 -Pattern '<body>(?<inner>.*)</body></html>'
        if($match)
        {
            $xml1 = Select-Xml -Content ('<html><body>' + ($match.Matches[0].Groups['inner'].Value) + '</body></html>') -XPath '/'
            if($xml1)
            {
                $payload = $xml1.Node.SelectNodes('//text()').InnerText
                $result = $payload
            }
        }
    }
    return $result
}