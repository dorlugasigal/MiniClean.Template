resource "azurerm_private_dns_zone" "kv_dns_zone" {
  name                = module.terraform_azurerm_environment_configuration.private_links["privatelink.vaultcore.azure.net"]
  resource_group_name = var.resource_group_name
}

resource "azurerm_private_dns_zone_virtual_network_link" "kv_network_link" {
  name                  = "dns-link-miniclean-${var.env}-kv"
  resource_group_name   = var.resource_group_name
  private_dns_zone_name = azurerm_private_dns_zone.kv_dns_zone.name
  virtual_network_id    = azurerm_virtual_network.core_vnet.id
}

resource "azurerm_private_dns_zone" "str_dns_zone" {
  name                = module.terraform_azurerm_environment_configuration.private_links["privatelink.blob.core.windows.net"]
  resource_group_name = var.resource_group_name
}

resource "azurerm_private_dns_zone_virtual_network_link" "storage_network_link" {
  name                  = "dns-link-miniclean-${var.env}-storage"
  resource_group_name   = var.resource_group_name
  private_dns_zone_name = azurerm_private_dns_zone.str_dns_zone.name
  virtual_network_id    = azurerm_virtual_network.core_vnet.id
}

# Setup DNS zone for web app
resource "azurerm_private_dns_zone" "app_service_dns_zone" {
  name                = module.terraform_azurerm_environment_configuration.private_links["privatelink.azurewebsites.net"]
  resource_group_name = var.resource_group_name
}

resource "azurerm_private_dns_zone_virtual_network_link" "app_service_network_link" {
  name                  = "dns-link-miniclean-${var.env}-app-service"
  resource_group_name   = var.resource_group_name
  private_dns_zone_name = azurerm_private_dns_zone.app_service_dns_zone.name
  virtual_network_id    = azurerm_virtual_network.core_vnet.id
}

resource "azurerm_private_dns_zone" "acr_dns_zone" {
  name                = module.terraform_azurerm_environment_configuration.private_links["privatelink.azurecr.io"]
  resource_group_name = var.resource_group_name
}

resource "azurerm_private_dns_zone_virtual_network_link" "acrlink" {
  name                  = "dns-link-miniclean-${var.env}-acr"
  resource_group_name   = var.resource_group_name
  private_dns_zone_name = azurerm_private_dns_zone.acr_dns_zone.name
  virtual_network_id    = azurerm_virtual_network.core_vnet.id
}

resource "azurerm_private_dns_zone" "health_dns_zone" {
  name                = module.terraform_azurerm_environment_configuration.private_links["privatelink.azurehealthcareapis.com"]
  resource_group_name = var.resource_group_name
}

resource "azurerm_private_dns_zone_virtual_network_link" "health_network_link" {
  name                  = "dns-link-miniclean-${var.env}-health-services"
  resource_group_name   = var.resource_group_name
  private_dns_zone_name = azurerm_private_dns_zone.health_dns_zone.name
  virtual_network_id    = azurerm_virtual_network.core_vnet.id
}