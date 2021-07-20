using System;
using Autofac;
using MyJetWallet.Sdk.NoSql;
using Service.Fees.MyNoSql;

namespace Service.Fees.Modules
{
    public class MyNoSqlModule : Module
    {
        private readonly Func<string> _myNoSqlServerWriterUrl;

        public MyNoSqlModule(Func<string> myNoSqlServerWriterUrl)
        {
            _myNoSqlServerWriterUrl = myNoSqlServerWriterUrl;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMyNoSqlWriter<FeesSettingsNoSqlEntity>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), FeesSettingsNoSqlEntity.TableName, true);
            builder.RegisterMyNoSqlWriter<AssetFeesNoSqlEntity>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), AssetFeesNoSqlEntity.TableName, true);
            builder.RegisterMyNoSqlWriter<SpotInstrumentFeesNoSqlEntity>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), SpotInstrumentFeesNoSqlEntity.TableName, true);
        }
    }
}