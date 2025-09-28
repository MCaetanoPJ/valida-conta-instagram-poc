# --- Estágio 1: Build (Compilação) ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY . .

WORKDIR /source/valida-conta-instagram-poc

RUN dotnet restore
RUN dotnet publish -c Release -o /app

# --- Estágio 2: Imagem Final de Execução ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app .

# --- ESTA É A LINHA QUE CORRIGE O PROBLEMA ---
# Força a aplicação a rodar na porta 80 dentro do contêiner
ENV ASPNETCORE_HTTP_PORTS=80

EXPOSE 80

ENTRYPOINT ["dotnet", "valida-conta-instagram-poc.dll"]