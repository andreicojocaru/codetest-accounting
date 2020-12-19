variable "resource_group" {
  type = object({
    name     = string
    location = string
  })
}

variable "webapps" {
  type = list(object({
    name = string
  }))
}
