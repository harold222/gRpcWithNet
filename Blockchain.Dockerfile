#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["BlockChain/BlockChain.csproj", "BlockChain/"]
RUN dotnet restore "BlockChain/BlockChain.csproj"
COPY . .
WORKDIR "/src/BlockChain"
RUN dotnet build "BlockChain.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlockChain.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlockChain.dll"]