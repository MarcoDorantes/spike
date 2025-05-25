Set-StrictMode -Version 3.0

Import-Module C:\temp\module\module1\module1.psd1

#ls $home | select -expand Name | Read-File
ls $home | select -expand Name | % { Read-File -FilePath $_ }