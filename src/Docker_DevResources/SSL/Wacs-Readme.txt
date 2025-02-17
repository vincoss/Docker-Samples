## Wacs
--renew --baseuri "https://acme-v02.api.letsencrypt.org/" --verbose --validation godaddy --validationmode dns-01 --apikey TODO: --apisecret TODO:
--renew --baseuri "https://acme-v02.api.letsencrypt.org/" --verbose --validation godaddy --validationmode dns-01 --apikey TODO: --apisecret TODO:

## Certificates location windows
%programdata%\win-acme\


## Godaddy validation

Download WACS executable and plugin
https://github.com/win-acme/win-acme/releases/
	plugin.validation.dns.godaddy.v2.2.6.1571.zip
	win-acme.v2.2.6.1571.x64.pluggable.zip

Ensure API Key and Secreet
https://developer.godaddy.com/keys

Validation
wacs.exe with --verbose --validation godaddy --validationmode dns-01

TXT record to add
try to add '_acme-challenge.cam',  no need include domain name in the dns record