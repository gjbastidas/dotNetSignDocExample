using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace dotNetSignDocExample.Services.EnvironmentService
{
  public class EnvironmentService : IEnvironmentService
  {
    public EnvironmentVariables GetEnvironmentVariables()
    {
      var env = new EnvironmentVariables();
      DotNetEnv.Env.Load();

      try
      {
        // Declare environment variables here such as:
        env.KmsKeyId = getValidatedEnvironmentVariable("KMS_KEY_ID", string.Empty, (x) => true);
        env.KmsSigningAlgorithm = getValidatedEnvironmentVariable("KMS_SIGNING_ALGORITHM", "ECDSA_SHA_256", isKmsSigningAlgorithmValid);
      }
      catch (Exception)
      {
        throw;
      }

      return env;
    }

    private string getValidatedEnvironmentVariable(string variableName, string defaultValue, Func<string, bool> validationFunc)
    {
      var value = DotNetEnv.Env.GetString(variableName, defaultValue);
      if (!validationFunc(value))
      {
        throw new Exception($"environment variable {variableName} is not valid");
      }
      return value;
    }

    private bool isKmsSigningAlgorithmValid(string value)
    {
      var isValid = !String.IsNullOrWhiteSpace(value); // cannot be empty

      // introduce other validations here

      return isValid;
    }
  }
}