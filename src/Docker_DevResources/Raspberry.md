
# Raspberry install OS

### Software
download raspberry imager

### Change settings
To access and change settings in Raspberry Pi Imager, press Ctrl + Shift + X after selecting an OS and storage device. 

`
Ensure SSH is enabled.
`

Name: 		raspberry-ac
UserName: 	admin
Password:	a123456789b

WIFI:		lala
KEY:		find-that-and-replace

### SSH
ssh admin@192.168.1.111

### Copy filed into the raspberry, recursive and single file

scp -r * admin@192.168.1.111:~/iocsamples
scp IoT_Samples.deps.jso admin@192.168.1.111:~/iocsamples

# linux commands

### Create dirctory

sudo mkdir test

### Remove directory

sudo rmdir test

### list directory

ls -a

### show system version

lsb_release -a

# Resources
https://www.youtube.com/watch?v=IhPaHZONmrY
