# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source
ARG TARGETARCH
# Copy project file and restore as distinct layers

COPY --link "locator/Locator.UserEmulator/Locator.UserEmulator.csproj" "locator/Locator.UserEmulator/Locator.UserEmulator.csproj"
COPY --link "shared/Shared/Shared.csproj" "shared/Shared/Shared.csproj"

RUN dotnet restore -a $TARGETARCH "locator/Locator.UserEmulator/Locator.UserEmulator.csproj"

# Copy source code and publish app
COPY --link locator/Locator.UserEmulator locator/Locator.UserEmulator
COPY --link shared/Shared shared/Shared
WORKDIR /source/locator/Locator.UserEmulator

RUN dotnet publish -a $TARGETARCH --no-restore -o /app


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
EXPOSE 33000
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
ENTRYPOINT ["./Locator.UserEmulator"]