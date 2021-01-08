FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
# Copy csproj and restore as distinct layers
WORKDIR /app
COPY ["cxOrganization.WebServiceAPI/cxOrganization.WebServiceAPI.csproj", "cxOrganization.WebServiceAPI/"]
COPY ["cxOrganization.WebServiceAPI/NuGet.Config", "cxOrganization.WebServiceAPI/"]
COPY ["cxOrganization.Adapter/cxOrganization.Adapter.csproj", "cxOrganization.Adapter/"]
COPY ["cxOrganization.WebServiceAPI/NuGet.Config", "cxOrganization.Adapter/"]
COPY ["cxOrganization.Domain/cxOrganization.Domain.csproj", "cxOrganization.Domain/"]
COPY ["cxOrganization.WebServiceAPI/NuGet.Config", "cxOrganization.Domain/"]
COPY ["cxOrganization.Business/cxOrganization.Business.csproj", "cxOrganization.Business/"]
COPY ["cxOrganization.WebServiceAPI/NuGet.Config", "cxOrganization.Business/"]
COPY ["cxOrganization.Domain/cxOrganization.Domain.csproj", "cxOrganization.Domain/"]
COPY ["cxOrganization.WebServiceAPI/NuGet.Config", "cxOrganization.Domain/"]

RUN dotnet restore "cxOrganization.WebServiceAPI/cxOrganization.WebServiceAPI.csproj"

# Copy everything else and build
COPY . .
WORKDIR /app/cxOrganization.WebServiceAPI

FROM build AS publish
RUN dotnet publish "cxOrganization.WebServiceAPI.csproj" -c Release -o out

FROM base AS final
WORKDIR /app/cxOrganization.WebServiceAPI
RUN apt-get update && apt-get install -y libgdiplus libc6-dev && ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
COPY --from=publish /app/cxOrganization.WebServiceAPI/out .
ENTRYPOINT ["dotnet", "cxOrganization.WebServiceAPI.dll"]
