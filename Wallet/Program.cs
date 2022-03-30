using Grpc.Net.Client;
using GrpcServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Dynamic;

namespace Wallet
{
    internal class Program
    {
        private static List<dynamic> Blocks = new List<dynamic>();
        private static int currentEditBlock = 0;

        static async Task Main(string[] args)
        {
            // get current enviroment
            // string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // protocol http unsafe
            //AppContext.SetSwitch(
            //    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",
            //    true
            //);

            // var channel = GrpcChannel.ForAddress(GetAddressServer());

            //var input = new HelloRequest { Name = "Harold" };
            //var client = new Greeter.GreeterClient(channel);
            //var reply = await client.SayHelloAsync(input);
            //Console.WriteLine($"Rspuesta: {reply.Message}");
            //Console.ReadLine();
            string command = "";
            do
            {
                Console.WriteLine("Ingrese la transaccion: ");
                command = Console.ReadLine();

                string[] users = command.Split('-');

                if (users.Length == 2)
                {
                    string[] amount = users[1].Split(':');

                    if (users.Length == 2)
                    {
                        AddNewTransaction(command);
                        Console.WriteLine("-------------------SIGUENTE TRANSACCION-------------------");
                    } else Console.WriteLine("SEGUIR EL FORMATO A-B:40");
                } else Console.WriteLine("SEGUIR EL FORMATO A-B:40");
            } while (command != "exit");

            Console.WriteLine("Bye");
        }

        private static int GetTotalBlocks() => Blocks.Count;

        private static void CreateNewBlock()
        {
            int totalBlocks = GetTotalBlocks();

            // previousHash if the first block save value defult
            // else save last generated hash
            dynamic newBlock = new ExpandoObject();
            newBlock.id = $"{totalBlocks}";
            newBlock.nonce = 0;
            newBlock.data = "";
            newBlock.previousHash = totalBlocks == 0 ? "00000" : Blocks.LastOrDefault().hash;
            newBlock.hash = "";

            Blocks.Add(newBlock);
            currentEditBlock = totalBlocks;

            PrintAllBlocks();
        }

        private static void AddNewTransaction(string transaction)
        {
            if (Blocks.Count > 0)
            {
                string[] allTransactions = Blocks.LastOrDefault().data.Split(',');

                if (allTransactions.Count() >= 3)
                {
                    // Close the block
                    string resultHash = "";
                    do
                    {
                        Blocks[currentEditBlock].nonce = Blocks[currentEditBlock].nonce + 1;
                        string allData =
                            $"{Blocks[currentEditBlock].id}+{Blocks[currentEditBlock].nonce}+{Blocks[currentEditBlock].data}+{Blocks[currentEditBlock].previousHash}";

                        resultHash = GenerateHash(allData);
                        // hash generate 000 at the beginning of the string
                    } while (resultHash.Substring(0, 3) != "000");

                    // save hash of block
                    Blocks[currentEditBlock].hash = resultHash;
                    // Create other block
                    CreateNewBlock();
                }
            } else CreateNewBlock();

            // save the transaction
            Blocks[currentEditBlock].data =
                Blocks[currentEditBlock].data == "" ?
                    transaction :
                    $"{Blocks[currentEditBlock].data},{transaction}";
        }

        private static string GenerateHash(string words)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string word = string.Format("{0}{1}", sha256, words);
                byte[] wordAsBytes = Encoding.UTF8.GetBytes(word);
                return Convert.ToBase64String(sha256.ComputeHash(wordAsBytes));
            }
        }

        private static string getSuperHash()
        {
            string allHashes = "";

            Blocks.ForEach(block =>
            {
                allHashes = allHashes != "" ?
                    $"{allHashes}{block.hash}" :
                    block.hash;
            });

            return allHashes != "" ? GenerateHash(allHashes) : "";
        }

        private static void PrintAllBlocks()
        {
            Console.WriteLine("--------------------RESUME BLOCKS----------------");
            Blocks.ForEach(block =>
                Console.WriteLine(
                    $"id: {block.id} - nonce: {block.nonce} - data: {block.data} - previousHash: {block.previousHash} - hash: {block.hash}"
                )
            );
            Console.WriteLine("-------------------------------------------------");
        }

        private static string GetCurrentEnvironmentName() => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        private static string GetAddressServer() =>
            GetCurrentEnvironmentName() == "development" ?
                "http://blockchain:80" :
                "http://localhost:5000";
    }
}
