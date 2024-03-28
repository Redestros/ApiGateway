terraform {
  required_providers {
    keycloak = {
      source  = "mrparkers/keycloak"
      version = "4.4.0"
    }
  }
}

provider "keycloak" {
  client_id     = "terraform"
  client_secret = "884e0f95-0f42-4a63-9b1f-94274655669e"
  url           = "http://localhost:8080"
}

resource "keycloak_realm" "api-gateway-demo" {
  realm             = "api-gateway-demo"
  enabled           = true
  display_name      = "Api Gateway Demo"
  display_name_html = "<b>Api Gateway Demo</b>"
}

resource "keycloak_user" "red" {
  realm_id = keycloak_realm.api-gateway-demo.id
  username = "red"

  email      = "red@gmail.com"
  first_name = "Red"
  last_name  = "Alert"

  initial_password {
    value     = "red"
    temporary = false
  }
}

resource "keycloak_openid_client" "angular-app" {
  realm_id    = keycloak_realm.api-gateway-demo.id
  
  client_id   = "angular-app"
  client_secret = "f1a8108f-5c5f-4000-b01a-e6f7217eb041"
  name        = "angular-app"
  description = "Angular app"

  standard_flow_enabled    = true

  access_type = "CONFIDENTIAL"

  base_url = "https://localhost:7140/info"
  
  valid_redirect_uris = [
    "https://localhost:7140/*"
  ]

  valid_post_logout_redirect_uris = [
    "https://localhost:7140/*"
  ]

  web_origins = [
    "https://localhost:7140"
  ]
}
