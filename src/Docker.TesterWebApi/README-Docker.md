## Docker.TesterWebSite

# Featured Tags

* `1.0` (LTS/Current)
  * `docker pull vincoss/dockertesterweb`

# About This Image

This image contains the ASP.NET Core test website.

## Container sample: Run a web application

```console
docker run -it --rm -p 8003:80 --name dockertesterweb vincoss/dockertesterweb
```

After the application starts, navigate to `http://localhost:8003` in your web browser.

# Full Tag Listing

## Linux amd64 Tags
Tags | Dockerfile | OS Version
-----------| -------------| -------------
1.0.0-bionic | [Dockerfile](https://github.com/vincoss/docker.testerwebsite/blob/master/Dockerfile.ubuntu-x64) | Ubuntu 18.04

## Linux arm64 Tags
Tags | Dockerfile | OS Version
-----------| -------------| -------------
1.0.0-bionic-arm64 | [Dockerfile](https://github.com/vincoss/docker.testerwebsite/blob/master/Dockerfile.ubuntu-arm64) | Ubuntu 18.04

## Windows
Tags | Dockerfile | OS Version
-----------| -------------| -------------
1.0.0-windows | [Dockerfile](https://github.com/vincoss/docker.testerwebsite/blob/master/Dockerfile.windows-x64) | Windows

# Feedback

* [Feedback & Issues](https://github.com/vincoss/docker.testerwebsite/issues)

# License

* [LICENSE](https://github.com/vincoss/docker.testerwebsite/blob/master/LICENSE)