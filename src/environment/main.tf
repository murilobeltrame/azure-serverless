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

variable "resource_group_name" {}

variable "resource_location" {
  default = "westus2"
}

variable "cosmosdb_name" {
  default = "mbcdatabase"
}

resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.resource_location
}

# Function App

# CosmosDb
resource "azurerm_cosmosdb_account" "account" {
  name                = "mbc_mongodb_account"
  resource_group_name = azurerm_resource_group.rg.name
}

resource "azurerm_cosmosdb_mongo_database" "database" {
  name                = "mbc_mongodb_database"
  resource_group_name = azurerm_resource_group.rg.name
  account_name        = azurerm_cosmosdb_account.account.name
}

resource "azurerm_cosmosdb_mongo_collection" "collection" {
  name                = "People"
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
