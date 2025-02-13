FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS="http://+:80;"
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Project.API/Project.API.csproj", "Project.API/"]
RUN dotnet restore "Project.API/Project.API.csproj"

COPY . .
WORKDIR "/src/Project.API"
RUN dotnet build "Project.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Project.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Project.API.dll"]
