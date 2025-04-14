# استخدم SDK الخاص بـ .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# نسخ ملفات المشروع
COPY *.csproj ./
RUN dotnet restore

# نسخ باقي الملفات
COPY . ./
RUN dotnet publish -c Release -o out

# المرحلة الثانية: فقط لتشغيل التطبيق (أخف)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# تشغيل التطبيق
ENTRYPOINT ["dotnet", "secondVersionFlowSync.dll"]
