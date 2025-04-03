FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY VideoGameApi.csproj ./
RUN dotnet restore VideoGameApi.csproj

COPY . .  
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .

EXPOSE 3000

ENTRYPOINT ["dotnet", "VideoGameApi.dll"]
