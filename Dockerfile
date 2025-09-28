# --- Estágio 1: Build (Compilação) ---
# Usamos a imagem do SDK do .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copia e restaura as dependências primeiro para otimizar o cache
COPY *.csproj .
RUN dotnet restore

# Copia o resto do código fonte e publica a aplicação
COPY . .
RUN dotnet publish -c Release -o /app --no-restore

# --- Estágio 2: Imagem Final de Execução ---
# Usamos a imagem de runtime do ASP.NET 8, que é muito mais leve
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copia os arquivos publicados do estágio de build para a imagem final
COPY --from=build /app .

# A porta que a aplicação expõe internamente
EXPOSE 80

# Comando para iniciar a API
# Verifique se o nome do .dll está correto!
ENTRYPOINT ["dotnet", "valida-conta-instagram-poc.dll"]