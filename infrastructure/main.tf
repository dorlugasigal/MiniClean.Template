terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.74.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "=3.1.0"
    }
  }

  backend "azurerm" {}
}

provider "azurerm" {
  features {}
}

module "terraform_azurerm_environment_configuration" {
  source          = "git::https://github.com/microsoft/terraform-azurerm-environment-configuration.git?ref=0.2.0"
  arm_environment = "public"
}

data "azurerm_client_config" "current" {}

data "external" "agent_ip" {
  program = ["bash", "-c", "curl -s 'https://api.ipify.org?format=json'"]
}

resource "azurerm_resource_group" "rg" {
  location = var.location
  name     = "rg-miniclean-${var.env}"
}

module "network" {
  source              = "./network"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  env                 = var.env
}

module "services" {
  source                           = "./services"
  location                         = var.location
  resource_group_name              = azurerm_resource_group.rg.name
  vnet_id                          = module.network.core_vnet_id
  app_plan_subnet_id               = module.network.app_plan_subnet_id
  services_subnet_id               = module.network.services_subnet_id
  image_tag_suffix                 = var.image_tag_suffix
  env                              = var.env
  runner_ip                        = var.runner_ip == "" ? [chomp(data.external.agent_ip.result.ip)] : [var.runner_ip]
  acr_zone_id                      = module.network.acr_dns_zone_id
  app_zone_id                      = module.network.app_service_dns_zone_id
  fhir_url                         = module.health-services.fhir_url
  current_object_id                = data.azurerm_client_config.current.object_id
  app_insights_instrumentation_key = module.services.app_insights_instrumentation_key
  vault_zone_id                    = module.network.kv_dns_zone_id
  storage_zone_id                  = module.network.storage_dns_zone_id
  tenant_id                        = data.azurerm_client_config.current.tenant_id
  sp_client_id                     = var.sp_client_id
  shared_resource_group_name       = var.shared_resource_group_name
}

module "health-services" {
  source                           = "./health-services"
  location                         = var.location
  resource_group_name              = azurerm_resource_group.rg.name
  services_subnet_id               = module.network.services_subnet_id
  env                              = var.env
  health_zone_id                   = module.network.health_dns_zone_id
  tenant_id                        = data.azurerm_client_config.current.tenant_id
  log_analytics_workspace_id       = module.services.log_analytics_workspace_id
  web_app_system_assigned_identity = module.services.web_app_system_assigned_identity
}
