variable "resource_group" {
  type = object({
    name     = string
    location = string
  })
}

variable "webapp_name" {
  type = string
}

variable "webapp_sku" {
  type = object({
    tier = string
    size = string
  })
}

variable "webapp_settings" {
  type    = map(string)
  default = null
}
