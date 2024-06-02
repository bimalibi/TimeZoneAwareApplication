FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MeetingScheduler/MeetingScheduler.csproj", "MeetingScheduler/"]
RUN dotnet restore "MeetingScheduler/MeetingScheduler.csproj"
COPY . .
WORKDIR "/src/MeetingScheduler"
RUN dotnet build "MeetingScheduler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MeetingScheduler.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeetingScheduler.dll"]
