# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["backend.csproj", "./"]
RUN dotnet restore "./backend.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "backend.csproj" -c Release -o /app/build
RUN dotnet publish "backend.csproj" -c Release -o /app/publish

# Estágio 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "backend.dll", "--urls", "http://0.0.0.0:80"]
