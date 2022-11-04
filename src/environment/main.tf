terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 2.65"
    }
  }
}

provider "azurerm" {
  features {}
}

variable "resource_group_name" {
  default = "AzWknd22Venturus"
}

variable "resource_location" {
  default = "westus2"
}

variable "cosmosdb_account_name" {
  default = "azwknd22venturus"
}

variable "cosmosdb_database_name" {
  default = "Records"
}

variable "cosmosdb_collection_name" {
  default = "People"
}

resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.resource_location
}

# Function App

# CosmosDb
resource "azurerm_cosmosdb_account" "account" {
  name                = var.cosmosdb_account_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  offer_type          = "Standard"

  consistency_policy {
    consistency_level       = "BoundedStaleness"
    max_interval_in_seconds = 400
    max_staleness_prefix    = 200000
  }

  geo_location {
    location          = azurerm_resource_group.rg.location
    failover_priority = 0
  }
}

resource "azurerm_cosmosdb_mongo_database" "database" {
  name                = var.cosmosdb_database_name
  resource_group_name = azurerm_resource_group.rg.name
  account_name        = azurerm_cosmosdb_account.account.name
}

resource "azurerm_cosmosdb_mongo_collection" "collection" {
  name                = var.cosmosdb_collection_name
  resource_group_name = azurerm_resource_group.rg.name
  account_name        = azurerm_cosmosdb_account.account.name
  database_name       = azurerm_cosmosdb_mongo_database.database.name

  shard_key  = "uniqueKey"
  throughput = 400
  index {
    keys   = ["_id"]
    unique = true
  }
}
