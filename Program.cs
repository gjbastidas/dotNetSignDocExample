using System;
using System.Threading.Tasks;
using System.Security.Cryptography;

using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;

namespace dotnetSignDocExample
{
  class Program
  {
    private const string messageType = "DIGEST";
    private static FileStream? fStream;
    private static MemoryStream? signature;
    public static async Task Main(string[] args)
    {
      // check env vars
      var KMSKeyId = Environment.GetEnvironmentVariable("KMS_KEY_ID");
      envVarValidate(KMSKeyId);
      var SignAlgorithm = Environment.GetEnvironmentVariable("KMS_SIGNING_ALG");
      envVarValidate(SignAlgorithm);
      
      // read doc
      string fPath = Path.Combine(Environment.CurrentDirectory, "message.txt");
      if (!File.Exists(fPath)){
        Console.WriteLine("doc doesn't exist");
        Environment.Exit(1);
      }
      fStream = File.OpenRead(fPath);
      Console.WriteLine("read doc successfully");
      
      // hash doc
      SHA256 sha256 = SHA256.Create();
      byte[] digestBin = sha256.ComputeHash(fStream);
      Console.WriteLine("hashed doc successfully");
      
      // set AWS KMS Client
      var client = new AmazonKeyManagementServiceClient();
      
      // sign doc
      try
      {
        var req = new SignRequest
        {
          KeyId = KMSKeyId,
          Message = new MemoryStream(digestBin),
          MessageType = messageType,
          SigningAlgorithm = SignAlgorithm,
        };
        var response = await client.SignAsync(req);
        string keyId = response.KeyId;
        signature = response.Signature;
        string signingAlgorithm = response.SigningAlgorithm;
        
        // write out signature file
        if (signature is null){
          throw new ArgumentNullException(nameof(signature));
        }
        using (FileStream fs = new FileStream("sign.bin", FileMode.Create, FileAccess.Write))
        {
          signature.WriteTo(fs);
          fs.Close();
        }
        Console.WriteLine("signed doc successfully");
      }
      catch (Exception e)
      {
        Console.WriteLine("{0} Exception caught", e);
        Environment.Exit(1);
      }

      // // validate signature
      // try
      // {
      //   var req = new VerifyRequest
      //   {
      //     KeyId = KMSKeyId,
      //     Message = new MemoryStream(), //TODO: read digest file
      //     MessageType = messageType,
      //     Signature = new MemoryStream(), //TODO: read from signature file
      //     SigningAlgorithm = SignAlgorithm,
      //   };
      //   var response = await client.VerifyAsync(req);
      //   Console.WriteLine("signature valid: {0}", response.SignatureValid);
      // }
      // catch (Exception e)
      // {
      //   Console.WriteLine("{0} Exception caught", e);
      //   Environment.Exit(1);
      // }
    }

    // private static void firmarDoc()
    // {

    // }
    private static void envVarValidate(string? envVar)
    {
      if (envVar is null || envVar.Length == 0){
        Console.WriteLine("invalid environment variable: {0}", envVar);
        Environment.Exit(1);
      }
    }
  }    
}
