# 1st stage: Build the react frontend
FROM node:16 AS frontend-build
WORKDIR /app
COPY quill.client/package.json quill.client/package-lock.json ./
RUN npm install

COPY quill.client/ ./
# This runs the Vite/CRA build process
RUN npm run build

# 2nd stage: Build the ASP.NET backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /app
COPY Quill.Server/*.csproj ./Quill.Server/
RUN dotnet restore ./Quill.Server/Quill.Server.csproj

# Copy the rest of the backend files
COPY Quill.Server/ ./Quill.Server/
WORKDIR /app/Quill.Server
RUN dotnet publish -c Release -o /app/published

# Debugging step: Check the output folder
RUN ls -la /app/published

# Use ASP.NET runtime to serve the backend
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final-backend
WORKDIR /app
# Copy the .NET publish output
COPY --from=backend-build /app/published .

# Expose frontend port
EXPOSE 80
# Expose backend port
EXPOSE 5000
ENTRYPOINT ["dotnet", "Quill.Server.dll"]