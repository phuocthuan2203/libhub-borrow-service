# Stage 1: Build the application
# Use the official .NET SDK image as a build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the .csproj file and restore dependencies
COPY ["LibHub.BorrowService/*.csproj", "LibHub.BorrowService/"]
RUN dotnet restore "LibHub.BorrowService/LibHub.BorrowService.csproj"

# Copy the rest of the application code
COPY LibHub.BorrowService/ LibHub.BorrowService/
# Build the application in Release mode
RUN dotnet build "LibHub.BorrowService/LibHub.BorrowService.csproj" -c Release -o /app/build

# Stage 2: Publish the application
FROM build AS publish
RUN dotnet publish "LibHub.BorrowService/LibHub.BorrowService.csproj" -c Release -o /app/publish

# Stage 3: Create the final, lightweight runtime image
# Use the official ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose the port the application will run on
EXPOSE 8080

# Define the entry point for the container
ENTRYPOINT ["dotnet", "LibHub.BorrowService.dll"]
