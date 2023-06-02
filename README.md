# eSign document using AWS KMS

Example of digitally signing a document using AWS KMS and AWS .NET SDK

## Load environment variables

Application variables

```shell
export KMS_KEY_ID="<replace with KMS key alias or ARN>"
export KMS_SIGNING_ALG="<replace with signing algoritm>" #example: ECDSA_SHA_256
```

You also need to export the respective AWS credentials.

## Run app

```
dotnet run
```

```
$ dotnet add package Microsoft.NET.Test.Sdk
$ dotnet add package Nunit3TestAdapter
$ dotnet add package NUnit
```
