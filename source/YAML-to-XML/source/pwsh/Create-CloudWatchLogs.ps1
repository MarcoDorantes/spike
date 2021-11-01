param
(
  [Parameter(Mandatory=$true)]
  [System.IO.FileInfo]
  $resources_file
)

$resources = C:\temp\host.exe $resources_file | Join-String

$loggroups = Select-Xml -Content $resources -XPath "/yaml/map/entry/key[child::text()='Resources']/parent::node()/value/map/entry[value/map/entry/key/child::text()='Type' and value/map/entry/value/child::text()='AWS::Logs::LogGroup']" | select -ExpandProperty Node | select -ExpandProperty key
foreach($loggroup in $loggroups)
{
    $retention_days = Select-Xml -Content $resources -XPath "/yaml/map/entry/key[child::text()='Resources']/parent::node()/value/map/entry/value/map/entry/value/map/entry[parent::node()/entry/value/child::text()='$loggroup' and key/child::text()='RetentionInDays']" | select -ExpandProperty Node|select -ExpandProperty value
    "`nCreating $loggroup with RetentionInDays = $retention_days"
    #Actual call to AWS here...

    $logstreams = Select-Xml -Content $resources -XPath "/yaml/map/entry/key[child::text()='Resources']/parent::node()/value/map/entry[value/map/entry/value/child::text()='AWS::Logs::LogStream' and value/map/entry/value/map/entry/value/map/entry/value/child::text()='$loggroup']" | select -ExpandProperty Node|select -ExpandProperty key
    foreach($logstream in $logstreams)
    {
        "`tLogGroup $loggroup : Creating $logstream"
        #Actual call to AWS here...
    }
}