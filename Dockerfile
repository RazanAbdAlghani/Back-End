# المرحلة الأولى: بناء المشروع
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# نسخ ملفات المشروع
COPY *.sln ./
COPY secondVersionFlowSync/*.csproj ./secondVersionFlowSync/
RUN dotnet restore

# نسخ باقي الملفات وبناء المشروع
COPY . ./
WORKDIR /app/secondVersionFlowSync
RUN dotnet publish -c Release -o /app/publish

# المرحلة الثانية: تشغيل المشروع
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# تحديد البورت (يمكن تغييره حسب إعداداتك في المشروع)
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# أمر التشغيل
ENTRYPOINT ["dotnet", "secondVersionFlowSync.dll"]
