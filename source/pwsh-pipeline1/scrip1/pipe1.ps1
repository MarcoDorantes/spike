Set-StrictMode -Version 3.0

Import-Module C:\temp\module\module1\module1.psd1

function Read-Source
{
    [CmdLetBinding()]
    param($file)

    begin
    {
        Write-Warning 'begin Read-Source'
    }
    process
    {
        Write-Warning 'process Read-Source'
       #cat $file | ?{![string]::IsNullOrWhiteSpace($_)}
    }
    end
    {
        Write-Warning 'end Read-Source'
        cat $file | ?{![string]::IsNullOrWhiteSpace($_)}
    }
}
#$lines = cat C:\temp\source1.txt | ?{![string]::IsNullOrWhiteSpace($_)}
#Read-Source C:\temp\source1.txt | Read-Pipe -Tag 'A' | Read-Pipe -Tag 'B'| Out-File C:\temp\afile.txt
#Start-Sleep -Seconds 60
#Read-Source C:\temp\source1.txt | Read-Pipe -Tag 'A' | Read-Pipe -Tag 'B'| Out-File C:\temp\afile.txt -Append

#cat C:\temp\source1.txt | ?{![string]::IsNullOrWhiteSpace($_)} | Read-Pipe -Tag 'A' | Read-Pipe -Tag 'B'| Out-File C:\temp\afile.txt
#cat C:\temp\source1.txt | ?{![string]::IsNullOrWhiteSpace($_)} | Read-Pipe -Tag 'A' | Read-Pipe -Tag 'B'
#Read-Source C:\temp\source1.txt | Read-Pipe -Tag 'A' | Read-Pipe -Tag 'B'
#cat C:\temp\source1.txt | % -Begin {'A-begin'} -Process {"A-next:$_"} -End {'A-end'} | % -Begin {'B-begin'} -Process {"B-next:$_"} -End {'B-end'}
#cat C:\temp\source1.txt | % -Begin {Write-Warning 'A-begin'; 'A-begin'} -Process {Write-Warning "A-next:$_"; "A-next:$_"} -End {Write-Warning 'A-end';'A-end'} | % -Begin {Write-Warning 'B-begin';'B-begin'} -Process {Write-Warning "B-next:$_";"B-next:$_"} -End {Write-Warning 'B-end';'B-end'}
Read-Source C:\temp\source1.txt | % -Begin {Write-Warning 'A-begin'; 'A-begin'} -Process {Write-Warning "A-next:$_"; "A-next:$_"} -End {Write-Warning 'A-end';'A-end'} | % -Begin {Write-Warning 'B-begin';'B-begin'} -Process {Write-Warning "B-next:$_";"B-next:$_"} -End {Write-Warning 'B-end';'B-end'}