FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app/Datahub.Queue.Manager

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app/Datahub.Queue.Manager
COPY --from=build-env /app/Datahub.Queue.Manager/out .
ENV HTTPS_PROXY=http://proxy.opal:3128
ENV HTTP_PROXY=http://proxy.opal:3128
RUN apt-get update && apt-get -y install telnet
ENTRYPOINT ["dotnet", "Datahub.Queue.Manager.dll"]
