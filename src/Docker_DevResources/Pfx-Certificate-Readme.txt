
## Windows
openssl pkcs12 -in X509Sample.pfx -nocerts -out Root.pem -nodes
openssl pkcs12 -in X509Sample.pfx -nokeys -out Root.crt -nodes

## Linux - Docker container
docker exec -it dockertesterweb bash
cd /var/appdata/

cp Root.crt /usr/local/share/ca-certificates/Root.crt
update-ca-certificates --fresh

## List all certificates
awk -v cmd='openssl x509 -noout -subject' '
    /BEGIN/{close(cmd)};{print | cmd}' < /etc/ssl/certs/ca-certificates.crt
	
## Other
cp Root.crt /usr/local/share/ca-certificates/Root.crt
update-ca-certificates --fresh

cp Root.crt /usr/share/ca-certificates/Root.crt
cp Root.crt /usr/local/share/ca-certificate

~/.dotnet/corefx/cryptography/x509stores/	# Must create certificate programmatically to have that path

## Issues
If not found add multiple times