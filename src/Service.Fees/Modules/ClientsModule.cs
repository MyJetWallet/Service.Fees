using Autofac;
using MyJetWallet.Sdk.NoSql;
using MyNoSqlServer.DataReader;
using Service.AssetsDictionary.Client;

namespace Service.Fees.Modules
{
    public class ClientsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var myNoSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));

            builder.RegisterAssetsDictionaryClients(myNoSqlClient);
        }
    }
}