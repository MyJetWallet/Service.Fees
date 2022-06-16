using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyNoSqlServer.DataReader;
using MyNoSqlServer.DataWriter;
using Newtonsoft.Json;
using ProtoBuf.Grpc.Client;
using Service.Fees.Domain.Models;
using Service.Fees.MyNoSql;

namespace TestApp
{
    class Program
    {

        public static string GetUrl()
        {
            return "http://192.168.70.80:5123"; //writer
            //return "192.168.1.80:5125";
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
        
        public static async Task FirstInitForProfiles()
        {
            // var groupWriter = new MyNoSqlServerDataWriter<FeeProfilesNoSqlEntity>(GetUrl, FeeProfilesNoSqlEntity.TableName, false);
            //
            // var profiles = new List<string>();
            // profiles.Add(FeeProfileConsts.DefaultProfile);
            // await groupWriter.InsertOrReplaceAsync(FeeProfilesNoSqlEntity.Create(profiles));
            //
            // var oldWriter = new MyNoSqlServerDataWriter<AssetFeesNoSqlEntity>(GetUrl, "myjetwallet-fees-assets", false);
            // var entities = await oldWriter.GetAsync();
            // Console.WriteLine(JsonConvert.SerializeObject(entities));
            //
            // var newWriter = new MyNoSqlServerDataWriter<AssetFeesNoSqlEntity>(GetUrl, AssetFeesNoSqlEntity.TableName, false);
            // foreach (var entity in entities.Select(t => t.AssetFees))
            // {
            //     entity.ProfileId = FeeProfileConsts.DefaultProfile;
            //     await newWriter.InsertOrReplaceAsync(AssetFeesNoSqlEntity.Create(entity));
            // }
            // var newEntities = await newWriter.GetAsync();
            // Console.WriteLine(JsonConvert.SerializeObject(newEntities));
        }

        
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();
            
            // await ReadAssetsFees();
            // await ReadInstrumentsFees();
            await FirstInitForProfiles();

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}