using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotNetSignDocExample.Services.EnvironmentService
{
  public class EnvironmentService : IEnvironmentService
  {
    public EnvironmentVariables GetEnvironmentVariables()
    {
      var env = new EnvironmentVariables();
      DotNetEnv.Env.Load();

      // Declare environment variables here:
      env.KmsKeyId = getValidatedEnvironmentVariable("KMS_KEY_ID", "");
      env.KmsSigningAlgorithm = getValidatedEnvironmentVariable("KMS_SIGNING_ALGORITHM", "ECDSA_SHA_256");

      return env;
    }

    private string getValidatedEnvironmentVariable(string variableName, string defaultValue)
    {
      return DotNetEnv.Env.GetString(variableName, defaultValue);
    }
  }
}