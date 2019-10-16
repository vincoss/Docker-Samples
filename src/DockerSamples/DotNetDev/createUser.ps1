$Password = Get-Content C:\temp\passwordEncrypted.txt | ConvertTo-SecureString -Key (1..16)
New-LocalUser "TeamCityAgentUser" -Password $Password -FullName "TeamCityAgentUser" -Description "Account for TeamCity Agent." 
Add-LocalGroupMember -Group "Administrators" -Member "TeamCityAgentUser"
Set-LocalUser -Name "TeamCityAgentUser" -PasswordNeverExpires 1