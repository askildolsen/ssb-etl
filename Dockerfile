FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY *.cs ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS resourcemodel
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT [ "dotnet", "ssb-etl.dll" ]
