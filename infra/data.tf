data "aws_api_gateway_rest_api" "example" {
  name = "GT-${var.projectName}"
}

output "api_gateway_id" {
  value = data.aws_api_gateway_rest_api.example.id
}

data "archive_file" "zip" {
  type        = "zip"
  source_file = "../lambda_auth.py"
  output_path = "../lambda_auth.zip"
}