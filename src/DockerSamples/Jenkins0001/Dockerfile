﻿FROM jenkins/jenkins:latest

RUN /usr/local/bin/install-plugins.sh git mstest matrix-auth workflow-aggregator docker-workflow blueocean credentials-binding

ENV JENKINS_USER admin
ENV JENKINS_PASS admin

# Skip initial setup
ENV JAVA_OPTS -Djenkins.install.runSetupWizard=false

COPY default-user.groovy /usr/share/jenkins/ref/init.groovy.d/
COPY executors.groovy /usr/share/jenkins/ref/init.groovy.d/executors.groovy

VOLUME /var/jenkins_home
