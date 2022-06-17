FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS final
COPY  release /release
WORKDIR /release
