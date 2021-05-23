FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ProfanityChecker.WebApi/ProfanityChecker.WebApi.csproj", "ProfanityChecker.WebApi/"]
RUN dotnet restore "ProfanityChecker.WebApi/ProfanityChecker.WebApi.csproj"
COPY . .
WORKDIR "/src/ProfanityChecker.WebApi"
RUN dotnet build "ProfanityChecker.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProfanityChecker.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /src/db ./db/
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProfanityChecker.WebApi.dll"]
