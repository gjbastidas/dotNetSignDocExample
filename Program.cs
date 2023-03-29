using System;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;

namespace dotnetSignDocExample
{
  class Program
  {
    private const string messageType = "DIGEST";
    private const string digestDocExtension = "digest.bin";
    private const string signatureDocExtension = "sign.bin";
    private static string? KMSKeyId;
    private static string? SignAlgorithm;
    
    public static async Task Main(string[] args)
    {
      // chequea env vars
      KMSKeyId = Environment.GetEnvironmentVariable("KMS_KEY_ID");
      validateEnvVar(KMSKeyId);
      SignAlgorithm = Environment.GetEnvironmentVariable("KMS_SIGNING_ALG");
      validateEnvVar(SignAlgorithm);
      
      // configura el cliente de AWS KMS
      var client = new AmazonKeyManagementServiceClient();

      Task<string> sd = signDoc(client, "message.txt");
      // Task<string> vs = verifyDocSignature(client, "message.txt");
      
      _ = await sd;
      // Thread.Sleep(10000);
      // _ = await vs;
    }

    /// dado el nombre del documento, lo firma digitalmente con KMS
    private static async Task<string> signDoc(AmazonKeyManagementServiceClient client, string doc)
    {
      // toma el nombre del archivo sin extension. ej: de 'message.txt' devuelve 'message'
      var fName = doc.Split('.')[0];

      // asume que el documento esta localmente
      string fPath = Path.Combine(Environment.CurrentDirectory, doc);
      validateFilePath(fPath);
      FileStream fs = File.OpenRead(fPath);
      Console.WriteLine("documento: {0}, leido satisfactoriamente", fPath);

      // hash doc
      SHA256 sha256 = SHA256.Create();
      byte[] digestBin = sha256.ComputeHash(fs);
      
      // crea el archivo binario con el hash del documento
      var hashBinFile = $"{fName}-{digestDocExtension}";
      var hashBinFilePath = Path.Combine(Environment.CurrentDirectory, hashBinFile);
      using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(hashBinFilePath)))
      {
        bw.Write(digestBin);
        bw.Close();
      }
      Console.WriteLine("documento: {0} creado satisfactoriamente", hashBinFilePath);

      // sign doc
      var req = new SignRequest
      {
        KeyId = KMSKeyId,
        Message = new MemoryStream(digestBin),
        MessageType = messageType,
        SigningAlgorithm = SignAlgorithm,
      };

      try
      {
        var response = await client.SignAsync(req);
        var signature = response.Signature;
        Console.WriteLine("el documento: {0} fue satisfactoriamente firmado con la clave: {1} con el algoritmo: {2}", doc, response.KeyId, response.SigningAlgorithm);
        
        var signBinFile = $"{fName}-{signatureDocExtension}";
        var signBinFilePath = Path.Combine(Environment.CurrentDirectory, signBinFile);
        using (FileStream signBinFs = new FileStream(signBinFilePath, FileMode.Create, FileAccess.Write))
        {
          signature.WriteTo(signBinFs);
          signBinFs.Close();
        }
        Console.WriteLine("la firma: {0} fue creada satisfactoriamente", signBinFilePath);
      }
      catch (Exception)
      {
        throw;
      }

      return "Done";
    }

    /// dado el nombre del documento, verifica que la firma sea valida
    private static async Task<string> verifyDocSignature(AmazonKeyManagementServiceClient client, string doc)
    {
      // toma el nombre del archivo sin extension. ej: de 'message.txt' devuelve 'message'
      var fName = doc.Split('.')[0];

      string digestFile = $"{fName}-{digestDocExtension}";
      string digestFilePath = Path.Combine(Environment.CurrentDirectory, digestFile);
      validateFilePath(digestFilePath);
      MemoryStream digest = new MemoryStream();
      readBinFileToMemStream(digestFilePath, ref digest);

      string signatureFile = $"{fName}-{signatureDocExtension}";
      string signatureFilePath = Path.Combine(Environment.CurrentDirectory, signatureFile);
      validateFilePath(signatureFilePath);
      MemoryStream signature = new MemoryStream();
      readBinFileToMemStream(digestFilePath, ref signature);

      var req = new VerifyRequest
      {
        KeyId = KMSKeyId,
        Message = digest,
        MessageType = messageType,
        Signature = signature,
        SigningAlgorithm = SignAlgorithm,
      };
      try
      {
        var response = await client.VerifyAsync(req);
        if (response.SignatureValid) {
          Console.WriteLine("la firma: {0} para el documento {1} es valida", signatureFile, doc);
        }
        else {
          Console.WriteLine("la firma: {0} para el documento {1} es invalida", signatureFile, doc);
        } 
      }
      catch (Exception)
      {
        throw;
      }

      return "Done";
    }
    
    /// valida el file path
    private static void validateFilePath(string path)
    {
      if (!File.Exists(path)){
        Console.WriteLine("documento: {0}, no existe", path);
        Environment.Exit(1);
      }
    }

    /// verifica que la env var sea valida
    private static void validateEnvVar(string? envVar)
    {
      if (envVar is null || envVar.Length == 0){
        Console.WriteLine("variable de entorno: {0}, no valida", envVar);
        Environment.Exit(1);
      }
    }
    
    /// lee el archivo binario y almacena contenido en el memorystream adjunto
    private static void readBinFileToMemStream(string filePath, ref MemoryStream ms){
      FileStream fs = File.OpenRead(filePath);
      fs.CopyTo(ms);
    }
  }    
}
