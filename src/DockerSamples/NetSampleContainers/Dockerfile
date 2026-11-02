
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

# Use the -c flag to pass a command string
ENTRYPOINT ["/bin/bash", "-c"]
CMD ["echo 'Hello from Bash!'"]

# Alternative

# Native Alpine shell using -c
#ENTRYPOINT ["/bin/sh", "-c"]
#CMD ["echo 'Hello from Sh!'"]


# NOTE: Linux paths are case sensitive!!! 
# docker build -f Dockerfile --no-cache -t bashshsample .
# docker run -it --rm --name bashshsample bashshsample