## Basic Api Gateway using DotNet, Yarp, Redis and Keycloak

This solution is an API Gateway implemented using YARP as a Reverse proxy, Keycloak as an Identity Provider, and Redis for session storage.

### Solution structure

- ApiGateway project: a combination of a reverse proxy and OAuth2 Client. It maps incoming requests to the appropriate backend/micro-service and manage Authentication & Authorization.
- Resource Server project: as name implies, this project is a simple micros service to test  ApiGateway functionalities. It exposes endpoints protected by Keycloak.
- Infra folder: contains necessary files to setup solution infrastructure
    - docker-compose file to spin up keycloak, postgresql and redis containers
    - create-terraform-client script to create a service account for terraform in order to provision keycloak resources.
    - [main.tf](http://main.tf) to create keycloak realm, client and user.

### How to run
1. Setup infrastructure
  ```bash
  cd infra
  docker compose up -d
  chmod +x ./create-terraform-client.sh
  terraform init
  terraform appy
  ```
2. Run Api Gateway
  ```bash
  cd ../src/Gateway
  dotnet run
  ```
3. Run microservice (in another terminal)
  ```bash
  $ cd ../src/ResourceServer
  $ dotnet run
  ```
