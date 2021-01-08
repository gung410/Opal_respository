FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
# Copy csproj and restore as distinct layers
WORKDIR /app
COPY ["Communication.DataAccess/Communication.DataAccess.csproj", "Communication.DataAccess/"]
COPY ["Communication.Processor.Sender/NuGet.Config", "Communication.Processor.Sender/"]
COPY ["Communication.Processor.Sender/Communication.Processor.Sender.csproj", "Communication.Processor.Sender/"]
RUN dotnet restore "Communication.Processor.Sender/Communication.Processor.Sender.csproj"

# Copy everything else and build
COPY . .
WORKDIR /app/Communication.Processor.Sender
RUN dotnet publish "Communication.Processor.Sender.csproj" -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app/Communication.Processor.Sender
COPY --from=build /app/Communication.Processor.Sender/out .
ENTRYPOINT ["dotnet", "Communication.Processor.Sender.dll"]