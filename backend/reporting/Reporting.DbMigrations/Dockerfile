# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source
ARG TARGETARCH
# Copy project file and restore as distinct layers

COPY --link "reporting/Reporting.DbMigrations/Reporting.DbMigrations.csproj" "reporting/Reporting.DbMigrations/Reporting.DbMigrations.csproj"

RUN dotnet restore -a $TARGETARCH "reporting/Reporting.DbMigrations/Reporting.DbMigrations.csproj"

# Copy source code and publish app
COPY --link reporting/Reporting.DbMigrations reporting/Reporting.DbMigrations
WORKDIR "/source/reporting/Reporting.DbMigrations"

RUN dotnet publish -a $TARGETARCH --no-restore -o /app


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
ENTRYPOINT ["./Reporting.DbMigrations"]