FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

# Copy csproj and restore as distinct layers
WORKDIR /app
COPY ["Communication.DataAccess/Communication.DataAccess.csproj", "Communication.DataAccess/"]
COPY ["Communication.Api/NuGet.Config", "Communication.Api/"]
COPY ["Communication.Api/Communication.Api.csproj", "Communication.Api/"]
RUN dotnet restore "Communication.Api/Communication.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR /app/Communication.Api
RUN dotnet publish "Communication.Api.csproj" -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app/Communication.Api
COPY --from=build /app/Communication.Api/out .
ENTRYPOINT ["dotnet", "Communication.Api.dll"]