# --- Estágio 1: Build (Compilação) ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copia o código TODO primeiro
COPY . .

# *** MUDANÇA IMPORTANTE AQUI ***
# Entra na subpasta onde o projeto realmente está
WORKDIR /source/valida-conta-instagram-poc

# Agora, executa os comandos de dentro da pasta do projeto
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# --- Estágio 2: Imagem Final de Execução ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copia os arquivos publicados do estágio de build
COPY --from=build /app .

EXPOSE 80

# O nome do .dll permanece o mesmo
ENTRYPOINT ["dotnet", "valida-conta-instagram-poc.dll"]