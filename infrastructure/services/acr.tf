resource "azurerm_container_registry" "acr" {
  name                = "acrminiclean${var.env}"
  location            = var.location
  resource_group_name = var.resource_group_name
  admin_enabled       = true
  sku                 = "Premium"

  identity {
    type = "SystemAssigned"
  }

  /*network_rule_set {
    default_action = "Deny"    
    ip_rule {
      action = "Allow"
      ip_range = var.runner_ip      
    }
  }*/
}

/*resource "azurerm_private_endpoint" "acr_privateendpoint" {
  name                = "pe-miniclean-${var.env}-acr"
  resource_group_name = var.resource_group_name
  location            = var.location
  subnet_id           = var.services_subnet_id
  
  private_dns_zone_group {
    name                 = "private-dns-zone-group"
    private_dns_zone_ids = [var.acr_zone_id]
  }

  private_service_connection {
    name                           = "psc-miniclean-${var.env}-acr"
    private_connection_resource_id = azurerm_container_registry.acr.id
    is_manual_connection           = false
    subresource_names = [
      "registry"
    ]
  }
}*/
