# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /QuestionmiPanel
COPY QuestionmiPanel/*.csproj .
RUN dotnet restore
COPY QuestionmiPanel .
RUN dotnet publish -c Release -o /publish
# COPY out/wwwroot/_content/hollytestclient out/wwwroot/

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /publish
COPY --from=build-env /publish .
EXPOSE 5066
ENTRYPOINT ["dotnet", "QuestionmiPanel.dll"]