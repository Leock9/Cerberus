resource "aws_lambda_function" "lambda" {
  depends_on         = [ aws_s3_bucket.lambda_bucket ]
  function_name      = "cerberus-lambda"
  s3_bucket          = aws_s3_bucket.lambda_bucket.bucket
  s3_key             = "lambda_cerberus.zip"
  runtime            = dotnet8
  role               = var.labRoleArn
  handler            = Cerberus.Lambda.Function.FunctionHandler
}

resource "aws_cloudwatch_log_group" "log_group" {
  name = "/aws/lambda/${aws_lambda_function.lambda.function_name}"
}

resource "aws_lambda_function_url" "lambda_url" {
  function_name      = aws_lambda_function.postech-lambda-auth-fastfood.function_name
  authorization_type = "NONE"
}