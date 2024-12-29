FROM ubuntu:latest
RUN apt update 
RUN apt -y upgrade 
RUN apt -y install \
    git curl build-essential pkg-config libssl-dev

USER ubuntu
RUN curl https://get.volta.sh | bash