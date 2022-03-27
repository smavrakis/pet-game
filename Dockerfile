FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

COPY ./src ./src
RUN dotnet restore src/PetGame/PetGame.csproj

WORKDIR /src/PetGame
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
COPY --from=build /src/PetGame/out .

EXPOSE 5888
ENTRYPOINT ["dotnet", "PetGame.dll"]