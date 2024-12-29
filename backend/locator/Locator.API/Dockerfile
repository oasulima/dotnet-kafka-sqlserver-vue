# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source
ARG TARGETARCH
# Copy project file and restore as distinct layers

COPY --link "locator/Locator.API/Locator.API.csproj" "locator/Locator.API/Locator.API.csproj"
COPY --link "shared/Shared/Shared.csproj" "shared/Shared/Shared.csproj"

RUN dotnet restore -a $TARGETARCH "locator/Locator.API/Locator.API.csproj"

# Copy source code and publish app
COPY --link locator/Locator.API locator/Locator.API
COPY --link shared/Shared shared/Shared
WORKDIR /source/locator/Locator.API

RUN dotnet publish -a $TARGETARCH --no-restore -o /app


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
EXPOSE 33000
WORKDIR /app
COPY --link --from=build /app .
COPY --link curl-8.11.0 .
RUN chmod +x curl-8.11.0
USER $APP_UID
ENTRYPOINT ["./Locator.API"]