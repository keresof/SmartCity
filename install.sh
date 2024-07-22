#! /bin/bash

# Install the certificate
dotnet dev-certs https --trust
dotnet restore
dotnet publish -c Release -o out
cp src/SmartCity.API/.env out/.env
cd out
dotnet SmartCity.API.dll