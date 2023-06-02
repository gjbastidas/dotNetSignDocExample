using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotNetSignDocExample.Models
{
  // This class stores environment variables values
  public class EnvironmentVariables
  {
    public string KmsKeyId { get; set; } = null!;
    public string KmsSigningAlgorithm { get; set; } = null!;
  }
}