## Resources
https://docs.ubuntu.com/core/en/stacks/network/
https://boxofcables.dev/getting-started-on-ubuntu-core-with-hyper-v/
https://dylanmc.ca/-/blog/2018/05/05/my-tiny-file-server-with-ubuntu-core-nextcloud-and-syncthing/
https://help.ubuntu.com/community/AutomaticallyMountPartitions

https://linuxhint.com/list_disks_ubuntu/

## Ubuntu Core

# Connect
ssh -i id_rsa_ubuntu_core ferdinandgad@192.168.227.198

# Reboot
sudo reboot

# Current path
pwd

# CPU
lscpu

# List all partitions
sudo lsblk -o NAME,FSTYPE,SIZE,MOUNTPOINT,LABEL
lsblk
lsblk -e7		#hide loop devices
lsblk -o NAME,FSTYPE,SIZE,MOUNTPOINT,LABEL		# Show property of interest

sudo fdisk -l		# list all attached disks, partitions, logical

# Disk Size
df -i
df -k
df -h

# Check who can write into the directory
ls -ld /writable
ls -rtl
ls -la

# Memory usage
free -h				# Show in MBs
top					# Print CPU and Memory usage
vmstat -s			# Show details
cat /proc/meminfo	# Print memory information

# Host name
hostname

# User
who
whoami

# Snap
snap list
snap find hello
sudo snap install hello
sudo snap refresh docker

## Create Directory

mkdir /home/ferdinandgad/docker-temp

# Exit
exit

# Check for updates
snap refresh
snap refresh --list

## Ubuntu Core Docker

# Install docker
sudo snap install docker

# Connect Docker to the System
sudo snap connect docker:home

## SCP (Secure copy protocol)

https://www.simplified.guide/ssh/copy-file
https://www.techrepublic.com/article/how-to-use-secure-copy-with-ssh-key-authentication/
https://www.hostinger.com/tutorials/using-scp-command-to-transfer-files/
https://linux.die.net/man/1/scp

# Copy from Linux to Windows (use sudo for protected directories -r for recusive directory copy)

scp -r /writable ferdinand@10.0.0.129:c:/temp/ubuntu_core/
sudo scp -r /var/snap/docker/common/var-lib-docker/volumes/ ferdinand@10.0.129:c:/temp/ubuntu_core/
 
# Copy file from remote server into current server (with security privileges)
sudo scp ferdinand@10.0.0.129:c:/temp/ubuntu_core/one/a.txt /writable

# Copy from from local machine to remote server (Not working since the folder need (sudo with security privileges which used does not have)
scp -i id_rsa_ubuntu_core /c/temp/ubuntu_core/one/a.txt ferdinandgad@192.168.52.56:~/writable
