import jenkins.model.Jenkins

Integer numberOfExecutors = 5

Jenkins jenkins = Jenkins.getInstance()

jenkins.setNumExecutors(numberOfExecutors)

jenkins.save()