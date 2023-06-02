using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace dotNetSignDocExample.Services.EnvironmentService.UnitTesting
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

    [TearDown]
    public void Cleanup()
    {
      Environment.SetEnvironmentVariable("KMS_KEY_ID", null);
      Environment.SetEnvironmentVariable("KMS_SIGNING_ALGORITHM", null);
    }

    [Test]
    [TestCase("", "", "", "ECDSA_SHA_256")]                                             // default values
    [TestCase("someTestKey", "someSignAlgorithm", "someTestKey", "someSignAlgorithm")]  // some other valid values
    public void GetEnvironmentVariables_ReturnsValidEnvironmentVariables(string testKeyIdValue, string testSignAlgorithmValue, string expectedKeyIdValue, string expectedSignAlgorithmValue)
    {
      Environment.SetEnvironmentVariable("KMS_KEY_ID", testKeyIdValue);
      Environment.SetEnvironmentVariable("KMS_SIGNING_ALGORITHM", testSignAlgorithmValue);

      var result = _environmentService.GetEnvironmentVariables();

      Assert.IsNotNull(result);
      Assert.AreEqual(expectedKeyIdValue, result.KmsKeyId);
      Assert.AreEqual(expectedSignAlgorithmValue, result.KmsSigningAlgorithm);
    }

    [Test]
    [TestCase("    ")]
    public void GetEnvironmentVariables_ReturnsError_WithBadEnvironmentVariablesValues(string value)
    {
      Environment.SetEnvironmentVariable("KMS_SIGNING_ALGORITHM", value);
      Assert.Catch<Exception>(() => _environmentService.GetEnvironmentVariables());
    }
  }
}