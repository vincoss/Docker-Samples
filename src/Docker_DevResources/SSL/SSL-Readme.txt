
## Intranet SSL

C:\_Data\DevResources\Tools\OpenSSL-Win32\openssl.exe req -new -x509 -extensions SAN -keyout private.pem -out public.pem -config config.cfg -days 50000

Pass@word1

openssl.exe pkcs12 -export -aes256 -in public.pem -inkey private.pem -out certificate.pfx

openssl.exe x509 -outform der -in public.pem -out public.der

## Resources
https://int64software.com/blog/2020/04/20/creating-a-self-signed-ssl-certificate-for-your-intranet-services/


https://mikewilliams.io/net-core-2-1-and-docker-how-to-get-docker-to-recognize-a-local-ssl-certificate-6e637e1e8800