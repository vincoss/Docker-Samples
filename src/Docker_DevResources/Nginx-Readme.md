
# NGINX 

### Basic commands
```
Start NGINX:        start nginx
Reload Config:      nginx.exe -s reload
Stop NGINX:         nginx.exe -s quit
Validate changes:   nginx.exe -t
```

### Verify
Open a web browser and go to http://localhost. You should see the "Welcome to nginx!" page.

### Kill Hidden Or Ghost NGINX Processes
```
Windows:            taskkill /f /im nginx.exe
```

### See nginx processes
```
Windows:            tasklist /fi "imagename eq nginx.exe"
```

### nginx/Windows directory
```
access_log   logs/site.log;
root         C:/web/html;
```

### Configure
```
Open conf\nginx.conf in a text editor like Notepad to change ports, set up reverse proxies, or adjust your website's root folder.
```

### Resources
https://nginx.org/
https://nginx.org/en/docs/windows.html