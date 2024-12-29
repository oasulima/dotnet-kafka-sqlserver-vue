FROM ubuntu:latest
RUN apt update 
RUN apt -y upgrade 
RUN apt -y install \
    git curl build-essential pkg-config libssl-dev libicu-dev

USER ubuntu
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 9.0
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0
RUN echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
RUN echo 'export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools' >> ~/.bashrc