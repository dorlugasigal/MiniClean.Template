resource "azurerm_role_assignment" "fhir_reader_role_assignment" {
  principal_id         = var.web_app_system_assigned_identity
  role_definition_name = "FHIR Data Contributor"
  scope                = azurerm_healthcare_fhir_service.fhir.id
}
