FROM microsoft/dotnet-framework:4.7.2-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY helloworld-windowsservercore/*.csproj ./helloworld-windowsservercore/
COPY helloworld-windowsservercore/*.config ./helloworld-windowsservercore/
RUN nuget restore

# copy everything else and build app
COPY helloworld-windowsservercore/. ./helloworld-windowsservercore/
WORKDIR /app/helloworld-windowsservercore
RUN msbuild /p:Configuration=Release


FROM microsoft/aspnet:4.7.2 AS runtime
WORKDIR /inetpub/wwwroot
COPY --from=build /app/helloworld-windowsservercore/. ./
