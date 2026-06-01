FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DomParserApi.csproj", "./"]
RUN dotnet restore "DomParserApi.csproj"
COPY . .
RUN dotnet publish "DomParserApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5175
ENV ASPNETCORE_URLS=http://+:5175
ENTRYPOINT ["dotnet", "DomParserApi.dll"]
