using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace dotNetSignDocExample.Services.EnvironmentService
{
  [TestFixture]
  public class EnvironmentServiceTests
  {
    private IEnvironmentService _environmentService = null!;

    [SetUp]
    public void Setup()
    {
      _environmentService = new EnvironmentService();
    }

    [Test]
    public void GetEnvironmentVariables_ReturnsValidEnvironmentVariables()
    {
      var result = _environmentService.GetEnvironmentVariables();
      Assert.IsNotNull(result);
      Assert.AreEqual("KMS_KEY_ID", result.KmsKeyId);
      Assert.AreEqual("ECDSA_SHA_256", result.KmsSigningAlgorithm);
    }
  }
}