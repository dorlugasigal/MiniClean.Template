locals {
  health_services_ws_name = "hsminiclean${var.env}"
  fhir_service_name       = "fhirminiclean${var.env}"
  fhir_url                = "https://hsminiclean${var.env}-fhirminiclean${var.env}.fhir.azurehealthcareapis.com"
  authority               = "https://login.microsoftonline.com/${var.tenant_id}"
  fhir_kind               = "fhir-R4"
}