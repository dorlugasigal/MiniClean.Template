resource "azurerm_healthcare_workspace" "healthcare_workspace" {
  name                = local.health_services_ws_name
  resource_group_name = var.resource_group_name
  location            = var.location

}

resource "azurerm_healthcare_fhir_service" "fhir" {
  name                = local.fhir_service_name
  resource_group_name = var.resource_group_name
  location            = var.location
  workspace_id        = azurerm_healthcare_workspace.healthcare_workspace.id
  kind                = local.fhir_kind

  authentication {
    authority = local.authority
    audience  = local.fhir_url
  }

  identity {
    type = "SystemAssigned"
  }
}

resource "azurerm_private_endpoint" "health_services_private_endpoint" {
  name                = "pe-sample-${var.env}-health-services"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.services_subnet_id

  private_dns_zone_group {
    name                 = "private-dns-zone-group"
    private_dns_zone_ids = [var.health_zone_id]
  }
  private_service_connection {
    private_connection_resource_id = azurerm_healthcare_workspace.healthcare_workspace.id
    name                           = "psc-sample-${var.env}-health-services"
    subresource_names              = ["healthcareworkspace"]
    is_manual_connection           = false
  }
}

resource "azurerm_monitor_diagnostic_setting" "fhir_diagnostic_setting" {
  name                       = "hs_fhir_diagnostic_setting"
  target_resource_id         = azurerm_healthcare_fhir_service.fhir.id
  log_analytics_workspace_id = var.log_analytics_workspace_id

  enabled_log {
    category = "AuditLogs"
  }

  metric {
    category = "AllMetrics"
  }
}