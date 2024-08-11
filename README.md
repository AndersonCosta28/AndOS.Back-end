### Introduction

This project is an implementation of Jason Taylor's clean architecture model adapted to create a solid structure for a web operating system. It allows users to create cloud storage accounts, including Azure Blob Storage, Google Cloud Platform (GCP) Storage, and Amazon Web Services (AWS) S3, acting as virtual hard drives. The API does not directly handle binaries but generates shareable links for downloading or sending files using the cloud storage platforms' SDKs.

### Features

- **User Creation and Cloud Storage Account Linking**: Users can create their cloud storage accounts and link them to the system, enabling the use of these services as virtual hard drives.
- **Support for Multiple Virtual Hard Drives**: It is possible to link more than one cloud storage account, each functioning as an independent virtual hard drive.
- **Custom Folder Structure**: The project creates a custom folder structure to avoid integration issues with future platforms.
- **Use of SDKs to Generate Shareable Links**: The API uses the SDKs of the cloud storage platforms to generate shareable links, eliminating the need to deal directly with binaries.

### Technologies Used

- **ASP.NET 8 CORE**
- **Clean Architecture Model** by Jason Taylor
- **CQRS (Command Query Responsibility Segregation)**
- **EF Core 8** for database access
- **ASP.NET Identity** for authentication and authorization
- **PostgreSQL** as the database management system
- **.NET Aspire**

### Future Features

- **AWS S3 Implementation**
- **GCP Storage Implementation**
- **Complete CQRS Implementation**, including the 'R' side

## Running Locally

### Prerequisites

* SDK .NET 8
* Workload Aspire
* Docker

### Steps to Run the Project

1. **Clone Necessary Repositories**: Clone the following repositories and place them at the same level as the main project.
    * [AndOS.Shared](https://github.com/AndersonCosta28/AndOS.Shared)
    * [AndOS.Core](https://github.com/AndersonCosta28/AndOS.Core)
