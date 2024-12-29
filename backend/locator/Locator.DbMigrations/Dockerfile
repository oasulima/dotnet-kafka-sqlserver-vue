# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source
ARG TARGETARCH
# Copy project file and restore as distinct layers

COPY --link "locator/Locator.DbMigrations/Locator.DbMigrations.csproj" "locator/Locator.DbMigrations/Locator.DbMigrations.csproj"

RUN dotnet restore -a $TARGETARCH "locator/Locator.DbMigrations/Locator.DbMigrations.csproj"

# Copy source code and publish app
COPY --link locator/Locator.DbMigrations locator/Locator.DbMigrations
WORKDIR "/source/locator/Locator.DbMigrations"

RUN dotnet publish -a $TARGETARCH --no-restore -o /app


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
ENTRYPOINT ["./Locator.DbMigrations"]