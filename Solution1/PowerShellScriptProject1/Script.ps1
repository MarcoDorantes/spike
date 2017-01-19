#
# Script.ps1
#
dir -Filter *.exe | ?{$_.name -match 'charges'} | % {"`t" + $_.Length + "`t" + $_.Name}