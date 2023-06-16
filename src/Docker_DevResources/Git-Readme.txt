
## Git hub migrate repo	https://gist.github.com/niksumeiko/8972566
git clone --mirror URL
cd <name_of_old_repo>
git remote add NEW-REMOTE <URL>
git push NEW-REMOTE --mirror

## Change repor remote
git remote set-url origin https://github.com/todo-name-org/your-repo.git

## Git repo remove large files	https://rtyley.github.io/bfg-repo-cleaner/
java -jar bfg.jar --strip-blobs-bigger-than 50M C:\Temp\your-repo-name
git reflog expire --expire=now --all && git gc --prune=now --aggressive
git push

## Remove large files and sensitive data
https://docs.github.com/en/github/authenticating-to-github/keeping-your-account-and-data-secure/removing-sensitive-data-from-a-repository