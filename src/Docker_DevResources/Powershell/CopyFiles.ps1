$OriginalDir = "C:Temp\"
$BackupDir = "D:\Temp"
$filter="*.exe"
$latest = Get-ChildItem -Path $OriginalDir -Filter $filter | Sort-Object LastAccessTime -Descending | Select-Object -First 1
$latest.name
Copy-Item -path "$OriginalDir\$latest" "$BackupDir\$latest"