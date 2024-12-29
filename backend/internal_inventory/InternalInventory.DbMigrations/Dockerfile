# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source
ARG TARGETARCH
# Copy project file and restore as distinct layers

COPY --link "internal_inventory/InternalInventory.DbMigrations/InternalInventory.DbMigrations.csproj" "internal_inventory/InternalInventory.DbMigrations/InternalInventory.DbMigrations.csproj"

RUN dotnet restore -a $TARGETARCH "internal_inventory/InternalInventory.DbMigrations/InternalInventory.DbMigrations.csproj"

# Copy source code and publish app
COPY --link internal_inventory/InternalInventory.DbMigrations internal_inventory/InternalInventory.DbMigrations

WORKDIR /source/internal_inventory/InternalInventory.DbMigrations

RUN dotnet publish -a $TARGETARCH --no-restore -o /app


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
ENTRYPOINT ["./InternalInventory.DbMigrations"]