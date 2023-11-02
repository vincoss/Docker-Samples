
## Intranet SSL

#1	create the key files

C:\_Data\DevResources\Tools\OpenSSL-Win32\openssl.exe req -new -x509 -extensions SAN -keyout private.pem -out public.pem -config config.cfg -days 50000
Pass@word1

#2	Converting your private key to PKCS#12	
openssl.exe pkcs12 -export -aes256 -in public.pem -inkey private.pem -out certificate.pfx	  

#3	Converting your Public Key to DER
openssl.exe x509 -outform der -in public.pem -out public.der	  


## example
C:\_Documents\_Data\DevResources\Tools\OpenSSL-Win32\openssl.exe req -x509 -nodes -new -sha256 -days 1024 -newkey rsa:2048 -keyout RootCA.key -out RootCA.pem -config config.cfg -days 50000
C:\_Documents\_Data\DevResources\Tools\OpenSSL-Win32\openssl.exe x509 -outform pem -in RootCA.pem -out RootCA.crt
C:\_Documents\_Data\DevResources\Tools\OpenSSL-Win32\openssl.exe pkcs12 -export -name "Test Certificate" -out localhost.pfx -inkey RootCA.key -in RootCA.crt
fdfd

# example test
C:\_Documents\_Data\DevResources\Tools\OpenSSL-Win32\openssl.exe s_server -cert RootCA.crt -key RootCA.key -www -accept 5167
https://10.0.0.76:5167/

## Resources
chrome://flags/#allow-insecure-localhost
https://gist.github.com/cecilemuller/9492b848eb8fe46d462abeb26656c4f8
https://serverfault.com/questions/973446/self-signed-ssl-i-created-for-localhost-cannot-be-trusted-even-though-i-have-alr
https://int64software.com/blog/2020/04/20/creating-a-self-signed-ssl-certificate-for-your-intranet-services/
https://mikewilliams.io/net-core-2-1-and-docker-how-to-get-docker-to-recognize-a-local-ssl-certificate-6e637e1e8800