FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

# This is the working location INSIDE the docker environment.
WORKDIR /src

# The root level of the project is the location of global.json.
# Copy all from docker context (root location where is the global.json located) to WORKDIR location.
# The docker context for this file is where is the global.json located because we are in monorepo style.
# Stay in the root level and run: docker build -f src/microservices/webinar-microservice/Api/Dockerfile .
# IMPORTANT: Rememer to start CMD interface at the root level of the project.
# We need to copy the whole thing because there are many stuffs that need to be included when running build.
# For example the Directory.Build.props and targets file.
# That explains why we may be differrent from many sample Dockerfile for asp.net core which copies some specific csproj file.
COPY . .

# Restore your microservice project.
RUN dotnet restore "src/microservices/webinar-microservice/Api/Conexus.Opal.Microservice.Webinar.Api.csproj"

# Setting current location to your main project and build it.
WORKDIR "/src/src/microservices/webinar-microservice/Api"
RUN dotnet build "Conexus.Opal.Microservice.Webinar.Api.csproj" -c Release -o /app/build

# From the successful previous step, we release the app.
FROM build AS publish
RUN dotnet publish "Conexus.Opal.Microservice.Webinar.Api.csproj" -c Release -o /app/publish

# Finally, we copy the final output and define the entry point of the application.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Conexus.Opal.Microservice.Webinar.Api.dll"]
