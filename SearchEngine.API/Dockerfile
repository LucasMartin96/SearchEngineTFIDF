FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY ["*.sln", "./"]

# Copy all project files
COPY ["SearchEngine.API/*.csproj", "SearchEngine.API/"]
COPY ["SearchEngine.Core/*.csproj", "SearchEngine.Core/"]
COPY ["SearchEngine.Infrastructure/*.csproj", "SearchEngine.Infrastructure/"]
COPY ["SearchEngine.Persistence/*.csproj", "SearchEngine.Persistence/"]
COPY ["SearchEngine.Shared/*.csproj", "SearchEngine.Shared/"]
COPY ["SearchEngine.Indexer/*.csproj", "SearchEngine.Indexer/"]

# Restore as distinct layers
RUN dotnet restore "SearchEngine.API/SearchEngine.API.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/SearchEngine.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Add environment variables for ports
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "SearchEngine.API.dll"]