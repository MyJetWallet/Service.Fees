using System;
using System.Threading;
using System.Threading.Tasks;
using MyNoSqlServer.DataReader;
using MyNoSqlServer.DataWriter;
using Newtonsoft.Json;
using ProtoBuf.Grpc.Client;
using Service.Fees.MyNoSql;

namespace TestApp
{
    class Program
    {

        public static string GetUrl()
        {
            // return "http://192.168.1.80:5123";
            return "192.168.1.80:5125";
        }

        public static async Task ReadAssetsFees()
        {
            // var myNoSqlClient = new MyNoSqlTcpClient( GetUrl,"local");
            // myNoSqlClient.Start();
            // Thread.Sleep(10000);
            
            // var subs = new MyNoSqlReadRepository<AssetFeesNoSqlEntity>(myNoSqlClient,
            //     AssetFeesNoSqlEntity.TableName);

            // var client = new AssetFeesClient(subs);
            // var fees = client.GetAssetFees("jetwallet", "BTC", OperationType.Withdrawal);
            //
            // Console.WriteLine(JsonConvert.SerializeObject(fees));
            //
            // var entities1 = subs.Get();
            // Console.WriteLine(JsonConvert.SerializeObject(entities1));

            
            var writer = new MyNoSqlServerDataWriter<AssetFeesNoSqlEntity>(GetUrl, AssetFeesNoSqlEntity.TableName, false);
            var entities = await writer.GetAsync();
            Console.WriteLine(JsonConvert.SerializeObject(entities));
        }

        public async static Task ReadInstrumentsFees()
        {
            //http://192.168.1.80:5123
            // var writer = new MyNoSqlServerDataWriter<SpotInstrumentFeesNoSqlEntity>(GetUrl, SpotInstrumentFeesNoSqlEntity.TableName, false);
            // var entities = await writer.GetAsync();
            // Console.WriteLine(JsonConvert.SerializeObject(entities));
            
            //192.168.1.80:5125
            var myNoSqlClient = new MyNoSqlTcpClient( GetUrl,"local");
            var subs = new MyNoSqlReadRepository<SpotInstrumentFeesNoSqlEntity>(myNoSqlClient,
                SpotInstrumentFeesNoSqlEntity.TableName);
            myNoSqlClient.Start();
            Thread.Sleep(10000);
            
            var entities = subs.Get();
            
            Console.WriteLine(JsonConvert.SerializeObject(entities));
            //
            // var entity = subs.Get(SpotInstrumentFeesNoSqlEntity.GeneratePartitionKey("jetwallet"),
            //     SpotInstrumentFeesNoSqlEntity.GenerateRowKey("ALGOUSD"));
            //
            // Console.WriteLine(JsonConvert.SerializeObject(entity));
            //
            // var entities = subs.Get(SpotInstrumentFeesNoSqlEntity.GeneratePartitionKey("jetwallet"));
            //
            // Console.WriteLine(JsonConvert.SerializeObject(entities));
            //
            // var entities1 = subs.Get();
            //
            // Console.WriteLine(JsonConvert.SerializeObject(entities1));
        }
        
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();
            
            // await ReadAssetsFees();
            await ReadInstrumentsFees();
            

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}