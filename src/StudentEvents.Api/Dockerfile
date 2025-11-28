# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/StudentEvents.Api/StudentEvents.Api.csproj", "src/StudentEvents.Api/"]
COPY ["src/StudentEvents.Domain/StudentEvents.Domain.csproj", "src/StudentEvents.Domain/"]
COPY ["src/StudentEvents.Infrastructure/StudentEvents.Infrastructure.csproj", "src/StudentEvents.Infrastructure/"]
COPY ["src/StudentEvents.Application/StudentEvents.Application.csproj", "src/StudentEvents.Application/"]
RUN dotnet restore "./src/StudentEvents.Api/StudentEvents.Api.csproj"
COPY . .
WORKDIR "/src/src/StudentEvents.Api"
RUN dotnet build "./StudentEvents.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StudentEvents.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudentEvents.Api.dll"]