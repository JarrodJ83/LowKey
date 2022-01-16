#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["LowKey.Data.Example.Net5WebApp/LowKey.Data.Example.Net5WebApp.csproj", "LowKey.Data.Example.Net5WebApp/"]
COPY ["LowKey.Data/LowKey.Data.csproj", "LowKey.Data/"]
COPY ["LowKey.Data.Sql/LowKey.Data.Sql.csproj", "LowKey.Data.Sql/"]
COPY ["LowKey.Extensions.Hosting/LowKey.Extensions.Hosting.csproj", "LowKey.Extensions.Hosting/"]
RUN dotnet restore "LowKey.Data.Example.Net5WebApp/LowKey.Data.Example.Net5WebApp.csproj"
COPY . .
WORKDIR "/src/LowKey.Data.Example.Net5WebApp"
RUN dotnet build "LowKey.Data.Example.Net5WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LowKey.Data.Example.Net5WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LowKey.Data.Example.Net5WebApp.dll"]
