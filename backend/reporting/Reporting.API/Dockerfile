# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source
ARG TARGETARCH
# Copy project file and restore as distinct layers

COPY --link ["reporting/Reporting.API/Reporting.API.csproj", "reporting/Reporting.API/Reporting.API.csproj"]

COPY --link ["shared/Shared/Shared.csproj", "shared/Shared/Shared.csproj"]

RUN dotnet restore -a $TARGETARCH "reporting/Reporting.API/Reporting.API.csproj"

# Copy source code and publish app
COPY --link reporting/Reporting.API reporting/Reporting.API
COPY --link shared/Shared shared/Shared
WORKDIR "/source/reporting/Reporting.API"

RUN dotnet publish -a $TARGETARCH --no-restore -o /app


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
EXPOSE 33000
WORKDIR /app
COPY --link --from=build /app .
COPY --link curl-8.11.0 .
RUN chmod +x curl-8.11.0
USER $APP_UID
ENTRYPOINT ["./Reporting.API"]