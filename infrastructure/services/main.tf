terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.74.0"
    }
  }
}

resource "random_string" "random_id" {
  length  = 4
  special = false
  upper   = false
}