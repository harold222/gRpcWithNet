using Grpc.Net.Client;
using GrpcServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Dynamic;
using TrasactionServer;
using Grpc.Core;

namespace Wallet
{
    internal class Program
    {
        private static List<dynamic> Blocks = new List<dynamic>();
        private static int currentEditBlock = 0;

        static async Task Main(string[] args)
        {
            // get current enviroment
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // protocol http unsafe
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",
                true
            );

            var channel = GrpcChannel.ForAddress(GetAddressServer());

            //var input = new HelloRequest { Name = "Harold" };
            //var client = new Greeter.GreeterClient(channel);
            //var reply = await client.SayHelloAsync(input);
            //Console.WriteLine($"Rspuesta: {reply.Message}");
            //Console.ReadLine();

            var client2 = new Customer.CustomerClient(channel);
            var resp = client2.GetCustomerInfo(new CustomerLookupModel { UserId = 1 });
            // Console.WriteLine($"resp: {resp}");

            List<CustomerModel> allCustomers = new List<CustomerModel>();

            using (var call = client2.GetNewCustomers(new NewCustomerRequest()))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    CustomerModel current = call.ResponseStream.Current;
                    allCustomers.Add(current);
                    Console.WriteLine(current.FirstName);
                }
            }

            string menu = "";
            do
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine("Ingrese la opcion a consultar:");
                Console.WriteLine("1) Consultar direccion");
                Console.WriteLine("2) Registrar transaccion");
                Console.WriteLine("3) Salir");

                string option = Console.ReadLine();

                if (option == "1")
                {
                    Console.WriteLine("Ingrese la direccion: ");
                    float result = foundAmountByDirection(Console.ReadLine());

                    if (result != -1)
                    {
                        Console.WriteLine($"El saldo total de esta direccion es: {result}");
                    }
                    else
                    {
                        Console.WriteLine("No se encontro esta direccion, vuelva a intentarlo.");
                    }
                }
                else if (option == "2")
                {
                    Console.WriteLine("Ingrese la transaccion: ");
                    string command = Console.ReadLine();

                    string[] users = command.Split('-');

                    if (users.Length == 2)
                    {
                        AddNewTransaction(command);
                    }
                    else Console.WriteLine("SEGUIR EL FORMATO A-B:40");
                } else
                {
                    Console.WriteLine("Ingrese una opcion valida.");
                }
            } while (menu != "exit" || menu == "3");

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

        private static float foundAmountByDirection(string dir)
        {
            float amount = -1;

            Blocks.ForEach(block =>
            {
                string[] allTransactions = block.data.Split(',');

                foreach (string transaction in allTransactions)
                {
                    string[] directions = transaction.Split('-');
                    if (directions.Length == 2)
                    {
                        string[] values = directions[1].Split(':');

                        if (directions[0] == dir)
                        {
                            if (amount == -1) amount = 0;
                            amount = amount - float.Parse(values[1]);
                        }
                        else if (values[0] == dir)
                        {
                            if (amount == -1) amount = 0;
                            amount = float.Parse(values[1]) + amount;
                        }
                    }
                }
            });

            return amount;
        }

        private static string GetCurrentEnvironmentName() => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        private static string GetAddressServer() =>
            GetCurrentEnvironmentName() == "development" ?
                "http://blockchain:80" :
                "http://localhost:5000";
    }
}
