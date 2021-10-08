
## Enable in ports
New-NetFirewallRule -DisplayName 'verdaccio-inbound' -Profile @('Domain', 'Private', 'Public') -Direction Inbound -Action Allow -Protocol TCP -LocalPort @('4873')