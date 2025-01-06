# Sử dụng hình ảnh .NET Core Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Sử dụng hình ảnh .NET SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["QuanLyChiTieuCaNhan.csproj", "./"]
RUN dotnet restore "QuanLyChiTieuCaNhan.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet publish "QuanLyChiTieuCaNhan.csproj" -c Release -o /app/publish

# Tạo hình ảnh chạy cuối cùng
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "QuanLyChiTieuCaNhan.dll"]
