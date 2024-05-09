resource "azurerm_virtual_network" "core_vnet" {
  name                = "vnet-miniclean-${var.env}"
  address_space       = ["192.168.0.0/16"]
  location            = var.location
  resource_group_name = var.resource_group_name
}

resource "azurerm_subnet" "services_subnet" {
  name                 = "snet-miniclean-${var.env}-services"
  resource_group_name  = var.resource_group_name
  virtual_network_name = azurerm_virtual_network.core_vnet.name
  address_prefixes     = ["192.168.1.0/24"]
}

resource "azurerm_subnet" "app_plan_subnet" {
  name                 = "snet-miniclean-${var.env}-app-plan"
  resource_group_name  = var.resource_group_name
  virtual_network_name = azurerm_virtual_network.core_vnet.name
  address_prefixes     = ["192.168.10.0/24"]


  delegation {
    name = "delegation"

    service_delegation {
      name    = "Microsoft.Web/serverFarms"
      actions = ["Microsoft.Network/virtualNetworks/subnets/action"]
    }
  }
}