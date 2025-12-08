# ViKo Events
## Overview

**ViKo Events**  is a webapp built on Angular.js, and Azure Functions using .NET, that allows the user to create, edit, browse, and manage events in its instituition.
It was conceived, and built, as an academic project for Vilniaus Kolegjia, regarding, an ERASMUS+ Internship, taken, within the institution.

---

## Features and Functionalities
- Role based user authentication (Admin, Teacher, Student)
- Create, Edit, View and Delete events
- Enroll in events
- View enrolled events and historic

### Role Permissions
- **Admin**
    - Promote users
    - Create, Edit, Delete, and Erase events 
    - View event registrations
    - View user information
    - View events across all scopes
- **Teacher**
    - Create, Edit, Delete events
    - View event registrations
    - View user information 
    - View Open, Closed and Finished events that are visible
- **Student**
    - Register for events
    - Unregister from events
    - View Open events only

--- 

## Built with:
### Frontend
- **Angular.js**
- **Tailwind.CSS**

### Backend
- **.NET with Azure Functions**
- **SQL Server running on Docker**

---

# Instalation and Configuration
## Prerequisites
### Frontend
- Node.js 20.19+
- Npm 10+
- Angular CLI 20+
### Backend
- .NET SDK 8.0+
- Azure Functions Core Tools
- Azurite Jobs
- SQL Server  

## Steps

1. Clone the repository to your machine:
```
git clone https://github.com/Fulipe/projeto_viko.git
```

2. Navigate to `/viko-app`. Install Angular dependencies and run it:
```
npm install
ng serve
```
> Angular app will be running at `http://localhost:4200` 

3. Navigate to `/docker`. Create the docker containers to host the SQL Server and Azurite Jobs and run it:
```
docker-compose up -d
```

4. Set your `local.settings.json` accordingly:

> Get the Microsoft provided [Azurite connection string](https://learn.microsoft.com/en-us/azure/storage/common/storage-connect-azurite?toc=%2Fazure%2Fstorage%2Fblobs%2Ftoc.json&bc=%2Fazure%2Fstorage%2Fblobs%2Fbreadcrumb%2Ftoc.json&tabs=blob-storage#use-connection-strings) and replace it with the placeholder 
```
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "<azurite-connectionString>",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    },
```
> Replace the placeholders with the credentials set in `docker-compose.yml`

```
    "ConnectionStrings": {
        "DefaultConnection": "Server=localhost,1433;Database=<setDbName>;User Id=<setUserId>;Password=<setUserPassword>;TrustServerCertificate=True;"
    },
}
``` 

5. Navigate to `/viko-api`. Run the .NET backend:
```
dotnet restore
dotnet ef database update
dotnet run
```  
> Backend API will be running at `http://localhost:7071`

---