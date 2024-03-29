FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Wallet/Wallet.csproj", "Wallet/"]
RUN dotnet restore "Wallet/Wallet.csproj"
COPY . .
WORKDIR "/src/Wallet"
RUN dotnet build "Wallet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Wallet.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wallet.dll"]