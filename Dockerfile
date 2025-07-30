FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["bookApi.csproj", "."]
RUN dotnet restore "bookApi.csproj"
COPY . .
RUN dotnet build "bookApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "bookApi.csproj" -c Release -o /app/publish



FROM base AS final
WORKDIR /app

RUN apt-get update && \
    apt-get install -y \
    pandoc \
    texlive-xetex \
    texlive-fonts-extra \
    texlive \
    texlive-latex-extra \
    texlive-lang-cyrillic \
    texlive-fonts-recommended \
    lmodern \
    cm-super \
    fonts-dejavu && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
COPY lib/Pandoc.dll /app/lib/

ENTRYPOINT ["dotnet", "bookApi.dll"]