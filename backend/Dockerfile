# Estágio 1: Build (usando o SDK completo do .NET 8)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Instala a ferramenta EF Globalmente na imagem de build e configura o PATH
RUN dotnet nuget locals all --clear && \
    dotnet tool install --global dotnet-ef --version 10.0.*
ENV PATH="$PATH:/root/.dotnet/tools"

# Copia o .csproj primeiro e restaura os pacotes (aproveita o cache do Docker)
COPY backend.csproj .
RUN dotnet restore "backend.csproj"

# Copia o restante do código-fonte e publica a aplicação
COPY . .
WORKDIR "/src/."
RUN dotnet publish "backend.csproj" -c Release -o /app/publish

# Estágio 2: Final (usando o runtime leve do ASP.NET 8)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expõe a porta que o ASPNETCORE_URLS irá usar
EXPOSE 8080

# Ponto de entrada para executar a aplicação
ENTRYPOINT ["dotnet", "backend.dll"]