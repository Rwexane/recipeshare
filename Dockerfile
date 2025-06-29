# Stage 1: Build Angular
FROM node:22.17.0 AS client-build
WORKDIR /app
# Copy the entire Angular project directory into the container's /app
COPY RecipeShareClientV2 ./RecipeShareClientV2
# Change the working directory to the Angular project root inside the container
WORKDIR /app/RecipeShareClientV2
# Install Node.js dependencies for the Angular project
RUN npm install
# Install Angular CLI globally so 'ng' command is available and executable
RUN npm install -g @angular/cli
# Build the Angular application for production.
# This command will generate the compiled static files in the 'dist/RecipeShareClientV2/browser' directory
# within this build stage's container.
RUN npm run build

# Stage 2: Build .NET API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy only the .csproj file first to leverage Docker's layer caching for dotnet restore
COPY RecipeShare.API/RecipeShare.API.csproj RecipeShare.API/
# Restore NuGet packages
RUN dotnet restore "RecipeShare.API/RecipeShare.API.csproj"
# Copy the rest of the source code
COPY . .
# Change the working directory to the .NET API project root
WORKDIR /src/RecipeShare.API
# Publish the .NET application in Release configuration to /app/publish
RUN dotnet publish "RecipeShare.API.csproj" -c Release -o /app/publish

# Stage 3: Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# Copy the published .NET API application from the 'build' stage to the current directory (/app)
COPY --from=build /app/publish .
# Copy the compiled Angular static files from the 'client-build' stage.
COPY --from=client-build /app/RecipeShareClientV2/dist/RecipeShareClientV2/browser/ ./wwwroot

# Expose port 8080, which your ASP.NET Core application listens on
EXPOSE 8080
# Define the entry point command to run your .NET API when the container starts
ENTRYPOINT ["dotnet", "RecipeShare.API.dll"]