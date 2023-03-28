```shell
dotnet new console --name dotnetSignDocument
dotnet add package AWSSDK.KeyManagementService

export AWS_PROFILE="default"
export KMS_KEY_ID="alias/test-firmar-docs"
export KMS_SIGNING_ALG="ECDSA_SHA_256"

dotnet run
```