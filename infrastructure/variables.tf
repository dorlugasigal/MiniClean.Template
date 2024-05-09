variable "env" {
  type = string
}

variable "location" {
  type = string
}

variable "runner_ip" {
  type    = string
  default = ""
}

variable "sp_client_id" {
  type = string
}

variable "shared_resource_group_name" {
  type = string
}

variable "image_tag_suffix" {
  type    = string
  default = "latest"
}