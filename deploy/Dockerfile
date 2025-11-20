FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

COPY Saphira.sln ./
COPY src/ ./src/

RUN dotnet restore
RUN dotnet build -c Release --no-restore
RUN dotnet publish src/Saphira/Saphira.csproj -c Release -o /app --no-build

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app

COPY --from=build /app .
COPY resources/ ./resources/

USER $APP_UID
ENTRYPOINT ["dotnet", "Saphira.dll"]
