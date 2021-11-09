![Deploy to GKE](https://github.com/kairome/reactivities-backend/workflows/google/badge.svg)


# Reactivities - Backend

Backend part of the Reactivities app. Visit [reactivities-frontend](https://github.com/kairome/reactivities-frontend)
to view frontend source code and check out the deployed version of the app.

## Description

Api project - houses controllers, middlewares and is the launch project

Application project - main business logic, contains services for db collection operations, authentication, validators

Domain project - contains dto and db collection documents definitions

Infrastructure project - contains services for signalr and photos service to work with images through cloudinary

Persistence project - contains mongodb context definition and generic get/write db operations services that all other db services inherit from

Migrator project - a separate project to run migrations on the db, needs its own connection string config

## Tech stack

Backend is written in .NET 5 with dependency injection services pattern.\
Signalr is used for websockets, i.e. notifications and real time chat

Mongodb is the database of choice, deployed to atlas servers

For images, [Cloudinary](https://cloudinary.com/) service is used


## Launch
Before launching the application, you will need to either populate the appsettings.json file or create an appsettings.Development.json file with the following values:
```json 
{
  "JWTSecretKey": "", - any string to encrypt the JWT token with
  "ConnectionStrings:MongoUri": "", - connection string for your mongodb
  "Cloudinary:ApiKey": "", - api key for cloudinary account
  "Cloudinary:CloudName": "", - cloudinary app name
  "Cloudinary:ApiSecret": "" - cloudinary acc api secret
}
```

Main launch settings are located in `launchSettings.json` if you use an IDE to start up the app.

Otherwise, do `dotnet build && cd Api/ && dotnet run`

### Production

You can test the prod version using docker by running `make build-image && make test-run`. This will create a docker image tagged `reactivities-backend` and run the container exposing backend
on port 8000