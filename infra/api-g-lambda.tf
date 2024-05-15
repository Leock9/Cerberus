resource "aws_api_gateway_resource" "auth" {
  depends_on = [data.aws_lambda_function.lambda]
  rest_api_id = api_gateway_id
  parent_id   = api_gateway_id.root_resource_id
  path_part   = "auth"
}

resource "aws_api_gateway_method" "auth_post" {
  rest_api_id   = api_gateway_id
  resource_id   = aws_api_gateway_resource.auth.id
  http_method   = "POST"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "auth_post_lambda" {
  rest_api_id             = api_gateway_id
  resource_id             = aws_api_gateway_resource.auth.id
  http_method             = aws_api_gateway_method.auth_post.http_method

  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = lambda_arn
}

resource "aws_api_gateway_deployment" "deployment" {
  depends_on = [aws_api_gateway_integration.auth_post_lambda]
  rest_api_id = api_gateway_id
  stage_name  = "stage"
  
   lifecycle {
    create_before_destroy = true
  }
}